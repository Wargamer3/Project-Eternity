using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

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
            ActivePlayer.Magic -= ActiveTerrain.CurrentToll;
            Map.UpdateTolls(ActivePlayer);
            Map.EndPlayerPhase();
        }

        public override void Draw(CustomSpriteBatch g)
        {
            int BoxHeight = 70;
            base.Draw(g);
            ActionPanelPlayerDefault.DrawPlayerInformation(g, Map, ActivePlayer, 30, Constants.Height / 20);
            GameScreen.DrawBox(g, new Vector2(30, Constants.Height / 20 + BoxHeight * 2), 200, 30, Color.White);
            g.DrawStringCentered(Map.fntArial12, "Creature Selection", new Vector2(130, Constants.Height / 20 + BoxHeight * 2 + 15), Color.White);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelPayTollPhase(Map);
        }
    }
}
