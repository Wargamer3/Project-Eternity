using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    class EnvironmentManagerSorcererStreet : EnvironmentManager
    {
        public EnvironmentManagerSorcererStreet(LifeSimMap Map)
            : base(Map)
        {
            GlobalZone = new MapZoneLifeSim(Map, ZoneShape.ZoneShapeTypes.Full);
        }

        public EnvironmentManagerSorcererStreet(BinaryReader BR, LifeSimMap Map)
            : base(BR, Map)
        {
            GlobalZone = new MapZoneLifeSim(BR, Map);

            int ListMapZoneCount = BR.ReadInt32();
            ListMapZone = new List<MapZone>(ListMapZoneCount);
            for (int Z = 0; Z < ListMapZoneCount; ++Z)
            {
                ListMapZone.Add(new MapZoneLifeSim(BR, Map));
            }
        }
    }
}
