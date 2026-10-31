using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public struct Outlook
    {
        public TimePeriod Period;
        public TimePeriod Dying;
        public float Strength;
        public MapZone Zone;
    }

    public static class SkyFade
    {
        class Slot
        {
            public bool Seen;
            public int Signature;
            public TimePeriod Shown;
            public TimePeriod Dying;
            public float Left;
        }

        static readonly Dictionary<MapZone, Slot> Slots = new Dictionary<MapZone, Slot>();

        public static void Reset()
        {
            Slots.Clear();
        }

        public static void Advance(NavMapGameManager World, double Seconds)
        {
            if (World == null || World.Clock == null)
            {
                return;
            }

            float Step = Seconds > 0d ? (float)Seconds : 0f;
            EnvironmentManager Root = RootClock(World);
            if (Root != null && Root.GlobalZone != null)
            {
                AdvanceZone(Root.GlobalZone, Forecast.Pick(Root.GlobalZone.ListTimePeriod, World.Clock.Hour, World.Clock.Day), Step);
            }

            IList<MapInfo> Maps = World.UpdatedMaps;
            if (Maps == null)
            {
                return;
            }

            for (int i = 0; i < Maps.Count; i++)
            {
                MapInfo Info = Maps[i];
                LifeSimMap Map = Info == null || Info.MapMapContainer == null ? null : Info.MapMapContainer.ActiveMap as LifeSimMap;
                if (Map == null || Map.MapEnvironment == null || Map.MapEnvironment.ListMapZone == null)
                {
                    continue;
                }

                for (int z = 0; z < Map.MapEnvironment.ListMapZone.Count; z++)
                {
                    MapZone Zone = Map.MapEnvironment.ListMapZone[z];
                    if (Zone == null)
                    {
                        continue;
                    }

                    AdvanceZone(Zone, Forecast.Pick(Zone.ListTimePeriod, World.Clock.Hour, World.Clock.Day), Step);
                }
            }
        }

        public static void AdvanceZone(MapZone Zone, TimePeriod Now, float Seconds)
        {
            if (Zone == null)
            {
                return;
            }

            Slot Slot;
            if (!Slots.TryGetValue(Zone, out Slot))
            {
                Slot = new Slot();
                Slots[Zone] = Slot;
            }

            int Signature = SignatureOf(Now);
            if (!Slot.Seen)
            {
                Slot.Seen = true;
                Slot.Signature = Signature;
                Slot.Shown = Copy(Now);
                return;
            }

            if (Signature != Slot.Signature)
            {
                Slot.Dying = Slot.Shown;
                Slot.Left = Slot.Shown == null ? 0f : Slot.Shown.CrossfadeLength;
                Slot.Signature = Signature;
                if (Slot.Left <= 0f)
                {
                    Slot.Dying = null;
                    Slot.Left = 0f;
                }
            }

            Slot.Shown = Copy(Now);
            if (Slot.Left > 0f)
            {
                Slot.Left -= Seconds;
                if (Slot.Left <= 0f)
                {
                    Slot.Left = 0f;
                    Slot.Dying = null;
                }
            }
        }

        public static void Read(MapZone Zone, out TimePeriod Dying, out float Strength)
        {
            Dying = null;
            Strength = 0f;
            Slot Slot;
            if (Zone == null || !Slots.TryGetValue(Zone, out Slot) || Slot.Dying == null || Slot.Left <= 0f || Slot.Dying.CrossfadeLength <= 0f)
            {
                return;
            }

            Dying = Slot.Dying;
            Strength = Slot.Left / Slot.Dying.CrossfadeLength;
            if (Strength > 1f)
            {
                Strength = 1f;
            }
        }

        static EnvironmentManager RootClock(NavMapGameManager World)
        {
            if (World.RootMapContainer == null || World.RootMapContainer.MapMapContainer == null)
            {
                return null;
            }

            LifeSimMap Root = World.RootMapContainer.MapMapContainer.ActiveMap as LifeSimMap;
            return Root == null ? null : Root.MapEnvironment;
        }

        static TimePeriod Copy(TimePeriod Period)
        {
            return Period == null ? null : new TimePeriod(Period);
        }

        static int SignatureOf(TimePeriod Period)
        {
            if (Period == null)
            {
                return -1;
            }

            return ((int)Period.WeatherType << 8) | (int)Period.VisibilityType;
        }
    }

    public static class Forecast
    {
        static TimePeriod Held;

        public static TimePeriod Pick(IList<TimePeriod> Periods, double Hour, int Day)
        {
            if (Periods == null || Periods.Count == 0)
            {
                return null;
            }

            TimePeriod Best = null;
            TimePeriod Latest = null;
            for (int i = 0; i < Periods.Count; i++)
            {
                TimePeriod Period = Periods[i];
                if (Period == null || !OnDay(Period, Day))
                {
                    continue;
                }

                if (Latest == null || Period.TimeStart >= Latest.TimeStart)
                {
                    Latest = Period;
                }

                if (Period.TimeStart <= Hour && (Best == null || Period.TimeStart >= Best.TimeStart))
                {
                    Best = Period;
                }
            }

            if (Best != null)
            {
                return Best;
            }

            return Latest ?? Periods[0];
        }

        public static MapZone Smallest(IList<MapZone> Zones, float LocalX, float LocalY, float SpanX, float SpanY)
        {
            MapZone Best = null;
            float BestArea = float.MaxValue;
            if (Zones == null)
            {
                return null;
            }

            for (int i = 0; i < Zones.Count; i++)
            {
                MapZone Zone = Zones[i];
                if (Zone == null || Zone.Shape == null || !Contains(Zone.Shape, LocalX, LocalY, SpanX, SpanY))
                {
                    continue;
                }

                float Area = AreaOf(Zone.Shape, SpanX, SpanY);
                if (Area < BestArea)
                {
                    Best = Zone;
                    BestArea = Area;
                }
            }

            return Best;
        }

        public static TimePeriod At(NavMapGameManager World, float WorldX, float WorldY)
        {
            return Look(World, WorldX, WorldY).Period;
        }

        public static TimePeriod At(WorldClock Clock, MapInfo Root, float WorldX, float WorldY)
        {
            return Look(Clock, Root, WorldX, WorldY).Period;
        }

        public static Outlook Look(NavMapGameManager World, float WorldX, float WorldY)
        {
            if (World == null)
            {
                return new Outlook();
            }

            return Look(World.Clock, World.RootMapContainer, WorldX, WorldY);
        }

        public static Outlook Look(WorldClock Clock, MapInfo Root, float WorldX, float WorldY)
        {
            Outlook Result = new Outlook();
            if (Held != null)
            {
                Result.Period = Held;
                return Result;
            }

            if (Clock == null)
            {
                return Result;
            }

            MapInfo Chunk = ChunkCross.At(WorldX, WorldY);
            MapZone Local = null;
            if (Chunk != null && Chunk.MapMapContainer != null)
            {
                float SpanX = Chunk.MapMapContainer.SpanX;
                float SpanY = Chunk.MapMapContainer.SpanY;
                Local = Smallest(ZonesOf(Chunk), WorldX - Chunk.MapMapContainer.OffsetX, WorldY - Chunk.MapMapContainer.OffsetY, SpanX, SpanY);
            }

            Result.Zone = Local != null ? Local : GlobalZone(Root);
            IList<TimePeriod> Periods = Result.Zone == null ? null : Result.Zone.ListTimePeriod;
            Result.Period = Pick(Periods, Clock.Hour, Clock.Day);
            SkyFade.Read(Result.Zone, out Result.Dying, out Result.Strength);
            return Result;
        }

        public static void SetGlobal(NavMapGameManager World, TimePeriod.WeatherTypes Weather, TimePeriod.VisibilityTypes Visibility)
        {
            Held = new TimePeriod("Weather");
            Held.WeatherType = Weather;
            Held.VisibilityType = Visibility;
            if (World == null || World.RootMapContainer == null || World.RootMapContainer.MapMapContainer == null)
            {
                return;
            }

            LifeSimMap Root = World.RootMapContainer.MapMapContainer.ActiveMap as LifeSimMap;
            if (Root == null || Root.MapEnvironment == null || Root.MapEnvironment.GlobalZone == null)
            {
                return;
            }

            MapZone Zone = Root.MapEnvironment.GlobalZone;
            if (Zone.ListTimePeriod == null)
            {
                Zone.ListTimePeriod = new List<TimePeriod>();
            }

            if (Zone.ListTimePeriod.Count == 0)
            {
                Zone.ListTimePeriod.Add(new TimePeriod("Weather"));
            }

            TimePeriod Period = Pick(Zone.ListTimePeriod, World.Clock.Hour, World.Clock.Day);
            if (Period == null)
            {
                Period = new TimePeriod("Weather");
                Zone.ListTimePeriod.Add(Period);
            }

            Period.WeatherType = Weather;
            Period.VisibilityType = Visibility;
        }

        public static int Influence(TimePeriod Period)
        {
            if (Period == null)
            {
                return 0;
            }

            int Value = FromWeather(Period.WeatherType);
            if (Period.VisibilityType == TimePeriod.VisibilityTypes.FogOfWar && Value > -4)
            {
                Value = -4;
            }

            return Value;
        }

        public static int Apply(int Points, TimePeriod Period, bool Sheltered)
        {
            if (!Sheltered)
            {
                Points += Influence(Period);
            }

            return Points;
        }

        public static int Blended(TimePeriod Period, TimePeriod Dying, float Strength)
        {
            int Now = Influence(Period);
            if (Dying == null || Strength <= 0f)
            {
                return Now;
            }

            int Was = Influence(Dying);
            return (int)Math.Round(Was * Strength + Now * (1f - Strength));
        }

        public static int Apply(int Points, Outlook Sky, bool Sheltered)
        {
            if (Sheltered)
            {
                return Points;
            }

            return Points + Blended(Sky.Period, Sky.Dying, Sky.Strength);
        }

        public static bool Falls(TimePeriod Period, bool Sheltered)
        {
            if (Sheltered || Period == null)
            {
                return false;
            }

            return Period.WeatherType == TimePeriod.WeatherTypes.SmallRain
                || Period.WeatherType == TimePeriod.WeatherTypes.HeavyRain
                || Period.WeatherType == TimePeriod.WeatherTypes.Thunderstorm;
        }

        public static bool Foggy(TimePeriod Period, bool Sheltered)
        {
            if (Sheltered || Period == null)
            {
                return false;
            }

            return Period.VisibilityType == TimePeriod.VisibilityTypes.FogOfWar;
        }

        public static bool Cloudy(TimePeriod Period)
        {
            if (Period == null)
            {
                return false;
            }

            return Period.WeatherType == TimePeriod.WeatherTypes.SomeClouds
                || Period.WeatherType == TimePeriod.WeatherTypes.Cloudy
                || Period.WeatherType == TimePeriod.WeatherTypes.Thunderstorm;
        }

        public static bool Storm(TimePeriod Period)
        {
            return Period != null && Period.WeatherType == TimePeriod.WeatherTypes.Thunderstorm;
        }

        static int FromWeather(TimePeriod.WeatherTypes Weather)
        {
            if (Weather == TimePeriod.WeatherTypes.SomeClouds || Weather == TimePeriod.WeatherTypes.Cloudy)
            {
                return -2;
            }

            if (Weather == TimePeriod.WeatherTypes.SmallRain)
            {
                return -6;
            }

            if (Weather == TimePeriod.WeatherTypes.HeavyRain)
            {
                return -8;
            }

            if (Weather == TimePeriod.WeatherTypes.Thunderstorm)
            {
                return -12;
            }

            return 0;
        }

        static bool OnDay(TimePeriod Period, int Day)
        {
            int Start = (int)Period.DayStart;
            return Start == 0 || Start == Day;
        }

        public static bool Contains(ZoneShape Shape, float LocalX, float LocalY, float SpanX, float SpanY)
        {
            if (Shape == null)
            {
                return false;
            }

            if (Shape.ZoneShapeType == ZoneShape.ZoneShapeTypes.Full)
            {
                return LocalX >= 0f && LocalY >= 0f && (SpanX <= 0f || LocalX < SpanX) && (SpanY <= 0f || LocalY < SpanY);
            }

            if (Shape.ZoneShapeType == ZoneShape.ZoneShapeTypes.Oval)
            {
                float DX = LocalX - Shape.Position.X;
                float DY = LocalY - Shape.Position.Y;
                return DX * DX + DY * DY <= Shape.Radius * Shape.Radius;
            }

            return LocalX >= Shape.Position.X && LocalY >= Shape.Position.Y
                && LocalX < Shape.Position.X + Shape.Size.X && LocalY < Shape.Position.Y + Shape.Size.Y;
        }

        public static float AreaOf(ZoneShape Shape, float SpanX, float SpanY)
        {
            if (Shape == null)
            {
                return float.MaxValue;
            }

            if (Shape.ZoneShapeType == ZoneShape.ZoneShapeTypes.Full)
            {
                float Wide = SpanX > 0f ? SpanX : 100000f;
                float Deep = SpanY > 0f ? SpanY : Wide;
                return Wide * Deep;
            }

            if (Shape.ZoneShapeType == ZoneShape.ZoneShapeTypes.Oval)
            {
                return Shape.Radius * Shape.Radius * 3.14159f;
            }

            return Shape.Size.X * Shape.Size.Y;
        }

        static IList<MapZone> ZonesOf(MapInfo Chunk)
        {
            LifeSimMap Map = Chunk.MapMapContainer.ActiveMap as LifeSimMap;
            if (Map == null || Map.MapEnvironment == null)
            {
                return null;
            }

            return Map.MapEnvironment.ListMapZone;
        }

        static MapZone GlobalZone(MapInfo RootChunk)
        {
            if (RootChunk == null || RootChunk.MapMapContainer == null)
            {
                return null;
            }

            LifeSimMap Root = RootChunk.MapMapContainer.ActiveMap as LifeSimMap;
            if (Root == null || Root.MapEnvironment == null)
            {
                return null;
            }

            return Root.MapEnvironment.GlobalZone;
        }
    }
}
