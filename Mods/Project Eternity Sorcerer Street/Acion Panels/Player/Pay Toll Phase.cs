using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelPayTollPhase : ActionPanelCardSelectionPhase
    {
        private const string PanelName = "PayToll";

        private readonly TerrainSorcererStreet ActiveTerrain;

        public ActionPanelPayTollPhase(SorcererStreetMap Map)
            : base(PanelName, Map, CreatureCard.CreatureCardType)
        {
        }

        public ActionPanelPayTollPhase(SorcererStreetMap Map, int ActivePlayerIndex, TerrainSorcererStreet ActiveTerrain)
            : base(PanelName, Map.ListActionMenuChoice, Map, ActivePlayerIndex, CreatureCard.CreatureCardType, "Pay")
        {
            this.ActiveTerrain = ActiveTerrain;
        }

        public override void OnCardSelected(Card CardSelected)
        {
            AddToPanelListAndSelect(new ActionPanelConfirmCreatureSummonBattle(Map, ActivePlayerIndex, (CreatureCard)CardSelected));
        }

        public override void OnEndCardSelected()
        {
            ActivePlayer.Gold -= ActiveTerrain.CurrentToll;
            Map.UpdateTolls(ActivePlayer);
            Map.EndPlayerPhase();
        }

        public override void Draw(CustomSpriteBatch g)
        {
            base.Draw(g);
            ActionPanelPlayerDefault.DrawPlayerInformation(g, Map, ActivePlayer);
            ActionPanelPlayerDefault.DrawPhase(g, Map, "Creature Selection");
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelPayTollPhase(Map);
        }
    }
}
