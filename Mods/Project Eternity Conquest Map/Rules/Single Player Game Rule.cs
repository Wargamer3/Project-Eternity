using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    class SinglePlayerGameRule : IGameRule
    {
        public const string SinglePlayerName = "Single Player";

        private readonly ConquestMap Owner;
        int HPRegenPerTurnFixed;
        int ENRegenPerTurnFixed;
        int SPRegenPerTurnFixed;
        int AmmoRegenPerTurnFixed;
        float HPRegenPerTurnPercent;
        float ENRegenPerTurnPercent;
        float SPRegenPerTurnPercent;
        float AmmoRegenPerTurnPercent;

        public string Name => "Single Player";

        public SinglePlayerGameRule(ConquestMap Owner)
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

            for (int L = 0; L < Owner.LayerManager.ListLayer.Count; L++)
            {
                MapLayer ActiveLayer = Owner.LayerManager.ListLayer[L];

                if (!Owner.IsEditor)
                {
                    for (int S = 0; S < ActiveLayer.ListUnitSpawn.Count; S++)
                    {
                        while  (ActiveLayer.ListUnitSpawn[S].SpawnPlayer >= Owner.ListPlayer.Count)
                        {
                            Player NewPlayer = new Player("Enemy", "CPU", false, false, Owner.ListPlayer.Count, Color.Red);
                            Owner.ListPlayer.Add(NewPlayer);
                        }
                    }
                }
            }
            int PlayerIndex = 0;
            foreach (Player ActivePlayer in Owner.ListPlayer)
            {
                if (ActivePlayer.Inventory == null)
                    continue;

                if (ActivePlayer.TeamIndex >= 0 && ActivePlayer.TeamIndex < Owner.ListMultiplayerColor.Count)
                {
                    ActivePlayer.Color = Owner.ListMultiplayerColor[ActivePlayer.TeamIndex];
                }

                ActivePlayer.ListCommander.AddRange(ActivePlayer.Inventory.ActiveLoadout.ListSpawnCommander);

                string PlayerTag = (ActivePlayer.TeamIndex + 1).ToString();
                int SpawnSquadIndex = 0;
                for (int L = 0; L < Owner.LayerManager.ListLayer.Count; L++)
                {
                    MapLayer ActiveLayer = Owner.LayerManager.ListLayer[L];

                    if (!Owner.IsEditor)
                    {
                        for (int S = 0; S < ActiveLayer.ListUnitSpawn.Count; S++)
                        {
                            if (ActiveLayer.ListUnitSpawn[S].SpawnPlayer == PlayerIndex)
                            {
                                UnitConquest NewUnit = new UnitConquest(ActiveLayer.ListUnitSpawn[S].UnitPath, Owner.Content, Owner.Params.DicRequirement, Owner.Params.DicEffect);

                                NewUnit.ReinitializeMembers(Owner.Params.DicUnitType[NewUnit.UnitTypeName]);

                                NewUnit.ReloadSkills(Owner.Params.DicUnitType[NewUnit.UnitTypeName], Owner.Params.DicRequirement, Owner.Params.DicEffect, Owner.Params.DicAutomaticSkillTarget, Owner.Params.DicManualSkillTarget);
                                Owner.SpawnUnit(PlayerIndex, NewUnit, 0, new Vector3(ActiveLayer.ListUnitSpawn[S].SpawnPositionX * Owner.TileSize.X, ActiveLayer.ListUnitSpawn[S].SpawnPositionY * Owner.TileSize.Y, ActiveLayer.ListUnitSpawn[S].SpawnLayer));
                                ++SpawnSquadIndex;

                                if (!ActivePlayer.IsPlayerControlled || !NewUnit.IsPlayerControlled)
                                {
                                    InitBot(NewUnit);
                                }
                            }
                        }
                    }

                    for (int S = 0; S < ActiveLayer.ListCampaignSpawns.Count; S++)
                    {
                        if (ActiveLayer.ListCampaignSpawns[S].Tag == PlayerTag)
                        {
                            UnitConquest NewUnit = (UnitConquest)ActivePlayer.Inventory.ActiveLoadout.ListSpawnSquad[SpawnSquadIndex].CurrentLeader;
                            if (NewUnit == null)
                            {
                                ++SpawnSquadIndex;
                                continue;
                            }

                            NewUnit.ReinitializeMembers(Owner.Params.DicUnitType[NewUnit.UnitTypeName]);

                            NewUnit.ReloadSkills(Owner.Params.DicUnitType[NewUnit.UnitTypeName], Owner.Params.DicRequirement, Owner.Params.DicEffect, Owner.Params.DicAutomaticSkillTarget, Owner.Params.DicManualSkillTarget);
                            Owner.SpawnUnit(PlayerIndex, NewUnit, 0, ActiveLayer.ListCampaignSpawns[S].Position);
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

                if (Owner.ListPlayer[0].ListUnit.Count > 0)
                {
                    Owner.CursorPosition = Owner.ListPlayer[0].ListUnit[0].Position;
                }
            }


            for (int L = 0; L < Owner.LayerManager.ListLayer.Count; L++)
            {
                MapLayer ActiveLayer = Owner.LayerManager.ListLayer[L];

                for (int B = 0; B < ActiveLayer.ListBuildingSpawn.Count; B++)
                {
                    Vector3 SpawnPosition = new Vector3(ActiveLayer.ListBuildingSpawn[B].SpawnPositionX * Owner.TileSize.X, ActiveLayer.ListBuildingSpawn[B].SpawnPositionY * Owner.TileSize.Y, ActiveLayer.ListBuildingSpawn[B].SpawnLayer);
                    BuildingConquest NewBuilding = new BuildingConquest(ActiveLayer.ListBuildingSpawn[B].BuildingPath, GameScreens.GameScreen.ContentFallback, null, null, null);
                    Owner.SpawnBuilding(PlayerIndex, NewBuilding, 0, SpawnPosition);
                }
            }
        }

        protected virtual void InitBot(UnitConquest NewSquad)
        {
            NewSquad.SquadAI = new ConquestScripAIContainer(new ConquestAIInfo(Owner, NewSquad));
            NewSquad.SquadAI.Load("Default AI");
        }

        public int GetRemainingResapwn(int PlayerIndex)
        {
            throw new NotImplementedException();
        }

        public void OnNewTurn(int ActivePlayerIndex)
        {
            for (int S = 0; S < Owner.ListPlayer[Owner.ActivePlayerIndex].ListUnit.Count; S++)
            {
                UnitConquest ActiveUnit = Owner.ListPlayer[Owner.ActivePlayerIndex].ListUnit[S];

                if (ActiveUnit.HP > 0)
                {
                    ActiveUnit.HealUnit((int)(HPRegenPerTurnFixed + ActiveUnit.MaxHP * HPRegenPerTurnPercent * 0.01f));
                    ActiveUnit.RefillEN((int)(ENRegenPerTurnFixed + ActiveUnit.MaxEN * ENRegenPerTurnPercent * 0.01f));
                    ActiveUnit.RefillAmmo((byte)AmmoRegenPerTurnFixed, AmmoRegenPerTurnPercent);
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

                    foreach (UnitConquest ActiveUnit in ActivePlayer.ListUnit)
                    {
                        if (ActiveUnit.HP > 0)
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
                ConquestMap ActiveMap = (ConquestMap)Owner.ListSubMap[i];

                foreach (Player ActivePlayer in ActiveMap.ListPlayer)
                {
                    if (ActivePlayer.IsPlayerControlled && ActivePlayer.IsAlive)
                    {
                        ActivePlayer.IsAlive = false;

                        foreach (UnitConquest ActiveUnit in ActivePlayer.ListUnit)
                        {
                            if (ActiveUnit.HP > 0)
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

        public void OnManualVictory(int EXP, uint Money)
        {
            BattleMap.ClearedStages++;
            GameScreen.FMODSystem.sndActiveBGM.Stop();
            NewIntermissionScreen NewIntermissionScreen = new NewIntermissionScreen(Owner.ListPlayer[0], Owner.PlayerRoster);
            Owner.RemoveAllScreens();
            Owner.PushScreen(NewIntermissionScreen);
        }

        public void OnManualDefeat(int EXP, uint Money)
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

        public List<GameRuleError> Validate(RoomInformations Room)
        {
            return new List<GameRuleError>();
        }
    }
}
