using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.Editors.SorcererStreetCharacterEditor
{
    public partial class DetailsEditor : Form
    {
        private enum ItemSelectionChoices { ShopSprite,MapSprite,  Model };

        private ItemSelectionChoices ItemSelectionChoice;

        public DetailsEditor()
        {
            InitializeComponent();
        }

        private void btnChangeShopSprite_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.ShopSprite;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathSorcererStreetShopSprites));
        }

        private void btnChangeMapSprite_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.MapSprite;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathSorcererStreetMapSprites));
        }

        private void btnChange3DModel_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Model;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathSorcererStreetCharacterModels));
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
                    case ItemSelectionChoices.ShopSprite:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(48);
                        txtShopSprite.Text = Name;
                        break;

                    case ItemSelectionChoices.MapSprite:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(36);
                        txtMapSprite.Text = Name;
                        break;

                    case ItemSelectionChoices.Model:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(42);
                        txt3DModel.Text = Name;
                        break;
                }
            }
        }
    }
}
