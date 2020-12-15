using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens;

namespace Project_Eternity
{
    public sealed class IntermissionScreen : GameScreen
    {
        private struct PartMenu
        {
            public string Name;
            public string[] Categories;
            public bool Open;
            public PartMenu(string Name, string[] Categories)
            {
                this.Name = Name;
                this.Categories = Categories;
                this.Open = false;
            }
        };

        private enum MenuChoice { StartBattle = 0, View = 1, Customize = 2, Data = 3, Multiplayer = 4, Exit = 5 };

        PartMenu[] Menu;
        int SelectedChoice = 0;
        int SubMenu = -1;
        int SelectedAlpha = 150;
        bool SelectedAlphaAppearing = true;
        Texture2D sprRectangle;
        SoundEffect Up;
        SoundEffect Down;
        SpriteFont fntArial12;
        public IntermissionScreen()
            : base()
        {
            this.RequireDrawFocus = true;
        }

        public override void Load()
        {
            Menu = new PartMenu[] { new PartMenu("Start battle", new string[] { }),
                                    new PartMenu("View", new string[] {"Pilot View", "Unit View", "Parts View" }),
                                    new PartMenu("Customize", new string[] { "Unit Customize", "Pilot Customize", "Parts Equip", "Shop", "Forge"}),
                                    new PartMenu("Data", new string[] { "Save", "Load", "Options"}),
                                    new PartMenu("VR training", new string[] { }),
                                    new PartMenu("Exit", new string[] { }) };
            Menu[2].Open = true;
            sprRectangle = Content.Load<Texture2D>("Rectangle");
            Up = Content.Load<SoundEffect>("SFX/menu_up");
            Down = Content.Load<SoundEffect>("SFX/menu_down");
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
        }
        public override void Update(GameTime gameTime)
        {
            if (SelectedAlphaAppearing)
            {//Increment SelectedAlpha before comparing it to 200
                if (++SelectedAlpha >= 200)
                    SelectedAlphaAppearing = false;
            }
            else
            {//Decrement SelectedAlpha before comparing it to 55
                if (--SelectedAlpha <= 55)
                    SelectedAlphaAppearing = true;
            }
            if (KeyboardHelper.InputUpPressed())
            {
                if (Menu[SelectedChoice].Open)
                {
                    if (SubMenu > -1)
                        SubMenu--;
                    else
                    {
                        //Decrement SelectedChoice before comparing it to 0
                        if (--SelectedChoice < 0)
                            SelectedChoice = Menu.Count() - 1;
                        SubMenu = -1;
                    }
                }
                else
                {
                    SelectedChoice -= SelectedChoice > 0 ? 1 : 0;
                    if (Menu[SelectedChoice].Open)
                        SubMenu = Menu[SelectedChoice].Categories.Count() - 1;
                }
                Up.Play();
            }
            else if (KeyboardHelper.InputDownPressed())
            {
                if (Menu[SelectedChoice].Open)
                {
                    if (SubMenu < Menu[SelectedChoice].Categories.Count() - 1)
                        SubMenu++;
                    else
                    {
                        SelectedChoice++;
                        SubMenu = -1;
                    }
                }
                else
                    //Increment SelectedChoice before comparing it to maximum value.
                    if (++SelectedChoice > Menu.Count() - 1)
                        SelectedChoice = 0;
                Down.Play();
            }
            else if (KeyboardHelper.InputConfirmPressed())
            {
                switch ((MenuChoice)SelectedChoice)
                {
                    case MenuChoice.StartBattle:
                        GameScreen.PushScreen(new UnitSelection(3));

                        GameScreen.RemoveScreen(this);
                        break;
                    case MenuChoice.View:
                        if (SubMenu == 0)//Pilot
                        {

                        }
                        else if (SubMenu == 1)//Units
                        {

                        }
                        else if (SubMenu == 2)//Parts
                        {

                        }
                        GameScreen.PushScreen(new Shop());
                        break;
                    case MenuChoice.Customize:
                        if (SubMenu == 0)//Unit
                        {
                            GameScreen.PushScreen(new SquadSelection());
                        }
                        else if (SubMenu == 1)//Pilot
                        {
                            GameScreen.PushScreen(new PilotSelection());
                        }
                        else if (SubMenu == 2)//Parts
                        {
                            //GameScreen.PushScreen(new PartsEquipScreen(serviceProvider, UnitArmoredCore.ListUnitArmoredCore[0]));
                        }
                        else if (SubMenu == 3)//Shop
                        {
                            GameScreen.PushScreen(new Shop());
                        }
                        else if (SubMenu == 4)//Forge
                        {
                            GameScreen.PushScreen(new Forge());
                        }
                        break;
                    case MenuChoice.Data:
                        
                        break;
                    case MenuChoice.Multiplayer:
                        GameScreen.PushScreen(new MultiplayerScreen());
                        break;
                    case MenuChoice.Exit:
                        GameScreen.RemoveScreen(this);
                        break;
                }
            }
        }
        public override void Draw(SpriteBatch g)
        {
            int X = Game.Width / 2 - 60;
            int Y = Game.Height / 2 - fntArial12.LineSpacing / 2;
            int BaseY = Game.Height / 2 - fntArial12.LineSpacing / 2;
            if (SubMenu != -1)
                Y -= fntArial12.LineSpacing * (SubMenu + 1);
            for (int i = SelectedChoice; i < Menu.Count(); i++)
            {
                g.DrawString(fntArial12, Menu[i].Name, new Vector2(X, Y), Color.FromNonPremultiplied(255, 255, 255, 255 - (int)((Y - BaseY) / 10.0) * 10));
                if (Menu[i].Open)
                    for (int j = 0; j < Menu[i].Categories.Count(); j++)
                        g.DrawString(fntArial12, Menu[i].Categories[j], new Vector2(X + 10, Y += fntArial12.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, 255 - (int)((Y - BaseY) / 10.0) * 10));
                Y += fntArial12.LineSpacing  * 3;
            }
            Y = BaseY - fntArial12.LineSpacing * 3;
            if (SubMenu != -1)
                Y -= fntArial12.LineSpacing * (SubMenu + 1);
            for (int i = SelectedChoice - 1; i >= 0; i--)
            {
                if (Menu[i].Open)
                {
                    for (int j = 0; j < Menu[i].Categories.Count(); j++)
                    {
                        g.DrawString(fntArial12, Menu[i].Categories[j], new Vector2(X + 10, Y), Color.FromNonPremultiplied(255, 255, 255, 255 - (int)((BaseY - Y) / 10.0) * 10));
                        Y -= fntArial12.LineSpacing;
                    }
                    Y -= fntArial12.LineSpacing;
                }
                g.DrawString(fntArial12, Menu[i].Name, new Vector2(X, Y), Color.FromNonPremultiplied(255, 255, 255, 255 - (int)((BaseY - Y) / 10.0) * 10));

                Y -= fntArial12.LineSpacing * 3;
            }
            //Draw cursor.
            g.Draw(sprRectangle, new Rectangle(0, BaseY, Game.Width, fntArial12.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, SelectedAlpha));
        }
    }
}
