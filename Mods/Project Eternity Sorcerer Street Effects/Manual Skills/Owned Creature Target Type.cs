using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetOwnedCreatureTargetType : ManualSkillActivationSorcererStreet
    {
        public static string Name = "Sorcerer Street Owned Creature";

        public SorcererStreetOwnedCreatureTargetType()
            : this(null)
        {

        }

        public SorcererStreetOwnedCreatureTargetType(SorcererStreetBattleParams Context)
            : base(Name, true, Context)
        {
        }

        public override bool CanActivateOnTarget(ManualSkill ActiveSkill)
        {
            return true;
        }

        public override void ActivateSkillFromMenu(ManualSkill ActiveSkill)
        {
            Params.Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelSelectCreatureSpell(Params.Map, ActiveSkill, true, false));
        }

        public override ManualSkillTarget Copy()
        {
            return new SorcererStreetOwnedCreatureTargetType(Params);
        }

        public override void CopyMembers(ManualSkillTarget Copy)
        {
        }
    }
}
