using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class EditBookCardScreen : GameScreen
    {
        #region Ressources

        private SpriteFont fntMenuTextBigger;
        private SpriteFont fntArial26;
        private SpriteFont fntOxanimumBoldTitle;
        private SpriteFont fntOxanimumRegular;

        private CardSymbols Symbols;

        #endregion

        private readonly Player ActivePlayer;
        private readonly CardBook ActiveBook;
        private readonly CardBook PlayerGlobalBook;
        private CardInfo ActiveCard;
        private CardInfo GlobalBookActiveCard;
        private AnimatedModel Map3DModel;

        byte OriginalCardQuantityOwned;
        int OriginalTotalCardQuantityOwned;

        public EditBookCardScreen(Player ActivePlayer, CardBook ActiveBook, CardInfo ActiveCard)
        {
            RequireFocus = true;
            RequireDrawFocus = true;
            this.ActivePlayer = ActivePlayer;
            this.ActiveBook = ActiveBook;
            PlayerGlobalBook = ActivePlayer.Inventory.GlobalBook;
            InitCard(ActiveCard);
        }

        public override void Load()
        {
            fntMenuTextBigger = Content.Load<SpriteFont>("Fonts/Arial18");
            fntArial26 = Content.Load<SpriteFont>("Fonts/Arial26");
            fntOxanimumRegular = Content.Load<SpriteFont>("Fonts/Oxanium Regular");
            fntOxanimumBoldTitle = GameScreen.ContentFallback.Load<SpriteFont>("Fonts/Oxanium Bold Title");

            Symbols = CardSymbols.Symbols;
        }

        private void InitCard(CardInfo ActiveCard)
        {
            this.ActiveCard = ActiveCard;
            GlobalBookActiveCard = PlayerGlobalBook.DicCardsByType[ActiveCard.Card.CardType][ActiveCard.Card.Path];
            OriginalCardQuantityOwned = ActiveCard.QuantityOwned;
            OriginalTotalCardQuantityOwned = ActiveBook.TotalCards;

            if (ActiveCard.Card is CreatureCard)
            {
                Map3DModel = ((CreatureCard)ActiveCard.Card).GamePiece.Unit3DModel;
            }
        }

        public override void Update(GameTime gameTime)
        {
            SorcererStreetInventoryScreen.CubeBackground.Update(gameTime);

            if (InputHelper.InputLeftPressed() && ActiveCard.QuantityOwned  < GlobalBookActiveCard.QuantityOwned)
            {
                ++ActiveCard.QuantityOwned;
                ++ActiveBook.TotalCards;
            }
            else if (InputHelper.InputRightHold() && ActiveCard.QuantityOwned - 1 > 0)
            {
                --ActiveCard.QuantityOwned;
                --ActiveBook.TotalCards;
            }
            else if (InputHelper.InputConfirmPressed())
            {
                RemoveScreen(this);
            }
            else if (InputHelper.InputCancelPressed())
            {
                ActiveCard.QuantityOwned = OriginalCardQuantityOwned;
                ActiveBook.TotalCards = OriginalTotalCardQuantityOwned;

                RemoveScreen(this);
            }
            else if (InputHelper.InputCommand1Pressed())
            {
                int CardIndex = ActiveBook.ListCard.IndexOf(ActiveCard) - 1;

                if (CardIndex < 0)
                {
                    CardIndex = ActiveBook.ListCard.Count - 1;
                }

                InitCard(ActiveBook.ListCard[CardIndex]);
            }
            else if (InputHelper.InputCommand2Pressed())
            {
                int CardIndex = ActiveBook.ListCard.IndexOf(ActiveCard) + 1;

                if (CardIndex >= ActiveBook.ListCard.Count)
                {
                    CardIndex = 0;
                }

                InitCard(ActiveBook.ListCard[CardIndex]);
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

            SorcererStreetInventoryScreen.CubeBackground.Draw(g);

            float DrawX = (int)(210 * Ratio);
            float DrawY = (int)(58 * Ratio);
            g.DrawString(fntOxanimumBoldTitle, "BOOK EDIT", new Vector2(DrawX, DrawY), ColorText);

            g.Draw(ActiveCard.Card.sprCard, new Vector2(Constants.Width / 4, Constants.Height / 2), null, Color.White, 0f, new Vector2(ActiveCard.Card.sprCard.Width / 2, ActiveCard.Card.sprCard.Height / 2), 0.8f, SpriteEffects.None, 0f);
            g.DrawStringCentered(fntArial26, "x" + ActiveCard.QuantityOwned, new Vector2(Constants.Width / 2, (int)(1800 * Ratio)), ColorText);
            ActiveCard.Card.DrawCardInfo(g, Symbols, fntMenuTextBigger, ActivePlayer, 0, (int)(300 * Ratio));

            DrawX = (int)(212 * Ratio);
            DrawY = (int)(2008 * Ratio);
            g.DrawString(fntOxanimumRegular, GlobalBookActiveCard.QuantityOwned + " card(s) in possession"
                + " [Arrows] Adjust Card Count"
                + " [Q] Toggle Info"
                + " [X] Confirm Card Count"
                + " [Z] Return", new Vector2(DrawX, DrawY), ColorText);

            g.End();
            g.Begin();
            DrawModel(Map3DModel, Matrix.CreateRotationX(MathHelper.ToRadians(180))
                * Matrix.CreateScale(2f) * Matrix.CreateTranslation(Constants.Width / 2, Constants.Height / 2 + 140, 0), Matrix.Identity);
        }

        private void DrawModel(AnimatedModel Model, Matrix World, Matrix View)
        {
            Matrix Projection = Matrix.CreateOrthographicOffCenter(0, Constants.Width, Constants.Height, 0, 300, -300);
            Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            Projection = HalfPixelOffset * Projection;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            GameScreen.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GameScreen.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GameScreen.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.White, 1f, 0);

            Model.Draw(View, Projection, World);
        }
    }
}
