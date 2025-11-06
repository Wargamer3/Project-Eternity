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

        public SorcererStreetAgainstDefensiveRequirement(SorcererStreetBattleParams Params)
            : base("Sorcerer Street Against Defensive", Params)
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
            return Params.GlobalContext.OpponentCreature.Creature.GetCurrentAbilities(Params.GlobalContext.EffectActivationPhase).IsDefensive;
        }

        public override BaseSkillRequirement Copy()
        {
            return new SorcererStreetAgainstDefensiveRequirement(Params);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
