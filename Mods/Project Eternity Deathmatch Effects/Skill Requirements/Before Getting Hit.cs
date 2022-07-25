using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class BeforeGettingHitRequirement : DeathmatchSkillRequirement
    {
        public BeforeGettingHitRequirement()
            : this(null)
        {
        }

        public BeforeGettingHitRequirement(DeathmatchParams Params)
            : base(BeforeGettingHitRequirementName, Params)
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
            BeforeGettingHitRequirement NewSkillEffect = new BeforeGettingHitRequirement(Params);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
