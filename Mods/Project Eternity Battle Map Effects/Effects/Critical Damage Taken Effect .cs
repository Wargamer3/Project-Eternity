using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class CriticalDamageTakenEffect : SkillEffect
    {
        public static string Name = "Critical Damage Taken Effect";

        private Operators.NumberTypes _NumberType;
        private string _BaseDamageValue;
        private string LastEvaluationResult;

        public CriticalDamageTakenEffect()
            : base(Name, true)
        {
        }

        public CriticalDamageTakenEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _NumberType = (Operators.NumberTypes)BR.ReadByte();
            _BaseDamageValue = BR.ReadString();
        }

        protected override void DoQuickLoad(BinaryReader BR, FormulaParser ActiveParser)
        {
            base.DoQuickLoad(BR, ActiveParser);

            LastEvaluationResult = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_NumberType);
            BW.Write(_BaseDamageValue);
        }

        protected override void DoQuickSave(BinaryWriter BW)
        {
            base.DoQuickSave(BW);

            BW.Write(LastEvaluationResult);
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = Params.GlobalContext.ActiveParser.Evaluate(_BaseDamageValue);
            LastEvaluationResult = EvaluationResult;

            string ExtraText = "";
            if (EvaluationResult != _BaseDamageValue)
            {
                ExtraText = " (" + _BaseDamageValue + ")";
            }

            if (NumberType == Operators.NumberTypes.Absolute)
            {
                int EvaluationValue = (int)double.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                Params.LocalContext.EffectTargetUnit.Boosts.CriticalDamageTakenFixedModifier = EvaluationValue;
                return "Will receive " + EvaluationValue + ExtraText + " less damage";
            }
            else if (NumberType == Operators.NumberTypes.Relative)
            {
                float EvaluationValue = float.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                Params.LocalContext.EffectTargetUnit.Boosts.CriticalDamageTakenReductionMultiplier = EvaluationValue;
                return "Will receive " + EvaluationValue * 100 + ExtraText + "% less damage";
            }

            return "Will deal " + _BaseDamageValue + ExtraText + " extra damage";
        }

        protected override void ReactivateEffect()
        {
            if (NumberType == Operators.NumberTypes.Absolute)
            {
                int EvaluationValue = (int)double.Parse(LastEvaluationResult, CultureInfo.InvariantCulture);
                Params.LocalContext.EffectTargetUnit.Boosts.CriticalDamageTakenFixedModifier = EvaluationValue;
            }
            else if (NumberType == Operators.NumberTypes.Relative)
            {
                float EvaluationValue = float.Parse(LastEvaluationResult, CultureInfo.InvariantCulture);
                Params.LocalContext.EffectTargetUnit.Boosts.CriticalDamageTakenReductionMultiplier = EvaluationValue;
            }
        }

        protected override BaseEffect DoCopy()
        {
            CriticalDamageTakenEffect NewEffect = new CriticalDamageTakenEffect(Params);

            NewEffect._NumberType = _NumberType;
            NewEffect._BaseDamageValue = _BaseDamageValue;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            CriticalDamageTakenEffect NewEffect = (CriticalDamageTakenEffect)Copy;

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
