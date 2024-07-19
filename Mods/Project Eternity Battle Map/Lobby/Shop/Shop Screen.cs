using System;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class ShopScreen : GameScreen
    {
        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;
        private SpriteFont fntOxanimumBold;
        private SpriteFont fntOxanimumBoldBig;
        private SpriteFont fntOxanimumBoldTitle;

        private Texture2D sprButtonSmallActive;
        private Texture2D sprButtonSmallInactive;
        private Texture2D sprButtonBackToLobby;
        private Texture2D sprMoney;

        private Texture2D sprTitleHighlight;
        private Texture2D sprBarLeft;
        private Texture2D sprBarMiddle;

        private RenderTarget2D CubeRenderTarget;
        private Model Cube;

        private EmptyBoxButton ReturnToLobbyButton;

        private EmptyBoxButton UnitFilterButton;
        private EmptyBoxButton CharacterFilterButton;
        private EmptyBoxButton EquipmentFilterButton;
        private EmptyBoxButton ConsumableFilterButton;

        private EmptyDropDownButton CurrentLocalPlayerButton;

        private IUIElement[] ArrayUIElement;

        public static Color BackgroundColor = Color.FromNonPremultiplied(65, 70, 65, 255);

        private BasicEffect IndexedLinesEffect;
        private IndexedLines BackgroundGrid;

        public int LeftSideX;
        public int LeftSideWidth;
        public int TopSectionHeight;
        public int HeaderSectionY;
        public int HeaderSectionHeight;

        public int BottomSectionHeight;
        public int BottomSectionY;

        public int MiddleSectionY;
        public int MiddleSectionHeight;

        private float RotationX;

        private GameScreen[] ArraySubScreen;

        private GameScreen ActiveSubScreen;

        public BattleMapOnlineClient OnlineGameClient;
        public BattleMapPlayer ActivePlayer;

        public ShopScreen(BattleMapOnlineClient OnlineGameClient)
        {
            this.OnlineGameClient = OnlineGameClient;
            ActivePlayer = (BattleMapPlayer)PlayerManager.ListLocalPlayer[0];

            LeftSideX = 10;
            LeftSideWidth = (int)(Constants.Width * 0.5);
            TopSectionHeight = (int)(Constants.Height * 0.11);
            HeaderSectionY = TopSectionHeight;
            HeaderSectionHeight = (int)(Constants.Height * 0.05);

            BottomSectionHeight = (int)(Constants.Height * 0.07);
            BottomSectionY = Constants.Height - BottomSectionHeight;

            MiddleSectionY = (HeaderSectionY + HeaderSectionHeight);
            MiddleSectionHeight = BottomSectionY - MiddleSectionY;
        }

        public override void Load()
        {
            //Constants.graphics.PreferMultiSampling = true;
            //Constants.graphics.ApplyChanges();

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            fntOxanimumBold = Content.Load<SpriteFont>("Fonts/Oxanium Bold");
            fntOxanimumBoldBig = Content.Load<SpriteFont>("Fonts/Oxanium Bold Big");
            fntOxanimumBoldTitle = Content.Load<SpriteFont>("Fonts/Oxanium Bold Title");

            sprButtonSmallActive = Content.Load<Texture2D>("Menus/Lobby/Shop/Button_Active");
            sprButtonSmallInactive = Content.Load<Texture2D>("Menus/Lobby/Shop/Button_Inactive");
            sprButtonBackToLobby = Content.Load<Texture2D>("Menus/Lobby/Shop/Button Back To Lobby");
            sprMoney = Content.Load<Texture2D>("Menus/Lobby/Shop/Frame Money");

            sprTitleHighlight = Content.Load<Texture2D>("Menus/Lobby/Shop/Title Highlight");
            sprBarLeft = Content.Load<Texture2D>("Menus/Lobby/Shop/Bar Left");
            sprBarMiddle = Content.Load<Texture2D>("Menus/Lobby/Shop/Bar Middle");

            Cube = Content.Load<Model>("Menus/Lobby/Cube thing");

            int CubeTargetHeight = 900;
            CubeRenderTarget = new RenderTarget2D(GraphicsDevice, (int)(CubeTargetHeight * 1.777777f), CubeTargetHeight, false,
                GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24, 16, RenderTargetUsage.DiscardContents);

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            ReturnToLobbyButton = new EmptyBoxButton(new Rectangle((int)(Constants.Width * 0.7f), 0, (int)(Constants.Width * 0.3), TopSectionHeight), fntArial12, "Back To Lobby", OnButtonOver, SelectBackToLobbyButton);

            UnitFilterButton = new EmptyBoxButton(new Rectangle(LeftSideX + 4, HeaderSectionY + 4, 60, HeaderSectionHeight - 8), fntArial12, "Units", OnButtonOver, SelectUnitFilterButton);
            CharacterFilterButton = new EmptyBoxButton(new Rectangle(LeftSideX + 64, HeaderSectionY + 4, 90, HeaderSectionHeight - 8), fntArial12, "Characters", OnButtonOver, SelectCharacterFilterButton);
            EquipmentFilterButton = new EmptyBoxButton(new Rectangle(LeftSideX + 154, HeaderSectionY + 4, 90, HeaderSectionHeight - 8), fntArial12, "Equipment", OnButtonOver, SelectEquipmentFilterButton);
            ConsumableFilterButton = new EmptyBoxButton(new Rectangle(LeftSideX + 244, HeaderSectionY + 4, 100, HeaderSectionHeight - 8), fntArial12, "Consumable", OnButtonOver, SelectConsumableFilterButton);

            CurrentLocalPlayerButton = new EmptyDropDownButton(new Rectangle(400, 8, 120, 45), fntArial12, "M&K",
                new string[] { "M&K", "Gamepad 1", "Gamepad 2", "Gamepad 3", "Gamepad 4" }, OnButtonOver, null);

            UnitFilterButton.CanBeChecked = true;
            CharacterFilterButton.CanBeChecked = true;
            EquipmentFilterButton.CanBeChecked = true;
            ConsumableFilterButton.CanBeChecked = true;

            UnitFilterButton.Select();

            ArrayUIElement = new IUIElement[]
            {
            };

            ShopUnitWhiteScreen NewShopUnitScreen = new ShopUnitWhiteScreen(this, ActivePlayer.UnlockInventory, ActivePlayer.Inventory);
            ShopCharacterScreen NewShopEquipmentScreen = new ShopCharacterScreen();
            ShopEquipmentScreen NewShopWeaponsScreen = new ShopEquipmentScreen();
            ShopConsumableScreen NewShopItemsScreen = new ShopConsumableScreen();

            ArraySubScreen = new GameScreen[] { NewShopUnitScreen, NewShopEquipmentScreen, NewShopWeaponsScreen, NewShopItemsScreen };

            foreach (GameScreen ActiveScreen in ArraySubScreen)
            {
                ActiveScreen.Content = Content;
                ActiveScreen.ListGameScreen = ListGameScreen;
                ActiveScreen.Load();
            }

            ActiveSubScreen = NewShopUnitScreen;

            IndexedLinesEffect = new BasicEffect(GraphicsDevice);
            IndexedLinesEffect.VertexColorEnabled = true;
            IndexedLinesEffect.DiffuseColor = new Vector3(1, 1, 1);

            VertexPositionColor[] ArrayVertex = new VertexPositionColor[24];

            short[] ArrayBackgroundGridIndices = new short[(ArrayVertex.Length * 2) - 2];
            for (int i = 0; i < ArrayBackgroundGridIndices.Length; ++i)
            {
                ArrayBackgroundGridIndices[i] = (short)(i);
            }

            Color LineColor = Color.White;

            ArrayVertex[0] = new VertexPositionColor(new Vector3(-0.5f, 0.5f, -0.5f), LineColor);
            ArrayVertex[1] = new VertexPositionColor(new Vector3(0.5f, 0.5f, -0.5f), LineColor);
            ArrayVertex[2] = new VertexPositionColor(new Vector3(0.5f, 0.5f, -0.5f), LineColor);
            ArrayVertex[3] = new VertexPositionColor(new Vector3(0.5f, -0.5f, -0.5f), LineColor);
            ArrayVertex[4] = new VertexPositionColor(new Vector3(0.5f, -0.5f, -0.5f), LineColor);
            ArrayVertex[5] = new VertexPositionColor(new Vector3(-0.5f, -0.5f, -0.5f), LineColor);
            ArrayVertex[6] = new VertexPositionColor(new Vector3(-0.5f, -0.5f, -0.5f), LineColor);
            ArrayVertex[7] = new VertexPositionColor(new Vector3(-0.5f, 0.5f, -0.5f), LineColor);

            ArrayVertex[8] = new VertexPositionColor(new Vector3(-0.5f, 0.5f, -0.5f), LineColor);
            ArrayVertex[9] = new VertexPositionColor(new Vector3(-0.5f, 0.5f, 0.5f), LineColor);
            ArrayVertex[10] = new VertexPositionColor(new Vector3(0.5f, 0.5f, -0.5f), LineColor);
            ArrayVertex[11] = new VertexPositionColor(new Vector3(0.5f, 0.5f, 0.5f), LineColor);
            ArrayVertex[12] = new VertexPositionColor(new Vector3(0.5f, -0.5f, -0.5f), LineColor);
            ArrayVertex[13] = new VertexPositionColor(new Vector3(0.5f, -0.5f, 0.5f), LineColor);
            ArrayVertex[14] = new VertexPositionColor(new Vector3(-0.5f, -0.5f, -0.5f), LineColor);
            ArrayVertex[15] = new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0.5f), LineColor);

            ArrayVertex[16] = new VertexPositionColor(new Vector3(-0.5f, 0.5f, 0.5f), LineColor);
            ArrayVertex[17] = new VertexPositionColor(new Vector3(0.5f, 0.5f, 0.5f), LineColor);
            ArrayVertex[18] = new VertexPositionColor(new Vector3(0.5f, 0.5f, 0.5f), LineColor);
            ArrayVertex[19] = new VertexPositionColor(new Vector3(0.5f, -0.5f, 0.5f), LineColor);
            ArrayVertex[20] = new VertexPositionColor(new Vector3(0.5f, -0.5f, 0.5f), LineColor);
            ArrayVertex[21] = new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0.5f), LineColor);
            ArrayVertex[22] = new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0.5f), LineColor);
            ArrayVertex[23] = new VertexPositionColor(new Vector3(-0.5f, 0.5f, 0.5f), LineColor);

            BackgroundGrid = new IndexedLines(ArrayVertex, ArrayBackgroundGridIndices);
        }

        public override void Unload()
        {
            SoundSystem.ReleaseSound(sndButtonOver);
            SoundSystem.ReleaseSound(sndButtonClick);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (IUIElement ActiveButton in ArrayUIElement)
            {
                ActiveButton.Update(gameTime);
            }

            ActiveSubScreen.Update(gameTime);
        }

        private void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        #region Buttons

        private void SelectBackToLobbyButton()
        {
            sndButtonClick.Play();
            RemoveScreen(this);
        }

        private void SelectUnitFilterButton()
        {
            sndButtonClick.Play();
            ActiveSubScreen = ArraySubScreen[0];

            CharacterFilterButton.Unselect();
            EquipmentFilterButton.Unselect();
            ConsumableFilterButton.Unselect();
        }

        private void SelectCharacterFilterButton()
        {
            sndButtonClick.Play();
            ActiveSubScreen = ArraySubScreen[1];

            UnitFilterButton.Unselect();
            EquipmentFilterButton.Unselect();
            ConsumableFilterButton.Unselect();
        }

        private void SelectEquipmentFilterButton()
        {
            sndButtonClick.Play();
            ActiveSubScreen = ArraySubScreen[2];

            UnitFilterButton.Unselect();
            CharacterFilterButton.Unselect();
            ConsumableFilterButton.Unselect();
        }

        private void SelectConsumableFilterButton()
        {
            sndButtonClick.Play();
            ActiveSubScreen = ArraySubScreen[3];

            UnitFilterButton.Unselect();
            CharacterFilterButton.Unselect();
            EquipmentFilterButton.Unselect();
        }

        #endregion

        public override void BeginDraw(CustomSpriteBatch g)
        {
            ActiveSubScreen.BeginDraw(g);

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

            /*float aspectRatio = Constants.Width / Constants.Height;

            Vector3 position = new Vector3(0, 0, 6);

            Vector3 target = new Vector3(0, 0, 3);

            Vector3 up = Vector3.Up;
            Matrix View = Matrix.CreateLookAt(position, target, up);
            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    aspectRatio,
                                                                    0.1f, 1000);

            IndexedLinesEffect.View = View;
            IndexedLinesEffect.Projection = Projection;
            IndexedLinesEffect.World = Matrix.CreateRotationX(1) * Matrix.CreateRotationY(1);

            foreach (EffectPass pass in IndexedLinesEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                BackgroundGrid.Draw(g);
            }
*/
            LeftSideX = 10;
            LeftSideWidth = (int)(Constants.Width * 0.5);
            TopSectionHeight = (int)(Constants.Height * 0.15);
            HeaderSectionY = TopSectionHeight;
            HeaderSectionHeight = (int)(Constants.Height * 0.05);

            BottomSectionHeight = (int)(Constants.Height * 0.07);
            BottomSectionY = Constants.Height - BottomSectionHeight;

            MiddleSectionY = (HeaderSectionY + HeaderSectionHeight);
            MiddleSectionHeight = BottomSectionY - MiddleSectionY;


            g.End();
            g.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            ActiveSubScreen.Draw(g);

            g.End();
            g.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);

            int DrawX = LeftSideX + 10;
            int DrawY = HeaderSectionY - 30;
            g.Draw(sprButtonSmallActive, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
            g.DrawString(fntOxanimumBoldBig, "Units", new Vector2(DrawX + 54, DrawY + 20), Color.FromNonPremultiplied(65, 70, 65, 255));

            DrawX += 250;
            g.Draw(sprButtonSmallInactive, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
            g.DrawString(fntOxanimumBoldBig, "Characters", new Vector2(DrawX + 24, DrawY + 20), Color.FromNonPremultiplied(65, 70, 65, 255));

            DrawX += 250;
            g.Draw(sprButtonSmallInactive, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
            g.DrawString(fntOxanimumBoldBig, "Equipment", new Vector2(DrawX + 24, DrawY + 20), Color.FromNonPremultiplied(65, 70, 65, 255));

            DrawX += 250;
            g.Draw(sprButtonSmallInactive, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
            g.DrawString(fntOxanimumBoldBig, "Consumable", new Vector2(DrawX + 14, DrawY + 20), Color.FromNonPremultiplied(65, 70, 65, 255));

            DrawX = (int)(Constants.Width - (sprButtonBackToLobby.Width + 50) * Ratio);
            DrawY = (int)(26 * Ratio);
            g.Draw(sprButtonBackToLobby, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);

            g.DrawString(fntOxanimumBoldBig, "Return To Loby", new Vector2(DrawX + 94, DrawY + 20), Color.FromNonPremultiplied(65, 70, 65, 255));


            foreach (IUIElement ActiveButton in ArrayUIElement)
            {
                ActiveButton.Draw(g);
            }
        }

        private void DrawBackground(CustomSpriteBatch g)
        {
            Color TextColor = Color.FromNonPremultiplied(65, 70, 65, 255);
            g.GraphicsDevice.Clear(Color.FromNonPremultiplied(243, 243, 243, 255));
            float Ratio = Constants.Height / 2160f;

            g.DrawLine(sprPixel, new Vector2(-1000 * Ratio, 364 * Ratio), new Vector2(3000 * Ratio, -1346 * Ratio), Color.FromNonPremultiplied(233, 233, 233, 255), 240);
            //
            //g.DrawLine(sprPixel, new Vector2(-1000 * Ratio, (1216 + offset) * Ratio), new Vector2(3000 * Ratio, (-494 + offset) * Ratio), Color.FromNonPremultiplied(243, 243, 243, 255), 200);
            //g.DrawLine(sprPixel, new Vector2(-1000 * Ratio, 1216 * Ratio), new Vector2(3000 * Ratio, -670 * Ratio), Color.FromNonPremultiplied(243, 243, 243, 255), 600);

            //g.DrawLine(sprPixel, new Vector2(2700 * Ratio, 2232 * Ratio), new Vector2(3500 * Ratio, 1825 * Ratio), Color.FromNonPremultiplied(243, 243, 243, 255), 300);

            g.End();

            BlendState blend = new BlendState();
            blend.AlphaSourceBlend = Blend.One;
            blend.AlphaDestinationBlend = Blend.One;
            blend.ColorSourceBlend = Blend.One;
            blend.ColorDestinationBlend = Blend.One;
            blend.AlphaBlendFunction = BlendFunction.Min;

            g.Begin(SpriteSortMode.Deferred, blend);

            g.Draw(CubeRenderTarget, new Vector2(400, 180), Color.FromNonPremultiplied(5, 5, 5, 255));
            g.End();
            g.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            g.DrawLine(sprPixel, new Vector2(-1000 * Ratio, 2544 * Ratio), new Vector2(3000 * Ratio, 658 * Ratio), Color.FromNonPremultiplied(233, 233, 233, 255), 600);
            g.DrawLine(sprPixel, new Vector2(1800 * Ratio, 2238 * Ratio), new Vector2(3560 * Ratio, 1344 * Ratio), Color.FromNonPremultiplied(233, 233, 233, 255), 200);
            g.End();

            g.Begin(SpriteSortMode.Deferred, blend, SamplerState.AnisotropicWrap, DepthStencilState.Default, RasterizerState.CullNone);

            g.Draw(CubeRenderTarget, new Vector2(1022, 392), null, Color.FromNonPremultiplied(23, 23, 23, 255), 0f, Vector2.Zero, 0.51f, SpriteEffects.None, 0f);
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
            g.DrawString(fntOxanimumBoldTitle, "SHOP", new Vector2((int)(210 * Ratio), (int)(58 * Ratio)), TextColor);

            int DrawX = (int)(Constants.Width - (sprMoney.Width + 170) * Ratio);
            int DrawY = (int)(Constants.Height - 210 * Ratio);
            g.Draw(sprMoney, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
            g.DrawString(fntOxanimumBold, "Money:", new Vector2(DrawX + 24 * Ratio, DrawY + 34 * Ratio), TextColor);
            g.DrawStringRightAligned(fntOxanimumBold, "14360 cr", new Vector2(DrawX + (sprMoney.Width - 30) * Ratio, DrawY + 35 * Ratio), Color.White);
        }
    }
}
