using System;
using System.IO;
using System.ComponentModel;

namespace ProjectEternity.Core.Scripts
{
    public sealed partial class ScriptingScriptHolder
    {
        public class ScriptTimer : CutsceneActionScript
        {
            public enum TimerTypes { Miliseconds, Seconds, Minutes };

            private int _EndingValue;
            private TimeSpan TimerValue;
            private TimerTypes _TimerType;

            public ScriptTimer()
                : base(150, 80, "Timer", new string[] { "Start Timer", "Reset Timer", "Pause Timer", "Stop and Reset" }, new string[] { "Timer Updated", "Timer Ended", "Timer Reset" })
            {
                _EndingValue = 1;
                _TimerType = TimerTypes.Seconds;
                IsEnded = false;
            }

            public override void ExecuteTrigger(int Index)
            {
                switch (Index)
                {
                    case 0://Start Timer
                        if (!IsActive && !IsEnded)
                        {
                            IsActive = true;
                            IsEnded = false;
                        }
                        break;

                    case 1://Reset Timer
                        TimerValue = new TimeSpan();
                        IsEnded = false;
                        //Timer Reset Trigger.
                        ExecuteEvent(this, 2);
                        break;

                    case 2://Stop Timer
                        IsActive = false;
                        break;
                }
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                if (!IsEnded)
                {
                    //Timer Update Trigger.
                    ExecuteEvent(this, 0);
                    TimerValue += gameTime.ElapsedGameTime;

                    bool IsTimerEnded = false;

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
                    if (IsTimerEnded)
                    {
                        IsEnded = true;
                        //Timer Ended Trigger.
                        ExecuteEvent(this, 1);
                    }
                }
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                TimerType = (TimerTypes)BR.ReadInt32();
                EndingValue = BR.ReadInt32();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write((int)TimerType);
                BW.Write(EndingValue);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptTimer();
            }

            #region Properties

            [CategoryAttribute("Timer behavior"),
            DescriptionAttribute("Frequency at which the Timer will updates."),
            DefaultValueAttribute(TimerTypes.Seconds)]
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

            [CategoryAttribute("Timer behavior"),
            DescriptionAttribute("Number at which the Timer will stop."),
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
