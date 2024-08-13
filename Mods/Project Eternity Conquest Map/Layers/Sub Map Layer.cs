using System;
using System.IO;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class SubMapLayer : MapLayer, ISubMapLayer
    {
        public SubMapLayer(ConquestMap Map, int LayerIndex)
            : base(Map, LayerIndex)
        {
        }

        public SubMapLayer(ConquestMap Map, BinaryReader BR, int LayerIndex)
            : base(Map, BR, LayerIndex)
        {
        }

        public override string ToString()
        {
            return " - Sub Layer";
        }
    }
}
