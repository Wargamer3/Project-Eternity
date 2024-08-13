using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
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
                return new DeathmatchGameRule((ConquestMap)Map, this);
            }
        }

        public override GameModeInfo Copy()
        {
            return new DeathmatchGameInfo(IsUnlocked, sprPreview);
        }

        #region Properties

        [DisplayNameAttribute("Resapwn Limit"),
        CategoryAttribute("Respawn"),
        DescriptionAttribute("How many points are allowed to respawn."),
        EditableInGame(true)]
        public int ResapwnLimit
        {
            get
            {
                return _ResapwnLimit;
            }
            set
            {
                _ResapwnLimit = Math.Max(_ResapwnLimitMin, Math.Min(value, _ResapwnLimitMax));
            }
        }

        [DisplayNameAttribute("Resapwn Limit Minimum"),
        CategoryAttribute("Respawn"),
        DescriptionAttribute("Minimum value allowed when modifying the respawn limit."),
        EditableInGame(false)]
        public int ResapwnLimitMin
        {
            get
            {
                return _ResapwnLimitMin;
            }
            set
            {
                _ResapwnLimitMin = value;
            }
        }

        [DisplayNameAttribute("Resapwn Limit Maximum"),
        CategoryAttribute("Respawn"),
        DescriptionAttribute("Maximum value allowed when modifying the respawn limit."),
        EditableInGame(false)]
        public int ResapwnLimitMax
        {
            get
            {
                return _ResapwnLimitMax;
            }
            set
            {
                _ResapwnLimitMax = value;
            }
        }

        [DisplayNameAttribute("Unit Value Limit"),
        CategoryAttribute("Respawn"),
        DescriptionAttribute("Maximum value of a unit allowed to spawn in the game."),
        EditableInGame(true)]
        public int UnitValueLimit
        {
            get
            {
                return _UnitValueLimit;
            }
            set
            {
                _UnitValueLimit = Math.Max(_UnitValueLimitMin, Math.Min(value, _UnitValueLimitMax));
            }
        }

        [DisplayNameAttribute("Unit Value Limit Minimum"),
        CategoryAttribute("Respawn"),
        DescriptionAttribute("Minimum value allowed when modifying the unit limit."),
        EditableInGame(false)]
        public int UnitValueLimitMin
        {
            get
            {
                return _UnitValueLimitMin;
            }
            set
            {
                _UnitValueLimitMin = value;
            }
        }

        [DisplayNameAttribute("Unit Value Limit Maximum"),
        CategoryAttribute("Respawn"),
        DescriptionAttribute("Maximum value allowed when modifying the unit limit."),
        EditableInGame(false)]
        public int UnitValueLimitMax
        {
            get
            {
                return _UnitValueLimitMax;
            }
            set
            {
                _UnitValueLimitMax = value;
            }
        }

        #endregion
    }

    class DeathmatchGameRule : LobbyMPGameRule
    {
        private readonly ConquestMap Owner;
        private readonly DeathmatchGameInfo GameInfo;

        public override string Name => GameInfo.Name;

        public DeathmatchGameRule(ConquestMap Owner, DeathmatchGameInfo GameInfo)
            : base(Owner)
        {
            this.Owner = Owner;
            this.GameInfo = GameInfo;
        }

        public override List<GameRuleError> Validate(RoomInformations Room)
        {
            List<GameRuleError> ListGameRuleError = new List<GameRuleError>();

            foreach (Player ActivePlayer in Room.GetLocalPlayers())
            {
                foreach (Squad ActiveSquad in ActivePlayer.Inventory.ActiveLoadout.ListSpawnSquad)
                {
                    for (int U = 0; U < ActiveSquad.UnitsInSquad; ++U)
                    {
                        Unit ActiveUnit = ActiveSquad.At(U);
                        if (ActiveUnit.UnitStat.SpawnCost > GameInfo.UnitValueLimit)
                        {
                            ListGameRuleError.Add(new GameRuleError("Unit Value is too high", ActiveUnit));
                        }
                    }
                }
            }

            return ListGameRuleError;
        }
    }
}
