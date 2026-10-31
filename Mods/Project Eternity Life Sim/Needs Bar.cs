using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public struct MotiveBar
    {
        public string Name;
        public string Short;
        public int Fill;
    }

    public static class Motives
    {
        public const int Full = 100;
        public const int RoomFull = 20;
        public const int Mess = 2;

        public static int Satisfaction(int Worse)
        {
            if (Worse < 0)
            {
                Worse = 0;
            }

            if (Worse > Full)
            {
                Worse = Full;
            }

            return Full - Worse;
        }

        public static int Room(PlayerCharacter Who)
        {
            if (Who == null || Who.SharedMapContex == null)
            {
                return 0;
            }

            MapInfo Here = Who.SharedMapContex.CurrentMapInfo;
            BuildingZone Zone = BuildingZones.At(Here, Who.Position.X, Who.Position.Y);
            int Points = 0;
            if (Zone != null)
            {
                Points = RoomPoints(Who, Zone, Here);
            }

            Outlook Sky = new Outlook();
            bool Dry = false;
            NavMapGameManager World = Who.SharedMapContex.RootMapContainer;
            if (World != null && World.Clock != null && Here != null)
            {
                Dry = Sheltered(Here, Who.Position.X, Who.Position.Y, BuildFloor.Index(Who.Position.Z));
                Vector3 Shift = ChunkOrigin.Shift(Here);
                Sky = Forecast.Look(World, Who.Position.X + Shift.X, Who.Position.Y + Shift.Z);
            }

            return Scale(Forecast.Apply(Points, Sky, Dry));
        }

        public static bool Sheltered(MapInfo Chunk, float LocalX, float LocalY, int Floor)
        {
            if (Chunk == null || Chunk.MapMapContainer == null)
            {
                return false;
            }

            float SpanX = Chunk.MapMapContainer.SpanX;
            float SpanY = Chunk.MapMapContainer.SpanY;
            if (SpanX <= 0f)
            {
                SpanX = 32f;
            }

            if (SpanY <= 0f)
            {
                SpanY = SpanX;
            }

            float Tile = TileOf(Chunk);
            ZoneFootprint Whole = new ZoneFootprint();
            Whole.Chunk = Chunk;
            Whole.Min = Vector3.Zero;
            Whole.Max = new Vector3(SpanX, SpanY, 0f);
            RoomCells Area = Flood(Whole, Tile);
            int Id;
            if (!Area.TryClosed(Key(Cell(LocalX, Tile), Cell(LocalY, Tile)), out Id))
            {
                return false;
            }

            return HasRoof(Chunk, LocalX, LocalY, Floor);
        }

        static bool HasRoof(MapInfo Chunk, float LocalX, float LocalY, int Floor)
        {
            if (Chunk.Buildings == null)
            {
                return false;
            }

            int Above = Floor + 1;
            for (int i = 0; i < Chunk.Buildings.Count; i++)
            {
                BuildingItem Box = Chunk.Buildings[i];
                if (Box != null && Box.Kind == BuildingKind.Floor && Box.Floor == Above && Box.FootprintContains(LocalX, LocalY))
                {
                    return true;
                }
            }

            return false;
        }

        static int RoomPoints(PlayerCharacter Who, BuildingZone Zone, MapInfo Here)
        {
            float Tile = TileOf(Here);
            ZoneFootprint Standing = Zone.Foot(Here);
            RoomCells HereRoom = Flood(Standing, Tile);
            long At = Key(Cell(Who.Position.X, Tile), Cell(Who.Position.Y, Tile));
            int Id;
            int Points = 0;
            if (HereRoom.TryClosed(At, out Id))
            {
                Points = Sum(Zone, Standing, Tile, HereRoom, Id);
            }
            else
            {
                for (int f = 0; f < Zone.Footprints.Count; f++)
                {
                    Points += Sum(Zone, Zone.Footprints[f], Tile, null, -1);
                }
            }

            return Points;
        }

        public static int Contribution(FurnitureItem Box)
        {
            if (Box == null)
            {
                return 0;
            }

            if (IsMess(Box.Interaction))
            {
                return -Mess;
            }

            CatalogItem Card = BuildCatalog.Find(Box.Interaction);
            if (Card != null && Card.Page == CatalogPage.Decorative)
            {
                return Card.Room;
            }

            return 0;
        }

        static int Sum(BuildingZone Zone, ZoneFootprint Foot, float Tile, RoomCells Room, int Id)
        {
            if (Zone == null || Foot == null || Foot.Chunk == null || Foot.Chunk.Furniture == null)
            {
                return 0;
            }

            if (Room == null)
            {
                Room = Flood(Foot, Tile);
            }

            int Points = 0;
            for (int b = 0; b < Foot.Chunk.Furniture.Count; b++)
            {
                FurnitureItem Box = Foot.Chunk.Furniture[b];
                if (Box == null || !Zone.Contains(Foot.Chunk, Box.Position.X, Box.Position.Y))
                {
                    continue;
                }

                long At = Key(Cell(Box.Position.X, Tile), Cell(Box.Position.Y, Tile));
                int BoxId;
                bool Known = Room.Component.TryGetValue(At, out BoxId);
                if (Id >= 0)
                {
                    if (!Known || BoxId != Id)
                    {
                        continue;
                    }
                }
                else if (Known && Room.Closed.Contains(BoxId))
                {
                    continue;
                }

                Points += Contribution(Box);
            }

            return Points;
        }

        static RoomCells Flood(ZoneFootprint Foot, float Tile)
        {
            RoomCells Area = new RoomCells();
            if (Foot == null || Foot.Chunk == null || Tile <= 0f)
            {
                return Area;
            }

            int X0 = (int)System.Math.Floor(Foot.Min.X / Tile);
            int Y0 = (int)System.Math.Floor(Foot.Min.Y / Tile);
            int X1 = (int)System.Math.Ceiling(Foot.Max.X / Tile) - 1;
            int Y1 = (int)System.Math.Ceiling(Foot.Max.Y / Tile) - 1;
            if (X1 < X0 || Y1 < Y0)
            {
                return Area;
            }

            HashSet<long> Walls = new HashSet<long>();
            MarkWalls(Foot, Tile, Walls);
            MarkTerrain(Foot.Chunk.MapMapContainer == null ? null : Foot.Chunk.MapMapContainer.ActiveMap, X0, Y0, X1, Y1, Walls);
            HashSet<long> Seen = new HashSet<long>();
            int Next = 0;
            for (int Y = Y0; Y <= Y1; Y++)
            {
                for (int X = X0; X <= X1; X++)
                {
                    long Start = Key(X, Y);
                    if (Seen.Contains(Start) || Walls.Contains(Start))
                    {
                        continue;
                    }

                    List<long> Component = new List<long>();
                    List<long> Queue = new List<long>();
                    Queue.Add(Start);
                    Seen.Add(Start);
                    bool Border = false;
                    for (int i = 0; i < Queue.Count; i++)
                    {
                        int CX;
                        int CY;
                        Split(Queue[i], out CX, out CY);
                        Component.Add(Queue[i]);
                        if (CX <= X0 || CX >= X1 || CY <= Y0 || CY >= Y1)
                        {
                            Border = true;
                        }

                        Push(Queue, Seen, Walls, CX - 1, CY, X0, Y0, X1, Y1, ref Border);
                        Push(Queue, Seen, Walls, CX + 1, CY, X0, Y0, X1, Y1, ref Border);
                        Push(Queue, Seen, Walls, CX, CY - 1, X0, Y0, X1, Y1, ref Border);
                        Push(Queue, Seen, Walls, CX, CY + 1, X0, Y0, X1, Y1, ref Border);
                    }

                    for (int c = 0; c < Component.Count; c++)
                    {
                        Area.Component[Component[c]] = Next;
                    }

                    if (!Border)
                    {
                        Area.Closed.Add(Next);
                    }

                    Next++;
                }
            }

            return Area;
        }

        static void Push(List<long> Queue, HashSet<long> Seen, HashSet<long> Walls, int X, int Y, int X0, int Y0, int X1, int Y1, ref bool Border)
        {
            if (X < X0 || X > X1 || Y < Y0 || Y > Y1)
            {
                Border = true;
                return;
            }

            long At = Key(X, Y);
            if (Walls.Contains(At) || Seen.Contains(At))
            {
                return;
            }

            Seen.Add(At);
            Queue.Add(At);
        }

        static void MarkWalls(ZoneFootprint Foot, float Tile, HashSet<long> Walls)
        {
            if (Foot.Chunk.Buildings == null)
            {
                return;
            }

            for (int i = 0; i < Foot.Chunk.Buildings.Count; i++)
            {
                BuildingItem Box = Foot.Chunk.Buildings[i];
                if (Box == null || (Box.Kind != BuildingKind.Wall && Box.Kind != BuildingKind.Door && Box.Kind != BuildingKind.Window))
                {
                    continue;
                }

                float HalfW = Box.Width > 0f ? Box.Width * 0.5f : Tile * 0.5f;
                float HalfL = Box.Length > 0f ? Box.Length * 0.5f : Tile * 0.5f;
                int X0 = Cell(Box.Position.X - HalfW + 0.01f, Tile);
                int X1 = Cell(Box.Position.X + HalfW - 0.01f, Tile);
                int Y0 = Cell(Box.Position.Y - HalfL + 0.01f, Tile);
                int Y1 = Cell(Box.Position.Y + HalfL - 0.01f, Tile);
                for (int Y = Y0; Y <= Y1; Y++)
                {
                    for (int X = X0; X <= X1; X++)
                    {
                        Walls.Add(Key(X, Y));
                    }
                }
            }
        }

        static void MarkTerrain(LifeSimMap Map, int X0, int Y0, int X1, int Y1, HashSet<long> Walls)
        {
            if (Map == null || Map.LayerManager == null || Map.LayerManager.ListLayer.Count == 0 || Map.MapSize.X <= 0)
            {
                return;
            }

            MapLayer Layer = Map.LayerManager.ListLayer[0];
            if (Layer.ArrayTerrain == null)
            {
                return;
            }

            for (int Y = Y0; Y <= Y1; Y++)
            {
                for (int X = X0; X <= X1; X++)
                {
                    if (X < 0 || Y < 0 || X >= Map.MapSize.X || Y >= Map.MapSize.Y)
                    {
                        continue;
                    }

                    if (Layer.ArrayTerrain[X, Y].TerrainTypeIndex == UnitStats.TerrainWallIndex)
                    {
                        Walls.Add(Key(X, Y));
                    }
                }
            }
        }

        static void Split(long Packed, out int X, out int Y)
        {
            X = (int)(Packed >> 32);
            Y = (int)Packed;
        }

        static int Scale(int Points)
        {
            if (Points <= 0)
            {
                return 0;
            }

            int Shown = Points * (Full / RoomFull);
            if (Shown > Full)
            {
                Shown = Full;
            }

            return Shown;
        }

        static bool IsMess(string Name)
        {
            if (string.IsNullOrEmpty(Name))
            {
                return false;
            }

            if (Name.IndexOf("Trash Can", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return false;
            }

            return Name.IndexOf("Litter", System.StringComparison.OrdinalIgnoreCase) >= 0
                || Name.IndexOf("Trash", System.StringComparison.OrdinalIgnoreCase) >= 0;
        }

        static float TileOf(MapInfo Chunk)
        {
            LifeSimMap Map = Chunk == null || Chunk.MapMapContainer == null ? null : Chunk.MapMapContainer.ActiveMap;
            if (Map != null && Map.TileSize.X > 0)
            {
                return Map.TileSize.X;
            }

            return 32f;
        }

        static int Cell(float Local, float Tile)
        {
            return (int)System.Math.Floor(Local / Tile);
        }

        static long Key(int X, int Y)
        {
            return ((long)X << 32) | (uint)Y;
        }

        public static MotiveBar[] Bars(PlayerCharacter Who)
        {
            if (Who == null)
            {
                return new MotiveBar[]
                {
                    Named("Hunger", "Hun", 0),
                    Named("Hygiene", "Hyg", 0),
                    Named("Energy", "Nrg", 0),
                    Named("Social", "Soc", 0),
                    Named("Comfort", "Com", 0),
                    Named("Bladder", "Bld", 0),
                    Named("Fun", "Fun", 0),
                    Named("Room", "Room", 0),
                };
            }

            return new MotiveBar[]
            {
                Named("Hunger", "Hun", Satisfaction(Who.Hunger)),
                Named("Hygiene", "Hyg", Satisfaction(Who.Hygiene)),
                Named("Energy", "Nrg", Satisfaction(Who.Sleep)),
                Named("Social", "Soc", Satisfaction(Who.Social)),
                Named("Comfort", "Com", Satisfaction(Who.Comfort)),
                Named("Bladder", "Bld", Satisfaction(Who.Bladder)),
                Named("Fun", "Fun", Satisfaction(Who.Fun)),
                Named("Room", "Room", Room(Who)),
            };
        }

        static MotiveBar Named(string Name, string Short, int Fill)
        {
            MotiveBar Bar = new MotiveBar();
            Bar.Name = Name;
            Bar.Short = Short;
            Bar.Fill = Fill;
            return Bar;
        }

        class RoomCells
        {
            public Dictionary<long, int> Component = new Dictionary<long, int>();
            public HashSet<int> Closed = new HashSet<int>();

            public bool TryClosed(long At, out int Id)
            {
                if (!Component.TryGetValue(At, out Id))
                {
                    Id = -1;
                    return false;
                }

                return Closed.Contains(Id);
            }
        }
    }

    public static class NeedsBar
    {
        static readonly Color Panel = new Color(72, 88, 132, 214);
        static readonly Color Edge = new Color(168, 186, 220, 230);
        static readonly Color Green = new Color(42, 186, 58);
        static readonly Color Red = new Color(196, 38, 42);
        static readonly Color Outline = new Color(18, 22, 36, 230);
        static readonly Color Ring = new Color(36, 48, 92, 240);

        public static void Draw(CustomSpriteBatch g, PlayerCharacter Who)
        {
            Texture2D Pixel = GameScreen.sprPixel;
            if (g == null || Pixel == null)
            {
                return;
            }

            Viewport Pane = g.GraphicsDevice.Viewport;
            int Margin = 18;
            int Height = 112;
            if (Height > Pane.Height / 3)
            {
                Height = Pane.Height / 3;
            }

            Rectangle Bar = new Rectangle(Margin, Pane.Height - Height - 12, Pane.Width - Margin * 2, Height);
            if (Bar.Width < 80 || Bar.Height < 48)
            {
                return;
            }

            Rounded(g, Pixel, new Rectangle(Bar.X - 2, Bar.Y - 2, Bar.Width + 4, Bar.Height + 4), 22, Edge);
            Rounded(g, Pixel, Bar, 20, Panel);

            int Face = Bar.Height - 28;
            if (Face > 78)
            {
                Face = 78;
            }

            int FaceX = Bar.X + 16 + Face / 2;
            int FaceY = Bar.Y + Bar.Height / 2;
            Disc(g, Pixel, FaceX, FaceY, Face / 2 + 4, Ring);
            Disc(g, Pixel, FaceX, FaceY, Face / 2, new Color(120, 140, 180, 255));
            Texture2D Portrait = Who == null ? null : (Who.SpriteUnit ?? Who.SpriteMap);
            if (Portrait != null)
            {
                int Side = Face - 8;
                g.Draw(Portrait, new Rectangle(FaceX - Side / 2, FaceY - Side / 2, Side, Side), Color.White);
            }

            MotiveBar[] Bars = Motives.Bars(Who);
            int Start = FaceX + Face / 2 + 22;
            int End = Bar.Right - 16;
            int Span = End - Start;
            if (Span < Bars.Length * 12)
            {
                return;
            }

            int Slot = Span / Bars.Length;
            int MeterW = Slot > 36 ? 16 : 12;
            int MeterH = Bar.Height - 40;
            if (MeterH < 24)
            {
                MeterH = 24;
            }

            int MeterTop = Bar.Y + 10;
            SpriteFont Font = TextHelper.fntShadowFont;
            for (int i = 0; i < Bars.Length; i++)
            {
                int Center = Start + Slot * i + Slot / 2;
                Meter(g, Pixel, Center - MeterW / 2, MeterTop, MeterW, MeterH, Bars[i].Fill);
                if (Font == null)
                {
                    continue;
                }

                string Label = Bars[i].Name;
                if (Font.MeasureString(Label).X > Slot - 4)
                {
                    Label = Bars[i].Short;
                }

                TextHelper.DrawTextMiddleAligned(g, Label, new Vector2(Center, MeterTop + MeterH + 4), Color.White);
            }
        }

        static void Meter(CustomSpriteBatch g, Texture2D Pixel, int X, int Y, int W, int H, int Fill)
        {
            if (Fill < 0)
            {
                Fill = 0;
            }

            if (Fill > Motives.Full)
            {
                Fill = Motives.Full;
            }

            Rounded(g, Pixel, new Rectangle(X - 2, Y - 2, W + 4, H + 4), W / 2 + 2, Outline);
            int GreenRows = H * Fill / Motives.Full;
            int Radius = W / 2;
            for (int Row = 0; Row < H; Row++)
            {
                int Inset = Cap(Radius, Row < Radius ? Row : (Row >= H - Radius ? H - 1 - Row : Radius));
                int Width = W - Inset * 2;
                if (Width <= 0)
                {
                    continue;
                }

                bool Satisfied = H - 1 - Row < GreenRows;
                g.Draw(Pixel, new Rectangle(X + Inset, Y + Row, Width, 1), Satisfied ? Green : Red);
            }
        }

        static void Disc(CustomSpriteBatch g, Texture2D Pixel, int CenterX, int CenterY, int Radius, Color Color)
        {
            for (int Row = -Radius; Row <= Radius; Row++)
            {
                int Half = (int)System.Math.Sqrt(Radius * Radius - Row * Row);
                g.Draw(Pixel, new Rectangle(CenterX - Half, CenterY + Row, Half * 2, 1), Color);
            }
        }

        static void Rounded(CustomSpriteBatch g, Texture2D Pixel, Rectangle Box, int Radius, Color Color)
        {
            if (Radius > Box.Width / 2)
            {
                Radius = Box.Width / 2;
            }

            if (Radius > Box.Height / 2)
            {
                Radius = Box.Height / 2;
            }

            if (Radius < 1)
            {
                g.Draw(Pixel, Box, Color);
                return;
            }

            g.Draw(Pixel, new Rectangle(Box.X, Box.Y + Radius, Box.Width, Box.Height - Radius * 2), Color);
            for (int Row = 0; Row < Radius; Row++)
            {
                int Inset = Cap(Radius, Row);
                g.Draw(Pixel, new Rectangle(Box.X + Inset, Box.Y + Row, Box.Width - Inset * 2, 1), Color);
                g.Draw(Pixel, new Rectangle(Box.X + Inset, Box.Bottom - 1 - Row, Box.Width - Inset * 2, 1), Color);
            }
        }

        static int Cap(int Radius, int Row)
        {
            float FromCenter = Radius - Row - 0.5f;
            float Inside = Radius * Radius - FromCenter * FromCenter;
            if (Inside < 0f)
            {
                Inside = 0f;
            }

            return (int)System.Math.Ceiling(Radius - System.Math.Sqrt(Inside));
        }
    }
}
