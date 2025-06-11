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

        private TextButton ReturnToLobbyButton;

        private TextButton UnitFilterButton;
        private TextButton CharacterFilterButton;
        private TextButton EquipmentFilterButton;
        private TextButton ConsumableFilterButton;

        private CubeBackgroundSmall CubeBackground;

        private IUIElement[] ArrayUIElement;

        public static Color BackgroundColor = Color.FromNonPremultiplied(65, 70, 65, 255);

        public int LeftSideWidth;
        public int TopSectionHeight;
        public int HeaderSectionY;
        public int HeaderSectionHeight;

        public int BottomSectionHeight;
        public int BottomSectionY;

        public int MiddleSectionY;
        public int MiddleSectionHeight;

        private GameScreen[] ArraySubScreen;

        private GameScreen ActiveSubScreen;

        public BattleMapOnlineClient OnlineGameClient;
        public BattleMapPlayer ActivePlayer;

        public ShopScreen(BattleMapOnlineClient OnlineGameClient)
        {
            this.OnlineGameClient = OnlineGameClient;
            ActivePlayer = (BattleMapPlayer)PlayerManager.ListLocalPlayer[0];

            LeftSideWidth = (int)(Constants.Width * 0.5);
            TopSectionHeight = (int)(Constants.Height * 0.15);
            HeaderSectionY = TopSectionHeight;
            HeaderSectionHeight = (int)(Constants.Height * 0.05);

            BottomSectionHeight = (int)(Constants.Height * 0.07);
            BottomSectionY = Constants.Height - BottomSectionHeight;

            MiddleSectionY = (HeaderSectionY + HeaderSectionHeight);
            MiddleSectionHeight = BottomSectionY - MiddleSectionY;

            CubeBackground = new CubeBackgroundSmall();
        }

        public override void Load()
        {
            //Constants.graphics.PreferMultiSampling = true;
            //Constants.graphics.ApplyChanges();

            CubeBackground.Load(Content);

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            fntOxanimumBold = Content.Load<SpriteFont>("Fonts/Oxanium Bold");
            fntOxanimumBoldBig = Content.Load<SpriteFont>("Fonts/Oxanium Bold Big");
            fntOxanimumBoldTitle = Content.Load<SpriteFont>("Fonts/Oxanium Bold Title");

            sprButtonSmallActive = Content.Load<Texture2D>("Menus/Lobby/Button Tab");
            sprButtonSmallInactive = Content.Load<Texture2D>("Menus/Lobby/Shop/Button Inactive");
            sprButtonBackToLobby = Content.Load<Texture2D>("Menus/Lobby/Button Back To Lobby");
            sprMoney = Content.Load<Texture2D>("Menus/Lobby/Shop/Frame Money");

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            float Ratio = Constants.Height / 2160f;
            int DrawX = (int)(Constants.Width - 502 * Ratio);
            int DrawY = (int)(100 * Ratio);
            ReturnToLobbyButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Big}{Centered}{Color:65,70,65,255}Return To Lobby}}", "Menus/Lobby/Button Back To Lobby", new Vector2(DrawX, DrawY), 4, 1, Ratio, OnButtonOver, SelectBackToLobbyButton);

            DrawX = 280;
            DrawY = 342;
            UnitFilterButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Big}{Centered}{Color:65,70,65,255}Units}}", "Menus/Lobby/Button Tab", new Vector2(DrawX * Ratio, DrawY * Ratio), 4, 1, Ratio, OnButtonOver, SelectUnitFilterButton);
            DrawX += 500;
            CharacterFilterButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Big}{Centered}{Color:65,70,65,255}Characters}}", "Menus/Lobby/Button Tab", new Vector2(DrawX * Ratio, DrawY * Ratio), 4, 1, Ratio, OnButtonOver, SelectCharacterFilterButton);
            DrawX += 500;
            EquipmentFilterButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Big}{Centered}{Color:65,70,65,255}Equipment}}", "Menus/Lobby/Button Tab", new Vector2(DrawX * Ratio, DrawY * Ratio), 4, 1, Ratio, OnButtonOver, SelectEquipmentFilterButton);
            DrawX += 500;
            ConsumableFilterButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Big}{Centered}{Color:65,70,65,255}Consumable}}", "Menus/Lobby/Button Tab", new Vector2(DrawX * Ratio, DrawY * Ratio), 4, 1, Ratio, OnButtonOver, SelectConsumableFilterButton);

            UnitFilterButton.CanBeChecked = true;
            CharacterFilterButton.CanBeChecked = true;
            EquipmentFilterButton.CanBeChecked = true;
            ConsumableFilterButton.CanBeChecked = true;

            UnitFilterButton.Select();

            ArrayUIElement = new IUIElement[]
            {
                ReturnToLobbyButton,
                UnitFilterButton, CharacterFilterButton, EquipmentFilterButton, ConsumableFilterButton,
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

            CubeBackground.BeginDraw(g);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(BackgroundColor);

            DrawBackground(g);

            g.End();
            g.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            ActiveSubScreen.Draw(g);

            g.End();
            g.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);

            foreach (IUIElement ActiveButton in ArrayUIElement)
            {
                ActiveButton.Draw(g);
            }
        }

        private void DrawBackground(CustomSpriteBatch g)
        {
            CubeBackground.Draw(g);

            Color TextColor = Color.FromNonPremultiplied(65, 70, 65, 255);
            float Ratio = Constants.Height / 2160f;
            g.DrawString(fntOxanimumBoldTitle, "SHOP", new Vector2((int)(210 * Ratio), (int)(58 * Ratio)), TextColor);

            int DrawX = (int)(Constants.Width - (sprMoney.Width + 170) * Ratio);
            int DrawY = (int)(Constants.Height - 210 * Ratio);
            g.Draw(sprMoney, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0f);
            g.DrawString(fntOxanimumBold, "Money:", new Vector2(DrawX + 24 * Ratio, DrawY + 34 * Ratio), TextColor);
            g.DrawStringRightAligned(fntOxanimumBold, "14360 cr", new Vector2(DrawX + (sprMoney.Width - 30) * Ratio, DrawY + 35 * Ratio), Color.White);
        }
    }
}
