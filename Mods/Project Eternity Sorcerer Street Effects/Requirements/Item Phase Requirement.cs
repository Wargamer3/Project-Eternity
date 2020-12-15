using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetItemPhaseRequirement : SorcererStreetRequirement
    {
        public SorcererStreetItemPhaseRequirement()
            : this(null)
        {
        }

        public SorcererStreetItemPhaseRequirement(SorcererStreetBattleContext GlobalContext)
            : base(ActionPanelBattleItemModifierPhase.RequirementName, GlobalContext)
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

        protected override void DoSave(BinaryWriter BW)
        {
        }

        protected override void Load(BinaryReader BR)
        {
        }
    }
}
