﻿using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class LoadingScreen : GameScreen
    {
        private BattleMap ScreenToLoad;
        private BattleMapOnlineClient Client;
        private bool HasAskedForInfo;

        public LoadingScreen(BattleMap ScreenToLoad, BattleMapOnlineClient Client)
        {
            this.ScreenToLoad = ScreenToLoad;
            this.Client = Client;

            HasAskedForInfo = false;
        }

        public override void Load()
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (Client == null)
            {
                RemoveScreen(this);

                ScreenToLoad.Load();

                ListGameScreen.Insert(0, ScreenToLoad);
            }
            else if (Client != null && Client.IsConnected)
            {
                if (!HasAskedForInfo)
                {
                    Client.SetGame(ScreenToLoad);
                    Client.Host.Send(new AskGameDataScriptClient());
                    HasAskedForInfo = true;
                }
                else if (!ScreenToLoad.IsInit && Client.Host.IsGameReady)
                {
                    ScreenToLoad.Init();
                    ScreenToLoad.TogglePreview(true);
                    RemoveScreen(this);
                    ListGameScreen.Insert(0, ScreenToLoad);
                }
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.GraphicsDevice.Clear(Color.Black);
            DrawRectangle(g, Vector2.Zero, new Vector2(Constants.Width, Constants.Height), Color.Black);
            TextHelper.DrawText(g, "Loading", new Vector2(10, 10), Color.White);
        }
    }
}
