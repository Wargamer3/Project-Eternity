using System;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    class DeathmatchGameRule : LobbyMPGameRule
    {
        private readonly DeathmatchMap Owner;

        public DeathmatchGameRule(DeathmatchMap Owner)
            : base(Owner)
        {
            this.Owner = Owner;
        }
    }
}
