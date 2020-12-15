using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    partial class DeathmatchMap
    {
        public class ActionPanelDebugScreen : ActionPanelDeathmatch
        {

            public ActionPanelDebugScreen(DeathmatchMap Map)
                : base("Log", Map)
            {
            }

            public override void OnSelect()
            {
                Map.PushScreen(Debug);
                Debug.Init();
                RemoveFromPanelList(this);
            }

            public override void DoUpdate(GameTime gameTime)
            {

            }

            public override void Draw(CustomSpriteBatch g)
            {
            }
        }
    }
}
