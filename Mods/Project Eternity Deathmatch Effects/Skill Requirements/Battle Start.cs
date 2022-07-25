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

        public BattleStartRequirement(DeathmatchParams Params)
            : base(BattleStartRequirementName, Params)
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
            BattleStartRequirement NewSkillEffect = new BattleStartRequirement(Params);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
