using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ProjectEternity.Core.Editor
{
    public partial class ItemSelector : Form
    {
        private List<string> SelectedItem;//Using this list to return Items in the order they were selected.

        public ItemSelector()
        {
            InitializeComponent();
            SelectedItem = new List<string>();
        }

        public ItemSelector(MenuFilter Items, bool MutipleSelection = true)
            :this()
        {
            AddMenu(Items);
            lvItems.MultiSelect = MutipleSelection;
        }

        public void AddMenu(MenuFilter Items, ListViewGroup ActiveNode = null)
        {
            ListViewGroup NewGroup;
            if (Items.ListItem != null)
            {
                foreach (KeyValuePair<string, string> Item in Items.ListItem)
                {
                    ListViewItem NewItem = new ListViewItem(Item.Key);
                    NewItem.Group = ActiveNode;
                    NewItem.Tag = Item.Value;
                    lvItems.Items.Add(NewItem);
                }
            }
            for (int M = 0; M < Items.ListMenu.Count; M++)
            {
                NewGroup = new ListViewGroup(Items.ListMenu.ElementAt(M).Key);
                lvItems.Groups.Add(NewGroup);
                AddMenu(Items.ListMenu.ElementAt(M).Value, NewGroup);
            }
        }

        public List<string> GetResult()
        {
            return SelectedItem;
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void lvItems_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            string ItemTag = (string)e.Item.Tag;
            if (e.IsSelected)
            {
                if (!SelectedItem.Contains(ItemTag))
                {
                    SelectedItem.Add(ItemTag);
                }
            }
            else
            {
                SelectedItem.Remove(ItemTag);
            }
        }
    }
}
