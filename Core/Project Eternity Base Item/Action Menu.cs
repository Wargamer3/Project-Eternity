using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.Core.Item
{
    public class ActionPanelHolder
    {
        private readonly List<ActionPanel> ListActionPanel;

        public ActionPanelHolder()
        {
            ListActionPanel = new List<ActionPanel>();
        }

        public void Add(ActionPanel NewActionPanel)
        {
            ListActionPanel.Add(NewActionPanel);
        }

        public void AddToPanelListAndSelect(ActionPanel Panel)
        {
            ListActionPanel.Add(Panel);
            Panel.OnSelect();
        }

        public void Remove(ActionPanel NewActionPanel)
        {
            ListActionPanel.Remove(NewActionPanel);
        }

        public void RemoveAllActionPanels()
        {
            ListActionPanel.Clear();
        }

        public void RemoveAllSubActionPanels()
        {
            if (ListActionPanel.Count > 1)
            {
                ListActionPanel.RemoveRange(1, ListActionPanel.Count - 1);
            }
        }

        public ActionPanel Last()
        {
            return ListActionPanel.Last();
        }

        public bool HasMainPanel
        {
            get { return ListActionPanel.Count > 0; }
        }

        public bool HasSubPanels
        {
            get { return ListActionPanel.Count > 1; }
        }
    }

    /// <summary>
    /// Used to make menus.
    /// </summary>
    public abstract class ActionPanel
    {
        public const int PannelHeight = 32;
        public const int MinActionMenuWidth = 90;
        public int ActionMenuWidth;

        public readonly string Name;
        protected readonly List<ActionPanel> ListNextChoice;//Array of choices to display.
        public int ActionMenuCursor;
        private readonly bool CanCancel;
        public bool IsEnabled;

        // List of ActionPanel shared through every ActionPanel objects.
        protected readonly ActionPanelHolder ListActionMenuChoice;

        public int MenuX;
        public int MenuY;

        public ActionPanel(string Name, ActionPanelHolder ListActionMenuChoice, bool CanCancel)
        {
            this.CanCancel = CanCancel;
            this.Name = Name;
            this.ListActionMenuChoice = ListActionMenuChoice;
            this.IsEnabled = true;
            ActionMenuWidth = MinActionMenuWidth;

            ListNextChoice = new List<ActionPanel>();
            if (ListActionMenuChoice.HasMainPanel)
            {
                ActionPanel LastPanel = ListActionMenuChoice.Last();

                MenuX = LastPanel.MenuX;
                MenuY = LastPanel.MenuY;
            }
        }

        public virtual void AddChoiceToCurrentPanel(ActionPanel Panel)
        {
            ListNextChoice.Add(Panel);
        }

        public void AddToPanelListAndSelect(ActionPanel Panel)
        {
            ListActionMenuChoice.AddToPanelListAndSelect(Panel);
        }

        public void AddToPanelList(ActionPanel Panel)
        {
            ListActionMenuChoice.Add(Panel);
        }

        public void RemoveFromPanelList(ActionPanel Panel)
        {
            ListActionMenuChoice.Remove(Panel);
        }

        public void ReplaceChoiceInCurrentPanel(ActionPanel Panel, Type PanelToReplace)
        {
            int CurrentIndex = -1;

            for (int P = 0; P < ListActionMenuChoice.Last().ListNextChoice.Count; P++)
            {
                if (ListActionMenuChoice.Last().ListNextChoice[P].GetType() == PanelToReplace)
                {
                    CurrentIndex = P;
                    break;
                }
            }

            if (CurrentIndex >= 0)
                ListActionMenuChoice.Last().ListNextChoice[CurrentIndex] = Panel;
        }

        /// <summary>
        /// This will crash the game if an action panel is not added immediately after.
        /// </summary>
        public void RemoveAllActionPanels()
        {
            ListActionMenuChoice.RemoveAllActionPanels();
        }

        public void RemoveAllSubActionPanels()
        {
            ListActionMenuChoice.RemoveAllSubActionPanels();
        }

        public override string ToString()
        {
            return Name;
        }

        public abstract void OnSelect();

        public void Update(GameTime gameTime)
        {
            if (CanCancel && InputHelper.InputCancelPressed())
            {
                CancelPanel();
            }
            else
            {
                DoUpdate(gameTime);
            }
        }

        public abstract void DoUpdate(GameTime gameTime);

        public void CancelPanel()
        {
            OnCancelPanel();

            RemoveFromPanelList(this);

            if (ListActionMenuChoice.HasSubPanels)
            {
                ListActionMenuChoice.Last().OnSelect();
            }
        }

        protected abstract void OnCancelPanel();

        public abstract void Draw(CustomSpriteBatch g);
    }
}
