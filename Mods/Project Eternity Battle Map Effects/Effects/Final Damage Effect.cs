using System;
using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class FinalDamageEffect : SkillEffect
    {
        public static string Name = "Final Damage Effect";

        private Operators.NumberTypes _NumberType;
        private string _FinalDamageValue;
        private string LastEvaluationResult;

        public FinalDamageEffect()
            : base(Name, true)
        {
            LastEvaluationResult = string.Empty;
        }

        public FinalDamageEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
            LastEvaluationResult = string.Empty;
        }

        protected override void Load(BinaryReader BR)
        {
            _NumberType = (Operators.NumberTypes)BR.ReadByte();
            _FinalDamageValue = BR.ReadString();
        }

        protected override void DoQuickLoad(BinaryReader BR, FormulaParser ActiveParser)
        {
            base.DoQuickLoad(BR, ActiveParser);

            LastEvaluationResult = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_NumberType);
            BW.Write(_FinalDamageValue);
        }

        protected override void DoQuickSave(BinaryWriter BW)
        {
            base.DoQuickSave(BW);

            BW.Write(LastEvaluationResult);
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = Params.GlobalContext.ActiveParser.Evaluate(_FinalDamageValue);
            LastEvaluationResult = EvaluationResult;

            string ExtraText = "";
            if (EvaluationResult != _FinalDamageValue)
            {
                ExtraText = " (" + _FinalDamageValue + ")";
            }

            if (NumberType == Operators.NumberTypes.Absolute)
            {
                int EvaluationValue = (int)double.Parse(EvaluationResult, CultureInfo.InvariantCulture);

                Params.LocalContext.EffectTargetUnit.Boosts.FinalDamageModifier = EvaluationValue;

                return "Will deal " + EvaluationValue + ExtraText + " extra damage";
            }
            else if (NumberType == Operators.NumberTypes.Relative)
            {
                float EvaluationValue = float.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                Params.LocalContext.EffectTargetUnit.Boosts.FinalDamageMultiplier = EvaluationValue;

                return "Will deal " + EvaluationValue * 100 + ExtraText + "% damage";
            }

            return _FinalDamageValue;
        }

        protected override void ReactivateEffect()
        {
            if (NumberType == Operators.NumberTypes.Absolute)
            {
                int EvaluationValue = (int)double.Parse(LastEvaluationResult, CultureInfo.InvariantCulture);

                Params.LocalContext.EffectTargetUnit.Boosts.FinalDamageModifier = EvaluationValue;
            }
            else if (NumberType == Operators.NumberTypes.Relative)
            {
                float EvaluationValue = float.Parse(LastEvaluationResult, CultureInfo.InvariantCulture);
                Params.LocalContext.EffectTargetUnit.Boosts.FinalDamageMultiplier = EvaluationValue;
            }
        }

        protected override BaseEffect DoCopy()
        {
            FinalDamageEffect NewEffect = new FinalDamageEffect(Params);

            NewEffect._NumberType = _NumberType;
            NewEffect._FinalDamageValue = _FinalDamageValue;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            FinalDamageEffect NewEffect = (FinalDamageEffect)Copy;

            _NumberType = NewEffect._NumberType;
            _FinalDamageValue = NewEffect._FinalDamageValue;
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
        public string FinalDamageValue
        {
            get { return _FinalDamageValue; }
            set { _FinalDamageValue = value; }
        }

        #endregion
    }
}
