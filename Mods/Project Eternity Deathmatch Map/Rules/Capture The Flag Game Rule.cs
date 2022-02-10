using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class CaptureTheFlagGameRule : LobbyMPGameRule
    {
        private readonly DeathmatchMap Owner;
        public Dictionary<int, int> DicPointsByTeam;

        public CaptureTheFlagGameRule(DeathmatchMap Owner)
            : base(Owner)
        {
            this.Owner = Owner;

            DicPointsByTeam = new Dictionary<int, int>();
        }

        public override void Init()
        {
            base.Init();

            DicPointsByTeam.Add(0, 0);
            DicPointsByTeam.Add(1, 0);

            foreach (Player ActivePlayer in Owner.ListPlayer)
            {
                if (!DicPointsByTeam.ContainsKey(ActivePlayer.Team))
                {
                    DicPointsByTeam.Add(ActivePlayer.Team, 0);
                }
            }
        }

        public void GetPoint(int ActiveTeam)
        {
            ++DicPointsByTeam[ActiveTeam];
        }

        public override void Draw(CustomSpriteBatch g)
        {
            base.Draw(g);

            g.DrawString(Owner.fntArial8, DicPointsByTeam[0].ToString(), new Vector2(Constants.Width / 2 - 50, 12), Color.White);
            g.DrawString(Owner.fntArial8, DicPointsByTeam[1].ToString(), new Vector2(Constants.Width / 2 + 50, 12), Color.White);
        }
    }
}
