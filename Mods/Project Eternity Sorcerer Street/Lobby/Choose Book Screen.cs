using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ChooseBookScreen : GameScreen
    {
        #region Ressources

        private CardSymbols Symbols;

        private SpriteFont fntArial12;

        #endregion

        private readonly Player ActivePlayer;
        private readonly CardBook ActiveBook;

        private int CursorIndex;

        public ChooseBookScreen(CardSymbols Symbols, Player ActivePlayer)
        {
            this.Symbols = Symbols;
            this.ActivePlayer = ActivePlayer;

            ActiveBook = ActivePlayer.Inventory.ActiveBook;
        }

        public override void Load()
        {
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
        }

        public override void Update(GameTime gameTime)
        {
            if (InputHelper.InputConfirmPressed())
            {
                if (CursorIndex < ActivePlayer.Inventory.ListBook.Count)
                {
                    PushScreen(new EditBookScreen(Symbols, ActivePlayer, ActivePlayer.Inventory.ListBook[CursorIndex]));
                }
                else if (CursorIndex == ActivePlayer.Inventory.ListBook.Count)
                {
                    PushScreen(new EditBookNameScreen(ActivePlayer));
                }
                else
                {
                    RemoveScreen(this);
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
                    CursorIndex = ActivePlayer.Inventory.ListBook.Count + 1;
                }
            }
            else if (InputHelper.InputDownPressed())
            {
                if (++CursorIndex > ActivePlayer.Inventory.ListBook.Count + 1)
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

            for (int B = 0; B < ActivePlayer.Inventory.ListBook.Count; ++B)
            {
                DrawBox(g, new Vector2(X, Y), Constants.Width / 2, EntryHeight, Color.White);
                g.DrawString(fntArial12, ActivePlayer.Inventory.ListBook[B].BookName, new Vector2(X + 150, Y + EntryHeight / 2 - fntArial12.LineSpacing / 2), Color.White);
                Y += EntryHeight + 10;
            }

            DrawBox(g, new Vector2(X, Y), Constants.Width / 2, EntryHeight, Color.White);
            g.DrawString(fntArial12, "New", new Vector2(X + 150, Y + EntryHeight / 2 - fntArial12.LineSpacing / 2), Color.White);
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
