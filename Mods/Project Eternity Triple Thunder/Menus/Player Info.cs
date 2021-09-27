using System;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.TripleThunderScreen.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class PlayerInfo : GameScreen
    {
        #region Ressources

        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;
        private Texture2D sprBackground;
        private Texture2D sprOverlay;
        private Texture2D sprRankingChoices;
        private Texture2D sprLicenseChoices;

        private InteractiveButton AddAsFriendButton;
        private InteractiveButton PlayTogetherButton;
        private InteractiveButton RecordButton;

        private InteractiveButton CancelButton;
        private InteractiveButton OKButton;

        private InteractiveButton[] ArrayMenuButton;

        #endregion

        private readonly TripleThunderOnlineClient OnlineGameClient;
        private readonly CommunicationClient OnlineCommunicationClient;

        public PlayerInfo(TripleThunderOnlineClient OnlineClient, CommunicationClient OnlineCommunicationClient, string PlayerName)
        {
            this.OnlineGameClient = OnlineClient;
            this.OnlineCommunicationClient = OnlineCommunicationClient;
        }

        public override void Load()
        {
            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            sprBackground = Content.Load<Texture2D>("Triple Thunder/Menus/Player Info/Background");
            sprOverlay = Content.Load<Texture2D>("Triple Thunder/Menus/Player Info/Overlay");

            sprRankingChoices = Content.Load<Texture2D>("Triple Thunder/Menus/Player Info/Ranking");
            sprLicenseChoices = Content.Load<Texture2D>("Triple Thunder/Menus/Player Info/License");

            AddAsFriendButton = new InteractiveButton(Content, "Triple Thunder/Menus/Player Info/Add As Friend", new Vector2(533, 242), OnButtonOver, SelectCaptureTheFlag);
            PlayTogetherButton = new InteractiveButton(Content, "Triple Thunder/Menus/Player Info/Play Together", new Vector2(533, 242), OnButtonOver, SelectCaptureTheFlag);
            RecordButton = new InteractiveButton(Content, "Triple Thunder/Menus/Player Info/Record", new Vector2(533, 242), OnButtonOver, SelectCaptureTheFlag);

            CancelButton = new InteractiveButton(Content, "Triple Thunder/Menus/Common/Cancel Button", new Vector2(490, 481), OnButtonOver, Cancel);
            OKButton = new InteractiveButton(Content, "Triple Thunder/Menus/Common/OK Button", new Vector2(565, 481), OnButtonOver, CreateRoom);

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");

            ArrayMenuButton = new InteractiveButton[]
            {
                AddAsFriendButton, PlayTogetherButton, RecordButton,
                CancelButton, OKButton,
            };
        }

        public override void Unload()
        {
            SoundSystem.ReleaseSound(sndButtonOver);
            SoundSystem.ReleaseSound(sndButtonClick);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (InteractiveButton ActiveButton in ArrayMenuButton)
            {
                ActiveButton.Update(gameTime);
            }
        }

        #region Button methods

        private void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        private void SelectCaptureTheFlag()
        {
        }

        public void CreateRoom()
        {
            sndButtonClick.Play();
            OKButton.Disable();
        }

        public void Cancel()
        {
            sndButtonClick.Play();
            RemoveScreen(this);
        }

        #endregion

        public override void Draw(CustomSpriteBatch g)
        {
            g.Draw(sprBackground, new Vector2(Constants.Width / 2, Constants.Height / 2), null, Color.White, 0f, new Vector2(sprBackground.Width / 2, sprBackground.Height / 2), 1f, SpriteEffects.None, 0f);

            foreach (InteractiveButton ActiveButton in ArrayMenuButton)
            {
                ActiveButton.Draw(g);
            }
        }
    }
}
