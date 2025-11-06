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

        public SorcererStreetOnLapBonusRequirement(SorcererStreetBattleParams Params)
            : base("Sorcerer Street On Lap Bonus", Params)
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
            return new SorcererStreetOnLapBonusRequirement(Params);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
