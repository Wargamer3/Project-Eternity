using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetOnMoveRequirement : SorcererStreetRequirement
    {
        public SorcererStreetOnMoveRequirement()
            : this(null)
        {
        }

        public SorcererStreetOnMoveRequirement(SorcererStreetBattleParams Params)
            : base("Sorcerer Street On Move", Params)
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
            return new SorcererStreetOnMoveRequirement(Params);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
