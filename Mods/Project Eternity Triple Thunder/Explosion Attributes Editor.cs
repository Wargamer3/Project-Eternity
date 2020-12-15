using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public partial class ExplosionAttributesEditor : Form
    {
        private enum ItemSelectionChoices { AnimationProjectile, TextureProjectile, Sound };

        private ItemSelectionChoices ItemSelectionChoice;
        public Weapon.ExplosionOptions ExplosionAttributes;
        
        public ExplosionAttributesEditor()
        {
            InitializeComponent();
        }

        public ExplosionAttributesEditor(Weapon.ExplosionOptions ExplosionAttributes)
            : this()
        {
            this.ExplosionAttributes = ExplosionAttributes;

            txtExplosionRadius.Value = (decimal)ExplosionAttributes.ExplosionRadius;

            txtExplosionWindPowerAtCenter.Value = (decimal)ExplosionAttributes.ExplosionWindPowerAtCenter;
            txtExplosionWindPowerAtEdge.Value = (decimal)ExplosionAttributes.ExplosionWindPowerAtEdge;
            txtExplosionWindPowerToSelfMultiplier.Value = (decimal)ExplosionAttributes.ExplosionWindPowerToSelfMultiplier;

            txtExplosionDamageAtCenter.Value = (decimal)ExplosionAttributes.ExplosionDamageAtCenter;
            txtExplosionDamageAtEdge.Value = (decimal)ExplosionAttributes.ExplosionDamageAtEdge;
            txtExplosionDamageToSelfMultiplier.Value = (decimal)ExplosionAttributes.ExplosionDamageToSelfMultiplier;

            txtProjectilePath.Text = ExplosionAttributes.ExplosionAnimation.Path;
            txtSoundPath.Text = ExplosionAttributes.sndExplosionPath;
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

        private void btnSelectSound_Click(object sender, System.EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Sound;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathSFX));
        }

        private void btnConfirm_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void txtExplosionAttributes_ValueChanged(object sender, System.EventArgs e)
        {
            ExplosionAttributes.ExplosionRadius = (float)txtExplosionRadius.Value;

            ExplosionAttributes.ExplosionWindPowerAtCenter = (float)txtExplosionWindPowerAtCenter.Value;
            ExplosionAttributes.ExplosionWindPowerAtEdge = (float)txtExplosionWindPowerAtEdge.Value;
            ExplosionAttributes.ExplosionWindPowerToSelfMultiplier = (float)txtExplosionWindPowerToSelfMultiplier.Value;

            ExplosionAttributes.ExplosionDamageAtCenter = (float)txtExplosionDamageAtCenter.Value;
            ExplosionAttributes.ExplosionDamageAtEdge = (float)txtExplosionDamageAtEdge.Value;
            ExplosionAttributes.ExplosionDamageToSelfMultiplier = (float)txtExplosionDamageToSelfMultiplier.Value;
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
                        ExplosionAttributes.ExplosionAnimation.IsAnimated = true;
                        ExplosionAttributes.ExplosionAnimation.Name = Name;
                        txtProjectilePath.Text = Name;
                        break;

                    case ItemSelectionChoices.TextureProjectile:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(27);
                        ExplosionAttributes.ExplosionAnimation.IsAnimated = false;
                        ExplosionAttributes.ExplosionAnimation.Name = Name;
                        txtProjectilePath.Text = Name;
                        break;

                    case ItemSelectionChoices.Sound:
                        Name = Items[I].Substring(12);
                        ExplosionAttributes.sndExplosionPath = Name;
                        txtSoundPath.Text = Name;
                        break;
                }
            }
        }
    }
}
