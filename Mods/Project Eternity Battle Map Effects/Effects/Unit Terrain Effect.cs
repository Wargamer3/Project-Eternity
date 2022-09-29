using System;
using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class UnitTerrainEffect : SkillEffect
    {
        public static string Name = "Unit Terrain Effect";

        private byte _TerrainIndex;
        private Operators.NumberTypes _NumberType;
        private string _Value;
        private bool _CanDowngrade;
        private int LastEvaluationResult;

        public UnitTerrainEffect()
            : base(Name, true)
        {
        }

        public UnitTerrainEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _TerrainIndex = BR.ReadByte();
            _NumberType = (Operators.NumberTypes)BR.ReadByte();
            _Value = BR.ReadString();
            _CanDowngrade = BR.ReadBoolean();
        }

        protected override void DoQuickLoad(BinaryReader BR, FormulaParser ActiveParser)
        {
            base.DoQuickLoad(BR, ActiveParser);

            LastEvaluationResult = BR.ReadInt32();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_TerrainIndex);
            BW.Write((byte)_NumberType);
            BW.Write(_Value);
            BW.Write(_CanDowngrade);
        }

        protected override void DoQuickSave(BinaryWriter BW)
        {
            base.DoQuickSave(BW);

            BW.Write(LastEvaluationResult);
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = Params.GlobalContext.ActiveParser.Evaluate(_Value);
            int FinalValue = (int)double.Parse(EvaluationResult, CultureInfo.InvariantCulture);
            LastEvaluationResult = FinalValue;

            string ExtraText = string.Empty;

            if (EvaluationResult != _Value)
            {
                ExtraText = " (" + _Value + ")";
            }

            if (NumberType == Operators.NumberTypes.Absolute)
            {
                if (_CanDowngrade)
                {
                    Params.LocalContext.EffectTargetUnit.Boosts.DicTerrainLetterAttributeModifier[_TerrainIndex] = FinalValue;
                }
                else if (FinalValue < Params.LocalContext.EffectTargetUnit.DicRankByMovement[_TerrainIndex])
                {
                    Params.LocalContext.EffectTargetUnit.Boosts.DicTerrainLetterAttributeModifier[_TerrainIndex] = Math.Max(Params.LocalContext.EffectTargetUnit.DicRankByMovement[_TerrainIndex], FinalValue);
                }

                return "Absolute - " + _TerrainIndex + " - " + (_CanDowngrade ? "Downgradable - " : "")
                    + Params.LocalContext.EffectTargetUnit.Boosts.DicTerrainLetterAttributeModifier[_TerrainIndex]
                    + ExtraText;
            }
            else if (NumberType == Operators.NumberTypes.Relative)
            {
                Params.LocalContext.EffectTargetUnit.Boosts.DicTerrainLetterAttributeModifier[_TerrainIndex] = Math.Min(Unit.Grades.Length, Math.Max(0, Params.LocalContext.EffectTargetUnit.Boosts.DicTerrainLetterAttributeModifier[_TerrainIndex] + FinalValue));

                return "Absolute - " + _TerrainIndex + " - " + (_CanDowngrade ? "Downgradable - " : "")
                    + Params.LocalContext.EffectTargetUnit.Boosts.DicTerrainLetterAttributeModifier[_TerrainIndex]
                    + ExtraText;
            }

            return _TerrainIndex + " - " + (_CanDowngrade ? "Downgradable - " : "")
                + Params.LocalContext.EffectTargetUnit.Boosts.DicTerrainLetterAttributeModifier[_TerrainIndex]
                + ExtraText;
        }

        protected override void ReactivateEffect()
        {
            if (NumberType == Operators.NumberTypes.Absolute)
            {
                if (_CanDowngrade)
                {
                    Params.LocalContext.EffectTargetUnit.Boosts.DicTerrainLetterAttributeModifier[_TerrainIndex] = LastEvaluationResult;
                }
                else if (LastEvaluationResult < Params.LocalContext.EffectTargetUnit.DicRankByMovement[_TerrainIndex])
                {
                    Params.LocalContext.EffectTargetUnit.Boosts.DicTerrainLetterAttributeModifier[_TerrainIndex] = Math.Max(Params.LocalContext.EffectTargetUnit.DicRankByMovement[_TerrainIndex], LastEvaluationResult);
                }
            }
            else if (NumberType == Operators.NumberTypes.Relative)
            {
                Params.LocalContext.EffectTargetUnit.Boosts.DicTerrainLetterAttributeModifier[_TerrainIndex] = Math.Min(Unit.Grades.Length, Math.Max(0, Params.LocalContext.EffectTargetUnit.Boosts.DicTerrainLetterAttributeModifier[_TerrainIndex] + LastEvaluationResult));
            }
        }

        protected override BaseEffect DoCopy()
        {
            UnitTerrainEffect NewEffect = new UnitTerrainEffect(Params);

            NewEffect._TerrainIndex = _TerrainIndex;
            NewEffect._NumberType = _NumberType;
            NewEffect._Value = _Value;
            NewEffect._CanDowngrade = _CanDowngrade;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            UnitTerrainEffect NewEffect = (UnitTerrainEffect)Copy;

            _TerrainIndex = NewEffect._TerrainIndex;
            _NumberType = NewEffect._NumberType;
            _Value = NewEffect._Value;
            _CanDowngrade = NewEffect._CanDowngrade;
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
            get { return _Value; }
            set { _Value = value; }
        }

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public byte Terrain
        {
            get { return _TerrainIndex; }
            set { _TerrainIndex = value; }
        }

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public bool CanDowngrade
        {
            get { return _CanDowngrade; }
            set { _CanDowngrade = value; }
        }

        #endregion
    }
}
