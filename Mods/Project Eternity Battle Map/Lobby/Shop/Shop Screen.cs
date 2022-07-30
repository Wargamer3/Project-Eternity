﻿using System;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class ShopScreen : GameScreen
    {
        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;

        private BoxButton ReturnToLobbyButton;

        private BoxButton UnitFilterButton;
        private BoxButton CharacterFilterButton;
        private BoxButton EquipmentFilterButton;
        private BoxButton ConsumableFilterButton;

        private DropDownButton CurrentLocalPlayerButton;

        private IUIElement[] ArrayUIElement;

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
            ActivePlayer = PlayerManager.ListLocalPlayer[0];
            ActivePlayer.Level = 50;

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

            ReturnToLobbyButton = new BoxButton(new Rectangle((int)(Constants.Width * 0.7f), 0, (int)(Constants.Width * 0.3), TopSectionHeight), fntArial12, "Back To Lobby", OnButtonOver, SelectBackToLobbyButton);

            UnitFilterButton = new BoxButton(new Rectangle(LeftSideWidth + 4, HeaderSectionY + 4, 60, HeaderSectionHeight - 8), fntArial12, "Units", OnButtonOver, SelectUnitFilterButton);
            CharacterFilterButton = new BoxButton(new Rectangle(LeftSideWidth + 64, HeaderSectionY + 4, 90, HeaderSectionHeight - 8), fntArial12, "Characters", OnButtonOver, SelectCharacterFilterButton);
            EquipmentFilterButton = new BoxButton(new Rectangle(LeftSideWidth + 154, HeaderSectionY + 4, 90, HeaderSectionHeight - 8), fntArial12, "Equipment", OnButtonOver, SelectEquipmentFilterButton);
            ConsumableFilterButton = new BoxButton(new Rectangle(LeftSideWidth + 244, HeaderSectionY + 4, 100, HeaderSectionHeight - 8), fntArial12, "Consumable", OnButtonOver, SelectConsumableFilterButton);

            CurrentLocalPlayerButton = new DropDownButton(new Rectangle(400, 8, 120, 45), fntArial12, "M&K",
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

            ShopUnitScreen NewShopCharacterScreen = new ShopUnitScreen(this);
            ShopCharacterScreen NewShopEquipmentScreen = new ShopCharacterScreen();
            ShopEquipmentScreen NewShopWeaponsScreen = new ShopEquipmentScreen();
            ShopConsumableScreen NewShopItemsScreen = new ShopConsumableScreen();

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
            DrawBox(g, new Vector2(), Constants.Width, Constants.Height, Color.White);

            DrawBox(g, new Vector2(0, 0), (int)(Constants.Width * 0.7), TopSectionHeight, Color.White);
            g.DrawString(fntArial12, "SHOP", new Vector2(10, 20), Color.White);

            //Left side
            DrawBox(g, new Vector2(0, HeaderSectionY), LeftSideWidth, HeaderSectionHeight, Color.White);
            DrawBox(g, new Vector2(0, MiddleSectionY), LeftSideWidth, MiddleSectionHeight, Color.White);
            DrawBox(g, new Vector2(0, BottomSectionY), LeftSideWidth, Constants.Height - BottomSectionY, Color.White);
            DrawBox(g, new Vector2(LeftSideWidth, HeaderSectionY), LeftSideWidth, HeaderSectionHeight, Color.White);

            DrawBox(g, new Vector2(LeftSideWidth, MiddleSectionY), LeftSideWidth, MiddleSectionHeight, Color.White);


            DrawBox(g, new Vector2(LeftSideWidth, BottomSectionY), LeftSideWidth, Constants.Height - BottomSectionY, Color.White);
            g.DrawStringRightAligned(fntArial12, "Money: 14360 cr", new Vector2(LeftSideWidth - 12, BottomSectionY + 11), Color.White);

            ActiveSubScreen.Draw(g);

            foreach (IUIElement ActiveButton in ArrayUIElement)
            {
                ActiveButton.Draw(g);
            }
        }
    }
}