using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class UnitStatMultiplierEffect : SkillEffect
    {
        public static string Name = "Unit Stat Multiplier Effect";

        private Core.Effects.UnitStats _UnitStat;
        private float _Value;

        public UnitStatMultiplierEffect()
            : base(Name, true)
        {
        }

        public UnitStatMultiplierEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _UnitStat = (Core.Effects.UnitStats)BR.ReadByte();
            _Value = BR.ReadSingle();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_UnitStat);
            BW.Write(_Value);
        }

        protected override string DoExecuteEffect()
        {
            switch (_UnitStat)
            {
                case UnitStats.MaxHP:
                    Params.LocalContext.EffectTargetUnit.Boosts.HPMaxMultiplier = _Value;
                    return "Max HP Multiplier - " + _Value;

                case UnitStats.MaxEN:
                    Params.LocalContext.EffectTargetUnit.Boosts.ENMaxMultiplier = _Value;
                    return "Max EN Multiplier - " + _Value;

                case UnitStats.Armor:
                    Params.LocalContext.EffectTargetUnit.Boosts.ArmorMultiplier = _Value;
                    return "Armor Multiplier - " + _Value;

                case UnitStats.Mobility:
                    Params.LocalContext.EffectTargetUnit.Boosts.MobilityMultiplier = _Value;
                    return "Mobility Multiplier - " + _Value;

                case UnitStats.MaxMV:
                    Params.LocalContext.EffectTargetUnit.Boosts.MVMaxMultiplier = _Value;
                    return "Max MV Multiplier - " + _Value;
            }

            return _Value.ToString();
        }

        protected override BaseEffect DoCopy()
        {
            UnitStatMultiplierEffect NewEffect = new UnitStatMultiplierEffect(Params);

            NewEffect._UnitStat = _UnitStat;
            NewEffect._Value = _Value;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            UnitStatMultiplierEffect NewEffect = (UnitStatMultiplierEffect)Copy;

            _UnitStat = NewEffect._UnitStat;
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
        public float Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        #endregion
    }
}
