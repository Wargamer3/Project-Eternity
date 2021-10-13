using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelRefillDeckPhase : ActionPanelSorcererStreet
    {
        private const string PanelName = "RefillDeck";

        private int ActivePlayerIndex;
        private readonly Player ActivePlayer;
        private readonly Random Random;

        public ActionPanelRefillDeckPhase(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelRefillDeckPhase(SorcererStreetMap Map, int ActivePlayerIndex)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
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
            AddToPanelListAndSelect(new ActionPanelSpellCardSelectionPhase(Map, ActivePlayerIndex));
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelRefillDeckPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
