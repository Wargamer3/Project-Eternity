using System;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ShopConfirmBuyCharacterScreen : GameScreen
    {
        private TextButton BuyButton;
        private TextButton CancelButton;

        private IUIElement[] ArrayUIElement;

        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;
        private SpriteFont fntFinlanderFont;
        private SpriteFont fntOxanimumBold;
        private SpriteFont fntOxanimumBoldBig;
        private SpriteFont fntOxanimumBoldBigger;
        private SpriteFont fntOxanimumLightBigger;

        private Texture2D sprFrameBuy;

        private Texture2D sprLand;
        private Texture2D sprSea;
        private Texture2D sprSky;
        private Texture2D sprSpace;

        private SorcererStreetInventory Inventory;
        private UnlockablePlayerCharacter CharacterToBuy;

        private double AnimationProgression;

        int DrawX;
        int DrawY;

        public ShopConfirmBuyCharacterScreen(SorcererStreetInventory Inventory, UnlockablePlayerCharacter CharacterToBuy)
        {
            this.Inventory = Inventory;
            this.CharacterToBuy = CharacterToBuy;

        }

        public override void Load()
        {
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            fntFinlanderFont = Content.Load<SpriteFont>("Fonts/Finlander Font");
            fntOxanimumBold = Content.Load<SpriteFont>("Fonts/Oxanium Bold");
            fntOxanimumBoldBig = Content.Load<SpriteFont>("Fonts/Oxanium Bold Big");
            fntOxanimumBoldBigger = Content.Load<SpriteFont>("Fonts/Oxanium Bold Bigger");
            fntOxanimumLightBigger = Content.Load<SpriteFont>("Fonts/Oxanium Light Bigger");

            sprFrameBuy = Content.Load<Texture2D>("Menus/Lobby Menu/Shop/Frame Confirm Buy");

            sprLand = Content.Load<Texture2D>("Deathmatch/Status Screen/Ground");
            sprSea = Content.Load<Texture2D>("Deathmatch/Status Screen/Sea");
            sprSky = Content.Load<Texture2D>("Deathmatch/Status Screen/Sky");
            sprSpace = Content.Load<Texture2D>("Deathmatch/Status Screen/Space");

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            float Ratio = Constants.Height / 2160f;
            int DrawX = (int)(Constants.Width / 2 - 300 * Ratio);
            int DrawY = Constants.Height / 2 - (int)(sprFrameBuy.Height * Ratio / 2) + (int)(750 * Ratio);
            BuyButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Bigger}{Centered}{Color:65,70,65,255}Buy}}", "Menus/Lobby/Button Color", new Vector2(DrawX, DrawY), 4, 1, Ratio, OnButtonOver, BuyUnit);

            DrawX = (int)(Constants.Width / 2 + 300 * Ratio);
            CancelButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Bigger}{Centered}{Color:243, 243, 243, 255}Cancel}}", "Menus/Lobby/Button Close", new Vector2(DrawX, DrawY), 4, 1, Ratio, OnButtonOver, Cancel);

            DrawX = Constants.Width / 2 - sprFrameBuy.Width / 2;
            DrawY = sprFrameBuy.Height / 2;

            ArrayUIElement = new IUIElement[]
            {
                BuyButton, CancelButton,
            };
        }

        public override void Update(GameTime gameTime)
        {
            AnimationProgression += gameTime.ElapsedGameTime.TotalSeconds * 2d;
            foreach (IUIElement ActiveButton in ArrayUIElement)
            {
                ActiveButton.Update(gameTime);
            }
        }

        private void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        private void BuyUnit()
        {
            sndButtonClick.Play();
            Inventory.DicOwnedCharacter.Add(CharacterToBuy.CharacterToBuy.CharacterPath, new PlayerCharacterInfo(CharacterToBuy.CharacterToBuy));

            RemoveScreen(this);
        }


        private void Cancel()
        {
            sndButtonClick.Play();
            RemoveScreen(this);
        }


        public override void Draw(CustomSpriteBatch g)
        {
            float Ratio = Constants.Height / 2160f;
            Color HeaderColor = Color.FromNonPremultiplied(243, 243, 243, 255);
            Color TextColor = Color.FromNonPremultiplied(74, 79, 74, 255);

            int DrawX = Constants.Width / 2 - (int)(sprFrameBuy.Width * Ratio / 2);
            int DrawY = Constants.Height / 2 - (int)(sprFrameBuy.Height * Ratio / 2);

            g.Draw(sprPixel, new Rectangle(0, 0, Constants.Width, Constants.Height), Color.FromNonPremultiplied(255, 255, 255, 50));

            g.Draw(sprFrameBuy, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);

            g.DrawStringCentered(fntOxanimumBoldBigger, "BUY A UNIT", new Vector2(Constants.Width / 2, DrawY + 120 * Ratio), Color.White);


            g.DrawStringCentered(fntOxanimumLightBigger, "Do you want to buy", new Vector2(Constants.Width / 2, DrawY + 300 * Ratio), TextColor);
            g.DrawStringCentered(fntOxanimumBold, CharacterToBuy.CharacterToBuy.Name.ToUpper(), new Vector2(Constants.Width / 2, DrawY + 435 * Ratio), TextColor);

            int TextWidth = (int)(fntOxanimumLightBigger.MeasureString("For ").X + fntOxanimumBold.MeasureString(CharacterToBuy.CharacterToBuy.Price + " cr ?").X);

            g.DrawString(fntOxanimumLightBigger, "For ", new Vector2(Constants.Width / 2 - TextWidth / 2, DrawY + 570 * Ratio), TextColor);
            g.DrawString(fntOxanimumBold, CharacterToBuy.CharacterToBuy.Price + " cr ?", new Vector2(Constants.Width / 2 - TextWidth / 2 + fntOxanimumLightBigger.MeasureString("For ").X, DrawY + 560 * Ratio), TextColor);

            foreach (IUIElement ActiveButton in ArrayUIElement)
            {
                ActiveButton.Draw(g);
            }
        }
    }
}
