using System;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class ShopScreen : GameScreen
    {
        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;

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

        public readonly int LeftSideX;
        public readonly int LeftSideWidth;
        public readonly int TopSectionHeight;
        public readonly int HeaderSectionY;
        public readonly int HeaderSectionHeight;

        public readonly int BottomSectionHeight;
        public readonly int BottomSectionY;

        public readonly int MiddleSectionY;
        public readonly int MiddleSectionHeight;

        private GameScreen[] ArraySubScreen;

        private GameScreen ActiveSubScreen;

        public BattleMapPlayer ActivePlayer;

        public ShopScreen()
        {
            ActivePlayer = (BattleMapPlayer)PlayerManager.ListLocalPlayer[0];

            LeftSideX = 10;
            LeftSideWidth = (int)(Constants.Width * 0.5);
            TopSectionHeight = (int)(Constants.Height * 0.1);
            HeaderSectionY = TopSectionHeight;
            HeaderSectionHeight = (int)(Constants.Height * 0.05);

            BottomSectionHeight = (int)(Constants.Height * 0.07);
            BottomSectionY = Constants.Height - BottomSectionHeight;

            MiddleSectionY = (HeaderSectionY + HeaderSectionHeight);
            MiddleSectionHeight = BottomSectionY - MiddleSectionY;
        }

        public override void Load()
        {
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");

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
                CurrentLocalPlayerButton, ReturnToLobbyButton,
                UnitFilterButton, CharacterFilterButton, EquipmentFilterButton, ConsumableFilterButton,
            };

            ShopUnitScreen NewShopUnitScreen = new ShopUnitScreen(this, ActivePlayer.UnlockInventory, ActivePlayer.Inventory);
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

        public override void Draw(CustomSpriteBatch g)
        {
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(BackgroundColor);

            float aspectRatio = Constants.Width / Constants.Height;

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

            g.End();
            g.Begin();

            ActiveSubScreen.Draw(g);

            foreach (IUIElement ActiveButton in ArrayUIElement)
            {
                ActiveButton.Draw(g);
            }
        }
    }
}
