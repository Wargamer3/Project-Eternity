using System;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    class GameOptionsScreen : GameScreen
    {
        private SpriteFont fntText;
        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private BoxButton GametypeButton;
        private BoxButton SelectMapButton;
        private BoxButton GameRulesButton;
        private BoxButton MutatorsButton;
        private BoxButton BotsConfigButton;

        private BoxButton CloseButton;

        private IUIElement[] ArrayUIElement;

        private readonly RoomInformations Room;

        private GamePreparationScreen Owner;

        private GameScreen ActiveTab;
        private GameScreen[] ArrayOptionTab;
        private GameOptionsSelectMapScreen SelectMapScreen;

        public GameOptionsScreen(RoomInformations Room, GamePreparationScreen Owner)
        {
            this.Room = Room;
            this.Owner = Owner;
        }

        public override void Load()
        {
            fntText = Content.Load<SpriteFont>("Fonts/Arial10");
            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            ArrayOptionTab = new GameScreen[5];
            ArrayOptionTab[0] = new GameOptionsGametypeScreen(Room, this);
            ArrayOptionTab[1] = SelectMapScreen = new GameOptionsSelectMapScreen(Room, Owner);
            ArrayOptionTab[2] = new GameOptionsGameRulesScreen(Room);
            ArrayOptionTab[3] = new GameOptionsMutatorsScreen(Room);
            ArrayOptionTab[4] = new GameOptionsBotConfigScreen(Room);

            for (int T = 0; T < ArrayOptionTab.Length; ++T)
            {
                ArrayOptionTab[T].Load();
            }

            ActiveTab = ArrayOptionTab[0];
            int HeaderHeight = (int)(Constants.Height * 0.05);
            int TabSectionY = HeaderHeight;
            int TabSectionHeight = (int)(Constants.Height * 0.06);
            int TabY = TabSectionY + (int)(Constants.Height * 0.005);
            int TabWidth = (int)(Constants.Height * 0.15);
            int TabHeight = (int)(Constants.Height * 0.05);
            int TabOffset = (int)(Constants.Height * 0.05);

            int X = 10 + 0 * (TabWidth + TabOffset);
            GametypeButton = new BoxButton(new Rectangle(X, TabY, TabWidth, TabHeight), fntText, ArrayOptionTab[0].ToString(), OnButtonOver, OnGametypePressed);
            X = 10 + 1 * (TabWidth + TabOffset);
            SelectMapButton = new BoxButton(new Rectangle(X, TabY, TabWidth, TabHeight), fntText, ArrayOptionTab[1].ToString(), OnButtonOver, OnSelectMapPressed);
            X = 10 + 2 * (TabWidth + TabOffset);
            GameRulesButton = new BoxButton(new Rectangle(X, TabY, TabWidth, TabHeight), fntText, ArrayOptionTab[2].ToString(), OnButtonOver, OnGameRulePressed);
            X = 10 + 3 * (TabWidth + TabOffset);
            MutatorsButton = new BoxButton(new Rectangle(X, TabY, TabWidth, TabHeight), fntText, ArrayOptionTab[3].ToString(), OnButtonOver, OnMutatorPressed);
            X = 10 + 4 * (TabWidth + TabOffset);
            BotsConfigButton = new BoxButton(new Rectangle(X, TabY, TabWidth, TabHeight), fntText, ArrayOptionTab[4].ToString(), OnButtonOver, OnBotsConfigPressed);

            int CloseButtonX = (int)(Constants.Width * 0.86);
            int CloseButtonY = (int)(Constants.Height * 0.92);
            int CloseButtonWidth = (int)(Constants.Width * 0.12);
            int CloseButtonHeight = (int)(Constants.Height * 0.06);

            CloseButton = new BoxButton(new Rectangle(CloseButtonX, CloseButtonY, CloseButtonWidth, CloseButtonHeight), fntText, "Close", OnButtonOver, OnClosePressed);

            GametypeButton.CanBeChecked = true;
            SelectMapButton.CanBeChecked = true;
            GameRulesButton.CanBeChecked = true;
            MutatorsButton.CanBeChecked = true;
            BotsConfigButton.CanBeChecked = true;

            GametypeButton.Select();
            SelectMapButton.Disable();
            GameRulesButton.Disable();
            MutatorsButton.Disable();
            BotsConfigButton.Disable();

            ArrayUIElement = new IUIElement[]
            {
                GametypeButton, SelectMapButton, GameRulesButton, MutatorsButton, BotsConfigButton,
                CloseButton,
            };
        }

        private void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        public override void Update(GameTime gameTime)
        {
            ActiveTab.Update(gameTime);

            foreach (IUIElement ActiveElement in ArrayUIElement)
            {
                ActiveElement.Update(gameTime);
            }
        }

        private void OnGametypePressed()
        {
            ActiveTab = ArrayOptionTab[0];

            SelectMapButton.Unselect();
            GameRulesButton.Unselect();
            MutatorsButton.Unselect();
            BotsConfigButton.Unselect();
        }

        private void OnSelectMapPressed()
        {
            ActiveTab = ArrayOptionTab[1];

            GametypeButton.Unselect();
            GameRulesButton.Unselect();
            MutatorsButton.Unselect();
            BotsConfigButton.Unselect();
        }

        private void OnGameRulePressed()
        {
            ActiveTab = ArrayOptionTab[2];

            GametypeButton.Unselect();
            SelectMapButton.Unselect();
            MutatorsButton.Unselect();
            BotsConfigButton.Unselect();
        }

        private void OnMutatorPressed()
        {
            ActiveTab = ArrayOptionTab[3];

            GametypeButton.Unselect();
            SelectMapButton.Unselect();
            GameRulesButton.Unselect();
            BotsConfigButton.Unselect();
        }

        private void OnBotsConfigPressed()
        {
            ActiveTab = ArrayOptionTab[4];

            GametypeButton.Unselect();
            SelectMapButton.Unselect();
            GameRulesButton.Unselect();
            MutatorsButton.Unselect();
        }

        private void OnClosePressed()
        {
            Owner.OptionsClosed();
            RemoveScreen(this);
        }

        public void OnGametypeUpdate()
        {
            SelectMapScreen.UpdateMaps();
            SelectMapButton.Enable();
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawBox(g, new Vector2(), Constants.Width, Constants.Height, Color.White);
            int HeaderHeight = (int)(Constants.Height * 0.05);
            int TabSectionY = HeaderHeight;
            int TabSectionHeight = (int)(Constants.Height * 0.06);
            DrawBox(g, new Vector2(), Constants.Width, Constants.Height, Color.White);
            DrawBox(g, new Vector2(), Constants.Width, HeaderHeight, Color.White);
            DrawBox(g, new Vector2(0, TabSectionY), Constants.Width, TabSectionHeight, Color.White);

            ActiveTab.Draw(g);

            foreach (IUIElement ActiveElement in ArrayUIElement)
            {
                ActiveElement.Draw(g);
            }
        }
    }
}
