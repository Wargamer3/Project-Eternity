using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class AfterMissRequirement : DeathmatchSkillRequirement
    {
        public AfterMissRequirement()
            : this(null)
        {
        }

        public AfterMissRequirement(DeathmatchParams Params)
            : base(AfterMissRequirementName, Params)
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
            AfterMissRequirement NewSkillEffect = new AfterMissRequirement(Params);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
