﻿using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    class GameOptionsGameRulesScreen : GameScreen
    {
        private readonly RoomInformations Room;

        public GameOptionsGameRulesScreen(RoomInformations Room)
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
            return "Game Rules";
        }
    }
}
