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

        private Texture2D sprTitleHighlight;
        private Texture2D sprBarLeft;
        private Texture2D sprBarMiddle;

        private RenderTarget2D CubeRenderTarget;
        private Model Cube;

        private TextButton GametypeButton;
        private TextButton SelectMapButton;
        private TextButton GameRulesButton;
        private TextButton MutatorsButton;
        private TextButton BotsConfigButton;

        private TextButton CloseButton;

        private IUIElement[] ArrayUIElement;

        protected readonly RoomInformations Room;

        private GamePreparationScreen Owner;

        private GameScreen ActiveTab;
        private GameScreen[] ArrayOptionTab;
        private GameOptionsSelectMapScreen SelectMapScreen;
        private GameOptionsGameRulesScreen GameRuleScreen;

        private GameOptionsMutatorsScreen MutatorsScreen;

        public static Color BackgroundColor = Color.FromNonPremultiplied(65, 70, 65, 255);

        private float RotationX;

        public GameOptionsScreen(RoomInformations Room, GamePreparationScreen Owner)
        {
            this.Room = Room;
            this.Owner = Owner;
        }

        public override void Load()
        {
            Cube = Content.Load<Model>("Menus/Lobby/Cube thing");

            int CubeTargetHeight = 900;
            CubeRenderTarget = new RenderTarget2D(GraphicsDevice, (int)(CubeTargetHeight * 1.777777f), CubeTargetHeight, false,
                GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24, 16, RenderTargetUsage.DiscardContents);

            fntText = Content.Load<SpriteFont>("Fonts/Arial10");
            fntOxanimumBoldTitle = Content.Load<SpriteFont>("Fonts/Oxanium Bold Title");
            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            sprTitleHighlight = Content.Load<Texture2D>("Menus/Lobby/Shop/Title Highlight");
            sprBarLeft = Content.Load<Texture2D>("Menus/Lobby/Shop/Bar Left");
            sprBarMiddle = Content.Load<Texture2D>("Menus/Lobby/Shop/Bar Middle");

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
            GametypeButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Big}{Centered}{Color:65,70,65,255}Game Type}}", "Menus/Lobby/Button Tab", new Vector2(DrawX, DrawY), 4, 1, Ratio, OnButtonOver, OnGametypePressed);
            DrawX += (int)(520 * Ratio);
            SelectMapButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Big}{Centered}{Color:65,70,65,255}Select Map}}", "Menus/Lobby/Button Tab", new Vector2(DrawX, DrawY), 4, 1, Ratio, OnButtonOver, OnSelectMapPressed);
            DrawX += (int)(520 * Ratio);
            GameRulesButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Big}{Centered}{Color:65,70,65,255}Game Rules}}", "Menus/Lobby/Button Tab", new Vector2(DrawX, DrawY), 4, 1, Ratio, OnButtonOver, OnGameRulePressed);
            DrawX += (int)(520 * Ratio);
            MutatorsButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Big}{Centered}{Color:65,70,65,255}Mutators}}", "Menus/Lobby/Button Tab", new Vector2(DrawX, DrawY), 4, 1, Ratio, OnButtonOver, OnMutatorPressed);
            DrawX += (int)(520 * Ratio);
            BotsConfigButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Big}{Centered}{Color:65,70,65,255}Bot Config}}", "Menus/Lobby/Button Tab", new Vector2(DrawX, DrawY), 4, 1, Ratio, OnButtonOver, OnBotsConfigPressed);

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
            g.GraphicsDevice.SetRenderTarget(CubeRenderTarget);
            g.GraphicsDevice.Clear(Color.Transparent);
            float aspectRatio = 1f;

            Vector3 position = new Vector3(0, 0, 6);

            Vector3 target = new Vector3(0, 0, 3);

            Vector3 up = Vector3.Up;
            Matrix View = Matrix.CreateLookAt(position, target, up);
            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(0.40f,
                                                                    aspectRatio,
                                                                    1000f, 18000);

            ((BasicEffect)Cube.Meshes[0].Effects[0]).DiffuseColor = new Vector3(248f / 255f);
            Cube.Draw(Matrix.CreateTranslation(0, 0, 0) * Matrix.CreateRotationX(1) * Matrix.CreateRotationY(1)
                * Matrix.CreateRotationX(0) * Matrix.CreateRotationY(RotationX)
                * Matrix.CreateScale(0.4f) * Matrix.CreateTranslation(0, 0, -4200), View, Projection);
            g.GraphicsDevice.SetRenderTarget(null);
            RotationX += 0.00625f;
        }

        public override void Draw(CustomSpriteBatch g)
        {
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
            Color TextColor = Color.FromNonPremultiplied(65, 70, 65, 255);
            g.GraphicsDevice.Clear(Color.FromNonPremultiplied(243, 243, 243, 255));
            float Ratio = Constants.Height / 2160f;

            g.DrawLine(sprPixel, new Vector2(-1000 * Ratio, 364 * Ratio), new Vector2(3000 * Ratio, -1346 * Ratio), Color.FromNonPremultiplied(233, 233, 233, 255), 240);
            g.End();

            BlendState blend = new BlendState();
            blend.AlphaSourceBlend = Blend.One;
            blend.AlphaDestinationBlend = Blend.One;
            blend.ColorSourceBlend = Blend.One;
            blend.ColorDestinationBlend = Blend.One;
            blend.AlphaBlendFunction = BlendFunction.Min;

            g.Begin(SpriteSortMode.Deferred, blend);

            g.Draw(CubeRenderTarget, new Vector2(400, 180), null, Color.FromNonPremultiplied(5, 5, 5, 255), 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            g.End();
            g.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            g.DrawLine(sprPixel, new Vector2(-1000 * Ratio, 2544 * Ratio), new Vector2(3000 * Ratio, 658 * Ratio), Color.FromNonPremultiplied(233, 233, 233, 255), 600);
            g.DrawLine(sprPixel, new Vector2(1800 * Ratio, 2238 * Ratio), new Vector2(3560 * Ratio, 1344 * Ratio), Color.FromNonPremultiplied(233, 233, 233, 255), 200);
            g.End();

            g.Begin(SpriteSortMode.Deferred, blend, SamplerState.AnisotropicWrap, DepthStencilState.Default, RasterizerState.CullNone);

            g.Draw(CubeRenderTarget, new Vector2(1022, 392), null, Color.FromNonPremultiplied(23, 23, 23, 255), 0f, Vector2.Zero, 0.51f, SpriteEffects.None, 0.9f);
            g.End();
            g.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            int LineX = 89;
            int LineY = 222;
            g.Draw(sprBarLeft, new Rectangle((int)(LineX * Ratio), (int)(LineY * Ratio), (int)Math.Ceiling(sprBarLeft.Width * Ratio), (int)Math.Ceiling(sprBarLeft.Height * Ratio)), Color.White);
            g.Draw(sprBarMiddle, new Rectangle((int)(LineX * Ratio + sprBarLeft.Width * Ratio), (int)(LineY * Ratio), (int)Math.Ceiling(2518 * Ratio), (int)Math.Ceiling(sprBarMiddle.Height * Ratio)), Color.White);

            g.Draw(sprTitleHighlight, new Vector2((int)(160 * Ratio), (int)(46 * Ratio)), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
            g.DrawString(fntOxanimumBoldTitle, "Create Match", new Vector2((int)(210 * Ratio), (int)(58 * Ratio)), TextColor);
        }
    }
}
