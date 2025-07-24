using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetPlayerTargetType : ManualSkillActivationSorcererStreet
    {
        public SorcererStreetPlayerTargetType()
            : this(null)
        {

        }

        public SorcererStreetPlayerTargetType(SorcererStreetBattleParams Params)
            : base(PlayerTargetType, true, Params)
        {
        }

        public override bool CanActivateOnTarget(ManualSkill ActiveSkill)
        {
            return false;
        }

        public override void ActivateSkillFromMenu(ManualSkill ActiveSkill)
        {
            Params.Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelSelectPlayerSpell(Params.Map, ActiveSkill, false));
        }

        public override ManualSkillTarget Copy()
        {
            return new SorcererStreetPlayerTargetType(Params);
        }
    }
}
