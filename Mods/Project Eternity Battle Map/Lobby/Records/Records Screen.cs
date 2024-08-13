using System;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class RecordsScreenWhite : GameScreen
    {
        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;
        private SpriteFont fntOxanimumBoldTitle;

        private Texture2D sprTitleHighlight;
        private Texture2D sprBarLeft;
        private Texture2D sprBarMiddle;

        private RenderTarget2D CubeRenderTarget;
        private Model Cube;

        private IUIElement[] ArrayUIElement;

        public static Color BackgroundColor = Color.FromNonPremultiplied(65, 70, 65, 255);

        float RotationX;

        public readonly int TopSectionHeight;
        private readonly BattleMapPlayer ActivePlayer;
        private readonly Roster PlayerRoster;
        private GameScreen ActiveTab;
        private GameScreen[] ArrayRecordTabs;

        public RecordsScreenWhite(BattleMapPlayer ActivePlayer, Roster PlayerRoster)
            : base()
        {
            this.ActivePlayer = ActivePlayer;
            this.PlayerRoster = PlayerRoster;

            TopSectionHeight = (int)(Constants.Height * 0.1);

            this.RequireDrawFocus = true;
        }

        public override void Load()
        {
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            fntOxanimumBoldTitle = Content.Load<SpriteFont>("Fonts/Oxanium Bold Title");

            sprTitleHighlight = Content.Load<Texture2D>("Menus/Lobby/Shop/Title Highlight");
            sprBarLeft = Content.Load<Texture2D>("Menus/Lobby/Shop/Bar Left");
            sprBarMiddle = Content.Load<Texture2D>("Menus/Lobby/Shop/Bar Middle");

            Cube = Content.Load<Model>("Menus/Lobby/Cube thing");

            int CubeTargetHeight = 900;
            CubeRenderTarget = new RenderTarget2D(GraphicsDevice, (int)(CubeTargetHeight * 1.777777f), CubeTargetHeight, false,
                GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24, 16, RenderTargetUsage.DiscardContents);

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            int LineY = TopSectionHeight + 5;
            int TabWidth = 100;
            float NumberOfTabs = 5;
            float Spacing = (Constants.Width - (NumberOfTabs * TabWidth)) / NumberOfTabs;
            float TabX = 5;

            BoxButton PlayerButton = new BoxButton(new Rectangle((int)TabX, LineY, TabWidth, 40), fntArial12, "Player", OnButtonOver, SelectPlayerRecordsButton);
            PlayerButton.CanBeChecked = true;
            PlayerButton.Select();
            TabX += Spacing + TabWidth;
            BoxButton UnitButton = new BoxButton(new Rectangle((int)TabX, LineY, TabWidth, 40), fntArial12, "Units", OnButtonOver, SelectUnitRecordsButton);
            UnitButton.CanBeChecked = true;
            TabX += Spacing + TabWidth;
            BoxButton BattleButton = new BoxButton(new Rectangle((int)TabX, LineY, TabWidth, 40), fntArial12, "Battle", OnButtonOver, SelectBattleRecordsButton);
            BattleButton.CanBeChecked = true;
            TabX += Spacing + TabWidth;
            BoxButton BonusButton = new BoxButton(new Rectangle((int)TabX, LineY, TabWidth, 40), fntArial12, "Bonuses", OnButtonOver, SelectBonusRecordsButton);
            BonusButton.CanBeChecked = true;
            TabX += Spacing + TabWidth;
            BoxButton ProgressionButton = new BoxButton(new Rectangle((int)TabX, LineY, TabWidth, 40), fntArial12, "Progression", OnButtonOver, SelectProgressionRecordsButton);
            ProgressionButton.CanBeChecked = true;
            TabX += Spacing + TabWidth;
            BoxButton MultiplayerButton = new BoxButton(new Rectangle((int)TabX, LineY, TabWidth, 40), fntArial12, "Progression", OnButtonOver, SelectMultiplayerRecordsButton);
            MultiplayerButton.CanBeChecked = true;
            TabX += Spacing + TabWidth;

            ArrayUIElement = new IUIElement[] { PlayerButton, UnitButton, BattleButton, BonusButton, ProgressionButton };

            GameScreen PlayerTab = new PlayerRecordsTabWhite(ActivePlayer, TopSectionHeight + 55);
            GameScreen UnitTab = new UnitRecordsTabWhite(ActivePlayer, PlayerRoster, TopSectionHeight + 55);
            GameScreen BattleTab = new BattleRecordsTabWhite(ActivePlayer, TopSectionHeight + 55);
            GameScreen BonusTab = new BonusRecordsTabWhite(ActivePlayer, TopSectionHeight + 55);
            GameScreen ProgressionTab = new ProgressionRecordsTabWhite(ActivePlayer, TopSectionHeight + 55);
            GameScreen MultiplayerTab = new MultiplayerRecordsTabWhite(ActivePlayer, TopSectionHeight + 55);

            ArrayRecordTabs = new GameScreen[] { PlayerTab, UnitTab, BattleTab, BonusTab, ProgressionTab };

            foreach (GameScreen ActiveScreen in ArrayRecordTabs)
            {
                ActiveScreen.Load();
            }

            ActiveTab = ArrayRecordTabs[0];
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var a in ArrayUIElement)
            {
                a.Update(gameTime);
            }

            ActiveTab.Update(gameTime);
        }
        private void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        #region Buttons

        private void SelectPlayerRecordsButton()
        {
            sndButtonClick.Play();
            ActiveTab = ArrayRecordTabs[0];
            ArrayUIElement[1].Unselect();
            ArrayUIElement[2].Unselect();
            ArrayUIElement[3].Unselect();
            ArrayUIElement[4].Unselect();
        }

        private void SelectUnitRecordsButton()
        {
            sndButtonClick.Play();
            ActiveTab = ArrayRecordTabs[1];
            ArrayUIElement[0].Unselect();
            ArrayUIElement[2].Unselect();
            ArrayUIElement[3].Unselect();
            ArrayUIElement[4].Unselect();
        }

        private void SelectBattleRecordsButton()
        {
            sndButtonClick.Play();
            ActiveTab = ArrayRecordTabs[2];
            ArrayUIElement[0].Unselect();
            ArrayUIElement[1].Unselect();
            ArrayUIElement[3].Unselect();
            ArrayUIElement[4].Unselect();
        }

        private void SelectBonusRecordsButton()
        {
            sndButtonClick.Play();
            ActiveTab = ArrayRecordTabs[3];
            ArrayUIElement[0].Unselect();
            ArrayUIElement[1].Unselect();
            ArrayUIElement[3].Unselect();
            ArrayUIElement[4].Unselect();
        }

        private void SelectProgressionRecordsButton()
        {
            sndButtonClick.Play();
            ActiveTab = ArrayRecordTabs[4];
            ArrayUIElement[0].Unselect();
            ArrayUIElement[1].Unselect();
            ArrayUIElement[2].Unselect();
            ArrayUIElement[3].Unselect();
        }

        private void SelectMultiplayerRecordsButton()
        {
            sndButtonClick.Play();
            ActiveTab = ArrayRecordTabs[4];
            ArrayUIElement[0].Unselect();
            ArrayUIElement[1].Unselect();
            ArrayUIElement[2].Unselect();
            ArrayUIElement[3].Unselect();
        }

        #endregion

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
            float Ratio = Constants.Height / 2160f;
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(BackgroundColor);

            DrawBackground(g);

            foreach (var a in ArrayUIElement)
            {
                a.Draw(g);
            }

            ActiveTab.Draw(g);
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

            g.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            int LineX = 89;
            int LineY = 222;
            g.Draw(sprBarLeft, new Rectangle((int)(LineX * Ratio), (int)(LineY * Ratio), (int)Math.Ceiling(sprBarLeft.Width * Ratio), (int)Math.Ceiling(sprBarLeft.Height * Ratio)), Color.White);
            g.Draw(sprBarMiddle, new Rectangle((int)(LineX * Ratio + sprBarLeft.Width * Ratio), (int)(LineY * Ratio), (int)Math.Ceiling(2518 * Ratio), (int)Math.Ceiling(sprBarMiddle.Height * Ratio)), Color.White);

            LineX = 110;
            LineY = 1907;
            g.Draw(sprBarLeft, new Rectangle((int)(LineX * Ratio), (int)(LineY * Ratio), (int)Math.Ceiling(sprBarLeft.Width * Ratio), (int)Math.Ceiling(sprBarLeft.Height * Ratio)), Color.White);
            g.Draw(sprBarMiddle, new Rectangle((int)(LineX * Ratio + sprBarLeft.Width * Ratio), (int)(LineY * Ratio), (int)Math.Ceiling(3592 * Ratio), (int)Math.Ceiling(sprBarMiddle.Height * Ratio)), Color.White);

            g.Draw(sprTitleHighlight, new Vector2((int)(160 * Ratio), (int)(46 * Ratio)), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
            g.DrawString(fntOxanimumBoldTitle, "RECORDS", new Vector2((int)(210 * Ratio), (int)(58 * Ratio)), TextColor);
        }
    }
}
