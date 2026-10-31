using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.UnitTests
{
    [TestClass]
    public class LifeSimPlayTests
    {
        [TestMethod]
        public void AdminAndBuildKeysLeaveTheBodyAsItWas()
        {
            PlayerCharacter Ada = new PlayerCharacter(null, null, Vector3.Zero);
            Ada.AIControlled = true;
            PlaySession Session = new PlaySession();

            Session.ApplyKey(Keys.F1);
            Assert.IsTrue(Session.AdminOpen);
            Session.ApplyKey(Keys.F1);
            Assert.IsFalse(Session.AdminOpen);

            Session.ApplyKey(Keys.Escape);
            Assert.IsTrue(Session.BuildOpen);
            Session.ApplyKey(Keys.Escape);
            Assert.IsFalse(Session.BuildOpen);
            Assert.IsTrue(Ada.AIControlled);
        }

        [TestMethod]
        public void CameraToggleCyclesFirstThirdAndLife()
        {
            PlayerCharacter Ada = new PlayerCharacter(null, null, Vector3.Zero);
            PlaySession Session = new PlaySession();
            Session.TakeCharacter(Ada);
            int Taken = Session.TakeCount;

            Session.ToggleView();
            Assert.AreEqual(CameraStyle.Orbit, Session.View);
            Assert.AreEqual(Taken, Session.TakeCount);
            Assert.IsFalse(Ada.AIControlled);
            Assert.AreSame(Ada, Session.Controlled);

            Session.ToggleView();
            Assert.AreEqual(CameraStyle.Life, Session.View);
            Assert.AreEqual(Taken, Session.TakeCount);
            Assert.IsTrue(Ada.AIControlled);
            Assert.IsNull(Session.Controlled);
            Assert.AreSame(Ada, Session.LifeSelection);

            Session.ToggleView();
            Assert.AreEqual(CameraStyle.Eyes, Session.View);
            Assert.AreEqual(Taken + 1, Session.TakeCount);
            Assert.IsFalse(Ada.AIControlled);
            Assert.AreSame(Ada, Session.Controlled);
        }

        [TestMethod]
        public void WatchSitsInAnEmptyChairAndStandsWhenTheChairIsTaken()
        {
            LifeSimMap Map = NewMap(6, 4);
            Fill(Map, UnitStats.TerrainLandIndex);
            MapInfo Room = new MapInfo(new MapContainer("Room", Map), null, null);
            PlayerCharacter Ada = new PlayerCharacter(null, Room, Center(Map, 0, 2));
            Ada.AIControlled = true;
            FurnitureItem Screen = BuildPlan.Commit("Television", Center(Map, 2, 1), Vector3.Zero, false, 0, true, 32f, Room.Furniture, Room.Buildings).AddFurniture[0];
            FurnitureItem Chair = BuildPlan.Commit("Chair", Center(Map, 2, 2), Vector3.Zero, false, 0, true, 32f, Room.Furniture, Room.Buildings).AddFurniture[0];
            Room.Furniture.Add(Screen);
            Room.Furniture.Add(Chair);

            Assert.IsTrue(LifeOrder.Watch(Ada, Screen, Map));
            List<AutomatedAction> Steps = Ada.TopObjective().ListRemainingStep;
            Assert.AreEqual("Watch", Steps[Steps.Count - 1].ActionName);
            bool Sat = false;
            for (int i = 0; i < Steps.Count; i++)
            {
                if (Steps[i].ActionName == "Sit Down")
                {
                    Sat = true;
                }
            }

            Assert.IsTrue(Sat);
            FollowPath Path = Steps[0] as FollowPath;
            Assert.IsNotNull(Path);
            Vector3 End = Path.Segments[Path.Segments.Count - 1].Polyline[Path.Segments[Path.Segments.Count - 1].Polyline.Count - 1];
            Assert.IsTrue(Vector3.Distance(End, Chair.Position) < Map.TileSize.X);

            PlayerCharacter Guest = new PlayerCharacter(null, Room, Chair.Position);
            Map.ListCharacter.Add(Guest);
            Ada.ListAIObjective.Clear();
            Assert.IsTrue(LifeOrder.Watch(Ada, Screen, Map));
            Steps = Ada.TopObjective().ListRemainingStep;
            Assert.AreEqual("Watch", Steps[Steps.Count - 1].ActionName);
            for (int i = 0; i < Steps.Count; i++)
            {
                Assert.AreNotEqual("Sit Down", Steps[i].ActionName);
            }

            Path = Steps[0] as FollowPath;
            End = Path.Segments[Path.Segments.Count - 1].Polyline[Path.Segments[Path.Segments.Count - 1].Polyline.Count - 1];
            Assert.IsTrue(Vector3.Distance(End, Chair.Position) > Map.TileSize.X * 0.5f);
        }

        [TestMethod]
        public void FridgeOrderLeavesTheAIOnAndWalksBesideTheCube()
        {
            LifeSimMap Map = NewMap(4, 1);
            Fill(Map, UnitStats.TerrainLandIndex);
            MapInfo Room = new MapInfo(new MapContainer("Room", Map), null, null);
            PlayerCharacter Ada = new PlayerCharacter(null, Room, Center(Map, 0, 0));
            Ada.AIControlled = true;
            FurnitureItem Fridge = BoxPlace.Place(Room.Furniture, Center(Map, 3, 0), Map.TileSize.X, BoxKind.SmallCube, "Fridge", false);
            Room.Furniture.Add(Fridge);

            LifeOrder.Fridge(Ada, Fridge, Map);

            Assert.IsTrue(Ada.AIControlled);
            Assert.IsTrue(Ada.TopObjective().IsPlayerOrder);
            Assert.AreEqual("Eat", Ada.TopObjective().ListRemainingStep[Ada.TopObjective().ListRemainingStep.Count - 1].ActionName);
            FollowPath Path = Ada.TopObjective().ListRemainingStep[0] as FollowPath;
            Assert.IsNotNull(Path);
            Vector3 End = Path.Segments[Path.Segments.Count - 1].Polyline[Path.Segments[Path.Segments.Count - 1].Polyline.Count - 1];
            Assert.IsTrue(Vector3.Distance(End, Fridge.Beside()) < Map.TileSize.X);
        }

        [TestMethod]
        public void AGroundOrderWalksTheSelectedCharacterThere()
        {
            LifeSimMap Map = NewMap(4, 1);
            Fill(Map, UnitStats.TerrainLandIndex);
            MapInfo Room = new MapInfo(new MapContainer("Room", Map), null, null);
            PlayerCharacter Ada = new PlayerCharacter(null, Room, Center(Map, 0, 0));
            Ada.AIControlled = true;
            Vector3 Spot = Center(Map, 3, 0);

            Assert.IsTrue(LifeOrder.Walk(Ada, Spot, Map));

            Assert.IsTrue(Ada.AIControlled);
            FollowPath Path = Ada.TopObjective().ListRemainingStep[0] as FollowPath;
            Assert.IsNotNull(Path);
            Vector3 End = Path.Segments[Path.Segments.Count - 1].Polyline[Path.Segments[Path.Segments.Count - 1].Polyline.Count - 1];
            Assert.IsTrue(Vector3.Distance(End, Spot) < Map.TileSize.X);
        }

        [TestMethod]
        public void AWalkOrderOnTheNextMapCrossesTheBorderThenWalksToTheSpot()
        {
            LifeSimMap WestMap = NewMap(4, 4);
            LifeSimMap EastMap = NewMap(4, 4);
            Fill(WestMap, UnitStats.TerrainLandIndex);
            Fill(EastMap, UnitStats.TerrainLandIndex);
            MapInfo West = new MapInfo(new MapContainer("West", WestMap), null, null);
            MapInfo East = new MapInfo(new MapContainer("East", EastMap), null, null);
            WestMap.Chunk = West;
            EastMap.Chunk = East;
            float Span = WestMap.MapSize.X * WestMap.TileSize.X;
            West.MapMapContainer.SpanX = Span;
            West.MapMapContainer.SpanY = Span;
            East.MapMapContainer.OffsetX = (long)Span;
            East.MapMapContainer.SpanX = Span;
            East.MapMapContainer.SpanY = Span;
            ChunkHop Border = new ChunkHop();
            Border.Name = "Border";
            Border.FromChunk = West;
            Border.ToChunk = East;
            Border.FromPosition = new Vector3(Span, Span * 0.5f, 0f);
            Border.ToPosition = new Vector3(0f, Span * 0.5f, 0f);
            Border.IsOpen = true;
            West.ListLink.Add(Border);
            East.ListLink.Add(Border.Reversed());

            PlayerCharacter Ada = new PlayerCharacter(null, West, Center(WestMap, 0, 2));
            Ada.AIControlled = true;
            Vector3 Spot = Center(EastMap, 2, 2);

            East.ListLink.Clear();
            West.ListLink.Clear();
            Assert.IsFalse(LifeOrder.Walk(Ada, Spot, EastMap));
            Assert.IsNull(Ada.TopObjective());

            West.ListLink.Add(Border);
            East.ListLink.Add(Border.Reversed());
            Assert.IsTrue(LifeOrder.Walk(Ada, Spot, EastMap));

            List<AutomatedAction> Steps = Ada.TopObjective().ListRemainingStep;
            Assert.AreEqual(3, Steps.Count);
            FollowPath ToBorder = Steps[0] as FollowPath;
            Assert.IsNotNull(ToBorder);
            Assert.AreEqual("Cross Portal", Steps[1].ActionName);
            FollowPath ToSpot = Steps[2] as FollowPath;
            Assert.IsNotNull(ToSpot);
            Vector3 Edge = ToBorder.Segments[ToBorder.Segments.Count - 1].Polyline[ToBorder.Segments[ToBorder.Segments.Count - 1].Polyline.Count - 1];
            Vector3 Arrival = ToSpot.Segments[0].Polyline[0];
            Vector3 End = ToSpot.Segments[ToSpot.Segments.Count - 1].Polyline[ToSpot.Segments[ToSpot.Segments.Count - 1].Polyline.Count - 1];
            Assert.IsTrue(Edge.X > Ada.Position.X);
            Assert.IsTrue(Arrival.X < WestMap.TileSize.X);
            Assert.IsTrue(Vector3.Distance(End, Spot) < EastMap.TileSize.X);
            Assert.AreEqual(Span, Edge.X, 0.01f);
        }

        [TestMethod]
        public void SheWalksAcrossTheBorderWithoutStoppingOrJumping()
        {
            LifeSimMap WestMap = NewMap(4, 4);
            LifeSimMap EastMap = NewMap(4, 4);
            Fill(WestMap, UnitStats.TerrainLandIndex);
            Fill(EastMap, UnitStats.TerrainLandIndex);
            MapInfo West = new MapInfo(new MapContainer("West", WestMap), null, null);
            MapInfo East = new MapInfo(new MapContainer("East", EastMap), null, null);
            WestMap.Chunk = West;
            EastMap.Chunk = East;
            float Span = WestMap.MapSize.X * WestMap.TileSize.X;
            West.MapMapContainer.SpanX = Span;
            West.MapMapContainer.SpanY = Span;
            East.MapMapContainer.OffsetX = (long)Span;
            East.MapMapContainer.SpanX = Span;
            East.MapMapContainer.SpanY = Span;
            ChunkHop Border = new ChunkHop();
            Border.Name = "Border";
            Border.FromChunk = West;
            Border.ToChunk = East;
            Border.FromPosition = new Vector3(Span, Span * 0.5f, 0f);
            Border.ToPosition = new Vector3(0f, Span * 0.5f, 0f);
            Border.IsOpen = true;
            West.ListLink.Add(Border);
            East.ListLink.Add(Border.Reversed());
            PlayerCharacter Ada = new PlayerCharacter(null, West, Center(WestMap, 0, 2));
            Ada.AIControlled = true;
            List<MapInfo> PreviousMaps = ChunkCross.Maps;
            ChunkCross.Maps = new List<MapInfo> { West, East };
            Assert.IsTrue(LifeOrder.Walk(Ada, Center(EastMap, 2, 2), EastMap));

            float Previous = Ada.Position.X + West.MapMapContainer.OffsetX;
            bool Moving = false;
            bool Crossed = false;
            float Jump = 0f;
            int Still = 0;
            int WorstStill = 0;
            GameTime Tick = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1.0 / 60.0));
            try
            {
                for (int Frame = 0; Frame < 900 && !(Crossed && Ada.TopObjective() == null); Frame++)
                {
                    Ada.Update(Tick);
                    float Now = Ada.Position.X + Ada.SharedMapContex.CurrentMapInfo.MapMapContainer.OffsetX;
                    float Delta = Math.Abs(Now - Previous);
                    if (!Crossed && Ada.SharedMapContex.CurrentMapInfo == East)
                    {
                        Crossed = true;
                        Jump = Delta;
                    }

                    if (Delta > 0.05f)
                    {
                        Moving = true;
                        if (Still > WorstStill)
                        {
                            WorstStill = Still;
                        }

                        Still = 0;
                    }
                    else if (Moving)
                    {
                        Still++;
                    }

                    Previous = Now;
                }
            }
            finally
            {
                ChunkCross.Maps = PreviousMaps;
            }

            if (Still > WorstStill)
            {
                WorstStill = Still;
            }

            Assert.IsTrue(Crossed);
            Assert.IsTrue(Jump < 2f, "Border jump was " + Jump);
            Assert.IsTrue(WorstStill <= 2, "Stood still for " + WorstStill + " frames");
        }

        [TestMethod]
        public void WalkOrderMovesAcrossAFloorTheNavMeshSkips()
        {
            LifeSimMap Map = NewMap(4, 1);
            Fill(Map, UnitStats.TerrainWallIndex);
            MapInfo Room = new MapInfo(new MapContainer("Room", Map), null, null);
            PlayerCharacter Ada = new PlayerCharacter(null, Room, Center(Map, 0, 0));
            Ada.AIControlled = true;
            Vector3 Spot = Center(Map, 3, 0);

            Assert.IsTrue(LifeOrder.Walk(Ada, Spot, Map));

            FollowPath Path = Ada.TopObjective().ListRemainingStep[0] as FollowPath;
            Assert.IsNotNull(Path);
            Vector3 End = Path.Segments[Path.Segments.Count - 1].Polyline[Path.Segments[Path.Segments.Count - 1].Polyline.Count - 1];
            Assert.IsTrue(Vector3.Distance(End, Spot) < Map.TileSize.X * 0.5f);
        }

        [TestMethod]
        public void IdleWalkStaysPutWithoutADestination()
        {
            LifeSimMap Map = NewMap(4, 4);
            Fill(Map, UnitStats.TerrainLandIndex);
            MapInfo Room = new MapInfo(new MapContainer("Room", Map), null, null);
            Vector3 Start = Center(Map, 2, 2);
            PlayerCharacter Ada = new PlayerCharacter(null, Room, Start);
            Ada.AIControlled = true;
            WalkToPositionAction Walk = new WalkToPositionAction();
            Walk.Init(Ada.SharedMapContex);
            Ada.AddAction(Walk);

            Ada.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1)));

            Assert.AreEqual(0, Walk.AIWeight);
            Assert.AreEqual(0, Walk.GetAIExecutionPlan(null).Count);
            Assert.AreEqual(0, Ada.ListAIObjective.Count);
            Assert.AreEqual(Start, Ada.Position);
        }

        [TestMethod]
        public void LaterMoveOrdersWaitBehindTheOneSheIsDoing()
        {
            LifeSimMap Map = NewMap(4, 4);
            Fill(Map, UnitStats.TerrainLandIndex);
            MapInfo Room = new MapInfo(new MapContainer("Room", Map), null, null);
            PlayerCharacter Ada = new PlayerCharacter(null, Room, Center(Map, 0, 0));
            Assert.IsTrue(LifeOrder.Walk(Ada, Center(Map, 0, 3), Map));
            Assert.IsTrue(LifeOrder.Walk(Ada, Center(Map, 3, 3), Map));
            Assert.IsTrue(LifeOrder.Walk(Ada, Center(Map, 3, 0), Map));

            Assert.AreEqual(3, Ada.ListAIObjective.Count);
            AIObjective Doing = Ada.TopObjective();
            Assert.AreSame(Doing, Ada.ListAIObjective[2]);
            Assert.AreEqual("Follow Path", Doing.ListRemainingStep[0].ActionName);
            Assert.AreEqual("Move", Ada.ListAIObjective[1].GoalName);
            Assert.AreEqual("Move", Ada.ListAIObjective[0].GoalName);
            Assert.AreEqual(1, Ada.ListAIObjective[1].ListRemainingStep.Count);
            Assert.AreEqual(1, Ada.ListAIObjective[0].ListRemainingStep.Count);

            Doing.ListRemainingStep.Clear();
            Ada.AIControlled = true;
            Ada.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.05f)));
            Assert.AreSame(Ada.ListAIObjective[1], Ada.TopObjective());
            Assert.AreEqual("Follow Path", Ada.TopObjective().ListRemainingStep[0].ActionName);
        }

        [TestMethod]
        public void AQueuedMoveStartsWhereSheIsInsteadOfWhereSheClicked()
        {
            LifeSimMap Map = NewMap(6, 6);
            Fill(Map, UnitStats.TerrainLandIndex);
            MapInfo Room = new MapInfo(new MapContainer("Room", Map), null, null);
            Vector3 Start = Center(Map, 0, 0);
            Vector3 First = Center(Map, 0, 5);
            Vector3 Second = Center(Map, 5, 5);
            Vector3 Third = Center(Map, 5, 0);
            PlayerCharacter Ada = new PlayerCharacter(null, Room, Start);
            Ada.AIControlled = true;
            Assert.IsTrue(LifeOrder.Walk(Ada, First, Map));
            Assert.IsTrue(LifeOrder.Walk(Ada, Second, Map));
            Assert.IsTrue(LifeOrder.Walk(Ada, Third, Map));

            Ada.SetPosition(First);
            Ada.TopObjective().ListRemainingStep.Clear();
            Ada.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.05f)));
            FollowPath ToSecond = Ada.TopObjective().ListRemainingStep[0] as FollowPath;
            Assert.IsNotNull(ToSecond);
            Vector3 Began = ToSecond.Segments[0].Polyline[0];
            Assert.IsTrue(Vector3.Distance(Began, First) < 20f, "Second move starts at " + Began);
            Assert.IsTrue(Vector3.Distance(Began, Start) > 40f, "Second move walks back to " + Began);

            Ada.SetPosition(Second);
            Ada.TopObjective().ListRemainingStep.Clear();
            Ada.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.05f)));
            FollowPath ToThird = Ada.TopObjective().ListRemainingStep[0] as FollowPath;
            Assert.IsNotNull(ToThird);
            Vector3 Again = ToThird.Segments[0].Polyline[0];
            Assert.IsTrue(Vector3.Distance(Again, Second) < 20f, "Third move starts at " + Again);
            for (int s = 0; s < ToThird.Segments.Count; s++)
            {
                for (int p = 0; p < ToThird.Segments[s].Polyline.Count; p++)
                {
                    Vector3 Point = ToThird.Segments[s].Polyline[p];
                    Assert.IsTrue(Vector3.Distance(Point, Start) > 40f, "Third move revisits the start at " + Point);
                    Assert.IsTrue(Vector3.Distance(Point, First) > 40f, "Third move revisits the first stop at " + Point);
                }
            }
        }

        [TestMethod]
        public void AutonomousPathGoesAroundAWall()
        {
            LifeSimMap Map = NewMap(8, 3);
            Fill(Map, UnitStats.TerrainLandIndex);
            MapInfo Room = new MapInfo(new MapContainer("Room", Map), null, null);
            PlayerCharacter Ada = new PlayerCharacter(null, Room, Center(Map, 0, 0));
            BuildingItem Wall = new BuildingItem();
            Wall.Kind = BuildingKind.Wall;
            Wall.Position = new Vector3(96f, 48f, 0f);
            Wall.Width = 192f;
            Wall.Length = 8f;
            Wall.Height = 96f;
            Map.Buildings.Add(Wall);

            FollowPath Path = Ada.PlanPath(Map, Center(Map, 0, 2), false);
            float Furthest = 0f;
            for (int s = 0; s < Path.Segments.Count; s++)
            {
                for (int p = 0; p < Path.Segments[s].Polyline.Count; p++)
                {
                    if (Path.Segments[s].Polyline[p].X > Furthest)
                    {
                        Furthest = Path.Segments[s].Polyline[p].X;
                    }
                }
            }

            Assert.IsTrue(Furthest > 140f, "Path went through the wall, furthest X " + Furthest);
        }

        [TestMethod]
        public void AnAdminCubeBlocksTheRouteAndTheBody()
        {
            LifeSimMap Map = NewMap(8, 4);
            Fill(Map, UnitStats.TerrainLandIndex);
            MapInfo Room = new MapInfo(new MapContainer("Room", Map), null, null);
            Vector3 Start = Center(Map, 1, 1);
            Vector3 Goal = Center(Map, 6, 1);
            FurnitureItem Cube = BoxPlace.Place(Room.Furniture, Center(Map, 3, 1), Map.TileSize.X, BoxKind.SmallCube, null, false);
            BoxPlace.Keep(Room, Cube);
            FurnitureItem Long = BoxPlace.Place(Room.Furniture, Center(Map, 3, 2), Map.TileSize.X, BoxKind.Long, null, false);
            BoxPlace.Keep(Room, Long);
            Assert.IsTrue(Cube.BlocksBody());
            Assert.IsTrue(Long.BlocksBody());
            Assert.IsFalse(Long.IsRope);

            PlayerCharacter Ada = new PlayerCharacter(null, Room, Start);
            Ada.AIControlled = true;
            Assert.IsTrue(LifeOrder.Walk(Ada, Goal, Map));
            FollowPath Path = Ada.TopObjective().ListRemainingStep[0] as FollowPath;
            for (int s = 0; s < Path.Segments.Count; s++)
            {
                List<Vector3> Line = Path.Segments[s].Polyline;
                for (int p = 1; p < Line.Count; p++)
                {
                    Assert.IsFalse(Cube.HitsMove(Line[p - 1], Line[p]), "Path cuts the cube");
                }
            }

            GameTime Step = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.05f));
            for (int i = 0; i < 900 && System.Math.Abs(Ada.Position.X - Goal.X) > 8f; i++)
            {
                Ada.Update(Step);
                Assert.IsFalse(Cube.FootprintContains(Ada.Position.X, Ada.Position.Y), "Walked into the cube at " + Ada.Position);
            }

            Assert.IsTrue(System.Math.Abs(Ada.Position.X - Goal.X) <= 8f, "Never got past the cube, at " + Ada.Position);

            FurnitureItem Upstairs = BoxPlace.Place(null, new Vector3(Center(Map, 3, 1).X, Center(Map, 3, 1).Y, BuildFloor.Surface(1)), Map.TileSize.X, BoxKind.SmallCube, null, false);
            Assert.AreEqual(1, Upstairs.Floor);
            Assert.AreEqual(BuildFloor.Surface(1), Upstairs.Position.Z, 0.001f);
        }

        [TestMethod]
        public void SheWalksAroundAThinWallInsteadOfIntoIt()
        {
            LifeSimMap Map = NewMap(8, 6);
            Fill(Map, UnitStats.TerrainLandIndex);
            MapInfo Room = new MapInfo(new MapContainer("Room", Map), null, null);
            BuildingItem Wall = Span(new Vector3(0f, 96f, 0f), new Vector3(220f, 96f, 0f), false);
            Map.Buildings.Add(Wall);
            PlayerCharacter Ada = new PlayerCharacter(null, Room, Center(Map, 1, 1));
            Ada.AIControlled = true;
            Vector3 Goal = Center(Map, 1, 4);

            Assert.IsTrue(LifeOrder.Walk(Ada, Goal, Map));
            FollowPath Path = Ada.TopObjective().ListRemainingStep[0] as FollowPath;
            Assert.IsNotNull(Path);
            for (int s = 0; s < Path.Segments.Count; s++)
            {
                List<Vector3> Line = Path.Segments[s].Polyline;
                for (int p = 1; p < Line.Count; p++)
                {
                    Assert.IsFalse(Wall.HitsMove(Line[p - 1], Line[p]), "Path cuts the wall from " + Line[p - 1] + " to " + Line[p]);
                }
            }

            GameTime Step = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.05f));
            for (int i = 0; i < 900 && Ada.Position.Y < Goal.Y - 8f; i++)
            {
                Ada.Update(Step);
                Assert.IsFalse(Wall.FootprintContains(Ada.Position.X, Ada.Position.Y), "Walked into the wall at " + Ada.Position);
            }

            Assert.IsTrue(Ada.Position.Y > 120f, "Never got past the wall, at " + Ada.Position);
        }

        [TestMethod]
        public void AWallPlacedOnHerRouteMakesHerGoAround()
        {
            LifeSimMap Map = NewMap(8, 6);
            Fill(Map, UnitStats.TerrainLandIndex);
            MapInfo Room = new MapInfo(new MapContainer("Room", Map), null, null);
            PlayerCharacter Ada = new PlayerCharacter(null, Room, Center(Map, 1, 1));
            Ada.AIControlled = true;
            Vector3 Goal = Center(Map, 1, 4);
            Assert.IsTrue(LifeOrder.Walk(Ada, Goal, Map));
            FollowPath Started = Ada.TopObjective().ListRemainingStep[0] as FollowPath;

            GameTime Step = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.05f));
            for (int i = 0; i < 20; i++)
            {
                Ada.Update(Step);
            }

            Assert.IsTrue(Ada.Position.Y > Started.Segments[0].Polyline[0].Y, "She never left the start");
            float Ahead = Ada.Position.Y + 24f;
            BuildingItem Wall = Span(new Vector3(0f, Ahead, 0f), new Vector3(180f, Ahead, 0f), false);
            Map.Buildings.Add(Wall);
            Ada.Update(Step);
            FollowPath Replanned = Ada.TopObjective().ListRemainingStep[0] as FollowPath;
            Assert.IsNotNull(Replanned);
            Assert.AreNotSame(Started, Replanned);
            for (int s = 0; s < Replanned.Segments.Count; s++)
            {
                List<Vector3> Line = Replanned.Segments[s].Polyline;
                for (int p = 1; p < Line.Count; p++)
                {
                    Assert.IsFalse(Wall.HitsMove(Line[p - 1], Line[p]), "Replanned path still cuts the wall");
                }
            }

            for (int i = 0; i < 900 && Ada.Position.Y < Goal.Y - 8f; i++)
            {
                Ada.Update(Step);
                Assert.IsFalse(Wall.FootprintContains(Ada.Position.X, Ada.Position.Y), "Walked into the new wall at " + Ada.Position);
            }

            Assert.IsTrue(Ada.Position.Y > 120f, "Stopped at the new wall, at " + Ada.Position);
        }

        [TestMethod]
        public void HittingAWallMakesHerCalculateANewPath()
        {
            LifeSimMap Map = NewMap(8, 6);
            Fill(Map, UnitStats.TerrainLandIndex);
            MapInfo Room = new MapInfo(new MapContainer("Room", Map), null, null);
            PlayerCharacter Ada = new PlayerCharacter(null, Room, Center(Map, 1, 1));
            Ada.AIControlled = true;
            Assert.IsTrue(LifeOrder.Walk(Ada, Center(Map, 1, 4), Map));
            FollowPath Started = Ada.TopObjective().ListRemainingStep[0] as FollowPath;
            BuildingItem Wall = Span(new Vector3(70f, 32f, 0f), new Vector3(70f, 80f, 0f), false);
            Map.Buildings.Add(Wall);

            Vector3 Before = Ada.Position;
            Ada.StepBody(new Vector3(40f, 0f, 0f));
            Assert.AreEqual(Before, Ada.Position, "The shove went through the wall");
            Ada.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.05f)));

            FollowPath Replanned = Ada.TopObjective().ListRemainingStep[0] as FollowPath;
            Assert.IsNotNull(Replanned);
            Assert.AreNotSame(Started, Replanned);
            Assert.IsTrue(Replanned.Segments.Count > 0);
            Assert.IsFalse(Wall.FootprintContains(Ada.Position.X, Ada.Position.Y));
        }

        [TestMethod]
        public void ThinWallKeepsTheDoorwayAndTheFloorBesideIt()
        {
            LifeSimMap Map = NewMap(6, 4);
            Fill(Map, UnitStats.TerrainLandIndex);
            MapInfo Room = new MapInfo(new MapContainer("Room", Map), null, null);
            Map.Buildings.Add(Span(new Vector3(0f, 64f, 0f), new Vector3(64f, 64f, 0f), false));
            Map.Buildings.Add(Span(new Vector3(96f, 64f, 0f), new Vector3(192f, 64f, 0f), false));
            Map.Buildings.Add(Span(new Vector3(64f, 64f, 0f), new Vector3(96f, 64f, 0f), true));
            PlayerCharacter Ada = new PlayerCharacter(null, Room, Center(Map, 1, 0));

            FollowPath Through = Ada.PlanPath(Map, Center(Map, 2, 2), false);
            Assert.IsTrue(Through.Segments.Count > 0, "No route through the doorway");
            bool Crossed = false;
            float Furthest = 0f;
            for (int s = 0; s < Through.Segments.Count; s++)
            {
                for (int p = 0; p < Through.Segments[s].Polyline.Count; p++)
                {
                    Vector3 Point = Through.Segments[s].Polyline[p];
                    if (Point.X > Furthest)
                    {
                        Furthest = Point.X;
                    }

                    if (Point.Y > 70f && Point.X > 48f && Point.X < 112f)
                    {
                        Crossed = true;
                    }
                }
            }

            Assert.IsTrue(Crossed, "Route missed the doorway, furthest X " + Furthest);

            FollowPath Beside = Ada.PlanPath(Map, new Vector3(48f, 54f, 0f), false);
            Assert.IsTrue(Beside.Segments.Count > 0);
            Vector3 End = Beside.Segments[Beside.Segments.Count - 1].Polyline[Beside.Segments[Beside.Segments.Count - 1].Polyline.Count - 1];
            Assert.IsTrue(End.Y > 48f, "Stopped a tile short of the wall, at " + End);
        }

        [TestMethod]
        public void AFurnishedRoomStillFindsAPath()
        {
            LifeSimMap Map = NewMap(12, 10);
            Fill(Map, UnitStats.TerrainLandIndex);
            MapInfo Room = new MapInfo(new MapContainer("House", Map), null, null);
            Map.Buildings.Add(Span(new Vector3(32f, 128f, 0f), new Vector3(160f, 128f, 0f), false));
            Map.Buildings.Add(Span(new Vector3(192f, 128f, 0f), new Vector3(320f, 128f, 0f), false));
            Map.Buildings.Add(Span(new Vector3(160f, 128f, 0f), new Vector3(192f, 128f, 0f), true));
            Map.Buildings.Add(Span(new Vector3(32f, 32f, 0f), new Vector3(32f, 256f, 0f), false));
            for (int i = 0; i < 6; i++)
            {
                FurnitureItem Table = new FurnitureItem();
                Table.Width = 24f;
                Table.Length = 24f;
                Table.Height = 16f;
                Table.Position = new Vector3(64f + i * 32f, 64f, 0f);
                Map.Furniture.Add(Table);
            }

            PlayerCharacter Ada = new PlayerCharacter(null, Room, Center(Map, 2, 2));
            FollowPath Across = Ada.PlanPath(Map, Center(Map, 9, 2), false);
            Assert.IsTrue(Across.Segments.Count > 0, "No route across the room");

            FollowPath Outside = Ada.PlanPath(Map, Center(Map, 5, 6), false);
            Assert.IsTrue(Outside.Segments.Count > 0, "No route out the doorway");
            bool Crossed = false;
            for (int s = 0; s < Outside.Segments.Count; s++)
            {
                for (int p = 0; p < Outside.Segments[s].Polyline.Count; p++)
                {
                    Vector3 Point = Outside.Segments[s].Polyline[p];
                    if (Point.Y > 140f && Point.X > 150f && Point.X < 210f)
                    {
                        Crossed = true;
                    }
                }
            }

            Assert.IsTrue(Crossed, "Left the room by missing the doorway");
        }

        static BuildingItem Span(Vector3 A, Vector3 B, bool Door)
        {
            BuildingItem Piece = new BuildingItem();
            Piece.Kind = Door ? BuildingKind.Door : BuildingKind.Wall;
            Piece.Width = Vector3.Distance(A, B);
            Piece.Length = 4f;
            Piece.Height = 96f;
            Piece.Angle = (float)Math.Atan2(B.Y - A.Y, B.X - A.X);
            Piece.Position = (A + B) * 0.5f;
            return Piece;
        }

        [TestMethod]
        public void WalkOrderGoesAroundAWall()
        {
            LifeSimMap Map = NewMap(8, 3);
            MapInfo Room = new MapInfo(new MapContainer("Room", Map), null, null);
            PlayerCharacter Ada = new PlayerCharacter(null, Room, Center(Map, 0, 0));
            Ada.AIControlled = true;
            BuildingItem Wall = new BuildingItem();
            Wall.Kind = BuildingKind.Wall;
            Wall.Position = new Vector3(96f, 48f, 0f);
            Wall.Width = 192f;
            Wall.Length = 8f;
            Wall.Height = 96f;
            Map.Buildings.Add(Wall);
            Vector3 Spot = Center(Map, 0, 2);

            Assert.IsTrue(LifeOrder.Walk(Ada, Spot, Map));

            FollowPath Path = Ada.TopObjective().ListRemainingStep[0] as FollowPath;
            float Furthest = 0f;
            for (int s = 0; s < Path.Segments.Count; s++)
            {
                for (int p = 0; p < Path.Segments[s].Polyline.Count; p++)
                {
                    if (Path.Segments[s].Polyline[p].X > Furthest)
                    {
                        Furthest = Path.Segments[s].Polyline[p].X;
                    }
                }
            }

            Assert.IsTrue(Furthest > 140f, "Path went through the wall, furthest X " + Furthest);
            Vector3 End = Path.Segments[Path.Segments.Count - 1].Polyline[Path.Segments[Path.Segments.Count - 1].Polyline.Count - 1];
            Assert.IsTrue(Vector3.Distance(End, Spot) < Map.TileSize.X);
        }

        [TestMethod]
        public void DefaultFloorLetsTheBodyWalkAndAPlacedWallStopsIt()
        {
            LifeSimMap Map = NewMap(4, 1);
            MapInfo Room = new MapInfo(new MapContainer("Room", Map), null, null);
            Vector3 Start = Center(Map, 0, 0);
            PlayerCharacter Walker = new PlayerCharacter(null, Room, Start);
            Walker.Move(new Vector3(0f, 1f, 0f), 1, 0.5);
            Assert.IsTrue(Walker.Position.Y > Start.Y + 1f, "Floor held the body at " + Walker.Position);

            PlayerCharacter Blocked = new PlayerCharacter(null, Room, Start);
            BuildingItem Wall = new BuildingItem();
            Wall.Kind = BuildingKind.Wall;
            Wall.Position = new Vector3(Start.X, Start.Y + 4f, Start.Z);
            Wall.Width = 64f;
            Wall.Length = 4f;
            Wall.Height = 96f;
            Room.Buildings.Add(Wall);
            Blocked.Move(new Vector3(0f, 1f, 0f), 1, 0.5);
            Assert.IsTrue(Blocked.Position.Y < Wall.Position.Y, "Walked through the wall, at " + Blocked.Position);
        }

        [TestMethod]
        public void JumpNeedsTheBodyAndAStepStacksOnACube()
        {
            PlayerCharacter Ada = new PlayerCharacter(null, null, Vector3.Zero);
            float Ground = Ada.Position.Z;
            Assert.IsFalse(Ada.TryJump());
            Assert.AreEqual(Ground, Ada.Position.Z, 0.001f);

            Ada.Body.CanJump = true;
            Assert.IsTrue(Ada.TryJump());
            Assert.IsTrue(Ada.Position.Z > Ground);

            List<FurnitureItem> Boxes = new List<FurnitureItem>();
            FurnitureItem Cube = BoxPlace.Place(Boxes, Vector3.Zero, 32f, BoxKind.SmallCube, null, false);
            Boxes.Add(Cube);
            FurnitureItem Step = BoxPlace.Place(Boxes, Vector3.Zero, 32f, BoxKind.Step, null, false);
            Assert.AreEqual(Cube.Height, Step.Position.Z, 0.001f);

            FurnitureItem Rope = BoxPlace.Place(Boxes, new Vector3(64f, 0f, 0f), 32f, BoxKind.Long, "Rope", false);
            Assert.AreEqual(Traversal.Rope, Rope.Link().Kind);
            Assert.IsNull(Rope.LinkFor(new TraversalCapabilities()));
            TraversalCapabilities Climber = new TraversalCapabilities();
            Climber.CanRope = true;
            Assert.IsNotNull(Rope.LinkFor(Climber));
        }

        [TestMethod]
        public void AFightUsesTopDownUntilItPopsAndWallsHidePeople()
        {
            PlayerCharacter Ada = new PlayerCharacter(null, null, new Vector3(16f, 16f, 0f));
            PlaySession Session = new PlaySession();
            Session.TakeCharacter(Ada);
            Session.ToggleView();
            AIObjective Fight = new AIObjective();
            Fight.IsFight = true;
            Ada.ListAIObjective.Add(Fight);

            Session.WatchFight(Ada);
            Assert.AreEqual(CameraStyle.TopDown, Session.View);
            Assert.IsTrue(Ada.TurnBased);

            Ada.ListAIObjective.Remove(Fight);
            Session.WatchFight(Ada);
            Assert.AreEqual(CameraStyle.Orbit, Session.View);
            Assert.IsFalse(Ada.TurnBased);

            PlayerCharacter Hidden = new PlayerCharacter(null, null, new Vector3(16f, 80f, 0f));
            PlayerCharacter Seen = new PlayerCharacter(null, null, new Vector3(16f, 48f, 0f));
            List<Point> Walls = new List<Point> { new Point(0, 2) };
            Assert.IsFalse(BattleSight.CanSee(Ada.Position, Vector3.UnitY, Hidden.Position, Walls, 32f));
            Assert.IsTrue(BattleSight.CanSee(Ada.Position, Vector3.UnitY, Seen.Position, Walls, 32f));
        }

        [TestMethod]
        public void SwitchingCharactersUsesOnlyTheAssignedList()
        {
            PlayerCharacter Ada = new PlayerCharacter(null, null, Vector3.Zero);
            PlayerCharacter Ben = new PlayerCharacter(null, null, Vector3.Zero);
            Ada.Name = "Ada";
            Ben.Name = "Ben";
            AIObjective Guard = new AIObjective();
            Guard.IsStandingOrder = true;
            Guard.ListRemainingStep.Add(new WaitAction(Ada, 1));
            Ada.ListAIObjective.Add(Guard);
            Ada.ListCurrentAIObjectiveStep = Guard.ListRemainingStep;

            PlaySession Session = new PlaySession();
            Session.ToggleAssigned(Ada);
            Session.ToggleAssigned(Ben);
            Session.TakeCharacter(Ada);
            Session.SwitchAssigned(1);

            Assert.IsTrue(Ada.AIControlled);
            Assert.AreSame(Guard, Ada.TopObjective());
            Assert.AreEqual(1, Ada.ListCurrentAIObjectiveStep.Count);
            Assert.IsFalse(Ben.AIControlled);
            Assert.AreSame(Ben, Session.Controlled);

            Session.SelectSlot(9);
            Assert.AreSame(Ben, Session.Controlled);
        }

        [TestMethod]
        public void DeletingABoxAndReadingStrengthLeaveTheBondAlone()
        {
            PlayerCharacter Ada = new PlayerCharacter(null, null, Vector3.Zero);
            PlayerCharacter Ben = new PlayerCharacter(null, null, Vector3.Zero);
            Ben.Name = "Ben";
            Ada.STR = 14;
            CharacterRelationship Bond = Ada.RelationshipWith(Ben);
            Bond.Status = RelationshipStatus.Married;
            Bond.Mood = RelationshipMood.Angry;
            Bond.AddProgress(40f);

            List<string> Lines = CharacterPage.Lines(Ada);
            Assert.IsTrue(Lines.Contains("STR: 14"));
            Assert.AreEqual(RelationshipStatus.Married, Bond.Status);
            Assert.AreEqual(40f, Bond.Progress, 0.001f);

            List<FurnitureItem> Boxes = new List<FurnitureItem>();
            FurnitureItem Bed = BoxPlace.Place(Boxes, Vector3.Zero, 32f, BoxKind.SmallCube, "Bed", false);
            Boxes.Add(Bed);
            Assert.AreEqual("Bed", Bed.Interaction);
            Boxes.Remove(Bed);
            Assert.AreEqual(0, Boxes.Count);
        }

        [TestMethod]
        public void TheFlyingCursorLandsOnTheGroundItLooksAt()
        {
            Vector3 Ground = CursorGround.From(new Vector3(48f, 0f, 96f));
            Assert.AreEqual(48f, Ground.X, 0.001f);
            Assert.AreEqual(96f, Ground.Y, 0.001f);
            Assert.AreEqual(0f, Ground.Z, 0.001f);

            FurnitureItem Cube = BoxPlace.Place(null, Ground, 32f, BoxKind.SmallCube, null, false);
            Assert.AreEqual(96f, Cube.Position.Y, 0.001f);

            BuildingItem Wall = BoxPlace.Wall(32f, 1, 2);
            Assert.AreEqual(BuildingKind.Wall, Wall.Kind);
            Assert.AreEqual(48f, Wall.Position.X, 0.001f);
            Assert.AreEqual(80f, Wall.Position.Y, 0.001f);
            Assert.IsTrue(Wall.Height > 32f);
        }

        [TestMethod]
        public void AWallCellIsLeftOutOfTheBake()
        {
            LifeSimMap Map = NewMap(3, 1);
            Fill(Map, UnitStats.TerrainLandIndex);
            BoxPlace.PlaceWall(Map, 1, 0);

            NavSourceGeometry Source = GridNavGeometry.Build(Map);
            for (int i = 0; i < Source.Triangles.Count; i++)
            {
                Vector3 Centroid = (Source.Triangles[i].A + Source.Triangles[i].B + Source.Triangles[i].C) / 3f;
                float X0 = Map.TileSize.X;
                Assert.IsFalse(Centroid.X >= X0 && Centroid.X < X0 + Map.TileSize.X && Centroid.Y >= 0f && Centroid.Y < Map.TileSize.Y);
            }
        }

        [TestMethod]
        public void OneCharacterStepsAsideAtADoor()
        {
            Vector3 Door = new Vector3(10f, 0f, 0f);
            Vector3 Beside = new Vector3(40f, 0f, 0f);
            PlayerCharacter Standing = new PlayerCharacter(null, null, Door);
            PlayerCharacter Crossing = new PlayerCharacter(null, null, new Vector3(0f, 0f, 0f));

            AIObjective Stay = new AIObjective();
            Stay.ListRemainingStep.Add(new WaitAction(Standing, 1));
            Standing.ListAIObjective.Add(Stay);

            LifeSimMap Map = NewMap(2, 1);
            Fill(Map, UnitStats.TerrainLandIndex);
            List<PathSegment> Segments = LocalPathQuery.Find(Map, Crossing.Position, new Vector3(20f, 0f, 0f), Crossing.Body, RouteStyle.Direct);
            AIObjective Trip = new AIObjective();
            Trip.ListRemainingStep.Add(new FollowPath(Crossing, Segments));
            Crossing.ListAIObjective.Add(Trip);

            DoorYield.Settle(Standing, Crossing, Door, Beside);

            Assert.AreEqual(Beside.X, Standing.Position.X, 0.001f);
            Assert.AreSame(Stay, Standing.TopObjective());
            Assert.IsInstanceOfType(Standing.TopObjective().ListRemainingStep[0], typeof(StepAsideAction));
            Assert.IsInstanceOfType(Crossing.TopObjective().ListRemainingStep[0], typeof(FollowPath));
        }

        [TestMethod]
        public void BodyStopsAtAWallAndWalksThroughADoor()
        {
            PhysicsManager Physics = new PhysicsManager();
            Physics.Initialize();

            FurnitureItem Ground = new FurnitureItem();
            Ground.Position = new Vector3(0f, 40f, -4f);
            Ground.Width = 500f;
            Ground.Length = 500f;
            Ground.Height = 4f;

            BuildingItem Wall = new BuildingItem();
            Wall.Kind = BuildingKind.Wall;
            Wall.Position = new Vector3(0f, 80f, 0f);
            Wall.Width = 64f;
            Wall.Length = 4f;
            Wall.Height = 96f;

            BuildingItem Door = new BuildingItem();
            Door.Kind = BuildingKind.Door;
            Door.Position = new Vector3(120f, 80f, 0f);
            Door.Width = 32f;
            Door.Length = 8f;
            Door.Height = 96f;

            FurnitureItem Table = new FurnitureItem();
            Table.Position = new Vector3(-120f, 80f, 0f);
            Table.Width = 32f;
            Table.Length = 32f;
            Table.Height = 24f;

            Physics.SyncSolids(new List<FurnitureItem> { Ground, Table }, new List<BuildingItem> { Wall, Door });

            PlayerCharacter IntoWall = new PlayerCharacter(null, null, new Vector3(0f, 0f, 0f));
            Physics.Adopt(IntoWall);
            for (int i = 0; i < 240; i++)
            {
                Physics.Steer(IntoWall, new Vector3(0f, 1f, 0f), 40f);
                Physics.Update();
            }

            Assert.IsTrue(IntoWall.Position.Y > 20f, "Never reached the wall, at " + IntoWall.Position);
            Assert.IsTrue(IntoWall.Position.Y < 76f, "Walked through the wall, at " + IntoWall.Position);

            PlayerCharacter IntoTable = new PlayerCharacter(null, null, new Vector3(-120f, 0f, 0f));
            Physics.Adopt(IntoTable);
            for (int i = 0; i < 240; i++)
            {
                Physics.Steer(IntoTable, new Vector3(0f, 1f, 0f), 40f);
                Physics.Update();
            }

            Assert.IsTrue(IntoTable.Position.Y > 20f, "Never reached the table, at " + IntoTable.Position);
            Assert.IsTrue(IntoTable.Position.Y < 72f, "Walked through the table, at " + IntoTable.Position);

            PlayerCharacter ThroughDoor = new PlayerCharacter(null, null, new Vector3(120f, 0f, 0f));
            Physics.Adopt(ThroughDoor);
            for (int i = 0; i < 240; i++)
            {
                Physics.Steer(ThroughDoor, new Vector3(0f, 1f, 0f), 40f);
                Physics.Update();
            }

            Assert.IsTrue(ThroughDoor.Position.Y > 100f, "Door blocked the body, at " + ThroughDoor.Position);
        }

        [TestMethod]
        public void DefaultTilesDoNotBoxTheBody()
        {
            LifeSimMap Map = NewMap(5, 5);
            PhysicsManager Physics = new PhysicsManager();
            Physics.Initialize();

            FurnitureItem Ground = new FurnitureItem();
            Ground.Position = new Vector3(80f, 80f, -4f);
            Ground.Width = 400f;
            Ground.Length = 400f;
            Ground.Height = 4f;
            Physics.SyncSolids(new List<FurnitureItem> { Ground }, null);

            Vector3 Start = Center(Map, 2, 2);
            PlayerCharacter Walker = new PlayerCharacter(null, null, Start);
            Physics.Adopt(Walker);
            for (int i = 0; i < 180; i++)
            {
                Physics.Steer(Walker, new Vector3(1f, 0f, 0f), 40f);
                Physics.Update();
            }

            Assert.IsTrue(Walker.Position.X > Start.X + Map.TileSize.X, "Stuck in the spawn tile, at " + Walker.Position);
        }

        [TestMethod]
        public void LifeCursorUsesTheBuildTileCursor()
        {
            LifeSimMap Map = NewMap(4, 4);
            Vector3 Cursor = MapCursor.Slide(new Vector3(10f, 0f, 12f), 0.1f, false, true, false, true);
            Assert.AreEqual(26f, Cursor.X, 0.01f);
            Assert.AreEqual(28f, Cursor.Z, 0.01f);

            Vector3 Ground = MapCursor.Clamp(CursorGround.From(Cursor), Map, MapCursor.TileOf(Map));
            MapCursor.ShowTile(Map, Ground, MapCursor.TileOf(Map));
            Assert.AreEqual(10f, Map.CursorPosition.X, 0.01f);
            Assert.AreEqual(12f, Map.CursorPosition.Y, 0.01f);
            Assert.AreEqual(Map.CursorPosition, Map.CursorPositionVisible);

            Vector3 Past = MapCursor.Clamp(new Vector3(-40f, 500f, 0f), Map, 32f);
            Assert.AreEqual(0f, Past.X, 0.01f);
            Assert.IsTrue(Past.Y < 4 * 32f);
        }

        static LifeSimMap NewMap(int Width, int Height)
        {
            LifeSimMap Map = new LifeSimMap();
            Map.MapSize = new Point(Width, Height);
            Map.LayerManager.ListLayer.Add(new MapLayer(Map, 0));
            return Map;
        }

        static void Fill(LifeSimMap Map, byte TerrainIndex)
        {
            for (int Y = 0; Y < Map.MapSize.Y; Y++)
            {
                for (int X = 0; X < Map.MapSize.X; X++)
                {
                    Terrain Tile = Map.LayerManager.ListLayer[0].ArrayTerrain[X, Y];
                    Tile.TerrainTypeIndex = TerrainIndex;
                    Tile.Height = 0f;
                    Tile.WorldPosition.X = X * Map.TileSize.X;
                    Tile.WorldPosition.Y = Y * Map.TileSize.Y;
                    Tile.WorldPosition.Z = 0f;
                }
            }
        }

        static Vector3 Center(LifeSimMap Map, int X, int Y)
        {
            return GridNavGeometry.CellCenter(Map, Map.LayerManager.ListLayer[0].ArrayTerrain[X, Y]);
        }
    }
}
