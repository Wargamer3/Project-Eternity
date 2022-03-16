using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    class SinglePlayerGameRule : IGameRule
    {
        private readonly DeathmatchMap Owner;

        public SinglePlayerGameRule(DeathmatchMap Owner)
        {
            this.Owner = Owner;
        }

        public void Init()
        {
            int PlayerIndex = 0;
            foreach (Player ActivePlayer in Owner.ListPlayer)
            {
                if (ActivePlayer.Inventory == null)
                    continue;

                ActivePlayer.ListCommander.AddRange(ActivePlayer.Inventory.ActiveLoadout.ListSpawnCommander);

                string PlayerTag = (ActivePlayer.Team + 1).ToString();
                int SpawnSquadIndex = 0;
                for (int L = 0; L < Owner.LayerManager.ListLayer.Count; L++)
                {
                    BaseMapLayer ActiveLayer = Owner.LayerManager[L];
                    for (int S = 0; S < ActiveLayer.ListSingleplayerSpawns.Count; S++)
                    {
                        if (ActiveLayer.ListSingleplayerSpawns[S].Tag == PlayerTag)
                        {
                            Squad NewSquad = ActivePlayer.Inventory.ActiveLoadout.ListSpawnSquad[SpawnSquadIndex];
                            if (NewSquad == null)
                            {
                                ++SpawnSquadIndex;
                                continue;
                            }

                            for (int U = 0; U < NewSquad.UnitsInSquad; ++U)
                            {
                                NewSquad.At(U).ReinitializeMembers(Owner.DicUnitType[NewSquad.At(U).UnitTypeName]);
                            }

                            NewSquad.ReloadSkills(Owner.DicUnitType, Owner.DicRequirement, Owner.DicEffect, Owner.DicAutomaticSkillTarget, Owner.DicManualSkillTarget);
                            Owner.SpawnSquad(PlayerIndex, NewSquad, 0, ActiveLayer.ListSingleplayerSpawns[S].Position, L);
                            ++SpawnSquadIndex;

                            if (SpawnSquadIndex >= ActivePlayer.Inventory.ActiveLoadout.ListSpawnSquad.Count)
                            {
                                break;
                            }
                        }
                    }

                    if (SpawnSquadIndex >= ActivePlayer.Inventory.ActiveLoadout.ListSpawnSquad.Count)
                    {
                        break;
                    }
                }

                ++PlayerIndex;

                if (Owner.ListPlayer[0].ListSquad.Count > 0)
                {
                    Owner.CursorPosition = Owner.ListPlayer[0].ListSquad[0].Position;
                }
            }
        }

        public void OnNewTurn(int ActivePlayerIndex)
        {
        }

        public void OnSquadDefeated(int AttackerSquadPlayerIndex, Squad AttackerSquad, int DefeatedSquadPlayerIndex, Squad DefeatedSquad)
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

        public void Update(GameTime gameTime)
        {
            if (!Owner.IsEditor)
            {
                Owner.ListActionMenuChoice.Last().Update(gameTime);
            }
        }

        public void BeginDraw(CustomSpriteBatch g)
        {

        }

        public void Draw(CustomSpriteBatch g)
        {

        }
    }
}
