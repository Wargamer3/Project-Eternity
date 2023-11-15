using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetPlayerTargetType : ManualSkillActivationSorcererStreet
    {
        public SorcererStreetPlayerTargetType()
            : this(null)
        {

        }

        public SorcererStreetPlayerTargetType(SorcererStreetBattleParams Context)
            : base(PlayerTargetType, true, Context)
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
            return new SorcererStreetPlayerTargetType(Params);
        }
    }
}
