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

        #region Ressources

        private FMODSound sndBGM;
        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

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
        private int SelectingTeam;
        public Texture2D sprMapImage;

        private readonly BattleMapOnlineClient OnlineGameClient;
        private readonly CommunicationClient OnlineCommunicationClient;
        private bool IsHost;

        public GamePreparationScreen(BattleMapOnlineClient OnlineGameClient, CommunicationClient OnlineCommunicationClient, RoomInformations Room)
        {
            RequireDrawFocus = true;
            this.OnlineGameClient = OnlineGameClient;
            this.OnlineCommunicationClient = OnlineCommunicationClient;
            this.Room = Room;
            SelectingTeam = -1;

            Segments = 360 / SegmentIncrement * 4;

            TunnelBehaviorSpeed = new TunnelBehaviorSpeedManager();
            TunnelBehaviorColor = new TunnelBehaviorColorManager();
            TunnelBehaviorSize = new TunnelBehaviorSizeManager();
            TunnelBehaviorRotation = new TunnelBehaviorRotationManager();
            TunnelBehaviorDirection = new TunnelBehaviorDirectionManager();

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

            #region Ressources

            sndBGM = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/Music/Wait Room.mp3");
            sndBGM.SetLoop(true);
            sndBGM.PlayAsBGM();

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

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
            TunnelBehaviorSpeed.Update(gameTime);
            TunnelBehaviorColor.Update(gameTime);
            TunnelBehaviorSize.Update(gameTime);
            TunnelBehaviorRotation.Update(gameTime);
            TunnelBehaviorDirection.Update(gameTime);

            CreateAnimatedBackground(gameTime);

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

            HandleTeamSelection();
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

        private void HandleTeamSelection()
        {
            if (SelectingTeam == -1 && Room.GameInfo != null && Room.GameInfo.UseTeams && Room.ListMapTeam.Count > 0 && MouseHelper.InputLeftButtonPressed())
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

                DrawY = 45 + Room.ListRoomPlayer.Count * 45;
                PlayerIndex = (MouseHelper.MouseStateCurrent.Y - DrawY) / 45;
                if (PlayerIndex >= 0 && PlayerIndex < Room.ListRoomBot.Count)
                {
                    Rectangle TeamCollisionBox = new Rectangle(DrawX + 280, DrawY + PlayerIndex * 45, 80, 25);
                    if (TeamCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                    {
                        SelectingTeam = Room.ListRoomPlayer.Count + PlayerIndex;
                    }
                }
            }
            else if (SelectingTeam != -1 && MouseHelper.InputLeftButtonPressed())
            {
                int DrawX = 10;
                int DrawY = 45 + 30 + SelectingTeam * 45;
                int TeamIndex = (MouseHelper.MouseStateCurrent.Y - DrawY) / 25;
                if (TeamIndex >= 0 && TeamIndex < Room.ListMapTeam.Count && SelectingTeam < Room.ListRoomPlayer.Count)
                {
                    Rectangle TeamCollisionBox = new Rectangle(DrawX + 285, DrawY + TeamIndex * 25, 85, 25);
                    if (TeamCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                    {
                        Room.ListRoomPlayer[SelectingTeam].Team = TeamIndex;
                    }
                }
                else if (TeamIndex >= 0 && TeamIndex < Room.ListMapTeam.Count)
                {
                    Rectangle TeamCollisionBox = new Rectangle(DrawX + 285, DrawY + TeamIndex * 25, 85, 25);
                    if (TeamCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                    {
                        Room.ListRoomBot[SelectingTeam - Room.ListRoomPlayer.Count].Team = TeamIndex;
                    }
                }

                SelectingTeam = -1;
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
            sndButtonClick.Play();

            if (OnlineGameClient != null && OnlineGameClient.IsConnected)
            {
                OnlineGameClient.StartGame();
            }
            else
            {
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

            if (SelectingTeam >= 0)
            {
                int DrawX = 10;
                int DrawY = 45 + SelectingTeam * 45;
                DrawBox(g, new Vector2(DrawX + 280, DrawY + 25), 85, 5 + 25 * Room.ListMapTeam.Count, Color.White);
                for (int T = 0; T < Room.ListMapTeam.Count; T++)
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
            DrawEmptyBox(g, new Vector2(DrawX, DrawY), 60, 25);
            DrawEmptyBox(g, new Vector2(DrawX + 65, DrawY), 70, 25);
            DrawEmptyBox(g, new Vector2(DrawX + 140, DrawY), 200, 25);
            if (Room.GameInfo != null && Room.GameInfo.UseTeams && Room.ListMapTeam.Count > 0)
            {
                DrawBox(g, new Vector2(DrawX + 280, DrawY), 85, 25, Color.White);
                g.DrawStringMiddleAligned(fntText, ListAllTeamInfo[PlayerToDraw.Team].TeamName, new Vector2(DrawX + 322, DrawY + 5), Color.White);
            }

            g.DrawString(fntText, "Lv. 50", new Vector2(DrawX + 72, DrawY + 5), Color.White);
            g.DrawString(fntText, PlayerToDraw.Name, new Vector2(DrawX + 145, DrawY + 5), Color.White);

            for (int S = 0; S < PlayerToDraw.Inventory.ActiveLoadout.ListSpawnSquad.Count; S++)
            {
                if (PlayerToDraw.Inventory.ActiveLoadout.ListSpawnSquad[S] != null)
                {
                    g.Draw(PlayerToDraw.Inventory.ActiveLoadout.ListSpawnSquad[S].At(0).SpriteMap, new Rectangle(DrawX + 370 + S * 35, DrawY - 3, 32, 32), Color.White);
                }
            }
            if (PlayerToDraw.IsHost())
            {
                g.DrawString(fntText, "Host", new Vector2(DrawX + 6, DrawY + 5), Color.White);
            }
            else if (PlayerToDraw.IsReady())
            {
                g.DrawString(fntText, "Ready", new Vector2(DrawX + 6, DrawY + 5), Color.White);
            }

            if (SelectingTeam == -1)
            {
                Rectangle PlayerInfoCollisionBox = new Rectangle(DrawX, DrawY, 320, 25);
                Rectangle TeamCollisionBox = new Rectangle(DrawX + 280, DrawY, 85, 25);
                if (Room.GameInfo != null && Room.GameInfo.UseTeams && Room.ListMapTeam.Count > 0 && TeamCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y)
                    && (PlayerManager.ListLocalPlayer.Contains(PlayerToDraw) || Room.ListRoomBot.Contains(PlayerToDraw)))
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
