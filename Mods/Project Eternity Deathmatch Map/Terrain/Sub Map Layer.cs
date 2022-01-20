using System;
using System.IO;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class SubMapLayer : MapLayer, ISubMapLayer
    {
        public SubMapLayer(DeathmatchMap Map, int LayerIndex)
            : base(Map, LayerIndex)
        {
        }

        public SubMapLayer(DeathmatchMap Map, BinaryReader BR, int LayerIndex)
            : base(Map, BR, LayerIndex)
        {
        }

        public override string ToString()
        {
            return " - Sub Layer";
        }
    }
}
