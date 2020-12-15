using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.ControlHelper;
using System;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    /// <summary>
    /// Used to make menus.
    /// </summary>
    public abstract class BattleMapActionPanel : ActionPanel
    {
        public BattleMapActionPanel(string Name, ActionPanelHolder ListActionMenuChoice, bool CanCancel)
            : base(Name, ListActionMenuChoice, CanCancel)
        {
        }

        public override void AddChoiceToCurrentPanel(ActionPanel Panel)
        {
            base.AddChoiceToCurrentPanel(Panel);
            ActionMenuWidth = Math.Max(ActionMenuWidth, (int)GameScreen.fntShadowFont.MeasureString(Panel.Name).X + 35);
        }

        protected void NavigateThroughNextChoices(FMOD.FMODSound sndSelection, FMOD.FMODSound sndConfirm)
        {
            if (InputHelper.InputUpPressed())
            {
                ActionMenuCursor -= (ActionMenuCursor > 0) ? 1 : 0;

                sndSelection.Play();
            }
            else if (InputHelper.InputDownPressed())
            {
                ActionMenuCursor += (ActionMenuCursor < ListNextChoice.Count - 1) ? 1 : 0;

                sndSelection.Play();
            }
            else if (MouseHelper.MouseMoved())
            {
                int X = MenuX;
                int Y = MenuY;

                if (X + ActionMenuWidth >= Constants.Width)
                    X = Constants.Width - ActionMenuWidth;

                int MenuHeight = (ListNextChoice.Count) * PannelHeight + 6 * 2;
                if (Y + MenuHeight >= Constants.Height)
                    Y = Constants.Height - MenuHeight;

                if (MouseHelper.MouseStateCurrent.X >= X && MouseHelper.MouseStateCurrent.X < X + MinActionMenuWidth
                    && MouseHelper.MouseStateCurrent.Y >= Y + 6 && MouseHelper.MouseStateCurrent.Y < Y + MenuHeight - 12)
                {
                    int NewValue = (MouseHelper.MouseStateCurrent.Y - Y - 6) / PannelHeight;
                    if (NewValue != ActionMenuCursor)
                    {
                        ActionMenuCursor = NewValue;

                        sndSelection.Play();
                    }
                }
            }
            else if (InputHelper.InputConfirmPressed())
            {
                AddToPanelListAndSelect(ListNextChoice[ActionMenuCursor]);
                sndConfirm.Play();
            }
        }

        public void DrawNextChoice(CustomSpriteBatch g)
        {
            //Draw the action panel.
            int X = MenuX;
            int Y = MenuY;

            if (X + ActionMenuWidth >= Constants.Width)
                X = Constants.Width - ActionMenuWidth;

            int MenuHeight = (ListNextChoice.Count) * PannelHeight + 6 * 2;
            if (Y + MenuHeight >= Constants.Height)
                Y = Constants.Height - MenuHeight;

            X += 20;
            GameScreen.DrawBox(g, new Vector2(X, Y), ActionMenuWidth, MenuHeight, Color.White);
            X += 10;
            Y += 14;

            //Draw the choices.
            for (int A = 0; A < ListNextChoice.Count; A++)
            {
                GameScreen.DrawText(g, ListNextChoice[A].ToString(), new Vector2(X, Y), Color.White);
                Y += PannelHeight;
            }

            Y = MenuY;
            if (Y + MenuHeight >= Constants.Height)
                Y = Constants.Height - MenuHeight;
            //Draw the menu cursor.
            g.Draw(GameScreen.sprPixel, new Rectangle(X, 9 + Y + ActionMenuCursor * PannelHeight, ActionMenuWidth - 20, PannelHeight - 5), Color.FromNonPremultiplied(255, 255, 255, 200));
        }
    }
}
