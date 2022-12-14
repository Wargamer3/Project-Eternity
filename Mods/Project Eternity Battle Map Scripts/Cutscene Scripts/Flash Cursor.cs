using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{

    public sealed partial class ExtraBattleMapCutsceneScriptHolder
    {
        public class ScriptFlashCursor : BattleMapScript
        {
            private double _EndingValue;
            private double _TimeVisible;
            private double _TimeInvisible;

            private double TimerValueTotal;
            private double TimerValueVisible;
            private bool IsCursorVisible;

            public ScriptFlashCursor()
                : this(null)
            {
            }

            public ScriptFlashCursor(BattleMap Map)
                : base(Map, 100, 50, "Flash Cursor", new string[] { "Flash cursor" }, new string[] { "Flash ended" })
            {
                _EndingValue = 2;
                _TimeVisible = 0.5f;
                _TimeInvisible = 0.5f;
                IsEnded = false;
            }

            public override void ExecuteTrigger(int Index)
            {
                if (!IsActive && !IsEnded)
                {
                    IsActive = true;
                    IsEnded = false;
                }
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                if (!IsEnded)
                {
                    //Timer Update Trigger.
                    TimerValueTotal += gameTime.ElapsedGameTime.TotalSeconds;
                    TimerValueVisible += gameTime.ElapsedGameTime.TotalSeconds;

                    if (IsCursorVisible)
                    {
                        Map.CursorPositionVisible = Map.CursorPosition;
                        if (TimerValueVisible >= _TimeVisible)
                        {
                            TimerValueVisible = 0;
                            IsCursorVisible = false;
                        }
                    }
                    else
                    {
                        Map.CursorPositionVisible = new Microsoft.Xna.Framework.Vector3(-10, -10, 0);
                        if (TimerValueVisible >= _TimeInvisible)
                        {
                            TimerValueVisible = 0;
                            IsCursorVisible = true;
                        }
                    }

                    if (TimerValueTotal >= _EndingValue)
                    {
                        IsEnded = true;
                        //Timer Ended Trigger.
                        ExecuteEvent(this, 0);
                    }
                }
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                EndingValue = BR.ReadDouble();
                TimeVisible = BR.ReadDouble();
                TimeInvisible = BR.ReadDouble();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(EndingValue);
                BW.Write(TimeVisible);
                BW.Write(TimeInvisible);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptFlashCursor(Map);
            }

            #region Properties

            [CategoryAttribute("Timer behavior"),
            DescriptionAttribute("Number of seconds the effect will last."),
            DefaultValueAttribute(1)]
            public double EndingValue
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

            [CategoryAttribute("Timer behavior"),
            DescriptionAttribute("Number of seconds the cursor will stay visible"),
            DefaultValueAttribute(1)]
            public double TimeVisible
            {
                get
                {
                    return _TimeVisible;
                }
                set
                {
                    _TimeVisible = value;
                }
            }

            [CategoryAttribute("Timer behavior"),
            DescriptionAttribute("Number of seconds the cursor will stay invisible"),
            DefaultValueAttribute(1)]
            public double TimeInvisible
            {
                get
                {
                    return _TimeInvisible;
                }
                set
                {
                    _TimeInvisible = value;
                }
            }

            #endregion
        }
    }
}
