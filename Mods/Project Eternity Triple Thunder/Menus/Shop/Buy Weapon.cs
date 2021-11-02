using System;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class BuyWeapon : GameScreen
    {
        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntText;

        private Texture2D sprBackground;
        private Texture2D sprHeader;
        private InteractiveButton BuyButton;
        private InteractiveButton CancelButton;

        private readonly WeaponMenuEquipment EquipmentToBuy;
        private readonly Player Owner;
        private readonly PlayerInventory PlayerInventory;

        public BuyWeapon(WeaponMenuEquipment EquipmentToBuy, Player Owner)
        {
            this.EquipmentToBuy = EquipmentToBuy;
            this.Owner = Owner;
            PlayerInventory = Owner.Equipment;
        }

        public override void Load()
        {
            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            fntText = Content.Load<SpriteFont>("Fonts/Arial10");

            sprBackground = Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Buy Weapon Background");
            sprHeader = Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Buy Weapon Text");

            BuyButton = new InteractiveButton(Content, "Triple Thunder/Menus/Shop/Buy Button", new Vector2(568, 462), OnButtonOver, Buy);
            CancelButton = new InteractiveButton(Content, "Triple Thunder/Menus/Common/Cancel Button", new Vector2(488, 462), OnButtonOver, Cancel);
        }

        public override void Unload()
        {
            SoundSystem.ReleaseSound(sndButtonOver);
            SoundSystem.ReleaseSound(sndButtonClick);
        }

        public override void Update(GameTime gameTime)
        {
            BuyButton.Update(gameTime);
            CancelButton.Update(gameTime);
        }

        private void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        private void Buy()
        {
            sndButtonClick.Play();
            PlayerInventory.ListWeapon.Add(EquipmentToBuy);
            Owner.SaveLocally();
            RemoveScreen(this);
        }

        private void Cancel()
        {
            sndButtonClick.Play();
            RemoveScreen(this);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.Draw(sprBackground, new Vector2(Constants.Width / 2, Constants.Height / 2), null, Color.White, 0f, new Vector2(sprBackground.Width / 2, sprBackground.Height / 2), 1f, SpriteEffects.None, 0f);
            g.Draw(sprHeader, new Vector2(196, 122), Color.White);
            g.Draw(EquipmentToBuy.sprFull, new Vector2(223, 175), Color.White);
            g.DrawString(fntText, EquipmentToBuy.Category, new Vector2(227, 288), Color.White);
            g.DrawString(fntText, EquipmentToBuy.Name, new Vector2(227, 311), Color.White);
            g.DrawStringRightAligned(fntText, EquipmentToBuy.Price + "CR", new Vector2(577, 311), Color.White);
            g.DrawString(fntText, "Lv." + EquipmentToBuy.MinLevel, new Vector2(385, 336), Color.White);

            BuyButton.Draw(g);
            CancelButton.Draw(g);
        }
    }
}
