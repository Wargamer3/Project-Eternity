using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
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

        public void Init()
        {
            Owner.ListPlayer.Clear();

            Player NewPlayer = new Player("Player", "Human", true, false, 0, Color.Blue);
            Owner.ListPlayer.Add(NewPlayer);

            if (Owner.DicSpawnSquadByPlayer.Count > 0 && Owner.DicSpawnSquadByPlayer["Player"].Count > 0)
            {
                int SpawnSquadIndex = 0;
                for (int S = 0; S < Owner.ListSingleplayerSpawns.Count; S++)
                {
                    if (Owner.ListSingleplayerSpawns[S].Tag == "P")
                    {
                        for (int U = 0; U < Owner.DicSpawnSquadByPlayer["Player"][SpawnSquadIndex].UnitsInSquad; ++U)
                        {
                            Owner.DicSpawnSquadByPlayer["Player"][SpawnSquadIndex].At(U).ReinitializeMembers(Owner.DicUnitType[Owner.DicSpawnSquadByPlayer["Player"][SpawnSquadIndex].At(U).UnitTypeName]);
                        }
                        Owner.DicSpawnSquadByPlayer["Player"][SpawnSquadIndex].ReloadSkills(Owner.DicUnitType, Owner.DicRequirement, Owner.DicEffect, Owner.DicAutomaticSkillTarget, Owner.DicManualSkillTarget);
                        Owner.SpawnSquad(0, Owner.DicSpawnSquadByPlayer["Player"][SpawnSquadIndex], 0, Owner.ListSingleplayerSpawns[S].Position);
                        ++SpawnSquadIndex;
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            Owner.ListActionMenuChoice.Last().Update(gameTime);
        }

        public void OnSquadDefeated(int DefeatedSquadPlayerIndex, Squad DefeatedSquad)
        {
            bool HumanPlayersLost = true;

            foreach (Player ActivePlayer in Owner.ListPlayer)
            {
                if (ActivePlayer.IsPlayerControlled && ActivePlayer.IsAlive)
                {
                    ActivePlayer.IsAlive = false;

                    foreach (Squad ActiveSquad in ActivePlayer.ListSquad)
                    {
                        if (!ActiveSquad.IsDead)
                        {
                            HumanPlayersLost = false;
                            ActivePlayer.IsAlive = true;
                            break;
                        }
                    }
                }
            }

            for (int i = 0; HumanPlayersLost && i < Owner.ListSubMap.Count; i++)
            {
                DeathmatchMap ActiveMap = (DeathmatchMap)Owner.ListSubMap[i];

                foreach (Player ActivePlayer in ActiveMap.ListPlayer)
                {
                    if (ActivePlayer.IsPlayerControlled && ActivePlayer.IsAlive)
                    {
                        ActivePlayer.IsAlive = false;

                        foreach (Squad ActiveSquad in ActivePlayer.ListSquad)
                        {
                            if (!ActiveSquad.IsDead)
                            {
                                HumanPlayersLost = false;
                                ActivePlayer.IsAlive = true;
                                break;
                            }
                        }
                    }
                }
            }

            if (HumanPlayersLost)
            {
                Owner.PushScreen(new GameOverMenu());
            }
        }

        public void OnManualVictory()
        {
            BattleMap.ClearedStages++;
            GameScreen.FMODSystem.sndActiveBGM.Stop();
            NewIntermissionScreen NewIntermissionScreen = new NewIntermissionScreen(Owner.PlayerRoster);
            Owner.RemoveAllScreens();
            Owner.PushScreen(NewIntermissionScreen);
        }

        public void OnManualDefeat()
        {
        }

        public void BeginDraw(CustomSpriteBatch g)
        {

        }

        public void Draw(CustomSpriteBatch g)
        {

        }
    }
}
