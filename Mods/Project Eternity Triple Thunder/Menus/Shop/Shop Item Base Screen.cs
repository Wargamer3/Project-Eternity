using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FMOD;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class ShopItemBaseScreen : GameScreen
    {
        #region Ressources

        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        protected SpriteFont fntText;

        private Texture2D sprMyCharactersBackground;
        private AnimatedSprite MyCharacterButton;
        private AnimatedSprite MyCharacterOutline;
        private AnimatedSprite MyItemOutline;
        private AnimatedSprite MyWeaponOutline;

        private Texture2D sprMyEquipmentBackground;
        private InteractiveButton ResetSlotButton;

        private InteractiveButton[] ArrayMenuButton;

        #endregion

        protected readonly Player Owner;
        private readonly PlayerInventory PlayerInventory;
        
        private MenuEquipment DragAndDropEquipment;

        protected bool IsDragDropActive { get { return DragAndDropEquipment != null; } }

        public ShopItemBaseScreen(Player Owner)
        {
            this.Owner = Owner;
            PlayerInventory = Owner.Equipment;

            DragAndDropEquipment = null;
        }

        public override void Load()
        {
            #region Ressources

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            fntText = Content.Load<SpriteFont>("Fonts/Arial10");

            sprMyCharactersBackground = Content.Load<Texture2D>("Triple Thunder/Menus/Shop/My Characters Background");
            sprMyEquipmentBackground = Content.Load<Texture2D>("Triple Thunder/Menus/Shop/My Equipment Background");

            ResetSlotButton = new InteractiveButton(Content, "Triple Thunder/Menus/Shop/Reset Slot", new Vector2(535, 549), OnButtonOver, null);

            MyCharacterButton = new AnimatedSprite(Content, "Triple Thunder/Menus/Shop/My Character Icon", Vector2.Zero, 0, 1, 4);
            MyCharacterOutline = new AnimatedSprite(Content, "Triple Thunder/Menus/Shop/My Character Outline", Vector2.Zero, 0, 1, 4);
            MyCharacterOutline.SetFrame(2);
            MyItemOutline = new AnimatedSprite(Content, "Triple Thunder/Menus/Shop/My Item Outline", Vector2.Zero, 0, 1, 4);
            MyItemOutline.SetFrame(2);
            MyWeaponOutline = new AnimatedSprite(Content, "Triple Thunder/Menus/Shop/My Weapon Outline", Vector2.Zero, 0, 1, 4);
            MyWeaponOutline.SetFrame(2);

            ArrayMenuButton = new InteractiveButton[]
            {
                ResetSlotButton,
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
            if (!IsDragDropActive)
            {
                foreach (InteractiveButton ActiveButton in ArrayMenuButton)
                {
                    ActiveButton.Update(gameTime);
                }

                UpdateEquipmentPage();
            }
            else
            {
                DoDragDrop();
            }
        }

        #region Buttons Callback

        protected void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        #endregion

        private void StartDragDrop(MenuEquipment EquipmentToDrag)
        {
            DragAndDropEquipment = EquipmentToDrag;
        }

        private void DoDragDrop()
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

        private void UpdateEquipmentPage()
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
            DrawMyCharactersAndEquipment(g);

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
