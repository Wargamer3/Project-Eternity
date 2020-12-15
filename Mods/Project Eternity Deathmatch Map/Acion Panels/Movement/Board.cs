using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelBoard : ActionPanelDeathmatch
    {
        private UnitMapComponent TransportUnit;
        private Squad ActiveSquad;

        public ActionPanelBoard(DeathmatchMap Map, UnitMapComponent TransportUnit, Squad ActiveSquad)
            : base("Board", Map, false)
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

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
