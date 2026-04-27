using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.UnitTests.AI
{
    public class Pathfinder
    {
        private NavMap Map;

        public List<Action> FindPath(Character ActiveCharacter, Vector3 Start, Vector3 End)
        {
            List<Action> ListTravelAction = new List<Action>();

            int Distance = 100;
            List<Action> ListMovementAction = ActiveCharacter.GetActionForGoal(MoveAction.MoveGoal);
            ActiveCharacter.SetObjective(MoveAction.MoveDistance, Distance);
            ActiveCharacter.SetObjective(MoveAction.MoveGoal, End);

            foreach (Action ActiveMovement in ListMovementAction)
            {
                ActiveMovement.UpdatePrecondition(string.Empty, Map);
            }

            return ListTravelAction;
        }
    }
}
