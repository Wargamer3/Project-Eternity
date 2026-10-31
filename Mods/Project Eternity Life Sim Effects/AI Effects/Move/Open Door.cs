using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class GoThroughDoorAction : LifeSimAIAction
    {
        public const string GoThroughDoorGoal = "GoThroughDoor";
        private const string GoThroughDoorActionName = "Go Through Door";

        public string TargetName;
        public DoorOpenManner Manner = DoorOpenManner.Normally;

        public GoThroughDoorAction()
            : base(GoThroughDoorGoal, GoThroughDoorActionName)
        {
        }

        public override AIAction Copy()
        {
            GoThroughDoorAction Clone = new GoThroughDoorAction();
            CopyTo(Clone);
            Clone.TargetName = TargetName;
            Clone.Manner = Manner;
            return Clone;
        }

        public override bool Execute(GameTime gameTime, NavMapGameManager Map)
        {
            throw new NotImplementedException();
        }

        public override List<AutomatedAction> GetAIExecutionPlan(NavMapGameManager Map)
        {
            return LifePlan.UseDoor(Params.Owner, DoorName(), "Open Door", Manner);
        }

        public override void UpdatePrecondition(string Event, NavMapGameManager Map)
        {
            AIWeight = LifePlan.CanOpen(Params.Owner, DoorName()) ? 1 : 0;
        }

        string DoorName()
        {
            if (!string.IsNullOrEmpty(TargetName))
            {
                return TargetName;
            }

            object Stored = Params.Owner.GetObjective("Door");
            return Stored as string;
        }
    }

    public class BreakThroughDoorAction : LifeSimAIAction
    {
        private const string BreakThroughDoorActionName = "Break Through Door";

        public string TargetName;

        public BreakThroughDoorAction()
            : base(MoveAction.MoveGoal, BreakThroughDoorActionName)
        {
        }

        public override AIAction Copy()
        {
            BreakThroughDoorAction Clone = new BreakThroughDoorAction();
            CopyTo(Clone);
            Clone.TargetName = TargetName;
            return Clone;
        }

        public override bool Execute(GameTime gameTime, NavMapGameManager Map)
        {
            throw new NotImplementedException();
        }

        public override List<AutomatedAction> GetAIExecutionPlan(NavMapGameManager Map)
        {
            return LifePlan.UseDoor(Params.Owner, DoorName(), "Break Door");
        }

        public override void UpdatePrecondition(string Event, NavMapGameManager Map)
        {
            AIWeight = LifePlan.CanBreak(Params.Owner, DoorName()) ? 1 : 0;
        }

        string DoorName()
        {
            if (!string.IsNullOrEmpty(TargetName))
            {
                return TargetName;
            }

            object Stored = Params.Owner.GetObjective("Door");
            return Stored as string;
        }
    }

    public class LockDoorAction : LifeSimAIAction
    {
        private const string LockDoorActionName = "Lock Door";

        public string TargetName;

        public LockDoorAction()
            : base(MoveAction.MoveGoal, LockDoorActionName)
        {
        }

        public override AIAction Copy()
        {
            LockDoorAction Clone = new LockDoorAction();
            CopyTo(Clone);
            Clone.TargetName = TargetName;
            return Clone;
        }

        public override bool Execute(GameTime gameTime, NavMapGameManager Map)
        {
            throw new NotImplementedException();
        }

        public override List<AutomatedAction> GetAIExecutionPlan(NavMapGameManager Map)
        {
            return LifePlan.UseDoor(Params.Owner, DoorName(), "Lock Door");
        }

        public override void UpdatePrecondition(string Event, NavMapGameManager Map)
        {
            AIWeight = LifePlan.CanLock(Params.Owner, DoorName()) ? 1 : 0;
        }

        string DoorName()
        {
            if (!string.IsNullOrEmpty(TargetName))
            {
                return TargetName;
            }

            object Stored = Params.Owner.GetObjective("Door");
            return Stored as string;
        }
    }

    public class UnlockDoorAction : LifeSimAIAction
    {
        private const string UnlockDoorActionName = "Unlock Door";

        public string TargetName;

        public UnlockDoorAction()
            : base(MoveAction.MoveGoal, UnlockDoorActionName)
        {
        }

        public override AIAction Copy()
        {
            UnlockDoorAction Clone = new UnlockDoorAction();
            CopyTo(Clone);
            Clone.TargetName = TargetName;
            return Clone;
        }

        public override bool Execute(GameTime gameTime, NavMapGameManager Map)
        {
            throw new NotImplementedException();
        }

        public override List<AutomatedAction> GetAIExecutionPlan(NavMapGameManager Map)
        {
            return LifePlan.UseDoor(Params.Owner, DoorName(), "Unlock Door");
        }

        public override void UpdatePrecondition(string Event, NavMapGameManager Map)
        {
            AIWeight = LifePlan.CanUnlock(Params.Owner, DoorName()) ? 1 : 0;
        }

        string DoorName()
        {
            if (!string.IsNullOrEmpty(TargetName))
            {
                return TargetName;
            }

            object Stored = Params.Owner.GetObjective("Door");
            return Stored as string;
        }
    }
}
