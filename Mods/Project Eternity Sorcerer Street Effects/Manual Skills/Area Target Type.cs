using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetAreaTargetType : ManualSkillActivationSorcererStreet
    {
        public static string Name = "Sorcerer Street Area";

        public SorcererStreetAreaTargetType()
            : this(null)
        {

        }

        public SorcererStreetAreaTargetType(SorcererStreetBattleParams Context)
            : base(Name, true, Context)
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
            return new SorcererStreetAreaTargetType(Params);
        }
    }
}
