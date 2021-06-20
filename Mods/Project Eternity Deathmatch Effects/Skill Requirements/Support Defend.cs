using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class SupportDefendRequirement : DeathmatchSkillRequirement
    {
        public SupportDefendRequirement()
            : this(null)
        {
        }

        public SupportDefendRequirement(DeathmatchContext Context)
            : base(SupportDefendRequirementName, Context)
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
            SupportDefendRequirement NewSkillEffect = new SupportDefendRequirement(Context);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
