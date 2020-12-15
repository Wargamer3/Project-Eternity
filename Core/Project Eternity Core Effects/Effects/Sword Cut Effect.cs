using System;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Core.Effects
{
    public sealed class SwordCutEffect : SkillEffect
    {
        public static string Name = "Sword Cut Effect";

        public SwordCutEffect()
            : base(Name, true)
        {
        }

        public SwordCutEffect(UnitEffectParams Params)
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
            throw new NotImplementedException();
        }

        protected override BaseEffect DoCopy()
        {
            SwordCutEffect NewEffect = new SwordCutEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
