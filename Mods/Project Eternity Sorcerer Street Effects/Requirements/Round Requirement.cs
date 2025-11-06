using System;
using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetRoundRequirement : SorcererStreetRequirement
    {
        private Operators.LogicOperators _LogicOperator;
        private string _RoundLeft;

        public SorcererStreetRoundRequirement()
            : this(null)
        {
            _LogicOperator = Operators.LogicOperators.LowerOrEqual;
            _RoundLeft = string.Empty;
        }

        public SorcererStreetRoundRequirement(SorcererStreetBattleParams Params)
            : base("Sorcerer Street Round Reached", Params)
        {
            _LogicOperator = Operators.LogicOperators.LowerOrEqual;
            _RoundLeft = string.Empty;
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write((byte)_LogicOperator);
            BW.Write(_RoundLeft);
        }

        protected override void Load(BinaryReader BR)
        {
            _LogicOperator = (Operators.LogicOperators)BR.ReadByte();
            _RoundLeft = BR.ReadString();
        }

        public override bool CanActivatePassive()
        {
            int RoundLeftFinal = int.Parse(Params.ActiveParser.Evaluate(_RoundLeft), CultureInfo.InvariantCulture);

            return Operators.CompareValue(_LogicOperator, Params.GlobalContext.CurrentTurn, RoundLeftFinal);
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetRoundRequirement NewRequirement = new SorcererStreetRoundRequirement(Params);

            NewRequirement._LogicOperator = _LogicOperator;
            NewRequirement._RoundLeft = _RoundLeft;

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            SorcererStreetRoundRequirement CopyRequirement = (SorcererStreetRoundRequirement)Copy;

            _LogicOperator = CopyRequirement._LogicOperator;
            _RoundLeft = CopyRequirement._RoundLeft;
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
        public string RoundLeft
        {
            get
            {
                return _RoundLeft;
            }
            set
            {
                _RoundLeft = value;
            }
        }

        #endregion
    }
}
