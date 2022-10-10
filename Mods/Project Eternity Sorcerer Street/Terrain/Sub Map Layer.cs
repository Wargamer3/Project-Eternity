using System;
using System.IO;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class SubMapLayer : MapLayer, ISubMapLayer
    {
        public SubMapLayer(SorcererStreetMap Map, int LayerIndex)
            : base(Map, LayerIndex)
        {
        }

        public SubMapLayer(SorcererStreetMap Map, BinaryReader BR, int LayerIndex)
            : base(Map, BR, LayerIndex)
        {
        }

        public override string ToString()
        {
            return " - Sub Layer";
        }
    }
}
