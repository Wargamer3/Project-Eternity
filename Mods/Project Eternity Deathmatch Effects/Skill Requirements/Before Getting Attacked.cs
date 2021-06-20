using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class BeforeGettingAttackedRequirement : DeathmatchSkillRequirement
    {
        public BeforeGettingAttackedRequirement()
            : this(null)
        {
        }

        public BeforeGettingAttackedRequirement(DeathmatchContext Context)
            : base(BeforeGettingAttackedRequirementName, Context)
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
            BeforeGettingAttackedRequirement NewSkillEffect = new BeforeGettingAttackedRequirement(Context);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
