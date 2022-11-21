using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.GameScreens.UI;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class EditBookCardListScreen : GameScreen
    {
        #region Ressources

        private SpriteFont fntArial12;

        public Texture2D sprMenuCursor;

        public Texture2D sprElementAir;
        public Texture2D sprElementEarth;
        public Texture2D sprElementFire;
        public Texture2D sprElementWater;
        public Texture2D sprElementNeutral;
        public Texture2D sprElementMulti;

        public Texture2D sprItemsWeapon;
        public Texture2D sprItemsArmor;
        public Texture2D sprItemsTool;
        public Texture2D sprItemsScroll;

        public Texture2D sprSpellsSingle;
        public Texture2D sprSpellsMultiple;

        public Texture2D sprEnchantSingle;
        public Texture2D sprEnchantMultiple;

        #endregion

        private BoxScrollbar MissionScrollbar;

        private const int NumberOfFilterCard = 13;
        private const int CardSpacing = 7;
        private const int CardWidth = 85;
        private const int CardHeight = 110;
        private int HeaderHeight = Constants.Height / 16;

        private readonly Player ActivePlayer;
        private readonly CardBook ActiveBook;

        private int CursorIndex;
        private int ScrollbarIndex;

        public EditBookCardListScreen(Player ActivePlayer, CardBook ActiveBook)
        {
            this.ActivePlayer = ActivePlayer;
            this.ActiveBook = ActiveBook;
        }

        public override void Load()
        {
            float MaxY = Constants.Height / 6 + 130 * 2 + (CardHeight + 20) * ((ActiveBook.ListCard.Count - 14) / 7);
            MissionScrollbar = new BoxScrollbar(new Vector2(Constants.Width - 20, Constants.Height / 6), Constants.Height - Constants.Height / 3, MaxY, OnMissionScrollbarChange);

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");

            sprMenuCursor = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Cursor");

            sprElementAir = Content.Load<Texture2D>("Sorcerer Street/Ressources/Elements/Air");
            sprElementEarth = Content.Load<Texture2D>("Sorcerer Street/Ressources/Elements/Earth");
            sprElementFire = Content.Load<Texture2D>("Sorcerer Street/Ressources/Elements/Fire");
            sprElementWater = Content.Load<Texture2D>("Sorcerer Street/Ressources/Elements/Water");
            sprElementMulti = Content.Load<Texture2D>("Sorcerer Street/Ressources/Elements/Multi");
            sprElementNeutral = Content.Load<Texture2D>("Sorcerer Street/Ressources/Elements/Neutral");

            sprItemsWeapon = Content.Load<Texture2D>("Sorcerer Street/Ressources/Weapon");
            sprItemsArmor = Content.Load<Texture2D>("Sorcerer Street/Ressources/Armor");
            sprItemsTool = Content.Load<Texture2D>("Sorcerer Street/Ressources/Tool");
            sprItemsScroll = Content.Load<Texture2D>("Sorcerer Street/Ressources/Scroll");

            sprSpellsSingle = Content.Load<Texture2D>("Sorcerer Street/Ressources/Single Instant");
            sprSpellsMultiple = Content.Load<Texture2D>("Sorcerer Street/Ressources/Multiple Instant");

            sprEnchantSingle = Content.Load<Texture2D>("Sorcerer Street/Ressources/Single Enchant");
            sprEnchantMultiple = Content.Load<Texture2D>("Sorcerer Street/Ressources/Multiple Enchant");
        }

        private void OnMissionScrollbarChange(float ScrollbarValue)
        {
            ScrollbarIndex = (int)ScrollbarValue;
        }

        public override void Update(GameTime gameTime)
        {
            MissionScrollbar.Update(gameTime);

            if (InputHelper.InputConfirmPressed())
            {
                switch (CursorIndex)
                {
                    case 0:
                        PushScreen(new EditBookCardListFilterScreen(ActivePlayer, ActiveBook, "creatures"));
                        break;

                    case 1:
                        PushScreen(new EditBookCardListFilterScreen(ActivePlayer, ActiveBook, "neutral"));
                        break;

                    case 2:
                        PushScreen(new EditBookCardListFilterScreen(ActivePlayer, ActiveBook, "fire"));
                        break;

                    case 3:
                        PushScreen(new EditBookCardListFilterScreen(ActivePlayer, ActiveBook, "water"));
                        break;

                    case 4:
                        PushScreen(new EditBookCardListFilterScreen(ActivePlayer, ActiveBook, "earth"));
                        break;

                    case 5:
                        PushScreen(new EditBookCardListFilterScreen(ActivePlayer, ActiveBook, "air"));
                        break;

                    case 6:
                        PushScreen(new EditBookCardListFilterScreen(ActivePlayer, ActiveBook, "multi"));
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
            DrawBox(g, new Vector2(-5, -5), Constants.Width + 10, Constants.Height + 10, Color.White);

            DrawCategoryCards(g);
            DrawBookCards(g, Constants.Height / 6 + (CardHeight + 20) * 2 - ScrollbarIndex);

            float X = -10;
            float Y = Constants.Height / 20;
            DrawBox(g, new Vector2(X, Y), Constants.Width + 20, HeaderHeight, Color.White);

            X = Constants.Width / 20;
            Y += HeaderHeight / 2 - fntArial12.LineSpacing / 2;
            g.DrawString(fntArial12, "Book Edit", new Vector2(X, Y), Color.White);
            g.DrawStringMiddleAligned(fntArial12, ActivePlayer.Name + "/" + ActiveBook.BookName, new Vector2(Constants.Width / 2, Y), Color.White);
            X = Constants.Width - Constants.Width / 8;
            g.DrawStringRightAligned(fntArial12, ActiveBook.TotalCards + " card(s)", new Vector2(X, Y), Color.White);
            g.DrawString(fntArial12, "OK", new Vector2(X + 20, Y), Color.White);

            int CursorX = Constants.Width / 2 - CardWidth / 2 - (CardWidth + CardSpacing) * 3 + (CardWidth + CardSpacing) * (CursorIndex % 7);
            int CursorY = Constants.Height / 6 + CardHeight/ 2 + (CardHeight + 20) * (CursorIndex / 7);
            g.Draw(sprMenuCursor, new Rectangle(CursorX, CursorY - ScrollbarIndex, 40, 40), Color.White);

            X = -10;
            Y = Constants.Height - Constants.Height / 20 - HeaderHeight;
            DrawBox(g, new Vector2(X, Y), Constants.Width + 20, HeaderHeight, Color.White);
            X = Constants.Width / 18;
            Y += HeaderHeight / 2 - fntArial12.LineSpacing / 2;
            g.DrawString(fntArial12, "Edit this Book's contents", new Vector2(X, Y), Color.White);

            MissionScrollbar.Draw(g);
        }

        private void DrawCategoryCards(CustomSpriteBatch g)
        {
            float X = Constants.Width / 2 - CardWidth / 2 - (CardWidth + CardSpacing) * 3;
            float Y = Constants.Height / 6 - ScrollbarIndex;

            DrawBox(g, new Vector2(X, Y), CardWidth, CardHeight, Color.White);
            TextHelper.DrawTextMiddleAligned(g, "Creature", new Vector2(X + CardWidth / 2, Y + 20), Color.White);
            g.Draw(sprElementNeutral, new Vector2(X + CardWidth / 2 - 8, Y + CardHeight / 1.8f), Color.White);
            g.Draw(sprElementEarth, new Vector2(X - 10 + CardWidth / 2 - 8, Y - 10 + CardHeight / 1.8f), Color.White);
            g.Draw(sprElementFire, new Vector2(X + 14 + CardWidth / 2 - 8, Y - 10 + CardHeight / 1.8f), Color.White);
            g.Draw(sprElementWater, new Vector2(X - 10 + CardWidth / 2 - 8, Y + 10 + CardHeight / 1.8f), Color.White);
            g.Draw(sprElementAir, new Vector2(X + 14 + CardWidth / 2 - 8, Y + 10 + CardHeight / 1.8f), Color.White);
            X += CardWidth + CardSpacing;

            DrawBox(g, new Vector2(X, Y), CardWidth, CardHeight, Color.White);
            TextHelper.DrawTextMiddleAligned(g, "Creature", new Vector2(X + CardWidth / 2, Y + 20), Color.White);
            g.Draw(sprElementNeutral, new Vector2(X + CardWidth / 2 - 8, Y + CardHeight / 1.8f), Color.White);
            X += CardWidth + CardSpacing;

            DrawBox(g, new Vector2(X, Y), CardWidth, CardHeight, Color.White);
            TextHelper.DrawTextMiddleAligned(g, "Creature", new Vector2(X + CardWidth / 2, Y + 20), Color.White);
            g.Draw(sprElementFire, new Vector2(X + CardWidth / 2 - 8, Y + CardHeight / 1.8f), Color.White);
            X += CardWidth + CardSpacing;

            DrawBox(g, new Vector2(X, Y), CardWidth, CardHeight, Color.White);
            TextHelper.DrawTextMiddleAligned(g, "Creature", new Vector2(X + CardWidth / 2, Y + 20), Color.White);
            g.Draw(sprElementWater, new Vector2(X + CardWidth / 2 - 8, Y + CardHeight / 1.8f), Color.White);
            X += CardWidth + CardSpacing;

            DrawBox(g, new Vector2(X, Y), CardWidth, CardHeight, Color.White);
            TextHelper.DrawTextMiddleAligned(g, "Creature", new Vector2(X + CardWidth / 2, Y + 20), Color.White);
            g.Draw(sprElementEarth, new Vector2(X + CardWidth / 2 - 8, Y + CardHeight / 1.8f), Color.White);
            X += CardWidth + CardSpacing;

            DrawBox(g, new Vector2(X, Y), CardWidth, CardHeight, Color.White);
            TextHelper.DrawTextMiddleAligned(g, "Creature", new Vector2(X + CardWidth / 2, Y + 20), Color.White);
            g.Draw(sprElementAir, new Vector2(X + CardWidth / 2 - 8, Y + CardHeight / 1.8f), Color.White);
            X += CardWidth + CardSpacing;

            DrawBox(g, new Vector2(X, Y), CardWidth, CardHeight, Color.White);
            TextHelper.DrawTextMiddleAligned(g, "Creature", new Vector2(X + CardWidth / 2, Y + 20), Color.White);
            g.Draw(sprElementMulti, new Vector2(X + CardWidth / 2 - 8, Y + CardHeight / 1.8f), Color.White);
            X += CardWidth + CardSpacing;

            X = Constants.Width / 2 - CardWidth / 2 - (CardWidth + CardSpacing) * 3;
            Y += CardHeight + 20;

            DrawBox(g, new Vector2(X, Y), CardWidth, CardHeight, Color.White);
            TextHelper.DrawTextMiddleAligned(g, "Item", new Vector2(X + CardWidth / 2, Y + 20), Color.White);
            g.Draw(sprItemsWeapon, new Vector2(X + CardWidth / 2 - 8, Y + CardHeight / 1.8f), Color.White);
            X += CardWidth + CardSpacing;

            DrawBox(g, new Vector2(X, Y), CardWidth, CardHeight, Color.White);
            TextHelper.DrawTextMiddleAligned(g, "Spell", new Vector2(X + CardWidth / 2, Y + 20), Color.White);
            g.Draw(sprSpellsMultiple, new Vector2(X + CardWidth / 2 - 8, Y + CardHeight / 1.8f), Color.White);
            X += CardWidth + CardSpacing;

            DrawBox(g, new Vector2(X, Y), CardWidth, CardHeight, Color.White);
            TextHelper.DrawTextMiddleAligned(g, "New", new Vector2(X + CardWidth / 2, Y + 20), Color.White);
            X += CardWidth + CardSpacing;

            DrawBox(g, new Vector2(X, Y), CardWidth, CardHeight, Color.White);
            TextHelper.DrawTextMiddleAligned(g, "All", new Vector2(X + CardWidth / 2, Y + 20), Color.White);
            X += CardWidth + CardSpacing;

            DrawBox(g, new Vector2(X, Y), CardWidth, CardHeight, Color.White);
            TextHelper.DrawTextMiddleAligned(g, "Catalog", new Vector2(X + CardWidth / 2, Y + 20), Color.White);
            X += CardWidth + CardSpacing;

            DrawBox(g, new Vector2(X, Y), CardWidth, CardHeight, Color.White);
            TextHelper.DrawTextMiddleAligned(g, "End", new Vector2(X + CardWidth / 2, Y + 20), Color.White);
            X += CardWidth + CardSpacing;
        }

        private void DrawBookCards(CustomSpriteBatch g, float StartY)
        {
            int CardNumberBoxWidth = (int)(CardWidth / 3.2f);

            for (int C = 0; C <  ActiveBook.ListCard.Count; ++C)
            {
                float X = Constants.Width / 2 - CardWidth / 2 - (CardWidth + CardSpacing) * 3 + (CardWidth + CardSpacing) * (C % 7);
                float Y = StartY + (CardHeight + 20) * (C / 7);

                g.Draw(ActiveBook.ListCard[C].sprCard, new Rectangle((int)X, (int)Y, CardWidth, CardHeight), new Rectangle(0, 0, ActiveBook.ListCard[C].sprCard.Width, ActiveBook.ListCard[C].sprCard.Height), Color.White);
                DrawBox(g, new Vector2(X + CardWidth / 2 - CardNumberBoxWidth / 2, Y + CardHeight - CardNumberBoxWidth / 2), CardNumberBoxWidth, CardNumberBoxWidth, Color.White);
                TextHelper.DrawTextMiddleAligned(g, ActiveBook.ListCard[C].QuantityOwned.ToString(), new Vector2(X + CardWidth / 2, Y + 3 + CardHeight - CardNumberBoxWidth / 2), Color.White);
            }
        }
    }
}
