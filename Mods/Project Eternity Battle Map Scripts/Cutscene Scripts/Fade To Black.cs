using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{

    public sealed partial class ExtraBattleMapCutsceneScriptHolder
    {
        public class ScriptFadeToBlack : BattleMapScript
        {
            #region Timer

            public enum TimerTypes { Miliseconds, Seconds, Minutes };

            private int _EndingValue;
            private TimeSpan TimerValue;
            private TimerTypes _TimerType;

            #endregion

            private float FadeIncrementValue;
            private bool IsTimerEnded;

            public ScriptFadeToBlack()
                : this(null)
            {
                _EndingValue = 1;
                _TimerType = TimerTypes.Seconds;

                IsEnded = false;
            }

            public ScriptFadeToBlack(BattleMap Map)
                : base(Map, 100, 50, "Fade To Black", new string[] { "Start" }, new string[] { "Fade Ended" })
            {
                _EndingValue = 1;
                _TimerType = TimerTypes.Seconds;

                IsEnded = false;
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
                Map.FadeIsActive = true;
                Map.FadeAlpha = 0;

                if (_EndingValue == 0)
                {
                    FadeIncrementValue = 255;
                    Map.FadeAlpha = 255;
                }
                else if (_EndingValue > 0)
                {
                    FadeIncrementValue = 255 / (float)_EndingValue;
                    if (_TimerType == TimerTypes.Seconds)
                        FadeIncrementValue /= 1000f;
                    if (_TimerType == TimerTypes.Minutes)
                        FadeIncrementValue /= 60f;
                }
                else
                {
                    FadeIncrementValue = 255 / (float)_EndingValue;
                    if (_TimerType == TimerTypes.Seconds)
                        FadeIncrementValue /= 1000f;
                    if (_TimerType == TimerTypes.Minutes)
                        FadeIncrementValue /= 60f;
                }
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                if (!IsTimerEnded)
                {
                    TimerValue += gameTime.ElapsedGameTime;
                    if (_EndingValue > 0)
                    {
                        Map.FadeAlpha = Math.Min(255, (float)TimerValue.TotalMilliseconds * FadeIncrementValue);
                    }
                    else
                    {
                        Map.FadeAlpha = Math.Max(0, 255 + (float)TimerValue.TotalMilliseconds * FadeIncrementValue);
                    }

                    switch (_TimerType)
                    {
                        case TimerTypes.Miliseconds:
                            if (TimerValue.Milliseconds >= Math.Abs(_EndingValue))
                                IsTimerEnded = true;
                            break;

                        case TimerTypes.Seconds:
                            if (TimerValue.Seconds >= Math.Abs(_EndingValue))
                                IsTimerEnded = true;
                            break;

                        case TimerTypes.Minutes:
                            if (TimerValue.Minutes >= Math.Abs(_EndingValue))
                                IsTimerEnded = true;
                            break;
                    }
                }
                else
                {
                    ExecuteEvent(this, 0);
                    IsEnded = true;
                }
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                TimerType = (ScriptFadeToBlack.TimerTypes)BR.ReadInt32();
                TimeToFadeToBlack = BR.ReadInt32();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write((int)TimerType);
                BW.Write(TimeToFadeToBlack);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptFadeToBlack(Map);
            }

            #region Properties

            [CategoryAttribute("Fade To Black"),
            DescriptionAttribute(""),
            DefaultValueAttribute(0)]
            public TimerTypes TimerType
            {
                get
                {
                    return _TimerType;
                }
                set
                {
                    _TimerType = value;
                }
            }

            [CategoryAttribute("Fade To Black"),
            DescriptionAttribute(""),
            DefaultValueAttribute(5)]
            public int TimeToFadeToBlack
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
