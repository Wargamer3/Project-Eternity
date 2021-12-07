using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class ENCostEffect : SkillEffect
    {
        public static string Name = "EN Cost Effect";

        private string _ENCostValue;
        private int LastEvaluationResult;

        public ENCostEffect()
            : base(Name, true)
        {
        }

        public ENCostEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _ENCostValue = BR.ReadString();
        }

        protected override void DoQuickLoad(BinaryReader BR, FormulaParser ActiveParser)
        {
            base.DoQuickLoad(BR, ActiveParser);

            LastEvaluationResult = BR.ReadInt32();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_ENCostValue);
        }

        protected override void DoQuickSave(BinaryWriter BW)
        {
            base.DoQuickSave(BW);

            BW.Write(LastEvaluationResult);
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = Params.GlobalContext.ActiveParser.Evaluate(_ENCostValue);
            int EvaluationValue = (int)double.Parse(Params.GlobalContext.ActiveParser.Evaluate(_ENCostValue), CultureInfo.InvariantCulture);
            LastEvaluationResult = EvaluationValue;

            Params.LocalContext.EffectTargetUnit.Boosts.ENCostModifier = EvaluationValue;

            if (EvaluationResult != _ENCostValue)
            {
                return "EN cost changed by " + EvaluationValue + " (" + _ENCostValue + ")";
            }

            return "EN cost changed by " + _ENCostValue;
        }

        protected override void ReactivateEffect()
        {
            Params.LocalContext.EffectTargetUnit.Boosts.ENCostModifier = LastEvaluationResult;
        }

        protected override BaseEffect DoCopy()
        {
            ENCostEffect NewEffect = new ENCostEffect(Params);

            NewEffect._ENCostValue = _ENCostValue;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            ENCostEffect NewEffect = (ENCostEffect)Copy;

            _ENCostValue = NewEffect._ENCostValue;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public string Value
        {
            get { return _ENCostValue; }
            set { _ENCostValue = value; }
        }

        #endregion
    }
}
