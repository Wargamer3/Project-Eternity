using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class MovementAlgorithmDeathmatch : MovementAlgorithm
    {
        DeathmatchMap Map;

        public MovementAlgorithmDeathmatch(DeathmatchMap Map)
        {
            this.Map = Map;
        }

        public override float GetMVCost(UnitMapComponent MapComponent, UnitStats UnitStat, MovementAlgorithmTile CurrentNode, MovementAlgorithmTile TerrainToGo)
        {
            float MovementCostToNeighbor = 0;
            if (MapComponent.CurrentMovement == UnitStats.TerrainAir)
            {
                if (Map.GetTerrainLetterAttribute(UnitStat, UnitStats.TerrainAir) == 'C'
                    || Map.GetTerrainLetterAttribute(UnitStat, UnitStats.TerrainAir) == 'D'
                    || Map.GetTerrainLetterAttribute(UnitStat, UnitStats.TerrainAir) == '-')
                {
                    MovementCostToNeighbor += 0.5f;
                }
                else
                {
                    MovementCostToNeighbor += 1;
                }
            }
            else
            {
                string TerrainType = Map.GetTerrainType(TerrainToGo);

                if (!UnitStat.ListTerrainChoices.Contains(TerrainType))
                {
                    return -1;
                }

                char TerrainCharacter = Map.GetTerrainLetterAttribute(UnitStat, TerrainType);

                if ((TerrainCharacter == 'C' || TerrainCharacter == 'D' || TerrainCharacter == '-') && TerrainType != UnitStats.TerrainLand)
                    MovementCostToNeighbor += TerrainToGo.MVMoveCost + 0.5f;
                else if (TerrainCharacter == 'S' && TerrainToGo.MVMoveCost > 1)
                    MovementCostToNeighbor += TerrainToGo.MVMoveCost / 2;
                else
                    MovementCostToNeighbor += TerrainToGo.MVMoveCost;

                if (TerrainToGo.TerrainTypeIndex != GetTile(CurrentNode.Position.X, CurrentNode.Position.Y, MapComponent.LayerIndex).TerrainTypeIndex)
                    MovementCostToNeighbor += TerrainToGo.MVEnterCost;
            }

            return MovementCostToNeighbor;
        }

        public override MovementAlgorithmTile GetTile(float PosX, float PosY, int LayerIndex)
        {
            if (PosX < 0 || PosY < 0 || PosX >= Map.MapSize.X || PosY >= Map.MapSize.Y)
            {
                return null;
            }

            return Map.GetTerrain(PosX, PosY, LayerIndex);
        }
    }
}