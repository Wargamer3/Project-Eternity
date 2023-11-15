using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetAgainstDefensiveRequirement : SorcererStreetRequirement
    {
        public SorcererStreetAgainstDefensiveRequirement()
            : this(null)
        {
        }

        public SorcererStreetAgainstDefensiveRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Sorcerer Street Against Defensive", GlobalContext)
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
            return GlobalContext.OpponentCreature.Creature.GetCurrentAbilities(GlobalContext.EffectActivationPhase).IsDefensive;
        }

        public override BaseSkillRequirement Copy()
        {
            return new SorcererStreetAgainstDefensiveRequirement(GlobalContext);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
