using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class AfterGettingAttackedRequirement : DeathmatchSkillRequirement
    {
        public AfterGettingAttackedRequirement()
            : this(null)
        {
        }

        public AfterGettingAttackedRequirement(DeathmatchContext Context)
            : base(AfterGettingAttackedRequirementName, Context)
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
            AfterGettingAttackedRequirement NewSkillEffect = new AfterGettingAttackedRequirement(Context);

            return NewSkillEffect;
        }
    }
}
