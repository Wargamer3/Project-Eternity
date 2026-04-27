using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.UnitTests.AI
{
    public class GoThroughDoorAction : CharacterAction
    {
        public const string GoThroughDoorGoal = "GoThroughDoor";

        private Door DoorToUse;

        public GoThroughDoorAction(Character Owner, Door DoorToUse)
            : base(GoThroughDoorGoal, Owner)
        {
            this.DoorToUse = DoorToUse;

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
            int CharacterSpeed = Owner.DicExtraStatByName["Speed"];
        }
    }

    public class BreakThroughDoorAction : CharacterAction
    {
        private Door DoorToBreakThrough;

        public BreakThroughDoorAction(Character Owner, Door DoorToBreakThrough)
            : base(MoveAction.MoveGoal, Owner)
        {
            this.DoorToBreakThrough = DoorToBreakThrough;

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
            int CharacterSpeed = Owner.DicExtraStatByName["Speed"];
        }
    }

    public class LockDoorAction : CharacterAction
    {
        private Door DoorToLock;

        public LockDoorAction(Character Owner, Door DoorToLock)
            : base(MoveAction.MoveGoal, Owner)
        {
            this.DoorToLock = DoorToLock;

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
            int CharacterSpeed = Owner.DicExtraStatByName["Speed"];
        }
    }

    public class UnlockDoorAction : CharacterAction
    {
        private Door DoorToUnlock;

        public UnlockDoorAction(Character Owner, Door DoorToUnlock)
            : base(MoveAction.MoveGoal, Owner)
        {
            this.DoorToUnlock = DoorToUnlock;

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
            int CharacterSpeed = Owner.DicExtraStatByName["Speed"];
        }
    }
}
