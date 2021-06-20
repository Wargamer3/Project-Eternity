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

        public BeforeAttackRequirement(DeathmatchContext Context)
            : base(BeforeAttackRequirementName, Context)
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
            BeforeAttackRequirement NewSkillEffect = new BeforeAttackRequirement(Context);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
