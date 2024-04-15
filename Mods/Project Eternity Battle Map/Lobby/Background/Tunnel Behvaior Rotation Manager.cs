using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    class TunnelBehaviorRotationManager
    {
        private bool AllowDynamicChanges;
        private bool AllowDynamicSmallChanges;

        private float RotationCurrent;
        private float RotationOld;
        private float RotationNext;
        private double TimeRemainingForTransitionInSeconds;
        private double TimeRemainingBeforeNextChangeInSeconds;

        private float RotationSmallChange;
        private float RotationSmallChangeOld;
        private float RotationSmallChangeNext;
        private double TimeRemainingForSmallChangeTransitionInSeconds;
        private double TimeRemainingBeforeNextSmallChangeInSeconds;

        public static float MaxRotation = 360f;
        private float MaxRotationDeviationBetweenChange = 90f;
        private float MaxSmallChangeRotationSpeed = 25f;

        private const double TimeBeforeNextChangeInSecondsChange = 5;
        private const double TimeBeforeNextChangeInSecondsChangeMin = 3;
        private const double TimeBeforeNextChangeInSecondsSmallChange = 2;
        private const double TimeBeforeNextChangeInSecondsSmallChangeMin = 1;
        private const double TransitionTimeInSeconds = 2;
        private const double TransitionTimeInSecondsSmallChange = 1;

        public float TunnelRotationFinal
        {
            get
            {
                return RotationCurrent;
            }
        }

        public TunnelBehaviorRotationManager()
        {
            AllowDynamicChanges = true;
            AllowDynamicSmallChanges = true;
        }

        public void Update(double TimeElapsedInSeconds)
        {
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
                RotationCurrent = RotationOld
                    + (RotationNext - RotationOld) * (1 - (float)(TimeRemainingForTransitionInSeconds / (TransitionTimeInSeconds * 2)));

                TimeRemainingForTransitionInSeconds -= TimeElapsedInSeconds;
            }
            else
            {
                RotationCurrent = RotationNext;

                if (TimeRemainingBeforeNextChangeInSeconds <= 0)
                {
                    RotationOld = RotationNext;

                    TimeRemainingBeforeNextChangeInSeconds = TimeBeforeNextChangeInSecondsChangeMin + RandomHelper.NextDouble() * TimeBeforeNextChangeInSecondsChange;
                    TimeRemainingForTransitionInSeconds = TransitionTimeInSeconds + RandomHelper.NextDouble() * TransitionTimeInSeconds;

                    RotationNext += -MaxRotationDeviationBetweenChange + (float)RandomHelper.NextDouble() * MaxRotationDeviationBetweenChange * 2;
                    if (RotationNext < 0)
                    {
                        RotationNext += MaxRotation;
                    }
                    RotationNext = RotationNext % MaxRotation;
                }
            }
        }

        private void UpdateDynamicSmallChanges(double TimeElapsedInSeconds)
        {
            TimeRemainingBeforeNextSmallChangeInSeconds -= TimeElapsedInSeconds;

            if (TimeRemainingForSmallChangeTransitionInSeconds > 0)
            {
                RotationSmallChange = RotationSmallChangeOld
                    + (RotationSmallChangeNext - RotationSmallChangeOld) * (1 - (float)(TimeRemainingForSmallChangeTransitionInSeconds / (TransitionTimeInSecondsSmallChange * 2)));

                TimeRemainingForSmallChangeTransitionInSeconds -= TimeElapsedInSeconds;
            }
            else
            {
                RotationSmallChange = RotationSmallChangeNext;

                if (TimeRemainingBeforeNextSmallChangeInSeconds <= 0)
                {
                    RotationSmallChangeOld = RotationSmallChangeNext;

                    TimeRemainingBeforeNextSmallChangeInSeconds = TimeBeforeNextChangeInSecondsSmallChangeMin + RandomHelper.NextDouble() * TimeBeforeNextChangeInSecondsSmallChange;
                    TimeRemainingForSmallChangeTransitionInSeconds = TransitionTimeInSecondsSmallChange + RandomHelper.NextDouble() * TransitionTimeInSecondsSmallChange;

                    RotationSmallChangeNext = (float)RandomHelper.NextDouble() * MaxSmallChangeRotationSpeed;
                }
            }

            RotationCurrent += RotationSmallChange;
        }
    }
}
