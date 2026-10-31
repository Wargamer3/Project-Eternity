using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.UnitTests
{
    [TestClass]
    public class LifeSimWalkTests
    {
        [TestMethod]
        public void GroundFollowPathMovesTheBodyAndArrives()
        {
            LifeSimMap Map = NewMap(1, 4, 1);
            Fill(Map, 0, UnitStats.TerrainLandIndex, 0f);
            Vector3 Start = Center(Map, 0, 0, 0);
            Vector3 Goal = Center(Map, 0, 0, 3);
            PlayerCharacter Character = new PlayerCharacter(null, null, Start);
            List<PathSegment> Segments = LocalPathQuery.Find(Map, Start, Goal, Character.Body, RouteStyle.Direct);
            GroundGait.Apply(Segments, false, Character.Body);

            FollowPath Path = new FollowPath(Character, Segments);
            Path.OnStarted();
            Assert.IsInstanceOfType(Path.Active, typeof(GroundController));
            Assert.AreEqual(Traversal.Walk, Path.Active.Kind);

            GameTime Step = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.05));
            Assert.IsTrue(Path.Update(Step));
            float FirstStep = Vector3.Distance(Start, Character.Position);
            Assert.IsTrue(FirstStep > 0.01f);
            Assert.IsTrue(FirstStep < Vector3.Distance(Start, Goal) * 0.5f);

            for (int i = 0; i < 400 && Path.Update(Step); i++)
            {
            }

            Vector3 End = Segments[Segments.Count - 1].Polyline[Segments[Segments.Count - 1].Polyline.Count - 1];
            Assert.IsTrue(Vector3.Distance(Character.Position, End) < Map.TileSize.X * 0.5f);
        }

        [TestMethod]
        public void SlopeFollowRaisesTheBody()
        {
            PlayerCharacter Character = new PlayerCharacter(null, null, Vector3.Zero);
            PathSegment Slope = new PathSegment();
            Slope.Kind = Traversal.Walk;
            Slope.RequiredPose = Pose.Stand;
            Slope.Surface = NavArea.Slope;
            Slope.Polyline.Add(Vector3.Zero);
            Slope.Polyline.Add(new Vector3(0f, 40f, 20f));

            FollowPath Path = new FollowPath(Character, new List<PathSegment> { Slope });
            Path.OnStarted();
            GameTime Step = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.05));
            for (int i = 0; i < 200 && Path.Update(Step); i++)
            {
            }

            Assert.IsTrue(Character.Position.Y > 30f);
            Assert.IsTrue(Character.Position.Z > 15f);
        }

        [TestMethod]
        public void LadderDispatchesClimbAndDoesNotStandUp()
        {
            LifeSimMap Blocked = NewMap(3, 1, 1);
            SetFloor(Blocked, 0, 0, 0, UnitStats.TerrainLandIndex, 0f);
            SetFloor(Blocked, 0, 1, 0, UnitStats.TerrainWallIndex, 0f);
            SetFloor(Blocked, 0, 2, 0, UnitStats.TerrainLandIndex, 0f);
            AddLink(Blocked, "Ladder", 0, 0, 2, 0);

            Vector3 Start = Center(Blocked, 0, 0, 0);
            Vector3 Goal = Center(Blocked, 0, 2, 0);
            PlayerCharacter Climber = new PlayerCharacter(null, null, Start);
            Climber.Body.CanLadder = true;
            List<PathSegment> Climbed = LocalPathQuery.Find(Blocked, Start, Goal, Climber.Body, RouteStyle.Direct);
            Assert.IsTrue(HasKind(Climbed, Traversal.Ladder));

            List<AutomatedAction> Expanded = new List<AutomatedAction>();
            for (int i = 0; i < Climbed.Count; i++)
            {
                Expanded.Add(new PoseMarker(Climber, Climbed[i].RequiredPose, Climbed[i].RequiredPose, false));
            }

            List<AutomatedAction> Spliced = StanceSequence.Splice(Pose.Stand, Expanded);
            Assert.AreEqual(Expanded.Count, Spliced.Count);
            for (int i = 0; i < Spliced.Count; i++)
            {
                Assert.AreNotEqual("Stand Up", Spliced[i].ActionName);
            }

            FollowPath Path = new FollowPath(Climber, Climbed);
            Path.OnStarted();
            bool SawLadder = Path.Active is LadderController;
            if (SawLadder)
            {
                Assert.AreEqual(Pose.Climb, Climber.CurrentPose);
            }

            GameTime Step = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.05));
            for (int i = 0; i < 800; i++)
            {
                bool Running = Path.Update(Step);
                if (Path.Active is LadderController)
                {
                    SawLadder = true;
                    Assert.AreEqual(Pose.Climb, Climber.CurrentPose);
                }

                if (!Running)
                {
                    break;
                }
            }

            Assert.IsTrue(SawLadder);

            PlayerCharacter Walker = new PlayerCharacter(null, null, Start);
            List<PathSegment> NoClimb = LocalPathQuery.Find(Blocked, Start, Goal, Walker.Body, RouteStyle.Direct);
            Assert.AreEqual(0, NoClimb.Count);
        }

        [TestMethod]
        public void CharacterWithoutALadderTakesTheGroundRoute()
        {
            LifeSimMap Map = NewMap(3, 3, 1);
            Fill(Map, 0, UnitStats.TerrainLandIndex, 0f);
            SetFloor(Map, 0, 1, 0, UnitStats.TerrainWallIndex, 0f);
            AddLink(Map, "Ladder", 0, 0, 2, 0);

            Vector3 Start = Center(Map, 0, 0, 0);
            Vector3 Goal = Center(Map, 0, 2, 0);
            PlayerCharacter Walker = new PlayerCharacter(null, null, Start);
            List<PathSegment> Around = LocalPathQuery.Find(Map, Start, Goal, Walker.Body, RouteStyle.Direct);
            Assert.IsTrue(Around.Count > 0);
            Assert.IsFalse(HasKind(Around, Traversal.Ladder));
            Vector3 End = Around[Around.Count - 1].Polyline[Around[Around.Count - 1].Polyline.Count - 1];
            Assert.IsTrue(Vector3.Distance(End, Goal) < Map.TileSize.X);

            PlayerCharacter Climber = new PlayerCharacter(null, null, Start);
            Climber.Body.CanLadder = true;
            List<PathSegment> Shortcut = LocalPathQuery.Find(Map, Start, Goal, Climber.Body, RouteStyle.Direct);
            Assert.IsTrue(HasKind(Shortcut, Traversal.Ladder));
        }

        [TestMethod]
        public void OptionalLinkIsUsedWhenPersonalityPrefersIt()
        {
            LifeSimMap Map = NewMap(3, 2, 1);
            Fill(Map, 0, UnitStats.TerrainLandIndex, 0f);
            SetFloor(Map, 0, 1, 0, UnitStats.TerrainWallIndex, 0f);
            AddLink(Map, "Leap", 0, 0, 2, 0);

            Vector3 Start = Center(Map, 0, 0, 0);
            Vector3 Goal = Center(Map, 0, 2, 0);
            PlayerCharacter Plain = new PlayerCharacter(null, null, Start);
            Plain.Body.CanLeap = true;
            FollowPath Walked = Plain.PlanPath(Map, Goal, false);
            Assert.IsFalse(HasKind(Walked.Segments, Traversal.Leap));

            PlayerCharacter Active = new PlayerCharacter(null, null, Start);
            Active.Body.CanLeap = true;
            Personality Parkour = new Personality();
            Parkour.PrefersOptionalLinks = true;
            Active.ListPersonality.Add(Parkour);
            FollowPath Leapt = Active.PlanPath(Map, Goal, false);
            Assert.IsTrue(HasKind(Leapt.Segments, Traversal.Leap));
        }

        [TestMethod]
        public void RunIntoACrouchSlidesOrSlowsBeforeTheOpening()
        {
            TraversalCapabilities Slider = new TraversalCapabilities();
            List<PathSegment> Slid = RunThenCrouch();
            GroundGait.Apply(Slid, true, Slider);
            Assert.AreEqual(Traversal.Run, Slid[0].Kind);
            Assert.AreEqual(Traversal.Slide, Slid[1].Kind);
            Assert.AreEqual(Pose.Crouch, Slid[1].RequiredPose);
            Assert.AreEqual(Traversal.Crouch, Slid[2].Kind);

            TraversalCapabilities Careful = new TraversalCapabilities();
            Careful.CanSlide = false;
            List<PathSegment> Slowed = RunThenCrouch();
            GroundGait.Apply(Slowed, true, Careful);
            Assert.AreEqual(Traversal.Run, Slowed[0].Kind);
            Assert.AreEqual(Traversal.Crouch, Slowed[1].Kind);
            Assert.AreEqual(Traversal.Crouch, Slowed[2].Kind);

            TraversalCapabilities Runner = new TraversalCapabilities();
            PathSegment Water = new PathSegment();
            Water.Kind = Traversal.Walk;
            Water.Surface = NavArea.ShallowWater;
            Water.RequiredPose = Pose.Stand;
            Water.Polyline.Add(Vector3.Zero);
            Water.Polyline.Add(new Vector3(0f, 30f, 0f));
            List<PathSegment> Wet = new List<PathSegment> { Water };
            GroundGait.Apply(Wet, true, Runner);
            Assert.AreEqual(Traversal.Walk, Wet[0].Kind);
        }

        [TestMethod]
        public void EachLinkDispatchesItsOwnController()
        {
            PlayerCharacter Character = new PlayerCharacter(null, null, Vector3.Zero);
            Traversal[] Links = new Traversal[]
            {
                Traversal.Jump, Traversal.Leap, Traversal.LeapOver, Traversal.JumpPad,
                Traversal.Ladder, Traversal.Rope, Traversal.WallRun, Traversal.WallClimb, Traversal.LedgeClimb,
            };

            for (int i = 0; i < Links.Length; i++)
            {
                PathSegment Segment = new PathSegment();
                Segment.Kind = Links[i];
                Segment.RequiredPose = TraversalPoses.For(Links[i]);
                Segment.Polyline.Add(Vector3.Zero);
                Segment.Polyline.Add(new Vector3(0f, 0f, 20f));
                TraversalController Controller = TraversalControllers.Create(Character, Segment);
                Assert.AreEqual(Links[i].ToString() + "Controller", Controller.GetType().Name);
                Assert.AreEqual(Segment.RequiredPose, Character.CurrentPose);
            }
        }

        [TestMethod]
        public void LadderTickMovesAlongTheLink()
        {
            PlayerCharacter Character = new PlayerCharacter(null, null, Vector3.Zero);
            PathSegment Ladder = new PathSegment();
            Ladder.Kind = Traversal.Ladder;
            Ladder.RequiredPose = Pose.Climb;
            Ladder.Polyline.Add(Vector3.Zero);
            Ladder.Polyline.Add(new Vector3(0f, 0f, 32f));
            FollowPath Path = new FollowPath(Character, new List<PathSegment> { Ladder });
            Path.OnStarted();
            Assert.IsInstanceOfType(Path.Active, typeof(LadderController));
            Assert.AreEqual(Pose.Climb, Character.CurrentPose);

            Path.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.05)));
            Assert.IsTrue(Character.Position.Z > 0.1f);
            Assert.IsTrue(Character.Position.Z < 10f);
        }

        [TestMethod]
        public void SittingThenEatInsertsStandUpAndHitsStayInPose()
        {
            PlayerCharacter Character = new PlayerCharacter(null, null, Vector3.Zero);
            PoseMarker Eat = new PoseMarker(Character, Pose.Stand, Pose.Stand, false);
            List<AutomatedAction> Stood = StanceSequence.Splice(Pose.Sit, new List<AutomatedAction> { Eat });
            Assert.AreEqual(2, Stood.Count);
            Assert.IsInstanceOfType(Stood[0], typeof(StandUpAction));
            Assert.AreSame(Eat, Stood[1]);

            PoseMarker Seat = new PoseMarker(Character, Pose.Sit, Pose.Sit, false);
            List<AutomatedAction> Sat = StanceSequence.Splice(Pose.Stand, new List<AutomatedAction> { Seat });
            Assert.IsInstanceOfType(Sat[0], typeof(SitDownAction));

            PoseMarker Hit = new PoseMarker(Character, Pose.Stand, Pose.Stand, true);
            List<AutomatedAction> OnRope = StanceSequence.Splice(Pose.Climb, new List<AutomatedAction> { Hit });
            Assert.AreEqual(1, OnRope.Count);
            Assert.AreSame(Hit, OnRope[0]);

            Character.CurrentPose = Pose.Climb;
            AIObjective Guard = new AIObjective();
            Guard.GoalName = "Guard";
            Character.ListAIObjective.Add(Guard);
            Character.ApplyReaction(Hit);
            Assert.AreEqual(Pose.Climb, Character.CurrentPose);
            Assert.AreEqual(1, Character.ListAIObjective.Count);
            Assert.AreEqual("Guard", Character.ListAIObjective[0].GoalName);

            PoseMarker Downed = new PoseMarker(Character, Pose.Climb, Pose.Incapacitated, false);
            Character.ApplyReaction(Downed);
            Assert.AreEqual(Pose.Incapacitated, Character.CurrentPose);
            Assert.AreEqual(1, Character.ListAIObjective.Count);
            Assert.AreEqual("Incapacitated", Character.ListAIObjective[0].GoalName);
            Assert.AreSame(Downed, Character.ListAIObjective[0].ListRemainingStep[0]);
        }

        private static List<PathSegment> RunThenCrouch()
        {
            PathSegment Run = new PathSegment();
            Run.Kind = Traversal.Run;
            Run.Surface = NavArea.Ground;
            Run.RequiredPose = Pose.Stand;
            Run.Polyline.Add(new Vector3(0f, 0f, 0f));
            Run.Polyline.Add(new Vector3(0f, 40f, 0f));

            PathSegment Crouch = new PathSegment();
            Crouch.Kind = Traversal.Crouch;
            Crouch.Surface = NavArea.Crouch;
            Crouch.RequiredPose = Pose.Crouch;
            Crouch.Polyline.Add(new Vector3(0f, 40f, 0f));
            Crouch.Polyline.Add(new Vector3(0f, 48f, 0f));
            Crouch.Polyline.Add(new Vector3(0f, 80f, 0f));
            return new List<PathSegment> { Run, Crouch };
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

        private static void AddLink(LifeSimMap Map, string Tag, int FromX, int FromY, int ToX, int ToY)
        {
            TeleportPoint Link = new TeleportPoint(Map.LayerManager.ListLayer[0].ArrayTerrain[FromX, FromY].WorldPosition, Tag, 0, 0, 0);
            Link.OtherMapEntryPoint = new Point(ToX, ToY);
            Link.OtherMapEntryLayer = 0;
            Map.LayerManager.ListLayer[0].ListTeleportPoint.Add(Link);
        }

        private static Vector3 Center(LifeSimMap Map, int Layer, int X, int Y)
        {
            return GridNavGeometry.CellCenter(Map, Map.LayerManager.ListLayer[Layer].ArrayTerrain[X, Y]);
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

        private class PoseMarker : AutomatedAction
        {
            private readonly Pose Needed;
            private readonly Pose Left;
            private readonly bool Stay;

            public PoseMarker(PlayerCharacter Owner, Pose Needed, Pose Left, bool Stay)
                : base("Marker", Owner)
            {
                this.Needed = Needed;
                this.Left = Left;
                this.Stay = Stay;
            }

            public override Pose PoseNeeded => Needed;

            public override Pose PoseLeft => Left;

            public override bool HitStaysInPose => Stay;

            public override void OnStarted()
            {
            }

            public override void OnCancel()
            {
            }

            public override bool Update(GameTime gameTime)
            {
                return false;
            }

            public override void DoRead(ByteReader BR)
            {
            }

            public override void DoWrite(ByteWriter BW)
            {
            }

            protected override AutomatedAction Copy()
            {
                return new PoseMarker(Owner, Needed, Left, Stay);
            }

            public override void Draw(CustomSpriteBatch g)
            {
            }

            public override void DrawIcon(CustomSpriteBatch g, Vector2 Position)
            {
            }
        }
    }
}
