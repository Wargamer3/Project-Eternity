using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetGlobalAbilityRequirement : SorcererStreetBattleRequirement
    {
        public SorcererStreetGlobalAbilityRequirement()
            : this(null)
        {
        }

        public SorcererStreetGlobalAbilityRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Sorcerer Street Global Ability", GlobalContext)
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
            return new SorcererStreetGlobalAbilityRequirement(GlobalContext);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
