using System;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class ShopUnitScreen : GameScreen
    {
        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;

        private readonly ShopScreen Owner;

        private Squad DragAndDropEquipment;

        protected bool IsDragDropActive { get { return DragAndDropEquipment != null; } }
        BattleMapPlayerShopInventory Inventory;
        List<ShopItemUnit> ListUnitToBuy;

        public ShopUnitScreen(ShopScreen Owner)
        {
            this.Owner = Owner;
        }

        public override void Load()
        {
            Inventory = new BattleMapPlayerShopInventory();
            Inventory.PopulateUnlockedPlayerItems(Owner.ActivePlayer.Name);

            Inventory.UpdateAvailableItems(Owner.ActivePlayer.Level);
            ListUnitToBuy = new List<ShopItemUnit>(Inventory.ListAvailableUnitToBuy);

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");
        }

        public override void Update(GameTime gameTime)
        {
            if (Inventory.IsLoading)
            {
                Inventory.UpdateAvailableItems(Owner.ActivePlayer.Level);
                ListUnitToBuy = new List<ShopItemUnit>(Inventory.ListAvailableUnitToBuy);
            }
            Inventory.UpdateAvailableItems(Owner.ActivePlayer.Level);
            ListUnitToBuy = new List<ShopItemUnit>(Inventory.ListAvailableUnitToBuy);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            //Left side
            g.DrawString(fntArial12, "Units", new Vector2(10, Owner.TopSectionHeight + 5), Color.White);

            int DrawY = Owner.MiddleSectionY + 5;
            for (int i = 0; i < ListUnitToBuy.Count; ++i)
            {
                DrawBox(g, new Vector2(5, DrawY), Owner.LeftSideWidth - 95, 45, Color.White);
                DrawBox(g, new Vector2(11, DrawY + 4), 38, 38, Color.White);
                if (ListUnitToBuy[i].UnitToBuy != null)
                {
                    g.DrawString(fntArial12, ListUnitToBuy[i].UnitToBuy.ItemName, new Vector2(48, DrawY + 11), Color.White);
                    g.Draw(ListUnitToBuy[i].UnitToBuy.SpriteMap, new Vector2(11 + 3, DrawY + 7), Color.White);
                }

                DrawBox(g, new Vector2(Owner.LeftSideWidth - 90, DrawY), 85, 45, Color.White);
                g.DrawStringRightAligned(fntArial12, ListUnitToBuy[i].Price + " cr", new Vector2(Owner.LeftSideWidth - 12, DrawY + 11), Color.White);
                DrawY += 50;
            }
            for (int i = 0; i < Inventory.ListLockedUnit.Count; ++i)
            {
                DrawBox(g, new Vector2(5, DrawY), Owner.LeftSideWidth - 95, 45, Color.Gray);
                DrawBox(g, new Vector2(11, DrawY + 4), 38, 38, Color.Gray);
                if (Inventory.ListLockedUnit[i].UnitToBuy != null)
                {
                    g.DrawString(fntArial12, Inventory.ListLockedUnit[i].UnitToBuy.ItemName, new Vector2(48, DrawY + 11), Color.White);
                    g.Draw(Inventory.ListLockedUnit[i].UnitToBuy.SpriteMap, new Vector2(11 + 3, DrawY + 7), Color.White);
                }

                DrawBox(g, new Vector2(Owner.LeftSideWidth - 90, DrawY), 85, 45, Color.Gray);
                g.DrawStringRightAligned(fntArial12, Inventory.ListLockedUnit[i].Price + " cr", new Vector2(Owner.LeftSideWidth - 12, DrawY + 11), Color.White);
                DrawY += 50;
            }

            //Right side
            DrawY = Owner.MiddleSectionY + 5;
            for (int i = 0; i < Owner.ActivePlayer.Inventory.ListOwnedSquad.Count; ++i)
            {
                DrawBox(g, new Vector2(Owner.LeftSideWidth + 5, DrawY), Owner.LeftSideWidth - 95, 45, Color.White);
                g.DrawString(fntArial12, Owner.ActivePlayer.Inventory.ListOwnedSquad[i].CurrentLeader.ItemName, new Vector2(Owner.LeftSideWidth + 48, DrawY + 11), Color.White);
                DrawBox(g, new Vector2(Owner.LeftSideWidth + 11, DrawY + 4), 38, 38, Color.White);
                g.Draw(Owner.ActivePlayer.Inventory.ListOwnedSquad[i].CurrentLeader.SpriteMap, new Vector2(Owner.LeftSideWidth + 11 + 3, DrawY + 7), Color.White);

                if (MouseHelper.MouseStateCurrent.X >= Owner.LeftSideWidth + 11 && MouseHelper.MouseStateCurrent.X < Owner.LeftSideWidth + 11 + 38
                    && MouseHelper.MouseStateCurrent.Y >= DrawY + 4 && MouseHelper.MouseStateCurrent.Y < DrawY + 4 + 38)
                {
                    g.Draw(sprPixel, new Rectangle(Owner.LeftSideWidth + 11, DrawY + 4, 38, 38), Color.FromNonPremultiplied(255, 255, 255, 127));
                }
                DrawY += 50;
            }

            if (DragAndDropEquipment != null)
            {
                g.Draw(DragAndDropEquipment.CurrentLeader.SpriteMap, new Vector2(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y), Color.White);
            }
        }
    }
}
