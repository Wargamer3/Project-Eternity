using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetBattleStartRequirement : SorcererStreetRequirement
    {
        public SorcererStreetBattleStartRequirement()
            : this(null)
        {
        }

        public SorcererStreetBattleStartRequirement(SorcererStreetBattleParams Params)
            : base(ActionPanelBattleAttackPhase.BattleStartRequirement, Params)
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
            return new SorcererStreetBattleStartRequirement(Params);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
