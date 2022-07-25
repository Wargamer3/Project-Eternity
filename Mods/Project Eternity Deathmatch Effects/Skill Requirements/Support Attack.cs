using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class SupportAttackRequirement : DeathmatchSkillRequirement
    {
        public SupportAttackRequirement()
            : this(null)
        {
        }

        public SupportAttackRequirement(DeathmatchParams Params)
            : base(SupportAttackRequirementName, Params)
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
            SupportAttackRequirement NewSkillEffect = new SupportAttackRequirement(Params);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
