using System.Collections.Generic;
using Microsoft.Xna.Framework;
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

        protected override List<MovementAlgorithmTile> AddSuccessor(MovementAlgorithmTile ActiveNode, float OffsetX, float OffsetY, int LayerIndex)
        {
            List<MovementAlgorithmTile> ListTerrainSuccessor = new List<MovementAlgorithmTile>();
            List<int> ListLayerPossibility;
            Map.GetNextLayerIndex(new Vector3(ActiveNode.Position.X, ActiveNode.Position.Y, LayerIndex), (int)(ActiveNode.Position.X + OffsetX), (int)(ActiveNode.Position.Y + OffsetY), 1f, 15, out ListLayerPossibility);

            foreach (int ActiveLayerIndex in ListLayerPossibility)
            {
                if (Map.CheckForObstacleAtPosition(new Vector3(ActiveNode.Position.X + OffsetX, ActiveNode.Position.Y + OffsetY, ActiveLayerIndex), Vector3.Zero))
                {
                    continue;
                }

                MovementAlgorithmTile ActiveTile = GetTile(ActiveNode.Position.X + OffsetX, ActiveNode.Position.Y + OffsetY, ActiveLayerIndex);
                //Wall
                if (ActiveTile == null || ActiveTile.MVEnterCost == -1 || ActiveTile.MovementCost == -1
                    || ActiveTile.TerrainTypeIndex == UnitStats.TerrainWallIndex || ActiveTile.TerrainTypeIndex == UnitStats.TerrainVoidIndex)
                {
                    continue;
                }

                //If the NewNode is the parent, skip it.
                //Used for an undefined map or if you don't need to calculate the whole map.
                //ListSuccessors.Add(new AStarNode(ActiveNode, AX, AY));
                ActiveTile.LayerIndex = ActiveLayerIndex;
                ListTerrainSuccessor.Add(ActiveTile);
            }

            return ListTerrainSuccessor;
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

                if (TerrainToGo.TerrainTypeIndex != GetTile(CurrentNode.Position.X, CurrentNode.Position.Y, (int)MapComponent.Z).TerrainTypeIndex)
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