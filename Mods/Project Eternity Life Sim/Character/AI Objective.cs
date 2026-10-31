using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class AIObjective
    {
        public string GoalName;
        public AIAction ChosenAction;
        public Vector3 Place;
        public bool HasPlace;
        public PlayerCharacter TrackedCharacter;
        public List<AutomatedAction> ListRemainingStep;
        public bool IsStandingOrder;
        public bool IsPlayerOrder;
        public bool Yielding;
        public bool IsFight;
        public float LeashRadius;
        public int Urgency;
        public int Precision;

        public AIObjective()
        {
            GoalName = string.Empty;
            ListRemainingStep = new List<AutomatedAction>();
        }
    }
}
