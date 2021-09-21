using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class CriticalBaseDamageEffect : SkillEffect
    {
        public static string Name = "Critical Base Damage Effect";

        private Operators.NumberTypes _NumberType;
        private string _BaseDamageValue;
        private string LastEvaluationResult;

        public CriticalBaseDamageEffect()
            : base(Name, true)
        {
        }

        public CriticalBaseDamageEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _NumberType = (Operators.NumberTypes)BR.ReadByte();
            _BaseDamageValue = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_NumberType);
            BW.Write(_BaseDamageValue);
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = FormulaParser.ActiveParser.Evaluate(_BaseDamageValue);
            LastEvaluationResult = EvaluationResult;

            string ExtraText = "";
            if (EvaluationResult != _BaseDamageValue)
            {
                ExtraText = " (" + _BaseDamageValue + ")";
            }

            if (NumberType == Operators.NumberTypes.Absolute)
            {
                int EvaluationValue = (int)double.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                Params.LocalContext.EffectTargetUnit.Boosts.CriticalBaseDamageModifier = EvaluationValue;
                return "Will deal " + EvaluationValue + ExtraText + " extra damage";
            }
            else if (NumberType == Operators.NumberTypes.Relative)
            {
                float EvaluationValue = float.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                Params.LocalContext.EffectTargetUnit.Boosts.CriticalBaseDamageMultiplier = EvaluationValue;
                return "Will deal " + EvaluationValue * 100 + ExtraText + "% damage";
            }

            return "Will deal " + _BaseDamageValue + ExtraText + " extra damage";
        }

        protected override void ReactivateEffect()
        {
            if (NumberType == Operators.NumberTypes.Absolute)
            {
                int EvaluationValue = (int)double.Parse(LastEvaluationResult, CultureInfo.InvariantCulture);
                Params.LocalContext.EffectTargetUnit.Boosts.CriticalBaseDamageModifier = EvaluationValue;
            }
            else if (NumberType == Operators.NumberTypes.Relative)
            {
                float EvaluationValue = float.Parse(LastEvaluationResult, CultureInfo.InvariantCulture);
                Params.LocalContext.EffectTargetUnit.Boosts.CriticalBaseDamageMultiplier = EvaluationValue;
            }
        }

        protected override BaseEffect DoCopy()
        {
            CriticalBaseDamageEffect NewEffect = new CriticalBaseDamageEffect(Params);

            NewEffect._NumberType = _NumberType;
            NewEffect._BaseDamageValue = _BaseDamageValue;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            CriticalBaseDamageEffect NewEffect = (CriticalBaseDamageEffect)Copy;

            _NumberType = NewEffect._NumberType;
            _BaseDamageValue = NewEffect._BaseDamageValue;
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
        public string BaseDamageValue
        {
            get { return _BaseDamageValue; }
            set { _BaseDamageValue = value; }
        }

        #endregion
    }
}
