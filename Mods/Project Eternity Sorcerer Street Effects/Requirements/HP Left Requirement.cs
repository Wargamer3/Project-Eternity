using System;
using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetHPLeftRequirement : SorcererStreetRequirement
    {
        public enum Targets { Self, Opponent }

        private Targets _Target;
        private Operators.LogicOperators _LogicOperator;
        private Operators.NumberTypes _NumberType;
        private string _HPLeft;

        public SorcererStreetHPLeftRequirement()
            : this(null)
        {
            _Target = Targets.Self;
            _LogicOperator = Operators.LogicOperators.LowerOrEqual;
            _NumberType = Operators.NumberTypes.Relative;
            _HPLeft = string.Empty;
        }

        public SorcererStreetHPLeftRequirement(SorcererStreetBattleParams Params)
            : base("Sorcerer Street HP Left", Params)
        {
            _Target = Targets.Self;
            _LogicOperator = Operators.LogicOperators.LowerOrEqual;
            _NumberType = Operators.NumberTypes.Relative;
            _HPLeft = string.Empty;
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write((byte)_Target);
            BW.Write((byte)_LogicOperator);
            BW.Write((byte)_NumberType);
            BW.Write(_HPLeft);
        }

        protected override void Load(BinaryReader BR)
        {
            _Target = (Targets)BR.ReadByte();
            _LogicOperator = (Operators.LogicOperators)BR.ReadByte();
            _NumberType = (Operators.NumberTypes)BR.ReadByte();
            _HPLeft = BR.ReadString();
        }

        public override bool CanActivatePassive()
        {
            int CreatureHP = 0;
            int CreatureMaxHP = 0;
            switch (_Target)
            {
                case Targets.Self:
                    CreatureHP = Params.GlobalContext.SelfCreature.FinalHP;
                    CreatureMaxHP = Params.GlobalContext.SelfCreature.Creature.MaxHP;
                    break;
                case Targets.Opponent:
                    CreatureHP = Params.GlobalContext.OpponentCreature.FinalHP;
                    CreatureMaxHP = Params.GlobalContext.OpponentCreature.Creature.MaxHP;
                    break;
            }

            if (_NumberType == Operators.NumberTypes.Absolute)
            {
                int HPLeftFinal = int.Parse(Params.ActiveParser.Evaluate(HPLeft), CultureInfo.InvariantCulture);
                return Operators.CompareValue(LogicOperator, CreatureHP, HPLeftFinal);
            }
            else
            {
                int ExpectedHPPercent = int.Parse(Params.ActiveParser.Evaluate(HPLeft), CultureInfo.InvariantCulture);
                int HPPercent = (CreatureHP * 100) / CreatureMaxHP;
                return Operators.CompareValue(LogicOperator, HPPercent, ExpectedHPPercent);
            }
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetHPLeftRequirement NewRequirement = new SorcererStreetHPLeftRequirement(Params);

            NewRequirement._Target = _Target;
            NewRequirement._LogicOperator = _LogicOperator;
            NewRequirement._NumberType = _NumberType;
            NewRequirement._HPLeft = _HPLeft;

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            SorcererStreetHPLeftRequirement CopyRequirement = Copy as SorcererStreetHPLeftRequirement;

            if (CopyRequirement != null)
            {
                _Target = CopyRequirement._Target;
                _LogicOperator = CopyRequirement._LogicOperator;
                _NumberType = CopyRequirement._NumberType;
                _HPLeft = CopyRequirement._HPLeft;
            }
        }

        #region Properties

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public Targets Target
        {
            get
            {
                return _Target;
            }
            set
            {
                _Target = value;
            }
        }

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public Operators.LogicOperators LogicOperator
        {
            get
            {
                return _LogicOperator;
            }
            set
            {
                _LogicOperator = value;
            }
        }

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public Operators.NumberTypes NumberType
        {
            get
            {
                return _NumberType;
            }
            set
            {
                _NumberType = value;
            }
        }

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public string HPLeft
        {
            get
            {
                return _HPLeft;
            }
            set
            {
                _HPLeft = value;
            }
        }

        #endregion
    }
}
