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
    public class LifeSimDoorTests
    {
        [TestMethod]
        public void ADoorAndAWindowShareClosedOpenLockedAndBroken()
        {
            Door Gate = Door.CreateDefaultDoor(Vector3.Zero);
            Assert.AreEqual(PassageState.Closed, Gate.State);
            Gate.State = PassageState.Locked;
            Assert.IsTrue(Gate.IsClosed);
            Assert.IsTrue(Gate.IsLocked);
            Gate.State = PassageState.Open;
            Assert.IsFalse(Gate.IsClosed);
            Assert.IsFalse(Gate.IsLocked);
            Gate.State = PassageState.Broken;
            Assert.AreEqual(0, Gate.HP);
            Assert.IsTrue(Passage.LetsPeopleThrough(Gate.State));

            Window Glass = new Window();
            Assert.AreEqual(PassageState.Closed, Glass.State);
            Glass.State = PassageState.Locked;
            Assert.IsTrue(Glass.IsLocked);
            Glass.State = PassageState.Broken;
            Assert.AreEqual(PassageState.Broken, Glass.State);
            Assert.IsTrue(Passage.LetsPeopleThrough(Glass.State));
            Assert.IsFalse(Passage.LetsPeopleThrough(PassageState.Closed));
            Assert.IsFalse(Passage.LetsPeopleThrough(PassageState.Locked));
        }

        [TestMethod]
        public void ALockedDoorOpensOnlyWithAKnownKeyOrAKnownLockpick()
        {
            PlayerCharacter Character = PersonAt(out BelievedObject Door, out WorldMarker Marker, out ChunkHop Gate);
            Door.State = PassageState.Locked;
            Door.KeyName = "Brass Key";
            Marker.State = PassageState.Locked;
            Marker.KeyName = "Brass Key";
            Gate.IsOpen = false;

            UnlockDoorAction Unlock = new UnlockDoorAction();
            Unlock.Init(Character.SharedMapContex);
            Unlock.TargetName = "Front Door";
            Assert.AreEqual(0, Unlock.GetAIExecutionPlan(null).Count);

            GiveItem(Character, "Brass Key");
            Assert.AreEqual(0, Unlock.GetAIExecutionPlan(null).Count);

            Character.AddKnowledge(new KnowledgeInfo("Brass Key", new[] { "Key" }, null));
            List<AutomatedAction> Unlocked = Unlock.GetAIExecutionPlan(null);
            Assert.AreEqual("Unlock Door", Unlocked[Unlocked.Count - 1].ActionName);
            Assert.IsFalse(Unlocked[Unlocked.Count - 1].Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1))));
            Assert.AreEqual(PassageState.Closed, Door.State);
            Assert.IsFalse(Door.IsOpen);
            Assert.IsNull(Character.Belief.OpenLink(Gate.FromChunk, Gate.ToChunk));
            Assert.IsTrue(Character.IsCarrying("Brass Key"));

            PlayerCharacter Picker = PersonAt(out BelievedObject Picked, out WorldMarker PickedMarker, out ChunkHop PickedGate);
            Picked.State = PassageState.Locked;
            PickedMarker.State = PassageState.Locked;
            PickedGate.IsOpen = false;
            GiveItem(Picker, "Lockpick");
            Assert.IsFalse(DoorOpening.KnowsLockpick(Picker));
            Picker.AddKnowledge(new KnowledgeInfo("Lockpick", new[] { "Lockpick" }, null));
            UnlockDoorAction Pick = new UnlockDoorAction();
            Pick.Init(Picker.SharedMapContex);
            Pick.TargetName = "Front Door";
            List<AutomatedAction> PickedOpen = Pick.GetAIExecutionPlan(null);
            Assert.AreEqual("Unlock Door", PickedOpen[PickedOpen.Count - 1].ActionName);
            Assert.IsFalse(PickedOpen[PickedOpen.Count - 1].Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1))));
            Assert.AreEqual(PassageState.Closed, Picked.State);
            Assert.IsFalse(Passage.LetsPeopleThrough(Picked.State));
        }

        [TestMethod]
        public void AClosedDoorOpensSlowlyNormallyQuicklyOrBashed()
        {
            PlayerCharacter Character = PersonAt(out BelievedObject Door, out WorldMarker Marker, out ChunkHop Gate);
            GoThroughDoorAction Open = new GoThroughDoorAction();
            Open.Init(Character.SharedMapContex);
            Open.TargetName = "Front Door";

            Open.Manner = DoorOpenManner.Slowly;
            AutomatedAction Slow = Open.GetAIExecutionPlan(null)[1];
            Assert.AreEqual("Open Door Slowly", Slow.ActionName);
            Assert.IsTrue(Slow.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1))));
            Assert.AreEqual(PassageState.Closed, Door.State);

            Open.Manner = DoorOpenManner.Normally;
            AutomatedAction Normal = Open.GetAIExecutionPlan(null)[1];
            Assert.AreEqual("Open Door", Normal.ActionName);
            Assert.IsFalse(Normal.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1))));
            Assert.AreEqual(PassageState.Open, Door.State);
            Assert.IsTrue(Gate.IsOpen);

            Door.State = PassageState.Closed;
            Marker.State = PassageState.Closed;
            Gate.IsOpen = false;
            Character.Belief.SetLinkOpen(Gate.FromChunk, Gate.ToChunk, false);
            Open.Manner = DoorOpenManner.Quickly;
            AutomatedAction Quick = Open.GetAIExecutionPlan(null)[1];
            Assert.AreEqual("Open Door Quickly", Quick.ActionName);
            Assert.IsFalse(Quick.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1))));

            Door.State = PassageState.Closed;
            Marker.State = PassageState.Closed;
            Open.Manner = DoorOpenManner.Bash;
            AutomatedAction StandingBash = Open.GetAIExecutionPlan(null)[1];
            Assert.AreEqual("Bash Door", StandingBash.ActionName);
            Assert.IsTrue(StandingBash.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.5))));

            Door.State = PassageState.Locked;
            Assert.AreEqual(0, Open.GetAIExecutionPlan(null).Count);
            Assert.IsTrue(DoorOpening.Seconds(DoorOpenManner.Bash, false) > DoorOpening.Seconds(DoorOpenManner.Bash, true));
        }

        [TestMethod]
        public void BashingWhileRunningIsQuickerAndBreakingALockedDoorNeedsNoKey()
        {
            PlayerCharacter Runner = PersonAt(out BelievedObject Door, out WorldMarker Marker, out ChunkHop Gate);
            Personality Hurried = new Personality();
            Hurried.PrefersRun = true;
            Runner.ListPersonality.Add(Hurried);
            Door.State = PassageState.Closed;
            GoThroughDoorAction Open = new GoThroughDoorAction();
            Open.Init(Runner.SharedMapContex);
            Open.TargetName = "Front Door";
            Open.Manner = DoorOpenManner.Bash;
            List<AutomatedAction> Ran = Open.GetAIExecutionPlan(null);
            Assert.AreEqual("Bash Door", Ran[Ran.Count - 1].ActionName);
            Assert.IsFalse(Ran[Ran.Count - 1].Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.5))));
            Assert.AreEqual(PassageState.Open, Door.State);

            PlayerCharacter Breaker = PersonAt(out BelievedObject Locked, out WorldMarker LockedMarker, out ChunkHop LockedGate);
            Locked.State = PassageState.Locked;
            Locked.KeyName = "Brass Key";
            LockedMarker.State = PassageState.Locked;
            LockedGate.IsOpen = false;
            BreakThroughDoorAction Smash = new BreakThroughDoorAction();
            Smash.Init(Breaker.SharedMapContex);
            Smash.TargetName = "Front Door";
            List<AutomatedAction> Broken = Smash.GetAIExecutionPlan(null);
            Assert.AreEqual("Break Door", Broken[Broken.Count - 1].ActionName);
            Assert.IsFalse(Broken[Broken.Count - 1].Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1))));
            Assert.AreEqual(PassageState.Broken, Locked.State);
            Assert.IsTrue(Locked.IsOpen);
            Assert.IsNotNull(Breaker.Belief.OpenLink(LockedGate.FromChunk, LockedGate.ToChunk));
        }

        [TestMethod]
        public void AWindowKeepsItsOwnPassageWhenItIsSeen()
        {
            MapInfo Room = new MapInfo(new MapContainer("Room", null), null, null);
            MapInfo Yard = new MapInfo(new MapContainer("Yard", null), null, null);
            ChunkHop Hop = new ChunkHop();
            Hop.FromChunk = Room;
            Hop.ToChunk = Yard;
            Hop.IsOpen = true;
            WorldMarker Glass = new WorldMarker();
            Glass.Name = "Kitchen Window";
            Glass.Kind = WorldMarkerKind.Window;
            Glass.State = PassageState.Locked;
            Glass.KeyName = "Brass Key";
            Glass.Link = Hop;
            Room.ListMarker.Add(Glass);

            PlayerCharacter Character = new PlayerCharacter(null, Room, Vector3.Zero);
            Character.PerceptionRadius = 20;
            Character.Perceive();

            BelievedObject Seen = Character.Belief.FindObject("Kitchen Window");
            Assert.AreEqual(WorldMarkerKind.Window, Seen.Kind);
            Assert.AreEqual(PassageState.Closed, Seen.State);
            Assert.AreEqual("Brass Key", Seen.KeyName);
            Assert.IsNotNull(Character.Belief.RouteLink(Room, Yard));
            Assert.IsTrue(Hop.IsOpen);

            Glass.State = PassageState.Broken;
            Character.Perceive();
            Assert.AreEqual(PassageState.Broken, Seen.State);
            Assert.IsNotNull(Character.Belief.OpenLink(Room, Yard));
        }

        [TestMethod]
        public void ALockedDoorIsLearnedOnlyWhenTheOpenAttemptFails()
        {
            MapInfo Home = new MapInfo(new MapContainer("Home", null), null, null);
            MapInfo Hall = new MapInfo(new MapContainer("Hall", null), null, null);
            MapInfo Lane = new MapInfo(new MapContainer("Lane", null), null, null);
            MapInfo Shop = new MapInfo(new MapContainer("Shop", null), null, null);
            Home.AddNeighbour(Hall);
            Home.AddNeighbour(Lane);
            Hall.AddNeighbour(Home);
            Hall.AddNeighbour(Shop);
            Lane.AddNeighbour(Home);
            Lane.AddNeighbour(Shop);

            ChunkHop Front = Hop(Home, Hall);
            ChunkHop Back = Hop(Home, Lane);
            ChunkHop HallRoad = Hop(Hall, Shop);
            ChunkHop LaneRoad = Hop(Lane, Shop);
            Home.ListLink.Add(Front);
            Home.ListLink.Add(Back);

            PlayerCharacter Character = new PlayerCharacter(null, Home, Vector3.Zero);
            Character.Belief.LearnArea(Home);
            Character.Belief.LearnArea(Hall);
            Character.Belief.LearnArea(Lane);
            Character.Belief.LearnArea(Shop);
            Character.Belief.LearnLink(Front);
            Character.Belief.LearnLink(Back);
            Character.Belief.LearnLink(HallRoad);
            Character.Belief.LearnLink(LaneRoad);
            BelievedObject FrontDoor = Remember(Character, "Front Door", Home, Home, Hall);
            BelievedObject BackDoor = Remember(Character, "Back Door", Home, Home, Lane);
            FrontDoor.State = PassageState.Closed;
            BackDoor.State = PassageState.Closed;

            Assert.IsNotNull(Character.Belief.RouteLink(Home, Hall));
            Assert.IsNotNull(Character.Belief.RouteLink(Home, Lane));

            WorldMarker Marker = new WorldMarker();
            Marker.Name = "Front Door";
            Marker.Kind = WorldMarkerKind.Door;
            Marker.State = PassageState.Locked;
            Marker.LocalPosition = Vector3.Zero;
            Marker.Link = Front;
            Front.IsOpen = false;
            Home.ListMarker.Add(Marker);

            Character.PerceptionRadius = 30;
            Character.Perceive();
            Assert.AreEqual(PassageState.Closed, FrontDoor.State);
            Assert.IsNotNull(Character.Belief.RouteLink(Home, Hall));
            Assert.AreEqual(PassageState.Locked, Marker.State);

            GoThroughDoorAction Open = new GoThroughDoorAction();
            Open.Init(Character.SharedMapContex);
            Open.TargetName = "Front Door";
            List<AutomatedAction> Attempt = Open.GetAIExecutionPlan(null);
            Assert.AreEqual("Open Door", Attempt[Attempt.Count - 1].ActionName);
            Assert.IsFalse(Attempt[Attempt.Count - 1].Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1))));
            Assert.AreEqual(PassageState.Locked, FrontDoor.State);
            Assert.AreEqual(PassageState.Locked, Marker.State);
            Assert.IsNull(Character.Belief.RouteLink(Home, Hall));

            List<ChunkHop> After = new InterMapMovementAlgorithm().FindHops(Home, "Shop", Character.Belief);
            Assert.IsTrue(After[0].Joins(Home, Lane));
            Assert.IsFalse(After[0].Joins(Home, Hall));
        }

        static PlayerCharacter PersonAt(out BelievedObject Door, out WorldMarker Marker, out ChunkHop Gate)
        {
            LifeSimMap Map = new LifeSimMap();
            Map.MapSize = new Point(1, 4);
            Map.LayerManager.ListLayer.Add(new MapLayer(Map, 0));
            for (int Y = 0; Y < Map.MapSize.Y; Y++)
            {
                Terrain Tile = Map.LayerManager.ListLayer[0].ArrayTerrain[0, Y];
                Tile.TerrainTypeIndex = UnitStats.TerrainLandIndex;
                Tile.Height = 0;
                Tile.WorldPosition.Z = 0;
                Tile.MovementCost = 1;
            }

            MapInfo Room = new MapInfo(new MapContainer("Room", Map), null, null);
            MapInfo Hall = new MapInfo(new MapContainer("Hall", null), null, null);
            Vector3 Start = GridNavGeometry.CellCenter(Map, Map.LayerManager.ListLayer[0].ArrayTerrain[0, 0]);
            Vector3 DoorAt = GridNavGeometry.CellCenter(Map, Map.LayerManager.ListLayer[0].ArrayTerrain[0, 3]);
            PlayerCharacter Character = new PlayerCharacter(null, Room, Start);
            Door = new BelievedObject();
            Door.Name = "Front Door";
            Door.Kind = WorldMarkerKind.Door;
            Door.Chunk = Room;
            Door.LocalPosition = DoorAt;
            Door.State = PassageState.Closed;
            Character.Belief.ListObject.Add(Door);

            Gate = new ChunkHop();
            Gate.Name = "Front Door";
            Gate.FromChunk = Room;
            Gate.ToChunk = Hall;
            Gate.FromPosition = DoorAt;
            Gate.ToPosition = new Vector3(8, 0, 0);
            Gate.IsOpen = false;
            Marker = new WorldMarker();
            Marker.Name = "Front Door";
            Marker.Kind = WorldMarkerKind.Door;
            Marker.State = PassageState.Closed;
            Marker.LocalPosition = DoorAt;
            Marker.Link = Gate;
            Room.ListMarker.Add(Marker);
            Character.Belief.LearnLink(Gate);
            return Character;
        }

        static ChunkHop Hop(MapInfo From, MapInfo To)
        {
            ChunkHop Link = new ChunkHop();
            Link.FromChunk = From;
            Link.ToChunk = To;
            Link.IsOpen = true;
            return Link;
        }

        static BelievedObject Remember(PlayerCharacter Character, string Name, MapInfo Chunk, MapInfo From, MapInfo To)
        {
            BelievedObject Door = new BelievedObject();
            Door.Name = Name;
            Door.Kind = WorldMarkerKind.Door;
            Door.Chunk = Chunk;
            Door.State = PassageState.Closed;
            Door.Hop = Character.Belief.OpenLink(From, To);
            Character.Belief.ListObject.Add(Door);
            return Door;
        }

        static void GiveItem(PlayerCharacter Character, string Name)
        {
            Item Carried = new Item();
            Carried.Name = Name;
            Carried.ArrayTags = new string[] { Name };
            Character.AddItem(new ItemInfo(Carried, 1));
        }
    }
}
