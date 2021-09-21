using System;
using System.IO;
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

        protected override void Load(BinaryReader BR)
        {
        }

        protected override void Save(BinaryWriter BW)
        {
        }

        protected override string DoExecuteEffect()
        {
            throw new NotImplementedException();
        }

        protected override void ReactivateEffect()
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
