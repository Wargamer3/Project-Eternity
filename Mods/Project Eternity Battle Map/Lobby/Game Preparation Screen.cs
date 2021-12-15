using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FMOD;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
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

        #region Ressources

        private FMODSound sndBGM;
        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntText;
        private Texture2D fntTest;

        private Texture2D sprHostText;
        private Texture2D sprReadyText;

        private TextInput ChatInput;

        private BoxButton RoomSettingButton;
        private BoxButton PlayerSettingsButton;

        private BoxButton ReadyButton;
        private BoxButton StartButton;
        private BoxButton BackToLobbyButton;

        private AnimatedSprite sprTabChat;

        private IUIElement[] ArrayMenuButton;

        #endregion

        private readonly RoomInformations Room;
        private Point MapSize;
        private List<int> ListMapTeam;
        private List<TeamInfo> ListAllTeamInfo;
        private int SelectingTeam;
        public Texture2D sprMapImage;

        private readonly BattleMapOnlineClient OnlineGameClient;
        private readonly CommunicationClient OnlineCommunicationClient;
        private bool IsHost;

        public GamePreparationScreen(BattleMapOnlineClient OnlineGameClient, CommunicationClient OnlineCommunicationClient, RoomInformations Room)
        {
            this.OnlineGameClient = OnlineGameClient;
            this.OnlineCommunicationClient = OnlineCommunicationClient;
            this.Room = Room;
            SelectingTeam = -1;

            if (Room.ListRoomPlayer.Count == 0)
            {
                foreach (BattleMapPlayer ActivePlayer in PlayerManager.ListLocalPlayer)
                {
                    ActivePlayer.OnlinePlayerType = BattleMapPlayer.PlayerTypeHost;
                    Room.AddLocalPlayer(ActivePlayer);
                }
            }
        }

        public override void Load()
        {
            #region Ressources

            sndBGM = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/Music/Wait Room.mp3");
            sndBGM.SetLoop(true);
            sndBGM.PlayAsBGM();

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            fntText = Content.Load<SpriteFont>("Fonts/Arial10");
            ChatInput = new TextInput(fntText, sprPixel, sprPixel, new Vector2(68, 518), new Vector2(470, 20), SendMessage);

            fntTest = Content.Load<Texture2D>("Triple Thunder/HUD/Menus/Numbers White");
            sprHostText = Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Player Host Text");
            sprReadyText = Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Player Ready Text");

            int LeftSideWidth = (int)(Constants.Width * 0.7);
            int RoomNameHeight = (int)(Constants.Height * 0.05);
            int PlayerZoneY = RoomNameHeight;
            int PlayerZoneHeight = (int)(Constants.Height * 0.65);
            int ChatZoneY = PlayerZoneY + PlayerZoneHeight;
            int ChatZoneHeight = Constants.Height - ChatZoneY;
            int RightSideWidth = Constants.Width - LeftSideWidth;
            int MapDetailTextY = (int)(Constants.Height * 0.3);
            int MapDetailTextHeight = (int)(Constants.Height * 0.3);
            int RoomOptionWidth = (int)((RightSideWidth - 15) * 0.5);
            int RoomOptionHeight = (int)(Constants.Height * 0.08);
            RoomSettingButton = new BoxButton(new Rectangle(LeftSideWidth + 5, MapDetailTextY + 5, RoomOptionWidth, RoomOptionHeight), fntText, "Room Settings", OnButtonOver, OpenRoomSettingsScreen);
            PlayerSettingsButton = new BoxButton(new Rectangle(LeftSideWidth + 5 + RoomOptionWidth + 5, MapDetailTextY + 5, RoomOptionWidth, RoomOptionHeight), fntText, "Player Settings", OnButtonOver, PlayerSettingsScreen);

            ReadyButton = new BoxButton(new Rectangle(LeftSideWidth + 5, Constants.Height - RoomOptionHeight - 5, RoomOptionWidth, RoomOptionHeight), fntText, "Ready", OnButtonOver, Ready);
            StartButton = new BoxButton(new Rectangle(LeftSideWidth + 5, Constants.Height - RoomOptionHeight - 5, RoomOptionWidth, RoomOptionHeight), fntText, "Start", OnButtonOver, StartGame);
            BackToLobbyButton = new BoxButton(new Rectangle(LeftSideWidth + 5 + RoomOptionWidth + 5, Constants.Height - RoomOptionHeight - 5, RoomOptionWidth, RoomOptionHeight), fntText, "Leave", OnButtonOver, ReturnToLobby);

            sprTabChat = new AnimatedSprite(Content, "Triple Thunder/Menus/Channel/Tab Chat", new Vector2(0, 0), 0, 1, 4);

            #endregion

            StartButton.Disable();

            UpdateReadyOrHost();

            ListMapTeam = new List<int>();
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

            if (SelectingTeam == -1 && Room.UseTeams && ListMapTeam.Count > 0 && MouseHelper.InputLeftButtonPressed())
            {
                int DrawX = 10;
                int DrawY = 45;
                int PlayerIndex = (MouseHelper.MouseStateCurrent.Y - DrawY) / 45;
                if (PlayerIndex >= 0 && PlayerIndex < Room.ListRoomPlayer.Count)
                {
                    Rectangle TeamCollisionBox = new Rectangle(DrawX + 280, DrawY + PlayerIndex * 45, 80, 25);
                    if (PlayerManager.ListLocalPlayer.Contains(Room.ListRoomPlayer[PlayerIndex]) && TeamCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                    {
                        SelectingTeam = PlayerIndex;
                    }
                }
            }
            else if (SelectingTeam != -1 && MouseHelper.InputLeftButtonPressed())
            {
                int DrawX = 10;
                int DrawY = 45 + 30 + SelectingTeam * 45;
                int TeamIndex = (MouseHelper.MouseStateCurrent.Y - DrawY) / 25;
                if (TeamIndex >= 0 && TeamIndex < ListMapTeam.Count)
                {
                    Rectangle TeamCollisionBox = new Rectangle(DrawX + 285, DrawY + TeamIndex * 25, 85, 25);
                    if (TeamCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                    {
                        Room.ListRoomPlayer[SelectingTeam].Team = TeamIndex;
                    }
                }
                SelectingTeam = -1;
            }
        }

        private void AssignButtons()
        {
            IsHost = false;
            foreach (BattleMapPlayer ActivePlayer in Room.GetLocalPlayers())
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

        public void AddPlayer(BattleMapPlayer NewPlayer)
        {
            Room.ListRoomPlayer.Add(NewPlayer);
            
            UpdateReadyOrHost();
        }

        public void UpdateSelectedMap(string MapName, string MapType, string MapPath, byte MinNumberOfPlayer, byte MaxNumberOfPlayer)
        {
            Room.MapName = MapName;
            Room.MapType = MapType;
            Room.MapPath = MapPath;
            LoadMapInfo();
            Room.MinNumberOfPlayer = MinNumberOfPlayer;
            Room.MaxNumberOfPlayer = MaxNumberOfPlayer;
        }

        private void LoadMapInfo()
        {
            FileStream FS = new FileStream("Content/Maps/" + Room.MapType + "/" + Room.MapPath + ".pem", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            MapSize.X = BR.ReadInt32();
            MapSize.Y = BR.ReadInt32();
            int TileSizeX = BR.ReadInt32();
            int TileSizeY = BR.ReadInt32();

            int CameraPositionX = BR.ReadInt32();
            int CameraPositionY = BR.ReadInt32();

            Room.MinNumberOfPlayer = BR.ReadByte();
            Room.MaxNumberOfPlayer = BR.ReadByte();

            string Description = BR.ReadString();

            int ListBackgroundsPathCount = BR.ReadInt32();
            for (int B = 0; B < ListBackgroundsPathCount; B++)
            {
                string BackgroundsPath = BR.ReadString();
            }
            int ListForegroundsPathCount = BR.ReadInt32();
            for (int F = 0; F < ListForegroundsPathCount; F++)
            {
                string ForegroundsPath = BR.ReadString();
            }

            Color[] ArrayColor = new Color[16];
            //Deathmatch colors
            for (int D = 0; D < 16; D++)
            {
                ArrayColor[D].R = BR.ReadByte();
                ArrayColor[D].G = BR.ReadByte();
                ArrayColor[D].B = BR.ReadByte();
            }

            int ListSingleplayerSpawnsCount = BR.ReadInt32();

            for (int S = 0; S < ListSingleplayerSpawnsCount; S++)
            {
                EventPoint NewPoint = new EventPoint(BR);
            }

            int ListSpawnsCount = BR.ReadInt32();
            List<EventPoint> ListSpawns = new List<EventPoint>(ListSpawnsCount);

            ListMapTeam.Clear();
            for (int S = 0; S < ListSpawnsCount; S++)
            {
                EventPoint NewPoint = new EventPoint(BR);
                int ColorIndex = 1;

                if (!string.IsNullOrWhiteSpace(NewPoint.Tag))
                    ColorIndex = Convert.ToInt32(NewPoint.Tag);

                if (!ListMapTeam.Contains(ColorIndex))
                {
                    ListMapTeam.Add(ColorIndex);
                }
            }

            FS.Close();
            BR.Close();
        }

        private void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        public void UpdateCharacter(BattleMapPlayer PlayerToUpdate)
        {
            if (Room.GetLocalPlayer() == PlayerToUpdate)
            {
            }
        }

        public void UpdateReadyOrHost()
        {
            AssignButtons();

            if (IsHost)
            {
                bool IsEveryoneReady = true;

                foreach (BattleMapPlayer ActivePlayer in Room.ListRoomPlayer)
                {
                    if (ActivePlayer.OnlinePlayerType != BattleMapPlayer.PlayerTypeHost && ActivePlayer.OnlinePlayerType != BattleMapPlayer.PlayerTypeReady)
                    {
                        IsEveryoneReady = false;
                    }
                }

                if (IsEveryoneReady && Room.MapPath != null)
                {
                    StartButton.Enable();
                }
                else
                {
                    StartButton.Disable();
                }
            }
            else
            {
                ReadyButton.Enable();
            }
        }

        private void SendMessage(string InputMessage)
        {
            ChatInput.SetText(string.Empty);
            OnlineCommunicationClient.SendMessage(OnlineCommunicationClient.Chat.ActiveTabID, new ChatManager.ChatMessage(DateTime.UtcNow, InputMessage, ChatManager.MessageColors.White));
        }

        public void OptionsClosed()
        {
            UpdateReadyOrHost();

            if (OnlineGameClient != null)
            {
                ReadyButton.Disable();

                OnlineGameClient.Host.Send(new AskChangeMapScriptClient(Room.MapName, Room.MapType, Room.MapPath, Room.MinNumberOfPlayer, Room.MaxNumberOfPlayer));
            }
        }

        #region Button methods

        private void OpenRoomSettingsScreen()
        {
            PushScreen(new GameOptionsScreen(Room, this));
        }

        private void PlayerSettingsScreen()
        {

        }

        private void ReturnToLobby()
        {
            if (OnlineGameClient != null && OnlineGameClient.IsConnected)
            {
                OnlineGameClient.Host.Send(new LeaveRoomScriptClient());
            }

            if (OnlineCommunicationClient != null && OnlineCommunicationClient.IsConnected)
            {
                OnlineCommunicationClient.Host.Send(new LeaveCommunicationGroupScriptClient(Room.RoomID));
                OnlineCommunicationClient.Chat.CloseTab(Room.RoomID);
            }

            sndButtonClick.Play();
            RemoveScreen(this);
        }

        private void StartGame()
        {
            sndButtonClick.Play();

            if (OnlineGameClient != null && OnlineGameClient.IsConnected)
            {
                OnlineGameClient.StartGame();
            }
            else
            {
                Dictionary<string, List<Squad>> DicSpawnSquadByPlayer = new Dictionary<string, List<Squad>>();
                for (int P = 0; P < Room.ListRoomPlayer.Count; ++P)
                {
                    DicSpawnSquadByPlayer.Add(Room.ListRoomPlayer[P].Name, Room.ListRoomPlayer[P].Inventory.ActiveLoadout.ListSquad);
                }

                BattleMap NewMap;

                if (Room.MapPath == "Random")
                {
                    NewMap = BattleMap.DicBattmeMapType[Room.MapType].GetNewMap(Room.RoomType);
                }
                else
                {
                    NewMap = BattleMap.DicBattmeMapType[Room.MapType].GetNewMap(Room.RoomType);
                }

                NewMap.BattleMapPath = Room.MapPath;
                NewMap.DicSpawnSquadByPlayer = DicSpawnSquadByPlayer;
                NewMap.ListGameScreen = ListGameScreen;

                for (int P = 0; P < 10; P++)
                {
                    if (P < PlayerManager.ListLocalPlayer.Count)
                    {
                        BattleMapPlayer ActivePlayer = PlayerManager.ListLocalPlayer[P];
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

                if (Room.GetLocalPlayer().OnlinePlayerType == BattleMapPlayer.PlayerTypePlayer)
                {
                    OnlineGameClient.Host.Send(new AskChangePlayerTypeScriptClient(BattleMapPlayer.PlayerTypeReady));
                }
                else
                {
                    OnlineGameClient.Host.Send(new AskChangePlayerTypeScriptClient(BattleMapPlayer.PlayerTypePlayer));
                }
            }
        }

        #endregion

        public override void Draw(CustomSpriteBatch g)
        {
            g.End();
            g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            int LeftSideWidth = (int)(Constants.Width * 0.7);
            int RoomNameHeight = (int)(Constants.Height * 0.05);
            int PlayerZoneY = RoomNameHeight;
            int PlayerZoneHeight = (int)(Constants.Height * 0.65);
            int ChatZoneY = PlayerZoneY + PlayerZoneHeight;
            int ChatZoneHeight = Constants.Height - ChatZoneY;

            DrawBox(g, new Vector2(0, 0), LeftSideWidth, RoomNameHeight, Color.White);
            g.DrawString(fntText, "Room Name:", new Vector2(5, 7), Color.White);
            g.DrawString(fntText, Room.RoomName, new Vector2(95, 7), Color.White);
            DrawBox(g, new Vector2(0, PlayerZoneY), LeftSideWidth, PlayerZoneHeight, Color.White);
            DrawBox(g, new Vector2(0, ChatZoneY), LeftSideWidth, ChatZoneHeight, Color.White);
            g.DrawString(fntText, "Chat", new Vector2(10, ChatZoneY + 10), Color.White);


            int RightSideWidth = Constants.Width - LeftSideWidth;
            int MapDetailTextY = (int)(Constants.Height * 0.3);
            int MapDetailTextHeight = (int)(Constants.Height * 0.3);
            int RoomOptionHeight = (int)(Constants.Height * 0.08);

            DrawBox(g, new Vector2(LeftSideWidth, 0), RightSideWidth, Constants.Height, Color.White);
            g.DrawString(fntText, "Player Info", new Vector2(LeftSideWidth + 10, 7), Color.White);
            DrawBox(g, new Vector2(LeftSideWidth, MapDetailTextY), RightSideWidth, MapDetailTextHeight, Color.White);

            int GameModeY = MapDetailTextY + RoomOptionHeight;

            g.DrawString(fntText, "Game Mode:", new Vector2(LeftSideWidth + 10, GameModeY + 10), Color.White);
            g.DrawStringMiddleAligned(fntText, "Campaign", new Vector2(LeftSideWidth + RightSideWidth - 45, GameModeY + 10), Color.White);
            GameModeY += 15;
            g.DrawString(fntText, "Players:", new Vector2(LeftSideWidth + 10, GameModeY + 10), Color.White);
            if (Room.MinNumberOfPlayer != Room.MaxNumberOfPlayer)
            {
                g.DrawStringMiddleAligned(fntText, Room.MinNumberOfPlayer + " - " + Room.MaxNumberOfPlayer, new Vector2(LeftSideWidth + RightSideWidth - 45, GameModeY + 10), Color.White);
            }
            else
            {
                g.DrawStringMiddleAligned(fntText, Room.MinNumberOfPlayer.ToString(), new Vector2(LeftSideWidth + RightSideWidth - 45, GameModeY + 10), Color.White);
            }
            GameModeY += 15;
            g.DrawString(fntText, "Map Size:", new Vector2(LeftSideWidth + 10, GameModeY + 10), Color.White);
            g.DrawStringMiddleAligned(fntText, MapSize.X + " x " + MapSize.Y, new Vector2(LeftSideWidth + RightSideWidth - 45, GameModeY + 10), Color.White);
            GameModeY += 110;
            g.Draw(sprMapImage, new Vector2(LeftSideWidth + (RightSideWidth - sprMapImage.Width) / 2, GameModeY), Color.White);
            GameModeY += 85;
            g.DrawString(fntText, "Map:", new Vector2(LeftSideWidth + 10, GameModeY + 10), Color.White);
            if (Room.MapPath != null)
            {
                g.DrawStringMiddleAligned(fntText, Room.MapName, new Vector2(LeftSideWidth + RightSideWidth - 45, GameModeY + 10), Color.White);
            }
            GameModeY += 15;
            g.DrawString(fntText, "Details:", new Vector2(LeftSideWidth + 10, GameModeY + 10), Color.White);
            GameModeY += 15;
            g.DrawString(fntText, "Tutorial map aimed to introduce the basics of the\r\ncampaign mode", new Vector2(LeftSideWidth + 15, GameModeY + 10), Color.White);


            foreach (IUIElement ActiveButton in ArrayMenuButton)
            {
                ActiveButton.Draw(g);
            }

            if (OnlineCommunicationClient != null)
            {
                ChatHelper.DrawChat(g, fntText, OnlineCommunicationClient.Chat, ChatInput);
            }

            for (int P = 0; P < Room.ListRoomPlayer.Count; ++P)
            {
                int DrawX = 10;
                int DrawY = 45 + P * 45;

                DrawBox(g, new Vector2(DrawX - 5, DrawY - 10), LeftSideWidth - 10, 45, Color.White);
                DrawPlayerBox(g, DrawX, DrawY, P);
            }
            if (SelectingTeam >= 0)
            {
                int DrawX = 10;
                int DrawY = 45 + SelectingTeam * 45;
                DrawBox(g, new Vector2(DrawX + 280, DrawY + 25), 85, 5 + 25 * ListMapTeam.Count, Color.White);
                for (int T = 0; T < ListMapTeam.Count; T++)
                {
                    Rectangle TeamCollisionBox = new Rectangle(DrawX + 285, DrawY + 30 + T * 25, 85, 25);
                    if (TeamCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                    {
                        g.Draw(sprPixel, new Rectangle(DrawX + 285, DrawY + 28 + T * 25, 75, 22), Color.FromNonPremultiplied(255, 255, 255, 127));
                    }
                    g.DrawString(fntText, ListAllTeamInfo[T].TeamName, new Vector2(DrawX + 285, DrawY + 30 + T * 25), Color.White);
                }
            }
        }

        private void DrawPlayerBox(CustomSpriteBatch g, int DrawX, int DrawY, int PlayerToDrawIndex)
        {
            BattleMapPlayer PlayerToDraw = Room.ListRoomPlayer[PlayerToDrawIndex];

            DrawBox(g, new Vector2(DrawX, DrawY), 50, 25, Color.White);
            DrawBox(g, new Vector2(DrawX + 50, DrawY), 50, 25, Color.White);
            DrawBox(g, new Vector2(DrawX + 100, DrawY), 180, 25, Color.White);
            if (Room.UseTeams && ListMapTeam.Count > 0)
            {
                DrawBox(g, new Vector2(DrawX + 280, DrawY), 85, 25, Color.White);
                g.DrawStringMiddleAligned(fntText, ListAllTeamInfo[PlayerToDraw.Team].TeamName, new Vector2(DrawX + 322, DrawY + 5), Color.White);
            }

            g.DrawString(fntText, "Lv. 50", new Vector2(DrawX + 57, DrawY + 5), Color.White);
            g.DrawString(fntText, PlayerToDraw.Name, new Vector2(DrawX + 110, DrawY + 5), Color.White);

            for (int S = 0; S < PlayerToDraw.Inventory.ActiveLoadout.ListSquad.Count; S++)
            {
                if (PlayerToDraw.Inventory.ActiveLoadout.ListSquad[S] != null)
                {
                    g.Draw(PlayerToDraw.Inventory.ActiveLoadout.ListSquad[S][0].SpriteMap, new Rectangle(DrawX + 370 + S * 35, DrawY - 3, 32, 32), Color.White);
                }
            }

            if (PlayerToDraw.OnlinePlayerType == BattleMapPlayer.PlayerTypeHost)
            {
                g.DrawString(fntText, "Host", new Vector2(DrawX + 6, DrawY + 5), Color.White);
            }
            else if (PlayerToDraw.OnlinePlayerType == BattleMapPlayer.PlayerTypeReady)
            {
                g.DrawString(fntText, "Ready", new Vector2(DrawX + 6, DrawY + 5), Color.White);
            }
            if (SelectingTeam == -1)
            {
                Rectangle PlayerInfoCollisionBox = new Rectangle(DrawX, DrawY, 320, 25);
                Rectangle TeamCollisionBox = new Rectangle(DrawX + 280, DrawY, 85, 25);
                if (Room.UseTeams && ListMapTeam.Count > 0 && PlayerManager.ListLocalPlayer.Contains(PlayerToDraw) && TeamCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                {
                    g.Draw(sprPixel, new Rectangle(DrawX + 280, DrawY, 85, 25), Color.FromNonPremultiplied(255, 255, 255, 127));
                }
                else if (PlayerInfoCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                {
                    g.Draw(sprPixel, new Rectangle(DrawX, DrawY, 280, 25), Color.FromNonPremultiplied(255, 255, 255, 127));
                }
            }
        }
    }
}
