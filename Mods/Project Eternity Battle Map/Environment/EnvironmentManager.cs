using System.Collections.Generic;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class EnvironmentManager
    {
        public BattleMapOverlay Overlay;
        public List<VolatileSubstance> ListVolatileSubstance;

        public List<TimePeriod> ListTimePeriod;

        public EnvironmentManager()
        {
            ListTimePeriod = new List<TimePeriod>();
            ListVolatileSubstance = new List<VolatileSubstance>();
        }
    }
}
