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
            ActivePlayer.Magic -= ActiveTerrain.CurrentToll;
            Map.UpdateTolls(ActivePlayer);
            Map.EndPlayerPhase();
        }

        public override void Draw(CustomSpriteBatch g)
        {
            base.Draw(g);
            int ActionInfoBoxX = Constants.Width / 16;
            int ActionInfoBoxY = Constants.Height / 3;
            int ActionInfoBoxWidth = Constants.Width / 5;
            int ActionInfoBoxHeight = Constants.Height / 14;
            ActionPanelPlayerDefault.DrawPlayerInformation(g, Map, ActivePlayer, Constants.Width / 16, Constants.Height / 10);
            MenuHelper.DrawBorderlessBox(g, new Vector2(ActionInfoBoxX, ActionInfoBoxY), ActionInfoBoxWidth, ActionInfoBoxHeight);
            g.DrawStringCentered(Map.fntArial12, "Creature Selection", new Vector2(ActionInfoBoxX + ActionInfoBoxWidth / 2, ActionInfoBoxY + ActionInfoBoxHeight / 2), Color.White);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelPayTollPhase(Map);
        }
    }
}
