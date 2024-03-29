﻿using System;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.TripleThunderScreen.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class CreateRoomMission : GameScreen
    {
        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;
        private Texture2D sprBackground;
        private InteractiveButton CancelButton;
        private InteractiveButton OKButton;

        private TextInput RoomNameInput;
        private TextInput PasswordInput;

        private readonly TripleThunderOnlineClient OnlineGameClient;
        private readonly CommunicationClient OnlineCommunicationClient;
        private readonly string RoomType;

        private string RoomSubtype;
        private byte MinNumberOfPlayer;
        private byte MaxNumberOfPlayer;

        public CreateRoomMission(TripleThunderOnlineClient OnlineGameClient, CommunicationClient OnlineCommunicationClient, string RoomType)
        {
            this.OnlineGameClient = OnlineGameClient;
            this.OnlineCommunicationClient = OnlineCommunicationClient;
            this.RoomType = RoomType;
            if (RoomType == RoomInformations.RoomTypeBattle)
            {
                RoomSubtype = "Deathmatch";
                MinNumberOfPlayer = 2;
                MaxNumberOfPlayer = 8;
            }
            else
            {
                RoomSubtype = string.Empty;
                MinNumberOfPlayer = 1;
                MaxNumberOfPlayer = 3;
            }
        }

        public override void Load()
        {
            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            sprBackground = Content.Load<Texture2D>("Triple Thunder/Menus/Create Room/Background Mission");

            CancelButton = new InteractiveButton(Content, "Triple Thunder/Menus/Common/Cancel Button", new Vector2(490, 380), OnButtonOver, Cancel);
            OKButton = new InteractiveButton(Content, "Triple Thunder/Menus/Common/OK Button", new Vector2(565, 380), OnButtonOver, CreateMissionSelect);

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            RoomNameInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(270, 255), new Vector2(314, 20));
            PasswordInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(470, 287), new Vector2(84, 20), null, true);

            RoomNameInput.SetText("Let's have fun!");
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
            RoomNameInput.Update(gameTime);
            PasswordInput.Update(gameTime);
        }

        private void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        public void CreateMissionSelect()
        {
            sndButtonClick.Play();
            OKButton.Disable();

            if (OnlineGameClient != null && OnlineGameClient.IsConnected)
            {
                Dictionary<string, OnlineScript> DicCreateRoomScript = new Dictionary<string, OnlineScript>();
                DicCreateRoomScript.Add(SendRoomIDScriptClient.ScriptName, new SendRoomIDScriptClient(OnlineGameClient, OnlineCommunicationClient, this,
                    RoomNameInput.Text, RoomType, RoomSubtype, MinNumberOfPlayer, MaxNumberOfPlayer));
                OnlineGameClient.Host.AddOrReplaceScripts(DicCreateRoomScript);
                OnlineGameClient.CreateRoom(RoomNameInput.Text, RoomType, RoomSubtype, MinNumberOfPlayer, MaxNumberOfPlayer);
            }
            else
            {
                PushScreen(new MissionSelect(null, null, new MissionRoomInformations("No ID needed", RoomNameInput.Text, RoomType, RoomSubtype, MinNumberOfPlayer, MaxNumberOfPlayer)));
                RemoveScreen(this);
            }
        }

        public void Cancel()
        {
            sndButtonClick.Play();
            RemoveScreen(this);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.Draw(sprBackground, new Vector2(Constants.Width / 2, Constants.Height / 2), null, Color.White, 0f, new Vector2(sprBackground.Width / 2, sprBackground.Height / 2), 1f, SpriteEffects.None, 0f);

            CancelButton.Draw(g);
            OKButton.Draw(g);
            RoomNameInput.Draw(g);
            PasswordInput.Draw(g);
        }
    }
}
