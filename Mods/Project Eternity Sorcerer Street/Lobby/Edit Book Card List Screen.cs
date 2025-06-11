using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class EditBookCardListScreen : GameScreen
    {
        #region Ressources

        private CardSymbols Symbols;
        private IconHolder Icons;

        private SpriteFont fntOxanimumBoldTitle;
        private SpriteFont fntOxanimumRegular;
        private SpriteFont fntMenuText;

        private Texture2D sprExtraFrame;

        private Texture2D sprScrollbarBackground;
        private Texture2D sprScrollbar;

        #endregion

        private Scrollbar MissionScrollbar;

        private const int NumberOfFilterCard = 13;
        private const int CardSpacing = 7;
        private const int CardWidth = 85;
        private const int CardHeight = 110;
        private int HeaderHeight = Constants.Height / 16;

        private readonly Player ActivePlayer;
        private readonly CardBook ActiveBook;

        private int CursorIndex;
        private int ScrollbarIndex;

        public EditBookCardListScreen(CardSymbols Symbols, IconHolder Icons, Player ActivePlayer, CardBook ActiveBook)
        {
            RequireFocus = true;
            RequireDrawFocus = true;
            this.Symbols = Symbols;
            this.Icons = Icons;
            this.ActivePlayer = ActivePlayer;
            this.ActiveBook = ActiveBook;
        }

        public override void Load()
        {
            float Ratio = Constants.Height / 2160f;
            float MaxY = Constants.Height / 6 + 130 * 2 + (CardHeight + 20) * ((ActiveBook.ListCard.Count - 14) / 7);

            sprExtraFrame = Content.Load<Texture2D>("Menus/Lobby/Extra Frame 2");

            sprScrollbarBackground = Content.Load<Texture2D>("Menus/Lobby/Scrollbar Background");
            sprScrollbar = Content.Load<Texture2D>("Menus/Lobby/Scrollbar Bar");

            MissionScrollbar = new Scrollbar(sprScrollbar, new Vector2(3780 * Ratio, 360 * Ratio), Ratio, (int)(sprScrollbarBackground.Height * Ratio), 10, OnScrollbarChange);

            fntMenuText = Content.Load<SpriteFont>("Fonts/Arial12");
            fntOxanimumRegular = Content.Load<SpriteFont>("Fonts/Oxanium Regular");
            fntOxanimumBoldTitle = GameScreen.ContentFallback.Load<SpriteFont>("Fonts/Oxanium Bold Title");
        }

        private void OnScrollbarChange(float ScrollbarValue)
        {
            ScrollbarIndex = (int)ScrollbarValue;
        }

        public override void Update(GameTime gameTime)
        {
            SorcererStreetInventoryScreen.CubeBackground.Update(gameTime);

            MissionScrollbar.Update(gameTime);

            if (InputHelper.InputConfirmPressed())
            {
                switch (CursorIndex)
                {
                    case 0:
                        PushScreen(new EditBookCardListFilterScreen(ActivePlayer, ActiveBook, EditBookCardListFilterScreen.Filters.Creatures));
                        break;

                    case 1:
                        PushScreen(new EditBookCardListFilterScreen(ActivePlayer, ActiveBook, EditBookCardListFilterScreen.Filters.Neutral));
                        break;

                    case 2:
                        PushScreen(new EditBookCardListFilterScreen(ActivePlayer, ActiveBook, EditBookCardListFilterScreen.Filters.Fire));
                        break;

                    case 3:
                        PushScreen(new EditBookCardListFilterScreen(ActivePlayer, ActiveBook, EditBookCardListFilterScreen.Filters.Water));
                        break;

                    case 4:
                        PushScreen(new EditBookCardListFilterScreen(ActivePlayer, ActiveBook, EditBookCardListFilterScreen.Filters.Earth));
                        break;

                    case 5:
                        PushScreen(new EditBookCardListFilterScreen(ActivePlayer, ActiveBook, EditBookCardListFilterScreen.Filters.Air));
                        break;

                    case 6:
                        PushScreen(new EditBookCardListFilterScreen(ActivePlayer, ActiveBook, EditBookCardListFilterScreen.Filters.Dual));
                        break;
                }
                if (CursorIndex > NumberOfFilterCard)
                {
                    PushScreen(new EditBookCardScreen(ActivePlayer, ActiveBook, ActiveBook.ListCard[CursorIndex - NumberOfFilterCard - 1]));
                }
            }
            else if (InputHelper.InputCancelPressed())
            {
                RemoveScreen(this);
            }
            else if (InputHelper.InputRightPressed())
            {
                CursorIndex += 1;
                if (CursorIndex > NumberOfFilterCard && CursorIndex - NumberOfFilterCard > ActiveBook.ListCard.Count)
                {
                    CursorIndex -= CursorIndex % 7;
                }
                else if (CursorIndex == NumberOfFilterCard)
                {
                    CursorIndex -= NumberOfFilterCard % 7;
                }
                else if (CursorIndex % 7 == 0)
                {
                    CursorIndex -= 7;
                }
            }
            else if (InputHelper.InputLeftPressed())
            {
                CursorIndex -= 1;
                if (CursorIndex < 0)
                {
                    CursorIndex = 6;
                }
                else if (CursorIndex == 6)
                {
                    CursorIndex = NumberOfFilterCard - 1;
                }
                else if (CursorIndex % 7 == 6)
                {
                    if (CursorIndex >= NumberOfFilterCard)
                    {
                        if (CursorIndex - NumberOfFilterCard + 7 > ActiveBook.ListCard.Count)
                        {
                            CursorIndex = NumberOfFilterCard + ActiveBook.ListCard.Count;
                        }
                        else
                        {
                            CursorIndex += 7;
                        }
                    }
                    else
                    {
                        CursorIndex += 7;
                    }
                }
            }
            else if (InputHelper.InputDownPressed())
            {
                if (CursorIndex + 7 > NumberOfFilterCard + ActiveBook.ListCard.Count)
                {
                    if (CursorIndex > NumberOfFilterCard)
                    {
                        CursorIndex = CursorIndex % 7;
                    }
                    else if (ActiveBook.ListCard.Count < 7)
                    {
                        CursorIndex = NumberOfFilterCard + ActiveBook.ListCard.Count;
                    }
                }
                else if (CursorIndex + 7 == NumberOfFilterCard)
                {
                    CursorIndex = NumberOfFilterCard - 1;
                }
                else
                {
                    CursorIndex += 7;

                    if (CursorIndex > ActiveBook.ListCard.Count + NumberOfFilterCard)
                    {
                        CursorIndex -= 7;
                    }
                }

                int CursorY = (CardHeight + 20) * (CursorIndex / 7);

                if (CursorY < ScrollbarIndex)
                {
                    ScrollbarIndex = (CardHeight + 20) * (CursorIndex / 7);
                }
                else if (CursorY > ScrollbarIndex + (CardHeight + 20) * 2)
                {
                    ScrollbarIndex = (CardHeight + 20) * ((CursorIndex / 7) - 2);
                }
            }
            else if (InputHelper.InputUpPressed())
            {
                CursorIndex -= 7;
                if (CursorIndex < 0)
                {
                    if ((CursorIndex + 7) % 7 >= ActiveBook.ListCard.Count % 7)
                    {
                        CursorIndex = (ActiveBook.ListCard.Count / 7 + 2 - 1) * 7 + (CursorIndex + 7) % 7;
                    }
                    else
                    {
                        CursorIndex = (ActiveBook.ListCard.Count / 7 + 2) * 7 + (CursorIndex + 7) % 7;
                    }
                }

                int CursorY = (CardHeight + 20) * (CursorIndex / 7);

                if (CursorY < ScrollbarIndex)
                {
                    ScrollbarIndex = (CardHeight + 20) * (CursorIndex / 7);
                }
                else if (CursorY > ScrollbarIndex + (CardHeight + 20) * 2)
                {
                    ScrollbarIndex = (CardHeight + 20) * ((CursorIndex / 7) - 2);
                }
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float Ratio = Constants.Height / 2160f;
            Color ColorText = Color.FromNonPremultiplied(65, 70, 65, 255);

            SorcererStreetInventoryScreen.CubeBackground.Draw(g);

            DrawCategoryCards(g);
            DrawBookCards(g, Constants.Height / 6 + (CardHeight + 20) * 2 - ScrollbarIndex);

            float DrawX = (int)(210 * Ratio);
            float DrawY = (int)(58 * Ratio);
            g.DrawString(fntOxanimumBoldTitle, "BOOK EDIT", new Vector2(DrawX, DrawY), ColorText);

            DrawY = (int)(80 * Ratio);
            g.DrawStringMiddleAligned(fntOxanimumRegular, ActivePlayer.Name + "/" + ActiveBook.BookName, new Vector2(Constants.Width / 2, DrawY), ColorText);
            DrawX = Constants.Width - Constants.Width / 8;
            g.DrawStringRightAligned(fntOxanimumRegular, ActiveBook.TotalCards + " card(s)", new Vector2(DrawX, DrawY), ColorText);
            g.DrawString(fntOxanimumRegular, "OK", new Vector2(DrawX + 20, DrawY), ColorText);

            int CursorX = (int)(1150 * Ratio) + (CardWidth + CardSpacing) * (CursorIndex % 7);
            int CursorY = Constants.Height / 6 + CardHeight/ 2 + (CardHeight + 20) * (CursorIndex / 7);
            MenuHelper.DrawFingerIcon(g, new Vector2(CursorX, CursorY - ScrollbarIndex));

            SorcererStreetInventoryScreen.DrawBookInformationSmall(g, sprExtraFrame, fntMenuText, "Book Information", Symbols, Icons, ActivePlayer.Inventory.GlobalBook);

            DrawX = (int)(212 * Ratio);
            DrawY = (int)(2008 * Ratio);
            g.DrawString(fntOxanimumRegular, "Edit this Book's contents", new Vector2(DrawX, DrawY), ColorText);

            MissionScrollbar.Draw(g);
        }

        private void DrawCategoryCards(CustomSpriteBatch g)
        {
            Color ColorBox = Color.FromNonPremultiplied(204, 204, 204, 255);
            int X = Constants.Width / 2 - CardWidth / 2 - (CardWidth + CardSpacing) * 3;
            int Y = Constants.Height / 6 - ScrollbarIndex;

            g.Draw(sprPixel, new Rectangle(X, Y, CardWidth, CardHeight), ColorBox);
            TextHelper.DrawTextMiddleAligned(g, "Creature", new Vector2(X + CardWidth / 2, Y + 20), Color.White);
            g.Draw(Symbols.sprElementNeutral, new Vector2(X + CardWidth / 2 - 8, Y + CardHeight / 1.8f), Color.White);
            g.Draw(Symbols.sprElementEarth, new Vector2(X - 10 + CardWidth / 2 - 8, Y - 10 + CardHeight / 1.8f), Color.White);
            g.Draw(Symbols.sprElementFire, new Vector2(X + 14 + CardWidth / 2 - 8, Y - 10 + CardHeight / 1.8f), Color.White);
            g.Draw(Symbols.sprElementWater, new Vector2(X - 10 + CardWidth / 2 - 8, Y + 10 + CardHeight / 1.8f), Color.White);
            g.Draw(Symbols.sprElementAir, new Vector2(X + 14 + CardWidth / 2 - 8, Y + 10 + CardHeight / 1.8f), Color.White);
            X += CardWidth + CardSpacing;

            g.Draw(sprPixel, new Rectangle(X, Y, CardWidth, CardHeight), ColorBox);
            TextHelper.DrawTextMiddleAligned(g, "Creature", new Vector2(X + CardWidth / 2, Y + 20), Color.White);
            g.Draw(Symbols.sprElementNeutral, new Vector2(X + CardWidth / 2 - 8, Y + CardHeight / 1.8f), Color.White);
            X += CardWidth + CardSpacing;

            g.Draw(sprPixel, new Rectangle(X, Y, CardWidth, CardHeight), ColorBox);
            TextHelper.DrawTextMiddleAligned(g, "Creature", new Vector2(X + CardWidth / 2, Y + 20), Color.White);
            g.Draw(Symbols.sprElementFire, new Vector2(X + CardWidth / 2 - 8, Y + CardHeight / 1.8f), Color.White);
            X += CardWidth + CardSpacing;

            g.Draw(sprPixel, new Rectangle(X, Y, CardWidth, CardHeight), ColorBox);
            TextHelper.DrawTextMiddleAligned(g, "Creature", new Vector2(X + CardWidth / 2, Y + 20), Color.White);
            g.Draw(Symbols.sprElementWater, new Vector2(X + CardWidth / 2 - 8, Y + CardHeight / 1.8f), Color.White);
            X += CardWidth + CardSpacing;

            g.Draw(sprPixel, new Rectangle(X, Y, CardWidth, CardHeight), ColorBox);
            TextHelper.DrawTextMiddleAligned(g, "Creature", new Vector2(X + CardWidth / 2, Y + 20), Color.White);
            g.Draw(Symbols.sprElementEarth, new Vector2(X + CardWidth / 2 - 8, Y + CardHeight / 1.8f), Color.White);
            X += CardWidth + CardSpacing;

            g.Draw(sprPixel, new Rectangle(X, Y, CardWidth, CardHeight), ColorBox);
            TextHelper.DrawTextMiddleAligned(g, "Creature", new Vector2(X + CardWidth / 2, Y + 20), Color.White);
            g.Draw(Symbols.sprElementAir, new Vector2(X + CardWidth / 2 - 8, Y + CardHeight / 1.8f), Color.White);
            X += CardWidth + CardSpacing;

            g.Draw(sprPixel, new Rectangle(X, Y, CardWidth, CardHeight), ColorBox);
            TextHelper.DrawTextMiddleAligned(g, "Creature", new Vector2(X + CardWidth / 2, Y + 20), Color.White);
            g.Draw(Symbols.sprElementMulti, new Vector2(X + CardWidth / 2 - 8, Y + CardHeight / 1.8f), Color.White);
            X += CardWidth + CardSpacing;

            X = Constants.Width / 2 - CardWidth / 2 - (CardWidth + CardSpacing) * 3;
            Y += CardHeight + 20;

            g.Draw(sprPixel, new Rectangle(X, Y, CardWidth, CardHeight), ColorBox);
            TextHelper.DrawTextMiddleAligned(g, "Item", new Vector2(X + CardWidth / 2, Y + 20), Color.White);
            g.Draw(Symbols.sprItemsWeapon, new Vector2(X + CardWidth / 2 - 8, Y + CardHeight / 1.8f), Color.White);
            X += CardWidth + CardSpacing;

            g.Draw(sprPixel, new Rectangle(X, Y, CardWidth, CardHeight), ColorBox);
            TextHelper.DrawTextMiddleAligned(g, "Spell", new Vector2(X + CardWidth / 2, Y + 20), Color.White);
            g.Draw(Symbols.sprSpellsMultiple, new Vector2(X + CardWidth / 2 - 8, Y + CardHeight / 1.8f), Color.White);
            X += CardWidth + CardSpacing;

            g.Draw(sprPixel, new Rectangle(X, Y, CardWidth, CardHeight), ColorBox);
            TextHelper.DrawTextMiddleAligned(g, "New", new Vector2(X + CardWidth / 2, Y + 20), Color.White);
            X += CardWidth + CardSpacing;

            g.Draw(sprPixel, new Rectangle(X, Y, CardWidth, CardHeight), ColorBox);
            TextHelper.DrawTextMiddleAligned(g, "All", new Vector2(X + CardWidth / 2, Y + 20), Color.White);
            X += CardWidth + CardSpacing;

            g.Draw(sprPixel, new Rectangle(X, Y, CardWidth, CardHeight), ColorBox);
            TextHelper.DrawTextMiddleAligned(g, "Catalog", new Vector2(X + CardWidth / 2, Y + 20), Color.White);
            X += CardWidth + CardSpacing;

            g.Draw(sprPixel, new Rectangle(X, Y, CardWidth, CardHeight), ColorBox);
            TextHelper.DrawTextMiddleAligned(g, "End", new Vector2(X + CardWidth / 2, Y + 20), Color.White);
            X += CardWidth + CardSpacing;
        }

        private void DrawBookCards(CustomSpriteBatch g, int StartY)
        {
            Color ColorBox = Color.FromNonPremultiplied(204, 204, 204, 255);
            int CardNumberBoxWidth = (int)(CardWidth / 3.2f);

            for (int C = 0; C <  ActiveBook.ListCard.Count; ++C)
            {
                int X = Constants.Width / 2 - CardWidth / 2 - (CardWidth + CardSpacing) * 3 + (CardWidth + CardSpacing) * (C % 7);
                int Y = StartY + (CardHeight + 20) * (C / 7);

                g.Draw(ActiveBook.ListCard[C].Card.sprCard, new Rectangle((int)X, (int)Y, CardWidth, CardHeight), new Rectangle(0, 0, ActiveBook.ListCard[C].Card.sprCard.Width, ActiveBook.ListCard[C].Card.sprCard.Height), Color.White);
                g.Draw(sprPixel, new Rectangle(X + CardWidth / 2 - CardNumberBoxWidth / 2, Y + CardHeight - CardNumberBoxWidth / 2, CardNumberBoxWidth, CardNumberBoxWidth), ColorBox);
                TextHelper.DrawTextMiddleAligned(g, ActiveBook.ListCard[C].QuantityOwned.ToString(), new Vector2(X + CardWidth / 2, Y + 3 + CardHeight - CardNumberBoxWidth / 2), Color.White);
            }
        }
    }
}
