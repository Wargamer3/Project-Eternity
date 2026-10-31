using System.Collections.Generic;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public enum CombatTaste
    {
        Close,
        Snipe,
        Hide,
        BehindTheirLines,
    }

    public enum RouteStyle
    {
        Direct,
        Covered,
        Flank,
    }

    public class PersonalityWeight
    {
        public string ActionName;
        public int Delta;
    }

    public class Personality
    {
        public string Name;
        public string Description;
        public List<string> ListForbiddenActionName;
        public List<string> ListGrantedActionName;
        public List<PersonalityWeight> ListWeightDelta;
        public bool WaiveRealTimeBudget;
        public bool PrefersRun;
        public bool PrefersOptionalLinks;
        public bool HasCombatTaste;
        public CombatTaste Taste;
        public int DurationAdjustSeconds;
        public float NeedRate = 1f;

        public Personality()
        {
            Name = string.Empty;
            Description = string.Empty;
            ListForbiddenActionName = new List<string>();
            ListGrantedActionName = new List<string>();
            ListWeightDelta = new List<PersonalityWeight>();
        }

        public static float FightDistance(CombatTaste Taste, float WeaponNear, float WeaponFar)
        {
            if (WeaponFar < WeaponNear)
            {
                WeaponFar = WeaponNear;
            }

            float Wanted = Taste == CombatTaste.Snipe ? WeaponFar : WeaponNear;
            if (Wanted < WeaponNear)
            {
                Wanted = WeaponNear;
            }

            if (Wanted > WeaponFar)
            {
                Wanted = WeaponFar;
            }

            return Wanted;
        }

        public static RouteStyle RouteFor(CombatTaste Taste)
        {
            if (Taste == CombatTaste.Hide)
            {
                return RouteStyle.Covered;
            }

            if (Taste == CombatTaste.BehindTheirLines)
            {
                return RouteStyle.Flank;
            }

            return RouteStyle.Direct;
        }
    }
}
