using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetBeforeBattleStartRequirement : SorcererStreetBattleRequirement
    {
        public SorcererStreetBeforeBattleStartRequirement()
            : this(null)
        {
        }

        public SorcererStreetBeforeBattleStartRequirement(SorcererStreetBattleContext GlobalContext)
            : base(ActionPanelBattleAttackPhase.BeforeBattleStartRequirement, GlobalContext)
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
            return new SorcererStreetBeforeBattleStartRequirement(GlobalContext);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
