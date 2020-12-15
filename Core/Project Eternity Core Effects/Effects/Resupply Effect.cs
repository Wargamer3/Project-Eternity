using ProjectEternity.Core.Item;

namespace ProjectEternity.Core.Effects
{
    public sealed class ResupplyEffect : SkillEffect
    {
        public static string Name = "Resupply Effect";

        public ResupplyEffect()
            : base(Name, true)
        {
        }

        public ResupplyEffect(UnitEffectParams Params)
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
            Params.LocalContext.EffectTargetUnit.Boosts.ResupplyModifier = true;

            return string.Empty;
        }

        protected override BaseEffect DoCopy()
        {
            ResupplyEffect NewEffect = new ResupplyEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
