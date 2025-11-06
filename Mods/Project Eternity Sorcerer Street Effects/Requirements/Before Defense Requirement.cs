using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetBeforeDefenseRequirement : SorcererStreetRequirement
    {
        public SorcererStreetBeforeDefenseRequirement()
            : this(null)
        {
        }

        public SorcererStreetBeforeDefenseRequirement(SorcererStreetBattleParams Params)
            : base(ActionPanelBattleAttackPhase.BeforeDefenseRequirement, Params)
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
            return new SorcererStreetBeforeDefenseRequirement(Params);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
