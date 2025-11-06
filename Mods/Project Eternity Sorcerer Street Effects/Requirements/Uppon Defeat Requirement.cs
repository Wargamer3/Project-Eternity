using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetUponDefeatRequirement : SorcererStreetRequirement
    {
        public SorcererStreetUponDefeatRequirement()
            : this(null)
        {
        }

        public SorcererStreetUponDefeatRequirement(SorcererStreetBattleParams Params)
            : base(ActionPanelBattleAttackPhase.UponDefeatRequirement, Params)
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
            return new SorcererStreetUponDefeatRequirement(Params);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
