using System;
using System.IO;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class SubMapLayer : MapLayer, ISubMapLayer
    {
        public SubMapLayer(DeathmatchMap Map)
            : base(Map)
        {
        }

        public SubMapLayer(DeathmatchMap Map, BinaryReader BR)
            : base(Map, BR)
        {
        }

        public override string ToString()
        {
            return " - Sub Layer";
        }
    }
}
