using System;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.Editors.CharacterEditor
{
    public partial class DetailsEditor : Form
    {
        private enum ItemSelectionChoices { Portrait, BustPortrait, BoxPortrait };

        private ItemSelectionChoices ItemSelectionChoice;

        public DetailsEditor()
        {
            InitializeComponent();
        }

        private void btnSelectPortrait_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Portrait;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathVisualNovelPortraits));
        }

        private void btnAddBustPortrait_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.BustPortrait;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathVisualNovelBustPortraits));
        }

        private void btnRemoveBustPortrait_Click(object sender, EventArgs e)
        {
            if (lstBust.SelectedIndex >= 0)
            {
                lstBust.Items.RemoveAt(lstBust.SelectedIndex);
            }
        }

        private void btnAddBoxPortrait_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.BoxPortrait;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathVisualNovelPortraits));
        }

        private void btnRemoveBoxPortrait_Click(object sender, EventArgs e)
        {
            if (lstBox.SelectedIndex >= 0)
            {
                lstBox.Items.RemoveAt(lstBox.SelectedIndex);
            }
        }

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            string Name;
            for (int I = 0; I < Items.Count; I++)
            {
                switch (ItemSelectionChoice)
                {
                    case ItemSelectionChoices.Portrait:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(32);
                        txtPortrait.Text = Name;
                        break;

                    case ItemSelectionChoices.BustPortrait:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(37);
                        lstBust.Items.Add(Name);
                        break;

                    case ItemSelectionChoices.BoxPortrait:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(32);
                        lstBox.Items.Add(Name);
                        break;
                }
            }
        }
    }
}
