using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelDiscardCardPhase : ActionPanelCardSelectionPhase
    {
        private const string PanelName = "DiscardCard";

        private int MaximumCardsAllowed;

        public ActionPanelDiscardCardPhase(SorcererStreetMap Map)
            : base(PanelName, Map, null)
        {
            DrawDrawInfo = true;
        }

        public ActionPanelDiscardCardPhase(SorcererStreetMap Map, int ActivePlayerIndex, int MaximumCardsAllowed)
            : base(PanelName, Map.ListActionMenuChoice, Map, ActivePlayerIndex, null)
        {
            DrawDrawInfo = true;

            this.MaximumCardsAllowed = MaximumCardsAllowed;
        }

        public override void OnCardSelected(Card CardSelected)
        {
            ActivePlayer.ListCardInHand.Remove(CardSelected);

            if (ActivePlayer.ListCardInHand.Count <= MaximumCardsAllowed)
            {
                RemoveFromPanelList(this);
            }
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelDiscardCardPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            int BoxHeight = 70;
            base.Draw(g);
            ActionPanelPlayerDefault.DrawPlayerInformation(g, Map, ActivePlayer, 30, Constants.Height / 20);
            GameScreen.DrawBox(g, new Vector2(30, Constants.Height / 20 + BoxHeight * 2), 200, 30, Color.White);
            g.DrawStringCentered(Map.fntArial12, "Discard a card", new Vector2(130, Constants.Height / 20 + BoxHeight * 2 + 15), Color.White);
        }
    }
}
