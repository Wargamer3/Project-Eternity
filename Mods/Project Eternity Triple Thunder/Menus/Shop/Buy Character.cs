using System;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class BuyCharacter : GameScreen
    {
        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntText;

        private Texture2D sprBackground;
        private Texture2D sprHeader;
        private InteractiveButton BuyButton;
        private InteractiveButton CancelButton;

        private readonly CharacterMenuEquipment CharacterToBuy;
        private readonly Player Owner;
        private readonly PlayerInventory PlayerInventory;

        public BuyCharacter(CharacterMenuEquipment CharacterToBuy, Player Owner)
        {
            this.CharacterToBuy = CharacterToBuy;
            this.Owner = Owner;
            PlayerInventory = Owner.Equipment;
        }

        public override void Load()
        {
            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            fntText = Content.Load<SpriteFont>("Fonts/Arial10");

            sprBackground = Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Buy Character Background");
            sprHeader = Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Buy Character Text");

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
            PlayerInventory.ListCharacter.Add(CharacterToBuy);
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
            g.Draw(CharacterToBuy.sprFull, new Vector2(215, 172), Color.White);
            g.DrawStringMiddleAligned(fntText, CharacterToBuy.Name, new Vector2(513, 175), Color.White);
            g.DrawString(fntText, "5", new Vector2(473, 199), Color.White);
            g.DrawString(fntText, "6", new Vector2(515, 199), Color.White);
            g.DrawString(fntText, "4", new Vector2(557, 199), Color.White);

            BuyButton.Draw(g);
            CancelButton.Draw(g);
        }
    }
}
