using System.Collections.Generic;
using System.Windows.Forms;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.Editors.TripleThunderWeaponEditor
{
    public partial class ProjectileEditor : Form
    {
        private enum ItemSelectionChoices { AnimationProjectile, TextureProjectile, AnimationTrail, TextureTrail };

        private ItemSelectionChoices ItemSelectionChoice;

        public bool IsProjectileAnimated;
        public bool IsTrailAnimated;

        public ProjectileEditor()
        {
            InitializeComponent();

            IsProjectileAnimated = false;
            IsTrailAnimated = false;
            cboProjectileEffect.SelectedIndex = 0;
            cboTrailStyle.SelectedIndex = 0;
            lstBulletTypes.Items.Add("Normal");
            lstBulletTypes.SelectedIndex = 0;
        }

        private void lstBulletTypes_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (lstBulletTypes.SelectedIndex >= 0)
            {
                txtBulletType.Text = lstBulletTypes.Items[lstBulletTypes.SelectedIndex].ToString();
            }
        }

        private void txtBulletType_TextChanged(object sender, System.EventArgs e)
        {
            if (lstBulletTypes.SelectedIndex >= 0)
            {
                lstBulletTypes.Items[lstBulletTypes.SelectedIndex] = txtBulletType.Text;
            }
        }

        private void btnAddBulletType_Click(object sender, System.EventArgs e)
        {
            lstBulletTypes.Items.Add("New type");
        }

        private void btnRemoveBulletType_Click(object sender, System.EventArgs e)
        {
            if (lstBulletTypes.SelectedIndex >= 0)
            {
                lstBulletTypes.Items.RemoveAt(lstBulletTypes.SelectedIndex);
            }
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

        private void cboTrailStyle_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (cboTrailStyle.SelectedIndex == 0 || cboTrailStyle.SelectedIndex == 1)
            {
                txtTrailPath.Enabled = false;
                btnTextureTrail.Enabled = false;
                btnAnimatedTrail.Enabled = false;
                cboTrailEffect.Enabled = false;
                cboTrailEffect.SelectedIndex = -1;
            }
            else if (cboTrailStyle.SelectedIndex == 2)
            {
                txtTrailPath.Enabled = true;
                btnTextureTrail.Enabled = true;
                btnAnimatedTrail.Enabled = true;
                cboTrailEffect.Enabled = true;
                cboTrailEffect.SelectedIndex = 1;
            }
        }

        private void btnAnimatedTrail_Click(object sender, System.EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.AnimationTrail;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathAnimations));
        }

        private void btnTextureTrail_Click(object sender, System.EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.TextureTrail;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathAnimationsSprites));
        }

        private void btnConfirm_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
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

                    case ItemSelectionChoices.AnimationTrail:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(30);
                        IsTrailAnimated = true;
                        txtTrailPath.Text = Name;
                        break;

                    case ItemSelectionChoices.TextureTrail:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(27);
                        IsTrailAnimated = false;
                        txtTrailPath.Text = Name;
                        break;
                }
            }
        }
    }
}
