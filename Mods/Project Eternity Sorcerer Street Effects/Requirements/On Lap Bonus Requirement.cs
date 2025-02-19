using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetOnLapBonusRequirement : SorcererStreetRequirement
    {
        public SorcererStreetOnLapBonusRequirement()
            : this(null)
        {
        }

        public SorcererStreetOnLapBonusRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Sorcerer Street On Lap Bonus", GlobalContext)
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
            return new SorcererStreetOnLapBonusRequirement(GlobalContext);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
