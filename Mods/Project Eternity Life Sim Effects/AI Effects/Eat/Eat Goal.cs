using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public abstract class EatAction : LifeSimAIAction
    {
        public const string EatGoal = "Eat";
        private const string EatActionName = "Eat";

        public EatAction()
            : base(EatGoal, EatActionName)
        {
        }
    }

    public class EatJerkyAction : LifeSimAIAction
    {
        private const string EatJerkyActionName = "Eat Jerky";

        private bool CanEat;

        public EatJerkyAction()
            : base(EatAction.EatGoal, EatJerkyActionName)
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
            CanEat = Params.Owner.HasItemInCategory("Jerky");

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
        private const string GoToRestaurantActionName = "Go To Restaurant";

        private bool CanEat;

        public GoToRestaurantAction()
            : base(EatAction.EatGoal, GoToRestaurantActionName)
        {
        }

        public void Execute()
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
            var a = Map.FindPath(Params.CurrentMapInfo, Params.Owner.Position, "Restaurant");
            Params.Owner.SetObjective(MoveAction.MoveGoal, new Vector3());
            List<AutomatedAction> ListMovementGoal = Params.Owner.GetActionForGoal(MoveAction.MoveGoal);
            return ListMovementGoal;
        }

        public override void UpdatePrecondition(string Event, NavMapGameManager Map)
        {
            CanEat = Params.Owner.HasKnowledgeInCategory("Restaurant");

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
