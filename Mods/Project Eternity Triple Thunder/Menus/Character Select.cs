using System;
using System.Collections.Generic;
using System.IO;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class CharacterSelect : GameScreen
    {
        private class CharacterInfo
        {
            public Texture2D sprCharacterImage;
            public Texture2D sprCharacterPortrait;

            public string Name;

            public CharacterInfo(string Name, Texture2D sprCharacterImage, Texture2D sprCharacterPortrait)
            {
                this.sprCharacterImage = sprCharacterImage;
                this.sprCharacterPortrait = sprCharacterPortrait;
                this.Name = Name;
            }
        }

        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntText;
        private Texture2D sprBackground;
        private AnimatedSprite CharacterBackground;
        private InteractiveButton CancelButton;
        private InteractiveButton OKButton;

        private readonly List<CharacterInfo> ListCharacterInfo;
        private CharacterInfo SelectedCharacterInfo;
        private readonly Player ActivePlayer;
        private readonly InteractiveButton.OnClick OnConfirm;

        private const int CharacterPixelOffset = 48;

        public CharacterSelect(Player ActivePlayer, InteractiveButton.OnClick OnConfirm)
        {
            this.ActivePlayer = ActivePlayer;
            this.OnConfirm = OnConfirm;
            ListCharacterInfo = new List<CharacterInfo>();
        }

        public override void Load()
        {
            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            fntText = Content.Load<SpriteFont>("Fonts/Arial10");

            sprBackground = Content.Load<Texture2D>("Triple Thunder/Menus/Character Select/Background");

            CharacterBackground = new AnimatedSprite(Content, "Triple Thunder/Menus/Character Select/Character Background", Vector2.Zero, 0, 1, 4);

            CancelButton = new InteractiveButton(Content, "Triple Thunder/Menus/Common/Cancel Button", new Vector2(508, 510), OnButtonOver, Cancel);
            OKButton = new InteractiveButton(Content, "Triple Thunder/Menus/Common/OK Button", new Vector2(590, 510), OnButtonOver, Confirm);

            foreach (string ActiveCharacterPath in Directory.EnumerateFiles("Content/Units/Triple Thunder/Players"))
            {
                string ActiveCharacter = ActiveCharacterPath.Remove(0, 37);
                ActiveCharacter = ActiveCharacter.Remove(ActiveCharacter.Length - 4);

                CharacterInfo NewCharacterInfo = new CharacterInfo(ActiveCharacter,
                    Content.Load<Texture2D>("Triple Thunder/Menus/Character Select/" + ActiveCharacter),
                    Content.Load<Texture2D>("Triple Thunder/Menus/Character Select/" + ActiveCharacter + " Portrait"));

                ListCharacterInfo.Add(NewCharacterInfo);

                if (ActivePlayer.Equipment.CharacterType == ActiveCharacter)
                {
                    SelectedCharacterInfo = NewCharacterInfo;
                }
            }
        }

        public override void Unload()
        {
            SoundSystem.ReleaseSound(sndButtonOver);
            SoundSystem.ReleaseSound(sndButtonClick);
        }

        public override void Update(GameTime gameTime)
        {
            CancelButton.Update(gameTime);
            OKButton.Update(gameTime);

            if (MouseHelper.InputLeftButtonPressed() && MouseHelper.MouseStateCurrent.X >= 425 && MouseHelper.MouseStateCurrent.X <= 428 + 182 && MouseHelper.MouseStateCurrent.Y <= 480)
            {
                int SelectIndex = (MouseHelper.MouseStateCurrent.Y - 97) / CharacterPixelOffset;
                if (SelectIndex >= 0 && SelectIndex < ListCharacterInfo.Count)
                {
                    SelectedCharacterInfo = ListCharacterInfo[SelectIndex];
                    ActivePlayer.Equipment.CharacterType = SelectedCharacterInfo.Name;
                }
            }
        }

        private void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        public void Confirm()
        {
            sndButtonClick.Play();
            RemoveScreen(this);
            OnConfirm();
        }

        public void Cancel()
        {
            sndButtonClick.Play();
            RemoveScreen(this);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.Draw(sprBackground, new Vector2(Constants.Width / 2, Constants.Height /2), null, Color.White, 0f, new Vector2(sprBackground.Width / 2, sprBackground.Height / 2), 1f, SpriteEffects.None, 0f);

            g.Draw(SelectedCharacterInfo.sprCharacterImage, new Vector2(171, 101), Color.White);

            for (int C = 0; C < ListCharacterInfo.Count; ++C)
            {
                CharacterBackground.Draw(g, new Vector2(512, 122 + C * CharacterPixelOffset), Color.White);
                g.Draw(ListCharacterInfo[C].sprCharacterPortrait, new Vector2(545, 97 + C * CharacterPixelOffset), Color.White);
                g.DrawString(fntText, ListCharacterInfo[C].Name, new Vector2(440, 110 + C * CharacterPixelOffset), Color.White);
            }

            CancelButton.Draw(g);
            OKButton.Draw(g);
        }
    }
}
