using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class ForestScatter
    {
        static readonly Dictionary<string, FbxModel> Models = new Dictionary<string, FbxModel>();
        static readonly Dictionary<string, GltfModel> Glbs = new Dictionary<string, GltfModel>();
        static readonly HashSet<string> Failed = new HashSet<string>();
        static readonly System.Diagnostics.Stopwatch WindClock = System.Diagnostics.Stopwatch.StartNew();
        static bool LeafFailed;

        // Height of Tree 1 after its node scale. Tree 2 is a little shorter and stays that way.
        const float GlbHeight = 1.70f;

        public static void Fill(MapInfo Info)
        {
            Info.Scatter = new List<ScatterSpot>();
            if (Info == null || Info.Forest == null || !Info.Forest.HasTrees || Info.Heights == null)
            {
                return;
            }

            HeightField Field = Info.Heights;
            float Span = (Field.SamplesX - 1) * Field.Spacing;
            int Seed = Info.Forest.Seed == 0 ? Info.ChunkX * 73856093 ^ Info.ChunkZ * 19349663 : Info.Forest.Seed;
            float Step = 18f;
            if (Info.Forest.Density > 0.01f)
            {
                Step = 18f / Info.Forest.Density;
            }

            if (Step < 10f)
            {
                Step = 10f;
            }

            int Guard = 0;
            for (float Y = Step * 0.5f; Y < Span && Guard < 80; Y += Step)
            {
                for (float X = Step * 0.5f; X < Span && Guard < 80; X += Step)
                {
                    float JitterX = Hash(Seed, (int)X, (int)Y) * Step * 0.6f;
                    float JitterY = Hash(Seed, (int)Y, (int)X) * Step * 0.6f;
                    float LocalX = X + JitterX;
                    float LocalY = Y + JitterY;
                    float Height;
                    float SlopeX;
                    float SlopeY;
                    if (!Field.Sample(LocalX, LocalY, out Height, out SlopeX, out SlopeY))
                    {
                        continue;
                    }

                    float Slope = (float)Math.Sqrt(SlopeX * SlopeX + SlopeY * SlopeY);
                    if (Slope > Info.Forest.MaxSlope || Height < Info.Forest.MinHeight || Height > Info.Forest.MaxHeight || Height < Ocean.Level + 0.8f)
                    {
                        continue;
                    }

                    if (OnWater(Info, LocalX, LocalY) || NearBuilding(Info, LocalX, LocalY))
                    {
                        continue;
                    }

                    Vector4 Blend = Field.ReadBlend(LocalX, LocalY);
                    if (Blend.Z > 0.55f)
                    {
                        continue;
                    }

                    if (Blend.Y > 0.5f && Hash(Seed, (int)LocalY, (int)LocalX + 3) > 0.4f)
                    {
                        continue;
                    }

                    if (Blend.W <= 0.4f && Blend.Y > 0.45f && Height < 22f && !DesertPocket(Info))
                    {
                        continue;
                    }

                    string Kind = Species(Blend, Height, Seed, (int)LocalX, (int)LocalY);
                    if (Kind == "Cactus")
                    {
                        if (Height < Ocean.Level + 2.5f || Hash(Seed, (int)LocalX + 5, (int)LocalY + 13) > 0.16f || Crowded(Info.Scatter, LocalX, LocalY, 72f))
                        {
                            continue;
                        }
                    }
                    else if (Crowded(Info.Scatter, LocalX, LocalY, 104f))
                    {
                        continue;
                    }

                    string Model = Pick(Kind, Seed, (int)LocalX, (int)LocalY, Height);
                    if (string.IsNullOrEmpty(Model))
                    {
                        continue;
                    }

                    ScatterSpot Spot = new ScatterSpot();
                    Spot.Position = new Vector3(LocalX, LocalY, Height);
                    Spot.Model = Model;
                    bool Rock = Model.IndexOf("Rock", StringComparison.OrdinalIgnoreCase) >= 0;
                    bool Low = Model.IndexOf("Bush", StringComparison.OrdinalIgnoreCase) >= 0 || Model.IndexOf("Grass", StringComparison.OrdinalIgnoreCase) >= 0 || Model.IndexOf("Flower", StringComparison.OrdinalIgnoreCase) >= 0 || Model.IndexOf("Plant", StringComparison.OrdinalIgnoreCase) >= 0;
                    Spot.Meters = Rock ? 1.5f : 6f;
                    Spot.Height = Rock ? 48f : Low ? 60f : 240f;
                    Info.Scatter.Add(Spot);
                    Guard++;
                }
            }

            Cover(Info, Field, Span, Seed);
        }

        static void Cover(MapInfo Info, HeightField Field, float Span, int Seed)
        {
            int Guard = 0;
            float Step = 10f;
            for (float Y = Step * 0.5f; Y < Span && Guard < 100; Y += Step)
            {
                for (float X = Step * 0.5f; X < Span && Guard < 100; X += Step)
                {
                    if (Hash(Seed, (int)X + 17, (int)Y + 29) > 0.3f)
                    {
                        continue;
                    }

                    float JitterX = Hash(Seed, (int)X + 2, (int)Y + 8) * Step * 0.7f;
                    float JitterY = Hash(Seed, (int)Y + 5, (int)X + 11) * Step * 0.7f;
                    float LocalX = X + JitterX;
                    float LocalY = Y + JitterY;
                    float Height;
                    float SlopeX;
                    float SlopeY;
                    if (!Field.Sample(LocalX, LocalY, out Height, out SlopeX, out SlopeY))
                    {
                        continue;
                    }

                    float Slope = (float)Math.Sqrt(SlopeX * SlopeX + SlopeY * SlopeY);
                    if (Slope > Info.Forest.MaxSlope || Height < Info.Forest.MinHeight || Height > Info.Forest.MaxHeight || Height < Ocean.Level + 0.8f)
                    {
                        continue;
                    }

                    if (OnWater(Info, LocalX, LocalY) || NearBuilding(Info, LocalX, LocalY))
                    {
                        continue;
                    }

                    Vector4 Blend = Field.ReadBlend(LocalX, LocalY);
                    if (Blend.Z > 0.55f || Blend.Y > 0.5f)
                    {
                        continue;
                    }

                    int Which = Hash(Seed, (int)LocalX, (int)LocalY + 4) < 0.5f ? 1 : 2;
                    ScatterSpot Spot = new ScatterSpot();
                    Spot.Position = new Vector3(LocalX, LocalY, Height);
                    Spot.Model = Path.Combine(WorldSlot.ModelsRoot(), "Grass", "Grass " + Which.ToString() + ".glb");
                    Spot.Meters = 0.4f;
                    Spot.Height = 36f;
                    Info.Scatter.Add(Spot);
                    Guard++;
                }
            }
        }

        public static void Collect(MapInfo Info, List<BoxVolume> Solids)
        {
            if (Info == null || Info.Scatter == null || Solids == null)
            {
                return;
            }

            for (int i = 0; i < Info.Scatter.Count; i++)
            {
                BoxVolume Trunk;
                if (TrunkOf(Info.Scatter[i], out Trunk))
                {
                    Solids.Add(Trunk);
                }
            }
        }

        public static bool Hits(MapInfo Info, Vector3 From, Vector3 To)
        {
            if (Info == null || Info.Scatter == null)
            {
                return false;
            }

            for (int i = 0; i < Info.Scatter.Count; i++)
            {
                BoxVolume Trunk;
                if (TrunkOf(Info.Scatter[i], out Trunk) && Trunk.Hits(From, To))
                {
                    return true;
                }
            }

            return false;
        }

        static bool IsGrass(string Model)
        {
            return !string.IsNullOrEmpty(Model) && Model.IndexOf("Grass", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        static bool IsLow(string Model)
        {
            if (string.IsNullOrEmpty(Model))
            {
                return false;
            }

            return Model.IndexOf("Bush", StringComparison.OrdinalIgnoreCase) >= 0
                || Model.IndexOf("Grass", StringComparison.OrdinalIgnoreCase) >= 0
                || Model.IndexOf("Flower", StringComparison.OrdinalIgnoreCase) >= 0
                || Model.IndexOf("Plant", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        static bool TrunkOf(ScatterSpot Spot, out BoxVolume Box)
        {
            Box = new BoxVolume();
            if (string.IsNullOrEmpty(Spot.Model) || Spot.Meters <= 0.01f)
            {
                return false;
            }

            if (IsLow(Spot.Model))
            {
                return false;
            }

            bool Rock = Spot.Model.IndexOf("Rock", StringComparison.OrdinalIgnoreCase) >= 0;
            bool Cactus = Spot.Model.IndexOf("Cactus", StringComparison.OrdinalIgnoreCase) >= 0;
            float Scale = Spot.Height / Spot.Meters;
            float Wide = Rock ? 0.75f * Scale : Cactus ? 0.55f * Scale : 0.4f * Scale;
            if (Wide < 14f)
            {
                Wide = 14f;
            }

            Box.Position = Spot.Position;
            Box.Width = Wide;
            Box.Length = Wide;
            Box.Height = (Cactus || Rock ? 1f : 1.8f) * Scale;
            return true;
        }

        // How far the ground contact around a trunk reaches. Bushes and grass return 0.
        // How much rain still reaches the ground. 1 in the open, lower under a crown, never a solid roof.
        public static float Shade(float WorldX, float WorldZ)
        {
            float Open = 1f;
            List<MapInfo> Maps = ChunkCross.Maps;
            if (Maps == null)
            {
                return Open;
            }

            for (int m = 0; m < Maps.Count; m++)
            {
                MapInfo Info = Maps[m];
                if (Info == null || Info.Scatter == null || Info.Scatter.Count == 0 || Info.MapMapContainer == null)
                {
                    continue;
                }

                float SpanX = Info.MapMapContainer.SpanX;
                float SpanY = Info.MapMapContainer.SpanY;
                float LocalX = WorldX - Info.MapMapContainer.OffsetX;
                float LocalY = WorldZ - Info.MapMapContainer.OffsetY;
                if (LocalX < -140f || LocalY < -140f || LocalX > SpanX + 140f || LocalY > SpanY + 140f)
                {
                    continue;
                }

                for (int i = 0; i < Info.Scatter.Count; i++)
                {
                    ScatterSpot Spot = Info.Scatter[i];
                    if (Spot.Height < 140f)
                    {
                        continue;
                    }

                    float DX = Spot.Position.X - LocalX;
                    float DY = Spot.Position.Y - LocalY;
                    float Radius = Spot.Height * 0.42f;
                    float Reach = Radius * Radius;
                    float Dist2 = DX * DX + DY * DY;
                    if (Dist2 >= Reach)
                    {
                        continue;
                    }

                    float Dist = (float)Math.Sqrt(Dist2) / Radius;
                    float Leaf = (1f - Dist) * (1f - Dist) * 0.62f;
                    Open *= 1f - Leaf;
                    if (Open < 0.22f)
                    {
                        return 0.22f;
                    }
                }
            }

            return Open;
        }

        public static float TrunkReach(ScatterSpot Spot)
        {
            BoxVolume Box;
            if (!TrunkOf(Spot, out Box))
            {
                return 0f;
            }

            return Box.Width * 0.95f;
        }

        public static void Draw(GraphicsDevice Device, MapInfo Info, Matrix View, Matrix Projection)
        {
            Draw(Device, Info, View, Projection, false);
            Draw(Device, Info, View, Projection, true);
        }

        public static void Draw(GraphicsDevice Device, MapInfo Info, Matrix View, Matrix Projection, bool Leaves)
        {
            MatchDetail();
            if (Device == null || Info == null || Info.Scatter == null || Info.Scatter.Count == 0)
            {
                return;
            }

            Vector3 Shift = ChunkOrigin.Shift(Info);
            PrepareWind(Device);
            GltfModel.LeafFillDir = SkyDraw.FillDirection;
            GltfModel.LeafFillColor = SkyDraw.FillColor;
            GltfModel.ShadeBark = true;
            try
            {
                ResetBatches();
                DrawPlaced(Device, Info, View, Projection, Shift, Leaves);
                DrawBatches(View, Projection, Leaves);
            }
            finally
            {
                GltfModel.ShadeBark = false;
            }
        }

        static readonly Dictionary<GltfModel, Matrix[]> BatchWorlds = new Dictionary<GltfModel, Matrix[]>();
        static readonly Dictionary<GltfModel, int> BatchCounts = new Dictionary<GltfModel, int>();
        static readonly List<GltfModel> BatchOrder = new List<GltfModel>();

        static void ResetBatches()
        {
            for (int i = 0; i < BatchOrder.Count; i++)
            {
                BatchCounts[BatchOrder[i]] = 0;
            }

            BatchOrder.Clear();
        }

        static void Keep(GltfModel Model, Matrix World)
        {
            Matrix[] Worlds;
            if (!BatchWorlds.TryGetValue(Model, out Worlds))
            {
                Worlds = new Matrix[64];
                BatchWorlds.Add(Model, Worlds);
                BatchCounts.Add(Model, 0);
            }

            int Count = BatchCounts[Model];
            if (Count == 0)
            {
                BatchOrder.Add(Model);
            }

            if (Count == Worlds.Length)
            {
                Array.Resize(ref Worlds, Worlds.Length * 2);
                BatchWorlds[Model] = Worlds;
            }

            Worlds[Count] = World;
            BatchCounts[Model] = Count + 1;
        }

        public static void Draw(GraphicsDevice Device, IList<MapInfo> Maps, Matrix View, Matrix Projection, bool Leaves)
        {
            MatchDetail();
            if (Device == null || Maps == null || Maps.Count == 0)
            {
                return;
            }

            PrepareWind(Device);
            GltfModel.LeafFillDir = SkyDraw.FillDirection;
            GltfModel.LeafFillColor = SkyDraw.FillColor;
            GltfModel.ShadeBark = true;
            try
            {
                ResetBatches();
                for (int i = 0; i < Maps.Count; i++)
                {
                    MapInfo Info = Maps[i];
                    if (Info == null || Info.Scatter == null || Info.Scatter.Count == 0)
                    {
                        continue;
                    }

                    DrawPlaced(Device, Info, View, Projection, ChunkOrigin.Shift(Info), Leaves);
                }

                DrawBatches(View, Projection, Leaves);
            }
            finally
            {
                GltfModel.ShadeBark = false;
            }
        }

        static void DrawPlaced(GraphicsDevice Device, MapInfo Info, Matrix View, Matrix Projection, Vector3 Shift, bool Translucent)
        {
            for (int i = 0; i < Info.Scatter.Count; i++)
            {
                ScatterSpot Spot = Info.Scatter[i];
                if (Spot.Meters <= 0.01f || IsGrass(Spot.Model))
                {
                    continue;
                }

                Vector3 Feet = new Vector3(Spot.Position.X + Shift.X, Spot.Position.Z + Shift.Y, Spot.Position.Y + Shift.Z);
                Matrix World = Place(Spot, Feet);
                if (IsGlb(Spot.Model))
                {
                    GltfModel Model = LoadGlb(Device, Spot.Model);
                    if (Model == null)
                    {
                        continue;
                    }

                    Keep(Model, World);
                    continue;
                }

                if (Translucent)
                {
                    continue;
                }

                FbxModel ModelFbx = Load(Device, Spot.Model);
                if (ModelFbx == null)
                {
                    continue;
                }

                ModelFbx.Draw(View, Projection, World);
            }
        }

        static void DrawBatches(Matrix View, Matrix Projection, bool Translucent)
        {
            for (int i = 0; i < BatchOrder.Count; i++)
            {
                GltfModel Model = BatchOrder[i];
                int Count = BatchCounts[Model];
                if (Count > 0)
                {
                    Model.DrawMany(View, Projection, BatchWorlds[Model], Count, Translucent);
                }
            }
        }

        public static void DrawShadows(Effect Depth, IList<MapInfo> Maps, Matrix View, Matrix Projection, Vector3 Focus, float Radius, bool Leaves = true, bool Cover = false)
        {
            MatchDetail();
            if (Depth == null || Maps == null || Radius <= 0f)
            {
                return;
            }

            float Limit = Radius * Radius;
            PrepareWind(Depth.GraphicsDevice);
            ResetBatches();
            for (int m = 0; m < Maps.Count; m++)
            {
                MapInfo Info = Maps[m];
                if (Info == null || Info.Scatter == null)
                {
                    continue;
                }

                Vector3 Shift = ChunkOrigin.Shift(Info);
                for (int i = 0; i < Info.Scatter.Count; i++)
                {
                    ScatterSpot Spot = Info.Scatter[i];
                    if (Spot.Meters <= 0.01f || IsGrass(Spot.Model) || (IsLow(Spot.Model) && !Cover))
                    {
                        continue;
                    }

                    Vector3 Feet = new Vector3(Spot.Position.X + Shift.X, Spot.Position.Z + Shift.Y, Spot.Position.Y + Shift.Z);
                    Vector3 Flat = Feet - Focus;
                    Flat.Y = 0f;
                    if (Flat.LengthSquared() > Limit)
                    {
                        continue;
                    }

                    Matrix World = Place(Spot, Feet);
                    if (IsGlb(Spot.Model))
                    {
                        GltfModel Model = LoadGlb(Depth.GraphicsDevice, Spot.Model);
                        if (Model != null)
                        {
                            Keep(Model, World);
                        }

                        continue;
                    }

                    FbxModel ModelFbx = Load(Depth.GraphicsDevice, Spot.Model);
                    if (ModelFbx == null)
                    {
                        continue;
                    }

                    ModelFbx.DrawDepth(Depth, View, Projection, World);
                }
            }

            for (int i = 0; i < BatchOrder.Count; i++)
            {
                GltfModel Model = BatchOrder[i];
                int Count = BatchCounts[Model];
                if (Count > 0)
                {
                    Model.DrawDepthMany(Depth, View, Projection, BatchWorlds[Model], Count, Leaves);
                }
            }
        }

        static bool DesertPocket(MapInfo Info)
        {
            int RegionX = Info.ChunkX >= 0 ? Info.ChunkX / 3 : -((-Info.ChunkX + 2) / 3);
            int RegionZ = Info.ChunkZ >= 0 ? Info.ChunkZ / 3 : -((-Info.ChunkZ + 2) / 3);
            int Salt = Info.Forest.Seed == 0 ? 1 : Info.Forest.Seed;
            return Hash(Salt, RegionX + 29, RegionZ + 47) < 0.12f;
        }

        static bool NearBuilding(MapInfo Info, float X, float Y)
        {
            if (HitsBuilding(Info, X, Y))
            {
                return true;
            }

            if (Info.Heights == null)
            {
                return false;
            }

            float SpanX = (Info.Heights.SamplesX - 1) * Info.Heights.Spacing;
            float SpanY = (Info.Heights.SamplesZ - 1) * Info.Heights.Spacing;
            const float Edge = 150f;
            if (X < Edge && HitsBuilding(Neighbor(Info, -1, 0), X + SpanX, Y))
            {
                return true;
            }

            if (X > SpanX - Edge && HitsBuilding(Neighbor(Info, 1, 0), X - SpanX, Y))
            {
                return true;
            }

            if (Y < Edge && HitsBuilding(Neighbor(Info, 0, -1), X, Y + SpanY))
            {
                return true;
            }

            return Y > SpanY - Edge && HitsBuilding(Neighbor(Info, 0, 1), X, Y - SpanY);
        }

        static bool HitsBuilding(MapInfo Info, float X, float Y)
        {
            if (Info == null || Info.Buildings == null)
            {
                return false;
            }

            for (int i = 0; i < Info.Buildings.Count; i++)
            {
                BuildingItem Box = Info.Buildings[i];
                if (Box == null)
                {
                    continue;
                }

                float Reach = 48f;
                if (!string.IsNullOrEmpty(Box.Model) && Box.Model.IndexOf("Medieval Village Pack", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Reach = Box.Model.IndexOf("Props", StringComparison.OrdinalIgnoreCase) >= 0 ? 40f : 150f;
                }

                float DX = Box.Position.X - X;
                float DY = Box.Position.Y - Y;
                if (DX * DX + DY * DY < Reach * Reach)
                {
                    return true;
                }
            }

            return false;
        }

        static bool Crowded(List<ScatterSpot> Spots, float X, float Y, float Gap)
        {
            if (Spots == null)
            {
                return false;
            }

            float Limit = Gap * Gap;
            for (int i = 0; i < Spots.Count; i++)
            {
                float DX = Spots[i].Position.X - X;
                float DY = Spots[i].Position.Y - Y;
                if (DX * DX + DY * DY < Limit)
                {
                    return true;
                }
            }

            return false;
        }

        static bool OnWater(MapInfo Info, float X, float Y)
        {
            const float Bank = 1.4f;
            if (Covered(Info, X, Y, Bank))
            {
                return true;
            }

            if (Info.Heights == null)
            {
                return false;
            }

            float LimitX = (Info.Heights.SamplesX - 1) * Info.Heights.Spacing;
            float LimitY = (Info.Heights.SamplesZ - 1) * Info.Heights.Spacing;
            if (X < Info.Heights.Spacing && Covered(Neighbor(Info, -1, 0), X + LimitX, Y, Bank))
            {
                return true;
            }

            if (X > LimitX - Info.Heights.Spacing && Covered(Neighbor(Info, 1, 0), X - LimitX, Y, Bank))
            {
                return true;
            }

            if (Y < Info.Heights.Spacing && Covered(Neighbor(Info, 0, -1), X, Y + LimitY, Bank))
            {
                return true;
            }

            if (Y > LimitY - Info.Heights.Spacing && Covered(Neighbor(Info, 0, 1), X, Y - LimitY, Bank))
            {
                return true;
            }

            return false;
        }

        static MapInfo Neighbor(MapInfo Info, int DX, int DZ)
        {
            MapInfo Other;
            if (Info == null || !WorldStream.ByCoord.TryGetValue(WorldStream.Key(Info.ChunkX + DX, Info.ChunkZ + DZ), out Other))
            {
                return null;
            }

            return Other;
        }

        static bool Covered(MapInfo Info, float X, float Y, float Bank)
        {
            if (Info == null || Info.Water == null || Info.Heights == null)
            {
                return false;
            }

            for (int i = 0; i < Info.Water.Count; i++)
            {
                WaterVolume Volume = Info.Water[i];
                if (Volume != null && Volume.Covers(Info.Heights, X, Y, Bank))
                {
                    return true;
                }
            }

            return false;
        }

        static string Species(Vector4 Blend, float Height, int Seed, int X, int Y)
        {
            float Roll = Hash(Seed, X + 9, Y + 4);
            if (Blend.W > 0.4f)
            {
                return Roll > 0.5f ? "PineTree_Snow" : "BirchTree_Snow";
            }

            if (Blend.Y > 0.45f && Height < 22f)
            {
                return "Cactus";
            }

            if (Height < 8f && Blend.X > 0.55f && Blend.Y < 0.2f)
            {
                return "PalmTree";
            }

            if (Height > 16f || Roll > 0.62f)
            {
                return "PineTree";
            }

            return "CommonTree";
        }

        static int PlacedDetail = int.MinValue;

        public static void MatchDetail()
        {
            int Want = PictureOptions.Trees <= 0 ? 0 : 1;
            if (PlacedDetail == Want)
            {
                return;
            }

            PlacedDetail = Want;
            List<MapInfo> Maps = ChunkCross.Maps;
            if (Maps == null)
            {
                return;
            }

            for (int i = 0; i < Maps.Count; i++)
            {
                MapInfo Info = Maps[i];
                if (Info != null && Info.BodyReady && Info.Heights != null)
                {
                    Fill(Info);
                }
            }
        }

        static string Pick(string SetName, int Seed, int X, int Y, float Height)
        {
            if (PictureOptions.Trees <= 0)
            {
                return LowTree(SetName, Seed, X, Y);
            }

            int Which = Hash(Seed, X, Y) < 0.5f ? 1 : 2;
            return Path.Combine(WorldSlot.ModelsRoot(), "Trees 2", "Tree " + Which.ToString() + ".glb");
        }

        static string LowTree(string SetName, int Seed, int X, int Y)
        {
            int Variant = 1 + (int)(Hash(Seed, X, Y) * 5f);
            if (Variant > 5)
            {
                Variant = 5;
            }

            string Name = SetName;
            float Roll = Hash(Seed, Y, X);
            if (Roll > 0.82f && SetName.IndexOf("Snow", StringComparison.OrdinalIgnoreCase) < 0 && SetName.IndexOf("Cactus", StringComparison.OrdinalIgnoreCase) < 0 && SetName.IndexOf("Palm", StringComparison.OrdinalIgnoreCase) < 0)
            {
                Name = SetName + "_Autumn";
            }
            else if (Roll < 0.08f && SetName.IndexOf("Dead", StringComparison.OrdinalIgnoreCase) < 0 && SetName.IndexOf("Cactus", StringComparison.OrdinalIgnoreCase) < 0)
            {
                Name = SetName + "_Dead";
            }

            if (Name.StartsWith("Palm", StringComparison.OrdinalIgnoreCase) && Variant > 4)
            {
                Variant = 4;
            }

            string FileName = Name + "_" + Variant.ToString() + ".fbx";
            string Full = Path.Combine(WorldSlot.ModelsRoot(), "Trees", FileName);
            if (!File.Exists(Full))
            {
                Full = Path.Combine(WorldSlot.ModelsRoot(), "Trees", SetName + "_" + Variant.ToString() + ".fbx");
            }

            return Full;
        }

        static void PrepareWind(GraphicsDevice Device)
        {
            if (GltfModel.LeafEffect == null && !LeafFailed && Device != null)
            {
                try
                {
                    string PathName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Life Sim", "Shaders", "Foliage.mgfx");
                    if (!File.Exists(PathName))
                    {
                        LeafFailed = true;
                    }
                    else
                    {
                        GltfModel.LeafEffect = new Effect(Device, File.ReadAllBytes(PathName));
                    }
                }
                catch (Exception)
                {
                    LeafFailed = true;
                }
            }

            float Time = (float)WindClock.Elapsed.TotalSeconds;
            GltfModel.LeafWindTime = Time;
            Vector2 Wind = Climate.Now.Wind;
            if (Wind.LengthSquared() < 0.01f)
            {
                Wind = new Vector2(1f, 0.2f);
            }

            GltfModel.LeafWindDir = Vector2.Normalize(Wind);
            GltfModel.LeafWindStrength = 12f + Climate.Now.Strength * 1.8f;
            GltfModel.LeafSunDir = SkyDraw.KeyDirection;
            GltfModel.LeafSunColor = SkyDraw.KeyColor;
            GltfModel.LeafAmbient = SkyDraw.SkyAmbient;
            CharacterShadow.Apply(GltfModel.LeafEffect);
        }

        static bool IsGlb(string PathName)
        {
            return PathName != null && PathName.EndsWith(".glb", StringComparison.OrdinalIgnoreCase);
        }

        static Matrix Place(ScatterSpot Spot, Vector3 Feet)
        {
            if (IsGlb(Spot.Model))
            {
                float Scale = Spot.Height / GlbHeight;
                return Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Feet);
            }

            float FbxScale = Spot.Height / Spot.Meters;
            return Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateScale(FbxScale) * Matrix.CreateTranslation(Feet);
        }

        static GltfModel LoadGlb(GraphicsDevice Device, string PathName)
        {
            if (string.IsNullOrEmpty(PathName) || Failed.Contains(PathName) || !File.Exists(PathName))
            {
                return null;
            }

            GltfModel Model;
            if (Glbs.TryGetValue(PathName, out Model))
            {
                return Model;
            }

            try
            {
                Model = new GltfModel(Device, PathName, false);
                Glbs.Add(PathName, Model);
                return Model;
            }
            catch (Exception)
            {
                Failed.Add(PathName);
                return null;
            }
        }

        static FbxModel Load(GraphicsDevice Device, string PathName)
        {
            if (string.IsNullOrEmpty(PathName) || Failed.Contains(PathName) || !File.Exists(PathName))
            {
                return null;
            }

            FbxModel Model;
            if (Models.TryGetValue(PathName, out Model))
            {
                return Model;
            }

            try
            {
                Model = new FbxModel(Device, PathName);
                Models.Add(PathName, Model);
                return Model;
            }
            catch (Exception)
            {
                Failed.Add(PathName);
                return null;
            }
        }

        static float Hash(int Seed, int X, int Y)
        {
            int N = Seed * 374761393 + X * 668265263 + Y * 2146121005;
            N = (N ^ (N >> 13)) * 1274126177;
            return ((N ^ (N >> 16)) & 255) / 255f;
        }
    }

    public static class LandmarkDraw
    {
        static readonly Dictionary<string, FbxModel> Models = new Dictionary<string, FbxModel>();

        public static void Draw(GraphicsDevice Device, MapInfo Info, Matrix View, Matrix Projection)
        {
            if (Device == null || Info == null || Info.Landmarks == null)
            {
                return;
            }

            Vector3 Shift = ChunkOrigin.Shift(Info);
            for (int i = 0; i < Info.Landmarks.Count; i++)
            {
                LandmarkHeader Landmark = Info.Landmarks[i];
                if (Landmark == null || !Landmark.MeshOpen || string.IsNullOrEmpty(Landmark.Mesh))
                {
                    continue;
                }

                FbxModel Model;
                if (!Models.TryGetValue(Landmark.Mesh, out Model))
                {
                    if (!File.Exists(Landmark.Mesh))
                    {
                        continue;
                    }

                    try
                    {
                        Model = new FbxModel(Device, Landmark.Mesh);
                        Models.Add(Landmark.Mesh, Model);
                    }
                    catch (Exception)
                    {
                        Models.Add(Landmark.Mesh, null);
                        continue;
                    }
                }

                if (Model == null)
                {
                    continue;
                }

                Vector3 Feet = new Vector3(Landmark.Min.X + Shift.X, Landmark.Min.Y + Shift.Y, Landmark.Min.Z + Shift.Z);
                Matrix World = Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateScale(16f) * Matrix.CreateTranslation(Feet);
                Model.Draw(View, Projection, World);
            }
        }
    }

    public static class MeshChunk
    {
        static readonly Dictionary<string, FbxModel> Models = new Dictionary<string, FbxModel>();

        public static void Ensure(MapInfo Info, float CameraY)
        {
            if (Info == null || Info.Kind != GroundKind.Mesh || Info.Bands == null || Info.Bands.Count == 0)
            {
                return;
            }

            int BandIndex = 0;
            float Best = float.MaxValue;
            for (int i = 0; i < Info.Bands.Count; i++)
            {
                MeshBand Band = Info.Bands[i];
                float Mid = (Band.MinY + Band.MaxY) * 0.5f;
                if (float.IsInfinity(Mid))
                {
                    BandIndex = i;
                    break;
                }

                float Distance = Math.Abs(CameraY - Mid);
                if (Distance < Best)
                {
                    Best = Distance;
                    BandIndex = i;
                }
            }

            if (Info.ActiveBand == BandIndex && Info.MeshVertices != null)
            {
                return;
            }

            Info.ActiveBand = BandIndex;
            MeshBand Chosen = Info.Bands[BandIndex];
            string PathName = string.IsNullOrEmpty(Chosen.Model) ? Info.MeshPath : Chosen.Model;
            if (string.IsNullOrEmpty(PathName) || !File.Exists(PathName) || GameScreen.GraphicsDevice == null)
            {
                return;
            }

            FbxModel Model;
            if (!Models.TryGetValue(PathName, out Model))
            {
                try
                {
                    Model = new FbxModel(GameScreen.GraphicsDevice, PathName);
                    Models.Add(PathName, Model);
                }
                catch (Exception)
                {
                    Models.Add(PathName, null);
                    return;
                }
            }

            if (Model == null)
            {
                return;
            }

            Info.MeshModel = Model;
            Info.MeshScale = Chosen.Scale <= 0.01f ? 16f : Chosen.Scale;
            List<Vector3> Source = new List<Vector3>();
            List<int> SourceIndices = new List<int>();
            Model.CollectTriangles(Source, SourceIndices);
            Matrix Spin = Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateScale(Info.MeshScale);
            List<Vector3> Kept = new List<Vector3>();
            List<int> KeptIndices = new List<int>();
            for (int i = 0; i + 2 < SourceIndices.Count; i += 3)
            {
                Vector3 A = Vector3.Transform(Source[SourceIndices[i]], Spin);
                Vector3 B = Vector3.Transform(Source[SourceIndices[i + 1]], Spin);
                Vector3 C = Vector3.Transform(Source[SourceIndices[i + 2]], Spin);
                float MidY = (A.Y + B.Y + C.Y) / 3f;
                if (MidY < Chosen.MinY || MidY > Chosen.MaxY)
                {
                    continue;
                }

                int Base = Kept.Count;
                Kept.Add(A);
                Kept.Add(B);
                Kept.Add(C);
                KeptIndices.Add(Base);
                KeptIndices.Add(Base + 1);
                KeptIndices.Add(Base + 2);
            }

            Info.MeshVertices = Kept.ToArray();
            Info.MeshIndices = KeptIndices.ToArray();
            WorldStream.Submit(Info);
        }

        public static void Draw(GraphicsDevice Device, MapInfo Info, Matrix View, Matrix Projection)
        {
            if (Info == null || Info.MeshModel == null)
            {
                return;
            }

            Vector3 Shift = ChunkOrigin.Shift(Info);
            Matrix World = Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateScale(Info.MeshScale) * Matrix.CreateTranslation(Shift);
            Info.MeshModel.Draw(View, Projection, World);
        }
    }
}
