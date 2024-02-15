﻿using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class SorcererStreetGamePreparationScreen : GamePreparationScreen
    {
        Matrix Projection;
        public SorcererStreetGamePreparationScreen(BattleMapOnlineClient OnlineGameClient, CommunicationClient OnlineCommunicationClient, RoomInformations Room)
            : base(OnlineGameClient, OnlineCommunicationClient, Room)
        {
            Projection = Matrix.CreateOrthographicOffCenter(0, Constants.Width, Constants.Height, 0, 300, -300);
            Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            Projection = HalfPixelOffset * Projection;
        }

        protected override void OpenRoomSettingsScreen()
        {
            PushScreen(new SorcererStreetGameOptionsScreen(Room, this));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            foreach (Player ActivePlayer in Room.ListRoomPlayer)
            {
                if (ActivePlayer.GamePiece.Character.Unit3DModel != null)
                {
                    ActivePlayer.GamePiece.Character.Unit3DModel.Update(gameTime);
                }
            }
        }

        protected override void DrawPlayers(CustomSpriteBatch g)
        {
            Matrix View = Matrix.Identity;
            for (int P = 0; P < Room.ListRoomPlayer.Count; P++)
            {
                Player ActivePlayer = (Player)Room.ListRoomPlayer[P];

                float X = 5 + P * 125;

                DrawBox(g, new Vector2(X, 35), 125, 220, Color.White);
                g.DrawStringCentered(fntText, ActivePlayer.Name, new Vector2(X + 60, 235), Color.White);
            }

            GameScreen.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GameScreen.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GameScreen.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.White, 1f, 0);

            for (int P = 0; P < Room.ListRoomPlayer.Count; P++)
            {
                Player ActivePlayer = (Player)Room.ListRoomPlayer[P];
                if (ActivePlayer.GamePiece.Character.Unit3DModel == null)
                {
                    continue;
                }

                float X = 5 + P * 125;
                Matrix World = Matrix.CreateRotationX(MathHelper.ToRadians(180)) * Matrix.CreateScale(1f) * Matrix.CreateTranslation(X + 65, 210, 0);

                ActivePlayer.GamePiece.Character.Unit3DModel.PlayAnimation("Walking");
                ActivePlayer.GamePiece.Character.Unit3DModel.Draw(View, Projection, World);
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
