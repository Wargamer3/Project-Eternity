using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelChooseTerritory : ActionPanelSorcererStreet
    {
        private readonly Player ActivePlayer;

        public ActionPanelChooseTerritory(SorcererStreetMap Map, Player ActivePlayer)
            : base("Choose Territory", Map, false)
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
        }
    }
}
