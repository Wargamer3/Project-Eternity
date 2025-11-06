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

        public SorcererStreetItemPhaseRequirement(SorcererStreetBattleParams Params)
            : base(ActionPanelBattleItemModifierPhase.RequirementName, Params)
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
            return new SorcererStreetItemPhaseRequirement(Params);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
