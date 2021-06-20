using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class AfterHitRequirement : DeathmatchSkillRequirement
    {
        public AfterHitRequirement()
            : this(null)
        {
        }

        public AfterHitRequirement(DeathmatchContext Context)
            : base(AfterHitRequirementName, Context)
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
            AfterHitRequirement NewSkillEffect = new AfterHitRequirement(Context);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
