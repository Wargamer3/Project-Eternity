using Microsoft.Xna.Framework;
using ProjectEternity.Core;

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

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
