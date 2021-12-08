using System;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    class HordeGameRule : LobbyMPGameRule
    {
        private readonly DeathmatchMap Owner;

        public HordeGameRule(DeathmatchMap Owner)
            : base(Owner)
        {
            this.Owner = Owner;
        }

        public override void OnSquadDefeated(int DefeatedSquadPlayerIndex, Squad DefeatedSquad)
        {
            bool HordePlayerAILost = true;

            for (int i = 0; HordePlayerAILost && i < Owner.ListSubMap.Count; i++)
            {
                DeathmatchMap ActiveMap = (DeathmatchMap)Owner.ListSubMap[i];

                foreach (Squad ActiveSquad in ActiveMap.ListPlayer[10].ListSquad)
                {
                    if (!ActiveSquad.IsDead)
                    {
                        HordePlayerAILost = false;
                        break;
                    }
                }
            }

            if (HordePlayerAILost)
            {
                ActionPanelPhaseChange.FinishAIPlayerTurn(Owner);
            }
        }
    }
}
