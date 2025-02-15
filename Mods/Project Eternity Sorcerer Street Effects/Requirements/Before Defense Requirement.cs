using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetBeforeDefenseRequirement : SorcererStreetBattleRequirement
    {
        public SorcererStreetBeforeDefenseRequirement()
            : this(null)
        {
        }

        public SorcererStreetBeforeDefenseRequirement(SorcererStreetBattleContext GlobalContext)
            : base(ActionPanelBattleAttackPhase.BeforeDefenseRequirement, GlobalContext)
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
            return new SorcererStreetBeforeDefenseRequirement(GlobalContext);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
