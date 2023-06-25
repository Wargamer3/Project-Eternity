using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class EditBookScreen : GameScreen
    {
        #region Ressources

        private CardSymbols Symbols;

        private SpriteFont fntArial12;

        #endregion

        private readonly Player ActivePlayer;
        private readonly CardBook ActiveBook;

        private int CursorIndex;

        public EditBookScreen(CardSymbols Symbols, Player ActivePlayer, CardBook ActiveBook)
        {
            this.Symbols = Symbols;
            this.ActivePlayer = ActivePlayer;
            this.ActiveBook = ActiveBook;
        }

        public override void Load()
        {
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
        }

        public override void Update(GameTime gameTime)
        {
            if (InputHelper.InputConfirmPressed())
            {
                switch (CursorIndex)
                {
                    case 0:
                        PushScreen(new EditBookCardListScreen(Symbols, ActivePlayer, ActiveBook));
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        PushScreen(new EditBookNameScreen(ActivePlayer, ActiveBook));
                        break;
                    case 5:
                        break;
                    case 6:
                        RemoveScreen(this);
                        break;
                }
            }
            else if (InputHelper.InputCancelPressed())
            {
                RemoveScreen(this);
            }
            else if (InputHelper.InputUpPressed())
            {
                if (--CursorIndex < 0)
                {
                    CursorIndex = 6;
                }
            }
            else if (InputHelper.InputDownPressed())
            {
                if (++CursorIndex > 6)
                {
                    CursorIndex = 0;
                }
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawBox(g, new Vector2(-5, -5), Constants.Width + 10, Constants.Height + 10, Color.White);

            float X = -10;
            float Y = Constants.Height / 20;
            int HeaderHeight = Constants.Height / 16;
            DrawBox(g, new Vector2(X, Y), Constants.Width + 20, HeaderHeight, Color.White);

            X = Constants.Width / 20;
            Y += HeaderHeight / 2 - fntArial12.LineSpacing / 2;
            g.DrawString(fntArial12, "Book Edit", new Vector2(X, Y), Color.White);
            g.DrawStringMiddleAligned(fntArial12, ActivePlayer.Name + "/" + ActiveBook.BookName, new Vector2(Constants.Width / 2, Y), Color.White);
            X = Constants.Width - Constants.Width / 8;
            g.DrawStringRightAligned(fntArial12, ActiveBook.ListCard.Count + " card(s)", new Vector2(X, Y), Color.White);
            g.DrawString(fntArial12, "OK", new Vector2(X + 20, Y), Color.White);

            X = -10;
            Y = Constants.Height / 7;
            int EntryHeight = Constants.Height / 20;
            DrawBox(g, new Vector2(X, Y), Constants.Width / 2, EntryHeight, Color.White);
            g.DrawString(fntArial12, "Edit", new Vector2(X + 150, Y + EntryHeight / 2 - fntArial12.LineSpacing / 2), Color.White);
            Y += EntryHeight + 10;
            DrawBox(g, new Vector2(X, Y), Constants.Width / 2, EntryHeight, Color.White);
            g.DrawString(fntArial12, "Change Book Cover", new Vector2(X + 150, Y + EntryHeight / 2 - fntArial12.LineSpacing / 2), Color.White);
            Y += EntryHeight + 10;
            DrawBox(g, new Vector2(X, Y), Constants.Width / 2, EntryHeight, Color.White);
            g.DrawString(fntArial12, "Edit Profile", new Vector2(X + 150, Y + EntryHeight / 2 - fntArial12.LineSpacing / 2), Color.White);
            Y += EntryHeight + 10;
            DrawBox(g, new Vector2(X, Y), Constants.Width / 2, EntryHeight, Color.White);
            g.DrawString(fntArial12, "Copy", new Vector2(X + 150, Y + EntryHeight / 2 - fntArial12.LineSpacing / 2), Color.White);
            Y += EntryHeight + 10;
            DrawBox(g, new Vector2(X, Y), Constants.Width / 2, EntryHeight, Color.White);
            g.DrawString(fntArial12, "Name Change", new Vector2(X + 150, Y + EntryHeight / 2 - fntArial12.LineSpacing / 2), Color.White);
            Y += EntryHeight + 10;
            DrawBox(g, new Vector2(X, Y), Constants.Width / 2, EntryHeight, Color.White);
            g.DrawString(fntArial12, "Reset", new Vector2(X + 150, Y + EntryHeight / 2 - fntArial12.LineSpacing / 2), Color.White);
            Y += EntryHeight + 10;
            DrawBox(g, new Vector2(X, Y), Constants.Width / 2, EntryHeight, Color.White);
            g.DrawString(fntArial12, "Return", new Vector2(X + 150, Y + EntryHeight / 2 - fntArial12.LineSpacing / 2), Color.White);

            MenuHelper.DrawFingerIcon(g, new Vector2(95, Constants.Height / 7 + EntryHeight / 3 + CursorIndex * (EntryHeight + 10)));

            SorcererStreetInventoryScreen.DrawBookInformation(g, fntArial12, "Book Information", Symbols, ActivePlayer.Inventory.GlobalBook);

            X = -10;
            Y = Constants.Height - 100;
            DrawBox(g, new Vector2(X, Y), Constants.Width + 20, HeaderHeight, Color.White);
            X = Constants.Width / 18;
            Y += HeaderHeight / 2 - fntArial12.LineSpacing / 2;
            g.DrawString(fntArial12, "Edit this Book's contents", new Vector2(X, Y), Color.White);
        }
    }
}
