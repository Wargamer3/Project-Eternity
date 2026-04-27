using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public abstract class EatAction : LifeSimAIAction
    {
        public const string EatGoal = "Eat";

        public EatAction(PlayerCharacter Owner)
            : base(EatGoal, Owner)
        {
            UpdatePrecondition(string.Empty, null);
        }
    }

    public class EatJerkyAction : LifeSimAIAction
    {
        private bool CanEat;

        public EatJerkyAction(PlayerCharacter Owner)
            : base(EatAction.EatGoal, Owner)
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
            CanEat = Owner.HasItemInCategory("Jerky");

            if (CanEat)
            {
                AIWeight = 40;
                TimeRequired = 1;
            }
            else
            {
                AIWeight = 0;
            }
        }
    }

    public class GoToRestaurantAction : LifeSimAIAction
    {
        private bool CanEat;

        public GoToRestaurantAction(PlayerCharacter Owner)
            : base(EatAction.EatGoal, Owner)
        {
            UpdatePrecondition(string.Empty, null);
        }

        public void Execute()
        {
            Owner.SetObjective(MoveAction.MoveGoal, new Vector3());
            List<AIAction> a = Owner.GetActionForGoal(MoveAction.MoveGoal);
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
            var a = Map.FindPath(Owner.CurrentMapName, Owner.WorldPosition, "Restaurant");
            Owner.SetObjective(MoveAction.MoveGoal, new Vector3());
            List<AIAction> ListMovementGoal = Owner.GetActionForGoal(MoveAction.MoveGoal);
            return ListMovementGoal;
        }

        public override void UpdatePrecondition(string Event, NavMapGameManager Map)
        {
            CanEat = Owner.HasKnowledgeInCategory("Restaurant");

            if (CanEat)
            {
                AIWeight = 40;
                TimeRequired = 10;
            }
            else
            {
                AIWeight = 0;
            }
        }
    }
}
