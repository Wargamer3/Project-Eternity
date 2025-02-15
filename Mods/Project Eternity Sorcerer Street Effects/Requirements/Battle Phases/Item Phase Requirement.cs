using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetItemPhaseRequirement : SorcererStreetBattleRequirement
    {
        public SorcererStreetItemPhaseRequirement()
            : this(null)
        {
        }

        public SorcererStreetItemPhaseRequirement(SorcererStreetBattleContext GlobalContext)
            : base(ActionPanelBattleItemModifierPhase.RequirementName, GlobalContext)
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
            return new SorcererStreetItemPhaseRequirement(GlobalContext);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
