using System;
using System.IO;
using System.ComponentModel;
using System.Globalization;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetSTRequirement : SorcererStreetRequirement
    {
        public enum Targets { Self, Opponent }

        private Targets _Target;
        private Operators.LogicOperators _LogicOperator;
        private string _ST;

        public SorcererStreetSTRequirement()
            : this(null)
        {
            _Target = Targets.Self;
            _LogicOperator = Operators.LogicOperators.LowerOrEqual;
            _ST = string.Empty;
        }

        public SorcererStreetSTRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Sorcerer Street ST", GlobalContext)
        {
            _Target = Targets.Self;
            _LogicOperator = Operators.LogicOperators.LowerOrEqual;
            _ST = string.Empty;
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write((byte)_Target);
            BW.Write((byte)_LogicOperator);
            BW.Write(_ST);
        }

        protected override void Load(BinaryReader BR)
        {
            _Target = (Targets)BR.ReadByte();
            _LogicOperator = (Operators.LogicOperators)BR.ReadByte();
            _ST = BR.ReadString();
        }

        public override bool CanActivatePassive()
        {
            int CreatureST = 0;
            switch (_Target)
            {
                case Targets.Self:
                    CreatureST = GlobalContext.SelfCreature.FinalST;
                    break;
                case Targets.Opponent:
                    CreatureST = GlobalContext.OpponentCreature.FinalST;
                    break;
            }

            int STFinal = int.Parse(GlobalContext.ActiveParser.Evaluate(_ST), CultureInfo.InvariantCulture);
            return Operators.CompareValue(LogicOperator, CreatureST, STFinal);
        }

        public override BaseSkillRequirement Copy()
        {
            return new SorcererStreetSTRequirement(GlobalContext);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
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
        public string ST
        {
            get
            {
                return _ST;
            }
            set
            {
                _ST = value;
            }
        }

        #endregion
    }
}
