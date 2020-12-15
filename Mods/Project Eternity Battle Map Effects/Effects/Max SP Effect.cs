using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class MaxSPEffect : SkillEffect
    {
        public static string Name = "Max SP Effect";

        private Operators.NumberTypes _NumberType;
        private string _MaxSPValue;

        public MaxSPEffect()
            : base(Name, true)
        {
        }

        public MaxSPEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _NumberType = (Operators.NumberTypes)BR.ReadByte();
            _MaxSPValue = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_NumberType);
            BW.Write(_MaxSPValue);
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = FormulaParser.ActiveParser.Evaluate(_MaxSPValue);

            string Extra = string.Empty;

            if (EvaluationResult != _MaxSPValue)
            {
                Extra = " (" + _MaxSPValue + ")";
            }

            int EvaluationValue = 0;

            if (NumberType == Operators.NumberTypes.Absolute)
            {
                EvaluationValue = (int)double.Parse(EvaluationResult, CultureInfo.InvariantCulture);
            }
            else if (NumberType == Operators.NumberTypes.Relative)
            {
                EvaluationValue = (int)(Params.LocalContext.EffectTargetCharacter.MaxSP * float.Parse(EvaluationResult, CultureInfo.InvariantCulture));
            }

            Params.LocalContext.EffectTargetCharacter.BonusMaxSP += EvaluationValue;

            return "Max SP increased by " + EvaluationValue + Extra;
        }

        protected override BaseEffect DoCopy()
        {
            MaxSPEffect NewEffect = new MaxSPEffect(Params);

            NewEffect._NumberType = _NumberType;
            NewEffect._MaxSPValue = _MaxSPValue;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            MaxSPEffect NewEffect = (MaxSPEffect)Copy;

            _NumberType = NewEffect._NumberType;
            _MaxSPValue = NewEffect._MaxSPValue;
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
        public string MaxSPValue
        {
            get { return _MaxSPValue; }
            set { _MaxSPValue = value; }
        }

        #endregion
    }
}
