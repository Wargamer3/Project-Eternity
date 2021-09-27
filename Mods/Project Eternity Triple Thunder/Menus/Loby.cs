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
using ProjectEternity.GameScreens.TripleThunderScreen.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class Loby : GameScreen
    {
        #region Ressources

        private FMODSound sndBGM;
        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;
        private Texture2D sprBackground;
        private Texture2D sprTitleBattle;
        private Texture2D sprLicenseAll;

        private List<string> ListChatHistory;
        private TextInput ChatInput;

        private InteractiveButton CreateARoomButton;
        private InteractiveButton QuickStartButton;
        private InteractiveButton WaitingRoomOnlyButton;

        private InteractiveButton ShowSUVRoomsFilter;
        private InteractiveButton ShowDMRoomsFilter;
        private InteractiveButton ShowCTFRoomsFilter;

        private InteractiveButton ShowAllRoomsFilter;

        private InteractiveButton InfoButton;
        private InteractiveButton RankingButton;
        private InteractiveButton OptionsButton;
        private InteractiveButton HelpButton;

        private InteractiveButton ShowAllPlayersFilter;
        private InteractiveButton ShowFriendsFilter;
        private InteractiveButton ShowGuildsFilter;
        private InteractiveButton SearchPlayerButton;

        private InteractiveButton ShopButton;
        private InteractiveButton MetalButton;

        private AnimatedSprite sprRoom;
        private AnimatedSprite sprRoomState;
        private AnimatedSprite sprRoomType;

        private InteractiveButton[] ArrayMenuButton;

        #endregion

        private readonly TripleThunderOnlineClient OnlineGameClient;
        private readonly CommunicationClient OnlineCommunicationClient;
        public readonly Dictionary<string, RoomInformations> DicAllRoom;
        private Player[] ArrayLobyPlayer;
        private string RoomType;

        public Loby()
        {
            RoomType = RoomInformations.RoomTypeMission;
            DicAllRoom = new Dictionary<string, RoomInformations>();
            ListChatHistory = new List<string>();
            ArrayLobyPlayer = new Player[0];

            Dictionary<string, OnlineScript> DicOnlineGameClientScripts = new Dictionary<string, OnlineScript>();
            Dictionary<string, OnlineScript> DicOnlineCommunicationClientScripts = new Dictionary<string, OnlineScript>();

            OnlineGameClient = new TripleThunderOnlineClient(DicOnlineGameClientScripts);
            OnlineCommunicationClient = new CommunicationClient(DicOnlineCommunicationClientScripts);

            DicOnlineGameClientScripts.Add(ConnectionSuccessScriptClient.ScriptName, new ConnectionSuccessScriptClient());
            DicOnlineGameClientScripts.Add(RedirectScriptClient.ScriptName, new RedirectScriptClient(OnlineGameClient));
            DicOnlineGameClientScripts.Add(LoginSuccessScriptClient.ScriptName, new LoginSuccessScriptClient(this));
            DicOnlineGameClientScripts.Add(RoomListScriptClient.ScriptName, new RoomListScriptClient(this));
            DicOnlineGameClientScripts.Add(JoinRoomLocalScriptClient.ScriptName, new JoinRoomLocalScriptClient(OnlineGameClient, OnlineCommunicationClient, this, false));
            DicOnlineGameClientScripts.Add(CreatePlayerScriptClient.ScriptName, new CreatePlayerScriptClient(OnlineGameClient));
            DicOnlineGameClientScripts.Add(ServerIsReadyScriptClient.ScriptName, new ServerIsReadyScriptClient());
            DicOnlineGameClientScripts.Add(JoinRoomFailedScriptClient.ScriptName, new JoinRoomFailedScriptClient(OnlineGameClient, this));

            DicOnlineCommunicationClientScripts.Add(LoginSuccessScriptClient.ScriptName, new LoginSuccessScriptClient(this));
            DicOnlineCommunicationClientScripts.Add(ReceiveGlobalMessageScriptClient.ScriptName, new ReceiveGlobalMessageScriptClient(OnlineCommunicationClient, this));
            DicOnlineCommunicationClientScripts.Add(PlayerListScriptClient.ScriptName, new PlayerListScriptClient(OnlineCommunicationClient, this));
        }

        public override void Load()
        {
            Trace.Listeners.Add(new TextWriterTraceListener("ClientError.log", "myListener"));

            InitOnlineGameClient();
            InitOnlineCommunicationClient();

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

            CreateARoomButton = new InteractiveButton(Content, "Triple Thunder/Menus/Channel/Create A Room Button", new Vector2(190, 99),
                                                                    "Triple Thunder/Menus/Channel/Create A Room Icon", new Vector2(-25, -9), 20, OnButtonOver, CreateARoom);
            QuickStartButton = new InteractiveButton(Content, "Triple Thunder/Menus/Channel/Quick Start Button", new Vector2(87, 98),
                                                                    "Triple Thunder/Menus/Channel/Quick Start Icon", new Vector2(-25, 2), 7, OnButtonOver, null);
            WaitingRoomOnlyButton = new InteractiveButton(Content, "Triple Thunder/Menus/Channel/Waiting Room Only", new Vector2(447, 85), OnButtonOver, null);

            ShowSUVRoomsFilter = new InteractiveButton(Content, "Triple Thunder/Menus/Channel/Show SUV Rooms", new Vector2(327, 125), OnButtonOver, null);
            ShowDMRoomsFilter = new InteractiveButton(Content, "Triple Thunder/Menus/Channel/Show DM Rooms", new Vector2(390, 125), OnButtonOver, null);
            ShowCTFRoomsFilter = new InteractiveButton(Content, "Triple Thunder/Menus/Channel/Show CTR Rooms", new Vector2(453, 125), OnButtonOver, null);
            ShowAllRoomsFilter = new InteractiveButton(Content, "Triple Thunder/Menus/Channel/Show All Rooms", new Vector2(511, 125), OnButtonOver, null);

            InfoButton = new InteractiveButton(Content, "Triple Thunder/Menus/Channel/Info Button", new Vector2(622, 63),
                                                            "Triple Thunder/Menus/Channel/Info Icon", new Vector2(-32, -6), 11, OnButtonOver, null);
            RankingButton = new InteractiveButton(Content, "Triple Thunder/Menus/Channel/Ranking Button", new Vector2(732, 63),
                                                                "Triple Thunder/Menus/Channel/Ranking Icon", new Vector2(-32, -2), 10, OnButtonOver, null);
            OptionsButton = new InteractiveButton(Content, "Triple Thunder/Menus/Channel/Option Button", new Vector2(622, 97),
                                                                "Triple Thunder/Menus/Channel/Option Icon", new Vector2(-32, 0), 6, OnButtonOver, null);
            HelpButton = new InteractiveButton(Content, "Triple Thunder/Menus/Channel/Help Button", new Vector2(732, 97),
                                                            "Triple Thunder/Menus/Channel/Help Icon", new Vector2(-32, 0), 8, OnButtonOver, null);

            ShowAllPlayersFilter = new InteractiveButton(Content, "Triple Thunder/Menus/Channel/All Players Filter", new Vector2(612, 148), OnButtonOver, null);
            ShowFriendsFilter = new InteractiveButton(Content, "Triple Thunder/Menus/Channel/Friends Filter", new Vector2(665, 148), OnButtonOver, null);
            ShowGuildsFilter = new InteractiveButton(Content, "Triple Thunder/Menus/Channel/Guild Filter", new Vector2(723, 148), OnButtonOver, null);
            SearchPlayerButton = new InteractiveButton(Content, "Triple Thunder/Menus/Channel/Search Button", new Vector2(750, 472), OnButtonOver, null);
            
            MetalButton = new InteractiveButton(Content, "Triple Thunder/Menus/Channel/Medal Button", new Vector2(623, 514),
                                                            "Triple Thunder/Menus/Channel/Medal Icon", new Vector2(-29, 0), 6, OnButtonOver, null);
            ShopButton = new InteractiveButton(Content, "Triple Thunder/Menus/Channel/Shop Button", new Vector2(733, 514),
                                                            "Triple Thunder/Menus/Channel/Shop Icon", new Vector2(-25, 0), 7, OnButtonOver, OpenShop);

            sprRoom = new AnimatedSprite(Content, "Triple Thunder/Menus/Channel/Room", new Vector2(0, 0), 0, 1, 4);
            sprRoom.SetFrame(2);
            sprRoomState = new AnimatedSprite(Content, "Triple Thunder/Menus/Channel/Room State_strip2", new Vector2(0, 0), 0);
            sprRoomType = new AnimatedSprite(Content, "Triple Thunder/Menus/Channel/Room Type", new Vector2(0, 0), 0, 3, 1);

            ArrayMenuButton = new InteractiveButton[]
            {
                CreateARoomButton, QuickStartButton, WaitingRoomOnlyButton,
                ShowSUVRoomsFilter, ShowDMRoomsFilter, ShowCTFRoomsFilter, ShowAllRoomsFilter,
                InfoButton, RankingButton, OptionsButton, HelpButton,
                ShowAllPlayersFilter, ShowFriendsFilter, ShowGuildsFilter, SearchPlayerButton,
                ShopButton, MetalButton,
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
                        PlayerManager.ListLocalPlayer.Add(new Player(PlayerManager.OnlinePlayerID, PlayerManager.OnlinePlayerName, Player.PlayerTypes.Offline, false, 0));
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
                    if (ListServerIP.Count == 0)
                    {
                        TryConnecting = false;
                        PlayerManager.ListLocalPlayer.Add(new Player(PlayerManager.OnlinePlayerID, PlayerManager.OnlinePlayerName, Player.PlayerTypes.Offline, false, 0));
                        PlayerManager.ListLocalPlayer[0].LoadLocally(Content);
                    }
                }
            }
            while (TryConnecting);
        }

        public void IdentifyToCommunicationServer(string PlayerName, byte[] PlayerInfo)
        {
            if (OnlineCommunicationClient.Host != null)
            {
                OnlineCommunicationClient.Host.Send(new IdentifyScriptClient(PlayerName, PlayerInfo));
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
            OnlineGameClient.ExecuteDelayedScripts();
            OnlineCommunicationClient.ExecuteDelayedScripts();

            if (FMODSystem.sndActiveBGM != sndBGM)
            {
                sndBGM.PlayAsBGM();
            }

            ChatInput.Update(gameTime);

            Rectangle LicenseBox = new Rectangle(572, 16, 24, 24);
            if (LicenseBox.Intersects(new Rectangle(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y, 1, 1)))
            {
                if (MouseHelper.InputLeftButtonPressed())
                {
                    if (RoomType == RoomInformations.RoomTypeMission)
                    {
                        RoomType = RoomInformations.RoomTypeBattle;
                    }
                    else
                    {
                        RoomType = RoomInformations.RoomTypeMission;
                    }
                }
            }

            int i = 0;
            foreach (KeyValuePair<string, RoomInformations> ActiveRoom in DicAllRoom)
            {
                int X = 148 + (i % 2) * 248;
                int Y = 169 + (i / 2) * 60;

                if (MouseHelper.MouseStateCurrent.X >= X - sprRoom.Origin.X && MouseHelper.MouseStateCurrent.X <= X + sprRoom.Origin.X
                    && MouseHelper.MouseStateCurrent.Y >= Y - sprRoom.Origin.Y && MouseHelper.MouseStateCurrent.Y <= Y + sprRoom.Origin.Y)
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

            foreach (InteractiveButton ActiveButton in ArrayMenuButton)
            {
                ActiveButton.Update(gameTime);
            }
        }

        internal void UpdateRooms(List<RoomInformations> ListRoomUpdates)
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

        private void CreateARoom()
        {
            sndButtonClick.Play();
            if (RoomType == RoomInformations.RoomTypeMission)
            {
                PushScreen(new CreateRoomMission(OnlineGameClient, OnlineCommunicationClient, RoomType));
            }
            else
            {
                PushScreen(new CreateRoomBattle(OnlineGameClient, OnlineCommunicationClient, RoomType));
            }
        }

        private void OpenShop()
        {
            sndButtonClick.Play();
            PushScreen(new Shop(PlayerManager.ListLocalPlayer[0]));
        }

        public void AddMessage(string Message, Color MessageColor)
        {
            ListChatHistory.Add(Message);
        }

        private void SendMessage(string InputMessage)
        {
            ChatInput.SetText(string.Empty);
            OnlineCommunicationClient.Host.Send(new SendGlobalMessageScriptClient(InputMessage, 0, 0, 0));
        }

        public void PopulatePlayerNames(Player[] ArrayPlayerName)
        {
            this.ArrayLobyPlayer = ArrayPlayerName;
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.Draw(sprBackground, Vector2.Zero, Color.White);
            if (RoomType == RoomInformations.RoomTypeMission)
            {
                g.Draw(sprLicenseAll, new Rectangle(572, 16, 24, 24), new Rectangle(1 * 24, 0, 24, 24), Color.White);
            }
            else
            {
                g.Draw(sprTitleBattle, new Vector2(160, 16), Color.White);
                g.Draw(sprLicenseAll, new Rectangle(572, 16, 24, 24), new Rectangle(2 * 24, 0, 24, 24), Color.White);
            }

            g.DrawString(fntArial12, "Lv." + PlayerManager.OnlinePlayerLevel, new Vector2(610, 17), Color.White);
            g.DrawString(fntArial12, PlayerManager.OnlinePlayerName, new Vector2(670, 15), Color.White);

            g.End();
            g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            ChatInput.Draw(g);

            int i = 0;
            foreach (RoomInformations ActiveRoom in DicAllRoom.Values)
            {
                float X = 148 + (i % 2) * 248;
                float Y = 169 + (i / 2) * 60;

                if (MouseHelper.MouseStateCurrent.X >= X - sprRoom.Origin.X && MouseHelper.MouseStateCurrent.X <= X + sprRoom.Origin.X
                    && MouseHelper.MouseStateCurrent.Y >= Y - sprRoom.Origin.Y && MouseHelper.MouseStateCurrent.Y <= Y + sprRoom.Origin.Y)
                {
                    sprRoom.SetFrame(2);
                }
                else
                {
                    sprRoom.SetFrame(0);
                }

                sprRoom.Draw(g, new Vector2(X, Y), Color.White);
                g.DrawString(fntArial12, ActiveRoom.RoomName, new Vector2(X - 70, Y - 18), Color.White);
                g.DrawString(fntArial12, ActiveRoom.CurrentPlayerCount.ToString(), new Vector2(X - 5, Y), Color.White);
                g.DrawString(fntArial12, ActiveRoom.MaxNumberOfPlayer.ToString(), new Vector2(X + 20, Y), Color.White);

                if (ActiveRoom.IsPlaying)
                {
                    sprRoomState.SetFrame(0);
                }
                else
                {
                    sprRoomState.SetFrame(1);
                }
                sprRoomState.Draw(g, new Vector2(X + 65, Y + 8), Color.White);

                if (ActiveRoom.RoomSubtype == "Deathmatch")
                {
                    sprRoomType.SetFrame(0);
                    sprRoomType.Draw(g, new Vector2(X - 38, Y + 14), Color.White);
                }
                else if (ActiveRoom.RoomSubtype == "Capture The Flag")
                {
                    sprRoomType.SetFrame(1);
                    sprRoomType.Draw(g, new Vector2(X - 38, Y + 14), Color.White);
                }
                else if (ActiveRoom.RoomSubtype == "Survival")
                {
                    sprRoomType.SetFrame(2);
                    sprRoomType.Draw(g, new Vector2(X - 38, Y + 14), Color.White);
                }
                ++i;
            }

            foreach (InteractiveButton ActiveButton in ArrayMenuButton)
            {
                ActiveButton.Draw(g);
            }

            for (int P = 0; P < ArrayLobyPlayer.Length; P++)
            {
                float X = 635;
                float Y = 166 + P * fntArial12.LineSpacing;
                g.DrawString(fntArial12, ArrayLobyPlayer[P].Name, new Vector2(X, Y), Color.White);
            }

            for (int M = 0; M < ListChatHistory.Count; M++)
            {
                float X = 30;
                float Y = 430 + M * fntArial12.LineSpacing;
                g.DrawString(fntArial12, ListChatHistory[M], new Vector2(X, Y), Color.White);
            }
        }
    }
}
