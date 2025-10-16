using System;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.Editors.AttackEditor
{
    public partial class PERAttackEditor : Form
    {
        private enum ItemSelectionChoices { AnimationProjectile, TextureProjectile, Model, SkillChain };

        private ItemSelectionChoices ItemSelectionChoice;

        public bool IsProjectileAnimated;

        public PERAttackEditor()
        {
            InitializeComponent();
        }

        private void btnUseAnimatedProjectile_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.AnimationProjectile;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathAnimations));
        }

        private void btnUseTextureProjectile_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.TextureProjectile;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathAnimationsSprites));
        }

        private void btn3DModelPath_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Model;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathAttackModels));
        }

        private void btnSelectSkillChain_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.SkillChain;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathAttackSkillChains));
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

                    case ItemSelectionChoices.SkillChain:
                        Name = Items[I].Substring(0, Items[0].Length - 5).Substring(29);
                        txtSkillChain.Text = Name;
                        break;
                }
            }
        }
    }
}
