using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    class TunnelBehaviorSizeManager
    {
        private bool AllowDynamicChanges;
        private bool AllowDynamicSmallChanges;

        private float SizeCurrent;
        private float SizeOld;
        private float SizeNext;
        private double TimeRemainingForTransitionInSeconds;
        private double TimeRemainingBeforeNextChangeInSeconds;

        private float SizeSmallChange;
        private float SizeSmallChangeOld;
        private float SizeSmallChangeNext;
        private double TimeRemainingForSmallChangeTransitionInSeconds;
        private double TimeRemainingBeforeNextSmallChangeInSeconds;

        public static float MinSize = 0.5f;
        public static float MaxSize = 2.5f;
        private float MaxSizeDeviationBetweenChange = 0.8f;
        private float MaxSmallChangeSizeSpeed = 0.05f;

        private const double TimeBeforeNextChangeInSecondsChange = 10;
        private const double TimeBeforeNextChangeInSecondsChangeMin = 5;
        private const double TimeBeforeNextChangeInSecondsSmallChange = 2;
        private const double TimeBeforeNextChangeInSecondsSmallChangeMin = 1;
        private const double TransitionTimeInSeconds = 5;
        private const double TransitionTimeInSecondsSmallChange = 1;

        public float TunnelSizeFinal
        {
            get
            {
                return SizeCurrent;
            }
        }

        public TunnelBehaviorSizeManager()
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
                SizeCurrent = SizeOld
                    + (SizeNext - SizeOld) * (1 - (float)(TimeRemainingForTransitionInSeconds / (TransitionTimeInSeconds * 2)));

                TimeRemainingForTransitionInSeconds -= TimeElapsedInSeconds;
            }
            else
            {
                SizeCurrent = SizeNext;

                if (TimeRemainingBeforeNextChangeInSeconds <= 0)
                {
                    SizeOld = SizeNext;

                    TimeRemainingBeforeNextChangeInSeconds = TimeBeforeNextChangeInSecondsChangeMin + RandomHelper.NextDouble() * TimeBeforeNextChangeInSecondsChange;
                    TimeRemainingForTransitionInSeconds = TransitionTimeInSeconds + RandomHelper.NextDouble() * TransitionTimeInSeconds;

                    SizeNext += -MaxSizeDeviationBetweenChange + (float)RandomHelper.NextDouble() * MaxSizeDeviationBetweenChange * 2;
                    SizeNext = Math.Min(Math.Max(SizeNext, MinSize), MaxSize);
                }
            }
        }

        private void UpdateDynamicSmallChanges(double TimeElapsedInSeconds)
        {
            TimeRemainingBeforeNextSmallChangeInSeconds -= TimeElapsedInSeconds;

            if (TimeRemainingForSmallChangeTransitionInSeconds > 0)
            {
                SizeSmallChange = SizeSmallChangeOld
                    + (SizeSmallChangeNext - SizeSmallChangeOld) * (1 - (float)(TimeRemainingForSmallChangeTransitionInSeconds / (TransitionTimeInSecondsSmallChange * 2)));

                TimeRemainingForSmallChangeTransitionInSeconds -= TimeElapsedInSeconds;
            }
            else
            {
                SizeSmallChange = SizeSmallChangeNext;

                if (TimeRemainingBeforeNextSmallChangeInSeconds <= 0)
                {
                    SizeSmallChangeOld = SizeSmallChangeNext;

                    TimeRemainingBeforeNextSmallChangeInSeconds = TimeBeforeNextChangeInSecondsSmallChangeMin + RandomHelper.NextDouble() * TimeBeforeNextChangeInSecondsSmallChange;
                    TimeRemainingForSmallChangeTransitionInSeconds = TransitionTimeInSecondsSmallChange + RandomHelper.NextDouble() * TransitionTimeInSecondsSmallChange;

                    SizeSmallChangeNext = (float)RandomHelper.NextDouble() * MaxSmallChangeSizeSpeed;
                }
            }

            SizeCurrent += SizeSmallChange;
        }
    }
}
