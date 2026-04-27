using System;
using System.IO;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class MapZoneLifeSim : MapZone
    {
        private readonly LifeSimMap Map;

        public MapZoneLifeSim(LifeSimMap Map, ZoneShape.ZoneShapeTypes ZoneShapeType)
            : base(Map, ZoneShapeType)
        {
            this.Map = Map;
        }

        public MapZoneLifeSim(BinaryReader BR, LifeSimMap Map)
            : base(BR, Map)
        {
            this.Map = Map;

            UpdateOverlayTypeOfSky();
            UpdateOverlayWeater();
            UpdateOverlayVisibility();
        }

        public MapZoneLifeSim(MapZoneLifeSim Copy)
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
            return new MapZoneLifeSim(this);
        }
    }
}
