using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public abstract class MoveAction : LifeSimAIAction
    {
        public const string MoveGoal = "Move";
        public const string MoveDistance = "Distance";

        public MoveAction()
            : base(MoveGoal, MoveGoal)
        {
            UpdatePrecondition(string.Empty, null);
        }

        public static List<AutomatedAction> Follow(LifeSimCharacterParams Params, Vector3 EndPosition, bool Hurried, float Pace = 1f)
        {
            List<AutomatedAction> Steps = new List<AutomatedAction>();
            if (Params == null || Params.Owner == null)
            {
                return Steps;
            }

            Params.Owner.MovePace = Pace;

            Vector3 Goal = EndPosition;
            object Stored = Params.Owner.GetObjective(MoveGoal);
            if (Stored is Vector3 Place)
            {
                Goal = Place;
            }

            FollowPath Path = Params.Owner.PlanPath(Params.CurrentMapInfo, Goal, Hurried);
            if (Path.Segments.Count == 0)
            {
                return Steps;
            }

            Steps.Add(Path);
            return Steps;
        }
    }

    public class WalkToPositionAction : LifeSimAIAction
    {
        private const string WalkToPositionActionName = "Walk To Position";

        private Vector3 EndPosition;

        public WalkToPositionAction()
            : base(MoveAction.MoveGoal, WalkToPositionActionName)
        {
        }

        public override AIAction Copy()
        {
            WalkToPositionAction Clone = new WalkToPositionAction();
            CopyTo(Clone);
            return Clone;
        }

        public override bool Execute(GameTime gameTime, NavMapGameManager Map)
        {
            int CharacterSpeed = Params.Owner.DicExtraStatByName["Speed"];
            Vector3 Direction = Vector3.Normalize(EndPosition - Params.Owner.Position);
            //Owner.Position += Direction;
            return true;
        }

        public override List<AutomatedAction> GetAIExecutionPlan(NavMapGameManager Map)
        {
            if (!HasDestination())
            {
                return new List<AutomatedAction>();
            }

            return MoveAction.Follow(Params, EndPosition, false);
        }

        public override void UpdatePrecondition(string Event, NavMapGameManager Map)
        {
            object ObjectivePosition = Params == null || Params.Owner == null ? null : Params.Owner.GetObjective(MoveAction.MoveGoal);
            if (!(ObjectivePosition is Vector3))
            {
                AIWeight = 0;
                return;
            }

            EndPosition = (Vector3)ObjectivePosition;
            AIWeight = 1;
        }

        bool HasDestination()
        {
            if (Params == null || Params.Owner == null)
            {
                return false;
            }

            return Params.Owner.GetObjective(MoveAction.MoveGoal) is Vector3;
        }
    }

    public class RunToPositionAction : LifeSimAIAction
    {
        private const string RunToPositionActionName = "Run To Position";

        private Vector3 EndPosition;
        private bool IsWalking;

        public RunToPositionAction()
            : base(MoveAction.MoveGoal, RunToPositionActionName)
        {
        }

        public override AIAction Copy()
        {
            RunToPositionAction Clone = new RunToPositionAction();
            CopyTo(Clone);
            return Clone;
        }

        public override bool Execute(GameTime gameTime, NavMapGameManager Map)
        {
            throw new NotImplementedException();
        }

        public override List<AutomatedAction> GetAIExecutionPlan(NavMapGameManager Map)
        {
            return MoveAction.Follow(Params, EndPosition, true);
        }

        public override void UpdatePrecondition(string Event, NavMapGameManager Map)
        {
            if (IsWalking)
            {
                return;
            }

            object ObjectivePosition = Params.Owner.GetObjective(MoveAction.MoveGoal);
            if (ObjectivePosition == null)
            {
                AIWeight = 0;
                return;
            }

            IsWalking = true;
            EndPosition = (Vector3)ObjectivePosition;
            int Distance = (int)Params.Owner.GetObjective(MoveAction.MoveDistance);
            int CharacterSpeed = Params.Owner.DicExtraStatByName["Speed"];
        }
    }

    public class TiptoeAction : LifeSimAIAction
    {
        public const float Pace = 0.45f;
        private const string TiptoeActionName = "Tiptoe";

        public TiptoeAction()
            : base(MoveAction.MoveGoal, TiptoeActionName)
        {
        }

        public override AIAction Copy()
        {
            TiptoeAction Clone = new TiptoeAction();
            CopyTo(Clone);
            return Clone;
        }

        public override bool Execute(GameTime gameTime, NavMapGameManager Map)
        {
            return true;
        }

        public override List<AutomatedAction> GetAIExecutionPlan(NavMapGameManager Map)
        {
            return MoveAction.Follow(Params, Vector3.Zero, false, Pace);
        }

        public override void UpdatePrecondition(string Event, NavMapGameManager Map)
        {
            if (Params == null || Params.Owner == null || Params.Owner.GetObjective(MoveAction.MoveGoal) == null)
            {
                AIWeight = 0;
                return;
            }

            AIWeight = 1;
        }
    }

    public class DriveAction : LifeSimAIAction
    {
        private const string DriveActionName = "Drive";

        public DriveAction()
            : base(MoveAction.MoveGoal, DriveActionName)
        {
        }

        public override AIAction Copy()
        {
            DriveAction Clone = new DriveAction();
            CopyTo(Clone);
            return Clone;
        }

        public override bool Execute(GameTime gameTime, NavMapGameManager Map)
        {
            return true;
        }

        public override List<AutomatedAction> GetAIExecutionPlan(NavMapGameManager Map)
        {
            if (Params == null || Params.Owner == null)
            {
                return new List<AutomatedAction>();
            }

            Params.Owner.MovePace = 1f;
            object Stored = Params.Owner.GetObjective(MoveAction.MoveGoal);
            Vector3 Goal = Stored is Vector3 Place ? Place : Vector3.Zero;
            return LifePlan.Drive(Params.Owner, Goal);
        }

        public override void UpdatePrecondition(string Event, NavMapGameManager Map)
        {
            if (Params == null || Params.Owner == null || Params.Owner.Belief == null)
            {
                AIWeight = 0;
                return;
            }

            if (Params.Owner.GetObjective(MoveAction.MoveGoal) == null || Params.Owner.Belief.FindTagged("Car") == null)
            {
                AIWeight = 0;
                return;
            }

            AIWeight = 2;
        }
    }
}
