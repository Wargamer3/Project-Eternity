using System;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    class CampaignGameRule : LobbyMPGameRule
    {
        private readonly DeathmatchMap Owner;

        public CampaignGameRule(DeathmatchMap Owner)
            : base(Owner)
        {
            this.Owner = Owner;
            UseTeamsForSpawns = false;
        }
    }
}
