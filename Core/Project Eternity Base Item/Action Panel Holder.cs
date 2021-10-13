using System.Linq;
using System.Collections.Generic;

namespace ProjectEternity.Core.Item
{
    public class ActionPanelHolder
    {
        private readonly List<ActionPanel> ListActionPanel;

        public ActionPanelHolder()
        {
            ListActionPanel = new List<ActionPanel>();
        }

        public virtual void Add(ActionPanel NewActionPanel)
        {
            ListActionPanel.Add(NewActionPanel);
        }

        public virtual void AddToPanelListAndSelect(ActionPanel NewActionPanel)
        {
            ListActionPanel.Add(NewActionPanel);
            NewActionPanel.OnSelect();
        }

        public void Set(ActionPanel[] ArrayNewActionPanel)
        {
            ListActionPanel.Clear();
            ListActionPanel.AddRange(ArrayNewActionPanel);
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

        public List<ActionPanel> GetActionPanels()
        {
            return new List<ActionPanel>(ListActionPanel);
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
}
