using System;
using System.IO;
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
    public class BattleSelect : GameScreen, IMissionSelect
    {
        #region Ressources

        private FMODSound sndBGM;
        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntText;
        private Texture2D fntTest;

        private Texture2D sprBackground;
        private Dictionary<string, Texture2D> DicCharacterIconByName;
        private Texture2D sprActivePlayerIcon;
        private Texture2D sprHostText;
        private Texture2D sprReadyText;
        private Texture2D sprTeamSeparatorBlue;
        private Texture2D sprTeamSeparatorRed;

        private TextInput ChatInput;

        private InteractiveButton ChangeRoomNameButton;
        private InteractiveButton CharacterSelectButton;
        private InteractiveButton ActivateItemButton;
        private InteractiveButton WeaponLimitButton;
        private InteractiveButton MapSelectButton;
        private Texture2D MapTextOverlay;
        private Texture2D MapStar;
        private AnimatedSprite MapLevelCategory;
        private InteractiveButton ModeSelectButton;
        private AnimatedSprite ModeSelectTextButton;
        private InteractiveButton MatchTypeButton;
        private InteractiveButton RedTeamButton;
        private InteractiveButton BlueTeamButton;
        private InteractiveButton KillPlusButton;
        private InteractiveButton KillMinusButton;
        private InteractiveButton TimePlusButton;
        private InteractiveButton TimeMinusButton;

        private InteractiveButton InviteButton;
        private InteractiveButton ReadyButton;
        private InteractiveButton StartButton;
        private InteractiveButton BackToLobbyButton;

        private AnimatedSprite PlayerInfo;
        private AnimatedSprite PlayerInfoOutline;

        private AnimatedSprite sprTabChat;

        private InteractiveButton[] ArrayMenuButton;

        #endregion

        private readonly BattleRoomInformations Room;
        public Texture2D sprMapImage;

        private readonly TripleThunderOnlineClient OnlineGameClient;
        private readonly CommunicationClient OnlineCommunicationClient;
        private bool IsHost;

        public BattleSelect(TripleThunderOnlineClient OnlineGameClient, CommunicationClient OnlineCommunicationClient, BattleRoomInformations Room)
        {
            this.OnlineGameClient = OnlineGameClient;
            this.OnlineCommunicationClient = OnlineCommunicationClient;
            this.Room = Room;

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

            sprBackground = Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Background Battle");
            fntTest = Content.Load<Texture2D>("Triple Thunder/HUD/Menus/Numbers White");
            sprHostText = Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Player Host Text");
            sprReadyText = Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Player Ready Text");
            sprTeamSeparatorBlue = Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Team Separator Blue");
            sprTeamSeparatorRed = Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Team Separator Red");

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
            ActivateItemButton = new InteractiveButton(Content, "Triple Thunder/Menus/Wait Room/Activate Item Button", new Vector2(625, 250), OnButtonOver, null);
            WeaponLimitButton = new InteractiveButton(Content, "Triple Thunder/Menus/Wait Room/Weapon Limit Button", new Vector2(727, 250), OnButtonOver, null);
            MapSelectButton = new InteractiveButton(Content, "Triple Thunder/Menus/Wait Room/Map Select Button", new Vector2(675, 416), OnButtonOver, OpenMapSelectScreen);
            MapTextOverlay = Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Map Text");
            MapStar = Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Map Star");
            MapLevelCategory = new AnimatedSprite(Content, "Triple Thunder/Menus/Wait Room/Map Level", new Vector2(0, 0), 0, 1, 3);
            ModeSelectButton = new InteractiveButton(Content, "Triple Thunder/Menus/Wait Room/Mode Select Arrows", new Vector2(698, 207), OnButtonOver, OpenModeSelectScreen);
            ModeSelectTextButton = new AnimatedSprite(Content, "Triple Thunder/Menus/Wait Room/Mode Select Text", new Vector2(697, 209), 0, 1, 3);
            MatchTypeButton = new InteractiveButton(Content, "Triple Thunder/Menus/Wait Room/Match Type Team Button", new Vector2(100, 67), OnButtonOver, null);
            RedTeamButton = new InteractiveButton(Content, "Triple Thunder/Menus/Wait Room/Red Team Button", new Vector2(215, 65), OnButtonOver, RedTeamSelected);
            RedTeamButton.CanBeChecked = true;
            BlueTeamButton = new InteractiveButton(Content, "Triple Thunder/Menus/Wait Room/Blue Team Button", new Vector2(300, 65), OnButtonOver, BlueTeamSelected);
            BlueTeamButton.CanBeChecked = true;
            RedTeamButton.Select();
            RedTeamSelected();
            KillPlusButton = new InteractiveButton(Content, "Triple Thunder/Menus/Wait Room/Plus Button", new Vector2(638, 300), OnButtonOver, KillPlus);
            KillMinusButton = new InteractiveButton(Content, "Triple Thunder/Menus/Wait Room/Minus Button", new Vector2(752, 300), OnButtonOver, KillMinus);
            TimePlusButton = new InteractiveButton(Content, "Triple Thunder/Menus/Wait Room/Plus Button", new Vector2(638, 324), OnButtonOver, TimePlus);
            TimeMinusButton = new InteractiveButton(Content, "Triple Thunder/Menus/Wait Room/Minus Button", new Vector2(752, 324), OnButtonOver, TimeMinus);

            InviteButton = new InteractiveButton(Content, "Triple Thunder/Menus/Wait Room/Invite Button", new Vector2(490, 30), OnButtonOver, null);
            ReadyButton = new InteractiveButton(Content, "Triple Thunder/Menus/Wait Room/Ready Button", new Vector2(450, 66), 8, OnButtonOver, Ready);
            StartButton = new InteractiveButton(Content, "Triple Thunder/Menus/Wait Room/Start Button", new Vector2(450, 66), OnButtonOver, StartGame);
            BackToLobbyButton = new InteractiveButton(Content, "Triple Thunder/Menus/Common/Back To Lobby", new Vector2(678, 565),
                                                            "Triple Thunder/Menus/Common/Back Arrow Icon", new Vector2(-86, 0), 6, OnButtonOver, ReturnToLobby);

            PlayerInfo = new AnimatedSprite(Content, "Triple Thunder/Menus/Wait Room/Player Info", new Vector2(0, 0), 0, 1, 3);
            PlayerInfoOutline = new AnimatedSprite(Content, "Triple Thunder/Menus/Wait Room/Player Info Outline", new Vector2(0, 0), 0, 1, 4);
            PlayerInfoOutline.SetFrame(2);

            sprTabChat = new AnimatedSprite(Content, "Triple Thunder/Menus/Channel/Tab Chat", new Vector2(0, 0), 0, 1, 4);

            UpdateReadyOrHost();

            #endregion

            UpdateRoomSubtype(Room.RoomSubtype);
            Room.MapPath = "Random";
            sprMapImage = Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Map Icons/Random");

            if (OnlineGameClient != null && OnlineGameClient.IsConnected)
            {
                DirectoryInfo MapDirectory = new DirectoryInfo(Content.RootDirectory + "/Maps/Triple Thunder/Battle/");

                FileInfo[] ArrayMapFile = MapDirectory.GetFiles("*.ttm");
                Random Random = new Random();
                string FileName = ArrayMapFile[Random.Next(ArrayMapFile.Length)].Name;
                FileName = FileName.Remove(FileName.Length - 4);
                OnlineGameClient.Host.Send(new AskChangeMapScriptClient(Room.CurrentDifficulty, "Battle/" + FileName));
            }

            foreach (Player ActivePlayer in Room.ListRoomPlayer)
            {
                ActivePlayer.CharacterPreview = new RobotAnimation("Characters/" + ActivePlayer.Equipment.CharacterType, null, Vector2.Zero, 0, new PlayerInventory(), new MuteSFXGenerator(), new List<ComboWeapon>());
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

            if (OnlineCommunicationClient != null)
            {
                ChatHelper.UpdateChat(gameTime, OnlineCommunicationClient.Chat, ChatInput);
            }

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
                Player NewPlayer = new Player("", "", Player.PlayerTypes.Player, 0);
                Room.AddLocalPlayer(NewPlayer);
                NewPlayer.GameplayType = GameplayTypes.Controller1;
            }
            else if (KeyboardHelper.KeyPressed(Microsoft.Xna.Framework.Input.Keys.F2))
            {
                Player NewPlayer = new Player("", "", Player.PlayerTypes.Player, 0);
                Room.AddLocalPlayer(NewPlayer);
                NewPlayer.GameplayType = GameplayTypes.Controller2;
            }
            else if (KeyboardHelper.KeyPressed(Microsoft.Xna.Framework.Input.Keys.F3))
            {
                Player NewPlayer = new Player("", "", Player.PlayerTypes.Player, 0);
                Room.AddLocalPlayer(NewPlayer);
                NewPlayer.GameplayType = GameplayTypes.Controller3;
            }
            else if (KeyboardHelper.KeyPressed(Microsoft.Xna.Framework.Input.Keys.F4))
            {
                Player NewPlayer = new Player("", "", Player.PlayerTypes.Player, 0);
                Room.AddLocalPlayer(NewPlayer);
                NewPlayer.GameplayType = GameplayTypes.Controller4;
            }
        }

        private void AssignButtons()
        {
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
                    ChangeRoomNameButton, CharacterSelectButton, ActivateItemButton, WeaponLimitButton,
                    MapSelectButton, ModeSelectButton, MatchTypeButton,
                    RedTeamButton, BlueTeamButton, KillPlusButton, KillMinusButton, TimePlusButton, TimeMinusButton,
                    InviteButton, BackToLobbyButton, StartButton,
                };
            }
            else
            {
                ArrayMenuButton = new InteractiveButton[]
                {
                    ChangeRoomNameButton, CharacterSelectButton, ActivateItemButton, WeaponLimitButton,
                    MapSelectButton, ModeSelectButton, MatchTypeButton,
                    RedTeamButton, BlueTeamButton, KillPlusButton, KillMinusButton, TimePlusButton, TimeMinusButton,
                    InviteButton, BackToLobbyButton, ReadyButton,
                };
            }
        }

        public void AddPlayer(Player NewPlayer)
        {
            Room.ListRoomPlayer.Add(NewPlayer);
            NewPlayer.CharacterPreview = new RobotAnimation("Characters/" + NewPlayer.Equipment.CharacterType, null, Vector2.Zero, 0, new PlayerInventory(), new MuteSFXGenerator(), new List<ComboWeapon>());

            UpdateReadyOrHost();
        }

        public void UpdateSelectedMap(string CurrentDifficulty, string SelectedMapPath)
        {
        }

        private void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        public void UpdateCharacter(Player PlayerToUpdate)
        {
            if (Room.GetLocalPlayer() == PlayerToUpdate)
            {
                sprActivePlayerIcon = DicCharacterIconByName[PlayerToUpdate.Equipment.CharacterType];
            }

            PlayerToUpdate.CharacterPreview = new RobotAnimation("Characters/" + PlayerToUpdate.Equipment.CharacterType, null, Vector2.Zero, 0, new PlayerInventory(), new MuteSFXGenerator(), new List<ComboWeapon>());
        }

        public void UpdateRoomSubtype(string RoomSubtype)
        {
            Room.RoomSubtype = RoomSubtype;

            if (Room.RoomSubtype == "Deathmatch")
            {
                ModeSelectTextButton.SetFrame(0);
            }
            else if (Room.RoomSubtype == "Capture The Flag")
            {
                ModeSelectTextButton.SetFrame(1);
            }
            else if (Room.RoomSubtype == "Survival")
            {
                ModeSelectTextButton.SetFrame(2);
            }
        }

        public void UpdateReadyOrHost()
        {
            AssignButtons();

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

        private void SendMessage(string InputMessage)
        {
            ChatInput.SetText(string.Empty);
            OnlineCommunicationClient.SendMessage(OnlineCommunicationClient.Chat.ActiveTabID, new ChatManager.ChatMessage(DateTime.UtcNow, InputMessage, ChatManager.MessageColors.White));
        }

        #region Button methods

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

        private void OpenMapSelectScreen()
        {
            if (OnlineGameClient != null)
            {
                if (IsHost)
                {
                    PushScreen(new BattleMapSelect(Room, this));
                    sndButtonClick.Play();
                }
            }
            else
            {
                PushScreen(new BattleMapSelect(Room, this));
                sndButtonClick.Play();
            }
        }

        private void OpenCharacterSelectScreen()
        {
            sndButtonClick.Play();
            Player LocalPlayer = Room.GetLocalPlayer();
            PushScreen(new CharacterSelect(LocalPlayer, UpdateCharacter));
        }

        private void OpenModeSelectScreen()
        {
            if (OnlineGameClient != null)
            {
                if (IsHost)
                {
                    PushScreen(new BattleModeSelect(Room, this, OnlineGameClient));
                    sndButtonClick.Play();
                }
            }
            else
            {
                PushScreen(new BattleModeSelect(Room, this, OnlineGameClient));
                sndButtonClick.Play();
            }
        }

        private void KillMinus()
        {
            if (OnlineGameClient != null)
            {
                if (IsHost)
                {
                    OnlineGameClient.Host.Send(new AskChangeRoomExtrasBattleScriptClient(Room.MaxKill - 1, Room.MaxGameLengthInMinutes));
                }
            }
            else
            {
                --Room.MaxKill;
            }
        }

        private void KillPlus()
        {
            if (OnlineGameClient != null)
            {
                if (IsHost)
                {
                    OnlineGameClient.Host.Send(new AskChangeRoomExtrasBattleScriptClient(Room.MaxKill + 1, Room.MaxGameLengthInMinutes));
                }
            }
            else
            {
                ++Room.MaxKill;
            }
        }

        private void TimeMinus()
        {
            if (OnlineGameClient != null)
            {
                if (IsHost)
                {
                    OnlineGameClient.Host.Send(new AskChangeRoomExtrasBattleScriptClient(Room.MaxKill, Room.MaxGameLengthInMinutes - 1));
                }
            }
            else
            {
                --Room.MaxGameLengthInMinutes;
            }
        }

        private void TimePlus()
        {
            if (OnlineGameClient != null)
            {
                if (IsHost)
                {
                    OnlineGameClient.Host.Send(new AskChangeRoomExtrasBattleScriptClient(Room.MaxKill, Room.MaxGameLengthInMinutes + 1));
                }
            }
            else
            {
                ++Room.MaxGameLengthInMinutes;
            }
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
                FightingZone NewMap;

                if (Room.MapPath == "Random")
                {
                    DirectoryInfo MapDirectory = new DirectoryInfo(Content.RootDirectory + "/Maps/Triple Thunder/Battle/");

                    FileInfo[] ArrayMapFile = MapDirectory.GetFiles("*.ttm");
                    Random Random = new Random();
                    string FileName = ArrayMapFile[Random.Next(ArrayMapFile.Length)].Name;
                    FileName = FileName.Remove(FileName.Length - 4);
                    NewMap = new FightingZone("Battle/" + FileName, Room.UseTeams, Room.ListRoomPlayer);
                }
                else
                {
                    NewMap = new FightingZone(Room.MapPath, Room.UseTeams, Room.ListRoomPlayer);
                }

                NewMap.Rules = new BattleGameRules(Room, NewMap);
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

        private void RedTeamSelected()
        {
            Player LocalPlayer = Room.GetLocalPlayer();
            if (OnlineGameClient != null)
            {
                OnlineGameClient.Host.Send(new AskChangeTeamScriptClient(0));
            }
            else
            {
                LocalPlayer.Team = 0;
            }

            BlueTeamButton.Unselect();
        }

        private void BlueTeamSelected()
        {
            Player LocalPlayer = Room.GetLocalPlayer();
            if (OnlineGameClient != null)
            {
                OnlineGameClient.Host.Send(new AskChangeTeamScriptClient(1));
            }
            else
            {
                LocalPlayer.Team = 0;
            }
            RedTeamButton.Unselect();
        }

        #endregion

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
            g.Draw(sprTeamSeparatorBlue, new Vector2(285, 220), Color.White);
            g.Draw(sprTeamSeparatorRed, new Vector2(55, 220), Color.White);
            g.Draw(sprActivePlayerIcon, new Vector2(570, 11), Color.White);

            DrawNumberRightAligned(g, fntTest, Room.MaxKill, new Vector2(700, 294));
            DrawNumberRightAligned(g, fntTest, Room.MaxGameLengthInMinutes, new Vector2(700, 318));
            g.DrawString(fntText, "Off", new Vector2(643, 355), Color.White);
            DrawNumberRightAligned(g, fntTest, Room.MaxNumberOfPlayer, new Vector2(753, 356));

            g.Draw(sprMapImage, new Vector2(579, 437), Color.White);
            if (Room.MapPath != "Random")
            {
                g.Draw(MapTextOverlay, new Vector2(595, 486), Color.White);
                g.Draw(MapStar, new Vector2(616, 503), Color.White);
                MapLevelCategory.Draw(g, new Vector2(680, 513), Color.White);
                g.Draw(MapStar, new Vector2(740, 503), Color.White);
            }

            foreach (InteractiveButton ActiveButton in ArrayMenuButton)
            {
                ActiveButton.Draw(g);
            }

            if (OnlineCommunicationClient != null)
            {
                ChatHelper.DrawChat(g, sprTabChat, fntText, OnlineCommunicationClient.Chat, ChatInput);
            }

            ModeSelectTextButton.Draw(g);

            g.DrawString(fntText, Room.RoomName, new Vector2(75, 7), Color.White);

            int RedPlayerCount = 0;
            int BluePlayerCount = 0;

            for (int P = 0; P < Room.ListRoomPlayer.Count; ++P)
            {
                int DrawX = 0;
                int DrawY = 0;

                if (Room.ListRoomPlayer[P].Team == 0)
                {
                    DrawX = 155 + (PlayerInfo.SpriteWidth + 40) * (RedPlayerCount % 2);
                    DrawY = 125 + (RedPlayerCount / 2) * 64;
                    ++RedPlayerCount;
                }
                else if (Room.ListRoomPlayer[P].Team == 1)
                {
                    DrawX = 155 + (PlayerInfo.SpriteWidth + 40) * (BluePlayerCount % 2);
                    DrawY = 275 + (BluePlayerCount / 2) * 64;

                    ++BluePlayerCount;
                }

                DrawPlayerBox(g, DrawX, DrawY, Room.ListRoomPlayer[P], Room.ListRoomPlayer[P].Team == 1 || !Room.UseTeams);
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
