using System;
using System.Collections.Generic;
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
        public struct CharacterContainer
        {
            public List<CharacterContainer> ListFolder;
            public List<CharacterInfo> ListOwnedCharacter;

            public string Name;

            public CharacterContainer(HashSet<CharacterInfo> ListOwnedCharacter)
            {
                this.ListOwnedCharacter = new List<CharacterInfo>(ListOwnedCharacter);
                ListFolder = new List<CharacterContainer>();
                Name = string.Empty;
            }

            public CharacterContainer(string Name)
            {
                this.Name = Name;

                ListFolder = new List<CharacterContainer>();
                ListOwnedCharacter = new List<CharacterInfo>();
            }
        }

        private BoxScrollbar SquadListScrollbar;
        private BoxScrollbar InventoryScrollbar;

        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;

        private readonly BattleMapInventoryScreen Owner;
        private readonly CharacterContainer RootContainer;
        private readonly CharacterContainer CurrentContainer;
        private const int SquadHeight = 100;
        private const int BoxHeight = 50;
        private bool ShowCheckbox;

        private int MaxLoadouts;
        private int LoadoutSize;
        private int SquadScrollbarValue;
        private int InventoryScrollbarValue;

        private Character DragAndDropEquipment;

        protected bool IsDragDropActive { get { return DragAndDropEquipment != null; } }

        public InventoryCharacterScreen(BattleMapInventoryScreen Owner, int MaxLoadouts, int LoadoutSize)
        {
            this.Owner = Owner;
            this.MaxLoadouts = MaxLoadouts;
            this.LoadoutSize = LoadoutSize;
        }

        public override void Load()
        {
            SquadListScrollbar = new BoxScrollbar(new Vector2(BattleMapInventoryScreen.LeftSideWidth - 23, BattleMapInventoryScreen.MiddleSectionY + 3), BattleMapInventoryScreen.MiddleSectionHeight - 5, 10, OnSquadScrollbarChange);
            InventoryScrollbar = new BoxScrollbar(new Vector2(Constants.Width - 23, BattleMapInventoryScreen.MiddleSectionY + 3), BattleMapInventoryScreen.MiddleSectionHeight - 5, 10, OnInventoryScrollbarChange);

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            SquadListScrollbar.ChangeMaxValue(Owner.ActivePlayer.Inventory.ListSquadLoadout.Count * SquadHeight - BattleMapInventoryScreen.MiddleSectionHeight);
            InventoryScrollbar.ChangeMaxValue(Owner.ActivePlayer.Inventory.DicOwnedSquad.Count * BoxHeight - BattleMapInventoryScreen.MiddleSectionHeight);
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
                Character SelectedEquipment = GetOwnedSquadUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);
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
                int MouseX = MouseHelper.MouseStateCurrent.X;
                int MouseY = MouseHelper.MouseStateCurrent.Y;
                int SquadSlotIndex = X;
                int SqaudLoatoutIndex = Y;
                if (SqaudLoatoutIndex < Owner.ActivePlayer.Inventory.ListSquadLoadout.Count && SquadSlotIndex < LoadoutSize)
                {
                    if (MouseX >= DrawX + X * 40 && MouseX < DrawX + X * 40 + 38
                        && MouseY >= DrawY + Y * SquadHeight && MouseY < DrawY + Y * SquadHeight + 38)
                    {
                        Owner.ActivePlayer.Inventory.ListSquadLoadout[SqaudLoatoutIndex].ListSpawnSquad[SquadSlotIndex].CurrentLeader.ArrayCharacterActive[0] = DragAndDropEquipment;
                        Owner.ActivePlayer.SaveLocally();
                    }
                }

                DragAndDropEquipment = null;
            }
        }

        private Character GetOwnedSquadUnderMouse(int MouseX, int MouseY)
        {
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
                DrawEmptyBox(g, new Vector2(5, DrawY), BattleMapInventoryScreen.LeftSideWidth - 30, 90);
                DrawEmptyBox(g, new Vector2(BattleMapInventoryScreen.LeftSideWidth - 115, DrawY + 5), 85, 35);
                g.DrawString(fntArial12, "Rename", new Vector2(BattleMapInventoryScreen.LeftSideWidth - 108, DrawY + 11), Color.White);
                g.DrawString(fntArial12, "Loadout " + (L + 1), new Vector2(11, DrawY + 11), Color.White);

                int DrawX = 101;
                for (int S = 0; S < Owner.ActivePlayer.Inventory.ListSquadLoadout[L].ListSpawnSquad.Count; S++)
                {
                    DrawBox(g, new Vector2(DrawX, DrawY + 4), 38, 38, Color.FromNonPremultiplied(255, 255, 255, 0));
                    DrawBox(g, new Vector2(DrawX, DrawY + 45), 38, 38, Color.FromNonPremultiplied(255, 255, 255, 0));

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
                        g.Draw(sprPixel, new Rectangle(DrawX, DrawY + 4, 38, 38), Color.FromNonPremultiplied(255, 255, 255, 127));
                    }
                    if (MouseHelper.MouseStateCurrent.X >= DrawX && MouseHelper.MouseStateCurrent.X < DrawX + 38
                        && MouseHelper.MouseStateCurrent.Y >= DrawY + 4 && MouseHelper.MouseStateCurrent.Y < DrawY + 4 + 38)
                    {
                        g.Draw(sprPixel, new Rectangle(DrawX, DrawY + 4, 38, 38), Color.FromNonPremultiplied(255, 255, 255, 127));
                    }
                    DrawX += 40;
                }
                DrawY += SquadHeight;
            }

            //Right side
            DrawInventory(g, BattleMapInventoryScreen.LeftSideWidth + 5);

            if (DragAndDropEquipment != null)
            {
                g.Draw(DragAndDropEquipment.sprPortrait, new Vector2(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y), Color.White);
            }
        }

        private void DrawInventory(CustomSpriteBatch g, int X)
        {
            int BoxHeight = 70;
            int DrawY = BattleMapInventoryScreen.MiddleSectionY + 5 - InventoryScrollbarValue % BoxHeight;
            int LineHeight = BoxHeight + 10;
            int BoxWidth = 70;
            int SpriteBoxWidth = 38;
            int SpriteOffset = BoxWidth / 2 - SpriteBoxWidth / 2;

            int StartIndex = (int)Math.Floor(InventoryScrollbarValue / (double)BoxHeight);
            int BoxPerLine = (Constants.Width - X) / BoxWidth;

            int TotalItem = CurrentContainer.ListOwnedCharacter.Count + CurrentContainer.ListFolder.Count;

            for (int YPos = StartIndex; YPos < TotalItem; YPos += BoxPerLine)
            {
                for (int XPos = 0; XPos < BoxPerLine && XPos + YPos < TotalItem; ++XPos)
                {
                    int FinalX = X + XPos * BoxWidth;
                    int CurrentIndex = XPos + YPos;

                    DrawEmptyBox(g, new Vector2(FinalX, DrawY), BoxWidth, BoxHeight);

                    if (CurrentIndex < CurrentContainer.ListFolder.Count)
                    {
                        DrawBox(g, new Vector2(FinalX + 8, DrawY + 12), BoxWidth - 16, BoxHeight - 24, Color.FromNonPremultiplied(255, 255, 255, 0));
                        g.Draw(GameScreen.sprPixel, new Rectangle(FinalX + 8, DrawY + 12, BoxWidth - 16, BoxHeight - 24), BattleMapInventoryScreen.BackgroundColor);
                        g.DrawStringCenteredBackground(fntArial12, CurrentContainer.ListFolder[CurrentIndex].Name,
                            new Vector2(FinalX + BoxWidth / 2, DrawY + BoxHeight / 2), Color.White, sprPixel, BattleMapInventoryScreen.BackgroundColor);
                    }
                    else
                    {
                        CurrentIndex -= CurrentContainer.ListFolder.Count;

                        DrawBox(g, new Vector2(FinalX + SpriteOffset, DrawY + 4), 38, 38, Color.FromNonPremultiplied(255, 255, 255, 0));
                        if (ShowCheckbox)
                        {
                            DrawBox(g, new Vector2(FinalX + 6, DrawY + BoxHeight - 20), 15, 15, Color.White);
                        }

                        DrawBox(g, new Vector2(BattleMapInventoryScreen.LeftSideWidth + 11, DrawY + 4), 38, 38, Color.White);
                        g.Draw(CurrentContainer.ListOwnedCharacter[CurrentIndex].Pilot.sprPortrait, new Rectangle(BattleMapInventoryScreen.LeftSideWidth + 11 + 3, DrawY + 7, 32, 32), Color.White);

                        g.DrawString(fntArial12, "x" + CurrentContainer.ListOwnedCharacter[CurrentIndex].QuantityOwned,
                            new Vector2(FinalX + BoxWidth - 25, DrawY + BoxHeight - 20), Color.White);

                        if (MouseHelper.MouseStateCurrent.X >= FinalX + SpriteOffset && MouseHelper.MouseStateCurrent.X < FinalX + SpriteOffset + 38
                            && MouseHelper.MouseStateCurrent.Y >= DrawY + 4 && MouseHelper.MouseStateCurrent.Y < DrawY + 4 + 38)
                        {
                            g.Draw(sprPixel, new Rectangle(FinalX + 11, DrawY + 4, 38, 38), Color.FromNonPremultiplied(255, 255, 255, 127));

                            DrawBox(g, new Vector2(FinalX - 15, DrawY + BoxHeight), BoxWidth + 30, 25, Color.FromNonPremultiplied(255, 255, 255, 0));
                            g.DrawStringMiddleAligned(fntArial12,
                                CurrentContainer.ListOwnedCharacter[CurrentIndex].Pilot.Name,
                                new Vector2(FinalX + BoxWidth / 2, DrawY + BoxHeight), Color.White);
                        }
                    }
                }
                DrawY += LineHeight;
            }

            g.DrawStringRightAligned(fntArial12, CurrentContainer.ListOwnedCharacter[0].Pilot.Name,
                new Vector2(X + 70, BattleMapInventoryScreen.BottomSectionY + 11), Color.White);

            g.DrawString(fntArial12, "HP: " + CurrentContainer.ListOwnedCharacter[0].Pilot.MEL,
                new Vector2(X + 70, BattleMapInventoryScreen.BottomSectionY + 31), Color.White);
        }
    }
}
