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
            Point GridOffset;
            BattleMap ActiveMap;

            bool CursorMoved = CursorControlGrid(ActiveInputManager, out GridOffset, out ActiveMap);

            if (CursorMoved)
            {
                Vector3 NextTerrain = GetNextLayerTile(ActiveMap.CursorTerrain, GridOffset.X, GridOffset.Y, 1f, 15f, out _);

                if (NextTerrain == ActiveMap.CursorTerrain.WorldPosition)//Force movement
                {
                    ActiveMap.CursorPosition.Z = NextTerrain.Z;
                    ActiveMap.CursorPosition.X = Math.Max(0, Math.Min((ActiveMap.MapSize.X - 1) * TileSize.X, NextTerrain.X + GridOffset.X * TileSize.X));
                    ActiveMap.CursorPosition.Y = Math.Max(0, Math.Min((ActiveMap.MapSize.Y - 1) * TileSize.Y, NextTerrain.Y + GridOffset.Y * TileSize.Y));
                }
                else
                {
                    ActiveMap.CursorPosition = NextTerrain;
                }

                ActiveMap.CursorPosition += new Vector3(TileSize.X / 2, TileSize.Y / 2, 0);

                foreach (TeleportPoint ActiveTeleport in LayerManager.ListLayer[(int)CursorPosition.Z].ListTeleportPoint)
                {
                    if (ActiveTeleport.Position.X == CursorPosition.X && ActiveTeleport.Position.Y == CursorPosition.Y)
                    {
                        CursorPosition.X = ActiveTeleport.OtherMapEntryPoint.X * TileSize.X;
                        CursorPosition.Y = ActiveTeleport.OtherMapEntryPoint.Y * TileSize.X;
                        CursorPosition.Z = ActiveTeleport.OtherMapEntryLayer * LayerHeight;
                        break;
                    }
                }
            }

            if (ActiveInputManager.InputLButtonPressed())
            {
                if (ListPlayer[ActivePlayerIndex].ListUnit.Count == 0)
                    return CursorMoved;

                int UnitIndex = 0;
                if (ActiveUnit != null)
                    UnitIndex = ListPlayer[ActivePlayerIndex].ListUnit.IndexOf(ActiveUnit);

                int StartIndex = UnitIndex;
                bool UnmovedSquadFound = false;

                do
                {
                    ++UnitIndex;

                    if (UnitIndex >= ListPlayer[ActivePlayerIndex].ListUnit.Count)
                        UnitIndex = 0;

                    if (ListPlayer[ActivePlayerIndex].ListUnit[UnitIndex].HP > 0 && ListPlayer[ActivePlayerIndex].ListUnit[UnitIndex].CanMove)
                    {
                        UnmovedSquadFound = true;
                    }
                }
                while (StartIndex != UnitIndex && !UnmovedSquadFound);

                if (!UnmovedSquadFound)
                {
                    do
                    {
                        if (++UnitIndex >= ListPlayer[ActivePlayerIndex].ListUnit.Count)
                            UnitIndex = 0;
                    }
                    while (ListPlayer[ActivePlayerIndex].ListUnit[UnitIndex].HP == 0);
                }

                ActiveUnitIndex = UnitIndex;
                CursorPosition = ActiveUnit.Position;
                CursorPositionVisible = CursorPosition;

                if (ActiveUnit.X < Camera2DPosition.X || ActiveUnit.Y < Camera2DPosition.Y ||
                    ActiveUnit.X >= Camera2DPosition.X + ScreenSize.X || ActiveUnit.Y >= Camera2DPosition.Y + ScreenSize.Y)
                {
                    PushScreen(new CenterOnSquadCutscene(CenterCamera, this, ActiveUnit.Position));
                }
            }
            else if (ActiveInputManager.InputRButtonPressed())
            {
                if (ListPlayer[ActivePlayerIndex].ListUnit.Count == 0)
                    return CursorMoved;

                int UnitIndex = 0;
                if (ActiveUnit != null)
                    UnitIndex = ListPlayer[ActivePlayerIndex].ListUnit.IndexOf(ActiveUnit);
                int StartIndex = UnitIndex;
                bool UnmovedSquadFound = false;

                do
                {
                    --UnitIndex;

                    if (UnitIndex < 0)
                        UnitIndex = ListPlayer[ActivePlayerIndex].ListUnit.Count - 1;

                    if (ListPlayer[ActivePlayerIndex].ListUnit[UnitIndex].HP > 0 && ListPlayer[ActivePlayerIndex].ListUnit[UnitIndex].CanMove)
                    {
                        UnmovedSquadFound = true;
                    }
                }
                while (StartIndex != UnitIndex && !UnmovedSquadFound);

                if (!UnmovedSquadFound)
                {
                    do
                    {
                        if (--UnitIndex < 0)
                            UnitIndex = ListPlayer[ActivePlayerIndex].ListUnit.Count - 1;
                    }
                    while (ListPlayer[ActivePlayerIndex].ListUnit[UnitIndex].HP == 0);
                }

                ActiveUnitIndex = UnitIndex;
                CursorPosition = ActiveUnit.Position;
                CursorPositionVisible = CursorPosition;

                if (ActiveUnit.X < Camera2DPosition.X || ActiveUnit.Y < Camera2DPosition.Y ||
                    ActiveUnit.X >= Camera2DPosition.X + ScreenSize.X || ActiveUnit.Y >= Camera2DPosition.Y + ScreenSize.Y)
                {
                    PushScreen(new CenterOnSquadCutscene(CenterCamera, this, ActiveUnit.Position));
                }
            }
            return CursorMoved;
        }

        public override Vector3 GetFinalPosition(Vector3 WorldPosition)
        {
            int GridX = (int)WorldPosition.X / TileSize.X;
            int GridY = (int)WorldPosition.Y / TileSize.Y;
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
                        ListTerrainFound.Add(ActiveMap.GetTerrain(new Vector3(ActiveUnit.Position.X + X * TileSize.X, ActiveUnit.Position.Y + Y * TileSize.Y, ActiveUnit.Position.Z)));
                    }
                }
            }

            return ListTerrainFound;
        }

        public override Vector3 GetNextPosition(Vector3 WorldPosition, Vector3 Movement)
        {
            return GetFinalPosition(new Vector3(WorldPosition.X, WorldPosition.Y, GetNextLayerTile(GetTerrain(WorldPosition), (int)(Movement.X / TileSize.X), (int)(Movement.Y / TileSize.Y), 1f, 15f, out _).Z));
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
