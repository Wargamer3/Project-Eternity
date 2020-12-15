using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.AI;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.AI.DeathmatchMapScreen
{
    public sealed partial class DeathmatchScriptHolder
    {
        public class EndSquadTurn : DeathmatchScript, ScriptEvaluator
        {
            public EndSquadTurn()
                : base(150, 50, "End Squad Turn", new string[0], new string[0])
            {
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                Info.Map.FinalizeMovement(Info.ActiveSquad, 1);
                Info.ActiveSquad.EndTurn();

                Result = new List<object>();
                IsCompleted = true;
            }

            public override AIScript CopyScript()
            {
                return new EndSquadTurn();
            }
        }
    }
}
