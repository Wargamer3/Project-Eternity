using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class WorldStream
    {
        public static WorldSlot Slot;
        public static Dictionary<long, MapInfo> ByCoord = new Dictionary<long, MapInfo>();
        public const float RoomOpenDistance = 96f;

        public static long Key(int X, int Z)
        {
            return ((long)X << 32) ^ (uint)Z;
        }

        public static void Start(NavMapGameManager Manager)
        {
            Slot = WorldSlot.OpenOrCreate();
            ByCoord = new Dictionary<long, MapInfo>();
            if (Manager.AllChunks == null)
            {
                Manager.AllChunks = new List<MapInfo>();
            }
            else
            {
                Manager.AllChunks.Clear();
            }

            ChunkCross.Maps = Manager.AllChunks;
            Manager.FarChunk = null;
            MapInfo Root = ChunkAt(0, 0, true);
            Manager.RootMapContainer = Root;
            OpenView(Root);
            if (Root.MapMapContainer.ActiveMap != null)
            {
                Slot.ApplyClock(Root.MapMapContainer.ActiveMap);
            }
            TownLife.Open(Slot, Manager);
        }

        public static MapInfo ChunkAt(int X, int Z, bool Body)
        {
            long Id = Key(X, Z);
            MapInfo Info;
            if (!ByCoord.TryGetValue(Id, out Info))
            {
                Info = new MapInfo(new MapContainer(X + "_" + Z, null), null, null);
                Info.ChunkX = X;
                Info.ChunkZ = Z;
                ChunkFile.Place(Slot, Info);
                ByCoord.Add(Id, Info);
                if (ChunkCross.Maps != null && !ChunkCross.Maps.Contains(Info))
                {
                    ChunkCross.Maps.Add(Info);
                }
            }

            if (!Info.HeaderReady)
            {
                if (!ChunkFile.Read(Slot, Info, false, false))
                {
                    Info.Kind = GroundKind.None;
                    Info.HeaderReady = true;
                    Info.Forest = new ForestRecipe();
                    if (Body)
                    {
                        Info.Heights = HeightField.Create(Slot.ChunkSize, Slot.Spacing, 0f);
                    }
                }
            }

            bool FreshBody = false;
            if (Body && !Info.BodyReady && Info.Kind != GroundKind.None)
            {
                ChunkFile.Read(Slot, Info, true, false);
                FreshBody = Info.Kind == GroundKind.Heightmap && Info.Heights != null;
                if (FreshBody)
                {
                    ForestScatter.Fill(Info);
                    WeldGround(Info);
                }
            }

            if (Body && Info.Kind == GroundKind.None && Info.Heights == null)
            {
                Info.Heights = HeightField.Create(Slot.ChunkSize, Slot.Spacing, 0f);
            }

            LinkNeighbors(Info);
            return Info;
        }

        static void LinkNeighbors(MapInfo Info)
        {
            ChunkRing.Border(Info, Neighbor(Info, 1, 0));
            ChunkRing.Border(Info, Neighbor(Info, -1, 0));
            ChunkRing.Border(Info, Neighbor(Info, 0, 1));
            ChunkRing.Border(Info, Neighbor(Info, 0, -1));
        }

        static void WeldGround(MapInfo Info)
        {
            WeldOne(Info);
            WeldOne(Neighbor(Info, 1, 0));
            WeldOne(Neighbor(Info, -1, 0));
            WeldOne(Neighbor(Info, 0, 1));
            WeldOne(Neighbor(Info, 0, -1));
        }

        static void WeldOne(MapInfo Info)
        {
            if (Info == null || Info.Heights == null)
            {
                return;
            }

            WeldSide(Info, 1, 0, 0);
            WeldSide(Info, -1, 0, 1);
            WeldSide(Info, 0, 1, 2);
            WeldSide(Info, 0, -1, 3);
        }

        static MapInfo Neighbor(MapInfo Info, int DX, int DZ)
        {
            if (Info == null)
            {
                return null;
            }

            MapInfo Other;
            if (!ByCoord.TryGetValue(Key(Info.ChunkX + DX, Info.ChunkZ + DZ), out Other))
            {
                return null;
            }

            return Other;
        }

        static void WeldSide(MapInfo Info, int DX, int DZ, int Side)
        {
            MapInfo Other;
            if (!ByCoord.TryGetValue(Key(Info.ChunkX + DX, Info.ChunkZ + DZ), out Other) || Other.Heights == null)
            {
                return;
            }

            Info.Heights.Weld(Other.Heights, Side);
        }

        public static void OpenView(MapInfo Info)
        {
            if (Info == null || Info.MapMapContainer == null || Info.ViewReady)
            {
                return;
            }

            if (Info.Kind == GroundKind.Grid && !string.IsNullOrEmpty(Info.GridPath))
            {
                LifeSimMap Grid = new LifeSimMap(Info.GridPath, new GameModeInfo());
                Grid.Chunk = Info;
                Info.MapMapContainer.ActiveMap = Grid;
                Grid.Furniture = Info.Furniture;
                Grid.Buildings = Info.Buildings;
                Grid.Water = Info.Water;
                Grid.Load();
                Grid.Init();
                Grid.TogglePreview(true);
                if (Slot != null)
                {
                    Slot.ApplyClock(Grid);
                }

                Info.ViewReady = true;
                return;
            }

            if (Info.Forest == null)
            {
                Info.Forest = new ForestRecipe();
            }

            Submit(Info);
            if (Info.Kind == GroundKind.Mesh)
            {
                MeshChunk.Ensure(Info, 0f);
            }

            Info.ViewReady = true;
        }

        public static void Submit(MapInfo Info)
        {
            if (Info == null || LifeSimMap.PhysicsManager == null)
            {
                return;
            }

            Vector3 Shift = ChunkOrigin.Shift(Info);
            LifeSimMap.PhysicsManager.ClearChunkMeshes(Info);
            if (Info.Heights != null && Info.Heights.PhysicsVertices != null && Info.Heights.PhysicsIndices != null && Info.Heights.PhysicsIndices.Length >= 3)
            {
                LifeSimMap.PhysicsManager.AddChunkMesh(Info, Info.Heights.PhysicsVertices, Info.Heights.PhysicsIndices, Shift);
            }

            if (Info.MeshVertices != null && Info.MeshIndices != null && Info.MeshIndices.Length >= 3)
            {
                LifeSimMap.PhysicsManager.AddChunkMesh(Info, Info.MeshVertices, Info.MeshIndices, Shift);
            }
        }

        public static void Gather(NavMapGameManager Manager, HashSet<MapInfo> Keep, IList<Vector3> Eyes)
        {
            if (Slot == null || Eyes == null)
            {
                return;
            }

            float BodyReach = NavMapGameManager.DrawChunkRadius;
            float HeaderReach = BodyReach + Slot.ChunkSize;
            for (int i = 0; i < Eyes.Count; i++)
            {
                Vector3 Eye = Eyes[i];
                int MinX = (int)Math.Floor((Eye.X - HeaderReach) / Slot.ChunkSize);
                int MaxX = (int)Math.Floor((Eye.X + HeaderReach) / Slot.ChunkSize);
                int MinZ = (int)Math.Floor((Eye.Z - HeaderReach) / Slot.ChunkSize);
                int MaxZ = (int)Math.Floor((Eye.Z + HeaderReach) / Slot.ChunkSize);
                for (int Z = MinZ; Z <= MaxZ; Z++)
                {
                    for (int X = MinX; X <= MaxX; X++)
                    {
                        float ChunkMinX = X * Slot.ChunkSize;
                        float ChunkMinZ = Z * Slot.ChunkSize;
                        float NearX = Eye.X < ChunkMinX ? ChunkMinX : Eye.X > ChunkMinX + Slot.ChunkSize ? ChunkMinX + Slot.ChunkSize : Eye.X;
                        float NearZ = Eye.Z < ChunkMinZ ? ChunkMinZ : Eye.Z > ChunkMinZ + Slot.ChunkSize ? ChunkMinZ + Slot.ChunkSize : Eye.Z;
                        float DX = Eye.X - NearX;
                        float DZ = Eye.Z - NearZ;
                        float Distance = (float)Math.Sqrt(DX * DX + DZ * DZ);
                        if (Distance > HeaderReach)
                        {
                            continue;
                        }

                        bool Body = Distance <= BodyReach;
                        MapInfo Info = ChunkAt(X, Z, Body);
                        if (Body)
                        {
                            Keep.Add(Info);
                            if (Info.Kind == GroundKind.Mesh)
                            {
                                MeshChunk.Ensure(Info, Eye.Y);
                            }
                        }

                        ConsiderLandmarks(Info, Eye, Keep);
                    }
                }
            }
        }

        static void ConsiderLandmarks(MapInfo Info, Vector3 Eye, HashSet<MapInfo> Keep)
        {
            if (Info.Landmarks == null)
            {
                return;
            }

            Vector3 Shift = ChunkOrigin.Shift(Info);
            for (int i = 0; i < Info.Landmarks.Count; i++)
            {
                LandmarkHeader Landmark = Info.Landmarks[i];
                Vector3 Min = Landmark.Min + Shift;
                Vector3 Max = Landmark.Max + Shift;
                if (DistanceToBox(Eye, Min, Max) <= Slot.GroundRadius)
                {
                    Keep.Add(Info);
                    Landmark.MeshOpen = true;
                }
                else
                {
                    Landmark.MeshOpen = false;
                }

                if (Landmark.Rooms == null)
                {
                    continue;
                }

                for (int r = 0; r < Landmark.Rooms.Count; r++)
                {
                    RoomHeader Room = Landmark.Rooms[r];
                    Vector3 RoomMin = Room.Min + Shift;
                    Vector3 RoomMax = Room.Max + Shift;
                    bool Open = Landmark.MeshOpen && DistanceToBox(Eye, RoomMin, RoomMax) <= RoomOpenDistance;
                    if (Open && !Room.PropsOpen)
                    {
                        ChunkFile.ReadRoomProps(Slot, Info, Room);
                        AdoptProps(Info, Room.Props, true);
                    }
                    else if (!Open && Room.PropsOpen)
                    {
                        AdoptProps(Info, Room.Props, false);
                        Room.PropsOpen = false;
                    }
                }
            }
        }

        static void AdoptProps(MapInfo Info, List<FurnitureItem> Props, bool Add)
        {
            if (Props == null)
            {
                return;
            }

            if (Info.Furniture == null)
            {
                Info.Furniture = new List<FurnitureItem>();
            }

            for (int i = 0; i < Props.Count; i++)
            {
                FurnitureItem Prop = Props[i];
                if (Prop == null)
                {
                    continue;
                }

                Prop.RoomProp = true;
                if (Add)
                {
                    if (!Info.Furniture.Contains(Prop))
                    {
                        Info.Furniture.Add(Prop);
                    }
                }
                else
                {
                    Info.Furniture.Remove(Prop);
                }
            }
        }

        public static float DistanceToBox(Vector3 Point, Vector3 Min, Vector3 Max)
        {
            float X = Point.X < Min.X ? Min.X : Point.X > Max.X ? Max.X : Point.X;
            float Y = Point.Y < Min.Y ? Min.Y : Point.Y > Max.Y ? Max.Y : Point.Y;
            float Z = Point.Z < Min.Z ? Min.Z : Point.Z > Max.Z ? Max.Z : Point.Z;
            float DX = Point.X - X;
            float DY = Point.Y - Y;
            float DZ = Point.Z - Z;
            return (float)Math.Sqrt(DX * DX + DY * DY + DZ * DZ);
        }

        public static void Save(MapInfo Info)
        {
            if (Slot == null || Info == null)
            {
                return;
            }

            if (Info.Kind == GroundKind.None && Info.Heights != null)
            {
                Info.Kind = GroundKind.Heightmap;
            }

            ChunkFile.Write(Slot, Info, true);
        }
    }
}
