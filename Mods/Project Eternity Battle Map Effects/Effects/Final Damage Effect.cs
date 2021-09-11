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
        private string LastEvaluationResult = null;

        public FinalDamageEffect()
            : base(Name, true)
        {
        }

        public FinalDamageEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _NumberType = (Operators.NumberTypes)BR.ReadByte();
            _FinalDamageValue = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_NumberType);
            BW.Write(_FinalDamageValue);
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = GetFinalDamageValue();

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

        private string GetFinalDamageValue()
        {
            string EvaluationResult;

            try
            {
                EvaluationResult = FormulaParser.ActiveParser.Evaluate(_FinalDamageValue);
                LastEvaluationResult = EvaluationResult;
            }
            catch(Exception)
            {
                return LastEvaluationResult;
            }

            return EvaluationResult;
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
