using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelCreatureCardSelectionPhase : ActionPanelCardSelectionPhase
    {
        private const string PanelName = "CreatureCardSelection";

        public ActionPanelCreatureCardSelectionPhase(SorcererStreetMap Map)
            : base(PanelName, Map, CreatureCard.CreatureCardType)
        {
            DrawDrawInfo = true;
        }

        public ActionPanelCreatureCardSelectionPhase(SorcererStreetMap Map, int ActivePlayerIndex)
            : base(PanelName, Map.ListActionMenuChoice, Map, ActivePlayerIndex, CreatureCard.CreatureCardType, "End turn")
        {
            DrawDrawInfo = true;
        }

        public override void OnEndCardSelected()
        {
            Map.EndPlayerPhase();
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelCreatureCardSelectionPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            int BoxHeight = 70;
            base.Draw(g);
            ActionPanelPlayerDefault.DrawPlayerInformation(g, Map, ActivePlayer, 30, Constants.Height / 20);
            GameScreen.DrawBox(g, new Vector2(30, Constants.Height / 20 + BoxHeight * 2), 200, 30, Color.White);
            g.DrawStringCentered(Map.fntArial12, "Creature Selection", new Vector2(130, Constants.Height / 20 + BoxHeight * 2 + 15), Color.White);
        }
    }
}
