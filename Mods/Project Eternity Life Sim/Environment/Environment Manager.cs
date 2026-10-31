using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class EnvironmentManagerLifeSim : EnvironmentManager
    {
        public EnvironmentManagerLifeSim(LifeSimMap Map)
            : base()
        {
            GlobalZone = new MapZoneLifeSim(Map, ZoneShape.ZoneShapeTypes.Full);
        }

        public EnvironmentManagerLifeSim(BinaryReader BR, LifeSimMap Map)
            : base(BR)
        {
            GlobalZone = new MapZoneLifeSim(BR, Map);

            int ListMapZoneCount = BR.ReadInt32();
            ListMapZone = new List<MapZone>(ListMapZoneCount);
            for (int Z = 0; Z < ListMapZoneCount; ++Z)
            {
                ListMapZone.Add(new MapZoneLifeSim(BR, Map));
            }
        }

        protected override void AdvanceClock(GameTime gameTime)
        {
        }
    }
}
