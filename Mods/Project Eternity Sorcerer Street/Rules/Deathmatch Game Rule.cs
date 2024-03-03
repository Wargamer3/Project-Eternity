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
    public class DeathmatchGameInfo : GameModeInfo
    {
        public const string ModeName = "Deathmatch";

        private int _ResapwnLimit;
        private int _ResapwnLimitMin;
        private int _ResapwnLimitMax;
        private int _UnitValueLimit;
        private int _UnitValueLimitMin;
        private int _UnitValueLimitMax;

        public DeathmatchGameInfo(bool IsUnlocked, Texture2D sprPreview)
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
                return new DeathmatchGameRule(null, this);
            }
            else
            {
                return new DeathmatchGameRule((SorcererStreetMap)Map, this);
            }
        }

        public override GameModeInfo Copy()
        {
            return new DeathmatchGameInfo(IsUnlocked, sprPreview);
        }

        #region Properties


        #endregion
    }

    class DeathmatchGameRule : IGameRule
    {
        private readonly SorcererStreetMap Owner;

        public string Name => "Deathmatch";

        public DeathmatchGameRule(SorcererStreetMap Owner, DeathmatchGameInfo GameInfo)
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

                    List<MovementAlgorithmTile> ListPossibleSpawnPoint = Owner.GetMultiplayerSpawnLocations(ActivePlayer.Team);
                    int SpawnSquadIndex = 0;
                    foreach (MovementAlgorithmTile ActiveSpawn in ListPossibleSpawnPoint)
                    {
                        ActivePlayer.GamePiece.SetPosition(new Vector3(ActiveSpawn.InternalPosition.X, ActiveSpawn.InternalPosition.Y, ActiveSpawn.LayerIndex));

                        ++SpawnSquadIndex;

                        if (!ActivePlayer.IsPlayerControlled)
                        {
                            //ActivePlayer.GamePiece.AI = new SorcererStreetAIContainer(new SorcererStreetAIInfo(Owner, ActivePlayer));
                            //ActivePlayer.GamePiece.AI.Load("Multiplayer/Easy");
                        }

                        if (Owner != ActiveSpawn.Owner)
                        {
                            ActiveSpawn.Owner.AddUnit(P, ActivePlayer.GamePiece, ActiveSpawn);
                            Owner.RemoveUnit(P, ActivePlayer.GamePiece);
                            Owner.SelectPlatform(Owner.GetPlatform(ActiveSpawn.Owner));
                        }

                        for (int C = 0; C < 4 && ActivePlayer.ListRemainingCardInDeck.Count > 0; ++C)
                        {
                            int RandomCardIndex = RandomHelper.Next(ActivePlayer.ListRemainingCardInDeck.Count);
                            Card DrawnCard = ActivePlayer.ListRemainingCardInDeck[RandomCardIndex];

                            ActivePlayer.ListCardInHand.Add(DrawnCard);
                            ActivePlayer.ListRemainingCardInDeck.RemoveAt(RandomCardIndex);
                        }

                        ActivePlayer.Team = P;
                        ActivePlayer.TotalMagic = ActivePlayer.Gold = Owner.MagicAtStart;
                        break;
                    }
                }
            }
        }

        public int GetRemainingResapwn(int PlayerIndex)
        {
            throw new NotImplementedException();
        }

        public void OnNewTurn(int ActivePlayerIndex)
        {
        }

        public void OnSquadDefeated(int AttackerSquadPlayerIndex, Squad AttackerSquad, int DefeatedSquadPlayerIndex, Squad DefeatedSquad)
        {
        }

        public void OnManualVictory(int EXP, uint Money)
        {
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
            return new List<GameRuleError>();
        }
    }
}
