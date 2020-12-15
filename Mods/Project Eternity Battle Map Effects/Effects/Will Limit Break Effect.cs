using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class WillLimitBreakEffect : SkillEffect
    {
        public static string Name = "Limit Break Effect";

        private string _LimitBreakValue;

        public WillLimitBreakEffect()
            : base(Name, true)
        {
        }

        public WillLimitBreakEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _LimitBreakValue = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_LimitBreakValue);
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = FormulaParser.ActiveParser.Evaluate(_LimitBreakValue);
            Params.LocalContext.EffectTargetCharacter.MaxWill += (int)double.Parse(EvaluationResult, CultureInfo.InvariantCulture);

            if (EvaluationResult != _LimitBreakValue)
                return "Max Will increased by " + (int)double.Parse(FormulaParser.ActiveParser.Evaluate(_LimitBreakValue), CultureInfo.InvariantCulture) + " (" + _LimitBreakValue + ")";
            else
                return "Max Will increased by " + _LimitBreakValue;
        }

        protected override BaseEffect DoCopy()
        {
            WillLimitBreakEffect NewEffect = new WillLimitBreakEffect(Params);

            NewEffect._LimitBreakValue = _LimitBreakValue;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            WillLimitBreakEffect NewEffect = (WillLimitBreakEffect)Copy;

            _LimitBreakValue = NewEffect._LimitBreakValue;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public string Value
        {
            get { return _LimitBreakValue; }
            set { _LimitBreakValue = value; }
        }

        #endregion
    }
}
