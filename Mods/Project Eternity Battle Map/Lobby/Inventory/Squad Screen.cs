using System;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class InventorySquadScreen : GameScreen
    {
        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;

        private readonly BattleMapInventoryScreen Owner;

        private int MaxLoadouts;
        private int LoadoutSize;

        private Squad DragAndDropEquipment;

        protected bool IsDragDropActive { get { return DragAndDropEquipment != null; } }

        public InventorySquadScreen(BattleMapInventoryScreen Owner)
        {
            this.Owner = Owner;
            MaxLoadouts = 0;
            LoadoutSize = 0;
        }

        public override void Load()
        {
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            IniFile IniUnlocks = IniFile.ReadFromFile("Content/Battle Lobby Unlocks.ini");
            foreach (string RequiredLevel in IniUnlocks.ReadAllValues("Loadout Slots"))
            {
                if (Owner.ActivePlayer.Level >= int.Parse(RequiredLevel))
                {
                    ++MaxLoadouts;
                }
            }

            foreach (string RequiredLevel in IniUnlocks.ReadAllValues("Loadout Sizes"))
            {
                if (Owner.ActivePlayer.Level >= int.Parse(RequiredLevel))
                {
                    ++LoadoutSize;
                }
            }

            for (int L = 0; L < MaxLoadouts; ++L)
            {
                PlayerLoadout ActiveLoadout;
                if (L >= Owner.ActivePlayer.Inventory.ListSquadLoadout.Count)
                {
                    ActiveLoadout = new PlayerLoadout();
                    Owner.ActivePlayer.Inventory.ListSquadLoadout.Add(ActiveLoadout);
                }
                else
                {
                    ActiveLoadout = Owner.ActivePlayer.Inventory.ListSquadLoadout[L];
                }

                for (int S = 0; S < LoadoutSize; S++)
                {
                    if (S >= ActiveLoadout.ListSpawnSquad.Count)
                    {
                        ActiveLoadout.ListSpawnSquad.Add(null);
                    }
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsDragDropActive)
            {
                UpdateEquipmentPage();
            }
            else
            {
                DoDragDrop();
            }
        }

        private void UpdateEquipmentPage()
        {
            if (MouseHelper.InputLeftButtonPressed())
            {
                Squad SelectedEquipment = GetOwnedSquadUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);
                if (SelectedEquipment != null)
                {
                    StartDragDrop(SelectedEquipment);
                }
            }
        }

        private void StartDragDrop(Squad EquipmentToDrag)
        {
            DragAndDropEquipment = EquipmentToDrag;
        }

        private void DoDragDrop()
        {
            int DrawX = 101;
            int DrawY = BattleMapInventoryScreen.MiddleSectionY + 5 + 4;

            if (InputHelper.InputConfirmReleased())
            {
                int X = (MouseHelper.MouseStateCurrent.X - DrawX) / 40;
                int Y = (MouseHelper.MouseStateCurrent.Y - DrawY) / 40;

                int SquadSlotIndex = X;
                int SqaudLoatoutIndex = Y;
                if (SqaudLoatoutIndex < Owner.ActivePlayer.Inventory.ListSquadLoadout.Count && SquadSlotIndex < LoadoutSize)
                {
                    Rectangle PlayerInfoCollisionBox = new Rectangle(DrawX + X * 40, DrawY + Y * 40, 38, 38);

                    if (PlayerInfoCollisionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                    {
                        Owner.ActivePlayer.Inventory.ListSquadLoadout[SqaudLoatoutIndex].ListSpawnSquad[SquadSlotIndex] = DragAndDropEquipment;
                        Owner.ActivePlayer.SaveLocally();
                    }
                }

                DragAndDropEquipment = null;
            }
        }

        private Squad GetOwnedSquadUnderMouse(int MouseX, int MouseY)
        {
            int DrawX = BattleMapInventoryScreen.LeftSideWidth + 5;
            int DrawY = BattleMapInventoryScreen.MiddleSectionY + 5 + 4;

            if (MouseX >= DrawX && MouseX < DrawX + 38 && MouseY >= DrawY && MouseY < BattleMapInventoryScreen.MiddleSectionHeight)
            {
                int Y = (MouseY - DrawY) / 40;

                int SquadIndex = Y;
                if (SquadIndex < Owner.ActivePlayer.Inventory.ListOwnedSquad.Count)
                {
                    Rectangle PlayerInfoCollisionBox = new Rectangle(DrawX, DrawY + Y * 40, 38, 38);

                    if (PlayerInfoCollisionBox.Contains(MouseX, MouseY))
                    {
                        return Owner.ActivePlayer.Inventory.ListOwnedSquad[SquadIndex];
                    }
                }
            }

            return null;
        }

        public override void Draw(CustomSpriteBatch g)
        {
            //Left side
            g.DrawString(fntArial12, "Loadouts", new Vector2(10, BattleMapInventoryScreen.TopSectionHeight + 5), Color.White);

            int DrawY = BattleMapInventoryScreen.MiddleSectionY + 5;
            for (int L = 0; L < Owner.ActivePlayer.Inventory.ListSquadLoadout.Count; ++L)
            {
                DrawBox(g, new Vector2(5, DrawY), BattleMapInventoryScreen.LeftSideWidth - 10, 45, Color.White);
                DrawBox(g, new Vector2(BattleMapInventoryScreen.LeftSideWidth - 95, DrawY + 5), 85, 35, Color.White);
                g.DrawString(fntArial12, "Rename", new Vector2(BattleMapInventoryScreen.LeftSideWidth - 88, DrawY + 11), Color.White);
                g.DrawString(fntArial12, "Loadout " + (L + 1), new Vector2(11, DrawY + 11), Color.White);

                int DrawX = 101;
                for (int S = 0; S < Owner.ActivePlayer.Inventory.ListSquadLoadout[L].ListSpawnSquad.Count; S++)
                {
                    DrawBox(g, new Vector2(DrawX, DrawY + 4), 38, 38, Color.White);

                    Squad ActiveSquad = Owner.ActivePlayer.Inventory.ListSquadLoadout[L].ListSpawnSquad[S];
                    if (ActiveSquad != null)
                    {
                        g.Draw(ActiveSquad.CurrentLeader.SpriteMap, new Vector2(DrawX + 3, DrawY + 7), Color.White);
                    }

                    if (DragAndDropEquipment != null)
                    {
                        g.Draw(sprPixel, new Rectangle(DrawX, DrawY + 4, 38, 38), Color.FromNonPremultiplied(255, 255, 255, 127));
                    }
                    if (MouseHelper.MouseStateCurrent.X >= DrawX && MouseHelper.MouseStateCurrent.X < DrawX + 38
                        && MouseHelper.MouseStateCurrent.Y >= DrawY + 4 && MouseHelper.MouseStateCurrent.Y < DrawY + 4 + 38)
                    {
                        g.Draw(sprPixel, new Rectangle(DrawX, DrawY + 4, 38, 38), Color.FromNonPremultiplied(255, 255, 255, 127));
                    }
                    DrawX += 40;
                }
                DrawY += 50;
            }

            //Right side
            DrawY = BattleMapInventoryScreen.MiddleSectionY + 5;
            for (int i = 0; i < Owner.ActivePlayer.Inventory.ListOwnedSquad.Count; ++i)
            {
                DrawBox(g, new Vector2(BattleMapInventoryScreen.LeftSideWidth + 5, DrawY), BattleMapInventoryScreen.LeftSideWidth - 95, 45, Color.White);
                g.DrawString(fntArial12, Owner.ActivePlayer.Inventory.ListOwnedSquad[i].CurrentLeader.ItemName, new Vector2(BattleMapInventoryScreen.LeftSideWidth + 48, DrawY + 11), Color.White);
                DrawBox(g, new Vector2(BattleMapInventoryScreen.LeftSideWidth + 11, DrawY + 4), 38, 38, Color.White);
                g.Draw(Owner.ActivePlayer.Inventory.ListOwnedSquad[i].CurrentLeader.SpriteMap, new Vector2(BattleMapInventoryScreen.LeftSideWidth + 11 + 3, DrawY + 7), Color.White);

                if (MouseHelper.MouseStateCurrent.X >= BattleMapInventoryScreen.LeftSideWidth + 11 && MouseHelper.MouseStateCurrent.X < BattleMapInventoryScreen.LeftSideWidth + 11 + 38
                    && MouseHelper.MouseStateCurrent.Y >= DrawY + 4 && MouseHelper.MouseStateCurrent.Y < DrawY + 4 + 38)
                {
                    g.Draw(sprPixel, new Rectangle(BattleMapInventoryScreen.LeftSideWidth + 11, DrawY + 4, 38, 38), Color.FromNonPremultiplied(255, 255, 255, 127));
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
