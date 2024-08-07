﻿using System;
using System.Net;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FMOD;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class Lobby : GameScreen
    {
        private enum PlayerListTypes { All, Friends, Guild }

        #region Ressources

        private FMODSound sndBGM;
        private FMODSound sndButtonOver;
        protected FMODSound sndButtonClick;

        private SpriteFont fntButton;
        private SpriteFont fntArial12;

        private Texture2D sprBackground;
        private Texture2D sprTitleBattle;
        private Texture2D sprLicenseAll;

        private TextInput ChatInput;

        private EmptyBoxButton LocalPlayerSelectionButton;

        private EmptyBoxButton CreateARoomButton;
        private EmptyBoxButton QuickStartButton;
        private InteractiveButton WaitingRoomOnlyButton;

        private EmptyBoxButton InfoButton;
        private EmptyBoxButton RankingButton;
        private EmptyBoxButton OptionsButton;
        private EmptyBoxButton HelpButton;

        private EmptyBoxButton ShowAllPlayersFilter;
        private EmptyBoxButton ShowFriendsFilter;
        private EmptyBoxButton ShowGuildsFilter;

        private EmptyBoxButton ShopButton;
        private EmptyBoxButton InventoryButton;

        private IUIElement[] ArrayMenuButton;

        #endregion

        public static Color BackgroundColor = Color.FromNonPremultiplied(65, 70, 65, 255);

        int LeftSideWidth;
        int TopSectionHeight;
        int MiddleSectionY;
        int MiddleSectionHeight;
        int RoomSectionY;
        int RoomSectionHeight;
        int BottomSectionY;

        int RightSideWidth;
        int PlayerInfoHeight;
        int InventoryShopListHeight;
        int InventoryShopListY;
        int PlayerListY;
        int PlayerListHeight;

        protected readonly BattleMapOnlineClient OnlineGameClient;
        protected readonly CommunicationClient OnlineCommunicationClient;
        public readonly Dictionary<string, RoomInformations> DicAllRoom;
        private OnlinePlayerBase[] ArrayLobbyPlayer;
        private OnlinePlayerBase[] ArrayLobbyFriends;
        PlayerListTypes PlayerListType;

        public Lobby(bool UseOnline)
        {
            DicAllRoom = new Dictionary<string, RoomInformations>();

            ArrayLobbyPlayer = new OnlinePlayerBase[0];
            ArrayLobbyFriends = new OnlinePlayerBase[0];

            if (UseOnline)
            {
                Dictionary<string, OnlineScript> DicOnlineGameClientScripts = new Dictionary<string, OnlineScript>();
                Dictionary<string, OnlineScript> DicOnlineCommunicationClientScripts = new Dictionary<string, OnlineScript>();

                OnlineGameClient = new BattleMapOnlineClient(DicOnlineGameClientScripts);
                OnlineCommunicationClient = new CommunicationClient(DicOnlineCommunicationClientScripts);

                PopulateGameClientScripts(DicOnlineGameClientScripts);

                DicOnlineCommunicationClientScripts.Add(ReceiveGlobalMessageScriptClient.ScriptName, new ReceiveGlobalMessageScriptClient(OnlineCommunicationClient));
                DicOnlineCommunicationClientScripts.Add(ReceiveGroupMessageScriptClient.ScriptName, new ReceiveGroupMessageScriptClient(OnlineCommunicationClient));
                DicOnlineCommunicationClientScripts.Add(ReceiveGroupInviteScriptClient.ScriptName, new ReceiveGroupInviteScriptClient(OnlineCommunicationClient));
                DicOnlineCommunicationClientScripts.Add(ReceiveRemoteGroupInviteScriptClient.ScriptName, new ReceiveRemoteGroupInviteScriptClient(OnlineCommunicationClient));
                DicOnlineCommunicationClientScripts.Add(MessageListGroupScriptClient.ScriptName, new MessageListGroupScriptClient(OnlineCommunicationClient));
                DicOnlineCommunicationClientScripts.Add(PlayerListScriptClient.ScriptName, new PlayerListScriptClient(OnlineCommunicationClient, this));
                DicOnlineCommunicationClientScripts.Add(FriendListScriptClient.ScriptName, new FriendListScriptClient(OnlineCommunicationClient, this));
            }
        }

        public override void Load()
        {
            LeftSideWidth = (int)(Constants.Width * 0.7);
            TopSectionHeight = (int)(Constants.Height * 0.1);
            MiddleSectionY = TopSectionHeight;
            MiddleSectionHeight = (int)(Constants.Height * 0.6);
            RoomSectionY = (int)(MiddleSectionY + MiddleSectionHeight * 0.2);
            RoomSectionHeight = MiddleSectionHeight - (RoomSectionY - MiddleSectionY);
            BottomSectionY = MiddleSectionY + MiddleSectionHeight;

            RightSideWidth = Constants.Width - LeftSideWidth;
            PlayerInfoHeight = (int)(Constants.Height * 0.2);
            PlayerListY = PlayerInfoHeight;
            PlayerListHeight = (int)(Constants.Height * 0.6);
            InventoryShopListY = PlayerListY + PlayerListHeight;
            InventoryShopListHeight = Constants.Height - InventoryShopListY;

            int ButtonInfoHeight = (int)(Constants.Height * 0.07);

            int ButtonInventoryWidth = (int)((RightSideWidth - 40) * 0.5);
            int ButtonInventoryHeight = (int)(Constants.Height * 0.08);

            Trace.Listeners.Add(new TextWriterTraceListener("ClientError.log", "myListener"));

            PlayerManager.DicUnitType = Unit.LoadAllUnits();
            PlayerManager.DicRequirement = BaseSkillRequirement.LoadAllRequirements();
            PlayerManager.DicEffect = BaseEffect.LoadAllEffects();
            PlayerManager.DicAutomaticSkillTarget = AutomaticSkillTargetType.LoadAllTargetTypes();
            PlayerManager.DicManualSkillTarget = ManualSkillTarget.LoadAllTargetTypes();

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            fntButton = Content.Load<SpriteFont>("Fonts/Arial16");
            ChatInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(15, Constants.Height - 26), new Vector2(470, 20), SendMessage);

            sndBGM = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/Music/Channel.mp3");
            sndBGM.SetLoop(true);
            sndBGM.PlayAsBGM();

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            sprBackground = Content.Load<Texture2D>("Triple Thunder/Menus/Channel/Background");
            sprLicenseAll = Content.Load<Texture2D>("Triple Thunder/Menus/Channel/License All");
            sprTitleBattle = Content.Load<Texture2D>("Triple Thunder/Menus/Channel/Title Battle");

            LocalPlayerSelectionButton = new EmptyBoxButton(new Rectangle(400, 8, 150, ButtonInfoHeight), fntButton, "Local Players\r\nManagement", OnButtonOver, SelectLocalPlayers);

            QuickStartButton = new EmptyBoxButton(new Rectangle(47, TopSectionHeight + 5, ButtonInventoryWidth, ButtonInfoHeight), fntButton, "Quick Start", OnButtonOver, null);
            CreateARoomButton = new EmptyBoxButton(new  Rectangle(52 + ButtonInventoryWidth, TopSectionHeight + 5, ButtonInventoryWidth, ButtonInfoHeight), fntButton, "Create\n\ra Room", OnButtonOver, CreateARoom);
            WaitingRoomOnlyButton = new InteractiveButton(Content, "Triple Thunder/Menus/Channel/Waiting Room Only", new Vector2(447, TopSectionHeight + 15), OnButtonOver, null);

            InfoButton = new EmptyBoxButton(new Rectangle(LeftSideWidth + 10, 40, ButtonInventoryWidth, ButtonInfoHeight), fntButton, "Info", OnButtonOver, OpenInfo);
            RankingButton = new EmptyBoxButton(new Rectangle(Constants.Width - ButtonInventoryWidth - 10, 40, ButtonInventoryWidth, ButtonInfoHeight), fntButton, "Ranking", OnButtonOver, null);
            OptionsButton = new EmptyBoxButton(new Rectangle(LeftSideWidth + 10, 45 + ButtonInfoHeight, ButtonInventoryWidth, ButtonInfoHeight), fntButton, "Options", OnButtonOver, null);
            HelpButton = new EmptyBoxButton(new Rectangle(Constants.Width - ButtonInventoryWidth - 10, 45 + ButtonInfoHeight, ButtonInventoryWidth, ButtonInfoHeight), fntButton, "Help", OnButtonOver, null);

            ShowAllPlayersFilter = new EmptyBoxButton(new Rectangle(LeftSideWidth + 10, PlayerListY + 5, 60, 25), fntButton, "All", OnButtonOver, ShowAllPlayers);
            ShowFriendsFilter = new EmptyBoxButton(new Rectangle(LeftSideWidth + 10 + 67, PlayerListY + 5, 70, 25), fntButton, "Friends", OnButtonOver, ShowFriends);
            ShowGuildsFilter = new EmptyBoxButton(new Rectangle(LeftSideWidth + 10 + 67 + 77, PlayerListY + 5, 60, 25), fntButton, "Guild", OnButtonOver, ShowGuild);

            InventoryButton = new EmptyBoxButton(new Rectangle(LeftSideWidth + 10, InventoryShopListY + 10, ButtonInventoryWidth, ButtonInventoryHeight), fntButton, "Inventory", OnButtonOver, OpenInventory);
            ShopButton = new EmptyBoxButton(new Rectangle(Constants.Width - ButtonInventoryWidth - 10, InventoryShopListY + 10, ButtonInventoryWidth, ButtonInventoryHeight), fntButton, "Shop", OnButtonOver, OpenShop);

            ShowAllPlayersFilter.CanBeChecked = true;
            ShowFriendsFilter.CanBeChecked = true;
            ShowGuildsFilter.CanBeChecked = true;
            ShowAllPlayersFilter.Select();

            ArrayMenuButton = new IUIElement[]
            {
                LocalPlayerSelectionButton, CreateARoomButton, QuickStartButton,
                InfoButton, RankingButton, OptionsButton, HelpButton,
                ShowAllPlayersFilter, ShowFriendsFilter, ShowGuildsFilter,
                ShopButton, InventoryButton,
            };

            InitPlayer();
        }

        protected virtual void PopulateGameClientScripts(Dictionary<string, OnlineScript> DicOnlineGameClientScripts)
        {
            DicOnlineGameClientScripts.Add(ConnectionSuccessScriptClient.ScriptName, new ConnectionSuccessScriptClient());
            DicOnlineGameClientScripts.Add(SendRolesScriptClient.ScriptName, new SendRolesScriptClient());
            DicOnlineGameClientScripts.Add(RedirectScriptClient.ScriptName, new RedirectScriptClient(OnlineGameClient));
            DicOnlineGameClientScripts.Add(LoginSuccessScriptClient.ScriptName, new LoginSuccessScriptClient(this));
            DicOnlineGameClientScripts.Add(NewUnlocksScriptClient.ScriptName, new NewUnlocksScriptClient(this));
            DicOnlineGameClientScripts.Add(RoomListScriptClient.ScriptName, new RoomListScriptClient(this));
            DicOnlineGameClientScripts.Add(JoinRoomLocalScriptClient.ScriptName, new JoinRoomLocalScriptClient(OnlineGameClient, OnlineCommunicationClient, this, false));
            DicOnlineGameClientScripts.Add(JoinRoomFailedScriptClient.ScriptName, new JoinRoomFailedScriptClient(OnlineGameClient, this));
            DicOnlineGameClientScripts.Add(ServerIsReadyScriptClient.ScriptName, new ServerIsReadyScriptClient());
        }

        protected void InitPlayer()
        {
            if (OnlineGameClient != null)
            {
                InitOnlineGameClient();
                InitOnlineCommunicationClient();
            }
            else
            {
                InitOfflinePlayer();
            }
        }

        protected virtual void InitOfflinePlayer()
        {
            BattleMapPlayer NewPlayer = new BattleMapPlayer(PlayerManager.OnlinePlayerID, null, OnlinePlayerBase.PlayerTypes.Player, false, 0, true, Color.Blue);

            PlayerManager.ListLocalPlayer.Add(NewPlayer);
            NewPlayer.LoadLocally(GameScreen.ContentFallback);
        }

        public override void Unload()
        {
            SoundSystem.ReleaseSound(sndBGM);
            SoundSystem.ReleaseSound(sndButtonOver);
            SoundSystem.ReleaseSound(sndButtonClick);
        }

        protected void InitOnlineGameClient()
        {
            List<string> ListServerIP = new List<string>();
            bool TryConnecting = true;
            //Loop through every connections until you find a working server or none
            do
            {
                try
                {
                    IniFile ConnectionInfo = IniFile.ReadFromFile("Connection Info.ini");

                    if (ListServerIP.Count == 0)
                    {
                        ListServerIP = ConnectionInfo.ReadAllValues("Game Client Info");
                    }

                    int ServerIndex = RandomHelper.Next(ListServerIP.Count);
                    string[] ArraySelectedServerInfo = ListServerIP[ServerIndex].Split(',');
                    ListServerIP.RemoveAt(ServerIndex);

                    OnlineGameClient.Connect(IPAddress.Parse(ArraySelectedServerInfo[0]), int.Parse(ArraySelectedServerInfo[1]));

                    TryConnecting = false;
                }
                catch (Exception)
                {
                    if (ListServerIP.Count == 0)
                    {
                        TryConnecting = false;
                        InitOfflinePlayer();
                    }
                }
            }
            while (TryConnecting);
        }

        protected void InitOnlineCommunicationClient()
        {
            List<string> ListServerIP = new List<string>();
            bool TryConnecting = true;
            //Loop through every connections until you find a working server or none
            do
            {
                try
                {
                    IniFile ConnectionInfo = IniFile.ReadFromFile("Connection Info.ini");

                    if (ListServerIP.Count == 0)
                    {
                        ListServerIP = ConnectionInfo.ReadAllValues("Communication Client Info");
                    }

                    int ServerIndex = RandomHelper.Next(ListServerIP.Count);
                    string[] ArraySelectedServerInfo = ListServerIP[ServerIndex].Split(',');
                    ListServerIP.RemoveAt(ServerIndex);

                    OnlineCommunicationClient.Connect(IPAddress.Parse(ArraySelectedServerInfo[0]), int.Parse(ArraySelectedServerInfo[1]));

                    TryConnecting = false;
                }
                catch (Exception)
                {
                }
            }
            while (TryConnecting);
        }

        public void IdentifyToCommunicationServer(string PlayerID, string PlayerName, byte[] PlayerInfo)
        {
            if (OnlineCommunicationClient.Host != null)
            {
                OnlineCommunicationClient.Host.Send(new IdentifyScriptClient(PlayerID, PlayerName, false, PlayerInfo));
            }
        }

        public void AskForPlayerList()
        {
            if (OnlineCommunicationClient.Host != null)
            {
                OnlineCommunicationClient.Host.Send(new AskForPlayersScriptClient());
            }
        }

        public void AskForPlayerInventory()
        {
            OnlineGameClient.Host.Send(new AskPlayerInventoryScriptClient(PlayerManager.OnlinePlayerID));
        }

        public override void Update(GameTime gameTime)
        {
            if (OnlineGameClient != null)
            {
                OnlineGameClient.ExecuteDelayedScripts();
                OnlineCommunicationClient.ExecuteDelayedScripts();

                ChatHelper.UpdateChat(gameTime, OnlineCommunicationClient.Chat, ChatInput);
            }
            else
            {
                PendingUnlockScreen.CheckForUnlocks(this);
            }

            PendingUnlockScreen.UpdateUnlockScreens(this);

            if (FMODSystem.sndActiveBGM != sndBGM)
            {
                sndBGM.PlayAsBGM();
            }

            UpdateRooms();
            UpdatePlayers();

            foreach (IUIElement ActiveButton in ArrayMenuButton)
            {
                ActiveButton.Update(gameTime);
            }
        }

        private void UpdateRooms()
        {
            int BoxWidth = (int)(Constants.Width * 0.3);
            int BoxHeight = (int)(Constants.Height * 0.1);

            int i = 0;
            foreach (KeyValuePair<string, RoomInformations> ActiveRoom in DicAllRoom)
            {
                float X = 18 + (i % 2) * BoxWidth;
                float Y = 139 + (i / 2) * BoxHeight;

                if (MouseHelper.MouseStateCurrent.X >= X && MouseHelper.MouseStateCurrent.X <= X + BoxWidth
                    && MouseHelper.MouseStateCurrent.Y >= Y && MouseHelper.MouseStateCurrent.Y <= Y + BoxHeight)
                {
                    if (MouseHelper.InputLeftButtonPressed() && !ActiveRoom.Value.IsDead)
                    {
                        Dictionary<string, OnlineScript> DicCreateRoomScript = new Dictionary<string, OnlineScript>();
                        OnlineGameClient.Host.AddOrReplaceScripts(DicCreateRoomScript);
                        OnlineGameClient.JoinRoom(ActiveRoom.Key);
                        ActiveRoom.Value.IsDead = true;
                    }
                }

                ++i;
            }
        }

        private void UpdatePlayers()
        {
            if (PlayerListType == PlayerListTypes.All)
            {
                for (int P = 0; P < ArrayLobbyPlayer.Length; P++)
                {
                    OnlinePlayerBase ActivePlayer = ArrayLobbyPlayer[P];
                    float X = 635;
                    float Y = 166 + P * (fntArial12.LineSpacing + 4);

                    if (MouseHelper.MouseStateCurrent.X >= X && MouseHelper.MouseStateCurrent.X <= X + 100
                        && MouseHelper.MouseStateCurrent.Y >= Y && MouseHelper.MouseStateCurrent.Y <= Y + fntArial12.LineSpacing)
                    {
                        if (MouseHelper.InputLeftButtonPressed())
                        {

                        }
                        else if (MouseHelper.InputRightButtonPressed())
                        {
                            string GroupID;
                            if (string.Compare(PlayerManager.OnlinePlayerID, ActivePlayer.ConnectionID, StringComparison.InvariantCulture) < 0)
                            {
                                GroupID = PlayerManager.OnlinePlayerID + ActivePlayer.ConnectionID;
                            }
                            else
                            {
                                GroupID = ActivePlayer.ConnectionID + PlayerManager.OnlinePlayerID;
                            }
                            OnlineCommunicationClient.Host.Send(new CreateOrJoinCommunicationGroupScriptClient(GroupID, true));
                            OnlineCommunicationClient.Host.Send(new SendGroupInviteScriptClient(GroupID, PlayerManager.OnlinePlayerName, ActivePlayer.ConnectionID));
                            OnlineCommunicationClient.Chat.OpenTab(GroupID, ActivePlayer.Name);
                        }
                    }
                }
            }
            else if (PlayerListType == PlayerListTypes.Friends)
            {
                for (int P = 0; P < ArrayLobbyFriends.Length; P++)
                {
                    OnlinePlayerBase ActivePlayer = ArrayLobbyFriends[P];
                    float X = 635;
                    float Y = 166 + P * (fntArial12.LineSpacing + 4);

                    if (MouseHelper.MouseStateCurrent.X >= X && MouseHelper.MouseStateCurrent.X <= X + 100
                        && MouseHelper.MouseStateCurrent.Y >= Y && MouseHelper.MouseStateCurrent.Y <= Y + fntArial12.LineSpacing)
                    {
                        if (MouseHelper.InputLeftButtonPressed())
                        {
                        }
                        else if (MouseHelper.InputRightButtonPressed())
                        {
                            string GroupID;
                            if (string.Compare(PlayerManager.OnlinePlayerID, ActivePlayer.ConnectionID, StringComparison.InvariantCulture) < 0)
                            {
                                GroupID = PlayerManager.OnlinePlayerID + ActivePlayer.ConnectionID;
                            }
                            else
                            {
                                GroupID = ActivePlayer.ConnectionID + PlayerManager.OnlinePlayerID;
                            }
                            OnlineCommunicationClient.Host.Send(new CreateOrJoinCommunicationGroupScriptClient(GroupID, true));
                            OnlineCommunicationClient.Host.Send(new SendGroupInviteScriptClient(GroupID, PlayerManager.OnlinePlayerName, ActivePlayer.ConnectionID));
                            OnlineCommunicationClient.Chat.OpenTab(GroupID, ActivePlayer.Name);
                        }
                    }
                }
            }
        }

        internal void PopulateRooms(List<RoomInformations> ListRoomUpdates)
        {
            foreach (RoomInformations ActiveRoomUpdate in ListRoomUpdates)
            {
                if (ActiveRoomUpdate.IsDead)
                {
                    DicAllRoom.Remove(ActiveRoomUpdate.RoomID);
                }
                else if (DicAllRoom.ContainsKey(ActiveRoomUpdate.RoomID))
                {
                    DicAllRoom[ActiveRoomUpdate.RoomID] = ActiveRoomUpdate;
                }
                else
                {
                    DicAllRoom.Add(ActiveRoomUpdate.RoomID, ActiveRoomUpdate);
                }
            }
        }

        private void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        #region Buttons

        protected virtual void SelectLocalPlayers()
        {
            PushScreen(new LocalPlayerSelectionScreen());
            sndButtonClick.Play();
        }

        protected virtual void CreateARoom()
        {
            PushScreen(new CreateRoomScreen(OnlineGameClient, OnlineCommunicationClient, ""));
            sndButtonClick.Play();
        }

        protected virtual void OpenInfo()
        {
            PushScreen(new RecordsScreen((BattleMapPlayer)PlayerManager.ListLocalPlayer[0], null));
            sndButtonClick.Play();
        }

        protected virtual void OpenInventory()
        {
            PushScreen(new BattleMapInventoryWhiteScreen());
            sndButtonClick.Play();
        }

        protected virtual void OpenShop()
        {
            PushScreen(new ShopScreen(OnlineGameClient));
            sndButtonClick.Play();
        }

        private void ShowAllPlayers()
        {
            sndButtonClick.Play();
            ShowFriendsFilter.Unselect();
            ShowGuildsFilter.Unselect();
            PlayerListType = PlayerListTypes.All;
        }

        private void ShowFriends()
        {
            sndButtonClick.Play();
            ShowAllPlayersFilter.Unselect();
            ShowGuildsFilter.Unselect();
            PlayerListType = PlayerListTypes.Friends;
        }

        private void ShowGuild()
        {
            sndButtonClick.Play();
            ShowAllPlayersFilter.Unselect();
            ShowFriendsFilter.Unselect();
            PlayerListType = PlayerListTypes.Friends;
        }

        #endregion

        private void SendMessage(TextInput SenderInput, string InputMessage)
        {
            ChatInput.SetText(string.Empty);
            OnlineCommunicationClient.SendMessage(OnlineCommunicationClient.Chat.ActiveTabID, new ChatManager.ChatMessage(DateTime.UtcNow, InputMessage, ChatManager.MessageColors.White));
        }

        public void PopulatePlayers(OnlinePlayerBase[] ArrayLobbyPlayer)
        {
            this.ArrayLobbyPlayer = ArrayLobbyPlayer;
        }

        public void PopulateFriends(OnlinePlayerBase[] ArrayLobbyFriends)
        {
            this.ArrayLobbyFriends = ArrayLobbyFriends;
        }

        public override void Draw(CustomSpriteBatch g)
        {
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(BackgroundColor);

            g.End();
            g.Begin();

            //Left side
            DrawEmptyBox(g, new Vector2(5, 5), LeftSideWidth - 10, TopSectionHeight - 5);
            g.DrawString(fntArial12, "Player 1", new Vector2(10, 15), Color.White);
            g.DrawString(fntArial12, "Player 2", new Vector2(110, 15), Color.White);
            g.DrawString(fntArial12, "Player 3", new Vector2(210, 15), Color.White);
            g.DrawString(fntArial12, "Player 4", new Vector2(310, 15), Color.White);
            DrawEmptyBox(g, new Vector2(5, MiddleSectionY), LeftSideWidth - 10, MiddleSectionHeight - 10);
            DrawEmptyBox(g, new Vector2(5, RoomSectionY), LeftSideWidth - 10, RoomSectionHeight - 10);
            DrawEmptyBox(g, new Vector2(5, BottomSectionY), LeftSideWidth - 10, Constants.Height - BottomSectionY - 10);

            //Right side
            DrawEmptyBox(g, new Vector2(LeftSideWidth, PlayerListY), RightSideWidth, PlayerListHeight);
            DrawEmptyBox(g, new Vector2(LeftSideWidth, InventoryShopListY), RightSideWidth, InventoryShopListHeight);

            g.DrawString(fntArial12, "Lv." + PlayerManager.OnlinePlayerLevel, new Vector2(LeftSideWidth + 38, 17), Color.White);
            g.DrawString(fntArial12, PlayerManager.OnlinePlayerName, new Vector2(LeftSideWidth + 98, 15), Color.White);

            g.End();
            g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            DrawRooms(g);
            DrawPlayers(g);

            foreach (IUIElement ActiveElement in ArrayMenuButton)
            {
                ActiveElement.Draw(g);
            }

            if (OnlineCommunicationClient != null)
            {
                ChatHelper.DrawChat(g, fntArial12, OnlineCommunicationClient.Chat, ChatInput);
            }
        }

        private void DrawRooms(CustomSpriteBatch g)
        {
            int i = 0;
            foreach (RoomInformations ActiveRoom in DicAllRoom.Values)
            {
                int BoxWidth = (int)(Constants.Width * 0.3);
                int BoxHeight = (int)(Constants.Height * 0.1);
                float X = 18 + (i % 2) * BoxWidth;
                float Y = 139 + (i / 2) * BoxHeight;

                DrawBox(g, new Vector2(X, Y), (int)(Constants.Width * 0.3), (int)(Constants.Height * 0.1), Color.White);

                if (MouseHelper.MouseStateCurrent.X >= X && MouseHelper.MouseStateCurrent.X <= X + BoxWidth
                    && MouseHelper.MouseStateCurrent.Y >= Y && MouseHelper.MouseStateCurrent.Y <= Y + BoxHeight)
                {
                    g.Draw(sprPixel, new Rectangle((int)X + 4, (int)Y + 4, BoxWidth - 8, BoxHeight - 8), Color.FromNonPremultiplied(255, 255, 255, 127));
                }
                g.DrawString(fntArial12, "001", new Vector2(X +10, Y - 18 + 30), Color.White);

                g.DrawString(fntArial12, ActiveRoom.RoomName, new Vector2(X + 50, Y + 12), Color.White);
                g.DrawString(fntArial12, ActiveRoom.CurrentPlayerCount.ToString(), new Vector2(X + 115, Y + 30), Color.White);
                g.DrawString(fntArial12, "/", new Vector2(X + 130, Y + 30), Color.White);
                g.DrawString(fntArial12, ActiveRoom.MaxNumberOfPlayer.ToString(), new Vector2(X + 140, Y + 30), Color.White);

                g.DrawString(fntArial12, "Waiting", new Vector2(X + 160, Y + 30), Color.FromNonPremultiplied(0, 255, 0, 255));

                g.DrawString(fntArial12, "Campaign", new Vector2(X + 20, Y + 30), Color.Yellow);
                ++i;
            }
        }

        private void DrawPlayers(CustomSpriteBatch g)
        {
            if (PlayerListType == PlayerListTypes.All)
            {
                for (int P = 0; P < ArrayLobbyPlayer.Length; P++)
                {
                    float X = 580;
                    float Y = 166 + P * (fntArial12.LineSpacing + 4);

                    if (MouseHelper.MouseStateCurrent.X >= X && MouseHelper.MouseStateCurrent.X <= X + 177
                        && MouseHelper.MouseStateCurrent.Y >= Y && MouseHelper.MouseStateCurrent.Y <= Y + fntArial12.LineSpacing)
                    {
                    }

                    DrawBox(g, new Vector2(X, Y), 175, 20, Color.White);

                    g.DrawString(fntArial12, "Lv." + ArrayLobbyPlayer[P].Level, new Vector2(X + 5, Y), Color.White);
                    g.DrawString(fntArial12, ArrayLobbyPlayer[P].Name, new Vector2(X + 55, Y), Color.White);
                }
            }
            else if (PlayerListType == PlayerListTypes.Friends)
            {
                for (int P = 0; P < ArrayLobbyFriends.Length; P++)
                {
                    float X = 580;
                    float Y = 166 + P * (fntArial12.LineSpacing + 4);

                    if (MouseHelper.MouseStateCurrent.X >= X && MouseHelper.MouseStateCurrent.X <= X + 177
                        && MouseHelper.MouseStateCurrent.Y >= Y && MouseHelper.MouseStateCurrent.Y <= Y + fntArial12.LineSpacing)
                    {
                    }

                    g.DrawString(fntArial12, "Lv." + ArrayLobbyFriends[P].Level, new Vector2(X + 5, Y), Color.White);
                    g.DrawString(fntArial12, ArrayLobbyFriends[P].Name, new Vector2(X + 55, Y), Color.White);
                }
            }
        }
    }
}
