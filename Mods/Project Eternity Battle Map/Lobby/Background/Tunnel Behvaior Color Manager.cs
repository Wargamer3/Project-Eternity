using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    class TunnelBehaviorColorManager
    {
        private bool AllowDynamicChanges;
        private bool AllowDynamicSmallChanges;

        private float HueCurrent;
        private float HueOld;
        private float HueNext;
        private double TimeRemainingForTransitionInSeconds;
        private double TimeRemainingBeforeNextChangeInSeconds;

        private float HueSmallChange;
        private float HueSmallChangeOld;
        private float HueSmallChangeNext;
        private double TimeRemainingForSmallChangeTransitionInSeconds;
        private double TimeRemainingBeforeNextSmallChangeInSeconds;

        public static float MaxHue = 360;
        private float MaxHueDeviationBetweenChange = 90f;
        private float MaxSmallChangeHueSpeed = 10f;

        private const double TimeBeforeNextChangeInSecondsChange = 10;
        private const double TimeBeforeNextChangeInSecondsChangeMin = 5;
        private const double TimeBeforeNextChangeInSecondsSmallChange = 2;
        private const double TimeBeforeNextChangeInSecondsSmallChangeMin = 1;
        private const double TransitionTimeInSeconds = 5;
        private const double TransitionTimeInSecondsSmallChange = 1;

        public float TunnelHueFinal
        {
            get
            {
                return HueCurrent;
            }
        }

        public TunnelBehaviorColorManager()
        {
            AllowDynamicChanges = true;
            AllowDynamicSmallChanges = true;
        }

        public void Update(GameTime gameTime)
        {
            double TimeElapsedInSeconds = gameTime.ElapsedGameTime.TotalSeconds;

            if (AllowDynamicChanges)
            {
                UpdateDynamicChanges(TimeElapsedInSeconds);
            }

            if (AllowDynamicSmallChanges)
            {
                UpdateDynamicSmallChanges(TimeElapsedInSeconds);
            }
        }

        private void UpdateDynamicChanges(double TimeElapsedInSeconds)
        {
            TimeRemainingBeforeNextChangeInSeconds -= TimeElapsedInSeconds;

            if (TimeRemainingForTransitionInSeconds > 0)
            {
                HueCurrent = HueOld
                    + (HueNext - HueOld) * (1 - (float)(TimeRemainingForTransitionInSeconds / (TransitionTimeInSeconds * 2)));

                TimeRemainingForTransitionInSeconds -= TimeElapsedInSeconds;
            }
            else
            {
                HueCurrent = HueNext;

                if (TimeRemainingBeforeNextChangeInSeconds <= 0)
                {
                    HueOld = HueNext;

                    TimeRemainingBeforeNextChangeInSeconds = TimeBeforeNextChangeInSecondsChangeMin + RandomHelper.NextDouble() * TimeBeforeNextChangeInSecondsChange;
                    TimeRemainingForTransitionInSeconds = TransitionTimeInSeconds + RandomHelper.NextDouble() * TransitionTimeInSeconds;

                    HueNext += -MaxHueDeviationBetweenChange + (float)RandomHelper.NextDouble() * MaxHueDeviationBetweenChange * 2;
                    if (HueNext < 0)
                    {
                        HueNext += MaxHue;
                    }
                    HueNext = Math.Abs(HueNext) % MaxHue;
                }
            }
        }

        private void UpdateDynamicSmallChanges(double TimeElapsedInSeconds)
        {
            TimeRemainingBeforeNextSmallChangeInSeconds -= TimeElapsedInSeconds;

            if (TimeRemainingForSmallChangeTransitionInSeconds > 0)
            {
                HueSmallChange = HueSmallChangeOld
                    + (HueSmallChangeNext - HueSmallChangeOld) * (1 - (float)(TimeRemainingForSmallChangeTransitionInSeconds / (TransitionTimeInSecondsSmallChange * 2)));

                TimeRemainingForSmallChangeTransitionInSeconds -= TimeElapsedInSeconds;
            }
            else
            {
                HueSmallChange = HueSmallChangeNext;

                if (TimeRemainingBeforeNextSmallChangeInSeconds <= 0)
                {
                    HueSmallChangeOld = HueSmallChangeNext;

                    TimeRemainingBeforeNextSmallChangeInSeconds = TimeBeforeNextChangeInSecondsSmallChangeMin + RandomHelper.NextDouble() * TimeBeforeNextChangeInSecondsSmallChange;
                    TimeRemainingForSmallChangeTransitionInSeconds = TransitionTimeInSecondsSmallChange + RandomHelper.NextDouble() * TransitionTimeInSecondsSmallChange;

                    HueSmallChangeNext = (float)RandomHelper.NextDouble() * MaxSmallChangeHueSpeed;
                }
            }

            HueCurrent += HueSmallChange;
        }
    }
}
