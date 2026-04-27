using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class GoThroughDoorAction : LifeSimAIAction
    {
        public const string GoThroughDoorGoal = "GoThroughDoor";

        private Door DoorToUse;

        public GoThroughDoorAction(PlayerCharacter Owner, Door DoorToUse)
            : base(GoThroughDoorGoal, Owner)
        {
            this.DoorToUse = DoorToUse;

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
            int CharacterSpeed = Owner.DicExtraStatByName["Speed"];
        }
    }

    public class BreakThroughDoorAction : LifeSimAIAction
    {
        private Door DoorToBreakThrough;

        public BreakThroughDoorAction(PlayerCharacter Owner, Door DoorToBreakThrough)
            : base(MoveAction.MoveGoal, Owner)
        {
            this.DoorToBreakThrough = DoorToBreakThrough;

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
            int CharacterSpeed = Owner.DicExtraStatByName["Speed"];
        }
    }

    public class LockDoorAction : LifeSimAIAction
    {
        private Door DoorToLock;

        public LockDoorAction(PlayerCharacter Owner, Door DoorToLock)
            : base(MoveAction.MoveGoal, Owner)
        {
            this.DoorToLock = DoorToLock;

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
            int CharacterSpeed = Owner.DicExtraStatByName["Speed"];
        }
    }

    public class UnlockDoorAction : LifeSimAIAction
    {
        private Door DoorToUnlock;

        public UnlockDoorAction(PlayerCharacter Owner, Door DoorToUnlock)
            : base(MoveAction.MoveGoal, Owner)
        {
            this.DoorToUnlock = DoorToUnlock;

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
            int CharacterSpeed = Owner.DicExtraStatByName["Speed"];
        }
    }
}
