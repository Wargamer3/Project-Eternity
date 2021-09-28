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
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.TripleThunderScreen.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class MissionSelect : GameScreen, IMissionSelect
    {
        private struct MissionInfo
        {
            public readonly string MissionName;
            public readonly string MissionPath;
            public readonly string MissionDescription;
            public readonly Texture2D MissionImage;

            public MissionInfo(string MissionName, string MissionPath, string MissionDescription, Texture2D MissionImage)
            {
                this.MissionName = MissionName;
                this.MissionPath = MissionPath;
                this.MissionDescription = MissionDescription;
                this.MissionImage = MissionImage;
            }
        }

        #region Ressources

        private FMODSound sndBGM;
        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntText;

        private Texture2D sprBackground;
        private Dictionary<string, Texture2D> DicCharacterIconByName;
        private Texture2D sprActivePlayerIcon;
        private Texture2D sprHostText;
        private Texture2D sprReadyText;

        private TextInput ChatInput;

        private InteractiveButton ChangeRoomNameButton;
        private InteractiveButton CharacterSelectButton;
        private InteractiveButton ActivateItemButton;
        private InteractiveButton InviteButton;
        private InteractiveButton QuestEasyButton;
        private InteractiveButton QuestNormalButton;
        private InteractiveButton QuestHardButton;
        private InteractiveButton ReadyButton;
        private InteractiveButton StartButton;
        private InteractiveButton BackToLobbyButton;

        private Scrollbar MissionScrollbar;

        private AnimatedSprite PlayerInfo;
        private AnimatedSprite PlayerInfoOutline;
        private AnimatedSprite QuestButton;
        private AnimatedSprite QuestOutlineButton;

        private AnimatedSprite sprTabChat;

        private InteractiveButton[] ArrayMenuButton;

        #endregion

        private readonly MissionRoomInformations Room;
        private readonly List<MissionInfo> ListMissionInfo;
        private int MissionInfoStartIndex;
        private string CurrentMissionName;
        private string CurrentMissionDescription;
        private Texture2D sprCurrentMissionImage;

        public bool IsHost;

        private readonly TripleThunderOnlineClient OnlineGameClient;
        private readonly CommunicationClient OnlineCommunicationClient;

        public MissionSelect(TripleThunderOnlineClient OnlineGameClient, CommunicationClient OnlineCommunicationClient, MissionRoomInformations Room)
        {
            IsHost = true;
            this.OnlineGameClient = OnlineGameClient;
            this.OnlineCommunicationClient = OnlineCommunicationClient;
            this.Room = Room;

            ListMissionInfo = new List<MissionInfo>();
            if (Room.ListRoomPlayer.Count == 0)
            {
                PlayerManager.ListLocalPlayer[0].PlayerType = Player.PlayerTypeHost;
                Room.AddLocalPlayer(PlayerManager.ListLocalPlayer[0]);
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

            sprBackground = Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Background Mission");
            sprHostText = Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Player Host Text");
            sprReadyText = Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Player Ready Text");

            DicCharacterIconByName = new Dictionary<string, Texture2D>();
            foreach (string ActiveCharacterPath in Directory.EnumerateFiles("Content/Triple Thunder/Menus/Wait Room/Character Icons/", "*"))
            {
                string ActiveCharacter = ActiveCharacterPath.Remove(0, 55);
                ActiveCharacter = ActiveCharacter.Remove(ActiveCharacter.Length - 4);
                DicCharacterIconByName.Add(ActiveCharacter, Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Character Icons/" + ActiveCharacter));
            }

            sprActivePlayerIcon = DicCharacterIconByName["Jack"];

            ChangeRoomNameButton = new InteractiveButton(Content, "Triple Thunder/Menus/Wait Room/Change Room Name Button", new Vector2(350, 17), OnButtonOver, null);
            CharacterSelectButton = new InteractiveButton(Content, "Triple Thunder/Menus/Wait Room/Character Select Button", new Vector2(678, 160), OnButtonOver, OpenCharacterSelectScreen);
            ActivateItemButton = new InteractiveButton(Content, "Triple Thunder/Menus/Wait Room/Activate Item Button", new Vector2(628, 202), OnButtonOver, null);
            InviteButton = new InteractiveButton(Content, "Triple Thunder/Menus/Wait Room/Invite Button", new Vector2(490, 30), OnButtonOver, null);
            QuestEasyButton = new InteractiveButton(Content, "Triple Thunder/Menus/Wait Room/Quest Easy Button", new Vector2(230, 67), OnButtonOver, OnEasySelected);
            QuestNormalButton = new InteractiveButton(Content, "Triple Thunder/Menus/Wait Room/Quest Normal Button", new Vector2(340, 67), OnButtonOver, OnNormalSelected);
            QuestHardButton = new InteractiveButton(Content, "Triple Thunder/Menus/Wait Room/Quest Hard Button", new Vector2(480, 67), OnButtonOver, OnHardSelected);
            ReadyButton = new InteractiveButton(Content, "Triple Thunder/Menus/Wait Room/Ready Button", new Vector2(680, 500), 8, OnButtonOver, Ready);
            StartButton = new InteractiveButton(Content, "Triple Thunder/Menus/Wait Room/Start Button", new Vector2(680, 500), OnButtonOver, StartGame);
            BackToLobbyButton = new InteractiveButton(Content, "Triple Thunder/Menus/Common/Back To Lobby", new Vector2(678, 565),
                                                            "Triple Thunder/Menus/Common/Back Arrow Icon", new Vector2(-86, 0), 6, OnButtonOver, ReturnToLobby);

            PlayerInfo = new AnimatedSprite(Content, "Triple Thunder/Menus/Wait Room/Player Info", new Vector2(0, 0), 0, 1, 3);
            PlayerInfoOutline = new AnimatedSprite(Content, "Triple Thunder/Menus/Wait Room/Player Info Outline", new Vector2(0, 0), 0, 1, 4);
            PlayerInfoOutline.SetFrame(2);

            QuestButton = new AnimatedSprite(Content, "Triple Thunder/Menus/Wait Room/Quest Button", new Vector2(0, 0), 0, 1, 3);
            QuestOutlineButton = new AnimatedSprite(Content, "Triple Thunder/Menus/Wait Room/Quest Button Outline", new Vector2(0, 0), 0, 1, 4);
            QuestOutlineButton.SetFrame(2);

            sprTabChat = new AnimatedSprite(Content, "Triple Thunder/Menus/Channel/Tab Chat", new Vector2(0, 0), 0, 1, 4);

            QuestEasyButton.CanBeChecked = true;
            QuestNormalButton.CanBeChecked = true;
            QuestHardButton.CanBeChecked = true;

            MissionScrollbar = new Scrollbar(Content.Load<Texture2D>("Triple Thunder/Menus/Common/Scrollbar 2"), new Vector2(512, 108), 250, 0, OnMissionScrollbarChange);

            IsHost = false;
            foreach (Player ActivePlayer in Room.GetLocalPlayers())
            {
                if (ActivePlayer.IsHost())
                {
                    IsHost = true;
                }
            }

            if (IsHost)
            {
                ArrayMenuButton = new InteractiveButton[]
                {
                    ChangeRoomNameButton, CharacterSelectButton, ActivateItemButton, InviteButton,
                    QuestEasyButton, QuestNormalButton, QuestHardButton,
                    BackToLobbyButton, StartButton,
                };
            }
            else
            {
                ArrayMenuButton = new InteractiveButton[]
                {
                    ChangeRoomNameButton, CharacterSelectButton, ActivateItemButton, InviteButton,
                    QuestEasyButton, QuestNormalButton, QuestHardButton,
                    BackToLobbyButton, ReadyButton,
                };
            }

            #endregion

            ChangeDifficulty(Room.CurrentDifficulty);

            if (IsHost)
            {
                UpdateSelectedMission(ListMissionInfo[0]);
            }
            else if (string.IsNullOrEmpty(Room.MapPath))
            {
                UpdateSelectedMap(Room.CurrentDifficulty, ListMissionInfo[0].MissionPath);
            }
            else
            {
                UpdateSelectedMap(Room.CurrentDifficulty, Room.MapPath);
            }

            foreach (Player ActivePlayer in Room.ListRoomPlayer)
            {
                ActivePlayer.CharacterPreview = new RobotAnimation("Characters/" + ActivePlayer.Equipment.CharacterType, null, Vector2.Zero, 0, new PlayerEquipment(), new MuteSFXGenerator(), new List<Weapon>());
            }
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

            foreach (InteractiveButton ActiveButton in ArrayMenuButton)
            {
                ActiveButton.Update(gameTime);
            }

            ChatHelper.UpdateChat(gameTime, OnlineCommunicationClient.Chat, ChatInput);

            MissionScrollbar.Update(gameTime);

            foreach (Player ActiveRobot in Room.ListRoomPlayer)
            {
                if (ActiveRobot.CharacterPreview != null)
                {
                    ActiveRobot.CharacterPreview.Update(gameTime);
                    ActiveRobot.CharacterPreview.UpdateAllWeaponsAngle(new Vector2(5, 0));
                }
            }

            if (KeyboardHelper.KeyPressed(Microsoft.Xna.Framework.Input.Keys.F1))
            {
                Player NewPlayer = new Player("", "", Player.PlayerTypes.Player, false, 0);
                Room.AddLocalPlayer(NewPlayer);
                NewPlayer.GameplayType = GameplayTypes.Controller1;
            }
            else if (KeyboardHelper.KeyPressed(Microsoft.Xna.Framework.Input.Keys.F2))
            {
                Player NewPlayer = new Player("", "", Player.PlayerTypes.Player, false, 0);
                Room.AddLocalPlayer(NewPlayer);
                NewPlayer.GameplayType = GameplayTypes.Controller2;
            }
            else if (KeyboardHelper.KeyPressed(Microsoft.Xna.Framework.Input.Keys.F3))
            {
                Player NewPlayer = new Player("", "", Player.PlayerTypes.Player,  false, 0);
                Room.AddLocalPlayer(NewPlayer);
                NewPlayer.GameplayType = GameplayTypes.Controller3;
            }
            else if (KeyboardHelper.KeyPressed(Microsoft.Xna.Framework.Input.Keys.F4))
            {
                Player NewPlayer = new Player("", "", Player.PlayerTypes.Player, false, 0);
                Room.AddLocalPlayer(NewPlayer);
                NewPlayer.GameplayType = GameplayTypes.Controller4;
            }

            if (MouseHelper.InputLeftButtonPressed() && IsHost)
            {
                for (int M = MissionInfoStartIndex, i = 0; M < ListMissionInfo.Count && i < 4; ++M, ++i)
                {
                    Rectangle QuestButtonCollisionBox = new Rectangle(405 - (int)QuestButton.Origin.X,
                                                                    140 - (int)QuestButton.Origin.Y + i * 64,
                                                                    QuestButton.SpriteWidth,
                                                                    QuestButton.SpriteHeight);

                    if (QuestButtonCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                    {
                        UpdateSelectedMission(ListMissionInfo[M]);
                        break;
                    }
                }
            }
        }

        public void AddPlayer(Player NewPlayer)
        {
            Room.ListRoomPlayer.Add(NewPlayer);
            NewPlayer.CharacterPreview = new RobotAnimation("Characters/" + NewPlayer.Equipment.CharacterType, null, Vector2.Zero, 0, new PlayerEquipment(), new MuteSFXGenerator(), new List<Weapon>());

            UpdateReadyOrHost();
        }

        private void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        private void OpenCharacterSelectScreen()
        {
            sndButtonClick.Play();
            Player LocalPlayer = Room.GetLocalPlayer();
            PushScreen(new CharacterSelect(LocalPlayer, UpdateCharacter));
        }

        private void UpdateCharacter()
        {
            Player LocalPlayer = Room.GetLocalPlayer();
            if (OnlineGameClient != null)
            {
                OnlineGameClient.Host.Send(new AskChangeCharacterScriptClient(LocalPlayer.Equipment.CharacterType));
            }
            else
            {
                UpdateCharacter(LocalPlayer);
            }
        }

        public void UpdateCharacter(Player PlayerToUpdate)
        {
            if (Room.GetLocalPlayer() == PlayerToUpdate)
            {
                sprActivePlayerIcon = DicCharacterIconByName[PlayerToUpdate.Equipment.CharacterType];
            }

            PlayerToUpdate.CharacterPreview = new RobotAnimation("Characters/" + PlayerToUpdate.Equipment.CharacterType, null, Vector2.Zero, 0, new PlayerEquipment(), new MuteSFXGenerator(), new List<Weapon>());
        }

        public void UpdateReadyOrHost()
        {
            if (IsHost)
            {
                bool IsEveryoneReady = true;

                foreach (Player ActivePlayer in Room.ListRoomPlayer)
                {
                    if (ActivePlayer.PlayerType != Player.PlayerTypeHost && ActivePlayer.PlayerType != Player.PlayerTypeReady)
                    {
                        IsEveryoneReady = false;
                    }
                }

                if (IsEveryoneReady)
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

        #region Button methods

        private void ReturnToLobby()
        {
            if (OnlineGameClient != null && OnlineGameClient.IsConnected)
            {
                OnlineGameClient.Host.Send(new LeaveRoomScriptClient());
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
                RemoveAllScreens();

                FightingZone NewMap = new FightingZone(Room.MapPath, true, Room.ListRoomPlayer);
                NewMap.Rules = new MissionGameRules(Room, NewMap);
                PushScreen(NewMap);
            }
        }

        private void Ready()
        {
            sndButtonClick.Play();

            if (OnlineGameClient != null)
            {
                ReadyButton.Disable();

                if (Room.GetLocalPlayer().PlayerType == Player.PlayerTypePlayer)
                {
                    OnlineGameClient.Host.Send(new AskChangePlayerTypeScriptClient(Player.PlayerTypeReady));
                }
                else
                {
                    OnlineGameClient.Host.Send(new AskChangePlayerTypeScriptClient(Player.PlayerTypePlayer));
                }
            }
        }

        private void OnEasySelected()
        {
            QuestNormalButton.Uncheck();
            QuestHardButton.Uncheck();

            ChangeDifficulty("Easy");

            if (ListMissionInfo.Count > 0)
            {
                UpdateSelectedMap(Room.CurrentDifficulty, ListMissionInfo[0].MissionPath);
            }
        }

        private void OnNormalSelected()
        {
            QuestEasyButton.Uncheck();
            QuestHardButton.Uncheck();

            ChangeDifficulty("Normal");

            if (ListMissionInfo.Count > 0)
            {
                UpdateSelectedMap(Room.CurrentDifficulty, ListMissionInfo[0].MissionPath);
            }
        }

        private void OnHardSelected()
        {
            QuestEasyButton.Uncheck();
            QuestNormalButton.Uncheck();

            ChangeDifficulty("Hard");

            if (ListMissionInfo.Count > 0)
            {
                UpdateSelectedMap(Room.CurrentDifficulty, ListMissionInfo[0].MissionPath);
            }
        }

        #endregion

        private void ChangeDifficulty(string Difficulty)
        {
            Room.CurrentDifficulty = Difficulty;

            DirectoryInfo MapDirectory = new DirectoryInfo(Content.RootDirectory + "/Maps/Triple Thunder/Missions/" + Difficulty);

            ListMissionInfo.Clear();
            FileInfo[] ArrayMapFile = MapDirectory.GetFiles("*.ttm");
            foreach (FileInfo ActiveFile in ArrayMapFile)
            {
                FileStream FS = new FileStream(ActiveFile.FullName, FileMode.Open, FileAccess.Read);
                BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
                BR.BaseStream.Seek(0, SeekOrigin.Begin);

                string BackgroundName = BR.ReadString();

                Rectangle CameraBounds = new Rectangle(BR.ReadInt32(), BR.ReadInt32(), BR.ReadInt32(), BR.ReadInt32());
                string BGMPath = BR.ReadString();
                string Description = BR.ReadString();

                BR.Close();
                FS.Close();

                string FileName = ActiveFile.Name.Remove(ActiveFile.Name.Length - 4);

                Texture2D sprMissionImage = null;

                if (File.Exists("Content/Triple Thunder/Menus/Wait Room/Map Icon " + FileName + ".xnb"))
                {
                    sprMissionImage = Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Map Icon " + FileName);
                }

                ListMissionInfo.Add(new MissionInfo(FileName, "Missions/" + Difficulty + "/" + FileName, Description, sprMissionImage));
            }

            MissionScrollbar.ChangeMaxValue(ListMissionInfo.Count - 4);
        }

        private void UpdateSelectedMission(MissionInfo SelectedMissionInfo)
        {
            if (OnlineGameClient != null && OnlineGameClient.IsConnected)
            {
                OnlineGameClient.Host.Send(new AskChangeMapScriptClient(Room.CurrentDifficulty, SelectedMissionInfo.MissionPath));
            }

            Room.MapPath = SelectedMissionInfo.MissionPath;
            CurrentMissionName = SelectedMissionInfo.MissionName;
            CurrentMissionDescription = SelectedMissionInfo.MissionDescription;
            sprCurrentMissionImage = SelectedMissionInfo.MissionImage;
        }

        public void UpdateSelectedMap(string CurrentDifficulty, string SelectedMapPath)
        {
            if (ListMissionInfo.Count == 0 || CurrentDifficulty != Room.CurrentDifficulty)
            {
                ChangeDifficulty(CurrentDifficulty);
            }

            foreach (MissionInfo ActiveMissionInfo in ListMissionInfo)
            {
                if (ActiveMissionInfo.MissionPath == SelectedMapPath)
                {
                    Room.MapPath = ActiveMissionInfo.MissionPath;
                    CurrentMissionName = ActiveMissionInfo.MissionName;
                    CurrentMissionDescription = ActiveMissionInfo.MissionDescription;
                    sprCurrentMissionImage = ActiveMissionInfo.MissionImage;
                }
            }
        }

        public void UpdateRoomSubtype(string RoomSubtype)
        {
            Room.RoomSubtype = RoomSubtype;
        }

        private void OnMissionScrollbarChange(float ScrollbarValue)
        {
            MissionInfoStartIndex = (int)ScrollbarValue;
        }

        private void SendMessage(string InputMessage)
        {
            ChatInput.SetText(string.Empty);
            OnlineCommunicationClient.SendMessage(OnlineCommunicationClient.Chat.ActiveTabID, InputMessage, ChatManager.MessageColors.White);
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            g.BeginUnscaled(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            GraphicsDevice.Clear(Color.Black);

            foreach (Player ActiveRobot in Room.ListRoomPlayer)
            {
                if (ActiveRobot.CharacterPreview != null)
                {
                    ActiveRobot.CharacterPreview.BeginDraw(g);
                }
            }

            g.End();
        }

        public override void EndDraw(CustomSpriteBatch g)
        {
            foreach (Player ActiveRobot in Room.ListRoomPlayer)
            {
                if (ActiveRobot.CharacterPreview != null)
                {
                    ActiveRobot.CharacterPreview.EndDraw(g);
                }
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.Draw(sprBackground, Vector2.Zero, Color.White);
            g.Draw(sprActivePlayerIcon, new Vector2(570, 11), Color.White);
            foreach (InteractiveButton ActiveButton in ArrayMenuButton)
            {
                ActiveButton.Draw(g);
            }

            ChatHelper.DrawChat(g, sprTabChat, fntText, OnlineCommunicationClient.Chat, ChatInput);

            MissionScrollbar.Draw(g);

            g.DrawString(fntText, Room.RoomName, new Vector2(68, 7), Color.White);

            g.DrawStringMiddleAligned(fntText, CurrentMissionName, new Vector2(170, 114), Color.White);
            g.DrawStringMiddleAligned(fntText, CurrentMissionDescription, new Vector2(170, 270), Color.White);
            if (sprCurrentMissionImage != null)
            {
                g.Draw(sprCurrentMissionImage, new Vector2(57, 139), Color.White);
            }

            for (int P = 0; P < Room.ListRoomPlayer.Count; ++P)
            {
                int DrawX = 678;
                int DrawY = 310 + P * 64;

                DrawPlayerBox(g, DrawX, DrawY, Room.ListRoomPlayer[P], true);
            }

            for (int M = MissionInfoStartIndex, i = 0 ; M < ListMissionInfo.Count && i < 4; ++M, ++i)
            {
                QuestButton.Draw(g, new Vector2(405, 140 + i * 64), Color.White);
                g.DrawString(fntText, (M + 1).ToString().PadLeft(2, '0'), new Vector2(395, 120 + i * 64), Color.FromNonPremultiplied(0, 255, 0, 255));
                g.DrawString(fntText, "1", new Vector2(485, 120 + i * 64), Color.White);
                g.DrawString(fntText, ListMissionInfo[M].MissionName, new Vector2(328, 145 + i * 64), Color.FromNonPremultiplied(0, 255, 0, 255));

                Rectangle QuestButtonCollisionBox = new Rectangle(405 - (int)QuestButton.Origin.X,
                                                                140 - (int)QuestButton.Origin.Y + i * 64,
                                                                QuestButton.SpriteWidth,
                                                                QuestButton.SpriteHeight);

                if (QuestButtonCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                {
                    PlayerInfoOutline.Draw(g, new Vector2(405, 140 + i * 64), Color.White);
                }
            }
        }

        public void DrawPlayerBox(CustomSpriteBatch g, int DrawX, int DrawY, Player PlayerToDraw, bool IsBlue)
        {
            if (IsBlue)
            {
                PlayerInfo.SetFrame(0);
            }
            else
            {
                PlayerInfo.SetFrame(1);
            }

            PlayerInfo.Draw(g, new Vector2(DrawX, DrawY), Color.White);

            Rectangle PlayerInfoCollisionBox = new Rectangle(DrawX - (int)PlayerInfo.Origin.X,
                                                            DrawY - (int)PlayerInfo.Origin.Y,
                                                            PlayerInfo.SpriteWidth,
                                                            PlayerInfo.SpriteHeight);

            if (PlayerInfoCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
            {
                PlayerInfoOutline.Draw(g, new Vector2(DrawX, DrawY), Color.White);
            }

            g.DrawString(fntText, PlayerToDraw.Name, new Vector2(DrawX - 15, DrawY), Color.White);

            if (PlayerToDraw.CharacterPreview != null)
            {
                PlayerToDraw.CharacterPreview.Draw(g, new Vector2(75 - DrawX, -3 - DrawY));
            }

            if (PlayerToDraw.PlayerType == Player.PlayerTypeHost)
            {
                g.Draw(sprHostText, new Vector2(DrawX - 65, DrawY + 6), Color.White);
            }
            else if (PlayerToDraw.PlayerType == Player.PlayerTypeReady)
            {
                g.Draw(sprReadyText, new Vector2(DrawX - 65, DrawY + 6), Color.White);
            }
        }
    }
}
