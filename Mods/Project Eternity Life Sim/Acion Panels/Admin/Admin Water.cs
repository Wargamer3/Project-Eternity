using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelWater : ActionPanelLifeSimPlayer
    {
        public ActionPanelWater(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base("Water", Owner, MapManager, ListActionMenuChoice, true)
        {
        }

        public override void OnSelect()
        {
            AddChoiceToCurrentPanel(new ActionPanelWaterKind(Owner, MapManager, ListActionMenuChoice, WaterLook.Tropical));
            AddChoiceToCurrentPanel(new ActionPanelWaterKind(Owner, MapManager, ListActionMenuChoice, WaterLook.Northern));
            AddChoiceToCurrentPanel(new ActionPanelWaterKind(Owner, MapManager, ListActionMenuChoice, WaterLook.Cave));
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (NavigateThroughNextChoices(MapManager.sndSelection))
            {
            }
            else if (ConfirmNextChoices(MapManager.sndConfirm))
            {
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
            return new ActionPanelWater(Owner, MapManager, ListActionMenuChoice);
        }

        public override string ToString()
        {
            return "Water";
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawNextChoice(g);
        }
    }

    public class ActionPanelWaterKind : ActionPanelLifeSimPlayer
    {
        readonly WaterLook Look;

        public ActionPanelWaterKind(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, WaterLook Look)
            : base("WaterKind", Owner, MapManager, ListActionMenuChoice, true)
        {
            this.Look = Look;
        }

        public override void OnSelect()
        {
            AddChoiceToCurrentPanel(new ActionPanelPlaceWater(Owner, MapManager, ListActionMenuChoice, Look, false));
            AddChoiceToCurrentPanel(new ActionPanelPlaceWater(Owner, MapManager, ListActionMenuChoice, Look, true));
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (NavigateThroughNextChoices(MapManager.sndSelection))
            {
            }
            else if (ConfirmNextChoices(MapManager.sndConfirm))
            {
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
            return new ActionPanelWaterKind(Owner, MapManager, ListActionMenuChoice, Look);
        }

        public override string ToString()
        {
            if (Look == WaterLook.Northern)
            {
                return "Northern";
            }

            if (Look == WaterLook.Cave)
            {
                return "Cave";
            }

            return "Tropical";
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawNextChoice(g);
        }
    }

    public class ActionPanelPlaceWater : ActionPanelLifeSimPlayer
    {
        readonly WaterLook Look;
        readonly bool River;
        readonly WaterTool Tool = new WaterTool();
        bool Ready;

        public ActionPanelPlaceWater(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, WaterLook Look, bool River)
            : base("PlaceWater", Owner, MapManager, ListActionMenuChoice, false)
        {
            this.Look = Look;
            this.River = River;
            Tool.Look = Look;
        }

        public override void OnSelect()
        {
            Ready = false;
            Tool.Look = Look;
            WaterSession.Preview = null;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (!Ready)
            {
                Ready = true;
                Owner.EndKeys();
                return;
            }

            MoveCursor(gameTime);
            Vector3 Ground = CursorGround.From(Owner.InvisibleCharacterAsCursor.Position);
            WaterSession.Preview = River ? Tool.Ghost(Ground) : null;
            if (ActiveInputManager.InputCancelPressed())
            {
                if (Tool.HasStart)
                {
                    Tool.Cancel();
                    WaterSession.Preview = null;
                }
                else
                {
                    WaterSession.Preview = null;
                    MapManager.sndCancel.Play();
                    RemoveFromPanelList(this);
                }

                Owner.EndKeys();
                return;
            }

            if (Owner.KeyPressed(Keys.Enter) || ActiveInputManager.InputConfirmPressed())
            {
                MapInfo Here = CurrentInfo();
                if (Here != null && Here.Water != null)
                {
                    Tool.Confirm(Here.Water, Ground, River);
                }
            }

            Owner.EndKeys();
        }

        protected override void OnCancelPanel()
        {
            WaterSession.Preview = null;
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelPlaceWater(Owner, MapManager, ListActionMenuChoice, Look, River);
        }

        public override string ToString()
        {
            return River ? "River" : "Still Water";
        }

        public override void Draw(CustomSpriteBatch g)
        {
            string Line = River ? (Tool.HasStart ? "Confirm the end" : "Confirm the start") : "Confirm still water";
            TextHelper.DrawText(g, Line, new Vector2(40, 88), Color.White);
        }

        void MoveCursor(GameTime gameTime)
        {
            float Delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector3 Cursor = MapCursor.Slide(Owner.InvisibleCharacterAsCursor.Position, Delta, ActiveInputManager.InputUpHold(), ActiveInputManager.InputDownHold(), ActiveInputManager.InputLeftHold(), ActiveInputManager.InputRightHold());
            Vector3 Ground = CursorGround.From(Cursor);
            PlayerCharacter Marker = Owner.InvisibleCharacterAsCursor;
            LifeSimMap Map = CurrentMap();
            float Tile = MapCursor.TileOf(Map);
            if (!ChunkCross.TryMoveCursor(Marker, Ground.X, Ground.Y, Ground.Z))
            {
                Ground = MapCursor.Clamp(Ground, Map, Tile);
                Marker.SetPosition(new Vector3(Ground.X, Ground.Z, Ground.Y));
            }

            Map = CurrentMap();
            Tile = MapCursor.TileOf(Map);
            Ground = new Vector3(Marker.Position.X, Marker.Position.Z, Marker.Position.Y);
            MapCursor.ShowTile(Map, Ground, Tile);
            if (Owner.Camera == null && GameScreen.GraphicsDevice != null)
            {
                Owner.Camera = new LifeSimFreeCamera(GameScreen.GraphicsDevice, Owner);
            }

            LifeSimFreeCamera.Frame(Owner.Camera, ChunkOrigin.Camera(CurrentInfo(), Marker.Position));
        }

        MapInfo CurrentInfo()
        {
            if (Owner.InvisibleCharacterAsCursor.SharedMapContex == null)
            {
                return null;
            }

            return Owner.InvisibleCharacterAsCursor.SharedMapContex.CurrentMapInfo;
        }

        LifeSimMap CurrentMap()
        {
            MapInfo Here = CurrentInfo();
            if (Here == null || Here.MapMapContainer == null)
            {
                return null;
            }

            return Here.MapMapContainer.ActiveMap;
        }
    }
}
