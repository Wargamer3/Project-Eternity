using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetOnSummonedRequirement : SorcererStreetBattleRequirement
    {
        public SorcererStreetOnSummonedRequirement()
            : this(null)
        {
        }

        public SorcererStreetOnSummonedRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Sorcerer Street On Summoned", GlobalContext)
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
            return new SorcererStreetOnSummonedRequirement(GlobalContext);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
