using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class EvasionRateEffect : SkillEffect
    {
        public static string Name = "Evasion Rate Effect";

        private string _EvasionRateValue;
        private int LastEvaluationResult;

        public EvasionRateEffect()
            : base(Name, true)
        {
        }

        public EvasionRateEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
        }
        
        protected override void Load(BinaryReader BR)
        {
            _EvasionRateValue = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_EvasionRateValue);
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = FormulaParser.ActiveParser.Evaluate(_EvasionRateValue);
            int EvaluationValue = (int)double.Parse(EvaluationResult, CultureInfo.InvariantCulture);
            LastEvaluationResult = EvaluationValue;

            Params.LocalContext.EffectTargetUnit.Boosts.EvasionModifier += EvaluationValue;

            if (EvaluationResult != _EvasionRateValue)
            {
                return "Evasion was raised by " + EvaluationValue + " (" + _EvasionRateValue + ")";
            }

            return "Evasion was raised by " + EvaluationValue;
        }

        protected override void ReactivateEffect()
        {
            Params.LocalContext.EffectTargetUnit.Boosts.EvasionModifier += LastEvaluationResult;
        }

        protected override BaseEffect DoCopy()
        {
            EvasionRateEffect NewEffect = new EvasionRateEffect(Params);

            NewEffect._EvasionRateValue = _EvasionRateValue;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            EvasionRateEffect NewEffect = (EvasionRateEffect)Copy;

            _EvasionRateValue = NewEffect._EvasionRateValue;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public string Value
        {
            get { return _EvasionRateValue; }
            set { _EvasionRateValue = value; }
        }

        #endregion
    }
}
