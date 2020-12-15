using System.Collections.Generic;
using ProjectEternity.Core.AI;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{

    public sealed partial class GameScriptHolder
    {
        public class AimTowardEnemy : TripleThunderScript, ScriptEvaluator
        {
            public AimTowardEnemy()
                : base(100, 50, "Aim Toward Enemy", new string[0], new string[1] { "Enemy" })
            {
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                RobotAnimation TargetRobot = (RobotAnimation)ArrayReferences[0].ReferencedScript.GetContent();
                RobotAnimation CurrentRobot = Info.Owner;

                Vector2 Target = new Vector2(TargetRobot.Position.X - CurrentRobot.Position.X, TargetRobot.Position.Y - CurrentRobot.Position.Y);
                CurrentRobot.UpdateAllWeaponsAngle(Target);

                Result = new List<object>();
                IsCompleted = true;
            }

            public override AIScript CopyScript()
            {
                return new AimTowardEnemy();
            }
        }
    }
}
