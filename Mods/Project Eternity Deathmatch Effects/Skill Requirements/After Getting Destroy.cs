using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class AfterGettingDestroyRequirement : DeathmatchSkillRequirement
    {
        public AfterGettingDestroyRequirement()
            : this(null)
        {
        }

        public AfterGettingDestroyRequirement(DeathmatchParams Params)
            : base(AfterGettingDestroyRequirementName, Params)
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
            AfterGettingDestroyRequirement NewSkillEffect = new AfterGettingDestroyRequirement(Params);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
