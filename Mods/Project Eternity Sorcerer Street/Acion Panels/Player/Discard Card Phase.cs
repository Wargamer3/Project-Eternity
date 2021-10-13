using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelDiscardCardPhase : ActionPanelSorcererStreet
    {
        private const string PanelName = "DiscardCard";

        private int ActivePlayerIndex;
        private Player ActivePlayer;
        private int MaximumCardsAllowed;

        private int CardCursorIndex;

        public ActionPanelDiscardCardPhase(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelDiscardCardPhase(SorcererStreetMap Map, int ActivePlayerIndex, int MaximumCardsAllowed)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.MaximumCardsAllowed = MaximumCardsAllowed;
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
        }

        public override void OnSelect()
        {
            CardCursorIndex = 0;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputLeftPressed() && --CardCursorIndex < 0)
            {
                CardCursorIndex = ActivePlayer.ListCardInHand.Count - 1;
            }
            else if (InputHelper.InputRightPressed() && ++CardCursorIndex >= ActivePlayer.ListCardInHand.Count)
            {
                CardCursorIndex = 0;
            }
            else if (InputHelper.InputConfirmPressed())
            {
                ActivePlayer.ListCardInHand.RemoveAt(CardCursorIndex);

                if (ActivePlayer.ListCardInHand.Count <= MaximumCardsAllowed)
                {
                    RemoveFromPanelList(this);
                }
            }
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            CardCursorIndex = BR.ReadInt32();
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(CardCursorIndex);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelDiscardCardPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
