using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    class SinglePlayerGameRule : IGameRule
    {
        private readonly DeathmatchMap Owner;
        int HPRegenPerTurnFixed;
        int ENRegenPerTurnFixed;
        int SPRegenPerTurnFixed;
        int AmmoRegenPerTurnFixed;
        float HPRegenPerTurnPercent;
        float ENRegenPerTurnPercent;
        float SPRegenPerTurnPercent;
        float AmmoRegenPerTurnPercent;

        public SinglePlayerGameRule(DeathmatchMap Owner)
        {
            this.Owner = Owner;
        }

        public void Init()
        {
            IniFile SinglePlayerParams = IniFile.ReadFromFile("Content/Single Player Params.ini");
            HPRegenPerTurnFixed = int.Parse(SinglePlayerParams.ReadField("GameRule", "HPRegenPerTurnFixed"));
            ENRegenPerTurnFixed = int.Parse(SinglePlayerParams.ReadField("GameRule", "ENRegenPerTurnFixed"));
            SPRegenPerTurnFixed = int.Parse(SinglePlayerParams.ReadField("GameRule", "SPRegenPerTurnFixed"));
            AmmoRegenPerTurnFixed = int.Parse(SinglePlayerParams.ReadField("GameRule", "AmmoRegenPerTurnFixed"));

            HPRegenPerTurnPercent = int.Parse(SinglePlayerParams.ReadField("GameRule", "HPRegenPerTurnPercent"));
            ENRegenPerTurnPercent = int.Parse(SinglePlayerParams.ReadField("GameRule", "ENRegenPerTurnPercent"));
            SPRegenPerTurnPercent = int.Parse(SinglePlayerParams.ReadField("GameRule", "SPRegenPerTurnPercent"));
            AmmoRegenPerTurnPercent = int.Parse(SinglePlayerParams.ReadField("GameRule", "AmmoRegenPerTurnPercent"));

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
                            Owner.SpawnSquad(PlayerIndex, NewSquad, 0, new Vector2(ActiveLayer.ListSingleplayerSpawns[S].Position.X, ActiveLayer.ListSingleplayerSpawns[S].Position.Y), L);
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
            for (int S = 0; S < Owner.ListPlayer[Owner.ActivePlayerIndex].ListSquad.Count; S++)
            {
                Squad ActiveSquad = Owner.ListPlayer[Owner.ActivePlayerIndex].ListSquad[S];

                for (int U = ActiveSquad.UnitsAliveInSquad - 1; U >= 0; --U)
                {
                    ActiveSquad[U].HealUnit((int)(HPRegenPerTurnFixed + ActiveSquad[U].MaxHP * HPRegenPerTurnPercent * 0.01f));
                    ActiveSquad[U].RefillEN((int)(ENRegenPerTurnFixed + ActiveSquad[U].MaxEN * ENRegenPerTurnPercent * 0.01f));
                    ActiveSquad[U].RefillSP((int)(SPRegenPerTurnFixed + ActiveSquad[U].Pilot.MaxSP * SPRegenPerTurnPercent * 0.01f));
                    ActiveSquad[U].RefillAmmo((byte)AmmoRegenPerTurnFixed, AmmoRegenPerTurnPercent);
                }
            }
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
