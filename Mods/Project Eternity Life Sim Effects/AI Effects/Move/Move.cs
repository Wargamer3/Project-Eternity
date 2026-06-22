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
    }

    public class WalkToPositionAction : LifeSimAIAction
    {
        private const string WalkToPositionActionName = "Walk To Position";

        private Vector3 EndPosition;
        private bool IsWalking;

        public WalkToPositionAction()
            : base(MoveAction.MoveGoal, WalkToPositionActionName)
        {
            IsWalking = false;
        }

        public override bool Execute(GameTime gameTime, NavMapGameManager Map)
        {
            int CharacterSpeed = Params.Owner.DicExtraStatByName["Speed"];
            Vector3 Direction = Vector3.Normalize(EndPosition - Params.Owner.Position);
            //Owner.Position += Direction;
            return true;
        }

        public override ActionPanelLifeSimPlayer GetActionPanel()
        {
            throw new NotImplementedException();
        }

        public override List<AutomatedAction> GetAIExecutionPlan(NavMapGameManager Map)
        {
            throw new NotImplementedException();
        }

        public override void UpdatePrecondition(string Event, NavMapGameManager Map)
        {
            if (Event != string.Empty)
            {
            }
            else if (!IsWalking)
            {
                object ObjectivePosition = Params.Owner.GetObjective(MoveAction.MoveGoal);
                if (ObjectivePosition == null)
                {
                    AIWeight = 0;
                    return;
                }

                IsWalking = true;
                EndPosition = (Vector3)ObjectivePosition;
                int Distance = (int)Params.Owner.GetObjective(MoveAction.MoveDistance);

            }
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

        public override bool Execute(GameTime gameTime, NavMapGameManager Map)
        {
            throw new NotImplementedException();
        }

        public override ActionPanelLifeSimPlayer GetActionPanel()
        {
            throw new NotImplementedException();
        }

        public override List<AutomatedAction> GetAIExecutionPlan(NavMapGameManager Map)
        {
            throw new NotImplementedException();
        }

        public override void UpdatePrecondition(string Event, NavMapGameManager Map)
        {
            if (Event != string.Empty)
            {
            }
            else if (!IsWalking)
            {
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
    }
}
