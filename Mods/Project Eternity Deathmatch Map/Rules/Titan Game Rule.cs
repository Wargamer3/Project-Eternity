using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class TitanGameRule : LobbyMPGameRule
    {
        private readonly DeathmatchMap Owner;

        public TitanGameRule(DeathmatchMap Owner)
            : base(Owner)
        {
            this.Owner = Owner;
        }

        public override void Init()
        {
            base.Init();
        }

        public void GetPoint(int ActiveTeam)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
            base.Draw(g);
        }
    }
}
