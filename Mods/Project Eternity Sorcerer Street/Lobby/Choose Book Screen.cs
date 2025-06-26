using System;
using System.Collections.Generic;
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
        private IconHolder Icons;

        private SpriteFont fntOxanimumBoldTitle;
        private SpriteFont fntOxanimumRegular;
        private SpriteFont fntMenuText;

        private Texture2D sprExtraFrame;
        private Texture2D sprFrameTop;

        #endregion

        private readonly Player ActivePlayer;
        private readonly CardBook ActiveBook;

        private BookInventoryContainer CurrentContainer;
        private List<BookInventoryContainer> ListLastContainer;

        private int CursorIndex;

        public ChooseBookScreen(CardSymbols Symbols, IconHolder Icons, Player ActivePlayer)
        {
            RequireFocus = true;
            RequireDrawFocus = true;
            this.Symbols = Symbols;
            this.Icons = Icons;
            this.ActivePlayer = ActivePlayer;

            ActiveBook = ActivePlayer.Inventory.ActiveBook;
            CurrentContainer = ActivePlayer.Inventory.RootBookContainer;
            ListLastContainer = new List<BookInventoryContainer>();
        }

        public override void Load()
        {
            fntMenuText = Content.Load<SpriteFont>("Fonts/Arial12");
            fntOxanimumRegular = Content.Load<SpriteFont>("Fonts/Oxanium Regular");
            fntOxanimumBoldTitle = GameScreen.ContentFallback.Load<SpriteFont>("Fonts/Oxanium Bold Title");

            sprExtraFrame = Content.Load<Texture2D>("Menus/Lobby/Extra Frame 2");
            sprFrameTop = Content.Load<Texture2D>("Menus/Lobby/Room/Frame Top");
        }

        public override void Update(GameTime gameTime)
        {
            SorcererStreetInventoryScreen.CubeBackground.Update(gameTime);

            if (InputHelper.InputConfirmPressed())
            {
                if (CursorIndex < ActivePlayer.Inventory.DicOwnedBook.Count)
                {
                    PushScreen(new EditBookScreen(Symbols, Icons, ActivePlayer, CurrentContainer.ListBook[CursorIndex - CurrentContainer.DicFolder.Count]));
                }
                else if (CursorIndex == ActivePlayer.Inventory.DicOwnedBook.Count)//New Book
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
                    CursorIndex = ActivePlayer.Inventory.DicOwnedBook.Count + 1;
                }
            }
            else if (InputHelper.InputDownPressed())
            {
                if (++CursorIndex > ActivePlayer.Inventory.DicOwnedBook.Count + 1)
                {
                    CursorIndex = 0;
                }
            }
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            SorcererStreetInventoryScreen.CubeBackground.BeginDraw(g);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float Ratio = Constants.Height / 2160f;
            Color ColorBox = Color.FromNonPremultiplied(204, 204, 204, 255);
            Color ColorText = Color.FromNonPremultiplied(65, 70, 65, 255);

            SorcererStreetInventoryScreen.CubeBackground.Draw(g, true);

            float DrawX = (int)(210 * Ratio);
            float DrawY = (int)(58 * Ratio);
            g.DrawString(fntOxanimumBoldTitle, "BOOK EDIT", new Vector2(DrawX, DrawY), ColorText);

            SorcererStreetInventoryScreen.DrawBookIsReady(g, fntMenuText, ActivePlayer.Name, ActiveBook);

            DrawX = (int)(150 * Ratio);
            DrawY = (int)(400 * Ratio);

            int EntryHeight = (int)(108 * Ratio);
            int BoxHeight = (int)(994 * Ratio);

            g.Draw(sprFrameTop, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);
            g.Draw(sprPixel, new Rectangle((int)(DrawX), (int)(DrawY + sprFrameTop.Height * Ratio), (int)(sprFrameTop.Width * Ratio), BoxHeight), ColorBox);
            g.Draw(sprFrameTop, new Vector2(DrawX, DrawY + sprFrameTop.Height * Ratio + BoxHeight), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.FlipVertically, 0.9f);

            DrawX = (int)(250 * Ratio);
            DrawY = (int)(470 * Ratio);

            for (int B = 0; B < CurrentContainer.ListBook.Count; ++B)
            {
                g.DrawString(fntOxanimumRegular, CurrentContainer.ListBook[B].BookName, new Vector2(DrawX, DrawY + EntryHeight / 2 - fntOxanimumRegular.LineSpacing / 2), ColorText);
                DrawY += EntryHeight + 10;
            }

            g.DrawString(fntOxanimumRegular, "New", new Vector2(DrawX, DrawY + EntryHeight / 2 - fntOxanimumRegular.LineSpacing / 2), ColorText);
            DrawY += EntryHeight + 10;
            g.DrawString(fntOxanimumRegular, "Return", new Vector2(DrawX, DrawY + EntryHeight / 2 - fntOxanimumRegular.LineSpacing / 2), ColorText);

            DrawX = (int)(40 * Ratio);
            DrawY = (int)(450 * Ratio);
            MenuHelper.DrawFingerIcon(g, new Vector2(DrawX, DrawY + EntryHeight / 3 + CursorIndex * (EntryHeight + 10)));

            SorcererStreetInventoryScreen.DrawBookInformation(g, sprExtraFrame, fntMenuText, "Book Information", Symbols, Icons, ActivePlayer.Inventory.GlobalBook);

            DrawX = (int)(212 * Ratio);
            DrawY = (int)(2008 * Ratio);
            g.DrawString(fntOxanimumRegular, "Edit this Book's contents", new Vector2(DrawX, DrawY), ColorText);
        }
    }
}
