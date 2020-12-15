using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace Project_Eternity
{
    public sealed class MultiplayerScreen : GameScreen
    {
        private enum SelectChoice { PlayerList = 0, Messenger = 1, ChangeMap = 2, StartGame = 3 };
        private class MapAttributes
        {
            public string Name;
            private string Path;
            public int NumberOfPlayers;
            public int NumberOfTeam;
            public int UnitsPerTeam;
            public Point MapSize;
            public Color[] ColorSelection;

            public MapAttributes(string Name, string Path, int NumberOfPlayers, int NumberOfTeam, int UnitsPerTeam, Point MapSize, Color[] ColorSelection)
            {
                this.Name = Name;
                this.Path = Path;
                this.NumberOfPlayers = NumberOfPlayers;
                this.NumberOfTeam = NumberOfTeam;
                this.UnitsPerTeam = UnitsPerTeam;
                this.MapSize = MapSize;
                this.ColorSelection = ColorSelection;
            }
        }

        SelectChoice SelectedChoice = 0;
        int PlayerIndex;//Index of the cursor in the player list.
        int SubMenu;
        int PlayerOptionIndex;//Index of the selection menu used to change a player stats.
        int Stage;
        PlayerTypes[] Types;
        int CursorAlpha = 150;
        bool CursorAlphaAppearing = true;
        Texture2D sprRectangle;
        SpriteFont fntArial8;
        SpriteFont fntArial12;

        List<string> Messenger;//List of messages to draw.

        string Message;//Message being typed.
        string MessageDraw;//Croped message that fit in the text box.
        int MessageStartIndex;//Index of the starting letter of the message to draw.
        int MessageCursorIndex;//Index of the typing cursor in the message.
        float MessageCursorPosition;//X position of the cursor, based on its index.

        Color[] ColorSelection;
        List<Player> ListPlayer;

        DirectoryInfo dir;
        FileInfo[] ArrayMapFile;
        List<MapAttributes> ListMap;
        int CurrentMap;
        int CursorMap;
        int CursorMapMin;

        public MultiplayerScreen()
            : base()
        {
            this.RequireDrawFocus = true;
            Messenger = new List<string>();
            Message = "";
            MessageDraw = "";
            MessageStartIndex = 0;
            MessageCursorIndex = 0;
            SelectedChoice = SelectChoice.PlayerList;
            PlayerIndex = -1;
            Stage = -1;
            PlayerOptionIndex = -1;
            CurrentMap = 0;
        }

        public override void Load()
        {
            dir = new DirectoryInfo(Content.RootDirectory + "\\Maps");
            sprRectangle = Content.Load<Texture2D>("Rectangle");
            fntArial8 = Content.Load<SpriteFont>("Fonts/Arial8");
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            #region Players
            ListPlayer = new List<Player>();
            List<Unit> ListUnit;
            ListUnit = new List<Unit>();
            /*ListUnit.Add(new UnitArmoredCore(UnitArmoredCore.ListUnitArmoredCore[0]));
            ListUnit.Add(new UnitArmoredCore(UnitArmoredCore.ListUnitArmoredCore[0]));
            ListUnit.Add(new UnitArmoredCore(UnitArmoredCore.ListUnitArmoredCore[0]));
            ListPlayer.Add(new Player("Player", PlayerType.Human, ListUnit, 0));
            ListUnit = new List<Unit>();
            ListUnit.Add(new UnitArmoredCore(UnitArmoredCore.ListUnitArmoredCore[0]));
            ListUnit.Add(new UnitArmoredCore(UnitArmoredCore.ListUnitArmoredCore[0]));
            ListUnit.Add(new UnitArmoredCore(UnitArmoredCore.ListUnitArmoredCore[0]));
            ListPlayer.Add(new Player("Ally 1", PlayerType.CPUEasy, ListUnit, 0));
            ListUnit = new List<Unit>();
            ListUnit.Add(new UnitArmoredCore(UnitArmoredCore.ListUnitArmoredCore[0]));
            ListUnit[0].Init();
            ListUnit.Add(new UnitArmoredCore(UnitArmoredCore.ListUnitArmoredCore[0]));
            ListUnit[1].Init();
            ListUnit.Add(new UnitArmoredCore(UnitArmoredCore.ListUnitArmoredCore[0]));
            ListUnit[2].Init();
            ListPlayer.Add(new Player("Ally 2", PlayerType.CPUEasy, ListUnit, 0));
            ListUnit = new List<Unit>();
            ListUnit.Add(new UnitArmoredCore(UnitArmoredCore.ListUnitArmoredCore[0]));
            ListUnit[0].Init();
            ListUnit.Add(new UnitArmoredCore(UnitArmoredCore.ListUnitArmoredCore[0]));
            ListUnit[1].Init();
            ListUnit.Add(new UnitArmoredCore(UnitArmoredCore.ListUnitArmoredCore[0]));
            ListUnit[2].Init();
            ListPlayer.Add(new Player("Enemy 1", PlayerType.CPUEasy, ListUnit, 1));
            ListUnit = new List<Unit>();
            ListUnit.Add(new UnitArmoredCore(UnitArmoredCore.ListUnitArmoredCore[0]));
            ListUnit[0].Init();
            ListUnit.Add(new UnitArmoredCore(UnitArmoredCore.ListUnitArmoredCore[0]));
            ListUnit[1].Init();
            ListUnit.Add(new UnitArmoredCore(UnitArmoredCore.ListUnitArmoredCore[0]));
            ListUnit[2].Init();
            ListPlayer.Add(new Player("Enemy 2", PlayerType.CPUEasy, ListUnit, 1));
            ListUnit = new List<Unit>();
            ListUnit.Add(new UnitArmoredCore(UnitArmoredCore.ListUnitArmoredCore[0]));
            ListUnit[0].Init();
            ListUnit.Add(new UnitArmoredCore(UnitArmoredCore.ListUnitArmoredCore[0]));
            ListUnit[1].Init();
            ListUnit.Add(new UnitArmoredCore(UnitArmoredCore.ListUnitArmoredCore[0]));
            ListUnit[2].Init();
            ListPlayer.Add(new Player("Enemy 3", PlayerType.CPUEasy, ListUnit, 2));
            ListUnit = new List<Unit>();
            ListUnit.Add(new UnitArmoredCore(UnitArmoredCore.ListUnitArmoredCore[0]));
            ListUnit[0].Init();
            ListUnit.Add(new UnitArmoredCore(UnitArmoredCore.ListUnitArmoredCore[0]));
            ListUnit[1].Init();
            ListUnit.Add(new UnitArmoredCore(UnitArmoredCore.ListUnitArmoredCore[0]));
            ListUnit[2].Init();
            ListPlayer.Add(new Player("Enemy 4", PlayerType.CPUEasy, ListUnit, 2));
            ListUnit = new List<Unit>();
            ListUnit.Add(new UnitArmoredCore(UnitArmoredCore.ListUnitArmoredCore[0]));
            ListUnit[0].Init();
            ListUnit.Add(new UnitArmoredCore(UnitArmoredCore.ListUnitArmoredCore[0]));
            ListUnit[1].Init();
            ListUnit.Add(new UnitArmoredCore(UnitArmoredCore.ListUnitArmoredCore[0]));*/
            ListUnit[2].Init();
            ListPlayer.Add(new Player("Enemy 5", PlayerTypes.CPUEasy, ListUnit, 3));
            #endregion
            //Load directory info, abort if none
            if (!dir.Exists)
                throw new DirectoryNotFoundException();
            ListMap = new List<MapAttributes>();
            //Load all files that matches the file filter
            ArrayMapFile = dir.GetFiles("*.pem");
            foreach (FileInfo file in ArrayMapFile)
            {
                FileStream FS = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
                BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
                Point NewMapSize = Point.Zero;
                BR.BaseStream.Seek(0, SeekOrigin.Begin);
                string MapName = BR.ReadString();
                NewMapSize.X = BR.ReadInt32();
                NewMapSize.Y = BR.ReadInt32();
                int TileSizeX = BR.ReadInt32();
                int TileSizeY = BR.ReadInt32();
                int NumberOfTeams = BR.ReadInt32();//Number of teams.
                int UnitsPerTeam = BR.ReadInt32();//Number of Units per team.

                Color[] ColorSelection = new Color[16];
                //Deathmatch colors
                for (int D = 0; D < 16; D++)
                {
                    int Red = BR.ReadByte();
                    int Green = BR.ReadByte();
                    int Blue = BR.ReadByte();
                    ColorSelection[D] = Color.FromNonPremultiplied(Red, Green, Blue, 255);
                }

                ListMap.Add(new MapAttributes(MapName, file.FullName, NumberOfTeams, NumberOfTeams, UnitsPerTeam, new Point(NewMapSize.X, NewMapSize.Y), ColorSelection));
                FS.Close();
            }
        }
        public override void Update(GameTime gameTime)
        {
            if (CursorAlphaAppearing)
            {//Increment SelectedAlpha before comparing it to 200
                CursorAlpha += 5;
                if (CursorAlpha >= 200)
                    CursorAlphaAppearing = false;
            }
            else
            {//Decrement SelectedAlpha before comparing it to 55
                CursorAlpha -= 5;
                if (CursorAlpha <= 55)
                    CursorAlphaAppearing = true;
            }

            #region Stage == -1
            if (Stage == -1)
            {
                if (KeyboardHelper.InputUpPressed())
                {
                    if ((int)SelectedChoice < 2)
                        SelectedChoice -= (int)SelectedChoice > 0 ? 1 : 0;
                    else if ((int)SelectedChoice < 4)
                        SelectedChoice -= (int)SelectedChoice > 2 ? 1 : 0;
                }
                else if (KeyboardHelper.InputDownPressed())
                {
                    if ((int)SelectedChoice < 2)
                        SelectedChoice += (int)SelectedChoice < 1 ? 1 : 0;
                    else if ((int)SelectedChoice < 4)
                        SelectedChoice += (int)SelectedChoice < 3 ? 1 : 0;
                }
                if (KeyboardHelper.InputLeftPressed() && (int)SelectedChoice >= 2)
                        SelectedChoice = SelectChoice.PlayerList;
                else if (KeyboardHelper.InputRightPressed() && (int)SelectedChoice < 2)
                    SelectedChoice = SelectChoice.ChangeMap;
                else if (KeyboardHelper.InputConfirmPressed())
                {
                    switch (SelectedChoice)
                    {
                        case SelectChoice.PlayerList:
                            PlayerIndex = 0;
                            break;
                        case SelectChoice.ChangeMap:
                            CursorMap = CurrentMap;
                            break;
                        case SelectChoice.StartGame:
                            GameScreen.PushScreen(new DeathmatchMap(ArrayMapFile[CurrentMap].FullName, ListPlayer, 1));
                            GameScreen.RemoveScreen(this);
                            break;
                    }
                    Stage = 0;
                }

            }
            #endregion
            else
            {
                switch (SelectedChoice)
                {
                    #region Messenger
                    case SelectChoice.Messenger:

                        Microsoft.Xna.Framework.Input.Keys[] Input = KeyboardHelper.KeyPressed();
                        for (int i = 0; i < Input.Count(); i++)
                        {
                            if (KeyboardHelper.KeyPressed(Input[i]))
                            {
                                if (Input[i] == Microsoft.Xna.Framework.Input.Keys.Space)
                                {
                                    Message = Message.Insert(MessageCursorIndex, " ");
                                    MessageDraw = MessageDraw.Insert(MessageCursorIndex, " ");
                                    MessageCursorIndex++;
                                    UpdateMessengerCursor();
                                }
                                else if (Input[i] == Microsoft.Xna.Framework.Input.Keys.Enter)
                                {
                                    Messenger.Add(Message);
                                    Message = "";
                                    MessageDraw = "";
                                    MessageCursorIndex = 0;
                                }
                                else if (Input[i] == Microsoft.Xna.Framework.Input.Keys.Back)
                                {
                                    if (Message.Length > 0)
                                    {
                                        MessageCursorIndex--;
                                        Message = Message.Remove(MessageCursorIndex, 1);
                                        MessageDraw = MessageDraw.Remove(MessageCursorIndex, 1);
                                        UpdateMessengerCursor();
                                    }
                                }
                                else if ((int)Input[i] >= 65 && (int)Input[i] <= 90)
                                {
                                    Message = Message.Insert(MessageCursorIndex, Input[i].ToString());
                                    MessageDraw = MessageDraw.Insert(MessageCursorIndex, Input[i].ToString());
                                    MessageCursorIndex++;
                                    UpdateMessengerCursor();
                                }
                                else if (Input[i] == Microsoft.Xna.Framework.Input.Keys.Left)
                                {
                                    MessageCursorIndex -= MessageCursorIndex > 0 ? 1 : 0;
                                    UpdateMessengerCursor();
                                }
                                else if (Input[i] == Microsoft.Xna.Framework.Input.Keys.Right)
                                {
                                    MessageCursorIndex += MessageCursorIndex < Message.Count() ? 1 : 0;
                                    UpdateMessengerCursor();
                                }
                            }
                        }
                        if (KeyboardHelper.InputCancelPressed())
                            Stage = -1;
                        break;
                    #endregion
                    #region PlayerList
                    case SelectChoice.PlayerList:
                        switch (Stage)
                        {
                            case 0://Player selection
                                if (KeyboardHelper.InputUpPressed())
                                    PlayerIndex -= PlayerIndex > 0 ? 1 : 0;
                                else if (KeyboardHelper.InputDownPressed())
                                    PlayerIndex += PlayerIndex < Math.Min(ListPlayer.Count, ListMap[CursorMap].NumberOfPlayers - 1) ? 1 : 0;
                                if (KeyboardHelper.InputLeftPressed()
                                    || KeyboardHelper.InputRightPressed()
                                    || KeyboardHelper.InputConfirmPressed())
                                {
                                    SubMenu = 0;
                                    Stage++;
                                }
                                else if (KeyboardHelper.InputCancelPressed())
                                    Stage = -1;
                                break;
                            case 1://SubMenu selection
                                if (KeyboardHelper.InputUpPressed())
                                    PlayerIndex -= PlayerIndex > 0 ? 1 : 0;
                                else if (KeyboardHelper.InputDownPressed())
                                {
                                    PlayerIndex += PlayerIndex < Math.Min(ListPlayer.Count, ListMap[CursorMap].NumberOfPlayers - 1) ? 1 : 0;
                                    if (PlayerIndex == ListMap[CursorMap].NumberOfPlayers - 1 && ListPlayer.Count < ListMap[CursorMap].NumberOfPlayers)
                                        SubMenu = 0;
                                }
                                if (KeyboardHelper.InputLeftPressed() && (PlayerIndex != ListMap[CursorMap].NumberOfPlayers - 1 || ListPlayer.Count >= ListMap[CursorMap].NumberOfPlayers))
                                    SubMenu -= SubMenu > 0 ? 1 : 0;
                                else if (KeyboardHelper.InputRightPressed() && (PlayerIndex != ListMap[CursorMap].NumberOfPlayers - 1 || ListPlayer.Count >= ListMap[CursorMap].NumberOfPlayers))
                                    SubMenu += SubMenu < 2 ? 1 : 0;
                                else if (KeyboardHelper.InputCancelPressed())
                                    Stage--;
                                else if (KeyboardHelper.InputConfirmPressed())
                                {
                                    Stage++;
                                    PlayerOptionIndex = 0;
                                    if (SubMenu == 0)
                                    {//If the index is on the last player or on an empty space
                                        if (PlayerIndex == ListPlayer.Count - 1 || PlayerIndex >= ListPlayer.Count)
                                            Types = new PlayerTypes[] { PlayerTypes.Closed, PlayerTypes.Human, PlayerTypes.CPUEasy };
                                        else
                                            Types = new PlayerTypes[] { PlayerTypes.Human, PlayerTypes.CPUEasy };
                                    }
                                }
                                break;
                            case 2://PlayerOption selection
                                switch (SubMenu)
                                {
                                    case 0://Player type
                                        if (KeyboardHelper.InputUpPressed())
                                            PlayerOptionIndex -= PlayerOptionIndex > 0 ? 1 : 0;
                                        else if (KeyboardHelper.InputDownPressed())
                                            PlayerOptionIndex += PlayerOptionIndex < Types.Count() - 1 ? 1 : 0;
                                        else if (KeyboardHelper.InputConfirmPressed())
                                        {
                                            if (PlayerIndex < ListPlayer.Count)
                                            {
                                                if (Types[PlayerOptionIndex] == PlayerTypes.Closed)
                                                    ListPlayer.RemoveAt(PlayerIndex);
                                                else
                                                    ListPlayer[PlayerIndex].PType = Types[PlayerOptionIndex];
                                            }
                                            else
                                            {
                                                if (Types[PlayerOptionIndex] == PlayerTypes.Closed)
                                                    ListPlayer.RemoveAt(PlayerIndex);
                                                else
                                                {
                                                    List<Unit> ListUnit;
                                                    ListUnit = new List<Unit>();
                                                    /*ListUnit.Add(new UnitArmoredCore(UnitArmoredCore.ListUnitArmoredCore[0]));
                                                    ListUnit[0].Init();
                                                    ListUnit.Add(new UnitArmoredCore(UnitArmoredCore.ListUnitArmoredCore[0]));
                                                    ListUnit[1].Init();
                                                    ListUnit.Add(new UnitArmoredCore(UnitArmoredCore.ListUnitArmoredCore[0]));
                                                    ListUnit[2].Init();*/
                                                    ListPlayer.Add(new Player("New player", Types[PlayerOptionIndex], ListUnit, 0));
                                                }
                                            }
                                            Stage--;
                                        }
                                        else if (KeyboardHelper.InputCancelPressed())
                                            Stage--;
                                        break;
                                    case 1://Team
                                        if (KeyboardHelper.InputUpPressed())
                                            PlayerOptionIndex -= PlayerOptionIndex > 0 ? 1 : 0;
                                        else if (KeyboardHelper.InputDownPressed())
                                            PlayerOptionIndex += PlayerOptionIndex < ListMap[CursorMap].NumberOfTeam - 1 ? 1 : 0;
                                        else if (KeyboardHelper.InputConfirmPressed())
                                        {
                                            ListPlayer[PlayerIndex].Team = PlayerOptionIndex;
                                            Stage--;
                                        }
                                        else if (KeyboardHelper.InputCancelPressed())
                                            Stage--;
                                        break;
                                }
                                break;
                        }
                        break;
                    #endregion
                    case SelectChoice.ChangeMap:
                        if (KeyboardHelper.InputUpPressed())
                        {
                            CursorMap--;
                            if (CursorMap < 0)
                                CursorMap = ListMap.Count - 1;
                        }
                        else if (KeyboardHelper.InputDownPressed())
                        {
                            CursorMap++;
                            if (CursorMap > ListMap.Count - 1)
                                CursorMap = 0;
                        }
                        else if (KeyboardHelper.InputConfirmPressed())
                        {
                            CurrentMap = CursorMap;
                            Stage--;
                        }
                        else if (KeyboardHelper.InputCancelPressed())
                            Stage--;
                        break;
                }
            }
            if (KeyboardHelper.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
                GameScreen.RemoveScreen(this);
        }
        public override void Draw(SpriteBatch g)
        {
            //Right border
            g.Draw(sprRectangle, new Rectangle(Game.Width - 200, 0, 1, Game.Height), Color.Black);
            //Map preview
            g.Draw(sprRectangle, new Rectangle(Game.Width - 196, 0, 192, 192), Color.White);
            g.Draw(sprRectangle, new Rectangle(Game.Width - 200, 196, 200, 1), Color.Black);
            g.DrawString(fntArial12, "Map name", new Vector2(Game.Width - 65, 201), Color.Black, 0, new Vector2(fntArial12.MeasureString("Map name").X, 0), 1, SpriteEffects.None, 0);
            g.Draw(sprRectangle, new Rectangle(Game.Width - 200, 226, 200, 1), Color.Black);

            g.DrawString(fntArial12, "Number of players: " + ListMap[CursorMap].NumberOfPlayers, new Vector2(Game.Width - 190, 230), Color.Black);
            g.DrawString(fntArial12, "Number of teams: " + ListMap[CursorMap].NumberOfTeam, new Vector2(Game.Width - 190, 230 + fntArial12.LineSpacing), Color.Black);
            g.DrawString(fntArial12, "Units per team: " + ListMap[CursorMap].UnitsPerTeam, new Vector2(Game.Width - 190, 230 + fntArial12.LineSpacing * 2), Color.Black);
            g.DrawString(fntArial12, "Map size: " + ListMap[CursorMap].MapSize.X + " x " + ListMap[CursorMap].MapSize.Y, new Vector2(Game.Width - 190, 230 + fntArial12.LineSpacing * 3), Color.Black);
            g.DrawString(fntArial12, "Game mode: " + 0, new Vector2(Game.Width - 190, 230 + fntArial12.LineSpacing * 4), Color.Black);

            g.Draw(sprRectangle, new Rectangle(Game.Width - 200, Game.Height - 50, 200, 1), Color.Black);
            g.DrawString(fntArial12, "Change map", new Vector2(Game.Width - 140, Game.Height - 46), Color.Black);
            g.Draw(sprRectangle, new Rectangle(Game.Width - 200, Game.Height - 25, 200, 1), Color.Black);
            g.DrawString(fntArial12, "Start game", new Vector2(Game.Width - 140, Game.Height - 21), Color.Black);

            if (Stage == -1)
            {
                //Highlight the current selection.
                if (SelectedChoice == SelectChoice.PlayerList)
                    g.Draw(sprRectangle, new Rectangle(0, 24, 446, 379), Color.FromNonPremultiplied(255, 255, 255, CursorAlpha));
                else if (SelectedChoice == SelectChoice.Messenger)
                    g.Draw(sprRectangle, new Rectangle(0, Game.Height - 24, 446, 24), Color.FromNonPremultiplied(255, 255, 255, CursorAlpha));
                else if (SelectedChoice == SelectChoice.ChangeMap)
                    g.Draw(sprRectangle, new Rectangle(Game.Width - 199, Game.Height - 50, 200, 25), Color.FromNonPremultiplied(255, 255, 255, CursorAlpha));
                else if (SelectedChoice == SelectChoice.StartGame)
                    g.Draw(sprRectangle, new Rectangle(Game.Width - 199, Game.Height - 25, 200, 25), Color.FromNonPremultiplied(255, 255, 255, CursorAlpha));
                //g.Draw(sprRectangle, new Rectangle(Game.Width - 199, 227, 200, 281), Color.FromNonPremultiplied(255, 255, 255, CursorAlpha));
                DrawLeftScreen(g);
            }
            else
            {
                switch (SelectedChoice)
                {
                    #region PlayerList
                    case SelectChoice.PlayerList:
                        DrawLeftScreen(g);
                        switch (Stage)
                        {
                            case 0:
                                //Draw cursor.
                                g.Draw(sprRectangle, new Rectangle(0, 24 + PlayerIndex * fntArial12.LineSpacing, 446, fntArial12.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, CursorAlpha));
                                break;
                            case 1:
                                switch (SubMenu)
                                {
                                    case 0://Player type
                                        g.Draw(sprRectangle, new Rectangle(0, 24 + PlayerIndex * fntArial12.LineSpacing, 100, fntArial12.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, CursorAlpha));
                                        break;
                                    case 1://Team
                                        g.Draw(sprRectangle, new Rectangle(230, 24 + PlayerIndex * fntArial12.LineSpacing, 55, fntArial12.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, CursorAlpha));
                                        break;
                                    case 2://Color
                                        g.Draw(sprRectangle, new Rectangle(289, 24 + PlayerIndex * fntArial12.LineSpacing, fntArial12.LineSpacing, fntArial12.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, CursorAlpha));
                                        break;
                                }
                                break;
                            case 2:
                                switch (SubMenu)
                                {
                                    case 0://Player type
                                        g.Draw(sprRectangle, new Rectangle(0, 24 + (PlayerIndex + 1) * fntArial12.LineSpacing, 100, fntArial12.LineSpacing * Types.Count()), Color.DarkBlue);
                                        for (int i = 0; i < Types.Count(); i++)
                                        {
                                            g.DrawString(fntArial12, Types[i].ToString(), new Vector2(0, 24 + (PlayerIndex + 1) * fntArial12.LineSpacing + i * fntArial12.LineSpacing), Color.White);
                                            if (i == PlayerOptionIndex)
                                                g.Draw(sprRectangle, new Rectangle(0, 24 + (PlayerIndex + 1) * fntArial12.LineSpacing + i * fntArial12.LineSpacing, 100, fntArial12.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, CursorAlpha));
                                        }
                                        break;
                                    case 1://Team
                                        g.Draw(sprRectangle, new Rectangle(230, 24 + (PlayerIndex + 1) * fntArial12.LineSpacing, 100, fntArial12.LineSpacing * ListMap[CursorMap].NumberOfTeam), Color.DarkBlue);
                                        for (int i = 0; i < ListMap[CursorMap].NumberOfTeam; i++)
                                        {
                                            g.DrawString(fntArial12, i.ToString(), new Vector2(230, 24 + (PlayerIndex + 1) * fntArial12.LineSpacing + i * fntArial12.LineSpacing), Color.White);
                                            if (i == PlayerOptionIndex)
                                                g.Draw(sprRectangle, new Rectangle(230, 24 + (PlayerIndex + 1) * fntArial12.LineSpacing + i * fntArial12.LineSpacing, 100, fntArial12.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, CursorAlpha));
                                        }
                                        break;
                                    case 2://Color
                                        g.Draw(sprRectangle, new Rectangle(289, 24 + (PlayerIndex + 1) * fntArial12.LineSpacing, fntArial12.LineSpacing, fntArial12.LineSpacing * ColorSelection.Count()), Color.DarkBlue);
                                        for (int i = 0; i < ColorSelection.Count(); i++)
                                        {
                                            g.Draw(sprRectangle, new Rectangle(290, 25 + (PlayerIndex + 1) * fntArial12.LineSpacing + i * fntArial12.LineSpacing, fntArial12.LineSpacing - 2, fntArial12.LineSpacing - 2), ColorSelection[i]);
                                            if (i == PlayerOptionIndex)
                                                g.Draw(sprRectangle, new Rectangle(289, 24 + (PlayerIndex + 1) * fntArial12.LineSpacing + i * fntArial12.LineSpacing, fntArial12.LineSpacing, fntArial12.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, CursorAlpha));
                                        }
                                        break;
                                }
                                break;
                        }
                        break;
                    #endregion
                    case SelectChoice.ChangeMap:
                        for (int i = CursorMapMin; i < ListMap.Count && i < 10; i++)
                        {
                            g.DrawString(fntArial12, ListMap[i].Name, new Vector2(5, 3 + i * fntArial12.LineSpacing), Color.Black);
                            if (i == CursorMap)//Draw cursor.
                                g.Draw(sprRectangle, new Rectangle(0, 3 + CursorMap * fntArial12.LineSpacing, 446, fntArial12.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, CursorAlpha));
                        }
                        break;
                }
            }
        }

        public void DrawLeftScreen(SpriteBatch g)
        {
            //Player list
            g.DrawString(fntArial12, "Player type", new Vector2(5, 3), Color.Black);
            g.DrawString(fntArial12, "Name", new Vector2(100, 3), Color.Black);
            g.DrawString(fntArial12, "Team", new Vector2(220, 3), Color.Black);
            g.DrawString(fntArial12, "Color", new Vector2(290, 3), Color.Black);
            g.DrawString(fntArial12, "Ping", new Vector2(390, 3), Color.Black);
            g.Draw(sprRectangle, new Rectangle(0, 23, Game.Width - 200, 1), Color.Black);
            for (int P = 0; P < ListPlayer.Count; P++)
            {
                g.DrawString(fntArial12, ListPlayer[P].PType.ToString(), new Vector2(5, 25 + P * fntArial12.LineSpacing), Color.Black);
                g.DrawString(fntArial12, ListPlayer[P].Name, new Vector2(100, 25 + P * fntArial12.LineSpacing), Color.Black);
                g.DrawString(fntArial12, ListPlayer[P].Team.ToString(), new Vector2(230, 25 + P * fntArial12.LineSpacing), Color.Black);
                //g.Draw(sprRectangle, new Rectangle(290, 25 + P * fntArial12.LineSpacing, fntArial12.LineSpacing - 2, fntArial12.LineSpacing - 2), ListPlayer[P].Color);
            }
            if (ListPlayer.Count < ListMap[CursorMap].NumberOfPlayers)
            {
                g.DrawString(fntArial12, "Closed", new Vector2(5, 25 + ListPlayer.Count * fntArial12.LineSpacing), Color.Black);
            }
            //Messenger
            g.Draw(sprRectangle, new Rectangle(0, Game.Height - 105, Game.Width - 200, 1), Color.Black);
            for (int i = 0; i < Messenger.Count(); i++)
            {
                g.DrawString(fntArial8, Messenger[i], new Vector2(5, Game.Height - 105 + i * fntArial8.LineSpacing), Color.Black);
            }
            //Message box
            g.Draw(sprRectangle, new Rectangle(0, Game.Height - 25, Game.Width - 200, 1), Color.Black);
            //Draw cursor.
            g.Draw(sprRectangle, new Rectangle(5 + (int)MessageCursorPosition, Game.Height - 24, 1, 23), Color.FromNonPremultiplied(0, 0, 0, CursorAlpha));
            //Draw text.
            g.DrawString(fntArial12, MessageDraw, new Vector2(5, Game.Height - 22), Color.Black);
        }

        public void UpdateMessengerCursor()
        {
            if (MessageCursorIndex < MessageStartIndex)
            {
                while (MessageCursorIndex < MessageStartIndex)
                    MessageStartIndex--;
                MessageDraw = Message.Substring(MessageStartIndex);
                while (fntArial12.MeasureString(MessageDraw).X > Game.Width - 210)
                    MessageDraw = MessageDraw.Substring(0, MessageDraw.Length - 1);
            }
            MessageCursorPosition = fntArial12.MeasureString(Message.Substring(MessageStartIndex, MessageCursorIndex - MessageStartIndex)).X;
            //Crop the text so it fits in the message box.
            if (MessageCursorPosition > Game.Width - 210)
            {
                MessageDraw = Message.Substring(MessageStartIndex);
                while ((MessageDraw.Length + MessageStartIndex) > MessageCursorIndex)
                    MessageDraw = MessageDraw.Substring(0, MessageDraw.Length - 1);
                while (fntArial12.MeasureString(MessageDraw).X > Game.Width - 210)
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
    }
}
