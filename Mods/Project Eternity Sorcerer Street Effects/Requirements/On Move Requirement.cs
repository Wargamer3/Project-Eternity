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

        public SorcererStreetOnMoveRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Sorcerer Street On Move", GlobalContext)
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
            return new SorcererStreetOnMoveRequirement(GlobalContext);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
