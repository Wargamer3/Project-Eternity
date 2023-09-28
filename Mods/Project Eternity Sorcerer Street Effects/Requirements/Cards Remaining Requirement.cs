using System;
using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using static ProjectEternity.Core.Operators;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetCardsRemainingRequirement : SorcererStreetRequirement
    {
        public enum Targets { SelfHand, SelfDeck, OpponentHand, OpponentDeck }

        private Targets _Target;
        private LogicOperators _LogicOperator;
        private NumberTypes _SignOperator;
        private string _Value;

        public SorcererStreetCardsRemainingRequirement()
            : this(null)
        {
        }

        public SorcererStreetCardsRemainingRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Sorcerer Street Cards Remaining", GlobalContext)
        {
            _Target = Targets.SelfHand;
            _LogicOperator = LogicOperators.LowerOrEqual;
            _SignOperator = NumberTypes.Absolute;
            _Value = "3";
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
            string EvaluationResult = GlobalContext.ActiveParser.Evaluate(_Value);

            int CurrentCards = 0;

            if (_Target == Targets.SelfHand)
            {
                CurrentCards = GlobalContext.SelfCreature.Owner.ListCardInHand.Count;
            }
            else if (_Target == Targets.SelfDeck)
            {
                CurrentCards = GlobalContext.SelfCreature.Owner.ListCardInDeck.Count;
            }
            else if (_Target == Targets.OpponentHand)
            {
                CurrentCards = GlobalContext.OpponentCreature.Owner.ListCardInHand.Count;
            }
            else if (_Target == Targets.OpponentDeck)
            {
                CurrentCards = GlobalContext.OpponentCreature.Owner.ListCardInDeck.Count;
            }

            if (_SignOperator == NumberTypes.Absolute)
            {
                return Operators.CompareValue(_LogicOperator, CurrentCards, int.Parse(EvaluationResult, CultureInfo.InvariantCulture));
            }
            else
            {
                float FinalResult = CurrentCards * float.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                return Operators.CompareValue(_LogicOperator, CurrentCards, (int)FinalResult);
            }
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetCardsRemainingRequirement NewRequirement = new SorcererStreetCardsRemainingRequirement(GlobalContext);

            NewRequirement._Target = _Target;
            NewRequirement._LogicOperator = _LogicOperator;
            NewRequirement._SignOperator = _SignOperator;
            NewRequirement._Value = _Value;

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            SorcererStreetCardsRemainingRequirement CopyRequirement = (SorcererStreetCardsRemainingRequirement)Copy;

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
