using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelMainMenuSave : ActionPanelConquest
    {
        public ActionPanelMainMenuSave(ConquestMap Map)
            : base("Save", Map)
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
            return new ActionPanelMainMenuSave(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
