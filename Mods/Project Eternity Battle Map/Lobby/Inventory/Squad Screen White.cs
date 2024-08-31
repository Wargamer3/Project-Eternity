using System;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class InventorySquadWhiteScreen : GameScreen
    {
        private Scrollbar LoadoutListScrollbar;
        private Scrollbar InventoryScrollbar;

        #region Ressources

        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;
        private SpriteFont fntOxanimumLight;
        private SpriteFont fntOxanimumLightSmall;
        private SpriteFont fntOxanimumRegularSmall;
        private SpriteFont fntOxanimumBoldSmall;
        private SpriteFont fntOxanimumBoldSmaller;
        private SpriteFont fntOxanimumBoldBig;

        private Texture2D sprLoadouts;
        private Texture2D sprLoadoutsFrame;
        private Texture2D sprLoadoutsName;
        private Texture2D sprFrameDescription;

        private Texture2D sprButtonFolderInactive;
        private Texture2D sprButtonRename;

        private Texture2D sprScrollbarBackground;
        private Texture2D sprScrollbar;

        #endregion

        public static int LoadoutX;
        public static int LoadoutEntryHeightWithOffset;
        public static int LoadoutBoxSize;
        public static int LoadoutBoxWidthWithOffset;
        public static int LoadoutBoxHeightWithOffset;

        public static int BoxHeight;
        public static int BoxWidth;
        public static int BoxWidthFinal;
        public static int BoxOffset;
        public static int BoxOffsetFinal ;
        public static int BoxWithOffsetFinal;
        public static int SpriteSizeFinal;
        public static int SpriteOffset;

        public static int InventoryX;
        public static int InventoryWidth;
        public static int BoxPerLine;

        public static int FolderBoxWidth;
        public static int FolderBoxHeight;
        public static int FolderWidthFinal;
        public static int FolderBoxHeightFinal;
        public static int FolderOffset;
        public static int FolderOffsetFinal;
        public static int FolderWithOffsetFinal;
        public static int FolderPerLine;

        public static int BoxLineHeight;

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
        }

        public override void Load()
        {
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            fntOxanimumLight = Content.Load<SpriteFont>("Fonts/Oxanium Light Big");
            fntOxanimumLightSmall = Content.Load<SpriteFont>("Fonts/Oxanium Light");
            fntOxanimumRegularSmall = Content.Load<SpriteFont>("Fonts/Oxanium Regular Small");
            fntOxanimumBoldSmall = Content.Load<SpriteFont>("Fonts/Oxanium Bold Small");
            fntOxanimumBoldSmaller = Content.Load<SpriteFont>("Fonts/Oxanium Bold Smaller");
            fntOxanimumBoldBig = Content.Load<SpriteFont>("Fonts/Oxanium Bold Big");

            sprLoadouts = Content.Load<Texture2D>("Menus/Lobby/Frame Outline");
            sprLoadoutsFrame = Content.Load<Texture2D>("Menus/Lobby/Inventory/Frame Loadout");
            sprLoadoutsName = Content.Load<Texture2D>("Menus/Lobby/Inventory/Frame Loadout Name");
            sprFrameDescription = Content.Load<Texture2D>("Menus/Lobby/Inventory/Frame Description");

            sprButtonFolderInactive = Content.Load<Texture2D>("Menus/Lobby/Inventory/Folder Button Inactive");
            sprButtonRename = Content.Load<Texture2D>("Menus/Lobby/Inventory/Button Rename");

            sprScrollbarBackground = Content.Load<Texture2D>("Menus/Lobby/Scrollbar Background");
            sprScrollbar = Content.Load<Texture2D>("Menus/Lobby/Scrollbar Bar");

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            float Ratio = Constants.Height / 2160f;

            InventoryScrollbar = new Scrollbar(sprScrollbar, new Vector2(2144 * Ratio, 648 * Ratio), Ratio, (int)(sprScrollbarBackground.Height * Ratio), 10, OnInventoryScrollbarChange);
            LoadoutListScrollbar = new Scrollbar(sprScrollbar, new Vector2(2144 * Ratio, 648 * Ratio), Ratio, (int)(sprScrollbarBackground.Height * Ratio), 10, OnSquadScrollbarChange);

            LoadoutX = (int)(2250 * Ratio);
            LoadoutEntryHeightWithOffset = (int)(190 * Ratio);
            LoadoutBoxSize = (int)(68 * Ratio);
            LoadoutBoxWidthWithOffset = (int)(84 * Ratio);
            LoadoutBoxHeightWithOffset = (int)(86 * Ratio);

            BoxHeight = 120;
            BoxWidth = 176;
            BoxWidthFinal = (int)(BoxWidth * Ratio);
            BoxOffset = 88;
            BoxOffsetFinal = (int)(BoxOffset * Ratio);
            BoxWithOffsetFinal = (int)(BoxWidthFinal + BoxOffsetFinal);
            SpriteSizeFinal = (int)(128 * Ratio);
            SpriteOffset = (BoxWidthFinal - SpriteSizeFinal) / 2;

            BoxLineHeight = (int)(276 * Ratio);

            InventoryX = (int)(140 * Ratio);
            InventoryWidth = (int)(LoadoutX - InventoryX - BoxWidthFinal - BoxOffset);
            BoxPerLine = InventoryWidth / BoxWithOffsetFinal;

            FolderBoxWidth = sprButtonFolderInactive.Width;
            FolderBoxHeight = (int)(sprButtonFolderInactive.Height);
            FolderWidthFinal = (int)(FolderBoxWidth * Ratio);
            FolderBoxHeightFinal = (int)(FolderBoxHeight * Ratio);
            FolderOffset = (int)(40 * Ratio);
            FolderOffsetFinal = (int)(FolderOffset * Ratio);
            FolderWithOffsetFinal = (int)(FolderWidthFinal + FolderOffsetFinal);
            FolderPerLine = InventoryWidth / FolderWithOffsetFinal;

            LoadoutListScrollbar.ChangeMaxValue(Owner.ActivePlayer.Inventory.ListSquadLoadout.Count * LoadoutEntryHeightWithOffset - BattleMapInventoryWhiteScreen.MiddleSectionHeight);
            InventoryScrollbar.ChangeMaxValue(CurrentContainer.ListUnit.Count * BoxHeight - BattleMapInventoryWhiteScreen.MiddleSectionHeight);
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsDragDropActive)
            {
                LoadoutListScrollbar.Update(gameTime);
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

                if (ListLastContainer.Count > 0)
                {
                    --SelectedItemIndex;
                }

                if (SelectedItemIndex >= 0)
                {
                    StartDragDrop(CurrentContainer.ListUnit[SelectedItemIndex]);
                }
                else
                {
                    int SelectedFolderIndex = GetFolderUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);

                    if (SelectedFolderIndex >= 0)
                    {
                        if (ListLastContainer.Count > 0 && SelectedFolderIndex == 0)
                        {
                            CurrentContainer = ListLastContainer[ListLastContainer.Count - 1];
                            ListLastContainer.Remove(CurrentContainer);
                        }
                        else
                        {
                            ListLastContainer.Add(CurrentContainer);
                            if (ListLastContainer.Count > 1)
                            {
                                CurrentContainer = CurrentContainer.ListFolder[SelectedFolderIndex - 1];
                            }
                            else
                            {
                                CurrentContainer = CurrentContainer.ListFolder[SelectedFolderIndex];
                            }
                        }
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
            float Ratio = Constants.Height / 2160f;
            int DrawX = LoadoutX;
            int DrawY = (int)(462 * Ratio - SquadScrollbarValue % LoadoutEntryHeightWithOffset);

            if (InputHelper.InputConfirmReleased())
            {
                int MouseX = MouseHelper.MouseStateCurrent.X;
                int MouseY = MouseHelper.MouseStateCurrent.Y;
                int MouseXFinal = (int)((MouseX - DrawX) % BoxWithOffsetFinal);
                int MouseYFinal = (MouseY - DrawY) % BoxLineHeight;

                int SquadSlotIndex = (MouseHelper.MouseStateCurrent.X - DrawX) / LoadoutBoxWidthWithOffset;
                int SqaudLoatoutIndex = (MouseHelper.MouseStateCurrent.Y - DrawY) / LoadoutEntryHeightWithOffset;

                if (SquadSlotIndex >= 0 && SquadSlotIndex < LoadoutSize && SqaudLoatoutIndex < Owner.ActivePlayer.Inventory.ListSquadLoadout.Count
                    && MouseXFinal >= 0 && MouseXFinal < LoadoutBoxSize
                    && MouseYFinal >= 0 && MouseYFinal < LoadoutBoxSize)
                {
                    Character OldPilot = Owner.ActivePlayer.Inventory.ListSquadLoadout[SqaudLoatoutIndex].ListSpawnSquad[SquadSlotIndex].At(0).Pilot;
                    Squad ReplacementSquad = new Squad("Squad " + SquadSlotIndex, DragAndDropEquipment.Leader);
                    ReplacementSquad.At(0).ArrayCharacterActive[0] = OldPilot;

                    Owner.ActivePlayer.Inventory.ListSquadLoadout[SqaudLoatoutIndex].ListSpawnSquad[SquadSlotIndex] = new Squad("Squad " + SquadSlotIndex, DragAndDropEquipment.Leader);
                    Owner.ActivePlayer.SaveLocally();
                }

                DragAndDropEquipment = null;
            }
        }

        private int GetOwnedSquadUnderMouse(int MouseX, int MouseY)
        {
            float Ratio = Constants.Height / 2160f;
            int X = (int)(140 * Ratio);

            int DrawY = (int)(648 * Ratio - InventoryScrollbarValue % BoxHeight);

            int MouseIndex = (int)((MouseX - X) / BoxWithOffsetFinal + ((MouseY - DrawY) / BoxLineHeight) * BoxPerLine);
            int MouseXFinal = (int)((MouseX - X) % BoxWithOffsetFinal);
            int MouseYFinal = (MouseY - DrawY) % BoxLineHeight;

            int ItemCount = CurrentContainer.ListUnit.Count;

            if (MouseIndex >= 0 && MouseIndex < ItemCount
                && MouseXFinal > 0 && MouseXFinal < BoxWidthFinal && MouseX < InventoryWidth
                && MouseYFinal >= 0 && MouseYFinal < BoxLineHeight)
            {
                return MouseIndex;
            }

            return -1;
        }

        private int GetFolderUnderMouse(int MouseX, int MouseY)
        {
            float Ratio = Constants.Height / 2160f;

            int X = (int)(140 * Ratio);
            int DrawY = (int)(444 * Ratio) - InventoryScrollbarValue % BoxHeight;

            int MouseIndex = (int)((MouseX - X) / FolderWithOffsetFinal + ((MouseY - DrawY) / BoxLineHeight) * BoxPerLine);
            int MouseXFinal = (int)((MouseX - X) % FolderWithOffsetFinal);
            int MouseYFinal = (MouseY - DrawY) % BoxLineHeight;

            int ItemCount = CurrentContainer.ListFolder.Count;
            if (ListLastContainer.Count > 0)
            {
                ItemCount += 1;
            }

            if (MouseIndex >= 0 && MouseIndex < ItemCount
                && MouseXFinal > 0 && MouseXFinal < FolderWidthFinal && MouseXFinal < InventoryWidth
                && MouseYFinal >= 4 && MouseYFinal < FolderBoxHeightFinal)
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
            g.Draw(sprScrollbarBackground, new Vector2(2144 * Ratio, 648 * Ratio), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);

            InventoryScrollbar.Draw(g);
            //LoadoutListScrollbar.Draw(g);

            DrawLoadout(g);

            DrawInventory(g, InventoryX);

            if (DragAndDropEquipment != null)
            {
                g.Draw(DragAndDropEquipment.Leader.SpriteMap, new Rectangle(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y, LoadoutBoxSize, LoadoutBoxSize), Color.White);
            }
        }

        private void DrawLoadout(CustomSpriteBatch g)
        {
            float Ratio = Constants.Height / 2160f;
            Color TextColor = Color.FromNonPremultiplied(65, 70, 65, 255);
            g.Draw(sprLoadouts, new Vector2(LoadoutX - 80 * Ratio, 260 * Ratio), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
            g.Draw(sprLoadoutsFrame, new Vector2(LoadoutX - 30 * Ratio, 416 * Ratio), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
            g.DrawString(fntOxanimumBoldBig, "Loadouts", new Vector2((BattleMapInventoryWhiteScreen.LoadoutX) * Ratio, 294 * Ratio), TextColor);

            int DrawY = (int)(462 * Ratio - SquadScrollbarValue % LoadoutEntryHeightWithOffset);

            for (int L = (int)Math.Floor(SquadScrollbarValue / (double)LoadoutEntryHeightWithOffset); L < 5; ++L)
            {
                int DrawX = LoadoutX;

                g.Draw(sprLoadoutsName, new Vector2((int)(DrawX + 420 * Ratio), DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
                g.Draw(sprButtonRename, new Vector2((int)(DrawX + 1074 * Ratio), DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
                g.DrawString(fntOxanimumRegularSmall, "Rename", new Vector2((int)(DrawX + 1114 * Ratio), (int)((DrawY + 16 * Ratio))), TextColor);
                g.DrawString(fntOxanimumRegularSmall, "Loadout " + (L + 1), new Vector2((int)(DrawX + 466 * Ratio), (int)((DrawY + 16 * Ratio))), TextColor);

                for (int S = 0; S < 5; S++)
                {
                    g.Draw(sprPixel, new Rectangle(DrawX, DrawY, LoadoutBoxSize, LoadoutBoxSize), Color.FromNonPremultiplied(10, 10, 10, 255));
                    g.Draw(sprPixel, new Rectangle(DrawX, DrawY + LoadoutBoxHeightWithOffset, LoadoutBoxSize, LoadoutBoxSize), Color.FromNonPremultiplied(10, 10, 10, 255));

                    if (L < Owner.ActivePlayer.Inventory.ListSquadLoadout.Count && S < Owner.ActivePlayer.Inventory.ListSquadLoadout[L].ListSpawnSquad.Count)
                    {
                        Squad ActiveSquad = Owner.ActivePlayer.Inventory.ListSquadLoadout[L].ListSpawnSquad[S];
                        if (ActiveSquad != null)
                        {
                            g.Draw(ActiveSquad.CurrentLeader.SpriteMap, new Rectangle(DrawX, (int)(DrawY), LoadoutBoxSize, LoadoutBoxSize), Color.White);

                            if (ActiveSquad.CurrentLeader.Pilot != null)
                            {
                                g.Draw(ActiveSquad.CurrentLeader.Pilot.sprPortrait, new Rectangle(DrawX, DrawY + LoadoutBoxHeightWithOffset, LoadoutBoxSize, LoadoutBoxSize), Color.White);
                            }
                        }
                    }

                    if (DragAndDropEquipment != null)
                    {
                        g.Draw(sprPixel, new Rectangle(DrawX, DrawY, LoadoutBoxSize, LoadoutBoxSize), Color.FromNonPremultiplied(255, 255, 255, 127));
                    }
                    if (MouseHelper.MouseStateCurrent.X >= DrawX && MouseHelper.MouseStateCurrent.X < DrawX + LoadoutBoxSize
                        && MouseHelper.MouseStateCurrent.Y >= DrawY && MouseHelper.MouseStateCurrent.Y < DrawY + LoadoutBoxSize)
                    {
                        g.Draw(sprPixel, new Rectangle(DrawX, DrawY, LoadoutBoxSize, LoadoutBoxSize), Color.FromNonPremultiplied(255, 255, 255, 127));
                    }

                    DrawX += LoadoutBoxWidthWithOffset;
                }

                DrawY += LoadoutEntryHeightWithOffset;
            }
        }

        private void DrawFolders(CustomSpriteBatch g, int X)
        {
            Color TextColor = Color.FromNonPremultiplied(65, 70, 65, 255);
            float Ratio = Constants.Height / 2160f;
            int DrawY = (int)(444 * Ratio) - InventoryScrollbarValue % BoxHeight;

            int StartIndex = (int)Math.Floor(InventoryScrollbarValue / (double)FolderBoxHeight);
            int TotalItem = CurrentContainer.ListFolder.Count;

            if (ListLastContainer.Count > 0)
            {
                TotalItem += 1;
            }

            int XPos = 0;

            if (ListLastContainer.Count > 0)
            {
                int FinalX = X + XPos * FolderWithOffsetFinal;

                g.Draw(sprButtonFolderInactive, new Vector2(FinalX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
                g.DrawStringCentered(fntOxanimumLight, "Last Folder", new Vector2(FinalX + FolderBoxWidth * Ratio / 2, DrawY + FolderBoxHeightFinal / 2 + 6 * Ratio), TextColor);

                ++XPos;
            }

            for (int YPos = StartIndex; YPos < TotalItem; YPos += FolderPerLine)
            {
                while (XPos < FolderPerLine && XPos + YPos < TotalItem)
                {
                    int FinalX = X + XPos * FolderWithOffsetFinal;
                    int CurrentIndex = XPos + YPos;

                    if (ListLastContainer.Count > 0)
                    {
                        CurrentIndex -= 1;
                    }

                    g.Draw(sprButtonFolderInactive, new Vector2(FinalX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
                    g.DrawStringCentered(fntOxanimumLight, CurrentContainer.ListFolder[CurrentIndex].Name, new Vector2(FinalX + FolderBoxWidth * Ratio / 2, DrawY + FolderBoxHeightFinal / 2 + 6 * Ratio), TextColor);

                    ++XPos;
                }

                XPos = 0;

                DrawY += BoxLineHeight;
            }
        }

        private void DrawInventory(CustomSpriteBatch g, int X)
        {
            float Ratio = Constants.Height / 2160f;
            Color TextColor = Color.FromNonPremultiplied(65, 70, 65, 255);
            int DrawY = (int)(648 * Ratio - InventoryScrollbarValue % BoxHeight);

            int StartIndex = (int)Math.Floor(InventoryScrollbarValue / (double)BoxHeight);
            int TotalItem = CurrentContainer.ListUnit.Count;

            DrawFolders(g, X);
            int XPos = 0;
            for (int YPos = StartIndex; YPos < TotalItem; YPos += BoxPerLine)
            {
                while (XPos < BoxPerLine && XPos + YPos < TotalItem)
                {
                    int FinalX = (int)(X + XPos * BoxWithOffsetFinal);
                    int CurrentIndex = XPos + YPos;

                    g.Draw(sprPixel, new Rectangle(FinalX, DrawY, BoxWidthFinal, BoxHeight), Color.FromNonPremultiplied(16, 16, 16, 255));
                    g.Draw(sprPixel, new Rectangle(FinalX + 5, DrawY + 5, BoxWidthFinal - 10, 80), TextColor);
                    g.Draw(sprPixel, new Rectangle(FinalX + 5, DrawY + BoxHeight - (int)(66 * Ratio), BoxWidthFinal - 10, (int)(56 * Ratio)), Color.FromNonPremultiplied(243, 243, 243, 255));
                    if (ShowCheckbox)
                    {
                        DrawBox(g, new Vector2(FinalX + 6, DrawY + BoxHeight - 20), 15, 15, Color.White);
                    }

                    if (CurrentIndex < CurrentContainer.ListUnit.Count)
                    {
                        g.Draw(CurrentContainer.ListUnit[CurrentIndex].Leader.SpriteMap, new Rectangle(FinalX + SpriteOffset, DrawY + SpriteOffset, SpriteSizeFinal, SpriteSizeFinal), Color.White);

                        g.DrawString(fntOxanimumBoldSmall, "x" + CurrentContainer.ListUnit[CurrentIndex].QuantityOwned, new Vector2(FinalX + BoxWidthFinal - 40, DrawY + BoxHeight - 30), TextColor);
                    }

                    ++XPos;
                }
                XPos = 0;

                DrawY += (int)(276 * Ratio);
            }
            DrawSelection(g, X);
        }

        private void DrawSelection(CustomSpriteBatch g, int X)
        {
            float Ratio = Constants.Height / 2160f;
            Color TextColor = Color.FromNonPremultiplied(65, 70, 65, 255);

            int DrawY = (int)(648 * Ratio - InventoryScrollbarValue % BoxHeight);

            int SelectedItemIndex = GetOwnedSquadUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);

            //Hover
            if (SelectedItemIndex >= 0)
            {
                int FinalX = (int)(X + ((MouseHelper.MouseStateCurrent.X - X) / BoxWithOffsetFinal) * BoxWithOffsetFinal);
                int FinalY = DrawY + ((MouseHelper.MouseStateCurrent.Y - DrawY) / BoxLineHeight) * BoxLineHeight;

                UnitInfo SelectedEquipment = CurrentContainer.ListUnit[SelectedItemIndex];

                LastSelectedEquipment = SelectedEquipment;

                g.Draw(sprPixel, new Rectangle(FinalX, FinalY, BoxWidthFinal, BoxHeight), Color.FromNonPremultiplied(255, 255, 255, 127));

                DrawBox(g, new Vector2(FinalX - BoxOffsetFinal / 2, FinalY + BoxHeight), BoxWithOffsetFinal, (int)(50 * Ratio), Color.Black);
                g.DrawStringMiddleAligned(fntArial12,
                    SelectedEquipment.Leader.ItemName,
                    new Vector2(FinalX + BoxWidthFinal / 2, FinalY + BoxHeight + (int)(6 * Ratio)), Color.White);
            }
            else
            {
                int SelectedFolderIndex = GetFolderUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);

                if (SelectedFolderIndex >= 0)
                {
                    DrawY = (int)(444 * Ratio) - InventoryScrollbarValue % BoxHeight;

                    int FinalX = (int)(X + ((MouseHelper.MouseStateCurrent.X - X) / FolderWithOffsetFinal) * FolderWithOffsetFinal);
                    int FinalY = DrawY + ((MouseHelper.MouseStateCurrent.Y - DrawY) / BoxLineHeight) * BoxLineHeight;

                    g.Draw(sprPixel, new Rectangle(FinalX, FinalY, FolderWidthFinal, FolderBoxHeightFinal), Color.FromNonPremultiplied(255, 255, 255, 127));
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
