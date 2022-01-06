﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class MovementAlgorithmSorcererStreet : MovementAlgorithm
    {
        SorcererStreetMap Map;

        public MovementAlgorithmSorcererStreet(SorcererStreetMap Map)
        {
            this.Map = Map;
        }

        protected override List<MovementAlgorithmTile> AddSuccessor(MovementAlgorithmTile ActiveNode, float OffsetX, float OffsetY, int LayerIndex)
        {
            List<MovementAlgorithmTile> ListTerrainSuccessor = new List<MovementAlgorithmTile>();
            List<int> ListLayerPossibility;
            Map.GetNextLayerIndex(new Vector3(ActiveNode.Position.X, ActiveNode.Position.Y, LayerIndex), (int)OffsetX, (int)OffsetY, 1f, 15, out ListLayerPossibility);

            foreach (int ActiveLayerIndex in ListLayerPossibility)
            {
                MovementAlgorithmTile ActiveTile = GetTile(ActiveNode.Position.X + OffsetX, ActiveNode.Position.X + OffsetY, ActiveLayerIndex);
                //Wall
                if (ActiveTile == null || ActiveTile.MVEnterCost == -1 || ActiveTile.MovementCost == -1
                    || ActiveTile.TerrainTypeIndex == UnitStats.TerrainWallIndex || ActiveTile.TerrainTypeIndex == UnitStats.TerrainVoidIndex)
                {
                    continue;
                }

                //If the NewNode is the parent, skip it.
                if (ActiveNode.ParentTemp == null)
                {
                    //Used for an undefined map or if you don't need to calculate the whole map.
                    //ListSuccessors.Add(new AStarNode(ActiveNode, AX, AY));
                    ActiveTile.ParentTemp = ActiveNode;
                    ListTerrainSuccessor.Add(ActiveTile);
                }
            }

            return ListTerrainSuccessor;
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