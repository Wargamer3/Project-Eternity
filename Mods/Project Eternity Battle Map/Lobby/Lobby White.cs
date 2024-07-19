using System;
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
    public class LobbyWhite : GameScreen
    {
        private enum PlayerListTypes { All, Friends, Guild }

        #region Ressources

        private FMODSound sndBGM;
        private FMODSound sndButtonOver;
        protected FMODSound sndButtonClick;

        private SpriteFont fntArial12;
        private SpriteFont fntaAtomicMd;
        private SpriteFont fntOxanimumBold;
        private SpriteFont fntOxanimumBoldBig;
        private SpriteFont fntOxanimumBoldTitle;

        private Texture2D sprTitleHighlight;
        private Texture2D sprButtonInfo;
        private Texture2D sprButtonSettings;
        private Texture2D sprButtonBigColor;
        private Texture2D sprButtonBigGray;
        private Texture2D sprButtonSmall;
        private Texture2D sprFriendsList;
        private Texture2D sprButtonFriendsActive;
        private Texture2D sprButtonFriendsInactive;
        private Texture2D sprChatBox;

        private Texture2D sprRoom;
        private Texture2D sprRoomHover;
        private Texture2D sprRoomFull;

        private Texture2D sprIconPlayerActive;
        private Texture2D sprIconPlayerInactive;
        private Texture2D sprButtonLocalPlayerManagement;

        private TextInput ChatInput;

        private Model Cube;

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

        float RotationX;

        protected readonly BattleMapOnlineClient OnlineGameClient;
        protected readonly CommunicationClient OnlineCommunicationClient;
        public readonly Dictionary<string, RoomInformations> DicAllRoom;
        private OnlinePlayerBase[] ArrayLobbyPlayer;
        private OnlinePlayerBase[] ArrayLobbyFriends;
        PlayerListTypes PlayerListType;

        public LobbyWhite(bool UseOnline)
        {
            DicAllRoom = new Dictionary<string, RoomInformations>();

            ArrayLobbyPlayer = new OnlinePlayerBase[0];
            ArrayLobbyFriends = new OnlinePlayerBase[0];

            /*if (UseOnline)
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
            }*/
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
            fntaAtomicMd = Content.Load<SpriteFont>("Fonts/a Atomic Md");
            fntOxanimumBold = Content.Load<SpriteFont>("Fonts/Oxanium Bold");
            fntOxanimumBoldBig = Content.Load<SpriteFont>("Fonts/Oxanium Bold Big");
            fntOxanimumBoldTitle = Content.Load<SpriteFont>("Fonts/Oxanium Bold Title");

            sprTitleHighlight = Content.Load<Texture2D>("Menus/Lobby/Shop/Title Highlight");
            sprButtonInfo = Content.Load<Texture2D>("Menus/Lobby/Button Info");
            sprButtonSettings = Content.Load<Texture2D>("Menus/Lobby/Button Settings");
            sprButtonBigColor = Content.Load<Texture2D>("Menus/Lobby/Button Big Color");
            sprButtonBigGray = Content.Load<Texture2D>("Menus/Lobby/Button Big Gray");
            sprButtonSmall = Content.Load<Texture2D>("Menus/Lobby/Button Small");
            sprFriendsList = Content.Load<Texture2D>("Menus/Lobby/Extra Frame");
            sprButtonFriendsActive = Content.Load<Texture2D>("Menus/Lobby/Button Friend Active");
            sprButtonFriendsInactive = Content.Load<Texture2D>("Menus/Lobby/Button Friend Inactive");
            sprChatBox = Content.Load<Texture2D>("Menus/Lobby/Chat Box");

            sprRoom = Content.Load<Texture2D>("Menus/Lobby/Room Generic");
            sprRoomHover = Content.Load<Texture2D>("Menus/Lobby/Room Hover");
            sprRoomFull = Content.Load<Texture2D>("Menus/Lobby/Room Full");
            sprButtonLocalPlayerManagement = Content.Load<Texture2D>("Menus/Lobby/Button Local Player Management");

            sprIconPlayerActive = Content.Load<Texture2D>("Menus/Lobby/Icon Player Active");
            sprIconPlayerInactive = Content.Load<Texture2D>("Menus/Lobby/Icon Player Inactive");

            ChatInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(15, Constants.Height - 26), new Vector2(470, 20), SendMessage);

            Cube = Content.Load<Model>("Menus/Lobby/Cube thing");

            sndBGM = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/Music/Channel.mp3");
            sndBGM.SetLoop(true);
            sndBGM.PlayAsBGM();

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");


            ArrayMenuButton = new IUIElement[]
            {
            };

            InitPlayer();
        }

        protected virtual void PopulateGameClientScripts(Dictionary<string, OnlineScript> DicOnlineGameClientScripts)
        {
            /*DicOnlineGameClientScripts.Add(ConnectionSuccessScriptClient.ScriptName, new ConnectionSuccessScriptClient());
            DicOnlineGameClientScripts.Add(SendRolesScriptClient.ScriptName, new SendRolesScriptClient());
            DicOnlineGameClientScripts.Add(RedirectScriptClient.ScriptName, new RedirectScriptClient(OnlineGameClient));
            DicOnlineGameClientScripts.Add(LoginSuccessScriptClient.ScriptName, new LoginSuccessScriptClient(this));
            DicOnlineGameClientScripts.Add(NewUnlocksScriptClient.ScriptName, new NewUnlocksScriptClient(this));
            DicOnlineGameClientScripts.Add(RoomListScriptClient.ScriptName, new RoomListScriptClient(this));
            DicOnlineGameClientScripts.Add(JoinRoomLocalScriptClient.ScriptName, new JoinRoomLocalScriptClient(OnlineGameClient, OnlineCommunicationClient, this, false));
            DicOnlineGameClientScripts.Add(JoinRoomFailedScriptClient.ScriptName, new JoinRoomFailedScriptClient(OnlineGameClient, this));
            DicOnlineGameClientScripts.Add(ServerIsReadyScriptClient.ScriptName, new ServerIsReadyScriptClient());*/
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
            PushScreen(new BattleMapInventoryScreen());
            sndButtonClick.Play();
        }

        protected virtual void OpenShop()
        {
            PushScreen(new ShopScreen(OnlineGameClient));
            sndButtonClick.Play();
        }

        /*private void ShowAllPlayers()
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
        */
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
            DrawBackground(g);
            float Ratio = Constants.Height / 2160f;
            Color TextColorLight = Color.FromNonPremultiplied(243, 243, 243, 255);
            Color TextColorDark = Color.FromNonPremultiplied(65, 70, 65, 255);
            int DrawX = 90;
            int DrawY = 26;
            g.Draw(sprTitleHighlight, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 1f);
            g.DrawString(fntOxanimumBoldTitle, "Lobby", new Vector2(DrawX + 30, DrawY + 2), TextColorDark);

            DrawX = 350;
            DrawY = 22;
            g.Draw(sprButtonLocalPlayerManagement, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);
            g.DrawString(fntOxanimumBold, "Local Players Management", new Vector2(DrawX + 30, DrawY + 24), TextColorLight);

            DrawX = Constants.Width - 222;
            DrawY = 22;
            g.Draw(sprButtonInfo, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);
            DrawX += 90;
            g.Draw(sprButtonSettings, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);

            DrawX = 870;
            DrawY = 6;
            g.Draw(sprIconPlayerActive, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);
            g.DrawString(fntOxanimumBold, "1", new Vector2(DrawX + 70, DrawY + 12), TextColorLight);
            DrawX += 110;
            g.Draw(sprIconPlayerInactive, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);
            g.DrawString(fntOxanimumBold, "2", new Vector2(DrawX + 70, DrawY + 12), TextColorLight);
            DrawX += 110;
            g.Draw(sprIconPlayerInactive, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);
            g.DrawString(fntOxanimumBold, "3", new Vector2(DrawX + 70, DrawY + 12), TextColorLight);
            DrawX += 110;
            g.Draw(sprIconPlayerInactive, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);
            g.DrawString(fntOxanimumBold, "4", new Vector2(DrawX + 70, DrawY + 12), TextColorLight);

            DrawX = 40;
            DrawY = 142;
            g.Draw(sprButtonBigColor, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);
            g.DrawString(fntOxanimumBoldBig, "Quick Start", new Vector2(DrawX + 30, DrawY + 34), TextColorDark);
            DrawX = 445;
            g.Draw(sprButtonBigGray, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);
            g.DrawString(fntOxanimumBoldBig, "Create a Room", new Vector2(DrawX + 30, DrawY + 34), TextColorDark);

            DrawX = Constants.Width - 550;
            DrawY = 147;
            g.Draw(sprButtonSmall, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);
            g.DrawStringCentered(fntOxanimumBold, "Inventory", new Vector2(DrawX + 120, DrawY + 47), TextColorDark);
            DrawX += 290;
            g.Draw(sprButtonSmall, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);
            g.DrawStringCentered(fntOxanimumBold, "Shop", new Vector2(DrawX + 120, DrawY + 47), TextColorDark);

            DrawX = Constants.Width - 550;
            DrawY = 238;
            g.Draw(sprButtonSmall, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);
            g.DrawStringCentered(fntOxanimumBold, "Info", new Vector2(DrawX + 120, DrawY + 47), TextColorDark);
            DrawX += 290;
            g.Draw(sprButtonSmall, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);
            g.DrawStringCentered(fntOxanimumBold, "Ranking", new Vector2(DrawX + 120, DrawY + 47), TextColorDark);

            DrawX = 30;
            DrawY = 780;
            g.Draw(sprChatBox, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);

            DrawX = Constants.Width - 540;
            DrawY = 350;
            g.Draw(sprFriendsList, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);
            DrawX += 30;
            DrawY = 370;
            g.Draw(sprButtonFriendsActive, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);
            g.DrawStringCentered(fntaAtomicMd, "All", new Vector2(DrawX + 55, DrawY + 17), TextColorDark);
            DrawX += 116;
            g.Draw(sprButtonFriendsInactive, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);
            g.DrawStringCentered(fntaAtomicMd, "Friends", new Vector2(DrawX + 55, DrawY + 17), TextColorDark);
            DrawX += 116;
            g.Draw(sprButtonFriendsInactive, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);
            g.DrawStringCentered(fntaAtomicMd, "Guild", new Vector2(DrawX + 55, DrawY + 17), TextColorDark);

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

        private void DrawBackground(CustomSpriteBatch g)
        {
            Color ColorBackground = Color.FromNonPremultiplied(243, 243, 243, 255);
            Color ColorLine = Color.FromNonPremultiplied(233, 233, 233, 255);
            Color ColorLine2 = Color.FromNonPremultiplied(204, 204, 204, 64);
            float Ratio = Constants.Height / 2160f;

            g.GraphicsDevice.Clear(ColorBackground);
            g.DrawLine(sprPixel, new Vector2(0 * Ratio, 262 * Ratio), new Vector2(3560 * Ratio, 1940 * Ratio), ColorLine, 500);
            g.DrawLine(sprPixel, new Vector2(66 * Ratio, 1232 * Ratio), new Vector2(1366 * Ratio, 1894 * Ratio), ColorLine, 200);

            g.DrawLine(sprPixel, new Vector2(2500 * Ratio, -592 * Ratio), new Vector2(3800 * Ratio, -36 * Ratio), ColorLine, 220);

            float aspectRatio = 1f;

            Vector3 position = new Vector3(0, 0, 6);

            Vector3 target = new Vector3(0, 0, 3);

            Vector3 up = Vector3.Up;
            Matrix View = Matrix.CreateLookAt(position, target, up);
            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(0.40f,
                                                                    aspectRatio,
                                                                    1000f, 18000);

            ((BasicEffect)Cube.Meshes[0].Effects[0]).DiffuseColor = new Vector3(248f / 255f);
            Cube.Draw(Matrix.CreateTranslation(0, 0, 0) * Matrix.CreateRotationX(1f) * Matrix.CreateRotationY(-0.9f)
                * Matrix.CreateRotationX(0) * Matrix.CreateRotationY(RotationX)
                * Matrix.CreateScale(0.4f) * Matrix.CreateTranslation(-280, -170, -4800), View, Projection);

            g.DrawLine(sprPixel, new Vector2(-550 * Ratio, 766 * Ratio), new Vector2(2550 * Ratio, -696 * Ratio), ColorLine2, 630);
            g.DrawLine(sprPixel, new Vector2(2200 * Ratio, 2402 * Ratio), new Vector2(2700 * Ratio, 2188 * Ratio), ColorLine2, 630);
            g.End();
            g.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);

            RotationX += 0.005f;
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
