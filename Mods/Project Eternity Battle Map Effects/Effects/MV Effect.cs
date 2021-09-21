using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class MVEffect : SkillEffect
    {
        public static string Name = "MV Effect";

        private string _MVValue;
        private int LastEvaluationResult;

        public MVEffect()
            : base(Name, true)
        {
        }

        public MVEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _MVValue = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_MVValue);
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = FormulaParser.ActiveParser.Evaluate(_MVValue);
            int EvaluationValue = (int)double.Parse(EvaluationResult, CultureInfo.InvariantCulture);
            LastEvaluationResult = EvaluationValue;

            Params.LocalContext.EffectTargetUnit.Boosts.MovementModifier = EvaluationValue;

            if (EvaluationResult != _MVValue)
            {
                return "MV increased by " + EvaluationValue + " (" + _MVValue + ")";
            }

            return "MV increased by " + _MVValue;
        }

        protected override void ReactivateEffect()
        {
            Params.LocalContext.EffectTargetUnit.Boosts.MovementModifier = LastEvaluationResult;
        }

        protected override BaseEffect DoCopy()
        {
            MVEffect NewEffect = new MVEffect(Params);

            NewEffect._MVValue = _MVValue;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            MVEffect NewEffect = (MVEffect)Copy;

            _MVValue = NewEffect._MVValue;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public string Value
        {
            get { return _MVValue; }
            set { _MVValue = value; }
        }

        #endregion
    }
}
