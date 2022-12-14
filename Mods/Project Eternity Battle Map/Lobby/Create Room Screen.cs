using System;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class CreateRoomScreen : GameScreen
    {
        //PVP
        private const string GameModeDeathmatch = "Deathmatch";//Gain points for kills and assists, respawn on death
        private const string GameModeObjective = "Objective";//One team must complete objectives while another prevent them
        private const string GameModeAssault = "Assault";//Team deathmatch with limited respawns
        private const string GameModeConquest = "Conquest";//Teams must fight to capture respawn bases that give them points. The starting base may or may not be capturable
        private const string GameModeOnslaught = "Onslaught";//Teams must fight to capture respawn bases that give them access to the enemy base's core. Last team with a core win
        private const string GameModeKingOfTheHill = "King Of The Hill";//Hold a position without enemies to win points
        private const string GameModeBunny = "Bunny";//Unit that holds the flag become the bunny and gets points for kills, everyone else try to kill the bunny
        private const string GameModeFreezeTag = "Freeze Tag";//Killing an enemy freeze him, when every enemies are frozen you win. Teamates can unfreeze an ally by staying next to them for 2 turns
        private const string GameModeJailbreak = "Jailbreak";//Killing an enemy send him to your prison, capture everyone to win. Teamates can be freed by standing on a switch
        private const string GameModeMutant = "Mutant";//First kill transform you into the mutant, a unit with overpowered stats and attacks. Only the Mutant can kill or be killed
        private const string GameModeKaiju = "Kaiju";//One player controls giant monsters while the other players use their units.

        //PVE
        private const string GameModeCampaign = "Campaign";//Classic mission based mode, no respawn
        private const string GameModeInvasion = "Horde";//Wave survival mode, respawn at the start of each wave

        #region Ressources

        protected FMODSound sndButtonOver;
        protected FMODSound sndButtonClick;

        private SpriteFont fntArial12;

        protected BoxButton CancelButton;
        protected BoxButton OKButton;

        protected TextInput RoomNameInput;
        protected TextInput PasswordInput;

        private DropDownButton RoomTypeButton;
        private DropDownButton RoomSubtypeButton;

        private IUIElement[] ArrayMenuButton;

        #endregion

        protected readonly BattleMapOnlineClient OnlineGameClient;
        protected readonly CommunicationClient OnlineCommunicationClient;
        protected readonly string RoomType;

        protected string RoomSubtype;
        protected byte MinNumberOfPlayer;
        protected byte MaxNumberOfPlayer;

        public CreateRoomScreen(BattleMapOnlineClient OnlineClient, CommunicationClient OnlineCommunicationClient, string RoomType)
        {
            this.OnlineGameClient = OnlineClient;
            this.OnlineCommunicationClient = OnlineCommunicationClient;
            this.RoomType = RoomType;
            RoomSubtype = "Deathmatch";
            MinNumberOfPlayer = 1;
            MaxNumberOfPlayer = 8;
        }

        public override void Load()
        {
            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");

            int BoxWidth = (int)(Constants.Width * 0.55);
            int InnerBoxWidth = (int)(BoxWidth * 0.95);
            int BoxHeigth = (int)(Constants.Height * 0.7);
            int BoxX = Constants.Width / 2 - BoxWidth / 2;
            int InnerBoxX = BoxX + (int)(BoxWidth * 0.025);
            int BoxY = Constants.Height / 2 - BoxHeigth / 2;

            int RoomInfoX = InnerBoxX;
            int RoomInfoY = BoxY + (int)(BoxHeigth * 0.1);
            int RoomInfoHeight = (int)(BoxHeigth * 0.2);

            int ModeX = InnerBoxX;
            int ModeY = RoomInfoY + RoomInfoHeight;
            int ModeHeight = (int)(BoxHeigth * 0.28);
            int ModeBoxWith = (int)(BoxWidth * 0.37);
            int ModeBoxSeparation = (int)(BoxWidth * 0.01);
            RoomTypeButton = new DropDownButton(new Rectangle(ModeX + 64, ModeY + 15, ModeBoxWith, 30), fntArial12, "PVE", new string[] { "PVE", "PVP" }, null, null);
            RoomSubtypeButton = new DropDownButton(new Rectangle(ModeX + 64 + ModeBoxWith + ModeBoxSeparation + 10, ModeY + 15, ModeBoxWith, 30), fntArial12,
                "Campaign", new string[] { "Campaign", "Horde" }, null, null);

            int PlayerX = InnerBoxX;
            int PlayerY = ModeY + ModeHeight;
            int PlayerHeight = (int)(BoxHeigth * 0.1);

            int ButtonsWidth = (int)(BoxWidth * 0.17);
            int ButtonsHeight = (int)(BoxHeigth * 0.07);
            int ButtonOKX = InnerBoxX + (int)(InnerBoxWidth * 0.8);
            int ButtonCancelX = ButtonOKX - ButtonsWidth - 5;
            int ButtonsY = BoxY + (int)(BoxHeigth * 0.90);

            CancelButton = new BoxButton(new Rectangle(ButtonCancelX, ButtonsY, ButtonsWidth, ButtonsHeight), fntArial12, "Cancel", OnButtonOver, Cancel);
            OKButton = new BoxButton(new Rectangle(ButtonOKX, ButtonsY, ButtonsWidth, ButtonsHeight), fntArial12, "OK", OnButtonOver, CreateRoom);

            RoomNameInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(RoomInfoX + 74, RoomInfoY + 20), new Vector2(314, 20));
            PasswordInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(BoxX + 280, BoxY + 90), new Vector2(84, 20), null, true);

            RoomNameInput.SetText("Let's have fun!");

            ArrayMenuButton = new IUIElement[]
            {
                RoomTypeButton, RoomSubtypeButton,
                RoomNameInput, PasswordInput,
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
            foreach (IUIElement ActiveButton in ArrayMenuButton)
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
            RoomSubtype = "Capture The Flag";
        }

        public virtual void CreateRoom()
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
                PushScreen(new GamePreparationScreen(null, null, new BattleMapRoomInformations("No ID needed", RoomNameInput.Text, RoomType, RoomSubtype, MinNumberOfPlayer, MaxNumberOfPlayer)));
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
            int BoxWidth = (int)(Constants.Width * 0.55);
            int InnerBoxWidth = (int)(BoxWidth * 0.95);
            int BoxHeigth = (int)(Constants.Height * 0.7);
            int BoxX = Constants.Width / 2 - BoxWidth / 2;
            int InnerBoxX = BoxX + (int)(BoxWidth * 0.025);
            int BoxY = Constants.Height / 2 - BoxHeigth / 2;

            int RoomInfoX = InnerBoxX;
            int RoomInfoY = BoxY + (int)(BoxHeigth * 0.1);
            int RoomInfoHeight = (int)(BoxHeigth * 0.2);
            DrawBox(g, new Vector2(BoxX, BoxY), BoxWidth, BoxHeigth, Color.White);
            g.DrawString(fntArial12, "Create a Room", new Vector2(BoxX + 20, BoxY + 15), Color.White);
            DrawBox(g, new Vector2(RoomInfoX, RoomInfoY), InnerBoxWidth, RoomInfoHeight, Color.White);
            g.DrawString(fntArial12, "Name", new Vector2(BoxX + 25, RoomInfoY + 18), Color.White);
            DrawBox(g, new Vector2(RoomInfoX + 64, RoomInfoY + 15), (int)(BoxWidth * 0.75), 30, Color.White);
            g.DrawString(fntArial12, "Make a locked room", new Vector2(BoxX + 25, RoomInfoY + 53), Color.White);
            DrawBox(g, new Vector2(BoxX + 175, BoxY + 95), (int)(BoxWidth * 0.05), 22, Color.White);
            g.DrawString(fntArial12, "Password", new Vector2(BoxX + 200, RoomInfoY + 53), Color.White);
            DrawBox(g, new Vector2(BoxX + 272, BoxY + 90), (int)(BoxWidth * 0.32), 30, Color.White);
            g.DrawString(fntArial12, "4 digit", new Vector2(BoxX + 360, RoomInfoY + 53), Color.White);
            DrawBox(g, new Vector2(BoxX + 280, BoxY + 90), (int)(BoxWidth * 0.17), 30, Color.White);

            int ModeX = InnerBoxX;
            int ModeY = RoomInfoY + RoomInfoHeight;
            int ModeHeight = (int)(BoxHeigth * 0.28);
            int ModeBoxWith = (int)(BoxWidth * 0.37);
            int ModeBoxSeparation = (int)(BoxWidth * 0.01);
            DrawBox(g, new Vector2(ModeX, ModeY), InnerBoxWidth, ModeHeight, Color.White);
            g.DrawString(fntArial12, "Mode", new Vector2(ModeX + 15, ModeY + 18), Color.White);

            DrawBox(g, new Vector2(ModeX + 10, ModeY + 48), InnerBoxWidth - 20, 60, Color.White);
            g.DrawString(fntArial12, "Campaign", new Vector2(ModeX + 18, ModeY + 53), Color.White);
            g.DrawString(fntArial12, "Progress through multiple missions to save the world", new Vector2(ModeX + 18, ModeY + 71), Color.White);

            int PlayerX = InnerBoxX;
            int PlayerY = ModeY + ModeHeight;
            int PlayerHeight = (int)(BoxHeigth * 0.1);
            DrawBox(g, new Vector2(PlayerX, PlayerY), InnerBoxWidth, PlayerHeight, Color.White);
            g.DrawString(fntArial12, "Max Players", new Vector2(PlayerX + 15, PlayerY + 10), Color.White);

            foreach (IUIElement ActiveButton in ArrayMenuButton)
            {
                ActiveButton.Draw(g);
            }
        }
    }
}
