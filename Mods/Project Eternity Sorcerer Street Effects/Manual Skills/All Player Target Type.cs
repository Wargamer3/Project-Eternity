using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetAllPlayerTargetType : ManualSkillActivationSorcererStreet
    {
        public SorcererStreetAllPlayerTargetType()
            : this(null)
        {

        }

        public SorcererStreetAllPlayerTargetType(SorcererStreetBattleParams Context)
            : base(AllPlayerTargetType, true, Context)
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
            return new SorcererStreetAllPlayerTargetType(Params);
        }
    }
}
