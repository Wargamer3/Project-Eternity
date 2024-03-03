using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.TripleThunderScreen.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public abstract class GameRules
    {
        protected readonly FightingZone Map;

        public GameRules(FightingZone Map)
        {
            this.Map = Map;
        }

        public abstract void Init();

        public abstract void Load(ContentManager Content);

        public abstract void Update(GameTime gameTime);

        public abstract void OnKill(RobotAnimation KillerPlayer, RobotAnimation KilledPlayer);

        public abstract void Draw(CustomSpriteBatch g, Player PlayerInfo);

        protected void EndGame()
        {
            if (Map.IsServer)
            {
                Map.GameGroup.SetGame(null);

                foreach (IOnlineConnection ActivePlayer in Map.GameGroup.Room.ListUniqueOnlineConnection)
                {
                    ActivePlayer.IsGameReady = false;
                    ActivePlayer.Send(new GameEndedScriptServer());
                }
            }
            else if (Map.IsOfflineOrServer)
            {
                Map.PushScreen(new GameEndBattleScreen(Map));
            }
        }
    }
}
