using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleItemSelectionPhase : ActionPanelCardSelectionPhase
    {
        private const string PanelName = "BattleItemSelection";

        public ActionPanelBattleItemSelectionPhase(SorcererStreetMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelBattleItemSelectionPhase(ActionPanelHolder ListActionMenuChoice, SorcererStreetMap Map, int ActivePlayerIndex)
            : base(PanelName, ListActionMenuChoice, Map, ActivePlayerIndex, "Item", "End")
        {
        }

        public override void OnCardSelected(Card CardSelected)
        {
        }

        public override void OnEndCardSelected()
        {
            RemoveFromPanelList(this);
            //Flip the item cards to reveal their content then start the battle
            AddToPanelListAndSelect(new ActionPanelBattleLandModifierPhase(ListActionMenuChoice, Map, ActivePlayer.GamePiece));
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelBattleItemSelectionPhase(Map);
        }
    }
}
