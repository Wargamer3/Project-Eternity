using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetCreaturePhaseRequirement : SorcererStreetRequirement
    {
        public SorcererStreetCreaturePhaseRequirement()
            : this(null)
        {
        }

        public SorcererStreetCreaturePhaseRequirement(SorcererStreetBattleContext GlobalContext)
            : base(ActionPanelBattleCreatureModifierPhase.RequirementName, GlobalContext)
        {
        }
        
        public override bool CanActivatePassive()
        {
            return false;
        }

        public override BaseSkillRequirement Copy()
        {
            return new SorcererStreetCreaturePhaseRequirement(GlobalContext);
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }

        protected override void Load(BinaryReader BR)
        {
        }
    }
}
