using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetGlobalAbilityRequirement : SorcererStreetRequirement
    {
        public SorcererStreetGlobalAbilityRequirement()
            : this(null)
        {
        }

        public SorcererStreetGlobalAbilityRequirement(SorcererStreetBattleParams Params)
            : base("Sorcerer Street Global Ability", Params)
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
            return new SorcererStreetGlobalAbilityRequirement(Params);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
