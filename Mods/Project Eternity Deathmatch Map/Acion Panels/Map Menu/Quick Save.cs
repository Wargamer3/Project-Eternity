using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelQuickSave : ActionPanelDeathmatch
    {
        public ActionPanelQuickSave(DeathmatchMap Map)
            : base("Quick Save", Map, false)
        {
        }

        public override void OnSelect()
        {
            Map.SaveTemporaryMap();
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
            return new ActionPanelQuickSave(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
