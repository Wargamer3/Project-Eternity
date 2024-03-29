﻿using System;
using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class HPRegenEffect : SkillEffect
    {
        public static string Name = "HP Regen Effect";

        private Operators.NumberTypes _NumberType;
        private string _HPRegenValue;

        public HPRegenEffect()
            : base(Name, false)
        {
        }

        public HPRegenEffect(UnitEffectParams Params)
            : base(Name, false, Params)
        {
        }
        
        protected override void Load(BinaryReader BR)
        {
            _NumberType = (Operators.NumberTypes)BR.ReadByte();
            _HPRegenValue = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_NumberType);
            BW.Write(_HPRegenValue);
        }

        public override bool CanActivate()
        {
            return Params.GlobalContext.EffectTargetUnit.HP < Params.GlobalContext.EffectTargetUnit.MaxHP;
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = Params.GlobalContext.ActiveParser.Evaluate(_HPRegenValue);

            string ExtraText = string.Empty;

            if (EvaluationResult != _HPRegenValue)
            {
                ExtraText = " (" + _HPRegenValue + ")";
            }

            int EvaluationValue = 0;
            if (_NumberType == Operators.NumberTypes.Absolute)
            {
                EvaluationValue = (int)double.Parse(EvaluationResult, CultureInfo.InvariantCulture);
            }
            else if (_NumberType == Operators.NumberTypes.Relative)
            {
                EvaluationValue = (int)Math.Round(Params.LocalContext.EffectTargetUnit.MaxHP * float.Parse(EvaluationResult, CultureInfo.InvariantCulture));
            }

            Params.LocalContext.EffectTargetUnit.HealUnit(EvaluationValue);

            return "HP regenerated by " + EvaluationValue + ExtraText;
        }

        protected override void ReactivateEffect()
        {
            //Don't heal on reactivation
        }

        protected override BaseEffect DoCopy()
        {
            HPRegenEffect NewEffect = new HPRegenEffect(Params);

            NewEffect._NumberType = _NumberType;
            NewEffect._HPRegenValue = _HPRegenValue;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            HPRegenEffect NewEffect = (HPRegenEffect)Copy;

            _NumberType = NewEffect._NumberType;
            _HPRegenValue = NewEffect._HPRegenValue;
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
        public string HPRegenValue
        {
            get { return _HPRegenValue; }
            set { _HPRegenValue = value; }
        }

        #endregion
    }
}
