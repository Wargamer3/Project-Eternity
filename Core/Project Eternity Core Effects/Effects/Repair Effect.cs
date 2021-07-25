using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Core.Effects
{
    public sealed class RepairEffect : SkillEffect
    {
        public static string Name = "Repair Effect";

        public RepairEffect()
            : base(Name, true)
        {
        }

        public RepairEffect(UnitEffectParams Params)
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
            Params.LocalContext.EffectTargetUnit.Boosts.RepairModifier = true;

            return string.Empty;
        }

        protected override BaseEffect DoCopy()
        {
            RepairEffect NewEffect = new RepairEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
