using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class BeforeGettingHitRequirement : DeathmatchSkillRequirement
    {
        public BeforeGettingHitRequirement()
            : this(null)
        {
        }

        public BeforeGettingHitRequirement(DeathmatchContext Context)
            : base(BeforeGettingHitRequirementName, Context)
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
            BeforeGettingHitRequirement NewSkillEffect = new BeforeGettingHitRequirement(Context);

            return NewSkillEffect;
        }
    }
}
