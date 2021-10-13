using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelCreatureCardSelectionPhase : ActionPanelCardSelectionPhase
    {
        private const string PanelName = "CreatureCardSelection";

        public ActionPanelCreatureCardSelectionPhase(SorcererStreetMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelCreatureCardSelectionPhase(SorcererStreetMap Map, int ActivePlayerIndex)
            : base(PanelName, Map.ListActionMenuChoice, Map, ActivePlayerIndex, CreatureCard.CreatureCardType, "End turn")
        {
        }

        public override void OnEndCardSelected()
        {
            RemoveAllActionPanels();
            Map.EndPlayerPhase();
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelCreatureCardSelectionPhase(Map);
        }
    }
}
