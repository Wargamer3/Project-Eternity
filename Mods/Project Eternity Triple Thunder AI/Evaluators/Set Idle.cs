using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.AI;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{

    public sealed partial class GameScriptHolder
    {
        public class SetIdle : TripleThunderScript, ScriptEvaluator
        {
            public SetIdle()
                : base(100, 50, "Set Idle", new string[0], new string[0])
            {
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                RobotAnimation CurrentRobot = Info.Owner;

                CurrentRobot.SetIdle();

                Result = new List<object>();
                IsCompleted = true;
            }

            public override void Load(BinaryReader BR)
            {
                base.Load(BR);
            }

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);
            }

            public override AIScript CopyScript()
            {
                return new SetIdle();
            }
        }
    }
}
