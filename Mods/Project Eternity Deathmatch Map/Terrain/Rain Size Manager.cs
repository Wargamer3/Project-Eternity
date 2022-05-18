using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    class WeatherRainSizeManager//Smaller = more affected by wind, Bigger = fall faster
    {
        private bool AllowDynamicChanges;
        private bool AllowDynamicSmallChanges;

        private float RainSize;
        private float RainSizeOld;
        private float RainSizeNext;
        private double TimeRemainingForTransitionInSeconds;
        private double TimeRemainingBeforeNextChangeInSeconds;

        private float[] ArrayNextRainSizeSmallChange;
        private int NextRainSizeSmallChangeIndex;

        private float RainSizeSmallChange;
        private float RainSizeSmallChangeOld;
        private float RainSizeSmallChangeNext;
        private double TimeRemainingForSmallChangeTransitionInSeconds;
        private double TimeRemainingBeforeNextSmallChangeInSeconds;

        public static float MaxRainSize = 2.5f;
        private float MaxRainSizeDeviationBetweenChange = 0.8f;
        private float MaxSmallChangeRainSizeSpeed = 0.5f;

        private const double TimeBeforeNextChangeInSecondsChange = 30;
        private const double TimeBeforeNextChangeInSecondsChangeMin = 10;
        private const double TimeBeforeNextChangeInSecondsSmallChange = 5;
        private const double TimeBeforeNextChangeInSecondsSmallChangeMin = 1;
        private const double TransitionTimeInSeconds = 2;
        private const double TransitionTimeInSecondsSmallChange = 1;
        public float RainSizeFinal
        {
            get
            {
                if (NextRainSizeSmallChangeIndex >= ArrayNextRainSizeSmallChange.Length) NextRainSizeSmallChangeIndex = 0;
                return RainSize + ArrayNextRainSizeSmallChange[NextRainSizeSmallChangeIndex++];
            }
        }

        public WeatherRainSizeManager()
        {
            AllowDynamicChanges = true;
            AllowDynamicSmallChanges = true;

            NextRainSizeSmallChangeIndex = 0;
            ArrayNextRainSizeSmallChange = new float[100];
            for (int R = 0; R < ArrayNextRainSizeSmallChange.Length; ++R)
            {
                ArrayNextRainSizeSmallChange[R] = (float)RandomHelper.NextDouble() * MaxSmallChangeRainSizeSpeed;
            }
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
                RainSize = RainSizeOld
                    + (RainSizeNext - RainSizeOld) * (1 - (float)(TimeRemainingForTransitionInSeconds / (TransitionTimeInSeconds * 2)));

                TimeRemainingForTransitionInSeconds -= TimeElapsedInSeconds;
            }
            else
            {
                RainSize = RainSizeNext;

                if (TimeRemainingBeforeNextChangeInSeconds <= 0)
                {
                    RainSizeOld = RainSizeNext;

                    TimeRemainingBeforeNextChangeInSeconds = TimeBeforeNextChangeInSecondsChangeMin + RandomHelper.NextDouble() * TimeBeforeNextChangeInSecondsChange;
                    TimeRemainingForTransitionInSeconds = TransitionTimeInSeconds + RandomHelper.NextDouble() * TransitionTimeInSeconds;

                    RainSizeNext += -MaxRainSizeDeviationBetweenChange + (float)RandomHelper.NextDouble() * MaxRainSizeDeviationBetweenChange * 2;
                    RainSizeNext = Math.Min(Math.Max(RainSizeNext, 1), MaxRainSize);
                }
            }
        }

        private void UpdateDynamicSmallChanges(double TimeElapsedInSeconds)
        {
            TimeRemainingBeforeNextSmallChangeInSeconds -= TimeElapsedInSeconds;

            if (TimeRemainingForSmallChangeTransitionInSeconds > 0)
            {
                RainSizeSmallChange = RainSizeSmallChangeOld
                    + (RainSizeSmallChangeNext - RainSizeSmallChangeOld) * (1 - (float)(TimeRemainingForSmallChangeTransitionInSeconds / (TransitionTimeInSecondsSmallChange * 2)));

                TimeRemainingForSmallChangeTransitionInSeconds -= TimeElapsedInSeconds;
            }
            else
            {
                RainSizeSmallChange = RainSizeSmallChangeNext;

                if (TimeRemainingBeforeNextSmallChangeInSeconds <= 0)
                {
                    RainSizeSmallChangeOld = RainSizeSmallChangeNext;

                    TimeRemainingBeforeNextSmallChangeInSeconds = TimeBeforeNextChangeInSecondsSmallChangeMin + RandomHelper.NextDouble() * TimeBeforeNextChangeInSecondsSmallChange;
                    TimeRemainingForSmallChangeTransitionInSeconds = TransitionTimeInSecondsSmallChange + RandomHelper.NextDouble() * TransitionTimeInSecondsSmallChange;

                    RainSizeSmallChangeNext = (float)RandomHelper.NextDouble() * MaxSmallChangeRainSizeSpeed;
                }
            }

            RainSize += RainSizeSmallChange;
        }
    }
}
