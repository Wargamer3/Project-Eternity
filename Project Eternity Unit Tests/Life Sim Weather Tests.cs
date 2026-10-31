using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.UnitTests
{
    [TestClass]
    public class LifeSimWeatherTests
    {
        [TestMethod]
        public void TheClockUsesTheMultiplierAndWrapsTheDay()
        {
            WorldClock Clock = new WorldClock();
            Clock.Hour = 12;
            Clock.HoursInDay = 24;
            Clock.Multiplier = 2;
            Clock.Loop = EnvironmentManager.TimeLoopTypes.FirstDay;

            Clock.Advance(10, 0);

            Assert.AreEqual(8d, Clock.Hour, 0.001d);
            Assert.AreEqual(0, Clock.Day);
            Assert.IsFalse(Clock.Stopped);
        }

        [TestMethod]
        public void TheLoopReturnsToTheFirstDayOrStaysOnTheLast()
        {
            WorldClock First = new WorldClock();
            First.Hour = 0;
            First.HoursInDay = 10;
            First.Day = 2;
            First.Loop = EnvironmentManager.TimeLoopTypes.FirstDay;
            First.Advance(10, 2);
            Assert.AreEqual(0, First.Day);

            WorldClock Last = new WorldClock();
            Last.Hour = 0;
            Last.HoursInDay = 10;
            Last.Day = 2;
            Last.Loop = EnvironmentManager.TimeLoopTypes.LastDay;
            Last.Advance(10, 2);
            Assert.AreEqual(2, Last.Day);

            WorldClock Stop = new WorldClock();
            Stop.Hour = 0;
            Stop.HoursInDay = 10;
            Stop.Day = 1;
            Stop.Loop = EnvironmentManager.TimeLoopTypes.Stop;
            Stop.Advance(10, 1);
            Assert.IsTrue(Stop.Stopped);
            Assert.AreEqual(10d, Stop.Hour, 0.001d);
        }

        [TestMethod]
        public void TheScheduleRepeatsAndDayStartCanHoldAPeriod()
        {
            List<TimePeriod> Periods = new List<TimePeriod>();
            Periods.Add(Period("Morning", 8, 0, TimePeriod.WeatherTypes.Regular));
            Periods.Add(Period("Night", 20, 0, TimePeriod.WeatherTypes.SmallRain));
            Periods.Add(Period("Storm Day", 9, 2, TimePeriod.WeatherTypes.Thunderstorm));

            Assert.AreEqual("Morning", Forecast.Pick(Periods, 10, 1).Name);
            Assert.AreEqual("Night", Forecast.Pick(Periods, 21, 1).Name);
            Assert.AreEqual("Night", Forecast.Pick(Periods, 2, 1).Name);
            Assert.AreEqual("Storm Day", Forecast.Pick(Periods, 12, 2).Name);
            Assert.AreEqual("Morning", Forecast.Pick(Periods, 10, 3).Name);
        }

        [TestMethod]
        public void TheSmallestLocalZoneWinsAndRainSkipsARoofedHouse()
        {
            LifeSimMap Map = new LifeSimMap();
            MapInfo Chunk = new MapInfo(new MapContainer("Town", Map), null, null);
            Chunk.MapMapContainer.SpanX = 200f;
            Chunk.MapMapContainer.SpanY = 200f;
            Map.Chunk = Chunk;
            Map.MapEnvironment.GlobalZone.ListTimePeriod[0].WeatherType = TimePeriod.WeatherTypes.SmallRain;
            Map.MapEnvironment.GlobalZone.ListTimePeriod[0].TimeStart = 0;

            MapZoneLifeSim Wide = new MapZoneLifeSim(Map, ZoneShape.ZoneShapeTypes.Rectangle);
            Wide.Shape.Position = Vector2.Zero;
            Wide.Shape.Size = new Vector2(100f, 100f);
            Wide.ListTimePeriod[0].WeatherType = TimePeriod.WeatherTypes.Cloudy;
            Wide.ListTimePeriod[0].TimeStart = 0;
            MapZoneLifeSim Pocket = new MapZoneLifeSim(Map, ZoneShape.ZoneShapeTypes.Rectangle);
            Pocket.Shape.Position = new Vector2(10f, 10f);
            Pocket.Shape.Size = new Vector2(20f, 20f);
            Pocket.ListTimePeriod[0].WeatherType = TimePeriod.WeatherTypes.Thunderstorm;
            Pocket.ListTimePeriod[0].TimeStart = 0;
            Map.MapEnvironment.ListMapZone.Add(Wide);
            Map.MapEnvironment.ListMapZone.Add(Pocket);

            Assert.AreSame(Pocket, Forecast.Smallest(Map.MapEnvironment.ListMapZone, 15f, 15f, 200f, 200f));
            Assert.AreSame(Wide, Forecast.Smallest(Map.MapEnvironment.ListMapZone, 80f, 80f, 200f, 200f));

            List<MapInfo> Previous = ChunkCross.Maps;
            ChunkCross.Maps = new List<MapInfo> { Chunk };
            WorldClock Clock = new WorldClock();
            Clock.Hour = 12;
            Clock.Day = 0;
            try
            {
                Assert.AreEqual(TimePeriod.WeatherTypes.Thunderstorm, Forecast.At(Clock, Chunk, 15f, 15f).WeatherType);
                Assert.AreEqual(TimePeriod.WeatherTypes.Cloudy, Forecast.At(Clock, Chunk, 80f, 80f).WeatherType);
                Assert.AreEqual(TimePeriod.WeatherTypes.SmallRain, Forecast.At(Clock, Chunk, 150f, 150f).WeatherType);
            }
            finally
            {
                ChunkCross.Maps = Previous;
            }

            Assert.AreEqual(-12, Forecast.Influence(Pocket.ListTimePeriod[0]));
            Assert.AreEqual(-2, Forecast.Influence(Wide.ListTimePeriod[0]));
            Assert.AreEqual(-6, Forecast.Influence(Map.MapEnvironment.GlobalZone.ListTimePeriod[0]));
            Assert.IsTrue(Forecast.Falls(Map.MapEnvironment.GlobalZone.ListTimePeriod[0], false));
            Assert.IsFalse(Forecast.Falls(Map.MapEnvironment.GlobalZone.ListTimePeriod[0], true));
            Assert.AreEqual(-4, Forecast.Apply(8, Pocket.ListTimePeriod[0], false));
            Assert.AreEqual(8, Forecast.Apply(8, Pocket.ListTimePeriod[0], true));
        }

        [TestMethod]
        public void FogLowersTheRoomAndAHouseNeedsARoof()
        {
            TimePeriod Fog = Period("Fog", 0, 0, TimePeriod.WeatherTypes.Regular);
            Fog.VisibilityType = TimePeriod.VisibilityTypes.FogOfWar;
            Assert.AreEqual(-4, Forecast.Influence(Fog));
            Assert.IsTrue(Forecast.Foggy(Fog, false));
            Assert.IsFalse(Forecast.Foggy(Fog, true));

            MapInfo House = new MapInfo(new MapContainer("House", null), null, null);
            House.MapMapContainer.SpanX = 160f;
            House.MapMapContainer.SpanY = 160f;
            for (int Y = 0; Y <= 3; Y++)
            {
                for (int X = 0; X <= 3; X++)
                {
                    if (X != 0 && X != 3 && Y != 0 && Y != 3)
                    {
                        continue;
                    }

                    BuildingItem Wall = new BuildingItem();
                    Wall.Kind = BuildingKind.Wall;
                    Wall.Width = 32f;
                    Wall.Length = 32f;
                    Wall.Position = new Vector3((X + 0.5f) * 32f, (Y + 0.5f) * 32f, 0f);
                    House.Buildings.Add(Wall);
                }
            }

            Assert.IsFalse(Motives.Sheltered(House, 48f, 48f, 0));

            BuildingItem Roof = new BuildingItem();
            Roof.Kind = BuildingKind.Floor;
            Roof.Floor = 1;
            Roof.Width = 96f;
            Roof.Length = 96f;
            Roof.Position = new Vector3(64f, 64f, 96f);
            House.Buildings.Add(Roof);

            Assert.IsTrue(Motives.Sheltered(House, 48f, 48f, 0));
            Assert.IsFalse(Motives.Sheltered(House, 144f, 80f, 0));
        }

        [TestMethod]
        public void AChangedSkyDiesDownOverTheCrossfadeLength()
        {
            SkyFade.Reset();
            LifeSimMap Map = new LifeSimMap();
            MapZone Zone = Map.MapEnvironment.GlobalZone;
            TimePeriod Period = Zone.ListTimePeriod[0];
            Period.WeatherType = TimePeriod.WeatherTypes.SmallRain;
            Period.CrossfadeLength = 4f;
            SkyFade.AdvanceZone(Zone, Period, 0f);

            Period.WeatherType = TimePeriod.WeatherTypes.Regular;
            SkyFade.AdvanceZone(Zone, Period, 0f);
            TimePeriod Dying;
            float Strength;
            SkyFade.Read(Zone, out Dying, out Strength);
            Assert.AreEqual(1f, Strength, 0.001f);
            Assert.AreEqual(TimePeriod.WeatherTypes.SmallRain, Dying.WeatherType);
            Assert.AreEqual(-3, Forecast.Blended(Period, Dying, 0.5f));

            SkyFade.AdvanceZone(Zone, Period, 4f);
            SkyFade.Read(Zone, out Dying, out Strength);
            Assert.AreEqual(0f, Strength, 0.001f);
            Assert.IsNull(Dying);
            Assert.AreEqual(0, Forecast.Blended(Period, Dying, Strength));

            Period.WeatherType = TimePeriod.WeatherTypes.Regular;
            Period.CrossfadeLength = 0f;
            SkyFade.AdvanceZone(Zone, Period, 0f);
            Period.WeatherType = TimePeriod.WeatherTypes.HeavyRain;
            SkyFade.AdvanceZone(Zone, Period, 0f);
            SkyFade.Read(Zone, out Dying, out Strength);
            Assert.IsNull(Dying);
            Assert.AreEqual(-8, Forecast.Blended(Period, Dying, Strength));
        }

        [TestMethod]
        public void TheShellSkipsIndoorAirAndARoofStillRipples()
        {
            SkyFade.Reset();
            WeatherDraw.Reset();
            LifeSimMap Map = new LifeSimMap();
            MapInfo Chunk = new MapInfo(new MapContainer("House", Map), null, null);
            Chunk.MapMapContainer.SpanX = 160f;
            Chunk.MapMapContainer.SpanY = 160f;
            Map.Chunk = Chunk;
            Map.MapEnvironment.GlobalZone.ListTimePeriod[0].WeatherType = TimePeriod.WeatherTypes.Thunderstorm;
            Map.MapEnvironment.GlobalZone.ListTimePeriod[0].TimeStart = 0;
            Close(Chunk);
            BuildingItem Roof = new BuildingItem();
            Roof.Kind = BuildingKind.Floor;
            Roof.Floor = 1;
            Roof.Width = 96f;
            Roof.Length = 96f;
            Roof.Position = new Vector3(64f, 64f, 96f);
            Chunk.Buildings.Add(Roof);
            BuildingItem Slab = new BuildingItem();
            Slab.Kind = BuildingKind.Floor;
            Slab.Floor = 0;
            Slab.Width = 32f;
            Slab.Length = 32f;
            Slab.Position = new Vector3(48f, 48f, 0f);
            Chunk.Buildings.Add(Slab);

            List<MapInfo> Previous = ChunkCross.Maps;
            ChunkCross.Maps = new List<MapInfo> { Chunk };
            WorldClock Clock = new WorldClock();
            Clock.Hour = 12;
            try
            {
                Assert.AreEqual(0f, WeatherDraw.ShellCover(Clock, Chunk, 48f, 48f), 0.001f);
                Assert.AreEqual(1f, WeatherDraw.ShellCover(Clock, Chunk, 144f, 80f), 0.001f);
                Assert.AreEqual(0, WeatherDraw.OpenSectors(Clock, Chunk, new Vector3(48f, 10f, 48f), 8f, 12));
                Assert.AreEqual(12, WeatherDraw.OpenSectors(Clock, Chunk, new Vector3(144f, 10f, 80f), 12f, 12));
                int Mixed = WeatherDraw.OpenSectors(Clock, Chunk, new Vector3(144f, 10f, 80f), 100f, 12);
                Assert.IsTrue(Mixed > 0 && Mixed < 12);
                Assert.IsTrue(WeatherDraw.RoofInRain(Clock, Chunk, Chunk, Roof));
                Assert.IsFalse(WeatherDraw.RoofInRain(Clock, Chunk, Chunk, Slab));
                Assert.IsTrue(WeatherDraw.StrikeAllowed(Clock, Chunk, 144f, 80f));
                Assert.IsFalse(WeatherDraw.StrikeAllowed(Clock, Chunk, 48f, 48f));

                Map.MapEnvironment.GlobalZone.ListTimePeriod[0].WeatherType = TimePeriod.WeatherTypes.HeavyRain;
                Assert.IsFalse(WeatherDraw.StrikeAllowed(Clock, Chunk, 144f, 80f));
                Assert.AreEqual(1f, WeatherDraw.ShellCover(Clock, Chunk, 144f, 80f), 0.001f);
            }
            finally
            {
                ChunkCross.Maps = Previous;
                WeatherDraw.Reset();
                SkyFade.Reset();
            }
        }

        [TestMethod]
        public void AStrikeBrightensOnlyWhileItIsVisible()
        {
            WeatherDraw.Reset();
            WeatherDraw.Step(false, 10d);
            Assert.AreEqual(1f, WeatherDraw.Light, 0.001f);

            WeatherDraw.Reset();
            WeatherDraw.Step(true, 3d);
            Assert.IsTrue(WeatherDraw.Light > 1f);
            Assert.AreEqual(WeatherDraw.Brightness(1.2d, 1.2d), WeatherDraw.Light, 0.001f);

            WeatherDraw.Step(false, 0.2d);
            Assert.AreEqual(1f, WeatherDraw.Light, 0.001f);

            WeatherDraw.Reset();
            WeatherDraw.Step(true, 3d);
            WeatherDraw.Step(true, 1.2d);
            Assert.AreEqual(1f, WeatherDraw.Light, 0.001f);
            WeatherDraw.Reset();
        }

        static void Close(MapInfo House)
        {
            for (int Y = 0; Y <= 3; Y++)
            {
                for (int X = 0; X <= 3; X++)
                {
                    if (X != 0 && X != 3 && Y != 0 && Y != 3)
                    {
                        continue;
                    }

                    BuildingItem Wall = new BuildingItem();
                    Wall.Kind = BuildingKind.Wall;
                    Wall.Width = 32f;
                    Wall.Length = 32f;
                    Wall.Position = new Vector3((X + 0.5f) * 32f, (Y + 0.5f) * 32f, 0f);
                    House.Buildings.Add(Wall);
                }
            }
        }

        static TimePeriod Period(string Name, double Hour, int Day, TimePeriod.WeatherTypes Weather)
        {
            TimePeriod Period = new TimePeriod(Name);
            Period.TimeStart = Hour;
            Period.DayStart = Day;
            Period.WeatherType = Weather;
            return Period;
        }
    }
}
