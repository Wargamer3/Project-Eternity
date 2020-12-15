using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelViewMap : ActionPanelSorcererStreet
    {
        private readonly Player ActivePlayer;

        private Vector2 CursorPosition;

        public ActionPanelViewMap(SorcererStreetMap Map, Player ActivePlayer)
            : base("View Map", Map, true)
        {
            this.ActivePlayer = ActivePlayer;
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

        public override void Draw(CustomSpriteBatch g)
        {
            //Draw Cursor
            g.Draw(GameScreen.sprPixel, new Rectangle((int)CursorPosition.X, (int)CursorPosition.Y, 32, 32), Color.White);
            GameScreen.DrawBox(g, new Vector2(5, Constants.Height - 300), 300, 300, Color.White);
            //Draw selected terrain information
        }
    }
}
