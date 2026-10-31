using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class Ocean
    {
        public const float Level = 8f;
        const int Cells = 128;
        const float Step = 8f;
        const int FarCells = 160;
        const float FarStep = 40f;

        const int RiverSlots = 24;
        const int LakeSlots = 48;
        const int BlockSlots = 32;

        static Effect Effect;
        static VertexBuffer Vertices;
        static IndexBuffer Indices;
        static VertexBuffer FarVertices;
        static IndexBuffer FarIndices;
        static bool Failed;
        static readonly Vector4[] RiverStart = new Vector4[RiverSlots];
        static readonly Vector4[] RiverEnd = new Vector4[RiverSlots];
        static readonly Vector4[] LakeDisc = new Vector4[LakeSlots];
        static readonly Vector4[] BlockRect = new Vector4[BlockSlots];
        static readonly int[] Live = new int[Cells * Cells * 6];
        static readonly int[] FarLive = new int[FarCells * FarCells * 6];
        static readonly int[] NearSeen = new int[Cells * Cells];
        static readonly int[] FarSeen = new int[FarCells * FarCells];
        static int SeenToken;
        static int BlockCount;

        public static bool Covers(float TerrainHeight)
        {
            return TerrainHeight < Level - 0.2f;
        }

        public static bool Covers(HeightField Ground, float LocalX, float LocalY)
        {
            if (Ground == null)
            {
                return false;
            }

            float Height;
            float SlopeX;
            float SlopeY;
            return Ground.Sample(LocalX, LocalY, out Height, out SlopeX, out SlopeY) && Covers(Height);
        }

        public static float HeightAt(float WorldX, float WorldZ)
        {
            return OceanField.HeightAt(WorldX, WorldZ);
        }

        public static WaterShift Drift(Vector3 Feet, Vector3 Shift, HeightField Ground, float Dt)
        {
            WaterShift Moved = new WaterShift();
            Moved.Position = Feet;
            if (!Covers(Ground, Feet.X, Feet.Y))
            {
                return Moved;
            }

            float Surface = HeightAt(Feet.X + Shift.X, Feet.Y + Shift.Z);
            float Terrain;
            float SlopeX;
            float SlopeY;
            Ground.Sample(Feet.X, Feet.Y, out Terrain, out SlopeX, out SlopeY);
            float Draft = WaterPlace.CharacterDensity * WaterPlace.SwimBody;
            float Target = Surface - Draft;
            if (Target < Terrain)
            {
                Target = Terrain;
            }

            Vector2 Flow = Climate.Now.Wind * 0.12f;
            Moved.Position.X += Flow.X * Dt;
            Moved.Position.Y += Flow.Y * Dt;
            Moved.Position.Z = Target;
            Moved.InWater = true;
            Moved.Surface = Surface;
            Moved.Submerged = WaterPlace.CharacterDensity;
            return Moved;
        }

        // The wave textures are render targets. Filling them while the scene target is bound
        // discards the land and sky already drawn into it, then the water draws on top.
        public static void Prepare(GraphicsDevice Device)
        {
            if (Device == null)
            {
                return;
            }

            OceanField.Advance(Climate.Now.Wind, WaterDraw.Time);
            OceanField.Upload(Device);
        }

        public static void Draw(GraphicsDevice Device, Matrix View, Matrix Projection, IList<MapInfo> Terrain)
        {
            if (Device == null)
            {
                return;
            }

            Ensure(Device);
            if (Failed || Effect == null || Indices == null)
            {
                return;
            }

            int RiverCount;
            int LakeCount;
            Collect(Terrain, out RiverCount, out LakeCount);
            Vector3 Eye = Matrix.Invert(View).Translation;
            float OriginX = (float)Math.Floor(Eye.X / Step) * Step;
            float OriginZ = (float)Math.Floor(Eye.Z / Step) * Step;
            int Drawn = Select(Terrain, OriginX, OriginZ, Cells, Step, Live, 0f);
            int FarDrawn = Select(Terrain, OriginX, OriginZ, FarCells, FarStep, FarLive, Cells * Step * 0.5f);
            if (Drawn < 3 && FarDrawn < 3)
            {
                return;
            }

            if (OceanField.Far == null)
            {
                return;
            }

            Effect.Parameters["WorldViewProj"].SetValue(View * Projection);
            Effect.Parameters["Origin"].SetValue(new Vector3(OriginX, Level, OriginZ));
            Effect.Parameters["Eye"].SetValue(Eye);
            Effect.Parameters["Sizes"].SetValue(OceanField.Sizes);
            Effect.Parameters["Disp0"].SetValue(OceanField.Far);
            Effect.Parameters["Disp1"].SetValue(OceanField.Mid);
            Effect.Parameters["Disp2"].SetValue(OceanField.Near);
            Effect.Parameters["SunDir"].SetValue(SkyDraw.SunDirection);
            Effect.Parameters["SunColor"].SetValue(SkyDraw.KeyColor);
            Effect.Parameters["FogColor"].SetValue(SkyDraw.FogColor);
            Device.BlendState = BlendState.Opaque;
            Device.DepthStencilState = DepthStencilState.Default;
            Device.RasterizerState = RasterizerState.CullNone;
            if (FarDrawn >= 3 && FarVertices != null)
            {
                FarIndices.SetData(FarLive, 0, FarDrawn);
                Effect.Parameters["DepthBias"].SetValue(0.00002f);
                Device.SetVertexBuffer(FarVertices);
                Device.Indices = FarIndices;
                foreach (EffectPass Pass in Effect.CurrentTechnique.Passes)
                {
                    Pass.Apply();
                    Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, FarDrawn / 3);
                }
            }

            if (Drawn >= 3)
            {
                Indices.SetData(Live, 0, Drawn);
                Effect.Parameters["DepthBias"].SetValue(0f);
                Device.SetVertexBuffer(Vertices);
                Device.Indices = Indices;
                foreach (EffectPass Pass in Effect.CurrentTechnique.Passes)
                {
                    Pass.Apply();
                    Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, Drawn / 3);
                }
            }
        }

        static void Collect(IList<MapInfo> Terrain, out int RiverCount, out int LakeCount)
        {
            RiverCount = 0;
            LakeCount = 0;
            BlockCount = 0;
            if (Terrain == null)
            {
                return;
            }

            for (int m = 0; m < Terrain.Count; m++)
            {
                MapInfo Info = Terrain[m];
                if (Info == null || Info.Water == null)
                {
                    continue;
                }

                Vector3 Shift = ChunkOrigin.Shift(Info);
                if (Info.Water.Count > 0)
                {
                    HoldChunk(Info, Shift);
                }

                for (int v = 0; v < Info.Water.Count; v++)
                {
                    WaterVolume Volume = Info.Water[v];
                    if (Volume == null)
                    {
                        continue;
                    }

                    if (Volume.Kind == WaterKind.River)
                    {
                        if (RiverCount >= RiverSlots)
                        {
                            continue;
                        }

                        float Half = Volume.Width * 0.5f;
                        RiverStart[RiverCount] = new Vector4(Volume.Start.X + Shift.X, Volume.Start.Y + Shift.Z, Half, 0f);
                        RiverEnd[RiverCount] = new Vector4(Volume.End.X + Shift.X, Volume.End.Y + Shift.Z, 0f, 0f);
                        RiverCount++;
                        continue;
                    }

                    if (Volume.LakeCells != null && Volume.Cell > 0.01f)
                    {
                        float Reach = Volume.Cell * 0.62f;
                        for (int c = 0; c < Volume.LakeCells.Length && LakeCount < LakeSlots; c++)
                        {
                            int Packed = Volume.LakeCells[c];
                            int CellX = Packed & 65535;
                            int CellY = Packed >> 16;
                            float CenterX = (CellX + 0.5f) * Volume.Cell + Shift.X;
                            float CenterZ = (CellY + 0.5f) * Volume.Cell + Shift.Z;
                            LakeDisc[LakeCount] = new Vector4(CenterX, CenterZ, Reach, 0f);
                            LakeCount++;
                        }

                        continue;
                    }

                    if (LakeCount >= LakeSlots)
                    {
                        continue;
                    }

                    LakeDisc[LakeCount] = new Vector4(Volume.Start.X + Shift.X, Volume.Start.Y + Shift.Z, Volume.Radius, 0f);
                    LakeCount++;
                }
            }
        }

        static int Select(IList<MapInfo> Terrain, float OriginX, float OriginZ, int Grid, float Cell, int[] Into, float Hole)
        {
            if (Terrain == null || Terrain.Count == 0)
            {
                return 0;
            }

            int[] Seen = Grid == Cells ? NearSeen : FarSeen;
            SeenToken++;
            if (SeenToken == int.MaxValue)
            {
                Array.Clear(NearSeen, 0, NearSeen.Length);
                Array.Clear(FarSeen, 0, FarSeen.Length);
                SeenToken = 1;
            }

            float Half = Grid * Cell * 0.5f;
            float MinX = OriginX - Half;
            float MinZ = OriginZ - Half;
            int Row = Grid + 1;
            int At = 0;
            float Shore = Level + 0.4f;
            for (int m = 0; m < Terrain.Count; m++)
            {
                MapInfo Info = Terrain[m];
                if (Info == null || Info.Heights == null || !Dips(Info.Heights, Shore))
                {
                    continue;
                }

                Vector3 Shift = ChunkOrigin.Shift(Info);
                float SpanX = (Info.Heights.SamplesX - 1) * Info.Heights.Spacing;
                float SpanZ = (Info.Heights.SamplesZ - 1) * Info.Heights.Spacing;
                int XStart = (int)Math.Floor((Shift.X - MinX) / Cell);
                int ZStart = (int)Math.Floor((Shift.Z - MinZ) / Cell);
                int XEnd = (int)Math.Ceiling((Shift.X + SpanX - MinX) / Cell);
                int ZEnd = (int)Math.Ceiling((Shift.Z + SpanZ - MinZ) / Cell);
                if (XStart < 0)
                {
                    XStart = 0;
                }

                if (ZStart < 0)
                {
                    ZStart = 0;
                }

                if (XEnd > Grid)
                {
                    XEnd = Grid;
                }

                if (ZEnd > Grid)
                {
                    ZEnd = Grid;
                }

                for (int Z = ZStart; Z < ZEnd; Z++)
                {
                    float Z0 = MinZ + Z * Cell;
                    float Z1 = Z0 + Cell;
                    int RowSeen = Z * Grid;
                    for (int X = XStart; X < XEnd; X++)
                    {
                        int Mark = RowSeen + X;
                        if (Seen[Mark] == SeenToken)
                        {
                            continue;
                        }

                        float X0 = MinX + X * Cell;
                        float X1 = X0 + Cell;
                        if (Hole > 0f && X0 >= OriginX - Hole && X1 <= OriginX + Hole && Z0 >= OriginZ - Hole && Z1 <= OriginZ + Hole)
                        {
                            Seen[Mark] = SeenToken;
                            continue;
                        }

                        if (Blocked((X0 + X1) * 0.5f, (Z0 + Z1) * 0.5f)
                            || (!Wet(Info.Heights, Shift, X0, Z0, Shore)
                                && !Wet(Info.Heights, Shift, X1, Z0, Shore)
                                && !Wet(Info.Heights, Shift, X0, Z1, Shore)
                                && !Wet(Info.Heights, Shift, X1, Z1, Shore)))
                        {
                            continue;
                        }

                        Seen[Mark] = SeenToken;
                        int A = Z * Row + X;
                        Into[At++] = A;
                        Into[At++] = A + Row;
                        Into[At++] = A + 1;
                        Into[At++] = A + 1;
                        Into[At++] = A + Row;
                        Into[At++] = A + Row + 1;
                    }
                }
            }

            return At;
        }

        static bool Dips(HeightField Field, float Shore)
        {
            float[] Heights = Field.Heights;
            if (Heights == null)
            {
                return false;
            }

            for (int i = 0; i < Heights.Length; i++)
            {
                if (Heights[i] < Shore)
                {
                    return true;
                }
            }

            return false;
        }

        static bool Wet(HeightField Field, Vector3 Shift, float WorldX, float WorldZ, float Shore)
        {
            float Height;
            float SlopeX;
            float SlopeY;
            return Field.Sample(WorldX - Shift.X, WorldZ - Shift.Z, out Height, out SlopeX, out SlopeY) && Height < Shore;
        }

        static bool Blocked(float X, float Z)
        {
            for (int i = 0; i < BlockCount; i++)
            {
                Vector4 Rect = BlockRect[i];
                if (X >= Rect.X && X <= Rect.Z && Z >= Rect.Y && Z <= Rect.W)
                {
                    return true;
                }
            }

            return false;
        }

        static void HoldChunk(MapInfo Info, Vector3 Shift)
        {
            if (BlockCount >= BlockSlots)
            {
                return;
            }

            float Span = 256f;
            if (Info.MapMapContainer != null && Info.MapMapContainer.SpanX > 1f)
            {
                Span = Info.MapMapContainer.SpanX;
            }

            BlockRect[BlockCount] = new Vector4(Shift.X - Span, Shift.Z - Span, Shift.X + Span * 2f, Shift.Z + Span * 2f);
            BlockCount++;
        }

        static void Ensure(GraphicsDevice Device)
        {
            if (Effect != null || Failed)
            {
                return;
            }

            try
            {
                string PathName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Life Sim", "Shaders", "Ocean.mgfx");
                if (!File.Exists(PathName))
                {
                    Failed = true;
                    return;
                }

                Effect = new Effect(Device, File.ReadAllBytes(PathName));
                BuildGrid(Device, Cells, Step, out Vertices, out Indices);
                BuildGrid(Device, FarCells, FarStep, out FarVertices, out FarIndices);
            }
            catch (Exception)
            {
                Failed = true;
            }
        }

        static void BuildGrid(GraphicsDevice Device, int Grid, float Cell, out VertexBuffer Buffer, out IndexBuffer Index)
        {
            int Count = (Grid + 1) * (Grid + 1);
            VertexPosition[] Points = new VertexPosition[Count];
            float Half = Grid * Cell * 0.5f;
            int Write = 0;
            for (int Z = 0; Z <= Grid; Z++)
            {
                for (int X = 0; X <= Grid; X++)
                {
                    Points[Write++] = new VertexPosition(new Vector3(X * Cell - Half, 0f, Z * Cell - Half));
                }
            }

            Buffer = new VertexBuffer(Device, VertexPosition.VertexDeclaration, Points.Length, BufferUsage.WriteOnly);
            Buffer.SetData(Points);
            Index = new IndexBuffer(Device, IndexElementSize.ThirtyTwoBits, Grid * Grid * 6, BufferUsage.WriteOnly);
        }
    }
}
