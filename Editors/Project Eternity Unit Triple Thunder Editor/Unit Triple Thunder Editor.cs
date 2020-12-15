using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.Editors.UnitTripleThunderEditor
{
    public partial class UnitTripleThunderEditor : BaseEditor
    {
        private enum ItemSelectionChoices { DefaultWeapon, CrouchWeapon, RollWeapon, ProneWeapon, AddWeapon, };

        private ItemSelectionChoices ItemSelectionChoice;
        private UnitSounds UnitSoundsDialog;

        public UnitTripleThunderEditor()
        {
            InitializeComponent();
            UnitSoundsDialog = new UnitSounds();
        }

        public UnitTripleThunderEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                SaveItem(FilePath, "New Item");
            }

            LoadUnit(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { GUIRootPathUnitsTripleThunder, GUIRootPathUnits }, "Units/Triple Thunder/", new string[] { ".peu" }, typeof(UnitTripleThunderEditor))
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            //Create the Part file.
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);

            BW.Write((int)txtMaxHP.Value);
            BW.Write((int)txtMaxEN.Value);
            BW.Write((float)txtAcceleration.Value);
            BW.Write((float)txtTopSpeed.Value);
            BW.Write((float)txtJumpSpeed.Value);
            BW.Write(cbHasKnockback.Checked);
            BW.Write(cbIsDynamic.Checked);


            BW.Write(4);
            BW.Write(txtDefaultWeapon.Text);
            BW.Write(txtCrouchWeapon.Text);
            BW.Write(txtRollWeapon.Text);
            BW.Write(txtProneWeapon.Text);

            BW.Write(lstWeapons.Items.Count);
            for (int W = 0; W < lstWeapons.Items.Count; ++W)
            {
                BW.Write((string)lstWeapons.Items[W]);
            }

            BW.Write(UnitSoundsDialog.cbCrouchStart.Text);
            BW.Write(UnitSoundsDialog.cbCrouchMove.Text);
            BW.Write(UnitSoundsDialog.cbCrouchEnd.Text);

            BW.Write(UnitSoundsDialog.cbJetpackStart.Text);
            BW.Write(UnitSoundsDialog.cbJetpackUse.Text);
            BW.Write(UnitSoundsDialog.cbJetpackEnd.Text);

            BW.Write(UnitSoundsDialog.cbProneStart.Text);
            BW.Write(UnitSoundsDialog.cbProneMove.Text);
            BW.Write(UnitSoundsDialog.cbProneEnd.Text);

            BW.Write(UnitSoundsDialog.cbJumpStart.Text);
            BW.Write(UnitSoundsDialog.cbJumpEnd.Text);
            BW.Write(UnitSoundsDialog.cbStrainHop.Text);

            BW.Write(UnitSoundsDialog.cbStepNormal.Text);
            BW.Write(UnitSoundsDialog.cbStepGrass.Text);
            BW.Write(UnitSoundsDialog.cbStepWater.Text);

            BW.Write(UnitSoundsDialog.cbDeath.Text);
            BW.Write(UnitSoundsDialog.cbRoll.Text);
            BW.Write(UnitSoundsDialog.cbDash.Text);

            FS.Close();
            BW.Close();
        }

        /// <summary>
        /// Load a Unit item at selected path.
        /// </summary>
        /// <param name="PartPath">Path from which to open the Unit.</param>
        private void LoadUnit(string UnitPath)
        {
            string Name = UnitPath.Substring(0, UnitPath.Length - 4).Substring(29);
            this.Text = Name + " - Project Eternity Triple Thunder Unit Editor";

            FileStream FS = new FileStream("Content/Units/Triple Thunder/" + Name + ".peu", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            txtMaxHP.Value = BR.ReadInt32();
            txtMaxEN.Value = BR.ReadInt32();
            txtAcceleration.Value = (decimal)BR.ReadSingle();
            txtTopSpeed.Value = (decimal)BR.ReadSingle();
            txtJumpSpeed.Value = (decimal)BR.ReadSingle();
            cbHasKnockback.Checked = BR.ReadBoolean();
            cbIsDynamic.Checked = BR.ReadBoolean();

            int ExtraAnimationsCount = BR.ReadInt32();
            txtDefaultWeapon.Text = BR.ReadString();
            txtCrouchWeapon.Text = BR.ReadString();
            txtRollWeapon.Text = BR.ReadString();
            txtProneWeapon.Text = BR.ReadString();

            int lstWeaponsCount = BR.ReadInt32();
            for (int W = 0; W < lstWeaponsCount; ++W)
            {
                lstWeapons.Items.Add(BR.ReadString());
            }

            UnitSoundsDialog.cbCrouchStart.SelectedItem = BR.ReadString();
            UnitSoundsDialog.cbCrouchMove.SelectedItem = BR.ReadString();
            UnitSoundsDialog.cbCrouchEnd.SelectedItem = BR.ReadString();

            UnitSoundsDialog.cbJetpackStart.SelectedItem = BR.ReadString();
            UnitSoundsDialog.cbJetpackUse.SelectedItem = BR.ReadString();
            UnitSoundsDialog.cbJetpackEnd.SelectedItem = BR.ReadString();

            UnitSoundsDialog.cbProneStart.SelectedItem = BR.ReadString();
            UnitSoundsDialog.cbProneMove.SelectedItem = BR.ReadString();
            UnitSoundsDialog.cbProneEnd.SelectedItem = BR.ReadString();

            UnitSoundsDialog.cbJumpStart.SelectedItem = BR.ReadString();
            UnitSoundsDialog.cbJumpEnd.SelectedItem = BR.ReadString();
            UnitSoundsDialog.cbStrainHop.SelectedItem = BR.ReadString();

            UnitSoundsDialog.cbStepNormal.SelectedItem = BR.ReadString();
            UnitSoundsDialog.cbStepGrass.SelectedItem = BR.ReadString();
            UnitSoundsDialog.cbStepWater.SelectedItem = BR.ReadString();

            UnitSoundsDialog.cbDeath.SelectedItem = BR.ReadString();
            UnitSoundsDialog.cbRoll.SelectedItem = BR.ReadString();
            UnitSoundsDialog.cbDash.SelectedItem = BR.ReadString();

            FS.Close();
            BR.Close();
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, null);
        }

        private void tsmSounds_Click(object sender, EventArgs e)
        {
            UnitSoundsDialog.ShowDialog();
        }

        private void btnSelectWeapon_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.DefaultWeapon;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathTripleThunderWeapons));
        }

        private void btnSelectCrouchWeapon_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.CrouchWeapon;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathTripleThunderWeapons));
        }

        private void btnSelectRollWeapon_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.RollWeapon;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathTripleThunderWeapons));
        }

        private void btnSelectProneWeapon_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.ProneWeapon;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathTripleThunderWeapons));
        }

        private void btnAddWeapon_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.AddWeapon;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathTripleThunderWeapons));
        }

        private void btnRemoveWeapon_Click(object sender, EventArgs e)
        {
            if (lstWeapons.SelectedIndex >= 0)
            {
                lstWeapons.Items.RemoveAt(lstWeapons.SelectedIndex);
            }
        }

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            string Name;
            for (int I = 0; I < Items.Count; I++)
            {
                Name = Items[I].Substring(0, Items[0].Length - 4).Substring(31);
                switch (ItemSelectionChoice)
                {
                    case ItemSelectionChoices.DefaultWeapon:
                        txtDefaultWeapon.Text = Name;
                        break;

                    case ItemSelectionChoices.CrouchWeapon:
                        txtCrouchWeapon.Text = Name;
                        break;

                    case ItemSelectionChoices.RollWeapon:
                        txtRollWeapon.Text = Name;
                        break;

                    case ItemSelectionChoices.ProneWeapon:
                        txtProneWeapon.Text = Name;
                        break;

                    case ItemSelectionChoices.AddWeapon:
                        lstWeapons.Items.Add(Name);
                        break;
                }
            }
        }
    }
}
