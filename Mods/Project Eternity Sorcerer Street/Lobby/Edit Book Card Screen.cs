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

        private SpriteFont fntMenuText;
        private SpriteFont fntMenuTextBigger;
        private SpriteFont fntArial26;

        private CardSymbols Symbols;

        #endregion

        private readonly Player ActivePlayer;
        private readonly CardBook ActiveBook;
        private Card ActiveCard;
        private Card GlobalBookActiveCard;
        private AnimatedModel Map3DModel;

        int OriginalCardQuantityOwned;
        int OriginalTotalCardQuantityOwned;

        public EditBookCardScreen(Player ActivePlayer, CardBook ActiveBook, Card ActiveCard)
        {
            this.ActivePlayer = ActivePlayer;
            this.ActiveBook = CardBook.LoadGlobalBook();
            InitCard(ActiveCard);
        }

        public override void Load()
        {
            fntMenuText = Content.Load<SpriteFont>("Fonts/Arial16");
            fntMenuTextBigger = Content.Load<SpriteFont>("Fonts/Arial18");
            fntArial26 = Content.Load<SpriteFont>("Fonts/Arial26");

            Symbols = CardSymbols.Symbols;
        }

        private void InitCard(Card ActiveCard)
        {
            this.ActiveCard = ActiveCard;
            GlobalBookActiveCard = ActiveBook.DicCardsByType[ActiveCard.CardType][ActiveCard.Path];
            //GlobalBookActiveCard = ActiveCard;
            OriginalCardQuantityOwned = ActiveCard.QuantityOwned;
            OriginalTotalCardQuantityOwned = ActiveBook.TotalCards;

            if (ActiveCard is CreatureCard)
            {
                Map3DModel = ((CreatureCard)ActiveCard).GamePiece.Unit3DModel;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (InputHelper.InputRightPressed() && ActiveCard.QuantityOwned  < GlobalBookActiveCard.QuantityOwned)
            {
                ++ActiveCard.QuantityOwned;
                ++ActiveBook.TotalCards;
            }
            else if (InputHelper.InputLeftPressed() && ActiveCard.QuantityOwned - 1 > 0)
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
            else if (InputHelper.InputLeftPressed())
            {
                int CardIndex = ActiveBook.ListCard.IndexOf(ActiveCard) - 1;

                if (CardIndex < 0)
                {
                    CardIndex = ActiveBook.ListCard.Count - 1;
                }

                InitCard(ActiveBook.ListCard[CardIndex]);
            }
            else if (InputHelper.InputRightPressed())
            {
                int CardIndex = ActiveBook.ListCard.IndexOf(ActiveCard) + 1;

                if (CardIndex >= ActiveBook.ListCard.Count)
                {
                    CardIndex = 0;
                }

                InitCard(ActiveBook.ListCard[CardIndex]);
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
            Y += HeaderHeight / 2 - fntMenuText.LineSpacing / 2;
            g.DrawString(fntMenuText, "Book Edit", new Vector2(X, Y), Color.White);
            g.DrawStringMiddleAligned(fntMenuText, ActivePlayer.Name + "/" + ActiveBook.BookName, new Vector2(Constants.Width / 2, Y), Color.White);
            X = Constants.Width - Constants.Width / 8;
            g.DrawStringRightAligned(fntMenuText, ActiveBook.TotalCards + " card(s)", new Vector2(X, Y), Color.White);
            g.DrawString(fntMenuText, "OK", new Vector2(X + 20, Y), Color.White);

            g.Draw(ActiveCard.sprCard, new Vector2(Constants.Width / 4, Constants.Height / 2), null, Color.White, 0f, new Vector2(ActiveCard.sprCard.Width / 2, ActiveCard.sprCard.Height / 2), 0.8f, SpriteEffects.None, 0f);
            g.DrawStringCentered(fntArial26, "x" + ActiveCard.QuantityOwned, new Vector2(Constants.Width / 2, Constants.Height - Constants.Height / 16 - HeaderHeight - 30), Color.White);
            ActiveCard.DrawCardInfo(g, Symbols, fntMenuTextBigger, ActivePlayer, 0,  70);

            X = -10;
            Y = Constants.Height - Constants.Height / 16 - HeaderHeight;
            DrawBox(g, new Vector2(X, Y), Constants.Width + 20, HeaderHeight, Color.White);
            X = Constants.Width / 18;
            Y += HeaderHeight / 2 - fntMenuText.LineSpacing / 2;
            g.DrawString(fntMenuText, GlobalBookActiveCard.QuantityOwned + " card(s) in possession"
                + " [Arrows] Adjust Card Count"
                + " [Q] Toggle Info"
                + " [X] Confirm Card Count"
                + " [Z] Return", new Vector2(X, Y), Color.White);

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
