using System;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class GameOptionsScreen : GameScreen
    {
        private SpriteFont fntText;
        private SpriteFont fntOxanimumBoldTitle;
        public FMODSound sndButtonOver;
        public FMODSound sndButtonClick;

        private Texture2D sprBarLeft;
        private Texture2D sprBarMiddle;
        private CubeBackgroundSmall CubeBackground;

        private TextButton GametypeButton;
        public TextButton SelectMapButton;
        private TextButton GameRulesButton;
        private TextButton MutatorsButton;
        private TextButton BotsConfigButton;

        private TextButton CloseButton;

        private IUIElement[] ArrayUIElement;

        protected readonly RoomInformations Room;

        private GamePreparationScreen Owner;

        private GameScreen ActiveTab;
        public GameScreen[] ArrayOptionTab;
        private GameOptionsSelectMapScreen SelectMapScreen;
        private GameOptionsGameRulesScreen GameRuleScreen;

        private GameOptionsMutatorsScreen MutatorsScreen;

        public static Color BackgroundColor = Color.FromNonPremultiplied(65, 70, 65, 255);

        public GameOptionsScreen(RoomInformations Room, GamePreparationScreen Owner)
        {
            this.Room = Room;
            this.Owner = Owner;

            CubeBackground = new CubeBackgroundSmall();
        }

        public override void Load()
        {
            CubeBackground.Load(Content);

            sprBarLeft = Content.Load<Texture2D>("Menus/Lobby/Shop/Bar Left");
            sprBarMiddle = Content.Load<Texture2D>("Menus/Lobby/Shop/Bar Middle");

            fntText = Content.Load<SpriteFont>("Fonts/Arial10");
            fntOxanimumBoldTitle = Content.Load<SpriteFont>("Fonts/Oxanium Bold Title");
            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            ArrayOptionTab = new GameScreen[5];
            ArrayOptionTab[0] = GetGametypeScreen();
            ArrayOptionTab[1] = SelectMapScreen = new GameOptionsSelectMapScreen(Room, this, Owner);
            ArrayOptionTab[2] = GameRuleScreen = new GameOptionsGameRulesScreen(Room);
            ArrayOptionTab[3] = MutatorsScreen = new GameOptionsMutatorsScreen(Room, this);
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

            float Ratio = Constants.Height / 2160f;
            int DrawX = (int)(358 * Ratio);
            int DrawY = (int)(350 * Ratio);
            GametypeButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Big}{Centered}{Color:65,70,65,255}Game Type}}", "Menus/Lobby/Button Tab", new Vector2(DrawX, DrawY), 4, 1, Ratio, OnButtonOver, OnGametypeTabPressed);
            DrawX += (int)(520 * Ratio);
            SelectMapButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Big}{Centered}{Color:65,70,65,255}Select Map}}", "Menus/Lobby/Button Tab", new Vector2(DrawX, DrawY), 4, 1, Ratio, OnButtonOver, OnSelectMapTabPressed);
            DrawX += (int)(520 * Ratio);
            GameRulesButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Big}{Centered}{Color:65,70,65,255}Game Rules}}", "Menus/Lobby/Button Tab", new Vector2(DrawX, DrawY), 4, 1, Ratio, OnButtonOver, OnGameRuleTabPressed);
            DrawX += (int)(520 * Ratio);
            MutatorsButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Big}{Centered}{Color:65,70,65,255}Mutators}}", "Menus/Lobby/Button Tab", new Vector2(DrawX, DrawY), 4, 1, Ratio, OnButtonOver, OnMutatorTabPressed);
            DrawX += (int)(520 * Ratio);
            BotsConfigButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Big}{Centered}{Color:65,70,65,255}Bot Config}}", "Menus/Lobby/Button Tab", new Vector2(DrawX, DrawY), 4, 1, Ratio, OnButtonOver, OnBotsConfigTabPressed);

            DrawX = (int)(3350 * Ratio);
            DrawY = (int)(114 * Ratio);
            CloseButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Big}{Centered}{Color:65,70,65,255}Close}}", "Menus/Lobby/Button Back To Lobby", new Vector2(DrawX, DrawY), 4, 1, Ratio, OnButtonOver, OnClosePressed);

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

        protected virtual GameScreen GetGametypeScreen()
        {
            return new GameOptionsGametypeScreen(Room, this);
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

        private void OnGametypeTabPressed()
        {
            ActiveTab = ArrayOptionTab[0];

            SelectMapButton.Unselect();
            GameRulesButton.Unselect();
            MutatorsButton.Unselect();
            BotsConfigButton.Unselect();
        }

        public void OnSelectMapTabPressed()
        {
            ActiveTab = ArrayOptionTab[1];

            GametypeButton.Unselect();
            GameRulesButton.Unselect();
            MutatorsButton.Unselect();
            BotsConfigButton.Unselect();
        }

        private void OnGameRuleTabPressed()
        {
            ActiveTab = ArrayOptionTab[2];

            GametypeButton.Unselect();
            SelectMapButton.Unselect();
            MutatorsButton.Unselect();
            BotsConfigButton.Unselect();
        }

        private void OnMutatorTabPressed()
        {
            ActiveTab = ArrayOptionTab[3];

            GametypeButton.Unselect();
            SelectMapButton.Unselect();
            GameRulesButton.Unselect();
            BotsConfigButton.Unselect();
        }

        private void OnBotsConfigTabPressed()
        {
            ActiveTab = ArrayOptionTab[4];

            GametypeButton.Unselect();
            SelectMapButton.Unselect();
            GameRulesButton.Unselect();
            MutatorsButton.Unselect();
        }

        public void OnClosePressed()
        {
            Owner.OptionsClosed();
            RemoveScreen(this);
        }

        public void OnGametypeUpdate()
        {
            SelectMapScreen.UpdateMaps();
            SelectMapButton.Enable();
            MutatorsButton.Enable();
        }

        public void OnMapUpdate()
        {
            GameRulesButton.Enable();
            GameRuleScreen.UpdateGameParameters();
            MutatorsScreen.UpdateMutators();
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            CubeBackground.BeginDraw(g);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.End();
            g.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(BackgroundColor);

            DrawBackground(g);

            g.End();
            g.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);

            ActiveTab.Draw(g);

            foreach (IUIElement ActiveElement in ArrayUIElement)
            {
                ActiveElement.Draw(g);
            }
        }

        private void DrawBackground(CustomSpriteBatch g)
        {
            CubeBackground.Draw(g, false);
            Color TextColor = Color.FromNonPremultiplied(65, 70, 65, 255);
            float Ratio = Constants.Height / 2160f;

            g.DrawString(fntOxanimumBoldTitle, "Create Match", new Vector2((int)(210 * Ratio), (int)(58 * Ratio)), TextColor);
        }
    }
}
