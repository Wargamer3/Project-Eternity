using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetOnSummonedRequirement : SorcererStreetRequirement
    {
        public SorcererStreetOnSummonedRequirement()
            : this(null)
        {
        }

        public SorcererStreetOnSummonedRequirement(SorcererStreetBattleParams Params)
            : base("Sorcerer Street On Summoned", Params)
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
            return new SorcererStreetOnSummonedRequirement(Params);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
