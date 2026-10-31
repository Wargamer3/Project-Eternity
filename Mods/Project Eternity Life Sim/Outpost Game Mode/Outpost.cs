using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class ClassBonus
    {
        public const float ExtraSpeed = 1.25f;
        public const float FasterSwim = 1.6f;
        public const float FasterWallRun = 1.35f;

        public static void ApplyExtraSpeed(PlayerCharacter Character)
        {
            if (Character == null)
            {
                return;
            }

            Character.MovePace = ExtraSpeed;
        }

        public static void ApplyExtraJump(PlayerCharacter Character)
        {
            if (Character == null || Character.Body == null)
            {
                return;
            }

            Character.Body.CanJump = true;
        }

        public static void ApplyFasterSwim(PlayerCharacter Character)
        {
            if (Character == null || Character.Body == null)
            {
                return;
            }

            Character.Body.CanSwim = true;
            Character.SwimPace = FasterSwim;
        }

        public static void ApplyFasterWallRun(PlayerCharacter Character)
        {
            if (Character == null || Character.Body == null)
            {
                return;
            }

            Character.Body.CanWallRun = true;
            Character.WallRunPace = FasterWallRun;
        }
    }

    static class DeployablePlan
    {
        public static List<AutomatedAction> Build(PlayerCharacter Owner, MatchFacts Match, Vector3 Place, string Tag, WorldMarkerKind Kind, int Cost, float CutRadius, bool HasTraversal = false, Traversal TraversalKind = Traversal.JumpPad, Vector3 TraversalEnd = default)
        {
            if (Owner == null || Match == null || Match.Points < Cost)
            {
                return new List<AutomatedAction>();
            }

            List<AutomatedAction> Steps = LifePlan.GoTo(Owner, Owner.SharedMapContex.CurrentMapInfo, Place);
            Steps.Add(new PlaceDeployableStep(Owner, Match, Owner.SharedMapContex.CurrentMapInfo, Place, Tag, Kind, Cost, CutRadius, HasTraversal, TraversalKind, TraversalEnd));
            return StanceSequence.Splice(Owner.CurrentPose, Steps);
        }
    }

    public abstract class SpendAction : LifeSimAIAction
    {
        public MatchFacts Match;
        public Vector3 Place;
        protected readonly string TagName;

        protected SpendAction(string Name, string TagName)
            : base(Name, Name)
        {
            this.TagName = TagName;
        }

        protected abstract int CostOf(MatchFacts Match);
        protected abstract WorldMarkerKind MarkerKind { get; }
        protected virtual float Radius { get { return 0f; } }

        public override AIAction Copy()
        {
            SpendAction Clone = (SpendAction)Activator.CreateInstance(GetType());
            CopyTo(Clone);
            Clone.Match = Match;
            Clone.Place = Place;
            return Clone;
        }

        public override bool Execute(GameTime gameTime, NavMapGameManager Map)
        {
            return true;
        }

        public override List<AutomatedAction> GetAIExecutionPlan(NavMapGameManager Map)
        {
            if (!CanAfford())
            {
                return new List<AutomatedAction>();
            }

            return DeployablePlan.Build(Params.Owner, Match, Place, TagName, MarkerKind, CostOf(Match), Radius);
        }

        public override void UpdatePrecondition(string Event, NavMapGameManager Map)
        {
            AIWeight = CanAfford() && MatchRole.Accepts(Params.Owner, AIActionName) ? 1 : 0;
        }

        protected bool CanAfford()
        {
            return Params != null && Params.Owner != null && Match != null && Match.Points >= CostOf(Match);
        }
    }
}
