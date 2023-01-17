using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    /// <summary>
    /// Used to make menus.
    /// </summary>
    public abstract class BattleMapActionPanel : ActionPanel
    {
        protected PlayerInput ActiveInputManager;

        public BattleMapActionPanel(string Name, ActionPanelHolder ListActionMenuChoice, PlayerInput ActiveInputManager, bool CanCancel)
            : base(Name, ListActionMenuChoice, CanCancel)
        {
            this.ActiveInputManager = ActiveInputManager;
        }

        public override void AddChoiceToCurrentPanel(ActionPanel Panel)
        {
            base.AddChoiceToCurrentPanel(Panel);
            ActionMenuWidth = Math.Max(ActionMenuWidth, (int)TextHelper.fntShadowFont.MeasureString(Panel.ToString()).X + 35);
            UpdateFinalMenuPosition();
        }

        public override void AddChoicesToCurrentPanel(ActionPanel[] ArrayPanel)
        {
            base.AddChoicesToCurrentPanel(ArrayPanel);
            UpdateFinalMenuPosition();
        }

        protected void UpdateFinalMenuPosition()
        {
            FinalMenuX = BaseMenuX;
            FinalMenuY = BaseMenuY;

            if (FinalMenuX + ActionMenuWidth >= Constants.Width)
                FinalMenuX = Constants.Width - ActionMenuWidth;

            MenuHeight = (ListNextChoice.Count) * PannelHeight + 6 * 2;
            if (FinalMenuY + MenuHeight >= Constants.Height)
                FinalMenuY = Constants.Height - MenuHeight;
        }

        protected bool NavigateThroughNextChoices(FMOD.FMODSound sndSelection)
        {
            if (ActiveInputManager.InputUpPressed())
            {
                ActionMenuCursor -= (ActionMenuCursor > 0) ? 1 : 0;

                sndSelection.Play();
                return true;
            }
            else if (ActiveInputManager.InputDownPressed())
            {
                ActionMenuCursor += (ActionMenuCursor < ListNextChoice.Count - 1) ? 1 : 0;

                sndSelection.Play();
                return true;
            }
            else if (ActiveInputManager.InputMovePressed())
            {
                for (int C = 0; C < ListNextChoice.Count; C++)
                {
                    if (ActiveInputManager.IsInZone(FinalMenuX, FinalMenuY + 6 + C * PannelHeight, FinalMenuX + ActionMenuWidth, FinalMenuY + 6 + (C + 1) * PannelHeight))
                    {
                        if (ActionMenuCursor != C)
                        {
                            ActionMenuCursor = C;
                            sndSelection.Play();
                            return true;
                        }
                        break;
                    }
                }
            }

            return false;
        }

        protected bool ConfirmNextChoices(FMOD.FMODSound sndConfirm)
        {
            if (ActiveInputManager.InputConfirmPressed(FinalMenuX, FinalMenuY + 6 + ActionMenuCursor * PannelHeight,
                FinalMenuX + ActionMenuWidth, FinalMenuY + 6 + (ActionMenuCursor + 1) * PannelHeight))
            {
                AddToPanelListAndSelect(ListNextChoice[ActionMenuCursor]);
                sndConfirm.Play();
                return true;
            }

            return false;
        }

        public void DrawNextChoice(CustomSpriteBatch g)
        {
            //Draw the action panel.
            int X = FinalMenuX;
            int Y = FinalMenuY;

            X += 20;
            GameScreen.DrawBox(g, new Vector2(X, Y), ActionMenuWidth, MenuHeight, Color.White);
            X += 10;
            Y += 14;

            //Draw the choices.
            for (int A = 0; A < ListNextChoice.Count; A++)
            {
                if (ListNextChoice[A].IsEnabled)
                {
                    TextHelper.DrawText(g, ListNextChoice[A].ToString(), new Vector2(X, Y), Color.White);
                }
                else
                {
                    TextHelper.DrawText(g, ListNextChoice[A].ToString(), new Vector2(X, Y), Color.Gray);
                }
                Y += PannelHeight;
            }

            Y = BaseMenuY;
            if (Y + MenuHeight >= Constants.Height)
                Y = Constants.Height - MenuHeight;
            //Draw the menu cursor.
            g.Draw(GameScreen.sprPixel, new Rectangle(X, 9 + Y + ActionMenuCursor * PannelHeight, ActionMenuWidth - 20, PannelHeight - 5), Color.FromNonPremultiplied(255, 255, 255, 200));
        }

        public static Dictionary<string, BattleMapActionPanel> LoadFromAssembly(Assembly ActiveAssembly, Type TypeOfRequirement, params object[] Args)
        {
            Dictionary<string, BattleMapActionPanel> DicRequirement = new Dictionary<string, BattleMapActionPanel>();
            List<BattleMapActionPanel> ListSkillEffect = ReflectionHelper.GetObjectsFromBaseTypes<BattleMapActionPanel>(TypeOfRequirement, ActiveAssembly.GetTypes(), Args);

            foreach (BattleMapActionPanel Instance in ListSkillEffect)
            {
                DicRequirement.Add(Instance.Name, Instance);
            }

            return DicRequirement;
        }

        public static Dictionary<string, BattleMapActionPanel> LoadFromAssemblyFiles(string[] ArrayFilePath, Type TypeOfRequirement, params object[] Args)
        {
            Dictionary<string, BattleMapActionPanel> DicRequirement = new Dictionary<string, BattleMapActionPanel>();

            for (int F = 0; F < ArrayFilePath.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(ArrayFilePath[F]));
                foreach (KeyValuePair<string, BattleMapActionPanel> ActiveRequirement in LoadFromAssembly(ActiveAssembly, TypeOfRequirement, Args))
                {
                    DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
                }
            }

            return DicRequirement;
        }
    }
}
