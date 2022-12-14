using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.Core.Scripts
{
    public sealed partial class ScriptingScriptHolder
    {
        public class ScriptLoop : CutsceneActionScript
        {
            private int _NumberOfLoop;
            private int _CurrentLoop;

            public ScriptLoop()
                : base(140, 70, "Loop", new string[0], new string[] { "Loop incremented", "Loop ended" })
            {
                _CurrentLoop = 0;
                _CurrentLoop = 10;
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                if (_CurrentLoop < _NumberOfLoop)
                    ExecuteEvent(this, 0);
                _CurrentLoop++;
                if (_CurrentLoop == _NumberOfLoop)
                {
                    ExecuteEvent(this, 1);
                    IsEnded = true;
                }
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                NumberOfLoop = BR.ReadInt32();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(NumberOfLoop);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptLoop();
            }

            #region Properties

            [CategoryAttribute("Loop behavior"),
            DescriptionAttribute("The number of time the loop is repeated"),
            DefaultValueAttribute(10)]
            public int NumberOfLoop
            {
                get
                {
                    return _NumberOfLoop;
                }
                set
                {
                    _NumberOfLoop = value;
                }
            }

            #endregion
        }
    }
}
