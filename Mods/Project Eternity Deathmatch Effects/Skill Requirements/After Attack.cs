using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class AfterAttackRequirement : DeathmatchSkillRequirement
    {
        public AfterAttackRequirement()
            : this(null)
        {
        }

        public AfterAttackRequirement(DeathmatchContext Context)
            : base(AfterAttackRequirementName, Context)
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
            AfterAttackRequirement NewSkillEffect = new AfterAttackRequirement(Context);

            return NewSkillEffect;
        }
    }
}
