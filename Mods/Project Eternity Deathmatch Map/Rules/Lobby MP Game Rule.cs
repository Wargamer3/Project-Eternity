using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.Core.Attacks;

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

        private bool ShowRoomSummary;
        public const int TurnsToRespawn = 1;
        private readonly List<DeathInfo> ListDeadSquadInfo;
        int MaxKill = 15;
        int MaxGameLengthInMinutes = 10;
        private double GameLengthInSeconds;
        private Player CurrentPlayer { get { return Owner.ListPlayer[Owner.ActivePlayerIndex]; } }

        public LobbyMPGameRule(DeathmatchMap Owner)
        {
            this.Owner = Owner;

            GameLengthInSeconds = 0;
            ListDeadSquadInfo = new List<DeathInfo>();
        }

        public virtual void Init()
        {
            GameLengthInSeconds = MaxGameLengthInMinutes * 60;

            if (Owner.IsOfflineOrServer)
            {
                int PlayerIndex = 0;
                for (int P = 0; P < Owner.ListPlayer.Count; P++)
                {
                    Player ActivePlayer = Owner.ListPlayer[P];
                    if (ActivePlayer.Inventory == null)
                        continue;

                    List<MovementAlgorithmTile> ListPossibleSpawnPoint = Owner.GetSpawnLocations(ActivePlayer.Team);
                    int SpawnSquadIndex = 0;
                    foreach (MovementAlgorithmTile ActiveSpawn in ListPossibleSpawnPoint)
                    {
                        Squad NewSquad = ActivePlayer.Inventory.ActiveLoadout.ListSpawnSquad[SpawnSquadIndex];
                        if (NewSquad == null)
                        {
                            ++SpawnSquadIndex;
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

                        if (!ActivePlayer.IsPlayerControlled || !NewSquad.IsPlayerControlled)
                        {
                            NewSquad.SquadAI = new DeathmatchScripAIContainer(new DeathmatchAIInfo(Owner, NewSquad));
                            NewSquad.SquadAI.Load("Multiplayer/Capture The Flag/Easy");
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
                        ActiveSquad.CurrentLeader.RefillSP(10);
                        ActiveSquad.CurrentLeader.RefillEN(5);
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

            ListDeadSquadInfo.Add(new DeathInfo(DefeatedSquad, DefeatedSquadPlayerIndex, TurnsToRespawn));

            TemporaryAttackPickup DroppedWeapon = DefeatedSquad.At(0).OnDeath();
            if (DroppedWeapon != null)
            {
                DroppedWeapon.Position = AttackerSquad.Position;
                Owner.LayerManager.ListLayer[(int)DefeatedSquad.Z].ListAttackPickup.Add(DroppedWeapon);
            }
        }

        public void OnManualVictory()
        {
            List<LobbyVictoryScreen.PlayerGains> ListGains = new List<LobbyVictoryScreen.PlayerGains>();
            foreach (Player ActivePlayer in Owner.ListAllPlayer)
            {
                LobbyVictoryScreen.PlayerGains NewGains = new LobbyVictoryScreen.PlayerGains();
                NewGains.Exp = 100;
                NewGains.Money = 100;

                ListGains.Add(NewGains);
            }

            LobbyVictoryScreen NewLobbyVictoryScreen = new LobbyVictoryScreen(Owner, ListGains);
            Owner.PushScreen(NewLobbyVictoryScreen);
        }

        public void OnManualDefeat()
        {
        }

        private void RespawnSquad(int ActivePlayerIndex, Squad ActiveSquad)
        {
            List<MovementAlgorithmTile> ListPossibleSpawnPoint = Owner.GetSpawnLocations(Owner.ListPlayer[ActivePlayerIndex].Team);
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

            ActiveSquad.UpdateSquad();

            Owner.ActivateAutomaticSkills(ActiveSquad, string.Empty);
            ActiveSquad.SetPosition(Owner.CursorPosition);

            ActiveSquad.CurrentTerrainIndex = Owner.GetTerrainType(ActiveSquad.X, ActiveSquad.Y, (int)ActiveSquad.Z);
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
        }
    }
}
