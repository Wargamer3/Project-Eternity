using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.LifeSimScreen
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
                return new DeathmatchGameRule((LifeSimMap)Map, this);
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
        private readonly LifeSimMap Owner;

        public string Name => "Deathmatch";

        public DeathmatchGameRule(LifeSimMap Owner, DeathmatchGameInfo GameInfo)
        {
            this.Owner = Owner;
        }

        public void Init()
        {
            if (Owner.IsOfflineOrServer)
            {
            }
        }

        public int GetRemainingResapwn(int PlayerIndex)
        {
            throw new NotImplementedException();
        }

        public void OnTurnEnd(int ActivePlayerIndex)
        {
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
