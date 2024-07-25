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
    public class InventorySquadWhiteScreen : GameScreen
    {
        private EmptyBoxScrollbar SquadListScrollbar;
        private EmptyBoxScrollbar InventoryScrollbar;

        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;
        private SpriteFont fntOxanimumLight;
        private SpriteFont fntOxanimumLightSmall;
        private SpriteFont fntOxanimumRegular;
        private SpriteFont fntOxanimumRegularSmall;
        private SpriteFont fntOxanimumBold;
        private SpriteFont fntOxanimumBoldSmall;
        private SpriteFont fntOxanimumBoldSmaller;
        private SpriteFont fntOxanimumBoldBig;

        private Texture2D sprLoadouts;
        private Texture2D sprLoadoutsFrame;
        private Texture2D sprLoadoutsName;
        private Texture2D sprFrameDescription;

        private Texture2D sprButtonFolderActive;
        private Texture2D sprButtonFolderInactive;
        private Texture2D sprButtonRename;

        private Texture2D sprScrollbarBackground;
        private Texture2D sprScrollbar;

        private const int SquadHeight = 190;
        private const int BoxWidth = 70;
        private const int BoxHeight = 70;
        private const int LineHeight = BoxHeight + 10;
        private int LoadoutX;
        private bool ShowCheckbox = false;

        private readonly BattleMapInventoryWhiteScreen Owner;
        private UnitInventoryContainer CurrentContainer;
        private List<UnitInventoryContainer> ListLastContainer;

        private int MaxLoadouts;
        private int LoadoutSize;
        private int SquadScrollbarValue;
        private int InventoryScrollbarValue;

        private UnitInfo DragAndDropEquipment;
        private UnitInfo LastSelectedEquipment;

        protected bool IsDragDropActive { get { return DragAndDropEquipment != null; } }

        public InventorySquadWhiteScreen(BattleMapInventoryWhiteScreen Owner, int MaxLoadouts, int LoadoutSize)
        {
            this.Owner = Owner;
            this.MaxLoadouts = MaxLoadouts;
            this.LoadoutSize = LoadoutSize;
            CurrentContainer = Owner.ActivePlayer.Inventory.RootUnitContainer;
            ListLastContainer = new List<UnitInventoryContainer>();

            float Ratio = Constants.Height / 2160f;
            LoadoutX = (int)(2476 * Ratio);
        }

        public override void Load()
        {
            SquadListScrollbar = new EmptyBoxScrollbar(new Vector2(BattleMapInventoryWhiteScreen.LeftSideWidth - 23, BattleMapInventoryWhiteScreen.MiddleSectionY + 3), BattleMapInventoryWhiteScreen.MiddleSectionHeight - 5, 10, OnSquadScrollbarChange);
            InventoryScrollbar = new EmptyBoxScrollbar(new Vector2(Constants.Width - 23, BattleMapInventoryWhiteScreen.MiddleSectionY + 3), BattleMapInventoryWhiteScreen.MiddleSectionHeight - 5, 10, OnInventoryScrollbarChange);

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            fntOxanimumLight = Content.Load<SpriteFont>("Fonts/Oxanium Light Big");
            fntOxanimumLightSmall = Content.Load<SpriteFont>("Fonts/Oxanium Light");
            fntOxanimumRegular = Content.Load<SpriteFont>("Fonts/Oxanium Regular");
            fntOxanimumRegularSmall = Content.Load<SpriteFont>("Fonts/Oxanium Regular Small");
            fntOxanimumBold = Content.Load<SpriteFont>("Fonts/Oxanium Bold");
            fntOxanimumBoldSmall = Content.Load<SpriteFont>("Fonts/Oxanium Bold Small");
            fntOxanimumBoldSmaller = Content.Load<SpriteFont>("Fonts/Oxanium Bold Smaller");
            fntOxanimumBoldBig = Content.Load<SpriteFont>("Fonts/Oxanium Bold Big");

            sprLoadouts = Content.Load<Texture2D>("Menus/Lobby/Frame Outline");
            sprLoadoutsFrame = Content.Load<Texture2D>("Menus/Lobby/Inventory/Frame Loadout");
            sprLoadoutsName = Content.Load<Texture2D>("Menus/Lobby/Inventory/Frame Loadout Name");
            sprFrameDescription = Content.Load<Texture2D>("Menus/Lobby/Inventory/Frame Description");

            sprButtonFolderActive = Content.Load<Texture2D>("Menus/Lobby/Inventory/Folder Button Active");
            sprButtonFolderInactive = Content.Load<Texture2D>("Menus/Lobby/Inventory/Folder Button Inactive");
            sprButtonRename = Content.Load<Texture2D>("Menus/Lobby/Inventory/Button Rename");

            sprScrollbarBackground = Content.Load<Texture2D>("Menus/Lobby/Scrollbar Background");
            sprScrollbar = Content.Load<Texture2D>("Menus/Lobby/Scrollbar Bar");

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            SquadListScrollbar.ChangeMaxValue(Owner.ActivePlayer.Inventory.ListSquadLoadout.Count * SquadHeight - BattleMapInventoryWhiteScreen.MiddleSectionHeight);
            InventoryScrollbar.ChangeMaxValue(CurrentContainer.ListUnit.Count * BoxHeight - BattleMapInventoryWhiteScreen.MiddleSectionHeight);
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
            else if (MouseHelper.InputRightButtonPressed())
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

                if (SelectedItemIndex >= CurrentContainer.DicFolder.Count)
                {
                    //PushScreen(new InventoryUnitInformationScreen(Owner, CurrentContainer.ListUnit[SelectedItemIndex - CurrentContainer.DicFolder.Count].Leader));
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
            int DrawY = BattleMapInventoryWhiteScreen.MiddleSectionY + 5 + 4 - SquadScrollbarValue;

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
            int X = 5;
            int BoxHeight = 70;
            int DrawY = BattleMapInventoryWhiteScreen.MiddleSectionY + 5 - InventoryScrollbarValue % BoxHeight;
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
            float Ratio = Constants.Height / 2160f;
            g.Draw(sprScrollbarBackground, new Vector2((BattleMapInventoryWhiteScreen.LoadoutX - 76) * Ratio, 648 * Ratio), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
            SquadListScrollbar.Draw(g);
            InventoryScrollbar.Draw(g);

            DrawLoadout(g, 500);

            DrawInventory(g, 240);
            DrawSelection(g, 94);

            if (DragAndDropEquipment != null)
            {
                g.Draw(DragAndDropEquipment.Leader.SpriteMap, new Vector2(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y), Color.White);
            }
        }

        private void DrawLoadout(CustomSpriteBatch g, int X)
        {
            float Ratio = Constants.Height / 2160f;
            Color TextColor = Color.FromNonPremultiplied(65, 70, 65, 255);
            g.Draw(sprLoadouts, new Vector2((BattleMapInventoryWhiteScreen.LoadoutX  - 50) * Ratio, 260 * Ratio), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
            g.Draw(sprLoadoutsFrame, new Vector2(BattleMapInventoryWhiteScreen.LoadoutX * Ratio, 416 * Ratio), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
            g.DrawString(fntOxanimumBoldBig, "Loadouts", new Vector2((BattleMapInventoryWhiteScreen.LoadoutX) * Ratio, 294 * Ratio), TextColor);

            int LoadoutBoxSize = (int)(68 * Ratio);
            int DrawY = 416 + 46 - SquadScrollbarValue % SquadHeight;
            for (int L = (int)Math.Floor(SquadScrollbarValue / (double)SquadHeight); L < /*Owner.ActivePlayer.Inventory.ListSquadLoadout.Count*/5; ++L)
            {
                int DrawX = BattleMapInventoryWhiteScreen.LoadoutX + 30;

                g.Draw(sprLoadoutsName, new Vector2((int)((DrawX + 420) * Ratio), (int)(DrawY * Ratio)), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
                g.Draw(sprButtonRename, new Vector2((int)((DrawX + 1074) * Ratio), (int)(DrawY * Ratio)), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
                g.DrawString(fntOxanimumRegularSmall, "Rename", new Vector2((int)((DrawX + 1114) * Ratio), (int)((DrawY + 16) * Ratio)), TextColor);
                g.DrawString(fntOxanimumRegularSmall, "Loadout " + (L + 1), new Vector2((int)((DrawX + 466) * Ratio), (int)((DrawY + 16) * Ratio)), TextColor);

                for (int S = 0; S < /*Owner.ActivePlayer.Inventory.ListSquadLoadout[L].ListSpawnSquad.Count*/ 5; S++)
                {
                    g.Draw(sprPixel, new Rectangle((int)((DrawX + 0) * Ratio), (int)(DrawY * Ratio), LoadoutBoxSize, LoadoutBoxSize), Color.FromNonPremultiplied(10, 10, 10, 255));
                    g.Draw(sprPixel, new Rectangle((int)((DrawX + 0) * Ratio), (int)((DrawY + 86) * Ratio), LoadoutBoxSize, LoadoutBoxSize), Color.FromNonPremultiplied(10, 10, 10, 255));

                    /*Squad ActiveSquad = Owner.ActivePlayer.Inventory.ListSquadLoadout[L].ListSpawnSquad[S];
                    if (ActiveSquad != null)
                    {
                        g.Draw(ActiveSquad.CurrentLeader.SpriteMap, new Vector2(DrawX + 3, DrawY + 7), Color.White);
                        if (ActiveSquad.CurrentLeader.Pilot != null)
                        {
                            g.Draw(ActiveSquad.CurrentLeader.Pilot.sprPortrait, new Rectangle(DrawX + 3, DrawY + 48, 32, 32), Color.White);
                        }
                    }*/

                    if (DragAndDropEquipment != null)
                    {
                        g.Draw(sprPixel, new Rectangle(DrawX, DrawY + 4, 38, 38), Color.FromNonPremultiplied(255, 255, 255, 127));
                    }
                    if (MouseHelper.MouseStateCurrent.X >= DrawX && MouseHelper.MouseStateCurrent.X < DrawX + 38
                        && MouseHelper.MouseStateCurrent.Y >= DrawY + 4 && MouseHelper.MouseStateCurrent.Y < DrawY + 4 + 38)
                    {
                        g.Draw(sprPixel, new Rectangle(DrawX, DrawY + 4, 38, 38), Color.FromNonPremultiplied(255, 255, 255, 127));
                    }
                    DrawX += 84;
                }
                DrawY += SquadHeight;
            }
        }

        private void DrawFolders(CustomSpriteBatch g, int X)
        {
            Color TextColor = Color.FromNonPremultiplied(65, 70, 65, 255);
            float Ratio = Constants.Height / 2160f;
            int DrawY = (int)(444 * Ratio) - InventoryScrollbarValue % BoxHeight;
            int FolderBoxWidth = sprButtonFolderInactive.Width;
            int FolderBoxHeight = (int)(sprButtonFolderInactive.Height * Ratio / 2);
            int BoxOffset = 20;

            int StartIndex = (int)Math.Floor(InventoryScrollbarValue / (double)BoxHeight);
            int BoxPerLine = (Constants.Width - X) / FolderBoxWidth;

            int TotalItem = CurrentContainer.ListFolder.Count;

            if (ListLastContainer.Count > 0)
            {
                TotalItem += 1;
            }

            int XPos = 0;

            if (ListLastContainer.Count > 0)
            {
                int FinalX = X + XPos * FolderBoxWidth;
                g.Draw(sprButtonFolderInactive, new Vector2(FinalX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);

                g.DrawStringCentered(fntArial12, "Last Folder", new Vector2(FinalX + FolderBoxWidth / 2, DrawY + BoxHeight / 2), Color.White);

                ++XPos;
            }

            for (int YPos = StartIndex; YPos < TotalItem; YPos += BoxPerLine)
            {
                while (XPos < BoxPerLine && XPos + YPos < TotalItem)
                {
                    int FinalX = (int)((X - 100 + XPos * (FolderBoxWidth + BoxOffset)) * Ratio);
                    int CurrentIndex = XPos + YPos;

                    if (ListLastContainer.Count > 0)
                    {
                        CurrentIndex -= 1;
                    }

                    g.Draw(sprButtonFolderInactive, new Vector2(FinalX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
                    g.DrawStringCentered(fntOxanimumLight, CurrentContainer.ListFolder[CurrentIndex].Name, new Vector2(FinalX + FolderBoxWidth * Ratio / 2, DrawY + FolderBoxHeight), Color.White);

                    ++XPos;
                }

                XPos = 0;

                DrawY += LineHeight;
            }
        }

        private void DrawInventory(CustomSpriteBatch g, int X)
        {
            float Ratio = Constants.Height / 2160f;
            Color TextColor = Color.FromNonPremultiplied(65, 70, 65, 255);
            int BoxHeight = 120;
            int BoxWidthFinal = (int)(176 * Ratio);
            int DrawY = (int)(648 * Ratio - InventoryScrollbarValue % BoxHeight);
            int BoxOffset = 134;
            int SpriteSizeFinal = (int)(128 * Ratio);
            int SpriteOffset = (BoxWidthFinal - SpriteSizeFinal) / 2;

            int StartIndex = (int)Math.Floor(InventoryScrollbarValue / (double)BoxHeight);
            int InventoryWidth = (int)((BattleMapInventoryWhiteScreen.LoadoutX - X - 100 - BoxWidthFinal - BoxOffset) * Ratio);
            int BoxPerLine = InventoryWidth / BoxWidthFinal;

            int TotalItem = CurrentContainer.ListUnit.Count;

            if (ListLastContainer.Count > 0)
            {
                TotalItem += 1;
            }

            DrawFolders(g, X);
            int XPos = 0;
            for (int YPos = StartIndex; YPos < TotalItem; YPos += BoxPerLine)
            {
                while (XPos < BoxPerLine && XPos + YPos < TotalItem)
                {
                    int FinalX = (int)((X - 100 + XPos * (BoxWidthFinal + BoxOffset)) * Ratio);
                    int CurrentIndex = XPos + YPos;

                    g.Draw(sprPixel, new Rectangle(FinalX, DrawY, BoxWidthFinal, BoxHeight), Color.FromNonPremultiplied(16, 16, 16, 255));
                    g.Draw(sprPixel, new Rectangle(FinalX + 5, DrawY + 5, BoxWidthFinal - 10, 80), TextColor);
                    g.Draw(sprPixel, new Rectangle(FinalX + 5, DrawY + BoxHeight - (int)(66 * Ratio), BoxWidthFinal - 10, (int)(56 * Ratio)), Color.FromNonPremultiplied(243, 243, 243, 255));
                    if (ShowCheckbox)
                    {
                        DrawBox(g, new Vector2(FinalX + 6, DrawY + BoxHeight - 20), 15, 15, Color.White);
                    }

                    g.Draw(CurrentContainer.ListUnit[CurrentIndex].Leader.SpriteMap, new Rectangle(FinalX + SpriteOffset, DrawY + SpriteOffset, SpriteSizeFinal, SpriteSizeFinal), Color.White);

                    g.DrawString(fntOxanimumBoldSmall, "x" + CurrentContainer.ListUnit[CurrentIndex].QuantityOwned, new Vector2(FinalX + BoxWidthFinal - 40, DrawY + BoxHeight - 30), TextColor);

                    ++XPos;
                }
                XPos = 0;

                DrawY += (int)(276 * Ratio);
            }
        }

        private void DrawSelection(CustomSpriteBatch g, int X)
        {
            float Ratio = Constants.Height / 2160f;
            Color TextColor = Color.FromNonPremultiplied(65, 70, 65, 255);
            int DrawY = BattleMapInventoryWhiteScreen.MiddleSectionY + 5 - InventoryScrollbarValue % BoxHeight;

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

            g.Draw(sprFrameDescription, new Vector2(X * Ratio, 1938 * Ratio), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
            if (LastSelectedEquipment != null)
            {
                int SpriteSize = 128;
                int SpriteBoxSize = SpriteSize + 22;
                g.Draw(sprPixel, new Rectangle((int)((X + 32) * Ratio), (int)(1962 * Ratio), (int)(SpriteBoxSize * Ratio), (int)(SpriteBoxSize * Ratio)), TextColor);
                DrawBox(g, new Vector2((int)((X + 32) * Ratio), (int)(1962 * Ratio)), (int)(SpriteBoxSize * Ratio), (int)(SpriteBoxSize * Ratio), Color.Transparent);
                g.Draw(LastSelectedEquipment.Leader.SpriteMap, new Rectangle((int)((X + 40) * Ratio), (int)(1976 * Ratio), (int)(SpriteSize * Ratio), (int)(SpriteSize * Ratio)), Color.White);

                g.DrawString(fntOxanimumBoldSmaller, LastSelectedEquipment.Leader.ItemName, new Vector2((X + 240) * Ratio, 1976 * Ratio), TextColor);

                g.DrawString(fntOxanimumLightSmall, "HP: " + LastSelectedEquipment.Leader.MaxHP, new Vector2((X + 836) * Ratio, 1976 * Ratio), TextColor);

                g.DrawString(fntOxanimumLightSmall, "Rank: " + LastSelectedEquipment.Leader.QualityRank, new Vector2((X + 836) * Ratio, 2058 * Ratio), TextColor);

                g.DrawString(fntOxanimumLightSmall, "Spawn Cost: " + LastSelectedEquipment.Leader.UnitStat.SpawnCost, new Vector2((X + 1412) * Ratio, 1976 * Ratio), TextColor);
            }
        }
    }
}
