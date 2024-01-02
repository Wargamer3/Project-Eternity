using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public abstract class LobbyMPGameRule : IGameRule
    {
        private struct DeathInfo
        {
            public Squad DeadSquad;
            public int PlayerIndex;
            public int TurnRemaining;

            public DeathInfo(Squad DeadSquad, int PlayerIndex, int TurnRemaining)
            {
                this.DeadSquad = DeadSquad;
                this.PlayerIndex = PlayerIndex;
                this.TurnRemaining = TurnRemaining;
            }
        }

        private readonly DeathmatchMap Owner;

        protected int HPRegenPerTurnFixed;
        protected int ENRegenPerTurnFixed;
        protected int SPRegenPerTurnFixed;
        protected int AmmoRegenPerTurnFixed;
        protected float HPRegenPerTurnPercent;
        protected float ENRegenPerTurnPercent;
        protected float SPRegenPerTurnPercent;
        protected float AmmoRegenPerTurnPercent;

        protected bool UseTeamsForSpawns;
        protected bool ShowRoomSummary;
        public const int TurnsToRespawn = 1;
        private readonly List<DeathInfo> ListDeadSquadInfo;
        protected int MaxKill = 15;
        protected int MaxGameLengthInMinutes = 10;
        protected double GameLengthInSeconds;
        protected List<int> ListRemainingResapwn;
        protected Player CurrentPlayer { get { return Owner.ListPlayer[Owner.ActivePlayerIndex]; } }

        public abstract string Name { get; }

        public LobbyMPGameRule(DeathmatchMap Owner)
        {
            this.Owner = Owner;

            HPRegenPerTurnFixed = 0;
            ENRegenPerTurnFixed = 5;
            SPRegenPerTurnFixed = 10;
            AmmoRegenPerTurnFixed = 0;

            HPRegenPerTurnPercent = 0;
            ENRegenPerTurnPercent = 0;
            SPRegenPerTurnPercent = 0;
            AmmoRegenPerTurnPercent = 0;

            UseTeamsForSpawns = true;
            GameLengthInSeconds = 0;
            ListDeadSquadInfo = new List<DeathInfo>();
            ListRemainingResapwn = new List<int>();
        }

        public virtual void Init()
        {
            GameLengthInSeconds = MaxGameLengthInMinutes * 60;

            if (Owner.IsOfflineOrServer)
            {
                int PlayerIndex = 0;
                Dictionary<int, int> DicSpawnSquadIndexPerTeam = new Dictionary<int, int>();
                for (int P = 0; P < Owner.ListPlayer.Count; P++)
                {
                    Player ActivePlayer = Owner.ListPlayer[P];
                    if (ActivePlayer.Inventory == null)
                        continue;

                    int SpawnTeam = ActivePlayer.Team;
                    if (!UseTeamsForSpawns)
                    {
                        SpawnTeam = P;
                    }

                    if (ActivePlayer.Team >= 0 && SpawnTeam < Owner.ListMultiplayerColor.Count)
                    {
                        ActivePlayer.Color = Owner.ListMultiplayerColor[SpawnTeam];
                    }

                    if (!DicSpawnSquadIndexPerTeam.ContainsKey(SpawnTeam))
                    {
                        DicSpawnSquadIndexPerTeam.Add(SpawnTeam, 0);
                    }

                    List<MovementAlgorithmTile> ListPossibleSpawnPoint = Owner.GetSpawnLocations(SpawnTeam);

                    int SpawnSquadIndex = DicSpawnSquadIndexPerTeam[SpawnTeam];

                    foreach (MovementAlgorithmTile ActiveSpawn in ListPossibleSpawnPoint)
                    {
                        if (SpawnSquadIndex >= ActivePlayer.Inventory.ActiveLoadout.ListSpawnSquad.Count)
                        {
                            break;
                        }

                        Squad NewSquad = ActivePlayer.Inventory.ActiveLoadout.ListSpawnSquad[SpawnSquadIndex];
                        if (NewSquad == null)
                        {
                            ++SpawnSquadIndex;
                            DicSpawnSquadIndexPerTeam[SpawnTeam] = SpawnSquadIndex;
                            continue;
                        }

                        for (int U = 0; U < NewSquad.UnitsInSquad; ++U)
                        {
                            NewSquad.At(U).ReinitializeMembers(Owner.Params.DicUnitType[NewSquad.At(U).UnitTypeName]);
                        }

                        NewSquad.ReloadSkills(Owner.Params.DicUnitType, Owner.Params.DicRequirement, Owner.Params.DicEffect, Owner.Params.DicAutomaticSkillTarget, Owner.Params.DicManualSkillTarget);
                        Owner.SpawnSquad(PlayerIndex, NewSquad, 0, new Vector2(ActiveSpawn.InternalPosition.X, ActiveSpawn.InternalPosition.Y), ActiveSpawn.LayerIndex);
                        NewSquad.CurrentLeader.PilotSP = 0;
                        NewSquad.CurrentLeader.ConsumeEN(NewSquad.CurrentLeader.MaxEN);
                        ++SpawnSquadIndex;
                        DicSpawnSquadIndexPerTeam[SpawnTeam] = SpawnSquadIndex;

                        if (!ActivePlayer.IsPlayerControlled || !NewSquad.IsPlayerControlled)
                        {
                            InitBot(NewSquad);
                        }

                        if (Owner != ActiveSpawn.Owner)
                        {
                            ActiveSpawn.Owner.AddUnit(P, NewSquad, ActiveSpawn);
                            Owner.RemoveUnit(P, NewSquad);
                            Owner.SelectPlatform(Owner.GetPlatform(ActiveSpawn.Owner));
                            ActiveSpawn.Owner.CursorPosition = NewSquad.Position;
                            ActiveSpawn.Owner.CursorPositionVisible = NewSquad.Position;
                        }

                        Owner.CursorPosition = NewSquad.Position;
                        Owner.CursorPositionVisible = NewSquad.Position;

                        if (SpawnSquadIndex >= ActivePlayer.Inventory.ActiveLoadout.ListSpawnSquad.Count)
                        {
                            break;
                        }

                        if (SpawnSquadIndex >= ActivePlayer.Inventory.ActiveLoadout.ListSpawnSquad.Count)
                        {
                            break;
                        }
                    }

                    ++PlayerIndex;
                }
            }
        }

        protected virtual void InitBot(Squad NewSquad)
        {
            NewSquad.SquadAI = new DeathmatchScripAIContainer(new DeathmatchAIInfo(Owner, NewSquad));
            NewSquad.SquadAI.Load("Default AI");
        }

        public void OnNewTurn(int ActivePlayerIndex)
        {
            if (Owner.ListPlayer[ActivePlayerIndex].ListSquad.Count > 0)
            {
                Owner.CursorPosition = Owner.ListPlayer[ActivePlayerIndex].ListSquad[0].Position;
                Owner.CursorPositionVisible = Owner.CursorPosition;
                foreach (Squad ActiveSquad in Owner.ListPlayer[ActivePlayerIndex].ListSquad)
                {
                    if (!ActiveSquad.IsDead)
                    {
                        for (int U = ActiveSquad.UnitsAliveInSquad - 1; U >= 0; --U)
                        {
                            ActiveSquad[U].HealUnit((int)(HPRegenPerTurnFixed + ActiveSquad[U].MaxHP * HPRegenPerTurnPercent * 0.01f));
                            ActiveSquad[U].RefillEN((int)(ENRegenPerTurnFixed + ActiveSquad[U].MaxEN * ENRegenPerTurnPercent * 0.01f));
                            ActiveSquad[U].RefillSP((int)(SPRegenPerTurnFixed + ActiveSquad[U].Pilot.MaxSP * SPRegenPerTurnPercent * 0.01f));
                            ActiveSquad[U].RefillAmmo((byte)AmmoRegenPerTurnFixed, AmmoRegenPerTurnPercent);
                        }
                    }
                }
            }

            for (int S = ListDeadSquadInfo.Count - 1; S >= 0; S--)
            {
                DeathInfo RespawningSquad = ListDeadSquadInfo[S];

                if (RespawningSquad.PlayerIndex == ActivePlayerIndex)
                {
                    if (--RespawningSquad.TurnRemaining <= 0)
                    {
                        RespawnSquad(ActivePlayerIndex, RespawningSquad.DeadSquad);
                        ListDeadSquadInfo.RemoveAt(S);
                    }
                }
            }
        }

        public virtual void OnSquadDefeated(int AttackerSquadPlayerIndex, Squad AttackerSquad, int DefeatedSquadPlayerIndex, Squad DefeatedSquad)
        {
            if (AttackerSquadPlayerIndex != DefeatedSquadPlayerIndex)//Don't count suicides as kills
            {
                ++Owner.ListAllPlayer[AttackerSquadPlayerIndex].Kills;
            }

            ++Owner.ListAllPlayer[DefeatedSquadPlayerIndex].Death;

            AttackerSquad.At(0).RefillSP(5);
            AttackerSquad.At(0).RefillEN(5);
            AttackerSquad.At(0).RemoveEffects();

            if (AttackerSquad == null)
            {
                return;
            }

            if (DefeatedSquad.ItemHeld != null)
            {
                Owner.LayerManager.ListLayer[(int)DefeatedSquad.Position.Z].ListHoldableItem.Add(DefeatedSquad.ItemHeld);
                DefeatedSquad.ItemHeld.Position = DefeatedSquad.Position;
                DefeatedSquad.DropItem();
            }

            if (AttackerSquadPlayerIndex >= ListRemainingResapwn.Count || ListRemainingResapwn[DefeatedSquadPlayerIndex] > 0)
            {
                ListDeadSquadInfo.Add(new DeathInfo(DefeatedSquad, DefeatedSquadPlayerIndex, TurnsToRespawn));

                if (DefeatedSquadPlayerIndex < ListRemainingResapwn.Count)
                {
                    ListRemainingResapwn[DefeatedSquadPlayerIndex] -= DefeatedSquad.At(0).SpawnCost;
                    if (ListRemainingResapwn[DefeatedSquadPlayerIndex] < 0)
                    {
                        ListRemainingResapwn[DefeatedSquadPlayerIndex] = 0;
                    }
                }
            }

            if ((ListRemainingResapwn.Count <= 0 || ListRemainingResapwn[DefeatedSquadPlayerIndex] < 0) && Owner.IsOfflineOrServer)
            {
                CheckGameOver();
            }

            TemporaryAttackPickup DroppedWeapon = DefeatedSquad.At(0).OnDeath();
            if (DroppedWeapon != null)
            {
                DroppedWeapon.Position = AttackerSquad.Position;
                Owner.LayerManager.ListLayer[(int)DefeatedSquad.Z].ListAttackPickup.Add(DroppedWeapon);
            }
        }

        public void OnManualVictory(int EXP, uint Money)
        {
            List<LobbyVictoryScreen.PlayerGains> ListGains = new List<LobbyVictoryScreen.PlayerGains>();
            foreach (Player ActivePlayer in Owner.ListLocalPlayer)
            {
                LobbyVictoryScreen.PlayerGains NewGains = new LobbyVictoryScreen.PlayerGains();
                NewGains.EXP = EXP;
                NewGains.Money = Money;

                ListGains.Add(NewGains);
            }

            LobbyVictoryScreen NewLobbyVictoryScreen = new LobbyVictoryScreen(Owner, ListGains);
            Owner.PushScreen(NewLobbyVictoryScreen);
        }

        public void OnManualDefeat(int EXP, uint Money)
        {
            List<LobbyVictoryScreen.PlayerGains> ListGains = new List<LobbyVictoryScreen.PlayerGains>();
            foreach (Player ActivePlayer in Owner.ListLocalPlayer)
            {
                LobbyVictoryScreen.PlayerGains NewGains = new LobbyVictoryScreen.PlayerGains();
                NewGains.EXP = EXP;
                NewGains.Money = Money;

                ListGains.Add(NewGains);
            }

            LobbyDefeatScreen NewLobbyVictoryScreen = new LobbyDefeatScreen(Owner, ListGains);
            Owner.PushScreen(NewLobbyVictoryScreen);
        }

        public void CheckGameOver()
        {
            HashSet<int> ListAliveTeam = new HashSet<int>();

            for (int P = 0; P < Owner.ListAllPlayer.Count && P < 10; P++)
            {
                Player ActivePlayer = Owner.ListAllPlayer[P];

                foreach (Squad ActiveSquad in ActivePlayer.ListSquad)
                {
                    if (!ActiveSquad.IsDead)
                    {
                        ListAliveTeam.Add(ActivePlayer.Team);
                        break;
                    }
                }
            }

            if (ListAliveTeam.Count <= 1)
            {
                List<LobbyVictoryScreen.PlayerGains> ListGains = new List<LobbyVictoryScreen.PlayerGains>();
                foreach (Player ActivePlayer in Owner.ListAllPlayer)
                {
                    if (ActivePlayer.Team == -1)
                    {
                        continue;
                    }

                    LobbyVictoryScreen.PlayerGains NewGains = new LobbyVictoryScreen.PlayerGains();
                    foreach (Player OtherPlayer in Owner.ListAllPlayer)
                    {
                        if (ActivePlayer.Team == OtherPlayer.Team || OtherPlayer.Team == -1)
                        {
                            continue;
                        }

                        if (ListAliveTeam.Contains(ActivePlayer.Team))
                        {
                            NewGains.EXP += GetVictoryEXP(ActivePlayer, OtherPlayer);
                            NewGains.Money += GetVictoryMoney(ActivePlayer);
                        }
                        else
                        {
                            NewGains.EXP += GetDefeatEXP(ActivePlayer, OtherPlayer);
                            NewGains.Money += GetDefeatMoney(ActivePlayer);
                        }
                    }

                    ActivePlayer.EXP += NewGains.EXP;
                    ActivePlayer.Records.CurrentMoney += NewGains.Money;

                    if (ActivePlayer.IsPlayerControlled)
                    {
                        if (Owner.IsServer)
                        {
                            Owner.OnlineServer.Database.SavePlayerInventory(ActivePlayer.ConnectionID, ActivePlayer);
                        }
                        else
                        {
                            ActivePlayer.SaveLocally();
                        }
                    }

                    ListGains.Add(NewGains);
                }

                if (Owner.IsServer)
                {
                }
                else
                {
                    if (ListAliveTeam.Count == 0)
                    {
                        LobbyDrawScreen NewLobbyVictoryScreen = new LobbyDrawScreen(Owner, ListGains);
                        Owner.PushScreen(NewLobbyVictoryScreen);
                    }
                    else if (ListAliveTeam.Contains(PlayerManager.ListLocalPlayer[0].Team))
                    {
                        LobbyVictoryScreen NewLobbyVictoryScreen = new LobbyVictoryScreen(Owner, ListGains);
                        Owner.PushScreen(NewLobbyVictoryScreen);
                    }
                    else
                    {
                        LobbyDefeatScreen NewLobbyVictoryScreen = new LobbyDefeatScreen(Owner, ListGains);
                        Owner.PushScreen(NewLobbyVictoryScreen);
                    }
                }
            }
        }

        private int GetVictoryEXP(Player ActivePlayer, Player OtherPlayer)
        {
            return (int)Math.Ceiling(ActivePlayer.Level / 10d) * 10 + (ActivePlayer.Level - OtherPlayer.Level);
        }

        private int GetDefeatEXP(Player ActivePlayer, Player OtherPlayer)
        {
            if (OtherPlayer.Level > ActivePlayer.Level)
            {
                return (int)Math.Ceiling(ActivePlayer.Level / 10d) * 3 + (int)Math.Ceiling((ActivePlayer.Level - OtherPlayer.Level + 1) / 10d);
            }
            else
            {
                return Math.Max(0, (int)Math.Ceiling(ActivePlayer.Level / 10d) * 3 - ActivePlayer.Level - OtherPlayer.Level + 1);
            }
        }

        private uint GetVictoryMoney(Player ActivePlayer)
        {
            return 100u + (uint)(ActivePlayer.Level - 1) * 10u;
        }

        private uint GetDefeatMoney(Player ActivePlayer)
        {
            return 50u + (uint)ActivePlayer.Level * 5u;
        }

        private void RespawnSquad(int ActivePlayerIndex, Squad ActiveSquad)
        {
            List<MovementAlgorithmTile> ListPossibleSpawnPoint = Owner.GetSpawnLocations(Owner.ListPlayer[ActivePlayerIndex].Team);
            if (ListPossibleSpawnPoint.Count == 0)
            {
                return;
            }

            for (int U = 0; U < ActiveSquad.UnitsInSquad; ++U)
            {
                ActiveSquad.At(U).ReinitializeMembers(Owner.Params.DicUnitType[ActiveSquad.At(U).UnitTypeName]);
            }

            ActiveSquad.ReloadSkills(Owner.Params.DicUnitType, Owner.Params.DicRequirement, Owner.Params.DicEffect, Owner.Params.DicAutomaticSkillTarget, Owner.Params.DicManualSkillTarget);

            int RandomSapwnPointIndex = RandomHelper.Next(ListPossibleSpawnPoint.Count);
            MovementAlgorithmTile RandomSpawnPoint = ListPossibleSpawnPoint[RandomSapwnPointIndex];

            Owner.CursorPosition = new Vector3(RandomSpawnPoint.InternalPosition.X, RandomSpawnPoint.InternalPosition.Y, RandomSpawnPoint.LayerIndex);
            Owner.CursorPositionVisible = Owner.CursorPosition;

            ActiveSquad.At(0).HealUnit(ActiveSquad.At(0).MaxHP);
            ActiveSquad.At(0).EmptySP();

            ActiveSquad.UpdateSquad();

            Owner.ActivateAutomaticSkills(ActiveSquad, string.Empty);
            ActiveSquad.SetPosition(Owner.CursorPosition);

            ActiveSquad.CurrentTerrainIndex = Owner.GetTerrain(ActiveSquad).TerrainTypeIndex;
        }

        public virtual void Update(GameTime gameTime)
        {
            ShowRoomSummary = false;
            if (KeyboardHelper.KeyHold(Keys.Tab))
            {
                ShowRoomSummary = true;
            }

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
            GameScreen.DrawBox(g, new Vector2(3, 3), 200, 24, Color.FromNonPremultiplied(0, 0, 0, 40));
            g.DrawString(Owner.fntArial8, "DM", new Vector2(28, 7), Color.White);//Position
            g.DrawString(Owner.fntArial8, "1/1", new Vector2(95, 7), Color.White);//Position
            g.DrawString(Owner.fntArial8, "K:" + CurrentPlayer.Kills, new Vector2(134, 9), Color.White);
            g.DrawString(Owner.fntArial8, "D:" + CurrentPlayer.Death, new Vector2(164, 9), Color.White);

            GameScreen.DrawBox(g, new Vector2(Constants.Width / 2 - 100, 3), 200, 28, Color.FromNonPremultiplied(0, 0, 0, 40));
            GameScreen.DrawBox(g, new Vector2(Constants.Width / 2 - 40, 5), 80, 24, Color.FromNonPremultiplied(0, 0, 0, 90));
            int TimeRemaining = (int)GameLengthInSeconds;
            int MinutesRemaining = TimeRemaining / 60;
            int SecondsRemaining = TimeRemaining % 60;
            g.DrawStringRightAligned(Owner.fntArial8, MinutesRemaining.ToString().PadLeft(2, '0'), new Vector2(Constants.Width / 2 - 5, 12), Color.White);
            g.DrawString(Owner.fntArial8, ":", new Vector2(Constants.Width / 2, 12), Color.White);
            g.DrawString(Owner.fntArial8, SecondsRemaining.ToString().PadLeft(2, '0'), new Vector2(Constants.Width / 2 + 5, 12), Color.White);

            if (ShowRoomSummary)
            {
                ShowSummary(g);
            }
        }

        public void ShowSummary(CustomSpriteBatch g)
        {
            int Max = 0;
            for (int P = 0; P < Owner.ListAllPlayer.Count; ++P)
            {
                if (Owner.ListAllPlayer[P].Team == -1)
                {
                    continue;
                }

                ++Max;
            }

            GameScreen.DrawBox(g, new Vector2(500, 150), 250, Max * 26 + 65, Color.FromNonPremultiplied(0, 0, 0, 40));
            g.DrawString(Owner.fntArial8, "Kill", new Vector2(650, 150 + 6), Color.White);
            g.DrawString(Owner.fntArial8, "Death", new Vector2(690, 150 + 6), Color.White);

            float PosY = 175;
            for (int P = 0; P < Max; ++P)
            {
                if (Owner.ListAllPlayer[P].Team == -1)
                {
                    continue;
                }

                GameScreen.DrawBox(g, new Vector2(505, PosY), 20, 25, Color.FromNonPremultiplied(255, 0, 0, 200));
                GameScreen.DrawBox(g, new Vector2(505 + 20, PosY), 100, 25, Color.FromNonPremultiplied(255, 0, 0, 200));

                GameScreen.DrawBox(g, new Vector2(630, PosY), 50, 25, Color.FromNonPremultiplied(255, 0, 0, 200));
                GameScreen.DrawBox(g, new Vector2(680, PosY), 50, 25, Color.FromNonPremultiplied(255, 0, 0, 200));

                g.DrawString(Owner.fntArial8, Owner.ListAllPlayer[P].Name, new Vector2(540, PosY + 6), Color.White);
                g.DrawString(Owner.fntArial8, (P + 1).ToString(), new Vector2(515, PosY + 5), Color.White);
                g.DrawString(Owner.fntArial8, Owner.ListAllPlayer[P].Kills.ToString(), new Vector2(665, PosY + 5), Color.White);
                g.DrawString(Owner.fntArial8, Owner.ListAllPlayer[P].Death.ToString(), new Vector2(720, PosY + 5), Color.White);
                PosY += 26;
            }

            PosY -= 10;
            g.DrawString(Owner.fntArial8, "To Win", new Vector2(520, PosY + Max * 26), Color.White);
            g.DrawString(Owner.fntArial8, MaxGameLengthInMinutes + " MIN", new Vector2(620, PosY + Max * 26), Color.White);
            g.DrawString(Owner.fntArial8, MaxKill + " KILL", new Vector2(670, PosY + Max * 26), Color.White);
        }

        public abstract List<GameRuleError> Validate(RoomInformations Room);
    }
}
