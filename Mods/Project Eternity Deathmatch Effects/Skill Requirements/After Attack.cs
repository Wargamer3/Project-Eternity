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

        public AfterAttackRequirement(DeathmatchParams Params)
            : base(AfterAttackRequirementName, Params)
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
            AfterAttackRequirement NewSkillEffect = new AfterAttackRequirement(Params);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
