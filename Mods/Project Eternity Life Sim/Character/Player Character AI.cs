using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public partial class PlayerCharacter
    {
        const string MoveGoal = "Move";
        const string EatGoal = "Eat";
        const string SleepGoal = "Sleep";
        const string PickEvent = "Pick";

        bool SolidStoppedMove;
        int LastObstacleStamp = -1;

        public AIObjective TopObjective()
        {
            if (ListAIObjective.Count == 0)
            {
                return null;
            }

            return ListAIObjective[ListAIObjective.Count - 1];
        }

        public AIObjective BottomObjective()
        {
            if (ListAIObjective.Count == 0)
            {
                return null;
            }

            return ListAIObjective[0];
        }

        public void CommitBottom(AIObjective Objective)
        {
            if (ListAIObjective.Count > 0)
            {
                return;
            }

            ListAIObjective.Add(Objective);
            CurrentGoal = Objective.GoalName;
            RefillSteps(Objective);
            RefreshCombatFlag();
        }

        public void PushShortTerm(AIObjective Objective)
        {
            ListAIObjective.Add(Objective);
            CurrentGoal = Objective.GoalName;
            RefillSteps(Objective);
            RefreshCombatFlag();
        }

        public void PopTopObjective()
        {
            if (ListAIObjective.Count == 0)
            {
                return;
            }

            ListAIObjective.RemoveAt(ListAIObjective.Count - 1);
            RefreshCombatFlag();

            if (ListAIObjective.Count == 0)
            {
                CurrentGoal = null;
                ListCurrentAIObjectiveStep = new List<AutomatedAction>();
                return;
            }

            AIObjective Revealed = ListAIObjective[ListAIObjective.Count - 1];
            CurrentGoal = Revealed.GoalName;
            RefillSteps(Revealed);
        }

        public int EstimateDuration(AIAction Action)
        {
            int Estimate = Action.TimeRequired + DurationAdjust();
            if (Estimate < 0)
            {
                Estimate = 0;
            }

            return Estimate;
        }

        public int BillActionCost(AIAction Action)
        {
            if (Action.AIGoal == MoveGoal)
            {
                return 1;
            }

            return Action.ActionCost;
        }

        void WatchCommittedPlan()
        {
            RefreshTrackedTarget();
            RerouteAroundObstacles();
            EnforceLeash();
            DropUnaffordableObjective();
            PushUrgentObjective();
        }

        void RerouteAroundObstacles()
        {
            bool Hit = SolidStoppedMove;
            SolidStoppedMove = false;

            AIObjective Top = TopObjective();
            FollowPath Path = Top == null || Top.ListRemainingStep.Count == 0
                ? null
                : Top.ListRemainingStep[0] as FollowPath;
            if (Path == null || Path.Segments.Count == 0)
            {
                return;
            }

            MapInfo Here = SharedMapContex == null ? null : SharedMapContex.CurrentMapInfo;
            IList<FurnitureItem> Furniture = Here == null ? null : Here.Furniture;
            IList<BuildingItem> Buildings = Here == null ? null : Here.Buildings;
            if (Here == null || Furniture == null || Buildings == null)
            {
                return;
            }

            bool Crosses = RouteHitsSolid(Path, Furniture, Buildings);
            if (!Crosses && !Hit)
            {
                LastObstacleStamp = -1;
                return;
            }

            int Stamp = StampBoxes(Furniture, Buildings);
            if (Stamp == LastObstacleStamp)
            {
                if (Hit)
                {
                    StopFollowing(Top, Path);
                }

                return;
            }

            Vector3 Goal;
            if (!TryPathGoal(Path, out Goal))
            {
                return;
            }

            bool Hurried = Path.Segments[Path.Segments.Count - 1].Kind == Traversal.Run;
            FollowPath Next = PlanPath(Here, Goal, Hurried);
            LastObstacleStamp = Stamp;
            if (Next.Segments.Count == 0 || RouteHitsSolid(Next, Furniture, Buildings))
            {
                StopFollowing(Top, Path);
                return;
            }

            Path.OnCancel();
            Top.ListRemainingStep[0] = Next;
            ListCurrentAIObjectiveStep = Top.ListRemainingStep;
            Next.OnStarted();
        }

        static void StopFollowing(AIObjective Top, FollowPath Path)
        {
            Path.OnCancel();
            Top.ListRemainingStep.RemoveAt(0);
            if (Top.ListRemainingStep.Count > 0)
            {
                Top.ListRemainingStep[0].OnStarted();
            }
        }

        bool RouteHitsSolid(FollowPath Path, IList<FurnitureItem> Furniture, IList<BuildingItem> Buildings)
        {
            int StartSegment = Path.SegmentIndex;
            if (StartSegment < 0)
            {
                StartSegment = 0;
            }

            int StartPoint = 0;
            float Nearest = float.MaxValue;
            if (StartSegment < Path.Segments.Count && Path.Segments[StartSegment].Polyline != null)
            {
                List<Vector3> Line = Path.Segments[StartSegment].Polyline;
                for (int p = 0; p < Line.Count; p++)
                {
                    float DX = Line[p].X - Position.X;
                    float DY = Line[p].Y - Position.Y;
                    float Dist = DX * DX + DY * DY;
                    if (Dist < Nearest)
                    {
                        Nearest = Dist;
                        StartPoint = p;
                    }
                }
            }

            Vector3 From = Position;
            for (int s = StartSegment; s < Path.Segments.Count; s++)
            {
                List<Vector3> Line = Path.Segments[s].Polyline;
                if (Line == null)
                {
                    continue;
                }

                int First = s == StartSegment ? StartPoint : 0;
                for (int p = First; p < Line.Count; p++)
                {
                    if (SolidBetween(Furniture, Buildings, From, Line[p]))
                    {
                        return true;
                    }

                    From = Line[p];
                }
            }

            return false;
        }

        static bool SolidBetween(IList<FurnitureItem> Furniture, IList<BuildingItem> Buildings, Vector3 From, Vector3 To)
        {
            if (Furniture != null)
            {
                for (int i = 0; i < Furniture.Count; i++)
                {
                    FurnitureItem Box = Furniture[i];
                    if (Box != null && Box.HitsMove(From, To))
                    {
                        return true;
                    }
                }
            }

            if (Buildings != null)
            {
                for (int i = 0; i < Buildings.Count; i++)
                {
                    BuildingItem Box = Buildings[i];
                    if (Box != null && Box.HitsMove(From, To))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        static int StampBoxes(IList<FurnitureItem> Furniture, IList<BuildingItem> Buildings)
        {
            int Hash = 17;
            if (Furniture != null)
            {
                for (int i = 0; i < Furniture.Count; i++)
                {
                    FurnitureItem Box = Furniture[i];
                    if (Box == null || !Box.BlocksBody())
                    {
                        continue;
                    }

                    Hash = Hash * 31 + Box.Position.GetHashCode();
                    Hash = Hash * 31 + Box.Width.GetHashCode();
                    Hash = Hash * 31 + Box.Length.GetHashCode();
                    Hash = Hash * 31 + Box.Angle.GetHashCode();
                }
            }

            if (Buildings != null)
            {
                for (int i = 0; i < Buildings.Count; i++)
                {
                    BuildingItem Box = Buildings[i];
                    if (Box == null || !Box.BlocksBody())
                    {
                        continue;
                    }

                    Hash = Hash * 31 + Box.Position.GetHashCode();
                    Hash = Hash * 31 + Box.Width.GetHashCode();
                    Hash = Hash * 31 + Box.Length.GetHashCode();
                    Hash = Hash * 31 + Box.Angle.GetHashCode();
                }
            }

            return Hash;
        }

        static bool TryPathGoal(FollowPath Path, out Vector3 Goal)
        {
            for (int s = Path.Segments.Count - 1; s >= 0; s--)
            {
                List<Vector3> Line = Path.Segments[s].Polyline;
                if (Line != null && Line.Count > 0)
                {
                    Goal = Line[Line.Count - 1];
                    return true;
                }
            }

            Goal = Vector3.Zero;
            return false;
        }

        void RefreshTrackedTarget()
        {
            AIObjective Top = TopObjective();
            LifeSimAIAction Action = Top == null ? null : Top.ChosenAction as LifeSimAIAction;
            Vector3 Point;
            if (Action == null || !Action.Tracks(out Point))
            {
                return;
            }

            if (Top.ListRemainingStep.Count == 0)
            {
                return;
            }

            FollowPath Path = Top.ListRemainingStep[0] as FollowPath;
            if (Path == null || Path.Segments.Count == 0)
            {
                return;
            }

            PathSegment Last = Path.Segments[Path.Segments.Count - 1];
            if (Last.Polyline.Count == 0)
            {
                return;
            }

            Vector3 End = Last.Polyline[Last.Polyline.Count - 1];
            if (Vector3.Distance(End, Point) > 24f)
            {
                RefillSteps(Top);
            }
        }

        void RefillStandingOrder(AIObjective Top)
        {
            LifeSimAIAction Action = Top.ChosenAction as LifeSimAIAction;
            Vector3 Point;
            if (Action == null || !Action.Tracks(out Point))
            {
                return;
            }

            if (Vector3.Distance(Position, Point) <= NavMeshLimits.AgentRadius * 2f)
            {
                return;
            }

            RefillSteps(Top);
        }

        void AdvanceNeeds(GameTime gameTime)
        {
            float Scale = 1f;
            foreach (Personality ActivePersonality in ListPersonality)
            {
                Scale *= ActivePersonality.NeedRate;
            }

            NeedRemainder += (float)gameTime.ElapsedGameTime.TotalSeconds * Scale;
            int Whole = (int)NeedRemainder;
            if (Whole <= 0)
            {
                return;
            }

            Hunger += Whole;
            Sleep += Whole;
            NeedRemainder -= Whole;
        }

        void EnforceLeash()
        {
            if (ListAIObjective.Count < 2)
            {
                return;
            }

            AIObjective Bottom = ListAIObjective[0];
            if (!Bottom.IsStandingOrder || Bottom.LeashRadius <= 0 || !Bottom.HasPlace)
            {
                return;
            }

            AIObjective Top = ListAIObjective[ListAIObjective.Count - 1];
            if (!Top.IsFight)
            {
                return;
            }

            if (Vector3.Distance(Position, Bottom.Place) > Bottom.LeashRadius)
            {
                PopTopObjective();
            }
        }

        void DropUnaffordableObjective()
        {
            if (!HasAffordableWindow || WaivesRealTimeBudget() || ListAIObjective.Count == 0)
            {
                return;
            }

            AIObjective Top = ListAIObjective[ListAIObjective.Count - 1];
            if (Top.IsStandingOrder || Top.ChosenAction == null)
            {
                return;
            }

            if (EstimateDuration(Top.ChosenAction) > AffordableSeconds)
            {
                PopTopObjective();
            }
        }

        void PushUrgentObjective()
        {
            if (InPractice)
            {
                return;
            }

            AIObjective Top = TopObjective();
            if (Top == null)
            {
                return;
            }

            int Margin = Top.Precision > 0 ? HighPrecisionUrgencyMargin : ShortTermUrgencyMargin;
            List<AIAction> Candidates = new List<AIAction>();
            foreach (AIAction Candidate in AutonomousActions())
            {
                if (!IsOnStack(Candidate))
                {
                    Candidates.Add(Candidate);
                }
            }

            ScoreCandidates(Candidates);
            AIAction Urgent = null;
            foreach (AIAction Candidate in Candidates)
            {
                if (Candidate.AIWeight <= 0 || Candidate.Urgency < Top.Urgency + Margin)
                {
                    continue;
                }

                if (Urgent == null || Candidate.Urgency > Urgent.Urgency)
                {
                    Urgent = Candidate;
                }
            }

            if (Urgent == null)
            {
                return;
            }

            AIObjective ShortTerm = new AIObjective();
            ShortTerm.GoalName = Urgent.AIGoal;
            ShortTerm.ChosenAction = Urgent;
            ShortTerm.Urgency = Urgent.Urgency;
            ShortTerm.Precision = Urgent.Precision;
            ShortTerm.IsFight = Urgent.AIGoal == "Fight";
            PushShortTerm(ShortTerm);
        }

        public void EnterPractice()
        {
            InPractice = true;
            RefreshCombatFlag();
        }

        public void LeavePractice()
        {
            InPractice = false;
            RefreshCombatFlag();
        }

        void TryPickIdleGoal(GameTime gameTime)
        {
            if (InPractice)
            {
                return;
            }

            if (AITimer > 0)
            {
                AITimer -= gameTime.ElapsedGameTime.TotalSeconds;
                if (AITimer > 0)
                {
                    return;
                }

                AITimer = 0;
            }

            List<AIAction> Candidates = AutonomousActions();
            ScoreCandidates(Candidates);

            List<AIAction> ListBest = new List<AIAction>();
            int BestWeight = 0;
            int BestUrgency = 0;

            foreach (AIAction Candidate in Candidates)
            {
                if (Candidate.AIWeight <= 0)
                {
                    continue;
                }

                if (Candidate.AIWeight > BestWeight)
                {
                    ListBest.Clear();
                    ListBest.Add(Candidate);
                    BestWeight = Candidate.AIWeight;
                    BestUrgency = Candidate.Urgency;
                }
                else if (Candidate.AIWeight == BestWeight && Candidate.Urgency > BestUrgency)
                {
                    ListBest.Clear();
                    ListBest.Add(Candidate);
                    BestUrgency = Candidate.Urgency;
                }
                else if (Candidate.AIWeight == BestWeight && Candidate.Urgency == BestUrgency)
                {
                    ListBest.Add(Candidate);
                }
            }

            if (ListBest.Count == 0)
            {
                AITimer = IdleRetrySeconds;
                return;
            }

            AIAction Chosen = ListBest[RandomHelper.Next(ListBest.Count)];
            AIObjective Bottom = new AIObjective();
            Bottom.GoalName = Chosen.AIGoal;
            Bottom.ChosenAction = Chosen;
            Bottom.Urgency = Chosen.Urgency;
            Bottom.Precision = Chosen.Precision;
            CommitBottom(Bottom);

            if (TurnBased)
            {
                ActionPointsRemaining -= BillActionCost(Chosen);
            }
        }

        List<AIAction> AutonomousActions()
        {
            AcceptGrants();

            List<AIAction> ListCandidate = new List<AIAction>();
            foreach (KeyValuePair<string, List<AIAction>> ActiveGoal in DicActionByGoal)
            {
                foreach (AIAction ActiveAction in ActiveGoal.Value)
                {
                    if (!IsForbidden(ActiveAction.AIActionName))
                    {
                        ListCandidate.Add(ActiveAction);
                    }
                }
            }

            foreach (AIAction Granted in ListGrantedAutonomousAction)
            {
                if (!IsForbidden(Granted.AIActionName))
                {
                    ListCandidate.Add(Granted);
                }
            }

            return ListCandidate;
        }

        void AcceptGrants()
        {
            foreach (Personality ActivePersonality in ListPersonality)
            {
                foreach (string GrantedName in ActivePersonality.ListGrantedActionName)
                {
                    if (HasActionNamed(GrantedName))
                    {
                        continue;
                    }

                    AIAction RegistryAction;
                    if (!LifeSimCharacterParams.DicAIAction.TryGetValue(GrantedName, out RegistryAction))
                    {
                        continue;
                    }

                    LifeSimAIAction Granted = RegistryAction.Copy() as LifeSimAIAction;
                    if (Granted == null)
                    {
                        continue;
                    }

                    Granted.Init(SharedMapContex);
                    ListGrantedAutonomousAction.Add(Granted);
                }
            }
        }

        bool HasActionNamed(string ActionName)
        {
            foreach (KeyValuePair<string, List<AIAction>> ActiveGoal in DicActionByGoal)
            {
                foreach (AIAction ActiveAction in ActiveGoal.Value)
                {
                    if (ActiveAction.AIActionName == ActionName)
                    {
                        return true;
                    }
                }
            }

            foreach (AIAction Granted in ListGrantedAutonomousAction)
            {
                if (Granted.AIActionName == ActionName)
                {
                    return true;
                }
            }

            return false;
        }

        public bool Forbids(string ActionName)
        {
            return IsForbidden(ActionName);
        }

        bool IsForbidden(string ActionName)
        {
            foreach (Personality ActivePersonality in ListPersonality)
            {
                foreach (string ForbiddenName in ActivePersonality.ListForbiddenActionName)
                {
                    if (ForbiddenName == ActionName)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        void ScoreCandidates(List<AIAction> Candidates)
        {
            Dictionary<AIAction, int> BaseWeights = new Dictionary<AIAction, int>();
            foreach (AIAction Candidate in Candidates)
            {
                Candidate.UpdatePrecondition(PickEvent, SharedMapContex.RootMapContainer);
                BaseWeights.Add(Candidate, Candidate.AIWeight);
            }

            foreach (AIAction Candidate in Candidates)
            {
                int BaseWeight = BaseWeights[Candidate];
                int Adjusted = BaseWeight + WeightDelta(Candidate.AIActionName);
                if (Adjusted < 0)
                {
                    Adjusted = 0;
                }

                if (Adjusted == 0 && BaseWeight > 0 && IsOnlyCriticalOption(Candidate, Candidates, BaseWeights))
                {
                    Adjusted = 1;
                }

                if (MissesRealTimeWindow(Candidate) || MissesTurnPoints(Candidate))
                {
                    Adjusted = 0;
                }

                Candidate.AIWeight = Adjusted;
            }
        }

        bool IsOnlyCriticalOption(AIAction Action, List<AIAction> Candidates, Dictionary<AIAction, int> BaseWeights)
        {
            if (!IsCriticalGoal(Action.AIGoal))
            {
                return false;
            }

            int Available = 0;
            foreach (AIAction Candidate in Candidates)
            {
                if (Candidate.AIGoal == Action.AIGoal && BaseWeights[Candidate] > 0)
                {
                    Available += 1;
                }
            }

            return Available == 1;
        }

        bool IsCriticalGoal(string GoalName)
        {
            if (GoalName == EatGoal && Hunger >= CriticalHunger)
            {
                return true;
            }

            if (GoalName == SleepGoal && Sleep >= CriticalSleep)
            {
                return true;
            }

            return false;
        }

        int WeightDelta(string ActionName)
        {
            int Delta = 0;
            foreach (Personality ActivePersonality in ListPersonality)
            {
                foreach (PersonalityWeight ActiveWeight in ActivePersonality.ListWeightDelta)
                {
                    if (ActiveWeight.ActionName == ActionName)
                    {
                        Delta += ActiveWeight.Delta;
                    }
                }
            }

            return Delta;
        }

        int DurationAdjust()
        {
            int Adjust = 0;
            foreach (Personality ActivePersonality in ListPersonality)
            {
                Adjust += ActivePersonality.DurationAdjustSeconds;
            }

            return Adjust;
        }

        bool WaivesRealTimeBudget()
        {
            foreach (Personality ActivePersonality in ListPersonality)
            {
                if (ActivePersonality.WaiveRealTimeBudget)
                {
                    return true;
                }
            }

            return false;
        }

        bool MissesRealTimeWindow(AIAction Action)
        {
            if (!HasAffordableWindow || WaivesRealTimeBudget())
            {
                return false;
            }

            return EstimateDuration(Action) > AffordableSeconds;
        }

        bool MissesTurnPoints(AIAction Action)
        {
            if (!TurnBased)
            {
                return false;
            }

            return BillActionCost(Action) > ActionPointsRemaining;
        }

        bool IsOnStack(AIAction Action)
        {
            foreach (AIObjective Level in ListAIObjective)
            {
                if (Level.ChosenAction == Action)
                {
                    return true;
                }
            }

            return false;
        }

        public void GiveOrder(string Goal, List<AutomatedAction> Steps)
        {
            AIObjective Order = new AIObjective();
            Order.GoalName = Goal;
            Order.IsPlayerOrder = true;
            Order.ListRemainingStep = Steps ?? new List<AutomatedAction>();
            int Behind = Goal == MoveGoal ? FirstQueuedMove() : -1;
            if (Behind >= 0)
            {
                ListAIObjective.Insert(Behind, Order);
                return;
            }

            ListAIObjective.Add(Order);
            CurrentGoal = Goal;
            ListCurrentAIObjectiveStep = Order.ListRemainingStep;
            if (Order.ListRemainingStep.Count > 0)
            {
                Order.ListRemainingStep[0].OnStarted();
            }
        }

        void PlanQueuedMove(AIObjective Objective)
        {
            if (Objective.ListRemainingStep.Count == 0)
            {
                return;
            }

            FollowPath Path = Objective.ListRemainingStep[0] as FollowPath;
            if (Path == null || Path.Active != null || Path.SegmentIndex > 0)
            {
                return;
            }

            Vector3 Goal;
            if (!TryPathGoal(Path, out Goal))
            {
                return;
            }

            MapInfo Here = SharedMapContex == null ? null : SharedMapContex.CurrentMapInfo;
            if (Here == null)
            {
                return;
            }

            bool Hurried = Path.Segments[Path.Segments.Count - 1].Kind == Traversal.Run;
            FollowPath Next = PlanPath(Here, Goal, Hurried);
            if (Next.Segments.Count == 0)
            {
                return;
            }

            Objective.ListRemainingStep[0] = Next;
            Next.OnStarted();
        }

        int FirstQueuedMove()
        {
            for (int i = 0; i < ListAIObjective.Count; i++)
            {
                AIObjective Level = ListAIObjective[i];
                if (Level.IsPlayerOrder && Level.GoalName == MoveGoal)
                {
                    return i;
                }
            }

            return -1;
        }

        void RefillSteps(AIObjective Objective)
        {
            if (Objective.IsPlayerOrder || Objective.Yielding)
            {
                if (Objective.IsPlayerOrder && Objective.GoalName == MoveGoal)
                {
                    PlanQueuedMove(Objective);
                }

                ListCurrentAIObjectiveStep = Objective.ListRemainingStep;
                return;
            }

            if (Objective.ChosenAction == null)
            {
                Objective.ListRemainingStep = new List<AutomatedAction>();
                ListCurrentAIObjectiveStep = Objective.ListRemainingStep;
                return;
            }

            List<AutomatedAction> Steps = Objective.ChosenAction.GetAIExecutionPlan(SharedMapContex.RootMapContainer);
            if (Steps == null)
            {
                Steps = new List<AutomatedAction>();
            }

            Objective.ListRemainingStep = Steps;
            ListCurrentAIObjectiveStep = Steps;
            if (Steps.Count > 0)
            {
                Steps[0].OnStarted();
            }
        }

        public FollowPath PlanPath(MapInfo Chunk, Vector3 Goal, bool Hurried)
        {
            RouteStyle Style = RouteStyle.Direct;
            bool PreferRun = Hurried;
            bool PreferLinks = false;
            AIObjective Top = TopObjective();
            if (Top != null && Top.Urgency > 0)
            {
                PreferRun = true;
            }

            foreach (Personality ActivePersonality in ListPersonality)
            {
                if (ActivePersonality.PrefersRun)
                {
                    PreferRun = true;
                }

                if (ActivePersonality.PrefersOptionalLinks)
                {
                    PreferLinks = true;
                }

                if (Top != null && Top.IsFight && ActivePersonality.HasCombatTaste)
                {
                    Style = Personality.RouteFor(ActivePersonality.Taste);
                }
            }

            List<PathSegment> Segments = new List<PathSegment>();
            if (Chunk != null)
            {
                List<NavCut> Cuts = new List<NavCut>();
                List<NavOffMeshLink> Links = new List<NavOffMeshLink>();
                GatherLocalChanges(Cuts, Links);
                Segments = LocalPathQuery.Find(Chunk, Position, Goal, Body, Style, PreferLinks, Cuts, Links, Chunk.Furniture, Chunk.Buildings);
            }

            GroundGait.Apply(Segments, PreferRun, Body);
            return new FollowPath(this, Segments);
        }

        public FollowPath PlanPath(LifeSimMap Map, Vector3 Goal, bool Hurried)
        {
            if (Map == null)
            {
                return PlanPath((MapInfo)null, Goal, Hurried);
            }

            if (Map.Chunk != null && (Map.Chunk.Heights != null || Map.Chunk.Kind == GroundKind.Grid))
            {
                return PlanPath(Map.Chunk, Goal, Hurried);
            }

            MapInfo Stand = new MapInfo(new MapContainer("Grid", Map), null, null);
            Stand.Kind = GroundKind.Grid;
            Stand.Furniture = Map.Furniture;
            Stand.Buildings = Map.Buildings;
            return PlanPath(Stand, Goal, Hurried);
        }

        public void ApplyReaction(AutomatedAction Reaction)
        {
            if (Reaction == null || Reaction.HitStaysInPose)
            {
                return;
            }

            CurrentPose = Reaction.PoseLeft;
            if (Reaction.PoseLeft != Pose.Incapacitated)
            {
                return;
            }

            AIObjective Downed = new AIObjective();
            Downed.GoalName = "Incapacitated";
            Downed.ListRemainingStep.Add(Reaction);
            ListAIObjective.Clear();
            ListAIObjective.Add(Downed);
            ListCurrentAIObjectiveStep = Downed.ListRemainingStep;
            Reaction.OnStarted();
        }

        public ChunkHop NextHop()
        {
            if (ListPlannedHop == null || PlannedHopIndex < 0 || PlannedHopIndex >= ListPlannedHop.Count)
            {
                return null;
            }

            return ListPlannedHop[PlannedHopIndex];
        }

        public void AccountForWhatWasSeen()
        {
            AIObjective Top = TopObjective();
            if (Top == null || Top.ChosenAction == null)
            {
                return;
            }

            ChunkHop Next = NextHop();
            if (Next != null && Belief.RouteLink(Next.FromChunk, Next.ToChunk) == null)
            {
                RefillSteps(Top);
                if (Top.ListRemainingStep.Count == 0)
                {
                    WaitAction Pause = new WaitAction(this, 1);
                    Top.ListRemainingStep.Add(Pause);
                    ListCurrentAIObjectiveStep = Top.ListRemainingStep;
                    Pause.OnStarted();
                }

                return;
            }

            if (Top.ListRemainingStep.Count == 0 || Top.ListRemainingStep[0] is WaitAction)
            {
                return;
            }

            FollowPath Path = Top.ListRemainingStep[0] as FollowPath;
            if (Path == null || Path.Segments.Count == 0 || Path.SegmentIndex >= Path.Segments.Count)
            {
                return;
            }

            PathSegment Segment = Path.Segments[Path.SegmentIndex];
            if (Segment.Polyline.Count == 0)
            {
                return;
            }

            Vector3 Ahead = Segment.Polyline[Segment.Polyline.Count - 1];
            MapInfo Here = SharedMapContex.CurrentMapInfo;
            foreach (BelievedObject Object in Belief.ListObject)
            {
                if (Object.Kind != WorldMarkerKind.Blocker || Object.Chunk != Here)
                {
                    continue;
                }

                if (Vector3.Distance(Ahead, Object.LocalPosition) <= 12f)
                {
                    Top.ListRemainingStep.Insert(0, new WaitAction(this, 1));
                    ListCurrentAIObjectiveStep = Top.ListRemainingStep;
                    return;
                }
            }

            if (ObstacleOnPath(Path) == null)
            {
                return;
            }

            RefillSteps(Top);
            FollowPath Replanned = Top.ListRemainingStep.Count > 0 ? Top.ListRemainingStep[0] as FollowPath : null;
            if (Replanned != null && Replanned.Segments.Count > 0 && ObstacleOnPath(Replanned) == null)
            {
                return;
            }

            if (Top.ListRemainingStep.Count > 0 && Top.ListRemainingStep[0] is WaitAction)
            {
                return;
            }

            WaitAction ObstaclePause = new WaitAction(this, 1);
            Top.ListRemainingStep.Insert(0, ObstaclePause);
            ListCurrentAIObjectiveStep = Top.ListRemainingStep;
            ObstaclePause.OnStarted();
        }

        void GatherLocalChanges(List<NavCut> Cuts, List<NavOffMeshLink> Links)
        {
            MapInfo Here = SharedMapContex.CurrentMapInfo;
            if (Here == null || Belief == null || Body == null)
            {
                return;
            }

            foreach (BelievedObject Object in Belief.ListObject)
            {
                if (Object.Chunk != Here)
                {
                    continue;
                }

                if (Object.Kind == WorldMarkerKind.Obstacle)
                {
                    NavCut Cut = new NavCut();
                    Cut.Center = Object.LocalPosition;
                    Cut.Radius = Object.CutRadius > 0f ? Object.CutRadius : NavCut.DefaultRadius;
                    Cuts.Add(Cut);
                }

                if (Object.HasTraversal && Body.Allows(Object.TraversalKind))
                {
                    Links.Add(new NavOffMeshLink(Object.TraversalKind, Object.LocalPosition, Object.TraversalEnd));
                }
            }
        }

        BelievedObject ObstacleOnPath(FollowPath Path)
        {
            if (Path == null || Belief == null)
            {
                return null;
            }

            for (int i = Path.SegmentIndex; i < Path.Segments.Count; i++)
            {
                BelievedObject Hit = ObstacleOnRoute(Path.Segments[i]);
                if (Hit != null)
                {
                    return Hit;
                }
            }

            return null;
        }

        BelievedObject ObstacleOnRoute(PathSegment Segment)
        {
            if (Segment == null || Segment.Polyline.Count < 2)
            {
                return null;
            }

            MapInfo Here = SharedMapContex.CurrentMapInfo;
            foreach (BelievedObject Object in Belief.ListObject)
            {
                if (Object.Kind != WorldMarkerKind.Obstacle || Object.Chunk != Here)
                {
                    continue;
                }

                if (RouteCrosses(Segment, Object))
                {
                    return Object;
                }
            }

            return null;
        }

        static bool RouteCrosses(PathSegment Segment, BelievedObject Obstacle)
        {
            float Radius = Obstacle.CutRadius > 0f ? Obstacle.CutRadius : NavCut.DefaultRadius;
            float RadiusSq = Radius * Radius;
            int Last = Segment.Polyline.Count - 1;
            for (int i = 0; i < Last; i++)
            {
                if (SamplesHit(Segment.Polyline[i], Segment.Polyline[i + 1], Obstacle.LocalPosition, RadiusSq))
                {
                    return true;
                }
            }

            return false;
        }

        static bool SamplesHit(Vector3 From, Vector3 To, Vector3 Center, float RadiusSq)
        {
            Vector3 Flat = new Vector3(To.X - From.X, To.Y - From.Y, 0f);
            int Steps = (int)(Flat.Length() / 8f);
            if (Steps < 1)
            {
                Steps = 1;
            }

            for (int s = 0; s < Steps; s++)
            {
                float T = s / (float)Steps;
                float DX = From.X + (To.X - From.X) * T - Center.X;
                float DY = From.Y + (To.Y - From.Y) * T - Center.Y;
                if (DX * DX + DY * DY <= RadiusSq)
                {
                    return true;
                }
            }

            return false;
        }

        void RefreshCombatFlag()
        {
            IsInCombat = InPractice;
            foreach (AIObjective Level in ListAIObjective)
            {
                if (Level.IsFight)
                {
                    IsInCombat = true;
                    return;
                }
            }
        }
    }
}
