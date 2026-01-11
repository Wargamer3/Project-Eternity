using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.DeathmatchMapScreen.Online;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class MultiplayerScreen : GameScreen
    {
        private enum MenuChoices { None, PlayerList, PlayerEdit, PlayerType, SquadSelection, UnitSelection, PilotSelection, Messenger, JoinGame, ChangeMap, StartGame };

        private enum OnlineStatus: byte { Messenger = 0, JoinGame, StartGame, ChangePlayerType, NewOnlinePlayer };

        private class MapAttributes
        {
            public string Name;
            public string Path;
            public Point MapSize;
            public int MaxNumberOfPlayers;
            public List<EventPoint> ListSpawns;
            public Color[] ArrayColor = new Color[16];

            public MapAttributes(string Name, string Path, Point MapSize, int MaxNumberOfPlayers, List<EventPoint> ListSpawns, Color[] ArrayColor)
            {
                this.Name = Name;
                this.Path = Path;
                this.MapSize = MapSize;
                this.MaxNumberOfPlayers = MaxNumberOfPlayers;
                this.ListSpawns = ListSpawns;
                this.ArrayColor = ArrayColor;
            }
        }

        #region Variables

        private MenuChoices Stage;
        private MenuChoices CurrentSelection;
        private int ActivePlayerIndex;
        private int PlayerChoiceSubMenuIndex;
        private int ActiveSquadIndex;
        private int ActiveUnitMinIndex;
        private int ActiveUnitIndex;
        private int ActivePilotMinIndex;
        private int ActivePilotIndex;
        private SpriteFont fntArial8;
        private SpriteFont fntArial12;
        private Color CursorColor;

        public List<string> Messenger;//List of messages to draw.

        private string Message;//Message being typed.
        private string MessageDraw;//Croped message that fit in the text box.
        private int MessageStartIndex;//Index of the starting letter of the message to draw.
        private int MessageCursorIndex;//Index of the typing cursor in the message.
        private float MessageCursorPosition;//X position of the cursor, based on its index.
        private Keys[] lastPressedKeys;

        private string[] ListPlayerType;

        private Tuple<string, string>[] ArrayUnit;//Type, Name.
        private List<string> ListPilot;
        private int MaxItemToDraw;

        private List<MapAttributes> ListMap;
        private Color[] ArrayColor;
        private int MaxNumberOfPlayers;
        private int MapSizeX;
        private int MapSizeY;
        private int CursorMap;
        private int CursorMapMin;

        private DeathmatchMap NewMap;
        private PopUpInputBox IPInputBox;
        private bool IsFrozen;
        private OnlineConfiguration OnlinePlayers;

        public ActionPanelHolder ListActionMenuChoice;

        DeathmatchParams Params;

        #endregion

        public MultiplayerScreen()
            : base()
        {
            this.RequireDrawFocus = true;

            CursorMapMin = 0;
            ActiveUnitMinIndex = 0;
            IsFrozen = false;

            Messenger = new List<string>();
            Message = "";
            MessageDraw = "";
            MessageStartIndex = 0;
            MessageCursorIndex = 0;
            lastPressedKeys = new Keys[0];
            Stage = MenuChoices.None;
            CurrentSelection = MenuChoices.PlayerList;
            ListPlayerType = new string[5] { "Closed", "Open", "Human", "CPU", "Online" };
            Params = new DeathmatchParams(new BattleContext());
        }

        public override void Load()
        {
            NewMap = new DeathmatchMap(new GameModeInfo(), Params);
            DirectoryInfo MapDirectory = new DirectoryInfo(Content.RootDirectory + "/Deathmatch/Maps");
            fntArial8 = Content.Load<SpriteFont>("Fonts/Arial8");
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            MaxItemToDraw = (Constants.Height - 100) / fntArial12.LineSpacing - 1;
            CursorColor = Color.FromNonPremultiplied(255, 255, 255, 128);
            //Load directory info, abort if none
            if (!MapDirectory.Exists)
                throw new DirectoryNotFoundException();
            ListMap = new List<MapAttributes>();
            List<string> ListPlayerTag = new List<string>();

            IPInputBox = new PopUpInputBox("Enter an IP");
            OnlinePlayers = new OnlineConfiguration();
            //OnlinePlayers.StartListening();

            KeyValuePair<string, List<OnlineScript>> NewScripts = new DeathmatchLobyScriptHolder().GetNameAndContent(this);
            foreach (OnlineScript ActiveListScript in NewScripts.Value)
            {
                OnlinePlayers.DicOnlineScripts.Add(ActiveListScript.Name, ActiveListScript);
            }

            #region Load Map Info

            FileInfo[]  ArrayMapFile = MapDirectory.GetFiles("*.pem");
            foreach (FileInfo ActiveFile in ArrayMapFile)
            {
                FileStream FS = new FileStream(ActiveFile.FullName, FileMode.Open, FileAccess.Read);
                BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
                Point NewMapSize = Point.Zero;
                BR.BaseStream.Seek(0, SeekOrigin.Begin);
                string MapName = ActiveFile.FullName.Substring(0, ActiveFile.FullName.Length - 4).Substring(ActiveFile.FullName.LastIndexOf("Maps") + 16);
                NewMapSize.X = BR.ReadInt32();
                NewMapSize.Y = BR.ReadInt32();
                int TileSizeX = BR.ReadInt32();
                int TileSizeY = BR.ReadInt32();
                
                int CameraPositionX = BR.ReadInt32();
                int CameraPositionY = BR.ReadInt32();

                byte PlayersMin = BR.ReadByte();
                byte PlayersMax = BR.ReadByte();

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

                int ListMultiplayerSpawnsCount = BR.ReadInt32();
                List<EventPoint> ListSpawns = new List<EventPoint>(ListMultiplayerSpawnsCount);

                for (int S = 0; S < ListMultiplayerSpawnsCount; S++)
                {
                    EventPoint NewPoint = new EventPoint(BR);
                    int ColorIndex = 0;

                    if (!string.IsNullOrWhiteSpace(NewPoint.Tag))
                        ColorIndex = Convert.ToInt32(NewPoint.Tag) - 1;

                    NewPoint.ColorRed = ArrayColor[ColorIndex].R;
                    NewPoint.ColorGreen = ArrayColor[ColorIndex].G;
                    NewPoint.ColorBlue = ArrayColor[ColorIndex].B;
                    ListSpawns.Add(NewPoint);
                }

                ListPlayerTag.Clear();
                for (int S = 0; S < ListSpawns.Count; S++)
                {
                    if (!ListPlayerTag.Contains(ListSpawns[S].Tag))
                        ListPlayerTag.Add(ListSpawns[S].Tag);
                }

                ListMap.Add(new MapAttributes(MapName, ActiveFile.FullName, new Point(NewMapSize.X, NewMapSize.Y), ListPlayerTag.Count, ListSpawns, ArrayColor));
                FS.Close();
                BR.Close();
            }

            #endregion

            #region Load Units

            MapDirectory = new DirectoryInfo(Content.RootDirectory + "\\Units\\Normal");
            FileInfo[] ArrayUnitFile = MapDirectory.GetFiles("*.peu", SearchOption.AllDirectories);
            ArrayUnit = new Tuple<string,string>[ArrayUnitFile.Length];

            for (int i = ArrayUnitFile.Length - 1; i >= 0; --i)
            {
                string Name = ArrayUnitFile[i].FullName.Substring(0, ArrayUnitFile[i].FullName.Length - 4).Substring(ArrayUnitFile[i].FullName.LastIndexOf("Units") + 6);
                string[] UnitInfo = Name.Split(new[] { "\\" }, StringSplitOptions.None);
                string UnitType = UnitInfo[0];
                string UnitName = Name.Remove(0, UnitInfo[0].Length + 1);
                ArrayUnit[i] = new Tuple<string,string>(UnitType, UnitName);
            }

            #endregion

            #region Load Pilots

            MapDirectory = new DirectoryInfo(Content.RootDirectory + "\\Characters");
            FileInfo[] ArrayCharactetFile = MapDirectory.GetFiles("*.pec", SearchOption.AllDirectories);
            ListPilot = new List<string>();

            for (int i = ArrayCharactetFile.Length - 1; i >= 0; --i)
            {
                if (ArrayCharactetFile[i].Extension.Length > 4)//Because the retarded search system return files that ends with .pecs, ignore them.
                    continue;

                ListPilot.Add(ArrayCharactetFile[i].FullName.Substring(0, ArrayCharactetFile[i].FullName.Length - 4).Substring(ArrayCharactetFile[i].FullName.LastIndexOf("Characters") + 11));
            }

            #endregion
        }

        public DeathmatchMap LoadAutoplay()
        {
            Random Random = new Random();
            MapAttributes ActiveMapAttributes = ListMap.Where(x => x.Name == "Autoplay").First();
            DeathmatchMap Autoplay = new DeathmatchMap(ActiveMapAttributes.Name, new GameModeInfo(), new DeathmatchParams(new BattleContext()));
            Autoplay.ListGameScreen = ListGameScreen;
            while (Autoplay.ListPlayer.Count < ActiveMapAttributes.MaxNumberOfPlayers)
            {
                Player NewPlayer = new Player("Player " + (Autoplay.ListPlayer.Count + 1), "AI", false, false, Autoplay.ListPlayer.Count, ActiveMapAttributes.ArrayColor[Autoplay.ListPlayer.Count]);
                NewPlayer.IsAlive = false;
                Autoplay.ListPlayer.Add(NewPlayer);
            }

            return Autoplay;
        }
        
        public override void Update(GameTime gameTime)
        {
            if (OnlinePlayers.IsHost)
                OnlinePlayers.CheckForNewClient();
            
            OnlinePlayers.Update();

            if (IsFrozen)
                return;

            switch (Stage)
            {
                #region None

                case MenuChoices.None:
                    if (InputHelper.InputConfirmPressed())
                    {
                        if (CurrentSelection == MenuChoices.StartGame && OnlinePlayers.IsHost)
                        {
                            PushScreen(NewMap);
                            NewMap.TogglePreview(false);
                            RemoveScreen(this);
                        }
                        else if (CurrentSelection == MenuChoices.JoinGame)
                        {
                            IPInputBox.Open();
                            Stage = CurrentSelection;
                        }
                        else
                            Stage = CurrentSelection;
                    }
                    else if (InputHelper.InputLeftPressed())
                    {
                        if (CurrentSelection == MenuChoices.PlayerList)
                            CurrentSelection = MenuChoices.JoinGame;
                        else if (CurrentSelection == MenuChoices.Messenger)
                            CurrentSelection = MenuChoices.StartGame;
                        else if (CurrentSelection == MenuChoices.StartGame)
                            CurrentSelection = MenuChoices.Messenger;
                        else if (CurrentSelection == MenuChoices.ChangeMap || CurrentSelection == MenuChoices.JoinGame)
                            CurrentSelection = MenuChoices.PlayerList;
                    }
                    else if (InputHelper.InputRightPressed())
                    {
                        if (CurrentSelection == MenuChoices.PlayerList)
                            CurrentSelection = MenuChoices.JoinGame;
                        else if (CurrentSelection == MenuChoices.Messenger)
                            CurrentSelection = MenuChoices.StartGame;
                        else if (CurrentSelection == MenuChoices.StartGame)
                            CurrentSelection = MenuChoices.Messenger;
                        else if (CurrentSelection == MenuChoices.ChangeMap || CurrentSelection == MenuChoices.JoinGame)
                            CurrentSelection = MenuChoices.PlayerList;
                    }
                    else if (InputHelper.InputUpPressed())
                    {
                        if (CurrentSelection == MenuChoices.PlayerList)
                            CurrentSelection = MenuChoices.Messenger;
                        else if (CurrentSelection == MenuChoices.Messenger)
                            CurrentSelection = MenuChoices.PlayerList;
                        else if (CurrentSelection == MenuChoices.JoinGame)
                            CurrentSelection = MenuChoices.StartGame;
                        else if (CurrentSelection == MenuChoices.ChangeMap)
                            CurrentSelection = MenuChoices.JoinGame;
                        else if (CurrentSelection == MenuChoices.StartGame)
                            CurrentSelection = MenuChoices.ChangeMap;
                    }
                    else if (InputHelper.InputDownPressed())
                    {
                        if (CurrentSelection == MenuChoices.PlayerList)
                            CurrentSelection = MenuChoices.Messenger;
                        else if (CurrentSelection == MenuChoices.Messenger)
                            CurrentSelection = MenuChoices.PlayerList;
                        else if (CurrentSelection == MenuChoices.JoinGame)
                            CurrentSelection = MenuChoices.ChangeMap;
                        else if (CurrentSelection == MenuChoices.ChangeMap)
                            CurrentSelection = MenuChoices.StartGame;
                        else if (CurrentSelection == MenuChoices.StartGame)
                            CurrentSelection = MenuChoices.JoinGame;
                    }
                    break;

                #endregion

                #region Messenger

                case MenuChoices.Messenger:
                    Keys[] pressedKeys = KeyboardHelper.PlayerState.GetPressedKeys();
                    //check if any of the previous update's keys are no longer pressed
                    foreach (Keys key in lastPressedKeys)
                    {
                        if (!pressedKeys.Contains(key))
                        {
                            switch (key)
                            {
                                case Keys.Enter:
                                    OnlinePlayers­.ExecuteAndSend(new DeathmatchLobyScriptHolder.LobyMessageScript(this, Message));
                                    Message = "";
                                    MessageDraw = Message;
                                    UpdateMessengerCursor();
                                    break;

                                case Keys.Escape:
                                    Stage = MenuChoices.None;
                                    break;

                                case Keys.Right:
                                    if (MessageCursorIndex < Message.Length)
                                        MessageCursorIndex++;
                                    UpdateMessengerCursor();
                                    break;

                                case Keys.Left:
                                    if (MessageCursorIndex > 0)
                                        MessageCursorIndex--;
                                    UpdateMessengerCursor();
                                    break;

                                case Keys.Space:
                                    Message += " ";
                                    break;

                                case Keys.Back:
                                    if (MessageCursorIndex > 0)
                                        Message = Message.Remove(--MessageCursorIndex, 1);
                                    break;

                                case Keys.Delete:
                                    if (MessageCursorIndex < Message.Length)
                                        Message = Message.Remove(MessageCursorIndex, 1);
                                    break;

                                default:
                                    Message += key;
                                    MessageDraw = Message;
                                    break;
                            }
                        }
                    }
                    lastPressedKeys = pressedKeys;
                    break;

                #endregion

                #region Join Game

                case MenuChoices.JoinGame:
                    if (IPInputBox.IsOpen)
                        IPInputBox.Update(gameTime);
                    else
                    {
                        if (OnlinePlayers.JoinHost(IPAddress.Loopback))
                            Messenger.Add("Connection Confirmed");

                        Stage = MenuChoices.None;
                    }
                    break;

                #endregion

                #region Change Map

                case MenuChoices.ChangeMap:
                    if (InputHelper.InputConfirmPressed())
                    {
                        NewMap.BattleMapPath = ListMap[CursorMap].Name;
                        MaxNumberOfPlayers = ListMap[CursorMap].MaxNumberOfPlayers;
                        MapSizeX = ListMap[CursorMap].MapSize.X;
                        MapSizeY = ListMap[CursorMap].MapSize.Y;

                        if (NewMap.ListPlayer.Count > MaxNumberOfPlayers)
                            NewMap.ListPlayer.RemoveRange(MaxNumberOfPlayers, NewMap.ListPlayer.Count - MaxNumberOfPlayers);
                        else
                        {
                            while (NewMap.ListPlayer.Count < MaxNumberOfPlayers)
                            {
                                Player NewPlayer = new Player("Player " + (NewMap.ListPlayer.Count + 1), "Open", false, true, NewMap.ListPlayer.Count, Color.Blue);
                                NewPlayer.IsAlive = false;
                                NewMap.ListPlayer.Add(NewPlayer);
                            }
                        }

                        for (int P = 0; P < NewMap.ListPlayer.Count; P++)
                        {
                            NewMap.ListPlayer[P].ListSpawnPoint.Clear();
                            for (int S = 0; S < ListMap[CursorMap].ListSpawns.Count; S++)
                            {
                                if (Convert.ToInt32(ListMap[CursorMap].ListSpawns[S].Tag) == P + 1)
                                {
                                    NewMap.ListPlayer[P].ListSpawnPoint.Add(new PlayerEventPoint(ListMap[CursorMap].ListSpawns[S]));
                                }
                            }
                        }

                        Stage = MenuChoices.None;
                    }
                    else if (InputHelper.InputCancelPressed())
                    {
                        Stage = MenuChoices.None;
                    }
                    else if (InputHelper.InputUpPressed())
                    {
                        CursorMap--;
                        if (CursorMap < 0)
                            CursorMap = ListMap.Count - 1;
                    }
                    else if (InputHelper.InputDownPressed())
                    {
                        CursorMap++;
                        if (CursorMap >= ListMap.Count)
                            CursorMap = 0;
                    }
                    break;

                #endregion

                #region Player List

                case MenuChoices.PlayerList:
                    if (InputHelper.InputConfirmPressed())
                    {
                        if ((OnlinePlayers.IsHost && NewMap.ListPlayer[ActivePlayerIndex].OnlinePlayerType != "Online") ||
                            (NewMap.ListPlayer[ActivePlayerIndex].IsOnline && NewMap.ListPlayer[ActivePlayerIndex].OnlinePlayerType == "Open"))
                        {
                            Stage = MenuChoices.PlayerEdit;
                            CurrentSelection = MenuChoices.PlayerType;
                        }
                    }
                    else if (InputHelper.InputCancelPressed())
                    {
                        Stage = MenuChoices.None;
                        CurrentSelection = MenuChoices.PlayerList;
                    }
                    else if (InputHelper.InputUpPressed())
                    {
                        ActivePlayerIndex--;
                        if (ActivePlayerIndex < 0)
                            ActivePlayerIndex = NewMap.ListPlayer.Count - 1;
                    }
                    else if (InputHelper.InputDownPressed())
                    {
                        ActivePlayerIndex++;
                        if (ActivePlayerIndex >= NewMap.ListPlayer.Count)
                            ActivePlayerIndex = 0;
                    }
                    break;

                #endregion

                #region Player Edit

                case MenuChoices.PlayerEdit:
                    if (InputHelper.InputConfirmPressed())
                    {
                        Stage = CurrentSelection;
                        PlayerChoiceSubMenuIndex = 0;
                    }
                    else if (InputHelper.InputCancelPressed())
                    {
                        Stage = MenuChoices.PlayerList;
                    }
                    else if (InputHelper.InputLeftPressed())
                    {
                        if (CurrentSelection == MenuChoices.PlayerType)
                            CurrentSelection = MenuChoices.SquadSelection;
                        else
                            CurrentSelection--;
                    }
                    else if (InputHelper.InputRightPressed())
                    {
                        if (CurrentSelection == MenuChoices.SquadSelection)
                            CurrentSelection = MenuChoices.PlayerType;
                        else
                            CurrentSelection++;
                    }
                    break;

                #endregion

                #region Player Type

                case MenuChoices.PlayerType:
                    if (InputHelper.InputConfirmPressed())
                    {
                        if (NewMap.ListPlayer[ActivePlayerIndex].OnlinePlayerType != ListPlayerType[PlayerChoiceSubMenuIndex])
                        {
                            NewMap.ListPlayer[ActivePlayerIndex].OnlinePlayerType = ListPlayerType[PlayerChoiceSubMenuIndex];
                            if (NewMap.ListPlayer[ActivePlayerIndex].OnlinePlayerType == "Human")
                                NewMap.ListPlayer[ActivePlayerIndex].IsPlayerControlled = true;

                            if (NewMap.ListPlayer[ActivePlayerIndex].OnlinePlayerType == "Online")
                                NewMap.ListPlayer[ActivePlayerIndex].IsOnline = true;
                            else
                                NewMap.ListPlayer[ActivePlayerIndex].IsOnline = false;

                            Stage = MenuChoices.PlayerEdit;
                        }
                    }
                    else if (InputHelper.InputCancelPressed())
                    {
                        Stage = MenuChoices.PlayerEdit;
                    }
                    else if (InputHelper.InputUpPressed())
                    {
                        PlayerChoiceSubMenuIndex--;
                        if (PlayerChoiceSubMenuIndex < 0)
                            PlayerChoiceSubMenuIndex = 3;
                    }
                    else if (InputHelper.InputDownPressed())
                    {
                        PlayerChoiceSubMenuIndex++;
                        if (PlayerChoiceSubMenuIndex > 3)
                            PlayerChoiceSubMenuIndex = 0;
                    }
                    break;

                #endregion

                #region Squad Selection

                case MenuChoices.SquadSelection:
                    if (InputHelper.InputConfirmPressed())
                    {
                        Stage = MenuChoices.UnitSelection;
                        ActiveUnitIndex = 0;
                        ActiveUnitMinIndex = 0;
                    }
                    else if (InputHelper.InputCancelPressed())
                    {
                        Stage = MenuChoices.PlayerEdit;
                    }
                    else if (InputHelper.InputUpPressed())
                    {
                        ActiveSquadIndex--;
                        if (ActiveSquadIndex < 0)
                            ActiveSquadIndex = NewMap.ListPlayer[ActivePlayerIndex].ListSpawnPoint.Count - 1;
                    }
                    else if (InputHelper.InputDownPressed())
                    {
                        ActiveSquadIndex++;
                        if (ActiveSquadIndex >= NewMap.ListPlayer[ActivePlayerIndex].ListSpawnPoint.Count)
                            ActiveSquadIndex = 0;
                    }
                    else if (InputHelper.InputLeftPressed())
                    {
                        PlayerChoiceSubMenuIndex--;
                        if (PlayerChoiceSubMenuIndex < 0)
                            PlayerChoiceSubMenuIndex = 2;
                    }
                    else if (InputHelper.InputRightPressed())
                    {
                        PlayerChoiceSubMenuIndex++;
                        if (PlayerChoiceSubMenuIndex > 2)
                            PlayerChoiceSubMenuIndex = 0;
                    }
                    break;

                #endregion

                #region Unit Selection

                case MenuChoices.UnitSelection:
                    if (InputHelper.InputConfirmPressed())
                    {
                        Stage = MenuChoices.PilotSelection;
                        ActivePilotIndex = 0;
                        ActivePilotMinIndex = 0;
                    }
                    else if (InputHelper.InputCancelPressed())
                    {
                        Stage = MenuChoices.SquadSelection;
                    }
                    else if (InputHelper.InputUpPressed())
                    {
                        ActiveUnitIndex--;
                        if (ActiveUnitIndex < 0)
                        {
                            ActiveUnitIndex = ArrayUnit.Length - 1;
                            ActiveUnitMinIndex = Math.Max(0, ArrayUnit.Length - MaxItemToDraw);
                        }
                        else if (ActiveUnitIndex < ActiveUnitMinIndex)
                            ActiveUnitMinIndex = ActiveUnitIndex;
                    }
                    else if (InputHelper.InputDownPressed())
                    {
                        ActiveUnitIndex++;
                        if (ActiveUnitIndex >= ArrayUnit.Length)
                        {
                            ActiveUnitIndex = 0;
                            ActiveUnitMinIndex = 0;
                        }
                        else if (ActiveUnitIndex >= MaxItemToDraw + ActiveUnitMinIndex)
                            ActiveUnitMinIndex = ActiveUnitIndex - MaxItemToDraw + 1;
                    }
                    break;

                #endregion

                #region Pilot Selection

                case MenuChoices.PilotSelection:

                    if (InputHelper.InputConfirmPressed())
                    {
                        Stage = MenuChoices.SquadSelection;
                        if (PlayerChoiceSubMenuIndex == 0)
                        {
                            NewMap.ListPlayer[ActivePlayerIndex].ListSpawnPoint[ActiveSquadIndex].LeaderPilot = ListPilot[ActivePilotIndex];
                            NewMap.ListPlayer[ActivePlayerIndex].ListSpawnPoint[ActiveSquadIndex].LeaderTypeName = ArrayUnit[ActiveUnitIndex].Item1;
                            NewMap.ListPlayer[ActivePlayerIndex].ListSpawnPoint[ActiveSquadIndex].LeaderName = ArrayUnit[ActiveUnitIndex].Item2;
                        }
                        else if (PlayerChoiceSubMenuIndex == 1)
                        {
                            NewMap.ListPlayer[ActivePlayerIndex].ListSpawnPoint[ActiveSquadIndex].WingmanAPilot = ListPilot[ActivePilotIndex];
                            NewMap.ListPlayer[ActivePlayerIndex].ListSpawnPoint[ActiveSquadIndex].WingmanATypeName = ArrayUnit[ActiveUnitIndex].Item1;
                            NewMap.ListPlayer[ActivePlayerIndex].ListSpawnPoint[ActiveSquadIndex].WingmanAName = ArrayUnit[ActiveUnitIndex].Item2;
                        }
                        else if (PlayerChoiceSubMenuIndex == 2)
                        {
                            NewMap.ListPlayer[ActivePlayerIndex].ListSpawnPoint[ActiveSquadIndex].WingmanBPilot = ListPilot[ActivePilotIndex];
                            NewMap.ListPlayer[ActivePlayerIndex].ListSpawnPoint[ActiveSquadIndex].WingmanBTypeName = ArrayUnit[ActiveUnitIndex].Item1;
                            NewMap.ListPlayer[ActivePlayerIndex].ListSpawnPoint[ActiveSquadIndex].WingmanBName = ArrayUnit[ActiveUnitIndex].Item2;
                        }
                    }
                    else if (InputHelper.InputCancelPressed())
                    {
                        Stage = MenuChoices.UnitSelection;
                    }
                    else if (InputHelper.InputUpPressed())
                    {
                        ActivePilotIndex--;
                        if (ActivePilotIndex < 0)
                        {
                            ActivePilotIndex = ListPilot.Count - 1;
                            ActivePilotMinIndex = Math.Max(0, ActivePilotIndex - ListPilot.Count);
                        }
                        else if (ActivePilotIndex < ActivePilotMinIndex)
                            ActivePilotMinIndex = ActivePilotIndex;
                    }
                    else if (InputHelper.InputDownPressed())
                    {
                        ActivePilotIndex++;
                        if (ActivePilotIndex >= ListPilot.Count)
                        {
                            ActivePilotIndex = 0;
                            ActivePilotMinIndex = 0;
                        }
                        else if (ActivePilotIndex >= MaxItemToDraw + ActivePilotMinIndex)
                            ActivePilotMinIndex = ActivePilotIndex - MaxItemToDraw + 1;
                    }
                    break;

                #endregion
            }

            if (KeyboardHelper.KeyPressed(Keys.Escape))
            {
                if (Stage == MenuChoices.None)
                    RemoveScreen(this);
                else
                    Stage = MenuChoices.None;
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.End();
            g.Begin(SpriteSortMode.Deferred, null);

            #region Default Drawing

            g.Draw(sprPixel, new Rectangle(0, 0, Constants.Width, Constants.Height), Color.CornflowerBlue);
            //Right border
            DrawBox(g, new Vector2(Constants.Width - 200, 0), 200, 200, Color.White);
            g.Draw(sprPixel, new Rectangle(Constants.Width - 194, 6, 186, 186), Color.White);
            //Map preview
            DrawBox(g, new Vector2(Constants.Width - 200, 200), 200, 30, Color.White);
            g.DrawStringRightAligned(fntArial12, "Map name", new Vector2(Constants.Width - 65, 204), Color.Black);

            DrawBox(g, new Vector2(Constants.Width - 200, 230), 200, 171, Color.White);
            g.DrawString(fntArial12, "Max Number Of Players: " + MaxNumberOfPlayers, new Vector2(Constants.Width - 190, 230 + fntArial12.LineSpacing), Color.Black);
            g.DrawString(fntArial12, "Map size: " + MapSizeX + " x " + MapSizeY, new Vector2(Constants.Width - 190, 230 + fntArial12.LineSpacing * 3), Color.Black);
            g.DrawString(fntArial12, "Game mode: " + 0, new Vector2(Constants.Width - 190, 230 + fntArial12.LineSpacing * 4), Color.Black);

            DrawBox(g, new Vector2(Constants.Width - 200, Constants.Height - 79), 200, 26, Color.White);
            g.DrawString(fntArial12, "Join Game", new Vector2(Constants.Width - 140, Constants.Height - 74), Color.Black);

            DrawBox(g, new Vector2(Constants.Width - 200, Constants.Height - 53), 200, 26, Color.White);
            g.DrawString(fntArial12, "Change map", new Vector2(Constants.Width - 140, Constants.Height - 49), Color.Black);

            DrawBox(g, new Vector2(Constants.Width - 200, Constants.Height - 27), 200, 26, Color.White);
            g.DrawString(fntArial12, "Start game", new Vector2(Constants.Width - 140, Constants.Height - 23), Color.Black);

            //Player list
            DrawBox(g, new Vector2(0, 0), Constants.Width - 200, 26, Color.White);
            g.DrawString(fntArial12, "Player type", new Vector2(5, 3), Color.Black);
            g.DrawString(fntArial12, "Name", new Vector2(100, 3), Color.Black);
            g.DrawString(fntArial12, "Team", new Vector2(220, 3), Color.Black);
            g.DrawString(fntArial12, "Color", new Vector2(290, 3), Color.Black);
            g.DrawString(fntArial12, "Squads", new Vector2(370, 3), Color.Black);

            DrawBox(g, new Vector2(0, 26), Constants.Width - 200, Constants.Height - 130, Color.White);
            for (int P = 0; P < NewMap.ListPlayer.Count; P++)
            {
                if (NewMap.ListPlayer[P].IsOnline && NewMap.ListPlayer[P].OnlinePlayerType != "Open")
                    g.DrawString(fntArial12, "Online", new Vector2(5, 25 + P * fntArial12.LineSpacing), Color.Black);
                else
                    g.DrawString(fntArial12, NewMap.ListPlayer[P].OnlinePlayerType, new Vector2(5, 30 + P * fntArial12.LineSpacing), Color.Black);
                if (NewMap.ListPlayer[P].OnlinePlayerType == "Closed")
                    continue;

                g.DrawString(fntArial12, NewMap.ListPlayer[P].Name, new Vector2(100, 30 + P * fntArial12.LineSpacing), Color.Black);
                g.DrawString(fntArial12, NewMap.ListPlayer[P].TeamIndex.ToString(), new Vector2(230, 30 + P * fntArial12.LineSpacing), Color.Black);
            }
            //Messenger
            for (int i = 0; i < Messenger.Count(); i++)
            {
                g.DrawString(fntArial8, Messenger[i], new Vector2(5, Constants.Height - 105 + i * fntArial8.LineSpacing), Color.Black);
            }
            //Message box
            DrawBox(g, new Vector2(0, Constants.Height - 105), Constants.Width - 200, 80, Color.White);
            DrawBox(g, new Vector2(0, Constants.Height - 25), Constants.Width - 200, 25, Color.White);
            //Draw text.
            g.DrawString(fntArial12, MessageDraw, new Vector2(5, Constants.Height - 22), Color.Black);

            #endregion

            switch (Stage)
            {
                case MenuChoices.None:
                    switch (CurrentSelection)
                    {
                        case MenuChoices.PlayerList:
                            g.Draw(sprPixel, new Rectangle(0, 0, Constants.Width - 200, Constants.Height - 105), CursorColor);
                            break;

                        case MenuChoices.Messenger:
                            g.Draw(sprPixel, new Rectangle(0, Constants.Height - 105, Constants.Width - 200, 105), CursorColor);
                            break;

                        case MenuChoices.JoinGame:
                            g.Draw(sprPixel, new Rectangle(Constants.Width - 200, Constants.Height - 79, 200, 25), CursorColor);
                            break;

                        case MenuChoices.ChangeMap:
                            g.Draw(sprPixel, new Rectangle(Constants.Width - 200, Constants.Height - 53, 200, 25), CursorColor);
                            break;

                        case MenuChoices.StartGame:
                            g.Draw(sprPixel, new Rectangle(Constants.Width - 200, Constants.Height - 27, 200, 25), CursorColor);
                            break;
                    }
                    break;

                case MenuChoices.JoinGame:
                    IPInputBox.Draw(g);
                    break;

                case MenuChoices.PlayerList:
                    g.Draw(sprPixel, new Rectangle(4, 30 + ActivePlayerIndex * fntArial12.LineSpacing, Constants.Width - 209, fntArial12.LineSpacing), CursorColor);
                    break;

                #region PlayerEdit

                case MenuChoices.PlayerEdit:
                    switch (CurrentSelection)
                    {
                        case MenuChoices.PlayerType:
                            g.Draw(sprPixel, new Rectangle(4, 30 + ActivePlayerIndex * fntArial12.LineSpacing, 70, fntArial12.LineSpacing), CursorColor);
                            break;

                        case MenuChoices.SquadSelection:
                            g.Draw(sprPixel, new Rectangle(350, 30 + ActivePlayerIndex * fntArial12.LineSpacing, 85, fntArial12.LineSpacing), CursorColor);
                            break;
                    }
                    break;

                #endregion

                #region Player Type

                case MenuChoices.PlayerType:
                    DrawBox(g, new Vector2(0, 25 + ActivePlayerIndex * fntArial12.LineSpacing), 80, fntArial12.LineSpacing * 4 + 4, Color.White);
                    g.DrawString(fntArial12, "Closed", new Vector2(4, 27 + ActivePlayerIndex * fntArial12.LineSpacing), Color.Black);
                    g.DrawString(fntArial12, "Open", new Vector2(4, 27 + (ActivePlayerIndex + 1) * fntArial12.LineSpacing), Color.Black);
                    g.DrawString(fntArial12, "Human", new Vector2(4, 27 + (ActivePlayerIndex + 2) * fntArial12.LineSpacing), Color.Black);
                    g.DrawString(fntArial12, "CPU", new Vector2(4, 27 + (ActivePlayerIndex + 3) * fntArial12.LineSpacing), Color.Black);
                    g.Draw(sprPixel, new Rectangle(4, 29 + (ActivePlayerIndex + PlayerChoiceSubMenuIndex) * fntArial12.LineSpacing, 71, fntArial12.LineSpacing), CursorColor);
                    break;

                #endregion

                #region Squad Selection

                case MenuChoices.SquadSelection:
                    DrawBox(g, new Vector2(50, 50), Constants.Width - 100, Constants.Height - 100, Color.White);
                    g.DrawString(fntArial12, "Squad Name", new Vector2(55, 55), Color.White);
                    g.DrawString(fntArial12, "Leader", new Vector2(165, 55), Color.White);
                    g.DrawString(fntArial12, "Wingman A", new Vector2(295, 55), Color.White);
                    g.DrawString(fntArial12, "Wingman B", new Vector2(425, 55), Color.White);
                    for (int S = 0; S < NewMap.ListPlayer[ActivePlayerIndex].ListSpawnPoint.Count; S++)
                    {
                        g.DrawString(fntArial12, "Squad " + (S + 1), new Vector2(55, 75 + fntArial12.LineSpacing * S), Color.White);
                        if (NewMap.ListPlayer[ActivePlayerIndex].ListSpawnPoint[S].LeaderName != null)
                            g.DrawString(fntArial12, NewMap.ListPlayer[ActivePlayerIndex].ListSpawnPoint[S].LeaderName, new Vector2(160, 75 + fntArial12.LineSpacing * S), Color.White);
                        if (NewMap.ListPlayer[ActivePlayerIndex].ListSpawnPoint[S].WingmanAName != null)
                            g.DrawString(fntArial12, NewMap.ListPlayer[ActivePlayerIndex].ListSpawnPoint[S].WingmanAName, new Vector2(290, 75 + fntArial12.LineSpacing * S), Color.White);
                        if (NewMap.ListPlayer[ActivePlayerIndex].ListSpawnPoint[S].WingmanBName != null)
                            g.DrawString(fntArial12, NewMap.ListPlayer[ActivePlayerIndex].ListSpawnPoint[S].WingmanBName, new Vector2(420, 75 + fntArial12.LineSpacing * S), Color.White);
                    }
                    g.Draw(sprPixel, new Rectangle(160 + PlayerChoiceSubMenuIndex * 130, 55 + (ActiveSquadIndex + 1) * fntArial12.LineSpacing, 130, fntArial12.LineSpacing), CursorColor);
                    break;

                #endregion

                #region Unit Selection

                case MenuChoices.UnitSelection:
                    DrawBox(g, new Vector2(50, 50), Constants.Width - 100, Constants.Height - 100, Color.White);
                    g.DrawString(fntArial12, "Unit Type", new Vector2(55, 55), Color.White);
                    g.DrawString(fntArial12, "Unit Name", new Vector2(165, 55), Color.White);
                    for (int U = 0; U < MaxItemToDraw && U < ArrayUnit.Length; ++U)
                    {
                        g.DrawString(fntArial12, ArrayUnit[U + ActiveUnitMinIndex].Item1, new Vector2(55, 75 + fntArial12.LineSpacing * U), Color.White);
                        g.DrawString(fntArial12, ArrayUnit[U + ActiveUnitMinIndex].Item2, new Vector2(165, 75 + fntArial12.LineSpacing * U), Color.White);
                    }
                    g.Draw(sprPixel, new Rectangle(55, 55 + (ActiveUnitIndex + 1 - ActiveUnitMinIndex) * fntArial12.LineSpacing, Constants.Width - 110, fntArial12.LineSpacing), CursorColor);
                    break;

                #endregion

                #region Pilot Selection

                case MenuChoices.PilotSelection:
                    DrawBox(g, new Vector2(50, 50), Constants.Width - 100, Constants.Height - 100, Color.White);
                    g.DrawString(fntArial12, "Character Name", new Vector2(55, 55), Color.White);
                    int StartDrawingIndex = MaxItemToDraw - 1;
                    if (ListPilot.Count < StartDrawingIndex)
                        StartDrawingIndex = ListPilot.Count - ActivePilotMinIndex;
                    for (int U = StartDrawingIndex - 1; U >= 0; --U)
                    {
                        g.DrawString(fntArial12, ListPilot[U + ActivePilotMinIndex], new Vector2(55, 75 + fntArial12.LineSpacing * U), Color.White);
                    }
                    g.Draw(sprPixel, new Rectangle(55, 55 + (ActivePilotIndex + 1 - ActivePilotMinIndex) * fntArial12.LineSpacing, Constants.Width - 110, fntArial12.LineSpacing), CursorColor);
                    break;

                #endregion

                #region Change Map

                case MenuChoices.ChangeMap:
                    DrawBox(g, new Vector2(50, 50), Constants.Width - 100, Constants.Height - 100, Color.White);
                    for (int i = CursorMapMin; i < ListMap.Count && i < 10; i++)
                        g.DrawString(fntArial12, ListMap[i].Name, new Vector2(55, 50 + i * fntArial12.LineSpacing), Color.Black);

                    g.Draw(sprPixel, new Rectangle(50, 50 + CursorMap * fntArial12.LineSpacing, Constants.Width - 100, fntArial12.LineSpacing), CursorColor);
                    break;

                #endregion
            }
        }

        public void UpdateMessengerCursor()
        {
            if (MessageCursorIndex < MessageStartIndex)
            {
                while (MessageCursorIndex < MessageStartIndex)
                    MessageStartIndex--;
                MessageDraw = Message.Substring(MessageStartIndex);
                while (fntArial12.MeasureString(MessageDraw).X > Constants.Width - 210)
                    MessageDraw = MessageDraw.Substring(0, MessageDraw.Length - 1);
            }
            MessageCursorPosition = fntArial12.MeasureString(Message.Substring(MessageStartIndex, MessageCursorIndex - MessageStartIndex)).X;
            //Crop the text so it fits in the message box.
            if (MessageCursorPosition > Constants.Width - 210)
            {
                MessageDraw = Message.Substring(MessageStartIndex);
                while ((MessageDraw.Length + MessageStartIndex) > MessageCursorIndex)
                    MessageDraw = MessageDraw.Substring(0, MessageDraw.Length - 1);
                while (fntArial12.MeasureString(MessageDraw).X > Constants.Width - 210)
                {
                    MessageDraw = MessageDraw.Substring(1, MessageDraw.Length - 1);
                    MessageStartIndex++;
                }
                //Update the real cursor position.
                if (MessageCursorIndex - MessageStartIndex >= MessageDraw.Length)
                    MessageCursorPosition = fntArial12.MeasureString(MessageDraw).X;
            }
            else
                //Get the real cursor position.
                MessageCursorPosition = fntArial12.MeasureString(MessageDraw.Substring(0, MessageCursorIndex - MessageStartIndex)).X;
        }

        private Texture2D CreateMapPreview(MapAttributes ActiveMap)
        {
            int PreviewWidth = 200;
            int PreviewHeigh = 200;
            Texture2D MapPreview = new Texture2D(GraphicsDevice, PreviewWidth, PreviewHeigh);

            Color[] data = new Color[PreviewWidth * PreviewHeigh];
            for (int pixel = 0; pixel < data.Length; pixel++)
            {
                data[pixel] = Color.FromNonPremultiplied(255, 255, 255, 255);
            }

            foreach (EventPoint ActiveEvent in ActiveMap.ListSpawns)
            {
                int X = (int)ActiveEvent.Position.X;
                int Y = (int)ActiveEvent.Position.Y * ActiveMap.MapSize.X;
                data[X + Y] = Color.FromNonPremultiplied(255, 255, 255, 255);
            }

            //set the color
            MapPreview.SetData(data);
            return MapPreview;
        }
    }
}
