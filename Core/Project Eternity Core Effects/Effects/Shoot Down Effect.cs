using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Core.Effects
{
    public sealed class ShootDownEffect : SkillEffect
    {
        public static string Name = "Shoot Down Effect";

        public ShootDownEffect()
            : base(Name, true)
        {
        }

        public ShootDownEffect(UnitEffectParams Params)
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

        protected override BaseEffect DoCopy()
        {
            ShootDownEffect NewEffect = new ShootDownEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
