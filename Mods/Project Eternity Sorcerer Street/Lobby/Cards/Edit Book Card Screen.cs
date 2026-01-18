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
        private AnimatedModelTransparent Map3DModel;

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
                Map3DModel = new AnimatedModelTransparent(((CreatureCard)ActiveCard.Card).GamePiece.Unit3DModel);
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
            else if (KeyboardHelper.KeyReleased(Microsoft.Xna.Framework.Input.Keys.E))
            {
                PushScreen(new EditBookCardSkinScreen(ActivePlayer, ActiveCard));
            }
            else if (KeyboardHelper.KeyReleased(Microsoft.Xna.Framework.Input.Keys.A) && (ActiveCard.ListOwnedCardSkin.Count > 0 || ActiveCard.ListOwnedCardAlt.Count > 0))
            {
                ActiveCard.SelectedSkinIndex++;

                if (ActiveCard.SelectedSkinIndex < ActiveCard.ListOwnedCardSkin.Count)
                {
                    ActiveCard.CardSkin = ActiveCard.ListOwnedCardSkin[ActiveCard.SelectedSkinIndex].CardSkin;
                }
                else if (ActiveCard.SelectedSkinIndex < ActiveCard.ListOwnedCardSkin.Count + ActiveCard.ListOwnedCardAlt.Count)
                {
                    ActiveCard.CardSkin = ActiveCard.ListOwnedCardAlt[ActiveCard.SelectedSkinIndex - ActiveCard.ListOwnedCardSkin.Count].CardSkin;
                }
                else
                {
                    ActiveCard.SelectedSkinIndex = -1;
                    ActiveCard.CardSkin = ActiveCard.Card;
                }
            }
            else if (KeyboardHelper.KeyReleased(Microsoft.Xna.Framework.Input.Keys.D) && (ActiveCard.ListOwnedCardSkin.Count > 0 || ActiveCard.ListOwnedCardAlt.Count > 0))
            {
                ActiveCard.SelectedSkinIndex--;

                if (ActiveCard.SelectedSkinIndex < ActiveCard.ListOwnedCardSkin.Count)
                {
                    ActiveCard.CardSkin = ActiveCard.ListOwnedCardSkin[ActiveCard.SelectedSkinIndex].CardSkin;
                }
                else if (ActiveCard.SelectedSkinIndex < ActiveCard.ListOwnedCardSkin.Count + ActiveCard.ListOwnedCardAlt.Count)
                {
                    ActiveCard.CardSkin = ActiveCard.ListOwnedCardAlt[ActiveCard.SelectedSkinIndex - ActiveCard.ListOwnedCardSkin.Count].CardSkin;
                }
                else
                {
                    ActiveCard.SelectedSkinIndex = ActiveCard.ListOwnedCardSkin.Count + ActiveCard.ListOwnedCardAlt.Count - 1;
                    ActiveCard.CardSkin = ActiveCard.Card;
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

            g.Draw(ActiveCard.CardSkin.sprCard, new Vector2(Constants.Width / 4, Constants.Height / 2), null, Color.White, 0f, new Vector2(ActiveCard.CardSkin.sprCard.Width / 2, ActiveCard.CardSkin.sprCard.Height / 2), 0.8f, SpriteEffects.None, 0f);
            g.DrawStringCentered(fntArial26, "x" + ActiveCard.QuantityOwned, new Vector2(Constants.Width / 2, (int)(1800 * Ratio)), ColorText);
            ActiveCard.CardSkin.DrawCardInfo(g, Symbols, fntMenuTextBigger, ActivePlayer, 0, (int)(300 * Ratio));

            DrawX = (int)(212 * Ratio);
            DrawY = (int)(2008 * Ratio);
            g.DrawString(fntOxanimumRegular, GlobalBookActiveCard.QuantityOwned + " card(s) in possession"
                + " [Arrows] Adjust Card Count"
                + " [Q] Toggle Info"
                + " [A-D] Change Skin"
                + " [E] Edit Skins"
                + " [X] Confirm Card Count"
                + " [Z] Return", new Vector2(DrawX, DrawY), ColorText);

            DrawX = Constants.Width / 2;
            DrawY = (int)(1708 * Ratio);

            if (ActiveCard.SelectedSkinIndex < 0)
            {
                g.DrawStringCentered(fntOxanimumRegular, "Current Skin: Default", new Vector2(DrawX, DrawY), ColorText);
            }
            else
            {
                g.DrawStringCentered(fntOxanimumRegular, "Current Skin " + (ActiveCard.SelectedSkinIndex + 1) + ":" + ActiveCard.CardSkin.Name, new Vector2(DrawX, DrawY), ColorText);
            }

            g.End();
            g.Begin();

            var World = Matrix.CreateRotationX(MathHelper.ToRadians(180)) * Matrix.CreateRotationY(MathHelper.ToRadians(-45)) * Matrix.CreateRotationX(MathHelper.ToRadians(45))
                * Matrix.CreateScale(2f) * Matrix.CreateTranslation(Constants.Width / 2, Constants.Height / 2 + 140, 0);
            float aspectRatio = GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height;
            Matrix Projection = Matrix.CreateOrthographicOffCenter(0, Constants.Width, Constants.Height, 0, 600, -700);
            Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            Projection = HalfPixelOffset * Projection;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            GameScreen.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GameScreen.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GameScreen.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.White, 1f, 0);

            Map3DModel.Draw3D(GraphicsDevice, Matrix.Identity, Projection, World);
        }
    }
}
