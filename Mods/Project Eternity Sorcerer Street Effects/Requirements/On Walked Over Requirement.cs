using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetOnWalkedOverRequirement : SorcererStreetRequirement
    {
        public SorcererStreetOnWalkedOverRequirement()
            : this(null)
        {
        }

        public SorcererStreetOnWalkedOverRequirement(SorcererStreetBattleParams Params)
            : base("Sorcerer Street On Walked Over", Params)
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
            return new SorcererStreetOnWalkedOverRequirement(Params);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
