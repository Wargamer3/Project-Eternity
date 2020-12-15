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

        private Texture2D sprMyCharactersBackground;
        private AnimatedSprite MyCharacterButton;
        private AnimatedSprite MyCharacterOutline;
        private AnimatedSprite MyItemOutline;
        private AnimatedSprite MyWeaponOutline;

        private Texture2D sprMyEquipmentBackground;
        private Texture2D sprMyItemsBackground;
        private InteractiveButton ResetSlotButton;
        private Texture2D sprMyWeaponsBackground;

        private InteractiveButton BackToLobbyButton;

        private InteractiveButton CharacterFilterButton;
        private InteractiveButton EquipmentFilterButton;
        private InteractiveButton WeaponsFilterButton;
        private InteractiveButton ItemsFilterButton;
        private InteractiveButton OthersFilterButton;

        private AnimatedSprite BuyCharacterIcon;
        private AnimatedSprite BuyItemIcon;
        private AnimatedSprite BuyWeaponIcon;

        private InteractiveButton[] ArrayMenuButton;

        #endregion

        private ShopFilters ShopFilter;

        private readonly Player Owner;
        private readonly PlayerEquipment PlayerInventory;
        private readonly List<CharacterMenuEquipment> ListShopCharacter;
        private readonly List<MenuEquipment> ListShopEquipment;
        
        private MenuEquipment DragAndDropEquipment;

        public Shop(Player Owner)
        {
            this.Owner = Owner;
            PlayerInventory = Owner.Equipment;

            ShopFilter = ShopFilters.Characters;
            ListShopCharacter = new List<CharacterMenuEquipment>();
            ListShopEquipment = new List<MenuEquipment>();
            DragAndDropEquipment = null;
        }

        public override void Load()
        {
            ListShopCharacter.Add(new CharacterMenuEquipment("Soul", 100, null, Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Icons/Player Soul Portrait")));
            ListShopEquipment.Add(new MenuEquipment("Armor 1", EquipmentTypes.Armor, 150, Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Icons/Armor 01 Icon"), Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Icons/Armor 01 Full")));

            #region Ressources

            sndBGM = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/Music/Shop.mp3");
            sndBGM.SetLoop(true);
            sndBGM.PlayAsBGM();

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            fntText = Content.Load<SpriteFont>("Fonts/Arial10");

            sprBackground = Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Background");

            sprMyCharactersBackground = Content.Load<Texture2D>("Triple Thunder/Menus/Shop/My Characters Background");
            sprMyEquipmentBackground = Content.Load<Texture2D>("Triple Thunder/Menus/Shop/My Equipment Background");
            sprMyItemsBackground = Content.Load<Texture2D>("Triple Thunder/Menus/Shop/My Items Background");
            sprMyWeaponsBackground = Content.Load<Texture2D>("Triple Thunder/Menus/Shop/My Weapons Background");

            BackToLobbyButton = new InteractiveButton(Content, "Triple Thunder/Menus/Common/Back To Lobby", new Vector2(678, 28),
                                                            "Triple Thunder/Menus/Common/Back Arrow Icon", new Vector2(-86, 0), 6, OnButtonOver, ReturnToLobby);

            CharacterFilterButton = new InteractiveButton(Content, "Triple Thunder/Menus/Shop/Character Filter", new Vector2(60, 102), OnButtonOver, FilterCharacters);
            EquipmentFilterButton = new InteractiveButton(Content, "Triple Thunder/Menus/Shop/Equipment Filter", new Vector2(113, 102), OnButtonOver, FilterEquipment);
            WeaponsFilterButton = new InteractiveButton(Content, "Triple Thunder/Menus/Shop/Weapons Filter", new Vector2(169, 102), OnButtonOver, FilterWeapons);
            ItemsFilterButton = new InteractiveButton(Content, "Triple Thunder/Menus/Shop/Items Filter", new Vector2(225, 102), OnButtonOver, FilterItems);
            OthersFilterButton = new InteractiveButton(Content, "Triple Thunder/Menus/Shop/Others Filter", new Vector2(281, 102), OnButtonOver, FilterOther);

            ResetSlotButton = new InteractiveButton(Content, "Triple Thunder/Menus/Shop/Reset Slot", new Vector2(535, 549), OnButtonOver, null);

            BuyCharacterIcon = new AnimatedSprite(Content, "Triple Thunder/Menus/Shop/Buy Character Icon", Vector2.Zero, 0, 1, 4);
            BuyItemIcon = new AnimatedSprite(Content, "Triple Thunder/Menus/Shop/Buy Item Icon", Vector2.Zero, 0, 1, 4);
            BuyWeaponIcon = new AnimatedSprite(Content, "Triple Thunder/Menus/Shop/Buy Weapon Icon", Vector2.Zero, 0, 1, 4);

            MyCharacterButton = new AnimatedSprite(Content, "Triple Thunder/Menus/Shop/My Character Icon", Vector2.Zero, 0, 1, 4);
            MyCharacterOutline = new AnimatedSprite(Content, "Triple Thunder/Menus/Shop/My Character Outline", Vector2.Zero, 0, 1, 4);
            MyCharacterOutline.SetFrame(2);
            MyItemOutline = new AnimatedSprite(Content, "Triple Thunder/Menus/Shop/My Item Outline", Vector2.Zero, 0, 1, 4);
            MyItemOutline.SetFrame(2);
            MyWeaponOutline = new AnimatedSprite(Content, "Triple Thunder/Menus/Shop/My Weapon Outline", Vector2.Zero, 0, 1, 4);
            MyWeaponOutline.SetFrame(2);

            ArrayMenuButton = new InteractiveButton[]
            {
                BackToLobbyButton,
                CharacterFilterButton, EquipmentFilterButton, WeaponsFilterButton, ItemsFilterButton, OthersFilterButton,
                ResetSlotButton,
            };

            #endregion
        }

        public override void Unload()
        {
            SoundSystem.ReleaseSound(sndBGM);
            SoundSystem.ReleaseSound(sndButtonOver);
            SoundSystem.ReleaseSound(sndButtonClick);
        }

        public override void Update(GameTime gameTime)
        {
            if (FMODSystem.sndActiveBGM != sndBGM)
            {
                sndBGM.PlayAsBGM();
            }

            if (DragAndDropEquipment == null)
            {
                foreach (InteractiveButton ActiveButton in ArrayMenuButton)
                {
                    ActiveButton.Update(gameTime);
                }

                switch(ShopFilter)
                {
                    case ShopFilters.Characters:
                        UpdateCharacterPage();
                        break;
                    case ShopFilters.Equipment:
                        UpdateEquipmentPage();
                        break;
                }

            }
            else
            {
                DoDragDrop();
            }
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
        }

        private void FilterEquipment()
        {
            sndButtonClick.Play();
            ShopFilter = ShopFilters.Equipment;
        }

        private void FilterWeapons()
        {
            sndButtonClick.Play();
            ShopFilter = ShopFilters.Weapons;
        }

        private void FilterItems()
        {
            sndButtonClick.Play();
            ShopFilter = ShopFilters.Items;
        }

        private void FilterOther()
        {
            sndButtonClick.Play();
            ShopFilter = ShopFilters.Other;
        }

        #endregion

        private void StartDragDrop(MenuEquipment EquipmentToDrag)
        {
            DragAndDropEquipment = EquipmentToDrag;
        }

        private void DoDragDrop()
        {
            switch (ShopFilter)
            {
                case ShopFilters.Characters:
                    break;
                case ShopFilters.Equipment:
                    EquipmentDragDrop();
                    break;
            }
        }

        private void EquipmentDragDrop()
        {
            if (InputHelper.InputConfirmReleased())
            {
                switch (DragAndDropEquipment.EquipmentType)
                {
                    case EquipmentTypes.Etc:
                        if (MouseHelper.MouseStateCurrent.X >= 366 && MouseHelper.MouseStateCurrent.X < 366 + 37
                            && MouseHelper.MouseStateCurrent.Y >= 359 && MouseHelper.MouseStateCurrent.Y < 359 + 37)
                        {
                            PlayerInventory.SetEtc(DragAndDropEquipment);
                        }
                        break;

                    case EquipmentTypes.Head:
                        if (MouseHelper.MouseStateCurrent.X >= 446 && MouseHelper.MouseStateCurrent.X < 446 + 37
                            && MouseHelper.MouseStateCurrent.Y >= 355 && MouseHelper.MouseStateCurrent.Y < 355 + 37)
                        {
                            PlayerInventory.SetHead(DragAndDropEquipment);
                        }
                        break;

                    case EquipmentTypes.Armor:
                        if (MouseHelper.MouseStateCurrent.X >= 452 && MouseHelper.MouseStateCurrent.X < 452 + 37
                            && MouseHelper.MouseStateCurrent.Y >= 396 && MouseHelper.MouseStateCurrent.Y < 396 + 37)
                        {
                            PlayerInventory.SetArmor(DragAndDropEquipment);
                        }
                        break;

                    case EquipmentTypes.WeaponOption:
                        if (MouseHelper.MouseStateCurrent.X >= 383 && MouseHelper.MouseStateCurrent.X < 383 + 37
                            && MouseHelper.MouseStateCurrent.Y >= 434 && MouseHelper.MouseStateCurrent.Y < 434 + 37)
                        {
                            PlayerInventory.SetWeaponOption(DragAndDropEquipment);
                        }
                        break;

                    case EquipmentTypes.Booster:
                        if (MouseHelper.MouseStateCurrent.X >= 481 && MouseHelper.MouseStateCurrent.X < 481 + 37
                            && MouseHelper.MouseStateCurrent.Y >= 462 && MouseHelper.MouseStateCurrent.Y < 462 + 37)
                        {
                            PlayerInventory.SetBooster(DragAndDropEquipment);
                        }
                        break;

                    case EquipmentTypes.Shoes:
                        if (MouseHelper.MouseStateCurrent.X >= 389 && MouseHelper.MouseStateCurrent.X < 389 + 37
                            && MouseHelper.MouseStateCurrent.Y >= 521 && MouseHelper.MouseStateCurrent.Y < 521 + 37)
                        {
                            PlayerInventory.SetShoes(DragAndDropEquipment);
                        }
                        break;
                }

                DragAndDropEquipment = null;
            }
        }

        private void UpdateCharacterPage()
        {
            CharacterMenuEquipment SelectedCharacter = GetShopCharacterUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);
            if (SelectedCharacter != null && InputHelper.InputConfirmPressed())
            {
                PushScreen(new BuyCharacter(SelectedCharacter, Owner));
            }
        }

        private void UpdateEquipmentPage()
        {
            MenuEquipment EquipmentToBuy = GetShopEquipmentUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);
            if (EquipmentToBuy != null && InputHelper.InputConfirmPressed())
            {
                PushScreen(new BuyEquipment(EquipmentToBuy, Owner));
            }
            else
            {
                if (MouseHelper.InputLeftButtonPressed())
                {
                    MenuEquipment SelectedEquipment = GetOwnedEquipmentUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);
                    if (SelectedEquipment != null)
                    {
                        StartDragDrop(SelectedEquipment);
                    }
                }
            }
        }

        private CharacterMenuEquipment GetShopCharacterUnderMouse(int X, int Y)
        {
            if (X >= 34 && X <= 312 && Y >= 127)
            {
                int CharacterIndex = (Y - 152) / 60;
                if (CharacterIndex < ListShopCharacter.Count)
                {
                    return ListShopCharacter[CharacterIndex];
                }
            }

            return null;
        }

        private MenuEquipment GetShopEquipmentUnderMouse(int X, int Y)
        {
            if (X >= 34 && X <= 312 && Y >= 127)
            {
                int EquipmentIndex = (Y - 152) / 60;
                if (EquipmentIndex < ListShopEquipment.Count)
                {
                    return ListShopEquipment[EquipmentIndex];
                }
            }

            return null;
        }

        private MenuEquipment GetOwnedEquipmentUnderMouse(int MouseX, int MouseY)
        {
            if (MouseX >= 584 && MouseX < 780 && MouseY >= 382 && MouseY < 570)
            {
                int X = (MouseX - 584) / 49;
                int Y = (MouseY - 382) / 47;

                int EquipmentIndex = X + Y * 4;
                if (EquipmentIndex < PlayerInventory.ListEquipment.Count)
                {

                    Rectangle PlayerInfoCollisionBox = new Rectangle(584 + X * 49, 382 + Y * 47,
                                                                    PlayerInventory.ListEquipment[EquipmentIndex].sprIcon.Width,
                                                                    PlayerInventory.ListEquipment[EquipmentIndex].sprIcon.Height);

                    if (PlayerInfoCollisionBox.Contains(MouseX, MouseY))
                    {
                        return PlayerInventory.ListEquipment[EquipmentIndex];
                    }
                }
            }

            return null;
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.Draw(sprBackground, Vector2.Zero, Color.White);

            switch (ShopFilter)
            {
                case ShopFilters.Characters:
                    for (int C = 0; C < ListShopCharacter.Count; ++C)
                    {
                        CharacterMenuEquipment SelectedCharacter = GetShopCharacterUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);
                        if (SelectedCharacter == ListShopCharacter[C])
                        {
                            BuyCharacterIcon.SetFrame(2);
                        }
                        else
                        {

                            BuyCharacterIcon.SetFrame(0);
                        }
                        BuyCharacterIcon.Draw(g, new Vector2(172, 152 + C * 60), Color.White);
                        g.DrawString(fntText, ListShopCharacter[C].Name, new Vector2(102, 131 + C * 60), Color.White);
                        g.DrawString(fntText, "5", new Vector2(113, 155 + C * 60), Color.White);
                        g.DrawString(fntText, "5", new Vector2(145, 155 + C * 60), Color.White);
                        g.DrawString(fntText, "4", new Vector2(177, 155 + C * 60), Color.White);
                        g.DrawStringRightAligned(fntText, ListShopCharacter[C].Price + " CR", new Vector2(297, 155 + C * 60), Color.White);
                    }
                    DrawMyCharactersAndEquipment(g);
                    break;
                case ShopFilters.Equipment:
                    for (int C = 0; C < ListShopEquipment.Count; ++C)
                    {
                        MenuEquipment EquipmentToBuy = GetShopEquipmentUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);
                        if (EquipmentToBuy == ListShopEquipment[C])
                        {
                            BuyItemIcon.SetFrame(2);
                        }
                        else
                        {

                            BuyItemIcon.SetFrame(0);
                        }
                        BuyItemIcon.Draw(g, new Vector2(172, 152 + C * 60), Color.White);
                        g.Draw(ListShopEquipment[C].sprIcon, new Vector2(39, 131 + C * 60), Color.White);
                        g.DrawString(fntText, ListShopEquipment[C].Name, new Vector2(92, 131 + C * 60), Color.White);
                        g.DrawString(fntText, ListShopEquipment[C].EquipmentType.ToString(), new Vector2(92, 155 + C * 60), Color.White);
                        g.DrawStringRightAligned(fntText, ListShopEquipment[C].Price + " CR", new Vector2(297, 155 + C * 60), Color.White);
                    }
                    DrawMyCharactersAndEquipment(g);
                    break;
                case ShopFilters.Weapons:
                    g.Draw(sprMyWeaponsBackground, new Vector2(358, 84), Color.White);
                    break;
                case ShopFilters.Items:
                case ShopFilters.Other:
                    g.Draw(sprMyItemsBackground, new Vector2(358, 84), Color.White);
                    g.Draw(sprMyEquipmentBackground, new Vector2(358, 350), Color.White);
                    break;
            }

            foreach (InteractiveButton ActiveButton in ArrayMenuButton)
            {
                ActiveButton.Draw(g);
            }

            if (DragAndDropEquipment != null)
            {
                g.Draw(DragAndDropEquipment.sprIcon, new Vector2(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y), Color.White);
            }
        }

        private void DrawMyCharactersAndEquipment(CustomSpriteBatch g)
        {
            g.Draw(sprMyCharactersBackground, new Vector2(358, 84), Color.White);
            g.Draw(sprMyEquipmentBackground, new Vector2(358, 350), Color.White);

            for (int C = 0; C < PlayerInventory.ListCharacter.Count; ++C)
            {
                MyCharacterButton.Draw(g, new Vector2(680, 133 + C * 55), Color.White);
                g.DrawString(fntText, PlayerInventory.ListCharacter[C].Name, new Vector2(663, 113 + C * 55), Color.White);
                g.DrawString(fntText, "5", new Vector2(678, 135 + C * 55), Color.White);
                g.DrawString(fntText, "6", new Vector2(710, 135 + C * 55), Color.White);
                g.DrawString(fntText, "4", new Vector2(742, 135 + C * 55), Color.White);
                Rectangle PlayerInfoCollisionBox = new Rectangle(680 - (int)MyCharacterButton.Origin.X,
                                                                133 - (int)MyCharacterButton.Origin.Y + C * 55,
                                                                MyCharacterButton.SpriteWidth,
                                                                MyCharacterButton.SpriteHeight);
                if (PlayerInfoCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                {
                    MyCharacterOutline.Draw(g, new Vector2(680, 133 + C * 55), Color.White);
                }
            }

            for (int E = 0; E < PlayerInventory.ListEquipment.Count; ++E)
            {
                int X = 584 + (E % 4) * 49;
                int Y = 382 + (E / 4) * 47;
                g.Draw(PlayerInventory.ListEquipment[E].sprIcon, new Vector2(X, Y), Color.White);
                Rectangle PlayerInfoCollisionBox = new Rectangle(X,
                                                                Y,
                                                                PlayerInventory.ListEquipment[E].sprIcon.Width,
                                                                PlayerInventory.ListEquipment[E].sprIcon.Height);
                if (PlayerInfoCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                {
                    MyItemOutline.Draw(g, new Vector2(X + PlayerInventory.ListEquipment[E].sprIcon.Width /2, Y + PlayerInventory.ListEquipment[E].sprIcon.Height / 2), Color.White);
                }
            }

            if (PlayerInventory.EquipedEtc != null)
            {
                g.Draw(PlayerInventory.EquipedEtc.sprIcon, new Vector2(366, 359), Color.White);
            }
            if (PlayerInventory.EquipedHead != null)
            {
                g.Draw(PlayerInventory.EquipedHead.sprIcon, new Vector2(446, 355), Color.White);
            }
            if (PlayerInventory.EquipedArmor != null)
            {
                g.Draw(PlayerInventory.EquipedArmor.sprIcon, new Vector2(452, 396), Color.White);
            }
            if (PlayerInventory.EquipedWeaponOption != null)
            {
                g.Draw(PlayerInventory.EquipedWeaponOption.sprIcon, new Vector2(383, 434), Color.White);
            }
            if (PlayerInventory.EquipedBooster != null)
            {
                g.Draw(PlayerInventory.EquipedBooster.sprIcon, new Vector2(481, 462), Color.White);
            }
            if (PlayerInventory.EquipedShoes != null)
            {
                g.Draw(PlayerInventory.EquipedShoes.sprIcon, new Vector2(389, 521), Color.White);
            }

            DrawDragDropOverlay(g);
        }

        private void DrawDragDropOverlay(CustomSpriteBatch g)
        {
            if (DragAndDropEquipment != null)
            {
                switch (DragAndDropEquipment.EquipmentType)
                {
                    case EquipmentTypes.Etc:
                        MyItemOutline.Draw(g, new Vector2(384, 377), Color.White);
                        if (MouseHelper.MouseStateCurrent.X >= 366 && MouseHelper.MouseStateCurrent.X < 366 + 37
                            && MouseHelper.MouseStateCurrent.Y >= 359 && MouseHelper.MouseStateCurrent.Y < 359 + 37)
                        {
                            MyItemOutline.Draw(g, new Vector2(384, 377), Color.White);
                        }
                        break;

                    case EquipmentTypes.Head:
                        MyItemOutline.Draw(g, new Vector2(464, 373), Color.White);
                        if (MouseHelper.MouseStateCurrent.X >= 446 && MouseHelper.MouseStateCurrent.X < 446 + 37
                            && MouseHelper.MouseStateCurrent.Y >= 355 && MouseHelper.MouseStateCurrent.Y < 355 + 37)
                        {
                            MyItemOutline.Draw(g, new Vector2(464, 373), Color.White);
                        }
                        break;

                    case EquipmentTypes.Armor:
                        MyItemOutline.Draw(g, new Vector2(470, 414), Color.White);
                        if (MouseHelper.MouseStateCurrent.X >= 452 && MouseHelper.MouseStateCurrent.X < 452 + 37
                            && MouseHelper.MouseStateCurrent.Y >= 396 && MouseHelper.MouseStateCurrent.Y < 396 + 37)
                        {
                            MyItemOutline.Draw(g, new Vector2(470, 414), Color.White);
                        }
                        break;

                    case EquipmentTypes.WeaponOption:
                        MyItemOutline.Draw(g, new Vector2(401, 452), Color.White);
                        if (MouseHelper.MouseStateCurrent.X >= 383 && MouseHelper.MouseStateCurrent.X < 383 + 37
                            && MouseHelper.MouseStateCurrent.Y >= 434 && MouseHelper.MouseStateCurrent.Y < 434 + 37)
                        {
                            MyItemOutline.Draw(g, new Vector2(401, 452), Color.White);
                        }
                        break;

                    case EquipmentTypes.Booster:
                        MyItemOutline.Draw(g, new Vector2(499, 480), Color.White);
                        if (MouseHelper.MouseStateCurrent.X >= 481 && MouseHelper.MouseStateCurrent.X < 481 + 37
                            && MouseHelper.MouseStateCurrent.Y >= 462 && MouseHelper.MouseStateCurrent.Y < 462 + 37)
                        {
                            MyItemOutline.Draw(g, new Vector2(499, 480), Color.White);
                        }
                        break;

                    case EquipmentTypes.Shoes:
                        MyItemOutline.Draw(g, new Vector2(407, 539), Color.White);
                        if (MouseHelper.MouseStateCurrent.X >= 389 && MouseHelper.MouseStateCurrent.X < 389 + 37
                            && MouseHelper.MouseStateCurrent.Y >= 521 && MouseHelper.MouseStateCurrent.Y < 521 + 37)
                        {
                            MyItemOutline.Draw(g, new Vector2(407, 539), Color.White);
                        }
                        break;
                }
            }
        }
    }
}
