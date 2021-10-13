using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelGainPhase : ActionPanelSorcererStreet
    {
        private const string PanelName = "GainPhase";

        private int ActivePlayerIndex;
        private Player ActivePlayer;

        public ActionPanelGainPhase(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelGainPhase(SorcererStreetMap Map, int ActivePlayerIndex)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
        }

        public override void OnSelect()
        {
            ActivePlayer.Magic += Math.Min(50, 19 + Map.GameTurn);
        }

        public override void DoUpdate(GameTime gameTime)
        {
            FinishPhase();
        }

        public void FinishPhase()
        {
            RemoveFromPanelList(this);
            AddToPanelListAndSelect(new ActionPanelDrawCardPhase(Map, ActivePlayerIndex));
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActivePlayer.Magic = BR.ReadInt32();
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActivePlayer.Magic);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelGainPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            //Play coins animation
        }
    }
}
