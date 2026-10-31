using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public struct FieldSample
    {
        public Vector2 Wind;
        public float Strength;
        public float Rain;
        public float WetGround;
    }

    public static class Climate
    {
        public static FieldSample Now;

        public static void Advance(NavMapGameManager World, float Dt)
        {
            if (Dt < 0f)
            {
                Dt = 0f;
            }

            if (Dt > 0.1f)
            {
                Dt = 0.1f;
            }

            FieldSample Next = Now;
            Outlook Sky = SampleSky(World);
            Next.Strength = StrengthOf(Sky);
            Next.Rain = RainOf(Sky);
            float Angle = 0.4f;
            if (World != null && World.Clock != null)
            {
                Angle = (float)(World.Clock.Day * 1.7 + World.Clock.Hour * 0.21);
            }

            Next.Wind = new Vector2((float)System.Math.Cos(Angle), (float)System.Math.Sin(Angle)) * Next.Strength;
            if (Next.Rain > 0.05f)
            {
                Next.WetGround += Dt * Next.Rain * 0.14f;
                if (Next.WetGround > 1f)
                {
                    Next.WetGround = 1f;
                }
            }
            else if (Next.WetGround > 0f)
            {
                Next.WetGround -= Dt * 0.02f;
                if (Next.WetGround < 0f)
                {
                    Next.WetGround = 0f;
                }
            }

            Now = Next;
        }

        public static void Drench(NavMapGameManager World, float Dt)
        {
            if (World == null || World.UpdatedMaps == null)
            {
                return;
            }

            IList<MapInfo> Maps = World.UpdatedMaps;
            for (int m = 0; m < Maps.Count; m++)
            {
                MapInfo Info = Maps[m];
                if (Info == null || Info.People == null)
                {
                    continue;
                }

                for (int i = 0; i < Info.People.Count; i++)
                {
                    Drench(Info.People[i], Dt);
                }
            }
        }

        public static void Drench(PlayerCharacter Who, float Dt)
        {
            if (Who == null || Who.WasInWater || Dt <= 0f)
            {
                return;
            }

            if (Now.Rain > 0.05f && !Roof(Who))
            {
                float Through = 1f;
                if (Who.SharedMapContex != null && Who.SharedMapContex.CurrentMapInfo != null)
                {
                    Vector3 Shift = ChunkOrigin.Shift(Who.SharedMapContex.CurrentMapInfo);
                    Through = ForestScatter.Shade(Who.Position.X + Shift.X, Who.Position.Y + Shift.Z);
                }

                float Cap = 0.22f + 0.33f * Through;
                if (Who.WetAmount < Cap)
                {
                    Who.WetAmount += Dt * Now.Rain * 0.28f * Through;
                    if (Who.WetAmount > Cap)
                    {
                        Who.WetAmount = Cap;
                    }
                }

                return;
            }

            if (Who.WetAmount <= 0f)
            {
                return;
            }

            Who.WetAmount -= Dt / WaterPlace.DrySeconds;
            if (Who.WetAmount < 0f)
            {
                Who.WetAmount = 0f;
                Who.WetLine = 0f;
            }
        }

        public static bool Roof(PlayerCharacter Who)
        {
            if (Who == null || Who.SharedMapContex == null)
            {
                return false;
            }

            MapInfo Here = Who.SharedMapContex.CurrentMapInfo;
            if (Here == null)
            {
                return false;
            }

            return Motives.Sheltered(Here, Who.Position.X, Who.Position.Y, BuildFloor.Index(Who.Position.Z));
        }

        static Outlook SampleSky(NavMapGameManager World)
        {
            if (World == null)
            {
                return new Outlook();
            }

            float X = 0f;
            float Z = 0f;
            IList<MapInfo> Maps = World.UpdatedMaps;
            if (Maps != null)
            {
                for (int m = 0; m < Maps.Count; m++)
                {
                    MapInfo Info = Maps[m];
                    if (Info == null || Info.People == null || Info.People.Count == 0 || Info.People[0] == null)
                    {
                        continue;
                    }

                    Vector3 Shift = ChunkOrigin.Shift(Info);
                    X = Info.People[0].Position.X + Shift.X;
                    Z = Info.People[0].Position.Y + Shift.Z;
                    break;
                }
            }

            return Forecast.Look(World, X, Z);
        }

        static float StrengthOf(Outlook Sky)
        {
            float Now = OneStrength(Sky.Period);
            if (Sky.Dying == null || Sky.Strength <= 0f)
            {
                return Now;
            }

            float Was = OneStrength(Sky.Dying);
            return Now + (Was - Now) * Sky.Strength;
        }

        static float RainOf(Outlook Sky)
        {
            float Now = OneRain(Sky.Period);
            if (Sky.Dying == null || Sky.Strength <= 0f)
            {
                return Now;
            }

            float Was = OneRain(Sky.Dying);
            return Now + (Was - Now) * Sky.Strength;
        }

        static float OneStrength(TimePeriod Period)
        {
            if (Period == null)
            {
                return 2.2f;
            }

            switch (Period.WeatherType)
            {
                case TimePeriod.WeatherTypes.Windy:
                    return 7f;
                case TimePeriod.WeatherTypes.Sandstorm:
                    return 10f;
                case TimePeriod.WeatherTypes.Thunderstorm:
                    return 9f;
                case TimePeriod.WeatherTypes.HeavyRain:
                case TimePeriod.WeatherTypes.HeavySnow:
                    return 6f;
                case TimePeriod.WeatherTypes.SmallRain:
                case TimePeriod.WeatherTypes.LightSnow:
                    return 4f;
                default:
                    return 2.2f;
            }
        }

        static float OneRain(TimePeriod Period)
        {
            if (Period == null)
            {
                return 0f;
            }

            switch (Period.WeatherType)
            {
                case TimePeriod.WeatherTypes.SmallRain:
                    return 0.45f;
                case TimePeriod.WeatherTypes.HeavyRain:
                    return 0.8f;
                case TimePeriod.WeatherTypes.Thunderstorm:
                    return 1f;
                default:
                    return 0f;
            }
        }
    }
}
