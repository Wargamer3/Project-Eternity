using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class WorldGenerator
    {
        public enum Phase
        {
            Height,
            Water,
            Forest,
            City,
            Write,
            Done,
        }

        public const int Chunks = 24;
        public const int MinChunk = -12;

        public WorldSlot Slot;
        public Phase StepPhase;
        public int Cursor;
        public string Status = "Starting";
        public bool PictureDirty = true;

        int Samples;
        int Global;
        float[,] Heights;
        float[,] Moisture;
        Color[] Picture;
        bool WaterReady;
        List<WaterVolume>[] ChunkWater;
        bool[] WaterCells;
        float[] WaterSurface;
        List<BuildingItem>[] ChunkBuildings;
        ForestRecipe[] ChunkForest;
        int Seed;

        public static WorldGenerator Begin()
        {
            return Begin(Environment.TickCount);
        }

        public static WorldGenerator Begin(int Seed)
        {
            WorldGenerator Generator = new WorldGenerator();
            Generator.Seed = Seed;
            Generator.Slot = WorldSlot.CreateEmpty(WorldSlot.NextIndex());
            Generator.Slot.Seed = Seed;
            Generator.Slot.Spacing = 16f;
            Generator.Samples = Math.Max(2, (int)Math.Round(Generator.Slot.ChunkSize / Generator.Slot.Spacing) + 1);
            Generator.Global = Chunks * (Generator.Samples - 1) + 1;
            Generator.Heights = new float[Generator.Global, Generator.Global];
            Generator.Moisture = new float[Generator.Global, Generator.Global];
            Generator.Picture = new Color[Generator.Global * Generator.Global];
            for (int i = 0; i < Generator.Picture.Length; i++)
            {
                Generator.Picture[i] = new Color(20, 24, 18);
            }

            int Count = Chunks * Chunks;
            Generator.ChunkWater = new List<WaterVolume>[Count];
            Generator.ChunkBuildings = new List<BuildingItem>[Count];
            Generator.ChunkForest = new ForestRecipe[Count];
            for (int i = 0; i < Count; i++)
            {
                Generator.ChunkWater[i] = new List<WaterVolume>();
                Generator.ChunkBuildings[i] = new List<BuildingItem>();
                Generator.ChunkForest[i] = new ForestRecipe();
            }

            return Generator;
        }

        public bool Step()
        {
            if (StepPhase == Phase.Done)
            {
                return true;
            }

            if (StepPhase == Phase.Height)
            {
                FillChunk(Cursor);
                Status = "Land " + (Cursor + 1).ToString() + "/" + (Chunks * Chunks).ToString();
                Cursor++;
                PictureDirty = true;
                if (Cursor >= Chunks * Chunks)
                {
                    StepPhase = Phase.Water;
                    Cursor = 0;
                }

                return false;
            }

            if (StepPhase == Phase.Water)
            {
                if (!WaterReady)
                {
                    BuildWater();
                    WaterReady = true;
                    Status = "Water";
                    PictureDirty = true;
                    return false;
                }

                PaintWater(Cursor);
                Status = "Water " + (Cursor + 1).ToString() + "/" + (Chunks * Chunks).ToString();
                Cursor++;
                PictureDirty = true;
                if (Cursor >= Chunks * Chunks)
                {
                    StepPhase = Phase.Forest;
                    Cursor = 0;
                }

                return false;
            }

            if (StepPhase == Phase.Forest)
            {
                FillForest(Cursor);
                Status = "Forest " + (Cursor + 1).ToString() + "/" + (Chunks * Chunks).ToString();
                Cursor++;
                if (Cursor >= Chunks * Chunks)
                {
                    StepPhase = Phase.City;
                    Cursor = 0;
                }

                return false;
            }

            if (StepPhase == Phase.City)
            {
                BuildCities();
                Status = "Cities";
                StepPhase = Phase.Write;
                Cursor = 0;
                PictureDirty = true;
                Slot.Write();
                return false;
            }

            WriteChunk(Cursor);
            Status = "Save " + (Cursor + 1).ToString() + "/" + (Chunks * Chunks).ToString();
            Cursor++;
            if (Cursor >= Chunks * Chunks)
            {
                WritePicture();
                StepPhase = Phase.Done;
                Status = "Save " + Slot.Index.ToString() + "  " + Seed.ToString();
            }

            return StepPhase == Phase.Done;
        }

        public Texture2D Preview(GraphicsDevice Device)
        {
            if (Device == null || Picture == null)
            {
                return null;
            }

            if (Live == null || Live.Width != Global || Live.GraphicsDevice != Device)
            {
                Live = new Texture2D(Device, Global, Global, false, SurfaceFormat.Color);
                PictureDirty = true;
            }

            if (PictureDirty)
            {
                Live.SetData(Picture);
                PictureDirty = false;
            }

            return Live;
        }

        Texture2D Live;

        int ChunkXOf(int Index)
        {
            return MinChunk + Index % Chunks;
        }

        int ChunkZOf(int Index)
        {
            return MinChunk + Index / Chunks;
        }

        bool Holds(int ChunkX, int ChunkZ)
        {
            return ChunkX >= MinChunk && ChunkZ >= MinChunk && ChunkX < MinChunk + Chunks && ChunkZ < MinChunk + Chunks;
        }

        int Pack(int ChunkX, int ChunkZ)
        {
            return (ChunkZ - MinChunk) * Chunks + (ChunkX - MinChunk);
        }

        int Cell(int Chunk, int Local)
        {
            return (Chunk - MinChunk) * (Samples - 1) + Local;
        }

        float ToWorld(int CellIndex)
        {
            return MinChunk * Slot.ChunkSize + CellIndex * Slot.Spacing;
        }

        int FromWorld(float World)
        {
            int Index = (int)Math.Round((World - MinChunk * Slot.ChunkSize) / Slot.Spacing);
            if (Index < 0)
            {
                return 0;
            }

            if (Index > Global - 1)
            {
                return Global - 1;
            }

            return Index;
        }

        int ChunkOf(float World)
        {
            int Chunk = (int)Math.Floor(World / Slot.ChunkSize);
            if (Chunk < MinChunk)
            {
                return MinChunk;
            }

            int Last = MinChunk + Chunks - 1;
            if (Chunk > Last)
            {
                return Last;
            }

            return Chunk;
        }

        void FillChunk(int Index)
        {
            int ChunkX = ChunkXOf(Index);
            int ChunkZ = ChunkZOf(Index);
            for (int Z = 0; Z < Samples; Z++)
            {
                for (int X = 0; X < Samples; X++)
                {
                    int GX = Cell(ChunkX, X);
                    int GZ = Cell(ChunkZ, Z);
                    float WorldX = ToWorld(GX);
                    float WorldZ = ToWorld(GZ);
                    float Height;
                    float Wet;
                    SampleClimate(WorldX, WorldZ, out Height, out Wet);
                    Heights[GX, GZ] = Height;
                    Moisture[GX, GZ] = Wet;
                    Picture[GZ * Global + GX] = LandColor(Height, Wet, 0f);
                }
            }
        }

        void BuildWater()
        {
            EaseSeams();
            int[] Down = new int[Global * Global];
            float[] Accum = new float[Global * Global];
            int[] Order = new int[Global * Global];
            for (int i = 0; i < Order.Length; i++)
            {
                Order[i] = i;
                Accum[i] = 1f;
                int X = i % Global;
                int Z = i / Global;
                Down[i] = LowestNeighbor(X, Z);
            }

            Array.Sort(Order, delegate (int A, int B)
            {
                return Heights[B % Global, B / Global].CompareTo(Heights[A % Global, A / Global]);
            });

            for (int i = 0; i < Order.Length; i++)
            {
                int At = Order[i];
                int Next = Down[At];
                if (Next != At)
                {
                    Accum[Next] += Accum[At];
                }
            }

            bool[] Seen = new bool[Global * Global];
            bool[] Lake = new bool[Global * Global];
            WaterCells = new bool[Global * Global];
            WaterSurface = new float[Global * Global];
            for (int i = 0; i < WaterSurface.Length; i++)
            {
                WaterSurface[i] = float.NegativeInfinity;
            }
            float[] LakeSurface = new float[Global * Global];
            for (int i = 0; i < LakeSurface.Length; i++)
            {
                LakeSurface[i] = float.NegativeInfinity;
            }

            for (int i = 0; i < Down.Length; i++)
            {
                if (Down[i] == i)
                {
                    FloodSink(i, Seen, Lake, LakeSurface);
                }
            }

            TraceRivers(Down, Accum, Lake, LakeSurface);
            Relax();
            Recolor();
        }

        void FloodSink(int Index, bool[] Seen, bool[] Lake, float[] LakeSurface)
        {
            if (Seen[Index])
            {
                return;
            }

            int X = Index % Global;
            int Z = Index / Global;
            float Floor = Heights[X, Z];
            if (Floor < Ocean.Level)
            {
                return;
            }

            const float MaxDepth = 8f;
            const int MaxCells = 140;
            List<int> Cells = new List<int>();
            Queue<int> Open = new Queue<int>();
            Open.Enqueue(Index);
            Seen[Index] = true;
            bool Spilled = false;
            while (Open.Count > 0 && Cells.Count < MaxCells)
            {
                int At = Open.Dequeue();
                Cells.Add(At);
                int AX = At % Global;
                int AZ = At / Global;
                for (int DZ = -1; DZ <= 1; DZ++)
                {
                    for (int DX = -1; DX <= 1; DX++)
                    {
                        if (DX == 0 && DZ == 0)
                        {
                            continue;
                        }

                        int NX = AX + DX;
                        int NZ = AZ + DZ;
                        if (NX < 0 || NZ < 0 || NX >= Global || NZ >= Global)
                        {
                            Spilled = true;
                            continue;
                        }

                        if (Heights[NX, NZ] < Floor - 0.2f)
                        {
                            Spilled = true;
                        }

                        int Next = NZ * Global + NX;
                        if (Seen[Next] || Heights[NX, NZ] > Floor + MaxDepth)
                        {
                            continue;
                        }

                        Seen[Next] = true;
                        Open.Enqueue(Next);
                    }
                }
            }

            if (Spilled || Cells.Count < 8 || Cells.Count >= MaxCells)
            {
                return;
            }

            float Rim = float.MaxValue;
            for (int i = 0; i < Cells.Count; i++)
            {
                int AX = Cells[i] % Global;
                int AZ = Cells[i] / Global;
                for (int DZ = -1; DZ <= 1; DZ++)
                {
                    for (int DX = -1; DX <= 1; DX++)
                    {
                        if (DX == 0 && DZ == 0)
                        {
                            continue;
                        }

                        int NX = AX + DX;
                        int NZ = AZ + DZ;
                        if (NX < 0 || NZ < 0 || NX >= Global || NZ >= Global)
                        {
                            continue;
                        }

                        int Next = NZ * Global + NX;
                        if (Seen[Next] && Heights[NX, NZ] <= Floor + MaxDepth)
                        {
                            continue;
                        }

                        if (Heights[NX, NZ] < Rim)
                        {
                            Rim = Heights[NX, NZ];
                        }
                    }
                }
            }

            if (Rim > 100000f || Rim < Floor + 1.4f)
            {
                return;
            }

            float Surface = Rim - 0.6f;
            List<int> Pool = new List<int>();
            for (int i = 0; i < Cells.Count; i++)
            {
                int CX = Cells[i] % Global;
                int CZ = Cells[i] / Global;
                if (Heights[CX, CZ] <= Surface)
                {
                    Pool.Add(Cells[i]);
                }
            }

            if (Pool.Count < 6)
            {
                return;
            }

            CutRidges(Pool, Surface);
            SinkBowl(Pool, Surface);
            WaterVolume Volume = new WaterVolume();
            Volume.Kind = WaterKind.Lake;
            Volume.Look = Meltwater(X, Z, Surface);
            Volume.Cell = Slot.Spacing;
            Volume.Depth = 2.6f;
            Volume.Start.Z = Surface;
            float MinX = float.MaxValue;
            float MinY = float.MaxValue;
            float MaxX = float.MinValue;
            float MaxY = float.MinValue;
            for (int i = 0; i < Pool.Count; i++)
            {
                int CX = Pool[i] % Global;
                int CZ = Pool[i] / Global;
                Vector3 At = WorldAt(CX, CZ);
                MinX = Math.Min(MinX, At.X);
                MinY = Math.Min(MinY, At.Y);
                MaxX = Math.Max(MaxX, At.X);
                MaxY = Math.Max(MaxY, At.Y);
                Lake[Pool[i]] = true;
                LakeSurface[Pool[i]] = Surface;
                MarkWater(Pool[i], Surface);
            }

            Volume.Start = new Vector3((MinX + MaxX) * 0.5f, (MinY + MaxY) * 0.5f, Surface);
            Volume.End = Volume.Start;
            Volume.Radius = Math.Max(MaxX - MinX, MaxY - MinY) * 0.5f + Slot.Spacing;
            Volume.Width = Volume.Radius * 2f;
            Volume.LakeCells = PackLake(Pool);
            GiveWater(Volume);
        }

        void CutRidges(List<int> Pool, float Surface)
        {
            bool[] Wet = new bool[Global * Global];
            int MinX = Global;
            int MinZ = Global;
            int MaxX = 0;
            int MaxZ = 0;
            for (int i = 0; i < Pool.Count; i++)
            {
                int At = Pool[i];
                Wet[At] = true;
                int X = At % Global;
                int Z = At / Global;
                if (X < MinX)
                {
                    MinX = X;
                }

                if (Z < MinZ)
                {
                    MinZ = Z;
                }

                if (X > MaxX)
                {
                    MaxX = X;
                }

                if (Z > MaxZ)
                {
                    MaxZ = Z;
                }
            }

            const int Reach = 4;
            const float Lip = 8f;
            for (int Pass = 0; Pass < 8; Pass++)
            {
                List<int> Add = new List<int>();
                int X0 = Math.Max(0, MinX - Reach);
                int X1 = Math.Min(Global - 1, MaxX + Reach);
                int Z0 = Math.Max(0, MinZ - Reach);
                int Z1 = Math.Min(Global - 1, MaxZ + Reach);
                for (int Z = Z0; Z <= Z1; Z++)
                {
                    for (int X = X0; X <= X1; X++)
                    {
                        int At = Z * Global + X;
                        if (Wet[At] || Heights[X, Z] > Surface + Lip)
                        {
                            continue;
                        }

                        bool Across = Bridges(Wet, X, Z, 1, 0, Reach) || Bridges(Wet, X, Z, 0, 1, Reach);
                        if (!Across)
                        {
                            Across = Bridges(Wet, X, Z, 1, 1, Reach) || Bridges(Wet, X, Z, 1, -1, Reach);
                        }

                        if (Across)
                        {
                            Add.Add(At);
                        }
                    }
                }

                if (Add.Count == 0)
                {
                    return;
                }

                for (int i = 0; i < Add.Count; i++)
                {
                    int At = Add[i];
                    Wet[At] = true;
                    int X = At % Global;
                    int Z = At / Global;
                    if (Heights[X, Z] > Surface - 1f)
                    {
                        Heights[X, Z] = Surface - 1f;
                    }

                    if (X < MinX)
                    {
                        MinX = X;
                    }

                    if (Z < MinZ)
                    {
                        MinZ = Z;
                    }

                    if (X > MaxX)
                    {
                        MaxX = X;
                    }

                    if (Z > MaxZ)
                    {
                        MaxZ = Z;
                    }

                    Pool.Add(At);
                }
            }
        }

        bool Bridges(bool[] Wet, int X, int Z, int DX, int DZ, int Reach)
        {
            bool LowA = false;
            bool LowB = false;
            for (int Step = 1; Step <= Reach; Step++)
            {
                if (WetAt(Wet, X - DX * Step, Z - DZ * Step))
                {
                    LowA = true;
                }

                if (WetAt(Wet, X + DX * Step, Z + DZ * Step))
                {
                    LowB = true;
                }
            }

            return LowA && LowB;
        }

        bool WetAt(bool[] Wet, int X, int Z)
        {
            if (X < 0 || Z < 0 || X >= Global || Z >= Global)
            {
                return false;
            }

            return Wet[Z * Global + X];
        }

        void SinkBowl(List<int> Pool, float Surface)
        {
            bool[] In = new bool[Global * Global];
            for (int i = 0; i < Pool.Count; i++)
            {
                In[Pool[i]] = true;
            }

            int[] Dist = new int[Global * Global];
            int[] Queue = new int[Pool.Count];
            int Head = 0;
            int Tail = 0;
            for (int i = 0; i < Dist.Length; i++)
            {
                Dist[i] = -1;
            }

            for (int i = 0; i < Pool.Count; i++)
            {
                int At = Pool[i];
                int X = At % Global;
                int Z = At / Global;
                bool Edge = false;
                for (int Side = 0; Side < 4; Side++)
                {
                    int NX = X + (Side == 0 ? 1 : Side == 1 ? -1 : 0);
                    int NZ = Z + (Side == 2 ? 1 : Side == 3 ? -1 : 0);
                    if (NX < 0 || NZ < 0 || NX >= Global || NZ >= Global || !In[NZ * Global + NX])
                    {
                        Edge = true;
                        break;
                    }
                }

                if (Edge)
                {
                    Dist[At] = 0;
                    Queue[Tail++] = At;
                }
            }

            while (Head < Tail)
            {
                int At = Queue[Head++];
                int X = At % Global;
                int Z = At / Global;
                for (int Side = 0; Side < 4; Side++)
                {
                    int NX = X + (Side == 0 ? 1 : Side == 1 ? -1 : 0);
                    int NZ = Z + (Side == 2 ? 1 : Side == 3 ? -1 : 0);
                    if (NX < 0 || NZ < 0 || NX >= Global || NZ >= Global)
                    {
                        continue;
                    }

                    int Next = NZ * Global + NX;
                    if (!In[Next] || Dist[Next] >= 0)
                    {
                        continue;
                    }

                    Dist[Next] = Dist[At] + 1;
                    Queue[Tail++] = Next;
                }
            }

            float Bed = Surface - 6f;
            const int Ramp = 3;
            for (int i = 0; i < Pool.Count; i++)
            {
                int At = Pool[i];
                int X = At % Global;
                int Z = At / Global;
                int Steps = Dist[At] < 0 ? Ramp : Dist[At];
                float Target = Bed;
                if (Steps < Ramp)
                {
                    float Shore = Heights[X, Z];
                    Target = Shore + (Bed - Shore) * ((Steps + 1) / (float)(Ramp + 1));
                }

                if (Heights[X, Z] > Target)
                {
                    Heights[X, Z] = Target;
                }
            }
        }

        int[] PackLake(List<int> Cells)
        {
            int[] Packed = new int[Cells.Count];
            for (int i = 0; i < Cells.Count; i++)
            {
                int X = Cells[i] % Global;
                int Z = Cells[i] / Global;
                Packed[i] = X | (Z << 16);
            }

            return Packed;
        }

        void GiveWater(WaterVolume Volume)
        {
            float Margin = Math.Max(Volume.Radius, Volume.Width);
            int Lo = MinChunk;
            int Hi = MinChunk + Chunks - 1;
            int MinX = Math.Max(Lo, ChunkOf(Math.Min(Volume.Start.X, Volume.End.X) - Margin));
            int MaxX = Math.Min(Hi, ChunkOf(Math.Max(Volume.Start.X, Volume.End.X) + Margin));
            int MinZ = Math.Max(Lo, ChunkOf(Math.Min(Volume.Start.Y, Volume.End.Y) - Margin));
            int MaxZ = Math.Min(Hi, ChunkOf(Math.Max(Volume.Start.Y, Volume.End.Y) + Margin));
            for (int Z = MinZ; Z <= MaxZ; Z++)
            {
                for (int X = MinX; X <= MaxX; X++)
                {
                    WaterVolume Local = CopyWater(Volume, X, Z);
                    if (Local != null)
                    {
                        ChunkWater[Pack(X, Z)].Add(Local);
                    }
                }
            }
        }

        WaterVolume CopyWater(WaterVolume Source, int ChunkX, int ChunkZ)
        {
            Vector3 Origin = new Vector3(ChunkX * Slot.ChunkSize, ChunkZ * Slot.ChunkSize, 0f);
            WaterVolume Copy = new WaterVolume();
            Copy.Kind = Source.Kind;
            Copy.Look = Source.Look;
            Copy.Start = Source.Start - Origin;
            Copy.End = Source.End - Origin;
            Copy.Radius = Source.Radius;
            Copy.Width = Source.Width;
            Copy.Depth = Source.Depth;
            Copy.Speed = Source.Speed;
            Copy.Cell = Source.Cell;
            if (Source.Kind == WaterKind.Lake && Source.LakeCells != null)
            {
                List<int> Local = new List<int>();
                int Step = Samples - 1;
                int BaseX = (ChunkX - MinChunk) * Step;
                int BaseZ = (ChunkZ - MinChunk) * Step;
                for (int i = 0; i < Source.LakeCells.Length; i++)
                {
                    int GX = Source.LakeCells[i] & 65535;
                    int GZ = Source.LakeCells[i] >> 16;
                    int LocalX = GX - BaseX;
                    int LocalZ = GZ - BaseZ;
                    if (LocalX >= 0 && LocalZ >= 0 && LocalX < Samples && LocalZ < Samples)
                    {
                        Local.Add(LocalX | (LocalZ << 16));
                    }
                }

                if (Local.Count == 0)
                {
                    return null;
                }

                Copy.LakeCells = Local.ToArray();
            }

            return Copy;
        }

        void PaintWater(int Index)
        {
            List<WaterVolume> Volumes = ChunkWater[Index];
            int ChunkX = ChunkXOf(Index);
            int ChunkZ = ChunkZOf(Index);
            for (int i = 0; i < Volumes.Count; i++)
            {
                WaterVolume Volume = Volumes[i];
                for (int Z = 0; Z < Samples; Z++)
                {
                    for (int X = 0; X < Samples; X++)
                    {
                        float LocalX = X * Slot.Spacing;
                        float LocalY = Z * Slot.Spacing;
                        float Surface;
                        Vector2 Flow;
                        if (!Volume.Footprint(LocalX, LocalY, out Surface, out Flow))
                        {
                            continue;
                        }

                        int GX = Cell(ChunkX, X);
                        int GZ = Cell(ChunkZ, Z);
                        if (GX >= 0 && GZ >= 0 && GX < Global && GZ < Global)
                        {
                            Picture[GZ * Global + GX] = new Color(40, 90, 170);
                        }
                    }
                }
            }
        }

        void FillForest(int Index)
        {
            ForestRecipe Recipe = ChunkForest[Index];
            Recipe.Seed = Seed + Index * 17;
            Recipe.Density = 0.7f;
            Recipe.MaxSlope = 0.62f;
            Recipe.MinHeight = -20f;
            Recipe.MaxHeight = 64f;
            Recipe.SetName = "CommonTree";
        }

        void BuildCities()
        {
            TownBook Book = new TownBook();
            Book.WorldIndex = Slot.Index;
            List<Vector2> Sites = new List<Vector2>();
            int Towns = 12;
            float Keep = TownKeep();
            int Margin = Math.Max(4, (int)Math.Ceiling(Keep / Slot.Spacing));
            int Span = Global - Margin * 2;
            if (Span < 1)
            {
                Span = 1;
            }

            for (int n = 0; n < Towns * 200 && Sites.Count < Towns; n++)
            {
                int X = Margin + (Hash(n, 3) % Span);
                int Z = Margin + (Hash(n, 9) % Span);
                float Slope = SlopeAt(X, Z);
                if (!ClearOfWater(X, Z, Keep))
                {
                    continue;
                }

                if (Heights[X, Z] < Ocean.Level + 3f || Heights[X, Z] > 36f || Slope > 0.42f || Moisture[X, Z] > 0.82f)
                {
                    continue;
                }

                Vector2 Site = new Vector2(ToWorld(X), ToWorld(Z));
                bool Far = true;
                for (int s = 0; s < Sites.Count; s++)
                {
                    if (Vector2.Distance(Sites[s], Site) < Slot.ChunkSize * 5f)
                    {
                        Far = false;
                        break;
                    }
                }

                if (Far)
                {
                    Sites.Add(Site);
                }
            }

            List<Edge> Edges = Delaunay.Edges(Sites);
            if (Edges.Count == 0)
            {
                for (int i = 0; i < Sites.Count; i++)
                {
                    int Near = -1;
                    float Best = float.MaxValue;
                    for (int j = 0; j < Sites.Count; j++)
                    {
                        if (i == j)
                        {
                            continue;
                        }

                        float Distance = Vector2.DistanceSquared(Sites[i], Sites[j]);
                        if (Distance < Best)
                        {
                            Best = Distance;
                            Near = j;
                        }
                    }

                    if (Near > i)
                    {
                        Edge Fallback = new Edge();
                        Fallback.A = i;
                        Fallback.B = Near;
                        Fallback.Length = (float)Math.Sqrt(Best);
                        Edges.Add(Fallback);
                    }
                }
            }

            List<Edge> Roads = Spanning.Keep(Edges, Sites.Count);
            List<float>[] Mouths = new List<float>[Sites.Count];
            for (int i = 0; i < Sites.Count; i++)
            {
                Mouths[i] = new List<float>();
            }

            for (int i = 0; i < Roads.Count; i++)
            {
                Vector2 Delta = Sites[Roads[i].B] - Sites[Roads[i].A];
                float Angle = (float)Math.Atan2(Delta.X, -Delta.Y);
                Mouths[Roads[i].A].Add(Angle);
                Mouths[Roads[i].B].Add(Angle + (float)Math.PI);
            }

            bool[] Used = new bool[12];
            int Spin = Hash(Seed, 23);
            float[] Reach = new float[Sites.Count];
            for (int i = 0; i < Sites.Count; i++)
            {
                int Tier = (Spin + i * 5) % 12;
                int Guard = 0;
                while (Used[Tier] && Guard < 12)
                {
                    Tier = (Tier + 1) % 12;
                    Guard++;
                }

                Used[Tier] = true;
                int Houses;
                int Works;
                TownSize(Tier, out Houses, out Works);
                float Heading = Mouths[i].Count > 0 ? Mouths[i][0] : (Hash(Seed, i + 11) % 628) / 100f;
                Reach[i] = StampCity(Book, i, Sites[i], Houses, Works, Heading, Mouths[i]);
            }

            for (int i = 0; i < Roads.Count; i++)
            {
                StampRoad(Sites[Roads[i].A], Sites[Roads[i].B], Reach[Roads[i].A], Reach[Roads[i].B]);
                Book.Link(Roads[i].A, Roads[i].B);
            }

            Book.Save(Slot.CitiesPath);
        }

        static void TownSize(int Tier, out int Houses, out int Works)
        {
            switch (Tier)
            {
                case 0: Houses = 2; Works = 0; break;
                case 1: Houses = 3; Works = 0; break;
                case 2: Houses = 3; Works = 1; break;
                case 3: Houses = 4; Works = 1; break;
                case 4: Houses = 5; Works = 1; break;
                case 5: Houses = 6; Works = 1; break;
                case 6: Houses = 6; Works = 2; break;
                case 7: Houses = 7; Works = 2; break;
                case 8: Houses = 8; Works = 2; break;
                case 9: Houses = 8; Works = 3; break;
                case 10: Houses = 10; Works = 3; break;
                default: Houses = 10; Works = 4; break;
            }
        }

        float StampCity(TownBook Book, int Index, Vector2 Site, int Houses, int Works, float Heading, List<float> Mouths)
        {
            Vector2 Along = new Vector2((float)Math.Sin(Heading), (float)-Math.Cos(Heading));
            Vector2 Side = new Vector2(Along.Y, -Along.X);
            bool Street = Houses >= 5;
            string Root = Path.Combine(WorldSlot.ModelsRoot(), "Medieval Village Pack");
            string Props = Path.Combine(Root, "Props");
            string[] HomeNames = new string[] { "House_1", "House_2", "House_3", "House_4" };
            string[] WorkNames = new string[] { "Inn", "Blacksmith", "Mill", "Stable", "Sawmill", "Bell_Tower" };
            int WorkShift = Hash(Seed, Index + 8) % WorkNames.Length;
            List<Vector2> Spots = new List<Vector2>();
            List<float> Faces = new List<float>();
            List<string> Models = new List<string>();
            if (Street)
            {
                LayStreet(Site, Along, Side, Houses, Works, WorkShift, HomeNames, WorkNames, Root, Spots, Faces, Models);
            }
            else
            {
                LayRing(Site, Index, Houses, Works, WorkShift, HomeNames, WorkNames, Root, Spots, Faces, Models);
            }

            float Furthest = 80f;
            for (int i = 0; i < Spots.Count; i++)
            {
                Furthest = Math.Max(Furthest, Vector2.Distance(Site, Spots[i]));
            }

            Furthest += 70f;
            FlattenPad(Site.X, Site.Y, Furthest * 0.82f, Furthest + 36f);
            QuietForest(Site, Furthest + 48f);
            for (int i = 0; i < Spots.Count; i++)
            {
                PlaceModel(Spots[i], Models[i], 4f, 56f, Faces[i]);
            }

            Vector2 Well = Street ? Site + Side * 48f : Site;
            PlaceModel(Well, Path.Combine(Props, "Well.fbx"), 2.2f, 28f, Heading);
            if (Houses >= 3)
            {
                PlaceModel(Well + Along * 34f, Path.Combine(Props, "Bench_1.fbx"), 1f, 12f, Heading);
            }

            if (Houses >= 5)
            {
                PlaceModel(Well - Along * 36f + Side * 16f, Path.Combine(Props, "MarketStand_1.fbx"), 2f, 24f, Heading + (float)Math.PI);
            }

            if (Houses >= 8)
            {
                PlaceModel(Well + Along * 36f - Side * 28f, Path.Combine(Props, "Cart.fbx"), 2f, 18f, Heading);
                PlaceModel(Well - Side * 22f, Path.Combine(Props, "Crate.fbx"), 1f, 10f, Heading);
            }

            if (Street)
            {
                LaySpan(Site - Along * Furthest, Site + Along * Furthest);
            }

            for (int m = 0; m < Mouths.Count; m++)
            {
                float Turn = Mouths[m];
                if (Street && Math.Abs(WrapAngle(Turn - Heading)) < 0.4f)
                {
                    continue;
                }

                if (Street && Math.Abs(WrapAngle(Turn - Heading - (float)Math.PI)) < 0.4f)
                {
                    continue;
                }

                Vector2 Way = new Vector2((float)Math.Sin(Turn), (float)-Math.Cos(Turn));
                float Inner = Street ? 48f : Furthest * 0.38f;
                LaySpan(Site + Way * Inner, Site + Way * Furthest);
            }

            int Town = Book.AddTown(TownBook.TownName(Book.Towns.Count), Site.X, Site.Y, SampleWorld(Site.X, Site.Y), ChunkOf(Site.X), ChunkOf(Site.Y));
            Vector2 Keeper = Well + Along * 40f;
            Book.Admit(StarterAt(Index), Town, Keeper.X, Keeper.Y, true);
            int Extras = Math.Max(1, Houses / 2);
            for (int e = 0; e < Extras; e++)
            {
                float Sign = e % 2 == 0 ? 1f : -1f;
                Vector2 At = Site + Along * ((52f + (e / 2) * 44f) * Sign);
                Book.Admit(TownBook.NeighborName(Hash(Seed, Index * 19 + e + 3)), Town, At.X, At.Y, false);
            }

            PaintDisc(Site, Furthest * 0.5f, new Color(86, 78, 64));
            return Furthest;
        }

        static string StarterAt(int Index)
        {
            string[] Names = new string[]
            {
                "Wren", "Ada", "Bea", "Cora", "Nell", "Ida", "Mae", "Ruth", "Lark", "Fern", "Joy", "Tess",
            };
            return Names[Math.Abs(Index) % Names.Length];
        }

        void LayRing(Vector2 Site, int Index, int Houses, int Works, int WorkShift, string[] HomeNames, string[] WorkNames, string Root, List<Vector2> Spots, List<float> Faces, List<string> Models)
        {
            int Slots = Houses + Works;
            float Ring = Math.Max(120f, Slots * 46f);
            float Spin = (Hash(Seed, Index + 4) % 1000) / 1000f * (float)(Math.PI * 2.0);
            for (int i = 0; i < Slots; i++)
            {
                float Turn = Spin + i * (float)(Math.PI * 2.0) / Slots;
                Vector2 Offset = new Vector2((float)Math.Sin(Turn), (float)-Math.Cos(Turn)) * Ring;
                Spots.Add(Site + Offset);
                Faces.Add((float)Math.Atan2(-Offset.X, Offset.Y));
                if (i >= Houses)
                {
                    Models.Add(Path.Combine(Root, WorkNames[(WorkShift + i - Houses) % WorkNames.Length] + ".fbx"));
                }
                else
                {
                    Models.Add(Path.Combine(Root, HomeNames[(Hash(Seed, Index * 3 + i) + i) % HomeNames.Length] + ".fbx"));
                }
            }
        }

        void LayStreet(Vector2 Site, Vector2 Along, Vector2 Side, int Houses, int Works, int WorkShift, string[] HomeNames, string[] WorkNames, string Root, List<Vector2> Spots, List<float> Faces, List<string> Models)
        {
            int Left = (Houses + 1) / 2;
            int Right = Houses / 2;
            float Lot = 108f;
            float Gap = 100f;
            float Mid = (Math.Max(Left, Right) - 1) * 0.5f;
            for (int i = 0; i < Houses; i++)
            {
                bool OnLeft = i < Left;
                int Slot = OnLeft ? i : i - Left;
                float AlongAt = (Slot - Mid) * Lot;
                float SideSign = OnLeft ? -1f : 1f;
                Spots.Add(Site + Along * AlongAt + Side * (Gap * SideSign));
                Vector2 FaceDir = Side * -SideSign;
                Faces.Add((float)Math.Atan2(FaceDir.X, -FaceDir.Y));
                Models.Add(Path.Combine(Root, HomeNames[(Hash(Seed, i + WorkShift) + Slot) % HomeNames.Length] + ".fbx"));
            }

            float End = Mid * Lot + 118f;
            for (int w = 0; w < Works; w++)
            {
                float AlongAt = (w % 2 == 0 ? -1f : 1f) * End;
                float SideSign = ((w / 2) % 2 == 0 ? -1f : 1f);
                Spots.Add(Site + Along * AlongAt + Side * (Gap * SideSign));
                Vector2 FaceDir = Along * (AlongAt >= 0f ? -1f : 1f);
                Faces.Add((float)Math.Atan2(FaceDir.X, -FaceDir.Y));
                Models.Add(Path.Combine(Root, WorkNames[(WorkShift + w) % WorkNames.Length] + ".fbx"));
            }
        }

        void LaySpan(Vector2 From, Vector2 To)
        {
            float Length = Vector2.Distance(From, To);
            if (Length < 8f)
            {
                return;
            }

            int Steps = Math.Max(1, (int)Math.Round(Length / 32f));
            Vector2 Delta = To - From;
            float Angle = (float)Math.Atan2(Delta.X, -Delta.Y);
            string Model = Path.Combine(WorldSlot.ModelsRoot(), "Medieval Village Pack", "Props", "Path_Straight.fbx");
            for (int i = 0; i <= Steps; i++)
            {
                Vector2 At = Vector2.Lerp(From, To, i / (float)Steps);
                PlaceModel(At, Model, 2f, 8f, Angle);
            }
        }

        void StampRoad(Vector2 From, Vector2 To, float FromReach, float ToReach)
        {
            Vector2 Delta = To - From;
            float Length = Delta.Length();
            float Margin = FromReach + ToReach + 24f;
            if (Length < Margin)
            {
                return;
            }

            Vector2 Dir = Delta / Length;
            Vector2 Side = new Vector2(-Dir.Y, Dir.X);
            Vector2 At = From + Dir * (FromReach + 8f);
            Vector2 Goal = To - Dir * (ToReach + 8f);
            string Model = Path.Combine(WorldSlot.ModelsRoot(), "Medieval Village Pack", "Props", "Path_Straight.fbx");
            int Guard = 0;
            while (Vector2.DistanceSquared(At, Goal) > 30f * 30f && Guard < 420)
            {
                Guard++;
                Vector2 Want = Goal - At;
                if (Want.LengthSquared() < 1f)
                {
                    break;
                }

                Want.Normalize();
                Vector2 Step = RoadStep(At, Want, Side);
                Vector2 Next = At + Step;
                if (!UnderWater(Next) && !UnderWater(At))
                {
                    float Angle = (float)Math.Atan2(Step.X, -Step.Y);
                    PlaceModel(Next, Model, 2f, 8f, Angle);
                    PaintMark(Next, new Color(60, 58, 54));
                    FlattenPad(Next.X, Next.Y, 8f, 22f);
                }

                At = Next;
            }
        }

        Vector2 RoadStep(Vector2 At, Vector2 Want, Vector2 Side)
        {
            const float Step = 32f;
            Vector2[] Bend = new Vector2[]
            {
                Want,
                Normalize(Want + Side * 0.85f),
                Normalize(Want - Side * 0.85f),
                Normalize(Want + Side * 1.5f),
                Normalize(Want - Side * 1.5f),
            };
            for (int i = 0; i < Bend.Length; i++)
            {
                Vector2 Next = At + Bend[i] * Step;
                if (!UnderWater(Next) && SlopeAt(FromWorld(Next.X), FromWorld(Next.Y)) < 0.55f)
                {
                    return Bend[i] * Step;
                }
            }

            return Want * Step;
        }

        static Vector2 Normalize(Vector2 Value)
        {
            if (Value.LengthSquared() < 0.0001f)
            {
                return new Vector2(0f, 1f);
            }

            Value.Normalize();
            return Value;
        }

        static float WrapAngle(float Angle)
        {
            while (Angle > (float)Math.PI)
            {
                Angle -= (float)(Math.PI * 2.0);
            }

            while (Angle < -(float)Math.PI)
            {
                Angle += (float)(Math.PI * 2.0);
            }

            return Angle;
        }

        bool UnderWater(Vector2 At)
        {
            if (WaterCells == null)
            {
                return false;
            }

            int X = FromWorld(At.X);
            int Z = FromWorld(At.Y);
            int Cell = Z * Global + X;
            return Cell >= 0 && Cell < WaterCells.Length && WaterCells[Cell];
        }

        void QuietForest(Vector2 Site, float Radius)
        {
            int MinX = ChunkOf(Site.X - Radius);
            int MaxX = ChunkOf(Site.X + Radius);
            int MinZ = ChunkOf(Site.Y - Radius);
            int MaxZ = ChunkOf(Site.Y + Radius);
            for (int Z = MinZ; Z <= MaxZ; Z++)
            {
                for (int X = MinX; X <= MaxX; X++)
                {
                    if (!Holds(X, Z))
                    {
                        continue;
                    }

                    float CenterX = (X + 0.5f) * Slot.ChunkSize;
                    float CenterZ = (Z + 0.5f) * Slot.ChunkSize;
                    if (Vector2.Distance(Site, new Vector2(CenterX, CenterZ)) > Radius)
                    {
                        continue;
                    }

                    ChunkForest[Pack(X, Z)].Density = 0.08f;
                }
            }
        }

        void PaintDisc(Vector2 At, float Radius, Color Ink)
        {
            int CX = FromWorld(At.X);
            int CZ = FromWorld(At.Y);
            int Reach = Math.Max(1, (int)Math.Ceiling(Radius / Slot.Spacing));
            for (int DZ = -Reach; DZ <= Reach; DZ++)
            {
                for (int DX = -Reach; DX <= Reach; DX++)
                {
                    int X = CX + DX;
                    int Z = CZ + DZ;
                    if (X < 0 || Z < 0 || X >= Global || Z >= Global)
                    {
                        continue;
                    }

                    float Dist = (float)Math.Sqrt(DX * DX + DZ * DZ) * Slot.Spacing;
                    if (Dist <= Radius)
                    {
                        Picture[Z * Global + X] = Ink;
                    }
                }
            }
        }

        void PlaceModel(Vector2 At, string Model, float Meters, float Height, float Angle = 0f)
        {
            int ChunkX = ChunkOf(At.X);
            int ChunkZ = ChunkOf(At.Y);
            if (!Holds(ChunkX, ChunkZ))
            {
                return;
            }

            float Ground = SampleWorld(At.X, At.Y);
            BuildingItem Item = new BuildingItem();
            Item.Kind = BuildingKind.Floor;
            Item.Model = Model;
            Item.ModelMeters = Meters;
            Item.Position = new Vector3(At.X - ChunkX * Slot.ChunkSize, At.Y - ChunkZ * Slot.ChunkSize, Ground);
            Item.Width = 32f;
            Item.Length = 32f;
            Item.Height = Height;
            Item.Angle = Angle;
            Item.Swatch = new Vector3(0.62f, 0.52f, 0.38f);
            ChunkBuildings[Pack(ChunkX, ChunkZ)].Add(Item);
        }

        void PaintMark(Vector2 At, Color Ink)
        {
            int X = FromWorld(At.X);
            int Z = FromWorld(At.Y);
            Picture[Z * Global + X] = Ink;
        }

        void EaseSeams()
        {
            int Step = Samples - 1;
            for (int Edge = Step; Edge < Global - 1; Edge += Step)
            {
                for (int Z = 1; Z < Global - 1; Z++)
                {
                    EaseSample(Edge, Z, -1, 0, 1, 0);
                }

                for (int X = 1; X < Global - 1; X++)
                {
                    EaseSample(X, Edge, 0, -1, 0, 1);
                }
            }
        }

        void EaseSample(int EX, int EZ, int LX, int LZ, int RX, int RZ)
        {
            float Here = Heights[EX + LX, EZ + LZ];
            float Edge = Heights[EX, EZ];
            float There = Heights[EX + RX, EZ + RZ];
            float Jump = (Edge - Here) - (There - Edge);
            if (Math.Abs(Jump) < 0.05f || Math.Abs(Jump) > 2.5f)
            {
                return;
            }

            float Mid = (There - Here) * 0.5f;
            Heights[EX + LX, EZ + LZ] = Edge - Mid;
            Heights[EX + RX, EZ + RZ] = Edge + Mid;
        }

        void WriteChunk(int Index)
        {
            int ChunkX = ChunkXOf(Index);
            int ChunkZ = ChunkZOf(Index);
            MapInfo Info = new MapInfo(new MapContainer(ChunkX + "_" + ChunkZ, null), null, null);
            Info.ChunkX = ChunkX;
            Info.ChunkZ = ChunkZ;
            Info.Kind = GroundKind.Heightmap;
            Info.Heights = TakeField(ChunkX, ChunkZ);
            Info.Water = ChunkWater[Index];
            Info.Buildings = ChunkBuildings[Index];
            Info.Furniture = new List<FurnitureItem>();
            Info.Forest = ChunkForest[Index];
            Info.Landmarks = new List<LandmarkHeader>();
            ChunkFile.Place(Slot, Info);
            ChunkFile.Write(Slot, Info, true);
        }

        HeightField TakeField(int ChunkX, int ChunkZ)
        {
            HeightField Field = HeightField.Create(Slot.ChunkSize, Slot.Spacing, 0f);
            for (int Z = 0; Z < Field.SamplesZ; Z++)
            {
                for (int X = 0; X < Field.SamplesX; X++)
                {
                    int GX = Cell(ChunkX, X);
                    int GZ = Cell(ChunkZ, Z);
                    int At = Field.Index(X, Z);
                    float Height = Heights[GX, GZ];
                    float Wet = Moisture[GX, GZ];
                    float Slope = SlopeAt(GX, GZ);
                    Field.Heights[At] = Height;
                    Field.Blends[At] = Blend(Height, Wet, Slope);
                }
            }

            Field.Rebuild();
            return Field;
        }

        static Vector4 Blend(float Height, float Wet, float Slope)
        {
            float Snow = Fade(Height, 26f, 46f);
            float Rock = Fade(Slope, 0.22f, 0.7f) * (1f - Snow);
            float Dry = 1f - Fade(Wet, 0.3f, 0.62f);
            float Cover = 1f - Snow - Rock;
            if (Cover < 0f)
            {
                Cover = 0f;
            }

            float Dirt = Cover * Dry;
            float Grass = Cover * (1f - Dry);
            float Beach = (1f - Fade(Height, Ocean.Level + 0.3f, Ocean.Level + 4.5f)) * (1f - Fade(Slope, 0.1f, 0.38f));
            Beach *= 1f - Snow;
            Grass *= 1f - Beach;
            Dirt = Dirt * (1f - Beach) + Cover * Beach;
            float Sum = Grass + Dirt + Rock + Snow;
            if (Sum < 0.001f)
            {
                return new Vector4(1f, 0f, 0f, 0f);
            }

            return new Vector4(Grass / Sum, Dirt / Sum, Rock / Sum, Snow / Sum);
        }

        static Color LandColor(float Height, float Wet, float Slope)
        {
            Vector4 Ink = Blend(Height, Wet, Slope);
            float Red = 62f * Ink.X + 150f * Ink.Y + 120f * Ink.Z + 236f * Ink.W;
            float Green = 130f * Ink.X + 118f * Ink.Y + 122f * Ink.Z + 240f * Ink.W;
            float Blue = 54f * Ink.X + 64f * Ink.Y + 118f * Ink.Z + 244f * Ink.W;
            return new Color((int)Red, (int)Green, (int)Blue);
        }

        Vector3 WorldAt(int X, int Z)
        {
            return new Vector3(ToWorld(X), ToWorld(Z), Heights[X, Z]);
        }

        float SampleWorld(float X, float Y)
        {
            return Heights[FromWorld(X), FromWorld(Y)];
        }

        float SlopeAt(int X, int Z)
        {
            float Left = Heights[Math.Max(0, X - 1), Z];
            float Right = Heights[Math.Min(Global - 1, X + 1), Z];
            float Down = Heights[X, Math.Max(0, Z - 1)];
            float Up = Heights[X, Math.Min(Global - 1, Z + 1)];
            return (float)Math.Sqrt((Right - Left) * (Right - Left) + (Up - Down) * (Up - Down)) / (Slot.Spacing * 2f);
        }

        void SampleClimate(float WorldX, float WorldZ, out float Height, out float Wet)
        {
            float Ox = (Seed & 4095) * 0.37f;
            float Oz = ((Seed >> 12) & 4095) * 0.37f;
            float X = WorldX + Ox;
            float Z = WorldZ + Oz;
            float WarpX = Simplex.Noise(X * 0.00028f, Z * 0.00028f);
            float WarpZ = Simplex.Noise(X * 0.00028f + 40f, Z * 0.00028f + 11f);
            float SX = X + WarpX * 420f;
            float SZ = Z + WarpZ * 420f;
            float Broad = Simplex.Noise(SX * 0.00016f, SZ * 0.00016f);
            float Isles = Simplex.Noise(SX * 0.00055f + 19f, SZ * 0.00055f);
            float Land = Fade(Broad * 0.75f + Isles * 0.25f, -0.18f, 0.4f);
            float Hills = Fbm(SX, SZ, 5, 0.0007f);
            float RidgeMask = Fade(Simplex.Noise(SX * 0.00036f + 17f, SZ * 0.00036f), 0.05f, 0.62f);
            float Peaks = Ridged(SX + 25f, SZ - 13f, 4, 0.00105f);
            float Detail = Fbm(SX + 90f, SZ + 15f, 3, 0.0034f);
            float Inland = Math.Max(Ocean.Level + 1.4f, 16f + Hills * 30f + Detail * 7f + RidgeMask * Peaks * 38f);
            float Open = 1f - Land;
            float Bed = Ocean.Level - 1.8f - Open * 16f;
            float Shore = Fade(Land, 0.46f, 0.72f);
            Height = Bed + (Inland - Bed) * Shore;
            float Rain = Fbm(SX + 220f, SZ - 80f, 4, 0.00042f) * 0.5f + 0.5f;
            Wet = Rain * (0.4f + 0.6f * Land) * (1f - RidgeMask * 0.45f);
            if (Wet < 0f)
            {
                Wet = 0f;
            }

            if (Wet > 1f)
            {
                Wet = 1f;
            }
        }

        WaterLook Meltwater(int X, int Z, float Surface)
        {
            int Reach = Math.Max(1, (int)Math.Ceiling(480f / Slot.Spacing));
            float High = Surface;
            for (int DZ = -Reach; DZ <= Reach; DZ++)
            {
                for (int DX = -Reach; DX <= Reach; DX++)
                {
                    if (DX * DX + DZ * DZ > Reach * Reach)
                    {
                        continue;
                    }

                    int NX = X + DX;
                    int NZ = Z + DZ;
                    if (NX < 0 || NZ < 0 || NX >= Global || NZ >= Global)
                    {
                        continue;
                    }

                    if (Heights[NX, NZ] > High)
                    {
                        High = Heights[NX, NZ];
                    }
                }
            }

            if (High >= 26f || High >= Surface + 8f)
            {
                return WaterLook.Northern;
            }

            return WaterLook.Tropical;
        }

        static float Fade(float Value, float Edge0, float Edge1)
        {
            float Span = Edge1 - Edge0;
            if (Span < 0.0001f)
            {
                return Value >= Edge1 ? 1f : 0f;
            }

            float T = (Value - Edge0) / Span;
            if (T < 0f)
            {
                T = 0f;
            }

            if (T > 1f)
            {
                T = 1f;
            }

            return T * T * (3f - 2f * T);
        }

        static float Fbm(float X, float Z, int Octaves, float Frequency)
        {
            float Sum = 0f;
            float Amp = 1f;
            float Norm = 0f;
            for (int i = 0; i < Octaves; i++)
            {
                Sum += Simplex.Noise(X * Frequency, Z * Frequency) * Amp;
                Norm += Amp;
                Amp *= 0.5f;
                Frequency *= 2.03f;
            }

            return Norm > 0f ? Sum / Norm : 0f;
        }

        static float Ridged(float X, float Z, int Octaves, float Frequency)
        {
            float Sum = 0f;
            float Amp = 1f;
            float Norm = 0f;
            for (int i = 0; i < Octaves; i++)
            {
                float N = 1f - Math.Abs(Simplex.Noise(X * Frequency, Z * Frequency));
                N *= N;
                Sum += N * Amp;
                Norm += Amp;
                Amp *= 0.5f;
                Frequency *= 2.07f;
            }

            return Norm > 0f ? Sum / Norm : 0f;
        }

        void TraceRivers(int[] Down, float[] Accum, bool[] Lake, float[] LakeSurface)
        {
            int Count = Global * Global;
            bool[] Used = new bool[Count];
            List<int> Heads = new List<int>();
            float Threshold = 28f;
            for (int i = 0; i < Count; i++)
            {
                if (Accum[i] < Threshold || Lake[i] || Down[i] == i)
                {
                    continue;
                }

                int X = i % Global;
                int Z = i / Global;
                bool Head = true;
                for (int DZ = -1; DZ <= 1 && Head; DZ++)
                {
                    for (int DX = -1; DX <= 1; DX++)
                    {
                        if (DX == 0 && DZ == 0)
                        {
                            continue;
                        }

                        int NX = X + DX;
                        int NZ = Z + DZ;
                        if (NX < 0 || NZ < 0 || NX >= Global || NZ >= Global)
                        {
                            continue;
                        }

                        int Neighbor = NZ * Global + NX;
                        if (Down[Neighbor] == i && Accum[Neighbor] >= Threshold && !Lake[Neighbor])
                        {
                            Head = false;
                            break;
                        }
                    }
                }

                if (Head)
                {
                    Heads.Add(i);
                }
            }

            Heads.Sort(delegate (int A, int B)
            {
                return Accum[B].CompareTo(Accum[A]);
            });

            for (int h = 0; h < Heads.Count; h++)
            {
                if (Used[Heads[h]])
                {
                    continue;
                }

                List<int> Course = new List<int>();
                int At = Heads[h];
                int Guard = 0;
                while (At >= 0 && Guard++ < Count)
                {
                    if (Used[At])
                    {
                        Course.Add(At);
                        break;
                    }

                    Course.Add(At);
                    MarkCorridor(At, Used);
                    int Next = Down[At];
                    if (Next == At)
                    {
                        break;
                    }

                    if (Lake[Next])
                    {
                        Course.Add(Next);
                        break;
                    }

                    if (Heights[Next % Global, Next / Global] < Ocean.Level)
                    {
                        Course.Add(Next);
                        break;
                    }

                    At = Next;
                }

                if (Course.Count >= 8)
                {
                    LayRiver(Course, Accum, Lake, LakeSurface);
                }
            }
        }

        void MarkCorridor(int Cell, bool[] Used)
        {
            int X = Cell % Global;
            int Z = Cell / Global;
            for (int DZ = -1; DZ <= 1; DZ++)
            {
                for (int DX = -1; DX <= 1; DX++)
                {
                    if (DX != 0 && DZ != 0)
                    {
                        continue;
                    }

                    int NX = X + DX;
                    int NZ = Z + DZ;
                    if (NX < 0 || NZ < 0 || NX >= Global || NZ >= Global)
                    {
                        continue;
                    }

                    Used[NZ * Global + NX] = true;
                }
            }
        }

        void LayRiver(List<int> Course, float[] Accum, bool[] Lake, float[] LakeSurface)
        {
            float Level = float.MaxValue;
            List<Vector3> Points = new List<Vector3>();
            List<float> Widths = new List<float>();
            for (int i = 0; i < Course.Count; i++)
            {
                int Cell = Course[i];
                int X = Cell % Global;
                int Z = Cell / Global;
                float Surface;
                if (Lake[Cell] && LakeSurface[Cell] > -100000f)
                {
                    Surface = LakeSurface[Cell];
                    if (Surface > Level)
                    {
                        Surface = Level;
                    }
                }
                else
                {
                    Surface = Heights[X, Z] - 0.4f;
                    if (Surface > Level - 0.12f)
                    {
                        Surface = Level - 0.12f;
                    }
                }

                Level = Surface;
                float Width = 9f + (float)Math.Sqrt(Math.Max(1f, Accum[Cell])) * 0.42f;
                if (Width > 24f)
                {
                    Width = 24f;
                }

                Points.Add(new Vector3(ToWorld(X), ToWorld(Z), Surface));
                Widths.Add(Width);
                MarkWater(Cell, Surface);

                if (!Lake[Cell])
                {
                    Channel(X, Z, Surface, Width * 0.62f);
                }
            }

            int Last = 0;
            for (int i = 1; i < Points.Count; i++)
            {
                float DX = Points[i].X - Points[Last].X;
                float DY = Points[i].Y - Points[Last].Y;
                float Dist = (float)Math.Sqrt(DX * DX + DY * DY);
                bool End = i == Points.Count - 1;
                if (!End && Dist < 32f)
                {
                    continue;
                }

                if (Dist < 16f)
                {
                    continue;
                }

                WaterVolume River = new WaterVolume();
                River.Kind = WaterKind.River;
                River.Look = Meltwater(FromWorld(Points[i].X), FromWorld(Points[i].Y), Points[i].Z);
                River.Start = Points[Last];
                River.End = Points[i];
                River.Width = (Widths[Last] + Widths[i]) * 0.5f;
                River.Depth = 5f;
                River.Speed = 36f;
                GiveWater(River);
                Last = i;
            }
        }

        void Channel(int X, int Z, float Surface, float Radius)
        {
            int Reach = Math.Max(1, (int)Math.Ceiling(Radius / Slot.Spacing));
            float Bed = Surface - 2.6f;
            for (int DZ = -Reach; DZ <= Reach; DZ++)
            {
                for (int DX = -Reach; DX <= Reach; DX++)
                {
                    int NX = X + DX;
                    int NZ = Z + DZ;
                    if (NX < 0 || NZ < 0 || NX >= Global || NZ >= Global)
                    {
                        continue;
                    }

                    float Dist = (float)Math.Sqrt(DX * DX + DZ * DZ) * Slot.Spacing;
                    if (Dist > Radius)
                    {
                        continue;
                    }

                    MarkWater(NZ * Global + NX, Surface);

                    float Along = Dist / Math.Max(Radius, 0.01f);
                    float Want = Bed + (Heights[NX, NZ] - Bed) * Along * Along;
                    if (Heights[NX, NZ] > Want)
                    {
                        Heights[NX, NZ] = Want;
                    }
                }
            }
        }

        void Relax()
        {
            float[] Next = new float[Global * Global];
            float Limit = Slot.Spacing * 0.42f;
            for (int Pass = 0; Pass < 4; Pass++)
            {
                for (int Z = 0; Z < Global; Z++)
                {
                    for (int X = 0; X < Global; X++)
                    {
                        int At = Z * Global + X;
                        if (WaterCells != null && WaterCells[At])
                        {
                            Next[At] = Heights[X, Z];
                            continue;
                        }

                        float Height = Heights[X, Z];
                        if (X > 0)
                        {
                            Height = Math.Min(Height, Heights[X - 1, Z] + Limit);
                        }

                        if (X + 1 < Global)
                        {
                            Height = Math.Min(Height, Heights[X + 1, Z] + Limit);
                        }

                        if (Z > 0)
                        {
                            Height = Math.Min(Height, Heights[X, Z - 1] + Limit);
                        }

                        if (Z + 1 < Global)
                        {
                            Height = Math.Min(Height, Heights[X, Z + 1] + Limit);
                        }

                        Next[Z * Global + X] = Height;
                    }
                }

                for (int Z = 0; Z < Global; Z++)
                {
                    for (int X = 0; X < Global; X++)
                    {
                        Heights[X, Z] = Next[Z * Global + X];
                    }
                }
            }
        }

        void Recolor()
        {
            for (int Z = 0; Z < Global; Z++)
            {
                for (int X = 0; X < Global; X++)
                {
                    int At = Z * Global + X;
                    if (Heights[X, Z] < Ocean.Level)
                    {
                        Picture[At] = new Color(28, 78, 148);
                    }
                    else if (WaterCells != null && WaterCells[At])
                    {
                        Picture[At] = new Color(40, 90, 170);
                    }
                    else
                    {
                        Picture[At] = LandColor(Heights[X, Z], Moisture[X, Z], SlopeAt(X, Z));
                    }
                }
            }
        }

        int LowestNeighbor(int X, int Z)
        {
            int Best = Z * Global + X;
            float Height = Heights[X, Z];
            for (int DZ = -1; DZ <= 1; DZ++)
            {
                for (int DX = -1; DX <= 1; DX++)
                {
                    if (DX == 0 && DZ == 0)
                    {
                        continue;
                    }

                    int NX = X + DX;
                    int NZ = Z + DZ;
                    if (NX < 0 || NZ < 0 || NX >= Global || NZ >= Global)
                    {
                        continue;
                    }

                    if (Heights[NX, NZ] < Height)
                    {
                        Height = Heights[NX, NZ];
                        Best = NZ * Global + NX;
                    }
                }
            }

            return Best;
        }

        static float TownKeep()
        {
            int Houses = 10;
            int Left = (Houses + 1) / 2;
            float Mid = (Math.Max(Left, Houses / 2) - 1) * 0.5f;
            float End = Mid * 108f + 118f;
            float Building = (float)Math.Sqrt(End * End + 100f * 100f);
            return Building + 70f + 36f;
        }

        void MarkWater(int Index, float Surface)
        {
            if (WaterCells == null || Index < 0 || Index >= WaterCells.Length)
            {
                return;
            }

            WaterCells[Index] = true;
            if (WaterSurface != null && Surface > WaterSurface[Index])
            {
                WaterSurface[Index] = Surface;
            }
        }

        bool ClearOfWater(int X, int Z, float Radius)
        {
            if (WaterCells == null)
            {
                return true;
            }

            float Ground = Heights[X, Z];
            int Reach = Math.Max(1, (int)Math.Ceiling((Radius + 64f) / Slot.Spacing));
            float Limit = (Radius + 64f) * (Radius + 64f);
            float Pad = Radius * Radius;
            float Step = Slot.Spacing * Slot.Spacing;
            for (int DZ = -Reach; DZ <= Reach; DZ++)
            {
                for (int DX = -Reach; DX <= Reach; DX++)
                {
                    float Dist = (DX * DX + DZ * DZ) * Step;
                    if (Dist > Limit)
                    {
                        continue;
                    }

                    int NX = X + DX;
                    int NZ = Z + DZ;
                    if (NX < 0 || NZ < 0 || NX >= Global || NZ >= Global)
                    {
                        continue;
                    }

                    int At = NZ * Global + NX;
                    if (!WaterCells[At])
                    {
                        continue;
                    }

                    if (Dist <= Pad)
                    {
                        return false;
                    }

                    if (WaterSurface != null && WaterSurface[At] > Ground + 0.2f)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        bool HoldsBank(int X, int Z)
        {
            if (WaterCells == null)
            {
                return false;
            }

            for (int DZ = -2; DZ <= 2; DZ++)
            {
                for (int DX = -2; DX <= 2; DX++)
                {
                    int NX = X + DX;
                    int NZ = Z + DZ;
                    if (NX < 0 || NZ < 0 || NX >= Global || NZ >= Global)
                    {
                        continue;
                    }

                    if (WaterCells[NZ * Global + NX])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        void FlattenPad(float WorldX, float WorldY, float Hard, float Soft)
        {
            if (Soft < Hard + 1f)
            {
                Soft = Hard + 1f;
            }

            int CX = FromWorld(WorldX);
            int CZ = FromWorld(WorldY);
            float Height = Heights[CX, CZ];
            int Reach = Math.Max(1, (int)Math.Ceiling(Soft / Slot.Spacing));
            for (int DZ = -Reach; DZ <= Reach; DZ++)
            {
                for (int DX = -Reach; DX <= Reach; DX++)
                {
                    int NX = CX + DX;
                    int NZ = CZ + DZ;
                    if (NX < 0 || NZ < 0 || NX >= Global || NZ >= Global)
                    {
                        continue;
                    }

                    float Dist = (float)Math.Sqrt(DX * DX + DZ * DZ) * Slot.Spacing;
                    if (Dist > Soft || HoldsBank(NX, NZ))
                    {
                        continue;
                    }

                    if (Dist <= Hard)
                    {
                        Heights[NX, NZ] = Height;
                        continue;
                    }

                    float T = (Dist - Hard) / (Soft - Hard);
                    float Weight = (1f - T) * (1f - T);
                    Heights[NX, NZ] += (Height - Heights[NX, NZ]) * Weight;
                }
            }
        }

        void Bowl(int X, int Z, float Radius, float Surface)
        {
            int Reach = Math.Max(1, (int)Math.Ceiling(Radius / Slot.Spacing));
            for (int DZ = -Reach; DZ <= Reach; DZ++)
            {
                for (int DX = -Reach; DX <= Reach; DX++)
                {
                    int NX = X + DX;
                    int NZ = Z + DZ;
                    if (NX < 0 || NZ < 0 || NX >= Global || NZ >= Global)
                    {
                        continue;
                    }

                    float Dist = (float)Math.Sqrt(DX * DX + DZ * DZ) * Slot.Spacing;
                    if (Dist > Radius)
                    {
                        continue;
                    }

                    float Along = Dist / Math.Max(Radius, 0.01f);
                    float Want = Surface - 2.5f + 4f * Along * Along;
                    if (Heights[NX, NZ] > Want)
                    {
                        Heights[NX, NZ] = Want;
                    }
                }
            }
        }

        void Carve(int X, int Z, float Radius, float Depth)
        {
            int Reach = Math.Max(1, (int)Math.Ceiling(Radius / Slot.Spacing));
            for (int DZ = -Reach; DZ <= Reach; DZ++)
            {
                for (int DX = -Reach; DX <= Reach; DX++)
                {
                    int NX = X + DX;
                    int NZ = Z + DZ;
                    if (NX < 0 || NZ < 0 || NX >= Global || NZ >= Global)
                    {
                        continue;
                    }

                    float Dist = (float)Math.Sqrt(DX * DX + DZ * DZ) * Slot.Spacing;
                    if (Dist > Radius)
                    {
                        continue;
                    }

                    float Weight = 1f - Dist / Radius;
                    Heights[NX, NZ] -= Depth * Weight * Weight;
                }
            }
        }

        void WritePicture()
        {
            Png.Write(Slot.MinimapPath, Global, Global, Picture);
        }

        static int Hash(int A, int B)
        {
            int N = A * 374761393 + B * 668265263;
            N = (N ^ (N >> 13)) * 1274126177;
            return N & int.MaxValue;
        }
    }

    static class Simplex
    {
        static readonly int[] Perm = new int[512];

        static Simplex()
        {
            int[] Source = new int[] { 151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180 };
            for (int i = 0; i < 256; i++)
            {
                Perm[i] = Source[i];
                Perm[i + 256] = Source[i];
            }
        }

        public static float Noise(float X, float Y)
        {
            float S = (X + Y) * 0.3660254f;
            int I = (int)Math.Floor(X + S);
            int J = (int)Math.Floor(Y + S);
            float T = (I + J) * 0.2113248f;
            float X0 = X - (I - T);
            float Y0 = Y - (J - T);
            int I1 = X0 > Y0 ? 1 : 0;
            int J1 = X0 > Y0 ? 0 : 1;
            float X1 = X0 - I1 + 0.2113248f;
            float Y1 = Y0 - J1 + 0.2113248f;
            float X2 = X0 - 1f + 0.4226497f;
            float Y2 = Y0 - 1f + 0.4226497f;
            int II = I & 255;
            int JJ = J & 255;
            float N0 = Corner(Perm[II + Perm[JJ]], X0, Y0);
            float N1 = Corner(Perm[II + I1 + Perm[JJ + J1]], X1, Y1);
            float N2 = Corner(Perm[II + 1 + Perm[JJ + 1]], X2, Y2);
            return 70f * (N0 + N1 + N2);
        }

        static float Corner(int Hash, float X, float Y)
        {
            float T = 0.5f - X * X - Y * Y;
            if (T < 0f)
            {
                return 0f;
            }

            T *= T;
            int H = Hash & 7;
            float U = H < 4 ? X : Y;
            float V = H < 4 ? Y : X;
            return T * T * (((H & 1) == 0 ? U : -U) + ((H & 2) == 0 ? V : -V));
        }
    }

    struct Edge
    {
        public int A;
        public int B;
        public float Length;
    }

    static class Delaunay
    {
        public static List<Edge> Edges(List<Vector2> Sites)
        {
            List<Edge> Result = new List<Edge>();
            if (Sites == null || Sites.Count < 2)
            {
                return Result;
            }

            if (Sites.Count == 2)
            {
                Edge Only = new Edge();
                Only.A = 0;
                Only.B = 1;
                Only.Length = Vector2.Distance(Sites[0], Sites[1]);
                Result.Add(Only);
                return Result;
            }

            List<Vector2> Points = new List<Vector2>(Sites);
            Points.Add(new Vector2(-100000f, -100000f));
            Points.Add(new Vector2(100000f, -100000f));
            Points.Add(new Vector2(0f, 100000f));
            List<int[]> Triangles = new List<int[]>();
            Triangles.Add(new int[] { Points.Count - 3, Points.Count - 2, Points.Count - 1 });
            for (int s = 0; s < Sites.Count; s++)
            {
                List<int[]> Bad = new List<int[]>();
                for (int t = 0; t < Triangles.Count; t++)
                {
                    if (Inside(Points[s], Points[Triangles[t][0]], Points[Triangles[t][1]], Points[Triangles[t][2]]))
                    {
                        Bad.Add(Triangles[t]);
                    }
                }

                List<Edge> Boundary = new List<Edge>();
                for (int b = 0; b < Bad.Count; b++)
                {
                    AddBoundary(Boundary, Bad[b][0], Bad[b][1]);
                    AddBoundary(Boundary, Bad[b][1], Bad[b][2]);
                    AddBoundary(Boundary, Bad[b][2], Bad[b][0]);
                    Triangles.Remove(Bad[b]);
                }

                for (int b = 0; b < Boundary.Count; b++)
                {
                    Triangles.Add(new int[] { Boundary[b].A, Boundary[b].B, s });
                }
            }

            int Super = Sites.Count;
            for (int t = 0; t < Triangles.Count; t++)
            {
                int A = Triangles[t][0];
                int B = Triangles[t][1];
                int C = Triangles[t][2];
                if (A >= Super || B >= Super || C >= Super)
                {
                    continue;
                }

                AddUnique(Result, Sites, A, B);
                AddUnique(Result, Sites, B, C);
                AddUnique(Result, Sites, C, A);
            }

            return Result;
        }

        static void AddBoundary(List<Edge> Boundary, int A, int B)
        {
            for (int i = 0; i < Boundary.Count; i++)
            {
                if ((Boundary[i].A == A && Boundary[i].B == B) || (Boundary[i].A == B && Boundary[i].B == A))
                {
                    Boundary.RemoveAt(i);
                    return;
                }
            }

            Edge Edge = new Edge();
            Edge.A = A;
            Edge.B = B;
            Boundary.Add(Edge);
        }

        static void AddUnique(List<Edge> Result, List<Vector2> Sites, int A, int B)
        {
            for (int i = 0; i < Result.Count; i++)
            {
                if ((Result[i].A == A && Result[i].B == B) || (Result[i].A == B && Result[i].B == A))
                {
                    return;
                }
            }

            Edge Edge = new Edge();
            Edge.A = A;
            Edge.B = B;
            Edge.Length = Vector2.Distance(Sites[A], Sites[B]);
            Result.Add(Edge);
        }

        static bool Inside(Vector2 P, Vector2 A, Vector2 B, Vector2 C)
        {
            float AX = A.X - P.X;
            float AY = A.Y - P.Y;
            float BX = B.X - P.X;
            float BY = B.Y - P.Y;
            float CX = C.X - P.X;
            float CY = C.Y - P.Y;
            float Det = AX * (BY * (CX * CX + CY * CY) - CY * (BX * BX + BY * BY))
                - AY * (BX * (CX * CX + CY * CY) - CX * (BX * BX + BY * BY))
                + (AX * AX + AY * AY) * (BX * CY - CX * BY);
            return Det > 0f;
        }
    }

    static class Spanning
    {
        public static List<Edge> Keep(List<Edge> Edges, int Count)
        {
            List<Edge> Sorted = new List<Edge>(Edges);
            Sorted.Sort(delegate (Edge A, Edge B) { return A.Length.CompareTo(B.Length); });
            int[] Parent = new int[Math.Max(1, Count)];
            for (int i = 0; i < Parent.Length; i++)
            {
                Parent[i] = i;
            }

            List<Edge> Tree = new List<Edge>();
            List<Edge> Extra = new List<Edge>();
            for (int i = 0; i < Sorted.Count; i++)
            {
                int A = Find(Parent, Sorted[i].A);
                int B = Find(Parent, Sorted[i].B);
                if (A != B)
                {
                    Parent[B] = A;
                    Tree.Add(Sorted[i]);
                }
                else if (Extra.Count < 2)
                {
                    Extra.Add(Sorted[i]);
                }
            }

            Tree.AddRange(Extra);
            return Tree;
        }

        static int Find(int[] Parent, int Index)
        {
            while (Parent[Index] != Index)
            {
                Parent[Index] = Parent[Parent[Index]];
                Index = Parent[Index];
            }

            return Index;
        }
    }

    static class Png
    {
        public static void Write(string PathName, int Width, int Height, Color[] Pixels)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(PathName));
            byte[] Raw = new byte[Height * (1 + Width * 3)];
            for (int Y = 0; Y < Height; Y++)
            {
                int Row = Y * (1 + Width * 3);
                Raw[Row] = 0;
                for (int X = 0; X < Width; X++)
                {
                    Color Pixel = Pixels[Y * Width + X];
                    int At = Row + 1 + X * 3;
                    Raw[At] = Pixel.R;
                    Raw[At + 1] = Pixel.G;
                    Raw[At + 2] = Pixel.B;
                }
            }

            using (FileStream Stream = new FileStream(PathName, FileMode.Create, FileAccess.Write))
            using (BinaryWriter Writer = new BinaryWriter(Stream))
            {
                Writer.Write(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 });
                WriteChunk(Writer, "IHDR", Header(Width, Height));
                WriteChunk(Writer, "IDAT", Zlib(Raw));
                WriteChunk(Writer, "IEND", Array.Empty<byte>());
            }
        }

        static byte[] Header(int Width, int Height)
        {
            byte[] Bytes = new byte[13];
            WriteInt(Bytes, 0, Width);
            WriteInt(Bytes, 4, Height);
            Bytes[8] = 8;
            Bytes[9] = 2;
            return Bytes;
        }

        static byte[] Zlib(byte[] Raw)
        {
            using (MemoryStream Stream = new MemoryStream())
            {
                Stream.WriteByte(0x78);
                Stream.WriteByte(0x9C);
                using (DeflateStream Deflate = new DeflateStream(Stream, CompressionLevel.Fastest, true))
                {
                    Deflate.Write(Raw, 0, Raw.Length);
                }

                uint Adler = Adler32(Raw);
                Stream.WriteByte((byte)(Adler >> 24));
                Stream.WriteByte((byte)(Adler >> 16));
                Stream.WriteByte((byte)(Adler >> 8));
                Stream.WriteByte((byte)Adler);
                return Stream.ToArray();
            }
        }

        static uint Adler32(byte[] Data)
        {
            uint A = 1;
            uint B = 0;
            for (int i = 0; i < Data.Length; i++)
            {
                A = (A + Data[i]) % 65521;
                B = (B + A) % 65521;
            }

            return (B << 16) | A;
        }

        static void WriteChunk(BinaryWriter Writer, string Type, byte[] Data)
        {
            byte[] Name = System.Text.Encoding.ASCII.GetBytes(Type);
            byte[] Length = new byte[4];
            WriteInt(Length, 0, Data.Length);
            Writer.Write(Length);
            Writer.Write(Name);
            Writer.Write(Data);
            uint Crc = Crc32(Name, Data);
            byte[] Sum = new byte[4];
            WriteInt(Sum, 0, (int)Crc);
            Writer.Write(Sum);
        }

        static void WriteInt(byte[] Bytes, int At, int Value)
        {
            Bytes[At] = (byte)(Value >> 24);
            Bytes[At + 1] = (byte)(Value >> 16);
            Bytes[At + 2] = (byte)(Value >> 8);
            Bytes[At + 3] = (byte)Value;
        }

        static uint Crc32(byte[] Name, byte[] Data)
        {
            uint Crc = 0xFFFFFFFF;
            for (int i = 0; i < Name.Length; i++)
            {
                Crc = Step(Crc, Name[i]);
            }

            for (int i = 0; i < Data.Length; i++)
            {
                Crc = Step(Crc, Data[i]);
            }

            return Crc ^ 0xFFFFFFFF;
        }

        static uint Step(uint Crc, byte Value)
        {
            Crc ^= Value;
            for (int i = 0; i < 8; i++)
            {
                uint Mask = (Crc & 1) == 1 ? 0xEDB88320 : 0;
                Crc = (Crc >> 1) ^ Mask;
            }

            return Crc;
        }
    }
}
