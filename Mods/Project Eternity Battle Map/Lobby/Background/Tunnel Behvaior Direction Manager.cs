using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    class TunnelBehaviorDirectionManager
    {
        private bool AllowDynamicChanges;
        private bool AllowDynamicSmallChanges;

        public float ActiveDirection;
        private float DirectionCurrent;
        private float DirectionOld;
        private float DirectionNext;
        private double TimeRemainingForTransitionInSeconds;
        private double TimeRemainingBeforeNextChangeInSeconds;

        private float DirectionSmallChange;
        private float DirectionSmallChangeOld;
        private float DirectionSmallChangeNext;
        private double TimeRemainingForSmallChangeTransitionInSeconds;
        private double TimeRemainingBeforeNextSmallChangeInSeconds;

        public static float MaxDirection = 360f;
        private float MaxDirectionDeviationBetweenChange = 20f;
        private float MaxSmallChangeDirectionDirection = 5f;

        private const double TimeBeforeNextChangeInSecondsChange = 10;
        private const double TimeBeforeNextChangeInSecondsChangeMin = 5;
        private const double TimeBeforeNextChangeInSecondsSmallChange = 2;
        private const double TimeBeforeNextChangeInSecondsSmallChangeMin = 1;
        private const double TransitionTimeInSeconds = 5;
        private const double TransitionTimeInSecondsSmallChange = 1;

        public float TunnelDirectionFinal
        {
            get
            {
                return DirectionCurrent;
            }
        }

        public TunnelBehaviorDirectionManager()
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
                DirectionCurrent = DirectionOld
                    + (DirectionNext - DirectionOld) * (1 - (float)(TimeRemainingForTransitionInSeconds / (TransitionTimeInSeconds * 2)));

                TimeRemainingForTransitionInSeconds -= TimeElapsedInSeconds;
            }
            else
            {
                DirectionCurrent = DirectionNext;

                if (TimeRemainingBeforeNextChangeInSeconds <= 0)
                {
                    DirectionOld = DirectionNext;

                    TimeRemainingBeforeNextChangeInSeconds = TimeBeforeNextChangeInSecondsChangeMin + RandomHelper.NextDouble() * TimeBeforeNextChangeInSecondsChange;
                    TimeRemainingForTransitionInSeconds = TransitionTimeInSeconds + RandomHelper.NextDouble() * TransitionTimeInSeconds;

                    DirectionNext += -MaxDirectionDeviationBetweenChange + (float)RandomHelper.NextDouble() * MaxDirectionDeviationBetweenChange * 2;
                    if (DirectionNext < 0)
                    {
                        DirectionNext += MaxDirection;
                    }
                    DirectionNext = DirectionNext % MaxDirection;
                }
            }
        }

        private void UpdateDynamicSmallChanges(double TimeElapsedInSeconds)
        {
            TimeRemainingBeforeNextSmallChangeInSeconds -= TimeElapsedInSeconds;

            if (TimeRemainingForSmallChangeTransitionInSeconds > 0)
            {
                DirectionSmallChange = DirectionSmallChangeOld
                    + (DirectionSmallChangeNext - DirectionSmallChangeOld) * (1 - (float)(TimeRemainingForSmallChangeTransitionInSeconds / (TransitionTimeInSecondsSmallChange * 2)));

                TimeRemainingForSmallChangeTransitionInSeconds -= TimeElapsedInSeconds;
            }
            else
            {
                DirectionSmallChange = DirectionSmallChangeNext;

                if (TimeRemainingBeforeNextSmallChangeInSeconds <= 0)
                {
                    DirectionSmallChangeOld = DirectionSmallChangeNext;

                    TimeRemainingBeforeNextSmallChangeInSeconds = TimeBeforeNextChangeInSecondsSmallChangeMin + RandomHelper.NextDouble() * TimeBeforeNextChangeInSecondsSmallChange;
                    TimeRemainingForSmallChangeTransitionInSeconds = TransitionTimeInSecondsSmallChange + RandomHelper.NextDouble() * TransitionTimeInSecondsSmallChange;

                    DirectionSmallChangeNext = (float)RandomHelper.NextDouble() * MaxSmallChangeDirectionDirection;
                }
            }

            DirectionCurrent += DirectionSmallChange;
        }
    }
}
