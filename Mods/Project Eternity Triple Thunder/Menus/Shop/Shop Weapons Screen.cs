using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FMOD;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class ShopWeaponsScreen : GameScreen
    {
        public const string Pistol = "Pistol";
        public const string AssaultRifle = "Assault Rifle";
        public const string SniperRifle = "Sniper Rifle";
        public const string MachineGun = "Machine Gun";
        public const string SubMachineGun = "Sub Machine Gun";
        public const string Shotgun = "Shotgun";
        public const string GrenadeLauncher = "Grenade Launcher";

        #region Ressources

        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntText;

        private AnimatedSprite MyWeaponOutline;
        private Texture2D sprMyWeaponsBackground;

        private AnimatedSprite BuyWeaponIcon;

        private Scrollbar WeaponShopScrollbar;
        private Scrollbar WeaponInventoryScrollbar;

        private IUIElement[] ArrayMenuButton;

        #endregion

        private readonly Player Owner;
        private readonly PlayerInventory PlayerInventory;
        private readonly List<WeaponMenuEquipment> ListShopEquipment;

        private WeaponMenuEquipment DragAndDropEquipment;

        private int WeaponShopStartIndex;
        private int WeaponInventoryStartIndex;

        public ShopWeaponsScreen(Player Owner)
        {
            this.Owner = Owner;
            PlayerInventory = Owner.Equipment;

            ListShopEquipment = new List<WeaponMenuEquipment>();
            DragAndDropEquipment = null;
        }

        public override void Load()
        {
            #region Weapons

            ListShopEquipment.Add(new WeaponMenuEquipment("AWP", SniperRifle, 1, 150,
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/AWP Text"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/AWP Icon"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/AWP Full")));

            ListShopEquipment.Add(new WeaponMenuEquipment("Bubble Gun", MachineGun, 1, 150,
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/Bubble Gun Text"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/Bubble Gun Icon"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/Bubble Gun Full")));

            ListShopEquipment.Add(new WeaponMenuEquipment("Colt Python", Pistol, 1, 150,
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/Colt Python Text"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/Colt Python Icon"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/Colt Python Full")));

            ListShopEquipment.Add(new WeaponMenuEquipment("Dragunov", SniperRifle, 1, 150,
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/Dragunov Text"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/Dragunov Icon"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/Dragunov Full")));

            ListShopEquipment.Add(new WeaponMenuEquipment("F2000", MachineGun, 1, 150,
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/F2000 Text"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/F2000 Icon"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/F2000 Full")));

            ListShopEquipment.Add(new WeaponMenuEquipment("FAMAS", MachineGun, 1, 150,
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/FAMAS Text"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/FAMAS Icon"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/FAMAS Full")));

            ListShopEquipment.Add(new WeaponMenuEquipment("FN Five-seveN", Pistol, 1, 150,
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/FN Five-seveN Text"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/FN Five-seveN Icon"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/FN Five-seveN Full")));

            ListShopEquipment.Add(new WeaponMenuEquipment("FN Minimi", MachineGun, 1, 150,
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/FN Minimi Text"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/FN Minimi Icon"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/FN Minimi Full")));

            ListShopEquipment.Add(new WeaponMenuEquipment("FN P-90", SubMachineGun, 1, 150,
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/FN P-90 Text"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/FN P-90 Icon"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/FN P-90 Full")));

            ListShopEquipment.Add(new WeaponMenuEquipment("G3 SG1", SniperRifle, 1, 150,
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/G3 SG1 Text"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/G3 SG1 Icon"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/G3 SG1 Full")));

            ListShopEquipment.Add(new WeaponMenuEquipment("Glock 18C", Pistol, 1, 150,
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/Glock 18C Text"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/Glock 18C Icon"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/Glock 18C Full")));

            ListShopEquipment.Add(new WeaponMenuEquipment("GM94", Shotgun, 1, 150,
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/GM94 Text"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/GM94 Icon"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/GM94 Full")));

            ListShopEquipment.Add(new WeaponMenuEquipment("HK69A1", GrenadeLauncher, 1, 150,
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/HK69A1 Text"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/HK69A1 Icon"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/HK69A1 Full")));

            ListShopEquipment.Add(new WeaponMenuEquipment("IMI Uzi", SubMachineGun, 1, 150,
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/IMI Uzi Text"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/IMI Uzi Icon"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/IMI Uzi Full")));

            ListShopEquipment.Add(new WeaponMenuEquipment("Laser Gun", MachineGun, 1, 150,
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/Laser Gun Text"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/Laser Gun Icon"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/Laser Gun Full")));

            ListShopEquipment.Add(new WeaponMenuEquipment("M16A1", MachineGun, 1, 150,
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/M16A1 Text"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/M16A1 Icon"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/M16A1 Full")));

            ListShopEquipment.Add(new WeaponMenuEquipment("M16A2", MachineGun, 1, 150,
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/M16A2 Text"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/M16A2 Icon"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/M16A2 Full")));

            ListShopEquipment.Add(new WeaponMenuEquipment("M3", Shotgun, 1, 150,
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/M3 Text"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/M3 Icon"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/M3 Full")));

            ListShopEquipment.Add(new WeaponMenuEquipment("M60E4", MachineGun, 1, 150,
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/M60E4 Text"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/M60E4 Icon"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/M60E4 Full")));

            ListShopEquipment.Add(new WeaponMenuEquipment("MGL Mk 1", GrenadeLauncher, 1, 150,
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/MGL Mk 1 Text"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/MGL Mk 1 Icon"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/MGL Mk 1 Full")));

            ListShopEquipment.Add(new WeaponMenuEquipment("MP5", SubMachineGun, 1, 150,
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/MP5 Text"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/MP5 Icon"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/MP5 Full")));

            ListShopEquipment.Add(new WeaponMenuEquipment("MX1014", Shotgun, 1, 150,
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/MX1014 Text"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/MX1014 Icon"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/MX1014 Full")));

            ListShopEquipment.Add(new WeaponMenuEquipment("PSG-1", SniperRifle, 1, 150,
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/PSG-1 Text"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/PSG-1 Icon"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/PSG-1 Full")));

            ListShopEquipment.Add(new WeaponMenuEquipment("Skorpion", SubMachineGun, 1, 150,
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/Skorpion Text"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/Skorpion Icon"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/Skorpion Full")));

            ListShopEquipment.Add(new WeaponMenuEquipment("SPAS-12", Shotgun, 1, 150,
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/SPAS-12 Text"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/SPAS-12 Icon"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/SPAS-12 Full")));

            ListShopEquipment.Add(new WeaponMenuEquipment("Steyr AUG", SniperRifle, 1, 150,
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/Steyr AUG Text"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/Steyr AUG Icon"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/Steyr AUG Full")));

            ListShopEquipment.Add(new WeaponMenuEquipment("Type 14 Nambu", Pistol, 1, 150,
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/Type 14 Nambu Text"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/Type 14 Nambu Icon"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/Type 14 Nambu Full")));

            ListShopEquipment.Add(new WeaponMenuEquipment("USAS-12", Shotgun, 1, 150,
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/USAS-12 Text"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/USAS-12 Icon"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/USAS-12 Full")));

            ListShopEquipment.Add(new WeaponMenuEquipment("X-MAS Gun", Shotgun, 1, 150,
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/X-MAS Gun Text"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/X-MAS Gun Icon"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/X-MAS Gun Full")));

            #endregion

            #region Ressources

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            fntText = Content.Load<SpriteFont>("Fonts/Arial10");

            sprMyWeaponsBackground = Content.Load<Texture2D>("Triple Thunder/Menus/Shop/My Weapons Background");

            BuyWeaponIcon = new AnimatedSprite(Content, "Triple Thunder/Menus/Shop/Buy Weapon Icon", Vector2.Zero, 0, 1, 4);

            MyWeaponOutline = new AnimatedSprite(Content, "Triple Thunder/Menus/Shop/My Weapon Outline", Vector2.Zero, 0, 1, 4);
            MyWeaponOutline.SetFrame(2);

            WeaponShopScrollbar = new Scrollbar(Content.Load<Texture2D>("Triple Thunder/Menus/Common/Scrollbar 1"), new Vector2(310, 120), 380, ListShopEquipment.Count - 6, OnWeaponShopScrollbarChange);
            WeaponInventoryScrollbar = new Scrollbar(Content.Load<Texture2D>("Triple Thunder/Menus/Common/Scrollbar 1"), new Vector2(528, 115), 448, 0, OnWeaponInventoryScrollbarChange);

            ArrayMenuButton = new IUIElement[]
            {
                WeaponShopScrollbar, WeaponInventoryScrollbar,
            };

            #endregion
        }

        public override void Unload()
        {
            SoundSystem.ReleaseSound(sndButtonOver);
            SoundSystem.ReleaseSound(sndButtonClick);
        }

        public override void Update(GameTime gameTime)
        {
            if (DragAndDropEquipment == null)
            {
                foreach (IUIElement ActiveButton in ArrayMenuButton)
                {
                    ActiveButton.Update(gameTime);
                }

                if (MouseHelper.InputLeftButtonPressed())
                {
                    WeaponMenuEquipment SelectedEquipment = GetOwnedWeaponUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);
                    if (SelectedEquipment != null)
                    {
                        StartDragDrop(SelectedEquipment);
                    }
                    else
                    {
                        WeaponMenuEquipment EquipmentToBuy = GetShopWeaponUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);
                        if (EquipmentToBuy != null)
                        {
                            PushScreen(new BuyWeapon(EquipmentToBuy, Owner));
                        }
                    }
                }
            }
            else
            {
                DoDragDrop();
            }
        }

        private void StartDragDrop(WeaponMenuEquipment EquipmentToDrag)
        {
            DragAndDropEquipment = EquipmentToDrag;
        }

        private void DoDragDrop()
        {
            if (InputHelper.InputConfirmReleased())
            {
                int XOffset = MyWeaponOutline.SpriteWidth / 2;
                int YOffset = MyWeaponOutline.SpriteHeight / 2;

                if (MouseHelper.MouseStateCurrent.X >= 700 - XOffset && MouseHelper.MouseStateCurrent.X < 700 + XOffset
                    && MouseHelper.MouseStateCurrent.Y >= 140 - YOffset && MouseHelper.MouseStateCurrent.Y < 140 + YOffset)
                {
                    PlayerInventory.SetPrimaryWeapon(DragAndDropEquipment, 0);
                }

                if (MouseHelper.MouseStateCurrent.X >= 700 - XOffset && MouseHelper.MouseStateCurrent.X < 700 + XOffset
                    && MouseHelper.MouseStateCurrent.Y >= 200 - YOffset && MouseHelper.MouseStateCurrent.Y < 200 + YOffset)
                {
                    PlayerInventory.SetPrimaryWeapon(DragAndDropEquipment, 1);
                }

                DragAndDropEquipment = null;
            }
        }

        private void OnWeaponShopScrollbarChange(float ScrollbarValue)
        {
            WeaponShopStartIndex = (int)ScrollbarValue;
        }

        private void OnWeaponInventoryScrollbarChange(float ScrollbarValue)
        {
            WeaponInventoryStartIndex = (int)ScrollbarValue;
        }

        private WeaponMenuEquipment GetOwnedWeaponUnderMouse(int X, int Y)
        {
            if (X >= 384 && X <= 384 + 126 && Y >= 117)
            {
                int EquipmentIndex = WeaponInventoryStartIndex + (Y - 117) / 55;
                if (EquipmentIndex < PlayerInventory.ListWeapon.Count)
                {
                    return PlayerInventory.ListWeapon[EquipmentIndex];
                }
            }


            return null;
        }

        private WeaponMenuEquipment GetShopWeaponUnderMouse(int X, int Y)
        {
            if (X >= 34 && X <= 312 && Y >= 127)
            {
                int EquipmentIndex = WeaponShopStartIndex + (Y - 128) / 60;
                if (EquipmentIndex < ListShopEquipment.Count)
                {
                    return ListShopEquipment[EquipmentIndex];
                }
            }

            return null;
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.Draw(sprMyWeaponsBackground, new Vector2(358, 84), Color.White);

            foreach (IUIElement ActiveButton in ArrayMenuButton)
            {
                ActiveButton.Draw(g);
            }

            float DrawY = 117;
            for (int C = WeaponInventoryStartIndex; C < PlayerInventory.ListWeapon.Count && C < WeaponShopStartIndex + 9; ++C)
            {
                g.Draw(PlayerInventory.ListWeapon[C].sprIcon, new Vector2(384, DrawY), Color.White);
                Rectangle PlayerInfoCollisionBox = new Rectangle(384,
                                                                (int)DrawY,
                                                                (int)PlayerInventory.ListWeapon[C].sprIcon.Width,
                                                                (int)PlayerInventory.ListWeapon[C].sprIcon.Height);
                if (PlayerInfoCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                {
                    MyWeaponOutline.Draw(g, new Vector2(384 + PlayerInventory.ListWeapon[C].sprIcon.Width / 2,
                        DrawY + PlayerInventory.ListWeapon[C].sprIcon.Height / 2), Color.White);
                }

                DrawY += 50;
            }

            DrawY = 128;
            for (int C = WeaponShopStartIndex; C < ListShopEquipment.Count && C < WeaponShopStartIndex + 6; ++C)
            {
                MenuEquipment EquipmentToBuy = GetShopWeaponUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);
                if (EquipmentToBuy == ListShopEquipment[C])
                {
                    BuyWeaponIcon.SetFrame(2);
                }
                else
                {

                    BuyWeaponIcon.SetFrame(0);
                }
                BuyWeaponIcon.Draw(g, new Vector2(172, DrawY + 24), Color.White);
                g.Draw(ListShopEquipment[C].sprText, new Vector2(43, DrawY), Color.White);
                g.Draw(ListShopEquipment[C].sprIcon, new Vector2(39, DrawY + 11), Color.White);
                g.DrawStringRightAligned(fntText, ListShopEquipment[C].Category, new Vector2(297, DrawY + 3), Color.White);
                g.DrawStringRightAligned(fntText, "Lv." + ListShopEquipment[C].MinLevel.ToString(), new Vector2(210, DrawY + 27), Color.White);
                g.DrawStringRightAligned(fntText, ListShopEquipment[C].Price + " CR", new Vector2(297, DrawY + 27), Color.White);

                DrawY += 60;
            }

            if (PlayerInventory.ArrayEquipedPrimaryWeapon[0] != null)
            {
                g.Draw(PlayerInventory.ArrayEquipedPrimaryWeapon[0].sprIcon, new Vector2(637, 122), Color.White);
            }
            if (PlayerInventory.ArrayEquipedPrimaryWeapon[1] != null)
            {
                g.Draw(PlayerInventory.ArrayEquipedPrimaryWeapon[1].sprIcon, new Vector2(637, 182), Color.White);
            }

            if (DragAndDropEquipment != null)
            {
                g.Draw(DragAndDropEquipment.sprIcon, new Vector2(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y), Color.White);
            }

            DrawDragDropOverlay(g);
        }

        private void DrawDragDropOverlay(CustomSpriteBatch g)
        {
            if (DragAndDropEquipment != null)
            {
                int XOffset = MyWeaponOutline.SpriteWidth / 2;
                int YOffset = MyWeaponOutline.SpriteHeight / 2;
                MyWeaponOutline.Draw(g, new Vector2(700, 140), Color.White);
                if (MouseHelper.MouseStateCurrent.X >= 700 - XOffset && MouseHelper.MouseStateCurrent.X < 700 + XOffset
                    && MouseHelper.MouseStateCurrent.Y >= 140 - YOffset && MouseHelper.MouseStateCurrent.Y < 140 + YOffset)
                {
                    MyWeaponOutline.Draw(g, new Vector2(700, 140), Color.White);
                }

                MyWeaponOutline.Draw(g, new Vector2(700, 200), Color.White);
                if (MouseHelper.MouseStateCurrent.X >= 700 - XOffset && MouseHelper.MouseStateCurrent.X < 700 + XOffset
                    && MouseHelper.MouseStateCurrent.Y >= 200 - YOffset && MouseHelper.MouseStateCurrent.Y < 200 + YOffset)
                {
                    MyWeaponOutline.Draw(g, new Vector2(700, 200), Color.White);
                }
            }
        }
    }
}
