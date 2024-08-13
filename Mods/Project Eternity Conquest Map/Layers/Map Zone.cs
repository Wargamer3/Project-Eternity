using System;
using System.IO;
using ProjectEternity.Core.Units.Conquest;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class MapZoneConquest : MapZone
    {
        private readonly ConquestMap Map;

        public MapZoneConquest(ConquestMap Map, ZoneShape.ZoneShapeTypes ZoneShapeType)
            : base(Map, ZoneShapeType)
        {
            this.Map = Map;
        }

        public MapZoneConquest(BinaryReader BR, ConquestMap Map)
            : base(BR, Map)
        {
            this.Map = Map;

            UpdateOverlayTypeOfSky();
            UpdateOverlayWeater();
            UpdateOverlayVisibility();
        }

        public MapZoneConquest(MapZoneConquest Copy)
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
                for (int S = 0; S < Map.ListPlayer[Map.ActivePlayerIndex].ListUnit.Count; S++)
                {
                    UnitConquest ActiveUnit = Map.ListPlayer[Map.ActivePlayerIndex].ListUnit[S];

                    Map.ActivateAutomaticSkills(CurrentTimePeriod.PassiveSkill, null, ActiveUnit);
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
            return new MapZoneConquest(this);
        }
    }
}
