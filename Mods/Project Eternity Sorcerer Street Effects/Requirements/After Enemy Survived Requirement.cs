using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreesAfterEnemySurvivedRequirement : SorcererStreetRequirement
    {
        public SorcererStreesAfterEnemySurvivedRequirement()
            : this(null)
        {
        }

        public SorcererStreesAfterEnemySurvivedRequirement(SorcererStreetBattleParams Params)
            : base(ActionPanelBattleAttackPhase.AfterEnemySurvivedRequirement, Params)
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
            return new SorcererStreesAfterEnemySurvivedRequirement(Params);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
