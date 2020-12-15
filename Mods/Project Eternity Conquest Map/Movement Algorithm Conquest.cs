using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class MovementAlgorithmConquest : MovementAlgorithm
    {
        ConquestMap Map;

        public MovementAlgorithmConquest(ConquestMap Map)
        {
            this.Map = Map;
        }

        public override float GetMVCost(UnitMapComponent MapComponent, UnitStats UnitStat, MovementAlgorithmTile CurrentNode, MovementAlgorithmTile TerrainToGo)
        {
            return TerrainToGo.MVMoveCost;
        }

        public override MovementAlgorithmTile GetTile(float PosX, float PosY, int LayerIndex)
        {
            if (PosX < 0 || PosY < 0 || PosX >= Map.MapSize.X || PosY >= Map.MapSize.Y)
            {
                return null;
            }

            return Map.ListLayer[LayerIndex].ArrayTerrain[(int)PosX, (int)PosY];
        }
    }
}