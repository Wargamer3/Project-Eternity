﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class MovementAlgorithmDeathmatch : MovementAlgorithm
    {
        private readonly DeathmatchMap Map;
        private const bool AllowGoThroughGround = false;

        public MovementAlgorithmDeathmatch(DeathmatchMap Map)
        {
            this.Map = Map;
        }

        protected override List<MovementAlgorithmTile> AddSuccessor(MovementAlgorithmTile ActiveNode, float OffsetX, float OffsetY, int StartingLayerIndex,
            UnitMapComponent MapComponent, bool IgnoreObstacles)
        {
            List<MovementAlgorithmTile> ListTerrainSuccessor = new List<MovementAlgorithmTile>();
            List<int> ListLayerPossibility;
            int NextRegularMovementLayerIndex = Map.GetNextLayerIndex(new Vector3(ActiveNode.Position.X, ActiveNode.Position.Y, StartingLayerIndex), (int)(ActiveNode.Position.X + OffsetX), (int)(ActiveNode.Position.Y + OffsetY),
                1f, 15, out ListLayerPossibility);

            foreach (int ActiveLayerIndex in ListLayerPossibility)
            {
                MovementAlgorithmTile ActiveTile = GetTile(ActiveNode.Position.X + OffsetX, ActiveNode.Position.Y + OffsetY, ActiveLayerIndex);
                //Wall
                if (ActiveTile == null || ActiveTile.MVEnterCost == -1 || ActiveTile.MovementCost == -1
                    || ActiveTile.TerrainTypeIndex == UnitStats.TerrainWallIndex || ActiveTile.TerrainTypeIndex == UnitStats.TerrainVoidIndex)
                {
                    continue;
                }

                if (!AllowGoThroughGround && ActiveLayerIndex < NextRegularMovementLayerIndex && ActiveLayerIndex != NextRegularMovementLayerIndex && ListLayerPossibility.Contains(StartingLayerIndex))
                {
                    continue;
                }

                foreach (TeleportPoint ActiveTeleport in Map.LayerManager.ListLayer[ActiveLayerIndex].ListTeleportPoint)
                {
                    if (ActiveTeleport.Position.X == ActiveTile.Position.X && ActiveTeleport.Position.Y == ActiveTile.Position.Y)
                    {
                        ListTerrainSuccessor.Add(GetTile(ActiveTeleport.OtherMapEntryPoint.X, ActiveTeleport.OtherMapEntryPoint.Y, ActiveTeleport.OtherMapEntryLayer));
                        break;
                    }
                }

                if (IgnoreObstacles
                    || (ActiveNode.Position.X == MapComponent.X && ActiveNode.Position.Y == MapComponent.Y && ActiveNode.LayerIndex == MapComponent.Z)
                    || !Map.CheckForObstacleAtPosition(new Vector3(ActiveTile.Position.X, ActiveTile.Position.Y, ActiveLayerIndex), Vector3.Zero))
                {
                    ListTerrainSuccessor.Add(ActiveTile);
                }
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

        public override bool IsBlocked(MovementAlgorithmTile CurrentNode)
        {
            for (int P = 0; P < Map.ListPlayer.Count; P++)
            {
                int SquadIndex = Map.CheckForSquadAtPosition(P, new Vector3(CurrentNode.Position.X, CurrentNode.Position.Y, CurrentNode.LayerIndex), Vector3.Zero);
                if (SquadIndex >= 0)
                    return true;
            }

            return false;
        }
    }
}