using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetBeforeAttackRequirement : SorcererStreetRequirement
    {
        public SorcererStreetBeforeAttackRequirement()
            : this(null)
        {
        }

        public SorcererStreetBeforeAttackRequirement(SorcererStreetBattleParams Params)
            : base(ActionPanelBattleAttackPhase.BeforeAttackRequirement, Params)
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
            return new SorcererStreetBeforeAttackRequirement(Params);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
