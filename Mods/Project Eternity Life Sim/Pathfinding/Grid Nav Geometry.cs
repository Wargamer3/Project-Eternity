using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class GridNavGeometry
    {
        public static NavSourceGeometry Build(LifeSimMap Map)
        {
            return Build(Map, false);
        }

        public static NavSourceGeometry Build(LifeSimMap Map, bool IncludeDefaultFloor)
        {
            NavSourceGeometry Geometry = new NavSourceGeometry();
            if (Map == null || Map.LayerManager == null)
            {
                return Geometry;
            }

            for (int LayerIndex = 0; LayerIndex < Map.LayerManager.ListLayer.Count; LayerIndex++)
            {
                MapLayer Layer = Map.LayerManager.ListLayer[LayerIndex];
                AddFloors(Map, Layer, LayerIndex, Geometry, IncludeDefaultFloor);
                AddTaggedLinks(Map, Layer, LayerIndex, Geometry, IncludeDefaultFloor);
            }

            return Geometry;
        }

        private static void AddFloors(LifeSimMap Map, MapLayer Layer, int LayerIndex, NavSourceGeometry Geometry, bool IncludeDefaultFloor)
        {
            int Width = Map.MapSize.X;
            int Height = Map.MapSize.Y;
            bool[] Keep = new bool[Width * Height];
            NavArea[] Area = new NavArea[Width * Height];
            bool[] CoveredBySlope = new bool[Width * Height];

            for (int Y = 0; Y < Height; Y++)
            {
                for (int X = 0; X < Width; X++)
                {
                    Terrain Tile = Layer.ArrayTerrain[X, Y];
                    if (!IsWalkableFloor(Tile, IncludeDefaultFloor))
                    {
                        continue;
                    }

                    float Gap = GapAbove(Map, X, Y, Tile.WorldPosition.Z, LayerIndex);
                    NavArea Floor = FloorArea(Tile);
                    if (Gap >= 0f && Gap < NavMeshLimits.CrawlHeight)
                    {
                        continue;
                    }

                    if (Floor == NavArea.Ground && Gap >= 0f && Gap < NavMeshLimits.CrouchHeight)
                    {
                        Floor = NavArea.Crawl;
                    }
                    else if (Floor == NavArea.Ground && Gap >= 0f && Gap < NavMeshLimits.StandingHeight)
                    {
                        Floor = NavArea.Crouch;
                    }

                    int Index = X + Y * Width;
                    Keep[Index] = true;
                    Area[Index] = Floor;
                }
            }

            for (int Y = 0; Y < Height; Y++)
            {
                for (int X = 0; X < Width; X++)
                {
                    if (!Keep[X + Y * Width])
                    {
                        continue;
                    }

                    TrySlope(Map, Layer, Keep, CoveredBySlope, X, Y, X + 1, Y, true);
                    TrySlope(Map, Layer, Keep, CoveredBySlope, X, Y, X, Y + 1, false);
                }
            }

            for (int Y = 0; Y < Height; Y++)
            {
                for (int X = 0; X < Width; X++)
                {
                    int Index = X + Y * Width;
                    if (!Keep[Index] || CoveredBySlope[Index])
                    {
                        continue;
                    }

                    AddFlatTop(Geometry, Layer.ArrayTerrain[X, Y], Map.TileSize, Area[Index]);
                }
            }

            for (int Y = 0; Y < Height; Y++)
            {
                for (int X = 0; X < Width; X++)
                {
                    if (!Keep[X + Y * Width])
                    {
                        continue;
                    }

                    AddSlopeIfLower(Geometry, Map, Layer, Keep, X, Y, X + 1, Y, true);
                    AddSlopeIfLower(Geometry, Map, Layer, Keep, X, Y, X, Y + 1, false);
                }
            }
        }

        private static void TrySlope(LifeSimMap Map, MapLayer Layer, bool[] Keep, bool[] CoveredBySlope, int X, int Y, int NextX, int NextY, bool AlongX)
        {
            if (NextX >= Map.MapSize.X || NextY >= Map.MapSize.Y)
            {
                return;
            }

            int Width = Map.MapSize.X;
            if (!Keep[X + Y * Width] || !Keep[NextX + NextY * Width])
            {
                return;
            }

            Terrain First = Layer.ArrayTerrain[X, Y];
            Terrain Second = Layer.ArrayTerrain[NextX, NextY];
            Terrain Lower = LowerOf(First, Second);
            if (Lower == null || !IsSlopeBetween(Map, First, Second, AlongX))
            {
                return;
            }

            CoveredBySlope[Lower.GridPosition.X + Lower.GridPosition.Y * Width] = true;
        }

        private static void AddSlopeIfLower(NavSourceGeometry Geometry, LifeSimMap Map, MapLayer Layer, bool[] Keep, int X, int Y, int NextX, int NextY, bool AlongX)
        {
            if (NextX >= Map.MapSize.X || NextY >= Map.MapSize.Y)
            {
                return;
            }

            int Width = Map.MapSize.X;
            if (!Keep[X + Y * Width] || !Keep[NextX + NextY * Width])
            {
                return;
            }

            Terrain First = Layer.ArrayTerrain[X, Y];
            Terrain Second = Layer.ArrayTerrain[NextX, NextY];

            if (!IsSlopeBetween(Map, First, Second, AlongX))
            {
                return;
            }

            Terrain Lower = LowerOf(First, Second);
            Terrain Upper = First == Lower ? Second : First;
            AddSlopeQuad(Geometry, Map, Lower, Upper, AlongX);
        }

        private static bool IsSlopeBetween(LifeSimMap Map, Terrain First, Terrain Second, bool AlongX)
        {
            float Rise = Math.Abs(First.WorldPosition.Z - Second.WorldPosition.Z);
            if (Rise <= NavMeshLimits.AgentMaxClimb)
            {
                return false;
            }

            float Run = AlongX ? Map.TileSize.X : Map.TileSize.Y;
            if (Run <= 0f)
            {
                return false;
            }

            float Angle = (float)(Math.Atan2(Rise, Run) * (180.0 / Math.PI));
            return Angle <= NavMeshLimits.MaxWalkableSlope;
        }

        private static Terrain LowerOf(Terrain First, Terrain Second)
        {
            if (First.WorldPosition.Z < Second.WorldPosition.Z)
            {
                return First;
            }

            if (Second.WorldPosition.Z < First.WorldPosition.Z)
            {
                return Second;
            }

            return null;
        }

        private static void AddFlatTop(NavSourceGeometry Geometry, Terrain Tile, Point TileSize, NavArea Area)
        {
            float X0 = Tile.WorldPosition.X;
            float Y0 = Tile.WorldPosition.Y;
            float X1 = X0 + TileSize.X;
            float Y1 = Y0 + TileSize.Y;
            float Z = Tile.WorldPosition.Z;
            AddQuad(Geometry,
                new Vector3(X0, Y0, Z),
                new Vector3(X0, Y1, Z),
                new Vector3(X1, Y1, Z),
                new Vector3(X1, Y0, Z),
                Area);
        }

        private static void AddSlopeQuad(NavSourceGeometry Geometry, LifeSimMap Map, Terrain Lower, Terrain Upper, bool AlongX)
        {
            float X0 = Lower.WorldPosition.X;
            float Y0 = Lower.WorldPosition.Y;
            float X1 = X0 + Map.TileSize.X;
            float Y1 = Y0 + Map.TileSize.Y;
            float Low = Lower.WorldPosition.Z;
            float High = Upper.WorldPosition.Z;

            if (AlongX)
            {
                float ZLeft = Upper.GridPosition.X < Lower.GridPosition.X ? High : Low;
                float ZRight = Upper.GridPosition.X < Lower.GridPosition.X ? Low : High;
                AddQuad(Geometry,
                    new Vector3(X0, Y0, ZLeft),
                    new Vector3(X0, Y1, ZLeft),
                    new Vector3(X1, Y1, ZRight),
                    new Vector3(X1, Y0, ZRight),
                    NavArea.Slope);
                return;
            }

            float ZDown = Upper.GridPosition.Y < Lower.GridPosition.Y ? High : Low;
            float ZUp = Upper.GridPosition.Y < Lower.GridPosition.Y ? Low : High;
            AddQuad(Geometry,
                new Vector3(X0, Y0, ZDown),
                new Vector3(X0, Y1, ZUp),
                new Vector3(X1, Y1, ZUp),
                new Vector3(X1, Y0, ZDown),
                NavArea.Slope);
        }

        private static void AddQuad(NavSourceGeometry Geometry, Vector3 A, Vector3 B, Vector3 C, Vector3 D, NavArea Area)
        {
            Geometry.Triangles.Add(new NavTriangle(A, B, C, Area));
            Geometry.Triangles.Add(new NavTriangle(A, C, D, Area));
        }

        private static void AddTaggedLinks(LifeSimMap Map, MapLayer Layer, int LayerIndex, NavSourceGeometry Geometry, bool IncludeDefaultFloor)
        {
            for (int i = 0; i < Layer.ListTeleportPoint.Count; i++)
            {
                TeleportPoint Point = Layer.ListTeleportPoint[i];
                Traversal Kind;
                if (!Enum.TryParse(Point.Tag, out Kind) || !IsLink(Kind))
                {
                    continue;
                }

                int StartX = (int)Math.Floor(Point.Position.X / Map.TileSize.X);
                int StartY = (int)Math.Floor(Point.Position.Y / Map.TileSize.Y);
                Terrain Start = FloorAt(Map, LayerIndex, StartX, StartY);
                Terrain End = FloorAt(Map, Point.OtherMapEntryLayer, Point.OtherMapEntryPoint.X, Point.OtherMapEntryPoint.Y);
                if (!IsWalkableFloor(Start, IncludeDefaultFloor) || !IsWalkableFloor(End, IncludeDefaultFloor))
                {
                    continue;
                }

                Geometry.Links.Add(new NavOffMeshLink(Kind, CellCenter(Map, Start), CellCenter(Map, End)));
            }
        }

        private static Terrain FloorAt(LifeSimMap Map, int LayerIndex, int X, int Y)
        {
            if (LayerIndex < 0 || LayerIndex >= Map.LayerManager.ListLayer.Count)
            {
                return null;
            }

            if (X < 0 || Y < 0 || X >= Map.MapSize.X || Y >= Map.MapSize.Y)
            {
                return null;
            }

            return Map.LayerManager.ListLayer[LayerIndex].ArrayTerrain[X, Y];
        }

        public static Vector3 CellCenter(LifeSimMap Map, Terrain Tile)
        {
            return new Vector3(
                Tile.WorldPosition.X + Map.TileSize.X * 0.5f,
                Tile.WorldPosition.Y + Map.TileSize.Y * 0.5f,
                Tile.WorldPosition.Z);
        }

        private static NavArea FloorArea(Terrain Tile)
        {
            if (Tile.TerrainTypeIndex == UnitStats.TerrainSeaIndex)
            {
                return NavArea.ShallowWater;
            }

            if (Tile.TerrainTypeIndex == UnitStats.TerrainUnderwaterIndex)
            {
                return NavArea.DeepWater;
            }

            return NavArea.Ground;
        }

        private static bool IsWalkableFloor(Terrain Tile)
        {
            return IsWalkableFloor(Tile, false);
        }

        private static bool IsWalkableFloor(Terrain Tile, bool IncludeDefaultFloor)
        {
            if (Tile == null || Tile.MovementCost == -1f)
            {
                return false;
            }

            if (Tile.TerrainTypeIndex == UnitStats.TerrainVoidIndex || Tile.TerrainTypeIndex == UnitStats.TerrainAirIndex)
            {
                return false;
            }

            if (Tile.TerrainTypeIndex == UnitStats.TerrainWallIndex && !IncludeDefaultFloor)
            {
                return false;
            }

            return true;
        }

        private static bool IsSolidCeiling(Terrain Tile)
        {
            if (Tile == null)
            {
                return false;
            }

            return Tile.TerrainTypeIndex != UnitStats.TerrainVoidIndex && Tile.TerrainTypeIndex != UnitStats.TerrainAirIndex;
        }

        private static float GapAbove(LifeSimMap Map, int X, int Y, float FloorZ, int FloorLayer)
        {
            float Nearest = -1f;
            for (int LayerIndex = FloorLayer + 1; LayerIndex < Map.LayerManager.ListLayer.Count; LayerIndex++)
            {
                Terrain Above = Map.LayerManager.ListLayer[LayerIndex].ArrayTerrain[X, Y];
                if (!IsSolidCeiling(Above))
                {
                    continue;
                }

                float Gap = Above.WorldPosition.Z - FloorZ;
                if (Gap <= 0f)
                {
                    continue;
                }

                if (Nearest < 0f || Gap < Nearest)
                {
                    Nearest = Gap;
                }
            }

            return Nearest;
        }

        private static bool IsLink(Traversal Kind)
        {
            return Kind == Traversal.Jump
                || Kind == Traversal.Leap
                || Kind == Traversal.LeapOver
                || Kind == Traversal.JumpPad
                || Kind == Traversal.Ladder
                || Kind == Traversal.Rope
                || Kind == Traversal.WallRun
                || Kind == Traversal.WallClimb
                || Kind == Traversal.LedgeClimb;
        }
    }
}
