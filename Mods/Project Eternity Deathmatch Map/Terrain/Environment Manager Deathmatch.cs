using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    class EnvironmentManagerDeathmatch : EnvironmentManager
    {
        public EnvironmentManagerDeathmatch(DeathmatchMap Map)
            : base(Map)
        {
            GlobalZone = new MapZoneDeathmatch(Map, ZoneShape.ZoneShapeTypes.Full);
        }

        public EnvironmentManagerDeathmatch(BinaryReader BR, DeathmatchMap Map)
            : base(BR, Map)
        {
            GlobalZone = new MapZoneDeathmatch(BR, Map);

            int ListMapZoneCount = BR.ReadInt32();
            ListMapZone = new List<MapZone>(ListMapZoneCount);
            for (int Z = 0; Z < ListMapZoneCount; ++Z)
            {
                ListMapZone.Add(new MapZoneDeathmatch(BR, Map));
            }
        }
    }
}
