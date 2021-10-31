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
        #region Ressources

        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntText;

        private AnimatedSprite MyWeaponOutline;
        private Texture2D sprMyWeaponsBackground;

        private AnimatedSprite BuyWeaponIcon;

        private InteractiveButton[] ArrayMenuButton;

        #endregion

        private readonly Player Owner;
        private readonly PlayerEquipment PlayerInventory;
        private readonly List<MenuEquipment> ListShopEquipment;
        
        private MenuEquipment DragAndDropEquipment;

        public ShopWeaponsScreen(Player Owner)
        {
            this.Owner = Owner;
            PlayerInventory = Owner.Equipment;

            ListShopEquipment = new List<MenuEquipment>();
            DragAndDropEquipment = null;
        }

        public override void Load()
        {
            ListShopEquipment.Add(new MenuEquipment("Armor 1", EquipmentTypes.Armor, 150, Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Icons/Armor 01 Icon"), Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Icons/Armor 01 Full")));

            #region Ressources

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            fntText = Content.Load<SpriteFont>("Fonts/Arial10");

            sprMyWeaponsBackground = Content.Load<Texture2D>("Triple Thunder/Menus/Shop/My Weapons Background");

            BuyWeaponIcon = new AnimatedSprite(Content, "Triple Thunder/Menus/Shop/Buy Weapon Icon", Vector2.Zero, 0, 1, 4);

            MyWeaponOutline = new AnimatedSprite(Content, "Triple Thunder/Menus/Shop/My Weapon Outline", Vector2.Zero, 0, 1, 4);
            MyWeaponOutline.SetFrame(2);

            ArrayMenuButton = new InteractiveButton[]
            {
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
                foreach (InteractiveButton ActiveButton in ArrayMenuButton)
                {
                    ActiveButton.Update(gameTime);
                }
            }
            else
            {
                DoDragDrop();
            }
        }

        private void StartDragDrop(MenuEquipment EquipmentToDrag)
        {
            DragAndDropEquipment = EquipmentToDrag;
        }

        private void DoDragDrop()
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.Draw(sprMyWeaponsBackground, new Vector2(358, 84), Color.White);

            foreach (InteractiveButton ActiveButton in ArrayMenuButton)
            {
                ActiveButton.Draw(g);
            }

            if (DragAndDropEquipment != null)
            {
                g.Draw(DragAndDropEquipment.sprIcon, new Vector2(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y), Color.White);
            }
        }
    }
}
