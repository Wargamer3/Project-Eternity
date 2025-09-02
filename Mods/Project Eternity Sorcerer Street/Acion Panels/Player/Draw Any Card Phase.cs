using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelDrawAnyCardPhase : ActionPanelSorcererStreet
    {
        private const string PanelName = "DrawAnyCard";

        private int ActivePlayerIndex;
        private Player ActivePlayer;
        private static CardBook AllCardsBook;
        private EditBookCardListFilterScreen CardSelectionScreen;

        public ActionPanelDrawAnyCardPhase(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
            AllCardsBook = CardBook.LoadGlobalBook();
        }

        public ActionPanelDrawAnyCardPhase(SorcererStreetMap Map, int ActivePlayerIndex)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
        }

        public override void OnSelect()
        {
            AllCardsBook = CardBook.LoadGlobalBook();
            CardSelectionScreen = new EditBookCardListFilterScreen(AllCardsBook, EditBookCardListFilterScreen.Filters.Spell, null, true, false);
            Map.PushScreen(CardSelectionScreen);
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (CardSelectionScreen == null || (CardSelectionScreen.ListSelectedCard != null && CardSelectionScreen.ListSelectedCard.Count == 0))
            {
                return;
            }

            if (CardSelectionScreen.ListSelectedCard != null)
            {
                foreach (Card ActiveCard in CardSelectionScreen.ListSelectedCard)
                {
                    ActivePlayer.ListCardInHand.Add(ActiveCard);
                }

                if (ActivePlayer.ListCardInHand.Count > 6)
                {
                    AddToPanelListAndSelect(new ActionPanelDiscardCardPhase(Map, ActivePlayerIndex, 6));
                }
            }

            CardSelectionScreen = null;
            RemoveFromPanelList(this);
        }

        public override void UpdatePassive(GameTime gameTime)
        {
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];

        }

        public override void ExecuteUpdate(byte[] ArrayUpdateData)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);

        }

        public override byte[] DoWriteUpdate()
        {
            return new byte[] {  };
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelDrawAnyCardPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }

        public override string ToString()
        {
            return "Add any cards to your hand.";
        }
    }
}
