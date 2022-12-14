using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelMainMenuIntel : ActionPanelConquest
    {
        public ActionPanelMainMenuIntel(ConquestMap Map)
            : base("Intel", Map)
        {
        }

        public override void OnSelect()
        {
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
            return new ActionPanelMainMenuIntel(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
