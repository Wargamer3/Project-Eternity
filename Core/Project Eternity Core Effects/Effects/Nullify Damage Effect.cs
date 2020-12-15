using ProjectEternity.Core.Item;

namespace ProjectEternity.Core.Effects
{
    public sealed class NullifyDamageEffect : SkillEffect
    {
        public static string Name = "Nullify Damage Effect";

        public NullifyDamageEffect()
            : base(Name, true)
        {
        }

        public NullifyDamageEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
        }
        
        protected override void Load(System.IO.BinaryReader BR)
        {
        }

        protected override void Save(System.IO.BinaryWriter BW)
        {
        }

        public override bool CanActivate()
        {
            return !Params.GlobalContext.EffectTargetUnit.Boosts.NullifyDamageModifier;
        }

        protected override string DoExecuteEffect()
        {
            Params.LocalContext.EffectTargetUnit.Boosts.NullifyDamageModifier = true;

            return string.Empty;
        }

        protected override BaseEffect DoCopy()
        {
            NullifyDamageEffect NewEffect = new NullifyDamageEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
