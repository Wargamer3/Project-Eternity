using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetBattleEndRequirement : SorcererStreetRequirement
    {
        public SorcererStreetBattleEndRequirement()
            : this(null)
        {
        }

        public SorcererStreetBattleEndRequirement(SorcererStreetBattleContext GlobalContext)
            : base(ActionPanelBattleDefenderDefeatedPhase.BattleEndRequirementName, GlobalContext)
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
            return new SorcererStreetBattleEndRequirement(GlobalContext);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
