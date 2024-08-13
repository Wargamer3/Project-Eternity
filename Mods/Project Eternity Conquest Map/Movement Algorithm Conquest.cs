using System;
using System.Collections.Generic;
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

        protected override List<MovementAlgorithmTile> AddSuccessor(MovementAlgorithmTile StartingNode, float OffsetX, float OffsetY,
            UnitMapComponent MapComponent, UnitStats Stats, bool IgnoreObstacles)
        {
            List<MovementAlgorithmTile> ListTerrainSuccessor = new List<MovementAlgorithmTile>();
            List<MovementAlgorithmTile> ListLayerPossibility;
            Map.GetNextLayerIndex(StartingNode, (int)OffsetX, (int)OffsetY, 1f, 15, out ListLayerPossibility);

            foreach (MovementAlgorithmTile ActiveDestination in ListLayerPossibility)
            {
                MovementAlgorithmTile ActiveTile = GetTile((int)(StartingNode.WorldPosition.X + OffsetX), (int)(StartingNode.WorldPosition.Y + OffsetY), ActiveDestination.LayerIndex);
                //Wall
                if (ActiveTile == null || ActiveTile.MovementCost == -1
                    || ActiveTile.TerrainTypeIndex == UnitStats.TerrainWallIndex || ActiveTile.TerrainTypeIndex == UnitStats.TerrainVoidIndex)
                {
                    continue;
                }

                //If the NewNode is the parent, skip it.
                if (StartingNode.ParentTemp == null)
                {
                    //Used for an undefined map or if you don't need to calculate the whole map.
                    //ListSuccessors.Add(new AStarNode(ActiveNode, AX, AY));
                    ActiveTile.ParentTemp = StartingNode;
                    ListTerrainSuccessor.Add(ActiveTile);
                }
            }

            return ListTerrainSuccessor;
        }

        public override float GetMVCost(UnitMapComponent MapComponent, UnitStats UnitStat, MovementAlgorithmTile CurrentNode, MovementAlgorithmTile TerrainToGo)
        {
            return 1;
        }

        public override MovementAlgorithmTile GetTile(int PosX, int PosY, int LayerIndex)
        {
            if (PosX < 0 || PosY < 0 || PosX >= Map.MapSize.X || PosY >= Map.MapSize.Y)
            {
                return null;
            }

            return Map.LayerManager.ListLayer[LayerIndex].ArrayTerrain[PosX, PosY];
        }

        public override bool IsBlocked(MovementAlgorithmTile CurrentNode)
        {
            return false;
        }
    }
}