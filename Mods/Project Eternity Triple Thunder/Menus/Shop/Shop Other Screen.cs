using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class ShopOtherScreen : ShopItemBaseScreen
    {
        #region Ressources

        private AnimatedSprite BuyItemIcon;

        #endregion

        private readonly List<MenuEquipment> ListShopEquipment;

        public ShopOtherScreen(Player Owner)
            : base(Owner)
        {
            ListShopEquipment = new List<MenuEquipment>();
        }

        public override void Load()
        {
            base.Load();

            ListShopEquipment.Add(new MenuEquipment("Armor 1", EquipmentTypes.Armor, 150, Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Icons/Armor 01 Icon"), Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Icons/Armor 01 Full")));

            BuyItemIcon = new AnimatedSprite(Content, "Triple Thunder/Menus/Shop/Buy Item Icon", Vector2.Zero, 0, 1, 4);

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!IsDragDropActive)
            {
                MenuEquipment EquipmentToBuy = GetShopEquipmentUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);
                if (EquipmentToBuy != null && InputHelper.InputConfirmPressed())
                {
                    PushScreen(new BuyEquipment(EquipmentToBuy, Owner));
                }
            }
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

        public override void Draw(CustomSpriteBatch g)
        {
            base.Draw(g);

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
        }
    }
}
