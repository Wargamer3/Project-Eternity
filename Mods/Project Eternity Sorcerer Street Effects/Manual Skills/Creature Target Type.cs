using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetCreatureTargetType : ManualSkillActivationSorcererStreet
    {
        public static string Name = "Sorcerer Street Creature";

        public SorcererStreetCreatureTargetType()
            : this(null)
        {

        }

        public SorcererStreetCreatureTargetType(SorcererStreetBattleParams Context)
            : base(Name, true, Context)
        {
        }

        public override bool CanActivateOnTarget(ManualSkill ActiveSkill)
        {
            return true;
        }

        public override void ActivateSkillFromMenu(ManualSkill ActiveSkill)
        {
            Params.Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelSelectCreatureSpell(Params.Map, ActiveSkill, true));
        }

        public override ManualSkillTarget Copy()
        {
            return new SorcererStreetCreatureTargetType(Params);
        }

        public override void CopyMembers(ManualSkillTarget Copy)
        {
        }
    }
}
