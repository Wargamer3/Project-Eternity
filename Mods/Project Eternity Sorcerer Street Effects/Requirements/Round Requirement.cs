using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetRoundRequirement : SorcererStreetRequirement
    {
        private Operators.LogicOperators _LogicOperator;
        private string _HPLeft;

        public SorcererStreetRoundRequirement()
            : this(null)
        {
            _LogicOperator = Operators.LogicOperators.LowerOrEqual;
            _HPLeft = string.Empty;
        }

        public SorcererStreetRoundRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Sorcerer Street Round Reached", GlobalContext)
        {
            _LogicOperator = Operators.LogicOperators.LowerOrEqual;
            _HPLeft = string.Empty;
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write((byte)_LogicOperator);
            BW.Write(_HPLeft);
        }

        protected override void Load(BinaryReader BR)
        {
            _LogicOperator = (Operators.LogicOperators)BR.ReadByte();
            _HPLeft = BR.ReadString();
        }

        public override bool CanActivatePassive()
        {
            return false;
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetRoundRequirement NewRequirement = new SorcererStreetRoundRequirement(GlobalContext);

            NewRequirement._LogicOperator = _LogicOperator;
            NewRequirement._HPLeft = _HPLeft;

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            SorcererStreetRoundRequirement CopyRequirement = (SorcererStreetRoundRequirement)Copy;

            _LogicOperator = CopyRequirement._LogicOperator;
            _HPLeft = CopyRequirement._HPLeft;
        }

        #region Properties

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
