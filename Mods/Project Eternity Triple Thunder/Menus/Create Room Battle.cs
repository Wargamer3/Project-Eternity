using System;
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
    public class CreateRoomBattle : GameScreen
    {
        #region Ressources

        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;
        private Texture2D sprBackground;
        private Texture2D sprTextCaptureTheFlag;
        private Texture2D sprTextDeathmatch;
        private Texture2D sprTextSurvival;
        private InteractiveButton CaptureTheFlagButton;
        private InteractiveButton DeathmatchButton;
        private InteractiveButton SurvivalButton;
        private InteractiveButton MaxPlayers2Button;
        private InteractiveButton MaxPlayers4Button;
        private InteractiveButton MaxPlayers6Button;
        private InteractiveButton MaxPlayers8Button;

        private InteractiveButton TeamBalanceOnButton;
        private InteractiveButton TeamBalanceOffButton;

        private InteractiveButton CancelButton;
        private InteractiveButton OKButton;

        private TextInput RoomNameInput;
        private TextInput PasswordInput;

        private InteractiveButton[] ArrayMenuButton;

        #endregion

        private readonly TripleThunderOnlineClient OnlineGameClient;
        private readonly CommunicationClient OnlineCommunicationClient;
        private readonly string RoomType;

        private string RoomSubtype;
        private byte MinNumberOfPlayer;
        private byte MaxNumberOfPlayer;

        public CreateRoomBattle(TripleThunderOnlineClient OnlineClient, CommunicationClient OnlineCommunicationClient, string RoomType)
        {
            this.OnlineGameClient = OnlineClient;
            this.OnlineCommunicationClient = OnlineCommunicationClient;
            this.RoomType = RoomType;
            RoomSubtype = "Deathmatch";
            MinNumberOfPlayer = 2;
            MaxNumberOfPlayer = 8;
        }

        public override void Load()
        {
            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            sprBackground = Content.Load<Texture2D>("Triple Thunder/Menus/Create Room/Background Battle");
            sprTextCaptureTheFlag = Content.Load<Texture2D>("Triple Thunder/Menus/Create Room/Capture The Flag Text");
            sprTextDeathmatch = Content.Load<Texture2D>("Triple Thunder/Menus/Create Room/Deathmatch Text");
            sprTextSurvival = Content.Load<Texture2D>("Triple Thunder/Menus/Create Room/Survival Text");

            CaptureTheFlagButton = new InteractiveButton(Content, "Triple Thunder/Menus/Create Room/Capture The Flag Button", new Vector2(533, 242), OnButtonOver, SelectCaptureTheFlag);
            DeathmatchButton = new InteractiveButton(Content, "Triple Thunder/Menus/Create Room/Deathmatch Button", new Vector2(313, 242), OnButtonOver, SelectDeathmatch);
            SurvivalButton = new InteractiveButton(Content, "Triple Thunder/Menus/Create Room/Survival Button", new Vector2(423, 242), OnButtonOver, SelectSurvival);
            MaxPlayers2Button = new InteractiveButton(Content, "Triple Thunder/Menus/Create Room/Max Players 2 Button", new Vector2(353, 354), OnButtonOver, MaxPlayers2);
            MaxPlayers4Button = new InteractiveButton(Content, "Triple Thunder/Menus/Create Room/Max Players 4 Button", new Vector2(388, 354), OnButtonOver, MaxPlayers4);
            MaxPlayers6Button = new InteractiveButton(Content, "Triple Thunder/Menus/Create Room/Max Players 6 Button", new Vector2(422, 354), OnButtonOver, MaxPlayers6);
            MaxPlayers8Button = new InteractiveButton(Content, "Triple Thunder/Menus/Create Room/Max Players 8 Button", new Vector2(457, 354), OnButtonOver, MaxPlayers8);
            TeamBalanceOnButton = new InteractiveButton(Content, "Triple Thunder/Menus/Create Room/On Button", new Vector2(370, 395), OnButtonOver, null);
            TeamBalanceOffButton = new InteractiveButton(Content, "Triple Thunder/Menus/Create Room/Off Button", new Vector2(453, 395), OnButtonOver, null);

            CaptureTheFlagButton.CanBeChecked = true;
            DeathmatchButton.CanBeChecked = true;
            SurvivalButton.CanBeChecked = true;
            DeathmatchButton.Select();

            MaxPlayers2Button.CanBeChecked = true;
            MaxPlayers4Button.CanBeChecked = true;
            MaxPlayers6Button.CanBeChecked = true;
            MaxPlayers8Button.CanBeChecked = true;
            MaxPlayers8Button.Select();

            TeamBalanceOnButton.CanBeChecked = true;
            TeamBalanceOffButton.CanBeChecked = true;
            TeamBalanceOffButton.Select();

            CancelButton = new InteractiveButton(Content, "Triple Thunder/Menus/Common/Cancel Button", new Vector2(490, 481), OnButtonOver, Cancel);
            OKButton = new InteractiveButton(Content, "Triple Thunder/Menus/Common/OK Button", new Vector2(565, 481), OnButtonOver, CreateRoom);

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            RoomNameInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(270, 155), new Vector2(314, 20));
            PasswordInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(470, 187), new Vector2(84, 20), null, true);

            RoomNameInput.SetText("Let's have fun!");

            ArrayMenuButton = new InteractiveButton[]
            {
                CaptureTheFlagButton, DeathmatchButton, SurvivalButton,
                MaxPlayers2Button, MaxPlayers4Button, MaxPlayers6Button, MaxPlayers8Button,
                TeamBalanceOnButton, TeamBalanceOffButton,
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

            RoomNameInput.Update(gameTime);
            PasswordInput.Update(gameTime);
        }

        #region Button methods

        private void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        private void SelectCaptureTheFlag()
        {
            DeathmatchButton.Unselect();
            SurvivalButton.Unselect();

            RoomSubtype = "Capture The Flag";
        }

        private void SelectDeathmatch()
        {
            CaptureTheFlagButton.Unselect();
            SurvivalButton.Unselect();

            RoomSubtype = "Deathmatch";
        }

        private void SelectSurvival()
        {
            CaptureTheFlagButton.Unselect();
            DeathmatchButton.Unselect();

            RoomSubtype = "Survival";
        }

        private void MaxPlayers2()
        {
            MaxPlayers4Button.Unselect();
            MaxPlayers6Button.Unselect();
            MaxPlayers8Button.Unselect();

            MaxNumberOfPlayer = 2;
        }

        private void MaxPlayers4()
        {
            MaxPlayers2Button.Unselect();
            MaxPlayers6Button.Unselect();
            MaxPlayers8Button.Unselect();

            MaxNumberOfPlayer = 4;
        }

        private void MaxPlayers6()
        {
            MaxPlayers2Button.Unselect();
            MaxPlayers4Button.Unselect();
            MaxPlayers8Button.Unselect();

            MaxNumberOfPlayer = 6;
        }

        private void MaxPlayers8()
        {
            MaxPlayers2Button.Unselect();
            MaxPlayers4Button.Unselect();
            MaxPlayers6Button.Unselect();

            MaxNumberOfPlayer = 8;
        }

        public void CreateRoom()
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
                PushScreen(new BattleSelect(null, null, new BattleRoomInformations("No ID needed", RoomNameInput.Text, RoomType, RoomSubtype, MinNumberOfPlayer, MaxNumberOfPlayer)));
                RemoveScreen(this);
            }
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

            if (RoomSubtype == "Capture The Flag")
            {
                g.Draw(sprTextCaptureTheFlag, new Vector2(255, 280), Color.White);
            }
            else if (RoomSubtype == "Deathmatch")
            {
                g.Draw(sprTextDeathmatch, new Vector2(255, 280), Color.White);
            }
            else if (RoomSubtype == "Survival")
            {
                g.Draw(sprTextSurvival, new Vector2(255, 280), Color.White);
            }

            RoomNameInput.Draw(g);
            PasswordInput.Draw(g);
        }
    }
}
