﻿using System;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.UI;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class ShopUnitWhiteScreen : GameScreen
    {
        private enum MenuSelections { Nothing, Folders, Unit }

        private EmptyBoxScrollbar SquadListScrollbar;

        #region Ressources

        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;
        private SpriteFont fntFinlanderFont;
        private SpriteFont fntOxanimumBold;
        private SpriteFont fntOxanimumLight;

        private Texture2D sprListEntry;
        private Texture2D sprListEntrySub;

        private Texture2D sprDropDownClose;
        private Texture2D sprDropDownOpen;
        private Texture2D sprSelectAUnitToBuy;

        private Texture2D sprLand;
        private Texture2D sprSea;
        private Texture2D sprSky;
        private Texture2D sprSpace;
        private RenderTarget2D UnitListRenderTarget;

        #endregion

        private MenuSelections MenuSelection;
        private int SelectionIndex;

        private readonly ShopScreen Owner;

        int FolderX = 20;
        int FolderWidth = 70;
        int FolderHeight = 70;

        int UnitX = 25;
        int UnitWidth = 250 + 125 + 105 + 105 + 90 + 80 + 95 + 70 - 25;
        int UnitHeight = 50;

        private int SquadScrollbarValue;

        private BattleMapPlayerShopUnlockInventory UnlockInventory;
        private BattleMapPlayerInventory Inventory;

        private BattleMapPlayerShopUnlockInventory.UnitUnlockContainer CurrentContainer;
        private List<BattleMapPlayerShopUnlockInventory.UnitUnlockContainer> ListLastContainer;

        public ShopUnitWhiteScreen(ShopScreen Owner, BattleMapPlayerShopUnlockInventory UnlockInventory, BattleMapPlayerInventory Inventory)
        {
            this.Owner = Owner;
            this.UnlockInventory = UnlockInventory;
            this.Inventory = Inventory;
            CurrentContainer = UnlockInventory.RootUnitContainer;
            ListLastContainer = new List<BattleMapPlayerShopUnlockInventory.UnitUnlockContainer>();
        }

        public override void Load()
        {
            UnitListRenderTarget = new RenderTarget2D(GraphicsDevice, 1000, 650, false,
                GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);

            SquadListScrollbar = new EmptyBoxScrollbar(new Vector2(Owner.LeftSideWidth, Owner.MiddleSectionY + 3), Owner.MiddleSectionHeight - 5, 10, OnSquadScrollbarChange);

            SquadListScrollbar.ChangeMaxValue(300);

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            fntFinlanderFont = Content.Load<SpriteFont>("Fonts/Finlander Font");
            fntOxanimumBold = Content.Load<SpriteFont>("Fonts/Oxanium Bold");
            fntOxanimumLight = Content.Load<SpriteFont>("Fonts/Oxanium Light");

            sprListEntry = Content.Load<Texture2D>("Menus/Lobby/Shop/ContentFrame_Main");
            sprListEntrySub = Content.Load<Texture2D>("Menus/Lobby/Shop/ContentFrame_Sub");
            sprDropDownClose = Content.Load<Texture2D>("Menus/Lobby/Shop/Listing_Close");
            sprDropDownOpen = Content.Load<Texture2D>("Menus/Lobby/Shop/Listing_Open");
            sprSelectAUnitToBuy = Content.Load<Texture2D>("Menus/Lobby/Shop/Frame Unit To Buy");

            sprLand = Content.Load<Texture2D>("Menus/Status Screen/Ground");
            sprSea = Content.Load<Texture2D>("Menus/Status Screen/Sea");
            sprSky = Content.Load<Texture2D>("Menus/Status Screen/Sky");
            sprSpace = Content.Load<Texture2D>("Menus/Status Screen/Space");

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");
        }

        public override void Update(GameTime gameTime)
        {
            if (Owner.OnlineGameClient != null)
            {
            }
            else
            {
                PendingUnlockScreen.CheckForUnlocks(this);
            }

            PendingUnlockScreen.UpdateUnlockScreens(this);

            SquadListScrollbar.Update(gameTime);

            int FolderY = Owner.MiddleSectionY + 55;

            int DrawY = FolderY + 45;
            int FolderOffsetX = (FolderWidth + 5);

            int FolderCount = CurrentContainer.DicFolder.Count;
            if (ListLastContainer.Count > 0)
            {
                FolderCount += 1;
            }

            int FoldersOnLine = (Owner.LeftSideWidth - FolderX) / FolderOffsetX;
            int NumberOfFolderLines = (int)Math.Ceiling(FolderCount / (float)FoldersOnLine);
            DrawY += FolderHeight * NumberOfFolderLines;
            DrawY += 10;

            DrawY += 40;

            DrawY += CurrentContainer.ListUnlockedUnit.Count * UnitHeight;
            DrawY += CurrentContainer.ListLockedUnit.Count * UnitHeight;

            int BottomY = (Owner.BottomSectionY - 155) - (Owner.MiddleSectionY + 55);

            SquadListScrollbar.ChangeMaxValue(DrawY - FolderY - BottomY);

            MenuSelection = MenuSelections.Nothing;
            UpdateFolderUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);
            UpdateUnitsUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);

            if (InputHelper.InputConfirmPressed())
            {
                if (MenuSelection == MenuSelections.Folders)
                {
                    if (SelectionIndex == 0 && ListLastContainer.Count > 0)
                    {
                        CurrentContainer = ListLastContainer[ListLastContainer.Count - 1];
                        ListLastContainer.Remove(CurrentContainer);
                    }
                    else
                    {
                        ListLastContainer.Add(CurrentContainer);
                        if (ListLastContainer.Count > 1)
                        {
                            CurrentContainer = CurrentContainer.ListFolder[SelectionIndex - 1];
                        }
                        else
                        {
                            CurrentContainer = CurrentContainer.ListFolder[SelectionIndex];
                        }
                    }
                }
                else if (MenuSelection == MenuSelections.Unit)
                {
                    UnlockableItem Itemfound;
                    UnlockableUnit OwnerUnit;
                    GetRealUnitItem(out Itemfound, out OwnerUnit);
                    if (Itemfound is UnlockableUnit)
                    {
                        if (Owner.OnlineGameClient != null)
                        {
                        }
                        else
                        {
                            PushScreen(new ShopConfirmBuyUnitScreen(Inventory, (UnlockableUnit)Itemfound));
                        }
                    }
                    else if (Itemfound is UnlockableUnitSkin)
                    {
                        if (Owner.OnlineGameClient != null)
                        {
                        }
                        else
                        {
                            PushScreen(new ShopConfirmBuyUnitSkinScreen(Inventory, OwnerUnit.Path, (UnlockableUnitSkin)Itemfound));
                        }
                    }
                    else if (Itemfound is UnlockableUnitAlt)
                    {
                        //PushScreen(new ShopConfirmBuyUnitScreen(Inventory, (UnlockableUnitAlt)Itemfound));
                    }
                }
            }
        }

        private void OnSquadScrollbarChange(float ScrollbarValue)
        {
            SquadScrollbarValue = (int)ScrollbarValue;
        }

        private int GetFolderY()
        {
            return Owner.MiddleSectionY + 55 - SquadScrollbarValue;
        }

        private int GetUnitsY()
        {
            int UnitY = GetFolderY();

            UnitY += 45;
            int FolderOffsetX = (FolderWidth + 5);

            int FolderCount = CurrentContainer.DicFolder.Count;
            if (ListLastContainer.Count > 0)
            {
                FolderCount += 1;
            }

            int FoldersOnLine = (Owner.LeftSideWidth - FolderX) / FolderOffsetX;
            int NumberOfFolderLines = (int)Math.Ceiling(FolderCount / (float)FoldersOnLine);
            UnitY += FolderHeight * NumberOfFolderLines;
            UnitY += 10;

            return UnitY;
        }

        private void UpdateFolderUnderMouse(int MouseX, int MouseY)
        {
            int FolderY = GetFolderY();
            if (FolderY <= 137 || MouseY <= 217)
            {
                return;
            }

            int FolderCount = CurrentContainer.DicFolder.Count;
            if (ListLastContainer.Count > 0)
            {
                FolderCount += 1;
            }

            FolderY += 45;
            int FolderOffsetX = (FolderWidth + 5);
            int IndexX = (int)Math.Floor((MouseX - FolderX) / (float)FolderOffsetX);
            int IndexY = (int)Math.Floor((MouseY - FolderY) / (float)FolderHeight);
            int FoldersOnLine = (Owner.LeftSideWidth - FolderX) / FolderOffsetX;
            int NumberOfFolderLines = (int)Math.Ceiling(FolderCount / (float)FoldersOnLine);

            if (IndexX >= 0 && IndexX < FoldersOnLine && IndexY >= 0 && IndexY < NumberOfFolderLines && IndexX + IndexY * FoldersOnLine < FolderCount)
            {
                if (MouseX - FolderX < IndexX * FolderOffsetX + FolderWidth)
                {
                    MenuSelection = MenuSelections.Folders;
                    SelectionIndex = IndexX + IndexY * FoldersOnLine;
                    return;
                }
            }
        }

        private void UpdateUnitsUnderMouse(int MouseX, int MouseY)
        {
            int UnitsY = GetUnitsY();
            UnitsY += 40;
            int IndexY = (int)Math.Floor((MouseY - UnitsY) / (float)UnitHeight);
            int MaxIndex = (int)Math.Floor((Owner.BottomSectionY - 155 - UnitsY) / (float)UnitHeight);

            if (MouseX >= UnitX && MouseX - UnitX < UnitWidth && IndexY >= 0 && IndexY < MaxIndex)
            {
                MenuSelection = MenuSelections.Unit;
                SelectionIndex = IndexY;
                return;
            }
        }

        private int GetRealUnitItem(out UnlockableItem Item, out UnlockableUnit ItemOwner)
        {
            int RealIndex = 0;
            int UnitIndex = 0;
            Item = null;
            ItemOwner = null;

            foreach (UnlockableUnit ActiveUnit in CurrentContainer.ListUnlockedUnit)
            {
                ItemOwner = ActiveUnit;
                if (SelectionIndex == RealIndex)
                {
                    Item = CurrentContainer.ListUnlockedUnit[UnitIndex];
                    return RealIndex;
                }
                if (RealIndex + ActiveUnit.ListUnlockedSkin.Count >= SelectionIndex)
                {
                    Item = CurrentContainer.ListUnlockedUnit[UnitIndex].ListUnlockedSkin[RealIndex + ActiveUnit.ListUnlockedSkin.Count - SelectionIndex];
                    return RealIndex + (RealIndex + ActiveUnit.ListUnlockedSkin.Count - SelectionIndex);
                }
                RealIndex += ActiveUnit.ListUnlockedSkin.Count;
                if (RealIndex + ActiveUnit.ListUnlockedAlt.Count >= SelectionIndex)
                {
                    Item = CurrentContainer.ListUnlockedUnit[UnitIndex].ListUnlockedAlt[RealIndex + ActiveUnit.ListUnlockedAlt.Count - SelectionIndex];
                    return RealIndex + (RealIndex + ActiveUnit.ListUnlockedAlt.Count - SelectionIndex);
                }
                RealIndex += ActiveUnit.ListUnlockedAlt.Count;
                if (RealIndex + ActiveUnit.ListLockedSkin.Count >= SelectionIndex)
                {
                    Item = CurrentContainer.ListUnlockedUnit[UnitIndex].ListLockedSkin[RealIndex + ActiveUnit.ListLockedSkin.Count - SelectionIndex];
                    return RealIndex + (RealIndex + ActiveUnit.ListLockedSkin.Count - SelectionIndex);
                }
                RealIndex += ActiveUnit.ListLockedSkin.Count;
                if (RealIndex + ActiveUnit.ListLockedAlt.Count >= SelectionIndex)
                {
                    Item = CurrentContainer.ListUnlockedUnit[UnitIndex].ListLockedAlt[RealIndex + ActiveUnit.ListLockedAlt.Count - SelectionIndex];
                    return RealIndex + (RealIndex + ActiveUnit.ListLockedAlt.Count - SelectionIndex);
                }
                RealIndex += ActiveUnit.ListLockedAlt.Count;
                ++RealIndex;
                ++UnitIndex;
            }

            return SelectionIndex;
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            g.GraphicsDevice.SetRenderTarget(UnitListRenderTarget);
            g.GraphicsDevice.Clear(Color.Transparent);
            g.Begin();
            DrawUnitList(g);
            g.End();
            g.GraphicsDevice.SetRenderTarget(null);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float Ratio = Constants.Height / 2160f;
            Color TextColor = Color.FromNonPremultiplied(65, 70, 65, 255);

            DrawFilters(g);
                        
            int DrawY = 300;
            g.Draw(UnitListRenderTarget, new Vector2(0, DrawY), Color.White);

            //Bottom side
            int DrawX = (int)(45 / Ratio);
            DrawY = (int)(Constants.Height - 210 * Ratio);
            g.Draw(sprSelectAUnitToBuy, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
            g.DrawString(fntOxanimumBold, "Select a Unit to buy", new Vector2(DrawX + (int)(12 / Ratio), DrawY + (int)(10 / Ratio)), TextColor);

            SquadListScrollbar.Draw(g);
            UnitWidth = 250 + 125 + 105 + 105 + 90 + 80 + 95 + 70;
            DrawY = Owner.BottomSectionY - 155;
            int BottomHeight = 182;
            DrawEmptyBox(g, new Vector2(5, DrawY), 240, BottomHeight);
            DrawEmptyBox(g, new Vector2(275, DrawY), 206, BottomHeight);
            DrawEmptyBox(g, new Vector2(509, DrawY), 106, BottomHeight);
            DrawEmptyBox(g, new Vector2(643, DrawY), 130, BottomHeight);

            DrawY = Owner.BottomSectionY;
            DrawY += Owner.BottomSectionHeight - 45;

            if (MenuSelection == MenuSelections.Unit && SelectionIndex < CurrentContainer.ListUnlockedUnit.Count)
            {
                UnlockableItem Itemfound;
                GetRealUnitItem(out Itemfound, out _);
                g.Draw(sprPixel, new Rectangle(UnitX, GetUnitsY() + 40 + UnitHeight * SelectionIndex, UnitWidth, 45), Color.FromNonPremultiplied(255, 255, 255, 150));
                if (Itemfound is UnlockableUnit)
                {
                    DrawSelectedUnitStats(g, ((UnlockableUnit)Itemfound).UnitToBuy);
                }
                else if (Itemfound is UnlockableUnitSkin)
                {
                    DrawSelectedUnitStats(g, ((UnlockableUnitSkin)Itemfound).UnitSkinToBuy);
                }
            }
        }

        private void DrawFilters(CustomSpriteBatch g)
        {
            float Ratio = Constants.Height / 2160f;
            int DrawY = Owner.MiddleSectionY + 15;

            g.DrawString(fntOxanimumBold, "Filters", new Vector2(58, DrawY + 11), Color.FromNonPremultiplied(65, 70, 65, 255));
            g.Draw(sprDropDownClose, new Vector2(180, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
            g.DrawString(fntOxanimumBold, "Sort By", new Vector2(458, DrawY + 11), Color.FromNonPremultiplied(65, 70, 65, 255));
            g.Draw(sprDropDownClose, new Vector2(580, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);

        }

        private void DrawUnitList(CustomSpriteBatch g)
        {
            Color TextColor = Color.FromNonPremultiplied(65, 70, 65, 255);
            float Ratio = Constants.Height / 2160f * 0.8f;
            int DrawY = GetFolderY() - 265;
            int DrawX = 5;

            DrawFolders(g, DrawY);

            DrawY += 140;

            int UnitStartIndex = (int)Math.Max(0, Math.Floor((SquadScrollbarValue - 158) / (double)UnitHeight));

            DrawY += UnitHeight * UnitStartIndex;

            for (int U = UnitStartIndex; U < CurrentContainer.ListUnlockedUnit.Count; ++U)
            {
                DrawX = UnitX;
                UnlockableUnit ActiveUnit = CurrentContainer.ListUnlockedUnit[U];
                if (ActiveUnit.UnitToBuy != null)
                {
                    DrawUnitDetails(g, ref DrawX, DrawY, ActiveUnit.UnitToBuy, true);
                    if (Inventory.DicOwnedUnit.ContainsKey(ActiveUnit.Path))
                    {
                        g.Draw(sprSelectAUnitToBuy, new Rectangle(DrawX + 20, DrawY, 100, 40), Color.White);
                        g.DrawStringRightAligned(fntOxanimumLight, Inventory.DicOwnedUnit[ActiveUnit.Path].QuantityOwned + " Owned",
                            new Vector2(DrawX + 108, DrawY + 11), TextColor);
                    }

                    if (ActiveUnit.ListUnlockedSkin.Count > 0)
                    {
                        if (ActiveUnit.ShowSkin || true)
                        {
                            DrawX = UnitX + 20;
                            g.DrawString(fntArial12, "v", new Vector2(DrawX - 20, DrawY + 11), Color.White);
                            foreach (UnlockableUnitSkin ActiveSkin in ActiveUnit.ListUnlockedSkin)
                            {
                                DrawY += UnitHeight;

                                if (ActiveSkin.UnitSkinToBuy != null)
                                {
                                    g.Draw(sprListEntrySub, new Vector2(DrawX, DrawY - 11), null, Color.White, 0, Vector2.Zero, Ratio, SpriteEffects.None, 0);
                                    DrawX += 35;
                                    DrawUnitDetails(g, ref DrawX, DrawY, ActiveSkin.UnitSkinToBuy, false);
                                    if (Inventory.DicOwnedUnit.ContainsKey(ActiveSkin.SkinTypeAndPath))
                                    {
                                        g.DrawStringRightAlignedBackground(fntArial12, "Owned", new Vector2(DrawX + 65, DrawY + 11), Color.White, sprPixel, TextColor);
                                    }
                                }
                            }
                            foreach (UnlockableUnitAlt a in ActiveUnit.ListUnlockedAlt)
                            {
                                DrawY += UnitHeight;
                            }
                            foreach (UnlockableUnitSkin ActiveSkin in ActiveUnit.ListLockedSkin)
                            {
                                if (ActiveSkin.UnitSkinToBuy != null)
                                {
                                    g.Draw(ActiveSkin.UnitSkinToBuy.SpriteMap, new Rectangle(DrawX + 9, DrawY + 7, 32, 32), Color.White);
                                    g.DrawString(fntArial12, ActiveSkin.UnitSkinToBuy.ItemName, new Vector2(DrawX + 46, DrawY + 11), Color.White);
                                }

                                DrawY += UnitHeight;
                            }
                            foreach (UnlockableUnitAlt a in ActiveUnit.ListLockedAlt)
                            {
                                DrawY += UnitHeight;
                            }
                        }
                        else
                        {
                            g.DrawString(fntArial12, ">", new Vector2(DrawX - 20, DrawY + 11), Color.White);
                        }
                    }
                }

                DrawY += UnitHeight;
            }

            //Locked Units
            for (int U = Math.Max(0, UnitStartIndex - CurrentContainer.ListUnlockedUnit.Count); U < CurrentContainer.ListLockedUnit.Count; ++U)
            {
                DrawX = UnitX;
                g.Draw(sprPixel, new Rectangle(DrawX, DrawY, UnitWidth - 80, 45), Color.FromNonPremultiplied(0, 0, 0, 100));
                DrawEmptyBox(g, new Vector2(DrawX, DrawY), 190, 45);
                DrawBox(g, new Vector2(DrawX + 6, DrawY + 4), 38, 38, Color.Gray);
                if (CurrentContainer.ListLockedUnit[U].UnitToBuy != null)
                {
                    g.DrawString(fntArial12, CurrentContainer.ListLockedUnit[U].UnitToBuy.ItemName, new Vector2(DrawX + 46, DrawY + 11), Color.DarkGray);
                    g.Draw(CurrentContainer.ListLockedUnit[U].UnitToBuy.SpriteMap, new Vector2(DrawX + 9, DrawY + 7), Color.White);
                }

                DrawX += 250 + 125 + 105 + 105 + 90 + 80;
                DrawEmptyBox(g, new Vector2(DrawX, DrawY), 85, 45);
                if (CurrentContainer.ListLockedUnit[U].UnitToBuy != null)
                {
                    g.DrawStringRightAligned(fntArial12, CurrentContainer.ListLockedUnit[U].UnitToBuy.Price + " cr", new Vector2(DrawX + 75, DrawY + 11), Color.DarkGray);
                }

                DrawY += UnitHeight;
            }

        }

        private void DrawUnitDetails(CustomSpriteBatch g, ref int DrawX, int DrawY, Unit ActiveUnit, bool DrawRank)
        {
            Color TextColor = Color.FromNonPremultiplied(65, 70, 65, 255);
            float Ratio = Constants.Height / 2160f * 0.8f;
            if (DrawRank)
            {
                g.Draw(sprListEntry, new Vector2(DrawX + 2, DrawY), null, Color.White, 0, Vector2.Zero, Ratio, SpriteEffects.None, 0);
            }
            g.Draw(ActiveUnit.SpriteMap, new Rectangle(DrawX + 9, DrawY + 7, 32, 32), Color.White);
            g.DrawString(fntOxanimumLight, ActiveUnit.ItemName, new Vector2(DrawX + 46, DrawY + 11), TextColor);

            DrawX += 250;

            if (DrawRank)
            {
                g.DrawString(fntOxanimumLight, "Rank: " + ActiveUnit.QualityRank + " (" + ActiveUnit.UnitStat.SpawnCost + ")", new Vector2(DrawX + 35, DrawY + 11), TextColor);
                DrawX += 125;
            }
            else
            {
                DrawX += 75;
            }
            g.DrawString(fntOxanimumLight, "HP: " + ActiveUnit.MaxHP.ToString(), new Vector2(DrawX + 40, DrawY + 11), TextColor);

            DrawX += 105;
            g.DrawString(fntOxanimumLight, "EN: " + ActiveUnit.MaxEN.ToString(), new Vector2(DrawX + 40, DrawY + 11), TextColor);

            DrawX += 105;
            g.DrawStringRightAligned(fntOxanimumLight, "MV: " + ActiveUnit.MaxMovement, new Vector2(DrawX + 80, DrawY + 11), TextColor);

            DrawX += 90;
            g.DrawStringRightAligned(fntOxanimumLight, "SPD: " + ActiveUnit.Mobility, new Vector2(DrawX + 92, DrawY + 11), TextColor);

            DrawX += 80;
            g.DrawStringRightAligned(fntOxanimumLight, ActiveUnit.Price + " cr", new Vector2(DrawX + 95, DrawY + 11), TextColor);

            DrawX += 95;
        }

        private void DrawFolders(CustomSpriteBatch g, int DrawY)
        {
            int FolderX = this.FolderX;
            int FolderOffsetX = (FolderWidth + 5);

            int FoldersOnLine = (Owner.LeftSideWidth - FolderX) / FolderOffsetX;
            int NumberOfFolderLines = (int)Math.Ceiling(CurrentContainer.DicFolder.Count / (float)FoldersOnLine);

            if (DrawY <= (NumberOfFolderLines - 1) * FolderHeight)
            {
                return;
            }

            g.DrawString(fntArial12, "FOLDERS", new Vector2(5, DrawY), Color.White);

            DrawY += 34;

            if (ListLastContainer.Count > 0)
            {
                g.Draw(sprPixel, new Rectangle(FolderX, DrawY, FolderWidth, FolderHeight), Color.FromNonPremultiplied(0, 0, 0, 50));
                DrawEmptyBox(g, new Vector2(FolderX, DrawY), FolderWidth, FolderHeight);
                g.Draw(ListLastContainer[ListLastContainer.Count - 1].IconUnit.UnitToBuy.SpriteMap,
                    new Rectangle(FolderX + FolderWidth / 2 - 16, DrawY + 7, 32, 32), Color.White);

                g.DrawStringMiddleAligned(fntArial12, "Last Folder", new Vector2(FolderX + FolderWidth / 2, DrawY + FolderHeight - 25), Color.White);
                FolderX += FolderOffsetX;
            }

            foreach (string ActiveFolder in CurrentContainer.DicFolder.Keys)
            {
                g.Draw(sprPixel, new Rectangle(FolderX, DrawY, FolderWidth, FolderHeight), Color.FromNonPremultiplied(0, 0, 0, 50));
                DrawEmptyBox(g, new Vector2(FolderX, DrawY), FolderWidth, FolderHeight);
                g.Draw(CurrentContainer.DicFolder[ActiveFolder].IconUnit.UnitToBuy.SpriteMap,
                    new Rectangle(FolderX + FolderWidth / 2 - 16, DrawY + 7, 32, 32), Color.White);

                g.DrawStringMiddleAligned(fntArial12, ActiveFolder, new Vector2(FolderX + FolderWidth / 2, DrawY + FolderHeight - 25), Color.White);
                FolderX += FolderOffsetX;
                if (FolderX > Owner.LeftSideWidth)
                {
                    FolderX = 20;
                    DrawY += FolderHeight;
                }
            }

            if (MenuSelection == MenuSelections.Folders)
            {
                g.Draw(sprPixel, new Rectangle(this.FolderX + FolderOffsetX * (SelectionIndex % FoldersOnLine), GetFolderY() + 45 + FolderHeight * (SelectionIndex / FoldersOnLine), FolderWidth, FolderHeight), Color.FromNonPremultiplied(255, 255, 255, 150));
            }
        }

        private void DrawSelectedUnitStats(CustomSpriteBatch g, Unit ActiveUnit)
        {
            int DrawY = Owner.BottomSectionY - 155;

            DrawY += 5;

            int DrawX = 15;

            int DistanceBetweenText = 16;
            int MenuOffset = (int)(DistanceBetweenText * 0.5);

            #region Stats 
            g.DrawString(fntFinlanderFont, "HP", new Vector2(DrawX, DrawY - MenuOffset + 10 + DistanceBetweenText), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveUnit.MaxHP.ToString(), new Vector2(DrawX + 207, DrawY - MenuOffset + 10 + DistanceBetweenText), Color.White);

            g.DrawString(fntFinlanderFont, "EN", new Vector2(DrawX, DrawY - MenuOffset + 10 + DistanceBetweenText * 2 + fntFinlanderFont.LineSpacing), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveUnit.MaxEN.ToString(), new Vector2(DrawX + 207, DrawY - MenuOffset + 10 + DistanceBetweenText * 2 + fntFinlanderFont.LineSpacing), Color.White);

            g.DrawString(fntFinlanderFont, "Armor", new Vector2(DrawX, DrawY - MenuOffset + 10 + DistanceBetweenText * 3 + fntFinlanderFont.LineSpacing * 2), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveUnit.Armor.ToString(), new Vector2(DrawX + 207, DrawY - MenuOffset + 10 + DistanceBetweenText * 3 + fntFinlanderFont.LineSpacing * 2), Color.White);
            g.DrawString(fntFinlanderFont, "Mobility", new Vector2(DrawX, DrawY - MenuOffset + 10 + DistanceBetweenText * 4 + fntFinlanderFont.LineSpacing * 3), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveUnit.Mobility.ToString(), new Vector2(DrawX + 207, DrawY - MenuOffset + 10 + DistanceBetweenText * 4 + fntFinlanderFont.LineSpacing * 3), Color.White);

            DrawX += 260 + 10;

            g.DrawString(fntFinlanderFont, "MV", new Vector2(DrawX, DrawY - MenuOffset + 10 + DistanceBetweenText), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveUnit.MaxMovement.ToString(), new Vector2(DrawX + 90, DrawY - MenuOffset + 10 + DistanceBetweenText), Color.White);
            g.DrawString(fntFinlanderFont, "Size", new Vector2(DrawX, DrawY - MenuOffset + 10 + DistanceBetweenText * 2 + fntFinlanderFont.LineSpacing), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, UnitStats.ListUnitSize[ActiveUnit.SizeIndex], new Vector2(DrawX + 90, DrawY - MenuOffset + 10 + DistanceBetweenText * 2 + fntFinlanderFont.LineSpacing), Color.White);
            g.DrawString(fntFinlanderFont, "Move Type", new Vector2(DrawX, DrawY - MenuOffset + 10 + DistanceBetweenText * 3 + fntFinlanderFont.LineSpacing * 2), Color.Yellow);

            #endregion

            if (ActiveUnit.DicRankByMovement.ContainsKey(UnitStats.TerrainAirIndex))
            {
                g.Draw(sprSky, new Vector2(DrawX, DrawY + 132), Color.White);
                DrawX += 50;
            }
            if (ActiveUnit.DicRankByMovement.ContainsKey(UnitStats.TerrainLandIndex))
            {
                g.Draw(sprLand, new Vector2(DrawX, DrawY + 132), Color.White);
                DrawX += 50;
            }
            if (ActiveUnit.DicRankByMovement.ContainsKey(UnitStats.TerrainSeaIndex))
            {
                g.Draw(sprSea, new Vector2(DrawX, DrawY + 132), Color.White);
                DrawX += 50;
            }
            if (ActiveUnit.DicRankByMovement.ContainsKey(UnitStats.TerrainSpaceIndex))
            {
                g.Draw(sprSpace, new Vector2(DrawX, DrawY + 132), Color.White);
                DrawX += 50;
            }

            int MiddlePosX = 520;

            int CurrentY = Owner.BottomSectionY - 150 + 52;
            DrawY = Owner.BottomSectionY - 155 + 5;

            #region Terrain

            g.DrawString(fntFinlanderFont, "Terrain", new Vector2(MiddlePosX + 3, DrawY - MenuOffset + 10 + DistanceBetweenText), Color.Yellow);

            g.Draw(sprSky, new Vector2(MiddlePosX + 10, CurrentY + 2), Color.White);
            if (ActiveUnit.UnitStat.DicRankByMovement.ContainsKey(UnitStats.TerrainAirIndex))
            {
                g.DrawString(fntFinlanderFont, ActiveUnit.TerrainLetterAttribute(UnitStats.TerrainAirIndex).ToString(), new Vector2(MiddlePosX + 34, CurrentY), Color.White);
            }
            else
            {
                g.DrawString(fntFinlanderFont, "-", new Vector2(MiddlePosX + 34, CurrentY), Color.White);
            }

            CurrentY += fntFinlanderFont.LineSpacing + 6;
            g.Draw(sprLand, new Vector2(MiddlePosX + 10, CurrentY + 2), Color.White);
            if (ActiveUnit.UnitStat.DicRankByMovement.ContainsKey(UnitStats.TerrainLandIndex))
            {
                g.DrawString(fntFinlanderFont, ActiveUnit.TerrainLetterAttribute(UnitStats.TerrainLandIndex).ToString(), new Vector2(MiddlePosX + 34, CurrentY), Color.White);
            }
            else
            {
                g.DrawString(fntFinlanderFont, "-", new Vector2(MiddlePosX + 34, CurrentY), Color.White);
            }

            CurrentY += fntFinlanderFont.LineSpacing + 6;
            g.Draw(sprSea, new Vector2(MiddlePosX + 10, CurrentY + 2), Color.White);
            if (ActiveUnit.UnitStat.DicRankByMovement.ContainsKey(UnitStats.TerrainSeaIndex))
            {
                g.DrawString(fntFinlanderFont, ActiveUnit.TerrainLetterAttribute(UnitStats.TerrainSeaIndex).ToString(), new Vector2(MiddlePosX + 34, CurrentY), Color.White);
            }
            else
            {
                g.DrawString(fntFinlanderFont, "-", new Vector2(MiddlePosX + 34, CurrentY), Color.White);
            }

            CurrentY += fntFinlanderFont.LineSpacing + 6;
            g.Draw(sprSpace, new Vector2(MiddlePosX + 10, CurrentY + 2), Color.White);
            if (ActiveUnit.UnitStat.DicRankByMovement.ContainsKey(UnitStats.TerrainSpaceIndex))
            {
                g.DrawString(fntFinlanderFont, ActiveUnit.TerrainLetterAttribute(UnitStats.TerrainSpaceIndex).ToString(), new Vector2(MiddlePosX + 34, CurrentY), Color.White);
            }
            else
            {
                g.DrawString(fntFinlanderFont, "-", new Vector2(MiddlePosX + 34, CurrentY), Color.White);
            }

            #endregion

            DrawY = Owner.BottomSectionY - 155 + 5;
            DrawX = 643 + 10;

            g.DrawString(fntFinlanderFont, "Rank", new Vector2(DrawX, DrawY - MenuOffset + 10 + DistanceBetweenText), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveUnit.QualityRank, new Vector2(DrawX + 120, DrawY - MenuOffset + 10 + DistanceBetweenText), Color.White);
            g.DrawString(fntFinlanderFont, "Spawn", new Vector2(DrawX, DrawY - MenuOffset + 10 + DistanceBetweenText * 3), Color.Yellow);
            g.DrawString(fntFinlanderFont, "Cost", new Vector2(DrawX, DrawY - MenuOffset + 10 + DistanceBetweenText * 4), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveUnit.UnitStat.SpawnCost.ToString(), new Vector2(DrawX + 120, DrawY - MenuOffset + 10 + DistanceBetweenText * 4), Color.White);
        }
    }
}
