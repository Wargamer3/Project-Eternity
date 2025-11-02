using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetAllCreaturesAreaTargetType : ManualSkillActivationSorcererStreet
    {
        public static string Name = "Sorcerer Street All Creatures Area";

        public SorcererStreetAllCreaturesAreaTargetType()
            : this(null)
        {

        }

        public SorcererStreetAllCreaturesAreaTargetType(SorcererStreetBattleParams Context)
            : base(Name, true, Context)
        {
        }

        public override bool CanActivateOnTarget(ManualSkill ActiveSkill)
        {
            return false;
        }

        public override void ActivateSkillFromMenu(ManualSkill ActiveSkill)
        {
            Params.Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelAllCreatureInAreaSpellConfirm(Params.Map, ActiveSkill, Params. GlobalPlayerContext.ActivePlayerIndex));
        }

        public override ManualSkillTarget Copy()
        {
            return new SorcererStreetAllCreaturesAreaTargetType(Params);
        }

        public override void CopyMembers(ManualSkillTarget Copy)
        {
        }
    }
}
