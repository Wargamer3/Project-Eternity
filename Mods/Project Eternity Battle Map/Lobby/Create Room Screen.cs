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
        private SpriteFont fntOxanimumBoldSmaller;
        private SpriteFont fntOxanimumBold;
        private SpriteFont fntOxanimumLightBigger;

        private Texture2D sprBackground;
        private Texture2D sprInputSmall;
        private Texture2D sprInputLarge;

        protected TextButton CancelButton;
        protected TextButton OKButton;
        protected TextButton LockedRoomCheckbox;

        protected TextInput RoomNameInput;
        protected TextInput PasswordInput;

        protected TextInput MaxPlayerInput;

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
            MinNumberOfPlayer = 1;
            MaxNumberOfPlayer = 1;
        }

        public override void Load()
        {
            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            fntOxanimumBold = Content.Load<SpriteFont>("Fonts/Oxanium Bold");
            fntOxanimumBoldSmaller = Content.Load<SpriteFont>("Fonts/Oxanium Bold Smaller");
            fntOxanimumLightBigger = Content.Load<SpriteFont>("Fonts/Oxanium Light Bigger");

            sprBackground = Content.Load<Texture2D>("Menus/Lobby/Popup/Popup Normal");
            sprInputSmall = Content.Load<Texture2D>("Menus/Lobby/Interactive/Input Small");
            sprInputLarge = Content.Load<Texture2D>("Menus/Lobby/Interactive/Input Large");

            float Ratio = Constants.Height / 2160f;
            int MenuWidth = (int)(sprBackground.Width * Ratio);
            int MenuHeight = (int)(sprBackground.Height * Ratio);
            int MenuX = Constants.Width / 2 - MenuWidth / 2;
            int MenuY = Constants.Height / 2 - MenuHeight / 2;

            OKButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Bigger}{Centered}{Color:243, 243, 243, 255}OK}}", "Menus/Lobby/Popup/Button Small Blue", new Vector2((int)(MenuX + MenuWidth - 300 * Ratio), (int)(MenuY + MenuHeight - 180 * Ratio)), 4, 1, Ratio, OnButtonOver, CreateRoom);
            CancelButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Bigger}{Centered}{Color:243, 243, 243, 255}Cancel}}", "Menus/Lobby/Popup/Button Small Grey", new Vector2((int)(MenuX + MenuWidth - 700 * Ratio), (int)(MenuY + MenuHeight - 180 * Ratio)), 4, 1, Ratio, OnButtonOver, Cancel);
            LockedRoomCheckbox = new TextButton(Content, "", "Menus/Lobby/Interactive/Checkbox", new Vector2(MenuX + 250 * Ratio, MenuY + 318 * Ratio), 4, 1, Ratio, OnButtonOver, null);
            LockedRoomCheckbox.CanBeChecked = true;
            LockedRoomCheckbox.CanBeUnChecked = true;

            RoomNameInput = new TextInput(fntOxanimumLightBigger, sprPixel, sprPixel, new Vector2(MenuX + 350 * Ratio + 20 * Ratio, MenuY + 100 * Ratio + 10 * Ratio), new Vector2(sprInputLarge.Width * Ratio - 40 * Ratio, sprInputLarge.Height * Ratio - 20 * Ratio));
            PasswordInput = new TextInput(fntOxanimumLightBigger, sprPixel, sprPixel, new Vector2((int)(MenuX + 1520 * Ratio + 20 * Ratio), (int)(MenuY + 300 * Ratio + 10 * Ratio)), new Vector2(sprInputSmall.Width * Ratio - 40 * Ratio, sprInputSmall.Height * Ratio - 20 * Ratio), null, true);

            MaxPlayerInput = new TextInput(fntOxanimumLightBigger, sprPixel, sprPixel, new Vector2(MenuX + 550 * Ratio + 210 * Ratio, MenuY + 480 * Ratio + 10 * Ratio), new Vector2(sprInputSmall.Width * Ratio - 40 * Ratio, sprInputSmall.Height * Ratio - 20 * Ratio));

            RoomNameInput.SetText("Let's have fun!");

            ArrayMenuButton = new IUIElement[]
            {
                RoomNameInput, PasswordInput, MaxPlayerInput, LockedRoomCheckbox,
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
                PlayerManager.ListLocalPlayer[0].OnlineClient.Roles.IsRoomHost = true;
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
            g.End();
            g.Begin();

            float Ratio = Constants.Height / 2160f;
            int MenuWidth = (int)(sprBackground.Width * Ratio);
            int MenuHeight = (int)(sprBackground.Height * Ratio);
            int MenuX = Constants.Width / 2 - MenuWidth / 2;
            int MenuY = Constants.Height / 2 - MenuHeight / 2;

            Color WhiteText = Color.FromNonPremultiplied(233, 233, 233, 255);
            Color BlackText = Color.FromNonPremultiplied(65, 70, 65, 255);

            g.Draw(sprBackground, new Vector2(MenuX, MenuY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 1f);

            g.DrawStringRightAligned(fntOxanimumBold, "Create a Room", new Vector2(MenuX + MenuWidth - 120 * Ratio, MenuY + 70 * Ratio), WhiteText);
            g.DrawString(fntOxanimumBold, "Name", new Vector2(MenuX + 120 * Ratio, MenuY + 120 * Ratio), BlackText);
            g.Draw(sprInputLarge, new Vector2(MenuX + 350 * Ratio, MenuY + 100 * Ratio), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 1f);
            g.DrawString(fntOxanimumLightBigger, "Make a locked room", new Vector2(MenuX + 360 * Ratio, MenuY + 300 * Ratio), BlackText);
            g.DrawString(fntOxanimumBold, "Password", new Vector2(MenuX + 1150 * Ratio, MenuY + 330 * Ratio), BlackText);
            g.Draw(sprInputLarge, new Rectangle((int)(MenuX + 1520 * Ratio), (int)(MenuY + 300 * Ratio), (int)(sprInputSmall.Width * Ratio), (int)(sprInputSmall.Height * Ratio)), null, Color.White);
            g.DrawStringCentered(fntOxanimumBold, "4 digit", new Vector2(MenuX + 1760 * Ratio, MenuY + 370 * Ratio), Color.FromNonPremultiplied(79, 84, 79, 255));

            g.Draw(sprInputSmall, new Vector2(MenuX + 550 * Ratio, MenuY + 480 * Ratio), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 1f);
            g.DrawString(fntOxanimumBold, "Max Players", new Vector2(MenuX + 120 * Ratio, MenuY + 500 * Ratio), BlackText);

            foreach (IUIElement ActiveButton in ArrayMenuButton)
            {
                ActiveButton.Draw(g);
            }
        }
    }
}
