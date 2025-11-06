using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetAttackBonusRequirement : SorcererStreetRequirement
    {
        public SorcererStreetAttackBonusRequirement()
            : this(null)
        {
        }

        public SorcererStreetAttackBonusRequirement(SorcererStreetBattleParams Params)
            : base(ActionPanelBattleAttackPhase.AttackBonusRequirement, Params)
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
            return new SorcererStreetAttackBonusRequirement(Params);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
