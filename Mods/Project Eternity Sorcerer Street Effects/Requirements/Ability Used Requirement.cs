using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetRegenerateUsedRequirement : SorcererStreetRequirement
    {
        public SorcererStreetRegenerateUsedRequirement()
            : this(null)
        {
        }

        public SorcererStreetRegenerateUsedRequirement(SorcererStreetBattleParams Params)
            : base("Sorcerer Street Regenerates Used", Params)
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
            return Params.GlobalContext.OpponentCreature.Creature.GetCurrentAbilities(Params.GlobalContext.EffectActivationPhase).Regenerate;
        }

        public override BaseSkillRequirement Copy()
        {
            return new SorcererStreetRegenerateUsedRequirement(Params);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
