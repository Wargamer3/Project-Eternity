using System.Collections.Generic;
using ProjectEternity.Core.AI;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{

    public sealed partial class GameScriptHolder
    {
        public class KillSelf : TripleThunderScript, ScriptEvaluator
        {
            public KillSelf()
                : base(100, 50, "Kill Self", new string[0], new string[0])
            {
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                RobotAnimation CurrentRobot = Info.Owner;

                Info.OwnerLayer.RemoveRobot(CurrentRobot);

                Result = new List<object>();
                IsCompleted = true;
            }

            public override AIScript CopyScript()
            {
                return new KillSelf();
            }
        }
    }
}
