using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class MovementAlgorithmConquest : MovementAlgorithm
    {
        private readonly ConquestMap Map;
        private const bool AllowGoThroughGround = false;

        public MovementAlgorithmConquest(ConquestMap Map)
        {
            this.Map = Map;
        }

        protected override List<MovementAlgorithmTile> AddSuccessor(MovementAlgorithmTile StartingNode, float GridOffsetX, float GridOffsetY,
            UnitMapComponent MapComponent, UnitStats UnitStat, bool IgnoreObstacles)
        {
            List<MovementAlgorithmTile> ListTerrainSuccessor = new List<MovementAlgorithmTile>();
            List<MovementAlgorithmTile> ListLayerPossibility;
            MovementAlgorithmTile NextRegularMovementDestination = Map.GetTerrain(Map.GetNextLayerTile(StartingNode, GridOffsetX * Map.TileSize.X, GridOffsetY * Map.TileSize.Y, 1f, 1, out ListLayerPossibility));

            if (NextRegularMovementDestination == StartingNode)
            {
                return ListTerrainSuccessor;
            }

            int NextRegularMovementLayerIndex = NextRegularMovementDestination.LayerIndex;

            foreach (MovementAlgorithmTile ActiveTile in ListLayerPossibility)
            {
                //Wall
                if (ActiveTile == null || ActiveTile.TerrainTypeIndex >= Map.TerrainHolder.ListConquestTerrainType.Count
                    || !Map.TerrainHolder.ListConquestTerrainType[ActiveTile.TerrainTypeIndex].DicMovementCostByMoveType.ContainsKey(UnitStat.UnitTypeIndex)
                    || Map.TerrainHolder.ListConquestTerrainType[ActiveTile.TerrainTypeIndex].DicMovementCostByMoveType[UnitStat.UnitTypeIndex] == 0 
                    || ActiveTile.MovementCost == -1)
                {
                    continue;
                }

                if (!AllowGoThroughGround && ActiveTile.LayerIndex < NextRegularMovementLayerIndex && ActiveTile.LayerIndex != NextRegularMovementLayerIndex && ListLayerPossibility.Contains(StartingNode))
                {
                    continue;
                }

                foreach (TeleportPoint ActiveTeleport in Map.LayerManager.ListLayer[ActiveTile.LayerIndex].ListTeleportPoint)
                {
                    if (ActiveTeleport.Position.X == ActiveTile.WorldPosition.X && ActiveTeleport.Position.Y == ActiveTile.WorldPosition.Y)
                    {
                        ListTerrainSuccessor.Add(GetTile(ActiveTeleport.OtherMapEntryPoint.X, ActiveTeleport.OtherMapEntryPoint.Y, ActiveTeleport.OtherMapEntryLayer));
                        break;
                    }
                }

                if (IgnoreObstacles
                    || (StartingNode.WorldPosition.X == MapComponent.X && StartingNode.WorldPosition.Y == MapComponent.Y && StartingNode.LayerIndex == MapComponent.Z)
                    || !ActiveTile.Owner.CheckForObstacleAtPosition(new Vector3(ActiveTile.WorldPosition.X, ActiveTile.WorldPosition.Y, ActiveTile.LayerIndex), Vector3.Zero))
                {
                    ListTerrainSuccessor.Add(ActiveTile);
                }
            }

            return ListTerrainSuccessor;
        }

        public override float GetMVCost(UnitMapComponent MapComponent, UnitStats UnitStat, MovementAlgorithmTile CurrentNode, MovementAlgorithmTile TerrainToGo)
        {
            return Map.GetMVCost(MapComponent, UnitStat, TerrainToGo.TerrainTypeIndex);
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