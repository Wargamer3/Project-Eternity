using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class UnitStatEffect : SkillEffect
    {
        public static string Name = "Unit Stat Effect";

        private UnitStats _UnitStat;
        private Operators.NumberTypes _NumberType;
        private string _Value;
        private string LastEvaluationResult;

        public UnitStatEffect()
            : base(Name, true)
        {
            _Value = "0";
        }

        public UnitStatEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
            _Value = "0";
        }

        protected override void Load(BinaryReader BR)
        {
            _UnitStat = (UnitStats)BR.ReadByte();
            _NumberType = (Operators.NumberTypes)BR.ReadByte();
            _Value = BR.ReadString();
        }

        protected override void DoQuickLoad(BinaryReader BR, FormulaParser ActiveParser)
        {
            base.DoQuickLoad(BR, ActiveParser);

            LastEvaluationResult = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_UnitStat);
            BW.Write((byte)_NumberType);
            BW.Write(_Value);
        }

        protected override void DoQuickSave(BinaryWriter BW)
        {
            base.DoQuickSave(BW);

            BW.Write(LastEvaluationResult);
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = Params.GlobalContext.ActiveParser.Evaluate(_Value);
            LastEvaluationResult = EvaluationResult;
            string Extra = "";
            if (EvaluationResult != _Value)
            {
                Extra = " (" + _Value + ")";
            }

            if (NumberType == Operators.NumberTypes.Absolute)
            {
                int EvaluationValue = (int)double.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                switch (_UnitStat)
                {
                    case UnitStats.MaxHP:
                        Params.LocalContext.EffectTargetUnit.Boosts.HPMaxModifier += EvaluationValue;
                        return "Map HP increased by " + EvaluationValue + Extra;

                    case UnitStats.MaxEN:
                        Params.LocalContext.EffectTargetUnit.Boosts.ENMaxModifier += EvaluationValue;
                        return "Map EN increased by " + EvaluationValue + Extra;

                    case UnitStats.Armor:
                        Params.LocalContext.EffectTargetUnit.Boosts.ArmorModifier += EvaluationValue;
                        return "Armored increased by " + EvaluationValue + Extra;

                    case UnitStats.Mobility:
                        Params.LocalContext.EffectTargetUnit.Boosts.MobilityModifier += EvaluationValue;
                        return "Mobility increased by " + EvaluationValue + Extra;

                    case UnitStats.MaxMV:
                        Params.LocalContext.EffectTargetUnit.Boosts.MVMaxModifier += EvaluationValue;
                        return "Max MV increased by " + EvaluationValue + Extra;
                }
            }
            else if (NumberType == Operators.NumberTypes.Relative)
            {
                float EvaluationValue = float.Parse(EvaluationResult, CultureInfo.InvariantCulture);

                switch (_UnitStat)
                {
                    case UnitStats.MaxHP:
                        Params.LocalContext.EffectTargetUnit.Boosts.HPMaxModifier += (int)(Params.LocalContext.EffectTargetUnit.MaxHP * EvaluationValue);
                        return "Map HP increased by " + (int)(Params.LocalContext.EffectTargetUnit.MaxHP * EvaluationValue) + Extra;

                    case UnitStats.MaxEN:
                        Params.LocalContext.EffectTargetUnit.Boosts.ENMaxModifier += (int)(Params.LocalContext.EffectTargetUnit.MaxEN * EvaluationValue);
                        return "Map EN increased by " + (int)(Params.LocalContext.EffectTargetUnit.MaxEN * EvaluationValue) + Extra;

                    case UnitStats.Armor:
                        Params.LocalContext.EffectTargetUnit.Boosts.ArmorModifier += (int)(Params.LocalContext.EffectTargetUnit.Armor * EvaluationValue);
                        return "Armor increased by " + (int)(Params.LocalContext.EffectTargetUnit.Armor * EvaluationValue) + Extra;

                    case UnitStats.Mobility:
                        Params.LocalContext.EffectTargetUnit.Boosts.MobilityModifier += (int)(Params.LocalContext.EffectTargetUnit.Mobility * EvaluationValue);
                        return "Mobility increased by " + (int)(Params.LocalContext.EffectTargetUnit.Mobility * EvaluationValue) + Extra;

                    case UnitStats.MaxMV:
                        Params.LocalContext.EffectTargetUnit.Boosts.MVMaxModifier += (int)(Params.LocalContext.EffectTargetUnit.MaxMovement * EvaluationValue);
                        return "Max MV increased by " + (int)(Params.LocalContext.EffectTargetUnit.MaxMovement * EvaluationValue) + Extra;
                }
            }

            return _Value;
        }

        protected override void ReactivateEffect()
        {
            if (NumberType == Operators.NumberTypes.Absolute)
            {
                int EvaluationValue = (int)double.Parse(LastEvaluationResult, CultureInfo.InvariantCulture);
                switch (_UnitStat)
                {
                    case UnitStats.MaxHP:
                        Params.LocalContext.EffectTargetUnit.Boosts.HPMaxModifier += EvaluationValue;
                        break;

                    case UnitStats.MaxEN:
                        Params.LocalContext.EffectTargetUnit.Boosts.ENMaxModifier += EvaluationValue;
                        break;

                    case UnitStats.Armor:
                        Params.LocalContext.EffectTargetUnit.Boosts.ArmorModifier += EvaluationValue;
                        break;

                    case UnitStats.Mobility:
                        Params.LocalContext.EffectTargetUnit.Boosts.MobilityModifier += EvaluationValue;
                        break;

                    case UnitStats.MaxMV:
                        Params.LocalContext.EffectTargetUnit.Boosts.MVMaxModifier += EvaluationValue;
                        break;
                }
            }
            else if (NumberType == Operators.NumberTypes.Relative)
            {
                float EvaluationValue = float.Parse(LastEvaluationResult, CultureInfo.InvariantCulture);

                switch (_UnitStat)
                {
                    case UnitStats.MaxHP:
                        Params.LocalContext.EffectTargetUnit.Boosts.HPMaxModifier += (int)(Params.LocalContext.EffectTargetUnit.MaxHP * EvaluationValue);
                        break;

                    case UnitStats.MaxEN:
                        Params.LocalContext.EffectTargetUnit.Boosts.ENMaxModifier += (int)(Params.LocalContext.EffectTargetUnit.MaxEN * EvaluationValue);
                        break;

                    case UnitStats.Armor:
                        Params.LocalContext.EffectTargetUnit.Boosts.ArmorModifier += (int)(Params.LocalContext.EffectTargetUnit.Armor * EvaluationValue);
                        break;

                    case UnitStats.Mobility:
                        Params.LocalContext.EffectTargetUnit.Boosts.MobilityModifier += (int)(Params.LocalContext.EffectTargetUnit.Mobility * EvaluationValue);
                        break;

                    case UnitStats.MaxMV:
                        Params.LocalContext.EffectTargetUnit.Boosts.MVMaxModifier += (int)(Params.LocalContext.EffectTargetUnit.MaxMovement * EvaluationValue);
                        break;
                }
            }
        }

        protected override BaseEffect DoCopy()
        {
            UnitStatEffect NewEffect = new UnitStatEffect(Params);

            NewEffect._UnitStat = _UnitStat;
            NewEffect._NumberType = _NumberType;
            NewEffect._Value = _Value;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            UnitStatEffect NewEffect = (UnitStatEffect)Copy;

            _UnitStat = NewEffect._UnitStat;
            _NumberType = NewEffect._NumberType;
            _Value = NewEffect._Value;
        }

        #region Properties

        [TypeConverter(typeof(UnitStatsConverter)),
        CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public Core.Effects.UnitStats UnitStat
        {
            get { return _UnitStat; }
            set { _UnitStat = value; }
        }

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
            get { return _Value; }
            set { _Value = value; }
        }

        #endregion
    }
}
