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
    public class Lobby : GameScreen
    {
        private enum PlayerListTypes { All, Friends, Guild }

        #region Ressources

        private FMODSound sndBGM;
        private FMODSound sndButtonOver;
        protected FMODSound sndButtonClick;

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

        private BasicEffect IndexedLinesEffect;
        private IndexedLines BackgroundGrid;

        public Vector3 BackgroundEmiterPosition;
        public Vector3[] ArrayNextPosition;
        public int CurrentPositionIndex;
        public int OldLineIndex;
        public int CurrentLineIndex;
        private int CylinderParts = 10;
        private int SegmentIncrement = 10;
        private int Segments;
        private TunnelBehaviorSpeedManager TunnelBehaviorSpeed;
        private TunnelBehaviorColorManager TunnelBehaviorColor;
        private TunnelBehaviorSizeManager TunnelBehaviorSize;
        private TunnelBehaviorRotationManager TunnelBehaviorRotation;
        private TunnelBehaviorDirectionManager TunnelBehaviorDirection;

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

            Segments = 360 / SegmentIncrement * 4;

            TunnelBehaviorSpeed = new TunnelBehaviorSpeedManager();
            TunnelBehaviorColor = new TunnelBehaviorColorManager();
            TunnelBehaviorSize = new TunnelBehaviorSizeManager();
            TunnelBehaviorRotation = new TunnelBehaviorRotationManager();
            TunnelBehaviorDirection = new TunnelBehaviorDirectionManager();

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
            IndexedLinesEffect = new BasicEffect(GraphicsDevice);
            IndexedLinesEffect.VertexColorEnabled = true;
            IndexedLinesEffect.DiffuseColor = new Vector3(1, 1, 1);

            int SegmentIncrement = 10;
            int Segments = 360 / SegmentIncrement;
            int Parts = 1 * Segments;
            int ArrayLength = (int)(Parts * 4);
            ArrayNextPosition = new Vector3[ArrayLength];
            VertexPositionColor[] ArrayBackgroundGridVertex = new VertexPositionColor[ArrayLength];
            // Initialize an array of indices of type short.
            short[] ArrayBackgroundGridIndices = new short[(ArrayBackgroundGridVertex.Length * 2) - 2];
            for (int Index = 0; Index < ArrayBackgroundGridVertex.Length; ++Index)
            {
                ArrayBackgroundGridVertex[Index] = new VertexPositionColor(
                    new Vector3(), Color.White);

                ArrayBackgroundGridIndices[Index] = (short)(Index);
            }

            BackgroundGrid = new IndexedLines(ArrayBackgroundGridVertex, ArrayBackgroundGridIndices);
            LeftSideWidth = Constants.Width - 290;
            TopSectionHeight = 60;
            MiddleSectionY = TopSectionHeight;
            MiddleSectionHeight = 65;

            RoomSectionY = (int)(MiddleSectionY + MiddleSectionHeight);
            BottomSectionY = 700;

            RoomSectionHeight = BottomSectionY - RoomSectionY;

            RightSideWidth = Constants.Width - LeftSideWidth - 1;
            PlayerInfoHeight = 125;
            InventoryShopListHeight = 100;
            InventoryShopListY = Constants.Height - InventoryShopListHeight - 1;
            PlayerListY = PlayerInfoHeight;
            PlayerListHeight = Constants.Height - InventoryShopListHeight - PlayerInfoHeight - 1;

            Trace.Listeners.Add(new TextWriterTraceListener("ClientError.log", "myListener"));

            PlayerManager.DicUnitType = Unit.LoadAllUnits();
            PlayerManager.DicRequirement = BaseSkillRequirement.LoadAllRequirements();
            PlayerManager.DicEffect = BaseEffect.LoadAllEffects();
            PlayerManager.DicAutomaticSkillTarget = AutomaticSkillTargetType.LoadAllTargetTypes();
            PlayerManager.DicManualSkillTarget = ManualSkillTarget.LoadAllTargetTypes();

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            ChatInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(15, Constants.Height - 26), new Vector2(470, 20), SendMessage);

            sndBGM = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/Music/Channel.mp3");
            sndBGM.SetLoop(true);
            sndBGM.PlayAsBGM();

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            sprBackground = Content.Load<Texture2D>("Triple Thunder/Menus/Channel/Background");
            sprLicenseAll = Content.Load<Texture2D>("Triple Thunder/Menus/Channel/License All");
            sprTitleBattle = Content.Load<Texture2D>("Triple Thunder/Menus/Channel/Title Battle");

            LocalPlayerSelectionButton = new EmptyBoxButton(new Rectangle(400, 8, 120, 45), fntArial12, "Local Players\r\nManagement", OnButtonOver, SelectLocalPlayers);

            QuickStartButton = new EmptyBoxButton(new Rectangle(47, 70, 100, 45), fntArial12, "Quick Start", OnButtonOver, null);
            CreateARoomButton = new EmptyBoxButton(new  Rectangle(150, 70, 100, 45), fntArial12, "Create\n\ra Room", OnButtonOver, CreateARoom);
            WaitingRoomOnlyButton = new InteractiveButton(Content, "Triple Thunder/Menus/Channel/Waiting Room Only", new Vector2(447, 85), OnButtonOver, null);

            InfoButton = new EmptyBoxButton(new Rectangle(LeftSideWidth + 10, 40, 100, 35), fntArial12, "Info", OnButtonOver, OpenInfo);
            RankingButton = new EmptyBoxButton(new Rectangle(Constants.Width - 120, 40, 100, 35), fntArial12, "Ranking", OnButtonOver, null);
            OptionsButton = new EmptyBoxButton(new Rectangle(LeftSideWidth + 10, 80, 100, 35), fntArial12, "Options", OnButtonOver, null);
            HelpButton = new EmptyBoxButton(new Rectangle(Constants.Width - 120, 80, 100, 35), fntArial12, "Help", OnButtonOver, null);

            ShowAllPlayersFilter = new EmptyBoxButton(new Rectangle(LeftSideWidth + 10, 130, 60, 25), fntArial12, "All", OnButtonOver, ShowAllPlayers);
            ShowFriendsFilter = new EmptyBoxButton(new Rectangle(LeftSideWidth + 10 + 62, 130, 60, 25), fntArial12, "Friends", OnButtonOver, ShowFriends);
            ShowGuildsFilter = new EmptyBoxButton(new Rectangle(LeftSideWidth + 10 + 62 + 62, 130, 60, 25), fntArial12, "Guild", OnButtonOver, ShowGuild);

            InventoryButton = new EmptyBoxButton(new Rectangle(LeftSideWidth + 10, InventoryShopListY + 10, 100, 45), fntArial12, "Inventory", OnButtonOver, OpenInventory);
            ShopButton = new EmptyBoxButton(new Rectangle(Constants.Width - 120, InventoryShopListY + 10, 100, 45), fntArial12, "Shop", OnButtonOver, OpenShop);

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

        private void CreateAnimatedBackground(GameTime gameTime)
        {
            Vector3 Up = Vector3.Up;

            int Parts = CylinderParts * Segments;
            int ArrayLength = Parts;

            float CylinderSize = TunnelBehaviorSize.TunnelSizeFinal;

            if (ArrayNextPosition == null || ArrayNextPosition.Length != ArrayLength)
            {
                ArrayNextPosition = new Vector3[ArrayLength];
                VertexPositionColor[] ArrayBackgroundGridVertex = new VertexPositionColor[ArrayLength];
                // Initialize an array of indices of type short.
                short[] ArrayBackgroundGridIndices = new short[(ArrayBackgroundGridVertex.Length * 2) - 2];
                for (int i = 0; i < ArrayBackgroundGridVertex.Length; ++i)
                {
                    ArrayBackgroundGridVertex[i] = new VertexPositionColor(
                        new Vector3(), Color.White);

                    ArrayBackgroundGridIndices[i] = (short)(i);
                }

                BackgroundGrid = new IndexedLines(ArrayBackgroundGridVertex, ArrayBackgroundGridIndices);
            }

            float Speed = 5;
            float SpeedX = (float)(Math.Cos(TunnelBehaviorDirection.ActiveDirection) * TunnelBehaviorSpeed.ActiveSpeed * gameTime.ElapsedGameTime.TotalSeconds);
            float SpeedY = (float)(Math.Sin(TunnelBehaviorDirection.ActiveDirection) * TunnelBehaviorSpeed.ActiveSpeed * gameTime.ElapsedGameTime.TotalSeconds);
            BackgroundEmiterPosition += new Vector3(SpeedX, SpeedY, (float)(Speed * 0.01f));

            ++CurrentPositionIndex;

            if (CurrentPositionIndex >= ArrayLength)
            {
                CurrentPositionIndex = 0;
            }

            ArrayNextPosition[CurrentPositionIndex] = BackgroundEmiterPosition;

            int NextLineIndex = (int)Math.Floor(CurrentPositionIndex / (float)Segments) * Segments;
            if (CurrentLineIndex != NextLineIndex)
            {
                OldLineIndex = CurrentLineIndex;
                TunnelBehaviorDirection.ActiveDirection = TunnelBehaviorDirection.TunnelDirectionFinal;
                TunnelBehaviorSpeed.ActiveSpeed = TunnelBehaviorSpeed.TunnelSpeedFinal;
            }

            CurrentLineIndex = NextLineIndex;

            int OldIndex = OldLineIndex;
            int Index = CurrentLineIndex;

            Color LineColor = ColorFromHSV(TunnelBehaviorColor.TunnelHueFinal, 1, 1);

            for (int X = 0; X < 360; X += SegmentIncrement)
            {
                float FinalRotation = X + TunnelBehaviorRotation.TunnelRotationFinal;
                Vector3 OldPosition = BackgroundGrid.ArrayVertex[OldIndex + 1].Position;
                Vector3 CurrentRightDistance = Vector3.Transform(Up, Matrix.CreateFromYawPitchRoll(0, 0, MathHelper.ToRadians(FinalRotation))) * CylinderSize;
                Vector3 NextRightDistance = Vector3.Transform(Up, Matrix.CreateFromYawPitchRoll(0, 0, MathHelper.ToRadians(FinalRotation + SegmentIncrement))) * CylinderSize;

                float CurrentX = BackgroundEmiterPosition.X;
                float CurrentY = BackgroundEmiterPosition.Y;
                float CurrentZ = BackgroundEmiterPosition.Z/* + X / 60f*/;

                //Draw cylinder lines
                BackgroundGrid.ArrayVertex[Index] = new VertexPositionColor(
                    OldPosition, LineColor);

                BackgroundGrid.ArrayVertex[Index + 1] = new VertexPositionColor(
                    new Vector3(CurrentX, CurrentY, CurrentZ) + CurrentRightDistance, LineColor);

                //Draw ring lines
                BackgroundGrid.ArrayVertex[Index + 2] = new VertexPositionColor(
                    new Vector3(CurrentX, CurrentY, CurrentZ) + CurrentRightDistance, LineColor);

                BackgroundGrid.ArrayVertex[Index + 3] = new VertexPositionColor(
                    new Vector3(CurrentX, CurrentY, CurrentZ) + NextRightDistance, LineColor);

                OldIndex += 4;
                Index += 4;
            }
        }

        private Color ColorFromHSV(float Hue, float Value, float Saturation)
        {
            double hh, p, q, t, ff;
            long i;
            hh = Hue;
            if (hh >= 360.0) hh = 0.0;
            hh /= 60.0;
            i = (long)hh;
            ff = hh - i;
            p = Value * (1.0 - Saturation) * 255;
            q = Value * (1.0 - (Saturation * ff)) * 255;
            t = Value * (1.0 - (Saturation * (1.0 - ff))) * 255;
            Value *= 255;

            switch (i)
            {
                case 0:
                    return Color.FromNonPremultiplied((int)Value, (int)t, (int)p, 255);
                case 1:
                    return Color.FromNonPremultiplied((int)q, (int)Value, (int)p, 255);
                case 2:
                    return Color.FromNonPremultiplied((int)p, (int)Value, (int)t, 255);
                case 3:
                    return Color.FromNonPremultiplied((int)p, (int)q, (int)Value, 255);
                case 4:
                    return Color.FromNonPremultiplied((int)t, (int)p, (int)Value, 255);
                default:
                    return Color.FromNonPremultiplied((int)Value, (int)p, (int)q, 255);

            }
        }

        public override void Update(GameTime gameTime)
        {
            TunnelBehaviorSpeed.Update(gameTime);
            TunnelBehaviorColor.Update(gameTime);
            TunnelBehaviorSize.Update(gameTime);
            TunnelBehaviorRotation.Update(gameTime);
            TunnelBehaviorDirection.Update(gameTime);

            CreateAnimatedBackground(gameTime);

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

            float aspectRatio = Constants.Width / Constants.Height;

            int DrawOffset = 700;
            int DrawLineIndex = CurrentPositionIndex - DrawOffset % ArrayNextPosition.Length;
            if (DrawLineIndex < 0)
            {
                DrawLineIndex += ArrayNextPosition.Length;
            }

            int DrawTargetLineIndex = (DrawLineIndex + 80) % ArrayNextPosition.Length;

            Vector3 position = new Vector3(ArrayNextPosition[DrawLineIndex].X,
                                            ArrayNextPosition[DrawLineIndex].Y,
                                            ArrayNextPosition[DrawLineIndex].Z);

            Vector3 target = new Vector3(ArrayNextPosition[DrawTargetLineIndex].X,
                                            ArrayNextPosition[DrawTargetLineIndex].Y,
                                            ArrayNextPosition[DrawTargetLineIndex].Z);

            Vector3 up = Vector3.Up;
            Matrix View = Matrix.CreateLookAt(position, target, up);
            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    aspectRatio,
                                                                    0.1f, 1000);

            IndexedLinesEffect.View = View;
            IndexedLinesEffect.Projection = Projection;
            IndexedLinesEffect.World = Matrix.Identity;

            foreach (EffectPass pass in IndexedLinesEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                BackgroundGrid.Draw(g);
            }

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


            LeftSideWidth = Constants.Width - 300;
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
