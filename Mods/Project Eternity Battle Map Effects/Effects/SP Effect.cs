using System;
using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class SPEffect : SkillEffect
    {
        public static string Name = "SP Effect";

        private Operators.NumberTypes _NumberType;
        private string _SPValue;

        public SPEffect()
            : base(Name, false)
        {
        }

        public SPEffect(UnitEffectParams Params)
            : base(Name, false, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _NumberType = (Operators.NumberTypes)BR.ReadByte();
            _SPValue = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_NumberType);
            BW.Write(_SPValue);
        }

        public override bool CanActivate()
        {
            return Params.GlobalContext.EffectTargetCharacter.SP < Params.GlobalContext.EffectTargetCharacter.MaxSP;
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = Params.GlobalContext.ActiveParser.Evaluate(SPValue);
            Character TargetCharacter = Params.LocalContext.EffectTargetCharacter;

            string Extra = string.Empty;

            if (EvaluationResult != SPValue)
            {
                Extra = " (" + SPValue + ")";
            }

            int EvaluationValue = 0;
            if (NumberType == Operators.NumberTypes.Absolute)
            {
                EvaluationValue = (int)double.Parse(EvaluationResult, CultureInfo.InvariantCulture);
            }
            else if (NumberType == Operators.NumberTypes.Relative)
            {
                EvaluationValue = (int)(TargetCharacter.SP * float.Parse(EvaluationResult, CultureInfo.InvariantCulture));
            }

            TargetCharacter.SP = Math.Min(TargetCharacter.MaxSP, TargetCharacter.SP + EvaluationValue);

            return "SP Increased by " + EvaluationValue + Extra;
        }

        protected override void ReactivateEffect()
        {
            //Don't regen SP on reactivation
        }

        protected override BaseEffect DoCopy()
        {
            SPEffect NewEffect = new SPEffect(Params);

            NewEffect._NumberType = _NumberType;
            NewEffect._SPValue = _SPValue;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            SPEffect NewEffect = (SPEffect)Copy;

            _NumberType = NewEffect._NumberType;
            _SPValue = NewEffect._SPValue;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public Operators.NumberTypes NumberType
        {
            get { return _NumberType; }
            set { _NumberType = value; }
        }

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public string SPValue
        {
            get { return _SPValue; }
            set { _SPValue = value; }
        }

        #endregion
    }
}
