using System.IO;
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
        protected override void Load(BinaryReader BR)
        {
        }

        protected override void Save(BinaryWriter BW)
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
