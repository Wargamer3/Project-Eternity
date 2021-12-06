using System;
using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class SupportAttackEffect : SkillEffect
    {
        public static string Name = "Support Attack Effect";

        private string _SupportAttackValue;
        private int LastEvaluationResult;

        public SupportAttackEffect()
            : base(Name, true)
        {
        }

        public SupportAttackEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _SupportAttackValue = BR.ReadString();
        }

        protected override void DoQuickLoad(BinaryReader BR)
        {
            base.DoQuickLoad(BR);

            LastEvaluationResult = BR.ReadInt32();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_SupportAttackValue);
        }

        protected override void DoQuickSave(BinaryWriter BW)
        {
            base.DoQuickSave(BW);

            BW.Write(LastEvaluationResult);
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = FormulaParser.ActiveParser.Evaluate(_SupportAttackValue);
            int FinalySupportAttackValue = (int)double.Parse(EvaluationResult, CultureInfo.InvariantCulture);
            LastEvaluationResult = FinalySupportAttackValue;

            Params.LocalContext.EffectTargetUnit.Boosts.SupportAttackModifierMax += FinalySupportAttackValue;
            Params.LocalContext.EffectTargetUnit.Boosts.SupportAttackModifier =
                Math.Min(Params.LocalContext.EffectTargetUnit.Boosts.SupportAttackModifierMax, Params.LocalContext.EffectTargetUnit.Boosts.SupportAttackModifier + FinalySupportAttackValue);

            if (EvaluationResult != _SupportAttackValue)
            {
                return "Support attack can be used " + FinalySupportAttackValue + " (" + _SupportAttackValue + ") more times";
            }

            return "Support attack can be used " + _SupportAttackValue + " more times";
        }

        protected override void ReactivateEffect()
        {
            Params.LocalContext.EffectTargetUnit.Boosts.SupportAttackModifierMax += LastEvaluationResult;
        }

        protected override BaseEffect DoCopy()
        {
            SupportAttackEffect NewEffect = new SupportAttackEffect(Params);

            NewEffect._SupportAttackValue = _SupportAttackValue;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            SupportAttackEffect NewEffect = (SupportAttackEffect)Copy;

            _SupportAttackValue = NewEffect._SupportAttackValue;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public string Value
        {
            get { return _SupportAttackValue; }
            set { _SupportAttackValue = value; }
        }

        #endregion
    }
}
