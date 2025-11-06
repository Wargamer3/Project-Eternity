using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetEnemyCreatureTargetType : ManualSkillActivationSorcererStreet
    {
        public static string Name = "Sorcerer Street Enemy Creature";

        public SorcererStreetEnemyCreatureTargetType()
            : this(null)
        {

        }

        public SorcererStreetEnemyCreatureTargetType(SorcererStreetBattleParams Context)
            : base(Name, true, Context)
        {
        }

        public override bool CanActivateOnTarget(ManualSkill ActiveSkill)
        {
            return true;
        }

        public override void ActivateSkillFromMenu(ManualSkill ActiveSkill)
        {
            Params.Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelSelectCreatureSpell(Params.Map, ActiveSkill, false));
        }

        public override ManualSkillTarget Copy()
        {
            return new SorcererStreetEnemyCreatureTargetType(Params);
        }

        public override void CopyMembers(ManualSkillTarget Copy)
        {
        }
    }
}
