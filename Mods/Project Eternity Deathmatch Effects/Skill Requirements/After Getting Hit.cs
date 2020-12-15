using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class AfterGettingHitRequirement : DeathmatchSkillRequirement
    {
        public AfterGettingHitRequirement()
            : this(null)
        {
        }

        public AfterGettingHitRequirement(DeathmatchContext Context)
            : base(AfterGettingHitRequirementName, Context)
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
            AfterGettingHitRequirement NewSkillEffect = new AfterGettingHitRequirement(Context);

            return NewSkillEffect;
        }
    }
}
