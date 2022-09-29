using ProjectEternity.Core.Units;
using ProjectEternity.Core.ControlHelper;
using System;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Units.Conquest;

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

                NewX += (int)CameraPosition.X;
                NewY += (int)CameraPosition.Y;

                if (NewX != CursorPosition.X || NewY != CursorPosition.Y)
                {
                    //Update the camera if needed.
                    if (CursorPosition.X - CameraPosition.X - 3 < 0 && CameraPosition.X > 0)
                        --CameraPosition.X;
                    else if (CursorPosition.X - CameraPosition.X + 3 >= ScreenSize.X && CameraPosition.X + ScreenSize.X < MapSize.X)
                        ++CameraPosition.X;

                    if (CursorPosition.Y - CameraPosition.Y - 3 < 0 && CameraPosition.Y > 0)
                        --CameraPosition.Y;
                    else if (CursorPosition.Y - CameraPosition.Y + 3 >= ScreenSize.Y && CameraPosition.Y + ScreenSize.Y < MapSize.Y)
                        ++CameraPosition.Y;

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
                if (CursorPosition.X - CameraPosition.X - 3 < 0 && CameraPosition.X > 0)
                    --CameraPosition.X;

                CursorPosition.X -= (CursorPosition.X > 0) ? 1 : 0;
            }
            else if (InputHelper.InputRightHold() && CanKeyboardMove)
            {
                //Update the camera if needed.
                if (CursorPosition.X - CameraPosition.X + 3 >= ScreenSize.X && CameraPosition.X + ScreenSize.X < MapSize.X)
                    ++CameraPosition.X;

                CursorPosition.X += (CursorPosition.X < MapSize.X - 1) ? 1 : 0;
            }
            //Y
            if (InputHelper.InputUpHold() && CanKeyboardMove)
            {
                //Update the camera if needed.
                if (CursorPosition.Y - CameraPosition.Y - 3 < 0 && CameraPosition.Y > 0)
                    --CameraPosition.Y;

                CursorPosition.Y -= (CursorPosition.Y > 0) ? 1 : 0;
            }
            else if (InputHelper.InputDownHold() && CanKeyboardMove)
            {
                //Update the camera if needed.
                if (CursorPosition.Y - CameraPosition.Y + 3 >= ScreenSize.Y && CameraPosition.Y + ScreenSize.Y < MapSize.Y)
                    ++CameraPosition.Y;

                CursorPosition.Y += (CursorPosition.Y < MapSize.Y - 1) ? 1 : 0;
            }
        }

        /// <summary>
        /// Called every time a player has finished his actions.
        /// </summary>
        public void OnNewPhase()
        {
            ListActionMenuChoice.RemoveAllActionPanels();
            if (FMODSystem.sndActiveBGMName != sndBattleThemeName && !string.IsNullOrEmpty(sndBattleThemeName))
            {
                sndBattleTheme.Stop();
                sndBattleTheme.SetLoop(true);
                sndBattleTheme.PlayAsBGM();
                FMODSystem.sndActiveBGMName = sndBattleThemeName;
            }

            do
            {
                ActivePlayerIndex++;
                if (ActivePlayerIndex >= ListPlayer.Count)
                {
                    OnNewTurn();
                }
            }
            while (!ListPlayer[ActivePlayerIndex].IsAlive);
            
            for (int U = 0; U < ListPlayer[ActivePlayerIndex].ListUnit.Count; U++)
            {
                UnitConquest ActiveUnit = ListPlayer[ActivePlayerIndex].ListUnit[U];
                ActivateAutomaticSkills(null, ListPlayer[ActivePlayerIndex].ListUnit[U], "Player Phase Start Requirement", null, ListPlayer[ActivePlayerIndex].ListUnit[U]);

                //Repair passive bonus.
                if (ActiveUnit.Boosts.RepairModifier)
                {
                    ActiveUnit.HealUnit((int)(ActiveUnit.MaxHP * 0.05));
                }

                //Resupply passive bonus.
                if (ActiveUnit.Boosts.ResupplyModifier)
                {
                    ActiveUnit.RefillEN((int)(ActiveUnit.MaxEN * 0.05));
                }
            }
        }

        /// <summary>
        /// Called every time every players has finished their actions.
        /// </summary>
        protected void OnNewTurn()
        {
            for (int P = 0; P < ListPlayer.Count; P++)
            {
                for (int U = 0; U < ListPlayer[P].ListUnit.Count; U++)
                {
                    UnitConquest ActiveUnit = ListPlayer[P].ListUnit[U];

                    //Remove 5 EN each time the Squad spend a turn in the air.
                    if (ActiveUnit.CurrentMovement == Core.Units.UnitStats.TerrainAirIndex)
                        ActiveUnit.ConsumeEN(5);
                }
            }

            UpdateMapEvent(EventTypeTurn, 0);

            ActivePlayerIndex = 0;
            GameTurn++;

            for (int P = 0; P < ListPlayer.Count; P++)
            {
                for (int U = 0; U < ListPlayer[P].ListUnit.Count; U++)
                {
                    UnitConquest ActiveUnit = ListPlayer[P].ListUnit[U];

                    ActiveUnit.StartTurn();

                    ActiveUnit.UpdateSkillsLifetime(SkillEffect.LifetimeTypeTurns);
                }
            }
        }
    }
}
