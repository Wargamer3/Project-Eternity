using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.TripleThunderScreen.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    class LoadingScreen : GameScreen
    {
        private FightingZone ScreenToLoad;
        private TripleThunderOnlineClient Client;
        private bool HasAskedForInfo;

        public LoadingScreen(FightingZone ScreenToLoad, TripleThunderOnlineClient Client)
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
                PushScreen(ScreenToLoad);
            }
            else if (Client != null && Client.IsConnected && Client.Host.IsGameReady)
            {
                if (!HasAskedForInfo)
                {
                    Client.SetGame(ScreenToLoad);
                    Client.Host.Send(new AskGameDataScriptClient());
                    HasAskedForInfo = true;
                }
                else if (ScreenToLoad.HasLoaded)
                {
                    RemoveScreen(this);
                    PushScreen(ScreenToLoad);
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
