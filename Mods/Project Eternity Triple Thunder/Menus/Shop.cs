using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FMOD;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class Shop : GameScreen
    {
        private enum ShopFilters { Characters, Equipment, Weapons, Items, Other }

        #region Ressources

        private FMODSound sndBGM;
        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntText;

        private Texture2D sprBackground;

        private InteractiveButton BackToLobbyButton;

        private InteractiveButton CharacterFilterButton;
        private InteractiveButton EquipmentFilterButton;
        private InteractiveButton WeaponsFilterButton;
        private InteractiveButton ItemsFilterButton;
        private InteractiveButton OthersFilterButton;

        private InteractiveButton[] ArrayMenuButton;

        #endregion

        private ShopFilters ShopFilter;

        private readonly Player Owner;
        private readonly PlayerEquipment PlayerInventory;

        private GameScreen[] ArrayShopScreen;
        private GameScreen ActiveShopScreen;

        public Shop(Player Owner)
        {
            this.Owner = Owner;
            PlayerInventory = Owner.Equipment;

            ShopFilter = ShopFilters.Characters;
        }

        public override void Load()
        {
            #region Ressources

            sndBGM = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/Music/Shop.mp3");
            sndBGM.SetLoop(true);
            sndBGM.PlayAsBGM();

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            fntText = Content.Load<SpriteFont>("Fonts/Arial10");

            sprBackground = Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Background");

            BackToLobbyButton = new InteractiveButton(Content, "Triple Thunder/Menus/Common/Back To Lobby", new Vector2(678, 28),
                                                            "Triple Thunder/Menus/Common/Back Arrow Icon", new Vector2(-86, 0), 6, OnButtonOver, ReturnToLobby);

            CharacterFilterButton = new InteractiveButton(Content, "Triple Thunder/Menus/Shop/Character Filter", new Vector2(60, 102), OnButtonOver, FilterCharacters);
            EquipmentFilterButton = new InteractiveButton(Content, "Triple Thunder/Menus/Shop/Equipment Filter", new Vector2(113, 102), OnButtonOver, FilterEquipment);
            WeaponsFilterButton = new InteractiveButton(Content, "Triple Thunder/Menus/Shop/Weapons Filter", new Vector2(169, 102), OnButtonOver, FilterWeapons);
            ItemsFilterButton = new InteractiveButton(Content, "Triple Thunder/Menus/Shop/Items Filter", new Vector2(225, 102), OnButtonOver, FilterItems);
            OthersFilterButton = new InteractiveButton(Content, "Triple Thunder/Menus/Shop/Others Filter", new Vector2(281, 102), OnButtonOver, FilterOther);

            ArrayMenuButton = new InteractiveButton[]
            {
                BackToLobbyButton,
                CharacterFilterButton, EquipmentFilterButton, WeaponsFilterButton, ItemsFilterButton, OthersFilterButton,
            };

            #endregion

            ShopCharactersScreen NewShopCharacterScreen = new ShopCharactersScreen(Owner);
            ShopEquipmentScreen NewShopEquipmentScreen = new ShopEquipmentScreen(Owner);
            ShopWeaponsScreen NewShopWeaponsScreen = new ShopWeaponsScreen(Owner);
            ShopItemsScreen NewShopItemsScreen = new ShopItemsScreen(Owner);
            ShopOtherScreen NewShopOtherScreen = new ShopOtherScreen(Owner);

            ArrayShopScreen = new GameScreen[] { NewShopCharacterScreen, NewShopEquipmentScreen, NewShopWeaponsScreen, NewShopItemsScreen, NewShopOtherScreen };

            foreach (GameScreen ActiveScreen in ArrayShopScreen)
            {
                ActiveScreen.Content = Content;
                ActiveScreen.Load();
            }

            ActiveShopScreen = NewShopCharacterScreen;
        }

        public override void Unload()
        {
            SoundSystem.ReleaseSound(sndBGM);
            SoundSystem.ReleaseSound(sndButtonOver);
            SoundSystem.ReleaseSound(sndButtonClick);

            foreach (GameScreen ActiveScreen in ArrayShopScreen)
            {
                ActiveScreen.Unload();
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (FMODSystem.sndActiveBGM != sndBGM)
            {
                sndBGM.PlayAsBGM();
            }

            foreach (InteractiveButton ActiveButton in ArrayMenuButton)
            {
                ActiveButton.Update(gameTime);
            }

            ActiveShopScreen.Update(gameTime);
        }

        #region Buttons Callback

        private void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        private void ReturnToLobby()
        {
            sndButtonClick.Play();
            RemoveScreen(this);
        }

        private void FilterCharacters()
        {
            sndButtonClick.Play();
            ShopFilter = ShopFilters.Characters;
            ActiveShopScreen = ArrayShopScreen[0];
        }

        private void FilterEquipment()
        {
            sndButtonClick.Play();
            ShopFilter = ShopFilters.Equipment;
            ActiveShopScreen = ArrayShopScreen[1];
        }

        private void FilterWeapons()
        {
            sndButtonClick.Play();
            ShopFilter = ShopFilters.Weapons;
            ActiveShopScreen = ArrayShopScreen[2];
        }

        private void FilterItems()
        {
            sndButtonClick.Play();
            ShopFilter = ShopFilters.Items;
            ActiveShopScreen = ArrayShopScreen[3];
        }

        private void FilterOther()
        {
            sndButtonClick.Play();
            ShopFilter = ShopFilters.Other;
            ActiveShopScreen = ArrayShopScreen[4];
        }

        #endregion

        public override void Draw(CustomSpriteBatch g)
        {
            g.Draw(sprBackground, Vector2.Zero, Color.White);

            ActiveShopScreen.Draw(g);

            foreach (InteractiveButton ActiveButton in ArrayMenuButton)
            {
                ActiveButton.Draw(g);
            }
        }
    }
}
