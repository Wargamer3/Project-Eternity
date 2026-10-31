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
    public class LifeSimLifePlanTests
    {
        [TestMethod]
        public void JerkyInTheBagIsEatenWithoutATrip()
        {
            PlayerCharacter Character = new PlayerCharacter(null, null, Vector3.Zero);
            GiveJerky(Character);
            EatJerkyAction Eat = new EatJerkyAction();
            Eat.Init(Character.SharedMapContex);
            Eat.UpdatePrecondition(string.Empty, null);

            Assert.AreEqual(40, Eat.AIWeight);
            List<AutomatedAction> Steps = Eat.GetAIExecutionPlan(null);
            Assert.AreEqual(1, Steps.Count);
            Assert.AreEqual("Eat", Steps[0].ActionName);

            Character.CurrentPose = Pose.Sit;
            List<AutomatedAction> Seated = Eat.GetAIExecutionPlan(null);
            Assert.AreEqual("Stand Up", Seated[0].ActionName);
            Assert.AreEqual("Eat", Seated[1].ActionName);

            Assert.IsFalse(Seated[1].Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1))));
            Assert.IsFalse(Character.IsCarrying("Jerky"));
        }

        [TestMethod]
        public void JerkyInADrawerIsFetchedOnlyBySomeoneWhoHasSeenIt()
        {
            LifeSimMap Map = NewMap(1, 4);
            Fill(Map, UnitStats.TerrainLandIndex);
            MapInfo Room = new MapInfo(new MapContainer("Room", Map), null, null);
            Vector3 Start = Center(Map, 0, 0);
            Vector3 DrawerAt = Center(Map, 0, 3);
            PlayerCharacter Seen = new PlayerCharacter(null, Room, Start);
            BelievedObject Drawer = new BelievedObject();
            Drawer.Name = "Drawer";
            Drawer.Tag = "Jerky";
            Drawer.Chunk = Room;
            Drawer.LocalPosition = DrawerAt;
            Drawer.Kind = WorldMarkerKind.Object;
            Seen.Belief.ListObject.Add(Drawer);
            WorldMarker Marker = new WorldMarker();
            Marker.Name = "Drawer";
            Marker.Tag = "Jerky";
            Marker.LocalPosition = DrawerAt;
            Room.ListMarker.Add(Marker);

            EatJerkyAction Fetch = new EatJerkyAction();
            Fetch.Init(Seen.SharedMapContex);
            Fetch.UpdatePrecondition(string.Empty, null);
            Assert.AreEqual(40, Fetch.AIWeight);

            WalkToPositionAction Walk = new WalkToPositionAction();
            Walk.Init(Seen.SharedMapContex);
            Seen.AddAction(Walk);
            List<AutomatedAction> Steps = Fetch.GetAIExecutionPlan(null);
            Assert.AreEqual("Follow Path", Steps[0].ActionName);
            Assert.AreEqual("Open", Steps[1].ActionName);
            Assert.AreEqual("Take Jerky", Steps[2].ActionName);
            Assert.AreEqual("Eat", Steps[3].ActionName);
            Assert.AreEqual(1, CountName(Steps, "Follow Path"));

            GameTime Second = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            Assert.IsFalse(Steps[1].Update(Second));
            Assert.IsFalse(Steps[2].Update(Second));
            Assert.IsNull(Seen.Belief.FindTagged("Jerky"));
            Assert.AreEqual(string.Empty, Marker.Tag);
            Assert.IsFalse(Steps[3].Update(Second));
            Assert.IsFalse(Seen.IsCarrying("Jerky"));

            PlayerCharacter Unseen = new PlayerCharacter(null, Room, Start);
            EatJerkyAction Missed = new EatJerkyAction();
            Missed.Init(Unseen.SharedMapContex);
            Missed.UpdatePrecondition(string.Empty, null);
            Assert.AreEqual(0, Missed.AIWeight);
            Assert.AreEqual(0, Missed.GetAIExecutionPlan(null).Count);
        }

        [TestMethod]
        public void RestaurantWritesItsOwnStepsAndAFightReplansThere()
        {
            LifeSimMap Map = NewMap(1, 4);
            Fill(Map, UnitStats.TerrainLandIndex);
            MapInfo Room = new MapInfo(new MapContainer("Room", Map), null, null);
            Vector3 Start = Center(Map, 0, 0);
            Vector3 Table = Center(Map, 0, 3);
            PlayerCharacter Character = new PlayerCharacter(null, Room, Start);
            BelievedObject Spot = new BelievedObject();
            Spot.Name = "Cafe";
            Spot.Tag = "Restaurant";
            Spot.Chunk = Room;
            Spot.LocalPosition = Table;
            Character.Belief.ListObject.Add(Spot);
            Character.Belief.LearnArea(Room);

            GoToRestaurantAction Restaurant = new GoToRestaurantAction();
            Restaurant.Init(Character.SharedMapContex);
            WalkToPositionAction Walk = new WalkToPositionAction();
            Walk.Init(Character.SharedMapContex);
            RunToPositionAction Run = new RunToPositionAction();
            Run.Init(Character.SharedMapContex);
            Character.AddAction(Walk);
            Character.AddAction(Run);
            Character.AddAction(Restaurant);

            List<AutomatedAction> Steps = Restaurant.GetAIExecutionPlan(null);
            Assert.AreEqual("Follow Path", Steps[0].ActionName);
            Assert.AreEqual("Eat", Steps[1].ActionName);
            Assert.AreEqual(1, CountName(Steps, "Follow Path"));

            AIObjective Bottom = new AIObjective();
            Bottom.GoalName = Restaurant.AIGoal;
            Bottom.ChosenAction = Restaurant;
            Character.CommitBottom(Bottom);
            Vector3 FirstStart = ((FollowPath)Character.TopObjective().ListRemainingStep[0]).Segments[0].Polyline[0];

            EatJerkyAction OtherFood = new EatJerkyAction();
            OtherFood.Init(Character.SharedMapContex);
            AIObjective Fight = new AIObjective();
            Fight.GoalName = "Fight";
            Fight.ChosenAction = OtherFood;
            Fight.IsFight = true;
            Character.PushShortTerm(Fight);

            Vector3 Moved = Center(Map, 0, 2);
            Character.SetPosition(Moved);
            Character.PopTopObjective();

            Assert.AreEqual(1, Character.ListAIObjective.Count);
            Assert.AreSame(Restaurant, Character.TopObjective().ChosenAction);
            FollowPath Replanned = (FollowPath)Character.TopObjective().ListRemainingStep[0];
            Vector3 Restarted = Replanned.Segments[0].Polyline[0];
            Assert.IsTrue(Vector3.Distance(Restarted, Moved) < Vector3.Distance(FirstStart, Moved));
        }

        [TestMethod]
        public void DoorPlanWalksThereThenLocksUnlocksOrOpens()
        {
            LifeSimMap Map = NewMap(1, 4);
            Fill(Map, UnitStats.TerrainLandIndex);
            MapInfo Room = new MapInfo(new MapContainer("Room", Map), null, null);
            PlayerCharacter Character = new PlayerCharacter(null, Room, Center(Map, 0, 0));
            BelievedObject Door = new BelievedObject();
            Door.Name = "Front Door";
            Door.Tag = "Door";
            Door.Kind = WorldMarkerKind.Door;
            Door.Chunk = Room;
            Door.LocalPosition = Center(Map, 0, 3);
            Door.State = PassageState.Closed;
            Character.Belief.ListObject.Add(Door);
            WorldMarker Marker = new WorldMarker();
            Marker.Name = "Front Door";
            Marker.State = PassageState.Closed;
            Room.ListMarker.Add(Marker);

            WalkToPositionAction Walk = new WalkToPositionAction();
            Walk.Init(Character.SharedMapContex);
            Character.AddAction(Walk);

            GoThroughDoorAction Open = new GoThroughDoorAction();
            Open.Init(Character.SharedMapContex);
            Open.TargetName = "Front Door";
            List<AutomatedAction> Opened = Open.GetAIExecutionPlan(null);
            Assert.AreEqual("Open Door", Opened[Opened.Count - 1].ActionName);

            LockDoorAction Lock = new LockDoorAction();
            Lock.Init(Character.SharedMapContex);
            Lock.TargetName = "Front Door";
            List<AutomatedAction> Locked = Lock.GetAIExecutionPlan(null);
            Assert.AreEqual("Follow Path", Locked[0].ActionName);
            Assert.AreEqual("Lock Door", Locked[1].ActionName);
            Assert.AreEqual(1, CountName(Locked, "Follow Path"));
            Assert.IsFalse(Locked[1].Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1))));
            Assert.AreEqual(PassageState.Locked, Door.State);
            Assert.IsFalse(Door.IsOpen);

            UnlockDoorAction Unlock = new UnlockDoorAction();
            Unlock.Init(Character.SharedMapContex);
            Unlock.TargetName = "Front Door";
            Assert.AreEqual(0, Unlock.GetAIExecutionPlan(null).Count);
        }

        [TestMethod]
        public void AClosedHopWaitsAndAnUnloadedChunkIsNotPathed()
        {
            MapInfo House = new MapInfo(new MapContainer("House", null), null, null);
            MapInfo Restaurant = new MapInfo(new MapContainer("Restaurant", null), null, null);
            House.AddNeighbour(Restaurant);
            Restaurant.AddNeighbour(House);
            ChunkHop Gate = new ChunkHop();
            Gate.Name = "Gate";
            Gate.FromChunk = House;
            Gate.ToChunk = Restaurant;
            Gate.FromPosition = new Vector3(4, 0, 0);
            Gate.ToPosition = new Vector3(8, 0, 0);
            Gate.IsOpen = true;
            House.ListLink.Add(Gate);

            PlayerCharacter Character = new PlayerCharacter(null, House, Vector3.Zero);
            Character.Belief.LearnArea(House);
            Character.Belief.LearnArea(Restaurant);
            Character.Belief.LearnLink(Gate);

            GoToRestaurantAction Meal = new GoToRestaurantAction();
            Meal.Init(Character.SharedMapContex);
            List<AutomatedAction> Planned = Meal.GetAIExecutionPlan(null);
            Assert.IsTrue(HasName(Planned, "Cross Portal"));
            Assert.IsFalse(HasName(Planned, "Follow Path"));
            Assert.IsNull(Restaurant.MapMapContainer.ActiveMap);

            AIObjective Bottom = new AIObjective();
            Bottom.GoalName = Meal.AIGoal;
            Bottom.ChosenAction = Meal;
            Character.CommitBottom(Bottom);
            Character.Belief.SetLinkOpen(House, Restaurant, false);
            Character.AccountForWhatWasSeen();

            Assert.AreEqual(1, Character.ListAIObjective.Count);
            Assert.AreSame(Meal, Character.TopObjective().ChosenAction);
            Assert.AreEqual("Wait", Character.TopObjective().ListRemainingStep[0].ActionName);
            Assert.IsFalse(HasName(Character.TopObjective().ListRemainingStep, "Eat"));
        }

        [TestMethod]
        public void ABlockerWaitsWithoutPoppingOrRebaking()
        {
            LifeSimMap Map = NewMap(1, 4);
            Fill(Map, UnitStats.TerrainLandIndex);
            MapInfo Room = new MapInfo(new MapContainer("Room", Map), null, null);
            Vector3 Start = Center(Map, 0, 0);
            Vector3 Table = Center(Map, 0, 3);
            PlayerCharacter Character = new PlayerCharacter(null, Room, Start);
            BelievedObject Spot = new BelievedObject();
            Spot.Name = "Cafe";
            Spot.Tag = "Restaurant";
            Spot.Chunk = Room;
            Spot.LocalPosition = Table;
            Character.Belief.ListObject.Add(Spot);

            GoToRestaurantAction Restaurant = new GoToRestaurantAction();
            Restaurant.Init(Character.SharedMapContex);
            AIObjective Bottom = new AIObjective();
            Bottom.GoalName = Restaurant.AIGoal;
            Bottom.ChosenAction = Restaurant;
            Character.CommitBottom(Bottom);

            List<PathSegment> Before = LocalPathQuery.Find(Map, Start, Table, Character.Body, RouteStyle.Direct);
            BelievedObject Person = new BelievedObject();
            Person.Name = "Someone";
            Person.Kind = WorldMarkerKind.Blocker;
            Person.Chunk = Room;
            Person.LocalPosition = Table;
            Character.Belief.ListObject.Add(Person);
            Character.AccountForWhatWasSeen();

            Assert.AreEqual(1, Character.ListAIObjective.Count);
            Assert.AreSame(Restaurant, Character.TopObjective().ChosenAction);
            Assert.AreEqual("Wait", Character.TopObjective().ListRemainingStep[0].ActionName);
            List<PathSegment> After = LocalPathQuery.Find(Map, Start, Table, Character.Body, RouteStyle.Direct);
            Assert.AreEqual(Before.Count, After.Count);
            Assert.IsTrue(After.Count > 0);
        }

        [TestMethod]
        public void TheNextChunkWakesOnlyWhenThePortalIsClose()
        {
            MapInfo Home = new MapInfo(new MapContainer("Home", null), null, null);
            MapInfo Shop = new MapInfo(new MapContainer("Shop", null), null, null);
            MapInfo CameraRoom = new MapInfo(new MapContainer("Camera", null), null, null);
            ChunkHop Gate = new ChunkHop();
            Gate.FromChunk = Home;
            Gate.ToChunk = Shop;
            Gate.FromPosition = Vector3.Zero;
            Gate.ToPosition = new Vector3(10, 0, 0);
            Gate.IsOpen = true;

            PlayerCharacter Character = new PlayerCharacter(null, Home, new Vector3(0, 250, 0));
            Character.ListPlannedHop.Add(Gate);
            List<MapInfo> Cameras = new List<MapInfo> { CameraRoom };
            List<PlayerCharacter> People = new List<PlayerCharacter> { Character };

            HashSet<MapInfo> Far = ChunkStreaming.Needed(Cameras, People, NavMapGameManager.MaximumMapLoadDistance);
            Assert.IsTrue(Far.Contains(Home));
            Assert.IsTrue(Far.Contains(CameraRoom));
            Assert.IsFalse(Far.Contains(Shop));

            Character.SetPosition(new Vector3(0, 40, 0));
            HashSet<MapInfo> Near = ChunkStreaming.Needed(Cameras, People, NavMapGameManager.MaximumMapLoadDistance);
            Assert.IsTrue(Near.Contains(Shop));
            Assert.IsTrue(Near.Contains(CameraRoom));
        }

        [TestMethod]
        public void PerceptionCopiesATraversalLinkInsideTheRadius()
        {
            MapInfo Room = new MapInfo(new MapContainer("Room", null), null, null);
            MapInfo Hall = new MapInfo(new MapContainer("Hall", null), null, null);
            ChunkHop Near = new ChunkHop();
            Near.FromChunk = Room;
            Near.ToChunk = Hall;
            Near.IsOpen = true;
            ChunkHop Far = new ChunkHop();
            Far.FromChunk = Room;
            Far.ToChunk = new MapInfo(new MapContainer("Yard", null), null, null);
            Far.IsOpen = true;

            WorldMarker Ladder = new WorldMarker();
            Ladder.Name = "Ladder";
            Ladder.Tag = "Ladder";
            Ladder.Kind = WorldMarkerKind.Object;
            Ladder.LocalPosition = new Vector3(5, 0, 0);
            Ladder.Link = Near;
            Ladder.IsOpen = true;
            WorldMarker Rope = new WorldMarker();
            Rope.Name = "Rope";
            Rope.Tag = "Rope";
            Rope.LocalPosition = new Vector3(80, 0, 0);
            Rope.Link = Far;
            Rope.IsOpen = true;
            Room.ListMarker.Add(Ladder);
            Room.ListMarker.Add(Rope);

            PlayerCharacter Character = new PlayerCharacter(null, Room, Vector3.Zero);
            Character.PerceptionRadius = 20;
            Character.Perceive();

            Assert.IsNotNull(Character.Belief.OpenLink(Room, Hall));
            Assert.IsNull(Character.Belief.OpenLink(Far.FromChunk, Far.ToChunk));
            Assert.IsNotNull(Character.Belief.FindObject("Ladder"));
            Assert.IsNull(Character.Belief.FindObject("Rope"));
        }

        static void GiveJerky(PlayerCharacter Character)
        {
            Item Food = new Item();
            Food.Name = "Jerky";
            Food.ArrayTags = new string[] { "Jerky" };
            Character.AddItem(new ItemInfo(Food, 1));
        }

        static int CountName(List<AutomatedAction> Steps, string Name)
        {
            int Count = 0;
            for (int i = 0; i < Steps.Count; i++)
            {
                if (Steps[i].ActionName == Name)
                {
                    Count++;
                }
            }

            return Count;
        }

        static bool HasName(List<AutomatedAction> Steps, string Name)
        {
            return CountName(Steps, Name) > 0;
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
                    Tile.Height = 0;
                    Tile.WorldPosition.Z = 0;
                    Tile.MovementCost = 1;
                }
            }
        }

        static Vector3 Center(LifeSimMap Map, int X, int Y)
        {
            return GridNavGeometry.CellCenter(Map, Map.LayerManager.ListLayer[0].ArrayTerrain[X, Y]);
        }
    }
}
