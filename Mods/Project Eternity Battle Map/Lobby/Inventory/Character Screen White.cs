using System;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.UI;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    class InventoryCharacterWhiteScreen : GameScreen
    {

        private EmptyBoxScrollbar CharacterListScrollbar;
        private EmptyBoxScrollbar InventoryScrollbar;

        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;

        private const int CharacterHeight = 100;
        private const int BoxWidth = 70;
        private const int BoxHeight = 70;
        private const int LineHeight = BoxHeight + 10;
        private bool ShowCheckbox = false;

        private readonly BattleMapInventoryWhiteScreen Owner;
        private CharacterInventoryContainer CurrentContainer;
        private List<CharacterInventoryContainer> ListLastContainer;

        private int MaxLoadouts;
        private int LoadoutSize;
        private int CharacterScrollbarValue;
        private int InventoryScrollbarValue;

        private CharacterInfo DragAndDropEquipment;
        private CharacterInfo LastSelectedEquipment;

        protected bool IsDragDropActive { get { return DragAndDropEquipment != null; } }

        public InventoryCharacterWhiteScreen(BattleMapInventoryWhiteScreen Owner, int MaxLoadouts, int LoadoutSize)
        {
            this.Owner = Owner;
            this.MaxLoadouts = MaxLoadouts;
            this.LoadoutSize = LoadoutSize;
            CurrentContainer = Owner.ActivePlayer.Inventory.RootCharacterContainer;
            ListLastContainer = new List<CharacterInventoryContainer>();
        }

        public override void Load()
        {
            CharacterListScrollbar = new EmptyBoxScrollbar(new Vector2(BattleMapInventoryScreen.LeftSideWidth - 23, BattleMapInventoryScreen.MiddleSectionY + 3), BattleMapInventoryScreen.MiddleSectionHeight - 5, 10, OnCharacterScrollbarChange);
            InventoryScrollbar = new EmptyBoxScrollbar(new Vector2(Constants.Width - 23, BattleMapInventoryScreen.MiddleSectionY + 3), BattleMapInventoryScreen.MiddleSectionHeight - 5, 10, OnInventoryScrollbarChange);

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            CharacterListScrollbar.ChangeMaxValue(Owner.ActivePlayer.Inventory.ListSquadLoadout.Count * CharacterHeight - BattleMapInventoryScreen.MiddleSectionHeight);
            InventoryScrollbar.ChangeMaxValue(CurrentContainer.ListCharacter.Count * BoxHeight - BattleMapInventoryScreen.MiddleSectionHeight);
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsDragDropActive)
            {
                CharacterListScrollbar.Update(gameTime);
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
                int SelectedItemIndex = GetOwnedCharacterUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);

                if (SelectedItemIndex < 0)
                {
                    return;
                }

                if (ListLastContainer.Count > 0)
                {
                    --SelectedItemIndex;
                }

                if (SelectedItemIndex == -1)
                {
                    CurrentContainer = ListLastContainer[ListLastContainer.Count - 1];
                    ListLastContainer.Remove(CurrentContainer);
                }
                else if (SelectedItemIndex >= CurrentContainer.DicFolder.Count)
                {
                    StartDragDrop(CurrentContainer.ListCharacter[SelectedItemIndex - CurrentContainer.DicFolder.Count]);
                }
                else
                {
                    ListLastContainer.Add(CurrentContainer);
                    if (ListLastContainer.Count > 1)
                    {
                        CurrentContainer = CurrentContainer.ListFolder[SelectedItemIndex];
                    }
                    else
                    {
                        CurrentContainer = CurrentContainer.ListFolder[SelectedItemIndex];
                    }
                }

            }
        }

        private void StartDragDrop(CharacterInfo EquipmentToDrag)
        {
            DragAndDropEquipment = EquipmentToDrag;
        }

        private void DoDragDrop()
        {
            int DrawX = 101;
            int DrawY = BattleMapInventoryScreen.MiddleSectionY + 5 + 4 + 45 - CharacterScrollbarValue;

            if (InputHelper.InputConfirmReleased())
            {
                int X = (MouseHelper.MouseStateCurrent.X - DrawX) / 40;
                int Y = (MouseHelper.MouseStateCurrent.Y - DrawY) / CharacterHeight;
                int MouseX = MouseHelper.MouseStateCurrent.X;
                int MouseY = MouseHelper.MouseStateCurrent.Y;
                int SquadSlotIndex = X;
                int SqaudLoatoutIndex = Y;

                if (SqaudLoatoutIndex < Owner.ActivePlayer.Inventory.ListSquadLoadout.Count && SquadSlotIndex < LoadoutSize)
                {
                    if (MouseX >= DrawX + X * 40 && MouseX < DrawX + X * 40 + 38
                        && MouseY >= DrawY + Y * CharacterHeight && MouseY < DrawY + Y * CharacterHeight + 38)
                    {
                        Owner.ActivePlayer.Inventory.ListSquadLoadout[SqaudLoatoutIndex].ListSpawnSquad[SquadSlotIndex].CurrentLeader.ArrayCharacterActive[0] = DragAndDropEquipment.Pilot;
                        Owner.ActivePlayer.SaveLocally();
                    }
                }

                DragAndDropEquipment = null;
            }
        }

        private int GetOwnedCharacterUnderMouse(int MouseX, int MouseY)
        {
            int X = BattleMapInventoryScreen.LeftSideWidth + 5;
            int BoxHeight = 70;
            int DrawY = BattleMapInventoryScreen.MiddleSectionY + 5 - InventoryScrollbarValue % BoxHeight;
            int LineHeight = BoxHeight + 10;
            int BoxWidth = 70;
            int SpriteBoxWidth = 38;
            int SpriteOffset = BoxWidth / 2 - SpriteBoxWidth / 2;

            int BoxPerLine = (Constants.Width - X) / BoxWidth;

            int MouseIndex = (MouseX - X) / BoxWidth + ((MouseY - DrawY) / LineHeight) * BoxPerLine;
            int MouseXFinal = (MouseX - X) % BoxWidth;
            int MouseYFinal = (MouseY - DrawY) % LineHeight;

            int ItemCount = CurrentContainer.ListCharacter.Count + CurrentContainer.ListFolder.Count;
            if (ListLastContainer.Count > 0)
            {
                ItemCount += 1;
            }

            if (MouseIndex >= 0 && MouseIndex < ItemCount
                && MouseXFinal >= SpriteOffset && MouseXFinal < SpriteOffset + 38
                && MouseYFinal >= 4 && MouseYFinal < 4 + 38)
            {
                return MouseIndex;
            }

            return -1;
        }

        private void OnCharacterScrollbarChange(float ScrollbarValue)
        {
            CharacterScrollbarValue = (int)ScrollbarValue;
        }

        private void OnInventoryScrollbarChange(float ScrollbarValue)
        {
            InventoryScrollbarValue = (int)ScrollbarValue;
        }

        public override void Draw(CustomSpriteBatch g)
        {
            CharacterListScrollbar.Draw(g);
            InventoryScrollbar.Draw(g);

            //Left side
            g.DrawString(fntArial12, "Loadouts", new Vector2(10, BattleMapInventoryScreen.TopSectionHeight + 5), Color.White);

            int DrawY = BattleMapInventoryScreen.MiddleSectionY + 5 - CharacterScrollbarValue % CharacterHeight;
            for (int L = (int)Math.Floor(CharacterScrollbarValue / (double)CharacterHeight); L < Owner.ActivePlayer.Inventory.ListSquadLoadout.Count; ++L)
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
                        g.Draw(sprPixel, new Rectangle(DrawX, DrawY + 45, 38, 38), Color.FromNonPremultiplied(255, 255, 255, 127));
                    }
                    if (MouseHelper.MouseStateCurrent.X >= DrawX && MouseHelper.MouseStateCurrent.X < DrawX + 38
                        && MouseHelper.MouseStateCurrent.Y >= DrawY + 45 && MouseHelper.MouseStateCurrent.Y < DrawY + 45 + 38)
                    {
                        g.Draw(sprPixel, new Rectangle(DrawX, DrawY + 45, 38, 38), Color.FromNonPremultiplied(255, 255, 255, 127));
                    }
                    DrawX += 40;
                }
                DrawY += CharacterHeight;
            }

            //Right side
            DrawInventory(g, BattleMapInventoryScreen.LeftSideWidth + 5);
            DrawSelection(g, BattleMapInventoryScreen.LeftSideWidth + 5);

            if (DragAndDropEquipment != null)
            {
                g.Draw(DragAndDropEquipment.Pilot.sprPortrait, new Vector2(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y), Color.White);
            }
        }

        private void DrawInventory(CustomSpriteBatch g, int X)
        {
            int DrawY = BattleMapInventoryScreen.MiddleSectionY + 5 - InventoryScrollbarValue % BoxHeight;
            int SpriteBoxWidth = 38;
            int SpriteOffset = BoxWidth / 2 - SpriteBoxWidth / 2;

            int StartIndex = (int)Math.Floor(InventoryScrollbarValue / (double)BoxHeight);
            int BoxPerLine = (Constants.Width - X) / BoxWidth;

            int TotalItem = CurrentContainer.ListCharacter.Count + CurrentContainer.ListFolder.Count;

            if (ListLastContainer.Count > 0)
            {
                TotalItem += 1;
            }

            int XPos = 0;

            if (ListLastContainer.Count > 0)
            {
                int FinalX = X + XPos * BoxWidth;
                DrawBox(g, new Vector2(FinalX + 8, DrawY + 12), BoxWidth - 16, BoxHeight - 24, Color.FromNonPremultiplied(255, 255, 255, 0));
                g.Draw(GameScreen.sprPixel, new Rectangle(FinalX + 8, DrawY + 12, BoxWidth - 16, BoxHeight - 24), Lobby.BackgroundColor);
                g.DrawStringCenteredBackground(fntArial12, "Last Folder",
                    new Vector2(FinalX + BoxWidth / 2, DrawY + BoxHeight / 2), Color.White, sprPixel, Lobby.BackgroundColor);

                ++XPos;
            }

            for (int YPos = StartIndex; YPos < TotalItem; YPos += BoxPerLine)
            {
                while (XPos < BoxPerLine && XPos + YPos < TotalItem)
                {
                    int FinalX = X + XPos * BoxWidth;
                    int CurrentIndex = XPos + YPos;

                    DrawEmptyBox(g, new Vector2(FinalX, DrawY), BoxWidth, BoxHeight);

                    if (ListLastContainer.Count > 0)
                    {
                        CurrentIndex -= 1;
                    }

                    if (CurrentIndex < CurrentContainer.ListFolder.Count)
                    {
                        DrawBox(g, new Vector2(FinalX + 8, DrawY + 12), BoxWidth - 16, BoxHeight - 24, Color.FromNonPremultiplied(255, 255, 255, 0));
                        g.Draw(GameScreen.sprPixel, new Rectangle(FinalX + 8, DrawY + 12, BoxWidth - 16, BoxHeight - 24), Lobby.BackgroundColor);
                        g.DrawStringCenteredBackground(fntArial12, CurrentContainer.ListFolder[CurrentIndex].Name,
                            new Vector2(FinalX + BoxWidth / 2, DrawY + BoxHeight / 2), Color.White, sprPixel, Lobby.BackgroundColor);
                    }
                    else
                    {
                        CurrentIndex -= CurrentContainer.ListFolder.Count;

                        DrawBox(g, new Vector2(FinalX + SpriteOffset, DrawY + 4), 38, 38, Color.FromNonPremultiplied(255, 255, 255, 0));
                        if (ShowCheckbox)
                        {
                            DrawBox(g, new Vector2(FinalX + 6, DrawY + BoxHeight - 20), 15, 15, Color.White);
                        }

                        g.Draw(CurrentContainer.ListCharacter[CurrentIndex].Pilot.sprPortrait, new Rectangle(FinalX + SpriteOffset + 3, DrawY + 7, 32, 32), Color.White);

                        g.DrawString(fntArial12, "x" + CurrentContainer.ListCharacter[CurrentIndex].QuantityOwned,
                            new Vector2(FinalX + BoxWidth - 25, DrawY + BoxHeight - 20), Color.White);
                    }

                    ++XPos;
                }
                XPos = 0;

                DrawY += LineHeight;
            }
        }

        private void DrawSelection(CustomSpriteBatch g, int X)
        {
            int DrawY = BattleMapInventoryScreen.MiddleSectionY + 5 - InventoryScrollbarValue % BoxHeight;

            int SelectedItemIndex = GetOwnedCharacterUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);

            if (ListLastContainer.Count > 0)
            {
                SelectedItemIndex -= 1;
            }

            //Hover
            if (SelectedItemIndex >= 0)
            {
                int FinalX = X + ((MouseHelper.MouseStateCurrent.X - X) / BoxWidth) * BoxWidth;
                int FinalY = DrawY + ((MouseHelper.MouseStateCurrent.Y - DrawY) / LineHeight) * LineHeight;

                if (SelectedItemIndex >= CurrentContainer.ListFolder.Count)
                {
                    CharacterInfo SelectedEquipment = CurrentContainer.ListCharacter[SelectedItemIndex - CurrentContainer.ListFolder.Count];

                    LastSelectedEquipment = SelectedEquipment;

                    g.Draw(sprPixel, new Rectangle(FinalX + 16, FinalY + 4, 38, 38), Color.FromNonPremultiplied(255, 255, 255, 127));
                    DrawBox(g, new Vector2(FinalX - 10, FinalY + BoxHeight), BoxWidth + 30, 25, Color.Black);
                    g.DrawStringMiddleAligned(fntArial12,
                        SelectedEquipment.Pilot.Name,
                        new Vector2(FinalX + 5 + BoxWidth / 2, FinalY + BoxHeight), Color.White);
                }
                else
                {
                    g.Draw(sprPixel, new Rectangle(FinalX + 8, DrawY + 12, BoxWidth - 16, BoxHeight - 24), Color.FromNonPremultiplied(255, 255, 255, 127));
                }
            }
            if (LastSelectedEquipment != null)
            {
                //Bottom
                g.DrawStringRightAlignedBackground(fntArial12, LastSelectedEquipment.Pilot.Name,
                    new Vector2(X + 70, BattleMapInventoryScreen.BottomSectionY + 11), Color.White, sprPixel, Lobby.BackgroundColor);
            }
        }
    }
}
