using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public abstract class MoveAction : LifeSimAIAction
    {
        public const string MoveGoal = "Move";
        public const string MoveDistance = "Distance";

        public MoveAction(PlayerCharacter Owner)
            : base(MoveGoal, Owner)
        {
            UpdatePrecondition(string.Empty, null);
        }
    }

    public class WalkToPositionAction : LifeSimAIAction
    {
        private Vector3 EndPosition;
        private bool IsWalking;

        public WalkToPositionAction(PlayerCharacter Owner)
            : base(MoveAction.MoveGoal, Owner)
        {
            IsWalking = false;
            UpdatePrecondition(string.Empty, null);
        }

        public override bool Execute(GameTime gameTime, NavMapGameManager Map)
        {
            int CharacterSpeed = Owner.DicExtraStatByName["Speed"];
            Vector3 Direction = Vector3.Normalize(EndPosition - Owner.WorldPosition);
            Owner.WorldPosition += Direction;
            return true;
        }

        public override ActionPanelLifeSim GetActionPanel()
        {
            throw new NotImplementedException();
        }

        public override List<AIAction> GetAIExecutionPlan(NavMapGameManager Map)
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
                object ObjectivePosition = Owner.GetObjective(MoveAction.MoveGoal);
                if (ObjectivePosition == null)
                {
                    AIWeight = 0;
                    return;
                }

                IsWalking = true;
                EndPosition = (Vector3)ObjectivePosition;
                int Distance = (int)Owner.GetObjective(MoveAction.MoveDistance);

            }
        }
    }

    public class RunToPositionAction : LifeSimAIAction
    {
        private Vector3 EndPosition;
        private bool IsWalking;

        public RunToPositionAction(PlayerCharacter Owner)
            : base(MoveAction.MoveGoal, Owner)
        {
            UpdatePrecondition(string.Empty, null);
        }

        public override bool Execute(GameTime gameTime, NavMapGameManager Map)
        {
            throw new NotImplementedException();
        }

        public override ActionPanelLifeSim GetActionPanel()
        {
            throw new NotImplementedException();
        }

        public override List<AIAction> GetAIExecutionPlan(NavMapGameManager Map)
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
                object ObjectivePosition = Owner.GetObjective(MoveAction.MoveGoal);
                if (ObjectivePosition == null)
                {
                    AIWeight = 0;
                    return;
                }

                IsWalking = true;
                EndPosition = (Vector3)ObjectivePosition;
                int Distance = (int)Owner.GetObjective(MoveAction.MoveDistance);
                int CharacterSpeed = Owner.DicExtraStatByName["Speed"];

            }
        }
    }
}
