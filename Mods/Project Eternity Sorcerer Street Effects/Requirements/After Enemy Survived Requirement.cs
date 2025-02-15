using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreesAfterEnemySurvivedRequirement : SorcererStreetBattleRequirement
    {
        public SorcererStreesAfterEnemySurvivedRequirement()
            : this(null)
        {
        }

        public SorcererStreesAfterEnemySurvivedRequirement(SorcererStreetBattleContext GlobalContext)
            : base(ActionPanelBattleAttackPhase.AfterEnemySurvivedRequirement, GlobalContext)
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
            return new SorcererStreesAfterEnemySurvivedRequirement(GlobalContext);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
