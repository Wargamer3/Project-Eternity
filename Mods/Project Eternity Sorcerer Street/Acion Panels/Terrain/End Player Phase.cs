using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelEndPlayerPhase : ActionPanelSorcererStreet
    {
        private readonly Player ActivePlayer;

        public ActionPanelEndPlayerPhase(SorcererStreetMap Map, Player ActivePlayer)
            : base("End", Map, false)
        {
            this.ActivePlayer = ActivePlayer;
        }

        public override void OnSelect()
        {
            RemoveAllActionPanels();
            Map.EndPlayerPhase();
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        protected override void OnCancelPanel()
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
