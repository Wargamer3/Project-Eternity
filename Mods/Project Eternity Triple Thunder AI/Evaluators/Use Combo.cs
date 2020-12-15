using System.Collections.Generic;
using ProjectEternity.Core.AI;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{

    public sealed partial class GameScriptHolder
    {
        public class UseCombo : TripleThunderScript, ScriptEvaluator
        {
            public UseCombo()
                : base(100, 50, "Use Combo", new string[0], new string[0])
            {
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                RobotAnimation CurrentRobot = Info.Owner;

                CurrentRobot.ActiveAttackStance = "Walking";
                CurrentRobot.UseCombo(gameTime, AttackInputs.LightPress);

                Result = new List<object>();
                IsCompleted = true;
            }

            public override AIScript CopyScript()
            {
                return new UseCombo();
            }
        }
    }
}
