using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetDestroyedOutsideBattleRequirement : SorcererStreetRequirement
    {
        public SorcererStreetDestroyedOutsideBattleRequirement()
            : this(null)
        {
        }

        public SorcererStreetDestroyedOutsideBattleRequirement(SorcererStreetBattleParams Params)
            : base("Sorcerer Street Destroyed Outside Battle", Params)
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
            return new SorcererStreetDestroyedOutsideBattleRequirement(Params);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
