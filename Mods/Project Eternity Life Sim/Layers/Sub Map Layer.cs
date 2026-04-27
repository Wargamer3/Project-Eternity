using System;
using System.IO;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class SubMapLayer : MapLayer, ISubMapLayer
    {
        public SubMapLayer(LifeSimMap Map, int LayerIndex)
            : base(Map, LayerIndex)
        {
        }

        public SubMapLayer(LifeSimMap Map, BinaryReader BR, int LayerIndex)
            : base(Map, BR, LayerIndex)
        {
        }

        public override string ToString()
        {
            return " - Sub Layer";
        }
    }
}
