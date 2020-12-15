using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.Editors.UnitNormalEditor
{
    public partial class DetailsEditor : Form
    {
        private enum ItemSelectionChoices { MapSprite, UnitSprite };

        private ItemSelectionChoices ItemSelectionChoice;

        public DetailsEditor()
        {
            InitializeComponent();
        }

        private void btnMapSprite_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.MapSprite;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathUnitsNormalMapSprites));
        }

        private void btnChangeUnitSprite_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.UnitSprite;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathUnitsNormalUnitSprites));
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
                    case ItemSelectionChoices.MapSprite:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(32);
                        txtMapSprite.Text = Name;
                        break;

                    case ItemSelectionChoices.UnitSprite:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(33);
                        txtUnitSprite.Text = Name;
                        break;
                }
            }
        }
    }
}
