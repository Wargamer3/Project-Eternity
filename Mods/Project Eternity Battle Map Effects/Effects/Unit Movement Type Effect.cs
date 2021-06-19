using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class UnitMovementTypeEffect : SkillEffect
    {
        public static string Name = "Unit Movement Type Effect";

        private bool _UseAir;
        private bool _UseLand;
        private bool _UseSea;
        private bool _UseSpace;

        public UnitMovementTypeEffect()
            : base(Name, true)
        {
        }

        public UnitMovementTypeEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _UseAir = BR.ReadBoolean();
            _UseLand = BR.ReadBoolean();
            _UseSea = BR.ReadBoolean();
            _UseSpace = BR.ReadBoolean();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_UseAir);
            BW.Write(_UseLand);
            BW.Write(_UseSea);
            BW.Write(_UseSpace);
        }

        protected override string DoExecuteEffect()
        {
            string Output = "Added support for ";

            if (_UseAir && !Params.LocalContext.EffectTargetUnit.ListTerrainChoices.Contains(Core.Units.UnitStats.TerrainAir))
            {
                Params.LocalContext.EffectTargetUnit.ListTerrainChoices.Add(Core.Units.UnitStats.TerrainAir);
                Output += Core.Units.UnitStats.TerrainAir + " ";
            }
            if (_UseLand && !Params.LocalContext.EffectTargetUnit.ListTerrainChoices.Contains(Core.Units.UnitStats.TerrainLand))
            {
                Params.LocalContext.EffectTargetUnit.ListTerrainChoices.Add(Core.Units.UnitStats.TerrainLand);
                Output += Core.Units.UnitStats.TerrainLand + " ";
            }
            if (_UseSea && !Params.LocalContext.EffectTargetUnit.ListTerrainChoices.Contains(Core.Units.UnitStats.TerrainSea))
            {
                Params.LocalContext.EffectTargetUnit.ListTerrainChoices.Add(Core.Units.UnitStats.TerrainSea);
                Output += Core.Units.UnitStats.TerrainSea + " ";
            }
            if (_UseSpace && !Params.LocalContext.EffectTargetUnit.ListTerrainChoices.Contains(Core.Units.UnitStats.TerrainSpace))
            {
                Params.LocalContext.EffectTargetUnit.ListTerrainChoices.Add(Core.Units.UnitStats.TerrainSpace);
                Output += Core.Units.UnitStats.TerrainSpace + " ";
            }

            return Output;
        }

        protected override BaseEffect DoCopy()
        {
            UnitMovementTypeEffect NewEffect = new UnitMovementTypeEffect(Params);

            NewEffect._UseAir = _UseAir;
            NewEffect._UseLand = _UseLand;
            NewEffect._UseSea = _UseSea;
            NewEffect._UseSpace = _UseSpace;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            UnitMovementTypeEffect NewEffect = (UnitMovementTypeEffect)Copy;

            _UseAir = NewEffect._UseAir;
            _UseLand = NewEffect._UseLand;
            _UseSea = NewEffect._UseSea;
            _UseSpace = NewEffect._UseSpace;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public bool UseAir
        {
            get { return _UseAir; }
            set { _UseAir = value; }
        }

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public bool UseLand
        {
            get { return _UseLand; }
            set { _UseLand = value; }
        }

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public bool UseSea
        {
            get { return _UseSea; }
            set { _UseSea = value; }
        }

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public bool UseSpace
        {
            get { return _UseSpace; }
            set { _UseSpace = value; }
        }

        #endregion
    }
}
