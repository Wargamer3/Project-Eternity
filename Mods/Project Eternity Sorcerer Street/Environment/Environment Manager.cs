using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    class EnvironmentManagerSorcererStreet : EnvironmentManager
    {
        public EnvironmentManagerSorcererStreet(SorcererStreetMap Map)
            : base(Map)
        {
            GlobalZone = new MapZoneSorcererStreet(Map, ZoneShape.ZoneShapeTypes.Full);
        }

        public EnvironmentManagerSorcererStreet(BinaryReader BR, SorcererStreetMap Map)
            : base(BR, Map)
        {
            GlobalZone = new MapZoneSorcererStreet(BR, Map);

            int ListMapZoneCount = BR.ReadInt32();
            ListMapZone = new List<MapZone>(ListMapZoneCount);
            for (int Z = 0; Z < ListMapZoneCount; ++Z)
            {
                ListMapZone.Add(new MapZoneSorcererStreet(BR, Map));
            }
        }
    }
}
