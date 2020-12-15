using Microsoft.Xna.Framework;
using ProjectEternity.Core;

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

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
