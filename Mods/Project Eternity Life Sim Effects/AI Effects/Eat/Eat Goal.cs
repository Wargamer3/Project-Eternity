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

        public override AIAction Copy()
        {
            EatJerkyAction Clone = new EatJerkyAction();
            CopyTo(Clone);
            return Clone;
        }

        public override bool Execute(GameTime gameTime, NavMapGameManager Map)
        {
            throw new NotImplementedException();
        }

        public override List<AutomatedAction> GetAIExecutionPlan(NavMapGameManager Map)
        {
            if (Params == null)
            {
                return new List<AutomatedAction>();
            }

            return LifePlan.EatJerky(Params.Owner);
        }

        public override void UpdatePrecondition(string Event, NavMapGameManager Map)
        {
            CanEat = Params.Owner.IsCarrying("Jerky") || Params.Owner.Belief.FindTagged("Jerky") != null;

            if (CanEat)
            {
                AIWeight = 40;
                TimeRequired = Params.Owner.IsCarrying("Jerky") ? 1 : 8;
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

        public override AIAction Copy()
        {
            GoToRestaurantAction Clone = new GoToRestaurantAction();
            CopyTo(Clone);
            return Clone;
        }

        public void Execute()
        {
        }

        public override bool Execute(GameTime gameTime, NavMapGameManager Map)
        {
            throw new NotImplementedException();
        }

        public override List<AutomatedAction> GetAIExecutionPlan(NavMapGameManager Map)
        {
            if (Params == null)
            {
                return new List<AutomatedAction>();
            }

            return LifePlan.Restaurant(Params.Owner);
        }

        public override void UpdatePrecondition(string Event, NavMapGameManager Map)
        {
            MapInfo Chunk;
            Vector3 Place;
            CanEat = LifePlan.KnowsRestaurant(Params.Owner, out Chunk, out Place);

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
