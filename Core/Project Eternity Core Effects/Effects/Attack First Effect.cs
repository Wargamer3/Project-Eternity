using ProjectEternity.Core.Item;

namespace ProjectEternity.Core.Effects
{
    public sealed class AttackFirstEffect : SkillEffect
    {
        public static string Name = "Attack First Effect";

        public AttackFirstEffect()
            : base(Name, true)
        {
        }

        public AttackFirstEffect(UnitEffectParams Params)
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
            Params.LocalContext.EffectTargetUnit.Boosts.AttackFirstModifier = true;

            return string.Empty;
        }

        protected override BaseEffect DoCopy()
        {
            AttackFirstEffect NewEffect = new AttackFirstEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
