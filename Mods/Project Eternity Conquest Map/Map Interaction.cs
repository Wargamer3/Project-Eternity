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
        public void CursorControl()
        {
            if (MouseHelper.MouseMoved())
            {
                int NewX = MouseHelper.MouseStateCurrent.X / TileSize.X;
                int NewY = MouseHelper.MouseStateCurrent.Y / TileSize.Y;

                if (NewX < 0)
                    NewX = 0;
                else if (NewX >= MapSize.X)
                    NewX = ScreenSize.X - 1;
                if (NewY < 0)
                    NewY = 0;
                else if (NewY >= MapSize.Y)
                    NewY = ScreenSize.Y - 1;

                NewX += (int)Camera2DPosition.X;
                NewY += (int)Camera2DPosition.Y;

                if (NewX != CursorPosition.X || NewY != CursorPosition.Y)
                {
                    //Update the camera if needed.
                    if (CursorPosition.X - Camera2DPosition.X - 3 < 0 && Camera2DPosition.X > 0)
                        --Camera2DPosition.X;
                    else if (CursorPosition.X - Camera2DPosition.X + 3 >= ScreenSize.X && Camera2DPosition.X + ScreenSize.X < MapSize.X)
                        ++Camera2DPosition.X;

                    if (CursorPosition.Y - Camera2DPosition.Y - 3 < 0 && Camera2DPosition.Y > 0)
                        --Camera2DPosition.Y;
                    else if (CursorPosition.Y - Camera2DPosition.Y + 3 >= ScreenSize.Y && Camera2DPosition.Y + ScreenSize.Y < MapSize.Y)
                        ++Camera2DPosition.Y;

                    CursorPosition.X = NewX;
                    CursorPosition.Y = NewY;
                }
            }
            bool CanKeyboardMove = false;
            if (InputHelper.InputLeftHold() || InputHelper.InputRightHold() || InputHelper.InputUpHold() || InputHelper.InputDownHold())
            {
                CursorHoldTime += 1.5f;

                if (CursorHoldTime <= 1.5f)
                    CanKeyboardMove = true;
                else if (CursorHoldTime >= 8)
                {
                    CanKeyboardMove = true;
                    CursorHoldTime -= 8;
                }
            }
            else
            {
                CursorHoldTime = -1;
            }
            //X
            if (InputHelper.InputLeftHold() && CanKeyboardMove)
            {
                //Update the camera if needed.
                if (CursorPosition.X - Camera2DPosition.X - 3 < 0 && Camera2DPosition.X > 0)
                    --Camera2DPosition.X;

                CursorPosition.X -= (CursorPosition.X > 0) ? 1 : 0;
            }
            else if (InputHelper.InputRightHold() && CanKeyboardMove)
            {
                //Update the camera if needed.
                if (CursorPosition.X - Camera2DPosition.X + 3 >= ScreenSize.X && Camera2DPosition.X + ScreenSize.X < MapSize.X)
                    ++Camera2DPosition.X;

                CursorPosition.X += (CursorPosition.X < MapSize.X - 1) ? 1 : 0;
            }
            //Y
            if (InputHelper.InputUpHold() && CanKeyboardMove)
            {
                //Update the camera if needed.
                if (CursorPosition.Y - Camera2DPosition.Y - 3 < 0 && Camera2DPosition.Y > 0)
                    --Camera2DPosition.Y;

                CursorPosition.Y -= (CursorPosition.Y > 0) ? 1 : 0;
            }
            else if (InputHelper.InputDownHold() && CanKeyboardMove)
            {
                //Update the camera if needed.
                if (CursorPosition.Y - Camera2DPosition.Y + 3 >= ScreenSize.Y && Camera2DPosition.Y + ScreenSize.Y < MapSize.Y)
                    ++Camera2DPosition.Y;

                CursorPosition.Y += (CursorPosition.Y < MapSize.Y - 1) ? 1 : 0;
            }
        }

        public bool UpdateMapNavigation(PlayerInput ActiveInputManager)
        {
            bool CursorMoved = CursorControl(ActiveInputManager);//Move the cursor
            if (CursorMoved)
            {
                foreach (TeleportPoint ActiveTeleport in LayerManager.ListLayer[(int)CursorPosition.Z].ListTeleportPoint)
                {
                    if (ActiveTeleport.Position.X == CursorPosition.X && ActiveTeleport.Position.Y == CursorPosition.Y)
                    {
                        CursorPosition.X = ActiveTeleport.OtherMapEntryPoint.X;
                        CursorPosition.Y = ActiveTeleport.OtherMapEntryPoint.Y;
                        CursorPosition.Z = ActiveTeleport.OtherMapEntryLayer;
                        break;
                    }
                }
            }

            if (InputHelper.InputLButtonPressed())
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
            else if (InputHelper.InputRButtonPressed())
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

        public List<MovementAlgorithmTile> GetAllTerrain(UnitMapComponent ActiveUnit, BattleMap ActiveMap)
        {
            List<MovementAlgorithmTile> ListTerrainFound = new List<MovementAlgorithmTile>();
            for (int X = 0; X < ActiveUnit.ArrayMapSize.GetLength(0); ++X)
            {
                for (int Y = 0; Y < ActiveUnit.ArrayMapSize.GetLength(1); ++Y)
                {
                    if (ActiveUnit.ArrayMapSize[X, Y])
                    {
                        ListTerrainFound.Add(ActiveMap.GetMovementTile((int)ActiveUnit.X + X, (int)ActiveUnit.Y + Y, (int)ActiveUnit.Z));
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
