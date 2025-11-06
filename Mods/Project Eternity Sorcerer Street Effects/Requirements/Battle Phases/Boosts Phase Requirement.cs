using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetBoostsPhaseRequirement : SorcererStreetRequirement
    {
        public SorcererStreetBoostsPhaseRequirement()
            : this(null)
        {
        }

        public SorcererStreetBoostsPhaseRequirement(SorcererStreetBattleParams Params)
            : base(ActionPanelBattleBoostsModifierPhase.RequirementName, Params)
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
            return new SorcererStreetBoostsPhaseRequirement(Params);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
