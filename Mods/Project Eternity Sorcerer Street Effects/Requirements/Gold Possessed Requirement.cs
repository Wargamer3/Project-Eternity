using System;
using System.IO;
using ProjectEternity.Core;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using static ProjectEternity.Core.Operators;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetGoldPossessedRequirement : SorcererStreetRequirement
    {
        public enum Targets { Self, Opponent }

        private Targets _Target;
        private LogicOperators _LogicOperator;
        private NumberTypes _SignOperator;
        private string _Value;

        public SorcererStreetGoldPossessedRequirement()
            : this(null)
        {
        }

        public SorcererStreetGoldPossessedRequirement(SorcererStreetBattleParams Params)
            : base("Sorcerer Street Gold Possessed", Params)
        {
            _Target = Targets.Self;
            _LogicOperator = LogicOperators.Lower;
            _SignOperator = NumberTypes.Absolute;
            _Value = "opponentplayer.gold";
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write((byte)_Target);
            BW.Write((byte)_LogicOperator);
            BW.Write((byte)_SignOperator);
            BW.Write(_Value);
        }

        protected override void Load(BinaryReader BR)
        {
            _Target = (Targets)BR.ReadByte();
            _LogicOperator = (LogicOperators)BR.ReadByte();
            _SignOperator = (NumberTypes)BR.ReadByte();
            _Value = BR.ReadString();
        }

        public override bool CanActivatePassive()
        {
            string EvaluationResult = Params.ActiveParser.Evaluate(_Value);

            Player FinalTarget;

            if (_Target == Targets.Self)
            {
                FinalTarget = Params.GlobalContext.SelfCreature.Owner;
            }
            else
            {
                FinalTarget = Params.GlobalContext.OpponentCreature.Owner;
            }

            if (_SignOperator == NumberTypes.Absolute)
            {
                return Operators.CompareValue(_LogicOperator, FinalTarget.Gold, int.Parse(EvaluationResult, CultureInfo.InvariantCulture));
            }
            else
            {
                float FinalResult = FinalTarget.Gold * float.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                return Operators.CompareValue(_LogicOperator, (int)FinalResult, int.Parse(EvaluationResult, CultureInfo.InvariantCulture));
            }
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetGoldPossessedRequirement NewRequirement = new SorcererStreetGoldPossessedRequirement(Params);

            NewRequirement._Target = _Target;
            NewRequirement._LogicOperator = _LogicOperator;
            NewRequirement._SignOperator = _SignOperator;
            NewRequirement._Value = _Value;

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            SorcererStreetGoldPossessedRequirement CopyRequirement = (SorcererStreetGoldPossessedRequirement)Copy;

            _Target = CopyRequirement._Target;
            _LogicOperator = CopyRequirement._LogicOperator;
            _SignOperator = CopyRequirement._SignOperator;
            _Value = CopyRequirement._Value;
        }

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
        public LogicOperators LogicOperator
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
        public NumberTypes SignOperator
        {
            get
            {
                return _SignOperator;
            }
            set
            {
                _SignOperator = value;
            }
        }

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }
    }
}
