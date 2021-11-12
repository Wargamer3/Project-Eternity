using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.TripleThunderScreen;

namespace ProjectEternity.Editors.TripleThunderWeaponEditor
{
    public partial class WeaponEditor : BaseEditor
    {
        private enum ItemSelectionChoices { ReloadCombo, SkillChain };

        private ItemSelectionChoices ItemSelectionChoice;

        private ProjectileEditor ProjectileDetailEditor;
        private ExplosionOptions ExplosionAttributes;

        public WeaponEditor()
        {
            InitializeComponent();
            ProjectileDetailEditor = new ProjectileEditor();
            ExplosionAttributes = new ExplosionOptions();
            ExplosionAttributes.ExplosionAnimation = new GameScreens.AnimationScreen.SimpleAnimation();
        }

        public WeaponEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;

            if (!File.Exists(FilePath))
            {
                cbProjectileType.SelectedIndex = 0;
                SaveItem(FilePath, "New Item");
            }

            LoadWeapon(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { GUIRootPathTripleThunderWeapons }, "Triple Thunder/Weapons/", new string[] { ".ttw" }, typeof(WeaponEditor)),
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            //Create the Part file.
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);

            BW.Write((float)txtDamage.Value);
            BW.Write((float)txtMaxDurability.Value);
            BW.Write((float)txtMinAngle.Value);
            BW.Write((float)txtMaxAngle.Value);
            BW.Write(ckUseRangedProperties.Checked);
            BW.Write(txtSkillChain.Text);

            ExplosionAttributes.Save(BW);

            if (ckUseRangedProperties.Checked)
            {
                BW.Write((float)txtAmmoPerMagazine.Value);
                BW.Write((float)txtAmmoRegen.Value);
                BW.Write((float)txtRecoil.Value);
                BW.Write((float)txtMaxRecoil.Value);
                BW.Write((float)txtRecoilRecoverySpeed.Value);
                BW.Write((int)txtNumberOfProjectiles.Value);
                BW.Write(cbProjectileType.SelectedIndex);
                BW.Write(txtReloadAnimation.Text);

                if (cbProjectileType.SelectedIndex > 0)
                {
                    BW.Write((float)ProjectileDetailEditor.txtProjectileSpeed.Value);
                    BW.Write(ProjectileDetailEditor.ckAffectedByGravity.Checked);
                    BW.Write(ProjectileDetailEditor.ckAllowRotation.Checked);

                    BW.Write(ProjectileDetailEditor.IsProjectileAnimated);
                    BW.Write(ProjectileDetailEditor.txtProjectilePath.Text);

                    BW.Write((byte)ProjectileDetailEditor.cboTrailStyle.SelectedIndex);

                    if (ProjectileDetailEditor.cboTrailStyle.SelectedIndex == 2)
                    {
                        BW.Write((byte)ProjectileDetailEditor.cboTrailEffect.SelectedIndex);
                        BW.Write(ProjectileDetailEditor.IsTrailAnimated);
                        BW.Write(ProjectileDetailEditor.txtTrailPath.Text);
                    }

                    BW.Write(1f); // Scale X
                    BW.Write(1f); // Scale Y
                }
            }

            FS.Close();
            BW.Close();
        }

        /// <summary>
        /// Load a Weapon at selected path.
        /// </summary>
        /// <param name="PartPath">Path from which to open the Combo.</param>
        private void LoadWeapon(string ComboPath)
        {
            string Name = ComboPath.Substring(0, ComboPath.Length - 4).Substring(31);
            this.Text = Name + " - Project Eternity Triple Thunder Weapon Editor";

            FileStream FS = new FileStream("Content/Triple Thunder/Weapons/" + Name + ".ttw", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            txtDamage.Value = (decimal)BR.ReadSingle();
            txtMaxDurability.Value = (decimal)BR.ReadSingle();
            txtMinAngle.Value = (decimal)BR.ReadSingle();
            txtMaxAngle.Value = (decimal)BR.ReadSingle();
            ckUseRangedProperties.Checked = BR.ReadBoolean();
            txtSkillChain.Text = BR.ReadString();

            ExplosionAttributes = new ExplosionOptions(BR);

            if (ckUseRangedProperties.Checked)
            {
                txtAmmoPerMagazine.Value = (decimal)BR.ReadSingle();
                txtAmmoRegen.Value = (decimal)BR.ReadSingle();
                txtRecoil.Value = (decimal)BR.ReadSingle();
                txtMaxRecoil.Value = (decimal)BR.ReadSingle();
                txtRecoilRecoverySpeed.Value = (decimal)BR.ReadSingle();
                txtNumberOfProjectiles.Value = BR.ReadInt32();
                cbProjectileType.SelectedIndex = BR.ReadInt32();
                txtReloadAnimation.Text = BR.ReadString();

                if (cbProjectileType.SelectedIndex > 0)
                {
                    ProjectileDetailEditor.txtProjectileSpeed.Value = (decimal)BR.ReadSingle();
                    ProjectileDetailEditor.ckAffectedByGravity.Checked = BR.ReadBoolean();
                    ProjectileDetailEditor.ckAllowRotation.Checked = BR.ReadBoolean();
                    ProjectileDetailEditor.IsProjectileAnimated = BR.ReadBoolean();
                    ProjectileDetailEditor.txtProjectilePath.Text = BR.ReadString();

                    ProjectileDetailEditor.cboTrailStyle.SelectedIndex = BR.ReadByte();

                    if (ProjectileDetailEditor.cboTrailStyle.SelectedIndex == 2)
                    {
                        ProjectileDetailEditor.cboTrailEffect.SelectedIndex = BR.ReadByte();
                        ProjectileDetailEditor.IsTrailAnimated = BR.ReadBoolean();
                        ProjectileDetailEditor.txtTrailPath.Text = BR.ReadString();
                    }

                    txtProjectile.Text = ProjectileDetailEditor.txtProjectilePath.Text;
                }
            }

            BR.Close();
            FS.Close();
        }

        private void btnReloadAnimation_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.ReloadCombo;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathTripleThunderCombos));
        }

        private void btnSelectSkillChain_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.SkillChain;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathTripleThunderSkillChains));
        }

        private void ckUseRangedProperties_CheckedChanged(object sender, EventArgs e)
        {
            gbRangedProperties.Enabled = ckUseRangedProperties.Checked;
        }

        private void cbProjectileType_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool EnableProjectileChanges = cbProjectileType.SelectedIndex > 0;

            btnSelectProjectile.Enabled = EnableProjectileChanges;
        }

        private void btnSelectProjectile_Click(object sender, EventArgs e)
        {
            bool IsProjectileAnimated = ProjectileDetailEditor.IsProjectileAnimated;
            string ProjectilePath = ProjectileDetailEditor.txtProjectilePath.Text;

            bool IsTrailAnimated = ProjectileDetailEditor.IsTrailAnimated;
            string TrailPath = ProjectileDetailEditor.txtTrailPath.Text;

            if (ProjectileDetailEditor.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                //User cancelled the changes.
                ProjectileDetailEditor.IsProjectileAnimated = IsProjectileAnimated;
                ProjectileDetailEditor.txtProjectilePath.Text = ProjectilePath;

                ProjectileDetailEditor.IsTrailAnimated = IsTrailAnimated;
                ProjectileDetailEditor.txtTrailPath.Text = TrailPath;
            }
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, null);
        }

        private void tsmExplosionAttributes_Click(object sender, EventArgs e)
        {
            ExplosionAttributesEditor NewEditor = new ExplosionAttributesEditor(ExplosionAttributes);

            if (NewEditor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ExplosionAttributes = NewEditor.ExplosionAttributes;
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
                    case ItemSelectionChoices.ReloadCombo:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(30);

                        txtReloadAnimation.Text = Name;
                        break;

                    case ItemSelectionChoices.SkillChain:
                        if (Items[I] == null)
                        {
                            txtSkillChain.Text = "";
                        }
                        else
                        {
                            Name = Items[I].Substring(0, Items[0].Length - 5).Substring(36);

                            txtSkillChain.Text = Name;
                        }
                        break;
                }
            }
        }
    }
}
