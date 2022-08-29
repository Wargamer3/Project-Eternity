using System;
using System.IO;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    class MapZoneDeathmatch : MapZone
    {
        private readonly DeathmatchMap Map;

        public MapZoneDeathmatch(DeathmatchMap Map, ZoneShape.ZoneShapeTypes ZoneShapeType)
            : base(Map, ZoneShapeType)
        {
            this.Map = Map;
        }

        public MapZoneDeathmatch(BinaryReader BR, DeathmatchMap Map)
            : base(BR, Map)
        {
            this.Map = Map;

            UpdateOverlayTypeOfSky();
            UpdateOverlayWeater();
            UpdateOverlayVisibility();
        }

        public MapZoneDeathmatch(MapZoneDeathmatch Copy)
            : base(Copy)
        {
            Map = Copy.Map;

            UpdateOverlayTypeOfSky();
            UpdateOverlayWeater();
            UpdateOverlayVisibility();
        }

        public override void OnNewPlayerPhase()
        {
            if (CurrentTimePeriod.PassiveSkill != null && Shape.ZoneShapeType == ZoneShape.ZoneShapeTypes.Full)
            {
                for (int S = 0; S < Map.ListPlayer[Map.ActivePlayerIndex].ListSquad.Count; S++)
                {
                    Squad ActiveSquad = Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[S];

                    for (int U = ActiveSquad.UnitsAliveInSquad - 1; U >= 0; --U)
                    {
                        Map.ActivateAutomaticSkills(CurrentTimePeriod.PassiveSkill, ActiveSquad, ActiveSquad[U]);
                    }
                }
            }
        }

        protected override void UpdateOverlayTypeOfSky()
        {
            switch (CurrentTimePeriod.SkyType)
            {
                case TimePeriod.SkyTypes.Disabled:
                    OverlayTypeOfSky = null;
                    break;

                case TimePeriod.SkyTypes.Regular:
                    OverlayTypeOfSky = new DayNightCycleColorOnly(Map, this);
                    break;
            }
        }

        protected override void UpdateOverlayWeater()
        {
            switch (CurrentTimePeriod.WeatherType)
            {
                case TimePeriod.WeatherTypes.Regular:
                    OverlayWeather = new DefaultWeather(Map, Shape);
                    break;

                case TimePeriod.WeatherTypes.SmallRain:
                    RainWeather SmallRainWeather = new RainWeather(Map, Shape);
                    SmallRainWeather.Init();
                    OverlayWeather = SmallRainWeather;

                    /*WetWeather SmallRainWeather = new WetWeather(Map, Shape);
                    SmallRainWeather.Init();
                    OverlayWeather = SmallRainWeather;*/
                    break;
            }
        }

        protected override void UpdateOverlayVisibility()
        {
            switch (CurrentTimePeriod.VisibilityType)
            {
                case TimePeriod.VisibilityTypes.Regular:
                    OverlayVisibility = null;
                    break;
            }
        }

        public override MapZone Copy()
        {
            return new MapZoneDeathmatch(this);
        }
    }
}
