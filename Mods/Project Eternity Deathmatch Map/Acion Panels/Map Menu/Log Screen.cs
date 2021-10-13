using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

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

            public override void DoRead(ByteReader BR)
            {
            }

            public override void DoWrite(ByteWriter BW)
            {
            }

            protected override ActionPanel Copy()
            {
                return new ActionPanelDebugScreen(Map);
            }

            public override void Draw(CustomSpriteBatch g)
            {
            }
        }
    }
}
