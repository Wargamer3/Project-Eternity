using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelOptions : ActionPanelDeathmatch
    {
        public ActionPanelOptions(DeathmatchMap Map)
            : base("Options", Map, false)
        {
        }

        public override void OnSelect()
        {
            Map.PushScreen(new OptionMenu());
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
            return new ActionPanelOptions(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
