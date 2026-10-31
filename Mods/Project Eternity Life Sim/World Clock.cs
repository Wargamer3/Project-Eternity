using System.Collections.Generic;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class WorldClock
    {
        public double Hour = 12;
        public int Day;
        public float HoursInDay = 24;
        public float Multiplier = 1;
        public EnvironmentManager.TimeLoopTypes Loop = EnvironmentManager.TimeLoopTypes.FirstDay;
        public bool Stopped;
        public bool SpeedUp;
        public static bool Rushing;
        // Off is one game minute per real second. On is the map multiplier, one game hour per real second at 1x.
        const float NormalPace = 1f / 60f;
        public float Pace = NormalPace;
        bool Ready;

        public void Read(EnvironmentManager Source)
        {
            if (Source == null)
            {
                return;
            }

            if (!Ready)
            {
                Hour = Source.TimeStart;
                Ready = true;
            }

            if (Source.HoursInDay > 0f)
            {
                HoursInDay = Source.HoursInDay;
            }

            Multiplier = Source.TimeMultiplier;
            Loop = Source.TimeLoopType;
        }

        public void Advance(double Seconds, int LastDay)
        {
            SpeedUp = PictureOptions.FastTime;
            Rushing = SpeedUp || Multiplier > 1f;
            Pace = SpeedUp ? 1f : NormalPace;
            if (Stopped)
            {
                return;
            }

            if (HoursInDay <= 0f)
            {
                HoursInDay = 24f;
            }

            Hour += Seconds * Multiplier * Pace;
            int Guard = 0;
            while (Hour >= HoursInDay && Guard < 10000)
            {
                Guard++;
                Hour -= HoursInDay;
                if (!NextDay(LastDay))
                {
                    Stopped = true;
                    Hour = HoursInDay;
                    return;
                }
            }
        }

        public int LastDayIndex(IList<TimePeriod> Periods)
        {
            int Last = 0;
            if (Periods == null)
            {
                return Last;
            }

            for (int i = 0; i < Periods.Count; i++)
            {
                if (Periods[i] == null)
                {
                    continue;
                }

                int Start = (int)Periods[i].DayStart;
                if (Start > Last)
                {
                    Last = Start;
                }
            }

            return Last;
        }

        public bool NextDay(int Last)
        {
            if (Loop == EnvironmentManager.TimeLoopTypes.Stop)
            {
                if (Day >= Last)
                {
                    return false;
                }

                Day++;
                return true;
            }

            if (Loop == EnvironmentManager.TimeLoopTypes.LastDay)
            {
                if (Day < Last)
                {
                    Day++;
                }

                return true;
            }

            Day++;
            if (Day > Last)
            {
                Day = 0;
            }

            return true;
        }
    }
}
