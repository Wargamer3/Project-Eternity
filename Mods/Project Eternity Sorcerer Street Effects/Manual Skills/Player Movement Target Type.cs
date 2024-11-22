using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Effects;

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
            ActiveSkill.ListEffect[0].ExecuteEffect();
        }

        public override ManualSkillTarget Copy()
        {
            return new SorcererStreetPlayerMovementTargetType(Params);
        }
    }
}
