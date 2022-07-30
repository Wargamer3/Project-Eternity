using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    class GameOptionsBotConfigScreen : GameScreen
    {
        private readonly RoomInformations Room;

        public GameOptionsBotConfigScreen(RoomInformations Room)
        {
            this.Room = Room;
        }

        public override void Load()
        {
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }

        public override string ToString()
        {
            return "Bot Config";
        }
    }
}
