using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelBuildCursor : ActionPanelLifeSimPlayer
    {
        readonly string Item;
        readonly bool Grabbing;
        readonly bool Deleting;

        bool Grid = true;
        int Floor;
        bool HasAnchor;
        Vector3 Anchor;
        bool Armed;
        FurnitureItem Held;
        Vector3 HeldHome;
        BuildResult Ghost = new BuildResult();

        public ActionPanelBuildCursor(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, string Item)
            : this(Owner, MapManager, ListActionMenuChoice, Item, false)
        {
        }

        public ActionPanelBuildCursor(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, string Item, bool Grabbing)
            : this(Owner, MapManager, ListActionMenuChoice, Item, Grabbing, false)
        {
        }

        public ActionPanelBuildCursor(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, string Item, bool Grabbing, bool Deleting)
            : base("BuildCursor", Owner, MapManager, ListActionMenuChoice, false)
        {
            this.Item = Item;
            this.Grabbing = Grabbing;
            this.Deleting = Deleting;
        }

        public override void OnSelect()
        {
            LifeSimMap Map = CurrentMap();
            if (Map != null)
            {
                Map.BuildPlacing = true;
                Map.HideFloorsAbove = true;
                Map.ShownFloor = Floor;
            }

            Frame();
        }

        public override void DoUpdate(GameTime gameTime)
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
            }
            else
            {
                Ground = new Vector3(Marker.Position.X, Marker.Position.Z, Marker.Position.Y);
                Map = CurrentMap();
                Tile = MapCursor.TileOf(Map);
            }

            if (Owner.KeyPressed(Keys.G))
            {
                Grid = !Grid;
                HasAnchor = false;
            }
            else if (Owner.PreviousPressed())
            {
                Floor = BuildFloor.Shift(Floor, -1);
                HasAnchor = false;
            }
            else if (Owner.NextPressed())
            {
                Floor = BuildFloor.Shift(Floor, 1);
                HasAnchor = false;
            }
            else if (Owner.BuildPressed())
            {
                PutBack();
                StopPlacing(Map);
                Owner.Session.CloseBuild();
                RemoveAllSubActionPanels();
                Owner.EndKeys();
                return;
            }
            else if (Armed && (ActiveInputManager.InputConfirmPressed() || Owner.KeyPressed(Keys.Enter)))
            {
                Place(Ground, Tile);
            }
            else if (Armed && ActiveInputManager.InputCancelPressed())
            {
                if (Held != null)
                {
                    PutBack();
                }
                else if (HasAnchor)
                {
                    HasAnchor = false;
                }
                else
                {
                    StopPlacing(Map);
                    RemoveFromPanelList(this);
                    Owner.EndKeys();
                    return;
                }
            }

            Armed = true;
            MapInfo Here = Marker.SharedMapContex == null ? null : Marker.SharedMapContex.CurrentMapInfo;
            Cursor = new Vector3(Ground.X, HeightField.View(Here, Ground.X, Ground.Y, Floor), Ground.Y);
            Owner.InvisibleCharacterAsCursor.SetPosition(Cursor);
            RefreshGhost(Ground, Tile);
            if (Map != null)
            {
                Map.BuildPlacing = true;
                Map.HideFloorsAbove = true;
                Map.ShownFloor = Floor;
            }

            MapCursor.ShowTile(Map, Ground, Tile);
            LifeSimFreeCamera.Frame(Owner.Camera, ChunkOrigin.Camera(Owner.InvisibleCharacterAsCursor.SharedMapContex.CurrentMapInfo, Cursor));
            Owner.EndKeys();
        }

        void RefreshGhost(Vector3 Ground, float Tile)
        {
            MapInfo Here = CurrentInfo();
            IList<FurnitureItem> Furniture = Here == null ? null : Here.Furniture;
            IList<BuildingItem> Buildings = Here == null ? null : Here.Buildings;
            if (Deleting)
            {
                BuildingItem Target = WallPlan.Under(Buildings, Ground, Floor);
                Ghost = new BuildResult();
                Ghost.BuildingGhost = Target;
                Ghost.Valid = false;
                Ghost.Reason = Target == null ? "Aim at a wall or floor" : "Enter deletes it";
                return;
            }

            if (Grabbing && Held == null)
            {
                FurnitureItem Under = BuildPlan.FurnitureAt(Furniture, Ground, Floor);
                Ghost = new BuildResult();
                Ghost.FurnitureGhost = Under;
                Ghost.Valid = Under != null;
                Ghost.Reason = Under == null ? "Aim at furniture" : "Enter picks it up";
                return;
            }

            if (Held != null)
            {
                Vector3 At = BuildFloor.SnapCenter(Ground, Tile, Grid);
                Held.Position = new Vector3(At.X, At.Y, BuildFloor.Surface(Floor));
                Held.Floor = Floor;
                Ghost = new BuildResult();
                Ghost.FurnitureGhost = Held;
                string Reason = BuildPlan.Refuse(Held, Furniture, Buildings);
                Ghost.Valid = Reason == null;
                Ghost.Reason = Reason;
                return;
            }

            Ghost = WallPlan.Handles(Item)
                ? WallPlan.Preview(Item, Ground, Anchor, HasAnchor, Floor, Grid, Tile, Buildings, Furniture, Here)
                : PlotSale.StandsAnywhere(Item)
                    ? BuildPlan.Preview(Item, Ground, Anchor, HasAnchor, Floor, Grid, Tile, Furniture, Buildings)
                    : BuildPlan.Preview(Item, Ground, Anchor, HasAnchor, Floor, Grid, Tile, Furniture, Buildings, Here);
            if (Ghost.Valid && !CanPay())
            {
                Ghost.Valid = false;
                Ghost.Reason = "Not enough money";
            }
        }

        void Place(Vector3 Ground, float Tile)
        {
            MapInfo Here = CurrentInfo();
            if (Deleting)
            {
                BuildingItem Target = WallPlan.Under(Here == null ? null : Here.Buildings, Ground, Floor);
                if (Target != null)
                {
                    BoxPlace.Drop(Here, Target);
                }

                return;
            }

            if (Grabbing && Held == null)
            {
                Held = BuildPlan.FurnitureAt(Here == null ? null : Here.Furniture, Ground, Floor);
                if (Held != null)
                {
                    HeldHome = Held.Position;
                    BoxPlace.Drop(Here, Held);
                }

                return;
            }

            if (Held != null)
            {
                if (Ghost != null && Ghost.Valid)
                {
                    BoxPlace.Keep(Here, Held);
                    Held = null;
                }

                return;
            }

            if (WallPlan.IsSpan(Item) && !HasAnchor)
            {
                Anchor = BuildFloor.SnapCorner(Ground, Tile, Grid);
                HasAnchor = true;
                return;
            }

            if (Ghost == null || !Ghost.Valid || !CanPay())
            {
                return;
            }

            IList<FurnitureItem> Furniture = Here == null ? null : Here.Furniture;
            IList<BuildingItem> Buildings = Here == null ? null : Here.Buildings;
            bool Anywhere = PlotSale.StandsAnywhere(Item);
            BuildResult Result = WallPlan.Handles(Item)
                ? WallPlan.Commit(Item, Ground, Anchor, HasAnchor, Floor, Grid, Tile, Buildings, Furniture, Here)
                : Anywhere
                    ? BuildPlan.Commit(Item, Ground, Anchor, HasAnchor, Floor, Grid, Tile, Furniture, Buildings)
                    : BuildPlan.Commit(Item, Ground, Anchor, HasAnchor, Floor, Grid, Tile, Furniture, Buildings, Here);
            if (!Result.Valid)
            {
                Ghost = Result;
                return;
            }

            BuildingZone Zone = BuildingZones.At(Here, Ground.X, Ground.Y);
            string Buyer = Owner.Session == null ? null : Owner.Session.HouseholdId;
            bool Admin = Owner.Session != null && Owner.Session.AdminOpen;
            if (!Anywhere && !PlotSale.MayBuild(Zone, Buyer, Admin))
            {
                Ghost = new BuildResult();
                Ghost.Reason = "This plot is not yours.";
                return;
            }

            Result.Apply(Here);
            if (MapManager.Home != null)
            {
                if (Item == "Garden Plot")
                {
                    MapManager.Home.Stations.Garden = true;
                }
                else if (Item == "Fishing Spot")
                {
                    MapManager.Home.Stations.Fishing = true;
                }
                else if (Item == "Forge")
                {
                    MapManager.Home.Stations.Forge = true;
                }
                else if (Item == "Alchemy Bench")
                {
                    MapManager.Home.Stations.Lab = true;
                }
                else if (Item == "Bed" || Item == "Tent")
                {
                    MapManager.Home.Stations.Beds++;
                }
            }

            int Cost = Price();
            if (Cost > 0 && Owner.Session != null)
            {
                Owner.Session.Funds -= Cost;
            }

            HasAnchor = false;
        }

        int Price()
        {
            CatalogItem Card = BuildCatalog.Find(Item);
            if (Card != null)
            {
                return Card.Price;
            }

            SurfaceItem Surface = WallCatalog.Find(Item);
            if (Surface != null)
            {
                return Surface.Price;
            }

            return 0;
        }

        bool CanPay()
        {
            if (Grabbing || Deleting || Held != null || Owner.Session == null)
            {
                return true;
            }

            return Owner.Session.Funds >= Price();
        }

        void PutBack()
        {
            if (Held == null)
            {
                return;
            }

            Held.Position = HeldHome;
            BoxPlace.Keep(CurrentInfo(), Held);
            Held = null;
        }

        void Frame()
        {
            Vector3 Cursor = Owner.InvisibleCharacterAsCursor.Position;
            Vector3 Ground = CursorGround.From(Cursor);
            MapCursor.ShowTile(CurrentMap(), Ground, MapCursor.TileOf(CurrentMap()));
            LifeSimFreeCamera.Frame(Owner.Camera, Cursor);
        }

        void StopPlacing(LifeSimMap Map)
        {
            if (Map == null)
            {
                return;
            }

            Map.BuildPlacing = false;
            Map.HideFloorsAbove = false;
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

        protected override void OnCancelPanel()
        {
            PutBack();
            StopPlacing(CurrentMap());
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelBuildCursor(Owner, MapManager, ListActionMenuChoice, Item, Grabbing, Deleting);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            if (Owner.Camera != null && Ghost != null && (Ghost.FurnitureGhost != null || Ghost.BuildingGhost != null))
            {
                g.End();
                Viewport Pane = g.GraphicsDevice.Viewport;
                float Aspect = Pane.Height > 0 ? Pane.Width / (float)Pane.Height : 1f;
                Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Aspect, 1f, 10000f);
                Vector3 Shift = ChunkOrigin.Shift(CurrentInfo());
                if (Ghost.FurnitureGhost != null)
                {
                    PlacedBoxDraw.Ghost(g.GraphicsDevice, Ghost.FurnitureGhost, Ghost.Valid, Owner.Camera.View, Projection, Shift);
                    float Tile = MapCursor.TileOf(CurrentMap());
                    PlacedBoxDraw.UseMarks(g.GraphicsDevice, FurnitureUse.Tiles(Ghost.FurnitureGhost, Tile), Tile, Owner.Camera.View, Projection, Shift);
                }
                else
                {
                    PlacedBoxDraw.Ghost(g.GraphicsDevice, Ghost.BuildingGhost, Ghost.Valid, Owner.Camera.View, Projection, Shift);
                }

                g.Begin();
            }

            MapManager.BeginOverlay(g);
            string GridLabel = Grid ? "Grid On" : "Grid Off";
            string Aim = Deleting ? "Delete" : Grabbing ? "Grab" : HasAnchor ? "Set the other end" : Item;
            TextHelper.DrawText(g, Aim + "   Floor " + (Floor + 1) + "   " + GridLabel, new Vector2(40, 20), Ghost != null && Ghost.Valid ? Color.White : Color.Red);
            string Hint = Deleting ? "Arrows move    Enter delete    Cancel back" : Grabbing ? "Arrows move    Enter pick or drop    Cancel back" : "Arrows move    G grid    Q/E floor    Enter place    Cancel back";
            bool OutsideZone = Ghost != null && !Ghost.Valid && Ghost.Reason == "This spot is outside a building zone.";
            if (!OutsideZone && Ghost != null && !string.IsNullOrEmpty(Ghost.Reason) && !Ghost.Valid)
            {
                Hint = Ghost.Reason;
            }

            TextHelper.DrawText(g, Hint, new Vector2(40, 48), Color.LightGray);
            if (OutsideZone)
            {
                TextHelper.DrawText(g, Ghost.Reason, new Vector2(40, 104), Color.OrangeRed);
            }
            if (Owner.Session != null)
            {
                TextHelper.DrawText(g, "Funds  " + Owner.Session.Funds, new Vector2(40, 76), Color.Gold);
            }
        }
    }
}
