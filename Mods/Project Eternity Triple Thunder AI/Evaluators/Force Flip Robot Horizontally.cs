using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using ProjectEternity.Core.AI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{

    public sealed partial class GameScriptHolder
    {
        public class ForceFlipRobotHorizontally : TripleThunderScript, ScriptEvaluator
        {
            private bool _FlipHorizontally;

            public ForceFlipRobotHorizontally()
                : base(150, 50, "Force Flip Robot Horizontally", new string[0], new string[0])
            {
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                if (_FlipHorizontally)
                {
                    Info.Owner.ActiveSpriteEffects = SpriteEffects.FlipHorizontally;
                }
                else
                {
                    Info.Owner.ActiveSpriteEffects = SpriteEffects.None;
                }

                Result = new List<object>();
                IsCompleted = true;
            }

            public override void Load(BinaryReader BR)
            {
                base.Load(BR);

                _FlipHorizontally = BR.ReadBoolean();
            }

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);

                BW.Write(_FlipHorizontally);
            }

            public override AIScript CopyScript()
            {
                return new ForceFlipRobotHorizontally();
            }

            [CategoryAttribute("Script Attributes"),
            DescriptionAttribute("")]
            public bool FlipHorizontally
            {
                get
                {
                    return _FlipHorizontally;
                }
                set
                {
                    _FlipHorizontally = value;
                }
            }
        }
    }
}
