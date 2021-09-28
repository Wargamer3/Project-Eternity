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

        private InteractiveButton GeneralButton;
        private InteractiveButton RecordButton;
        private InteractiveButton AddAsFriendButton;
        private InteractiveButton PlayTogetherButton;

        private InteractiveButton CloseButton;

        private InteractiveButton[] ArrayMenuButton;

        #endregion

        private readonly TripleThunderOnlineClient OnlineGameClient;
        private readonly CommunicationClient OnlineCommunicationClient;
        public readonly Player ActivePlayer;

        public PlayerInfo(TripleThunderOnlineClient OnlineClient, CommunicationClient OnlineCommunicationClient, Player ActivePlayer)
        {
            this.OnlineGameClient = OnlineClient;
            this.OnlineCommunicationClient = OnlineCommunicationClient;
            this.ActivePlayer = ActivePlayer;
        }

        public override void Load()
        {
            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            sprBackground = Content.Load<Texture2D>("Triple Thunder/Menus/Player Info/Background");
            sprOverlay = Content.Load<Texture2D>("Triple Thunder/Menus/Player Info/Overlay");

            sprRankingChoices = Content.Load<Texture2D>("Triple Thunder/Menus/Player Info/Ranking");
            sprLicenseChoices = Content.Load<Texture2D>("Triple Thunder/Menus/Player Info/License");

            GeneralButton = new InteractiveButton(Content, "Triple Thunder/Menus/Player Info/General", new Vector2(350, 152), OnButtonOver, SelectGeneralButton);
            RecordButton = new InteractiveButton(Content, "Triple Thunder/Menus/Player Info/Record", new Vector2(450, 152), OnButtonOver, SelectRecordButton);
            AddAsFriendButton = new InteractiveButton(Content, "Triple Thunder/Menus/Player Info/Add As Friend", new Vector2(350, 440), OnButtonOver, SelectAddAsFriendButton);
            PlayTogetherButton = new InteractiveButton(Content, "Triple Thunder/Menus/Player Info/Play Together", new Vector2(450, 440), OnButtonOver, SelectPlayTogetherButton);

            CloseButton = new InteractiveButton(Content, "Triple Thunder/Menus/Common/Close Button", new Vector2(478, 495), OnButtonOver, Close);

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");

            ArrayMenuButton = new InteractiveButton[]
            {
                GeneralButton, RecordButton,
                AddAsFriendButton, PlayTogetherButton, 
                CloseButton,
            };

            if (ActivePlayer.Ranking == 0)
            {
                Dictionary<string, OnlineScript> DicOnlineCommunicationClientScripts = new Dictionary<string, OnlineScript>();
                DicOnlineCommunicationClientScripts.Add(ClientInfoScriptClient.ScriptName, new ClientInfoScriptClient(OnlineCommunicationClient, this));
                OnlineCommunicationClient.Host.AddOrReplaceScripts(DicOnlineCommunicationClientScripts);

                OnlineCommunicationClient.Host.Send(new AskClientInfoScriptClient(ActivePlayer.ConnectionID));
            }
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

        private void SelectGeneralButton()
        {
        }

        private void SelectRecordButton()
        {
        }

        private void SelectAddAsFriendButton()
        {
            OnlineCommunicationClient.AddFriend(ActivePlayer.ConnectionID);
        }

        private void SelectPlayTogetherButton()
        {
        }

        public void Close()
        {
            sndButtonClick.Play();
            RemoveScreen(this);
        }

        #endregion

        public override void Draw(CustomSpriteBatch g)
        {
            int X = Constants.Width / 2 - sprBackground.Width / 2;
            int Y = Constants.Height / 2 - sprBackground.Height / 2;
            g.Draw(sprBackground, new Vector2(X, Y), Color.White);
            g.Draw(sprOverlay, new Vector2(X + 25, Y + 92), Color.White);

            g.DrawString(fntArial12, ActivePlayer.Name, new Vector2(X + 85, Y + 100), Color.White);
            g.DrawString(fntArial12, ActivePlayer.Guild, new Vector2(X + 85, Y + 128), Color.White);
            g.DrawString(fntArial12, ActivePlayer.Level.ToString(), new Vector2(X + 85, Y + 158), Color.White);

            g.Draw(sprLicenseChoices, new Vector2(X + 60, Y + 215), new Rectangle(0, 0, sprLicenseChoices.Width, sprLicenseChoices.Height / 3), Color.White);
            g.Draw(sprRankingChoices, new Vector2(X + 150, Y + 215), new Rectangle(0, 0, sprRankingChoices.Width, sprRankingChoices.Height / 5), Color.White);
            
            foreach (InteractiveButton ActiveButton in ArrayMenuButton)
            {
                ActiveButton.Draw(g);
            }
        }
    }
}
