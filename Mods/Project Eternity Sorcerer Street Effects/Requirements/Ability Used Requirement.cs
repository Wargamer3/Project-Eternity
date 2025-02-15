using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetRegenerateUsedRequirement : SorcererStreetBattleRequirement
    {
        public SorcererStreetRegenerateUsedRequirement()
            : this(null)
        {
        }

        public SorcererStreetRegenerateUsedRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Sorcerer Street Regenerates Used", GlobalContext)
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
            return GlobalContext.OpponentCreature.Creature.GetCurrentAbilities(GlobalContext.EffectActivationPhase).Regenerate;
        }

        public override BaseSkillRequirement Copy()
        {
            return new SorcererStreetRegenerateUsedRequirement(GlobalContext);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
