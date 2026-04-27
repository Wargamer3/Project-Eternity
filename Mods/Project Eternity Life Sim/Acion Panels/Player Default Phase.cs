using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelPlayerDefault : ActionPanelLifeSim
    {
        private const string PanelName = "PlayerDefault";


        public ActionPanelPlayerDefault(LifeSimMap Map)
            : base(PanelName, Map, false)
        {
        }

        public override void OnSelect()
        {
        }


        public override void DoUpdate(GameTime gameTime)
        {
        }


        protected override void OnCancelPanel()
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
            return new ActionPanelPlayerDefault(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
