using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelEndPlayerPhase : ActionPanelSorcererStreet
    {
        public ActionPanelEndPlayerPhase(SorcererStreetMap Map)
            : base("End", Map, false)
        {
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

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelEndPlayerPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
