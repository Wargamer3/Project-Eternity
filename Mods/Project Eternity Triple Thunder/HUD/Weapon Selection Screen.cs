using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.ControlHelper;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    class WeaponSelectionScreen : GameScreen
    {
        private class WeaponSelectionItem
        {
            private Texture2D sprIcon;
            private Texture2D sprText;

            public readonly string WeaponName;

            public bool IsDisabled;
            public bool IsSelected;

            public WeaponSelectionItem(string WeaponName)
            {
                this.WeaponName = WeaponName;
                IsDisabled = false;
                IsSelected = false;
            }

            public void Load(ContentManager Content)
            {
                sprIcon = Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/" + WeaponName + " Icon");
                sprText = Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Weapons/" + WeaponName + " Text");
            }

            public void Draw(CustomSpriteBatch g, float DrawX, float DrawY)
            {
                g.Draw(sprText, new Vector2(DrawX + 4, DrawY), Color.White);
                g.Draw(sprIcon, new Vector2(DrawX, DrawY + 11), Color.White);
            }
        }

        private Texture2D sprBackground;
        private Texture2D sprExtraWeaponBackground;
        private Texture2D sprNumber;

        private AnimatedSprite WeaponOutlineButton;

        private readonly Player Owner;
        private readonly List<WeaponSelectionItem> ListDefaultWeapon;
        private readonly List<WeaponSelectionItem> ListPersonalWeapon;
        private WeaponSelectionItem SelectedWeapon;

        public WeaponSelectionScreen(Player Owner)
        {
            this.Owner = Owner;
            ListDefaultWeapon = new List<WeaponSelectionItem>(6);
            ListPersonalWeapon = new List<WeaponSelectionItem>(6);
        }

        public override void Load()
        {
            sprBackground = Content.Load<Texture2D>("Triple Thunder/HUD/Menus/Weapon Selection/Background");
            sprExtraWeaponBackground = Content.Load<Texture2D>("Triple Thunder/HUD/Menus/Weapon Selection/Extra Weapon Background");
            sprNumber = Content.Load<Texture2D>("Triple Thunder/HUD/Menus/Weapon Selection/Number");

            WeaponOutlineButton = new AnimatedSprite(Content, "Triple Thunder/HUD/Menus/Weapon Selection/Weapon Button Outline", new Vector2(0, 0), 0, 1, 4);
            WeaponOutlineButton.SetFrame(2);

            ListDefaultWeapon.Add(new WeaponSelectionItem("AK-47"));
            ListDefaultWeapon.Add(new WeaponSelectionItem("Ingram MAC 10"));
            ListDefaultWeapon.Add(new WeaponSelectionItem("Scout"));
            ListDefaultWeapon.Add(new WeaponSelectionItem("M3"));
            ListDefaultWeapon.Add(new WeaponSelectionItem("M79"));
            ListDefaultWeapon.Add(new WeaponSelectionItem("MG42"));

            SelectedWeapon = ListDefaultWeapon[0];

            foreach (WeaponSelectionItem ActiveWeapon in ListDefaultWeapon)
            {
                ActiveWeapon.Load(Content);
            }

            foreach (WeaponMenuEquipment ActiveWeapon in Owner.Equipment.ArrayEquipedPrimaryWeapon)
            {
                ListPersonalWeapon.Add(new WeaponSelectionItem(ActiveWeapon.Name));
            }

            foreach (WeaponSelectionItem ActiveWeapon in ListPersonalWeapon)
            {
                ActiveWeapon.Load(Content);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (KeyboardHelper.KeyPressed(Keys.Escape))
            {
                RemoveScreen(this);
            }

            if (MouseHelper.InputLeftButtonPressed())
            {
                float X = Constants.Width / 2 - sprBackground.Width / 2;
                float DrawX = X + 68;
                float DrawY = 133;

                foreach (WeaponSelectionItem ActiveWeapon in ListDefaultWeapon)
                {
                    if (IsMouseOverItem(DrawX, DrawY))
                    {
                        SelectedWeapon = ActiveWeapon;
                    }
                    else
                    {
                        ActiveWeapon.IsSelected = false;
                    }

                    DrawY += 61;
                }

                DrawX = X + 268;
                DrawY = 133;
                foreach (WeaponSelectionItem ActiveWeapon in ListPersonalWeapon)
                {
                    if (IsMouseOverItem(DrawX, DrawY))
                    {
                        SelectedWeapon = ActiveWeapon;
                    }
                    else
                    {
                        ActiveWeapon.IsSelected = false;
                    }

                    DrawY += 61;
                }

                SelectedWeapon.IsSelected = true;
                Owner.Equipment.EquipedPrimaryWeapon = SelectedWeapon.WeaponName;
            }
        }

        private bool IsMouseOverItem(float DrawX, float DrawY)
        {
            return MouseHelper.MouseStateCurrent.X >= DrawX && MouseHelper.MouseStateCurrent.X < DrawX + 145
                && MouseHelper.MouseStateCurrent.Y >= DrawY && MouseHelper.MouseStateCurrent.Y < DrawY + 60;
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float X = Constants.Width / 2 - sprBackground.Width / 2;
            float Y = Constants.Height / 2 - sprBackground.Height / 2;

            g.Draw(sprBackground, new Vector2(X, Y), Color.White);

            float DrawX = X + 60;
            float DrawY = 133;
            int Number = 1;
            foreach (WeaponSelectionItem ActiveWeapon in ListDefaultWeapon)
            {
                if (ActiveWeapon.IsSelected)
                {
                    WeaponOutlineButton.SetFrame(3);
                }
                else if (ActiveWeapon.IsDisabled)
                {
                    WeaponOutlineButton.SetFrame(1);
                }
                else if (IsMouseOverItem(DrawX, DrawY))
                {
                    WeaponOutlineButton.SetFrame(2);
                }
                else
                {
                    WeaponOutlineButton.SetFrame(0);
                }

                DrawNumber(g, (int)DrawX - 17, (int)DrawY + 1, Number);
                WeaponOutlineButton.Draw(g, new Vector2(DrawX + 64, DrawY + 23), Color.White);
                ActiveWeapon.Draw(g, DrawX + 8, DrawY);
                DrawY += 61;
                ++Number;
            }

            DrawX = X + 260;
            DrawY = 133;
            foreach (WeaponSelectionItem ActiveWeapon in ListPersonalWeapon)
            {
                if (ActiveWeapon.IsSelected)
                {
                    WeaponOutlineButton.SetFrame(3);
                }
                else if (ActiveWeapon.IsDisabled)
                {
                    WeaponOutlineButton.SetFrame(1);
                }
                else if (IsMouseOverItem(DrawX, DrawY))
                {
                    WeaponOutlineButton.SetFrame(2);
                }
                else
                {
                    WeaponOutlineButton.SetFrame(0);
                }

                DrawNumber(g, (int)DrawX - 17, (int)DrawY + 1, Number);
                WeaponOutlineButton.Draw(g, new Vector2(DrawX + 64, DrawY + 23), Color.White);
                ActiveWeapon.Draw(g, DrawX + 8, DrawY);
                DrawY += 61;
                ++Number;
            }
        }

        private void DrawNumber(CustomSpriteBatch g, int DrawX, int DrawY, int Number)
        {
            g.Draw(sprNumber, new Rectangle(DrawX, DrawY, 15, 21), new Rectangle((Number - 1) * 15, 0, 15, 21), Color.White);
        }
    }
}
