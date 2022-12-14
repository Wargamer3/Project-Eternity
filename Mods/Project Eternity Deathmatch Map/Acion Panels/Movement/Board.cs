using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelBoard : ActionPanelDeathmatch
    {
        private const string PanelName = "Board";

        private UnitMapComponent TransportUnit;
        private Squad ActiveSquad;

        public ActionPanelBoard(DeathmatchMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelBoard(DeathmatchMap Map, UnitMapComponent TransportUnit, Squad ActiveSquad)
            : base(PanelName, Map, false)
        {
            this.TransportUnit = TransportUnit;
            this.ActiveSquad = ActiveSquad;
        }

        public override void OnSelect()
        {
            TransportUnit.ListTransportedUnit.Add(ActiveSquad);
            Map.ListPlayer[Map.ActivePlayerIndex].ListSquad.Remove(ActiveSquad);
            RemoveAllSubActionPanels();
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
            return new ActionPanelBoard(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
