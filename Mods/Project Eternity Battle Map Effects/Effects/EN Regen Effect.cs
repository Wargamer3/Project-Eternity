﻿using System;
using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class ENRegenEffect : SkillEffect
    {
        public static string Name = "EN Regen Effect";

        private Operators.NumberTypes _NumberType;
        private string _ENRegenValue;

        public ENRegenEffect()
            : base(Name, false)
        {
        }

        public ENRegenEffect(UnitEffectParams Params)
            : base(Name, false, Params)
        {
        }
        
        protected override void Load(BinaryReader BR)
        {
            _NumberType = (Operators.NumberTypes)BR.ReadByte();
            _ENRegenValue = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_NumberType);
            BW.Write(_ENRegenValue);
        }

        public override bool CanActivate()
        {
            return Params.GlobalContext.EffectTargetUnit.EN < Params.GlobalContext.EffectTargetUnit.MaxEN;
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = Params.GlobalContext.ActiveParser.Evaluate(_ENRegenValue);

            string ExtraText = string.Empty;

            if (EvaluationResult != _ENRegenValue)
            {
                ExtraText = " (" + _ENRegenValue + ")";
            }

            int EvaluationValue = 0;
            if (_NumberType == Operators.NumberTypes.Absolute)
            {
                EvaluationValue = (int)double.Parse(EvaluationResult, CultureInfo.InvariantCulture);
            }
            else if (_NumberType == Operators.NumberTypes.Relative)
            {
                EvaluationValue = (int)Math.Round(Params.LocalContext.EffectTargetUnit.MaxEN * float.Parse(EvaluationResult, CultureInfo.InvariantCulture));
            }

            Params.LocalContext.EffectTargetUnit.RefillEN(EvaluationValue);

            return "EN regenerated by " + EvaluationValue + ExtraText;
        }

        protected override void ReactivateEffect()
        {
            //Don't regen EN on reactivation
        }

        protected override BaseEffect DoCopy()
        {
            ENRegenEffect NewEffect = new ENRegenEffect(Params);

            NewEffect._NumberType = _NumberType;
            NewEffect._ENRegenValue = _ENRegenValue;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            ENRegenEffect NewEffect = (ENRegenEffect)Copy;

            _NumberType = NewEffect._NumberType;
            _ENRegenValue = NewEffect._ENRegenValue;
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
        public string ENRegenValue
        {
            get { return _ENRegenValue; }
            set { _ENRegenValue = value; }
        }

        #endregion
    }
}
