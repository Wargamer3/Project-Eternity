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

        public BeforeGettingAttackedRequirement(DeathmatchParams Params)
            : base(BeforeGettingAttackedRequirementName, Params)
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
            BeforeGettingAttackedRequirement NewSkillEffect = new BeforeGettingAttackedRequirement(Params);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
