using System;
using System.IO;
using System.ComponentModel;
using System.Globalization;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetDamageDealtRequirement : SorcererStreetRequirement
    {
        public enum Targets { Self, Opponent }

        private Targets _Target;
        private Operators.LogicOperators _LogicOperator;
        private string _DamageDelt;

        public SorcererStreetDamageDealtRequirement()
            : this(null)
        {
            _Target = Targets.Self;
            _LogicOperator = Operators.LogicOperators.LowerOrEqual;
            _DamageDelt = string.Empty;
        }

        public SorcererStreetDamageDealtRequirement(SorcererStreetBattleParams Params)
            : base("Sorcerer Street Damage Dealt", Params)
        {
            _Target = Targets.Self;
            _LogicOperator = Operators.LogicOperators.LowerOrEqual;
            _DamageDelt = string.Empty;
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write((byte)_Target);
            BW.Write((byte)_LogicOperator);
            BW.Write(_DamageDelt);
        }

        protected override void Load(BinaryReader BR)
        {
            _Target = (Targets)BR.ReadByte();
            _LogicOperator = (Operators.LogicOperators)BR.ReadByte();
            _DamageDelt = BR.ReadString();
        }

        public override bool CanActivatePassive()
        {
            int DamageDelt = 0;
            switch (_Target)
            {
                case Targets.Self:
                    DamageDelt = Params.GlobalContext.OpponentCreature.DamageReceived;
                    break;
                case Targets.Opponent:
                    DamageDelt = Params.GlobalContext.SelfCreature.DamageReceived;
                    break;
            }

            int DamageDeltFinal = int.Parse(Params.ActiveParser.Evaluate(_DamageDelt), CultureInfo.InvariantCulture);
            return Operators.CompareValue(LogicOperator, DamageDelt, DamageDeltFinal);
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetDamageDealtRequirement NewRequirement = new SorcererStreetDamageDealtRequirement(Params);

            NewRequirement._Target = _Target;
            NewRequirement._LogicOperator = _LogicOperator;
            NewRequirement._DamageDelt = _DamageDelt;

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            SorcererStreetDamageDealtRequirement NewRequirement = (SorcererStreetDamageDealtRequirement)Copy;

            _Target = NewRequirement._Target;
            _LogicOperator = NewRequirement._LogicOperator;
            _DamageDelt = NewRequirement._DamageDelt;
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
                return _DamageDelt;
            }
            set
            {
                _DamageDelt = value;
            }
        }

        #endregion
    }
}
