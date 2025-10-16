using ProjectEternity.Core.Editor;
using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProjectEternity.Editors.TripleThunderEditor
{
    public partial class MapProperties : Form
    {
        private enum ItemSelectionChoices { BGM, SFX };

        private ItemSelectionChoices ItemSelectionChoice;

        public MapProperties()
        {
            InitializeComponent();
        }

        private void btnSetMusic_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.BGM;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathMapBGM, "Select BGM", false));
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
                    case ItemSelectionChoices.BGM:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(17);
                        txtMusic.Text = Name;
                        break;
                }
            }
        }
    }
}
