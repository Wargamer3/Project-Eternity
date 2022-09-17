using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class OnDeathPERAttackRequirement : AttackPERRequirement
    {
        public OnDeathPERAttackRequirement()
            : this(null)
        {
        }

        public OnDeathPERAttackRequirement(AttackPERParams Params)
            : base(OnDeath, Params)
        {
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }

        protected override void Load(BinaryReader BR)
        {
        }

        public override bool CanActivatePassive()
        {
            return false;
        }

        public override BaseSkillRequirement Copy()
        {
            return new OnDeathPERAttackRequirement(Params);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
