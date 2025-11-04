using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FMOD;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class GamePreparationScreen : GameScreen
    {
        private struct TeamInfo
        {
            public string TeamName;
            public Color TeamColor;

            public TeamInfo(string TeamName, Color TeamColor)
            {
                this.TeamName = TeamName;
                this.TeamColor = TeamColor;
            }
        }

        private enum ActiveDropdownTypes { None, Loadout, Team }

        #region Ressources

        private FMODSound sndBGM;
        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;
        private FMODSound sndButtonDeny;

        protected SpriteFont fntText;
        private SpriteFont fntOxanimumRegular;
        private SpriteFont fntOxanimumBold;
        private SpriteFont fntOxanimumBoldSmall;
        private SpriteFont fntOxanimumBoldBig;
        private SpriteFont fntOxanimumBoldTitle;

        private CubeBackgroundSmall CubeBackground;

        private Texture2D sprRoomNameFrame;
        private Texture2D sprButtonBigColor;
        private Texture2D sprButtonBigGray;
        private Texture2D sprButtonBigBlack;
        private Texture2D sprButtonSmall;
        private Texture2D sprPlayerBox;
        private Texture2D sprPlayerIcon;
        private Texture2D sprRoomInfo;
        private Texture2D sprPlayerInfo;
        private Texture2D sprChatBox;

        private TextInput ChatInput;

        private AnimatedSprite sprTabChat;

        public TextButton RoomSettingButton;
        public TextButton PlayerSettingsButton;

        private TextButton ReadyButton;
        private TextButton StartButton;
        private TextButton BackToLobbyButton;

        private IUIElement[] ArrayMenuButton;

        #endregion

        int LeftSideWidth;
        int RoomNameHeight;
        int PlayerZoneY;
        int PlayerZoneHeight;
        int ChatZoneY;
        int ChatZoneHeight;
        int RightSideWidth;
        int MapDetailTextY;
        int MapDetailTextHeight;
        int RoomOptionWidth;
        int RoomOptionHeight;

        protected readonly RoomInformations Room;
        private Point MapSize;
        private List<TeamInfo> ListAllTeamInfo;
        public Texture2D sprMapImage;
        private List<GameRuleError> ListGameRuleError;

        private ActiveDropdownTypes ActiveDropdownType;
        private BattleMapPlayer ActivePlayer;
        private int ActivePlayerIndex;

        private readonly BattleMapOnlineClient OnlineGameClient;
        private readonly CommunicationClient OnlineCommunicationClient;
        private bool IsHost;

        public GamePreparationScreen(BattleMapOnlineClient OnlineGameClient, CommunicationClient OnlineCommunicationClient, RoomInformations Room)
        {
            this.OnlineGameClient = OnlineGameClient;
            this.OnlineCommunicationClient = OnlineCommunicationClient;
            this.Room = Room;

            ActiveDropdownType = ActiveDropdownTypes.None;

            ListGameRuleError = new List<GameRuleError>();
            CubeBackground = new CubeBackgroundSmall();

            if (Room.ListRoomPlayer.Count == 0)
            {
                foreach (OnlinePlayerBase ActivePlayer in PlayerManager.ListLocalPlayer)
                {
                    ActivePlayer.OnlinePlayerType = OnlinePlayerBase.PlayerTypePlayer;
                    Room.AddLocalPlayer(ActivePlayer);
                }
            }
        }

        public override void Load()
        {
            #region Ressources

            CubeBackground.Load(Content);

            sndBGM = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/Music/Wait Room.mp3");
            sndBGM.SetLoop(true);
            sndBGM.PlayAsBGM();

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");
            sndButtonDeny = new FMODSound(FMODSystem, "Content/SFX/Deny.mp3");

            fntText = Content.Load<SpriteFont>("Fonts/Arial16");
            fntOxanimumRegular = Content.Load<SpriteFont>("Fonts/Oxanium Regular");
            fntOxanimumBold = Content.Load<SpriteFont>("Fonts/Oxanium Bold");
            fntOxanimumBoldSmall = Content.Load<SpriteFont>("Fonts/Oxanium Bold Small");
            fntOxanimumBoldBig = Content.Load<SpriteFont>("Fonts/Oxanium Bold Big");
            fntOxanimumBoldTitle = Content.Load<SpriteFont>("Fonts/Oxanium Bold Title");

            sprRoomNameFrame = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Waiting/Room Name Frame");
            sprButtonBigColor = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Interactive/Button Big Color");
            sprButtonBigGray = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Interactive/Button Big Gray");
            sprButtonBigBlack = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Interactive/Button Big Black");
            sprButtonSmall = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Interactive/Button Small");
            sprPlayerBox = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Waiting/Player Slot");
            sprPlayerIcon = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Waiting/Player Icon");
            sprPlayerInfo = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Waiting/Player Info");
            sprRoomInfo = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Extra Frame");
            sprChatBox = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Chat Box");

            ChatInput = new TextInput(fntText, sprPixel, sprPixel, new Vector2(15, Constants.Height - 26), new Vector2(470, 20), SendMessage);

            LeftSideWidth = (int)(Constants.Width * 0.7);
            RoomNameHeight = (int)(Constants.Height * 0.05);
            PlayerZoneY = RoomNameHeight;
            PlayerZoneHeight = (int)(Constants.Height * 0.65);
            ChatZoneY = PlayerZoneY + PlayerZoneHeight;
            ChatZoneHeight = Constants.Height - ChatZoneY;
            RightSideWidth = Constants.Width - LeftSideWidth;
            MapDetailTextY = (int)(Constants.Height * 0.3);
            MapDetailTextHeight = (int)(Constants.Height * 0.3);
            RoomOptionWidth = (int)((RightSideWidth - 15) * 0.5);
            RoomOptionHeight = (int)(Constants.Height * 0.08);

            float Ratio = Constants.Height / 2160f;

            int DrawX = (int)(2758 * Ratio) + sprButtonSmall.Width / 4;
            int DrawY = (int)(726 * Ratio) + sprButtonSmall.Height / 4;

            RoomSettingButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Small}{Centered}{Color:65,70,65,255}Room Settings}}", "Deathmatch/Lobby Menu/Interactive/Button Tab", new Vector2(DrawX, DrawY), 4, 1, Ratio, OnButtonOver, OnOpenRoomSettingsScreenPressed);
            DrawX = (int)(3280 * Ratio) + sprButtonSmall.Width / 4;
            PlayerSettingsButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Small}{Centered}{Color:65,70,65,255}Player Settings}}", "Deathmatch/Lobby Menu/Interactive/Button Tab", new Vector2(DrawX, DrawY), 4, 1, Ratio, OnButtonOver, PlayerSettingsScreen);

            DrawX = (int)(2784 * Ratio) + sprButtonBigColor.Width / 4;
            DrawY = (int)(1648 * Ratio) + sprButtonBigColor.Width / 16;
            ReadyButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold}{Centered}{Color:65,70,65,255}Ready}}", "Deathmatch/Lobby Menu/Interactive/Button Big Color", new Vector2(DrawX, DrawY), 4, 1, Ratio, OnButtonOver, Ready);
            StartButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold}{Centered}{Color:65,70,65,255}Ready}}", "Deathmatch/Lobby Menu/Interactive/Button Big Color", new Vector2(DrawX, DrawY), 4, 1, Ratio, OnButtonOver, StartGame);
            DrawY = (int)(1876 * Ratio) + sprButtonBigColor.Width / 16;
            BackToLobbyButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold}{Centered}{Color:65,70,65,255}Leave}}", "Deathmatch/Lobby Menu/Interactive/Button Big Color", new Vector2(DrawX, DrawY), 4, 1, Ratio, OnButtonOver, ReturnToLobby);

            sprTabChat = new AnimatedSprite(Content, "Triple Thunder/Menus/Channel/Tab Chat", new Vector2(0, 0), 0, 1, 4);

            #endregion

            StartButton.Disable();

            UpdateReadyOrHost();

            ListAllTeamInfo = new List<TeamInfo>();
            ListAllTeamInfo.Add(new TeamInfo("Blue Team", Color.Blue));
            ListAllTeamInfo.Add(new TeamInfo("Red Team", Color.Red));
            ListAllTeamInfo.Add(new TeamInfo("Green Team", Color.Green));
            ListAllTeamInfo.Add(new TeamInfo("Yellow Team", Color.Yellow));

            sprMapImage = Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Map Icons/Random");
        }

        public override void Unload()
        {
            SoundSystem.ReleaseSound(sndBGM);
            SoundSystem.ReleaseSound(sndButtonOver);
            SoundSystem.ReleaseSound(sndButtonClick);
        }

        public override void Update(GameTime gameTime)
        {
            if (OnlineGameClient != null)
            {
                OnlineGameClient.ExecuteDelayedScripts();
            }

            if (OnlineCommunicationClient != null)
            {
                OnlineCommunicationClient.ExecuteDelayedScripts();
            }

            if (FMODSystem.sndActiveBGM != sndBGM)
            {
                sndBGM.PlayAsBGM();
            }

            foreach (IUIElement ActiveButton in ArrayMenuButton)
            {
                ActiveButton.Update(gameTime);
            }

            if (OnlineCommunicationClient != null)
            {
                ChatHelper.UpdateChat(gameTime, OnlineCommunicationClient.Chat, ChatInput);
            }

            HandleLoadoutSelection();
            HandleTeamSelection();
        }

        private void HandleLoadoutSelection()
        {
            if (ActiveDropdownType ==  ActiveDropdownTypes.None && MouseHelper.InputLeftButtonPressed())
            {
                int DrawX = 360;
                int DrawY = 75;
                int PlayerIndex = (MouseHelper.MouseStateCurrent.Y - DrawY) / 45;
                if (PlayerIndex >= 0 && PlayerIndex < Room.ListRoomPlayer.Count)
                {
                    Rectangle LoadoutCollisionBox = new Rectangle(DrawX, DrawY + PlayerIndex * 45, 150, 25);
                    if (PlayerManager.ListLocalPlayer.Contains(Room.ListRoomPlayer[PlayerIndex]) && LoadoutCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                    {
                        ActivePlayer = (BattleMapPlayer)Room.ListRoomPlayer[PlayerIndex];
                        ActivePlayerIndex = PlayerIndex;
                        ActiveDropdownType = ActiveDropdownTypes.Loadout;
                    }
                }

                DrawY = 45 + Room.ListRoomPlayer.Count * 45;
                PlayerIndex = (MouseHelper.MouseStateCurrent.Y - DrawY) / 45;
                if (PlayerIndex >= 0 && PlayerIndex < Room.ListRoomBot.Count)
                {
                    Rectangle TeamCollisionBox = new Rectangle(DrawX, DrawY + PlayerIndex * 45, 150, 25);
                    if (TeamCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                    {
                        ActivePlayer = (BattleMapPlayer)Room.ListRoomBot[PlayerIndex];
                        ActivePlayerIndex = Room.ListRoomPlayer.Count + PlayerIndex;
                        ActiveDropdownType = ActiveDropdownTypes.Loadout;
                    }
                }
            }
            else if (ActiveDropdownType == ActiveDropdownTypes.Loadout && MouseHelper.InputLeftButtonPressed())
            {
                int DrawX = 360;
                int DrawY = 45 + 30 + ActivePlayerIndex * 45;
                int LoadoutIndex = (MouseHelper.MouseStateCurrent.Y - DrawY) / 25;

                if (LoadoutIndex >= 0 && ActivePlayerIndex < Room.ListRoomPlayer.Count)
                {
                    Rectangle LoadoutCollisionBox = new Rectangle(DrawX, DrawY + LoadoutIndex * 25, 150, 25);
                    if (LoadoutCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                    {
                        ActivePlayer.Inventory.ActiveLoadout = ActivePlayer.Inventory.ListSquadLoadout[LoadoutIndex];
                    }
                }
                else if (LoadoutIndex >= 0 && LoadoutIndex < Room.ListMapTeam.Count)
                {
                    Rectangle LoadoutCollisionBox = new Rectangle(DrawX, DrawY + LoadoutIndex * 25, 150, 25);
                    if (LoadoutCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                    {
                        ActivePlayer.Inventory.ActiveLoadout = ActivePlayer.Inventory.ListSquadLoadout[LoadoutIndex];
                    }
                }

                ActivePlayerIndex = -1;
                ActivePlayer = null;
                ActiveDropdownType = ActiveDropdownTypes.None;
            }
        }

        private void HandleTeamSelection()
        {
            if (ActiveDropdownType == ActiveDropdownTypes.None && Room.GameInfo != null && Room.GameInfo.UseTeams && Room.ListMapTeam.Count > 0 && MouseHelper.InputLeftButtonPressed())
            {
                int DrawX = 515;
                int DrawY = 75;
                int PlayerIndex = (MouseHelper.MouseStateCurrent.Y - DrawY) / 45;
                if (PlayerIndex >= 0 && PlayerIndex < Room.ListRoomPlayer.Count)
                {
                    Rectangle TeamCollisionBox = new Rectangle(DrawX, DrawY + PlayerIndex * 45, 110, 25);
                    if (PlayerManager.ListLocalPlayer.Contains(Room.ListRoomPlayer[PlayerIndex]) && TeamCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                    {
                        ActivePlayer = (BattleMapPlayer)Room.ListRoomPlayer[PlayerIndex];
                        ActivePlayerIndex = PlayerIndex;
                        ActiveDropdownType = ActiveDropdownTypes.Team;
                    }
                }

                DrawY = 45 + Room.ListRoomPlayer.Count * 45;
                PlayerIndex = (MouseHelper.MouseStateCurrent.Y - DrawY) / 45;
                if (PlayerIndex >= 0 && PlayerIndex < Room.ListRoomBot.Count)
                {
                    Rectangle TeamCollisionBox = new Rectangle(DrawX, DrawY + PlayerIndex * 45, 110, 25);
                    if (TeamCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                    {
                        ActivePlayer = (BattleMapPlayer)Room.ListRoomBot[PlayerIndex];
                        ActivePlayerIndex = Room.ListRoomPlayer.Count + PlayerIndex;
                        ActiveDropdownType = ActiveDropdownTypes.Team;
                    }
                }
            }
            else if (ActiveDropdownType == ActiveDropdownTypes.Team && MouseHelper.InputLeftButtonPressed())
            {
                int DrawX = 515;
                int DrawY = 45 + 30 + ActivePlayerIndex * 45;
                int TeamIndex = (MouseHelper.MouseStateCurrent.Y - DrawY) / 25;
                if (TeamIndex >= 0 && TeamIndex < Room.ListMapTeam.Count && ActivePlayerIndex < Room.ListRoomPlayer.Count)
                {
                    Rectangle TeamCollisionBox = new Rectangle(DrawX, DrawY + TeamIndex * 25, 110, 25);
                    if (TeamCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                    {
                        ActivePlayer.TeamIndex = TeamIndex;
                    }
                }
                else if (TeamIndex >= 0 && TeamIndex < Room.ListMapTeam.Count)
                {
                    Rectangle TeamCollisionBox = new Rectangle(DrawX, DrawY + TeamIndex * 25, 110, 25);
                    if (TeamCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                    {
                        ActivePlayer.TeamIndex = TeamIndex;
                    }
                }

                ActivePlayerIndex = -1;
                ActivePlayer = null;
                ActiveDropdownType = ActiveDropdownTypes.None;
            }
        }

        private void AssignButtons()
        {
            IsHost = false;
            foreach (OnlinePlayerBase ActivePlayer in Room.GetLocalPlayers())
            {
                if (ActivePlayer.IsHost())
                {
                    IsHost = true;
                }
            }

            List<IUIElement> ListButton = new List<IUIElement>()
            {
                RoomSettingButton, PlayerSettingsButton,
                BackToLobbyButton,
            };

            if (IsHost)
            {
                ListButton.Add(StartButton);
            }
            else
            {
                ListButton.Add(ReadyButton);
            }

            ArrayMenuButton = ListButton.ToArray();
        }

        public void AddPlayer(OnlinePlayerBase NewPlayer)
        {
            Room.ListRoomPlayer.Add(NewPlayer);
            Room.ListOnlinePlayer.Add(NewPlayer.OnlineClient);

            UpdateReadyOrHost();
        }

        public void UpdateSelectedMap(string MapName, string MapModName, string MapPath, string GameMode, Point MapSize, string MapDescription, byte MinNumberOfPlayer, byte MaxNumberOfPlayer, byte MaxSquadPerPlayer, GameModeInfo GameInfo, List<string> ListMandatoryMutator, List<Color> ListMapTeam)
        {
            Room.MapName = MapName;
            Room.MapModName = MapModName;
            Room.MapPath = MapPath;
            Room.MapDescription = MapDescription;
            Room.GameMode = GameMode;
            Room.MapSize = MapSize;
            Room.GameInfo = GameInfo;
            Room.MinNumberOfPlayer = MinNumberOfPlayer;
            Room.MaxNumberOfPlayer = MaxNumberOfPlayer;
            Room.MaxSquadPerPlayer = MaxSquadPerPlayer;
            Room.ListMandatoryMutator = ListMandatoryMutator;
            Room.ListMapTeam = ListMapTeam;
            ListGameRuleError.Clear();
            ReadyButton.Enable();
            StartButton.Enable();
        }

        private void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        public void UpdateReadyOrHost()
        {
            AssignButtons();

            if (IsHost)
            {
                bool IsEveryoneReady = true;

                foreach (OnlinePlayerBase ActivePlayer in Room.ListRoomPlayer)
                {
                    if (!ActivePlayer.IsHost() && !ActivePlayer.IsReady())
                    {
                        IsEveryoneReady = false;
                    }
                }

                if (IsEveryoneReady && Room.MapPath != null && ListGameRuleError.Count == 0)
                {
                    StartButton.Enable();
                }
                else
                {
                    StartButton.Disable();
                }
            }
            else if (ListGameRuleError.Count == 0)
            {
                ReadyButton.Enable();
            }
            else
            {
                ReadyButton.Disable();
            }
        }

        private void SendMessage(TextInput SenderInput, string InputMessage)
        {
            ChatInput.SetText(string.Empty);
            OnlineCommunicationClient.SendMessage(OnlineCommunicationClient.Chat.ActiveTabID, new ChatManager.ChatMessage(DateTime.UtcNow, InputMessage, ChatManager.MessageColors.White));
        }

        public void OptionsClosed()
        {
            if (Room.GameInfo == null)
            {
                return;
            }

            ListGameRuleError = Room.GameInfo.GetRule(null).Validate(Room);

            UpdateReadyOrHost();

            if (OnlineGameClient != null)
            {
                ReadyButton.Disable();

                OnlineGameClient.Host.Send(new AskChangeMapScriptClient(Room));
            }
        }

        #region Button methods

        public void OnOpenRoomSettingsScreenPressed()
        {
            OpenRoomSettingsScreen();
        }

        public virtual GameOptionsScreen OpenRoomSettingsScreen()
        {
            GameOptionsScreen NewScreen = new GameOptionsScreen(Room, this);
            PushScreen(NewScreen);

            return NewScreen;
        }

        private void PlayerSettingsScreen()
        {
            PushScreen(new BattleMapInventoryScreen());
        }

        public void ReturnToLobby()
        {
            if (OnlineGameClient != null && OnlineGameClient.IsConnected)
            {
                OnlineGameClient.Host.Send(new LeaveRoomScriptClient());
            }

            if (OnlineCommunicationClient != null && OnlineCommunicationClient.IsConnected)
            {
                OnlineCommunicationClient.Chat.OpenGlobalTab();
                OnlineCommunicationClient.Chat.CloseTab(Room.RoomID);
                OnlineCommunicationClient.Host.Send(new JoinCommunicationGroupScriptClient("Global"));
                OnlineCommunicationClient.Host.Send(new LeaveCommunicationGroupScriptClient(Room.RoomID));
            }

            sndButtonClick.Play();
            RemoveScreen(this);
        }

        private void StartGame()
        {
            if (OnlineGameClient != null && OnlineGameClient.IsConnected)
            {
                sndButtonClick.Play();

                OnlineGameClient.StartGame();
            }
            else
            {
                sndButtonClick.Play();

                BattleMap NewMap;

                if (Room.MapPath == "Random")
                {
                    NewMap = BattleMap.DicBattmeMapType[Room.MapModName].GetNewMap(Room.GameInfo, string.Empty);
                }
                else
                {
                    NewMap = BattleMap.DicBattmeMapType[Room.MapModName].GetNewMap(Room.GameInfo, string.Empty);
                }

                NewMap.BattleMapPath = "Multiplayer/" + Room.MapPath;
                NewMap.ListGameScreen = ListGameScreen;
                NewMap.SetMutators(Room.ListMutator);
                NewMap.Room = Room;

                NewMap.Load();

                for (int P = 0; P < 10; P++)
                {
                    if (P < PlayerManager.ListLocalPlayer.Count && PlayerManager.ListLocalPlayer[P].OnlinePlayerType != OnlinePlayerBase.PlayerTypeNA)
                    {
                        OnlinePlayerBase ActivePlayer = PlayerManager.ListLocalPlayer[P];
                        NewMap.AddLocalPlayer(ActivePlayer);
                    }
                    else if (P < Room.MaxNumberOfBots)
                    {
                        OnlinePlayerBase ActivePlayer = Room.ListRoomBot[P - PlayerManager.ListLocalPlayer.Count];
                        NewMap.AddLocalPlayer(ActivePlayer);
                    }
                    else//Fill with empty players to ensure the enemy player is always player 10+
                    {
                        NewMap.AddLocalPlayer(null);
                    }
                }

                NewMap.Init();
                NewMap.TogglePreview(true);
                ListGameScreen.Insert(0, NewMap);
            }
        }

        private void Ready()
        {
            sndButtonClick.Play();

            if (OnlineGameClient != null)
            {
                ReadyButton.Disable();

                OnlineGameClient.Host.Send(new AskChangePlayerReadyScriptClient());
            }
        }

        #endregion

        public override void BeginDraw(CustomSpriteBatch g)
        {
            CubeBackground.BeginDraw(g);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float Ratio = Constants.Height / 2160f;
            Color TextColorLight = Color.FromNonPremultiplied(243, 243, 243, 255);
            Color TextColorDark = Color.FromNonPremultiplied(65, 70, 65, 255);
            Color BackgroundColor = Color.FromNonPremultiplied(65, 70, 65, 255);

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(BackgroundColor);

            CubeBackground.Draw(g, false);
            g.DrawString(fntOxanimumBoldTitle, "Lobby", new Vector2(120, 28), TextColorDark);
            g.Draw(sprRoomNameFrame, new Vector2(385, 24), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 1f);

            g.Draw(sprPlayerInfo, new Vector2(Constants.Width - 559, 34), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);

            g.Draw(sprRoomInfo, new Vector2(Constants.Width - 550, 342), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);

            if (Room.MapName != null)
            {
                int DrawY = 920;
                g.DrawString(fntOxanimumBoldSmall, "Game Mode:", new Vector2(2816 * Ratio, DrawY * Ratio), TextColorDark);
                g.DrawStringRightAligned(fntOxanimumBoldSmall, Room.GameMode, new Vector2(3688 * Ratio, DrawY * Ratio), TextColorDark);

                DrawY += 74;
                g.DrawString(fntOxanimumBoldSmall, "Players:", new Vector2(2816 * Ratio, DrawY * Ratio), TextColorDark);
                g.DrawStringRightAligned(fntOxanimumBoldSmall, Room.MinNumberOfPlayer + " - " + Room.MaxNumberOfPlayer, new Vector2(3688 * Ratio, DrawY * Ratio), TextColorDark);

                DrawY += 74;
                g.DrawString(fntOxanimumBoldSmall, "Map Size:", new Vector2(2816 * Ratio, DrawY * Ratio), TextColorDark);
                g.DrawStringRightAligned(fntOxanimumBoldSmall, Room.MapSize.X + " X " + Room.MapSize.Y, new Vector2(3688 * Ratio, DrawY * Ratio), TextColorDark);
                DrawY += 150;
                g.DrawString(fntOxanimumBoldSmall, "Map:", new Vector2(2816 * Ratio, DrawY * Ratio), TextColorDark);
                g.DrawStringRightAligned(fntOxanimumBoldSmall, Room.MapName, new Vector2(3688 * Ratio, DrawY * Ratio), TextColorDark);

                DrawY += 74;
                g.DrawString(fntOxanimumBoldSmall, "Details:", new Vector2(2816 * Ratio, DrawY * Ratio), TextColorDark);
                DrawY += 74;
                g.DrawString(fntOxanimumBoldSmall, Room.MapDescription, new Vector2(2816 * Ratio, DrawY * Ratio), TextColorDark);
            }

            foreach (IUIElement ActiveButton in ArrayMenuButton)
            {
                ActiveButton.Draw(g);
            }

            g.Draw(sprChatBox, new Vector2(30, 780), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);
            if (OnlineCommunicationClient != null)
            {
                ChatHelper.DrawChat(g, fntText, OnlineCommunicationClient.Chat, ChatInput);
            }

            DrawPlayers(g);
            DrawOpenDropdown(g);
        }

        protected virtual void DrawPlayers(CustomSpriteBatch g)
        {
            float Ratio = Constants.Height / 2160f;
            int PlayerDrawY = 292;
            int DrawY = PlayerDrawY + 55 * Room.ListRoomPlayer.Count;

            for (int P = 0; P < Room.ListRoomBot.Count && P < Room.MaxNumberOfBots - Room.ListRoomPlayer.Count; ++P)
            {
                int DrawX = 10;

                DrawPlayerBox(g, DrawX, DrawY, (BattleMapPlayer)Room.ListRoomBot[P]);

                DrawY += 160;
            }

            DrawY = PlayerDrawY;

            for (int P = Room.ListRoomPlayer.Count - 1; P >= 0; --P)
            {
                int DrawX = 17;

                DrawPlayerBox(g, DrawX, (int)(DrawY * Ratio), (BattleMapPlayer)Room.ListRoomPlayer[P]);

                DrawY += 160;
            }
            for (int P = 2; P >= 0; --P)
            {
                int DrawX = 17;

                g.Draw(sprPlayerBox, new Vector2(DrawX, (int)(DrawY * Ratio)), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);

                DrawY += 160;
            }

            for (int P = 0; P < Room.ListRoomBot.Count && P < Room.MaxNumberOfBots - Room.ListRoomPlayer.Count; ++P)
            {
                int DrawX = 10;

                DrawPlayerBox(g, DrawX, DrawY, (BattleMapPlayer)Room.ListRoomBot[P]);

                DrawY += 160;
            }
        }

        private void DrawPlayerBox(CustomSpriteBatch g, int DrawX, int DrawY, BattleMapPlayer PlayerToDraw)
        {
            float Ratio = Constants.Height / 2160f;
            g.Draw(sprPlayerBox, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);
            Rectangle PlayerInfoCollisionBox = new Rectangle(DrawX, DrawY, 320, 25);


            if (PlayerToDraw.IsHost())
            {
                g.DrawString(fntOxanimumBold, "Host", new Vector2(1230, DrawY + 18), Color.White);
            }
            else if (PlayerToDraw.IsReady())
            {
                g.DrawString(fntText, "Ready", new Vector2(DrawX + 6, DrawY + 5), Color.White);
            }

            DrawX += 85;
            g.DrawString(fntOxanimumRegular, PlayerToDraw.Name, new Vector2(DrawX + 5, DrawY + 18), Color.White);
            DrawX += 230;
            g.DrawString(fntOxanimumRegular, "Lv. 50", new Vector2(DrawX, DrawY + 18), Color.White);
            DrawX += 155;

            Rectangle LoadoutCollisionBox = new Rectangle(DrawX, DrawY, 150, 25);
            g.DrawString(fntOxanimumRegular, "Loadout 1", new Vector2(DrawX, DrawY + 18), Color.White);
            DrawX += 190;

            if (Room.GameInfo != null && Room.GameInfo.UseTeams && Room.ListMapTeam.Count > 0)
            {
                g.DrawStringMiddleAligned(fntOxanimumRegular, ListAllTeamInfo[PlayerToDraw.TeamIndex].TeamName, new Vector2(DrawX + 55, DrawY + 5), Color.White);
                DrawX += 120;
            }

            for (int S = 0; S < PlayerToDraw.Inventory.ActiveLoadout.ListSpawnSquad.Count; S++)
            {
                if (PlayerToDraw.Inventory.ActiveLoadout.ListSpawnSquad[S] != null)
                {
                    Rectangle UnitCollisionBox = new Rectangle(DrawX + S * 35, DrawY, 64, 64);

                    g.Draw(PlayerToDraw.Inventory.ActiveLoadout.ListSpawnSquad[S].At(0).SpriteMap, UnitCollisionBox, Color.White);

                    foreach (GameRuleError ActiveError in ListGameRuleError)
                    {
                        if (ActiveError.ErrorTarget == PlayerToDraw.Inventory.ActiveLoadout.ListSpawnSquad[S].At(0))
                        {
                            g.Draw(sprPixel, UnitCollisionBox, Color.FromNonPremultiplied(255, 0, 0, 127));
                            if (UnitCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                            {
                                List<string> ListDescriptionText = TextHelper.FitToWidth(fntText, ActiveError.Description, 100);
                                DrawBox(g, new Vector2(DrawX, DrawY + 30), 150, 30 + ListDescriptionText.Count * 30, Color.Black);
                                g.DrawString(fntText, "Error:", new Vector2(DrawX + 5, DrawY + 35), Color.White);
                                TextHelper.DrawTextMultiline(g, ListDescriptionText, TextHelper.TextAligns.Left, DrawX + 10 + 50, DrawY + 65, 100);
                            }
                        }
                    }
                }
            }
            
            if (ActiveDropdownType == ActiveDropdownTypes.None)
            {
                Rectangle TeamCollisionBox = new Rectangle(DrawX - 120, DrawY, 110, 25);

                if (LoadoutCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y) && (PlayerManager.ListLocalPlayer.Contains(PlayerToDraw)))
                {
                    g.Draw(sprPixel, LoadoutCollisionBox, Color.FromNonPremultiplied(255, 255, 255, 127));
                }
                else if (Room.GameInfo != null && Room.GameInfo.UseTeams && Room.ListMapTeam.Count > 0 && TeamCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y)
                    && (PlayerManager.ListLocalPlayer.Contains(PlayerToDraw) || Room.ListRoomBot.Contains(PlayerToDraw)))
                {
                    g.Draw(sprPixel, TeamCollisionBox, Color.FromNonPremultiplied(255, 255, 255, 127));
                }
                else if (PlayerInfoCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                {
                    g.Draw(sprPixel, new Rectangle(DrawX, DrawY, 280, 25), Color.FromNonPremultiplied(255, 255, 255, 127));
                }
            }
        }

        protected void DrawOpenDropdown(CustomSpriteBatch g)
        {
            if (ActiveDropdownType == ActiveDropdownTypes.Team)
            {
                int DrawX = 515;
                int DrawY = 45 + ActivePlayerIndex * 45;
                DrawBox(g, new Vector2(DrawX, DrawY + 25), 120, 5 + 25 * Room.ListMapTeam.Count, Color.Black);
                for (int T = 0; T < Room.ListMapTeam.Count; T++)
                {
                    Rectangle TeamCollisionBox = new Rectangle(DrawX, DrawY + 30 + T * 25, 110, 25);
                    if (TeamCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                    {
                        g.Draw(sprPixel, TeamCollisionBox, Color.FromNonPremultiplied(255, 255, 255, 127));
                    }
                    g.DrawString(fntText, ListAllTeamInfo[T].TeamName, new Vector2(DrawX + 5, DrawY + 30 + T * 25), Color.White);
                }
            }
            else if (ActiveDropdownType == ActiveDropdownTypes.Loadout)
            {
                int DrawX = 360;
                int DrawY = 45 + ActivePlayerIndex * 45;
                DrawBox(g, new Vector2(DrawX, DrawY + 25), 150, 5 + 25 * ActivePlayer.Inventory.ListSquadLoadout.Count, Color.Black);
                for (int T = 0; T < ActivePlayer.Inventory.ListSquadLoadout.Count; T++)
                {
                    Rectangle LoadoutCollisionBox = new Rectangle(DrawX, DrawY + 28 + T * 25, 150, 25);
                    if (LoadoutCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                    {
                        g.Draw(sprPixel, LoadoutCollisionBox, Color.FromNonPremultiplied(255, 255, 255, 127));
                    }
                    g.DrawString(fntText, ActivePlayer.Inventory.ListSquadLoadout[T].Name, new Vector2(DrawX + 5, DrawY + 30 + T * 25), Color.White);
                }
            }
        }
    }
}
