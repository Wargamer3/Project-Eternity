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

        public SupportDefendRequirement(DeathmatchParams Params)
            : base(SupportDefendRequirementName, Params)
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
            SupportDefendRequirement NewSkillEffect = new SupportDefendRequirement(Params);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
