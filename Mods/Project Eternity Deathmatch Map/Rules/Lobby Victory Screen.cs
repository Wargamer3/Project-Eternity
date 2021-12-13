using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Parts;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class LobbyVictoryScreen : GameScreen
    {
        public class PlayerGains
        {
            public int Exp;
            public int Money;
            public List<UnitPart> ListEquipment;
            public List<object> ListConsumable;

            public PlayerGains()
            {
                Exp = 0;
                Money = 0;
                ListEquipment = new List<UnitPart>();
                ListConsumable = new List<object>();
            }
        }

        private List<PlayerGains> ListPlayerGains;

        public LobbyVictoryScreen(List<PlayerGains> ListPlayerGains)
        {
            this.ListPlayerGains = ListPlayerGains;
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
    }
}
