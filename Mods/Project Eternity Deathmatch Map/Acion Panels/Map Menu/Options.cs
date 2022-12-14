using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

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
            return new ActionPanelOptions(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
