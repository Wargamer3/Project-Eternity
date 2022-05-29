using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    class WeatherWindSpeedManager
    {
        private bool AllowDynamicChanges;
        private bool AllowDynamicSmallChanges;

        public float WindSpeed;
        private float WindSpeedOld;
        private float WindSpeedNext;
        private double TimeRemainingForTransitionInSeconds;
        private double TimeRemainingBeforeNextChangeInSeconds;

        private float WindSpeedSmallChange;
        private float WindSpeedSmallChangeOld;
        private float WindSpeedSmallChangeNext;
        private double TimeRemainingForSmallChangeTransitionInSeconds;
        private double TimeRemainingBeforeNextSmallChangeInSeconds;

        public const float MaxWindSpeed = 15;
        private const float MaxWindSpeedDeviationBetweenChange = 10;
        private const float MaxSmallChangeWindSpeed = 2f;

        private const double TimeBeforeNextChangeInSecondsChange = 30;
        private const double TimeBeforeNextChangeInSecondsChangeMin = 10;
        private const double TimeBeforeNextChangeInSecondsSmallChange = 5;
        private const double TimeBeforeNextChangeInSecondsSmallChangeMin = 1;
        private const double TransitionTimeInSeconds = 2;
        private const double TransitionTimeInSecondsSmallChange = 1;

        public WeatherWindSpeedManager()
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
                WindSpeed = WindSpeedOld
                    + (WindSpeedNext - WindSpeedOld) * (1 - (float)(TimeRemainingForTransitionInSeconds / (TransitionTimeInSeconds * 2)));

                TimeRemainingForTransitionInSeconds -= TimeElapsedInSeconds;
            }
            else
            {
                WindSpeed = WindSpeedNext;

                if (TimeRemainingBeforeNextChangeInSeconds <= 0)
                {
                    WindSpeedOld = WindSpeedNext;

                    TimeRemainingBeforeNextChangeInSeconds = TimeBeforeNextChangeInSecondsChangeMin + RandomHelper.NextDouble() * TimeBeforeNextChangeInSecondsChange;
                    TimeRemainingForTransitionInSeconds = TransitionTimeInSeconds + RandomHelper.NextDouble() * TransitionTimeInSeconds;

                    WindSpeedNext += -MaxWindSpeedDeviationBetweenChange + (float)RandomHelper.NextDouble() * MaxWindSpeedDeviationBetweenChange * 2;
                    WindSpeedNext = Math.Min(Math.Max(WindSpeedNext, -MaxWindSpeed), MaxWindSpeed);
                }
            }
        }

        private void UpdateDynamicSmallChanges(double TimeElapsedInSeconds)
        {
            TimeRemainingBeforeNextSmallChangeInSeconds -= TimeElapsedInSeconds;

            if (TimeRemainingForSmallChangeTransitionInSeconds > 0)
            {
                WindSpeedSmallChange = WindSpeedSmallChangeOld
                    + (WindSpeedSmallChangeNext - WindSpeedSmallChangeOld) * (1 - (float)(TimeRemainingForSmallChangeTransitionInSeconds / (TransitionTimeInSecondsSmallChange * 2)));

                TimeRemainingForSmallChangeTransitionInSeconds -= TimeElapsedInSeconds;
            }
            else
            {
                WindSpeedSmallChange = WindSpeedSmallChangeNext;

                if (TimeRemainingBeforeNextSmallChangeInSeconds <= 0)
                {
                    WindSpeedSmallChangeOld = WindSpeedSmallChangeNext;

                    TimeRemainingBeforeNextSmallChangeInSeconds = TimeBeforeNextChangeInSecondsSmallChangeMin + RandomHelper.NextDouble() * TimeBeforeNextChangeInSecondsSmallChange;
                    TimeRemainingForSmallChangeTransitionInSeconds = TransitionTimeInSecondsSmallChange + RandomHelper.NextDouble() * TransitionTimeInSecondsSmallChange;

                    WindSpeedSmallChangeNext = -MaxSmallChangeWindSpeed + (float)RandomHelper.NextDouble() * MaxSmallChangeWindSpeed * 2;
                }
            }

            WindSpeed += WindSpeedSmallChange;
        }
    }
}
