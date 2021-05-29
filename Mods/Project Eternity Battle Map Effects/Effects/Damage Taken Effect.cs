﻿using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class DamageTakenEffect : SkillEffect
    {
        public static string Name = "Damage Taken Effect";

        private Operators.NumberTypes _NumberType;
        private string _DamageTakenValue;

        public DamageTakenEffect()
            : base(Name, true)
        {
            _DamageTakenValue = string.Empty;
        }

        public DamageTakenEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
            _DamageTakenValue = string.Empty;
        }

        protected override void Load(BinaryReader BR)
        {
            _NumberType = (Operators.NumberTypes)BR.ReadByte();
            _DamageTakenValue = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_NumberType);
            BW.Write(_DamageTakenValue);
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult;

            try
            {
                EvaluationResult = FormulaParser.ActiveParser.Evaluate(_DamageTakenValue);
            }
            catch
            {
                return string.Empty;
            }

            string ExtraText = string.Empty;
            if (EvaluationResult != _DamageTakenValue)
            {
                ExtraText = " (" + _DamageTakenValue + ")";
            }

            if (_NumberType == Operators.NumberTypes.Absolute)
            {
                int EvaluationValue = (int)double.Parse(EvaluationResult, CultureInfo.InvariantCulture);

                Params.LocalContext.EffectTargetUnit.Boosts.FinalDamageTakenFixedModifier = EvaluationValue;

                return "Damage taken increased by " + EvaluationValue + ExtraText;
            }
            else if (_NumberType == Operators.NumberTypes.Relative)
            {
                float EvaluationValue = float.Parse(EvaluationResult, CultureInfo.InvariantCulture);

                Params.LocalContext.EffectTargetUnit.Boosts.BaseDamageTakenReductionMultiplier = EvaluationValue;

                return "Damage taken will deal " + EvaluationValue * 100 + ExtraText + "% damage";
            }

            return "Damage taken increased by " + EvaluationResult;
        }

        protected override BaseEffect DoCopy()
        {
            DamageTakenEffect NewEffect = new DamageTakenEffect(Params);

            NewEffect._NumberType = _NumberType;
            NewEffect._DamageTakenValue = _DamageTakenValue;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            DamageTakenEffect NewEffect = (DamageTakenEffect)Copy;

            _NumberType = NewEffect._NumberType;
            _DamageTakenValue = NewEffect._DamageTakenValue;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public Operators.NumberTypes NumberType
        {
            get { return _NumberType; }
            set { _NumberType = value; }
        }

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public string Value
        {
            get { return _DamageTakenValue; }
            set { _DamageTakenValue = value; }
        }

        #endregion
    }
}
