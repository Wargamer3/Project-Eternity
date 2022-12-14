using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.Core.Scripts
{
    public sealed partial class ScriptingScriptHolder
    {
        public class ScriptCounter : CutsceneActionScript
        {
            private int CounterValue;
            private int _StartingValue;
            private int _IncrementValue;
            private int _EndingValue;

            public ScriptCounter()
                : base(140, 70, "Counter", new string[] { "Increment Counter", "Reset Counter" }, new string[] { "Counter Updated", "Counter Ended" })
            {
                _StartingValue = 0;
                _IncrementValue = 1;
                _EndingValue = 1;
            }

            public override void ExecuteTrigger(int Index)
            {
                switch (Index)
                {
                    case 0:
                        if (CounterValue < _EndingValue)
                        {
                            CounterValue += _IncrementValue;
                            ExecuteEvent(this, 0);
                            if (CounterValue == _EndingValue)
                            {
                                ExecuteEvent(this, 1);
                                IsEnded = true;
                            }
                        }
                        break;

                    case 1:
                        CounterValue = 0;
                        ExecuteEvent(this, 0);
                        break;
                }
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                throw new NotImplementedException();
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                StartingValue = BR.ReadInt32();
                IncrementValue = BR.ReadInt32();
                EndingValue = BR.ReadInt32();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(StartingValue);
                BW.Write(IncrementValue);
                BW.Write(EndingValue);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptCounter();
            }

            #region Properties

            [CategoryAttribute("Counter behavior"),
            DescriptionAttribute("Number at which the counter will start counting from."),
            DefaultValueAttribute(0)]
            public int StartingValue
            {
                get
                {
                    return _StartingValue;
                }
                set
                {
                    _StartingValue = value;
                }
            }

            [CategoryAttribute("Counter behavior"),
            DescriptionAttribute("1."),
            DefaultValueAttribute(1)]
            public int IncrementValue
            {
                get
                {
                    return _IncrementValue;
                }
                set
                {
                    _IncrementValue = value;
                }
            }

            [CategoryAttribute("Counter behavior"),
            DescriptionAttribute("2."),
            DefaultValueAttribute(1)]
            public int EndingValue
            {
                get
                {
                    return _EndingValue;
                }
                set
                {
                    _EndingValue = value;
                }
            }

            #endregion
        }
    }
}
