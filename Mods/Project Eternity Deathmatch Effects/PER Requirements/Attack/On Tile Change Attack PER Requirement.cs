using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class OnTileChangePERAttackRequirement : AttackPERRequirement
    {
        public OnTileChangePERAttackRequirement()
            : this(null)
        {
        }

        public OnTileChangePERAttackRequirement(AttackPERParams Params)
            : base(OnTileChange, Params)
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
            return new OnTileChangePERAttackRequirement(Params);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
