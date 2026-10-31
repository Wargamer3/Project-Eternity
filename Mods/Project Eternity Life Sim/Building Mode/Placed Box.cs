using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public enum BoxKind
    {
        SmallCube,
        Step,
        Long,
    }

    public enum BuildingKind
    {
        Wall,
        Door,
        Window,
        Floor,
        Stairs,
    }

    public struct BoxVolume
    {
        public Vector3 Position;
        public float Width;
        public float Height;
        public float Length;
        public float Angle;
        public int Floor;

        public bool Contains(Vector3 Point)
        {
            float HalfW = Width * 0.5f;
            float HalfL = Length * 0.5f;
            if (System.Math.Abs(Point.X - Position.X) > HalfW)
            {
                return false;
            }

            if (System.Math.Abs(Point.Y - Position.Y) > HalfL)
            {
                return false;
            }

            return Point.Z >= Position.Z - 0.01f && Point.Z <= Position.Z + Height;
        }

        public bool FootprintContains(float X, float Y)
        {
            float Along;
            float Across;
            MapToLocal(X, Y, out Along, out Across);
            return System.Math.Abs(Along) <= Width * 0.5f && System.Math.Abs(Across) <= Length * 0.5f;
        }

        public bool FootprintOverlaps(BoxVolume Other)
        {
            float DX = Other.Position.X - Position.X;
            float DY = Other.Position.Y - Position.Y;
            if (!OverlapOn(this, Other, DX, DY, (float)System.Math.Cos(Angle), (float)System.Math.Sin(Angle)))
            {
                return false;
            }

            if (!OverlapOn(this, Other, DX, DY, -(float)System.Math.Sin(Angle), (float)System.Math.Cos(Angle)))
            {
                return false;
            }

            if (!OverlapOn(this, Other, DX, DY, (float)System.Math.Cos(Other.Angle), (float)System.Math.Sin(Other.Angle)))
            {
                return false;
            }

            return OverlapOn(this, Other, DX, DY, -(float)System.Math.Sin(Other.Angle), (float)System.Math.Cos(Other.Angle));
        }

        public void MapToLocal(float X, float Y, out float Along, out float Across)
        {
            float DX = X - Position.X;
            float DY = Y - Position.Y;
            float C = (float)System.Math.Cos(Angle);
            float S = (float)System.Math.Sin(Angle);
            Along = DX * C + DY * S;
            Across = -DX * S + DY * C;
        }

        static bool OverlapOn(BoxVolume A, BoxVolume B, float DX, float DY, float AxisX, float AxisY)
        {
            float Distance = System.Math.Abs(DX * AxisX + DY * AxisY);
            float Reach = ReachOn(A, AxisX, AxisY) + ReachOn(B, AxisX, AxisY) - 0.05f;
            return Distance <= Reach;
        }

        static float ReachOn(BoxVolume Box, float AxisX, float AxisY)
        {
            float C = (float)System.Math.Cos(Box.Angle);
            float S = (float)System.Math.Sin(Box.Angle);
            float Along = System.Math.Abs(C * AxisX + S * AxisY);
            float Across = System.Math.Abs(-S * AxisX + C * AxisY);
            return Box.Width * 0.5f * Along + Box.Length * 0.5f * Across;
        }

        public bool Hits(Vector3 From, Vector3 To)
        {
            float Low = System.Math.Min(From.Z, To.Z);
            float High = System.Math.Max(From.Z, To.Z);
            if (High < Position.Z - 0.01f || Low > Position.Z + Height)
            {
                return false;
            }

            float AX;
            float AY;
            float BX;
            float BY;
            MapToLocal(From.X, From.Y, out AX, out AY);
            MapToLocal(To.X, To.Y, out BX, out BY);
            return SegmentCrosses(AX, AY, BX, BY, -Width * 0.5f, -Length * 0.5f, Width * 0.5f, Length * 0.5f);
        }

        public bool Crosses(Vector3 A, Vector3 B, Vector3 C)
        {
            float HalfW = System.Math.Max(0.5f, Width * 0.5f - 0.75f);
            float HalfL = System.Math.Max(0.5f, Length * 0.5f - 0.75f);
            if (Holds(A, HalfW, HalfL) || Holds(B, HalfW, HalfL) || Holds(C, HalfW, HalfL))
            {
                return true;
            }

            Vector3 Middle = new Vector3((A.X + B.X + C.X) / 3f, (A.Y + B.Y + C.Y) / 3f, A.Z);
            if (Holds(Middle, HalfW, HalfL))
            {
                return true;
            }

            float Cose = (float)System.Math.Cos(Angle);
            float Sine = (float)System.Math.Sin(Angle);
            Vector3 P0 = Corner(-HalfW, -HalfL, Cose, Sine);
            Vector3 P1 = Corner(HalfW, -HalfL, Cose, Sine);
            Vector3 P2 = Corner(HalfW, HalfL, Cose, Sine);
            Vector3 P3 = Corner(-HalfW, HalfL, Cose, Sine);
            if (InsideTriangle(P0, A, B, C) || InsideTriangle(P1, A, B, C) || InsideTriangle(P2, A, B, C) || InsideTriangle(P3, A, B, C))
            {
                return true;
            }

            return EdgeHits(A, B, HalfW, HalfL) || EdgeHits(B, C, HalfW, HalfL) || EdgeHits(C, A, HalfW, HalfL);
        }

        bool Holds(Vector3 Point, float HalfW, float HalfL)
        {
            float Along;
            float Across;
            MapToLocal(Point.X, Point.Y, out Along, out Across);
            return System.Math.Abs(Along) <= HalfW && System.Math.Abs(Across) <= HalfL;
        }

        Vector3 Corner(float Along, float Across, float Cose, float Sine)
        {
            return new Vector3(Position.X + Along * Cose - Across * Sine, Position.Y + Along * Sine + Across * Cose, Position.Z);
        }

        bool EdgeHits(Vector3 A, Vector3 B, float HalfW, float HalfL)
        {
            float AX;
            float AY;
            float BX;
            float BY;
            MapToLocal(A.X, A.Y, out AX, out AY);
            MapToLocal(B.X, B.Y, out BX, out BY);
            return SegmentCrosses(AX, AY, BX, BY, -HalfW, -HalfL, HalfW, HalfL);
        }

        static bool InsideTriangle(Vector3 Point, Vector3 A, Vector3 B, Vector3 C)
        {
            float AB = Edge(A, B, Point);
            float BC = Edge(B, C, Point);
            float CA = Edge(C, A, Point);
            bool Negative = AB < -0.01f || BC < -0.01f || CA < -0.01f;
            bool Positive = AB > 0.01f || BC > 0.01f || CA > 0.01f;
            return !(Negative && Positive);
        }

        static float Edge(Vector3 From, Vector3 To, Vector3 Point)
        {
            return (To.X - From.X) * (Point.Y - From.Y) - (To.Y - From.Y) * (Point.X - From.X);
        }

        public float Tread(Vector3 Point)
        {
            bool AlongX = Width >= Length;
            float Start = AlongX ? Position.X - Width * 0.5f : Position.Y - Length * 0.5f;
            float End = AlongX ? Position.X + Width * 0.5f : Position.Y + Length * 0.5f;
            float At = AlongX ? Point.X : Point.Y;
            float Span = End - Start;
            if (Span < 0.01f)
            {
                return Position.Z;
            }

            float T = (At - Start) / Span;
            if (T < 0f)
            {
                T = 0f;
            }
            else if (T > 1f)
            {
                T = 1f;
            }

            return Position.Z + T * Height;
        }

        static bool SegmentCrosses(float X0, float Y0, float X1, float Y1, float MinX, float MinY, float MaxX, float MaxY)
        {
            float T0 = 0f;
            float T1 = 1f;
            float DX = X1 - X0;
            float DY = Y1 - Y0;
            if (!Clip(-DX, X0 - MinX, ref T0, ref T1))
            {
                return false;
            }

            if (!Clip(DX, MaxX - X0, ref T0, ref T1))
            {
                return false;
            }

            if (!Clip(-DY, Y0 - MinY, ref T0, ref T1))
            {
                return false;
            }

            if (!Clip(DY, MaxY - Y0, ref T0, ref T1))
            {
                return false;
            }

            return T0 <= T1;
        }

        static bool Clip(float Direction, float Room, ref float T0, ref float T1)
        {
            if (System.Math.Abs(Direction) < 0.00001f)
            {
                return Room >= 0f;
            }

            float T = Room / Direction;
            if (Direction < 0f)
            {
                if (T > T1)
                {
                    return false;
                }

                if (T > T0)
                {
                    T0 = T;
                }
            }
            else
            {
                if (T < T0)
                {
                    return false;
                }

                if (T < T1)
                {
                    T1 = T;
                }
            }

            return true;
        }
    }

    public class FurnitureItem
    {
        public BoxVolume Body;
        public BoxKind Kind;
        public bool Upright;
        public string Interaction;
        public Vector3 Swatch;
        public int Price;
        public string Model;
        public float ModelMeters;
        public Vector3 ModelShift;
        public bool Open;
        public bool RoomProp;
        public List<ConvexBrush> Brushes;

        public Vector3 Position
        {
            get { return Body.Position; }
            set { Body.Position = value; }
        }

        public float Width
        {
            get { return Body.Width; }
            set { Body.Width = value; }
        }

        public float Height
        {
            get { return Body.Height; }
            set { Body.Height = value; }
        }

        public float Length
        {
            get { return Body.Length; }
            set { Body.Length = value; }
        }

        public float Angle
        {
            get { return Body.Angle; }
            set { Body.Angle = value; }
        }

        public int Floor
        {
            get { return Body.Floor; }
            set { Body.Floor = value; }
        }

        public bool IsRope
        {
            get { return Interaction == "Rope"; }
        }

        public bool BlocksBody()
        {
            return !IsRope && Body.Width > 0.01f && Body.Length > 0.01f && Body.Height > 2f;
        }

        public bool HitsMove(Vector3 From, Vector3 To)
        {
            return BlocksBody() && Body.Hits(From, To);
        }

        public bool FootprintContains(float X, float Y)
        {
            return Body.FootprintContains(X, Y);
        }

        public bool Contains(Vector3 Point)
        {
            return Body.Contains(Point);
        }

        public Vector3 Beside()
        {
            return Body.Position - new Vector3(Body.Width * 0.5f + 4f, 0f, 0f);
        }

        public NavOffMeshLink Link()
        {
            Vector3 Start = Body.Position - new Vector3(Body.Length * 0.5f, 0f, 0f);
            Vector3 End = Body.Position + new Vector3(Body.Length * 0.5f, 0f, 0f);
            return new NavOffMeshLink(Traversal.Rope, Start, End);
        }

        public NavOffMeshLink LinkFor(TraversalCapabilities BodyCaps)
        {
            if (!IsRope || BodyCaps == null || !BodyCaps.Allows(Traversal.Rope))
            {
                return null;
            }

            return Link();
        }

        public void MapToLocal(float X, float Y, out float Along, out float Across)
        {
            Body.MapToLocal(X, Y, out Along, out Across);
        }
    }

    public class BuildingItem
    {
        public BoxVolume Body;
        public BuildingKind Kind;
        public string Interaction;
        public Vector3 Swatch;
        public int Price;
        public string Model;
        public float ModelMeters;
        public Vector3 ModelShift;

        public Vector3 Position
        {
            get { return Body.Position; }
            set { Body.Position = value; }
        }

        public float Width
        {
            get { return Body.Width; }
            set { Body.Width = value; }
        }

        public float Height
        {
            get { return Body.Height; }
            set { Body.Height = value; }
        }

        public float Length
        {
            get { return Body.Length; }
            set { Body.Length = value; }
        }

        public float Angle
        {
            get { return Body.Angle; }
            set { Body.Angle = value; }
        }

        public int Floor
        {
            get { return Body.Floor; }
            set { Body.Floor = value; }
        }

        public bool BlocksBody()
        {
            return Kind == BuildingKind.Wall && Body.Width > 0.01f && Body.Length > 0.01f && Body.Height > 0.01f;
        }

        public bool TryHull(out BoxVolume Volume)
        {
            Volume = Body;
            if (string.IsNullOrEmpty(Model) || Model.IndexOf("Medieval Village Pack", System.StringComparison.OrdinalIgnoreCase) < 0 || Model.IndexOf("Path_", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return false;
            }

            bool Piece = Model.IndexOf("Props", System.StringComparison.OrdinalIgnoreCase) >= 0;
            float Fit = Piece ? 2.5f : 5f;
            float Scale = ModelMeters > 0.01f ? Height / ModelMeters * Fit : Fit;
            Volume.Width = (Piece ? 0.7f : 2.2f) * Scale;
            Volume.Length = (Piece ? 0.6f : 2.6f) * Scale;
            Volume.Height = (Piece ? 1.1f : 3.2f) * Scale;
            return Volume.Width > 0.5f && Volume.Height > 2f;
        }

        public bool HitsMove(Vector3 From, Vector3 To)
        {
            return BlocksBody() && Body.Hits(From, To);
        }

        public bool FootprintContains(float X, float Y)
        {
            return Body.FootprintContains(X, Y);
        }

        public float Tread(Vector3 Point)
        {
            return Body.Tread(Point);
        }
    }

    public static class BoxPlace
    {
        public static void SizeOf(BoxKind Kind, float Tile, bool Upright, out float Width, out float Height, out float Length)
        {
            if (Tile <= 0f)
            {
                Tile = 32f;
            }

            Width = Tile;
            Height = Tile;
            Length = Tile;
            if (Kind == BoxKind.Step)
            {
                Height = Tile * 0.5f;
            }
            else if (Kind == BoxKind.Long && Upright)
            {
                Height = Tile * 3f;
            }
            else if (Kind == BoxKind.Long)
            {
                Length = Tile * 3f;
            }
        }

        public static FurnitureItem Place(IList<FurnitureItem> Boxes, Vector3 Cursor, float Tile, BoxKind Kind, string Interaction, bool Upright)
        {
            float Width;
            float Height;
            float Length;
            SizeOf(Kind, Tile, Upright, out Width, out Height, out Length);

            float Base = Cursor.Z > 0f ? Cursor.Z : 0f;
            if (Boxes != null)
            {
                for (int i = 0; i < Boxes.Count; i++)
                {
                    FurnitureItem Under = Boxes[i];
                    float DX = System.Math.Abs(Cursor.X - Under.Position.X);
                    float DY = System.Math.Abs(Cursor.Y - Under.Position.Y);
                    if (DX <= (Width + Under.Width) * 0.5f && DY <= (Length + Under.Length) * 0.5f)
                    {
                        float Top = Under.Position.Z + Under.Height;
                        if (Top > Base)
                        {
                            Base = Top;
                        }
                    }
                }
            }

            FurnitureItem Box = new FurnitureItem();
            Box.Kind = Kind;
            Box.Width = Width;
            Box.Height = Height;
            Box.Length = Length;
            Box.Upright = Upright;
            Box.Interaction = Interaction;
            Box.Floor = BuildFloor.Index(Base);
            Box.Position = new Vector3(Cursor.X, Cursor.Y, Base);

            return Box;
        }

        public static FurnitureItem Under(IList<FurnitureItem> Boxes, Vector3 Cursor)
        {
            FurnitureItem Found = null;
            float Best = 0f;
            if (Boxes == null)
            {
                return null;
            }

            for (int i = 0; i < Boxes.Count; i++)
            {
                FurnitureItem Box = Boxes[i];
                if (!Box.Contains(new Vector3(Cursor.X, Cursor.Y, Box.Position.Z + 1f)) && System.Math.Abs(Cursor.X - Box.Position.X) > Box.Width && System.Math.Abs(Cursor.Y - Box.Position.Y) > Box.Length)
                {
                    continue;
                }

                if (System.Math.Abs(Cursor.X - Box.Position.X) > Box.Width * 0.5f + 8f || System.Math.Abs(Cursor.Y - Box.Position.Y) > Box.Length * 0.5f + 8f)
                {
                    continue;
                }

                if (Found == null || Box.Position.Z >= Best)
                {
                    Found = Box;
                    Best = Box.Position.Z;
                }
            }

            return Found;
        }

        public static void PlaceWall(LifeSimMap Map, int X, int Y)
        {
            if (Map == null || Map.LayerManager == null || Map.LayerManager.ListLayer.Count == 0)
            {
                return;
            }

            if (X < 0 || Y < 0 || X >= Map.MapSize.X || Y >= Map.MapSize.Y)
            {
                return;
            }

            Map.LayerManager.ListLayer[0].ArrayTerrain[X, Y].TerrainTypeIndex = UnitStats.TerrainWallIndex;
        }

        public static void ClearWall(LifeSimMap Map, int X, int Y)
        {
            if (Map == null || Map.LayerManager == null || Map.LayerManager.ListLayer.Count == 0)
            {
                return;
            }

            if (X < 0 || Y < 0 || X >= Map.MapSize.X || Y >= Map.MapSize.Y)
            {
                return;
            }

            Map.LayerManager.ListLayer[0].ArrayTerrain[X, Y].TerrainTypeIndex = UnitStats.TerrainLandIndex;
        }

        public static void Cell(Vector3 Ground, float Tile, out int X, out int Y)
        {
            if (Tile <= 0f)
            {
                Tile = 32f;
            }

            X = (int)System.Math.Floor(Ground.X / Tile);
            Y = (int)System.Math.Floor(Ground.Y / Tile);
        }

        public static BuildingItem Wall(float Tile, int X, int Y)
        {
            if (Tile <= 0f)
            {
                Tile = 32f;
            }

            float Width;
            float Height;
            float Length;
            SizeOf(BoxKind.Long, Tile, true, out Width, out Height, out Length);
            BuildingItem Box = new BuildingItem();
            Box.Kind = BuildingKind.Wall;
            Box.Width = Width;
            Box.Height = Height;
            Box.Length = Length;
            Box.Floor = 0;
            Box.Position = new Vector3((X + 0.5f) * Tile, (Y + 0.5f) * Tile, 0f);
            return Box;
        }

        public static void Keep(MapInfo Here, FurnitureItem Box)
        {
            if (Here == null || Box == null)
            {
                return;
            }

            if (Here.Furniture == null)
            {
                Here.Furniture = new List<FurnitureItem>();
            }

            Here.Furniture.Add(Box);
            LifeSimMap Map = Here.MapMapContainer == null ? null : Here.MapMapContainer.ActiveMap;
            if (Map != null && Map.Furniture != null && !object.ReferenceEquals(Map.Furniture, Here.Furniture))
            {
                Map.Furniture.Add(Box);
            }

            BuildingZones.Refresh(Here);
        }

        public static void Keep(MapInfo Here, BuildingItem Box)
        {
            if (Here == null || Box == null)
            {
                return;
            }

            if (Here.Buildings == null)
            {
                Here.Buildings = new List<BuildingItem>();
            }

            Here.Buildings.Add(Box);
            LifeSimMap Map = Here.MapMapContainer == null ? null : Here.MapMapContainer.ActiveMap;
            if (Map != null && Map.Buildings != null && !object.ReferenceEquals(Map.Buildings, Here.Buildings))
            {
                Map.Buildings.Add(Box);
            }

            BuildingZones.Refresh(Here);
        }

        public static void Drop(MapInfo Here, FurnitureItem Box)
        {
            if (Here == null || Box == null || Here.Furniture == null)
            {
                return;
            }

            Here.Furniture.Remove(Box);
            LifeSimMap Map = Here.MapMapContainer == null ? null : Here.MapMapContainer.ActiveMap;
            if (Map != null && Map.Furniture != null && !object.ReferenceEquals(Map.Furniture, Here.Furniture))
            {
                Map.Furniture.Remove(Box);
            }

            BuildingZones.Refresh(Here);
        }

        public static void Drop(MapInfo Here, BuildingItem Box)
        {
            if (Here == null || Box == null || Here.Buildings == null)
            {
                return;
            }

            Here.Buildings.Remove(Box);
            LifeSimMap Map = Here.MapMapContainer == null ? null : Here.MapMapContainer.ActiveMap;
            if (Map != null && Map.Buildings != null && !object.ReferenceEquals(Map.Buildings, Here.Buildings))
            {
                Map.Buildings.Remove(Box);
            }

            BuildingZones.Refresh(Here);
        }
    }

    public static class CursorGround
    {
        public static Vector3 From(Vector3 Cursor)
        {
            return new Vector3(Cursor.X, Cursor.Z, Cursor.Y);
        }
    }

    public static class PlacedBoxDraw
    {
        static BasicEffect Effect;
        static VertexBuffer Vertices;
        static IndexBuffer Indices;
        static GraphicsDevice Device;
        static bool Lit;
        static float Soak;

        public static void All(GraphicsDevice GraphicsDevice, IList<FurnitureItem> Furniture, IList<BuildingItem> Buildings, Matrix View, Matrix Projection, int ShownFloor, bool HideAbove)
        {
            All(GraphicsDevice, Furniture, Buildings, View, Projection, ShownFloor, HideAbove, Vector3.Zero);
        }

        public static void All(GraphicsDevice GraphicsDevice, IList<FurnitureItem> Furniture, IList<BuildingItem> Buildings, Matrix View, Matrix Projection, int ShownFloor, bool HideAbove, Vector3 Shift)
        {
            All(GraphicsDevice, Furniture, Buildings, View, Projection, ShownFloor, HideAbove, Shift, null);
        }

        public static void All(GraphicsDevice GraphicsDevice, IList<FurnitureItem> Furniture, IList<BuildingItem> Buildings, Matrix View, Matrix Projection, int ShownFloor, bool HideAbove, Vector3 Shift, HeightField Ground)
        {
            All(GraphicsDevice, Furniture, Buildings, View, Projection, ShownFloor, HideAbove, Shift, Ground, Vector3.Zero, false);
        }

        public static void All(GraphicsDevice GraphicsDevice, IList<FurnitureItem> Furniture, IList<BuildingItem> Buildings, Matrix View, Matrix Projection, int ShownFloor, bool HideAbove, Vector3 Shift, HeightField Ground, Vector3 Focus, bool HasFocus)
        {
            if (GraphicsDevice == null)
            {
                return;
            }

            if ((Furniture == null || Furniture.Count == 0) && (Buildings == null || Buildings.Count == 0))
            {
                return;
            }

            Ensure(GraphicsDevice);
            Brighten();
            Device.BlendState = BlendState.Opaque;
            Device.DepthStencilState = DepthStencilState.Default;
            Matrix Inverse = Matrix.Invert(View);
            Vector3 Eye = Inverse.Translation;
            Vector3 Forward = Inverse.Forward;
            Vector2 Look = new Vector2(Forward.X, Forward.Z);
            if (Look.LengthSquared() > 0.0001f)
            {
                Look.Normalize();
            }

            if (Furniture != null)
            {
                for (int i = 0; i < Furniture.Count; i++)
                {
                    if (Furniture[i].Brushes != null && Furniture[i].Brushes.Count > 0)
                    {
                        PropBrushMesh.Draw(GraphicsDevice, Furniture[i], View, Projection, Shift);
                        continue;
                    }

                    Sit(Furniture[i], Ground);
                    DrawPiece(Furniture[i].Body, Furniture[i].Swatch, Tint(Furniture[i]), false, false, Furniture[i].Model, Furniture[i].ModelMeters, Furniture[i].ModelShift, ShownFloor, HideAbove, Shift, Eye, Forward, Look, Focus, HasFocus, View, Projection);
                }
            }

            AimPaths(Buildings);
            if (Buildings != null)
            {
                for (int i = 0; i < Buildings.Count; i++)
                {
                    BuildingItem Box = Buildings[i];
                    bool Shell = Box.Kind == BuildingKind.Wall || Box.Kind == BuildingKind.Door || Box.Kind == BuildingKind.Window;
                    bool Opening = Box.Kind == BuildingKind.Door || Box.Kind == BuildingKind.Window;
                    if (!Shell && HidesHouse(Box, Shift, Eye, Forward, Focus, HasFocus))
                    {
                        continue;
                    }

                    DrawPiece(Box.Body, Box.Swatch, Tint(Box), Shell, Opening, Box.Model, Box.ModelMeters, Box.ModelShift, ShownFloor, HideAbove, Shift, Eye, Forward, Look, Focus, HasFocus, View, Projection);
                }
            }
        }

        static void AimPaths(IList<BuildingItem> Buildings)
        {
            if (Buildings == null)
            {
                return;
            }

            for (int i = 0; i < Buildings.Count; i++)
            {
                if (!IsPath(Buildings[i]))
                {
                    continue;
                }

                Vector2 Here = new Vector2(Buildings[i].Position.X, Buildings[i].Position.Y);
                Vector2 Aim = Vector2.Zero;
                float Best = 96f * 96f;
                for (int j = 0; j < Buildings.Count; j++)
                {
                    if (i == j || !IsPath(Buildings[j]))
                    {
                        continue;
                    }

                    Vector2 Delta = new Vector2(Buildings[j].Position.X, Buildings[j].Position.Y) - Here;
                    float Distance = Delta.LengthSquared();
                    if (Distance < 4f || Distance > Best)
                    {
                        continue;
                    }

                    Best = Distance;
                    Aim = Delta;
                }

                if (Aim.LengthSquared() < 4f)
                {
                    continue;
                }

                Buildings[i].Angle = (float)System.Math.Atan2(Aim.X, -Aim.Y);
                Buildings[i].Height = (float)System.Math.Sqrt(Best) * 1.08f;
            }
        }

        static bool IsPath(BuildingItem Box)
        {
            return Box != null && !string.IsNullOrEmpty(Box.Model) && Box.Model.IndexOf("Path_Straight", System.StringComparison.OrdinalIgnoreCase) >= 0;
        }

        static void Sit(FurnitureItem Box, HeightField Ground)
        {
            if (Box == null || Ground == null || Box.Floor > 0)
            {
                return;
            }

            if (System.Math.Abs(Box.Position.Z - BuildFloor.Surface(0)) > 1f)
            {
                return;
            }

            float Height;
            float SlopeX;
            float SlopeY;
            if (!Ground.Sample(Box.Position.X, Box.Position.Y, out Height, out SlopeX, out SlopeY))
            {
                return;
            }

            if (Height <= Box.Position.Z + 0.5f)
            {
                return;
            }

            Box.Position = new Vector3(Box.Position.X, Box.Position.Y, Height);
        }

        static void DrawPiece(BoxVolume Box, Vector3 Swatch, Vector3 Ink, bool Shell, bool Opening, string Model, float ModelMeters, Vector3 ModelShift, int ShownFloor, bool HideAbove, Vector3 Shift, Vector3 Eye, Vector3 Forward, Vector2 Look, Vector3 Focus, bool HasFocus, Matrix View, Matrix Projection)
        {
            if (HideAbove && Box.Floor > ShownFloor)
            {
                return;
            }

            float DrawnWidth = Box.Width;
            float DrawnHeight = Box.Height;
            float Base = Box.Position.Z;
            if (HideAbove && Shell && Box.Width > BuildFloor.Thickness * 2f)
            {
                DrawnWidth += BuildFloor.Thickness;
            }

            if (!HideAbove && Shell && BlocksView(Box, Shift, Eye, Focus, HasFocus, Look))
            {
                return;
            }

            if (HideAbove && Shell && InFront(Box, Shift, Eye, Forward, Look))
            {
                if (Opening)
                {
                    return;
                }

                float StubTop = BuildFloor.Surface(Box.Floor) + 14f;
                if (Base >= StubTop - 0.5f)
                {
                    return;
                }

                DrawnHeight = System.Math.Min(Base + Box.Height, StubTop) - Base;
                if (DrawnHeight < 1f)
                {
                    return;
                }

                Vector3 StubCenter = new Vector3(Box.Position.X, Base + DrawnHeight * 0.5f, Box.Position.Y) + Shift;
                Vector3 StubInk = Swatch.LengthSquared() > 0.01f ? Swatch : new Vector3(0.45f, 0.45f, 0.5f);
                Solid(StubCenter, DrawnWidth, DrawnHeight, Box.Length, StubInk, Box.Angle, 1f, View, Projection);
                return;
            }

            Soak = 0f;
            if (!Shell && Climate.Now.WetGround > 0.04f)
            {
                float WorldX = Box.Position.X + Shift.X;
                float WorldZ = Box.Position.Y + Shift.Z;
                if (!WeatherDraw.CeilingAbove(WorldX, WorldZ, Box.Floor))
                {
                    Soak = Climate.Now.WetGround * ForestScatter.Shade(WorldX, WorldZ);
                }
            }

            if (Soak > 0.04f)
            {
                Ink = Vector3.Lerp(Ink, Ink * new Vector3(0.42f, 0.48f, 0.52f), Soak);
            }

            if (Prop(Model, ModelMeters, ModelShift, Box, View, Projection, Shift) || Mesh(Model, ModelMeters, ModelShift, Box, View, Projection, Shift))
            {
                return;
            }

            Vector3 Center = new Vector3(Box.Position.X, Base + DrawnHeight * 0.5f, Box.Position.Y) + Shift;
            Solid(Center, DrawnWidth, DrawnHeight, Box.Length, Ink, Box.Angle, 1f, View, Projection);
        }

        static bool BlocksView(BoxVolume Box, Vector3 Shift, Vector3 Eye, Vector3 Focus, bool HasFocus, Vector2 Look)
        {
            if (!HasFocus || Look.LengthSquared() < 0.0001f)
            {
                return false;
            }

            float C = (float)System.Math.Cos(Box.Angle);
            float S = (float)System.Math.Sin(Box.Angle);
            Vector2 Normal = new Vector2(-S, C);
            float Face = System.Math.Abs(Normal.X * Look.X + Normal.Y * Look.Y);
            if (Face < 0.35f)
            {
                return false;
            }

            Vector3 From = LocalOf(Eye, Shift);
            Vector3 To = LocalOf(Focus, Shift);
            return InsideHull(Box, From) || Box.Hits(From, To);
        }

        static bool HidesHouse(BuildingItem Box, Vector3 Shift, Vector3 Eye, Vector3 Forward, Vector3 Focus, bool HasFocus)
        {
            BoxVolume Hull;
            if (Box == null || !Box.TryHull(out Hull))
            {
                return false;
            }

            Vector3 Aim = Focus;
            if (!HasFocus)
            {
                if (Forward.Y > -0.05f)
                {
                    return false;
                }

                float Ground = Hull.Position.Z + Shift.Y + 8f;
                float T = (Ground - Eye.Y) / Forward.Y;
                if (T < 0f)
                {
                    return false;
                }

                Aim = Eye + Forward * T;
            }

            Vector3 From = LocalOf(Eye, Shift);
            Vector3 To = LocalOf(Aim, Shift);
            if (InsideHull(Hull, From) || InsideHull(Hull, To))
            {
                return true;
            }

            return Hull.Hits(From, To);
        }

        static Vector3 LocalOf(Vector3 Render, Vector3 Shift)
        {
            return new Vector3(Render.X - Shift.X, Render.Z - Shift.Z, Render.Y - Shift.Y);
        }

        static bool InsideHull(BoxVolume Hull, Vector3 Point)
        {
            if (Point.Z < Hull.Position.Z - 1f || Point.Z > Hull.Position.Z + Hull.Height + 1f)
            {
                return false;
            }

            return Hull.FootprintContains(Point.X, Point.Y);
        }

        static bool InFront(BoxVolume Box, Vector3 Shift, Vector3 Eye, Vector3 Forward, Vector2 Look)
        {
            if (Look.LengthSquared() < 0.0001f || Forward.Y > -0.05f)
            {
                return false;
            }

            float C = (float)System.Math.Cos(Box.Angle);
            float S = (float)System.Math.Sin(Box.Angle);
            Vector2 Normal = new Vector2(-S, C);
            float Face = System.Math.Abs(Normal.X * Look.X + Normal.Y * Look.Y);
            if (Face < 0.45f)
            {
                return false;
            }

            float Ground = Box.Position.Z + Shift.Y;
            float T = (Ground - Eye.Y) / Forward.Y;
            if (T < 0f)
            {
                return false;
            }

            Vector2 Wall = new Vector2(Box.Position.X + Shift.X, Box.Position.Y + Shift.Z);
            Vector2 Cam = new Vector2(Eye.X, Eye.Z);
            Vector2 Focus = new Vector2(Eye.X + Forward.X * T, Eye.Z + Forward.Z * T);
            float CamSide = (Cam.X - Wall.X) * Normal.X + (Cam.Y - Wall.Y) * Normal.Y;
            float FocusSide = (Focus.X - Wall.X) * Normal.X + (Focus.Y - Wall.Y) * Normal.Y;
            return CamSide * FocusSide < 0f;
        }

        public static void Show(GraphicsDevice GraphicsDevice, FurnitureItem Box, Matrix View, Matrix Projection)
        {
            if (GraphicsDevice == null || Box == null)
            {
                return;
            }

            Ensure(GraphicsDevice);
            Device.BlendState = BlendState.Opaque;
            Device.DepthStencilState = DepthStencilState.Default;
            Device.RasterizerState = RasterizerState.CullCounterClockwise;
            if (Prop(Box.Model, Box.ModelMeters, Box.ModelShift, Box.Body, View, Projection, Vector3.Zero) || Mesh(Box.Model, Box.ModelMeters, Box.ModelShift, Box.Body, View, Projection, Vector3.Zero))
            {
                return;
            }

            Vector3 Center = new Vector3(Box.Position.X, Box.Position.Z + Box.Height * 0.5f, Box.Position.Y);
            Solid(Center, Box.Width, Box.Height, Box.Length, Tint(Box), Box.Angle, 1f, View, Projection);
        }

        public static void Show(GraphicsDevice GraphicsDevice, BuildingItem Box, Matrix View, Matrix Projection)
        {
            if (GraphicsDevice == null || Box == null)
            {
                return;
            }

            Ensure(GraphicsDevice);
            Device.BlendState = BlendState.Opaque;
            Device.DepthStencilState = DepthStencilState.Default;
            Device.RasterizerState = RasterizerState.CullCounterClockwise;
            if (Prop(Box.Model, Box.ModelMeters, Box.ModelShift, Box.Body, View, Projection, Vector3.Zero))
            {
                return;
            }

            Vector3 Center = new Vector3(Box.Position.X, Box.Position.Z + Box.Height * 0.5f, Box.Position.Y);
            Solid(Center, Box.Width, Box.Height, Box.Length, Tint(Box), Box.Angle, 1f, View, Projection);
        }

        public static void Ghost(GraphicsDevice GraphicsDevice, FurnitureItem Box, bool Valid, Matrix View, Matrix Projection, Vector3 Shift)
        {
            Ghost(GraphicsDevice, Box == null ? new BoxVolume() : Box.Body, Box == null ? Vector3.Zero : Tint(Box), Box == null ? null : Box.Model, Box == null ? 0f : Box.ModelMeters, Box == null ? Vector3.Zero : Box.ModelShift, Box != null, Valid, View, Projection, Shift);
        }

        public static void Ghost(GraphicsDevice GraphicsDevice, BuildingItem Box, bool Valid, Matrix View, Matrix Projection, Vector3 Shift)
        {
            Ghost(GraphicsDevice, Box == null ? new BoxVolume() : Box.Body, Box == null ? Vector3.Zero : Tint(Box), Box == null ? null : Box.Model, Box == null ? 0f : Box.ModelMeters, Box == null ? Vector3.Zero : Box.ModelShift, Box != null, Valid, View, Projection, Shift);
        }

        static void Ghost(GraphicsDevice GraphicsDevice, BoxVolume Box, Vector3 Ink, string Model, float ModelMeters, Vector3 ModelShift, bool Present, bool Valid, Matrix View, Matrix Projection, Vector3 Shift)
        {
            if (GraphicsDevice == null || !Present)
            {
                return;
            }

            Ensure(GraphicsDevice);
            Device.BlendState = BlendState.AlphaBlend;
            Device.DepthStencilState = DepthStencilState.None;
            Device.RasterizerState = RasterizerState.CullNone;
            if (Valid && (Prop(Model, ModelMeters, ModelShift, Box, View, Projection, Shift) || Mesh(Model, ModelMeters, ModelShift, Box, View, Projection, Shift)))
            {
                Device.BlendState = BlendState.Opaque;
                Device.DepthStencilState = DepthStencilState.Default;
                return;
            }

            Vector3 Center = new Vector3(Box.Position.X, Box.Position.Z + Box.Height * 0.5f + 0.4f, Box.Position.Y) + Shift;
            Vector3 Color = Valid ? Ink : new Vector3(0.85f, 0.15f, 0.15f);
            Solid(Center, Box.Width, Box.Height, Box.Length, Color, Box.Angle, 0.55f, View, Projection);
            Device.BlendState = BlendState.Opaque;
            Device.DepthStencilState = DepthStencilState.Default;
            Device.RasterizerState = RasterizerState.CullCounterClockwise;
        }

        public static void Highlight(GraphicsDevice GraphicsDevice, IList<FurnitureItem> Boxes, Matrix View, Matrix Projection, Vector3 Shift)
        {
            if (GraphicsDevice == null || Boxes == null || Boxes.Count == 0)
            {
                return;
            }

            Ensure(GraphicsDevice);
            Device.BlendState = BlendState.AlphaBlend;
            Device.DepthStencilState = DepthStencilState.None;
            Device.RasterizerState = RasterizerState.CullNone;
            Vector3 Pale = new Vector3(0.82f, 0.9f, 1f);
            for (int i = 0; i < Boxes.Count; i++)
            {
                FurnitureItem Box = Boxes[i];
                if (Box == null)
                {
                    continue;
                }

                Vector3 Center = new Vector3(Box.Position.X, Box.Position.Z + Box.Height * 0.5f, Box.Position.Y) + Shift;
                Solid(Center, Box.Width + 3f, Box.Height + 3f, Box.Length + 3f, Pale, Box.Angle, 0.4f, View, Projection);
            }

            Device.BlendState = BlendState.Opaque;
            Device.DepthStencilState = DepthStencilState.Default;
            Device.RasterizerState = RasterizerState.CullCounterClockwise;
        }

        public static void UseMarks(GraphicsDevice GraphicsDevice, IList<Vector3> Tiles, float Tile, Matrix View, Matrix Projection, Vector3 Shift)
        {
            if (GraphicsDevice == null || Tiles == null || Tiles.Count == 0 || Tile <= 0f)
            {
                return;
            }

            Ensure(GraphicsDevice);
            Device.BlendState = BlendState.AlphaBlend;
            Device.DepthStencilState = DepthStencilState.None;
            Device.RasterizerState = RasterizerState.CullNone;
            float Span = Tile * 0.88f;
            Vector3 Ink = new Vector3(0.35f, 0.78f, 0.95f);
            for (int i = 0; i < Tiles.Count; i++)
            {
                Vector3 Mark = Tiles[i];
                Vector3 Center = new Vector3(Mark.X, Mark.Z + 1.2f, Mark.Y) + Shift;
                Solid(Center, Span, 2f, Span, Ink, 0f, 0.4f, View, Projection);
            }

            Device.BlendState = BlendState.Opaque;
            Device.DepthStencilState = DepthStencilState.Default;
            Device.RasterizerState = RasterizerState.CullCounterClockwise;
        }

        public static void Marker(GraphicsDevice GraphicsDevice, Vector3 Feet, Matrix View, Matrix Projection)
        {
            if (GraphicsDevice == null)
            {
                return;
            }

            Ensure(GraphicsDevice);
            Vector3 Center = new Vector3(Feet.X, Feet.Z + 16f, Feet.Y);
            Solid(Center, 14f, 32f, 14f, new Vector3(0.25f, 0.75f, 0.35f), 0f, 1f, View, Projection);
        }

        static readonly Dictionary<string, GltfModel> Meshes = new Dictionary<string, GltfModel>();
        static readonly Dictionary<string, FbxModel> Props = new Dictionary<string, FbxModel>();
        static readonly HashSet<string> Unreadable = new HashSet<string>();

        public static void Dungeon(GraphicsDevice GraphicsDevice, DungeonFloor Floor, Matrix View, Matrix Projection, Vector3 Eye, Vector3 Focus, DungeonRun Run, int FloorIndex)
        {
            if (GraphicsDevice == null || Floor == null || Floor.Cells == null)
            {
                return;
            }

            Ensure(GraphicsDevice);
            Device.BlendState = BlendState.Opaque;
            Device.DepthStencilState = DepthStencilState.Default;
            Device.Clear(ClearOptions.DepthBuffer, Color.Transparent, 1f, 0);
            string Root = "Content/Life Sim/Assets/Models/Dungeon/";
            for (int i = 0; i < Floor.Cells.Length; i++)
            {
                DungeonCell Cell = Floor.Cells[i];
                if (!Cell.Walkable())
                {
                    continue;
                }

                Vector3 Feet = DungeonView.Spot(Cell.X, Cell.Y, Cell.Height);
                bool Portal = Cell.Role == DungeonRole.StairUp || Cell.Role == DungeonRole.StairDown;
                int RiseX = 0;
                int RiseY = 0;
                float Rise = 0f;
                float Angle = 0f;
                bool Step = Cell.Role == DungeonRole.Step && DungeonRun.StairAim(Floor, Cell, true, out RiseX, out RiseY, out Rise, out Angle);
                if (!Step)
                {
                    Kit(Root + "floor_tile_small.gltf", new Vector3(16f), Vector3.Zero, Feet, 0f, View, Projection);
                }

                if (Step)
                {
                    float ScaleY = Rise / 4f;
                    Kit(Root + "stairs_modular_center.gltf", new Vector3(16f, ScaleY, 8.25f), new Vector3(0f, 0f, -16f / 8.25f), Feet, Angle, View, Projection);
                }
                else if (Portal && DungeonRun.StairAim(Floor, Cell, false, out RiseX, out RiseY, out Rise, out Angle))
                {
                    Kit(Root + "stairs_modular_center.gltf", new Vector3(16f, 2f, 8.25f), new Vector3(0f, 0f, -16f / 8.25f), Feet, Angle, View, Projection);
                }

                Lip(Floor, Cell, 0, -1, 0f, Eye, Focus, Root, View, Projection);
                Lip(Floor, Cell, 0, 1, MathHelper.Pi, Eye, Focus, Root, View, Projection);
                Lip(Floor, Cell, 1, 0, -MathHelper.PiOver2, Eye, Focus, Root, View, Projection);
                Lip(Floor, Cell, -1, 0, MathHelper.PiOver2, Eye, Focus, Root, View, Projection);
            }

            if (Run == null)
            {
                return;
            }

            for (int i = 0; i < Floor.Spawns.Count; i++)
            {
                DungeonSpawn Spawn = Floor.Spawns[i];
                if (Spawn.HP <= 0)
                {
                    continue;
                }

                string Model = Spawn.Model;
                if (Spawn.Kind == DungeonPersonKind.Monster && string.IsNullOrEmpty(Model))
                {
                    Model = i % 2 == 0 ? "Bad Guy 1.glb" : "Bad Guy 2.glb";
                }

                DungeonCell UnderSpawn = Floor.At(Spawn.X, Spawn.Y);
                ShowBody(Spawn.Body, DungeonView.Spot(Spawn.X, Spawn.Y, UnderSpawn == null ? 0 : UnderSpawn.Height), 0f, View, Projection, Model);
            }

            foreach (string Id in Run.FloorOf.Keys)
            {
                int Here;
                if (!Run.FloorOf.TryGetValue(Id, out Here) || Here != FloorIndex)
                {
                    continue;
                }

                PartyMember Who = Run.Party.Find(Id);
                Run.Settle(Id);
                float X;
                float Y;
                float Yaw;
                if (Who == null || Who.Body == null || !Run.PosX.TryGetValue(Id, out X) || !Run.PosY.TryGetValue(Id, out Y))
                {
                    continue;
                }

                float Ground;
                if (!Run.Altitude.TryGetValue(Id, out Ground))
                {
                    Ground = DungeonRun.GroundAt(Floor, X, Y);
                }

                if (!Run.Facing.TryGetValue(Id, out Yaw))
                {
                    Yaw = 0f;
                }

                ShowBody(Who.Body, new Vector3(X, Ground, Y), Yaw, View, Projection, null);
            }
        }

        static void Lip(DungeonFloor Floor, DungeonCell Cell, int DX, int DY, float Angle, Vector3 Eye, Vector3 Focus, string Root, Matrix View, Matrix Projection)
        {
            DungeonCell Next = Floor.At(Cell.X + DX, Cell.Y + DY);
            if (Next != null && Next.Walkable())
            {
                return;
            }

            Vector3 Feet = DungeonView.Spot(Cell.X, Cell.Y, Cell.Height) + new Vector3(DX * 16f, 0f, DY * 16f);
            if (Hides(Feet, Eye, Focus))
            {
                return;
            }

            Kit(Root + "wall.gltf", new Vector3(8f, 10f, 8f), Vector3.Zero, Feet, Angle, View, Projection);
        }

        static bool Hides(Vector3 Feet, Vector3 Eye, Vector3 Focus)
        {
            Vector2 ToEye = new Vector2(Eye.X - Focus.X, Eye.Z - Focus.Z);
            Vector2 ToWall = new Vector2(Feet.X - Focus.X, Feet.Z - Focus.Z);
            if (ToEye.LengthSquared() < 1f || Vector2.Dot(ToEye, ToWall) <= 0f)
            {
                return false;
            }

            return ToWall.LengthSquared() < 40f * 40f;
        }

        static void ShowBody(PlayerCharacter Who, Vector3 Feet, float Yaw, Matrix View, Matrix Projection, string ModelFile)
        {
            if (Who == null)
            {
                return;
            }

            if (Who.BodyModel == null && Who.Unit3DSprite == null && Who.Unit3DModel == null && GameScreen.GraphicsDevice != null)
            {
                string FileName = string.IsNullOrEmpty(ModelFile) ? "Default.glb" : ModelFile;
                string Glb = Path.Combine("Content", "Life Sim", "Characters", "Models", FileName);
                if (File.Exists(Glb))
                {
                    try
                    {
                        Who.WearBody(GameScreen.GraphicsDevice, Glb);
                        if (!string.IsNullOrEmpty(ModelFile))
                        {
                            Who.BodyScale = 52f;
                        }
                    }
                    catch (System.Exception)
                    {
                    }
                }
            }

            Who.Position = new Vector3(Feet.X, Feet.Z, Feet.Y);
            Who.RotationMatrix = Matrix.CreateFromAxisAngle(Vector3.UnitY, Yaw);
            Who.Draw3DOnMap(Device, View, Projection, Vector3.Zero);
        }

        public static void Update(GameTime GameTime)
        {
            if (GameTime == null)
            {
                return;
            }

            foreach (GltfModel Model in Meshes.Values)
            {
                Model.Update(GameTime);
            }

            foreach (FbxModel Model in Props.Values)
            {
                Model.Update(GameTime);
            }
        }

        static bool Prop(string ModelPath, float ModelMeters, Vector3 ModelShift, BoxVolume Box, Matrix View, Matrix Projection, Vector3 Shift)
        {
            if (string.IsNullOrEmpty(ModelPath) || ModelMeters <= 0f || Device == null || !ModelPath.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (Unreadable.Contains(ModelPath))
            {
                return false;
            }

            FbxModel Model;
            if (!Props.TryGetValue(ModelPath, out Model))
            {
                if (!File.Exists(ModelPath))
                {
                    Unreadable.Add(ModelPath);
                    return false;
                }

                try
                {
                    Model = new FbxModel(Device, ModelPath);
                }
                catch (System.Exception)
                {
                    Unreadable.Add(ModelPath);
                    return false;
                }

                Props.Add(ModelPath, Model);
            }

            Vector3 Fit = new Vector3(Box.Height / ModelMeters);
            if (ModelPath.IndexOf("Path_Straight", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                float Span = Box.Height > 12f ? Box.Height : 48f;
                Fit = new Vector3(36f / 0.50f, 28f, Span / 0.98f);
            }
            else if (ModelPath.IndexOf("Medieval Village Pack", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                bool PropPiece = ModelPath.IndexOf("Props", System.StringComparison.OrdinalIgnoreCase) >= 0;
                Fit *= PropPiece ? 2.5f : 5f;
            }

            Vector3 Feet = new Vector3(Box.Position.X, Box.Position.Z, Box.Position.Y) + Shift;
            Matrix World = Matrix.CreateTranslation(ModelShift) * Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateScale(Fit) * Matrix.CreateRotationY(-Box.Angle) * Matrix.CreateTranslation(Feet);
            bool Wet = Soak > 0.04f;
            if (Wet)
            {
                Vector3 Dim = Vector3.Lerp(Vector3.One, new Vector3(0.42f, 0.48f, 0.52f), Soak);
                FbxModel.UseSun(SkyDraw.KeyDirection, SkyDraw.KeyColor * Dim, SkyDraw.SkyAmbient * Dim, SkyDraw.FillDirection, SkyDraw.FillColor * Dim);
            }

            Model.Draw(View, Projection, World);
            if (Wet)
            {
                FbxModel.UseSun(SkyDraw.KeyDirection, SkyDraw.KeyColor, SkyDraw.SkyAmbient, SkyDraw.FillDirection, SkyDraw.FillColor);
            }

            return true;
        }

        static bool Mesh(string ModelPath, float ModelMeters, Vector3 ModelShift, BoxVolume Box, Matrix View, Matrix Projection, Vector3 Shift)
        {
            if (string.IsNullOrEmpty(ModelPath) || ModelMeters <= 0f || Device == null || ModelPath.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (Unreadable.Contains(ModelPath))
            {
                return false;
            }

            GltfModel Model;
            if (!Meshes.TryGetValue(ModelPath, out Model))
            {
                if (!File.Exists(ModelPath))
                {
                    Unreadable.Add(ModelPath);
                    return false;
                }

                try
                {
                    Model = new GltfModel(Device, ModelPath, false);
                    Model.CharacterDraw = true;
                    Model.Light = ModelLight.Alcyon;
                }
                catch (System.Exception)
                {
                    Unreadable.Add(ModelPath);
                    return false;
                }

                Meshes.Add(ModelPath, Model);
            }

            float Scale = Box.Height / ModelMeters;
            Vector3 Feet = new Vector3(Box.Position.X, Box.Position.Z, Box.Position.Y) + Shift;
            return Drawn(Model, new Vector3(Scale), ModelShift, Feet, Box.Angle, View, Projection);
        }

        static bool Kit(string ModelPath, Vector3 Scale, Vector3 ModelShift, Vector3 Feet, float Angle, Matrix View, Matrix Projection)
        {
            if (string.IsNullOrEmpty(ModelPath) || Scale.X <= 0f || Device == null || Unreadable.Contains(ModelPath))
            {
                return false;
            }

            GltfModel Model;
            if (!Meshes.TryGetValue(ModelPath, out Model))
            {
                if (!File.Exists(ModelPath))
                {
                    Unreadable.Add(ModelPath);
                    return false;
                }

                try
                {
                    Model = new GltfModel(Device, ModelPath, false);
                    Model.CharacterDraw = true;
                    Model.Light = ModelLight.Alcyon;
                }
                catch (System.Exception)
                {
                    Unreadable.Add(ModelPath);
                    return false;
                }

                Meshes.Add(ModelPath, Model);
            }

            return Drawn(Model, Scale, ModelShift, Feet, Angle, View, Projection);
        }

        static bool Drawn(GltfModel Model, Vector3 Scale, Vector3 ModelShift, Vector3 Feet, float Angle, Matrix View, Matrix Projection)
        {
            Matrix World = Matrix.CreateTranslation(ModelShift) * Matrix.CreateScale(Scale) * Matrix.CreateRotationY(-Angle) * Matrix.CreateTranslation(Feet);
            ModelLight Light = SkyDraw.CharacterLight(World);
            if (Soak > 0.04f)
            {
                Vector3 Dim = Vector3.Lerp(Vector3.One, new Vector3(0.42f, 0.48f, 0.52f), Soak);
                Light.Ambient *= Dim;
                Light.Color *= Dim;
            }

            Model.Light = Light;
            Model.Draw(View, Projection, World);
            return true;
        }

        static void Solid(Vector3 Center, float Width, float Height, float Length, Vector3 Tint, float Angle, float Alpha, Matrix View, Matrix Projection)
        {
            Effect.View = View;
            Effect.Projection = Projection;
            Effect.Alpha = Alpha;
            Effect.World = Matrix.CreateScale(System.Math.Max(Width, 0.5f), System.Math.Max(Height, 0.5f), System.Math.Max(Length, 0.5f)) * Matrix.CreateRotationY(-Angle) * Matrix.CreateTranslation(Center);
            Effect.DiffuseColor = Tint;
            Device.SetVertexBuffer(Vertices);
            Device.Indices = Indices;
            foreach (EffectPass Pass in Effect.CurrentTechnique.Passes)
            {
                Pass.Apply();
                Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 12);
            }
        }

        static Vector3 Tint(BuildingItem Box)
        {
            if (Box.Swatch.LengthSquared() > 0.01f)
            {
                return Box.Swatch;
            }

            if (Box.Kind == BuildingKind.Door)
            {
                return new Vector3(0.55f, 0.36f, 0.18f);
            }

            if (Box.Kind == BuildingKind.Window)
            {
                return new Vector3(0.45f, 0.75f, 0.9f);
            }

            if (Box.Kind == BuildingKind.Floor)
            {
                return new Vector3(0.55f, 0.48f, 0.32f);
            }

            if (Box.Kind == BuildingKind.Stairs)
            {
                return new Vector3(0.62f, 0.62f, 0.66f);
            }

            return new Vector3(0.45f, 0.45f, 0.5f);
        }

        static Vector3 Tint(FurnitureItem Box)
        {
            if (Box.Swatch.LengthSquared() > 0.01f)
            {
                return Box.Swatch;
            }

            if (Box.Kind == BoxKind.Long && Box.Upright)
            {
                return new Vector3(0.45f, 0.45f, 0.5f);
            }

            if (Box.IsRope)
            {
                return new Vector3(0.55f, 0.32f, 0.16f);
            }

            if (Box.Kind == BoxKind.Step)
            {
                return new Vector3(0.72f, 0.72f, 0.76f);
            }

            if (Box.Interaction == "Bed")
            {
                return new Vector3(0.45f, 0.32f, 0.7f);
            }

            if (Box.Interaction == "Fridge")
            {
                return new Vector3(0.3f, 0.62f, 0.8f);
            }

            return new Vector3(0.9f, 0.55f, 0.22f);
        }

        static void Ensure(GraphicsDevice GraphicsDevice)
        {
            if (Vertices != null && Device == GraphicsDevice)
            {
                return;
            }

            Device = GraphicsDevice;
            const float Half = 0.5f;
            Vector3[] Corners = new Vector3[]
            {
                new Vector3(-Half, -Half, -Half),
                new Vector3(Half, -Half, -Half),
                new Vector3(Half, Half, -Half),
                new Vector3(-Half, Half, -Half),
                new Vector3(-Half, -Half, Half),
                new Vector3(Half, -Half, Half),
                new Vector3(Half, Half, Half),
                new Vector3(-Half, Half, Half)
            };
            Vector3[] Normals = new Vector3[]
            {
                Vector3.Backward,
                Vector3.Forward,
                Vector3.Left,
                Vector3.Right,
                Vector3.Down,
                Vector3.Up
            };
            int[] Faces = new int[]
            {
                0, 1, 2, 0, 2, 3,
                5, 4, 7, 5, 7, 6,
                4, 0, 3, 4, 3, 7,
                1, 5, 6, 1, 6, 2,
                4, 5, 1, 4, 1, 0,
                3, 2, 6, 3, 6, 7
            };

            VertexPositionNormalTexture[] Data = new VertexPositionNormalTexture[36];
            int[] Index = new int[36];
            for (int Vertex = 0; Vertex < 36; Vertex++)
            {
                Data[Vertex] = new VertexPositionNormalTexture(Corners[Faces[Vertex]], Normals[Vertex / 6], Vector2.Zero);
                Index[Vertex] = Vertex;
            }

            Vertices = new VertexBuffer(GraphicsDevice, typeof(VertexPositionNormalTexture), Data.Length, BufferUsage.WriteOnly);
            Vertices.SetData(Data);
            Indices = new IndexBuffer(GraphicsDevice, IndexElementSize.ThirtyTwoBits, Index.Length, BufferUsage.WriteOnly);
            Indices.SetData(Index);

            Effect = new BasicEffect(GraphicsDevice);
            Effect.LightingEnabled = true;
            Effect.TextureEnabled = false;
            Effect.VertexColorEnabled = false;
            Effect.EnableDefaultLighting();
            Lit = true;
        }

        static void Brighten()
        {
            if (!Lit || Effect == null)
            {
                return;
            }

            Effect.AmbientLightColor = SkyDraw.SkyAmbient;
            Effect.DirectionalLight0.Direction = -SkyDraw.KeyDirection;
            Effect.DirectionalLight0.DiffuseColor = SkyDraw.KeyColor;
            Effect.DirectionalLight1.Direction = -SkyDraw.FillDirection;
            Effect.DirectionalLight1.DiffuseColor = SkyDraw.FillColor;
            Effect.DirectionalLight2.DiffuseColor = SkyDraw.FillColor * 0.35f;
        }
    }
}
