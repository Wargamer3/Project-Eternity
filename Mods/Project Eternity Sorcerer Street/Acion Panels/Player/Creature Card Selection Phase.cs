namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelCreatureCardSelectionPhase : ActionPanelCardSelectionPhase
    {
        public ActionPanelCreatureCardSelectionPhase(SorcererStreetMap Map, Player ActivePlayer)
            : base(Map.ListActionMenuChoice, Map, ActivePlayer, CreatureCard.CreatureCardType, "End turn")
        {
        }

        public override void OnEndCardSelected()
        {
            RemoveAllActionPanels();
            Map.EndPlayerPhase();
        }
    }
}
