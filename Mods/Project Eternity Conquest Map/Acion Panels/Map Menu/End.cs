using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelMainMenuEnd : ActionPanelConquest
    {
        public ActionPanelMainMenuEnd(ConquestMap Map)
            : base("End", Map)
        {
        }

        public override void OnSelect()
        {
            Map.OnNewPhase();
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
