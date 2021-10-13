using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.WorldMapScreen
{
    public partial class UnitFactory
    {
        public class ActionPanelUpgrade : ActionPanelWorldMap
        {
            UnitFactory ActiveFactory;

            public ActionPanelUpgrade(WorldMap Map, UnitFactory ActiveFactory)
                : base("Upgrade Factory", Map, false)
            {
                this.ActiveFactory = ActiveFactory;
            }

            public override void OnSelect()
            {
                ++ActiveFactory.UpgadeLevel;
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
                return new ActionPanelUpgrade(Map, null);
            }


            public override void Draw(CustomSpriteBatch g)
            {
            }
        }
    }
}
