using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class BeforeGettingDestroyedRequirement : DeathmatchSkillRequirement
    {
        public BeforeGettingDestroyedRequirement()
            : this(null)
        {
        }

        public BeforeGettingDestroyedRequirement(DeathmatchContext Context)
            : base(BeforeGettingDestroyedRequirementName, Context)
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
            BeforeGettingDestroyedRequirement NewSkillEffect = new BeforeGettingDestroyedRequirement(Context);

            return NewSkillEffect;
        }
    }
}
