using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;
using ProjectEternity.Core;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class SorcererStreetGamePreparationScreen : GamePreparationScreen
    {
        public static Texture2D sprArrowDown;
        public static Texture2D sprIconHuman;
        public static Texture2D sprIconBot;

        private Matrix Projection;
        
        private ActionPanelHolder ListActionMenuChoice;

        public const int BoxWidth = 185;
        public const int BoxHeight = 296;
        private int DrawY = 35;
        public static List<Player> ListAvailablePlayer;//If a player is replaced by a bot

        private new SorcererStreetRoomInformations Room;

        public SorcererStreetGamePreparationScreen(BattleMapOnlineClient OnlineGameClient, CommunicationClient OnlineCommunicationClient, RoomInformations Room)
            : base(OnlineGameClient, OnlineCommunicationClient, Room)
        {
            this.Room = (SorcererStreetRoomInformations)Room;
            Projection = Matrix.CreateOrthographicOffCenter(0, Constants.Width, Constants.Height, 0, 300, -300);
            Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            Projection = HalfPixelOffset * Projection;

            ListAvailablePlayer = new List<Player>();
            ListActionMenuChoice = new ActionPanelHolder();
        }

        public override void Load()
        {
            base.Load();

            sprArrowDown = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Buttons/Arrow Down");
            sprIconHuman = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Buttons/Icon Human");
            sprIconBot = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Buttons/Icon Bot");

            ListActionMenuChoice.AddToPanelListAndSelect(new PlayerPreparationDefaultActionPanel(ListActionMenuChoice, fntText, Room, this));
        }

        public override GameOptionsScreen OpenRoomSettingsScreen()
        {
            GameOptionsScreen NewScreen = new SorcererStreetGameOptionsScreen(Room, this);
            PushScreen(NewScreen);

            return NewScreen;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            foreach (Player ActivePlayer in Room.ListRoomPlayer)
            {
                if (ActivePlayer != null && ActivePlayer.OnlinePlayerType != null && ActivePlayer.Inventory.Character.Character.Unit3DModel != null)
                {
                    ActivePlayer.Inventory.Character.Character.Unit3DModel.Update(gameTime);
                }
            }

            if (ListActionMenuChoice.HasMainPanel)
            {
                ListActionMenuChoice.Last().Update(gameTime);
            }
        }

        protected override void DrawPlayers(CustomSpriteBatch g)
        {
            float Ratio = Constants.Height / 2160f;
            if (Room.MapName != null)
            {
                if (Room.MapImage != null)
                {
                    g.DrawUnstretched(Room.MapImage, (int)(3260 * Ratio), (int)(140 * Ratio), (int)(800 * Ratio), (int)(450 * Ratio), new Vector2(Room.MapImage.Width / 2, 0));
                }

                DrawY = (int)(2732 * Ratio);
                g.DrawString(fntOxanimumBoldSmall, "Max Die Roll:", new Vector2(2816 * Ratio, DrawY * Ratio), TextColorDark);
                g.DrawStringRightAligned(fntOxanimumBoldSmall, Room.HighestDieRoll.ToString(), new Vector2(3688 * Ratio, DrawY * Ratio), TextColorDark);

                DrawY += (int)(148 * Ratio);
                g.DrawString(fntOxanimumBoldSmall, "Goal:", new Vector2(2816 * Ratio, DrawY * Ratio), TextColorDark);
                g.DrawStringRightAligned(fntOxanimumBoldSmall, Room.MagicGoal.ToString(), new Vector2(3688 * Ratio, DrawY * Ratio), TextColorDark);
            }

            DrawY = 120;
            for (int P = 0; P < Room.ListRoomPlayer.Count; P++)
            {
                int X = 5 + P * BoxWidth;

                g.Draw(GameScreen.sprPixel, new Rectangle(X + 5, DrawY, BoxWidth - 10, BoxHeight + 1), Color.FromNonPremultiplied(Lobby.BackgroundColor.R, Lobby.BackgroundColor.G, Lobby.BackgroundColor.B, 200));
                GameScreen.DrawEmptyBox(g, new Vector2(X, DrawY), BoxWidth, BoxHeight);

                if (Room.ListRoomPlayer[P] == null)
                {
                    g.DrawStringCentered(fntText, "Open", new Vector2(X + BoxWidth / 2, DrawY + 230), Color.White);
                    g.Draw(sprArrowDown, new Vector2(X + BoxWidth - 40, DrawY + 212), Color.White);
                    g.DrawStringCentered(fntText, "", new Vector2(X + BoxWidth / 2, DrawY + 255), Color.White);
                    g.DrawStringCentered(fntText, "", new Vector2(X + BoxWidth / 2, DrawY + 275), Color.White);
                }
                else if (Room.ListRoomPlayer[P].OnlinePlayerType == OnlinePlayerBase.PlayerTypePlayer)
                {
                    Player ActivePlayer = (Player)Room.ListRoomPlayer[P];

                    g.DrawStringCentered(fntText, ActivePlayer.Name, new Vector2(X + BoxWidth / 2, DrawY + 230), Color.White);
                    GameScreen.DrawEmptyBox(g, new Vector2(X + BoxWidth / 2 - sprIconHuman.Width / 2 - 5, DrawY + 177), sprIconHuman.Width + 5, sprIconHuman.Height + 5);
                    g.Draw(sprIconHuman, new Vector2(X + BoxWidth / 2 - sprIconHuman.Width / 2, DrawY + 180), Color.White);
                    g.Draw(sprArrowDown, new Vector2(X + BoxWidth - 40, DrawY + 212), Color.White);
                    g.Draw(sprArrowDown, new Vector2(X + BoxWidth - 40, DrawY + 212 + 25), Color.White);
                    g.Draw(sprArrowDown, new Vector2(X + BoxWidth - 40, DrawY + 212 + 25 * 2), Color.White);
                    if (ActivePlayer.TeamIndex == 0)
                    {
                        g.DrawStringCentered(fntText, "Red Team", new Vector2(X + BoxWidth / 2, DrawY + 255), Color.White);
                    }
                    else if (ActivePlayer.TeamIndex == 1)
                    {
                        g.DrawStringCentered(fntText, "Blue Team", new Vector2(X + BoxWidth / 2, DrawY + 255), Color.White);
                    }
                    else if (ActivePlayer.TeamIndex == 2)
                    {
                        g.DrawStringCentered(fntText, "Yellow Team", new Vector2(X + BoxWidth / 2, DrawY + 255), Color.White);
                    }
                    else if (ActivePlayer.TeamIndex == 3)
                    {
                        g.DrawStringCentered(fntText, "Green Team", new Vector2(X + BoxWidth / 2, DrawY + 255), Color.White);
                    }
                    g.DrawStringCentered(fntText, "Configure", new Vector2(X + BoxWidth / 2, DrawY + 280), Color.White);
                }
                else if (Room.ListRoomPlayer[P].OnlinePlayerType == OnlinePlayerBase.PlayerTypeBot)
                {
                    Player ActivePlayer = (Player)Room.ListRoomPlayer[P];

                    g.DrawStringCentered(fntText, "Bot", new Vector2(X + BoxWidth / 2, DrawY + 230), Color.White);
                    GameScreen.DrawEmptyBox(g, new Vector2(X + BoxWidth / 2 - sprIconHuman.Width / 2 - 5, DrawY + 177), sprIconHuman.Width + 5, sprIconHuman.Height + 5);
                    g.Draw(sprIconHuman, new Vector2(X + BoxWidth / 2 - sprIconHuman.Width / 2, DrawY + 180), Color.White);
                    g.Draw(sprArrowDown, new Vector2(X + BoxWidth - 40, DrawY + 212), Color.White);
                    g.Draw(sprArrowDown, new Vector2(X + BoxWidth - 40, DrawY + 212 + 25), Color.White);
                    g.Draw(sprArrowDown, new Vector2(X + BoxWidth - 40, DrawY + 212 + 25 * 2), Color.White);
                    if (ActivePlayer.TeamIndex == 0)
                    {
                        g.DrawStringCentered(fntText, "Red Team", new Vector2(X + BoxWidth / 2, DrawY + 255), Color.White);
                    }
                    else if (ActivePlayer.TeamIndex == 1)
                    {
                        g.DrawStringCentered(fntText, "Blue Team", new Vector2(X + BoxWidth / 2, DrawY + 255), Color.White);
                    }
                    else if (ActivePlayer.TeamIndex == 2)
                    {
                        g.DrawStringCentered(fntText, "Yellow Team", new Vector2(X + BoxWidth / 2, DrawY + 255), Color.White);
                    }
                    else if (ActivePlayer.TeamIndex == 3)
                    {
                        g.DrawStringCentered(fntText, "Green Team", new Vector2(X + BoxWidth / 2, DrawY + 255), Color.White);
                    }
                    g.DrawStringCentered(fntText, "Configure", new Vector2(X + BoxWidth / 2, DrawY + 280), Color.White);
                }
                else
                {
                    g.DrawStringCentered(fntText, "Closed", new Vector2(X + BoxWidth / 2, DrawY + 230), Color.White);
                    g.Draw(sprArrowDown, new Vector2(X + BoxWidth - 40, DrawY + 212), Color.White);
                    g.DrawStringCentered(fntText, "", new Vector2(X + BoxWidth / 2, DrawY + 255), Color.White);
                    g.DrawStringCentered(fntText, "", new Vector2(X + BoxWidth / 2, DrawY + 275), Color.White);
                }
            }

            if (ListActionMenuChoice.HasMainPanel)
            {
                ListActionMenuChoice.Last().Draw(g);
            }

            DrawY = 120;

            g.End();
            g.Begin();

            g.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            g.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            g.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GameScreen.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.White, 1f, 0);

            if (!IsOnTop)
            {
                return;
            }

            for (int P = 0; P < Room.ListRoomPlayer.Count; P++)
            {
                if (Room.ListRoomPlayer[P] == null)
                {
                    continue;
                }
                else if (Room.ListRoomPlayer[P].OnlinePlayerType == OnlinePlayerBase.PlayerTypeBot)
                {
                    Player ActivePlayer = (Player)Room.ListRoomBot[P];
                    float X = 5 + P * BoxWidth;
                    DrawCharacterPreview(g, ActivePlayer, (int)(X + BoxWidth / 2), DrawY + 170);

                }
                else if (Room.ListRoomPlayer[P].OnlinePlayerType == OnlinePlayerBase.PlayerTypePlayer)
                {
                    Player ActivePlayer = (Player)Room.ListRoomPlayer[P];
                    float X = 5 + P * BoxWidth;
                    DrawCharacterPreview(g, ActivePlayer, (int)(X + BoxWidth / 2), DrawY + 170);
                }
            }
        }

        private void DrawCharacterPreview(CustomSpriteBatch g, Player ActivePlayer, int X, int Y)
        {
            float Ratio = Constants.Height / 2160f;

            if (ActivePlayer.Inventory.Character.Character.Unit3DModel != null)
            {
                Matrix View = Matrix.Identity;
                Matrix World = Matrix.CreateRotationX(MathHelper.ToRadians(180)) * Matrix.CreateScale(0.6f) * Matrix.CreateTranslation(X, Y, 0);

                ActivePlayer.Inventory.Character.Character.Unit3DModel.ModelToDraw.PlayAnimation("Walking");
                ActivePlayer.Inventory.Character.Character.Unit3DModel.Draw3D(g.GraphicsDevice, View, Projection, World);
            }
            else if (ActivePlayer.Inventory.Character.Character.SpriteMap != null)
            {
                int SpriteWidthMax = (int)(300 * Ratio);
                int SpriteHeightMax = (int)(500 * Ratio);

                g.DrawUnstretched(ActivePlayer.Inventory.Character.Character.SpriteMap, X, Y, SpriteWidthMax, SpriteHeightMax, new Vector2(ActivePlayer.Inventory.Character.Character.SpriteMap.Width / 2, ActivePlayer.Inventory.Character.Character.SpriteMap.Height));
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
