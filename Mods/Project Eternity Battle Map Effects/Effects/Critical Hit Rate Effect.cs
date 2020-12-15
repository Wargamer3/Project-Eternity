using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class CriticalHitRateEffect : SkillEffect
    {
        public static string Name = "Critical Hit Rate Effect";

        private string _CriticalHitRateValue;

        public CriticalHitRateEffect()
            : base(Name, true)
        {
        }

        public CriticalHitRateEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _CriticalHitRateValue = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_CriticalHitRateValue);
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = FormulaParser.ActiveParser.Evaluate(_CriticalHitRateValue);
            int EvaluationValue = (int)double.Parse(EvaluationResult, CultureInfo.InvariantCulture);
            string ExtraText = string.Empty;

            if (EvaluationResult != _CriticalHitRateValue)
            {
                ExtraText = " (" + _CriticalHitRateValue + ")";
            }

            Params.LocalContext.EffectTargetUnit.Boosts.CriticalHitRateModifier = EvaluationValue;

            return "Critical hit chances increased by " + EvaluationValue + ExtraText + "%";
        }

        protected override BaseEffect DoCopy()
        {
            CriticalHitRateEffect NewEffect = new CriticalHitRateEffect(Params);

            NewEffect._CriticalHitRateValue = _CriticalHitRateValue;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            CriticalHitRateEffect NewEffect = (CriticalHitRateEffect)Copy;

            _CriticalHitRateValue = NewEffect._CriticalHitRateValue;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public string Value
        {
            get { return _CriticalHitRateValue; }
            set { _CriticalHitRateValue = value; }
        }

        #endregion
    }
}
