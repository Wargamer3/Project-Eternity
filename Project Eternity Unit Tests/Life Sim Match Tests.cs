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
    public class LifeSimMatchTests
    {
        [TestMethod]
        public void ANewCharacterStaysEmptyUntilDefaultAIIsApplied()
        {
            PlayerCharacter Born = new PlayerCharacter(null, null, Vector3.Zero);
            Assert.IsFalse(Born.HasAnyScript());

            PlayerCharacter Kept = new PlayerCharacter(null, null, Vector3.Zero);
            Kept.AddAction(new EatJerkyAction());
            DefaultCharacterAI.Assign(Kept);
            Assert.IsTrue(Kept.HasScript("Eat Jerky"));
            Assert.AreEqual(1, Kept.ScriptCount("Eat Jerky"));
            Assert.IsFalse(Kept.HasScript("Walk To Position"));

            PlayerCharacter Fresh = new PlayerCharacter(null, null, Vector3.Zero);
            DefaultCharacterAI.Assign(Fresh);
            DefaultCharacterAI.Assign(Fresh);
            Assert.AreEqual(1, Fresh.ScriptCount("Walk To Position"));
            Assert.AreEqual(1, Fresh.ScriptCount("Run To Position"));
            Assert.AreEqual(1, Fresh.ScriptCount("Tiptoe"));
            Assert.AreEqual(1, Fresh.ScriptCount("Drive"));
            Assert.IsTrue(Fresh.HasScript("Eat Jerky"));
            Assert.IsTrue(Fresh.HasScript("Go To Restaurant"));
            Assert.IsTrue(Fresh.HasScript("Go Through Door"));
            Assert.IsTrue(Fresh.HasScript("Lock Door"));
            Assert.IsTrue(Fresh.HasScript("Unlock Door"));

            PlayerCharacter Hooked = new PlayerCharacter(null, null, Vector3.Zero);
            Assert.IsFalse(Hooked.HasAnyScript());
            DefaultAI.ApplyIfEmpty(Hooked);
            Assert.AreEqual(1, Hooked.ScriptCount("Tiptoe"));
        }

        [TestMethod]
        public void TiptoeIsASlowerWalk()
        {
            LifeSimMap Map = NewMap(1, 4);
            Fill(Map, UnitStats.TerrainLandIndex);
            MapInfo Room = new MapInfo(new MapContainer("Lane", Map), null, null);
            Vector3 Start = Center(Map, 0, 0);
            Vector3 Goal = Center(Map, 0, 3);
            PlayerCharacter Character = new PlayerCharacter(null, Room, Start);
            Character.SetObjective(MoveAction.MoveGoal, Goal);

            TiptoeAction Tiptoe = new TiptoeAction();
            Tiptoe.Init(Character.SharedMapContex);
            List<AutomatedAction> Plan = Tiptoe.GetAIExecutionPlan(null);
            Assert.AreEqual("Follow Path", Plan[0].ActionName);
            Assert.AreEqual(TiptoeAction.Pace, Character.MovePace, 0.001f);

            Character.SetPosition(Start);
            Character.Move(Vector3.UnitY, 1, 0.2);
            float Slow = Character.Position.Y - Start.Y;

            WalkToPositionAction Walk = new WalkToPositionAction();
            Walk.Init(Character.SharedMapContex);
            Walk.GetAIExecutionPlan(null);
            Assert.AreEqual(1f, Character.MovePace, 0.001f);
            Character.SetPosition(Start);
            Character.MovementSpeed = Vector3.Zero;
            Character.Move(Vector3.UnitY, 1, 0.2);
            float Fast = Character.Position.Y - Start.Y;

            Assert.IsTrue(Slow > 0.01f);
            Assert.IsTrue(Slow < Fast);
        }

        [TestMethod]
        public void DriveNeedsABelievedCarAndOutrunsWalking()
        {
            LifeSimMap Map = NewMap(1, 4);
            Fill(Map, UnitStats.TerrainLandIndex);
            MapInfo Room = new MapInfo(new MapContainer("Road", Map), null, null);
            Vector3 Start = Center(Map, 0, 0);
            Vector3 Goal = Center(Map, 0, 3);
            PlayerCharacter Character = new PlayerCharacter(null, Room, Start);
            Character.SetObjective(MoveAction.MoveGoal, Goal);

            DriveAction OnFoot = new DriveAction();
            OnFoot.Init(Character.SharedMapContex);
            Assert.AreEqual(0, OnFoot.GetAIExecutionPlan(null).Count);

            BelievedObject Car = new BelievedObject();
            Car.Name = "Car";
            Car.Tag = "Car";
            Car.Chunk = Room;
            Car.LocalPosition = Start;
            Character.Belief.ListObject.Add(Car);

            List<AutomatedAction> Plan = OnFoot.GetAIExecutionPlan(null);
            DriveLegAction Leg = null;
            for (int i = 0; i < Plan.Count; i++)
            {
                Leg = Plan[i] as DriveLegAction;
                if (Leg != null)
                {
                    break;
                }
            }

            Assert.IsNotNull(Leg);
            float Before = Character.Position.Y;
            Assert.IsTrue(Leg.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.2))));
            float Moved = Character.Position.Y - Before;
            Assert.IsTrue(Moved > 10f);
            Assert.IsTrue(Moved < Vector3.Distance(Start, Goal));
        }

        [TestMethod]
        public void AnObstacleIsCutSoThePathGoesAround()
        {
            LifeSimMap Map = NewMap(3, 6);
            Fill(Map, UnitStats.TerrainLandIndex);
            MapInfo Room = new MapInfo(new MapContainer("Yard", Map), null, null);
            Vector3 Start = Center(Map, 1, 0);
            Vector3 Goal = Center(Map, 1, 5);
            Vector3 Block = Center(Map, 1, 3);
            PlayerCharacter Character = new PlayerCharacter(null, Room, Start);
            int Triangles = GridNavGeometry.Build(Map).Triangles.Count;

            MatchFacts Facts = new MatchFacts();
            Facts.OurFlag = Start;
            Facts.EnemyFlag = Goal;
            TakeTheFlagAction Take = new TakeTheFlagAction();
            Take.Match = Facts;
            AIObjective Bottom = MatchRole.Order(Character, Take, Start, 0f, false);
            Character.CommitBottom(Bottom);
            FollowPath Straight = (FollowPath)Character.TopObjective().ListRemainingStep[0];
            Assert.IsTrue(Crosses(Straight, Block, NavCut.DefaultRadius));

            BelievedObject Crate = new BelievedObject();
            Crate.Name = "Crate";
            Crate.Kind = WorldMarkerKind.Obstacle;
            Crate.Chunk = Room;
            Crate.LocalPosition = Block;
            Character.Belief.ListObject.Add(Crate);
            Character.AccountForWhatWasSeen();

            Assert.AreEqual(1, Character.ListAIObjective.Count);
            Assert.AreSame(Take, Character.TopObjective().ChosenAction);
            Assert.AreEqual("Follow Path", Character.TopObjective().ListRemainingStep[0].ActionName);
            FollowPath Around = (FollowPath)Character.TopObjective().ListRemainingStep[0];
            Assert.IsTrue(Around.Segments.Count > 0);
            Assert.IsFalse(Crosses(Around, Block, NavCut.DefaultRadius));
            Assert.AreEqual(Triangles, GridNavGeometry.Build(Map).Triangles.Count);
        }

        [TestMethod]
        public void ABelievedJumpPadIsUsedOnlyWhenTheBodyCan()
        {
            LifeSimMap Map = NewMap(3, 1);
            Fill(Map, UnitStats.TerrainLandIndex);
            SetFloor(Map, 1, 0, UnitStats.TerrainWallIndex);
            MapInfo Room = new MapInfo(new MapContainer("Gap", Map), null, null);
            Vector3 From = Center(Map, 0, 0);
            Vector3 To = Center(Map, 2, 0);

            WorldMarker Pad = new WorldMarker();
            Pad.Name = "Pad";
            Pad.Kind = WorldMarkerKind.Object;
            Pad.LocalPosition = From;
            Pad.HasTraversal = true;
            Pad.TraversalKind = Traversal.JumpPad;
            Pad.TraversalEnd = To;
            Room.ListMarker.Add(Pad);

            PlayerCharacter Near = new PlayerCharacter(null, Room, From);
            Near.PerceptionRadius = 64;
            Near.Perceive();
            Assert.IsTrue(Near.Belief.FindObject("Pad").HasTraversal);

            PlayerCharacter Far = new PlayerCharacter(null, Room, new Vector3(0f, 4000f, 0f));
            Far.PerceptionRadius = 8;
            Far.Perceive();
            Assert.IsNull(Far.Belief.FindObject("Pad"));

            Personality Eager = new Personality();
            Eager.PrefersOptionalLinks = true;
            Near.ListPersonality.Add(Eager);
            FollowPath Denied = Near.PlanPath(Map, To, false);
            Assert.IsFalse(HasKind(Denied, Traversal.JumpPad));

            Near.Body.CanJumpPad = true;
            FollowPath Used = Near.PlanPath(Map, To, false);
            Assert.IsTrue(HasKind(Used, Traversal.JumpPad));

            Far.Body.CanJumpPad = true;
            FollowPath Unseen = Far.PlanPath(Map, To, false);
            Assert.IsFalse(HasKind(Unseen, Traversal.JumpPad));
        }

        [TestMethod]
        public void FlagRolesReadTheSameHolder()
        {
            LifeSimMap Map = NewMap(1, 8);
            Fill(Map, UnitStats.TerrainLandIndex);
            MapInfo Room = new MapInfo(new MapContainer("Arena", Map), null, null);
            MatchFacts Match = new MatchFacts();
            Match.OurFlag = Center(Map, 0, 0);
            Match.EnemyFlag = Center(Map, 0, 7);
            Match.DroppedFlag = Center(Map, 0, 5);

            PlayerCharacter Ally = new PlayerCharacter(null, Room, Center(Map, 0, 4));
            Match.EnemyCarrier = Ally;
            Match.Holder = Ally;

            PlayerCharacter First = new PlayerCharacter(null, Room, Center(Map, 0, 1));
            PlayerCharacter Second = new PlayerCharacter(null, Room, Center(Map, 0, 2));
            DisruptAction FirstDisrupt = Role<DisruptAction>(First, Match);
            DisruptAction SecondDisrupt = Role<DisruptAction>(Second, Match);
            Vector3 FirstEnd = EndOf(FirstDisrupt.GetAIExecutionPlan(null));
            Vector3 SecondEnd = EndOf(SecondDisrupt.GetAIExecutionPlan(null));
            Assert.IsTrue(Vector3.Distance(FirstEnd, Ally.Position) < Map.TileSize.X);
            Assert.IsTrue(Vector3.Distance(SecondEnd, Ally.Position) < Map.TileSize.X);

            Ally.SetPosition(Center(Map, 0, 6));
            FirstEnd = EndOf(FirstDisrupt.GetAIExecutionPlan(null));
            SecondEnd = EndOf(SecondDisrupt.GetAIExecutionPlan(null));
            Assert.IsTrue(Vector3.Distance(FirstEnd, Ally.Position) < Map.TileSize.X);
            Assert.IsTrue(Vector3.Distance(SecondEnd, Ally.Position) < Map.TileSize.X);

            PlayerCharacter Runner = new PlayerCharacter(null, Room, Center(Map, 0, 3));
            TakeTheFlagAction Take = Role<TakeTheFlagAction>(Runner, Match);
            Assert.IsTrue(Vector3.Distance(EndOf(Take.GetAIExecutionPlan(null)), Match.EnemyFlag) < Map.TileSize.X);
            Match.Holder = Runner;
            Assert.IsTrue(Vector3.Distance(EndOf(Take.GetAIExecutionPlan(null)), Match.OurFlag) < Map.TileSize.X);
            Match.Holder = null;
            Match.FlagIsDropped = true;
            Assert.IsTrue(Vector3.Distance(EndOf(Take.GetAIExecutionPlan(null)), Match.DroppedFlag) < Map.TileSize.X);

            PlayerCharacter Escort = new PlayerCharacter(null, Room, Center(Map, 0, 1));
            Match.FlagIsDropped = false;
            Match.Holder = Ally;
            Ally.SetPosition(Center(Map, 0, 4));
            EscortHolderAction Beside = Role<EscortHolderAction>(Escort, Match);
            Vector3 Shoulder = Ally.Position + Beside.Offset;
            Assert.IsTrue(Vector3.Distance(EndOf(Beside.GetAIExecutionPlan(null)), Shoulder) < Map.TileSize.X);
            Match.Holder = null;
            Match.FlagIsDropped = true;
            Assert.IsTrue(Vector3.Distance(EndOf(Beside.GetAIExecutionPlan(null)), Match.DroppedFlag) < Map.TileSize.X);
        }

        [TestMethod]
        public void AGuardStaysAfterAFightAndAForbiddenRoleIsRefused()
        {
            LifeSimMap Map = NewMap(1, 8);
            Fill(Map, UnitStats.TerrainLandIndex);
            MapInfo Room = new MapInfo(new MapContainer("Stand", Map), null, null);
            MatchFacts Match = new MatchFacts();
            Match.OurFlag = Center(Map, 0, 0);
            Match.EnemyFlag = Center(Map, 0, 7);

            PlayerCharacter Character = new PlayerCharacter(null, Room, Center(Map, 0, 2));
            Character.AIControlled = true;
            GuardOurFlagAction Guard = new GuardOurFlagAction();
            Guard.Match = Match;
            AIObjective Bottom = MatchRole.Order(Character, Guard, Match.OurFlag, 30f, true);
            Character.CommitBottom(Bottom);

            EatJerkyAction Food = new EatJerkyAction();
            Food.Init(Character.SharedMapContex);
            Food.AIWeight = 100;
            Character.AddAction(Food);
            Character.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.05)));
            Assert.AreSame(Guard, Character.BottomObjective().ChosenAction);
            Assert.IsTrue(Character.BottomObjective().IsStandingOrder);

            AIObjective Fight = new AIObjective();
            Fight.GoalName = "Fight";
            Fight.IsFight = true;
            Fight.ChosenAction = Guard;
            Character.PushShortTerm(Fight);
            Character.SetPosition(Center(Map, 0, 7));
            Character.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.05)));
            Assert.AreEqual(1, Character.ListAIObjective.Count);
            Assert.AreSame(Guard, Character.BottomObjective().ChosenAction);
            Assert.IsTrue(Character.BottomObjective().IsStandingOrder);

            Personality Shy = new Personality();
            Shy.ListForbiddenActionName.Add("Take the Flag");
            Character.ListPersonality.Add(Shy);
            TakeTheFlagAction Take = new TakeTheFlagAction();
            Take.Match = Match;
            Assert.IsNull(MatchRole.Order(Character, Take, Match.EnemyFlag, 0f, false));

            PlayerCharacter Willing = new PlayerCharacter(null, Room, Center(Map, 0, 2));
            Assert.IsNotNull(MatchRole.Order(Willing, Role<TakeTheFlagAction>(Willing, Match), Match.EnemyFlag, 0f, false));
        }

        [TestMethod]
        public void HatchingAnEggCapturesTheNearestEmptyOutpost()
        {
            LifeSimMap Map = NewMap(1, 8);
            Fill(Map, UnitStats.TerrainLandIndex);
            MapInfo Room = new MapInfo(new MapContainer("Field", Map), null, null);
            MatchFacts Match = new MatchFacts();
            Match.Eggs = 1;
            Outpost Near = OutpostAt("Near", Room, Center(Map, 0, 2), OutpostSide.Empty, false);
            Outpost Far = OutpostAt("Far", Room, Center(Map, 0, 7), OutpostSide.Empty, false);
            Outpost Held = OutpostAt("Held", Room, Center(Map, 0, 1), OutpostSide.Ours, true);
            Match.Outposts.Add(Far);
            Match.Outposts.Add(Held);
            Match.Outposts.Add(Near);

            PlayerCharacter Character = new PlayerCharacter(null, Room, Center(Map, 0, 0));
            HatchEggAction Hatch = new HatchEggAction();
            Hatch.Match = Match;
            Hatch.Init(Character.SharedMapContex);
            List<AutomatedAction> Plan = Hatch.GetAIExecutionPlan(null);
            Assert.IsTrue(Vector3.Distance(EndOf(Plan), Near.Position) < Map.TileSize.X);
            HatchEggStep Step = FindStep<HatchEggStep>(Plan);
            Assert.IsFalse(Step.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1))));

            Assert.AreEqual(OutpostSide.Ours, Near.Side);
            Assert.IsTrue(Near.CreatureAlive);
            Assert.AreEqual(0, Match.Eggs);
            Assert.AreEqual(OutpostSide.Empty, Far.Side);
            Assert.AreEqual(OutpostSide.Ours, Held.Side);

            PlayerCharacter Teammate = new PlayerCharacter(null, Room, Center(Map, 0, 3));
            HatchEggAction Other = new HatchEggAction();
            Other.Match = Match;
            Other.Init(Teammate.SharedMapContex);
            Assert.AreEqual(0, Other.GetAIExecutionPlan(null).Count);
            Assert.AreEqual(OutpostSide.Ours, Match.Outposts[2].Side);
        }

        [TestMethod]
        public void KillingTheCreatureClearsAnEnemyOutpost()
        {
            LifeSimMap Map = NewMap(1, 8);
            Fill(Map, UnitStats.TerrainLandIndex);
            MapInfo Room = new MapInfo(new MapContainer("Field", Map), null, null);
            MatchFacts Match = new MatchFacts();
            Outpost Dead = OutpostAt("Dead", Room, Center(Map, 0, 2), OutpostSide.Enemy, false);
            Outpost Living = OutpostAt("Living", Room, Center(Map, 0, 6), OutpostSide.Enemy, true);
            Match.Outposts.Add(Dead);
            Match.Outposts.Add(Living);

            PlayerCharacter Character = new PlayerCharacter(null, Room, Center(Map, 0, 0));
            KillCreatureAction Kill = new KillCreatureAction();
            Kill.Match = Match;
            Kill.Init(Character.SharedMapContex);
            List<AutomatedAction> Plan = Kill.GetAIExecutionPlan(null);
            Assert.IsTrue(Vector3.Distance(EndOf(Plan), Living.Position) < Map.TileSize.X);
            Assert.IsFalse(FindStep<KillCreatureStep>(Plan).Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1))));

            Assert.IsFalse(Living.CreatureAlive);
            Assert.AreEqual(OutpostSide.Empty, Living.Side);
            Assert.AreEqual(OutpostSide.Enemy, Dead.Side);

            PlayerCharacter Teammate = new PlayerCharacter(null, Room, Center(Map, 0, 1));
            KillCreatureAction Other = new KillCreatureAction();
            Other.Match = Match;
            Other.Init(Teammate.SharedMapContex);
            Assert.AreEqual(0, Other.GetAIExecutionPlan(null).Count);
        }

        [TestMethod]
        public void PointsBuyATurretAndAWallOnlyTheViewerTreatsAsACut()
        {
            MatchFacts Match = new MatchFacts();
            Match.TickBase(10f);
            Assert.AreEqual(1, Match.Eggs);
            Assert.AreEqual(0, Match.Points);

            LifeSimMap Map = NewMap(3, 6);
            Fill(Map, UnitStats.TerrainLandIndex);
            MapInfo Room = new MapInfo(new MapContainer("Yard", Map), null, null);
            Outpost First = OutpostAt("A", Room, Center(Map, 0, 0), OutpostSide.Ours, true);
            Outpost Second = OutpostAt("B", Room, Center(Map, 2, 0), OutpostSide.Ours, true);
            Match.Outposts.Add(First);
            Match.Outposts.Add(Second);
            Match.TickBase(1f);
            Assert.AreEqual(2, Match.Points);

            PlayerCharacter Buyer = new PlayerCharacter(null, Room, Center(Map, 1, 0));
            PlaceTurretAction Turret = new PlaceTurretAction();
            Turret.Match = Match;
            Turret.Place = Center(Map, 1, 3);
            Turret.Init(Buyer.SharedMapContex);
            Assert.AreEqual(0, Turret.GetAIExecutionPlan(null).Count);

            Match.Points = Match.TurretCost;
            List<AutomatedAction> TurretPlan = Turret.GetAIExecutionPlan(null);
            Assert.IsFalse(FindStep<PlaceDeployableStep>(TurretPlan).Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1))));
            Assert.AreEqual(0, Match.Points);
            Assert.IsNotNull(Buyer.Belief.FindTagged("Turret"));

            PlayerCharacter Unseen = new PlayerCharacter(null, Room, new Vector3(0f, 4000f, 0f));
            Unseen.PerceptionRadius = 8;
            Unseen.Perceive();
            Assert.IsNull(Unseen.Belief.FindTagged("Turret"));

            PlayerCharacter Seen = new PlayerCharacter(null, Room, Turret.Place);
            Seen.PerceptionRadius = 64;
            Seen.Perceive();
            Assert.IsNotNull(Seen.Belief.FindTagged("Turret"));
            Assert.AreEqual(WorldMarkerKind.Object, Seen.Belief.FindTagged("Turret").Kind);

            Match.Points = Match.WallCost;
            Vector3 Block = Center(Map, 1, 3);
            PlaceWallAction Wall = new PlaceWallAction();
            Wall.Match = Match;
            Wall.Place = Block;
            Wall.Init(Buyer.SharedMapContex);
            Assert.IsFalse(FindStep<PlaceDeployableStep>(Wall.GetAIExecutionPlan(null)).Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1))));
            Assert.AreEqual(0, Match.Points);

            FollowPath Around = Buyer.PlanPath(Map, Center(Map, 1, 5), false);
            Assert.IsTrue(Around.Segments.Count > 0);
            Assert.IsFalse(Crosses(Around, Block, NavCut.DefaultRadius));

            PlayerCharacter Blind = new PlayerCharacter(null, Room, Center(Map, 1, 0));
            FollowPath Through = Blind.PlanPath(Map, Center(Map, 1, 5), false);
            Assert.IsTrue(Crosses(Through, Block, NavCut.DefaultRadius));

            Blind.SetPosition(Block);
            Blind.PerceptionRadius = 64;
            Blind.Perceive();
            FollowPath Learned = Blind.PlanPath(Map, Center(Map, 1, 5), false);
            Assert.IsFalse(Crosses(Learned, Block, NavCut.DefaultRadius));
        }

        [TestMethod]
        public void AClassBonusChangesMovementAndDoesNotGrantAJumpByPreference()
        {
            PlayerCharacter Quick = new PlayerCharacter(null, null, Vector3.Zero);
            PlayerCharacter Plain = new PlayerCharacter(null, null, Vector3.Zero);
            ClassBonus.ApplyExtraSpeed(Quick);
            Quick.Move(Vector3.UnitY, 1, 0.2);
            Plain.Move(Vector3.UnitY, 1, 0.2);
            Assert.AreEqual(ClassBonus.ExtraSpeed, Quick.MovePace, 0.001f);
            Assert.IsTrue(Quick.Position.Y > Plain.Position.Y);
            Assert.IsFalse(Quick.Body.CanJump);

            PlayerCharacter Jumper = new PlayerCharacter(null, null, Vector3.Zero);
            Personality Eager = new Personality();
            Eager.PrefersOptionalLinks = true;
            Jumper.ListPersonality.Add(Eager);
            Assert.IsFalse(Jumper.Body.CanJump);
            ClassBonus.ApplyExtraJump(Jumper);
            Assert.IsTrue(Jumper.Body.CanJump);
            Assert.AreEqual(1f, Jumper.MovePace, 0.001f);

            PlayerCharacter Swimmer = new PlayerCharacter(null, null, Vector3.Zero);
            PlayerCharacter Floater = new PlayerCharacter(null, null, Vector3.Zero);
            ClassBonus.ApplyFasterSwim(Swimmer);
            Assert.IsTrue(Swimmer.Body.CanSwim);
            Assert.AreEqual(ClassBonus.FasterSwim, Swimmer.SwimPace, 0.001f);
            PathSegment Water = SwimLane();
            new GroundController(Swimmer, Water).Tick(0.2);
            new GroundController(Floater, Water).Tick(0.2);
            Assert.IsTrue(Swimmer.Position.Y > Floater.Position.Y);

            PlayerCharacter Runner = new PlayerCharacter(null, null, Vector3.Zero);
            PlayerCharacter Walker = new PlayerCharacter(null, null, Vector3.Zero);
            ClassBonus.ApplyFasterWallRun(Runner);
            Assert.IsTrue(Runner.Body.CanWallRun);
            Assert.AreEqual(ClassBonus.FasterWallRun, Runner.WallRunPace, 0.001f);
            PathSegment Wall = WallLane();
            new WallRunController(Runner, Wall).Tick(0.1);
            new WallRunController(Walker, Wall).Tick(0.1);
            Assert.IsTrue(Runner.Position.Y > Walker.Position.Y);
        }

        [TestMethod]
        public void AFightStandsAtWeaponRangeAndLeavesTheGuardUnderIt()
        {
            LifeSimMap Map = NewMap(1, 8);
            Fill(Map, UnitStats.TerrainLandIndex);
            MapInfo Room = new MapInfo(new MapContainer("Pit", Map), null, null);
            Vector3 Home = Center(Map, 0, 1);
            PlayerCharacter Guard = new PlayerCharacter(null, Room, Home);
            PlayerCharacter Enemy = new PlayerCharacter(null, Room, Center(Map, 0, 2));
            Guard.AIControlled = true;
            Weapon Sword = new Weapon();
            Sword.Name = "Sword";
            Sword.NearRange = 2f;
            Sword.FarRange = 2f;
            Sword.DamageAmount = 4;
            Guard.Equip(Sword);

            MatchFacts Match = new MatchFacts();
            Match.OurFlag = Home;
            Match.EnemyFlag = Center(Map, 0, 7);
            GuardOurFlagAction Post = new GuardOurFlagAction();
            Post.Match = Match;
            Guard.CommitBottom(MatchRole.Order(Guard, Post, Home, 40f, true));

            FightAction Fight = new FightAction();
            Fight.Target = Enemy;
            Fight.Init(Guard.SharedMapContex);
            Guard.AddAction(Fight);
            Personality Snipe = new Personality();
            Snipe.HasCombatTaste = true;
            Snipe.Taste = CombatTaste.Snipe;
            Guard.ListPersonality.Add(Snipe);
            Guard.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.05)));

            Assert.AreEqual(2, Guard.ListAIObjective.Count);
            Assert.AreEqual("Fight", Guard.TopObjective().GoalName);
            Assert.AreSame(Post, Guard.BottomObjective().ChosenAction);
            Vector3 Stand = EndOf(Guard.TopObjective().ListRemainingStep);
            Assert.IsTrue(Vector3.Distance(Stand, Enemy.Position) < 8f);
            Assert.IsTrue(Vector3.Distance(Stand, Enemy.Position) < Vector3.Distance(Home, Enemy.Position));

            Weapon Bow = new Weapon();
            Bow.NearRange = 2f;
            Bow.FarRange = 20f;
            Bow.DamageAmount = 4;
            Guard.Equip(Bow);
            List<AutomatedAction> Far = Fight.GetAIExecutionPlan(null);
            Assert.IsTrue(Vector3.Distance(EndOf(Far), Enemy.Position) > 12f);

            Guard.PopTopObjective();
            Assert.AreEqual(1, Guard.ListAIObjective.Count);
            Assert.AreSame(Post, Guard.BottomObjective().ChosenAction);

            Enemy.CurrentHP = 4;
            Enemy.CurrentPose = Pose.Stand;
            StrikeStep Hit = FindStep<StrikeStep>(Far);
            Hit.OnStarted();
            Assert.AreEqual(Pose.Stand, Hit.PoseAtHit);
            Assert.IsFalse(Hit.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1))));
            Assert.AreEqual(0, Enemy.CurrentHP);
            Assert.AreEqual(Pose.Incapacitated, Enemy.CurrentPose);
            Assert.AreEqual("Incapacitated", Enemy.TopObjective().GoalName);
        }

        [TestMethod]
        public void HideUsesCoverAndFlankFallsBackWhenTheChunkHasNoMark()
        {
            LifeSimMap Map = NewMap(1, 8);
            Fill(Map, UnitStats.TerrainLandIndex);
            MapInfo Room = new MapInfo(new MapContainer("Pit", Map), null, null);
            PlayerCharacter Fighter = new PlayerCharacter(null, Room, Center(Map, 0, 1));
            PlayerCharacter Enemy = new PlayerCharacter(null, Room, Center(Map, 0, 6));
            Weapon Bow = new Weapon();
            Bow.NearRange = 2f;
            Bow.FarRange = 80f;
            Fighter.Equip(Bow);
            Personality Taste = new Personality();
            Taste.HasCombatTaste = true;
            Taste.Taste = CombatTaste.BehindTheirLines;
            Fighter.ListPersonality.Add(Taste);

            FightAction Fight = new FightAction();
            Fight.Target = Enemy;
            Fight.ThreatRadius = 300f;
            Fight.Init(Fighter.SharedMapContex);
            Vector3 Direct = EndOf(Fight.GetAIExecutionPlan(null));
            Assert.IsTrue(Vector3.Distance(Direct, FightStand.Point(Fighter, Enemy.Position, 2f)) < Map.TileSize.X);

            BelievedObject Flank = new BelievedObject();
            Flank.Name = "Side";
            Flank.Tag = "Flank";
            Flank.Chunk = Room;
            Flank.LocalPosition = Center(Map, 0, 3);
            Fighter.Belief.ListObject.Add(Flank);
            Assert.IsTrue(Vector3.Distance(EndOf(Fight.GetAIExecutionPlan(null)), Flank.LocalPosition) < Map.TileSize.X);

            Taste.Taste = CombatTaste.Hide;
            BelievedObject Cover = new BelievedObject();
            Cover.Name = "Rock";
            Cover.Tag = "Cover";
            Cover.Chunk = Room;
            Cover.LocalPosition = Center(Map, 0, 4);
            Fighter.Belief.ListObject.Add(Cover);
            Assert.IsTrue(Vector3.Distance(EndOf(Fight.GetAIExecutionPlan(null)), Cover.LocalPosition) < Map.TileSize.X);
        }

        [TestMethod]
        public void FlagRolesAreAssignedInOrderAndTasteDoesNotPickTheRunner()
        {
            LifeSimMap Map = NewMap(1, 4);
            Fill(Map, UnitStats.TerrainLandIndex);
            MapInfo Room = new MapInfo(new MapContainer("Field", Map), null, null);
            MatchFacts Match = new MatchFacts();
            Match.OurFlag = Center(Map, 0, 0);
            Match.EnemyFlag = Center(Map, 0, 3);

            PlayerCharacter Sniper = new PlayerCharacter(null, Room, Center(Map, 0, 1));
            PlayerCharacter Brawler = new PlayerCharacter(null, Room, Center(Map, 0, 2));
            Personality Far = new Personality();
            Far.HasCombatTaste = true;
            Far.Taste = CombatTaste.Snipe;
            Sniper.ListPersonality.Add(Far);
            Personality Near = new Personality();
            Near.HasCombatTaste = true;
            Near.Taste = CombatTaste.Close;
            Brawler.ListPersonality.Add(Near);
            MatchRole.AssignFlagRoles(new List<PlayerCharacter> { Sniper, Brawler }, Match);
            Assert.AreEqual("Take the Flag", Sniper.BottomObjective().ChosenAction.AIActionName);
            Assert.AreEqual("Guard our Flag", Brawler.BottomObjective().ChosenAction.AIActionName);

            PlayerCharacter Shy = new PlayerCharacter(null, Room, Center(Map, 0, 1));
            Personality Refuse = new Personality();
            Refuse.ListForbiddenActionName.Add("Take the Flag");
            Shy.ListPersonality.Add(Refuse);
            PlayerCharacter Runner = new PlayerCharacter(null, Room, Center(Map, 0, 2));
            MatchRole.AssignFlagRoles(new List<PlayerCharacter> { Shy, Runner }, Match);
            Assert.AreEqual("Guard our Flag", Shy.BottomObjective().ChosenAction.AIActionName);
            Assert.AreEqual("Take the Flag", Runner.BottomObjective().ChosenAction.AIActionName);
        }

        [TestMethod]
        public void NeedsRiseWithPersonalityAndASpeedStripIsOnlyForWhoSawIt()
        {
            PlayerCharacter Quick = new PlayerCharacter(null, null, Vector3.Zero);
            Quick.AIControlled = true;
            Personality Hungry = new Personality();
            Hungry.NeedRate = 2f;
            Quick.ListPersonality.Add(Hungry);
            Quick.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1)));
            Assert.AreEqual(2, Quick.Hunger);
            Assert.AreEqual(2, Quick.Sleep);

            PlayerCharacter Calm = new PlayerCharacter(null, null, Vector3.Zero);
            Calm.AIControlled = true;
            Personality Slow = new Personality();
            Slow.NeedRate = 0f;
            Calm.ListPersonality.Add(Slow);
            Calm.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1)));
            Assert.AreEqual(0, Calm.Hunger);

            PlayerCharacter Boosted = new PlayerCharacter(null, null, Vector3.Zero);
            BelievedObject Strip = new BelievedObject();
            Strip.Name = "Strip";
            Strip.Tag = "Speed Strip";
            Strip.LocalPosition = Vector3.Zero;
            Boosted.Belief.ListObject.Add(Strip);
            PlayerCharacter Plain = new PlayerCharacter(null, null, Vector3.Zero);
            Boosted.Move(Vector3.UnitY, 1, 0.2);
            Plain.Move(Vector3.UnitY, 1, 0.2);
            Assert.IsTrue(Boosted.Position.Y > Plain.Position.Y);

            LifeSimMap Map = NewMap(1, 4);
            Fill(Map, UnitStats.TerrainLandIndex);
            MapInfo Room = new MapInfo(new MapContainer("Road", Map), null, null);
            MatchFacts Match = new MatchFacts();
            Match.Points = Match.MineCost;
            PlayerCharacter Buyer = new PlayerCharacter(null, Room, Center(Map, 0, 0));
            PlaceMineAction Mine = new PlaceMineAction();
            Mine.Match = Match;
            Mine.Place = Center(Map, 0, 2);
            Mine.Init(Buyer.SharedMapContex);
            Assert.IsFalse(FindStep<PlaceDeployableStep>(Mine.GetAIExecutionPlan(null)).Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1))));
            Assert.AreEqual(0, Match.Points);
            Assert.IsNotNull(Buyer.Belief.FindTagged("Mine"));
            PlayerCharacter Other = new PlayerCharacter(null, Room, Center(Map, 0, 0));
            Assert.IsNull(Other.Belief.FindTagged("Mine"));
        }

        static Outpost OutpostAt(string Name, MapInfo Chunk, Vector3 Position, OutpostSide Side, bool CreatureAlive)
        {
            Outpost Outpost = new Outpost();
            Outpost.Name = Name;
            Outpost.Chunk = Chunk;
            Outpost.Position = Position;
            Outpost.Side = Side;
            Outpost.CreatureAlive = CreatureAlive;
            return Outpost;
        }

        static T FindStep<T>(List<AutomatedAction> Steps) where T : AutomatedAction
        {
            for (int i = 0; i < Steps.Count; i++)
            {
                T Found = Steps[i] as T;
                if (Found != null)
                {
                    return Found;
                }
            }

            return null;
        }

        static PathSegment SwimLane()
        {
            PathSegment Lane = new PathSegment();
            Lane.Kind = Traversal.Swim;
            Lane.RequiredPose = Pose.Stand;
            Lane.Surface = NavArea.DeepWater;
            Lane.Polyline.Add(Vector3.Zero);
            Lane.Polyline.Add(new Vector3(0f, 40f, 0f));
            return Lane;
        }

        static PathSegment WallLane()
        {
            PathSegment Lane = new PathSegment();
            Lane.Kind = Traversal.WallRun;
            Lane.RequiredPose = Pose.Climb;
            Lane.Surface = NavArea.Ground;
            Lane.Polyline.Add(Vector3.Zero);
            Lane.Polyline.Add(new Vector3(0f, 40f, 0f));
            return Lane;
        }

        static T Role<T>(PlayerCharacter Owner, MatchFacts Match) where T : LifeSimAIAction, new()
        {
            T Action = new T();
            if (Action is TakeTheFlagAction Take)
            {
                Take.Match = Match;
            }
            else if (Action is GuardOurFlagAction Guard)
            {
                Guard.Match = Match;
            }
            else if (Action is DisruptAction Disrupt)
            {
                Disrupt.Match = Match;
            }
            else if (Action is EscortHolderAction Escort)
            {
                Escort.Match = Match;
            }

            Action.Init(Owner.SharedMapContex);
            return Action;
        }

        static Vector3 EndOf(List<AutomatedAction> Steps)
        {
            FollowPath Path = null;
            for (int i = 0; i < Steps.Count; i++)
            {
                FollowPath Found = Steps[i] as FollowPath;
                if (Found != null)
                {
                    Path = Found;
                }
            }

            PathSegment Last = Path.Segments[Path.Segments.Count - 1];
            return Last.Polyline[Last.Polyline.Count - 1];
        }

        static bool HasKind(FollowPath Path, Traversal Kind)
        {
            if (Path == null)
            {
                return false;
            }

            for (int i = 0; i < Path.Segments.Count; i++)
            {
                if (Path.Segments[i].Kind == Kind)
                {
                    return true;
                }
            }

            return false;
        }

        static bool Crosses(FollowPath Path, Vector3 Center, float Radius)
        {
            float RadiusSq = Radius * Radius;
            for (int s = 0; s < Path.Segments.Count; s++)
            {
                List<Vector3> Line = Path.Segments[s].Polyline;
                for (int i = 0; i + 1 < Line.Count; i++)
                {
                    Vector3 From = Line[i];
                    Vector3 To = Line[i + 1];
                    int Steps = (int)(new Vector3(To.X - From.X, To.Y - From.Y, 0f).Length() / 8f);
                    if (Steps < 1)
                    {
                        Steps = 1;
                    }

                    for (int n = 0; n < Steps; n++)
                    {
                        float T = n / (float)Steps;
                        float DX = From.X + (To.X - From.X) * T - Center.X;
                        float DY = From.Y + (To.Y - From.Y) * T - Center.Y;
                        if (DX * DX + DY * DY <= RadiusSq)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
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
                    SetFloor(Map, X, Y, TerrainIndex);
                }
            }
        }

        static void SetFloor(LifeSimMap Map, int X, int Y, byte TerrainIndex)
        {
            Terrain Tile = Map.LayerManager.ListLayer[0].ArrayTerrain[X, Y];
            Tile.TerrainTypeIndex = TerrainIndex;
            Tile.Height = 0;
            Tile.WorldPosition.Z = 0;
            Tile.MovementCost = 1;
        }

        static Vector3 Center(LifeSimMap Map, int X, int Y)
        {
            return GridNavGeometry.CellCenter(Map, Map.LayerManager.ListLayer[0].ArrayTerrain[X, Y]);
        }
    }
}
