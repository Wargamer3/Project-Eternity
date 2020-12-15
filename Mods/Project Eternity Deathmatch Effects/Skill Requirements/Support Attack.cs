using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class SupportAttackRequirement : DeathmatchSkillRequirement
    {
        public SupportAttackRequirement()
            : this(null)
        {
        }

        public SupportAttackRequirement(DeathmatchContext Context)
            : base(SupportAttackRequirementName, Context)
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
            SupportAttackRequirement NewSkillEffect = new SupportAttackRequirement(Context);

            return NewSkillEffect;
        }
    }
}
