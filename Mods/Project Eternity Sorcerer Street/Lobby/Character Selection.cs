using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.UI;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class CharacterSelectionScreen : GameScreen
    {
        private enum MenuSelections { Nothing, Folders, Unit }

        #region Ressources

        private CardSymbols Symbols;

        private SpriteFont fntArial12;

        private EmptyBoxScrollbar InventoryScrollbar;

        private TunnelManager TunnelBackground;

        private IUIElement[] ArrayUIElement;

        #endregion

        private int BoxWidth = 160;
        private int BoxHeight = 140;

        private readonly Player ActivePlayer;

        private MenuSelections MenuSelection;
        private int SelectionIndex;
        private int InventoryScrollbarValue;

        private CharacterInventoryContainer CurrentContainer;
        private List<CharacterInventoryContainer> ListLastContainer;

        private int CursorIndex;

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

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");

            TunnelBackground = new TunnelManager();
            TunnelBackground.Load(GraphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
            TunnelBackground.Update(gameTime);

            foreach (IUIElement ActiveButton in ArrayUIElement)
            {
                ActiveButton.Update(gameTime);
            }

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
                    PlayerCharacter UnitToBuy = CurrentContainer.ListCharacter[SelectionIndex];
                }
            }
            else if (InputHelper.InputCancelPressed())
            {
                RemoveScreen(this);
            }
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
            Y += HeaderHeight / 2 - fntArial12.LineSpacing / 2;
            g.DrawString(fntArial12, "Character Selection", new Vector2(X, Y), Color.White);
            g.DrawStringMiddleAligned(fntArial12, ActivePlayer.Name, new Vector2(Constants.Width / 2, Y), Color.White);

            X = 10;
            Y = Constants.Height / 7;
            int LineHeight = 155;

            int DrawY = 150;
            int SpriteBoxWidth = 148;
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

            MenuHelper.DrawFingerIcon(g, new Vector2(-45, Constants.Height / 7 + LineHeight / 3 + CursorIndex * (LineHeight + 10)));

            X = -10;
            Y = Constants.Height - 100;
            DrawBox(g, new Vector2(X, Y), Constants.Width + 20, HeaderHeight, Color.White);
            X = Constants.Width / 18;
            Y += HeaderHeight / 2 - fntArial12.LineSpacing / 2;
            g.DrawString(fntArial12, "Select a character", new Vector2(X, Y), Color.White);

            //Right Side
            g.DrawString(fntArial12, "Select a skin", new Vector2(1500, Y), Color.White);
            DrawEmptyBox(g, new Vector2(1500, 760), 70, 35);
            g.DrawStringCentered(fntArial12, "Zeneth", new Vector2(1535, 780), Color.White);
            X = 1490;
            DrawEmptyBox(g, new Vector2(X, 800), 85, 74);
            g.Draw(CurrentContainer.ListCharacter[31].SpriteShop, new Rectangle(X + 5, 805, 75, 64), Color.White);

            foreach (IUIElement ActiveButton in ArrayUIElement)
            {
                ActiveButton.Draw(g);
            }

            g.Draw(GameScreen.sprPixel, new Rectangle(1300, 200, 450, 550), Color.FromNonPremultiplied(Lobby.BackgroundColor.R, Lobby.BackgroundColor.G, Lobby.BackgroundColor.B, 200));
            DrawEmptyBox(g, new Vector2(1300, 200), 450, 550);

            Matrix Projection = Matrix.CreateOrthographicOffCenter(0, Constants.Width, Constants.Height, 0, 300, -300);
            Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            Projection = HalfPixelOffset * Projection;

            g.End();
            g.Begin();

            g.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            g.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            g.GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            CurrentContainer.ListCharacter[31].Unit3DModel.Draw(Matrix.CreateRotationZ(MathHelper.ToRadians(180)) * Matrix.CreateRotationY(MathHelper.ToRadians(180)) * Matrix.CreateScale(2.5f) * Matrix.CreateTranslation(1550, 750, 0), Projection, Matrix.Identity);
        }
    }
}
