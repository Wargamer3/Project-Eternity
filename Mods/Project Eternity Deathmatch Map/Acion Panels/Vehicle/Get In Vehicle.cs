using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Vehicle;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelGetInVehicle : ActionPanelDeathmatch
    {
        private const string PanelName = "Get In Vehicle";

        private readonly Squad ActiveSquad;
        private readonly List<Vector3> ListMVHoverPoint;
        private readonly ActionPanel Owner;
        private readonly Vehicle ActiveVehicle;

        public ActionPanelGetInVehicle(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelGetInVehicle(DeathmatchMap Map, ActionPanel Owner, Squad ActiveSquad, List<Vector3> ListMVHoverPoint, Vehicle ActiveVehicle)
            : base(PanelName, Map)
        {
            this.ActiveSquad = ActiveSquad;
            this.Owner = Owner;
            this.ListMVHoverPoint = ListMVHoverPoint;
            this.ActiveVehicle = ActiveVehicle;
        }

        public override void OnSelect()
        {
            foreach (VehicleSeat ActiveSeat in ActiveVehicle.ListSeat)
            {
                if (ActiveSeat.User == null)
                {
                    AddChoiceToCurrentPanel(new ActionPanelPickVehicleSeat(Map, Owner, ActiveSquad, ListMVHoverPoint, ActiveSeat));
                }
            }
        }

        public static void AddIfUsable(DeathmatchMap Map, ActionPanel Owner, Squad ActiveSquad, List<Vector3> ListMVHoverPoint)
        {
            foreach (Vehicle ActiveVehicle in Map.ListVehicle)
            {
                Owner.AddChoiceToCurrentPanel(new ActionPanelGetInVehicle(Map, Owner, ActiveSquad, ListMVHoverPoint, ActiveVehicle));
                return;
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            NavigateThroughNextChoices(Map.sndSelection, Map.sndConfirm);
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelGetInVehicle(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawNextChoice(g);
        }
    }
}
