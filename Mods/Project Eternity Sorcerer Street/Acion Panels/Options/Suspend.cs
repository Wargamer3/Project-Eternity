using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelSuspend : ActionPanelSorcererStreet
    {
        public ActionPanelSuspend(SorcererStreetMap Map)
            : base("Suspend", Map, false)
        {
        }

        public override void OnSelect()
        {
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
            return new ActionPanelSuspend(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }

        public override string ToString()
        {
            return "Save the current state and end the game. You can continue the game from this point next time.";
        }
    }
}
