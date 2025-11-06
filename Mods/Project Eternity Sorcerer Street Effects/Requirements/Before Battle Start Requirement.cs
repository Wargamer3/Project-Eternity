using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetBeforeBattleStartRequirement : SorcererStreetRequirement
    {
        public SorcererStreetBeforeBattleStartRequirement()
            : this(null)
        {
        }

        public SorcererStreetBeforeBattleStartRequirement(SorcererStreetBattleParams Params)
            : base(ActionPanelBattleAttackPhase.BeforeBattleStartRequirement, Params)
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
            return new SorcererStreetBeforeBattleStartRequirement(Params);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
