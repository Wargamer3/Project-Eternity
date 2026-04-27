using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.UnitTests.AI
{
    public abstract class MoveAction : CharacterAction
    {
        public const string MoveGoal = "Move";
        public const string MoveDistance = "Distance";

        public MoveAction(Character Owner)
            : base(MoveGoal, Owner)
        {
            UpdatePrecondition(string.Empty, null);
        }
    }

    public class WalkToPositionAction : CharacterAction
    {
        private Vector3 EndPosition;
        private bool IsWalking;

        public WalkToPositionAction(Character Owner)
            : base(MoveAction.MoveGoal, Owner)
        {
            IsWalking = false;
            UpdatePrecondition(string.Empty, null);
        }

        public override bool Execute(GameTime gameTime, NavMap Map)
        {
            int CharacterSpeed = Owner.DicExtraStatByName["Speed"];
            Vector3 Direction = Vector3.Normalize(EndPosition - Owner.WorldPosition);
            Owner.WorldPosition += Direction;
            return true;
        }

        public override List<Action> GetExecutionPlan(NavMap Map)
        {
            throw new NotImplementedException();
        }

        public override void UpdatePrecondition(string Event, NavMap Map)
        {
            if (Event != string.Empty)
            {
            }
            else if (!IsWalking)
            {
                object ObjectivePosition = Owner.GetObjective(MoveAction.MoveGoal);
                if (ObjectivePosition == null)
                {
                    Weight = 0;
                    return;
                }

                IsWalking = true;
                EndPosition = (Vector3)ObjectivePosition;
                int Distance = (int)Owner.GetObjective(MoveAction.MoveDistance);

            }
        }
    }

    public class RunToPositionAction : CharacterAction
    {
        private Vector3 EndPosition;
        private bool IsWalking;

        public RunToPositionAction(Character Owner)
            : base(MoveAction.MoveGoal, Owner)
        {
            UpdatePrecondition(string.Empty, null);
        }

        public override bool Execute(GameTime gameTime, NavMap Map)
        {
            throw new NotImplementedException();
        }

        public override List<Action> GetExecutionPlan(NavMap Map)
        {
            throw new NotImplementedException();
        }

        public override void UpdatePrecondition(string Event, NavMap Map)
        {
            if (Event != string.Empty)
            {
            }
            else if (!IsWalking)
            {
                object ObjectivePosition = Owner.GetObjective(MoveAction.MoveGoal);
                if (ObjectivePosition == null)
                {
                    Weight = 0;
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
