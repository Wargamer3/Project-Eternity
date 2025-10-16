using System;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class InventoryCharacterWhiteScreen : GameScreen
    {
        private Scrollbar LoadoutListScrollbar;
        private Scrollbar InventoryScrollbar;

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
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            fntOxanimumLight = Content.Load<SpriteFont>("Fonts/Oxanium Light Big");
            fntOxanimumLightSmall = Content.Load<SpriteFont>("Fonts/Oxanium Light");
            fntOxanimumRegularSmall = Content.Load<SpriteFont>("Fonts/Oxanium Regular Small");
            fntOxanimumBoldSmall = Content.Load<SpriteFont>("Fonts/Oxanium Bold Small");
            fntOxanimumBoldSmaller = Content.Load<SpriteFont>("Fonts/Oxanium Bold Smaller");
            fntOxanimumBoldBig = Content.Load<SpriteFont>("Fonts/Oxanium Bold Big");

            sprLoadouts = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Frame Outline");
            sprLoadoutsFrame = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Inventory/Frame Loadout");
            sprLoadoutsName = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Inventory/Frame Loadout Name");
            sprFrameDescription = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Inventory/Frame Description");

            sprButtonFolderInactive = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Inventory/Folder Button Inactive");
            sprButtonRename = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Inventory/Button Rename");

            sprScrollbarBackground = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Scrollbar Background");
            sprScrollbar = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Scrollbar Bar");

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            float Ratio = Constants.Height / 2160f;

            InventoryScrollbar = new Scrollbar(sprScrollbar, new Vector2(2144 * Ratio, 648 * Ratio), Ratio, (int)(sprScrollbarBackground.Height * Ratio), 10, OnInventoryScrollbarChange);
            LoadoutListScrollbar = new Scrollbar(sprScrollbar, new Vector2(2144 * Ratio, 648 * Ratio), Ratio, (int)(sprScrollbarBackground.Height * Ratio), 10, OnCharacterScrollbarChange);

            InventoryScrollbar.ChangeMaxValue(CurrentContainer.ListCharacter.Count * InventorySquadWhiteScreen.BoxHeight - BattleMapInventoryWhiteScreen.MiddleSectionHeight);
            LoadoutListScrollbar.ChangeMaxValue(Owner.ActivePlayer.Inventory.ListSquadLoadout.Count * InventorySquadWhiteScreen.LoadoutEntryHeightWithOffset - BattleMapInventoryWhiteScreen.MiddleSectionHeight);
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
                int SelectedItemIndex = GetOwnedCharacterUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);

                if (ListLastContainer.Count > 0)
                {
                    --SelectedItemIndex;
                }

                if (SelectedItemIndex >= 0)
                {
                    StartDragDrop(CurrentContainer.ListCharacter[SelectedItemIndex]);
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
                int SelectedItemIndex = GetOwnedCharacterUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);

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
                    //PushScreen(new InventoryCharacterInformationScreen(Owner, CurrentContainer.ListCharacter[SelectedItemIndex - CurrentContainer.DicFolder.Count].Leader));
                }
            }
        }

        private void StartDragDrop(CharacterInfo EquipmentToDrag)
        {
            DragAndDropEquipment = EquipmentToDrag;
        }

        private void DoDragDrop()
        {
            float Ratio = Constants.Height / 2160f;
            int DrawX = InventorySquadWhiteScreen.LoadoutX;
            int DrawY = (int)(462 * Ratio - CharacterScrollbarValue % InventorySquadWhiteScreen.LoadoutEntryHeightWithOffset);

            if (InputHelper.InputConfirmReleased())
            {
                int MouseX = MouseHelper.MouseStateCurrent.X;
                int MouseY = MouseHelper.MouseStateCurrent.Y;
                int MouseXFinal = (int)((MouseX - DrawX) % InventorySquadWhiteScreen.BoxWithOffsetFinal);
                int MouseYFinal = (MouseY - DrawY) % InventorySquadWhiteScreen.BoxLineHeight;

                int SquadSlotIndex = (MouseHelper.MouseStateCurrent.X - DrawX) / InventorySquadWhiteScreen.LoadoutBoxWidthWithOffset;
                int SqaudLoatoutIndex = (MouseHelper.MouseStateCurrent.Y - DrawY) / InventorySquadWhiteScreen.LoadoutEntryHeightWithOffset;

                if (SquadSlotIndex >= 0 && SquadSlotIndex < LoadoutSize && SqaudLoatoutIndex < Owner.ActivePlayer.Inventory.ListSquadLoadout.Count
                    && MouseXFinal >= 0 && MouseXFinal < InventorySquadWhiteScreen.LoadoutBoxSize
                    && MouseYFinal >= InventorySquadWhiteScreen.LoadoutBoxHeightWithOffset && MouseYFinal < InventorySquadWhiteScreen.LoadoutBoxHeightWithOffset + InventorySquadWhiteScreen.LoadoutBoxSize)
                {
                    Owner.ActivePlayer.Inventory.ListSquadLoadout[SqaudLoatoutIndex].ListSpawnSquad[SquadSlotIndex].CurrentLeader.ArrayCharacterActive[0] = DragAndDropEquipment.Pilot;
                    Owner.ActivePlayer.SaveLocally();
                }

                DragAndDropEquipment = null;
            }
        }

        private int GetOwnedCharacterUnderMouse(int MouseX, int MouseY)
        {
            float Ratio = Constants.Height / 2160f;
            int X = (int)(140 * Ratio);

            int DrawY = (int)(648 * Ratio - InventoryScrollbarValue % InventorySquadWhiteScreen.BoxHeight);

            int MouseIndex = (int)((MouseX - X) / InventorySquadWhiteScreen.BoxWithOffsetFinal + ((MouseY - DrawY) / InventorySquadWhiteScreen.BoxLineHeight) * InventorySquadWhiteScreen.BoxPerLine);
            int MouseXFinal = (int)((MouseX - X) % InventorySquadWhiteScreen.BoxWithOffsetFinal);
            int MouseYFinal = (MouseY - DrawY) % InventorySquadWhiteScreen.BoxLineHeight;

            int ItemCount = CurrentContainer.ListCharacter.Count;

            if (MouseIndex >= 0 && MouseIndex < ItemCount
                && MouseXFinal > 0 && MouseXFinal < InventorySquadWhiteScreen.BoxWidthFinal && MouseX < InventorySquadWhiteScreen.InventoryWidth
                && MouseYFinal >= 0 && MouseYFinal < InventorySquadWhiteScreen.BoxLineHeight)
            {
                return MouseIndex;
            }

            return -1;
        }

        private int GetFolderUnderMouse(int MouseX, int MouseY)
        {
            float Ratio = Constants.Height / 2160f;

            int X = (int)(140 * Ratio);
            int DrawY = (int)(444 * Ratio) - InventoryScrollbarValue % InventorySquadWhiteScreen.BoxHeight;

            int MouseIndex = (int)((MouseX - X) / InventorySquadWhiteScreen.FolderWithOffsetFinal + ((MouseY - DrawY) / InventorySquadWhiteScreen.BoxLineHeight) * InventorySquadWhiteScreen.BoxPerLine);
            int MouseXFinal = (int)((MouseX - X) % InventorySquadWhiteScreen.FolderWithOffsetFinal);
            int MouseYFinal = (MouseY - DrawY) % InventorySquadWhiteScreen.BoxLineHeight;

            int ItemCount = CurrentContainer.ListFolder.Count;
            if (ListLastContainer.Count > 0)
            {
                ItemCount += 1;
            }

            if (MouseIndex >= 0 && MouseIndex < ItemCount
                && MouseXFinal > 0 && MouseXFinal < InventorySquadWhiteScreen.FolderWidthFinal && MouseXFinal < InventorySquadWhiteScreen.InventoryWidth
                && MouseYFinal >= 4 && MouseYFinal < InventorySquadWhiteScreen.FolderBoxHeightFinal)
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
            float Ratio = Constants.Height / 2160f;

            g.Draw(sprScrollbarBackground, new Vector2(2144 * Ratio, 648 * Ratio), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
            InventoryScrollbar.Draw(g);
            LoadoutListScrollbar.Draw(g);

            DrawLoadout(g);

            DrawInventory(g, InventorySquadWhiteScreen.InventoryX);

            if (DragAndDropEquipment != null)
            {
                g.Draw(DragAndDropEquipment.Pilot.sprPortrait, new Rectangle(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y, InventorySquadWhiteScreen.LoadoutBoxSize, InventorySquadWhiteScreen.LoadoutBoxSize), Color.White);
            }
        }

        private void DrawLoadout(CustomSpriteBatch g)
        {
            float Ratio = Constants.Height / 2160f;
            Color TextColor = Color.FromNonPremultiplied(65, 70, 65, 255);
            g.Draw(sprLoadouts, new Vector2(InventorySquadWhiteScreen.LoadoutX - 80 * Ratio, 260 * Ratio), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
            g.Draw(sprLoadoutsFrame, new Vector2(InventorySquadWhiteScreen.LoadoutX - 30 * Ratio, 416 * Ratio), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
            g.DrawString(fntOxanimumBoldBig, "Loadouts", new Vector2((BattleMapInventoryWhiteScreen.LoadoutX) * Ratio, 294 * Ratio), TextColor);

            int DrawY = (int)(462 * Ratio - CharacterScrollbarValue % InventorySquadWhiteScreen.LoadoutEntryHeightWithOffset);

            for (int L = (int)Math.Floor(CharacterScrollbarValue / (double)InventorySquadWhiteScreen.LoadoutEntryHeightWithOffset); L < 5; ++L)
            {
                int DrawX = InventorySquadWhiteScreen.LoadoutX;

                g.Draw(sprLoadoutsName, new Vector2((int)(DrawX + 420 * Ratio), DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
                g.Draw(sprButtonRename, new Vector2((int)(DrawX + 1074 * Ratio), DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
                g.DrawString(fntOxanimumRegularSmall, "Rename", new Vector2((int)(DrawX + 1114 * Ratio), (int)((DrawY + 16 * Ratio))), TextColor);
                g.DrawString(fntOxanimumRegularSmall, "Loadout " + (L + 1), new Vector2((int)(DrawX + 466 * Ratio), (int)((DrawY + 16 * Ratio))), TextColor);

                for (int S = 0; S < 5; S++)
                {
                    g.Draw(sprPixel, new Rectangle(DrawX, (int)(DrawY), InventorySquadWhiteScreen.LoadoutBoxSize, InventorySquadWhiteScreen.LoadoutBoxSize), Color.FromNonPremultiplied(10, 10, 10, 255));
                    g.Draw(sprPixel, new Rectangle(DrawX, (int)((DrawY + InventorySquadWhiteScreen.LoadoutBoxHeightWithOffset)), InventorySquadWhiteScreen.LoadoutBoxSize, InventorySquadWhiteScreen.LoadoutBoxSize), Color.FromNonPremultiplied(10, 10, 10, 255));

                    if (L < Owner.ActivePlayer.Inventory.ListSquadLoadout.Count && S < Owner.ActivePlayer.Inventory.ListSquadLoadout[L].ListSpawnSquad.Count)
                    {
                        Core.Units.Squad ActiveSquad = Owner.ActivePlayer.Inventory.ListSquadLoadout[L].ListSpawnSquad[S];

                        if (ActiveSquad != null)
                        {
                            g.Draw(ActiveSquad.CurrentLeader.SpriteMap, new Rectangle(DrawX, (int)(DrawY), InventorySquadWhiteScreen.LoadoutBoxSize, InventorySquadWhiteScreen.LoadoutBoxSize), Color.White);

                            if (ActiveSquad.CurrentLeader.Pilot != null)
                            {
                                g.Draw(ActiveSquad.CurrentLeader.Pilot.sprPortrait, new Rectangle(DrawX, DrawY + InventorySquadWhiteScreen.LoadoutBoxHeightWithOffset, InventorySquadWhiteScreen.LoadoutBoxSize, InventorySquadWhiteScreen.LoadoutBoxSize), Color.White);
                            }
                        }
                    }

                    if (DragAndDropEquipment != null)
                    {
                        g.Draw(sprPixel, new Rectangle(DrawX, DrawY + InventorySquadWhiteScreen.LoadoutBoxHeightWithOffset, InventorySquadWhiteScreen.LoadoutBoxSize, InventorySquadWhiteScreen.LoadoutBoxSize), Color.FromNonPremultiplied(255, 255, 255, 127));
                    }
                    if (MouseHelper.MouseStateCurrent.X >= DrawX && MouseHelper.MouseStateCurrent.X < DrawX + InventorySquadWhiteScreen.LoadoutBoxSize
                        && MouseHelper.MouseStateCurrent.Y >= DrawY + InventorySquadWhiteScreen.LoadoutBoxHeightWithOffset && MouseHelper.MouseStateCurrent.Y < DrawY + InventorySquadWhiteScreen.LoadoutBoxHeightWithOffset + InventorySquadWhiteScreen.LoadoutBoxSize)
                    {
                        g.Draw(sprPixel, new Rectangle(DrawX, DrawY + InventorySquadWhiteScreen.LoadoutBoxHeightWithOffset, InventorySquadWhiteScreen.LoadoutBoxSize, InventorySquadWhiteScreen.LoadoutBoxSize), Color.FromNonPremultiplied(255, 255, 255, 127));
                    }

                    DrawX += InventorySquadWhiteScreen.LoadoutBoxWidthWithOffset;
                }

                DrawY += InventorySquadWhiteScreen.LoadoutEntryHeightWithOffset;
            }
        }

        private void DrawFolders(CustomSpriteBatch g, int X)
        {
            Color TextColor = Color.FromNonPremultiplied(65, 70, 65, 255);
            float Ratio = Constants.Height / 2160f;
            int DrawY = (int)(444 * Ratio) - InventoryScrollbarValue % InventorySquadWhiteScreen.BoxHeight;

            int StartIndex = (int)Math.Floor(InventoryScrollbarValue / (double)InventorySquadWhiteScreen.FolderBoxHeight);
            int TotalItem = CurrentContainer.ListFolder.Count;

            if (ListLastContainer.Count > 0)
            {
                TotalItem += 1;
            }

            int XPos = 0;

            if (ListLastContainer.Count > 0)
            {
                int FinalX = X + XPos * InventorySquadWhiteScreen.FolderWithOffsetFinal;

                g.Draw(sprButtonFolderInactive, new Vector2(FinalX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
                g.DrawStringCentered(fntOxanimumLight, "Last Folder", new Vector2(FinalX + InventorySquadWhiteScreen.FolderBoxWidth * Ratio / 2, DrawY + InventorySquadWhiteScreen.FolderBoxHeightFinal / 2 + 6 * Ratio), TextColor);

                ++XPos;
            }

            for (int YPos = StartIndex; YPos < TotalItem; YPos += InventorySquadWhiteScreen.FolderPerLine)
            {
                while (XPos < InventorySquadWhiteScreen.FolderPerLine && XPos + YPos < TotalItem)
                {
                    int FinalX = X + XPos * InventorySquadWhiteScreen.FolderWithOffsetFinal;
                    int CurrentIndex = XPos + YPos;

                    if (ListLastContainer.Count > 0)
                    {
                        CurrentIndex -= 1;
                    }

                    g.Draw(sprButtonFolderInactive, new Vector2(FinalX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
                    g.DrawStringCentered(fntOxanimumLight, CurrentContainer.ListFolder[CurrentIndex].Name, new Vector2(FinalX + InventorySquadWhiteScreen.FolderBoxWidth * Ratio / 2, DrawY + InventorySquadWhiteScreen.FolderBoxHeightFinal / 2 + 6 * Ratio), TextColor);

                    ++XPos;
                }

                XPos = 0;

                DrawY += InventorySquadWhiteScreen.BoxLineHeight;
            }
        }

        private void DrawInventory(CustomSpriteBatch g, int X)
        {
            float Ratio = Constants.Height / 2160f;
            Color TextColor = Color.FromNonPremultiplied(65, 70, 65, 255);
            int DrawY = (int)(648 * Ratio - InventoryScrollbarValue % InventorySquadWhiteScreen.BoxHeight);

            int StartIndex = (int)Math.Floor(InventoryScrollbarValue / (double)InventorySquadWhiteScreen.BoxHeight);
            int TotalItem = CurrentContainer.ListCharacter.Count;

            DrawFolders(g, X);
            int XPos = 0;
            for (int YPos = StartIndex; YPos < TotalItem; YPos += InventorySquadWhiteScreen.BoxPerLine)
            {
                while (XPos < InventorySquadWhiteScreen.BoxPerLine && XPos + YPos < TotalItem)
                {
                    int FinalX = (int)(X + XPos * InventorySquadWhiteScreen.BoxWithOffsetFinal);
                    int CurrentIndex = XPos + YPos;

                    g.Draw(sprPixel, new Rectangle(FinalX, DrawY, InventorySquadWhiteScreen.BoxWidthFinal, InventorySquadWhiteScreen.BoxHeight), Color.FromNonPremultiplied(16, 16, 16, 255));
                    g.Draw(sprPixel, new Rectangle(FinalX + 5, DrawY + 5, InventorySquadWhiteScreen.BoxWidthFinal - 10, 80), TextColor);
                    g.Draw(sprPixel, new Rectangle(FinalX + 5, DrawY + InventorySquadWhiteScreen.BoxHeight - (int)(66 * Ratio), InventorySquadWhiteScreen.BoxWidthFinal - 10, (int)(56 * Ratio)), Color.FromNonPremultiplied(243, 243, 243, 255));
                    if (ShowCheckbox)
                    {
                        DrawBox(g, new Vector2(FinalX + 6, DrawY + InventorySquadWhiteScreen.BoxHeight - 20), 15, 15, Color.White);
                    }

                    if (CurrentIndex < CurrentContainer.ListCharacter.Count)
                    {
                        g.Draw(CurrentContainer.ListCharacter[CurrentIndex].Pilot.sprPortrait, new Rectangle(FinalX + InventorySquadWhiteScreen.SpriteOffset, DrawY + InventorySquadWhiteScreen.SpriteOffset, InventorySquadWhiteScreen.SpriteSizeFinal, InventorySquadWhiteScreen.SpriteSizeFinal), Color.White);

                        g.DrawString(fntOxanimumBoldSmall, "x" + CurrentContainer.ListCharacter[CurrentIndex].QuantityOwned, new Vector2(FinalX + InventorySquadWhiteScreen.BoxWidthFinal - 40, DrawY + InventorySquadWhiteScreen.BoxHeight - 30), TextColor);
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

            int DrawY = (int)(648 * Ratio - InventoryScrollbarValue % InventorySquadWhiteScreen.BoxHeight);

            int SelectedItemIndex = GetOwnedCharacterUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);

            //Hover
            if (SelectedItemIndex >= 0)
            {
                int FinalX = (int)(X + ((MouseHelper.MouseStateCurrent.X - X) / InventorySquadWhiteScreen.BoxWithOffsetFinal) * InventorySquadWhiteScreen.BoxWithOffsetFinal);
                int FinalY = DrawY + ((MouseHelper.MouseStateCurrent.Y - DrawY) / InventorySquadWhiteScreen.BoxLineHeight) * InventorySquadWhiteScreen.BoxLineHeight;

                CharacterInfo SelectedEquipment = CurrentContainer.ListCharacter[SelectedItemIndex];

                LastSelectedEquipment = SelectedEquipment;

                g.Draw(sprPixel, new Rectangle(FinalX, FinalY, InventorySquadWhiteScreen.BoxWidthFinal, InventorySquadWhiteScreen.BoxHeight), Color.FromNonPremultiplied(255, 255, 255, 127));

                DrawBox(g, new Vector2(FinalX - InventorySquadWhiteScreen.BoxOffsetFinal / 2, FinalY + InventorySquadWhiteScreen.BoxHeight), InventorySquadWhiteScreen.BoxWithOffsetFinal, (int)(50 * Ratio), Color.Black);
                g.DrawStringMiddleAligned(fntArial12,
                    SelectedEquipment.Pilot.Name,
                    new Vector2(FinalX + InventorySquadWhiteScreen.BoxWidthFinal / 2, FinalY + InventorySquadWhiteScreen.BoxHeight + (int)(6 * Ratio)), Color.White);
            }
            else
            {
                int SelectedFolderIndex = GetFolderUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);

                if (SelectedFolderIndex >= 0)
                {
                    DrawY = (int)(444 * Ratio) - InventoryScrollbarValue % InventorySquadWhiteScreen.BoxHeight;

                    int FinalX = (int)(X + ((MouseHelper.MouseStateCurrent.X - X) / InventorySquadWhiteScreen.FolderWithOffsetFinal) * InventorySquadWhiteScreen.FolderWithOffsetFinal);
                    int FinalY = DrawY + ((MouseHelper.MouseStateCurrent.Y - DrawY) / InventorySquadWhiteScreen.BoxLineHeight) * InventorySquadWhiteScreen.BoxLineHeight;

                    g.Draw(sprPixel, new Rectangle(FinalX, FinalY, InventorySquadWhiteScreen.FolderWidthFinal, InventorySquadWhiteScreen.FolderBoxHeightFinal), Color.FromNonPremultiplied(255, 255, 255, 127));
                }
            }

            g.Draw(sprFrameDescription, new Vector2(X * Ratio, 1938 * Ratio), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
            if (LastSelectedEquipment != null)
            {
                int SpriteSize = 128;
                int SpriteBoxSize = SpriteSize + 22;
                g.Draw(sprPixel, new Rectangle((int)((X + 32) * Ratio), (int)(1962 * Ratio), (int)(SpriteBoxSize * Ratio), (int)(SpriteBoxSize * Ratio)), TextColor);
                DrawBox(g, new Vector2((int)((X + 32) * Ratio), (int)(1962 * Ratio)), (int)(SpriteBoxSize * Ratio), (int)(SpriteBoxSize * Ratio), Color.Transparent);
                g.Draw(LastSelectedEquipment.Pilot.sprPortrait, new Rectangle((int)((X + 40) * Ratio), (int)(1976 * Ratio), (int)(SpriteSize * Ratio), (int)(SpriteSize * Ratio)), Color.White);

                g.DrawString(fntOxanimumBoldSmaller, LastSelectedEquipment.Pilot.Name, new Vector2((X + 240) * Ratio, 1976 * Ratio), TextColor);

                g.DrawString(fntOxanimumLightSmall, "MEL: " + LastSelectedEquipment.Pilot.MEL, new Vector2((X + 836) * Ratio, 1976 * Ratio), TextColor);

                g.DrawString(fntOxanimumLightSmall, "RNG: " + LastSelectedEquipment.Pilot.RNG, new Vector2((X + 836) * Ratio, 2058 * Ratio), TextColor);

                g.DrawString(fntOxanimumLightSmall, "DEF: " + LastSelectedEquipment.Pilot.DEF, new Vector2((X + 1412) * Ratio, 1976 * Ratio), TextColor);
            }
        }
    }
}
