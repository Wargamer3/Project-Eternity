using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class BattleStartRequirement : DeathmatchSkillRequirement
    {
        public BattleStartRequirement()
            : this(null)
        {
        }

        public BattleStartRequirement(DeathmatchContext Context)
            : base(BattleStartRequirementName, Context)
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
            BattleStartRequirement NewSkillEffect = new BattleStartRequirement(Context);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
