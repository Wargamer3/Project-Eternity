using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.UnitTests
{
    [TestClass]
    public class LifeSimAIActionTests
    {
        [TestMethod]
        public void CopyReturnsASeparateActionWithTheSameNumbers()
        {
            AssertCloneKeepsAuthoredNumbers(new WalkToPositionAction());
            AssertCloneKeepsAuthoredNumbers(new RunToPositionAction());
            AssertCloneKeepsAuthoredNumbers(new TiptoeAction());
            AssertCloneKeepsAuthoredNumbers(new DriveAction());
            AssertCloneKeepsAuthoredNumbers(new TakeTheFlagAction());
            AssertCloneKeepsAuthoredNumbers(new GuardOurFlagAction());
            AssertCloneKeepsAuthoredNumbers(new DisruptAction());
            AssertCloneKeepsAuthoredNumbers(new EscortHolderAction());
            AssertCloneKeepsAuthoredNumbers(new EatJerkyAction());
            AssertCloneKeepsAuthoredNumbers(new GoToRestaurantAction());
            AssertCloneKeepsAuthoredNumbers(new GoThroughDoorAction());
            AssertCloneKeepsAuthoredNumbers(new BreakThroughDoorAction());
            AssertCloneKeepsAuthoredNumbers(new LockDoorAction());
            AssertCloneKeepsAuthoredNumbers(new UnlockDoorAction());
        }

        [TestMethod]
        public void InitAIClonesTheFileActionPerCharacter()
        {
            SharedFileAction FileAction = new SharedFileAction();
            FileAction.AIWeight = 4;

            PlayerCharacter First = CharacterHolding(FileAction);
            PlayerCharacter Second = CharacterHolding(FileAction);

            First.GetActionForGoal(FileAction.AIGoal);
            Second.GetActionForGoal(FileAction.AIGoal);

            Assert.IsFalse(FileAction.FileInstanceWasInited);
            Assert.AreEqual(4, FileAction.AIWeight);
        }

        private static void AssertCloneKeepsAuthoredNumbers(AIAction FileAction)
        {
            FileAction.ActionCost = 2;
            FileAction.Urgency = 3;
            FileAction.TimeRequired = 4;
            FileAction.AIWeight = 5;
            FileAction.AILogicPath = "Logic";

            AIAction Clone = FileAction.Copy();

            Assert.AreNotSame(FileAction, Clone);
            Assert.AreEqual(FileAction.GetType(), Clone.GetType());
            Assert.AreEqual(FileAction.AIGoal, Clone.AIGoal);
            Assert.AreEqual(FileAction.AIActionName, Clone.AIActionName);
            Assert.AreEqual(2, Clone.ActionCost);
            Assert.AreEqual(3, Clone.Urgency);
            Assert.AreEqual(4, Clone.TimeRequired);
            Assert.AreEqual(5, Clone.AIWeight);
            Assert.AreEqual("Logic", Clone.AILogicPath);

            Clone.AIWeight = 9;
            Assert.AreEqual(5, FileAction.AIWeight);
        }

        private static PlayerCharacter CharacterHolding(LifeSimAIAction FileAction)
        {
            PlayerCharacter Character = new PlayerCharacter(null, null, Vector3.Zero);
            Character.DicProficiencyLevelByName = new Dictionary<string, ProficiencyLink>();

            AICharacterAction Group = (AICharacterAction)RuntimeHelpers.GetUninitializedObject(typeof(AICharacterAction));
            Group.ListAIAction = new List<LifeSimAIAction> { FileAction };

            CharacterAction Action = (CharacterAction)RuntimeHelpers.GetUninitializedObject(typeof(CharacterAction));
            Action.ListActionEffect = new List<ActionEffect>();
            Action.ListAIAction = new List<AICharacterAction> { Group };

            Character.ArrayCharacterAction = new CharacterAction[] { Action };
            Character.Init();
            return Character;
        }

        private class SharedFileAction : LifeSimAIAction
        {
            private PlayerCharacter BoundOwner;

            public SharedFileAction()
                : base("Eat", "Shared File")
            {
            }

            public bool FileInstanceWasInited => Params != null;

            public override AIAction Copy()
            {
                SharedFileAction Clone = new SharedFileAction();
                CopyTo(Clone);
                return Clone;
            }

            public override bool Execute(GameTime gameTime, NavMapGameManager Map)
            {
                return true;
            }

            public override List<AutomatedAction> GetAIExecutionPlan(NavMapGameManager Map)
            {
                return new List<AutomatedAction>();
            }

            public override void UpdatePrecondition(string Event, NavMapGameManager Map)
            {
                if (BoundOwner != null && BoundOwner != Params.Owner)
                {
                    throw new InvalidOperationException("Two characters are sharing one AI action.");
                }

                BoundOwner = Params.Owner;
                AIWeight = 1;
            }
        }
    }

    [TestClass]
    public class LifeSimDecisionTests
    {
        [TestMethod]
        public void IdleCharacterPicksOnlyWhileAIControlled()
        {
            PlayerCharacter Character = NewCharacter();
            AddScript(Character, "Eat", "Eat Jerky", 40, 0, 1, 1);

            Character.Update(Frame(1));
            Assert.AreEqual(0, Character.ListAIObjective.Count);

            Character.AIControlled = true;
            Character.Update(Frame(1));
            Assert.AreEqual("Eat Jerky", Character.ListAIObjective[0].ChosenAction.AIActionName);
            Assert.AreEqual(0, Character.ActionPointsRemaining);

            PlayerCharacter Waiting = NewCharacter();
            Waiting.AIControlled = true;
            ScriptedAction Quiet = AddScript(Waiting, "Eat", "Eat Jerky", 0, 0, 1, 1);
            Waiting.Update(Frame(1));
            Assert.AreEqual(0, Waiting.ListAIObjective.Count);

            Quiet.BaseWeight = 40;
            Waiting.Update(Frame(1));
            Assert.AreEqual(0, Waiting.ListAIObjective.Count);
            Waiting.Update(Frame(PlayerCharacter.IdleRetrySeconds));
            Assert.AreEqual(1, Waiting.ListAIObjective.Count);
        }

        [TestMethod]
        public void TakeControlStopsPlanningUntilReleased()
        {
            PlayerCharacter Character = NewCharacter();
            Character.AIControlled = true;
            AddScript(Character, "Eat", "Eat Jerky", 40, 0, 1, 1);
            Character.TakeControl(null);
            Character.Update(Frame(1));
            Assert.IsFalse(Character.AIControlled);
            Assert.AreEqual(0, Character.ListAIObjective.Count);

            Character.RemoveControl();
            Character.Update(Frame(1));
            Assert.IsTrue(Character.AIControlled);
            Assert.AreEqual(1, Character.ListAIObjective.Count);
        }

        [TestMethod]
        public void PoppingAChaseKeepsTheSameBottom()
        {
            PlayerCharacter Character = NewCharacter();
            Character.AIControlled = true;
            ScriptedAction Restaurant = AddScript(Character, "Eat", "Go To Restaurant", 40, 1, 5, 1);
            ScriptedAction Jerky = AddScript(Character, "Eat", "Eat Jerky", 40, 1, 1, 1);

            AIObjective Bottom = new AIObjective();
            Bottom.GoalName = Restaurant.AIGoal;
            Bottom.ChosenAction = Restaurant;
            Bottom.Urgency = 1;
            Character.CommitBottom(Bottom);

            AIObjective Chase = new AIObjective();
            Chase.GoalName = "Fight";
            Chase.ChosenAction = Jerky;
            Chase.IsFight = true;
            Character.PushShortTerm(Chase);
            Assert.IsTrue(Character.IsInCombat);

            Character.PopTopObjective();
            Assert.AreEqual(1, Character.ListAIObjective.Count);
            Assert.AreSame(Restaurant, Character.ListAIObjective[0].ChosenAction);
            Assert.AreEqual("Eat", Character.CurrentGoal);
            Assert.IsFalse(Character.IsInCombat);
        }

        [TestMethod]
        public void LeashPopsTheChaseAndKeepsTheGuard()
        {
            PlayerCharacter Character = NewCharacter();
            Character.AIControlled = true;
            ScriptedAction Guard = AddScript(Character, "Guard", "Guard Post", 10, 0, 1, 1);
            ScriptedAction Chase = AddScript(Character, "Fight", "Chase", 10, 0, 1, 1);

            AIObjective Bottom = new AIObjective();
            Bottom.GoalName = Guard.AIGoal;
            Bottom.ChosenAction = Guard;
            Bottom.IsStandingOrder = true;
            Bottom.HasPlace = true;
            Bottom.Place = Vector3.Zero;
            Bottom.LeashRadius = 10;
            Character.CommitBottom(Bottom);

            AIObjective Fight = new AIObjective();
            Fight.GoalName = Chase.AIGoal;
            Fight.ChosenAction = Chase;
            Fight.IsFight = true;
            Character.PushShortTerm(Fight);

            Character.SetPosition(new Vector3(100, 0, 0));
            Character.Update(Frame(1));

            Assert.AreEqual(1, Character.ListAIObjective.Count);
            Assert.AreSame(Guard, Character.BottomObjective().ChosenAction);
            Assert.IsTrue(Character.BottomObjective().IsStandingOrder);
        }

        [TestMethod]
        public void PersonalityBiasesAndCriticalHungerStillEats()
        {
            PlayerCharacter Outgoing = NewCharacter();
            Outgoing.AIControlled = true;
            AddScript(Outgoing, "Eat", "Eat Jerky", 40, 1, 1, 1);
            AddScript(Outgoing, "Eat", "Go To Restaurant", 40, 1, 1, 1);
            Personality OutgoingBias = new Personality();
            OutgoingBias.ListWeightDelta.Add(new PersonalityWeight { ActionName = "Go To Restaurant", Delta = 10 });
            Outgoing.ListPersonality.Add(OutgoingBias);
            Outgoing.Update(Frame(1));
            Assert.AreEqual("Go To Restaurant", Outgoing.TopObjective().ChosenAction.AIActionName);

            PlayerCharacter Picky = NewCharacter();
            Picky.AIControlled = true;
            Picky.Hunger = 100;
            ScriptedAction OnlyFood = AddScript(Picky, "Eat", "Eat Jerky", 40, 1, 1, 1);
            Personality Hate = new Personality();
            Hate.ListWeightDelta.Add(new PersonalityWeight { ActionName = "Eat Jerky", Delta = -100 });
            Picky.ListPersonality.Add(Hate);
            Picky.Update(Frame(1));
            Assert.AreSame(OnlyFood, Picky.TopObjective().ChosenAction);

            PlayerCharacter Late = NewCharacter();
            Late.AIControlled = true;
            Late.Hunger = 100;
            AddScript(Late, "Eat", "Eat Jerky", 40, 1, 30, 1);
            Late.HasAffordableWindow = true;
            Late.AffordableSeconds = 5;
            Late.ListPersonality.Add(Hate);
            Late.Update(Frame(1));
            Assert.AreEqual(0, Late.ListAIObjective.Count);

            PlayerCharacter Reckless = NewCharacter();
            Reckless.AIControlled = true;
            Reckless.Hunger = 100;
            AddScript(Reckless, "Eat", "Eat Jerky", 40, 1, 30, 1);
            Reckless.HasAffordableWindow = true;
            Reckless.AffordableSeconds = 5;
            Personality Waive = new Personality();
            Waive.WaiveRealTimeBudget = true;
            Waive.ListWeightDelta.Add(new PersonalityWeight { ActionName = "Eat Jerky", Delta = -100 });
            Reckless.ListPersonality.Add(Waive);
            Reckless.Update(Frame(1));
            Assert.AreEqual("Eat Jerky", Reckless.TopObjective().ChosenAction.AIActionName);
        }

        [TestMethod]
        public void ForbidHidesTheScriptWithoutRemovingIt()
        {
            PlayerCharacter Character = NewCharacter();
            Character.AIControlled = true;
            ScriptedAction Party = AddScript(Character, "Social", "Throw Party", 50, 1, 1, 1);
            AddScript(Character, "Eat", "Eat Jerky", 10, 1, 1, 1);
            Personality Loner = new Personality();
            Loner.ListForbiddenActionName.Add("Throw Party");
            Character.ListPersonality.Add(Loner);

            Character.Update(Frame(1));
            Assert.AreEqual("Eat Jerky", Character.TopObjective().ChosenAction.AIActionName);

            List<AutomatedAction> StillThere = Character.GetActionForGoal("Social");
            Assert.AreEqual(1, StillThere.Count);
            Assert.AreSame(Party.ParamsOwner, Character);
        }

        [TestMethod]
        public void TurnSpendsOnePointForAMoveAndRefusesACostlierAction()
        {
            PlayerCharacter Mover = NewCharacter();
            Mover.AIControlled = true;
            Mover.TurnBased = true;
            Mover.ActionPointsRemaining = 1;
            AddScript(Mover, "Move", "Walk To Position", 10, 0, 4, 5);
            Mover.Update(Frame(1));
            Assert.AreEqual(1, Mover.ListAIObjective.Count);
            Assert.AreEqual(0, Mover.ActionPointsRemaining);

            PlayerCharacter Attacker = NewCharacter();
            Attacker.AIControlled = true;
            Attacker.TurnBased = true;
            Attacker.ActionPointsRemaining = 1;
            AddScript(Attacker, "Attack", "Strike", 10, 0, 1, 2);
            Attacker.Update(Frame(1));
            Assert.AreEqual(0, Attacker.ListAIObjective.Count);
            Assert.AreEqual(1, Attacker.ActionPointsRemaining);
        }

        [TestMethod]
        public void WeaponRangeClampsTheFightDistance()
        {
            Assert.AreEqual(2f, Personality.FightDistance(CombatTaste.Close, 2f, 8f));
            Assert.AreEqual(8f, Personality.FightDistance(CombatTaste.Snipe, 2f, 8f));
            Assert.AreEqual(RouteStyle.Covered, Personality.RouteFor(CombatTaste.Hide));
            Assert.AreEqual(RouteStyle.Flank, Personality.RouteFor(CombatTaste.BehindTheirLines));
            Assert.AreEqual(RouteStyle.Direct, Personality.RouteFor(CombatTaste.Close));
        }

        [TestMethod]
        public void GrantAddsAScriptTheCharacterWasNotGiven()
        {
            ScriptedAction Registry = new ScriptedAction("Move", "Drive", 15, 0, 1, 1);
            LifeSimCharacterParams.DicAIAction["Drive"] = Registry;
            try
            {
                PlayerCharacter Character = NewCharacter();
                Character.AIControlled = true;
                Personality Driver = new Personality();
                Driver.ListGrantedActionName.Add("Drive");
                Character.ListPersonality.Add(Driver);
                Character.Update(Frame(1));

                Assert.IsFalse(Registry.WasInited);
                Assert.AreEqual("Drive", Character.TopObjective().ChosenAction.AIActionName);
                Assert.AreNotSame(Registry, Character.TopObjective().ChosenAction);
            }
            finally
            {
                LifeSimCharacterParams.DicAIAction.Remove("Drive");
            }
        }

        static PlayerCharacter NewCharacter()
        {
            return new PlayerCharacter(null, null, Vector3.Zero);
        }

        static ScriptedAction AddScript(PlayerCharacter Character, string Goal, string Name, int Weight, int Urgency, int TimeRequired, int ActionCost)
        {
            ScriptedAction Action = new ScriptedAction(Goal, Name, Weight, Urgency, TimeRequired, ActionCost);
            Action.Init(Character.SharedMapContex);
            Character.AddAction(Action);
            return Action;
        }

        static GameTime Frame(double Seconds)
        {
            return new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(Seconds));
        }

        class ScriptedAction : LifeSimAIAction
        {
            public int BaseWeight;

            public ScriptedAction(string Goal, string Name, int Weight, int Urgency, int TimeRequired, int ActionCost)
                : base(Goal, Name)
            {
                BaseWeight = Weight;
                this.Urgency = Urgency;
                this.TimeRequired = TimeRequired;
                this.ActionCost = ActionCost;
            }

            public bool WasInited => Params != null;
            public PlayerCharacter ParamsOwner => Params == null ? null : Params.Owner;

            public override AIAction Copy()
            {
                ScriptedAction Clone = new ScriptedAction(AIGoal, AIActionName, BaseWeight, Urgency, TimeRequired, ActionCost);
                CopyTo(Clone);
                Clone.BaseWeight = BaseWeight;
                return Clone;
            }

            public override bool Execute(GameTime gameTime, NavMapGameManager Map)
            {
                return true;
            }

            public override List<AutomatedAction> GetAIExecutionPlan(NavMapGameManager Map)
            {
                return new List<AutomatedAction> { new HoldAction(Params.Owner) };
            }

            public override void UpdatePrecondition(string Event, NavMapGameManager Map)
            {
                AIWeight = BaseWeight;
            }
        }

        class HoldAction : AutomatedAction
        {
            public HoldAction(PlayerCharacter Owner)
                : base("Hold", Owner)
            {
            }

            public override void OnStarted()
            {
            }

            public override void OnCancel()
            {
            }

            public override bool Update(GameTime gameTime)
            {
                return true;
            }

            public override void DoRead(ByteReader BR)
            {
            }

            public override void DoWrite(ByteWriter BW)
            {
            }

            protected override AutomatedAction Copy()
            {
                return new HoldAction(Owner);
            }

            public override void Draw(CustomSpriteBatch g)
            {
            }

            public override void DrawIcon(CustomSpriteBatch g, Vector2 Position)
            {
            }
        }
    }

    [TestClass]
    public class LifeSimBeliefTests
    {
        [TestMethod]
        public void SearchReturnsNestedAndNeighbourHopsWithoutTouchingTheMap()
        {
            MapInfo City1 = Area("City1");
            MapInfo City2 = Area("City2");
            MapInfo House = Area("House");
            MapInfo Restaurant = Area("Restaurant");
            City1.AddNestedMap(House);
            City2.AddNestedMap(Restaurant);
            City1.AddNeighbour(City2);
            City2.AddNeighbour(City1);

            ChunkHop HouseDoor = Portal(House, City1);
            ChunkHop Bridge = Portal(City1, City2);
            ChunkHop RestaurantDoor = Portal(City2, Restaurant);
            Remember(City1, 4f, House);
            Remember(City2, 4f, House);
            Remember(House, 4f, City1);
            Remember(Restaurant, 4f, City2);

            PlayerCharacter Walker = CharacterIn(House);
            Walker.Belief.LearnLink(HouseDoor);
            Walker.Belief.LearnLink(Bridge);
            Walker.Belief.LearnLink(RestaurantDoor);

            List<ChunkHop> Hops = Walker.GoToLocation("Restaurant");

            Assert.AreEqual(3, Hops.Count);
            Assert.AreSame(City1, Hops[0].ToChunk);
            Assert.AreSame(City2, Hops[1].ToChunk);
            Assert.AreSame(Restaurant, Hops[2].ToChunk);
            Assert.AreSame(City1, Hops[1].FromChunk);
            Assert.AreSame(City2, Hops[2].FromChunk);
            AssertUnchanged(City1, 4f, House);
            AssertUnchanged(City2, 4f, House);
            AssertUnchanged(House, 4f, City1);
            AssertUnchanged(Restaurant, 4f, City2);
            Assert.IsTrue(City1.HasNeighbour("City2"));
            Assert.IsTrue(City2.HasNested("Restaurant"));
        }

        [TestMethod]
        public void AFallenBridgeStaysOpenForSomeoneWhoHasNotSeenIt()
        {
            MapInfo City1 = Area("City1");
            MapInfo City2 = Area("City2");
            MapInfo House = Area("House");
            MapInfo Restaurant = Area("Restaurant");
            City1.AddNestedMap(House);
            City2.AddNestedMap(Restaurant);
            City1.AddNeighbour(City2);
            City2.AddNeighbour(City1);

            ChunkHop HouseDoor = Portal(House, City1);
            ChunkHop Bridge = Portal(City1, City2);
            ChunkHop RestaurantDoor = Portal(City2, Restaurant);
            WorldMarker Fallen = new WorldMarker();
            Fallen.Name = "North Bridge";
            Fallen.Tag = "Bridge";
            Fallen.Kind = WorldMarkerKind.Bridge;
            Fallen.LocalPosition = new Vector3(30, 0, 0);
            Fallen.IsOpen = false;
            Fallen.Link = Bridge;
            City1.ListMarker.Add(Fallen);

            PlayerCharacter Seen = CharacterIn(House);
            Seen.Belief.LearnLink(HouseDoor);
            Seen.Belief.LearnLink(Bridge);
            Seen.Belief.LearnLink(RestaurantDoor);
            Seen.SetPosition(new Vector3(30, 0, 0));
            Seen.SharedMapContex.CurrentMapInfo = City1;
            Seen.PerceptionRadius = 10;
            Seen.Perceive();

            PlayerCharacter Unseen = CharacterIn(House);
            Unseen.Belief.LearnLink(HouseDoor);
            Unseen.Belief.LearnLink(Bridge);
            Unseen.Belief.LearnLink(RestaurantDoor);

            Assert.AreEqual(0, Seen.GoToLocation("Restaurant").Count);
            Assert.AreEqual(3, Unseen.GoToLocation("Restaurant").Count);
            Assert.IsTrue(Bridge.IsOpen);
            Assert.IsNull(Seen.Belief.OpenLink(City1, City2));
            Assert.IsNotNull(Unseen.Belief.OpenLink(City1, City2));
            Assert.AreEqual(1f, City1.MovementCost);
            Assert.IsNull(City1.ParentTemp);
        }

        [TestMethod]
        public void MapKnowledgeAndSpawnSeedStayOffTheSharedMap()
        {
            MapInfo City = Area("City");
            ChunkHop Gate = Portal(City, Area("Road"));
            PlayerCharacter Character = new PlayerCharacter((NavMapGameManager)null, City, Vector3.Zero);
            Character.CopyLinksFromWorld = true;

            Character.AddMapKnowledge(City);
            Character.SeedBelief();

            Assert.IsNull(Character.SharedMapContex.RootMapContainer);
            Assert.IsTrue(Character.Belief.KnowsArea(City));
            Assert.IsNotNull(Character.Belief.OpenLink(Gate.FromChunk, Gate.ToChunk));
            Character.AddKnowledge(new KnowledgeInfo("Pizza Place", new[] { "Food/Restaurant" }, "not a vector"));
            Assert.IsTrue(Character.HasKnowledgeInCategory("Restaurant"));
            Assert.AreEqual(0, Character.GoToLocation("Pizza Place").Count);
        }

        static MapInfo Area(string Name)
        {
            return new MapInfo(new MapContainer(Name, null), new Vector3[0], new Vector3[0]);
        }

        static ChunkHop Portal(MapInfo From, MapInfo To)
        {
            ChunkHop Hop = new ChunkHop();
            Hop.Name = From.MapMapContainer.AreaName + " to " + To.MapMapContainer.AreaName;
            Hop.FromChunk = From;
            Hop.ToChunk = To;
            Hop.IsOpen = true;
            From.ListLink.Add(Hop);
            return Hop;
        }

        static PlayerCharacter CharacterIn(MapInfo Start)
        {
            return new PlayerCharacter((NavMapGameManager)null, Start, Vector3.Zero);
        }

        static void Remember(MapInfo Chunk, float Cost, MapInfo Parent)
        {
            Chunk.MovementCost = Cost;
            Chunk.ParentTemp = Parent;
        }

        static void AssertUnchanged(MapInfo Chunk, float Cost, MapInfo Parent)
        {
            Assert.AreEqual(Cost, Chunk.MovementCost);
            Assert.AreSame(Parent, Chunk.ParentTemp);
        }
    }
}
