using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.UnitTests.AI
{
    public abstract class EatAction : CharacterAction
    {
        public const string EatGoal = "Eat";

        public EatAction(Character Owner)
            : base(EatGoal, Owner)
        {
            UpdatePrecondition(string.Empty, null);
        }
    }

    public class EatJerkyAction : CharacterAction
    {
        private bool CanEat;

        public EatJerkyAction(Character Owner)
            : base(EatAction.EatGoal, Owner)
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
            CanEat = Owner.HasItemInCategory("Jerky");

            if (CanEat)
            {
                Weight = 40;
                TimeRequired = 1;
            }
            else
            {
                Weight = 0;
            }
        }
    }

    public class GoToRestaurantAction : CharacterAction
    {
        private bool CanEat;

        public GoToRestaurantAction(Character Owner)
            : base(EatAction.EatGoal, Owner)
        {
            UpdatePrecondition(string.Empty, null);
        }

        public void Execute()
        {
            Owner.SetObjective(MoveAction.MoveGoal, new Vector3());
            List<Action> a = Owner.GetActionForGoal(MoveAction.MoveGoal);
        }

        public override bool Execute(GameTime gameTime, NavMap Map)
        {
            throw new NotImplementedException();
        }

        public override List<Action> GetExecutionPlan(NavMap Map)
        {
            var a = Map.FindPath(Owner.CurrentMapName, Owner.WorldPosition, "Restaurant");
            Owner.SetObjective(MoveAction.MoveGoal, new Vector3());
            List<Action> ListMovementGoal = Owner.GetActionForGoal(MoveAction.MoveGoal);
            return ListMovementGoal;
        }

        public override void UpdatePrecondition(string Event, NavMap Map)
        {
            CanEat = Owner.HasKnowledgeInCategory("Restaurant");

            if (CanEat)
            {
                Weight = 40;
                TimeRequired = 10;
            }
            else
            {
                Weight = 0;
            }
        }
    }
}
