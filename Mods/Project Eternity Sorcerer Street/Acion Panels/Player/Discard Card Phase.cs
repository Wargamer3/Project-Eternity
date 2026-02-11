using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;

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

        public override void OnSelect()
        {
            PlayerAICardToUseIndex = RandomHelper.Next(ActivePlayer.ListCardInHand.Count);
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
            float Ratio = Constants.Height / 720f;
            int BoxWidth = (int)(353 * Ratio);
            int BoxHeight = (int)(40 * Ratio);
            int X = (int)(93 * Ratio);
            int Y = (int)(300 * Ratio);

            base.Draw(g);
            ActionPanelPlayerDefault.DrawPlayerInformation(g, Map, ActivePlayer);
            MenuHelper.DrawBox(g, new Vector2(X, Y), BoxWidth, BoxHeight);
            g.DrawStringCentered(Map.fntMenuText, "Discard a card", new Vector2(X + BoxWidth / 2, Y + BoxHeight / 2), Color.White);
        }
    }
}
