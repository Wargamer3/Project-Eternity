using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class BuildFloor
    {
        public const float Story = 96f;
        public const float Thickness = 4f;
        public const int Highest = 7;

        public static int Index(float Height)
        {
            if (Height < Story)
            {
                return 0;
            }

            int Floor = (int)(Height / Story);
            if (Floor > Highest)
            {
                return Highest;
            }

            return Floor;
        }

        public static float Surface(int Floor)
        {
            if (Floor < 0)
            {
                Floor = 0;
            }

            return Floor * Story;
        }

        public static int Shift(int Floor, int Direction)
        {
            int Next = Floor + Direction;
            if (Next < 0)
            {
                return 0;
            }

            if (Next > Highest)
            {
                return Highest;
            }

            return Next;
        }

        public static Vector3 SnapCenter(Vector3 Ground, float Tile, bool Grid)
        {
            if (!Grid || Tile <= 0f)
            {
                return new Vector3(Ground.X, Ground.Y, 0f);
            }

            int X = (int)System.Math.Floor(Ground.X / Tile);
            int Y = (int)System.Math.Floor(Ground.Y / Tile);
            return new Vector3((X + 0.5f) * Tile, (Y + 0.5f) * Tile, 0f);
        }

        public static Vector3 SnapCorner(Vector3 Ground, float Tile, bool Grid)
        {
            if (!Grid || Tile <= 0f)
            {
                return new Vector3(Ground.X, Ground.Y, 0f);
            }

            float X = (float)System.Math.Round(Ground.X / Tile) * Tile;
            float Y = (float)System.Math.Round(Ground.Y / Tile) * Tile;
            return new Vector3(X, Y, 0f);
        }
    }

    public class BuildResult
    {
        public bool Valid;
        public string Reason;
        public FurnitureItem FurnitureGhost;
        public BuildingItem BuildingGhost;
        public readonly List<FurnitureItem> AddFurniture = new List<FurnitureItem>();
        public readonly List<BuildingItem> AddBuilding = new List<BuildingItem>();
        public readonly List<FurnitureItem> RemoveFurniture = new List<FurnitureItem>();
        public readonly List<BuildingItem> RemoveBuilding = new List<BuildingItem>();

        public static BuildResult Outside(MapInfo Limit, Vector3 Cursor)
        {
            if (Limit == null || BuildingZones.Allows(Limit, Cursor.X, Cursor.Y))
            {
                return null;
            }

            BuildResult Denied = new BuildResult();
            Denied.Reason = "This spot is outside a building zone.";
            return Denied;
        }

        public void Apply(MapInfo Here)
        {
            if (Here == null || !Valid)
            {
                return;
            }

            for (int i = 0; i < RemoveFurniture.Count; i++)
            {
                BoxPlace.Drop(Here, RemoveFurniture[i]);
            }

            for (int i = 0; i < RemoveBuilding.Count; i++)
            {
                BoxPlace.Drop(Here, RemoveBuilding[i]);
            }

            for (int i = 0; i < AddFurniture.Count; i++)
            {
                BoxPlace.Keep(Here, AddFurniture[i]);
            }

            for (int i = 0; i < AddBuilding.Count; i++)
            {
                BoxPlace.Keep(Here, AddBuilding[i]);
            }
        }
    }

    public static class BuildPlan
    {
        public static BuildResult Preview(string Item, Vector3 Cursor, Vector3 Anchor, bool HasAnchor, int Floor, bool Grid, float Tile, IList<FurnitureItem> Furniture, IList<BuildingItem> Buildings)
        {
            return Commit(Item, Cursor, Anchor, HasAnchor, Floor, Grid, Tile, Furniture, Buildings, false);
        }

        public static BuildResult Commit(string Item, Vector3 Cursor, Vector3 Anchor, bool HasAnchor, int Floor, bool Grid, float Tile, IList<FurnitureItem> Furniture, IList<BuildingItem> Buildings)
        {
            return Commit(Item, Cursor, Anchor, HasAnchor, Floor, Grid, Tile, Furniture, Buildings, true);
        }

        public static BuildResult Preview(string Item, Vector3 Cursor, Vector3 Anchor, bool HasAnchor, int Floor, bool Grid, float Tile, IList<FurnitureItem> Furniture, IList<BuildingItem> Buildings, MapInfo Limit)
        {
            BuildResult Shown = Preview(Item, Cursor, Anchor, HasAnchor, Floor, Grid, Tile, Furniture, Buildings);
            Seat(Shown, Limit);
            return Gate(Shown, Limit, Cursor);
        }

        public static BuildResult Commit(string Item, Vector3 Cursor, Vector3 Anchor, bool HasAnchor, int Floor, bool Grid, float Tile, IList<FurnitureItem> Furniture, IList<BuildingItem> Buildings, MapInfo Limit)
        {
            BuildResult Denied = BuildResult.Outside(Limit, Cursor);
            if (Denied != null)
            {
                BuildResult Shown = Preview(Item, Cursor, Anchor, HasAnchor, Floor, Grid, Tile, Furniture, Buildings);
                Seat(Shown, Limit);
                if (Shown != null)
                {
                    Denied.FurnitureGhost = Shown.FurnitureGhost;
                }
                return Denied;
            }

            BuildResult Result = Commit(Item, Cursor, Anchor, HasAnchor, Floor, Grid, Tile, Furniture, Buildings);
            Seat(Result, Limit);
            return Result;
        }

        static void Seat(BuildResult Result, MapInfo Limit)
        {
            if (Result == null || Limit == null || Limit.Heights == null)
            {
                return;
            }

            Seat(Result.FurnitureGhost, Limit);
            for (int i = 0; i < Result.AddFurniture.Count; i++)
            {
                Seat(Result.AddFurniture[i], Limit);
            }
        }

        static void Seat(FurnitureItem Piece, MapInfo Limit)
        {
            if (Piece == null || Piece.Floor > 0)
            {
                return;
            }

            float Ground;
            float SlopeX;
            float SlopeY;
            if (!Limit.Heights.Sample(Piece.Position.X, Piece.Position.Y, out Ground, out SlopeX, out SlopeY))
            {
                return;
            }

            Piece.Position = new Vector3(Piece.Position.X, Piece.Position.Y, Ground);
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
                Denied.FurnitureGhost = Shown.FurnitureGhost;
            }

            return Denied;
        }

        static BuildResult Commit(string Item, Vector3 Cursor, Vector3 Anchor, bool HasAnchor, int Floor, bool Grid, float Tile, IList<FurnitureItem> Furniture, IList<BuildingItem> Buildings, bool Final)
        {
            if (Tile <= 0f)
            {
                Tile = 32f;
            }

            BuildResult Result = new BuildResult();
            FurnitureItem Piece = MakeFurniture(Item, Cursor, Floor, Grid, Tile);
            Result.FurnitureGhost = Piece;
            if (Piece == null)
            {
                Result.Reason = "Unknown piece";
                return Result;
            }

            CatalogItem Card = BuildCatalog.Find(Item);
            PlaceRule Rule = Card == null ? PlaceRule.Floor : Card.Rule;
            if (Rule == PlaceRule.Surface)
            {
                string Why;
                if (!Seat(Piece, Furniture, out Why))
                {
                    Result.Reason = Why;
                    return Result;
                }
            }
            else if (Rule == PlaceRule.Wall)
            {
                string Why;
                if (!Hang(Piece, Buildings, Cursor, Floor, Tile, out Why))
                {
                    Result.Reason = Why;
                    return Result;
                }

                if (Crowded(InFront(Piece, Tile), Furniture))
                {
                    Result.Reason = "Something blocks it";
                    return Result;
                }
            }
            else if (Rule == PlaceRule.Ceiling)
            {
                Piece.Position = new Vector3(Piece.Position.X, Piece.Position.Y, BuildFloor.Surface(Floor) + BuildFloor.Story - Piece.Height);
                if (!FurnitureSupported(Piece, Buildings))
                {
                    Result.Reason = "Needs a floor";
                    return Result;
                }

                if (FurnitureBlocked(Piece, Furniture, Buildings))
                {
                    Result.Reason = "Not on a wall or another piece";
                    return Result;
                }
            }
            else if (!FurnitureSupported(Piece, Buildings))
            {
                Result.Reason = "Needs a floor";
                return Result;
            }
            else if (FurnitureBlocked(Piece, Furniture, Buildings))
            {
                Result.Reason = "Not on a wall or another piece";
                return Result;
            }

            Result.Valid = true;
            if (Final)
            {
                Result.AddFurniture.Add(Piece);
            }

            return Result;
        }

        static FurnitureItem MakeFurniture(string Item, Vector3 Cursor, int Floor, bool Grid, float Tile)
        {
            Vector3 At = BuildFloor.SnapCenter(Cursor, Tile, Grid);
            FurnitureItem Box = new FurnitureItem();
            Box.Floor = Floor;
            Box.Position = new Vector3(At.X, At.Y, BuildFloor.Surface(Floor));
            Box.Interaction = Item;
            if (Item == "Bed" || Item == "Fridge")
            {
                Box.Kind = BoxKind.SmallCube;
                Box.Width = Tile * 0.75f;
                Box.Length = Tile * 0.75f;
                Box.Height = Item == "Bed" ? Tile * 0.4f : Tile * 0.8f;
                Paint(Box);
                return Box;
            }

            CatalogItem Card = BuildCatalog.Find(Item);
            float Span = 0.75f;
            float Depth = 0.75f;
            float Rise = 0.6f;
            if (Card != null)
            {
                if (Card.Span > 0f)
                {
                    Span = Card.Span;
                }

                if (Card.Depth > 0f)
                {
                    Depth = Card.Depth;
                }

                if (Card.Rise > 0f)
                {
                    Rise = Card.Rise;
                }
            }

            float Fit = BuildCatalog.FitOf(Card);
            Box.Kind = BoxKind.SmallCube;
            Box.Width = Tile * Span * Fit;
            Box.Length = Tile * Depth * Fit;
            Box.Height = Tile * Rise * Fit;
            Paint(Box);
            return Box;
        }

        static void Paint(FurnitureItem Box)
        {
            CatalogItem Card = BuildCatalog.Find(Box.Interaction);
            if (Card == null)
            {
                return;
            }

            Box.Swatch = Card.Swatch;
            Box.Price = Card.Price;
            Box.Model = Card.Model;
            Box.ModelMeters = Card.ModelMeters;
            Box.ModelShift = Card.ModelShift;
        }

        public static FurnitureItem FurnitureAt(IList<FurnitureItem> Boxes, Vector3 Ground, int Floor)
        {
            FurnitureItem Best = null;
            float BestDistance = 28f;
            if (Boxes == null)
            {
                return null;
            }

            for (int i = 0; i < Boxes.Count; i++)
            {
                FurnitureItem Box = Boxes[i];
                if (Box.Floor != Floor)
                {
                    continue;
                }

                if (!Box.FootprintContains(Ground.X, Ground.Y))
                {
                    float DX = Ground.X - Box.Position.X;
                    float DY = Ground.Y - Box.Position.Y;
                    float Distance = (float)System.Math.Sqrt(DX * DX + DY * DY);
                    if (Distance > BestDistance)
                    {
                        continue;
                    }

                    BestDistance = Distance;
                }

                Best = Box;
            }

            return Best;
        }

        public static string Refuse(FurnitureItem Piece, IList<FurnitureItem> Furniture, IList<BuildingItem> Buildings)
        {
            if (Piece == null)
            {
                return "Nothing to place";
            }

            if (!FurnitureSupported(Piece, Buildings))
            {
                return "Needs a floor";
            }

            if (FurnitureBlocked(Piece, Furniture, Buildings))
            {
                return "Not on a wall or another piece";
            }

            return null;
        }

        public static bool FurnitureSupported(FurnitureItem Piece, IList<BuildingItem> Buildings)
        {
            if (Piece == null || Piece.Floor <= 0)
            {
                return true;
            }

            return FloorUnder(Buildings, Piece.Position, Piece.Floor);
        }

        static bool FloorUnder(IList<BuildingItem> Buildings, Vector3 At, int Floor)
        {
            if (Buildings == null)
            {
                return false;
            }

            for (int i = 0; i < Buildings.Count; i++)
            {
                BuildingItem Box = Buildings[i];
                if (Box.Kind == BuildingKind.Floor && Box.Floor == Floor && Box.FootprintContains(At.X, At.Y))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool FurnitureBlocked(FurnitureItem Piece, IList<FurnitureItem> Furniture, IList<BuildingItem> Buildings)
        {
            if (Piece == null)
            {
                return false;
            }

            if (Furniture != null)
            {
                for (int i = 0; i < Furniture.Count; i++)
                {
                    FurnitureItem Other = Furniture[i];
                    if (object.ReferenceEquals(Other, Piece))
                    {
                        continue;
                    }

                    if (VolumesOverlap(Piece.Body, Other.Body))
                    {
                        return true;
                    }
                }
            }

            if (Buildings != null)
            {
                for (int i = 0; i < Buildings.Count; i++)
                {
                    BuildingItem Other = Buildings[i];
                    if (Other.Kind == BuildingKind.Floor || Other.Kind == BuildingKind.Stairs)
                    {
                        continue;
                    }

                    if (VolumesOverlap(Piece.Body, Other.Body))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        const float MountClearance = 0.72f;

        static bool Seat(FurnitureItem Piece, IList<FurnitureItem> Furniture, out string Reason)
        {
            Reason = "Needs a surface";
            FurnitureItem Host = SurfaceUnder(Furniture, Piece.Position, Piece.Floor);
            if (Host == null)
            {
                return false;
            }

            Piece.Position = new Vector3(Piece.Position.X, Piece.Position.Y, Host.Position.Z + Host.Height);
            Piece.Floor = Host.Floor;
            if (!Fits(Piece, Host))
            {
                Reason = "Doesn't fit";
                return false;
            }

            if (Crowded(Piece.Body, Furniture))
            {
                Reason = "Something blocks it";
                return false;
            }

            return true;
        }

        static FurnitureItem SurfaceUnder(IList<FurnitureItem> Furniture, Vector3 At, int Floor)
        {
            FurnitureItem Best = null;
            float BestTop = float.NegativeInfinity;
            if (Furniture == null)
            {
                return null;
            }

            for (int i = 0; i < Furniture.Count; i++)
            {
                FurnitureItem Box = Furniture[i];
                if (Box.Floor != Floor)
                {
                    continue;
                }

                CatalogItem Card = BuildCatalog.Find(Box.Interaction);
                if (Card == null || Card.Page != CatalogPage.Surfaces)
                {
                    continue;
                }

                if (!Box.FootprintContains(At.X, At.Y))
                {
                    continue;
                }

                float Top = Box.Position.Z + Box.Height;
                if (Top >= BestTop)
                {
                    Best = Box;
                    BestTop = Top;
                }
            }

            return Best;
        }

        static bool Fits(FurnitureItem Piece, FurnitureItem Seat)
        {
            float HalfW = Piece.Width * 0.5f;
            float HalfL = Piece.Length * 0.5f;
            return Seat.FootprintContains(Piece.Position.X - HalfW, Piece.Position.Y - HalfL)
                && Seat.FootprintContains(Piece.Position.X - HalfW, Piece.Position.Y + HalfL)
                && Seat.FootprintContains(Piece.Position.X + HalfW, Piece.Position.Y - HalfL)
                && Seat.FootprintContains(Piece.Position.X + HalfW, Piece.Position.Y + HalfL);
        }

        static bool Hang(FurnitureItem Piece, IList<BuildingItem> Buildings, Vector3 Cursor, int Floor, float Tile, out string Reason)
        {
            Reason = "Aim at a wall";
            BuildingItem Host = NearestWall(Buildings, Cursor, Floor, Tile * 0.75f);
            if (Host == null)
            {
                return false;
            }

            if (Piece.Width > Host.Width - 1f)
            {
                Reason = "Wall is too short";
                return false;
            }

            float C = (float)System.Math.Cos(Host.Angle);
            float S = (float)System.Math.Sin(Host.Angle);
            float DX = Cursor.X - Host.Position.X;
            float DY = Cursor.Y - Host.Position.Y;
            float Along = DX * C + DY * S;
            float Limit = Host.Width * 0.5f - Piece.Width * 0.5f;
            if (Along < -Limit)
            {
                Along = -Limit;
            }
            else if (Along > Limit)
            {
                Along = Limit;
            }

            float PerpX = -S;
            float PerpY = C;
            float Sign = DX * PerpX + DY * PerpY < 0f ? -1f : 1f;
            float Face = Host.Length * 0.5f + 0.4f;
            CatalogItem Hung = BuildCatalog.Find(Piece.Interaction);
            float Clearance = Hung != null && Hung.Mount >= 0f ? Hung.Mount : MountClearance;
            Piece.Position = new Vector3(
                Host.Position.X + C * Along + PerpX * Sign * Face,
                Host.Position.Y + S * Along + PerpY * Sign * Face,
                BuildFloor.Surface(Floor) + Tile * Clearance);
            Piece.Angle = Sign < 0f ? Host.Angle + (float)System.Math.PI : Host.Angle;
            Piece.Floor = Floor;
            return true;
        }

        static BuildingItem NearestWall(IList<BuildingItem> Buildings, Vector3 Cursor, int Floor, float Reach)
        {
            BuildingItem Best = null;
            float BestDistance = Reach;
            if (Buildings == null)
            {
                return null;
            }

            for (int i = 0; i < Buildings.Count; i++)
            {
                BuildingItem Box = Buildings[i];
                if (Box.Floor != Floor || Box.Kind != BuildingKind.Wall || Box.Height < BuildFloor.Story * 0.5f)
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
            float C = (float)System.Math.Cos(Wall.Angle);
            float S = (float)System.Math.Sin(Wall.Angle);
            float Half = Wall.Width * 0.5f;
            float StartX = Wall.Position.X - C * Half;
            float StartY = Wall.Position.Y - S * Half;
            float EndX = Wall.Position.X + C * Half;
            float EndY = Wall.Position.Y + S * Half;
            float SX = EndX - StartX;
            float SY = EndY - StartY;
            float Len2 = SX * SX + SY * SY;
            float T = 0f;
            if (Len2 > 0.01f)
            {
                T = ((Cursor.X - StartX) * SX + (Cursor.Y - StartY) * SY) / Len2;
                if (T < 0f)
                {
                    T = 0f;
                }
                else if (T > 1f)
                {
                    T = 1f;
                }
            }

            float DX = StartX + SX * T - Cursor.X;
            float DY = StartY + SY * T - Cursor.Y;
            return (float)System.Math.Sqrt(DX * DX + DY * DY);
        }

        static BoxVolume InFront(FurnitureItem Piece, float Tile)
        {
            float C = (float)System.Math.Cos(Piece.Angle);
            float S = (float)System.Math.Sin(Piece.Angle);
            float PerpX = -S;
            float PerpY = C;
            float Reach = Tile * 0.5f;
            BoxVolume Front = new BoxVolume();
            Front.Width = Piece.Width;
            Front.Length = Reach;
            Front.Height = Piece.Height;
            Front.Angle = Piece.Angle;
            Front.Floor = Piece.Floor;
            Front.Position = new Vector3(Piece.Position.X + PerpX * Reach * 0.5f, Piece.Position.Y + PerpY * Reach * 0.5f, Piece.Position.Z);
            return Front;
        }

        static bool Crowded(BoxVolume Zone, IList<FurnitureItem> Furniture)
        {
            if (Furniture == null)
            {
                return false;
            }

            for (int i = 0; i < Furniture.Count; i++)
            {
                if (VolumesOverlap(Zone, Furniture[i].Body))
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
    }
}
