using System;
using System.IO;
using System.ComponentModel;
using System.Globalization;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetMaxHPRequirement : SorcererStreetBattleRequirement
    {
        public enum Targets { Self, Opponent }

        private Targets _Target;
        private Operators.LogicOperators _LogicOperator;
        private string _MaxHP;

        public SorcererStreetMaxHPRequirement()
            : this(null)
        {
            _Target = Targets.Self;
            _LogicOperator = Operators.LogicOperators.LowerOrEqual;
            _MaxHP = string.Empty;
        }

        public SorcererStreetMaxHPRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Sorcerer Street Max HP", GlobalContext)
        {
            _Target = Targets.Self;
            _LogicOperator = Operators.LogicOperators.LowerOrEqual;
            _MaxHP = string.Empty;
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write((byte)_Target);
            BW.Write((byte)_LogicOperator);
            BW.Write(_MaxHP);
        }

        protected override void Load(BinaryReader BR)
        {
            _Target = (Targets)BR.ReadByte();
            _LogicOperator = (Operators.LogicOperators)BR.ReadByte();
            _MaxHP = BR.ReadString();
        }

        public override bool CanActivatePassive()
        {
            int CreatureMaxHP = 0;
            switch (_Target)
            {
                case Targets.Self:
                    CreatureMaxHP = GlobalContext.SelfCreature.Creature.MaxHP;
                    break;
                case Targets.Opponent:
                    CreatureMaxHP = GlobalContext.OpponentCreature.Creature.MaxHP;
                    break;
            }

            int MaxHPFinal = int.Parse(GlobalContext.ActiveParser.Evaluate(_MaxHP), CultureInfo.InvariantCulture);
            return Operators.CompareValue(LogicOperator, CreatureMaxHP, MaxHPFinal);
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetMaxHPRequirement NewRequirement = new SorcererStreetMaxHPRequirement(GlobalContext);

            NewRequirement._Target = _Target;
            NewRequirement._LogicOperator = _LogicOperator;
            NewRequirement._MaxHP = _MaxHP;

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            SorcererStreetMaxHPRequirement NewRequirement = (SorcererStreetMaxHPRequirement)Copy;

            _Target = NewRequirement._Target;
            _LogicOperator = NewRequirement._LogicOperator;
            _MaxHP = NewRequirement._MaxHP;
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
        public string MaxHP
        {
            get
            {
                return _MaxHP;
            }
            set
            {
                _MaxHP = value;
            }
        }

        #endregion
    }
}
