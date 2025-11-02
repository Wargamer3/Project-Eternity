using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetEnemyPlayerTargetType : ManualSkillActivationSorcererStreet
    {
        public static string Name = "Sorcerer Street Enemy Player";

        public SorcererStreetEnemyPlayerTargetType()
            : this(null)
        {

        }

        public SorcererStreetEnemyPlayerTargetType(SorcererStreetBattleParams Context)
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
            return new SorcererStreetEnemyPlayerTargetType(Params);
        }

        public override void CopyMembers(ManualSkillTarget Copy)
        {
        }
    }
}
