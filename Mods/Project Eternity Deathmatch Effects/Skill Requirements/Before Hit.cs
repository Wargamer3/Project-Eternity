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

        public BeforeHitRequirement(DeathmatchContext Context)
            : base(BeforeHitRequirementName, Context)
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
            BeforeHitRequirement NewSkillEffect = new BeforeHitRequirement(Context);

            return NewSkillEffect;
        }
    }
}
