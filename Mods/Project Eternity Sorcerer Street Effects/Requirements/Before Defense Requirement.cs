using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreesBeforeDefenseRequirement : SorcererStreetRequirement
    {
        public SorcererStreesBeforeDefenseRequirement()
            : this(null)
        {
        }

        public SorcererStreesBeforeDefenseRequirement(SorcererStreetBattleContext GlobalContext)
            : base(ActionPanelBattleAttackPhase.BeforeDefenseRequirement, GlobalContext)
        {
        }

        public override bool CanActivatePassive()
        {
            return false;
        }

        public override BaseSkillRequirement Copy()
        {
            return new SorcererStreesBeforeDefenseRequirement(GlobalContext);
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }

        protected override void Load(BinaryReader BR)
        {
        }
    }
}
