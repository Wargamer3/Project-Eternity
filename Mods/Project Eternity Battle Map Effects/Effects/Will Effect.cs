﻿using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class WillEffect : SkillEffect
    {
        public static string Name = "Will Effect";

        private Operators.NumberTypes _NumberType;
        private string _WillValue;

        public WillEffect()
            : base(Name, true)
        {
        }

        public WillEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _NumberType = (Operators.NumberTypes)BR.ReadByte();
            _WillValue = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_NumberType);
            BW.Write(_WillValue);
        }

        public override bool CanActivate()
        {
            return Params.GlobalContext.EffectTargetCharacter.Will < Params.GlobalContext.EffectTargetCharacter.MaxWill;
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = FormulaParser.ActiveParser.Evaluate(_WillValue);
            Character TargetCharacter = Params.LocalContext.EffectTargetCharacter;

            int WillBonus = 0;

            if (NumberType == Operators.NumberTypes.Absolute)
            {
                WillBonus = (int)double.Parse(FormulaParser.ActiveParser.Evaluate(_WillValue), CultureInfo.InvariantCulture);
            }
            else if (NumberType == Operators.NumberTypes.Relative)
            {
                WillBonus = (int)(TargetCharacter.Will * float.Parse(FormulaParser.ActiveParser.Evaluate(_WillValue), CultureInfo.InvariantCulture));
            }

            TargetCharacter.WillBonus += WillBonus;

            if (EvaluationResult != _WillValue)
            {
                return "Will increased by " + WillBonus + " (" + _WillValue + ")";
            }

            return "Will increased by " + _WillValue;
        }

        protected override BaseEffect DoCopy()
        {
            WillEffect NewEffect = new WillEffect(Params);

            NewEffect._NumberType = _NumberType;
            NewEffect._WillValue = _WillValue;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            WillEffect NewEffect = (WillEffect)Copy;

            _NumberType = NewEffect._NumberType;
            _WillValue = NewEffect._WillValue;
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
        public string WillValue
        {
            get { return _WillValue; }
            set { _WillValue = value; }
        }

        #endregion
    }
}
