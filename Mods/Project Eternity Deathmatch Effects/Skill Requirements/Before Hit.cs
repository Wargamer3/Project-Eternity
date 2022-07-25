using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class BeforeHitRequirement : DeathmatchSkillRequirement
    {
        public BeforeHitRequirement()
            : this(null)
        {
        }

        public BeforeHitRequirement(DeathmatchParams Params)
            : base(BeforeHitRequirementName, Params)
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
            BeforeHitRequirement NewSkillEffect = new BeforeHitRequirement(Params);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
