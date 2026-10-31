using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelSpawnBlocks : ActionPanelLifeSimPlayer
    {
        public ActionPanelSpawnBlocks(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base("SpawnBlocks", Owner, MapManager, ListActionMenuChoice, true)
        {
        }

        public override void OnSelect()
        {
            AddChoiceToCurrentPanel(new ActionPanelPlaceBox(Owner, MapManager, ListActionMenuChoice, BoxKind.SmallCube, null, false, "Small Cube"));
            AddChoiceToCurrentPanel(new ActionPanelPlaceBox(Owner, MapManager, ListActionMenuChoice, BoxKind.Step, null, false, "Step"));
            AddChoiceToCurrentPanel(new ActionPanelPlaceBox(Owner, MapManager, ListActionMenuChoice, BoxKind.Long, null, false, "Long Box"));
            AddChoiceToCurrentPanel(new ActionPanelPlaceBox(Owner, MapManager, ListActionMenuChoice, BoxKind.Long, null, true, "Tall Box"));
            AddChoiceToCurrentPanel(new ActionPanelPlaceWall(Owner, MapManager, ListActionMenuChoice));
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
            return new ActionPanelSpawnBlocks(Owner, MapManager, ListActionMenuChoice);
        }

        public override string ToString()
        {
            return "Spawn Blocks";
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawNextChoice(g);
        }
    }

    public class ActionPanelGlobalWeather : ActionPanelLifeSimPlayer
    {
        public ActionPanelGlobalWeather(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base("GlobalWeather", Owner, MapManager, ListActionMenuChoice, true)
        {
        }

        public override void OnSelect()
        {
            AddChoiceToCurrentPanel(new ActionPanelWeatherChoice(Owner, MapManager, ListActionMenuChoice, "Sunny", TimePeriod.WeatherTypes.Regular, TimePeriod.VisibilityTypes.Regular));
            AddChoiceToCurrentPanel(new ActionPanelWeatherChoice(Owner, MapManager, ListActionMenuChoice, "Some Clouds", TimePeriod.WeatherTypes.SomeClouds, TimePeriod.VisibilityTypes.Regular));
            AddChoiceToCurrentPanel(new ActionPanelWeatherChoice(Owner, MapManager, ListActionMenuChoice, "Cloudy", TimePeriod.WeatherTypes.Cloudy, TimePeriod.VisibilityTypes.Regular));
            AddChoiceToCurrentPanel(new ActionPanelWeatherChoice(Owner, MapManager, ListActionMenuChoice, "Light Rain", TimePeriod.WeatherTypes.SmallRain, TimePeriod.VisibilityTypes.Regular));
            AddChoiceToCurrentPanel(new ActionPanelWeatherChoice(Owner, MapManager, ListActionMenuChoice, "Heavy Rain", TimePeriod.WeatherTypes.HeavyRain, TimePeriod.VisibilityTypes.Regular));
            AddChoiceToCurrentPanel(new ActionPanelWeatherChoice(Owner, MapManager, ListActionMenuChoice, "Thunderstorm", TimePeriod.WeatherTypes.Thunderstorm, TimePeriod.VisibilityTypes.Regular));
            AddChoiceToCurrentPanel(new ActionPanelWeatherChoice(Owner, MapManager, ListActionMenuChoice, "Fog", TimePeriod.WeatherTypes.Regular, TimePeriod.VisibilityTypes.FogOfWar));
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
            return new ActionPanelGlobalWeather(Owner, MapManager, ListActionMenuChoice);
        }

        public override string ToString()
        {
            return "Weather";
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawNextChoice(g);
        }
    }

    public class ActionPanelWeatherChoice : ActionPanelLifeSimPlayer
    {
        readonly string Label;
        readonly TimePeriod.WeatherTypes Weather;
        readonly TimePeriod.VisibilityTypes Visibility;

        public ActionPanelWeatherChoice(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, string Label, TimePeriod.WeatherTypes Weather, TimePeriod.VisibilityTypes Visibility)
            : base("WeatherChoice", Owner, MapManager, ListActionMenuChoice, true)
        {
            this.Label = Label;
            this.Weather = Weather;
            this.Visibility = Visibility;
        }

        public override void OnSelect()
        {
            Forecast.SetGlobal(MapManager, Weather, Visibility);
            RemoveFromPanelList(this);
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
            return new ActionPanelWeatherChoice(Owner, MapManager, ListActionMenuChoice, Label, Weather, Visibility);
        }

        public override string ToString()
        {
            return Label;
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }

    public class ActionPanelPlaceDungeonGate : ActionPanelLifeSimPlayer
    {
        public ActionPanelPlaceDungeonGate(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base("PlaceDungeonGate", Owner, MapManager, ListActionMenuChoice, true)
        {
        }

        public override void OnSelect()
        {
            MapInfo Here = Owner.InvisibleCharacterAsCursor.SharedMapContex.CurrentMapInfo;
            LifeSimMap Map = Here == null || Here.MapMapContainer == null ? null : Here.MapMapContainer.ActiveMap as LifeSimMap;
            if (Here != null)
            {
                Vector3 Ground = CursorGround.From(Owner.InvisibleCharacterAsCursor.Position);
                float Tile = Map != null && Map.TileSize.X > 0f ? Map.TileSize.X : 32f;
                BuildResult Placed = BuildPlan.Commit("Dungeon Gate", Ground, Vector3.Zero, false, BuildFloor.Index(Ground.Z), true, Tile, Here.Furniture, Here.Buildings);
                Placed.Apply(Here);
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
            return new ActionPanelPlaceDungeonGate(Owner, MapManager, ListActionMenuChoice);
        }

        public override string ToString()
        {
            return "Dungeon Gate";
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
