using System;
using System.IO;
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

    }
}
