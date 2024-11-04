using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.BattleMapScreen.Online;
using ProjectEternity.Core.ControlHelper;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class SorcererStreetGamePreparationScreen : GamePreparationScreen
    {
        private enum ActiveDropdownTypes { None, PlayerType, SelectPlayerType, Team, SelectTeam, ConfigurePlayer, SelectConfigurePlayer }

        private Texture2D sprArrowDown;
        private Texture2D sprIconHuman;
        private Texture2D sprIconBot;

        private Matrix Projection;

        private ActiveDropdownTypes ActiveDropdownType;
        private int SelectedPlayerIndex;
        private int SelectedMenuIndex;

        private double HoverProgression;

        private int BoxWidth = 185;
        private int BoxHeight = 280;
        private int DrawY = 35;
        public static List<Player> ListAvailablePlayer;

        private int PlayerTypeItemCount => ListAvailablePlayer.Count + 3;

        public SorcererStreetGamePreparationScreen(BattleMapOnlineClient OnlineGameClient, CommunicationClient OnlineCommunicationClient, RoomInformations Room)
            : base(OnlineGameClient, OnlineCommunicationClient, Room)
        {
            Projection = Matrix.CreateOrthographicOffCenter(0, Constants.Width, Constants.Height, 0, 300, -300);
            Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            Projection = HalfPixelOffset * Projection;

            ListAvailablePlayer = new List<Player>();
            foreach (Player ActivePlayer in PlayerManager.ListLocalPlayer)
            {
                ListAvailablePlayer.Add(ActivePlayer);
            }
        }

        public override void Load()
        {
            base.Load();

            sprArrowDown = Content.Load<Texture2D>("Menus/Buttons/Arrow Down");
            sprIconHuman = Content.Load<Texture2D>("Menus/Buttons/Icon Human");
            sprIconBot = Content.Load<Texture2D>("Menus/Buttons/Icon Bot");
        }

        protected override void OpenRoomSettingsScreen()
        {
            PushScreen(new SorcererStreetGameOptionsScreen(Room, this));
        }

        public override void Update(GameTime gameTime)
        {
            HoverProgression += gameTime.ElapsedGameTime.TotalSeconds;
            base.Update(gameTime);
            foreach (Player ActivePlayer in Room.ListRoomPlayer)
            {
                if (ActivePlayer.Inventory.Character.Unit3DModel != null)
                {
                    ActivePlayer.Inventory.Character.Unit3DModel.Update(gameTime);
                }
            }

            if (ActiveDropdownType == ActiveDropdownTypes.PlayerType
                || ActiveDropdownType == ActiveDropdownTypes.ConfigurePlayer)
            {
                ActiveDropdownType = ActiveDropdownTypes.None;
            }

            if (ActiveDropdownType == ActiveDropdownTypes.None)
            {
                SelectedPlayerIndex = -1;
                for (int P = 0; P < Math.Max(Room.ListRoomPlayer.Count, Room.MaxNumberOfPlayer); P++)
                {
                    int X = 15 + P * BoxWidth;

                    if (MouseHelper.MouseStateCurrent.X >= X && MouseHelper.MouseStateCurrent.X <= X + BoxWidth - 20
                        && MouseHelper.MouseStateCurrent.Y >= DrawY + 215 && MouseHelper.MouseStateCurrent.Y <= DrawY + 240)
                    {
                        SelectedPlayerIndex = P;
                        ActiveDropdownType = ActiveDropdownTypes.PlayerType;

                        if (MouseHelper.InputLeftButtonPressed())
                        {
                            ActiveDropdownType = ActiveDropdownTypes.SelectPlayerType;
                        }
                    }
                    else if (MouseHelper.MouseStateCurrent.X >= X && MouseHelper.MouseStateCurrent.X <= X + BoxWidth - 20
                        && MouseHelper.MouseStateCurrent.Y >= DrawY + 260 && MouseHelper.MouseStateCurrent.Y <= DrawY + 385)
                    {
                        SelectedPlayerIndex = P;
                        ActiveDropdownType = ActiveDropdownTypes.ConfigurePlayer;

                        if (MouseHelper.InputLeftButtonPressed())
                        {
                            ActiveDropdownType = ActiveDropdownTypes.SelectConfigurePlayer;
                        }
                    }
                }
            }
            else if (ActiveDropdownType == ActiveDropdownTypes.SelectPlayerType)
            {
                int X = 15 + SelectedPlayerIndex * BoxWidth;

                for (int I = 0; I < PlayerTypeItemCount; ++I)
                {
                    int Y = DrawY + 218 + I * 25;
                    if (MouseHelper.MouseStateCurrent.X >= X && MouseHelper.MouseStateCurrent.X <= X + BoxWidth - 20
                        && MouseHelper.MouseStateCurrent.Y >= Y && MouseHelper.MouseStateCurrent.Y <= Y + 25)
                    {
                        SelectedMenuIndex = I;

                        if (MouseHelper.InputLeftButtonPressed())
                        {
                            AddBot(I);
                            ActiveDropdownType = ActiveDropdownTypes.None;
                            return;
                        }
                    }
                }
                if (MouseHelper.InputLeftButtonPressed())
                {
                    ActiveDropdownType = ActiveDropdownTypes.None;
                }
            }
            else if (ActiveDropdownType == ActiveDropdownTypes.SelectConfigurePlayer)
            {
                int X = 15 + SelectedPlayerIndex * BoxWidth;

                for (int I = 0; I < 2; ++I)
                {
                    int Y = DrawY + 218 + I * 25;
                    if (MouseHelper.MouseStateCurrent.X >= X && MouseHelper.MouseStateCurrent.X <= X + BoxWidth - 20
                        && MouseHelper.MouseStateCurrent.Y >= Y && MouseHelper.MouseStateCurrent.Y <= Y + 25)
                    {
                        SelectedMenuIndex = I;

                        if (MouseHelper.InputLeftButtonPressed())
                        {
                            if (I == 0)
                            {
                                PushScreen(new CharacterSelectionScreen(CardSymbols.Symbols, ListAvailablePlayer[SelectedPlayerIndex]));
                            }
                            else if (I == 1)
                            {
                                PushScreen(new ChooseBookScreen(CardSymbols.Symbols, ListAvailablePlayer[SelectedPlayerIndex]));
                            }
                            ActiveDropdownType = ActiveDropdownTypes.None;
                            return;
                        }
                    }
                }
                if (MouseHelper.InputLeftButtonPressed())
                {
                    ActiveDropdownType = ActiveDropdownTypes.None;
                }
            }
        }

        private void AddBot(int Index)
        {
            while (Room.ListRoomBot.Count <= Index)
            {
                Player NewBot = new Player(PlayerManager.OnlinePlayerID, "Bot " + (Index + 1), OnlinePlayerBase.PlayerTypes.Bot, false, 0, true, Color.Blue);
                NewBot.InitFirstTimeInventory();
                Room.ListRoomBot.Add(NewBot);
            }
        }

        protected override void DrawPlayers(CustomSpriteBatch g)
        {
            DrawY = 120;
            Matrix View = Matrix.Identity;
            for (int P = 0; P < Math.Max(Room.ListRoomPlayer.Count, Room.MaxNumberOfPlayer); P++)
            {
                int X = 5 + P * BoxWidth;

                DrawEmptyBox(g, new Vector2(X, DrawY), BoxWidth, BoxHeight);
                g.Draw(sprPixel, new Rectangle(X + 5, DrawY, BoxWidth - 10, BoxHeight), Color.FromNonPremultiplied(Lobby.BackgroundColor.R, Lobby.BackgroundColor.G, Lobby.BackgroundColor.B, 200));

                if (P < Room.ListRoomPlayer.Count)
                {
                    Player ActivePlayer = (Player)Room.ListRoomPlayer[P];

                    g.DrawStringCentered(fntText, ActivePlayer.Name, new Vector2(X + BoxWidth / 2, DrawY + 230), Color.White);
                    DrawEmptyBox(g, new Vector2(X + BoxWidth / 2 - sprIconHuman.Width / 2 - 5, DrawY + 177), sprIconHuman.Width + 5, sprIconHuman.Height + 5);
                    g.Draw(sprIconHuman, new Vector2(X + BoxWidth / 2 - sprIconHuman.Width / 2, DrawY + 180), Color.White);
                    g.Draw(sprArrowDown, new Vector2(X + BoxWidth - 40, DrawY + 212), Color.White);
                    g.DrawStringCentered(fntText, "Blue Team", new Vector2(X + BoxWidth / 2, DrawY + 255), Color.White);
                    g.DrawStringCentered(fntText, "Configure", new Vector2(X + BoxWidth / 2, DrawY + 275), Color.White);
                }
                else if (P < Room.ListRoomBot.Count)
                {
                    OnlinePlayerBase ActivePlayer = Room.ListRoomBot[P];

                    g.DrawStringCentered(fntText, ActivePlayer.Name, new Vector2(X + BoxWidth / 2, DrawY + 230), Color.White);
                    DrawEmptyBox(g, new Vector2(X + BoxWidth / 2 - sprIconHuman.Width / 2 - 5, DrawY + 177), sprIconHuman.Width + 5, sprIconHuman.Height + 5);
                    g.Draw(sprIconBot, new Vector2(X + BoxWidth / 2 - sprIconHuman.Width / 2, DrawY + 180), Color.White);
                    g.Draw(sprArrowDown, new Vector2(X + BoxWidth - 40, DrawY + 212), Color.White);
                    g.DrawStringCentered(fntText, "Blue Team", new Vector2(X + BoxWidth / 2, DrawY + 255), Color.White);
                    g.DrawStringCentered(fntText, "Configure", new Vector2(X + BoxWidth / 2, DrawY + 275), Color.White);
                }
                else
                {
                    g.DrawStringCentered(fntText, "Closed", new Vector2(X + BoxWidth / 2, DrawY + 230), Color.White);
                    g.Draw(sprArrowDown, new Vector2(X + BoxWidth - 40, DrawY + 212), Color.White);
                    g.DrawStringCentered(fntText, "Blue Team", new Vector2(X + BoxWidth / 2, DrawY + 255), Color.White);
                    g.DrawStringCentered(fntText, "Configure", new Vector2(X + BoxWidth / 2, DrawY + 275), Color.White);
                }
            }

            if (ActiveDropdownType == ActiveDropdownTypes.PlayerType)
            {
                g.Draw(sprPixel, new Rectangle(15 + SelectedPlayerIndex * BoxWidth, DrawY + 215, BoxWidth - 20, 25), Color.FromNonPremultiplied(255, 255, 255, 127));
            }
            else if (ActiveDropdownType == ActiveDropdownTypes.SelectPlayerType)
            {
                int X = 5 + SelectedPlayerIndex * BoxWidth;
                int Y = DrawY + 213;
                g.Draw(sprPixel, new Rectangle(X + 15, Y, BoxWidth - 30, PlayerTypeItemCount * 25 + 10), Lobby.BackgroundColor);
                DrawEmptyBox(g, new Vector2(X + 15, Y), BoxWidth - 30, PlayerTypeItemCount * 25 + 10, 10, HoverProgression);
                Y += 5;
                g.Draw(sprPixel, new Rectangle(X + 20, Y + SelectedMenuIndex * 25, BoxWidth - 40, 25), Color.FromNonPremultiplied(255, 255, 255, 127));

                Y += 13;
                for (int I = 0; I < ListAvailablePlayer.Count; ++I)
                {
                    g.DrawStringCentered(fntText, ListAvailablePlayer[I].Name, new Vector2(X + BoxWidth / 2, Y), Color.White);
                    Y += 25;
                }

                g.DrawStringCentered(fntText, "Bot", new Vector2(X + BoxWidth / 2, Y), Color.White);
                Y += 25;
                g.DrawStringCentered(fntText, "Open", new Vector2(X + BoxWidth / 2, Y), Color.White);
                Y += 25;
                g.DrawStringCentered(fntText, "Closed", new Vector2(X + BoxWidth / 2, Y), Color.White);
                Y += 25;
            }
            else if (ActiveDropdownType == ActiveDropdownTypes.ConfigurePlayer)
            {
                g.Draw(sprPixel, new Rectangle(15 + SelectedPlayerIndex * BoxWidth, DrawY + 260, BoxWidth - 20, 25), Color.FromNonPremultiplied(255, 255, 255, 127));
            }
            else if (ActiveDropdownType == ActiveDropdownTypes.SelectConfigurePlayer)
            {
                int X = 5 + SelectedPlayerIndex * BoxWidth;
                int NumberOfItems = ListAvailablePlayer.Count + 3;
                int Y = DrawY + 213;
                g.Draw(sprPixel, new Rectangle(X + 15, Y, BoxWidth - 30, NumberOfItems * 25 + 10), Lobby.BackgroundColor);
                DrawEmptyBox(g, new Vector2(X + 15, Y), BoxWidth - 30, NumberOfItems * 25 + 10, 10, HoverProgression);
                Y += 5;
                g.Draw(sprPixel, new Rectangle(X + 20, Y + SelectedMenuIndex * 25, BoxWidth - 40, 25), Color.FromNonPremultiplied(255, 255, 255, 127));

                Y += 13;
                g.DrawStringCentered(fntText, "Select Character", new Vector2(X + BoxWidth / 2, Y), Color.White);
                Y += 25;
                g.DrawStringCentered(fntText, "Edit Book", new Vector2(X + BoxWidth / 2, Y), Color.White);
            }

            g.End();
            g.Begin();

            GameScreen.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GameScreen.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GameScreen.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.White, 1f, 0);

            for (int P = 0; P < Math.Max(Room.ListRoomPlayer.Count, Room.MaxNumberOfPlayer); P++)
            {
                if (P < Room.ListRoomPlayer.Count)
                {
                    Player ActivePlayer = (Player)Room.ListRoomPlayer[P];
                    if (ActivePlayer.Inventory.Character.Unit3DModel == null)
                    {
                        continue;
                    }

                    float X = 5 + P * BoxWidth;
                    Matrix World = Matrix.CreateRotationX(MathHelper.ToRadians(180)) * Matrix.CreateScale(0.6f) * Matrix.CreateTranslation(X + BoxWidth / 2, DrawY + 170, 0);

                    ActivePlayer.Inventory.Character.Unit3DModel.PlayAnimation("Walking");
                    ActivePlayer.Inventory.Character.Unit3DModel.Draw(View, Projection, World);
                }
                else if (P < Room.ListRoomBot.Count)
                {
                    Player ActivePlayer = (Player)Room.ListRoomBot[P];
                    if (ActivePlayer.Inventory.Character.Unit3DModel == null)
                    {
                        continue;
                    }

                    float X = 5 + P * BoxWidth;
                    Matrix World = Matrix.CreateRotationX(MathHelper.ToRadians(180)) * Matrix.CreateScale(0.6f) * Matrix.CreateTranslation(X + BoxWidth / 2, DrawY + 170, 0);

                    ActivePlayer.Inventory.Character.Unit3DModel.PlayAnimation("Walking");
                    ActivePlayer.Inventory.Character.Unit3DModel.Draw(View, Projection, World);
                }
            }
        }
    }
    public class SorcererStreetLocalPlayerSelectionScreen : LocalPlayerSelectionScreen
    {
        public SorcererStreetLocalPlayerSelectionScreen()
        {
        }

        protected override OnlinePlayerBase GetNewPlayer()
        {
            Player NewPlayer = new Player(PlayerManager.OnlinePlayerID, "Player " + (PlayerManager.ListLocalPlayer.Count + 1), OnlinePlayerBase.PlayerTypes.Player, false, 0, true, Color.Blue);

            if (!File.Exists("User data/Profiles/" + NewPlayer.SaveFileFolder + NewPlayer.Name + ".bin"))
            {
                NewPlayer.InitFirstTimeInventory();
                NewPlayer.SaveLocally();
            }

            return NewPlayer;
        }
    }

    public class SorcererStreetGameOptionsScreen : GameOptionsScreen
    {
        public SorcererStreetGameOptionsScreen(RoomInformations Room, GamePreparationScreen Owner)
            : base(Room, Owner)
        {
        }

        protected override GameScreen GetGametypeScreen()
        {
            return new SorcererStreetGameOptionsGametypeScreen(Room, this);
        }
    }

    public class SorcererStreetGameOptionsGametypeScreen : GameOptionsGametypeScreen
    {
        public SorcererStreetGameOptionsGametypeScreen(RoomInformations Room, GameOptionsScreen Owner)
            : base(Room, Owner)
        {
        }

        protected override void LoadGameTypes()
        {
            GameModeInfo GametypeCampaign = new GameModeInfo("Campaign", "Classic mission based mode, no respawn.", GameModeInfo.CategoryPVE, true, null);

            GameModeInfo GametypeDeathmatch = new GameModeInfo("Deathmatch", "Gain points for kills and assists, respawn on death.", GameModeInfo.CategoryPVP, true, null);

            SelectedGametype = GametypeCampaign;
            ArrayGametypeCategory = new GametypeCategory[2];
            ArrayGametypeCategory[0] = new GametypeCategory("PVE", new GameModeInfo[] { GametypeCampaign });
            ArrayGametypeCategory[1] = new GametypeCategory("PVP", new GameModeInfo[]
            {
                GametypeDeathmatch, 
            });
        }
    }
}
