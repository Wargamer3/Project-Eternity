using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelPlayerEnemyUnitSelected : ActionPanelConquest
    {
        public ActionPanelPlayerEnemyUnitSelected(ConquestMap Map)
            : base("Player Enemy Unit Selected", Map)
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
            throw new System.NotImplementedException();
        }

        public override void DoWrite(ByteWriter BW)
        {
            throw new System.NotImplementedException();
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelPlayerEnemyUnitSelected(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
