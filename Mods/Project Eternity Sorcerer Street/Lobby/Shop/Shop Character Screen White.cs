using System;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ShopCharacterWhiteScreen : GameScreen
    {
        private enum MenuSelections { Nothing, Folders, Unit }

        private Scrollbar SquadListScrollbar;

        #region Ressources

        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;
        private SpriteFont fntFinlanderFont;
        private SpriteFont fntOxanimumBold;
        private SpriteFont fntOxanimumLight;

        private Texture2D sprListEntry;
        private Texture2D sprListEntrySub;
        private Texture2D sprArrowDown;
        private Texture2D sprArrowRight;

        private Texture2D sprDropDownClose;
        private Texture2D sprDropDownOpen;
        private Texture2D sprSelectAUnitToBuy;

        private Texture2D sprButtonFolderActive;
        private Texture2D sprButtonFolderInactive;

        private Texture2D sprFrameSelectedPopup;

        private Texture2D sprScrollbarBackground;
        private Texture2D sprScrollbar;

        private Texture2D sprLand;
        private Texture2D sprSea;
        private Texture2D sprSky;
        private Texture2D sprSpace;
        private RenderTarget2D UnitListRenderTarget;

        #endregion

        private MenuSelections MenuSelection;
        private int SelectionIndex;

        private readonly ShopScreen Owner;

        int FolderBoxWidth;
        int FolderBoxHeight;
        int FolderX;
        int FolderOffsetX;
        int FolderOffsetY;
        int UnitListY;

        int UnitX;
        int UnitWidth;
        int UnitHeight = 50;

        private int SquadScrollbarValue;

        private SorcererStreetPlayerUnlockInventory UnlockInventory;
        private SorcererStreetInventory Inventory;

        private SorcererStreetPlayerUnlockInventory.CharacterUnlockContainer CurrentContainer;
        private List<SorcererStreetPlayerUnlockInventory.CharacterUnlockContainer> ListLastContainer;

        public ShopCharacterWhiteScreen(ShopScreen Owner, SorcererStreetPlayerUnlockInventory UnlockInventory, SorcererStreetInventory Inventory)
        {
            this.Owner = Owner;
            this.UnlockInventory = UnlockInventory;
            this.Inventory = Inventory;
            CurrentContainer = UnlockInventory.RootCharacterContainer;
            ListLastContainer = new List<SorcererStreetPlayerUnlockInventory.CharacterUnlockContainer>();
            UnitX = 50;
            UnitWidth = 250 + 125 + 105 + 105 + 90 + 80 + 95 + 12;
        }

        public override void Load()
        {
            UnitListRenderTarget = new RenderTarget2D(GraphicsDevice, 1200, 650, false,
                GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            fntFinlanderFont = Content.Load<SpriteFont>("Fonts/Finlander Font");
            fntOxanimumBold = Content.Load<SpriteFont>("Fonts/Oxanium Bold");
            fntOxanimumLight = Content.Load<SpriteFont>("Fonts/Oxanium Light");

            sprListEntry = Content.Load<Texture2D>("Menus/Lobby/Shop/ContentFrame_Main");
            sprListEntrySub = Content.Load<Texture2D>("Menus/Lobby/Shop/ContentFrame_Sub");
            sprArrowDown = Content.Load<Texture2D>("Menus/Lobby/Shop/Arrow Down");
            sprArrowRight = Content.Load<Texture2D>("Menus/Lobby/Shop/Arrow Right");
            sprDropDownClose = Content.Load<Texture2D>("Menus/Lobby/Shop/Listing_Close");
            sprDropDownOpen = Content.Load<Texture2D>("Menus/Lobby/Shop/Listing_Open");
            sprSelectAUnitToBuy = Content.Load<Texture2D>("Menus/Lobby/Frame Outline");

            sprButtonFolderActive = Content.Load<Texture2D>("Menus/Lobby/Inventory/Folder Button Active");
            sprButtonFolderInactive = Content.Load<Texture2D>("Menus/Lobby/Inventory/Folder Button Inactive");

            sprFrameSelectedPopup = Content.Load<Texture2D>("Menus/Lobby/Shop/Frame Selected Popup");

            sprScrollbarBackground = Content.Load<Texture2D>("Menus/Lobby/Scrollbar Background");
            sprScrollbar = Content.Load<Texture2D>("Menus/Lobby/Scrollbar Bar");

            sprLand = Content.Load<Texture2D>("Menus/Status Screen/Ground");
            sprSea = Content.Load<Texture2D>("Menus/Status Screen/Sea");
            sprSky = Content.Load<Texture2D>("Menus/Status Screen/Sky");
            sprSpace = Content.Load<Texture2D>("Menus/Status Screen/Space");

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            float Ratio = Constants.Height / 2160f;

            FolderX = (int)(110 * Ratio);
            FolderBoxWidth = sprButtonFolderInactive.Width;
            FolderBoxHeight = sprButtonFolderInactive.Height;
            FolderOffsetX = (int)(40 * Ratio);
            FolderOffsetY = (int)(30 * Ratio);
            UnitListY = 600;

            SquadListScrollbar = new Scrollbar(sprScrollbar, new Vector2(UnitX + UnitWidth + 120, (UnitListY + 50) * Ratio), Ratio, (int)(sprScrollbarBackground.Height * Ratio), 10, OnSquadScrollbarChange);

            SquadListScrollbar.ChangeMaxValue(300);
        }

        public override void Update(GameTime gameTime)
        {
            float Ratio = Constants.Height / 2160f;

            if (Owner.OnlineGameClient != null)
            {
            }
            else
            {
                PendingUnlockScreen.CheckForUnlocks(this);
            }

            PendingUnlockScreen.UpdateUnlockScreens(this);

            SquadListScrollbar.Update(gameTime);

            int DrawY = 0;
            int FolderCount = CurrentContainer.DicFolder.Count;
            if (ListLastContainer.Count > 0)
            {
                FolderCount += 1;
            }

            int FoldersOnLine = (int)(UnitWidth / (FolderBoxWidth * Ratio));
            int NumberOfFolderLines = (int)Math.Ceiling(FolderCount / (float)FoldersOnLine);

            DrawY += (int)(FolderBoxHeight * NumberOfFolderLines * Ratio);
            DrawY += FolderOffsetY * NumberOfFolderLines;
            DrawY += GetUnitMenuHeight();

            SquadListScrollbar.ChangeMaxValue(DrawY);

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
                    UnlockablePlayerCharacter OwnerUnit;
                    GetRealUnitItem(out Itemfound, out OwnerUnit);
                    if (Itemfound is UnlockablePlayerCharacter)
                    {
                        if (Owner.OnlineGameClient != null)
                        {
                        }
                        else
                        {
                            PushScreen(new ShopConfirmBuyCharacterScreen(Inventory, (UnlockablePlayerCharacter)Itemfound));
                        }
                    }
                    else if (Itemfound is UnlockableCharacterSkin)
                    {
                        if (Owner.OnlineGameClient != null)
                        {
                        }
                        else
                        {
                            PushScreen(new ShopConfirmBuyCharacterSkinScreen(Inventory, OwnerUnit.Path, (UnlockableCharacterSkin)Itemfound));
                        }
                    }
                    else if (Itemfound is UnlockableCharacterAlt)
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
            return UnitListY - SquadScrollbarValue;
        }

        private int GetUnitsY()
        {
            float Ratio = Constants.Height / 2160f;
            int UnitY = (int)(GetFolderY() * Ratio);

            int FolderCount = CurrentContainer.DicFolder.Count;
            if (ListLastContainer.Count > 0)
            {
                FolderCount += 1;
            }

            int FoldersOnLine = (int)(UnitWidth / (FolderBoxWidth * Ratio));
            int NumberOfFolderLines = (int)Math.Ceiling(FolderCount / (float)FoldersOnLine);

            UnitY += (int)(FolderBoxHeight * NumberOfFolderLines * Ratio);
            UnitY += FolderOffsetY * NumberOfFolderLines;

            return UnitY;
        }

        private void UpdateFolderUnderMouse(int MouseX, int MouseY)
        {
            float Ratio = Constants.Height / 2160f;
            int FolderY = (int)(GetFolderY() * Ratio);
            if (MouseY < FolderY)
            {
                return;
            }

            int FolderCount = CurrentContainer.DicFolder.Count;
            if (ListLastContainer.Count > 0)
            {
                FolderCount += 1;
            }

            int IndexX = (int)Math.Floor((MouseX - FolderX) / (float)(FolderBoxWidth * Ratio + FolderOffsetX));
            int IndexY = (int)Math.Floor((MouseY - FolderY) / (float)(FolderBoxHeight * Ratio + FolderOffsetY));
            int FoldersOnLine = (int)(UnitWidth / (FolderBoxWidth * Ratio));
            int NumberOfFolderLines = (int)Math.Ceiling(FolderCount / (float)FoldersOnLine);

            if (IndexX >= 0 && IndexX < FoldersOnLine && IndexY >= 0 && IndexY < NumberOfFolderLines && IndexX + IndexY * FoldersOnLine < FolderCount)
            {
                if ((MouseX - FolderX) % ((FolderBoxWidth * Ratio) + FolderOffsetX) < (FolderBoxWidth * Ratio)
                    && (MouseY - FolderY) % ((FolderBoxHeight * Ratio) + FolderOffsetY) < (FolderBoxHeight * Ratio))
                {
                    MenuSelection = MenuSelections.Folders;
                    SelectionIndex = IndexX + IndexY * FoldersOnLine;
                    return;
                }
            }
        }

        private void UpdateUnitsUnderMouse(int MouseX, int MouseY)
        {
            float Ratio = Constants.Height / 2160f;
            int UnitsY = GetUnitsY();
            int IndexY = (int)Math.Floor((MouseY - UnitsY) / (float)UnitHeight);
            int MaxIndex = (int)Math.Floor((Owner.BottomSectionY - 155 - UnitsY) / (float)UnitHeight);

            if (MouseX >= UnitX && MouseX - UnitX < UnitWidth && IndexY >= 0 && IndexY < MaxIndex)
            {
                MenuSelection = MenuSelections.Unit;
                SelectionIndex = IndexY;
                return;
            }
        }

        private int GetRealUnitItem(out UnlockableItem Item, out UnlockablePlayerCharacter ItemOwner)
        {
            int RealIndex = 0;
            int UnitIndex = 0;
            Item = null;
            ItemOwner = null;

            foreach (UnlockablePlayerCharacter ActiveUnit in CurrentContainer.ListUnlockedCharacter)
            {
                ItemOwner = ActiveUnit;
                if (SelectionIndex == RealIndex)
                {
                    Item = CurrentContainer.ListUnlockedCharacter[UnitIndex];
                    return RealIndex;
                }
                if (RealIndex + ActiveUnit.ListUnlockedSkin.Count >= SelectionIndex)
                {
                    Item = CurrentContainer.ListUnlockedCharacter[UnitIndex].ListUnlockedSkin[RealIndex + ActiveUnit.ListUnlockedSkin.Count - SelectionIndex];
                    return RealIndex + (RealIndex + ActiveUnit.ListUnlockedSkin.Count - SelectionIndex);
                }
                RealIndex += ActiveUnit.ListUnlockedSkin.Count;
                if (RealIndex + ActiveUnit.ListUnlockedAlt.Count >= SelectionIndex)
                {
                    Item = CurrentContainer.ListUnlockedCharacter[UnitIndex].ListUnlockedAlt[RealIndex + ActiveUnit.ListUnlockedAlt.Count - SelectionIndex];
                    return RealIndex + (RealIndex + ActiveUnit.ListUnlockedAlt.Count - SelectionIndex);
                }
                RealIndex += ActiveUnit.ListUnlockedAlt.Count;
                if (RealIndex + ActiveUnit.ListLockedSkin.Count >= SelectionIndex)
                {
                    Item = CurrentContainer.ListUnlockedCharacter[UnitIndex].ListLockedSkin[RealIndex + ActiveUnit.ListLockedSkin.Count - SelectionIndex];
                    return RealIndex + (RealIndex + ActiveUnit.ListLockedSkin.Count - SelectionIndex);
                }
                RealIndex += ActiveUnit.ListLockedSkin.Count;
                if (RealIndex + ActiveUnit.ListLockedAlt.Count >= SelectionIndex)
                {
                    Item = CurrentContainer.ListUnlockedCharacter[UnitIndex].ListLockedAlt[RealIndex + ActiveUnit.ListLockedAlt.Count - SelectionIndex];
                    return RealIndex + (RealIndex + ActiveUnit.ListLockedAlt.Count - SelectionIndex);
                }
                RealIndex += ActiveUnit.ListLockedAlt.Count;
                ++RealIndex;
                ++UnitIndex;
            }

            return SelectionIndex;
        }

        private int GetUnitMenuHeight()
        {
            int DrawY = 0;
            for (int U = 0; U < CurrentContainer.ListUnlockedCharacter.Count; ++U)
            {
                UnlockablePlayerCharacter ActiveCharacter = CurrentContainer.ListUnlockedCharacter[U];
                if (ActiveCharacter.CharacterToBuy != null)
                {
                    if (ActiveCharacter.ListUnlockedSkin.Count > 0)
                    {
                        if (ActiveCharacter.ShowSkin || true)
                        {
                            foreach (UnlockableCharacterSkin ActiveSkin in ActiveCharacter.ListUnlockedSkin)
                            {
                                DrawY += UnitHeight;

                            }
                            foreach (UnlockableCharacterAlt a in ActiveCharacter.ListUnlockedAlt)
                            {
                                DrawY += UnitHeight;
                            }
                            foreach (UnlockableCharacterSkin ActiveSkin in ActiveCharacter.ListLockedSkin)
                            {
                                if (ActiveSkin.CharacterSkinToBuy != null)
                                {
                                }

                                DrawY += UnitHeight;
                            }
                            foreach (UnlockableCharacterAlt a in ActiveCharacter.ListLockedAlt)
                            {
                                DrawY += UnitHeight;
                            }
                        }
                    }
                }

                DrawY += UnitHeight;
            }

            for (int U = 0; U < CurrentContainer.ListLockedCharacter.Count; ++U)
            {
                DrawY += UnitHeight;
            }

            return DrawY;
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
                        
            int DrawY = (int)(UnitListY * Ratio);
            g.Draw(UnitListRenderTarget, new Vector2(0, DrawY), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);

            //Bottom side
            int DrawX = (int)(45 / Ratio);
            DrawY = (int)(Constants.Height - 210 * Ratio);
            g.Draw(sprSelectAUnitToBuy, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
            g.DrawString(fntOxanimumBold, "Select a Unit to buy", new Vector2(DrawX + (int)(12 / Ratio), DrawY + (int)(10 / Ratio)), TextColor);

            g.Draw(sprScrollbarBackground, new Vector2(UnitX + UnitWidth + 120, (UnitListY + 50) * Ratio), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.7f);

            SquadListScrollbar.Draw(g);

            if (MenuSelection == MenuSelections.Unit && SelectionIndex < CurrentContainer.ListUnlockedCharacter.Count)
            {
                UnlockableItem Itemfound;
                GetRealUnitItem(out Itemfound, out _);
                g.Draw(sprPixel, new Rectangle(UnitX, GetUnitsY() + UnitHeight * SelectionIndex, UnitWidth, 45), Color.FromNonPremultiplied(255, 255, 255, 150));
                if (Itemfound is UnlockablePlayerCharacter)
                {
                    DrawSelectedUnitStats(g, ((UnlockablePlayerCharacter)Itemfound).CharacterToBuy);
                }
                else if (Itemfound is UnlockableCharacterSkin)
                {
                    DrawSelectedUnitStats(g, ((UnlockableCharacterSkin)Itemfound).CharacterSkinToBuy);
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
            int DrawX = 5;

            float Ratio = Constants.Height / 2160f;
            int DrawY = (int)((GetFolderY() - UnitListY) * Ratio);
            DrawFolders(g, DrawY);

            DrawY = GetUnitsY() - (int)(UnitListY * Ratio);

            int UnitStartIndex = (int)Math.Max(0, Math.Floor(-DrawY / (double)UnitHeight));
            DrawY += UnitStartIndex * UnitHeight;
            Ratio = Constants.Height / 2160f * 0.8f;
            for (int U = UnitStartIndex; U < CurrentContainer.ListUnlockedCharacter.Count; ++U)
            {
                DrawX = UnitX;
                UnlockablePlayerCharacter ActiveUnit = CurrentContainer.ListUnlockedCharacter[U];
                if (ActiveUnit.CharacterToBuy != null)
                {
                    DrawUnitDetails(g, ref DrawX, DrawY, ActiveUnit.CharacterToBuy, true);
                    if (Inventory.DicOwnedCharacter.ContainsKey(ActiveUnit.Path))
                    {
                        g.Draw(sprSelectAUnitToBuy, new Rectangle(DrawX + 20, DrawY, 100, 40), Color.White);
                        g.DrawStringRightAligned(fntOxanimumLight, "Owned",
                            new Vector2(DrawX + 108, DrawY + 11), TextColor);
                    }

                    if (ActiveUnit.ListUnlockedSkin.Count > 0)
                    {
                        if (ActiveUnit.ShowSkin || true)
                        {
                            DrawX = UnitX + (int)(50 * Ratio);
                            g.Draw(sprArrowDown, new Vector2(DrawX - 130 * Ratio, DrawY + 40 * Ratio), null, Color.White, 0, Vector2.Zero, Ratio, SpriteEffects.None, 0);
                            foreach (UnlockableCharacterSkin ActiveSkin in ActiveUnit.ListUnlockedSkin)
                            {
                                DrawY += UnitHeight;

                                if (ActiveSkin.CharacterSkinToBuy != null)
                                {
                                    g.Draw(sprListEntrySub, new Vector2(DrawX, DrawY - 11), null, Color.White, 0, Vector2.Zero, Ratio, SpriteEffects.None, 0);
                                    DrawX += 35;
                                    DrawUnitDetails(g, ref DrawX, DrawY, ActiveSkin.CharacterSkinToBuy, false);
                                    if (Inventory.DicOwnedCharacter.ContainsKey(ActiveSkin.SkinTypeAndPath))
                                    {
                                        g.DrawStringRightAlignedBackground(fntArial12, "Owned", new Vector2(DrawX + 65, DrawY + 11), Color.White, sprPixel, TextColor);
                                    }
                                }
                            }
                            foreach (UnlockableCharacterAlt a in ActiveUnit.ListUnlockedAlt)
                            {
                                DrawY += UnitHeight;
                            }
                            foreach (UnlockableCharacterSkin ActiveSkin in ActiveUnit.ListLockedSkin)
                            {
                                if (ActiveSkin.CharacterSkinToBuy != null)
                                {
                                    g.Draw(ActiveSkin.CharacterSkinToBuy.SpriteMap, new Rectangle(DrawX + 9, DrawY + 7, 32, 32), Color.White);
                                    g.DrawString(fntArial12, ActiveSkin.CharacterSkinToBuy.Name, new Vector2(DrawX + 46, DrawY + 11), Color.White);
                                }

                                DrawY += UnitHeight;
                            }
                            foreach (UnlockableCharacterAlt a in ActiveUnit.ListLockedAlt)
                            {
                                DrawY += UnitHeight;
                            }
                        }
                        else
                        {
                            g.Draw(sprArrowRight, new Vector2(DrawX - 130 * Ratio, DrawY + 40 * Ratio), null, Color.White, 0, Vector2.Zero, Ratio, SpriteEffects.None, 0);
                        }
                    }
                }

                DrawY += UnitHeight;
            }

            //Locked Units
            for (int U = Math.Max(0, UnitStartIndex - CurrentContainer.ListUnlockedCharacter.Count); U < CurrentContainer.ListLockedCharacter.Count; ++U)
            {
                DrawX = UnitX;
                g.Draw(sprPixel, new Rectangle(DrawX, DrawY, UnitWidth - 80, 45), Color.FromNonPremultiplied(0, 0, 0, 100));
                DrawEmptyBox(g, new Vector2(DrawX, DrawY), 190, 45);
                DrawBox(g, new Vector2(DrawX + 6, DrawY + 4), 38, 38, Color.Gray);
                if (CurrentContainer.ListLockedCharacter[U].CharacterToBuy != null)
                {
                    g.DrawString(fntArial12, CurrentContainer.ListLockedCharacter[U].CharacterToBuy.Name, new Vector2(DrawX + 46, DrawY + 11), Color.DarkGray);
                    g.Draw(CurrentContainer.ListLockedCharacter[U].CharacterToBuy.SpriteMap, new Vector2(DrawX + 9, DrawY + 7), Color.White);
                }

                DrawX += 250 + 125 + 105 + 105 + 90 + 80;
                DrawEmptyBox(g, new Vector2(DrawX, DrawY), 85, 45);
                if (CurrentContainer.ListLockedCharacter[U].CharacterToBuy != null)
                {
                    g.DrawStringRightAligned(fntArial12, CurrentContainer.ListLockedCharacter[U].CharacterToBuy.Price + " cr", new Vector2(DrawX + 75, DrawY + 11), Color.DarkGray);
                }

                DrawY += UnitHeight;
            }

        }

        private void DrawUnitDetails(CustomSpriteBatch g, ref int DrawX, int DrawY, PlayerCharacter ActiveUnit, bool DrawRank)
        {
            Color TextColor = Color.FromNonPremultiplied(65, 70, 65, 255);
            float Ratio = Constants.Height / 2160f * 0.8f;
            if (DrawRank)
            {
                g.Draw(sprListEntry, new Vector2(DrawX + 2, DrawY), null, Color.White, 0, Vector2.Zero, Ratio, SpriteEffects.None, 0);
            }
            g.Draw(ActiveUnit.SpriteMap, new Rectangle(DrawX + 9, DrawY + 7, 32, 32), Color.White);
            g.DrawString(fntOxanimumLight, ActiveUnit.Name, new Vector2(DrawX + 46, DrawY + 11), TextColor);

            DrawX += 250;

            DrawX += 80;
            g.DrawStringRightAligned(fntOxanimumLight, ActiveUnit.Price + " cr", new Vector2(DrawX + 95, DrawY + 11), TextColor);

            DrawX += 95;
        }

        private void DrawFolders(CustomSpriteBatch g, int DrawY)
        {
            float Ratio = Constants.Height / 2160f;

            int FoldersOnLine = (int)(UnitWidth / (FolderBoxWidth * Ratio));
            int StartIndex = (int)Math.Floor(SquadScrollbarValue / (double)FolderBoxHeight);
            DrawY += (int)(FolderBoxHeight * Ratio + FolderOffsetY) * StartIndex;
            StartIndex *= FoldersOnLine;

            int TotalItem = CurrentContainer.ListFolder.Count;

            if (ListLastContainer.Count > 0)
            {
                TotalItem += 1;
            }

            int XPos = 0;

            if (ListLastContainer.Count > 0)
            {
                int FinalX = (int)(FolderX + XPos * (FolderBoxWidth * Ratio + FolderOffsetX));
                if (MenuSelection == MenuSelections.Folders && XPos + 0 == SelectionIndex)
                {
                    g.Draw(sprButtonFolderActive, new Vector2(FinalX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
                }
                else
                {
                    g.Draw(sprButtonFolderInactive, new Vector2(FinalX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
                }

                g.DrawStringCentered(fntOxanimumLight, "Last Folder", new Vector2(FinalX + FolderBoxWidth * Ratio / 2, DrawY + FolderBoxHeight * Ratio / 2), Color.White);

                ++XPos;
            }

            for (int YPos = StartIndex; YPos < TotalItem; YPos += FoldersOnLine)
            {
                while (XPos < FoldersOnLine && XPos + YPos < TotalItem)
                {
                    int FinalX = (int)(FolderX + XPos * (FolderBoxWidth * Ratio + FolderOffsetX));
                    int CurrentIndex = XPos + YPos;

                    if (ListLastContainer.Count > 0)
                    {
                        CurrentIndex -= 1;
                    }

                    if (MenuSelection == MenuSelections.Folders && XPos + YPos * FoldersOnLine == SelectionIndex)
                    {
                        g.Draw(sprButtonFolderActive, new Vector2(FinalX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
                    }
                    else
                    {
                        g.Draw(sprButtonFolderInactive, new Vector2(FinalX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
                    }

                    g.DrawStringCentered(fntOxanimumLight, CurrentContainer.ListFolder[CurrentIndex].Name, new Vector2(FinalX + FolderBoxWidth * Ratio / 2, DrawY + FolderBoxHeight * Ratio / 2), Color.White);

                    ++XPos;
                }

                XPos = 0;

                DrawY += (int)(FolderBoxHeight * Ratio + FolderOffsetY);
            }
        }

        private void DrawSelectedUnitStats(CustomSpriteBatch g, PlayerCharacter ActiveUnit)
        {
            float Ratio = Constants.Height / 2160f;
            int DrawX = 2505;
            int DrawY = 555;
            Color TextColor = Color.FromNonPremultiplied(65, 70, 65, 255);
            Color TextColorYellow = Color.FromNonPremultiplied(174, 138, 13, 255);


            g.Draw(sprFrameSelectedPopup, new Vector2(DrawX * Ratio, DrawY * Ratio), null, Color.White, 0, Vector2.Zero, Ratio, SpriteEffects.None, 0.8f);

            g.DrawStringCentered(fntOxanimumBold, ActiveUnit.Name, new Vector2((DrawX + sprFrameSelectedPopup.Width / 2) * Ratio, (DrawY + 80) * Ratio), Color.FromNonPremultiplied(243, 243, 243, 255));


            int MiddlePosX = 520;

            int CurrentY = Owner.BottomSectionY - 150 + 52;
            DrawY = 1290;
        }
    }
}
