using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetSelfCreatureTargetType : ManualSkillActivationSorcererStreet
    {
        public static string Name = "Sorcerer Street Self Creature";

        public SorcererStreetSelfCreatureTargetType()
            : this(null)
        {

        }

        public SorcererStreetSelfCreatureTargetType(SorcererStreetBattleParams Context)
            : base(Name, true, Context)
        {
        }

        public override bool CanActivateOnTarget(ManualSkill ActiveSkill)
        {
            return true;
        }

        public override void ActivateSkillFromMenu(ManualSkill ActiveSkill)
        {
        }

        public override ManualSkillTarget Copy()
        {
            return new SorcererStreetSelfCreatureTargetType(Params);
        }

        public override void CopyMembers(ManualSkillTarget Copy)
        {
        }
    }
}
