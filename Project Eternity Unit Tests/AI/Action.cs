using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.UnitTests.AI
{
    public abstract class Action
    {
        public readonly string Goal;
        public int ActionCost;
        public int Urgency;
        public int TimeRequired;

        public int Weight;

        public Action(string Goal)
        {
            this.Goal = Goal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Map"></param>
        /// <returns>Has Ended</returns>
        public abstract bool Execute(GameTime gameTime, NavMap Map);

        public Action ForceEnd()
        {
            return null;
        }

        public abstract List<Action> GetExecutionPlan(NavMap Map);

        public abstract void UpdatePrecondition(string Event, NavMap Map);//Called when something happen, could be a certain time passed or an interaction
    }

    public abstract class CharacterAction : Action
    {
        public readonly Character Owner;

        public CharacterAction(string Goal, Character Owner)
            : base(Goal)
        {
            this.Owner = Owner;
        }
    }
}
