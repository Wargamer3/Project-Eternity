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

        public AfterGettingDestroyRequirement(DeathmatchContext Context)
            : base(AfterGettingDestroyRequirementName, Context)
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
            AfterGettingDestroyRequirement NewSkillEffect = new AfterGettingDestroyRequirement(Context);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
