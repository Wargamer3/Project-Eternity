using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{

    public sealed partial class ExtraBattleMapCutsceneScriptHolder
    {
        public class ScriptShakeScreen : BattleMapScript
        {
            #region Timer

            public enum TimerTypes { Miliseconds, Seconds, Minutes };

            private int _EndingValue;
            private TimeSpan TimerValue;
            private TimerTypes _TimerType;

            #endregion

            private float _ShakingRadius;
            private bool IsTimerEnded;

            public ScriptShakeScreen()
                : this(null)
            {
                _EndingValue = 5;
                _TimerType = TimerTypes.Seconds;

                _ShakingRadius = 3;

                IsEnded = false;
            }

            public ScriptShakeScreen(BattleMap Map)
                : base(Map, 100, 50, "Shake Screen", new string[] { "Start" }, new string[] { "Shake Ended" })
            {
                _EndingValue = 5;
                _TimerType = TimerTypes.Seconds;

                _ShakingRadius = 3;

                IsEnded = false;
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
                Map.IsShaking = true;
                Map.IsShakingEnded = false;
                Map.ShakeRadiusMax = _ShakingRadius;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                if (!IsTimerEnded)
                {
                    TimerValue += gameTime.ElapsedGameTime;

                    switch (_TimerType)
                    {
                        case TimerTypes.Miliseconds:
                            if (TimerValue.Milliseconds >= _EndingValue)
                                IsTimerEnded = true;
                            break;

                        case TimerTypes.Seconds:
                            if (TimerValue.Seconds >= _EndingValue)
                                IsTimerEnded = true;
                            break;

                        case TimerTypes.Minutes:
                            if (TimerValue.Minutes >= _EndingValue)
                                IsTimerEnded = true;
                            break;
                    }
                }
                else if (IsTimerEnded && Map.IsShaking)
                {
                    Map.IsShakingEnded = true;
                }
                else if (!Map.IsShaking)
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
                ShakingRadius = BR.ReadSingle();
                TimerType = (ScriptShakeScreen.TimerTypes)BR.ReadInt32();
                EndingValue = BR.ReadInt32();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(ShakingRadius);
                BW.Write((int)TimerType);
                BW.Write(EndingValue);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptShakeScreen(Map);
            }

            #region Properties

            [CategoryAttribute("Shake attributes"),
            DescriptionAttribute("Amount of pixels the screen will move."),
            DefaultValueAttribute(3)]
            public float ShakingRadius
            {
                get
                {
                    return _ShakingRadius;
                }
                set
                {
                    _ShakingRadius = value;
                }
            }

            [CategoryAttribute("Spawn Delay"),
            DescriptionAttribute("Frequency at which the Timer will updates."),
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

            [CategoryAttribute("Spawn Delay"),
            DescriptionAttribute("Number at which the Unit will spawn."),
            DefaultValueAttribute(5)]
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
