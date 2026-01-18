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
        private IconHolder Icons;

        private SpriteFont fntOxanimumBoldTitle;
        private SpriteFont fntOxanimumRegular;
        private SpriteFont fntMenuText;

        private Texture2D sprExtraFrame;
        private Texture2D sprFrameTop;

        #endregion

        private enum MenuChoices { EditBook, ChangeBookCover, EditProfile, Copy, NameChange, ResetBook, Return}

        private readonly Player ActivePlayer;
        private readonly CardBook ActiveBook;

        private int CursorIndex;

        public EditBookScreen(CardSymbols Symbols, IconHolder Icons, Player ActivePlayer, CardBook ActiveBook)
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
            fntMenuText = Content.Load<SpriteFont>("Fonts/Arial12");
            fntOxanimumRegular = Content.Load<SpriteFont>("Fonts/Oxanium Regular");
            fntOxanimumBoldTitle = GameScreen.ContentFallback.Load<SpriteFont>("Fonts/Oxanium Bold Title");

            sprExtraFrame = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Extra Frame 2");
            sprFrameTop = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Room/Frame Top");
        }

        public override void Update(GameTime gameTime)
        {
            SorcererStreetInventoryScreen.CubeBackground.Update(gameTime);

            float Ratio = Constants.Height / 2160f;
            int EntryHeight = (int)(128 * Ratio);
            int DrawX = (int)(250 * Ratio);
            int DrawY = (int)(470 * Ratio) + EntryHeight / 2 - fntOxanimumRegular.LineSpacing / 2;

            if (MouseHelper.MouseMoved() && MouseHelper.MouseStateCurrent.X >= DrawX && MouseHelper.MouseStateCurrent.X - DrawX < (int)(sprFrameTop.Width * Ratio))
            {
                int MouseIndex = (int)Math.Floor((MouseHelper.MouseStateCurrent.Y - (float)DrawY) / EntryHeight);

                if (MouseIndex >= 0 && MouseIndex <= 6)
                {
                    CursorIndex = MouseIndex;
                }
            }

            if (InputHelper.InputConfirmPressed())
            {
                switch ((MenuChoices)CursorIndex)
                {
                    case MenuChoices.EditBook:
                        PushScreen(new EditBookCardListScreen(Symbols, Icons, ActivePlayer, ActiveBook));
                        break;
                    case MenuChoices.ChangeBookCover:
                        break;
                    case MenuChoices.EditProfile:
                        break;
                    case MenuChoices.Copy:
                        break;
                    case MenuChoices.NameChange:
                        PushScreen(new EditBookNameScreen(ActivePlayer, ActiveBook));
                        break;
                    case MenuChoices.ResetBook:
                        break;
                    case MenuChoices.Return:
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

            int EntryHeight = (int)(128 * Ratio);
            int BoxHeight = (int)(994 * Ratio);

            g.Draw(sprFrameTop, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);
            g.Draw(sprPixel, new Rectangle((int)(DrawX), (int)(DrawY + sprFrameTop.Height * Ratio), (int)(sprFrameTop.Width * Ratio), BoxHeight), ColorBox);
            g.Draw(sprFrameTop, new Vector2(DrawX, DrawY + sprFrameTop.Height * Ratio + BoxHeight), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.FlipVertically, 0.9f);

            DrawX = (int)(250 * Ratio);
            DrawY = (int)(470 * Ratio) + EntryHeight / 2 - fntOxanimumRegular.LineSpacing / 2;

            g.DrawString(fntOxanimumRegular, "Edit", new Vector2(DrawX, DrawY), ColorText);
            DrawY += EntryHeight;
            g.DrawString(fntOxanimumRegular, "Change Book Cover", new Vector2(DrawX, DrawY), ColorText);
            DrawY += EntryHeight;
            g.DrawString(fntOxanimumRegular, "Edit Profile", new Vector2(DrawX, DrawY), ColorText);
            DrawY += EntryHeight;
            g.DrawString(fntOxanimumRegular, "Copy", new Vector2(DrawX, DrawY), ColorText);
            DrawY += EntryHeight;
            g.DrawString(fntOxanimumRegular, "Name Change", new Vector2(DrawX, DrawY), ColorText);
            DrawY += EntryHeight;
            g.DrawString(fntOxanimumRegular, "Reset Book", new Vector2(DrawX, DrawY), ColorText);
            DrawY += EntryHeight;
            g.DrawString(fntOxanimumRegular, "Return", new Vector2(DrawX, DrawY), ColorText);

            DrawX = (int)(40 * Ratio);
            DrawY = (int)(490 * Ratio);
            MenuHelper.DrawFingerIcon(g, new Vector2(DrawX, DrawY + CursorIndex * EntryHeight));

            SorcererStreetInventoryScreen.DrawBookInformation(g, sprExtraFrame, fntMenuText, "Book Information", Symbols, Icons, ActivePlayer.Inventory.GlobalBook);

            DrawX = (int)(212 * Ratio);
            DrawY = (int)(2008 * Ratio);
            g.DrawString(fntOxanimumRegular, "Edit this Book's contents", new Vector2(DrawX, DrawY), ColorText);
        }
    }
}
