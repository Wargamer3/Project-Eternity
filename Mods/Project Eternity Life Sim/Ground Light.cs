using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class SunShade
    {
        class Slot
        {
            public int Revision;
            public int Sun;
            public Vector3 Baked;
            public float[] Shade;
            public int Request;
            public bool Queued;
        }

        class Grid
        {
            public float OriginX;
            public float OriginZ;
            public float Spacing;
            public int SamplesX;
            public int SamplesZ;
            public float[] Heights;
        }

        class Job
        {
            public HeightField Field;
            public int Revision;
            public int Sun;
            public Vector3 Toward;
            public int Request;
            public float[] PointX;
            public float[] PointY;
            public float[] PointZ;
            public float Step;
            public bool March;
            public float DirX;
            public float DirY;
            public float Rise;
            public Grid[] Grids;
            public float[] Result;
        }

        static readonly Dictionary<HeightField, Slot> Cache = new Dictionary<HeightField, Slot>();
        static readonly ConcurrentQueue<Job> Jobs = new ConcurrentQueue<Job>();
        static readonly ConcurrentQueue<Job> Done = new ConcurrentQueue<Job>();
        static readonly AutoResetEvent Wake = new AutoResetEvent(false);
        static readonly float[] Empty = new float[0];
        static Thread Worker;
        // Either speed keeps a bake until the sun has turned about eight degrees. The fine key steps much faster than that.
        const float SunKeep = 0.99f;

        public static float[] For(MapInfo Info)
        {
            Publish();
            HeightField Field = Info == null ? null : Info.Heights;
            HeightVertex[] Vertices = Field == null ? null : Field.DrawVertices;
            if (Vertices == null || Vertices.Length == 0)
            {
                return Empty;
            }

            Vector3 Sun = SkyDraw.SunDirection;
            int Key = SunKey(Sun);
            Slot Found;
            if (!Cache.TryGetValue(Field, out Found))
            {
                Found = new Slot();
                Cache[Field] = Found;
            }

            bool Current = Found.Shade != null && Found.Shade.Length == Vertices.Length;
            bool Held = Current && Found.Revision == Field.Revision;
            if (Held && Vector3.Dot(Found.Baked, Sun) >= SunKeep)
            {
                return Found.Shade;
            }

            bool Delay = WorldClock.Rushing && Current;
            if (Delay && Found.Queued)
            {
                return Found.Shade;
            }

            Job Next = Capture(Info, Field, Vertices, Sun, Key);
            if (Next == null)
            {
                return Found.Shade ?? Empty;
            }

            if (!Delay)
            {
                if (Found.Queued)
                {
                    Found.Request++;
                }

                Next.Result = March(Next);
                Found.Shade = Next.Result;
                Found.Revision = Field.Revision;
                Found.Sun = Key;
                Found.Baked = Sun;
                Found.Queued = false;
                return Found.Shade;
            }

            Found.Request++;
            Next.Request = Found.Request;
            Next.Toward = Sun;
            Found.Queued = true;
            EnsureThread();
            Jobs.Enqueue(Next);
            Wake.Set();
            return Found.Shade;
        }

        static void Publish()
        {
            Job Finished;
            while (Done.TryDequeue(out Finished))
            {
                Slot Found;
                if (Finished == null || Finished.Field == null || Finished.Result == null || !Cache.TryGetValue(Finished.Field, out Found))
                {
                    continue;
                }

                if (Finished.Request != Found.Request)
                {
                    continue;
                }

                Found.Queued = false;
                Found.Shade = Finished.Result;
                Found.Sun = Finished.Sun;
                Found.Baked = Finished.Toward;
                Found.Revision = Finished.Revision;
            }
        }

        static void EnsureThread()
        {
            if (Worker != null)
            {
                return;
            }

            Worker = new Thread(BakeLoop);
            Worker.IsBackground = true;
            Worker.Name = "Sun shade";
            Worker.Start();
        }

        static void BakeLoop()
        {
            while (true)
            {
                Wake.WaitOne();
                Job Next;
                while (Jobs.TryDequeue(out Next))
                {
                    try
                    {
                        Next.Result = March(Next);
                    }
                    catch (Exception)
                    {
                        Next.Result = new float[Next.PointX.Length];
                        for (int i = 0; i < Next.Result.Length; i++)
                        {
                            Next.Result[i] = 1f;
                        }
                    }

                    Done.Enqueue(Next);
                }
            }
        }

        static Job Capture(MapInfo Info, HeightField Field, HeightVertex[] Vertices, Vector3 Sun, int Key)
        {
            if (Info == null || Field == null || Vertices == null || Vertices.Length == 0 || Field.Heights == null)
            {
                return null;
            }

            Vector3 Shift = ChunkOrigin.Shift(Info);
            float Flat = (float)Math.Sqrt(Sun.X * Sun.X + Sun.Z * Sun.Z);
            bool Ray = Sun.Y >= 0.08f && Flat >= 0.02f;
            float Step = Math.Max(Field.Spacing, 8f);
            Job Next = new Job();
            Next.Field = Field;
            Next.Revision = Field.Revision;
            Next.Sun = Key;
            Next.Step = Step;
            Next.March = Ray;
            if (Ray)
            {
                Next.DirX = Sun.X / Flat;
                Next.DirY = Sun.Z / Flat;
                Next.Rise = Sun.Y / Flat;
            }

            int Count = Vertices.Length;
            Next.PointX = new float[Count];
            Next.PointY = new float[Count];
            Next.PointZ = new float[Count];
            for (int i = 0; i < Count; i++)
            {
                Next.PointX[i] = Vertices[i].Position.X + Shift.X;
                Next.PointY[i] = Vertices[i].Position.Y;
                Next.PointZ[i] = Vertices[i].Position.Z + Shift.Z;
            }

            Next.Grids = CopyReach(Info, Shift, Step);
            return Next;
        }

        static Grid[] CopyReach(MapInfo Info, Vector3 Shift, float Step)
        {
            List<Grid> Grids = new List<Grid>();
            HashSet<MapInfo> Seen = new HashSet<MapInfo>();
            AddGrid(Grids, Seen, Info);
            float SpanX = Info.MapMapContainer == null ? 0f : Info.MapMapContainer.SpanX;
            float SpanZ = Info.MapMapContainer == null ? 0f : Info.MapMapContainer.SpanY;
            float Reach = 12f * Step;
            float Probe = Math.Max(Step, SpanX > 1f ? SpanX * 0.5f : Step);
            float X0 = Shift.X - Reach;
            float X1 = Shift.X + Math.Max(SpanX, Step) + Reach;
            float Z0 = Shift.Z - Reach;
            float Z1 = Shift.Z + Math.Max(SpanZ, Step) + Reach;
            for (float Z = Z0; Z <= Z1; Z += Probe)
            {
                for (float X = X0; X <= X1; X += Probe)
                {
                    AddGrid(Grids, Seen, ChunkCross.At(X, Z));
                }
            }

            return Grids.ToArray();
        }

        static void AddGrid(List<Grid> Grids, HashSet<MapInfo> Seen, MapInfo Info)
        {
            if (Info == null || Info.Heights == null || Info.Heights.Heights == null || Info.MapMapContainer == null || !Seen.Add(Info))
            {
                return;
            }

            HeightField Field = Info.Heights;
            int Count = Field.SamplesX * Field.SamplesZ;
            if (Count <= 0 || Field.Heights.Length < Count || Field.Spacing <= 0.01f)
            {
                return;
            }

            Grid Copy = new Grid();
            Copy.OriginX = Info.MapMapContainer.OffsetX;
            Copy.OriginZ = Info.MapMapContainer.OffsetY;
            Copy.Spacing = Field.Spacing;
            Copy.SamplesX = Field.SamplesX;
            Copy.SamplesZ = Field.SamplesZ;
            Copy.Heights = new float[Count];
            Array.Copy(Field.Heights, Copy.Heights, Count);
            Grids.Add(Copy);
        }

        static float[] March(Job Next)
        {
            int Count = Next.PointX.Length;
            float[] Shade = new float[Count];
            for (int i = 0; i < Count; i++)
            {
                float Value = 1f;
                if (Next.March)
                {
                    float Height = Next.PointY[i];
                    float X = Next.PointX[i];
                    float Z = Next.PointZ[i];
                    for (int s = 1; s <= 12; s++)
                    {
                        float Travel = s * Next.Step;
                        float Ground;
                        if (!GroundAt(Next.Grids, X + Next.DirX * Travel, Z + Next.DirY * Travel, out Ground))
                        {
                            break;
                        }

                        if (Ground > Height + 0.35f + Next.Rise * Travel)
                        {
                            Value = 0.42f;
                            break;
                        }
                    }
                }

                Shade[i] = Value;
            }

            return Shade;
        }

        static int SunKey(Vector3 Sun)
        {
            int X = (int)Math.Round(Sun.X * 50f);
            int Y = (int)Math.Round(Sun.Y * 50f);
            int Z = (int)Math.Round(Sun.Z * 50f);
            return X + Y * 128 + Z * 16384;
        }

        static bool GroundAt(Grid[] Grids, float X, float Z, out float Height)
        {
            Height = 0f;
            if (Grids == null)
            {
                return false;
            }

            for (int i = 0; i < Grids.Length; i++)
            {
                if (Sample(Grids[i], X, Z, out Height))
                {
                    return true;
                }
            }

            return false;
        }

        static bool Sample(Grid Grid, float WorldX, float WorldZ, out float Height)
        {
            Height = 0f;
            if (Grid == null || Grid.Heights == null || Grid.Spacing <= 0.01f)
            {
                return false;
            }

            float GX = (WorldX - Grid.OriginX) / Grid.Spacing;
            float GZ = (WorldZ - Grid.OriginZ) / Grid.Spacing;
            int X0 = (int)Math.Floor(GX);
            int Z0 = (int)Math.Floor(GZ);
            if (X0 < 0 || Z0 < 0 || X0 >= Grid.SamplesX || Z0 >= Grid.SamplesZ)
            {
                return false;
            }

            int X1 = X0 + 1;
            int Z1 = Z0 + 1;
            if (X1 >= Grid.SamplesX || Z1 >= Grid.SamplesZ)
            {
                Height = Grid.Heights[Z0 * Grid.SamplesX + X0];
                return true;
            }

            float TX = GX - X0;
            float TZ = GZ - Z0;
            float H00 = Grid.Heights[Z0 * Grid.SamplesX + X0];
            float H10 = Grid.Heights[Z0 * Grid.SamplesX + X1];
            float H01 = Grid.Heights[Z1 * Grid.SamplesX + X0];
            float H11 = Grid.Heights[Z1 * Grid.SamplesX + X1];
            float Along0 = H00 + (H10 - H00) * TX;
            float Along1 = H01 + (H11 - H01) * TX;
            Height = Along0 + (Along1 - Along0) * TZ;
            return true;
        }
    }

    public static class LampGlow
    {
        public const int Tile = 16;
        public const int MaxLights = 64;
        public const int PerTile = 4;

        public static readonly Vector4[] Lights = new Vector4[MaxLights];
        public static int Count;
        public static Vector2 Grid = new Vector2(1f, 1f);
        public static Vector2 Origin;
        public static Texture2D Tiles;
        public static readonly Vector3 Color = new Vector3(1f, 0.78f, 0.42f);

        static readonly float[] Best = new float[MaxLights];
        static readonly float[] SpanX = new float[MaxLights];
        static readonly float[] SpanY = new float[MaxLights];
        static readonly float[] SpanRadius = new float[MaxLights];
        static byte[] Slots = new byte[PerTile];
        static float[] Far = new float[PerTile];
        static Color[] Pixels = new Color[1];
        const float Reach = 14f;

        public static void Gather(IList<MapInfo> Maps, Vector3 Eye, Matrix View, Matrix Projection, GraphicsDevice Device)
        {
            Count = 0;
            if (!PictureOptions.Lamps || Device == null)
            {
                return;
            }

            Viewport Pane = Device.Viewport;
            int TilesX = Math.Max(1, (Pane.Width + Tile - 1) / Tile);
            int TilesY = Math.Max(1, (Pane.Height + Tile - 1) / Tile);
            int Cells = TilesX * TilesY;
            if (Slots.Length != Cells * PerTile)
            {
                Slots = new byte[Cells * PerTile];
                Far = new float[Cells * PerTile];
                Pixels = new Color[Cells];
            }
            else
            {
                Array.Clear(Slots, 0, Slots.Length);
            }

            Origin = new Vector2(Pane.X, Pane.Y);
            Grid = new Vector2(TilesX, TilesY);
            if (Maps != null && Pane.Width > 0 && Pane.Height > 0)
            {
                Matrix ViewProjection = View * Projection;
                for (int m = 0; m < Maps.Count; m++)
                {
                    MapInfo Info = Maps[m];
                    if (Info == null || Info.Furniture == null)
                    {
                        continue;
                    }

                    Vector3 Shift = ChunkOrigin.Shift(Info);
                    for (int i = 0; i < Info.Furniture.Count; i++)
                    {
                        FurnitureItem Item = Info.Furniture[i];
                        if (Item == null || Item.Interaction != "Lamp")
                        {
                            continue;
                        }

                        float Head = Math.Max(Item.Height, 1.4f) * 0.75f;
                        Vector3 At = new Vector3(Item.Position.X + Shift.X, Item.Position.Z + Head, Item.Position.Y + Shift.Z);
                        float ScreenX;
                        float ScreenY;
                        float PixelRadius;
                        float Distance;
                        if (!Covers(At, Eye, View, ViewProjection, Projection, Pane, out ScreenX, out ScreenY, out PixelRadius, out Distance))
                        {
                            continue;
                        }

                        int Index = Keep(At, Distance);
                        if (Index < 0)
                        {
                            continue;
                        }

                        SpanX[Index] = ScreenX;
                        SpanY[Index] = ScreenY;
                        SpanRadius[Index] = PixelRadius;
                    }
                }
            }

            for (int i = 0; i < Count; i++)
            {
                Stamp(i, Best[i], SpanX[i], SpanY[i], SpanRadius[i], Pane, TilesX, TilesY);
            }

            for (int i = 0; i < Cells; i++)
            {
                int Base = i * PerTile;
                Pixels[i] = new Color(Slots[Base], Slots[Base + 1], Slots[Base + 2], Slots[Base + 3]);
            }

            if (Device == null)
            {
                return;
            }

            if (Tiles == null || Tiles.IsDisposed || Tiles.Width != TilesX || Tiles.Height != TilesY)
            {
                if (Tiles != null && !Tiles.IsDisposed)
                {
                    Tiles.Dispose();
                }

                Tiles = new Texture2D(Device, TilesX, TilesY, false, SurfaceFormat.Color);
            }

            Tiles.SetData(Pixels);
        }

        public static void Apply(Effect Shade)
        {
            if (Shade == null)
            {
                return;
            }

            EffectParameter Packed = Shade.Parameters["Lamps"];
            if (Packed != null)
            {
                Packed.SetValue(Lights);
            }

            EffectParameter Used = Shade.Parameters["LampCount"];
            if (Used != null)
            {
                Used.SetValue(Count);
            }

            EffectParameter Size = Shade.Parameters["LampGrid"];
            if (Size != null)
            {
                Size.SetValue(Grid);
            }

            EffectParameter Corner = Shade.Parameters["LampOrigin"];
            if (Corner != null)
            {
                Corner.SetValue(Origin);
            }

            EffectParameter Map = Shade.Parameters["LampTiles"];
            if (Map != null && Tiles != null)
            {
                Map.SetValue(Tiles);
            }

            EffectParameter Tint = Shade.Parameters["LampColor"];
            if (Tint != null)
            {
                Tint.SetValue(Color);
            }
        }

        static bool Covers(Vector3 At, Vector3 Eye, Matrix View, Matrix ViewProjection, Matrix Projection, Viewport Pane, out float ScreenX, out float ScreenY, out float PixelRadius, out float Distance)
        {
            ScreenX = 0f;
            ScreenY = 0f;
            PixelRadius = 0f;
            Distance = Vector3.DistanceSquared(At, Eye);
            Vector3 ViewPoint = Vector3.Transform(At, View);
            float Depth = -ViewPoint.Z;
            if (Depth + Reach < 0.2f)
            {
                return false;
            }

            if (Distance < Reach * Reach)
            {
                ScreenX = Pane.X + Pane.Width * 0.5f;
                ScreenY = Pane.Y + Pane.Height * 0.5f;
                PixelRadius = Math.Max(Pane.Width, Pane.Height);
                return true;
            }

            Vector4 Clip = Vector4.Transform(new Vector4(At, 1f), ViewProjection);
            if (Clip.W < 0.05f)
            {
                return false;
            }

            float NdcX = Clip.X / Clip.W;
            float NdcY = Clip.Y / Clip.W;
            ScreenX = Pane.X + (NdcX * 0.5f + 0.5f) * Pane.Width;
            ScreenY = Pane.Y + (1f - (NdcY * 0.5f + 0.5f)) * Pane.Height;
            float Scale = Pane.Height * 0.5f * Math.Abs(Projection.M22);
            PixelRadius = Depth < 0.25f ? Math.Max(Pane.Width, Pane.Height) : Reach / Depth * Scale;
            float Left = Pane.X;
            float Top = Pane.Y;
            float Right = Pane.X + Pane.Width;
            float Bottom = Pane.Y + Pane.Height;
            return ScreenX + PixelRadius >= Left && ScreenX - PixelRadius <= Right && ScreenY + PixelRadius >= Top && ScreenY - PixelRadius <= Bottom;
        }

        static int Keep(Vector3 At, float Distance)
        {
            Vector4 Packed = new Vector4(At, Reach);
            if (Count < Lights.Length)
            {
                Lights[Count] = Packed;
                Best[Count] = Distance;
                Count++;
                return Count - 1;
            }

            int Worst = 0;
            for (int i = 1; i < Lights.Length; i++)
            {
                if (Best[i] > Best[Worst])
                {
                    Worst = i;
                }
            }

            if (Distance >= Best[Worst])
            {
                return -1;
            }

            Lights[Worst] = Packed;
            Best[Worst] = Distance;
            return Worst;
        }

        static void Stamp(int LightIndex, float Distance, float ScreenX, float ScreenY, float PixelRadius, Viewport Pane, int TilesX, int TilesY)
        {
            int X0 = (int)Math.Floor((ScreenX - PixelRadius - Pane.X) / Tile);
            int Y0 = (int)Math.Floor((ScreenY - PixelRadius - Pane.Y) / Tile);
            int X1 = (int)Math.Floor((ScreenX + PixelRadius - Pane.X) / Tile);
            int Y1 = (int)Math.Floor((ScreenY + PixelRadius - Pane.Y) / Tile);
            if (X0 < 0)
            {
                X0 = 0;
            }

            if (Y0 < 0)
            {
                Y0 = 0;
            }

            if (X1 >= TilesX)
            {
                X1 = TilesX - 1;
            }

            if (Y1 >= TilesY)
            {
                Y1 = TilesY - 1;
            }

            byte Mark = (byte)(LightIndex + 1);
            for (int Y = Y0; Y <= Y1; Y++)
            {
                int Row = Y * TilesX;
                for (int X = X0; X <= X1; X++)
                {
                    int Base = (Row + X) * PerTile;
                    int Slot = -1;
                    for (int s = 0; s < PerTile; s++)
                    {
                        if (Slots[Base + s] == 0)
                        {
                            Slot = s;
                            break;
                        }
                    }

                    if (Slot < 0)
                    {
                        int Worst = 0;
                        for (int s = 1; s < PerTile; s++)
                        {
                            if (Far[Base + s] > Far[Base + Worst])
                            {
                                Worst = s;
                            }
                        }

                        if (Distance >= Far[Base + Worst])
                        {
                            continue;
                        }

                        Slot = Worst;
                    }

                    Slots[Base + Slot] = Mark;
                    Far[Base + Slot] = Distance;
                }
            }
        }
    }

    // How much sky each terrain vertex can see, from the height of the ground around it.
    // Flat ground stays open. A ditch or a bank closes it. Cached until the heights change.
    public static class GroundOpen
    {
        class Slot
        {
            public int Stamp;
            public float[] Open;
        }

        static readonly Dictionary<HeightField, Slot> Cache = new Dictionary<HeightField, Slot>();
        const int Steps = 3;
        const float Floor = 0.45f;

        public static float[] For(MapInfo Info)
        {
            HeightField Field = Info == null ? null : Info.Heights;
            HeightVertex[] Vertices = Field == null ? null : Field.DrawVertices;
            if (Vertices == null || Vertices.Length == 0 || Field.Spacing <= 0.01f)
            {
                return Empty;
            }

            int Key = StampOf(Info);
            Slot Found;
            if (!Cache.TryGetValue(Field, out Found))
            {
                Found = new Slot();
                Cache[Field] = Found;
            }

            if (Found.Open != null && Found.Open.Length == Vertices.Length && Found.Stamp == Key)
            {
                return Found.Open;
            }

            float[] Open = new float[Vertices.Length];
            Vector3 Shift = ChunkOrigin.Shift(Info);
            float Spacing = Field.Spacing;
            for (int i = 0; i < Vertices.Length; i++)
            {
                float WorldX = Vertices[i].Position.X + Shift.X;
                float WorldZ = Vertices[i].Position.Z + Shift.Z;
                float Height = Vertices[i].Position.Y + Shift.Y;
                Open[i] = Measure(WorldX, WorldZ, Height, Spacing);
            }

            Found.Open = Open;
            Found.Stamp = Key;
            return Open;
        }

        static readonly float[] Empty = new float[0];

        static float Measure(float WorldX, float WorldZ, float Height, float Spacing)
        {
            float Blocked = 0f;
            for (int Direction = 0; Direction < 8; Direction++)
            {
                double Angle = Direction * Math.PI / 4.0;
                float Dx = (float)Math.Cos(Angle);
                float Dz = (float)Math.Sin(Angle);
                float Horizon = 0f;
                for (int Step = 1; Step <= Steps; Step++)
                {
                    float Travel = Step * Spacing;
                    float Ground;
                    if (!HeightAt(WorldX + Dx * Travel, WorldZ + Dz * Travel, out Ground))
                    {
                        break;
                    }

                    float Slope = (Ground - Height - 0.6f) / Travel;
                    if (Slope > Horizon)
                    {
                        Horizon = Slope;
                    }
                }

                if (Horizon > 0f)
                {
                    Blocked += Horizon > 0.74f ? 1f : Horizon * 1.35f;
                }
            }

            float Cover = Blocked / 8f;
            float Open = 1f - Cover * 1.35f;
            if (Open < 0f)
            {
                Open = 0f;
            }

            return Floor + (1f - Floor) * Open;
        }

        static int StampOf(MapInfo Info)
        {
            int Key = 17;
            if (Info == null || Info.Heights == null || Info.MapMapContainer == null)
            {
                return Key;
            }

            Key = Key * 31 + Info.Heights.Revision;
            float X = Info.MapMapContainer.OffsetX;
            float Y = Info.MapMapContainer.OffsetY;
            float SpanX = Info.MapMapContainer.SpanX;
            float SpanY = Info.MapMapContainer.SpanY;
            Key = Key * 31 + RevisionAt(X - 8f, Y + SpanY * 0.5f);
            Key = Key * 31 + RevisionAt(X + SpanX + 8f, Y + SpanY * 0.5f);
            Key = Key * 31 + RevisionAt(X + SpanX * 0.5f, Y - 8f);
            Key = Key * 31 + RevisionAt(X + SpanX * 0.5f, Y + SpanY + 8f);
            return Key;
        }

        static int RevisionAt(float WorldX, float WorldY)
        {
            MapInfo Info = ChunkCross.At(WorldX, WorldY);
            if (Info == null || Info.Heights == null)
            {
                return 0;
            }

            return Info.Heights.Revision + 1;
        }

        static bool HeightAt(float X, float Z, out float Height)
        {
            Height = 0f;
            MapInfo Info = ChunkCross.At(X, Z);
            if (Info == null || Info.Heights == null || Info.MapMapContainer == null)
            {
                return false;
            }

            float SlopeX;
            float SlopeY;
            return Info.Heights.Sample(X - Info.MapMapContainer.OffsetX, Z - Info.MapMapContainer.OffsetY, out Height, out SlopeX, out SlopeY);
        }
    }
}
