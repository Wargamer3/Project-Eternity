using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    class TunnelBehaviorSpeedManager
    {
        private bool AllowDynamicChanges;
        private bool AllowDynamicSmallChanges;

        public float ActiveSpeed;
        private float SpeedCurrent;
        private float SpeedOld;
        private float SpeedNext;
        private double TimeRemainingForTransitionInSeconds;
        private double TimeRemainingBeforeNextChangeInSeconds;

        private float SpeedSmallChange;
        private float SpeedSmallChangeOld;
        private float SpeedSmallChangeNext;
        private double TimeRemainingForSmallChangeTransitionInSeconds;
        private double TimeRemainingBeforeNextSmallChangeInSeconds;

        public static float MaxSpeed = 2.5f;
        private float MaxSpeedDeviationBetweenChange = 0.8f;
        private float MaxSmallChangeSpeedSpeed = 0.05f;

        private const double TimeBeforeNextChangeInSecondsChange = 10;
        private const double TimeBeforeNextChangeInSecondsChangeMin = 5;
        private const double TimeBeforeNextChangeInSecondsSmallChange = 2;
        private const double TimeBeforeNextChangeInSecondsSmallChangeMin = 1;
        private const double TransitionTimeInSeconds = 5;
        private const double TransitionTimeInSecondsSmallChange = 1;

        public float TunnelSpeedFinal
        {
            get
            {
                return SpeedCurrent;
            }
        }

        public TunnelBehaviorSpeedManager()
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
                SpeedCurrent = SpeedOld
                    + (SpeedNext - SpeedOld) * (1 - (float)(TimeRemainingForTransitionInSeconds / (TransitionTimeInSeconds * 2)));

                TimeRemainingForTransitionInSeconds -= TimeElapsedInSeconds;
            }
            else
            {
                SpeedCurrent = SpeedNext;

                if (TimeRemainingBeforeNextChangeInSeconds <= 0)
                {
                    SpeedOld = SpeedNext;

                    TimeRemainingBeforeNextChangeInSeconds = TimeBeforeNextChangeInSecondsChangeMin + RandomHelper.NextDouble() * TimeBeforeNextChangeInSecondsChange;
                    TimeRemainingForTransitionInSeconds = TransitionTimeInSeconds + RandomHelper.NextDouble() * TransitionTimeInSeconds;

                    SpeedNext += -MaxSpeedDeviationBetweenChange + (float)RandomHelper.NextDouble() * MaxSpeedDeviationBetweenChange * 2;
                    SpeedNext = Math.Min(Math.Max(SpeedNext, 1), MaxSpeed);
                }
            }
        }

        private void UpdateDynamicSmallChanges(double TimeElapsedInSeconds)
        {
            TimeRemainingBeforeNextSmallChangeInSeconds -= TimeElapsedInSeconds;

            if (TimeRemainingForSmallChangeTransitionInSeconds > 0)
            {
                SpeedSmallChange = SpeedSmallChangeOld
                    + (SpeedSmallChangeNext - SpeedSmallChangeOld) * (1 - (float)(TimeRemainingForSmallChangeTransitionInSeconds / (TransitionTimeInSecondsSmallChange * 2)));

                TimeRemainingForSmallChangeTransitionInSeconds -= TimeElapsedInSeconds;
            }
            else
            {
                SpeedSmallChange = SpeedSmallChangeNext;

                if (TimeRemainingBeforeNextSmallChangeInSeconds <= 0)
                {
                    SpeedSmallChangeOld = SpeedSmallChangeNext;

                    TimeRemainingBeforeNextSmallChangeInSeconds = TimeBeforeNextChangeInSecondsSmallChangeMin + RandomHelper.NextDouble() * TimeBeforeNextChangeInSecondsSmallChange;
                    TimeRemainingForSmallChangeTransitionInSeconds = TransitionTimeInSecondsSmallChange + RandomHelper.NextDouble() * TransitionTimeInSecondsSmallChange;

                    SpeedSmallChangeNext = (float)RandomHelper.NextDouble() * MaxSmallChangeSpeedSpeed;
                }
            }

            SpeedCurrent += SpeedSmallChange;
        }
    }
}
