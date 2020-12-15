using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using ProjectEternity.Core.AI;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{

    public sealed partial class GameScriptHolder
    {
        public class SetSpeed : TripleThunderScript, ScriptEvaluator
        {
            private Vector2 _Speed;

            public SetSpeed()
                : base(100, 50, "Set Speed", new string[0], new string[0])
            {
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                RobotAnimation CurrentRobot = Info.Owner;

                CurrentRobot.Speed = _Speed;

                Result = new List<object>();
                IsCompleted = true;
            }

            public override void Load(BinaryReader BR)
            {
                base.Load(BR);
                _Speed = new Vector2(BR.ReadSingle(), BR.ReadSingle());
            }

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);
                BW.Write(_Speed.X);
                BW.Write(_Speed.Y);
            }

            public override AIScript CopyScript()
            {
                return new SetSpeed();
            }

            [CategoryAttribute("Script Attributes"),
            DescriptionAttribute("")]
            public Vector2 Speed
            {
                get
                {
                    return _Speed;
                }
                set
                {
                    _Speed = value;
                }
            }
        }
    }
}
