using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelBuildingZone : ActionPanelLifeSimPlayer
    {
        enum Stage
        {
            First,
            Second,
            Entry,
            Name,
            Price,
            Access,
        }

        Stage Step;
        Vector3 FirstCorner;
        Vector3 SecondCorner;
        MapInfo DoorChunk;
        Vector3 Door;
        string Typed = "";
        string PriceText = "";
        int ZonePrice;

        public ActionPanelBuildingZone(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base("BuildingZone", Owner, MapManager, ListActionMenuChoice, true)
        {
        }

        public override void OnSelect()
        {
            Step = Stage.First;
            Typed = "";
            Owner.ZoneDraft = new ZoneDraft();
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (Step == Stage.Access)
            {
                if (NavigateThroughNextChoices(MapManager.sndSelection))
                {
                }
                else if (ConfirmNextChoices(MapManager.sndConfirm))
                {
                }

                Owner.EndKeys();
                return;
            }

            if (Step == Stage.Price)
            {
                TypePrice();
                if (ConfirmPressed())
                {
                    int Parsed;
                    ZonePrice = int.TryParse(PriceText, out Parsed) ? Parsed : 0;
                    Step = Stage.Access;
                    AddChoiceToCurrentPanel(new ActionPanelZoneAccess(Owner, MapManager, ListActionMenuChoice, this, true));
                    AddChoiceToCurrentPanel(new ActionPanelZoneAccess(Owner, MapManager, ListActionMenuChoice, this, false));
                }

                Owner.EndKeys();
                return;
            }

            if (Step == Stage.Name)
            {
                TypeName();
                if (ConfirmPressed() && Typed.Length > 0)
                {
                    Step = Stage.Price;
                    PriceText = "";
                }

                Owner.EndKeys();
                return;
            }

            MoveCursor(gameTime);
            RememberDraft();
            if (ConfirmPressed())
            {
                Vector3 World = WorldPoint();
                if (Step == Stage.First)
                {
                    FirstCorner = World;
                    Step = Stage.Second;
                }
                else if (Step == Stage.Second)
                {
                    SecondCorner = World;
                    if (BuildingZones.Cover(MapManager.AllChunks, FirstCorner, SecondCorner).Count == 0)
                    {
                        Owner.EndKeys();
                        return;
                    }

                    Step = Stage.Entry;
                }
                else if (Step == Stage.Entry)
                {
                    DoorChunk = CurrentInfo();
                    PlayerCharacter Marker = Owner.InvisibleCharacterAsCursor;
                    Door = new Vector3(Marker.Position.X, Marker.Position.Z, Marker.Position.Y);
                    Step = Stage.Name;
                }
            }

            Owner.EndKeys();
        }

        public void Finish(bool Public)
        {
            BuildingZone Zone = new BuildingZone();
            Zone.Name = Typed;
            Zone.Public = Public;
            Zone.Price = ZonePrice;
            Zone.EntryChunk = DoorChunk;
            Zone.Entry = Door;
            Zone.Footprints.AddRange(BuildingZones.Cover(MapManager.AllChunks, FirstCorner, SecondCorner));
            BuildingZones.Add(MapManager.Zones, Zone);
            Owner.ZoneDraft = null;
            RemoveFromPanelList(this);
        }

        protected override void OnCancelPanel()
        {
            Owner.ZoneDraft = null;
            base.OnCancelPanel();
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelBuildingZone(Owner, MapManager, ListActionMenuChoice);
        }

        public override string ToString()
        {
            return "Building Zone";
        }

        public override void Draw(CustomSpriteBatch g)
        {
            string Line = "Confirm the first corner";
            if (Step == Stage.Second)
            {
                Line = "Confirm the other corner";
            }
            else if (Step == Stage.Entry)
            {
                Line = "Confirm the door";
            }
            else if (Step == Stage.Name)
            {
                Line = "Name: " + Typed;
            }
            else if (Step == Stage.Price)
            {
                Line = "Price: " + PriceText;
            }
            else if (Step == Stage.Access)
            {
                Line = Typed;
            }

            TextHelper.DrawText(g, Line, new Vector2(40, 88), Color.White);
            if (Step == Stage.Access)
            {
                DrawNextChoice(g);
            }
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

        void RememberDraft()
        {
            if (Owner.ZoneDraft == null)
            {
                Owner.ZoneDraft = new ZoneDraft();
            }

            ZoneDraft Draft = Owner.ZoneDraft;
            Vector3 World = WorldPoint();
            if (Step == Stage.First)
            {
                Draft.HasCorner = true;
                Draft.Corner = World;
                Draft.ShowRectangle = false;
                Draft.ShowEntry = false;
                return;
            }

            Draft.HasCorner = true;
            Draft.Corner = FirstCorner;
            Draft.Opposite = Step == Stage.Second ? World : SecondCorner;
            Draft.ShowRectangle = true;
            Draft.ShowEntry = Step == Stage.Entry;
            if (Draft.ShowEntry)
            {
                Draft.Entry = World;
            }
        }

        void TypePrice()
        {
            if (!Owner.OwnsKeyboard)
            {
                return;
            }

            if (Owner.KeyPressed(Keys.Back) && PriceText.Length > 0)
            {
                PriceText = PriceText.Substring(0, PriceText.Length - 1);
            }

            if (PriceText.Length >= 7)
            {
                return;
            }

            for (Keys Key = Keys.D0; Key <= Keys.D9; Key++)
            {
                if (Owner.KeyPressed(Key))
                {
                    PriceText += (char)('0' + (Key - Keys.D0));
                }
            }
        }

        void TypeName()
        {
            if (!Owner.OwnsKeyboard)
            {
                return;
            }

            if (Owner.KeyPressed(Keys.Back) && Typed.Length > 0)
            {
                Typed = Typed.Substring(0, Typed.Length - 1);
            }

            if (Typed.Length >= 24)
            {
                return;
            }

            for (Keys Key = Keys.A; Key <= Keys.Z; Key++)
            {
                if (Owner.KeyPressed(Key))
                {
                    Typed += Key.ToString();
                }
            }

            for (Keys Key = Keys.D0; Key <= Keys.D9; Key++)
            {
                if (Owner.KeyPressed(Key))
                {
                    Typed += (char)('0' + (Key - Keys.D0));
                }
            }

            if (Owner.KeyPressed(Keys.Space) && Typed.Length > 0)
            {
                Typed += " ";
            }
        }

        bool ConfirmPressed()
        {
            return Owner.KeyPressed(Keys.Enter) || ActiveInputManager.InputConfirmPressed();
        }

        Vector3 WorldPoint()
        {
            PlayerCharacter Marker = Owner.InvisibleCharacterAsCursor;
            Vector3 Shift = ChunkOrigin.Shift(CurrentInfo());
            return new Vector3(Marker.Position.X + Shift.X, Marker.Position.Z + Shift.Z, 0f);
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

    public class ActionPanelZoneAccess : ActionPanelLifeSimPlayer
    {
        readonly ActionPanelBuildingZone Parent;
        readonly bool Public;

        public ActionPanelZoneAccess(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, ActionPanelBuildingZone Parent, bool Public)
            : base("ZoneAccess", Owner, MapManager, ListActionMenuChoice, false)
        {
            this.Parent = Parent;
            this.Public = Public;
        }

        public override void OnSelect()
        {
            if (Parent != null)
            {
                Parent.Finish(Public);
            }

            RemoveFromPanelList(this);
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        protected override void OnCancelPanel()
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
            return new ActionPanelZoneAccess(Owner, MapManager, ListActionMenuChoice, Parent, Public);
        }

        public override string ToString()
        {
            return Public ? "Public" : "Private";
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }

    public class ActionPanelDeleteBuildingZone : ActionPanelLifeSimPlayer
    {
        public ActionPanelDeleteBuildingZone(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base("DeleteBuildingZone", Owner, MapManager, ListActionMenuChoice, true)
        {
        }

        public override void OnSelect()
        {
            MapInfo Here = Owner.InvisibleCharacterAsCursor.SharedMapContex == null ? null : Owner.InvisibleCharacterAsCursor.SharedMapContex.CurrentMapInfo;
            PlayerCharacter Marker = Owner.InvisibleCharacterAsCursor;
            BuildingZone Zone = BuildingZones.At(Here, Marker.Position.X, Marker.Position.Z);
            RemoveFromPanelList(this);
            if (Zone == null)
            {
                return;
            }

            AddToPanelListAndSelect(new ActionPanelConfirmDeleteZone(Owner, MapManager, ListActionMenuChoice, Zone));
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        protected override void OnCancelPanel()
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
            return new ActionPanelDeleteBuildingZone(Owner, MapManager, ListActionMenuChoice);
        }

        public override string ToString()
        {
            return "Delete Building Zone";
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }

    public class ActionPanelConfirmDeleteZone : ActionPanelLifeSimPlayer
    {
        readonly BuildingZone Zone;

        public ActionPanelConfirmDeleteZone(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, BuildingZone Zone)
            : base("ConfirmDeleteZone", Owner, MapManager, ListActionMenuChoice, true)
        {
            this.Zone = Zone;
        }

        public override void OnSelect()
        {
            AddChoiceToCurrentPanel(new ActionPanelDeleteZoneAnswer(Owner, MapManager, ListActionMenuChoice, Zone, true));
            AddChoiceToCurrentPanel(new ActionPanelDeleteZoneAnswer(Owner, MapManager, ListActionMenuChoice, Zone, false));
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (NavigateThroughNextChoices(MapManager.sndSelection))
            {
            }
            else if (ConfirmNextChoices(MapManager.sndConfirm))
            {
            }

            Owner.EndKeys();
        }

        protected override void OnCancelPanel()
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
            return new ActionPanelConfirmDeleteZone(Owner, MapManager, ListActionMenuChoice, Zone);
        }

        public override string ToString()
        {
            return "Delete zone";
        }

        public override void Draw(CustomSpriteBatch g)
        {
            string Name = Zone == null || string.IsNullOrEmpty(Zone.Name) ? "this zone" : Zone.Name;
            TextHelper.DrawText(g, "Delete " + Name + " and everything built in it?", new Vector2(40, 88), Color.White);
            DrawNextChoice(g);
        }
    }

    public class ActionPanelDeleteZoneAnswer : ActionPanelLifeSimPlayer
    {
        readonly BuildingZone Zone;
        readonly bool Yes;

        public ActionPanelDeleteZoneAnswer(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, BuildingZone Zone, bool Yes)
            : base("DeleteZoneAnswer", Owner, MapManager, ListActionMenuChoice, false)
        {
            this.Zone = Zone;
            this.Yes = Yes;
        }

        public override void OnSelect()
        {
            if (Yes)
            {
                BuildingZones.Delete(MapManager.Zones, Zone);
            }

            System.Collections.Generic.List<ActionPanel> Panels = GetActionPanelsCopy();
            for (int i = 0; i < Panels.Count; i++)
            {
                if (Panels[i] is ActionPanelConfirmDeleteZone || Panels[i] == this)
                {
                    RemoveFromPanelList(Panels[i]);
                }
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        protected override void OnCancelPanel()
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
            return new ActionPanelDeleteZoneAnswer(Owner, MapManager, ListActionMenuChoice, Zone, Yes);
        }

        public override string ToString()
        {
            return Yes ? "Yes" : "No";
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
