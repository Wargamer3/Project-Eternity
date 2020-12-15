using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleItemSelectionPhase : ActionPanelCardSelectionPhase
    {
        private readonly Player ActivePlayer;

        public ActionPanelBattleItemSelectionPhase(ActionPanelHolder ListActionMenuChoice, SorcererStreetMap Map, Player ActivePlayer)
            : base(ListActionMenuChoice, Map, ActivePlayer, "Item", "End")
        {
            this.ActivePlayer = ActivePlayer;
        }

        public override void OnCardSelected(Card CardSelected)
        {
        }

        public override void OnEndCardSelected()
        {
            RemoveFromPanelList(this);
            //Flip the item cards to reveal their content then start the battle
            AddToPanelListAndSelect(new ActionPanelBattleLandModifierPhase(ListActionMenuChoice, Map, Map.GetTerrain(ActivePlayer.GamePiece)));
        }
    }
}
