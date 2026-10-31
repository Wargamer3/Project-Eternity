using System;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public enum DungeonRole
    {
        Empty,
        Floor,
        Wall,
        Door,
        StairUp,
        StairDown,
        Step,
    }

    public class DungeonCell
    {
        public int X;
        public int Y;
        public DungeonRole Role;
        public int Height;
        public string Model = "";
        public int North;
        public int East;
        public int South;
        public int West;

        public bool Walkable()
        {
            return Role == DungeonRole.Floor || Role == DungeonRole.Door || Role == DungeonRole.StairUp || Role == DungeonRole.StairDown || Role == DungeonRole.Step;
        }
    }

    public class DungeonTile
    {
        public string Model;
        public int North;
        public int East;
        public int South;
        public int West;
        public DungeonRole Role;

        public DungeonTile(string Model, int North, int East, int South, int West, DungeonRole Role)
        {
            this.Model = Model;
            this.North = North;
            this.East = East;
            this.South = South;
            this.West = West;
            this.Role = Role;
        }

        public bool Fits(DungeonCell Cell)
        {
            if (Cell == null)
            {
                return false;
            }

            if (Role != DungeonTiles.RoleOf(Cell))
            {
                return false;
            }

            return North == Cell.North && East == Cell.East && South == Cell.South && West == Cell.West;
        }
    }

    public static class DungeonTiles
    {
        public static readonly List<DungeonTile> All = Build();

        public static bool Allows(DungeonCell Cell)
        {
            if (Cell == null || Cell.Role == DungeonRole.Empty)
            {
                return string.IsNullOrEmpty(Cell == null ? "" : Cell.Model);
            }

            for (int i = 0; i < All.Count; i++)
            {
                DungeonTile Tile = All[i];
                if (Tile.Model == Cell.Model && Tile.Role == RoleOf(Cell))
                {
                    return true;
                }
            }

            return false;
        }

        public static DungeonRole RoleOf(DungeonCell Cell)
        {
            if (Cell.Role == DungeonRole.Door)
            {
                return DungeonRole.Door;
            }

            if (Cell.Role == DungeonRole.Step)
            {
                return DungeonRole.Step;
            }

            if (Cell.Role == DungeonRole.StairUp || Cell.Role == DungeonRole.StairDown)
            {
                return Cell.Role;
            }

            if (Cell.Role == DungeonRole.Wall)
            {
                return DungeonRole.Wall;
            }

            return DungeonRole.Floor;
        }

        static List<DungeonTile> Build()
        {
            List<DungeonTile> Tiles = new List<DungeonTile>();
            Tiles.Add(new DungeonTile("floor_tile_small", 0, 0, 0, 0, DungeonRole.Floor));
            Tiles.Add(new DungeonTile("floor_dirt_small_A", 0, 0, 0, 0, DungeonRole.Floor));
            Tiles.Add(new DungeonTile("floor_wood_small", 0, 0, 0, 0, DungeonRole.Floor));
            AddWall(Tiles, "wall", 1, 1, 0, 1);
            AddWall(Tiles, "wall_cracked", 1, 1, 0, 1);
            AddWall(Tiles, "wall_corner", 1, 0, 0, 1);
            AddWall(Tiles, "wall_endcap", 1, 1, 0, 1);
            AddWall(Tiles, "wall_Tsplit", 1, 0, 0, 0);
            AddWall(Tiles, "wall_crossing", 0, 0, 0, 0);
            AddFacing(Tiles, "wall_doorway", 1, 1, 0, 1, DungeonRole.Door);
            Tiles.Add(new DungeonTile("wall_doorway", 0, 0, 0, 0, DungeonRole.Door));
            Tiles.Add(new DungeonTile("stairs", 0, 0, 0, 0, DungeonRole.StairUp));
            Tiles.Add(new DungeonTile("stairs_walled", 0, 0, 0, 0, DungeonRole.StairUp));
            Tiles.Add(new DungeonTile("stairs", 0, 0, 0, 0, DungeonRole.StairDown));
            Tiles.Add(new DungeonTile("stairs_walled", 0, 0, 0, 0, DungeonRole.StairDown));
            Tiles.Add(new DungeonTile("stairs_modular_center", 0, 0, 0, 0, DungeonRole.Step));
            return Tiles;
        }

        static void AddWall(List<DungeonTile> Tiles, string Model, int North, int East, int South, int West)
        {
            AddFacing(Tiles, Model, North, East, South, West, DungeonRole.Wall);
        }

        static void AddFacing(List<DungeonTile> Tiles, string Model, int North, int East, int South, int West, DungeonRole Role)
        {
            int N = North;
            int E = East;
            int S = South;
            int W = West;
            for (int Turn = 0; Turn < 4; Turn++)
            {
                Tiles.Add(new DungeonTile(Model, N, E, S, W, Role));
                int NextN = W;
                int NextE = N;
                int NextS = E;
                int NextW = S;
                N = NextN;
                E = NextE;
                S = NextS;
                W = NextW;
            }
        }
    }

    public class DungeonFloor
    {
        public int Index;
        public int Width;
        public int Height;
        public DungeonCell[] Cells;
        public int EntryX;
        public int EntryY;
        public int DownX = -1;
        public int DownY = -1;
        public int UpX = -1;
        public int UpY = -1;
        public bool EntryLeaves;
        public readonly List<DungeonSpawn> Spawns = new List<DungeonSpawn>();

        public DungeonCell At(int X, int Y)
        {
            if (X < 0 || Y < 0 || X >= Width || Y >= Height || Cells == null)
            {
                return null;
            }

            return Cells[Y * Width + X];
        }

        public bool Reaches(int X0, int Y0, int X1, int Y1)
        {
            DungeonCell Start = At(X0, Y0);
            DungeonCell Goal = At(X1, Y1);
            if (Start == null || Goal == null || !Start.Walkable() || !Goal.Walkable())
            {
                return false;
            }

            bool[] Seen = new bool[Width * Height];
            int[] QueueX = new int[Width * Height];
            int[] QueueY = new int[Width * Height];
            int Head = 0;
            int Tail = 0;
            QueueX[Tail] = X0;
            QueueY[Tail] = Y0;
            Tail++;
            Seen[Y0 * Width + X0] = true;
            int[] DX = new int[] { 1, -1, 0, 0 };
            int[] DY = new int[] { 0, 0, 1, -1 };
            while (Head < Tail)
            {
                int X = QueueX[Head];
                int Y = QueueY[Head];
                Head++;
                if (X == X1 && Y == Y1)
                {
                    return true;
                }

                DungeonCell Here = At(X, Y);
                for (int i = 0; i < 4; i++)
                {
                    int NX = X + DX[i];
                    int NY = Y + DY[i];
                    if (NX < 0 || NY < 0 || NX >= Width || NY >= Height || Seen[NY * Width + NX])
                    {
                        continue;
                    }

                    DungeonCell Next = At(NX, NY);
                    if (!CanCross(Here, Next))
                    {
                        continue;
                    }

                    Seen[NY * Width + NX] = true;
                    QueueX[Tail] = NX;
                    QueueY[Tail] = NY;
                    Tail++;
                }
            }

            return false;
        }

        public static bool CanCross(DungeonCell From, DungeonCell To)
        {
            if (From == null || To == null || !From.Walkable() || !To.Walkable())
            {
                return false;
            }

            int Rise = From.Height - To.Height;
            if (Rise < 0)
            {
                Rise = -Rise;
            }

            if (Rise == 0)
            {
                return true;
            }

            return Rise == 1 && (From.Role == DungeonRole.Step || To.Role == DungeonRole.Step);
        }
    }

    public static class DungeonGenerator
    {
        public static DungeonFloor Build(int Seed, int Index, int Count, int Width, int Height, int LinkX, int LinkY)
        {
            if (Width < 16)
            {
                Width = 16;
            }

            if (Height < 16)
            {
                Height = 16;
            }

            DungeonFloor Floor = new DungeonFloor();
            Floor.Index = Index;
            Floor.Width = Width;
            Floor.Height = Height;
            Floor.Cells = new DungeonCell[Width * Height];
            for (int Y = 0; Y < Height; Y++)
            {
                for (int X = 0; X < Width; X++)
                {
                    DungeonCell Cell = new DungeonCell();
                    Cell.X = X;
                    Cell.Y = Y;
                    Floor.Cells[Y * Width + X] = Cell;
                }
            }

            Random Rng = new Random(Seed + Index * 97);
            List<Room> Rooms = PlaceRooms(Rng, Width, Height);
            for (int i = 0; i < Rooms.Count; i++)
            {
                Fill(Floor, Rooms[i]);
            }

            for (int i = 1; i < Rooms.Count; i++)
            {
                Hall(Floor, CenterX(Rooms[i - 1]), CenterY(Rooms[i - 1]), CenterX(Rooms[i]), CenterY(Rooms[i]));
            }

            if (Index == 0)
            {
                Floor.EntryX = CenterX(Rooms[0]);
                Floor.EntryY = CenterY(Rooms[0]);
                Floor.At(Floor.EntryX, Floor.EntryY).Role = DungeonRole.StairUp;
                Floor.UpX = Floor.EntryX;
                Floor.UpY = Floor.EntryY;
                Floor.EntryLeaves = true;
            }
            else
            {
                int X = LinkX;
                int Y = LinkY;
                if (Floor.At(X, Y) == null)
                {
                    X = CenterX(Rooms[0]);
                    Y = CenterY(Rooms[0]);
                }

                Hall(Floor, CenterX(Rooms[0]), CenterY(Rooms[0]), X, Y);
                DungeonCell Up = Floor.At(X, Y);
                Up.Role = DungeonRole.StairUp;
                Up.Height = 0;
                Floor.UpX = X;
                Floor.UpY = Y;
                Floor.EntryX = X;
                Floor.EntryY = Y;
            }

            if (Index < Count - 1)
            {
                Room Last = Rooms[Rooms.Count - 1];
                int X = CenterX(Last);
                int Y = CenterY(Last);
                if (X == Floor.UpX && Y == Floor.UpY)
                {
                    X = Last.X + 1;
                    Y = Last.Y + 1;
                }

                DungeonCell Down = Floor.At(X, Y);
                if (Down != null)
                {
                    Down.Role = DungeonRole.StairDown;
                    Down.Height = 0;
                    Floor.DownX = X;
                    Floor.DownY = Y;
                }
            }

            Raise(Floor, Rooms);
            if (Floor.DownX >= 0 && !Floor.Reaches(Floor.EntryX, Floor.EntryY, Floor.DownX, Floor.DownY))
            {
                Flatten(Floor);
                Hall(Floor, Floor.EntryX, Floor.EntryY, Floor.DownX, Floor.DownY);
                Floor.At(Floor.EntryX, Floor.EntryY).Role = DungeonRole.StairUp;
                Floor.At(Floor.DownX, Floor.DownY).Role = DungeonRole.StairDown;
            }

            Ring(Floor);
            MarkDoors(Floor);
            Collapse(Floor, Rng);
            return Floor;
        }

        static void Raise(DungeonFloor Floor, List<Room> Rooms)
        {
            if (Rooms.Count < 2)
            {
                return;
            }

            Room Room = Rooms[1];
            DungeonCell Door = null;
            for (int Y = Room.Y; Y < Room.Y + Room.H; Y++)
            {
                for (int X = Room.X; X < Room.X + Room.W; X++)
                {
                    DungeonCell Cell = Floor.At(X, Y);
                    if (Cell == null || Cell.Role == DungeonRole.StairUp || Cell.Role == DungeonRole.StairDown || !Cell.Walkable())
                    {
                        continue;
                    }

                    if (OutsideWalk(Floor, Room, X, Y))
                    {
                        Door = Cell;
                    }
                }
            }

            if (Door == null)
            {
                return;
            }

            Door.Role = DungeonRole.Step;
            Door.Height = 0;
            for (int Y = Room.Y; Y < Room.Y + Room.H; Y++)
            {
                for (int X = Room.X; X < Room.X + Room.W; X++)
                {
                    DungeonCell Cell = Floor.At(X, Y);
                    if (Cell == null || Cell == Door || !Cell.Walkable() || Cell.Role == DungeonRole.StairUp || Cell.Role == DungeonRole.StairDown)
                    {
                        continue;
                    }

                    Cell.Height = 1;
                }
            }
        }

        static bool OutsideWalk(DungeonFloor Floor, Room Room, int X, int Y)
        {
            int[] DX = new int[] { 1, -1, 0, 0 };
            int[] DY = new int[] { 0, 0, 1, -1 };
            for (int i = 0; i < 4; i++)
            {
                int NX = X + DX[i];
                int NY = Y + DY[i];
                if (NX >= Room.X && NY >= Room.Y && NX < Room.X + Room.W && NY < Room.Y + Room.H)
                {
                    continue;
                }

                DungeonCell Next = Floor.At(NX, NY);
                if (Next != null && Next.Walkable() && Next.Height == 0)
                {
                    return true;
                }
            }

            return false;
        }

        static void Flatten(DungeonFloor Floor)
        {
            for (int i = 0; i < Floor.Cells.Length; i++)
            {
                DungeonCell Cell = Floor.Cells[i];
                if (Cell.Role == DungeonRole.Step)
                {
                    Cell.Role = DungeonRole.Floor;
                }

                Cell.Height = 0;
            }
        }

        static void Ring(DungeonFloor Floor)
        {
            List<DungeonCell> Edge = new List<DungeonCell>();
            for (int i = 0; i < Floor.Cells.Length; i++)
            {
                DungeonCell Cell = Floor.Cells[i];
                if (Cell.Role != DungeonRole.Empty || !TouchesWalk(Floor, Cell.X, Cell.Y))
                {
                    continue;
                }

                Edge.Add(Cell);
            }

            for (int i = 0; i < Edge.Count; i++)
            {
                Edge[i].Role = DungeonRole.Wall;
            }
        }

        static bool TouchesWalk(DungeonFloor Floor, int X, int Y)
        {
            DungeonCell East = Floor.At(X + 1, Y);
            DungeonCell West = Floor.At(X - 1, Y);
            DungeonCell South = Floor.At(X, Y + 1);
            DungeonCell North = Floor.At(X, Y - 1);
            return (East != null && East.Walkable()) || (West != null && West.Walkable()) || (South != null && South.Walkable()) || (North != null && North.Walkable());
        }

        static void MarkDoors(DungeonFloor Floor)
        {
            for (int i = 0; i < Floor.Cells.Length; i++)
            {
                DungeonCell Cell = Floor.Cells[i];
                if (Cell.Role != DungeonRole.Floor || Cell.Height != 0)
                {
                    continue;
                }

                DungeonCell East = Floor.At(Cell.X + 1, Cell.Y);
                DungeonCell West = Floor.At(Cell.X - 1, Cell.Y);
                DungeonCell South = Floor.At(Cell.X, Cell.Y + 1);
                DungeonCell North = Floor.At(Cell.X, Cell.Y - 1);
                bool OpenEast = East != null && East.Walkable();
                bool OpenWest = West != null && West.Walkable();
                bool OpenSouth = South != null && South.Walkable();
                bool OpenNorth = North != null && North.Walkable();
                bool Corridor = (OpenEast && OpenWest && !OpenNorth && !OpenSouth) || (OpenNorth && OpenSouth && !OpenEast && !OpenWest);
                if (Corridor)
                {
                    Cell.Role = DungeonRole.Door;
                }
            }
        }

        static void Collapse(DungeonFloor Floor, Random Rng)
        {
            List<DungeonTile>[] Options = new List<DungeonTile>[Floor.Cells.Length];
            for (int i = 0; i < Floor.Cells.Length; i++)
            {
                DungeonCell Cell = Floor.Cells[i];
                if (Cell.Role == DungeonRole.Empty)
                {
                    continue;
                }

                Sockets(Floor, Cell);
                Options[i] = Fit(Cell);
            }

            int Guard = 0;
            while (Guard < Floor.Cells.Length * 4)
            {
                Guard++;
                int Best = -1;
                int BestCount = int.MaxValue;
                for (int i = 0; i < Options.Length; i++)
                {
                    if (Options[i] == null || Options[i].Count <= 1 || Options[i].Count >= BestCount)
                    {
                        continue;
                    }

                    Best = i;
                    BestCount = Options[i].Count;
                }

                if (Best < 0)
                {
                    break;
                }

                List<DungeonTile> Bag = Options[Best];
                DungeonTile Pick = Bag[Rng.Next(Bag.Count)];
                Bag.Clear();
                Bag.Add(Pick);
            }

            for (int i = 0; i < Floor.Cells.Length; i++)
            {
                if (Options[i] == null || Options[i].Count == 0)
                {
                    continue;
                }

                Floor.Cells[i].Model = Options[i][0].Model;
            }
        }

        static void Sockets(DungeonFloor Floor, DungeonCell Cell)
        {
            Cell.North = Socket(Floor, Cell, 0, -1);
            Cell.East = Socket(Floor, Cell, 1, 0);
            Cell.South = Socket(Floor, Cell, 0, 1);
            Cell.West = Socket(Floor, Cell, -1, 0);
        }

        static int Socket(DungeonFloor Floor, DungeonCell Cell, int DX, int DY)
        {
            DungeonCell Other = Floor.At(Cell.X + DX, Cell.Y + DY);
            if (Other == null || Other.Role == DungeonRole.Empty)
            {
                return Cell.Walkable() ? 0 : 1;
            }

            if (Cell.Walkable() && Other.Walkable())
            {
                return 0;
            }

            if (!Cell.Walkable() && !Other.Walkable())
            {
                return 1;
            }

            return 0;
        }

        static List<DungeonTile> Fit(DungeonCell Cell)
        {
            List<DungeonTile> Found = new List<DungeonTile>();
            for (int i = 0; i < DungeonTiles.All.Count; i++)
            {
                DungeonTile Tile = DungeonTiles.All[i];
                if (Tile.Fits(Cell))
                {
                    Found.Add(Tile);
                }
            }

            if (Found.Count == 0)
            {
                Found.Add(new DungeonTile(Fallback(Cell.Role), Cell.North, Cell.East, Cell.South, Cell.West, Cell.Role));
            }

            return Found;
        }

        static string Fallback(DungeonRole Role)
        {
            if (Role == DungeonRole.Wall)
            {
                return "wall";
            }

            if (Role == DungeonRole.Door)
            {
                return "wall_doorway";
            }

            if (Role == DungeonRole.Step)
            {
                return "stairs_modular_center";
            }

            if (Role == DungeonRole.StairUp || Role == DungeonRole.StairDown)
            {
                return "stairs";
            }

            return "floor_tile_small";
        }

        static List<Room> PlaceRooms(Random Rng, int Width, int Height)
        {
            List<Room> Rooms = new List<Room>();
            int Guard = 0;
            while (Rooms.Count < 8 && Guard < 120)
            {
                Guard++;
                int W = 6 + Rng.Next(4);
                int H = 6 + Rng.Next(4);
                int X = 2 + Rng.Next(Width - W - 4);
                int Y = 2 + Rng.Next(Height - H - 4);
                Room Next = new Room();
                Next.X = X;
                Next.Y = Y;
                Next.W = W;
                Next.H = H;
                bool Hit = false;
                for (int i = 0; i < Rooms.Count; i++)
                {
                    if (Overlaps(Rooms[i], Next))
                    {
                        Hit = true;
                    }
                }

                if (!Hit)
                {
                    Rooms.Add(Next);
                }
            }

            if (Rooms.Count == 0)
            {
                Room Only = new Room();
                Only.X = 2;
                Only.Y = 2;
                Only.W = 6;
                Only.H = 6;
                Rooms.Add(Only);
            }

            return Rooms;
        }

        static bool Overlaps(Room A, Room B)
        {
            return A.X < B.X + B.W + 1 && A.X + A.W + 1 > B.X && A.Y < B.Y + B.H + 1 && A.Y + A.H + 1 > B.Y;
        }

        static void Fill(DungeonFloor Floor, Room Room)
        {
            for (int Y = Room.Y; Y < Room.Y + Room.H; Y++)
            {
                for (int X = Room.X; X < Room.X + Room.W; X++)
                {
                    DungeonCell Cell = Floor.At(X, Y);
                    if (Cell != null && Cell.Role == DungeonRole.Empty)
                    {
                        Cell.Role = DungeonRole.Floor;
                    }
                }
            }
        }

        static void Hall(DungeonFloor Floor, int X0, int Y0, int X1, int Y1)
        {
            int X = X0;
            int Y = Y0;
            Stamp(Floor, X, Y);
            while (X != X1)
            {
                X += X < X1 ? 1 : -1;
                Stamp(Floor, X, Y);
            }

            while (Y != Y1)
            {
                Y += Y < Y1 ? 1 : -1;
                Stamp(Floor, X, Y);
            }
        }

        static void Stamp(DungeonFloor Floor, int X, int Y)
        {
            DungeonCell Cell = Floor.At(X, Y);
            if (Cell == null)
            {
                return;
            }

            if (Cell.Role == DungeonRole.Empty || Cell.Role == DungeonRole.Wall)
            {
                Cell.Role = DungeonRole.Floor;
                Cell.Height = 0;
            }
        }

        static int CenterX(Room Room)
        {
            return Room.X + Room.W / 2;
        }

        static int CenterY(Room Room)
        {
            return Room.Y + Room.H / 2;
        }

        struct Room
        {
            public int X;
            public int Y;
            public int W;
            public int H;
        }
    }
}
