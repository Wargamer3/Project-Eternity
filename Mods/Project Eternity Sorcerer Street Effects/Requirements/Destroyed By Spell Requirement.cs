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

        public SorcererStreetDestroyedBySpellRequirement(SorcererStreetBattleParams Params)
            : base("Sorcerer Street Destroyed By Spell", Params)
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
            return new SorcererStreetDestroyedBySpellRequirement(Params);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
