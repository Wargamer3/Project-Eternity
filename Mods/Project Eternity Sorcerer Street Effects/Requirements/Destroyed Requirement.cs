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

        public SorcererStreetDestroyedOutsideBattleRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Sorcerer Street Destroyed Outside Battle", GlobalContext)
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
            return new SorcererStreetDestroyedOutsideBattleRequirement(GlobalContext);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
