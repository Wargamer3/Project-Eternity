using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class AfterGettingMissedRequirement : DeathmatchSkillRequirement
    {
        public AfterGettingMissedRequirement()
            : this(null)
        {
        }

        public AfterGettingMissedRequirement(DeathmatchParams Params)
            : base(AfterGettingMissedRequirementName, Params)
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
            AfterGettingMissedRequirement NewSkillEffect = new AfterGettingMissedRequirement(Params);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
