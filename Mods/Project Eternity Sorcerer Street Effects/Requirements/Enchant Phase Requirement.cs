using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetEnchantPhaseRequirement : SorcererStreetRequirement
    {
        public SorcererStreetEnchantPhaseRequirement()
            : this(null)
        {
        }

        public SorcererStreetEnchantPhaseRequirement(SorcererStreetBattleContext GlobalContext)
            : base(ActionPanelBattleEnchantModifierPhase.RequirementName, GlobalContext)
        {
        }

        public override bool CanActivatePassive()
        {
            return false;
        }

        public override BaseSkillRequirement Copy()
        {
            return new SorcererStreetEnchantPhaseRequirement(GlobalContext);
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }

        protected override void Load(BinaryReader BR)
        {
        }
    }
}
