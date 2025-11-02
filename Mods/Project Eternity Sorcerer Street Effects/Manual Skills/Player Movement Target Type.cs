using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetPlayerMovementTargetType : ManualSkillActivationSorcererStreet
    {
        public SorcererStreetPlayerMovementTargetType()
            : this(null)
        {

        }

        public SorcererStreetPlayerMovementTargetType(SorcererStreetBattleParams Params)
            : base(PlayerMovementTargetType, true, Params)
        {
        }

        public override bool CanActivateOnTarget(ManualSkill ActiveSkill)
        {
            return false;
        }

        public override void ActivateSkillFromMenu(ManualSkill ActiveSkill)
        {
            Params.Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelSelectPlayerSpell(Params.Map, ActiveSkill, true));
        }

        public override ManualSkillTarget Copy()
        {
            return new SorcererStreetPlayerMovementTargetType(Params);
        }

        public override void CopyMembers(ManualSkillTarget Copy)
        {
        }
    }
}
