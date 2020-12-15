using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class HitRateEffect : SkillEffect
    {
        public static string Name = "Hit Rate Effect";

        private string _HitRateValue;

        public HitRateEffect()
            : base(Name, true)
        {
        }

        public HitRateEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _HitRateValue = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_HitRateValue);
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = FormulaParser.ActiveParser.Evaluate(_HitRateValue);
            int EvaluationValue = (int)double.Parse(EvaluationResult, CultureInfo.InvariantCulture);

            Params.LocalContext.EffectTargetUnit.Boosts.AccuracyModifier += EvaluationValue;

            if (EvaluationResult != _HitRateValue)
            {
                return "Hit rate increased by " + EvaluationValue + " (" + _HitRateValue + ")";
            }

            return "Hit rate increased by " + _HitRateValue;
        }

        protected override BaseEffect DoCopy()
        {
            HitRateEffect NewEffect = new HitRateEffect(Params);

            NewEffect._HitRateValue = _HitRateValue;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            HitRateEffect NewEffect = (HitRateEffect)Copy;

            _HitRateValue = NewEffect._HitRateValue;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public string Value
        {
            get { return _HitRateValue; }
            set { _HitRateValue = value; }
        }

        #endregion
    }
}
