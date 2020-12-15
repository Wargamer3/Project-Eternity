using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelGainPhase : ActionPanelSorcererStreet
    {
        private readonly Player ActivePlayer;

        public ActionPanelGainPhase(SorcererStreetMap Map, Player ActivePlayer)
            : base("Gain Phase", Map, false)
        {
            this.ActivePlayer = ActivePlayer;
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
            AddToPanelListAndSelect(new ActionPanelDrawCardPhase(Map, ActivePlayer));
        }

        protected override void OnCancelPanel()
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
            //Play coins animation
        }
    }
}
