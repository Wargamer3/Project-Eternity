using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetBattleStartRequirement : SorcererStreetBattleRequirement
    {
        public SorcererStreetBattleStartRequirement()
            : this(null)
        {
        }

        public SorcererStreetBattleStartRequirement(SorcererStreetBattleContext GlobalContext)
            : base(ActionPanelBattleAttackPhase.BattleStartRequirement, GlobalContext)
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
            return new SorcererStreetBattleStartRequirement(GlobalContext);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
