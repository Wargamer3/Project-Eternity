using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelDeploy : ActionPanelDeathmatch
    {
        private const string PanelName = "Deploy";

        UnitMapComponent TransportUnit;

        public ActionPanelDeploy(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelDeploy(DeathmatchMap Map, UnitMapComponent TransportUnit)
            : base(PanelName, Map)
        {
            this.TransportUnit = TransportUnit;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        public override void Draw(CustomSpriteBatch g)
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
            return new ActionPanelDeploy(Map);
        }
    }
}
