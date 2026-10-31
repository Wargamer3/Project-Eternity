using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public enum TerrainStroke
    {
        Raise,
        Lower,
        Smooth,
        Flatten,
        Grass,
        Dirt,
        Rock,
        Snow,
    }

    public class ActionPanelTerrain : ActionPanelLifeSimPlayer
    {
        public ActionPanelTerrain(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base("Terrain", Owner, MapManager, ListActionMenuChoice, true)
        {
        }

        public override void OnSelect()
        {
            AddChoiceToCurrentPanel(new ActionPanelTerrainStroke(Owner, MapManager, ListActionMenuChoice, TerrainStroke.Raise));
            AddChoiceToCurrentPanel(new ActionPanelTerrainStroke(Owner, MapManager, ListActionMenuChoice, TerrainStroke.Lower));
            AddChoiceToCurrentPanel(new ActionPanelTerrainStroke(Owner, MapManager, ListActionMenuChoice, TerrainStroke.Smooth));
            AddChoiceToCurrentPanel(new ActionPanelTerrainStroke(Owner, MapManager, ListActionMenuChoice, TerrainStroke.Flatten));
            AddChoiceToCurrentPanel(new ActionPanelTerrainStroke(Owner, MapManager, ListActionMenuChoice, TerrainStroke.Grass));
            AddChoiceToCurrentPanel(new ActionPanelTerrainStroke(Owner, MapManager, ListActionMenuChoice, TerrainStroke.Dirt));
            AddChoiceToCurrentPanel(new ActionPanelTerrainStroke(Owner, MapManager, ListActionMenuChoice, TerrainStroke.Rock));
            AddChoiceToCurrentPanel(new ActionPanelTerrainStroke(Owner, MapManager, ListActionMenuChoice, TerrainStroke.Snow));
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
            return new ActionPanelTerrain(Owner, MapManager, ListActionMenuChoice);
        }

        public override string ToString()
        {
            return "Terrain";
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawNextChoice(g);
        }
    }

    public class ActionPanelTerrainStroke : ActionPanelLifeSimPlayer
    {
        readonly TerrainStroke Stroke;
        bool Ready;

        public ActionPanelTerrainStroke(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, TerrainStroke Stroke)
            : base("TerrainStroke", Owner, MapManager, ListActionMenuChoice, false)
        {
            this.Stroke = Stroke;
        }

        public override void OnSelect()
        {
            Ready = false;
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
            if (ActiveInputManager.InputCancelPressed())
            {
                MapManager.sndCancel.Play();
                RemoveFromPanelList(this);
                Owner.EndKeys();
                return;
            }

            bool Held = Mouse.GetState().LeftButton == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Enter) || ActiveInputManager.InputConfirmPressed();
            if (Held)
            {
                Apply();
            }

            Owner.EndKeys();
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelTerrainStroke(Owner, MapManager, ListActionMenuChoice, Stroke);
        }

        public override string ToString()
        {
            return Stroke.ToString();
        }

        public override void Draw(CustomSpriteBatch g)
        {
            TextHelper.DrawText(g, Stroke.ToString() + " the ground", new Vector2(40, 88), Color.White);
        }

        void Apply()
        {
            MapInfo Here = CurrentInfo();
            if (Here == null)
            {
                return;
            }

            if (Here.Heights == null && WorldStream.Slot != null)
            {
                Here.Heights = HeightField.Create(WorldStream.Slot.ChunkSize, WorldStream.Slot.Spacing, 0f);
                Here.Kind = GroundKind.Heightmap;
            }

            if (Here.Heights == null)
            {
                return;
            }

            Vector3 Ground = CursorGround.From(Owner.InvisibleCharacterAsCursor.Position);
            const float Radius = 28f;
            const float Amount = 0.35f;
            if (Stroke == TerrainStroke.Raise)
            {
                Here.Heights.Raise(Ground.X, Ground.Y, Radius, Amount);
            }
            else if (Stroke == TerrainStroke.Lower)
            {
                Here.Heights.Raise(Ground.X, Ground.Y, Radius, -Amount);
            }
            else if (Stroke == TerrainStroke.Smooth)
            {
                Here.Heights.Smooth(Ground.X, Ground.Y, Radius);
            }
            else if (Stroke == TerrainStroke.Flatten)
            {
                Here.Heights.Flatten(Ground.X, Ground.Y, Radius);
            }
            else
            {
                Vector4 Channel = new Vector4(1f, 0f, 0f, 0f);
                if (Stroke == TerrainStroke.Dirt)
                {
                    Channel = new Vector4(0f, 1f, 0f, 0f);
                }
                else if (Stroke == TerrainStroke.Rock)
                {
                    Channel = new Vector4(0f, 0f, 1f, 0f);
                }
                else if (Stroke == TerrainStroke.Snow)
                {
                    Channel = new Vector4(0f, 0f, 0f, 1f);
                }

                Here.Heights.Paint(Ground.X, Ground.Y, Radius, Channel);
            }

            Here.Heights.Rebuild();
            SyncEdge(Here, Ground, Radius);
            WorldStream.Submit(Here);
            WorldStream.Save(Here);
        }

        void SyncEdge(MapInfo Here, Vector3 Ground, float Radius)
        {
            bool East;
            bool West;
            bool North;
            bool South;
            if (Here.Heights == null || !Here.Heights.TouchesEdge(Ground.X, Ground.Y, Radius, out East, out West, out North, out South))
            {
                return;
            }

            if (East)
            {
                Copy(Here, 1, 0, 0);
            }

            if (West)
            {
                Copy(Here, -1, 0, 1);
            }

            if (North)
            {
                Copy(Here, 0, 1, 2);
            }

            if (South)
            {
                Copy(Here, 0, -1, 3);
            }
        }

        void Copy(MapInfo Here, int DX, int DZ, int Side)
        {
            MapInfo Neighbor = WorldStream.ChunkAt(Here.ChunkX + DX, Here.ChunkZ + DZ, true);
            if (Neighbor.Heights == null && WorldStream.Slot != null)
            {
                Neighbor.Heights = HeightField.Create(WorldStream.Slot.ChunkSize, WorldStream.Slot.Spacing, 0f);
            }

            if (Neighbor.Heights == null)
            {
                return;
            }

            Neighbor.Kind = GroundKind.Heightmap;
            Here.Heights.CopyEdge(Neighbor.Heights, Side);
            Neighbor.Heights.Rebuild();
            WorldStream.OpenView(Neighbor);
            WorldStream.Submit(Neighbor);
            WorldStream.Save(Neighbor);
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
            if (Owner.InvisibleCharacterAsCursor == null || Owner.InvisibleCharacterAsCursor.SharedMapContex == null)
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

    public enum PropBrushMode
    {
        AddBox,
        Clip,
        Subtract,
    }

    public class ActionPanelPropBrush : ActionPanelLifeSimPlayer
    {
        public ActionPanelPropBrush(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base("PropBrush", Owner, MapManager, ListActionMenuChoice, true)
        {
        }

        public override void OnSelect()
        {
            AddChoiceToCurrentPanel(new ActionPanelPropStroke(Owner, MapManager, ListActionMenuChoice, PropBrushMode.AddBox));
            AddChoiceToCurrentPanel(new ActionPanelPropStroke(Owner, MapManager, ListActionMenuChoice, PropBrushMode.Clip));
            AddChoiceToCurrentPanel(new ActionPanelPropStroke(Owner, MapManager, ListActionMenuChoice, PropBrushMode.Subtract));
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
            return new ActionPanelPropBrush(Owner, MapManager, ListActionMenuChoice);
        }

        public override string ToString()
        {
            return "Prop Brush";
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawNextChoice(g);
        }
    }

    public class ActionPanelPropStroke : ActionPanelLifeSimPlayer
    {
        readonly PropBrushMode Mode;
        bool Ready;

        public ActionPanelPropStroke(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, PropBrushMode Mode)
            : base("PropStroke", Owner, MapManager, ListActionMenuChoice, false)
        {
            this.Mode = Mode;
        }

        public override void OnSelect()
        {
            Ready = false;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (!Ready)
            {
                Ready = true;
                Owner.EndKeys();
                return;
            }

            if (ActiveInputManager.InputCancelPressed())
            {
                MapManager.sndCancel.Play();
                RemoveFromPanelList(this);
                Owner.EndKeys();
                return;
            }

            if (ActiveInputManager.InputConfirmPressed() || Owner.KeyPressed(Keys.Enter))
            {
                MapInfo Here = CurrentInfo();
                if (Here != null)
                {
                    Vector3 Ground = CursorGround.From(Owner.InvisibleCharacterAsCursor.Position);
                    FurnitureItem Item = PropBrushTool.Ensure(Here, Ground);
                    if (Mode == PropBrushMode.AddBox)
                    {
                        PropBrushTool.AddBox(Item, Ground);
                    }
                    else if (Mode == PropBrushMode.Clip)
                    {
                        PropBrushTool.Clip(Item);
                    }
                    else
                    {
                        PropBrushTool.Subtract(Item);
                    }

                    WorldStream.Save(Here);
                }
            }

            Owner.EndKeys();
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelPropStroke(Owner, MapManager, ListActionMenuChoice, Mode);
        }

        public override string ToString()
        {
            if (Mode == PropBrushMode.AddBox)
            {
                return "Add Box";
            }

            if (Mode == PropBrushMode.Clip)
            {
                return "Clip Wedge";
            }

            return "Subtract";
        }

        public override void Draw(CustomSpriteBatch g)
        {
            TextHelper.DrawText(g, ToString(), new Vector2(40, 88), Color.White);
        }

        MapInfo CurrentInfo()
        {
            if (Owner.InvisibleCharacterAsCursor == null || Owner.InvisibleCharacterAsCursor.SharedMapContex == null)
            {
                return null;
            }

            return Owner.InvisibleCharacterAsCursor.SharedMapContex.CurrentMapInfo;
        }
    }

    public class ActionPanelGenerateMap : ActionPanelLifeSimPlayer
    {
        WorldGenerator Job;
        bool Finished;
        int Replay;

        public ActionPanelGenerateMap(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, int Replay)
            : base("GenerateMap", Owner, MapManager, ListActionMenuChoice, true)
        {
            this.Replay = Replay;
        }

        public override void OnSelect()
        {
            Job = Replay == 0 ? WorldGenerator.Begin() : WorldGenerator.Begin(Replay);
            Finished = false;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (Job != null && !Finished)
            {
                Finished = Job.Step();
                if (Finished)
                {
                    MapManager.ReloadWorld();
                }
            }

            if (NavigateThroughNextChoices(MapManager.sndSelection))
            {
            }
            else if (ConfirmNextChoices(MapManager.sndConfirm))
            {
            }

            if (ActiveInputManager.InputCancelPressed())
            {
                RemoveFromPanelList(this);
            }

            if (Finished)
            {
                RemoveAllSubActionPanels();
            }

            Owner.EndKeys();
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelGenerateMap(Owner, MapManager, ListActionMenuChoice, Replay);
        }

        public override string ToString()
        {
            if (Replay != 0)
            {
                return "Regenerate " + Replay.ToString();
            }

            return "Generate New Map";
        }

        public override void Draw(CustomSpriteBatch g)
        {
            string Line = Job == null ? "Generate New Map" : Job.Status;
            TextHelper.DrawText(g, Line, new Vector2(40, 88), Color.White);
            if (Job == null || g.GraphicsDevice == null)
            {
                return;
            }

            Texture2D Picture = Job.Preview(g.GraphicsDevice);
            if (Picture != null)
            {
                g.Draw(Picture, new Rectangle(40, 160, 220, 220), Color.White);
            }
        }
    }
}
