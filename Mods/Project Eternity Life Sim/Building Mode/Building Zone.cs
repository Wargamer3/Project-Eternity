using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ZoneFootprint
    {
        public MapInfo Chunk;
        public Vector3 Min;
        public Vector3 Max;
        public NavSourceGeometry Interior;
    }

    public class ZoneDraft
    {
        public bool HasCorner;
        public Vector3 Corner;
        public bool ShowRectangle;
        public Vector3 Opposite;
        public bool ShowEntry;
        public Vector3 Entry;
    }

    public class BuildingZone
    {
        public string Name;
        public bool Public;
        public int Price;
        public string OwnerId;
        public readonly List<ZoneFootprint> Footprints = new List<ZoneFootprint>();
        public MapInfo EntryChunk;
        public Vector3 Entry;

        public bool Covers(MapInfo Chunk)
        {
            for (int i = 0; i < Footprints.Count; i++)
            {
                if (Footprints[i].Chunk == Chunk)
                {
                    return true;
                }
            }

            return false;
        }

        public bool Contains(MapInfo Chunk, float LocalX, float LocalY)
        {
            for (int i = 0; i < Footprints.Count; i++)
            {
                ZoneFootprint Foot = Footprints[i];
                if (Foot.Chunk != Chunk)
                {
                    continue;
                }

                if (LocalX >= Foot.Min.X && LocalY >= Foot.Min.Y && LocalX <= Foot.Max.X && LocalY <= Foot.Max.Y)
                {
                    return true;
                }
            }

            return false;
        }

        public ZoneFootprint Foot(MapInfo Chunk)
        {
            for (int i = 0; i < Footprints.Count; i++)
            {
                if (Footprints[i].Chunk == Chunk)
                {
                    return Footprints[i];
                }
            }

            return null;
        }
    }

    public static class BuildingZones
    {
        public static List<ZoneFootprint> Cover(IList<MapInfo> Maps, Vector3 WorldA, Vector3 WorldB)
        {
            List<ZoneFootprint> Found = new List<ZoneFootprint>();
            if (Maps == null)
            {
                return Found;
            }

            float Left = Math.Min(WorldA.X, WorldB.X);
            float Right = Math.Max(WorldA.X, WorldB.X);
            float Top = Math.Min(WorldA.Y, WorldB.Y);
            float Bottom = Math.Max(WorldA.Y, WorldB.Y);
            for (int i = 0; i < Maps.Count; i++)
            {
                MapInfo Map = Maps[i];
                if (Map == null || Map.MapMapContainer == null || Map.MapMapContainer.SpanX <= 0f || Map.MapMapContainer.SpanY <= 0f)
                {
                    continue;
                }

                float X0 = Map.MapMapContainer.OffsetX;
                float Y0 = Map.MapMapContainer.OffsetY;
                float X1 = X0 + Map.MapMapContainer.SpanX;
                float Y1 = Y0 + Map.MapMapContainer.SpanY;
                float L = Math.Max(Left, X0);
                float R = Math.Min(Right, X1);
                float T = Math.Max(Top, Y0);
                float B = Math.Min(Bottom, Y1);
                if (R < L || B < T)
                {
                    continue;
                }

                ZoneFootprint Foot = new ZoneFootprint();
                Foot.Chunk = Map;
                Foot.Min = new Vector3(L - X0, T - Y0, 0f);
                Foot.Max = new Vector3(R - X0, B - Y0, 0f);
                Found.Add(Foot);
            }

            return Found;
        }

        public static void Add(IList<BuildingZone> World, BuildingZone Zone)
        {
            if (Zone == null)
            {
                return;
            }

            if (World != null && !World.Contains(Zone))
            {
                World.Add(Zone);
            }

            for (int i = 0; i < Zone.Footprints.Count; i++)
            {
                MapInfo Chunk = Zone.Footprints[i].Chunk;
                if (Chunk == null)
                {
                    continue;
                }

                if (Chunk.Zones == null)
                {
                    Chunk.Zones = new List<BuildingZone>();
                }

                if (!Chunk.Zones.Contains(Zone))
                {
                    Chunk.Zones.Add(Zone);
                }
            }

            Grade(Zone);
            Rebuild(Zone);
        }

        public const float PadBlend = 48f;

        static void Grade(BuildingZone Zone)
        {
            float MinX;
            float MinY;
            float MaxX;
            float MaxY;
            if (!WorldBounds(Zone, out MinX, out MinY, out MaxX, out MaxY))
            {
                return;
            }

            List<MapInfo> Land = Around(Zone, MinX, MinY, MaxX, MaxY, PadBlend);
            float Height;
            if (!Mean(Land, MinX, MinY, MaxX, MaxY, out Height))
            {
                return;
            }

            List<MapInfo> Changed = new List<MapInfo>();
            for (int i = 0; i < Land.Count; i++)
            {
                MapInfo Chunk = Land[i];
                if (Chunk == null || Chunk.Heights == null || Chunk.MapMapContainer == null)
                {
                    continue;
                }

                Chunk.Kind = GroundKind.Heightmap;
                Chunk.Heights.LevelPad(Chunk.MapMapContainer.OffsetX, Chunk.MapMapContainer.OffsetY, MinX, MinY, MaxX, MaxY, Height, PadBlend);
                Changed.Add(Chunk);
            }

            if (Changed.Count == 0)
            {
                return;
            }

            for (int i = 0; i < Zone.Footprints.Count; i++)
            {
                ZoneFootprint Foot = Zone.Footprints[i];
                if (Foot == null)
                {
                    continue;
                }

                Foot.Min = new Vector3(Foot.Min.X, Foot.Min.Y, Height);
                Foot.Max = new Vector3(Foot.Max.X, Foot.Max.Y, Height);
            }

            if (Zone.EntryChunk != null && Zone.EntryChunk.Heights != null)
            {
                float Ground;
                float SlopeX;
                float SlopeY;
                if (Zone.EntryChunk.Heights.Sample(Zone.Entry.X, Zone.Entry.Y, out Ground, out SlopeX, out SlopeY))
                {
                    Zone.Entry = new Vector3(Zone.Entry.X, Zone.Entry.Y, Ground);
                }
            }

            for (int i = 0; i < Changed.Count; i++)
            {
                MapInfo Chunk = Changed[i];
                Chunk.Heights.Rebuild();
                if (Chunk.Forest != null && Chunk.Forest.HasTrees)
                {
                    ForestScatter.Fill(Chunk);
                }

                WorldStream.OpenView(Chunk);
                WorldStream.Submit(Chunk);
                WorldStream.Save(Chunk);
            }
        }

        static bool WorldBounds(BuildingZone Zone, out float MinX, out float MinY, out float MaxX, out float MaxY)
        {
            MinX = float.MaxValue;
            MinY = float.MaxValue;
            MaxX = float.MinValue;
            MaxY = float.MinValue;
            if (Zone == null)
            {
                return false;
            }

            for (int i = 0; i < Zone.Footprints.Count; i++)
            {
                ZoneFootprint Foot = Zone.Footprints[i];
                if (Foot == null || Foot.Chunk == null || Foot.Chunk.MapMapContainer == null || Foot.Chunk.Heights == null)
                {
                    continue;
                }

                float X = Foot.Chunk.MapMapContainer.OffsetX;
                float Y = Foot.Chunk.MapMapContainer.OffsetY;
                MinX = Math.Min(MinX, X + Foot.Min.X);
                MinY = Math.Min(MinY, Y + Foot.Min.Y);
                MaxX = Math.Max(MaxX, X + Foot.Max.X);
                MaxY = Math.Max(MaxY, Y + Foot.Max.Y);
            }

            return MaxX > MinX && MaxY > MinY;
        }

        static List<MapInfo> Around(BuildingZone Zone, float MinX, float MinY, float MaxX, float MaxY, float Blend)
        {
            List<MapInfo> Found = new List<MapInfo>();
            for (int i = 0; i < Zone.Footprints.Count; i++)
            {
                Keep(Found, Zone.Footprints[i] == null ? null : Zone.Footprints[i].Chunk);
            }

            if (WorldStream.Slot != null && WorldStream.Slot.ChunkSize > 1f)
            {
                float Size = WorldStream.Slot.ChunkSize;
                int X0 = (int)Math.Floor((MinX - Blend) / Size);
                int X1 = (int)Math.Floor((MaxX + Blend) / Size);
                int Z0 = (int)Math.Floor((MinY - Blend) / Size);
                int Z1 = (int)Math.Floor((MaxY + Blend) / Size);
                for (int Z = Z0; Z <= Z1; Z++)
                {
                    for (int X = X0; X <= X1; X++)
                    {
                        if (!Holds(Found, X, Z, Size))
                        {
                            Keep(Found, WorldStream.ChunkAt(X, Z, true));
                        }
                    }
                }
            }

            return Found;
        }

        static bool Holds(List<MapInfo> Found, int X, int Z, float Size)
        {
            float OriginX = X * Size;
            float OriginY = Z * Size;
            for (int i = 0; i < Found.Count; i++)
            {
                MapInfo Chunk = Found[i];
                if (Chunk != null && Chunk.MapMapContainer != null)
                {
                    float DX = Chunk.MapMapContainer.OffsetX - OriginX;
                    float DY = Chunk.MapMapContainer.OffsetY - OriginY;
                    if (DX > -1f && DX < 1f && DY > -1f && DY < 1f)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        static void Keep(List<MapInfo> Found, MapInfo Chunk)
        {
            if (Chunk == null || Found.Contains(Chunk))
            {
                return;
            }

            Found.Add(Chunk);
        }

        static bool Mean(List<MapInfo> Land, float MinX, float MinY, float MaxX, float MaxY, out float Height)
        {
            Height = 0f;
            double Sum = 0d;
            int Count = 0;
            for (int i = 0; i < Land.Count; i++)
            {
                MapInfo Chunk = Land[i];
                HeightField Field = Chunk == null ? null : Chunk.Heights;
                if (Field == null || Field.Heights == null || Field.Spacing <= 0.01f || Chunk.MapMapContainer == null)
                {
                    continue;
                }

                float OriginX = Chunk.MapMapContainer.OffsetX;
                float OriginY = Chunk.MapMapContainer.OffsetY;
                for (int Z = 0; Z < Field.SamplesZ; Z++)
                {
                    for (int X = 0; X < Field.SamplesX; X++)
                    {
                        float WorldX = OriginX + X * Field.Spacing;
                        float WorldY = OriginY + Z * Field.Spacing;
                        if (WorldX < MinX || WorldY < MinY || WorldX > MaxX || WorldY > MaxY)
                        {
                            continue;
                        }

                        Sum += Field.At(X, Z);
                        Count++;
                    }
                }
            }

            if (Count == 0)
            {
                return false;
            }

            Height = (float)(Sum / Count);
            return true;
        }

        public static void Delete(IList<BuildingZone> World, BuildingZone Zone)
        {
            if (Zone == null)
            {
                return;
            }

            List<ZoneFootprint> Feet = new List<ZoneFootprint>(Zone.Footprints);
            if (World != null)
            {
                World.Remove(Zone);
            }

            for (int i = 0; i < Feet.Count; i++)
            {
                MapInfo Chunk = Feet[i].Chunk;
                if (Chunk != null && Chunk.Zones != null)
                {
                    Chunk.Zones.Remove(Zone);
                }
            }

            for (int i = 0; i < Feet.Count; i++)
            {
                ClearFoot(Feet[i]);
            }
        }

        public static bool Allows(MapInfo Chunk, float LocalX, float LocalY)
        {
            return At(Chunk, LocalX, LocalY) != null;
        }

        public static BuildingZone At(MapInfo Chunk, float LocalX, float LocalY)
        {
            if (Chunk == null || Chunk.Zones == null)
            {
                return null;
            }

            for (int i = 0; i < Chunk.Zones.Count; i++)
            {
                BuildingZone Zone = Chunk.Zones[i];
                if (Zone != null && Zone.Contains(Chunk, LocalX, LocalY))
                {
                    return Zone;
                }
            }

            return null;
        }

        public static BuildingZone Shared(MapInfo Start, float StartX, float StartY, MapInfo Goal, float GoalX, float GoalY)
        {
            if (Start == null || Start.Zones == null || Goal == null)
            {
                return null;
            }

            for (int i = 0; i < Start.Zones.Count; i++)
            {
                BuildingZone Zone = Start.Zones[i];
                if (Zone != null && Zone.Contains(Start, StartX, StartY) && Zone.Contains(Goal, GoalX, GoalY))
                {
                    return Zone;
                }
            }

            return null;
        }

        public static BuildingZone Named(PlayerCharacter Owner, string Name)
        {
            if (Owner == null || string.IsNullOrEmpty(Name))
            {
                return null;
            }

            List<BuildingZone> Seen = new List<BuildingZone>();
            if (Owner.SharedMapContex != null && Owner.SharedMapContex.RootMapContainer != null)
            {
                Consider(Seen, Owner.SharedMapContex.RootMapContainer.Zones, Name);
            }

            MapInfo Here = Owner.SharedMapContex == null ? null : Owner.SharedMapContex.CurrentMapInfo;
            if (Here != null)
            {
                Consider(Seen, Here.Zones, Name);
            }

            if (Owner.Belief != null && Owner.Belief.ListKnownArea != null)
            {
                for (int i = 0; i < Owner.Belief.ListKnownArea.Count; i++)
                {
                    MapInfo Area = Owner.Belief.ListKnownArea[i];
                    if (Area != null)
                    {
                        Consider(Seen, Area.Zones, Name);
                    }
                }
            }

            for (int i = 0; i < Seen.Count; i++)
            {
                if (Seen[i].Public)
                {
                    return Seen[i];
                }
            }

            return null;
        }

        public static void Rebuild(BuildingZone Zone)
        {
            if (Zone == null)
            {
                return;
            }

            for (int i = 0; i < Zone.Footprints.Count; i++)
            {
                ZoneFootprint Foot = Zone.Footprints[i];
                Foot.Interior = Clip(Foot);
            }
        }

        public static void Refresh(MapInfo Chunk)
        {
            if (Chunk == null || Chunk.Zones == null)
            {
                return;
            }

            for (int i = 0; i < Chunk.Zones.Count; i++)
            {
                Rebuild(Chunk.Zones[i]);
            }
        }

        public static List<ChunkHop> Walk(BuildingZone Zone, MapInfo Start, MapInfo Goal)
        {
            List<ChunkHop> Empty = new List<ChunkHop>();
            if (Zone == null || Start == null || Goal == null || Start == Goal)
            {
                return Empty;
            }

            Dictionary<MapInfo, MapInfo> Parent = new Dictionary<MapInfo, MapInfo>();
            Dictionary<MapInfo, ChunkHop> EnteredBy = new Dictionary<MapInfo, ChunkHop>();
            List<MapInfo> Open = new List<MapInfo>();
            List<MapInfo> Closed = new List<MapInfo>();
            Open.Add(Start);
            MapInfo Found = null;
            while (Open.Count > 0)
            {
                MapInfo Current = Open[0];
                Open.RemoveAt(0);
                if (Closed.Contains(Current))
                {
                    continue;
                }

                Closed.Add(Current);
                if (Current == Goal)
                {
                    Found = Current;
                    break;
                }

                if (!Zone.Covers(Current) || Current.ListLink == null)
                {
                    continue;
                }

                for (int i = 0; i < Current.ListLink.Count; i++)
                {
                    ChunkHop Link = Current.ListLink[i];
                    if (Link == null || !Link.IsOpen || Link.IsPortal)
                    {
                        continue;
                    }

                    ChunkHop Forward = null;
                    if (Link.FromChunk == Current)
                    {
                        Forward = Link;
                    }
                    else if (Link.ToChunk == Current)
                    {
                        Forward = Link.Reversed();
                    }

                    if (Forward == null || Forward.ToChunk == null || !Zone.Covers(Forward.ToChunk) || Closed.Contains(Forward.ToChunk) || EnteredBy.ContainsKey(Forward.ToChunk))
                    {
                        continue;
                    }

                    EnteredBy[Forward.ToChunk] = Forward;
                    Parent[Forward.ToChunk] = Current;
                    Open.Add(Forward.ToChunk);
                }
            }

            if (Found == null)
            {
                return Empty;
            }

            List<ChunkHop> Path = new List<ChunkHop>();
            MapInfo Step = Found;
            while (Parent.ContainsKey(Step))
            {
                Path.Add(EnteredBy[Step]);
                Step = Parent[Step];
            }

            Path.Reverse();
            return Path;
        }

        static void Consider(List<BuildingZone> Seen, IList<BuildingZone> Zones, string Name)
        {
            if (Zones == null)
            {
                return;
            }

            for (int i = 0; i < Zones.Count; i++)
            {
                BuildingZone Zone = Zones[i];
                if (Zone == null || Seen.Contains(Zone) || Zone.Name != Name)
                {
                    continue;
                }

                Seen.Add(Zone);
            }
        }

        static void ClearFoot(ZoneFootprint Foot)
        {
            if (Foot == null || Foot.Chunk == null || Foot.Chunk.Furniture == null || Foot.Chunk.Buildings == null)
            {
                return;
            }

            List<FurnitureItem> InsideFurniture = new List<FurnitureItem>();
            for (int i = 0; i < Foot.Chunk.Furniture.Count; i++)
            {
                FurnitureItem Box = Foot.Chunk.Furniture[i];
                if (Box != null && Box.Position.X >= Foot.Min.X && Box.Position.Y >= Foot.Min.Y && Box.Position.X <= Foot.Max.X && Box.Position.Y <= Foot.Max.Y)
                {
                    InsideFurniture.Add(Box);
                }
            }

            List<BuildingItem> InsideBuildings = new List<BuildingItem>();
            for (int i = 0; i < Foot.Chunk.Buildings.Count; i++)
            {
                BuildingItem Box = Foot.Chunk.Buildings[i];
                if (Box != null && Box.Position.X >= Foot.Min.X && Box.Position.Y >= Foot.Min.Y && Box.Position.X <= Foot.Max.X && Box.Position.Y <= Foot.Max.Y)
                {
                    InsideBuildings.Add(Box);
                }
            }

            LifeSimMap Map = Foot.Chunk.MapMapContainer == null ? null : Foot.Chunk.MapMapContainer.ActiveMap;
            float Tile = Map != null && Map.TileSize.X > 0 ? Map.TileSize.X : 32f;
            for (int i = 0; i < InsideFurniture.Count; i++)
            {
                BoxPlace.Drop(Foot.Chunk, InsideFurniture[i]);
            }

            for (int i = 0; i < InsideBuildings.Count; i++)
            {
                BuildingItem Box = InsideBuildings[i];
                if (Box.Kind == BuildingKind.Wall && Map != null)
                {
                    BoxPlace.ClearWall(Map, (int)(Box.Position.X / Tile), (int)(Box.Position.Y / Tile));
                }

                BoxPlace.Drop(Foot.Chunk, Box);
            }
        }

        static NavSourceGeometry Clip(ZoneFootprint Foot)
        {
            NavSourceGeometry Geometry = new NavSourceGeometry();
            if (Foot == null || Foot.Chunk == null || Foot.Chunk.MapMapContainer == null)
            {
                return Geometry;
            }

            HeightField Field = Foot.Chunk.Heights;
            if (Field != null && Field.Spacing > 0.01f)
            {
                float Step = Field.Spacing;
                for (float Y = Foot.Min.Y; Y < Foot.Max.Y - 0.01f; Y += Step)
                {
                    float Y1 = Math.Min(Y + Step, Foot.Max.Y);
                    for (float X = Foot.Min.X; X < Foot.Max.X - 0.01f; X += Step)
                    {
                        float X1 = Math.Min(X + Step, Foot.Max.X);
                        float A;
                        float B;
                        float C;
                        float D;
                        if (!FieldHeight(Field, X, Y, out A) || !FieldHeight(Field, X, Y1, out B) || !FieldHeight(Field, X1, Y1, out C) || !FieldHeight(Field, X1, Y, out D))
                        {
                            continue;
                        }

                        Geometry.Triangles.Add(new NavTriangle(new Vector3(X, Y, A), new Vector3(X, Y1, B), new Vector3(X1, Y1, C), NavArea.Ground));
                        Geometry.Triangles.Add(new NavTriangle(new Vector3(X, Y, A), new Vector3(X1, Y1, C), new Vector3(X1, Y, D), NavArea.Ground));
                    }
                }
            }
            else
            {
                LifeSimMap Map = Foot.Chunk.MapMapContainer.ActiveMap;
                if (Map != null)
                {
                    NavSourceGeometry Full = GridNavGeometry.Build(Map);
                    for (int i = 0; i < Full.Triangles.Count; i++)
                    {
                        NavTriangle Triangle = Full.Triangles[i];
                        Vector3 Center = (Triangle.A + Triangle.B + Triangle.C) / 3f;
                        if (Center.X >= Foot.Min.X && Center.Y >= Foot.Min.Y && Center.X <= Foot.Max.X && Center.Y <= Foot.Max.Y)
                        {
                            Geometry.Triangles.Add(Triangle);
                        }
                    }
                }
            }

            if (Foot.Chunk.Furniture != null)
            {
                for (int i = 0; i < Foot.Chunk.Furniture.Count; i++)
                {
                    FurnitureItem Box = Foot.Chunk.Furniture[i];
                    if (Box != null && Box.BlocksBody() && Box.Position.X >= Foot.Min.X && Box.Position.Y >= Foot.Min.Y && Box.Position.X <= Foot.Max.X && Box.Position.Y <= Foot.Max.Y)
                    {
                        Geometry.Blockers.Add(Box.Body);
                    }
                }
            }

            if (Foot.Chunk.Buildings != null)
            {
                for (int i = 0; i < Foot.Chunk.Buildings.Count; i++)
                {
                    BuildingItem Box = Foot.Chunk.Buildings[i];
                    if (Box != null && Box.BlocksBody() && Box.Position.X >= Foot.Min.X && Box.Position.Y >= Foot.Min.Y && Box.Position.X <= Foot.Max.X && Box.Position.Y <= Foot.Max.Y)
                    {
                        Geometry.Blockers.Add(Box.Body);
                    }
                }
            }

            return Geometry;
        }

        static bool FieldHeight(HeightField Field, float LocalX, float LocalY, out float Height)
        {
            float SlopeX;
            float SlopeY;
            return Field.Sample(LocalX, LocalY, out Height, out SlopeX, out SlopeY);
        }
    }

    public static class BuildingZoneDraw
    {
        static BasicEffect Effect;

        public static void All(GraphicsDevice Device, Camera3D Camera, IList<MapInfo> Maps, IList<BuildingZone> Zones, ZoneDraft Draft)
        {
            if (Device == null || Camera == null || Maps == null)
            {
                return;
            }

            bool AnyZone = Zones != null && Zones.Count > 0;
            bool Ghost = Draft != null && Draft.HasCorner;
            if (!AnyZone && !Ghost)
            {
                return;
            }

            List<VertexPositionColor> Vertices = new List<VertexPositionColor>();
            Color Placed = new Color(80, 200, 120) * 0.35f;
            Color Live = new Color(80, 160, 255) * 0.45f;
            Color Mark = new Color(255, 220, 80) * 0.8f;
            if (AnyZone)
            {
                for (int z = 0; z < Zones.Count; z++)
                {
                    BuildingZone Zone = Zones[z];
                    if (Zone == null)
                    {
                        continue;
                    }

                    for (int i = 0; i < Zone.Footprints.Count; i++)
                    {
                        AddFoot(Vertices, Zone.Footprints[i], Placed);
                    }

                    if (Zone.EntryChunk != null)
                    {
                        AddLocal(Vertices, Zone.EntryChunk, Zone.Entry.X, Zone.Entry.Y, Mark);
                    }
                }
            }

            if (Ghost && Draft.ShowRectangle)
            {
                List<ZoneFootprint> Preview = BuildingZones.Cover(Maps, Draft.Corner, Draft.Opposite);
                for (int i = 0; i < Preview.Count; i++)
                {
                    AddFoot(Vertices, Preview[i], Live);
                }
            }
            else if (Ghost)
            {
                AddMark(Vertices, ChunkCross.At(Draft.Corner.X, Draft.Corner.Y), Draft.Corner.X, Draft.Corner.Y, Live);
            }

            if (Draft != null && Draft.ShowEntry)
            {
                AddMark(Vertices, ChunkCross.At(Draft.Entry.X, Draft.Entry.Y), Draft.Entry.X, Draft.Entry.Y, Mark);
            }

            if (Vertices.Count == 0)
            {
                return;
            }

            if (Effect == null || Effect.GraphicsDevice != Device)
            {
                Effect = new BasicEffect(Device);
                Effect.VertexColorEnabled = true;
                Effect.LightingEnabled = false;
                Effect.TextureEnabled = false;
            }

            Viewport Pane = Device.Viewport;
            float Aspect = Pane.Height > 0 ? Pane.Width / (float)Pane.Height : 1f;
            Effect.View = Camera.View;
            Effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Aspect, 1f, 10000f);
            Effect.World = Matrix.Identity;
            BlendState PreviousBlend = Device.BlendState;
            DepthStencilState PreviousDepth = Device.DepthStencilState;
            RasterizerState PreviousRaster = Device.RasterizerState;
            Device.BlendState = BlendState.AlphaBlend;
            Device.DepthStencilState = DepthStencilState.DepthRead;
            Device.RasterizerState = RasterizerState.CullNone;
            foreach (EffectPass Pass in Effect.CurrentTechnique.Passes)
            {
                Pass.Apply();
                Device.DrawUserPrimitives(PrimitiveType.TriangleList, Vertices.ToArray(), 0, Vertices.Count / 3);
            }

            Device.BlendState = PreviousBlend;
            Device.DepthStencilState = PreviousDepth;
            Device.RasterizerState = PreviousRaster;
        }

        static void AddFoot(List<VertexPositionColor> Vertices, ZoneFootprint Foot, Color Color)
        {
            if (Foot == null || Foot.Chunk == null)
            {
                return;
            }

            Sheet(Vertices, Foot.Chunk, Foot.Min.X, Foot.Min.Y, Foot.Max.X, Foot.Max.Y, Color);
        }

        static void AddMark(List<VertexPositionColor> Vertices, MapInfo Chunk, float WorldX, float WorldY, Color Color)
        {
            if (Chunk == null || Chunk.MapMapContainer == null)
            {
                return;
            }

            AddLocal(Vertices, Chunk, WorldX - Chunk.MapMapContainer.OffsetX, WorldY - Chunk.MapMapContainer.OffsetY, Color);
        }

        static void AddLocal(List<VertexPositionColor> Vertices, MapInfo Chunk, float LocalX, float LocalY, Color Color)
        {
            if (Chunk == null)
            {
                return;
            }

            Sheet(Vertices, Chunk, LocalX - 6f, LocalY - 6f, LocalX + 6f, LocalY + 6f, Color);
        }

        static void Sheet(List<VertexPositionColor> Vertices, MapInfo Chunk, float MinX, float MinY, float MaxX, float MaxY, Color Color)
        {
            if (MaxX < MinX || MaxY < MinY)
            {
                return;
            }

            HeightField Field = Chunk.Heights;
            float Step = Field != null && Field.Spacing > 1f ? Field.Spacing : Math.Max(MaxX - MinX, 1f);
            Vector3 Shift = ChunkOrigin.Shift(Chunk);
            for (float Y = MinY; Y < MaxY - 0.01f; Y += Step)
            {
                float Y1 = Math.Min(Y + Step, MaxY);
                for (float X = MinX; X < MaxX - 0.01f; X += Step)
                {
                    float X1 = Math.Min(X + Step, MaxX);
                    Quad(Vertices, Shift, X, Y, X1, Y1, Lift(Field, X, Y), Lift(Field, X1, Y), Lift(Field, X1, Y1), Lift(Field, X, Y1), Color);
                }
            }
        }

        static float Lift(HeightField Field, float LocalX, float LocalY)
        {
            float Height;
            float SlopeX;
            float SlopeY;
            if (Field != null && Field.Sample(LocalX, LocalY, out Height, out SlopeX, out SlopeY))
            {
                return Height + 0.35f;
            }

            return 1.5f;
        }

        static void Quad(List<VertexPositionColor> Vertices, Vector3 Shift, float MinX, float MinY, float MaxX, float MaxY, float AHeight, float BHeight, float CHeight, float DHeight, Color Color)
        {
            Vector3 A = new Vector3(MinX + Shift.X, AHeight, MinY + Shift.Z);
            Vector3 B = new Vector3(MaxX + Shift.X, BHeight, MinY + Shift.Z);
            Vector3 C = new Vector3(MaxX + Shift.X, CHeight, MaxY + Shift.Z);
            Vector3 D = new Vector3(MinX + Shift.X, DHeight, MaxY + Shift.Z);
            Vertices.Add(new VertexPositionColor(A, Color));
            Vertices.Add(new VertexPositionColor(B, Color));
            Vertices.Add(new VertexPositionColor(C, Color));
            Vertices.Add(new VertexPositionColor(A, Color));
            Vertices.Add(new VertexPositionColor(C, Color));
            Vertices.Add(new VertexPositionColor(D, Color));
        }
    }
}
