using System;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.UI;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class InventorySquadScreen : GameScreen
    {
        private EmptyBoxScrollbar SquadListScrollbar;
        private EmptyBoxScrollbar InventoryScrollbar;

        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;

        private const int SquadHeight = 100;
        private const int BoxWidth = 70;
        private const int BoxHeight = 70;
        private const int LineHeight = BoxHeight + 10;
        private bool ShowCheckbox = false;

        private readonly BattleMapInventoryScreen Owner;
        private UnitInventoryContainer CurrentContainer;
        private List<UnitInventoryContainer> ListLastContainer;

        private int MaxLoadouts;
        private int LoadoutSize;
        private int SquadScrollbarValue;
        private int InventoryScrollbarValue;

        private UnitInfo DragAndDropEquipment;
        private UnitInfo LastSelectedEquipment;

        protected bool IsDragDropActive { get { return DragAndDropEquipment != null; } }

        public InventorySquadScreen(BattleMapInventoryScreen Owner, int MaxLoadouts, int LoadoutSize)
        {
            this.Owner = Owner;
            this.MaxLoadouts = MaxLoadouts;
            this.LoadoutSize = LoadoutSize;
            CurrentContainer = Owner.ActivePlayer.Inventory.RootUnitContainer;
            ListLastContainer = new List<UnitInventoryContainer>();
        }

        public override void Load()
        {
            SquadListScrollbar = new EmptyBoxScrollbar(new Vector2(BattleMapInventoryScreen.LeftSideWidth - 23, BattleMapInventoryScreen.MiddleSectionY + 3), BattleMapInventoryScreen.MiddleSectionHeight - 5, 10, OnSquadScrollbarChange);
            InventoryScrollbar = new EmptyBoxScrollbar(new Vector2(Constants.Width - 23, BattleMapInventoryScreen.MiddleSectionY + 3), BattleMapInventoryScreen.MiddleSectionHeight - 5, 10, OnInventoryScrollbarChange);

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            SquadListScrollbar.ChangeMaxValue(Owner.ActivePlayer.Inventory.ListSquadLoadout.Count * SquadHeight - BattleMapInventoryScreen.MiddleSectionHeight);
            InventoryScrollbar.ChangeMaxValue(CurrentContainer.ListUnit.Count * BoxHeight - BattleMapInventoryScreen.MiddleSectionHeight);
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
                int SelectedItemIndex = GetOwnedSquadUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);

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
                    StartDragDrop(CurrentContainer.ListUnit[SelectedItemIndex - CurrentContainer.DicFolder.Count]);
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

        private void StartDragDrop(UnitInfo EquipmentToDrag)
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
                        Character OldPilot = Owner.ActivePlayer.Inventory.ListSquadLoadout[SqaudLoatoutIndex].ListSpawnSquad[SquadSlotIndex].At(0).Pilot;
                        Squad ReplacementSquad = new Squad("Squad " + SquadSlotIndex, DragAndDropEquipment.Leader);
                        ReplacementSquad.At(0).ArrayCharacterActive[0] = OldPilot;

                        Owner.ActivePlayer.Inventory.ListSquadLoadout[SqaudLoatoutIndex].ListSpawnSquad[SquadSlotIndex] = new Squad("Squad " + SquadSlotIndex, DragAndDropEquipment.Leader);
                        Owner.ActivePlayer.SaveLocally();
                    }
                }

                DragAndDropEquipment = null;
            }
        }

        private int GetOwnedSquadUnderMouse(int MouseX, int MouseY)
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

            int ItemCount = CurrentContainer.ListUnit.Count + CurrentContainer.ListFolder.Count;
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
            DrawSelection(g, BattleMapInventoryScreen.LeftSideWidth + 5);

            if (DragAndDropEquipment != null)
            {
                g.Draw(DragAndDropEquipment.Leader.SpriteMap, new Vector2(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y), Color.White);
            }
        }

        private void DrawInventory(CustomSpriteBatch g, int X)
        {
            int DrawY = BattleMapInventoryScreen.MiddleSectionY + 5 - InventoryScrollbarValue % BoxHeight;
            int SpriteBoxWidth = 38;
            int SpriteOffset = BoxWidth / 2 - SpriteBoxWidth / 2;

            int StartIndex = (int)Math.Floor(InventoryScrollbarValue / (double)BoxHeight);
            int BoxPerLine = (Constants.Width - X) / BoxWidth;

            int TotalItem = CurrentContainer.ListUnit.Count + CurrentContainer.ListFolder.Count;

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

                        g.Draw(CurrentContainer.ListUnit[CurrentIndex].Leader.SpriteMap, new Vector2(FinalX + SpriteOffset + 3, DrawY + 7), Color.White);

                        g.DrawString(fntArial12, "x" + CurrentContainer.ListUnit[CurrentIndex].QuantityOwned,
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

            int SelectedItemIndex = GetOwnedSquadUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);

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
                    UnitInfo SelectedEquipment = CurrentContainer.ListUnit[SelectedItemIndex - CurrentContainer.ListFolder.Count];

                    LastSelectedEquipment = SelectedEquipment;

                    g.Draw(sprPixel, new Rectangle(FinalX + 16, FinalY + 4, 38, 38), Color.FromNonPremultiplied(255, 255, 255, 127));
                    DrawBox(g, new Vector2(FinalX - 10, FinalY + BoxHeight), BoxWidth + 30, 25, Color.Black);
                    g.DrawStringMiddleAligned(fntArial12,
                        SelectedEquipment.Leader.ItemName,
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
                g.DrawStringRightAlignedBackground(fntArial12, LastSelectedEquipment.Leader.ItemName,
                    new Vector2(X + 70, BattleMapInventoryScreen.BottomSectionY + 11), Color.White, sprPixel, Lobby.BackgroundColor);

                g.DrawStringBackground(fntArial12, "HP: " + LastSelectedEquipment.Leader.MaxHP,
                    new Vector2(X + 70, BattleMapInventoryScreen.BottomSectionY + 31), Color.White, sprPixel, Lobby.BackgroundColor);

                g.DrawStringBackground(fntArial12, "Rank: " + LastSelectedEquipment.Leader.QualityRank,
                    new Vector2(X + 170, BattleMapInventoryScreen.BottomSectionY + 31), Color.White, sprPixel, Lobby.BackgroundColor);

                g.DrawStringBackground(fntArial12, "Spawn Cost: " + LastSelectedEquipment.Leader.SpawnCost,
                    new Vector2(X + 270, BattleMapInventoryScreen.BottomSectionY + 31), Color.White, sprPixel, Lobby.BackgroundColor);
            }
        }
    }
}
