using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class WallPlan
    {
        public static bool Handles(string Item)
        {
            if (Item == "Wall" || Item == "Door" || Item == "Window" || Item == "Floor" || Item == "Stairs")
            {
                return true;
            }

            return WallCatalog.Find(Item) != null;
        }

        public static bool Shows(BuildingItem Box, int Floor, bool HideAbove)
        {
            return Box == null || !HideAbove || Box.Floor <= Floor;
        }

        public static bool IsSpan(string Item)
        {
            if (Item == "Wall")
            {
                return true;
            }

            SurfaceItem Card = WallCatalog.Find(Item);
            return Card != null && Card.Use == SurfaceUse.Wall;
        }

        public static BuildResult Preview(string Item, Vector3 Cursor, Vector3 Anchor, bool HasAnchor, int Floor, bool Grid, float Tile, IList<BuildingItem> Buildings, IList<FurnitureItem> Furniture)
        {
            if (IsSpan(Item) && !HasAnchor)
            {
                BuildResult Post = new BuildResult();
                Post.Valid = true;
                Post.BuildingGhost = PostPiece(BuildFloor.SnapCorner(Cursor, Tile, Grid), Floor);
                Paint(Post.BuildingGhost, Item);
                Post.Reason = "Set the other end";
                return Post;
            }

            return Commit(Item, Cursor, Anchor, HasAnchor, Floor, Grid, Tile, Buildings, Furniture, false);
        }

        public static BuildResult Commit(string Item, Vector3 Cursor, Vector3 Anchor, bool HasAnchor, int Floor, bool Grid, float Tile, IList<BuildingItem> Buildings, IList<FurnitureItem> Furniture)
        {
            return Commit(Item, Cursor, Anchor, HasAnchor, Floor, Grid, Tile, Buildings, Furniture, true);
        }

        public static BuildResult Preview(string Item, Vector3 Cursor, Vector3 Anchor, bool HasAnchor, int Floor, bool Grid, float Tile, IList<BuildingItem> Buildings, IList<FurnitureItem> Furniture, MapInfo Limit)
        {
            BuildResult Shown = Preview(Item, Cursor, Anchor, HasAnchor, Floor, Grid, Tile, Buildings, Furniture);
            return Gate(Shown, Limit, Cursor);
        }

        public static BuildResult Commit(string Item, Vector3 Cursor, Vector3 Anchor, bool HasAnchor, int Floor, bool Grid, float Tile, IList<BuildingItem> Buildings, IList<FurnitureItem> Furniture, MapInfo Limit)
        {
            BuildResult Denied = BuildResult.Outside(Limit, Cursor);
            if (Denied != null)
            {
                BuildResult Shown = Preview(Item, Cursor, Anchor, HasAnchor, Floor, Grid, Tile, Buildings, Furniture);
                if (Shown != null)
                {
                    Denied.BuildingGhost = Shown.BuildingGhost;
                }

                return Denied;
            }

            return Commit(Item, Cursor, Anchor, HasAnchor, Floor, Grid, Tile, Buildings, Furniture);
        }

        static BuildResult Gate(BuildResult Shown, MapInfo Limit, Vector3 Cursor)
        {
            BuildResult Denied = BuildResult.Outside(Limit, Cursor);
            if (Denied == null)
            {
                return Shown;
            }

            if (Shown != null)
            {
                Denied.BuildingGhost = Shown.BuildingGhost;
            }

            return Denied;
        }

        public static BuildingItem MakeWall(Vector3 Anchor, Vector3 End, int Floor, float Tile, bool Grid)
        {
            return MakeWall(Anchor, End, Floor, Tile, Grid, null);
        }

        public static BuildingItem MakeWall(Vector3 Anchor, Vector3 End, int Floor, float Tile, bool Grid, string Texture)
        {
            Vector3 A = BuildFloor.SnapCorner(Anchor, Tile, Grid);
            Vector3 B = BuildFloor.SnapCorner(End, Tile, Grid);
            BuildingItem Wall = Between(A, B, Floor, BuildFloor.Surface(Floor), BuildFloor.Story, BuildingKind.Wall);
            Paint(Wall, Texture);
            return Wall;
        }

        public static BuildingItem Under(IList<BuildingItem> Boxes, Vector3 Ground, int Floor)
        {
            BuildingItem Wall = Nearest(Boxes, Ground, Floor, false);
            if (Wall != null)
            {
                return Wall;
            }

            if (Boxes == null)
            {
                return null;
            }

            for (int i = 0; i < Boxes.Count; i++)
            {
                BuildingItem Box = Boxes[i];
                if (Box.Floor == Floor && Box.Kind == BuildingKind.Floor && Box.FootprintContains(Ground.X, Ground.Y))
                {
                    return Box;
                }
            }

            return null;
        }

        static BuildResult Commit(string Item, Vector3 Cursor, Vector3 Anchor, bool HasAnchor, int Floor, bool Grid, float Tile, IList<BuildingItem> Buildings, IList<FurnitureItem> Furniture, bool Final)
        {
            if (Tile <= 0f)
            {
                Tile = 32f;
            }

            BuildResult Result = new BuildResult();
            SurfaceItem Card = WallCatalog.Find(Item);
            if (IsSpan(Item))
            {
                if (!HasAnchor)
                {
                    Result.Reason = "Set the other end";
                    Result.BuildingGhost = PostPiece(BuildFloor.SnapCorner(Cursor, Tile, Grid), Floor);
                    Paint(Result.BuildingGhost, Item);
                    return Result;
                }

                BuildingItem Wall = MakeWall(Anchor, Cursor, Floor, Tile, Grid, Item);
                Result.BuildingGhost = Wall;
                if (System.Math.Max(Wall.Width, Wall.Length) < Tile * 0.5f)
                {
                    Result.Reason = "Wall is too short";
                    return Result;
                }

                if (OnWall(Wall, Buildings))
                {
                    Result.Reason = "Wall already there";
                    return Result;
                }

                if (CrossesFurniture(Wall, Furniture))
                {
                    Result.Reason = "Wall crosses furniture";
                    return Result;
                }

                Result.Valid = true;
                if (Final)
                {
                    Result.AddBuilding.Add(Wall);
                }

                return Result;
            }

            if (Item == "Door" || Item == "Window" || (Card != null && (Card.Use == SurfaceUse.Door || Card.Use == SurfaceUse.Window)))
            {
                bool Window = Item == "Window" || (Card != null && Card.Use == SurfaceUse.Window);
                BuildingItem Host = Nearest(Buildings, Cursor, Floor, true);
                Result.BuildingGhost = Host == null ? PostPiece(Cursor, Floor) : OpeningPiece(Host, Cursor, Tile, Window, Card);
                if (Host == null)
                {
                    Result.Reason = "Aim at a wall";
                    return Result;
                }

                float Length = CutWidth(Card, Host, Tile, Window);
                bool Exact = Card != null && Card.Span > 0f;
                Vector3 HoleA;
                Vector3 HoleB;
                if (!Opening(Host, Cursor, Length, Exact, out HoleA, out HoleB))
                {
                    Result.Reason = "Wall is too short";
                    return Result;
                }

                Result.Valid = true;
                Result.RemoveBuilding.Add(Host);
                if (Final)
                {
                    Split(Host, Cursor, Tile, Window, Card, Result.AddBuilding);
                }
                else if (Result.BuildingGhost != null)
                {
                    Result.BuildingGhost.Floor = Floor;
                }

                return Result;
            }

            if (Card != null && Card.Use == SurfaceUse.Floor)
            {
                BuildingItem Plate = FloorTile(Cursor, Floor, Grid, Tile, Item);
                Result.BuildingGhost = Plate;
                if (!Supported(Plate, Buildings))
                {
                    Result.Reason = "Needs a floor";
                    return Result;
                }

                if (FloorBlocked(Plate, Buildings))
                {
                    Result.Reason = "Floor already there";
                    return Result;
                }

                Result.Valid = true;
                if (Final)
                {
                    Result.AddBuilding.Add(Plate);
                }

                return Result;
            }

            if (Item == "Floor" || Item == "Stairs")
            {
                return CommitPlate(Item, Cursor, Floor, Grid, Tile, Buildings, Final);
            }

            Result.Reason = "Unknown piece";
            return Result;
        }

        static BuildResult CommitPlate(string Item, Vector3 Cursor, int Floor, bool Grid, float Tile, IList<BuildingItem> Buildings, bool Final)
        {
            BuildResult Result = new BuildResult();
            BuildingItem Plate = MakePlate(Item, Cursor, Floor, Grid, Tile);
            Result.BuildingGhost = Plate;
            if (!Supported(Plate, Buildings))
            {
                Result.Reason = "Needs a floor";
                return Result;
            }

            if (FloorBlocked(Plate, Buildings))
            {
                Result.Reason = "Floor already there";
                return Result;
            }

            Result.Valid = true;
            if (Final)
            {
                Result.AddBuilding.Add(Plate);
            }

            return Result;
        }

        static BuildingItem MakePlate(string Item, Vector3 Cursor, int Floor, bool Grid, float Tile)
        {
            Vector3 At = BuildFloor.SnapCenter(Cursor, Tile, Grid);
            BuildingItem Plate = new BuildingItem();
            Plate.Kind = Item == "Stairs" ? BuildingKind.Stairs : BuildingKind.Floor;
            Plate.Floor = Floor;
            Plate.Position = new Vector3(At.X, At.Y, BuildFloor.Surface(Floor));
            Plate.Interaction = Item;
            Plate.Width = Item == "Stairs" ? Tile * 0.8f : Tile;
            Plate.Length = Item == "Stairs" ? Tile * 2f : Tile;
            Plate.Height = Item == "Stairs" ? BuildFloor.Story : 4f;
            return Plate;
        }

        static bool Supported(BuildingItem Plate, IList<BuildingItem> Buildings)
        {
            if (Plate == null || Plate.Floor <= 0 || Plate.Kind == BuildingKind.Floor)
            {
                return true;
            }

            if (Buildings == null)
            {
                return false;
            }

            for (int i = 0; i < Buildings.Count; i++)
            {
                BuildingItem Box = Buildings[i];
                if (Box.Kind == BuildingKind.Floor && Box.Floor == Plate.Floor && Box.FootprintContains(Plate.Position.X, Plate.Position.Y))
                {
                    return true;
                }
            }

            return false;
        }

        static bool FloorBlocked(BuildingItem Plate, IList<BuildingItem> Buildings)
        {
            if (Plate == null || Buildings == null)
            {
                return false;
            }

            for (int i = 0; i < Buildings.Count; i++)
            {
                BuildingItem Other = Buildings[i];
                if (Other.Kind == BuildingKind.Floor && Other.Floor == Plate.Floor && Plate.Body.FootprintOverlaps(Other.Body))
                {
                    return true;
                }
            }

            return false;
        }

        static bool OnWall(BuildingItem Piece, IList<BuildingItem> Buildings)
        {
            if (Piece == null || Buildings == null || Piece.Kind != BuildingKind.Wall)
            {
                return false;
            }

            for (int i = 0; i < Buildings.Count; i++)
            {
                if (SameRun(Piece, Buildings[i]))
                {
                    return true;
                }
            }

            return false;
        }

        static bool SameRun(BuildingItem Piece, BuildingItem Other)
        {
            if (Other == null || Piece.Floor != Other.Floor)
            {
                return false;
            }

            if (Other.Kind != BuildingKind.Wall && Other.Kind != BuildingKind.Door && Other.Kind != BuildingKind.Window)
            {
                return false;
            }

            float C = (float)System.Math.Cos(Piece.Angle);
            float S = (float)System.Math.Sin(Piece.Angle);
            float Parallel = System.Math.Abs(C * (float)System.Math.Cos(Other.Angle) + S * (float)System.Math.Sin(Other.Angle));
            if (Parallel < 0.92f)
            {
                return false;
            }

            float DX = Other.Position.X - Piece.Position.X;
            float DY = Other.Position.Y - Piece.Position.Y;
            float Across = System.Math.Abs(-DX * S + DY * C);
            if (Across > BuildFloor.Thickness + 0.5f)
            {
                return false;
            }

            float Along = DX * C + DY * S;
            float Overlap = Piece.Width * 0.5f + Other.Width * 0.5f - System.Math.Abs(Along);
            return Overlap > 1f;
        }

        static bool CrossesFurniture(BuildingItem Wall, IList<FurnitureItem> Furniture)
        {
            if (Wall == null || Furniture == null)
            {
                return false;
            }

            for (int i = 0; i < Furniture.Count; i++)
            {
                FurnitureItem Piece = Furniture[i];
                if (Piece != null && VolumesOverlap(Wall.Body, Piece.Body))
                {
                    return true;
                }
            }

            return false;
        }

        static bool VolumesOverlap(BoxVolume A, BoxVolume B)
        {
            if (!A.FootprintOverlaps(B))
            {
                return false;
            }

            float ATop = A.Position.Z + A.Height;
            float BTop = B.Position.Z + B.Height;
            return ATop > B.Position.Z + 0.5f && BTop > A.Position.Z + 0.5f;
        }

        static BuildingItem FloorTile(Vector3 Cursor, int Floor, bool Grid, float Tile, string Texture)
        {
            Vector3 At = BuildFloor.SnapCenter(Cursor, Tile, Grid);
            BuildingItem Box = new BuildingItem();
            Box.Kind = BuildingKind.Floor;
            Box.Floor = Floor;
            Box.Position = new Vector3(At.X, At.Y, BuildFloor.Surface(Floor));
            Box.Width = Tile;
            Box.Length = Tile;
            Box.Height = 4f;
            Paint(Box, Texture);
            return Box;
        }

        static void Paint(BuildingItem Box, string Texture)
        {
            if (Box == null || string.IsNullOrEmpty(Texture) || Texture == "Wall")
            {
                return;
            }

            SurfaceItem Card = WallCatalog.Find(Texture);
            if (Card == null || Card.Use == SurfaceUse.Door || Card.Use == SurfaceUse.Window)
            {
                return;
            }

            Box.Interaction = Card.Name;
            Box.Swatch = Card.Swatch;
            Box.Price = Card.Price;
        }

        static BuildingItem PostPiece(Vector3 At, int Floor)
        {
            BuildingItem Post = new BuildingItem();
            Post.Kind = BuildingKind.Wall;
            Post.Floor = Floor;
            Post.Width = BuildFloor.Thickness;
            Post.Length = BuildFloor.Thickness;
            Post.Height = BuildFloor.Story;
            Post.Position = new Vector3(At.X, At.Y, BuildFloor.Surface(Floor));
            return Post;
        }

        static BuildingItem Nearest(IList<BuildingItem> Buildings, Vector3 Cursor, int Floor, bool SolidOnly)
        {
            BuildingItem Best = null;
            float BestDistance = 16f;
            if (Buildings == null)
            {
                return null;
            }

            for (int i = 0; i < Buildings.Count; i++)
            {
                BuildingItem Box = Buildings[i];
                if (Box.Floor != Floor)
                {
                    continue;
                }

                if (SolidOnly)
                {
                    if (Box.Kind != BuildingKind.Wall || Box.Height < BuildFloor.Story * 0.5f)
                    {
                        continue;
                    }
                }
                else if (Box.Kind != BuildingKind.Wall && Box.Kind != BuildingKind.Door && Box.Kind != BuildingKind.Window)
                {
                    continue;
                }

                float Distance = DistanceToWall(Box, Cursor);
                if (Distance < BestDistance)
                {
                    Best = Box;
                    BestDistance = Distance;
                }
            }

            return Best;
        }

        static float DistanceToWall(BuildingItem Wall, Vector3 Cursor)
        {
            Vector3 Start;
            Vector3 End;
            Ends(Wall, out Start, out End);
            return DistanceToSegment(Start, End, Cursor);
        }

        static void Ends(BuildingItem Wall, out Vector3 Start, out Vector3 End)
        {
            float C = (float)System.Math.Cos(Wall.Angle);
            float S = (float)System.Math.Sin(Wall.Angle);
            Vector3 Dir = new Vector3(C, S, 0f) * (Wall.Width * 0.5f);
            Start = new Vector3(Wall.Position.X, Wall.Position.Y, 0f) - Dir;
            End = new Vector3(Wall.Position.X, Wall.Position.Y, 0f) + Dir;
        }

        static float DistanceToSegment(Vector3 Start, Vector3 End, Vector3 Point)
        {
            float SX = End.X - Start.X;
            float SY = End.Y - Start.Y;
            float Len2 = SX * SX + SY * SY;
            float T = 0f;
            if (Len2 > 0.01f)
            {
                T = ((Point.X - Start.X) * SX + (Point.Y - Start.Y) * SY) / Len2;
                if (T < 0f)
                {
                    T = 0f;
                }
                else if (T > 1f)
                {
                    T = 1f;
                }
            }

            float DX = Start.X + SX * T - Point.X;
            float DY = Start.Y + SY * T - Point.Y;
            return (float)System.Math.Sqrt(DX * DX + DY * DY);
        }

        static float CutWidth(SurfaceItem Card, BuildingItem Host, float Tile, bool Window)
        {
            if (Card == null || Card.Span <= 0f || Card.Rise <= 0f || Host == null)
            {
                return Tile;
            }

            float Height = Window ? Host.Height * Card.Rise / WallCatalog.KitRise : Host.Height;
            return Card.Span / Card.Rise * Height;
        }

        static float CutHeight(SurfaceItem Card, BuildingItem Host, bool Window)
        {
            if (Window)
            {
                if (Card != null && Card.Rise > 0f)
                {
                    return Host.Height * Card.Rise / WallCatalog.KitRise;
                }

                return Host.Height * 0.35f;
            }

            if (Card != null && Card.Rise > 0f)
            {
                return Host.Height;
            }

            return Host.Height * 0.92f;
        }

        static bool Opening(BuildingItem Host, Vector3 Cursor, float Length, bool Exact, out Vector3 A, out Vector3 B)
        {
            Vector3 Start;
            Vector3 End;
            Ends(Host, out Start, out End);
            float DX = End.X - Start.X;
            float DY = End.Y - Start.Y;
            float Span = (float)System.Math.Sqrt(DX * DX + DY * DY);
            A = Start;
            B = End;
            if (Span < 1f || (Exact && Length > Span + 0.01f))
            {
                return false;
            }

            float OpeningLength = Exact ? Length : System.Math.Min(Length, Span);
            float Half = OpeningLength * 0.5f;
            float Along = ((Cursor.X - Start.X) * DX + (Cursor.Y - Start.Y) * DY) / Span;
            if (Along < Half)
            {
                Along = Half;
            }
            else if (Along > Span - Half)
            {
                Along = Span - Half;
            }

            Vector3 Dir = new Vector3(DX / Span, DY / Span, 0f);
            A = Start + Dir * (Along - Half);
            B = Start + Dir * (Along + Half);
            return true;
        }

        static BuildingItem OpeningPiece(BuildingItem Host, Vector3 Cursor, float Tile, bool Window, SurfaceItem Card)
        {
            Vector3 A;
            Vector3 B;
            float Length = CutWidth(Card, Host, Tile, Window);
            bool Exact = Card != null && Card.Span > 0f;
            if (!Opening(Host, Cursor, Length, Exact, out A, out B))
            {
                return PostPiece(Cursor, Host.Floor);
            }

            float Rise = CutHeight(Card, Host, Window);
            float Base = Window ? SillHeight(Card, Host, Rise) : 0f;
            BuildingKind OpeningKind = Window ? BuildingKind.Window : BuildingKind.Door;
            BuildingItem Piece = Between(A, B, Host.Floor, Host.Position.Z + Base, Rise, OpeningKind);
            Piece.Interaction = Window ? "Window" : "Door";
            Stamp(Piece, Card);
            return Piece;
        }

        static float SillHeight(SurfaceItem Card, BuildingItem Host, float Rise)
        {
            float Sill = Host.Height * 0.35f;
            if (Card != null && Card.Rise > 0f && Sill + Rise > Host.Height - 4f)
            {
                Sill = System.Math.Max(0f, Host.Height - Rise - 4f);
            }

            return Sill;
        }

        static void Split(BuildingItem Host, Vector3 Cursor, float Tile, bool Window, SurfaceItem Card, List<BuildingItem> Add)
        {
            Vector3 Start;
            Vector3 End;
            Ends(Host, out Start, out End);
            Vector3 A;
            Vector3 B;
            float Length = CutWidth(Card, Host, Tile, Window);
            bool Exact = Card != null && Card.Span > 0f;
            if (!Opening(Host, Cursor, Length, Exact, out A, out B))
            {
                return;
            }

            AddPiece(Carry(Host, Between(Start, A, Host.Floor, Host.Position.Z, Host.Height, BuildingKind.Wall)), Add);
            AddPiece(Carry(Host, Between(B, End, Host.Floor, Host.Position.Z, Host.Height, BuildingKind.Wall)), Add);
            if (Window)
            {
                float Rise = CutHeight(Card, Host, true);
                float Sill = SillHeight(Card, Host, Rise);
                Add.Add(Carry(Host, Between(A, B, Host.Floor, Host.Position.Z, Sill, BuildingKind.Wall)));
                float Header = Host.Height - Sill - Rise;
                if (Header > 1f)
                {
                    Add.Add(Carry(Host, Between(A, B, Host.Floor, Host.Position.Z + Sill + Rise, Header, BuildingKind.Wall)));
                }

                BuildingItem Glass = Between(A, B, Host.Floor, Host.Position.Z + Sill, Rise, BuildingKind.Window);
                Glass.Interaction = "Window";
                Stamp(Glass, Card);
                Add.Add(Glass);
            }
            else
            {
                BuildingItem Door = Between(A, B, Host.Floor, Host.Position.Z, CutHeight(Card, Host, false), BuildingKind.Door);
                Door.Interaction = "Door";
                Stamp(Door, Card);
                Add.Add(Door);
            }
        }

        static void Stamp(BuildingItem Piece, SurfaceItem Card)
        {
            if (Piece == null || Card == null || string.IsNullOrEmpty(Card.Model))
            {
                return;
            }

            Piece.Model = Card.Model;
            Piece.ModelMeters = Card.Rise;
            Piece.ModelShift = Card.ModelShift;
            Piece.Swatch = Card.Swatch;
            Piece.Price = Card.Price;
        }

        static BuildingItem Carry(BuildingItem Host, BuildingItem Piece)
        {
            Piece.Swatch = Host.Swatch;
            Piece.Price = Host.Price;
            if (!string.IsNullOrEmpty(Host.Interaction))
            {
                Piece.Interaction = Host.Interaction;
            }

            return Piece;
        }

        static void AddPiece(BuildingItem Piece, List<BuildingItem> Add)
        {
            if (Piece.Width < 1f)
            {
                return;
            }

            Add.Add(Piece);
        }

        static BuildingItem Between(Vector3 A, Vector3 B, int Floor, float Z, float Height, BuildingKind Kind)
        {
            float DX = B.X - A.X;
            float DY = B.Y - A.Y;
            float Span = (float)System.Math.Sqrt(DX * DX + DY * DY);
            BuildingItem Piece = new BuildingItem();
            Piece.Kind = Kind;
            Piece.Floor = Floor;
            Piece.Height = Height;
            Piece.Width = System.Math.Max(Span, 0.01f);
            Piece.Length = BuildFloor.Thickness;
            Piece.Angle = (float)System.Math.Atan2(DY, DX);
            Piece.Position = new Vector3((A.X + B.X) * 0.5f, (A.Y + B.Y) * 0.5f, Z);
            return Piece;
        }
    }
}
