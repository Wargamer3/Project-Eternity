using System;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public struct IndexedLines
    {
        public VertexPositionColor[] ArrayVertex;
        private short[] ArrayIndices;

        public IndexedLines(VertexPositionColor[] ArrayVertex, short[] ArrayIndices)
        {
            this.ArrayVertex = ArrayVertex;
            this.ArrayIndices = ArrayIndices;
        }

        internal void Draw(CustomSpriteBatch g)
        {
            g.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                PrimitiveType.LineList,
                ArrayVertex,
                0,  // vertex buffer offset to add to each element of the index buffer
                ArrayVertex.Length,  // number of vertices in pointList
                ArrayIndices,  // the index buffer
                0,  // first index element to read
                ArrayIndices.Length / 2 - 1   // number of primitives to draw
            );
        }
    }

    public class BattleMapInventoryScreen : GameScreen
    {
        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;

        private TunnelManager TunnelBackground;

        private IUIElement ReturnToLobbyButton;

        private EmptyBoxButton UnitFilterButton;
        private EmptyBoxButton CharacterFilterButton;
        private EmptyBoxButton EquipmentFilterButton;
        private EmptyBoxButton ConsumableFilterButton;
        private EmptyDropDownButton CurrentLocalPlayerButton;

        private IUIElement[] ArrayUIElement;

        public static int LeftSideWidth;
        public static int TopSectionHeight;
        public static int HeaderSectionY;
        public static int HeaderSectionHeight;

        public static int BottomSectionHeight;
        public static int BottomSectionY;

        public static int MiddleSectionY;
        public static int MiddleSectionHeight;

        private GameScreen[] ArraySubScreen;

        private GameScreen ActiveSubScreen;

        public BattleMapPlayer ActivePlayer;

        public BattleMapInventoryScreen()
        {
            ActivePlayer = (BattleMapPlayer)PlayerManager.ListLocalPlayer[0];

            LeftSideWidth = (int)(Constants.Width * 0.5 + 20);
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
            TunnelBackground = new TunnelManager();
            TunnelBackground.Load(GraphicsDevice);

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            ReturnToLobbyButton = new EmptyBoxButton(new Rectangle((int)(Constants.Width * 0.7f), 0, (int)(Constants.Width * 0.3), TopSectionHeight - 30), fntArial12, "Back To Lobby", OnButtonOver, SelectBackToLobbyButton);

            UnitFilterButton = new EmptyBoxButton(new Rectangle(LeftSideWidth + 4, HeaderSectionY + 4, 58, HeaderSectionHeight - 8), fntArial12, "Units", OnButtonOver, SelectUnitFilterButton);
            CharacterFilterButton = new EmptyBoxButton(new Rectangle(LeftSideWidth + 64, HeaderSectionY + 4, 88, HeaderSectionHeight - 8), fntArial12, "Characters", OnButtonOver, SelectCharacterFilterButton);
            EquipmentFilterButton = new EmptyBoxButton(new Rectangle(LeftSideWidth + 154, HeaderSectionY + 4, 88, HeaderSectionHeight - 8), fntArial12, "Equipment", OnButtonOver, SelectEquipmentFilterButton);
            ConsumableFilterButton = new EmptyBoxButton(new Rectangle(LeftSideWidth + 244, HeaderSectionY + 4, 98, HeaderSectionHeight - 8), fntArial12, "Consumable", OnButtonOver, SelectConsumableFilterButton);

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

            int MaxLoadouts = 0;
            int LoadoutSize = 0;

            IniFile IniUnlocks = IniFile.ReadFromFile("Content/Battle Lobby Player Unlocks.ini");
            foreach (string RequiredLevel in IniUnlocks.ReadAllValues("Loadout Slots"))
            {
                if (ActivePlayer.Level >= int.Parse(RequiredLevel))
                {
                    ++MaxLoadouts;
                }
            }

            foreach (string RequiredLevel in IniUnlocks.ReadAllValues("Loadout Sizes"))
            {
                if (ActivePlayer.Level >= int.Parse(RequiredLevel))
                {
                    ++LoadoutSize;
                }
            }

            for (int L = 0; L < MaxLoadouts; ++L)
            {
                PlayerLoadout ActiveLoadout;
                if (L >= ActivePlayer.Inventory.ListSquadLoadout.Count)
                {
                    ActiveLoadout = new PlayerLoadout();
                    ActivePlayer.Inventory.ListSquadLoadout.Add(ActiveLoadout);
                }
                else
                {
                    ActiveLoadout = ActivePlayer.Inventory.ListSquadLoadout[L];
                }

                for (int S = 0; S < LoadoutSize; S++)
                {
                    if (S >= ActiveLoadout.ListSpawnSquad.Count)
                    {
                        ActiveLoadout.ListSpawnSquad.Add(null);
                    }
                }
            }

            InventorySquadScreen NewShopCharacterScreen = new InventorySquadScreen(this, MaxLoadouts, LoadoutSize);
            InventoryCharacterScreen NewShopEquipmentScreen = new InventoryCharacterScreen(this, MaxLoadouts, LoadoutSize);
            InventoryEquipmentScreen NewShopWeaponsScreen = new InventoryEquipmentScreen();
            InventoryConsumableScreen NewShopItemsScreen = new InventoryConsumableScreen();

            ArraySubScreen = new GameScreen[] { NewShopCharacterScreen, NewShopEquipmentScreen, NewShopWeaponsScreen, NewShopItemsScreen };

            foreach (GameScreen ActiveScreen in ArraySubScreen)
            {
                ActiveScreen.Content = Content;
                ActiveScreen.ListGameScreen = ListGameScreen;
                ActiveScreen.Load();
            }

            ActiveSubScreen = NewShopCharacterScreen;
        }

        public override void Unload()
        {
            SoundSystem.ReleaseSound(sndButtonOver);
            SoundSystem.ReleaseSound(sndButtonClick);

            foreach (GameScreen ActiveScreen in ArraySubScreen)
            {
                ActiveScreen.Unload();
            }
        }

        public override void Update(GameTime gameTime)
        {
            TunnelBackground.Update(gameTime);

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
            GraphicsDevice.Clear(Lobby.BackgroundColor);

            TunnelBackground.Draw(g);

            g.End();
            g.Begin();

            ActiveSubScreen.Draw(g);

            g.Draw(sprPixel, new Rectangle((int)(Constants.Width * 0.05), TopSectionHeight - 5, (int)(Constants.Width * 0.9), 1), Color.White);

            //Left side
            g.Draw(sprPixel, new Rectangle((int)(Constants.Width * 0.05), BottomSectionY + 5, (int)(Constants.Width * 0.9), 1), Color.White);

            g.DrawString(fntArial12, "INVENTORY", new Vector2(10, 20), Color.White);

            g.DrawStringRightAligned(fntArial12, "Money: 14360 cr", new Vector2(LeftSideWidth - 12, BottomSectionY + 11), Color.FromNonPremultiplied(255, 255, 255, 0));

            foreach (IUIElement ActiveButton in ArrayUIElement)
            {
                ActiveButton.Draw(g);
            }
        }
    }
}
