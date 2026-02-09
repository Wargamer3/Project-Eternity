using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.UI;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;
using FMOD;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class CharacterSelectionScreen : GameScreen
    {
        #region Ressources

        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private CardSymbols Symbols;

        private Texture2D sprSelectAUnitToBuy;

        private SpriteFont fntOxanimumBold;
        private SpriteFont fntOxanimumBoldTitle;
        private SpriteFont fntMenuText;

        private TextButton ReturnToLobbyButton;
        private EmptyBoxScrollbar InventoryScrollbar;
        private DropDownButton PlayerControlDropDown;

        private CubeBackgroundSmall CubeBackground;

        private IUIElement[] ArrayUIElement;

        #endregion

        private int CharacterMenuX = 70;
        private int CharacterMenuY = 150;
        private int BoxWidth = 160;
        private int BoxHeight = 140;
        private int SpriteBoxWidth = 148;
        private int LineHeight = 155;
        private int BoxPerLine;

        private int RightSectionCenterX = 1500;
        private int SkinSpriteWidth = 75;
        private int SkinSpriteHeight = 64;
        private int SkinBoxWidth = 90;
        private int SkinBoxHeight = 74;
        private int SkinSectionY = 800;

        private Player ActivePlayer;
        private readonly bool CanChangePlayer;

        private int SelectionIndex;
        private int InventoryScrollbarValue;

        private CharacterInventoryContainer CurrentContainer;
        private List<CharacterInventoryContainer> ListLastContainer;

        public CharacterSelectionScreen(CardSymbols Symbols, Player ActivePlayer, bool CanChangePlayer)
        {
            this.Symbols = Symbols;
            this.CanChangePlayer = CanChangePlayer;

            ListLastContainer = new List<CharacterInventoryContainer>();
            SetPlayer(ActivePlayer);

            CubeBackground = new CubeBackgroundSmall();
        }

        public override void Load()
        {
            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            float Ratio = Constants.Height / 2160f;
            CharacterMenuX = (int)(70 * Ratio);
            BoxPerLine = (Constants.Width - Constants.Width / 3 - CharacterMenuX) / BoxWidth;

            int DrawX = (int)(Constants.Width - 502 * Ratio);
            int DrawY = (int)(100 * Ratio);
            ReturnToLobbyButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Big}{Centered}{Color:65,70,65,255}Return To Lobby}}", "Deathmatch/Lobby Menu/Interactive/Button Back To Lobby", new Vector2(DrawX, DrawY), 4, 1, Ratio, OnButtonOver, SelectBackToLobbyButton);

            InventoryScrollbar = new EmptyBoxScrollbar(new Vector2(Constants.Width - 23, BattleMapInventoryScreen.MiddleSectionY + 3), BattleMapInventoryScreen.MiddleSectionHeight - 5, 10, OnInventoryScrollbarChange);
            InventoryScrollbar.ChangeMaxValue(CurrentContainer.ListCharacter.Count * BoxHeight - BattleMapInventoryScreen.MiddleSectionHeight);

            string[] ArrayPlayerName = new string[PlayerManager.ListLocalPlayer.Count];
            for (int P = 0; P < PlayerManager.ListLocalPlayer.Count; P++)
            {
                ArrayPlayerName[P] = "{{Text:{Font:Oxanium Light Bigger}{Centered}{Color:243, 243, 243, 255}" + PlayerManager.ListLocalPlayer[P].Name + "}}";
            }

            if (CanChangePlayer)
            {
                PlayerControlDropDown = new DropDownButton(Content, "{{Text:{Font:Oxanium Light Bigger}{Centered}{Color:243, 243, 243, 255}" + ActivePlayer.Name + "}}",
                    ArrayPlayerName,
                    "Deathmatch/Lobby Menu/Interactive/Button Grey",
                    new Vector2((int)(2400 * Ratio), (int)(120 * Ratio)), 4, 1, Ratio, OnButtonOver, (SelectedIndex, SelectedItem) => { OnPlayerControlChange(SelectedIndex, SelectedItem); });

                ArrayUIElement = new IUIElement[]
                {
                    PlayerControlDropDown, ReturnToLobbyButton,
                };
            }
            else
            {
                ArrayUIElement = new IUIElement[]
                {
                    ReturnToLobbyButton,
                };
            }

            fntMenuText = Content.Load<SpriteFont>("Fonts/Arial12");
            fntOxanimumBold = Content.Load<SpriteFont>("Fonts/Oxanium Bold");
            fntOxanimumBoldTitle = Content.Load<SpriteFont>("Fonts/Oxanium Bold Title");

            sprSelectAUnitToBuy = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Frame Outline");

            CubeBackground.Load(Content);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (IUIElement ActiveButton in ArrayUIElement)
            {
                ActiveButton.Update(gameTime);
            }

            UpdateCharacterSelection();
            UpdateCharacterSkinSelection();

            if (KeyboardHelper.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                RemoveScreen(this);
            }
        }

        private void SetPlayer(Player ActivePlayer)
        {
            this.ActivePlayer = ActivePlayer;
            CurrentContainer = ActivePlayer.Inventory.RootCharacterContainer;
            ListLastContainer.Clear();

            int ActiveIndex = CurrentContainer.ListCharacter.IndexOf(ActivePlayer.Inventory.Character);
            if (ActiveIndex >= 0)
            {
                this.SelectionIndex = ActiveIndex;
            }
        }

        private void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        private void SelectBackToLobbyButton()
        {
            sndButtonClick.Play();
            RemoveScreen(this);
        }

        private void OnPlayerControlChange(int SelectedIndex, string _)
        {
            sndButtonClick.Play();
            if (SelectedIndex >= 0)
            {
                SetPlayer((Player)PlayerManager.ListLocalPlayer[SelectedIndex]);
            }
        }

        private void UpdateCharacterSelection()
        {
            if (MouseHelper.InputLeftButtonPressed())
            {
                int SelectedCharacterIndex = GetCharacterUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);

                if (SelectedCharacterIndex < 0)
                {
                    return;
                }

                if (ListLastContainer.Count > 0)
                {
                    --SelectedCharacterIndex;
                }

                if (SelectedCharacterIndex == -1)
                {
                    CurrentContainer = ListLastContainer[ListLastContainer.Count - 1];
                    ListLastContainer.Remove(CurrentContainer);
                }
                else if (SelectedCharacterIndex >= CurrentContainer.DicFolder.Count)
                {
                    this.SelectionIndex = SelectedCharacterIndex - CurrentContainer.DicFolder.Count;
                    ActivePlayer.Inventory.Character = CurrentContainer.ListCharacter[SelectedCharacterIndex - CurrentContainer.ListFolder.Count];
                }
                else
                {
                    ListLastContainer.Add(CurrentContainer);
                    if (ListLastContainer.Count > 1)
                    {
                        CurrentContainer = CurrentContainer.ListFolder[SelectedCharacterIndex];
                    }
                    else
                    {
                        CurrentContainer = CurrentContainer.ListFolder[SelectedCharacterIndex];
                    }
                }
            }
        }

        private void UpdateCharacterSkinSelection()
        {
            if (MouseHelper.InputLeftButtonPressed())
            {
                int SelectedSkinIndex = GetCharacterSkinUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);

                if (SelectedSkinIndex < 0)
                {
                    return;
                }

                ActivePlayer.Inventory.ActiveSkinIndex = (byte)SelectedSkinIndex;
            }
        }

        private int GetCharacterUnderMouse(int MouseX, int MouseY)
        {
            int DrawY = CharacterMenuY - InventoryScrollbarValue % BoxHeight;
            int SpriteOffset = BoxWidth / 2 - SpriteBoxWidth / 2;

            int MouseIndex = (MouseX - CharacterMenuX) / BoxWidth + ((MouseY - DrawY) / LineHeight) * BoxPerLine;
            int MouseXFinal = (MouseX - CharacterMenuX) % BoxWidth;
            int MouseYFinal = (MouseY - DrawY) % LineHeight;

            int ItemCount = CurrentContainer.ListCharacter.Count + CurrentContainer.ListFolder.Count;
            if (ListLastContainer.Count > 0)
            {
                ItemCount += 1;
            }

            if (MouseIndex >= 0 && MouseIndex < ItemCount
                && MouseXFinal >= SpriteOffset && MouseXFinal < SpriteOffset + BoxWidth
                && MouseX >= CharacterMenuX && MouseX < CharacterMenuX + BoxPerLine * BoxWidth
                && MouseYFinal >= 4 && MouseYFinal < 4 + BoxHeight)
            {
                if (MouseIndex >= CurrentContainer.ListFolder.Count)
                {
                    foreach (PlayerCharacterSkin ActiveSkin in CurrentContainer.ListCharacter[MouseIndex - CurrentContainer.ListFolder.Count].ListOwnedUnitSkin)
                    {
                        if (ActiveSkin.CharacterSkin == null)
                        {
                            ActiveSkin.CharacterSkin = new PlayerCharacter(ActiveSkin.SkinRelativePath, GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
                        }
                    }
                }

                return MouseIndex;
            }

            return -1;
        }

        private int GetCharacterSkinUnderMouse(int MouseX, int MouseY)
        {
            int X = RightSectionCenterX;
            int DrawY = SkinSectionY;
            int SpriteOffset = 5;

            int BoxPerLine = 5;

            int SkinCount = ActivePlayer.Inventory.Character.ListOwnedUnitSkin.Count;
            int TotalSkinSize = SkinCount * (SkinBoxWidth + 5);

            X -= (TotalSkinSize - SkinBoxWidth - 5 + SkinBoxWidth) / 2;

            int MouseIndex = (MouseX - X) / SkinBoxWidth + ((MouseY - DrawY) / LineHeight) * BoxPerLine;
            int MouseXFinal = (MouseX - X) % SkinBoxWidth;
            int MouseYFinal = (MouseY - DrawY) % LineHeight;

            if (MouseIndex >= 0 && MouseIndex < SkinCount
                && MouseXFinal >= SpriteOffset && MouseXFinal < SpriteOffset + SkinBoxWidth
                && MouseX >= X && MouseX < X + BoxPerLine * SkinBoxWidth
                && MouseYFinal >= 4 && MouseYFinal < 4 + SkinBoxHeight)
            {
                return MouseIndex;
            }

            return -1;
        }

        private void OnInventoryScrollbarChange(float ScrollbarValue)
        {
            InventoryScrollbarValue = (int)ScrollbarValue;
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            CubeBackground.BeginDraw(g);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            CubeBackground.Draw(g, true);

            bool ShowCheckbox = false;

            int X = -10;
            int Y = Constants.Height / 20;

            Color TextColor = Color.FromNonPremultiplied(65, 70, 65, 255);
            float Ratio = Constants.Height / 2160f;
            X = Constants.Width / 18;
            g.DrawString(fntOxanimumBoldTitle, "CHARACTER", new Vector2((int)(210 * Ratio), (int)(58 * Ratio)), TextColor);
            g.DrawStringMiddleAligned(fntMenuText, ActivePlayer.Name, new Vector2(Constants.Width / 2, Y), Color.White);

            X = CharacterMenuX;
            Y = CharacterMenuY;

            int DrawY = CharacterMenuY;
            int SpriteOffset = BoxWidth / 2 - SpriteBoxWidth / 2;

            int StartIndex = (int)Math.Floor(InventoryScrollbarValue / (double)BoxHeight);
            int BoxPerLine = (Constants.Width - Constants.Width / 3 - X) / BoxWidth;

            int TotalItem = CurrentContainer.ListCharacter.Count + CurrentContainer.ListFolder.Count;
            TotalItem = 35;

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
                g.DrawStringCenteredBackground(fntMenuText, "Last Folder",
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
                        g.DrawStringCenteredBackground(fntMenuText, CurrentContainer.ListFolder[CurrentIndex].Name,
                            new Vector2(FinalX + BoxWidth / 2, DrawY + BoxHeight / 2), Color.White, sprPixel, Lobby.BackgroundColor);
                    }
                    else
                    {
                        CurrentIndex -= CurrentContainer.ListFolder.Count;

                        if (ShowCheckbox)
                        {
                            DrawBox(g, new Vector2(FinalX + 6, DrawY + BoxHeight - 20), 15, 15, Color.White);
                        }

                        g.Draw(CurrentContainer.ListCharacter[CurrentIndex].Character.SpriteShop, new Vector2(FinalX + SpriteOffset + 3, DrawY + 7), Color.White);
                    }

                    ++XPos;
                }
                XPos = 0;

                DrawY += LineHeight;
            }

            DrawY = CharacterMenuY;

            int SelectedCharacterIndex = GetCharacterUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);

            if (ListLastContainer.Count > 0)
            {
                SelectedCharacterIndex -= 1;
            }

            PlayerCharacterInfo CharacterToDraw = ActivePlayer.Inventory.Character;

            //Hover
            if (SelectedCharacterIndex >= 0)
            {
                int FinalX = X + ((MouseHelper.MouseStateCurrent.X - X) / BoxWidth) * BoxWidth;
                int FinalY = DrawY + ((MouseHelper.MouseStateCurrent.Y - DrawY) / LineHeight) * LineHeight;

                if (SelectedCharacterIndex >= CurrentContainer.ListFolder.Count)
                {
                    PlayerCharacterInfo SelectedCharacter = CurrentContainer.ListCharacter[SelectedCharacterIndex - CurrentContainer.ListFolder.Count];

                    CharacterToDraw = SelectedCharacter;

                    g.Draw(sprPixel, new Rectangle(FinalX, FinalY, BoxWidth, BoxHeight), Color.FromNonPremultiplied(255, 255, 255, 127));
                    DrawBox(g, new Vector2(FinalX - 10, FinalY + BoxHeight), BoxWidth + 30, 25, Color.Black);
                    g.DrawStringMiddleAligned(fntMenuText,
                        SelectedCharacter.Character.Name,
                        new Vector2(FinalX + 5 + BoxWidth / 2, FinalY + BoxHeight), Color.White);
                }
                else
                {
                    g.Draw(sprPixel, new Rectangle(FinalX + 8, DrawY + 12, BoxWidth - 16, BoxHeight - 24), Color.FromNonPremultiplied(255, 255, 255, 127));
                }
            }

            int CursorX = SelectionIndex % BoxPerLine;
            int CursorY = SelectionIndex / BoxPerLine;
            MenuHelper.DrawFingerIcon(g, new Vector2(CharacterMenuX + CursorX * BoxWidth - 100 * Ratio, DrawY + CursorY * LineHeight + 160 * Ratio));

            //Right Side

            X = RightSectionCenterX;
            int CharacterModelBoxWidth = 450;
            g.Draw(GameScreen.sprPixel, new Rectangle(X - CharacterModelBoxWidth / 2, 200, CharacterModelBoxWidth, 550), Color.FromNonPremultiplied(Lobby.BackgroundColor.R, Lobby.BackgroundColor.G, Lobby.BackgroundColor.B, 50));
            DrawEmptyBox(g, new Vector2(X - CharacterModelBoxWidth / 2, 200), CharacterModelBoxWidth, 550);

            g.DrawStringCentered(fntMenuText, "Select a skin", new Vector2(X, Y), Color.White);

            int SkinCount = CharacterToDraw.ListOwnedUnitSkin.Count;
            int TotalSkinSize = SkinCount * (SkinBoxWidth + 5);

            X -= (TotalSkinSize - SkinBoxWidth - 5) / 2;
            for (int S = 0; S < SkinCount; ++S)
            {
                DrawEmptyBox(g, new Vector2(X - SkinBoxWidth / 2 - 2, SkinSectionY), SkinBoxWidth, SkinBoxHeight);
                g.Draw(CharacterToDraw.ListOwnedUnitSkin[S].CharacterSkin.SpriteShop, new Rectangle(X, SkinSectionY + 37, SkinSpriteWidth, SkinSpriteHeight), null, Color.White, 0, new Vector2(CharacterToDraw.ListOwnedUnitSkin[S].CharacterSkin.SpriteShop.Width / 2, CharacterToDraw.ListOwnedUnitSkin[S].CharacterSkin.SpriteShop.Height / 2), SpriteEffects.None, 0);

                X += SkinBoxWidth;
            }

            X = RightSectionCenterX;
            X -= (TotalSkinSize - SkinBoxWidth - 5 + SkinBoxWidth) / 2;

            string SkinToDrawName = CharacterToDraw.Character.Name;

            int SelectedSkinIndex = GetCharacterSkinUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);
            if (SelectedSkinIndex >= 0)
            {
                SkinToDrawName = CharacterToDraw.ListOwnedUnitSkin[SelectedSkinIndex].CharacterSkin.Name;
                int FinalX = X + ((MouseHelper.MouseStateCurrent.X - X) / SkinBoxWidth) * SkinBoxWidth;
                int FinalY = SkinSectionY + ((MouseHelper.MouseStateCurrent.Y - SkinSectionY) / LineHeight) * LineHeight;

                g.Draw(sprPixel, new Rectangle(FinalX + 5, FinalY + 5, SkinBoxWidth - 10, SkinBoxHeight - 10), Color.FromNonPremultiplied(255, 255, 255, 127));
            }

            g.Draw(sprSelectAUnitToBuy, new Vector2(2550 * Ratio, 780), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
            g.DrawStringCentered(fntOxanimumBold, SkinToDrawName, new Vector2((2550 + sprSelectAUnitToBuy.Width / 2) * Ratio, 1640 * Ratio), TextColor);

            foreach (IUIElement ActiveButton in ArrayUIElement)
            {
                ActiveButton.Draw(g);
            }

            Matrix Projection = Matrix.CreateOrthographicOffCenter(0, Constants.Width, Constants.Height, 0, 300, -300);
            Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            Projection = HalfPixelOffset * Projection;

            g.End();
            g.Begin();

            if (CharacterToDraw.Character.Unit3DModel != null)
            {
                g.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                g.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
                g.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                CharacterToDraw.Character.Unit3DModel.Draw3D(g.GraphicsDevice, Matrix.CreateRotationZ(MathHelper.ToRadians(180)) * Matrix.CreateRotationY(MathHelper.ToRadians(180)) * Matrix.CreateScale(2.5f) * Matrix.CreateTranslation(1500, 750, 0), Projection, Matrix.Identity);
            }
            else if (CharacterToDraw.Character.SpriteMap != null)
            {
                X = (int)(2650 * Ratio);
                Y = (int)(500 * Ratio);
                int SpriteWidth = CharacterToDraw.Character.SpriteMap.Width;
                int SpriteHeight = CharacterToDraw.Character.SpriteMap.Height;
                int SpriteHeightMax = (int)(1000 * Ratio);

                g.Draw(CharacterToDraw.Character.SpriteMap, new Rectangle(X, Y, SpriteWidth, SpriteHeightMax), new Rectangle(0, 0, SpriteWidth, SpriteHeightMax), Color.White);
            }
        }
    }
}
