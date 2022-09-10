using System;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.Editors.AttackEditor
{
    public partial class PERAttackEditor : Form
    {
        private enum ItemSelectionChoices { AnimationProjectile, TextureProjectile, Model };

        private ItemSelectionChoices ItemSelectionChoice;

        public bool IsProjectileAnimated;

        public PERAttackEditor()
        {
            InitializeComponent();
        }

        private void btnUseAnimatedProjectile_Click(object sender, System.EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.AnimationProjectile;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathAnimations));
        }

        private void btnUseTextureProjectile_Click(object sender, System.EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.TextureProjectile;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathAnimationsSprites));
        }

        private void btn3DModelPath_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Model;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathAttackModels));
        }

        private void btnSelectSkillChain_Click(object sender, EventArgs e)
        {

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
                    case ItemSelectionChoices.AnimationProjectile:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(30);
                        IsProjectileAnimated = true;
                        txtProjectilePath.Text = Name;
                        break;

                    case ItemSelectionChoices.TextureProjectile:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(27);
                        IsProjectileAnimated = false;
                        txtProjectilePath.Text = Name;
                        break;

                    case ItemSelectionChoices.Model:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(23);
                        txt3DModelPath.Text = Name;
                        break;
                }
            }
        }
    }
}
