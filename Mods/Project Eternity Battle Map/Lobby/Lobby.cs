using System;
using System.Net;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FMOD;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen.Online;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class Lobby : GameScreen
    {
        private enum PlayerListTypes { All, Friends, Guild }

        #region Ressources

        private FMODSound sndBGM;
        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;
        private Texture2D sprBackground;
        private Texture2D sprTitleBattle;
        private Texture2D sprLicenseAll;

        private TextInput ChatInput;

        private BoxButton LocalPlayerSelectionButton;

        private BoxButton CreateARoomButton;
        private BoxButton QuickStartButton;
        private InteractiveButton WaitingRoomOnlyButton;

        private BoxButton InfoButton;
        private BoxButton RankingButton;
        private BoxButton OptionsButton;
        private BoxButton HelpButton;

        private BoxButton ShowAllPlayersFilter;
        private BoxButton ShowFriendsFilter;
        private BoxButton ShowGuildsFilter;

        private BoxButton ShopButton;
        private BoxButton InventoryButton;

        private BoxButton[] ArrayMenuButton;

        #endregion

        private readonly BattleMapOnlineClient OnlineGameClient;
        private readonly CommunicationClient OnlineCommunicationClient;
        public readonly Dictionary<string, RoomInformations> DicAllRoom;
        private BattleMapPlayer[] ArrayLobbyPlayer;
        private BattleMapPlayer[] ArrayLobbyFriends;
        PlayerListTypes PlayerListType;

        public Lobby(bool UseOnline)
        {
            DicAllRoom = new Dictionary<string, RoomInformations>();

            ArrayLobbyPlayer = new BattleMapPlayer[0];
            ArrayLobbyFriends = new BattleMapPlayer[0];

            if (UseOnline)
            {
                Dictionary<string, OnlineScript> DicOnlineGameClientScripts = new Dictionary<string, OnlineScript>();
                Dictionary<string, OnlineScript> DicOnlineCommunicationClientScripts = new Dictionary<string, OnlineScript>();

                OnlineGameClient = new BattleMapOnlineClient(DicOnlineGameClientScripts);
                OnlineCommunicationClient = new CommunicationClient(DicOnlineCommunicationClientScripts);

                DicOnlineGameClientScripts.Add(ConnectionSuccessScriptClient.ScriptName, new ConnectionSuccessScriptClient());
                DicOnlineGameClientScripts.Add(RedirectScriptClient.ScriptName, new RedirectScriptClient(OnlineGameClient));
                DicOnlineGameClientScripts.Add(LoginSuccessScriptClient.ScriptName, new LoginSuccessScriptClient(this));
                DicOnlineGameClientScripts.Add(RoomListScriptClient.ScriptName, new RoomListScriptClient(this));
                DicOnlineGameClientScripts.Add(JoinRoomLocalScriptClient.ScriptName, new JoinRoomLocalScriptClient(OnlineGameClient, OnlineCommunicationClient, this, false));
                DicOnlineGameClientScripts.Add(JoinRoomFailedScriptClient.ScriptName, new JoinRoomFailedScriptClient(OnlineGameClient, this));
                DicOnlineGameClientScripts.Add(ServerIsReadyScriptClient.ScriptName, new ServerIsReadyScriptClient());

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
            Trace.Listeners.Add(new TextWriterTraceListener("ClientError.log", "myListener"));

            PlayerManager.DicUnitType = Unit.LoadAllUnits();
            PlayerManager.DicRequirement = BaseSkillRequirement.LoadAllRequirements();
            PlayerManager.DicEffect = BaseEffect.LoadAllEffects();
            PlayerManager.DicAutomaticSkillTarget = AutomaticSkillTargetType.LoadAllTargetTypes();
            PlayerManager.DicManualSkillTarget = ManualSkillTarget.LoadAllTargetTypes();

            if (OnlineGameClient != null)
            {
                InitOnlineGameClient();
                InitOnlineCommunicationClient();
            }
            else
            {
                BattleMapPlayer NewPlayer = new BattleMapPlayer(PlayerManager.OnlinePlayerID, "Player " + (PlayerManager.ListLocalPlayer.Count + 1), BattleMapPlayer.PlayerTypes.Online, false, 0, true, Color.Blue);

                PlayerManager.ListLocalPlayer.Add(NewPlayer);
                PlayerManager.ListLocalPlayer[0].LoadLocally(Content);
            }

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            ChatInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(68, 518), new Vector2(470, 20), SendMessage);

            sndBGM = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/Music/Channel.mp3");
            sndBGM.SetLoop(true);
            sndBGM.PlayAsBGM();

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            sprBackground = Content.Load<Texture2D>("Triple Thunder/Menus/Channel/Background");
            sprLicenseAll = Content.Load<Texture2D>("Triple Thunder/Menus/Channel/License All");
            sprTitleBattle = Content.Load<Texture2D>("Triple Thunder/Menus/Channel/Title Battle");

            LocalPlayerSelectionButton = new BoxButton(new Rectangle(400, 8, 120, 45), fntArial12, "Local Players\r\nManagement", OnButtonOver, SelectLocalPlayers);

            QuickStartButton = new BoxButton(new Rectangle(47, 70, 100, 45), fntArial12, "Quick Start", OnButtonOver, null);
            CreateARoomButton = new BoxButton(new  Rectangle(150, 70, 100, 45), fntArial12, "Create\n\ra Room", OnButtonOver, CreateARoom);
            WaitingRoomOnlyButton = new InteractiveButton(Content, "Triple Thunder/Menus/Channel/Waiting Room Only", new Vector2(447, 85), OnButtonOver, null);

            InfoButton = new BoxButton(new Rectangle(572, 40, 100, 35), fntArial12, "Info", OnButtonOver, null);
            RankingButton = new BoxButton(new Rectangle(682, 40, 100, 35), fntArial12, "Ranking", OnButtonOver, null);
            OptionsButton = new BoxButton(new Rectangle(572, 74, 100, 35), fntArial12, "Options", OnButtonOver, null);
            HelpButton = new BoxButton(new Rectangle(682, 74, 100, 35), fntArial12, "Help", OnButtonOver, null);

            ShowAllPlayersFilter = new BoxButton(new Rectangle(572, 130, 60, 25), fntArial12, "All", OnButtonOver, ShowAllPlayers);
            ShowFriendsFilter = new BoxButton(new Rectangle(572 + 62, 130, 60, 25), fntArial12, "Friends", OnButtonOver, ShowFriends);
            ShowGuildsFilter = new BoxButton(new Rectangle(572 + 62 + 62, 130, 60, 25), fntArial12, "Guild", OnButtonOver, ShowGuild);
            
            InventoryButton = new BoxButton(new Rectangle(572, 514, 100, 45), fntArial12, "Inventory", OnButtonOver, OpenInventory);
            ShopButton = new BoxButton(new Rectangle(682, 514, 100, 45), fntArial12, "Shop", OnButtonOver, OpenShop);

            ShowAllPlayersFilter.CanBeChecked = true;
            ShowFriendsFilter.CanBeChecked = true;
            ShowGuildsFilter.CanBeChecked = true;
            ShowAllPlayersFilter.Select();

            ArrayMenuButton = new BoxButton[]
            {
                LocalPlayerSelectionButton, CreateARoomButton, QuickStartButton,
                InfoButton, RankingButton, OptionsButton, HelpButton,
                ShowAllPlayersFilter, ShowFriendsFilter, ShowGuildsFilter,
                ShopButton, InventoryButton,
            };
        }

        public override void Unload()
        {
            SoundSystem.ReleaseSound(sndBGM);
            SoundSystem.ReleaseSound(sndButtonOver);
            SoundSystem.ReleaseSound(sndButtonClick);
        }

        private void InitOnlineGameClient()
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
                        PlayerManager.ListLocalPlayer.Add(new BattleMapPlayer(PlayerManager.OnlinePlayerID, PlayerManager.OnlinePlayerName, BattleMapPlayer.PlayerTypes.Offline, false, 0, true, Color.Blue));
                        PlayerManager.ListLocalPlayer[0].LoadLocally(Content);
                    }
                }
            }
            while (TryConnecting);
        }

        private void InitOnlineCommunicationClient()
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

        public override void Update(GameTime gameTime)
        {
            if (OnlineGameClient != null)
            {
                OnlineGameClient.ExecuteDelayedScripts();
                OnlineCommunicationClient.ExecuteDelayedScripts();

                ChatHelper.UpdateChat(gameTime, OnlineCommunicationClient.Chat, ChatInput);
            }

            if (FMODSystem.sndActiveBGM != sndBGM)
            {
                sndBGM.PlayAsBGM();
            }

            UpdateRooms();
            UpdatePlayers();

            foreach (BoxButton ActiveButton in ArrayMenuButton)
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
                    BattleMapPlayer ActivePlayer = ArrayLobbyPlayer[P];
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
                    BattleMapPlayer ActivePlayer = ArrayLobbyFriends[P];
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

        private void SelectLocalPlayers()
        {
            PushScreen(new LocalPlayerSelectionScreen());
            sndButtonClick.Play();
        }

        private void CreateARoom()
        {
            PushScreen(new CreateRoomScreen(OnlineGameClient, OnlineCommunicationClient, ""));
            sndButtonClick.Play();
        }

        private void OpenInventory()
        {
            PushScreen(new InventoryScreen());
            sndButtonClick.Play();
        }

        private void OpenShop()
        {
            PushScreen(new ShopScreen());
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

        private void SendMessage(string InputMessage)
        {
            ChatInput.SetText(string.Empty);
            OnlineCommunicationClient.SendMessage(OnlineCommunicationClient.Chat.ActiveTabID, new ChatManager.ChatMessage(DateTime.UtcNow, InputMessage, ChatManager.MessageColors.White));
        }

        public void PopulatePlayers(BattleMapPlayer[] ArrayLobbyPlayer)
        {
            this.ArrayLobbyPlayer = ArrayLobbyPlayer;
        }

        public void PopulateFriends(BattleMapPlayer[] ArrayLobbyFriends)
        {
            this.ArrayLobbyFriends = ArrayLobbyFriends;
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.End();
            g.Begin();
            g.Draw(sprBackground, Vector2.Zero, Color.White);

            int LeftSideWidth = (int)(Constants.Width * 0.7);
            int TopSectionHeight = (int)(Constants.Height * 0.1);
            int MiddleSectionY = TopSectionHeight;
            int MiddleSectionHeight = (int)(Constants.Height * 0.6);
            int RoomSectionY = (int)(MiddleSectionY + MiddleSectionHeight * 0.2);
            int RoomSectionHeight = MiddleSectionHeight - (RoomSectionY - MiddleSectionY);
            int BottomSectionY = MiddleSectionY + MiddleSectionHeight;

            //Left side
            DrawBox(g, new Vector2(0, 0), LeftSideWidth, TopSectionHeight, Color.White);
            g.DrawString(fntArial12, "Player 1", new Vector2(10, 15), Color.White);
            g.DrawString(fntArial12, "Player 2", new Vector2(110, 15), Color.White);
            g.DrawString(fntArial12, "Player 3", new Vector2(210, 15), Color.White);
            g.DrawString(fntArial12, "Player 4", new Vector2(310, 15), Color.White);
            DrawBox(g, new Vector2(0, MiddleSectionY), LeftSideWidth, MiddleSectionHeight - RoomSectionHeight, Color.White);
            DrawBox(g, new Vector2(0, RoomSectionY), LeftSideWidth, RoomSectionHeight, Color.White);
            DrawBox(g, new Vector2(0, BottomSectionY), LeftSideWidth, Constants.Height - BottomSectionY, Color.White);

            int RightSideWidth = Constants.Width - LeftSideWidth;
            int PlayerInfoHeight = (int)(Constants.Height * 0.2);
            int PlayerListY = PlayerInfoHeight;
            int PlayerListHeight = (int)(Constants.Height * 0.6);
            int InventoryShopListY = PlayerListY + PlayerListHeight;
            int InventoryShopListHeight = Constants.Height - InventoryShopListY;

            //Right side
            DrawBox(g, new Vector2(LeftSideWidth, 0), RightSideWidth, PlayerListY, Color.White);
            DrawBox(g, new Vector2(LeftSideWidth, PlayerListY), RightSideWidth, PlayerListHeight, Color.White);
            DrawBox(g, new Vector2(LeftSideWidth, InventoryShopListY), RightSideWidth, InventoryShopListHeight, Color.White);
            g.DrawString(fntArial12, "Inventory*Shop", new Vector2(LeftSideWidth + 5, InventoryShopListY + 5), Color.White);


            g.DrawString(fntArial12, "Lv." + PlayerManager.OnlinePlayerLevel, new Vector2(610, 17), Color.White);
            g.DrawString(fntArial12, PlayerManager.OnlinePlayerName, new Vector2(670, 15), Color.White);

            g.End();
            g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            DrawRooms(g);
            if (OnlineCommunicationClient != null)
            {
                ChatHelper.DrawChat(g, fntArial12, OnlineCommunicationClient.Chat, ChatInput);
            }
            DrawPlayers(g);

            foreach (BoxButton ActiveButton in ArrayMenuButton)
            {
                ActiveButton.Draw(g);
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
