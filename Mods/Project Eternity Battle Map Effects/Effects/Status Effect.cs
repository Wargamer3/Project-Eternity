using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class StatusEffect : SkillEffect
    {
        public static string Name = "Status Effect";

        private StatusTypes _StatusType;
        private string _EffectValue;
        private int LastEvaluationResult;

        public StatusEffect()
            : base(Name, true)
        {
            _StatusType = StatusTypes.MEL;
        }

        public StatusEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _StatusType = (StatusTypes)BR.ReadByte();
            _EffectValue = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_StatusType);
            BW.Write(_EffectValue);
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = FormulaParser.ActiveParser.Evaluate(_EffectValue);
            int EvaluationValue = (int)double.Parse(EvaluationResult, CultureInfo.InvariantCulture);
            LastEvaluationResult = EvaluationValue;

            string Extra = "";
            if (EvaluationResult != _EffectValue)
            {
                Extra = " (" + _EffectValue + ")";
            }

            switch (StatusType)
            {
                case StatusTypes.MEL:
                    Params.LocalContext.EffectTargetCharacter.BonusMEL += EvaluationValue;
                    return "MEL increase by " + EvaluationValue + Extra;

                case StatusTypes.RNG:
                    Params.LocalContext.EffectTargetCharacter.BonusRNG += EvaluationValue;
                    return "RNG increase by " + EvaluationValue + Extra;

                case StatusTypes.DEF:
                    Params.LocalContext.EffectTargetCharacter.BonusDEF += EvaluationValue;
                    return "DEF increase by " + EvaluationValue + Extra;

                case StatusTypes.SKL:
                    Params.LocalContext.EffectTargetCharacter.BonusSKL += EvaluationValue;
                    return "SKL increase by " + EvaluationValue + Extra;

                case StatusTypes.EVA:
                    Params.LocalContext.EffectTargetCharacter.BonusEVA += EvaluationValue;
                    return "EVA increase by " + EvaluationValue + Extra;

                case StatusTypes.HIT:
                    Params.LocalContext.EffectTargetCharacter.BonusHIT += EvaluationValue;
                    return "HIT increase by " + EvaluationValue + Extra;
            }

            return _EffectValue;
        }

        protected override void ReactivateEffect()
        {
            switch (StatusType)
            {
                case StatusTypes.MEL:
                    Params.LocalContext.EffectTargetCharacter.BonusMEL += LastEvaluationResult;
                    break;

                case StatusTypes.RNG:
                    Params.LocalContext.EffectTargetCharacter.BonusRNG += LastEvaluationResult;
                    break;

                case StatusTypes.DEF:
                    Params.LocalContext.EffectTargetCharacter.BonusDEF += LastEvaluationResult;
                    break;

                case StatusTypes.SKL:
                    Params.LocalContext.EffectTargetCharacter.BonusSKL += LastEvaluationResult;
                    break;

                case StatusTypes.EVA:
                    Params.LocalContext.EffectTargetCharacter.BonusEVA += LastEvaluationResult;
                    break;

                case StatusTypes.HIT:
                    Params.LocalContext.EffectTargetCharacter.BonusHIT += LastEvaluationResult;
                    break;
            }
        }

        protected override BaseEffect DoCopy()
        {
            StatusEffect NewEffect = new StatusEffect(Params);

            NewEffect._EffectValue = _EffectValue;
            NewEffect._StatusType = _StatusType;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            StatusEffect NewEffect = (StatusEffect)Copy;

            _EffectValue = NewEffect._EffectValue;
            _StatusType = NewEffect._StatusType;
        }

        public override bool Equals(object obj)
        {
            if (obj is SkillEffect)
                return EffectTypeName == ((SkillEffect)obj).EffectTypeName && _StatusType == ((StatusEffect)obj)._StatusType;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region Properties

        [TypeConverter(typeof(StatusTypeConverter)),
        CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public StatusTypes StatusType
        {
            get { return _StatusType; }
            set { _StatusType = value; }
        }

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public string Value
        {
            get { return _EffectValue; }
            set { _EffectValue = value; }
        }

        #endregion
    }
}
