using System;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    class DefaultGameRule : IGameRule
    {
        private readonly DeathmatchMap Owner;

        public DefaultGameRule(DeathmatchMap Owner)
        {
            this.Owner = Owner;
        }

        public void OnSquadDefeated(int DefeatedSquadPlayerIndex, Squad DefeatedSquad, Unit UnitDefeated)
        {
            bool HumanPlayersLost = true;

            foreach (Player ActivePlayer in Owner.ListPlayer)
            {
                if (ActivePlayer.IsPlayerControlled && ActivePlayer.IsAlive)
                {
                    HumanPlayersLost = false;
                    break;
                }

                ActivePlayer.IsAlive = false;
            }

            for (int i = 0; HumanPlayersLost && i < Owner.ListSubMap.Count; i++)
            {
                DeathmatchMap ActiveMap = (DeathmatchMap)Owner.ListSubMap[i];

                foreach (Player ActivePlayer in ActiveMap.ListPlayer)
                {
                    if (ActivePlayer.IsPlayerControlled && ActivePlayer.IsAlive)
                    {
                        HumanPlayersLost = false;
                        break;
                    }

                    ActivePlayer.IsAlive = false;
                }
            }

            if (HumanPlayersLost)
            {
                Owner.PushScreen(new GameOverMenu());
            }
        }
    }
}
