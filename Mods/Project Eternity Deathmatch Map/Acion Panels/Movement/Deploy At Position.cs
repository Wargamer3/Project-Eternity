using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelDeployAtPosition : ActionPanelDeathmatch
    {
        private const string PanelName = "Deploy 2";

        private Squad ActiveSquad;

        public ActionPanelDeployAtPosition(DeathmatchMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelDeployAtPosition(DeathmatchMap Map, Squad ActiveSquad)
            : base(PanelName, Map, false)
        {
            this.ActiveSquad = ActiveSquad;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Map.ListPlayer[Map.ActivePlayerIndex].ListSquad.Remove(ActiveSquad);
            RemoveAllSubActionPanels();
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelDeployAtPosition(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
