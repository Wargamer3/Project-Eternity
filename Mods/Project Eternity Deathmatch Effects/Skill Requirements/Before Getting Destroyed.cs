using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class BeforeGettingDestroyedRequirement : DeathmatchSkillRequirement
    {
        public BeforeGettingDestroyedRequirement()
            : this(null)
        {
        }

        public BeforeGettingDestroyedRequirement(DeathmatchParams Params)
            : base(BeforeGettingDestroyedRequirementName, Params)
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
            BeforeGettingDestroyedRequirement NewSkillEffect = new BeforeGettingDestroyedRequirement(Params);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
