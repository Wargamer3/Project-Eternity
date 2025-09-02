using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.AI;
using ProjectEternity.GameScreens.ConquestMapScreen;

namespace ProjectEternity.AI.ConquestMapScreen
{
    public sealed partial class ConquestScriptHolder
    {
        public class AIEndSquadTurn : ConquestScript, ScriptEvaluator
        {
            public AIEndSquadTurn()
                : base(150, 50, "End Squad Turn Conquest", new string[0], new string[0])
            {
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                Info.Map.FinalizeMovement(Info.ActiveUnit, 1, new List<Vector3>());
                Info.ActiveUnit.EndTurn();

                Result = new List<object>();
                IsCompleted = true;
            }

            public override AIScript CopyScript()
            {
                return new AIEndSquadTurn();
            }
        }
    }
}
