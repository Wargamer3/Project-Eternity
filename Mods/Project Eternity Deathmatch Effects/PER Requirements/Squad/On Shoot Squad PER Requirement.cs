using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class OnShootPERAttackRequirement : SquadPERRequirement
    {
        public OnShootPERAttackRequirement()
            : this(null)
        {
        }

        public OnShootPERAttackRequirement(SquadPERParams Params)
            : base(OnShoot, Params)
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
            return new OnShootPERAttackRequirement(Params);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
