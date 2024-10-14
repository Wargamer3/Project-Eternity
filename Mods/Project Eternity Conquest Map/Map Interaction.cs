using System;
using System.Collections.Generic;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    partial class ConquestMap
    {
        /// <summary>
        /// Move the cursor on the map.
        /// </summary>
        /// <returns>Returns true if the cursor was moved</returns>
        public override bool CursorControl(PlayerInput ActiveInputManager)
        {
            Point Offset;
            BattleMap ActiveMap;

            bool CursorMoved = CursorControlGrid(ActiveInputManager, out Offset, out ActiveMap);

            if (CursorMoved)
            {
                MovementAlgorithmTile NextTerrain = GetNextLayerTile(ActiveMap.CursorTerrain, Offset.X, Offset.Y, 1f, 15f, out _);
                if (NextTerrain == ActiveMap.CursorTerrain)//Force movement
                {
                    ActiveMap.CursorPosition.Z = NextTerrain.LayerIndex;
                    ActiveMap.CursorPosition.X = Math.Max(0, Math.Min(ActiveMap.MapSize.X - 1, NextTerrain.GridPosition.X + Offset.X));
                    ActiveMap.CursorPosition.Y = Math.Max(0, Math.Min(ActiveMap.MapSize.Y - 1, NextTerrain.GridPosition.Y + Offset.Y));
                }
                else
                {
                    ActiveMap.CursorPosition.Z = NextTerrain.LayerIndex;
                    ActiveMap.CursorPosition.X = NextTerrain.GridPosition.X;
                    ActiveMap.CursorPosition.Y = NextTerrain.GridPosition.Y;
                }
            }

            return CursorMoved;
        }

        public override Vector3 GetFinalPosition(Vector3 WorldPosition)
        {
            int GridX = (int)WorldPosition.X / TileSize.X;
            int GridY = (int)WorldPosition.X / TileSize.Y;
            int LayerIndex = (int)WorldPosition.Z / LayerHeight;

            Terrain ActiveTerrain = LayerManager.ListLayer[LayerIndex].ArrayTerrain[GridX, GridY];
            DrawableTile ActiveTile = LayerManager.ListLayer[LayerIndex].ArrayTile[GridX, GridY];

            Vector2 PositionInTile = new Vector2(WorldPosition.X - ActiveTerrain.WorldPosition.X, WorldPosition.Y - ActiveTerrain.WorldPosition.Y);

            return WorldPosition + new Vector3(PositionInTile, ActiveTile.Terrain3DInfo.GetZOffset(PositionInTile, ActiveTerrain.Height));
        }

        public List<MovementAlgorithmTile> GetAllTerrain(UnitMapComponent ActiveUnit, ConquestMap ActiveMap)
        {
            List<MovementAlgorithmTile> ListTerrainFound = new List<MovementAlgorithmTile>();
            for (int X = 0; X < ActiveUnit.ArrayMapSize.GetLength(0); ++X)
            {
                for (int Y = 0; Y < ActiveUnit.ArrayMapSize.GetLength(1); ++Y)
                {
                    if (ActiveUnit.ArrayMapSize[X, Y])
                    {
                        ListTerrainFound.Add(ActiveMap.LayerManager.ListLayer[(int)ActiveUnit.Z].ArrayTerrain[(int)ActiveUnit.X + X, (int)ActiveUnit.Y + Y]);
                    }
                }
            }

            return ListTerrainFound;
        }

        public List<Vector3> GetPathToTerrain(MovementAlgorithmTile ActiveTerrain, Vector3 Position)
        {
            List<MovementAlgorithmTile> ListMovement = new List<MovementAlgorithmTile>();
            MovementAlgorithmTile FinalTile = ActiveTerrain;
            while (FinalTile != null)
            {
                if (ListMovement.Contains(FinalTile.ParentReal))
                {
                    break;
                }
                if (Position.X == FinalTile.WorldPosition.X && Position.Y == FinalTile.WorldPosition.Y && Position.Z == FinalTile.LayerIndex)
                {
                    ListMovement.Add(FinalTile);
                    break;
                }

                ListMovement.Add(FinalTile);

                FinalTile = FinalTile.ParentReal;
            }

            ListMovement.Reverse();
            List<Vector3> ListMVHoverPoints = new List<Vector3>();
            foreach (MovementAlgorithmTile ActiveTile in ListMovement)
            {
                ListMVHoverPoints.Add(new Vector3(ActiveTile.WorldPosition.X, ActiveTile.WorldPosition.Y, ActiveTile.LayerIndex));
            }

            return ListMVHoverPoints;
        }
    }
}
