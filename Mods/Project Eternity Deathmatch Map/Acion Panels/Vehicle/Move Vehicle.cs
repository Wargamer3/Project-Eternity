using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Vehicle;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelMoveVehicle : ActionPanelDeathmatch
    {
        private const string PanelName = "MoveVehicle";

        private readonly Vehicle ActiveVehicle;

        public ActionPanelMoveVehicle(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelMoveVehicle(DeathmatchMap Map, Vehicle ActiveVehicle)
            : base(PanelName, Map)
        {
            this.ActiveVehicle = ActiveVehicle;
        }

        public override void OnSelect()
        {
        }

        public static void AddIfUsable(DeathmatchMap Map, ActionPanel Owner)
        {
            foreach (Vehicle ActiveVehicle in Map.ListVehicle)
            {
                Vector3 CenterPos = new Vector3(ActiveVehicle.Position.X / Map.TileSize.X, ActiveVehicle.Position.Z / 32, ActiveVehicle.Position.Y / Map.TileSize.Y);
                Vector3 MinPos = CenterPos - new Vector3(ActiveVehicle.sprVehicle.Width / Map.TileSize.X, ActiveVehicle.sprVehicle.Height / Map.TileSize.Y, 0);
                Vector3 MaxPos = CenterPos + new Vector3(ActiveVehicle.sprVehicle.Width / Map.TileSize.X, ActiveVehicle.sprVehicle.Height / Map.TileSize.Y, 0);

                if (Map.CursorTerrain.WorldPosition.X >= MinPos.X && Map.CursorTerrain.WorldPosition.Y >= MinPos.Y && Map.CursorTerrain.WorldPosition.Z >= MinPos.Z
                    && Map.CursorTerrain.WorldPosition.X <= MaxPos.X && Map.CursorTerrain.WorldPosition.Y <= MaxPos.Y && Map.CursorTerrain.WorldPosition.Z <= MaxPos.Z)
                {
                    ActionPanelMoveVehicle NewPanel = new ActionPanelMoveVehicle(Map, ActiveVehicle);
                    NewPanel.AddChoiceToCurrentPanel(new ActionPanelControlVehicle(Map, ActiveVehicle));
                    Owner.AddToPanelList(NewPanel);
                    return;
                }
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
            return new ActionPanelMoveVehicle(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawNextChoice(g);
        }
    }
}
