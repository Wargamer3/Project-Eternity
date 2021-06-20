using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class BeforeMissRequirement : DeathmatchSkillRequirement
    {
        public BeforeMissRequirement()
            : this(null)
        {
        }

        public BeforeMissRequirement(DeathmatchContext Context)
            : base(BeforeMissRequirementName, Context)
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
            BeforeMissRequirement NewSkillEffect = new BeforeMissRequirement(Context);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
