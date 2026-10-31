using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public struct UseSpot
    {
        public Vector3 Point;
        public bool Sitting;
        public FurnitureItem Seat;
    }

    public static class FurnitureUse
    {
        public static bool IsScreen(FurnitureItem Box)
        {
            return Box != null && (Box.Interaction == "Television" || Box.Interaction == "Wall Television");
        }

        public static bool Covers(FurnitureItem Box, Vector3 Point, float Tile)
        {
            if (Box == null || Tile <= 0f)
            {
                return false;
            }

            CatalogItem Card = BuildCatalog.Find(Box.Interaction);
            if (Card == null || Card.ReachTiles <= 0)
            {
                return false;
            }

            float Along;
            float Across;
            Box.MapToLocal(Point.X, Point.Y, out Along, out Across);
            float HalfW = Box.Width * 0.5f;
            float HalfL = Box.Length * 0.5f;
            float Reach = Card.ReachTiles * Tile;
            if (Card.Approach == ApproachSide.Any)
            {
                if (System.Math.Abs(Along) <= HalfW && System.Math.Abs(Across) <= HalfL)
                {
                    return false;
                }

                return System.Math.Abs(Along) <= HalfW + Reach + 0.01f && System.Math.Abs(Across) <= HalfL + Reach + 0.01f;
            }

            float Spread = Card.SpreadTiles * Tile;
            if (System.Math.Abs(Along) > HalfW + Spread + 0.01f)
            {
                return false;
            }

            if (Across < HalfL - 0.01f)
            {
                return false;
            }

            return Across <= HalfL + Reach + 0.01f;
        }

        public static List<Vector3> Tiles(FurnitureItem Box, float Tile)
        {
            List<Vector3> Found = new List<Vector3>();
            if (Box == null || Tile <= 0f)
            {
                return Found;
            }

            CatalogItem Card = BuildCatalog.Find(Box.Interaction);
            if (Card == null || Card.ReachTiles <= 0)
            {
                return Found;
            }

            float Pad = (Card.ReachTiles + Card.SpreadTiles + 2) * Tile + Box.Width + Box.Length;
            int X0 = (int)System.Math.Floor((Box.Position.X - Pad) / Tile);
            int X1 = (int)System.Math.Floor((Box.Position.X + Pad) / Tile);
            int Y0 = (int)System.Math.Floor((Box.Position.Y - Pad) / Tile);
            int Y1 = (int)System.Math.Floor((Box.Position.Y + Pad) / Tile);
            float Ground = BuildFloor.Surface(Box.Floor);
            for (int X = X0; X <= X1; X++)
            {
                for (int Y = Y0; Y <= Y1; Y++)
                {
                    Vector3 Center = new Vector3((X + 0.5f) * Tile, (Y + 0.5f) * Tile, Ground);
                    if (Box.FootprintContains(Center.X, Center.Y))
                    {
                        continue;
                    }

                    if (Covers(Box, Center, Tile))
                    {
                        Found.Add(Center);
                    }
                }
            }

            return Found;
        }

        public static List<UseSpot> Choices(FurnitureItem Screen, IList<FurnitureItem> Room, IList<PlayerCharacter> People, PlayerCharacter Who, float Tile)
        {
            List<UseSpot> Seats = new List<UseSpot>();
            List<UseSpot> Standing = new List<UseSpot>();
            if (Screen == null || Tile <= 0f)
            {
                return Seats;
            }

            if (Room != null)
            {
                for (int i = 0; i < Room.Count; i++)
                {
                    FurnitureItem Seat = Room[i];
                    if (Seat == null || Seat == Screen || Seat.Floor != Screen.Floor)
                    {
                        continue;
                    }

                    CatalogItem Card = BuildCatalog.Find(Seat.Interaction);
                    if (Card == null || !Card.Seat)
                    {
                        continue;
                    }

                    int Count = Card.Room < 1 ? 1 : Card.Room;
                    float Gap = Seat.Width / Count;
                    float C = (float)System.Math.Cos(Seat.Angle);
                    float S = (float)System.Math.Sin(Seat.Angle);
                    for (int Spot = 0; Spot < Count; Spot++)
                    {
                        float Along = -Seat.Width * 0.5f + Gap * (Spot + 0.5f);
                        Vector3 Point = new Vector3(Seat.Position.X + Along * C, Seat.Position.Y + Along * S, Seat.Position.Z);
                        if (!Covers(Screen, Point, Tile) || Taken(Point, People, Who, Tile))
                        {
                            continue;
                        }

                        UseSpot Open = new UseSpot();
                        Open.Point = Point;
                        Open.Sitting = true;
                        Open.Seat = Seat;
                        Seats.Add(Open);
                    }
                }
            }

            List<Vector3> Marks = Tiles(Screen, Tile);
            for (int i = 0; i < Marks.Count; i++)
            {
                Vector3 Mark = Marks[i];
                if (Taken(Mark, People, Who, Tile) || Blocked(Mark, Room, Screen))
                {
                    continue;
                }

                UseSpot Open = new UseSpot();
                Open.Point = Mark;
                Standing.Add(Open);
            }

            Vector3 From = Who == null ? Screen.Position : Who.Position;
            ClosestFirst(Seats, From);
            ClosestFirst(Standing, From);
            Seats.AddRange(Standing);
            return Seats;
        }

        static bool Blocked(Vector3 Mark, IList<FurnitureItem> Room, FurnitureItem Screen)
        {
            if (Room == null)
            {
                return false;
            }

            for (int i = 0; i < Room.Count; i++)
            {
                FurnitureItem Box = Room[i];
                if (Box == null || Box == Screen || Box.Floor != Screen.Floor)
                {
                    continue;
                }

                if (Box.FootprintContains(Mark.X, Mark.Y))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool Taken(Vector3 Spot, IList<PlayerCharacter> People, PlayerCharacter Self, float Tile)
        {
            if (People == null || Tile <= 0f)
            {
                return false;
            }

            float Limit = Tile * 0.45f;
            float Limit2 = Limit * Limit;
            for (int i = 0; i < People.Count; i++)
            {
                PlayerCharacter Who = People[i];
                if (Who == null || Who == Self)
                {
                    continue;
                }

                float DX = Who.Position.X - Spot.X;
                float DY = Who.Position.Y - Spot.Y;
                if (DX * DX + DY * DY <= Limit2)
                {
                    return true;
                }
            }

            return false;
        }

        static void ClosestFirst(List<UseSpot> Spots, Vector3 From)
        {
            for (int i = 0; i < Spots.Count; i++)
            {
                int Best = i;
                float BestDistance = Distance2(Spots[i].Point, From);
                for (int j = i + 1; j < Spots.Count; j++)
                {
                    float Distance = Distance2(Spots[j].Point, From);
                    if (Distance < BestDistance)
                    {
                        Best = j;
                        BestDistance = Distance;
                    }
                }

                if (Best != i)
                {
                    UseSpot Swap = Spots[i];
                    Spots[i] = Spots[Best];
                    Spots[Best] = Swap;
                }
            }
        }

        static float Distance2(Vector3 A, Vector3 B)
        {
            float DX = A.X - B.X;
            float DY = A.Y - B.Y;
            return DX * DX + DY * DY;
        }
    }
}
