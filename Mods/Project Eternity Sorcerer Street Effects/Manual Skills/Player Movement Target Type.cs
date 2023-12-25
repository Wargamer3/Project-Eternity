using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetPlayerMovementTargetType : ManualSkillActivationSorcererStreet
    {
        public SorcererStreetPlayerMovementTargetType()
            : this(null)
        {

        }

        public SorcererStreetPlayerMovementTargetType(SorcererStreetBattleParams Context)
            : base(PlayerMovementTargetType, true, Context)
        {
        }

        public override bool CanActivateOnTarget(ManualSkill ActiveSkill)
        {
            return false;
        }

        public override void ActivateSkillFromMenu(ManualSkill ActiveSkill)
        {
        }

        public override ManualSkillTarget Copy()
        {
            return new SorcererStreetPlayerMovementTargetType(Params);
        }
    }
}
