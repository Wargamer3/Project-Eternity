using System;
using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class SupportDefendEffect : SkillEffect
    {
        public static string Name = "Support Defend Effect";

        private string _SupportDefendValue;
        private int LastEvaluationResult;

        public SupportDefendEffect()
            : base(Name, true)
        {
        }

        public SupportDefendEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _SupportDefendValue = BR.ReadString();
        }

        protected override void DoQuickLoad(BinaryReader BR)
        {
            base.DoQuickLoad(BR);

            LastEvaluationResult = BR.ReadInt32();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_SupportDefendValue);
        }

        protected override void DoQuickSave(BinaryWriter BW)
        {
            base.DoQuickSave(BW);

            BW.Write(LastEvaluationResult);
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = FormulaParser.ActiveParser.Evaluate(_SupportDefendValue);
            int FinalSupportDefendValue = (int)double.Parse(EvaluationResult, CultureInfo.InvariantCulture);
            LastEvaluationResult = FinalSupportDefendValue;
            Params.LocalContext.EffectTargetUnit.Boosts.SupportDefendModifierMax += FinalSupportDefendValue;
            Params.LocalContext.EffectTargetUnit.Boosts.SupportDefendModifier =
                Math.Min(Params.LocalContext.EffectTargetUnit.Boosts.SupportDefendModifierMax, Params.LocalContext.EffectTargetUnit.Boosts.SupportDefendModifier + FinalSupportDefendValue);

            if (EvaluationResult != _SupportDefendValue)
            {
                return "Support defend can be used " + FinalSupportDefendValue + " (" + _SupportDefendValue + ") more times";
            }

            return "Support defend can be used " + _SupportDefendValue + " more times";
        }

        protected override void ReactivateEffect()
        {
            Params.LocalContext.EffectTargetUnit.Boosts.SupportDefendModifierMax += LastEvaluationResult;
        }

        protected override BaseEffect DoCopy()
        {
            SupportDefendEffect NewEffect = new SupportDefendEffect(Params);

            NewEffect._SupportDefendValue = _SupportDefendValue;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            SupportDefendEffect NewEffect = (SupportDefendEffect)Copy;

            _SupportDefendValue = NewEffect._SupportDefendValue;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public string Value
        {
            get { return _SupportDefendValue; }
            set { _SupportDefendValue = value; }
        }

        #endregion
    }
}
