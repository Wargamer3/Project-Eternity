using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    abstract class LobbyMPGameRule : IGameRule
    {
        private readonly DeathmatchMap Owner;

        public LobbyMPGameRule(DeathmatchMap Owner)
        {
            this.Owner = Owner;
        }

        public virtual void Init()
        {
            if (Owner.IsOfflineOrServer)
            {
                int PlayerIndex = 1;
                foreach (Player ActivePlayer in Owner.ListPlayer)
                {
                    if (ActivePlayer.ListSquadToSpawn == null)
                        continue;

                    string PlayerTag = PlayerIndex.ToString();
                    int SpawnSquadIndex = 0;
                    for (int S = 0; S < Owner.ListMultiplayerSpawns.Count; S++)
                    {
                        if (Owner.ListMultiplayerSpawns[S].Tag == PlayerTag)
                        {
                            for (int U = 0; U < ActivePlayer.ListSquadToSpawn[SpawnSquadIndex].UnitsInSquad; ++U)
                            {
                                ActivePlayer.ListSquadToSpawn[SpawnSquadIndex].At(U).ReinitializeMembers(Owner.DicUnitType[ActivePlayer.ListSquadToSpawn[SpawnSquadIndex].At(U).UnitTypeName]);
                            }
                            ActivePlayer.ListSquadToSpawn[SpawnSquadIndex].ReloadSkills(Owner.DicUnitType, Owner.DicRequirement, Owner.DicEffect, Owner.DicAutomaticSkillTarget, Owner.DicManualSkillTarget);
                            Owner.SpawnSquad(PlayerIndex - 1, ActivePlayer.ListSquadToSpawn[SpawnSquadIndex], 0, Owner.ListMultiplayerSpawns[S].Position);
                            ++SpawnSquadIndex;
                            if (SpawnSquadIndex >= ActivePlayer.ListSquadToSpawn.Count)
                            {
                                break;
                            }
                        }
                    }

                    ++PlayerIndex;
                }
            }
        }

        public virtual void OnSquadDefeated(int DefeatedSquadPlayerIndex, Squad DefeatedSquad)
        {

        }

        public virtual void Update(GameTime gameTime)
        {
            if (!Owner.ListPlayer[Owner.ActivePlayerIndex].IsOnline)
            {
                Owner.ListActionMenuChoice.Last().Update(gameTime);
            }
            else if (Owner.ListPlayer[Owner.ActivePlayerIndex].IsOnline)
            {
                Owner.ListActionMenuChoice.Last().UpdatePassive(gameTime);
            }
        }

        public virtual void BeginDraw(CustomSpriteBatch g)
        {
        }

        public virtual void Draw(CustomSpriteBatch g)
        {
        }
    }
}
