using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetDestroyedBySpellRequirement : SorcererStreetRequirement
    {
        public SorcererStreetDestroyedBySpellRequirement()
            : this(null)
        {
        }

        public SorcererStreetDestroyedBySpellRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Sorcerer Street Destroyed By Spell", GlobalContext)
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
            return new SorcererStreetDestroyedBySpellRequirement(GlobalContext);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
