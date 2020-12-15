using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using ProjectEternity.Core.AI;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{

    public sealed partial class GameScriptHolder
    {
        public class Walk : TripleThunderScript, ScriptEvaluator
        {
            private MovementInputs _MovementInput;

            public Walk()
                : base(100, 50, "Walk", new string[0], new string[0])
            {
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                RobotAnimation CurrentRobot = Info.Owner;
                
                CurrentRobot.Move(_MovementInput);

                Result = new List<object>();
                IsCompleted = true;
            }

            public override void Load(BinaryReader BR)
            {
                base.Load(BR);
                _MovementInput = (MovementInputs)BR.ReadByte();
            }

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);
                BW.Write((byte)_MovementInput);
            }

            public override AIScript CopyScript()
            {
                return new Walk();
            }

            [CategoryAttribute("Script Attributes"),
            DescriptionAttribute("")]
            public MovementInputs Movement
            {
                get
                {
                    return _MovementInput;
                }
                set
                {
                    _MovementInput = value;
                }
            }
        }
    }
}
