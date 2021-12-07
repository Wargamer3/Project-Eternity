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
        private int LastEvaluationResult;

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

        protected override void DoQuickLoad(BinaryReader BR, FormulaParser ActiveParser)
        {
            base.DoQuickLoad(BR, ActiveParser);

            LastEvaluationResult = BR.ReadInt32();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_LimitBreakValue);
        }

        protected override void DoQuickSave(BinaryWriter BW)
        {
            base.DoQuickSave(BW);

            BW.Write(LastEvaluationResult);
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = Params.GlobalContext.ActiveParser.Evaluate(_LimitBreakValue);
            LastEvaluationResult = (int)double.Parse(EvaluationResult, CultureInfo.InvariantCulture);
            Params.LocalContext.EffectTargetCharacter.MaxWill += LastEvaluationResult;

            if (EvaluationResult != _LimitBreakValue)
                return "Max Will increased by " + (int)double.Parse(EvaluationResult, CultureInfo.InvariantCulture) + " (" + _LimitBreakValue + ")";
            else
                return "Max Will increased by " + _LimitBreakValue;
        }

        protected override void ReactivateEffect()
        {
            Params.LocalContext.EffectTargetCharacter.MaxWill += LastEvaluationResult;
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
