using System;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.UI;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    class InventoryCharacterScreen : GameScreen
    {
        private BoxScrollbar SquadListScrollbar;
        private BoxScrollbar InventoryScrollbar;

        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;

        private readonly BattleMapInventoryScreen Owner;
        private const int SquadHeight = 100;
        private const int BoxHeight = 50;

        private int SquadScrollbarValue;
        private int InventoryScrollbarValue;

        private Character DragAndDropEquipment;

        protected bool IsDragDropActive { get { return DragAndDropEquipment != null; } }

        public InventoryCharacterScreen(BattleMapInventoryScreen Owner)
        {
            this.Owner = Owner;
        }

        public override void Load()
        {
            SquadListScrollbar = new BoxScrollbar(new Vector2(BattleMapInventoryScreen.LeftSideWidth - 23, BattleMapInventoryScreen.MiddleSectionY + 3), BattleMapInventoryScreen.MiddleSectionHeight - 5, 10, OnSquadScrollbarChange);
            InventoryScrollbar = new BoxScrollbar(new Vector2(Constants.Width - 23, BattleMapInventoryScreen.MiddleSectionY + 3), BattleMapInventoryScreen.MiddleSectionHeight - 5, 10, OnInventoryScrollbarChange);

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            SquadListScrollbar.ChangeMaxValue(Owner.ActivePlayer.Inventory.ListSquadLoadout.Count * SquadHeight - BattleMapInventoryScreen.MiddleSectionHeight);
            InventoryScrollbar.ChangeMaxValue(Owner.ActivePlayer.Inventory.ListOwnedSquad.Count * BoxHeight - BattleMapInventoryScreen.MiddleSectionHeight);
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsDragDropActive)
            {
                SquadListScrollbar.Update(gameTime);
                InventoryScrollbar.Update(gameTime);
                UpdateEquipmentPage();
            }
            else
            {
                DoDragDrop();
            }
        }

        private void UpdateEquipmentPage()
        {
            if (MouseHelper.InputLeftButtonPressed())
            {
                Character SelectedEquipment = GetOwnedCharacterUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);
                if (SelectedEquipment != null)
                {
                    StartDragDrop(SelectedEquipment);
                }
            }
        }

        private void StartDragDrop(Character EquipmentToDrag)
        {
            DragAndDropEquipment = EquipmentToDrag;
        }

        private void DoDragDrop()
        {
            int DrawX = 101;
            int DrawY = BattleMapInventoryScreen.MiddleSectionY + 5 + 4 - SquadScrollbarValue;

            if (InputHelper.InputConfirmReleased())
            {
                int X = (MouseHelper.MouseStateCurrent.X - DrawX) / 40;
                int Y = (MouseHelper.MouseStateCurrent.Y - DrawY) / SquadHeight;

                int SquadSlotIndex = X;
                int SqaudLoatoutIndex = Y;
                int MouseX = MouseHelper.MouseStateCurrent.X;
                int MouseY = MouseHelper.MouseStateCurrent.Y;

                if (SqaudLoatoutIndex < Owner.ActivePlayer.Inventory.ListSquadLoadout.Count && SquadSlotIndex < Owner.ActivePlayer.Inventory.ListSquadLoadout[SqaudLoatoutIndex].ListSpawnSquad.Count
                    && Owner.ActivePlayer.Inventory.ListSquadLoadout[SqaudLoatoutIndex].ListSpawnSquad[SquadSlotIndex] != null)
                {
                    if (MouseX >= DrawX + X * 40 && MouseX < DrawX + X * 40 + 38
                        && MouseY >= DrawY + 48 + Y * SquadHeight && MouseY < DrawY + 48 + 38 + Y * SquadHeight)
                    {
                        Owner.ActivePlayer.Inventory.ListSquadLoadout[SqaudLoatoutIndex].ListSpawnSquad[SquadSlotIndex].CurrentLeader.ArrayCharacterActive[0] = DragAndDropEquipment;
                        Owner.ActivePlayer.SaveLocally();
                    }
                }

                DragAndDropEquipment = null;
            }
        }

        private Character GetOwnedCharacterUnderMouse(int MouseX, int MouseY)
        {
            int DrawX = BattleMapInventoryScreen.LeftSideWidth + 5;
            int DrawY = BattleMapInventoryScreen.MiddleSectionY + 5 + 4 - InventoryScrollbarValue;

            if (MouseX >= DrawX && MouseX < DrawX + 38 && MouseY >= DrawY && MouseY < BattleMapInventoryScreen.MiddleSectionHeight)
            {
                int Y = (MouseY - DrawY) / BoxHeight;

                int SquadIndex = Y;
                if (SquadIndex < Owner.ActivePlayer.Inventory.ListOwnedCharacter.Count)
                {
                    if (MouseX >= BattleMapInventoryScreen.LeftSideWidth + 11 && MouseX < BattleMapInventoryScreen.LeftSideWidth + 11 + 38
                        && MouseY >= DrawY + Y * BoxHeight && MouseY < DrawY + Y * BoxHeight + 38)
                    {
                        return Owner.ActivePlayer.Inventory.ListOwnedCharacter[SquadIndex];
                    }
                }
            }

            return null;
        }

        private void OnSquadScrollbarChange(float ScrollbarValue)
        {
            SquadScrollbarValue = (int)ScrollbarValue;
        }

        private void OnInventoryScrollbarChange(float ScrollbarValue)
        {
            InventoryScrollbarValue = (int)ScrollbarValue;
        }

        public override void Draw(CustomSpriteBatch g)
        {
            SquadListScrollbar.Draw(g);
            InventoryScrollbar.Draw(g);

            //Left side
            g.DrawString(fntArial12, "Loadouts", new Vector2(10, BattleMapInventoryScreen.TopSectionHeight + 5), Color.White);

            int DrawY = BattleMapInventoryScreen.MiddleSectionY + 5 - SquadScrollbarValue % SquadHeight;
            for (int L = (int)Math.Floor(SquadScrollbarValue / (double)SquadHeight); L < Owner.ActivePlayer.Inventory.ListSquadLoadout.Count; ++L)
            {
                DrawBox(g, new Vector2(5, DrawY), BattleMapInventoryScreen.LeftSideWidth - 30, 90, Color.White);
                DrawBox(g, new Vector2(BattleMapInventoryScreen.LeftSideWidth - 115, DrawY + 5), 85, 35, Color.White);
                g.DrawString(fntArial12, "Rename", new Vector2(BattleMapInventoryScreen.LeftSideWidth - 108, DrawY + 11), Color.White);
                g.DrawString(fntArial12, "Loadout " + (L + 1), new Vector2(11, DrawY + 11), Color.White);

                int DrawX = 101;
                for (int S = 0; S < Owner.ActivePlayer.Inventory.ListSquadLoadout[L].ListSpawnSquad.Count; S++)
                {
                    DrawBox(g, new Vector2(DrawX, DrawY + 4), 38, 38, Color.White);
                    DrawBox(g, new Vector2(DrawX, DrawY + 45), 38, 38, Color.White);

                    Squad ActiveSquad = Owner.ActivePlayer.Inventory.ListSquadLoadout[L].ListSpawnSquad[S];
                    if (ActiveSquad != null)
                    {
                        g.Draw(ActiveSquad.CurrentLeader.SpriteMap, new Vector2(DrawX + 3, DrawY + 7), Color.White);
                        if (ActiveSquad.CurrentLeader.Pilot != null)
                        {
                            g.Draw(ActiveSquad.CurrentLeader.Pilot.sprPortrait, new Rectangle(DrawX + 3, DrawY + 48, 32, 32), Color.White);
                        }
                    }

                    if (DragAndDropEquipment != null)
                    {
                        g.Draw(sprPixel, new Rectangle(DrawX, DrawY + 44, 38, 38), Color.FromNonPremultiplied(255, 255, 255, 127));
                    }
                    if (MouseHelper.MouseStateCurrent.X >= DrawX && MouseHelper.MouseStateCurrent.X < DrawX + 38
                        && MouseHelper.MouseStateCurrent.Y >= DrawY + 48 && MouseHelper.MouseStateCurrent.Y < DrawY + 48 + 38)
                    {
                        g.Draw(sprPixel, new Rectangle(DrawX, DrawY + 44, 38, 38), Color.FromNonPremultiplied(255, 255, 255, 127));
                    }
                    DrawX += 40;
                }
                DrawY += SquadHeight;
            }

            //Right side
            DrawY = BattleMapInventoryScreen.MiddleSectionY + 5 - InventoryScrollbarValue % BoxHeight;
            for (int i = (int)Math.Floor(InventoryScrollbarValue / (double)BoxHeight); i < Owner.ActivePlayer.Inventory.ListOwnedCharacter.Count; ++i)
            {
                DrawBox(g, new Vector2(BattleMapInventoryScreen.LeftSideWidth + 5, DrawY), BattleMapInventoryScreen.LeftSideWidth - 95, 45, Color.White);
                g.DrawString(fntArial12, Owner.ActivePlayer.Inventory.ListOwnedCharacter[i].Name, new Vector2(BattleMapInventoryScreen.LeftSideWidth + 48, DrawY + 11), Color.White);
                DrawBox(g, new Vector2(BattleMapInventoryScreen.LeftSideWidth + 11, DrawY + 4), 38, 38, Color.White);
                g.Draw(Owner.ActivePlayer.Inventory.ListOwnedCharacter[i].sprPortrait, new Rectangle(BattleMapInventoryScreen.LeftSideWidth + 11 + 3, DrawY + 7, 32, 32), Color.White);

                if (MouseHelper.MouseStateCurrent.X >= BattleMapInventoryScreen.LeftSideWidth + 11 && MouseHelper.MouseStateCurrent.X < BattleMapInventoryScreen.LeftSideWidth + 11 + 38
                    && MouseHelper.MouseStateCurrent.Y >= DrawY + 4 && MouseHelper.MouseStateCurrent.Y < DrawY + 4 + 38)
                {
                    g.Draw(sprPixel, new Rectangle(BattleMapInventoryScreen.LeftSideWidth + 11, DrawY + 4, 38, 38), Color.FromNonPremultiplied(255, 255, 255, 127));
                }
                DrawY += 50;
            }

            if (DragAndDropEquipment != null)
            {
                g.Draw(DragAndDropEquipment.sprPortrait, new Vector2(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y), Color.White);
            }
        }
    }
}
