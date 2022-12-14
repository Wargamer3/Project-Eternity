using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.WorldMapScreen
{
    public partial class UnitFactory
    {
        public class ActionPanelSetWaypoint : ActionPanelWorldMap
        {
            private const string PanelName = "Set Waypoint Factory";

            UnitFactory ActiveFactory;

            public ActionPanelSetWaypoint(WorldMap Map)
                : base(PanelName, Map, false)
            {
            }

            public ActionPanelSetWaypoint(WorldMap Map, UnitFactory ActiveFactory)
                : base(PanelName, Map, false)
            {
                this.ActiveFactory = ActiveFactory;
            }

            public override void OnSelect()
            {
            }

            public override void DoUpdate(GameTime gameTime)
            {
                Map.MoveCursor();

                if (InputHelper.InputConfirmPressed())
                {
                    ActiveFactory.Waypoint = Map.CursorPosition;
                    Map.sndConfirm.Play();
                    RemoveFromPanelList(this);
                }
                else if (InputHelper.InputCancelPressed())
                {
                    Map.sndCancel.Play();
                }
            }

            public override void DoRead(ByteReader BR)
            {
            }

            public override void DoWrite(ByteWriter BW)
            {
            }

            protected override ActionPanel Copy()
            {
                return new ActionPanelSetWaypoint(Map);
            }

            public override void Draw(CustomSpriteBatch g)
            {
                g.Draw(Map.sprWaypoint, new Vector2((Map.CursorPosition.X - Map.CameraPosition.X) * Map.TileSize.X, (Map.CursorPosition.Y - Map.CameraPosition.Y) * Map.TileSize.Y), Color.White);
            }
        }
    }
}
