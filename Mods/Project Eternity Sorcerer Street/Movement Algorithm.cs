using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class MovementAlgorithmSorcererStreet : MovementAlgorithm
    {
        SorcererStreetMap Map;
        private const bool AllowGoThroughGround = false;

        public MovementAlgorithmSorcererStreet(SorcererStreetMap Map)
        {
            this.Map = Map;
        }

        protected override List<MovementAlgorithmTile> AddSuccessor(MovementAlgorithmTile StartingNode, float OffsetX, float OffsetY,
            UnitMapComponent MapComponent, UnitStats UnitStat, bool IgnoreObstacles)
        {
            List<MovementAlgorithmTile> ListTerrainSuccessor = new List<MovementAlgorithmTile>();
            List<MovementAlgorithmTile> ListLayerPossibility;
            MovementAlgorithmTile NextRegularMovementDestination = Map.GetNextLayerTile(StartingNode, (int)OffsetX, (int)OffsetY,
                1f, 1, out ListLayerPossibility);

            if (NextRegularMovementDestination == null)
            {
                return ListTerrainSuccessor;
            }

            int NextRegularMovementLayerIndex = NextRegularMovementDestination.LayerIndex;

            foreach (MovementAlgorithmTile ActiveTile in ListLayerPossibility)
            {
                //Wall
                if (ActiveTile == null || !Map.TerrainRestrictions.CanMove(MapComponent, UnitStat, ActiveTile.TerrainTypeIndex) || ActiveTile.MovementCost == -1
                    || ActiveTile.TerrainTypeIndex == UnitStats.TerrainWallIndex || ActiveTile.TerrainTypeIndex == UnitStats.TerrainVoidIndex)
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
            return Map.TerrainRestrictions.GetMVCost(MapComponent, UnitStat, TerrainToGo.TerrainTypeIndex);
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