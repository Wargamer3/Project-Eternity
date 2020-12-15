using System;
using System.Linq;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelRefillDeckPhase : ActionPanelSorcererStreet
    {
        private readonly Player ActivePlayer;
        private readonly Random Random;

        public ActionPanelRefillDeckPhase(SorcererStreetMap Map, Player ActivePlayer)
            : base("Refill Deck", Map, false)
        {
            this.ActivePlayer = ActivePlayer;
            Random = new Random();
        }

        public override void OnSelect()
        {
            ActivePlayer.ListRemainingCardInDeck.AddRange(ActivePlayer.ArrayCardInDeck);

            int RemainingCardToShuffleCount = ActivePlayer.ListRemainingCardInDeck.Count;

            while (RemainingCardToShuffleCount-- > 1)
            {
                int ShuffleIndex = Random.Next(RemainingCardToShuffleCount + 1);
                Card ActiveCard = ActivePlayer.ListRemainingCardInDeck[ShuffleIndex];

                ActivePlayer.ListRemainingCardInDeck[ShuffleIndex] = ActivePlayer.ListRemainingCardInDeck[RemainingCardToShuffleCount];
                ActivePlayer.ListRemainingCardInDeck[RemainingCardToShuffleCount] = ActiveCard;
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            FinishPhase();
        }

        public void FinishPhase()
        {
            RemoveFromPanelList(this);
            AddToPanelListAndSelect(new ActionPanelSpellCardSelectionPhase(Map, ActivePlayer));
        }

        protected override void OnCancelPanel()
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
