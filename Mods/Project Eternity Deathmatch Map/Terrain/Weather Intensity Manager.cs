using System;
using FMOD;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    class WeatherIntensityManager
    {
        private bool AllowDynamicChangesIntensity;
        private bool AllowDynamicSmallChangesIntensity;

        public float Intensity;
        private float OldIntensity;
        private float NextIntensity;
        private double TimeRemainingForTransitionInSecondsIntensity;
        private double TimeRemainingBeforeNextChangeInSecondsIntensity;

        private float SmallChangeIntensity;
        private float SmallChangeOldIntensity;
        private float SmallChangeNextIntensity;
        private double TimeRemainingForTransitionInSecondsSmallChangeIntensity;
        private double TimeRemainingBeforeNextChangeInSecondsSmallChangeIntensity;

        private float MinIntensity = 100;
        public const float MaxIntensity = 700;
        private float MaxDeviationBetweenChangeIntensity = 300;
        private float MaxSmallChangeIntensity = 100;

        private const double TimeBeforeNextChangeInSecondsChange = 20;
        private const double TimeBeforeNextChangeInSecondsChangeMin = 5;
        private const double TimeBeforeNextChangeInSecondsSmallChange = 5;
        private const double TimeBeforeNextChangeInSecondsSmallChangeMin = 3;
        private const double TransitionTimeInSeconds = 4;
        private const double TransitionTimeInSecondsSmallChange = 1;

        private FMODSound RainLight;
        private FMODSound RainRegular;
        private FMODSound RaintMediumHeavy;
        private FMODSound RainHeavy;

        public WeatherIntensityManager(float StartIntensity)
        {
            AllowDynamicChangesIntensity = true;
            AllowDynamicSmallChangesIntensity = true;

            Intensity = StartIntensity;
            OldIntensity = StartIntensity;
            NextIntensity = StartIntensity;

            RainLight = new FMODSound(GameScreen.FMODSystem, "Content/SFX/Weather/Rain/Rain Light.wav");
            RainRegular = new FMODSound(GameScreen.FMODSystem, "Content/SFX/Weather/Rain/Rain Regular.wav");
            RaintMediumHeavy = new FMODSound(GameScreen.FMODSystem, "Content/SFX/Weather/Rain/Rain Medium Heavy.wav");
            RainHeavy = new FMODSound(GameScreen.FMODSystem, "Content/SFX/Weather/Rain/Rain Heavy.wav");

            RainLight.SetLoop(true);
            RainRegular.SetLoop(true);
            RaintMediumHeavy.SetLoop(true);
            RainHeavy.SetLoop(true);

            RainLight.Play();
            RainRegular.Play();
            RaintMediumHeavy.Play();
            RainHeavy.Play();

            RainLight.SetVolume(0f);
            RainRegular.SetVolume(1f);
            RaintMediumHeavy.SetVolume(0f);
            RainHeavy.SetVolume(0f);
        }

        public void Update(GameTime gameTime)
        {
            double TimeElapsedInSeconds = gameTime.ElapsedGameTime.TotalSeconds;

            if (AllowDynamicChangesIntensity)
            {
                UpdateDynamicChangesIntensity(TimeElapsedInSeconds);
            }

            if (AllowDynamicSmallChangesIntensity)
            {
                UpdateDynamicSmallChangesIntensity(TimeElapsedInSeconds);
            }

            UpdateBGM();
        }

        private void UpdateDynamicChangesIntensity(double TimeElapsedInSeconds)
        {
            TimeRemainingBeforeNextChangeInSecondsIntensity -= TimeElapsedInSeconds;

            if (TimeRemainingForTransitionInSecondsIntensity > 0)
            {
                Intensity = OldIntensity
                    + (NextIntensity - OldIntensity) * (1 - (float)(TimeRemainingForTransitionInSecondsIntensity / (TransitionTimeInSeconds * 2)));

                TimeRemainingForTransitionInSecondsIntensity -= TimeElapsedInSeconds;
            }
            else
            {
                Intensity = NextIntensity;

                if (TimeRemainingBeforeNextChangeInSecondsIntensity <= 0)
                {
                    OldIntensity = NextIntensity;

                    TimeRemainingBeforeNextChangeInSecondsIntensity = TimeBeforeNextChangeInSecondsChangeMin + RandomHelper.NextDouble() * TimeBeforeNextChangeInSecondsChange;
                    TimeRemainingForTransitionInSecondsIntensity = TransitionTimeInSeconds + RandomHelper.NextDouble() * TransitionTimeInSeconds;

                    NextIntensity += -MaxDeviationBetweenChangeIntensity + (float)RandomHelper.NextDouble() * MaxDeviationBetweenChangeIntensity * 2;
                    NextIntensity = Math.Min(Math.Max(NextIntensity, MinIntensity), MaxIntensity);
                }
            }
        }

        private void UpdateDynamicSmallChangesIntensity(double TimeElapsedInSeconds)
        {
            TimeRemainingBeforeNextChangeInSecondsSmallChangeIntensity -= TimeElapsedInSeconds;

            if (TimeRemainingForTransitionInSecondsSmallChangeIntensity > 0)
            {
                SmallChangeIntensity = SmallChangeOldIntensity
                    + (SmallChangeNextIntensity - SmallChangeOldIntensity) * (1 - (float)(TimeRemainingForTransitionInSecondsSmallChangeIntensity / (TransitionTimeInSecondsSmallChange * 2)));

                TimeRemainingForTransitionInSecondsSmallChangeIntensity -= TimeElapsedInSeconds;
            }
            else
            {
                SmallChangeIntensity = SmallChangeNextIntensity;

                if (TimeRemainingBeforeNextChangeInSecondsSmallChangeIntensity <= 0)
                {
                    SmallChangeOldIntensity = SmallChangeNextIntensity;

                    TimeRemainingBeforeNextChangeInSecondsSmallChangeIntensity = TimeBeforeNextChangeInSecondsSmallChangeMin + RandomHelper.NextDouble() * TimeBeforeNextChangeInSecondsSmallChange;
                    TimeRemainingForTransitionInSecondsSmallChangeIntensity = TransitionTimeInSecondsSmallChange + RandomHelper.NextDouble() * TransitionTimeInSecondsSmallChange;

                    SmallChangeNextIntensity = -MaxSmallChangeIntensity + (float)RandomHelper.NextDouble() * MaxSmallChangeIntensity * 2;
                }
            }

            Intensity = Math.Max(Intensity + SmallChangeIntensity, MinIntensity);
        }

        private void UpdateBGM()
        {
            if (Intensity < 200)
            {
                RainLight.SetVolume(1f);
                RainRegular.SetVolume(0f);
                RaintMediumHeavy.SetVolume(0f);
                RainHeavy.SetVolume(0f);
            }
            else if (Intensity < 400)
            {
                if (Intensity < 250)//Transition
                {
                    float Progression = (Intensity - 200) / 50f;
                    RainLight.SetVolume(1f - Progression);
                    RainRegular.SetVolume(Progression);
                    RaintMediumHeavy.SetVolume(0f);
                    RainHeavy.SetVolume(0f);
                }
                else
                {
                    RainLight.SetVolume(0f);
                    RainRegular.SetVolume(1f);
                    RaintMediumHeavy.SetVolume(0f);
                    RainHeavy.SetVolume(0f);
                }
            }
            else if (Intensity < 500)
            {
                if (Intensity < 425)//Transition
                {
                    float Progression = (Intensity - 200) / 25;
                    RainLight.SetVolume(0f);
                    RainRegular.SetVolume(1f - Progression);
                    RaintMediumHeavy.SetVolume(Progression);
                    RainHeavy.SetVolume(0f);
                }
                else
                {
                    RainLight.SetVolume(0f);
                    RainRegular.SetVolume(0f);
                    RaintMediumHeavy.SetVolume(1f);
                    RainHeavy.SetVolume(0f);
                }
            }
            else
            {
                if (Intensity < 550)//Transition
                {
                    float Progression = (Intensity - 200) / 50;
                    RainLight.SetVolume(0f);
                    RainRegular.SetVolume(0f);
                    RaintMediumHeavy.SetVolume(1f - Progression);
                    RainHeavy.SetVolume(Progression);
                }
                else
                {
                    RainLight.SetVolume(0f);
                    RainRegular.SetVolume(0f);
                    RaintMediumHeavy.SetVolume(0f);
                    RainHeavy.SetVolume(1f);
                }
            }
        }
    }
}
