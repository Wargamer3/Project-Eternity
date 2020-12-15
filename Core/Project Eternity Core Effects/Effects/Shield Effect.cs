using ProjectEternity.Core.Item;

namespace ProjectEternity.Core.Effects
{
    public sealed class ShieldEffect : SkillEffect
    {
        public static string Name = "Shield Effect";

        public ShieldEffect()
            : base(Name, true)
        {
        }

        public ShieldEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
        }
        protected override void Load(System.IO.BinaryReader BR)
        {
        }

        protected override void Save(System.IO.BinaryWriter BW)
        {
        }

        protected override string DoExecuteEffect()
        {
            Params.LocalContext.EffectTargetUnit.Boosts.ShieldModifier = true;

            return string.Empty;
        }

        protected override BaseEffect DoCopy()
        {
            ShieldEffect NewEffect = new ShieldEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
