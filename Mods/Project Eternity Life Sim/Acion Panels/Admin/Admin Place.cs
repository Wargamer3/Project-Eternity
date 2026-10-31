using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelPlaceBox : ActionPanelLifeSimPlayer
    {
        readonly BoxKind Kind;
        readonly string Interaction;
        readonly bool Upright;
        readonly string Label;

        public ActionPanelPlaceBox(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, BoxKind Kind, string Interaction, bool Upright, string Label)
            : base("PlaceBox", Owner, MapManager, ListActionMenuChoice, true)
        {
            this.Kind = Kind;
            this.Interaction = Interaction;
            this.Upright = Upright;
            this.Label = Label;
        }

        public override void OnSelect()
        {
            MapInfo Here = Owner.InvisibleCharacterAsCursor.SharedMapContex.CurrentMapInfo;
            float Tile = 32f;
            LifeSimMap Map = null;
            if (Here != null && Here.MapMapContainer != null)
            {
                Map = Here.MapMapContainer.ActiveMap as LifeSimMap;
            }

            if (Map != null && Map.TileSize.X > 0)
            {
                Tile = Map.TileSize.X;
            }

            if (Here != null)
            {
                Vector3 Ground = CursorGround.From(Owner.InvisibleCharacterAsCursor.Position);
                if (!BuildingZones.Allows(Here, Ground.X, Ground.Y))
                {
                    RemoveFromPanelList(this);
                    return;
                }

                FurnitureItem Box = BoxPlace.Place(Here.Furniture, Ground, Tile, Kind, Interaction, Upright);
                BoxPlace.Keep(Here, Box);
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
            return new ActionPanelPlaceBox(Owner, MapManager, ListActionMenuChoice, Kind, Interaction, Upright, Label);
        }

        public override string ToString()
        {
            return Label;
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }

    public class ActionPanelPlaceWall : ActionPanelLifeSimPlayer
    {
        public ActionPanelPlaceWall(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base("PlaceWall", Owner, MapManager, ListActionMenuChoice, true)
        {
        }

        public override void OnSelect()
        {
            MapInfo Here = Owner.InvisibleCharacterAsCursor.SharedMapContex.CurrentMapInfo;
            LifeSimMap Map = Here == null || Here.MapMapContainer == null ? null : Here.MapMapContainer.ActiveMap as LifeSimMap;
            if (Map != null && Map.TileSize.X > 0 && Here != null)
            {
                Vector3 Ground = CursorGround.From(Owner.InvisibleCharacterAsCursor.Position);
                if (!BuildingZones.Allows(Here, Ground.X, Ground.Y))
                {
                    RemoveFromPanelList(this);
                    return;
                }

                int X;
                int Y;
                BoxPlace.Cell(CursorGround.From(Owner.InvisibleCharacterAsCursor.Position), Map.TileSize.X, out X, out Y);
                BoxPlace.PlaceWall(Map, X, Y);
                BoxPlace.Keep(Here, BoxPlace.Wall(Map.TileSize.X, X, Y));
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
            return new ActionPanelPlaceWall(Owner, MapManager, ListActionMenuChoice);
        }

        public override string ToString()
        {
            return "Wall";
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }

    public class ActionPanelAssignCharacter : ActionPanelLifeSimPlayer
    {
        public ActionPanelAssignCharacter(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base("AssignCharacter", Owner, MapManager, ListActionMenuChoice, true)
        {
        }

        public override void OnSelect()
        {
            PlayerCharacter Found = CharacterUnderCursor();
            Owner.Session.ToggleAssigned(Found);
            RemoveFromPanelList(this);
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        PlayerCharacter CharacterUnderCursor()
        {
            MapInfo Here = Owner.InvisibleCharacterAsCursor.SharedMapContex.CurrentMapInfo;
            LifeSimMap Map = Here == null || Here.MapMapContainer == null ? null : Here.MapMapContainer.ActiveMap as LifeSimMap;
            if (Map == null)
            {
                return null;
            }

            return Map.MoveCursorAndGetCharacterUnderMouse();
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
            return new ActionPanelAssignCharacter(Owner, MapManager, ListActionMenuChoice);
        }

        public override string ToString()
        {
            return "Assign To Player";
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }

    public class ActionPanelDeleteSelection : ActionPanelLifeSimPlayer
    {
        public ActionPanelDeleteSelection(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base("DeleteSelection", Owner, MapManager, ListActionMenuChoice, true)
        {
        }

        public override void OnSelect()
        {
            MapInfo Here = Owner.InvisibleCharacterAsCursor.SharedMapContex.CurrentMapInfo;
            LifeSimMap Map = Here == null || Here.MapMapContainer == null ? null : Here.MapMapContainer.ActiveMap as LifeSimMap;
            Vector3 Ground = CursorGround.From(Owner.InvisibleCharacterAsCursor.Position);
            bool Removed = false;
            if (Here != null && WaterPlace.RemoveAt(Here.Water, Ground))
            {
                Removed = true;
            }

            FurnitureItem Furniture = Here == null || Removed ? null : BoxPlace.Under(Here.Furniture, Ground);
            if (Furniture != null && Here != null)
            {
                BoxPlace.Drop(Here, Furniture);
                Removed = true;
            }
            else if (!Removed && Here != null && Here.Buildings != null)
            {
                BuildingItem Building = null;
                for (int i = 0; i < Here.Buildings.Count; i++)
                {
                    BuildingItem Box = Here.Buildings[i];
                    if (Box != null && Box.FootprintContains(Ground.X, Ground.Y))
                    {
                        Building = Box;
                    }
                }

                if (Building != null)
                {
                    if (Building.Kind == BuildingKind.Wall && Map != null && Map.TileSize.X > 0)
                    {
                        BoxPlace.ClearWall(Map, (int)(Building.Position.X / Map.TileSize.X), (int)(Building.Position.Y / Map.TileSize.Y));
                    }

                    BoxPlace.Drop(Here, Building);
                    Removed = true;
                }
            }

            if (!Removed && Map != null)
            {
                PlayerCharacter Found = Map.MoveCursorAndGetCharacterUnderMouse();
                if (Found != null)
                {
                    Here.Drop(Found);
                    Owner.Session.Assigned.Remove(Found);
                    if (Owner.Session.Controlled == Found)
                    {
                        Owner.Session.ReleaseCharacter();
                    }
                }
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
            return new ActionPanelDeleteSelection(Owner, MapManager, ListActionMenuChoice);
        }

        public override string ToString()
        {
            return "Delete";
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }

    public class ActionPanelTakeCharacter : ActionPanelLifeSimPlayer
    {
        readonly PlayerCharacter Character;

        public ActionPanelTakeCharacter(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, PlayerCharacter Character)
            : base("TakeCharacter", Owner, MapManager, ListActionMenuChoice, true)
        {
            this.Character = Character;
        }

        public override void OnSelect()
        {
            Owner.Session.TakeCharacter(Character);
            Owner.ActiveControlledCharacter = Character;
            RemoveAllSubActionPanels();
            AddToPanelListAndSelect(new ActionPanelThirdPersonControlCharacter(Owner, Character, MapManager, ListActionMenuChoice));
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
            return new ActionPanelTakeCharacter(Owner, MapManager, ListActionMenuChoice, Character);
        }

        public override string ToString()
        {
            return Character == null ? "Take" : Character.Name;
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
