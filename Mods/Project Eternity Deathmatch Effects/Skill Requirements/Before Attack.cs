using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class BeforeAttackRequirement : DeathmatchSkillRequirement
    {
        public BeforeAttackRequirement()
            : this(null)
        {
        }

        public BeforeAttackRequirement(DeathmatchParams Params)
            : base(BeforeAttackRequirementName, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }

        public override BaseSkillRequirement Copy()
        {
            BeforeAttackRequirement NewSkillEffect = new BeforeAttackRequirement(Params);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
