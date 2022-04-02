using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Vehicle;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelPickVehicleSeat : ActionPanelDeathmatch
    {
        private const string PanelName = "PilotSeat";

        private readonly VehicleSeat ActiveSeat;
        private readonly Squad ActiveSquad;
        private readonly ActionPanel Owner;
        private readonly List<Vector3> ListMVHoverPoints;

        public ActionPanelPickVehicleSeat(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelPickVehicleSeat(DeathmatchMap Map, ActionPanel Owner, Squad ActiveSquad, List<Vector3> ListMVHoverPoints, VehicleSeat ActiveSeat)
            : base(PanelName, Map)
        {
            this.ActiveSquad = ActiveSquad;
            this.Owner = Owner;
            this.ListMVHoverPoints = ListMVHoverPoints;
            this.ActiveSeat = ActiveSeat;
        }

        public override void OnSelect()
        {
            Map.ListPlayer[Map.ActivePlayerIndex].ListSquad.Remove(ActiveSquad);
            ActiveSeat.User = ActiveSquad;

            Map.FinalizeMovement(ActiveSquad, (int)Map.GetTerrain(ActiveSquad).MovementCost, ListMVHoverPoints);
            ActiveSquad.EndTurn();
            ActiveSquad.CurrentLeader.UpdateSkillsLifetime(SkillEffect.LifetimeTypeOnAction);

            Map.ActiveSquadIndex = -1;
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
            return new ActionPanelPickVehicleSeat(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }

        public override string ToString()
        {
            return ActiveSeat.Name;
        }
    }
}
