﻿using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class AttackRangeEffect : SkillEffect
    {
        public static string Name = "Attack Range Effect";

        private string _RangeValue;
        private int LastEvaluationResult;

        public AttackRangeEffect()
            : base(Name, true)
        {
        }

        public AttackRangeEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _RangeValue = BR.ReadString();
        }

        protected override void DoQuickLoad(BinaryReader BR, FormulaParser ActiveParser)
        {
            base.DoQuickLoad(BR, ActiveParser);

            LastEvaluationResult = BR.ReadInt32();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_RangeValue);
        }

        protected override void DoQuickSave(BinaryWriter BW)
        {
            base.DoQuickSave(BW);

            BW.Write(LastEvaluationResult);
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = Params.GlobalContext.ActiveParser.Evaluate(_RangeValue);
            int EvaluationValue = (int)double.Parse(Params.GlobalContext.ActiveParser.Evaluate(_RangeValue), CultureInfo.InvariantCulture);
            LastEvaluationResult = EvaluationValue;

            string ExtraText = "";
            if (EvaluationResult != _RangeValue)
            {
                ExtraText = " (" + _RangeValue + ")";
            }

            Params.LocalContext.EffectTargetUnit.Boosts.RangeModifier = EvaluationValue;

            return "Attack range increased by " + EvaluationValue + ExtraText;
        }

        protected override void ReactivateEffect()
        {
            Params.LocalContext.EffectTargetUnit.Boosts.RangeModifier = LastEvaluationResult;
        }

        protected override BaseEffect DoCopy()
        {
            AttackRangeEffect NewEffect = new AttackRangeEffect(Params);

            NewEffect._RangeValue = _RangeValue;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            AttackRangeEffect NewEffect = (AttackRangeEffect)Copy;

            _RangeValue = NewEffect._RangeValue;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public string Value
        {
            get { return _RangeValue; }
            set { _RangeValue = value; }
        }

        #endregion
    }
}
