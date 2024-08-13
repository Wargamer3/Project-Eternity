using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    class EnvironmentManagerConquest : EnvironmentManager
    {
        public EnvironmentManagerConquest(ConquestMap Map)
            : base(Map)
        {
            GlobalZone = new MapZoneConquest(Map, ZoneShape.ZoneShapeTypes.Full);
        }

        public EnvironmentManagerConquest(BinaryReader BR, ConquestMap Map)
            : base(BR, Map)
        {
            GlobalZone = new MapZoneConquest(BR, Map);

            int ListMapZoneCount = BR.ReadInt32();
            ListMapZone = new List<MapZone>(ListMapZoneCount);
            for (int Z = 0; Z < ListMapZoneCount; ++Z)
            {
                ListMapZone.Add(new MapZoneConquest(BR, Map));
            }
        }
    }
}
