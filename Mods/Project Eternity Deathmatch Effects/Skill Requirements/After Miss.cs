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

        public AfterMissRequirement(DeathmatchContext Context)
            : base(AfterMissRequirementName, Context)
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
            AfterMissRequirement NewSkillEffect = new AfterMissRequirement(Context);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
