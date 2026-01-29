using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class CampaignGameInfo : GameModeInfo
    {
        public const string ModeName = "Deathmatch";

        private int _ResapwnLimit;
        private int _ResapwnLimitMin;
        private int _ResapwnLimitMax;
        private int _UnitValueLimit;
        private int _UnitValueLimitMin;
        private int _UnitValueLimitMax;

        public CampaignGameInfo(bool IsUnlocked, Texture2D sprPreview)
            : base(ModeName, "Gain points for kills and assists, respawn on death.", CategoryPVP, IsUnlocked, sprPreview)
        {
            _ResapwnLimit = 3000;
            _ResapwnLimitMin = 0;
            _ResapwnLimitMax = 100000;
            _UnitValueLimit = 400;
            _UnitValueLimitMin = 0;
            _UnitValueLimitMax = 10000;
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(_ResapwnLimit);
            BW.Write(_ResapwnLimitMin);
            BW.Write(_ResapwnLimitMax);

            BW.Write(_UnitValueLimit);
            BW.Write(_UnitValueLimitMin);
            BW.Write(_UnitValueLimitMax);
        }

        public override void Load(BinaryReader BR)
        {
            _ResapwnLimit = BR.ReadInt32();
            _ResapwnLimitMin = BR.ReadInt32();
            _ResapwnLimitMax = BR.ReadInt32();

            _UnitValueLimit = BR.ReadInt32();
            _UnitValueLimitMin = BR.ReadInt32();
            _UnitValueLimitMax = BR.ReadInt32();
        }

        public override IGameRule GetRule(BattleMap Map)
        {
            if (Map == null)
            {
                return new CampaignGameRule(null, this);
            }
            else
            {
                return new CampaignGameRule((SorcererStreetMap)Map, this);
            }
        }

        public override GameModeInfo Copy()
        {
            return new CampaignGameInfo(IsUnlocked, sprPreview);
        }

        #region Properties


        #endregion
    }

    class CampaignGameRule : IGameRule
    {
        private readonly SorcererStreetMap Owner;

        public string Name => "Campaign";

        public CampaignGameRule(SorcererStreetMap Owner, CampaignGameInfo GameInfo)
        {
            this.Owner = Owner;
        }

        public void Init()
        {
            if (Owner.IsOfflineOrServer)
            {
                for (int P = 0; P < Owner.ListPlayer.Count; P++)
                {
                    Player ActivePlayer = Owner.ListPlayer[P];
                    if (ActivePlayer.Inventory == null)
                        continue;

                    if (!Owner.DicTeam.ContainsKey(ActivePlayer.TeamIndex))
                    {
                        Owner.DicTeam.Add(ActivePlayer.TeamIndex, new Team(ActivePlayer.TeamIndex));
                        Owner.DicTeam[ActivePlayer.TeamIndex].TotalMagic = ActivePlayer.Gold = Owner.MagicAtStart;
                    }

                    List<Vector3> ListPossibleSpawnPoint = Owner.GetMultiplayerSpawnLocations(ActivePlayer.TeamIndex);
                    int SpawnSquadIndex = 0;
                    foreach (Vector3 ActiveSpawn in ListPossibleSpawnPoint)
                    {
                        ActivePlayer.GamePiece.SetPosition(Owner.GetFinalPosition(new Vector3(ActiveSpawn.X * Owner.TileSize.X, ActiveSpawn.Y * Owner.TileSize.Y, ActiveSpawn.Z)) + new Vector3(Owner.TileSize.X / 2, Owner.TileSize.Y / 2, 0));

                        ++SpawnSquadIndex;

                        if (!ActivePlayer.IsPlayerControlled)
                        {
                            //ActivePlayer.GamePiece.AI = new SorcererStreetAIContainer(new SorcererStreetAIInfo(Owner, ActivePlayer));
                            //ActivePlayer.GamePiece.AI.Load("Multiplayer/Easy");
                        }

                        for (int C = 0; C < 4 && ActivePlayer.ListRemainingCardInDeck.Count > 0; ++C)
                        {
                            int RandomCardIndex = RandomHelper.Next(ActivePlayer.ListRemainingCardInDeck.Count);
                            Card DrawnCard = ActivePlayer.ListRemainingCardInDeck[RandomCardIndex];

                            ActivePlayer.ListCardInHand.Add(DrawnCard);
                            ActivePlayer.ListRemainingCardInDeck.RemoveAt(RandomCardIndex);
                        }

                        Owner.DicTeam[ActivePlayer.TeamIndex].ListPlayer.Add(ActivePlayer);
                        break;
                    }
                }

                Owner.UpdatePlayersRank();
            }
        }

        public int GetRemainingResapwn(int PlayerIndex)
        {
            throw new NotImplementedException();
        }

        public void OnTurnEnd(int ActivePlayerIndex)
        {
            if (Owner.DicTeam[Owner.ListPlayer[ActivePlayerIndex].TeamIndex].TotalMagic > Owner.MagicGoal)
            {
                string FullMapPath = Owner.GetMapType() + "/" + Owner.BattleMapPath;

                List<LobbyVictoryScreen.PlayerGains> ListGains = new List<LobbyVictoryScreen.PlayerGains>();
                foreach (Player ActivePlayer in Owner.ListAllPlayer)
                {
                    if (ActivePlayer.TeamIndex == -1)
                    {
                        continue;
                    }

                    LobbyVictoryScreen.PlayerGains NewGains = new LobbyVictoryScreen.PlayerGains();
                    foreach (Player OtherPlayer in Owner.ListAllPlayer)
                    {
                        if (OtherPlayer.TeamIndex < 0)
                        {
                            continue;
                        }

                        if (OtherPlayer.TeamIndex  == ActivePlayer.TeamIndex)
                        {
                            NewGains.EXP += GetVictoryEXP(ActivePlayer, OtherPlayer);
                            NewGains.Money += GetVictoryMoney(ActivePlayer);
                        }
                        else
                        {
                            NewGains.EXP += GetDefeatEXP(ActivePlayer, OtherPlayer);
                            NewGains.Money += GetDefeatMoney(ActivePlayer);
                        }

                        if (!OtherPlayer.Records.DicCampaignLevelInformation.ContainsKey(FullMapPath))
                        {
                            OtherPlayer.Records.DicCampaignLevelInformation.Add(FullMapPath, new CampaignRecord(Owner.BattleMapPath, 0));
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

                Owner.PushScreen(new LobbyVictoryScreen(Owner, null));
                Owner.RemoveScreen(Owner);
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

        public void OnNewTurn(int ActivePlayerIndex)
        {
        }

        public void OnSquadDefeated(int AttackerSquadPlayerIndex, Squad AttackerSquad, int DefeatedSquadPlayerIndex, Squad DefeatedSquad)
        {
        }

        public void OnManualVictory(int EXP, uint Money)
        {
            string FullMapPath = Owner.GetMapType() + "/" + Owner.BattleMapPath;
            List<LobbyVictoryScreen.PlayerGains> ListGains = new List<LobbyVictoryScreen.PlayerGains>();
            foreach (Player ActivePlayer in Owner.ListLocalPlayer)
            {
                LobbyVictoryScreen.PlayerGains NewGains = new LobbyVictoryScreen.PlayerGains();
                NewGains.EXP = EXP;
                NewGains.Money = Money;

                if (!ActivePlayer.Records.DicCampaignLevelInformation.ContainsKey(FullMapPath))
                {
                    ActivePlayer.Records.DicCampaignLevelInformation.Add(FullMapPath, new CampaignRecord(Owner.BattleMapPath, 0));
                }

                ListGains.Add(NewGains);
            }

            LobbyVictoryScreen NewLobbyVictoryScreen = new LobbyVictoryScreen(Owner, ListGains);
            Owner.PushScreen(NewLobbyVictoryScreen);
        }

        public void OnManualDefeat(int EXP, uint Money)
        {
        }

        public void Update(GameTime gameTime)
        {
            if (!Owner.IsEditor)
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
        }

        public void BeginDraw(CustomSpriteBatch g)
        {

        }

        public void Draw(CustomSpriteBatch g)
        {

        }

        public List<GameRuleError> Validate(RoomInformations Room)
        {
            while (Room.ListRoomBot.Count <= Room.MaxNumberOfPlayer)
            {
                Player NewBot = new Player(PlayerManager.OnlinePlayerID, "Bot " + (Room.ListRoomBot.Count + 1), OnlinePlayerBase.PlayerTypes.Bot, false, Room.ListRoomBot.Count, false, Color.Blue);
                NewBot.InitFirstTimeInventory();
                Room.ListRoomBot.Add(NewBot);
            }

            while (Room.ListRoomBot.Count > Room.MaxNumberOfPlayer)
            {
                Room.ListRoomBot.RemoveAt(Room.ListRoomBot.Count - 1);
            }

            while (Room.ListRoomPlayer.Count <= Room.MaxNumberOfPlayer)
            {
                Room.ListRoomPlayer.Add(null);
            }

            while (Room.ListRoomPlayer.Count > Room.MaxNumberOfPlayer)
            {
                Room.ListRoomPlayer.RemoveAt(Room.ListRoomPlayer.Count - 1);
            }

            return new List<GameRuleError>();
        }
    }
}
