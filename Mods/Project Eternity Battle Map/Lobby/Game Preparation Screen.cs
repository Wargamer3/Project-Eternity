using System;
using System.IO;
using System.Text;
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

        private Texture2D sprHostText;
        private Texture2D sprReadyText;

        private TextInput ChatInput;

        private EmptyBoxButton RoomSettingButton;
        private EmptyBoxButton PlayerSettingsButton;

        private EmptyBoxButton ReadyButton;
        private EmptyBoxButton StartButton;
        private EmptyBoxButton BackToLobbyButton;

        private AnimatedSprite sprTabChat;

        private IUIElement[] ArrayMenuButton;

        #endregion

        private TunnelManager TunnelBackground;

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
            RequireDrawFocus = true;
            this.OnlineGameClient = OnlineGameClient;
            this.OnlineCommunicationClient = OnlineCommunicationClient;
            this.Room = Room;

            ActiveDropdownType = ActiveDropdownTypes.None;

            ListGameRuleError = new List<GameRuleError>();

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
            TunnelBackground = new TunnelManager();
            TunnelBackground.Load(GraphicsDevice);

            #region Ressources

            sndBGM = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/Music/Wait Room.mp3");
            sndBGM.SetLoop(true);
            sndBGM.PlayAsBGM();

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");
            sndButtonDeny = new FMODSound(FMODSystem, "Content/SFX/Deny.mp3");

            fntText = Content.Load<SpriteFont>("Fonts/Arial16");
            ChatInput = new TextInput(fntText, sprPixel, sprPixel, new Vector2(15, Constants.Height - 26), new Vector2(470, 20), SendMessage);

            sprHostText = Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Player Host Text");
            sprReadyText = Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Player Ready Text");

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

            RoomSettingButton = new EmptyBoxButton(new Rectangle(LeftSideWidth + 5, MapDetailTextY + 5, RoomOptionWidth, RoomOptionHeight), fntText, "Room Settings", OnButtonOver, OpenRoomSettingsScreen);
            PlayerSettingsButton = new EmptyBoxButton(new Rectangle(LeftSideWidth + 5 + RoomOptionWidth + 5, MapDetailTextY + 5, RoomOptionWidth, RoomOptionHeight), fntText, "Player Settings", OnButtonOver, PlayerSettingsScreen);

            ReadyButton = new EmptyBoxButton(new Rectangle(LeftSideWidth + 5, Constants.Height - RoomOptionHeight - 5, RoomOptionWidth, RoomOptionHeight), fntText, "Ready", OnButtonOver, Ready);
            StartButton = new EmptyBoxButton(new Rectangle(LeftSideWidth + 5, Constants.Height - RoomOptionHeight - 5, RoomOptionWidth, RoomOptionHeight), fntText, "Start", OnButtonOver, StartGame);
            BackToLobbyButton = new EmptyBoxButton(new Rectangle(LeftSideWidth + 5 + RoomOptionWidth + 5, Constants.Height - RoomOptionHeight - 5, RoomOptionWidth, RoomOptionHeight), fntText, "Leave", OnButtonOver, ReturnToLobby);

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
            TunnelBackground.UpdateColored(gameTime);

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
                        ActivePlayer.Team = TeamIndex;
                    }
                }
                else if (TeamIndex >= 0 && TeamIndex < Room.ListMapTeam.Count)
                {
                    Rectangle TeamCollisionBox = new Rectangle(DrawX, DrawY + TeamIndex * 25, 110, 25);
                    if (TeamCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                    {
                        ActivePlayer.Team = TeamIndex;
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

        public void UpdateSelectedMap(string MapName, string MapModName, string MapPath, string GameMode, byte MinNumberOfPlayer, byte MaxNumberOfPlayer, byte MaxSquadPerPlayer, GameModeInfo GameInfo, List<string> ListMandatoryMutator, List<Color> ListMapTeam)
        {
            Room.MapName = MapName;
            Room.MapModName = MapModName;
            Room.MapPath = MapPath;
            Room.GameMode = GameMode;
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
            ListGameRuleError = Room.GameInfo.GetRule(null).Validate(Room);

            UpdateReadyOrHost();

            if (OnlineGameClient != null)
            {
                ReadyButton.Disable();

                OnlineGameClient.Host.Send(new AskChangeMapScriptClient(Room));
            }
        }

        #region Button methods

        protected virtual void OpenRoomSettingsScreen()
        {
            PushScreen(new GameOptionsScreen(Room, this));
        }

        private void PlayerSettingsScreen()
        {
            PushScreen(new BattleMapInventoryScreen());
        }

        private void ReturnToLobby()
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

                NewMap.BattleMapPath = Room.MapPath;
                NewMap.ListGameScreen = ListGameScreen;
                NewMap.SetMutators(Room.ListMutator);

                for (int P = 0; P < 10; P++)
                {
                    if (P < PlayerManager.ListLocalPlayer.Count)
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

                NewMap.Load();
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

        public override void Draw(CustomSpriteBatch g)
        {
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Lobby.BackgroundColor);

            TunnelBackground.Draw(g);

            g.End();
            g.Begin();

            DrawEmptyBox(g, new Vector2(5, 5), LeftSideWidth - 10, RoomNameHeight - 10);
            g.DrawString(fntText, "Room Name: " + Room.RoomName, new Vector2(10, 7), Color.White);
            DrawEmptyBox(g, new Vector2(0, PlayerZoneY), LeftSideWidth, PlayerZoneHeight);
            DrawEmptyBox(g, new Vector2(5, ChatZoneY + 5), LeftSideWidth - 10, ChatZoneHeight - 10);
            g.DrawString(fntText, "Chat", new Vector2(10, ChatZoneY + 10), Color.White);

            DrawEmptyBox(g, new Vector2(LeftSideWidth + 5, 5), RightSideWidth - 10, MapDetailTextY - 10);
            g.DrawString(fntText, "Player Info", new Vector2(LeftSideWidth + 10, 7), Color.White);

            int GameModeY = MapDetailTextY + RoomOptionHeight;

            DrawEmptyBox(g, new Vector2(LeftSideWidth + 5, GameModeY + 5), RightSideWidth - 10, fntText.LineSpacing * 3);

            g.DrawString(fntText, "Game Mode:", new Vector2(LeftSideWidth + 10, GameModeY + 10), Color.White);
            g.DrawStringMiddleAligned(fntText, "Campaign", new Vector2(LeftSideWidth + RightSideWidth - 45, GameModeY + 10), Color.White);
            GameModeY += fntText.LineSpacing;
            g.DrawString(fntText, "Players:", new Vector2(LeftSideWidth + 10, GameModeY + 10), Color.White);
            if (Room.MinNumberOfPlayer != Room.MaxNumberOfPlayer)
            {
                g.DrawStringMiddleAligned(fntText, Room.MinNumberOfPlayer + " - " + Room.MaxNumberOfPlayer, new Vector2(LeftSideWidth + RightSideWidth - 45, GameModeY + 10), Color.White);
            }
            else
            {
                g.DrawStringMiddleAligned(fntText, Room.MinNumberOfPlayer.ToString(), new Vector2(LeftSideWidth + RightSideWidth - 45, GameModeY + 10), Color.White);
            }
            GameModeY += fntText.LineSpacing;
            g.DrawString(fntText, "Map Size:", new Vector2(LeftSideWidth + 10, GameModeY + 10), Color.White);
            g.DrawStringMiddleAligned(fntText, MapSize.X + " x " + MapSize.Y, new Vector2(LeftSideWidth + RightSideWidth - 45, GameModeY + 10), Color.White);
            GameModeY += 90;
            g.Draw(sprMapImage, new Vector2(LeftSideWidth + (RightSideWidth - sprMapImage.Width) / 2, GameModeY), Color.White);
            GameModeY += 85;

            DrawEmptyBox(g, new Vector2(LeftSideWidth + 5, GameModeY + 5), RightSideWidth - 10, fntText.LineSpacing * 4);

            g.DrawString(fntText, "Map:", new Vector2(LeftSideWidth + 10, GameModeY + 10), Color.White);
            if (Room.MapName != null)
            {
                g.DrawStringMiddleAligned(fntText, Room.MapName, new Vector2(LeftSideWidth + RightSideWidth - 45, GameModeY + 10), Color.White);
            }
            GameModeY += fntText.LineSpacing;
            g.DrawString(fntText, "Details:", new Vector2(LeftSideWidth + 10, GameModeY + 10), Color.White);
            GameModeY += fntText.LineSpacing;
            g.DrawString(fntText, "Tutorial map aimed to introduce the basics of the\r\ncampaign mode", new Vector2(LeftSideWidth + 15, GameModeY + 10), Color.White);


            foreach (IUIElement ActiveButton in ArrayMenuButton)
            {
                ActiveButton.Draw(g);
            }

            if (OnlineCommunicationClient != null)
            {
                ChatHelper.DrawChat(g, fntText, OnlineCommunicationClient.Chat, ChatInput);
            }

            DrawPlayers(g);
            DrawOpenDropdown(g);
        }

        protected virtual void DrawPlayers(CustomSpriteBatch g)
        {
            int LeftSideWidth = (int)(Constants.Width * 0.7);

            int DrawY = 65;

            for (int P = 0; P < Room.ListRoomPlayer.Count; ++P)
            {
                int DrawX = 10;

                g.Draw(sprPixel, new Rectangle(DrawX, DrawY, LeftSideWidth - 10, 45), Color.FromNonPremultiplied(0, 0, 0, 50));
                DrawEmptyBox(g, new Vector2(DrawX, DrawY), LeftSideWidth - 10, 45);
                DrawPlayerBox(g, DrawX + 5, DrawY + 10, (BattleMapPlayer)Room.ListRoomPlayer[P]);

                DrawY += 55;
            }

            for (int P = 0; P < Room.ListRoomBot.Count && P < Room.MaxNumberOfBots - Room.ListRoomPlayer.Count; ++P)
            {
                int DrawX = 10;

                g.Draw(sprPixel, new Rectangle(DrawX, DrawY, LeftSideWidth - 10, 45), Color.FromNonPremultiplied(0, 0, 0, 50));
                DrawEmptyBox(g, new Vector2(DrawX, DrawY), LeftSideWidth - 10, 45);
                DrawPlayerBox(g, DrawX, DrawY, (BattleMapPlayer)Room.ListRoomBot[P]);

                DrawY += 55;
            }

            while (DrawY < ChatZoneY)
            {
                int DrawX = 10;
                g.Draw(sprPixel, new Rectangle(DrawX, DrawY, LeftSideWidth - 10, 45), Color.FromNonPremultiplied(0, 0, 0, 50));

                DrawY += 55;
            }
        }

        private void DrawPlayerBox(CustomSpriteBatch g, int DrawX, int DrawY, BattleMapPlayer PlayerToDraw)
        {
            Rectangle PlayerInfoCollisionBox = new Rectangle(DrawX, DrawY, 320, 25);

            DrawEmptyBox(g, new Vector2(DrawX, DrawY), 60, 25);

            if (PlayerToDraw.IsHost())
            {
                g.DrawString(fntText, "Host", new Vector2(DrawX + 6, DrawY + 5), Color.White);
            }
            else if (PlayerToDraw.IsReady())
            {
                g.DrawString(fntText, "Ready", new Vector2(DrawX + 6, DrawY + 5), Color.White);
            }

            DrawX += 65;
            DrawEmptyBox(g, new Vector2(DrawX, DrawY), 70, 25);
            g.DrawString(fntText, "Lv. 50", new Vector2(DrawX + 7, DrawY + 5), Color.White);
            DrawX += 75;
            DrawEmptyBox(g, new Vector2(DrawX, DrawY), 200, 25);
            g.DrawString(fntText, PlayerToDraw.Name, new Vector2(DrawX + 5, DrawY + 5), Color.White);
            DrawX += 205;

            Rectangle LoadoutCollisionBox = new Rectangle(DrawX, DrawY, 150, 25);
            DrawEmptyBox(g, new Vector2(DrawX, DrawY), 150, 25);
            g.DrawString(fntText, "Loadout 1", new Vector2(DrawX + 5, DrawY + 5), Color.White);
            DrawX += 155;

            if (Room.GameInfo != null && Room.GameInfo.UseTeams && Room.ListMapTeam.Count > 0)
            {
                DrawEmptyBox(g, new Vector2(DrawX, DrawY), 110, 25);
                g.DrawStringMiddleAligned(fntText, ListAllTeamInfo[PlayerToDraw.Team].TeamName, new Vector2(DrawX + 55, DrawY + 5), Color.White);
                DrawX += 120;
            }

            for (int S = 0; S < PlayerToDraw.Inventory.ActiveLoadout.ListSpawnSquad.Count; S++)
            {
                if (PlayerToDraw.Inventory.ActiveLoadout.ListSpawnSquad[S] != null)
                {
                    g.Draw(PlayerToDraw.Inventory.ActiveLoadout.ListSpawnSquad[S].At(0).SpriteMap, new Rectangle(DrawX + S * 35, DrawY - 3, 32, 32), Color.White);
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
