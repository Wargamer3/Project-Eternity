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

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class CharacterSelectionScreen : GameScreen
    {
        #region Ressources

        private CardSymbols Symbols;

        private SpriteFont fntMenuText;

        private EmptyBoxScrollbar InventoryScrollbar;

        private TunnelManager TunnelBackground;

        private IUIElement[] ArrayUIElement;

        #endregion

        private int CharacterMenuX = 10;
        private int CharacterMenuY = 150;
        private int BoxWidth = 160;
        private int BoxHeight = 140;
        private int SpriteBoxWidth = 148;
        private int LineHeight = 155;

        private int RightSectionCenterX = 1500;
        private int SkinSpriteWidth = 75;
        private int SkinSpriteHeight = 64;
        private int SkinBoxWidth = 90;
        private int SkinBoxHeight = 74;
        private int SkinSectionY = 800;

        private readonly Player ActivePlayer;

        private int SelectionIndex;
        private int InventoryScrollbarValue;

        private CharacterInventoryContainer CurrentContainer;
        private List<CharacterInventoryContainer> ListLastContainer;

        public CharacterSelectionScreen(CardSymbols Symbols, Player ActivePlayer)
        {
            this.Symbols = Symbols;
            this.ActivePlayer = ActivePlayer;

            CurrentContainer = ActivePlayer.Inventory.RootCharacterContainer;
            ListLastContainer = new List<CharacterInventoryContainer>();
        }

        public override void Load()
        {
            InventoryScrollbar = new EmptyBoxScrollbar(new Vector2(Constants.Width - 23, BattleMapInventoryScreen.MiddleSectionY + 3), BattleMapInventoryScreen.MiddleSectionHeight - 5, 10, OnInventoryScrollbarChange);
            InventoryScrollbar.ChangeMaxValue(CurrentContainer.ListCharacter.Count * BoxHeight - BattleMapInventoryScreen.MiddleSectionHeight);

            ArrayUIElement = new IUIElement[]
            {
                InventoryScrollbar,
            };

            fntMenuText = Content.Load<SpriteFont>("Fonts/Arial12");

            TunnelBackground = new TunnelManager();
            TunnelBackground.Load(GraphicsDevice);
            for (int i = 0; i < 1400; ++i)
            {
                TunnelBackground.Update(0.016666);
            }
        }

        public override void Update(GameTime gameTime)
        {
            TunnelBackground.Update(gameTime.ElapsedGameTime.TotalSeconds);

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
            int X = CharacterMenuX;
            int DrawY = CharacterMenuY - InventoryScrollbarValue % BoxHeight;
            int SpriteOffset = BoxWidth / 2 - SpriteBoxWidth / 2;

            int BoxPerLine = (Constants.Width - Constants.Width / 3 - X) / BoxWidth;

            int MouseIndex = (MouseX - X) / BoxWidth + ((MouseY - DrawY) / LineHeight) * BoxPerLine;
            int MouseXFinal = (MouseX - X) % BoxWidth;
            int MouseYFinal = (MouseY - DrawY) % LineHeight;

            int ItemCount = CurrentContainer.ListCharacter.Count + CurrentContainer.ListFolder.Count;
            if (ListLastContainer.Count > 0)
            {
                ItemCount += 1;
            }

            if (MouseIndex >= 0 && MouseIndex < ItemCount
                && MouseXFinal >= SpriteOffset && MouseXFinal < SpriteOffset + BoxWidth
                && MouseX >= X && MouseX < X + BoxPerLine * BoxWidth
                && MouseYFinal >= 4 && MouseYFinal < 4 + BoxHeight)
            {
                if (MouseIndex >= CurrentContainer.ListFolder.Count)
                {
                    foreach (PlayerCharacterSkin ActiveSkin in CurrentContainer.ListCharacter[MouseIndex - CurrentContainer.ListFolder.Count].ListSkin)
                    {
                        if (ActiveSkin.Skin == null)
                        {
                            ActiveSkin.Skin = new PlayerCharacter(ActiveSkin.SkinPath, GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
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

            int SkinCount = ActivePlayer.Inventory.Character.ListSkin.Count;
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

        public override void Draw(CustomSpriteBatch g)
        {
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Lobby.BackgroundColor);

            TunnelBackground.Draw(g);

            g.End();
            g.Begin();

            bool ShowCheckbox = false;

            int X = -10;
            int Y = Constants.Height / 20;
            int HeaderHeight = Constants.Height / 16;
            DrawBox(g, new Vector2(X, Y), Constants.Width + 20, HeaderHeight, Color.White);

            X = Constants.Width / 20;
            Y += HeaderHeight / 2 - fntMenuText.LineSpacing / 2;
            g.DrawString(fntMenuText, "Character Selection", new Vector2(X, Y), Color.White);
            g.DrawStringMiddleAligned(fntMenuText, ActivePlayer.Name, new Vector2(Constants.Width / 2, Y), Color.White);

            X = CharacterMenuX;
            Y = CharacterMenuY;

            int DrawY = CharacterMenuY;
            int SpriteOffset = BoxWidth / 2 - SpriteBoxWidth / 2;

            int StartIndex = (int)Math.Floor(InventoryScrollbarValue / (double)BoxHeight);
            int BoxPerLine = (Constants.Width - Constants.Width / 3 - X) / BoxWidth;

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

                        g.Draw(CurrentContainer.ListCharacter[CurrentIndex].SpriteShop, new Vector2(FinalX + SpriteOffset + 3, DrawY + 7), Color.White);
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

            PlayerCharacter CharacterToDraw = ActivePlayer.Inventory.Character;

            //Hover
            if (SelectedCharacterIndex >= 0)
            {
                int FinalX = X + ((MouseHelper.MouseStateCurrent.X - X) / BoxWidth) * BoxWidth;
                int FinalY = DrawY + ((MouseHelper.MouseStateCurrent.Y - DrawY) / LineHeight) * LineHeight;

                if (SelectedCharacterIndex >= CurrentContainer.ListFolder.Count)
                {
                    PlayerCharacter SelectedCharacter = CurrentContainer.ListCharacter[SelectedCharacterIndex - CurrentContainer.ListFolder.Count];

                    CharacterToDraw = SelectedCharacter;

                    g.Draw(sprPixel, new Rectangle(FinalX, FinalY, BoxWidth, BoxHeight), Color.FromNonPremultiplied(255, 255, 255, 127));
                    DrawBox(g, new Vector2(FinalX - 10, FinalY + BoxHeight), BoxWidth + 30, 25, Color.Black);
                    g.DrawStringMiddleAligned(fntMenuText,
                        SelectedCharacter.Name,
                        new Vector2(FinalX + 5 + BoxWidth / 2, FinalY + BoxHeight), Color.White);
                }
                else
                {
                    g.Draw(sprPixel, new Rectangle(FinalX + 8, DrawY + 12, BoxWidth - 16, BoxHeight - 24), Color.FromNonPremultiplied(255, 255, 255, 127));
                }
            }

            MenuHelper.DrawFingerIcon(g, new Vector2(-45, Constants.Height / 7 + LineHeight / 3 + SelectionIndex * (LineHeight + 10)));

            X = -10;
            Y = Constants.Height - 100;
            DrawBox(g, new Vector2(X, Y), Constants.Width + 20, HeaderHeight, Color.White);
            X = Constants.Width / 18;
            Y += HeaderHeight / 2 - fntMenuText.LineSpacing / 2;
            g.DrawString(fntMenuText, "Select a character", new Vector2(X, Y), Color.White);

            //Right Side

            X = RightSectionCenterX;
            int CharacterModelBoxWidth = 450;
            g.Draw(GameScreen.sprPixel, new Rectangle(X - CharacterModelBoxWidth / 2, 200, CharacterModelBoxWidth, 550), Color.FromNonPremultiplied(Lobby.BackgroundColor.R, Lobby.BackgroundColor.G, Lobby.BackgroundColor.B, 200));
            DrawEmptyBox(g, new Vector2(X - CharacterModelBoxWidth / 2, 200), CharacterModelBoxWidth, 550);

            g.DrawStringCentered(fntMenuText, "Select a skin", new Vector2(X, Y), Color.White);

            int SkinCount = CharacterToDraw.ListSkin.Count;
            int TotalSkinSize = SkinCount * (SkinBoxWidth + 5);

            X -= (TotalSkinSize - SkinBoxWidth - 5) / 2;
            for (int S = 0; S < SkinCount; ++S)
            {
                DrawEmptyBox(g, new Vector2(X - SkinBoxWidth / 2 - 2, SkinSectionY), SkinBoxWidth, SkinBoxHeight);
                g.Draw(CharacterToDraw.ListSkin[S].Skin.SpriteShop, new Rectangle(X, SkinSectionY + 37, SkinSpriteWidth, SkinSpriteHeight), null, Color.White, 0, new Vector2(CharacterToDraw.ListSkin[S].Skin.SpriteShop.Width / 2, CharacterToDraw.ListSkin[S].Skin.SpriteShop.Height / 2), SpriteEffects.None, 0);

                X += SkinBoxWidth;
            }

            X = RightSectionCenterX;
            X -= (TotalSkinSize - SkinBoxWidth - 5 + SkinBoxWidth) / 2;

            PlayerCharacterSkin SkinToDraw = ActivePlayer.Inventory.Character.ListSkin[0];

            int SelectedSkinIndex = GetCharacterSkinUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);
            if (SelectedSkinIndex >= 0)
            {
                SkinToDraw = CharacterToDraw.ListSkin[SelectedSkinIndex];
                int FinalX = X + ((MouseHelper.MouseStateCurrent.X - X) / SkinBoxWidth) * SkinBoxWidth;
                int FinalY = SkinSectionY + ((MouseHelper.MouseStateCurrent.Y - SkinSectionY) / LineHeight) * LineHeight;

                g.Draw(sprPixel, new Rectangle(FinalX + 5, FinalY + 5, SkinBoxWidth - 10, SkinBoxHeight - 10), Color.FromNonPremultiplied(255, 255, 255, 127));
            }

            int NameWidth = (int)fntMenuText.MeasureString(SkinToDraw.Skin.Name).X;
            DrawEmptyBox(g, new Vector2(RightSectionCenterX - NameWidth / 2 - 5, 760), NameWidth + 6, 35);
            g.DrawStringCentered(fntMenuText, SkinToDraw.Skin.Name, new Vector2(RightSectionCenterX - 1, 780), Color.White);

            foreach (IUIElement ActiveButton in ArrayUIElement)
            {
                ActiveButton.Draw(g);
            }

            Matrix Projection = Matrix.CreateOrthographicOffCenter(0, Constants.Width, Constants.Height, 0, 300, -300);
            Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            Projection = HalfPixelOffset * Projection;

            g.End();
            g.Begin();

            g.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            g.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            g.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            CharacterToDraw.Unit3DModel.Draw(Matrix.CreateRotationZ(MathHelper.ToRadians(180)) * Matrix.CreateRotationY(MathHelper.ToRadians(180)) * Matrix.CreateScale(2.5f) * Matrix.CreateTranslation(1500, 750, 0), Projection, Matrix.Identity);
        }
    }
}
