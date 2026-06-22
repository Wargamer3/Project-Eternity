using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class GoThroughDoorAction : LifeSimAIAction
    {
        public const string GoThroughDoorGoal = "GoThroughDoor";
        private const string GoThroughDoorActionName = "Go Through Door";

        private Door DoorToUse;

        public GoThroughDoorAction()
            : base(GoThroughDoorGoal, GoThroughDoorActionName)
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
            int CharacterSpeed = Params.Owner.DicExtraStatByName["Speed"];
        }
    }

    public class BreakThroughDoorAction : LifeSimAIAction
    {
        private const string BreakThroughDoorActionName = "Break Through Door";

        private Door DoorToBreakThrough;

        public BreakThroughDoorAction()
            : base(MoveAction.MoveGoal, BreakThroughDoorActionName)
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
            int CharacterSpeed = Params.Owner.DicExtraStatByName["Speed"];
        }
    }

    public class LockDoorAction : LifeSimAIAction
    {
        private const string LockDoorActionName = "Lock Door";

        private Door DoorToLock;

        public LockDoorAction()
            : base(MoveAction.MoveGoal, LockDoorActionName)
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
            int CharacterSpeed = Params.Owner.DicExtraStatByName["Speed"];
        }
    }

    public class UnlockDoorAction : LifeSimAIAction
    {
        private const string UnlockDoorActionName = "Unlock Door";

        private Door DoorToUnlock;

        public UnlockDoorAction()
            : base(MoveAction.MoveGoal, UnlockDoorActionName)
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
            int CharacterSpeed = Params.Owner.DicExtraStatByName["Speed"];
        }
    }
}
