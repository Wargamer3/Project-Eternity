using System.Collections.Generic;
using ProjectEternity.Core.AI;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed partial class GameScriptHolder
    {
        public class Shoot : TripleThunderScript, ScriptEvaluator
        {
            public Shoot()
                : base(100, 50, "Shoot", new string[0], new string[0])
            {
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                RobotAnimation CurrentRobot = Info.Owner;

                CurrentRobot.Shoot(CurrentRobot.AnimationOrigin.Position, false);

                Result = new List<object>();
                IsCompleted = true;
            }

            public override AIScript CopyScript()
            {
                return new Shoot();
            }
        }
    }
}
