using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class BeforeGettingMissedRequirement : DeathmatchSkillRequirement
    {
        public BeforeGettingMissedRequirement()
            : this(null)
        {
        }

        public BeforeGettingMissedRequirement(DeathmatchParams Params)
            : base(BeforeGettingMissedRequirementName, Params)
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
            BeforeGettingMissedRequirement NewSkillEffect = new BeforeGettingMissedRequirement(Params);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
