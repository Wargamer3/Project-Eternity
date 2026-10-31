using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.UnitTests
{
    [TestClass]
    public class LifeSimNavTests
    {
        [TestMethod]
        public void RectangleOfTilesBakesAWalkAcrossTheTops()
        {
            LifeSimMap Map = NewMap(4, 4, 1);
            Fill(Map, 0, UnitStats.TerrainLandIndex, 0f);

            NavSourceGeometry Source = GridNavGeometry.Build(Map);

            Assert.AreEqual(32, Source.Triangles.Count);
            Assert.AreEqual(0, Source.Links.Count);
            for (int i = 0; i < Source.Triangles.Count; i++)
            {
                NavTriangle Triangle = Source.Triangles[i];
                Assert.AreEqual(NavArea.Ground, Triangle.Area);
                Assert.AreEqual(Triangle.A.Z, Triangle.B.Z);
                Assert.AreEqual(Triangle.A.Z, Triangle.C.Z);
            }

            Vector3 Start = Center(Map, 0, 0, 0);
            Vector3 Goal = Center(Map, 0, 3, 3);
            List<PathSegment> Segments = LocalPathQuery.Find(Map, Start, Goal, new TraversalCapabilities(), RouteStyle.Direct);

            Assert.IsTrue(Segments.Count > 0);
            Assert.AreEqual(Traversal.Walk, Segments[0].Kind);
            Assert.AreEqual(Pose.Stand, Segments[0].RequiredPose);
            Assert.AreEqual(NavArea.Ground, Segments[0].Surface);
            Vector3 End = Segments[Segments.Count - 1].Polyline[Segments[Segments.Count - 1].Polyline.Count - 1];
            Assert.IsTrue(Vector3.Distance(End, Goal) < Map.TileSize.X * 0.5f);
        }

        [TestMethod]
        public void WallCellIsLeftOutOfTheMesh()
        {
            LifeSimMap Map = NewMap(3, 1, 1);
            SetFloor(Map, 0, 0, 0, UnitStats.TerrainLandIndex, 0f);
            SetFloor(Map, 0, 1, 0, UnitStats.TerrainWallIndex, 0f);
            SetFloor(Map, 0, 2, 0, UnitStats.TerrainLandIndex, 0f);

            NavSourceGeometry Source = GridNavGeometry.Build(Map);

            Assert.AreEqual(4, Source.Triangles.Count);
            for (int i = 0; i < Source.Triangles.Count; i++)
            {
                Assert.IsFalse(CentroidInCell(Source.Triangles[i], Map, 1, 0));
            }

            List<PathSegment> Segments = LocalPathQuery.Find(Map, Center(Map, 0, 0, 0), Center(Map, 0, 2, 0), new TraversalCapabilities(), RouteStyle.Direct);
            Assert.AreEqual(0, Segments.Count);
        }

        [TestMethod]
        public void LowCeilingIsCrouch()
        {
            LifeSimMap Map = NewMap(1, 1, 2);
            SetFloor(Map, 0, 0, 0, UnitStats.TerrainLandIndex, 0f);
            SetFloor(Map, 1, 0, 0, UnitStats.TerrainLandIndex, 0f);

            NavSourceGeometry Source = GridNavGeometry.Build(Map);

            bool LowerIsCrouch = false;
            bool UpperIsGround = false;
            for (int i = 0; i < Source.Triangles.Count; i++)
            {
                NavTriangle Triangle = Source.Triangles[i];
                if (Triangle.A.Z == 0f)
                {
                    Assert.AreEqual(NavArea.Crouch, Triangle.Area);
                    LowerIsCrouch = true;
                }
                else
                {
                    Assert.AreEqual(NavArea.Ground, Triangle.Area);
                    UpperIsGround = true;
                }
            }

            Assert.IsTrue(LowerIsCrouch);
            Assert.IsTrue(UpperIsGround);
        }

        [TestMethod]
        public void TaggedLadderIsAnOffMeshLink()
        {
            LifeSimMap Map = NewMap(3, 1, 1);
            SetFloor(Map, 0, 0, 0, UnitStats.TerrainLandIndex, 0f);
            SetFloor(Map, 0, 1, 0, UnitStats.TerrainWallIndex, 0f);
            SetFloor(Map, 0, 2, 0, UnitStats.TerrainLandIndex, 0f);

            TeleportPoint Ladder = new TeleportPoint(Map.LayerManager.ListLayer[0].ArrayTerrain[0, 0].WorldPosition, "Ladder", 0, 0, 0);
            Ladder.OtherMapEntryPoint = new Point(2, 0);
            Ladder.OtherMapEntryLayer = 0;
            TeleportPoint Door = new TeleportPoint(Map.LayerManager.ListLayer[0].ArrayTerrain[0, 0].WorldPosition, "Door", 0, 0, 0);
            Door.OtherMapEntryPoint = new Point(2, 0);
            Door.OtherMapEntryLayer = 0;
            Map.LayerManager.ListLayer[0].ListTeleportPoint.Add(Ladder);
            Map.LayerManager.ListLayer[0].ListTeleportPoint.Add(Door);

            NavSourceGeometry Source = GridNavGeometry.Build(Map);

            Assert.AreEqual(1, Source.Links.Count);
            Assert.AreEqual(Traversal.Ladder, Source.Links[0].Kind);
            Assert.IsTrue(Vector3.Distance(Source.Links[0].Start, Center(Map, 0, 0, 0)) < 0.01f);
            Assert.IsTrue(Vector3.Distance(Source.Links[0].End, Center(Map, 0, 2, 0)) < 0.01f);

            TraversalCapabilities Climber = new TraversalCapabilities();
            Climber.CanLadder = true;
            List<PathSegment> Climbed = LocalPathQuery.Find(Map, Center(Map, 0, 0, 0), Center(Map, 0, 2, 0), Climber, RouteStyle.Direct);
            Assert.IsTrue(HasKind(Climbed, Traversal.Ladder));
            for (int i = 0; i < Climbed.Count; i++)
            {
                if (Climbed[i].Kind == Traversal.Ladder)
                {
                    Assert.AreEqual(Pose.Climb, Climbed[i].RequiredPose);
                }
            }

            List<PathSegment> Walker = LocalPathQuery.Find(Map, Center(Map, 0, 0, 0), Center(Map, 0, 2, 0), new TraversalCapabilities(), RouteStyle.Direct);
            Assert.AreEqual(0, Walker.Count);
        }

        [TestMethod]
        public void SlopeWithinTheLimitIsWalked()
        {
            LifeSimMap Map = NewMap(2, 1, 1);
            SetFloor(Map, 0, 0, 0, UnitStats.TerrainLandIndex, 0f);
            SetFloor(Map, 0, 1, 0, UnitStats.TerrainLandIndex, 0.9f);
            float Rise = Math.Abs(Map.LayerManager.ListLayer[0].ArrayTerrain[1, 0].WorldPosition.Z);
            float Angle = (float)(Math.Atan2(Rise, Map.TileSize.X) * (180.0 / Math.PI));
            Assert.IsTrue(Rise > NavMeshLimits.AgentMaxClimb);
            Assert.IsTrue(Angle <= NavMeshLimits.MaxWalkableSlope);

            NavSourceGeometry Source = GridNavGeometry.Build(Map);
            Assert.IsTrue(HasArea(Source, NavArea.Slope));

            List<PathSegment> Segments = LocalPathQuery.Find(Map, Center(Map, 0, 0, 0), Center(Map, 0, 1, 0), new TraversalCapabilities(), RouteStyle.Direct);
            Assert.IsTrue(HasSurface(Segments, NavArea.Slope));
        }

        [TestMethod]
        public void SteepDropIsAHole()
        {
            LifeSimMap Map = NewMap(2, 1, 1);
            SetFloor(Map, 0, 0, 0, UnitStats.TerrainLandIndex, 0f);
            SetFloor(Map, 0, 1, 0, UnitStats.TerrainLandIndex, 2f);
            float Rise = Math.Abs(Map.LayerManager.ListLayer[0].ArrayTerrain[1, 0].WorldPosition.Z);
            float Angle = (float)(Math.Atan2(Rise, Map.TileSize.X) * (180.0 / Math.PI));
            Assert.IsTrue(Angle > NavMeshLimits.MaxWalkableSlope);

            NavSourceGeometry Source = GridNavGeometry.Build(Map);
            Assert.IsFalse(HasArea(Source, NavArea.Slope));
            Assert.IsTrue(Source.Triangles.Count > 0);

            List<PathSegment> Segments = LocalPathQuery.Find(Map, Center(Map, 0, 0, 0), Center(Map, 0, 1, 0), new TraversalCapabilities(), RouteStyle.Direct);
            Assert.AreEqual(0, Segments.Count);
        }

        [TestMethod]
        public void DeepWaterIsSwimOnlyWhenTheBodyCanSwim()
        {
            LifeSimMap Map = NewMap(3, 1, 1);
            SetFloor(Map, 0, 0, 0, UnitStats.TerrainLandIndex, 0f);
            SetFloor(Map, 0, 1, 0, UnitStats.TerrainUnderwaterIndex, 0f);
            SetFloor(Map, 0, 2, 0, UnitStats.TerrainLandIndex, 0f);

            NavSourceGeometry Source = GridNavGeometry.Build(Map);
            Assert.IsTrue(HasArea(Source, NavArea.DeepWater));

            Vector3 Start = Center(Map, 0, 0, 0);
            Vector3 Goal = Center(Map, 0, 2, 0);
            TraversalCapabilities Swimmer = new TraversalCapabilities();
            Swimmer.CanSwim = true;
            List<PathSegment> Swam = LocalPathQuery.Find(Map, Start, Goal, Swimmer, RouteStyle.Direct);
            Assert.IsTrue(HasKind(Swam, Traversal.Swim));

            List<PathSegment> Walker = LocalPathQuery.Find(Map, Start, Goal, new TraversalCapabilities(), RouteStyle.Direct);
            Assert.AreEqual(0, Walker.Count);
        }

        private static LifeSimMap NewMap(int Width, int Height, int LayerCount)
        {
            LifeSimMap Map = new LifeSimMap();
            Map.MapSize = new Point(Width, Height);
            for (int Layer = 0; Layer < LayerCount; Layer++)
            {
                Map.LayerManager.ListLayer.Add(new MapLayer(Map, Layer));
            }

            return Map;
        }

        private static void Fill(LifeSimMap Map, int Layer, byte TerrainIndex, float HeightValue)
        {
            for (int Y = 0; Y < Map.MapSize.Y; Y++)
            {
                for (int X = 0; X < Map.MapSize.X; X++)
                {
                    SetFloor(Map, Layer, X, Y, TerrainIndex, HeightValue);
                }
            }
        }

        private static void SetFloor(LifeSimMap Map, int Layer, int X, int Y, byte TerrainIndex, float HeightValue)
        {
            Terrain Tile = Map.LayerManager.ListLayer[Layer].ArrayTerrain[X, Y];
            Tile.TerrainTypeIndex = TerrainIndex;
            Tile.Height = HeightValue;
            Tile.WorldPosition.Z = (HeightValue + Layer) * Map.LayerHeight;
            Tile.MovementCost = 1f;
        }

        private static Vector3 Center(LifeSimMap Map, int Layer, int X, int Y)
        {
            return GridNavGeometry.CellCenter(Map, Map.LayerManager.ListLayer[Layer].ArrayTerrain[X, Y]);
        }

        private static bool CentroidInCell(NavTriangle Triangle, LifeSimMap Map, int X, int Y)
        {
            Vector3 Centroid = (Triangle.A + Triangle.B + Triangle.C) / 3f;
            float X0 = X * Map.TileSize.X;
            float Y0 = Y * Map.TileSize.Y;
            return Centroid.X >= X0 && Centroid.X < X0 + Map.TileSize.X
                && Centroid.Y >= Y0 && Centroid.Y < Y0 + Map.TileSize.Y;
        }

        private static bool HasArea(NavSourceGeometry Source, NavArea Area)
        {
            for (int i = 0; i < Source.Triangles.Count; i++)
            {
                if (Source.Triangles[i].Area == Area)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasKind(List<PathSegment> Segments, Traversal Kind)
        {
            for (int i = 0; i < Segments.Count; i++)
            {
                if (Segments[i].Kind == Kind)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasSurface(List<PathSegment> Segments, NavArea Surface)
        {
            for (int i = 0; i < Segments.Count; i++)
            {
                if (Segments[i].Surface == Surface)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
