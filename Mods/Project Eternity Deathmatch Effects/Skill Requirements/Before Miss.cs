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

        public BeforeMissRequirement(DeathmatchParams Params)
            : base(BeforeMissRequirementName, Params)
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
            BeforeMissRequirement NewSkillEffect = new BeforeMissRequirement(Params);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
