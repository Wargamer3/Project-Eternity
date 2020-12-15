using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;
using System.Windows.Forms;

namespace ProjectEternity.Editors.UnitTripleThunderEditor
{
    public partial class UnitVehiculeTripleThunderEditor : BaseEditor
    {
        private enum ItemSelectionChoices { DefaultStance, ExtraStance, AddWeapon, };

        private ItemSelectionChoices ItemSelectionChoice;

        public UnitVehiculeTripleThunderEditor()
        {
            InitializeComponent();
            cbControlType.SelectedIndex = 0;
            cbCaptureType.SelectedIndex = 0;
        }

        public UnitVehiculeTripleThunderEditor(string FilePath, object[] Params)
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
                new EditorInfo(new string[] { GUIRootPathUnitsVehicleTripleThunder, GUIRootPathUnits }, "Units/Triple Thunder/Vehicles/", new string[] { ".peuv" }, typeof(UnitVehiculeTripleThunderEditor))
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
            BW.Write((byte)cbControlType.SelectedIndex);
            BW.Write((byte)cbCaptureType.SelectedIndex);
            BW.Write(cbHasKnockback.Checked);
            BW.Write(cbIsDynamic.Checked);

            BW.Write(lbExtraStances.Items.Count + 1);
            BW.Write(txtDefaultWeapon.Text);
            for(int i = 0; i < lbExtraStances.Items.Count; ++i)
            {
                BW.Write(lbExtraStances.Items[i].ToString());
            }

            BW.Write(lstWeapons.Items.Count);
            for (int W = 0; W < lstWeapons.Items.Count; ++W)
            {
                BW.Write((string)lstWeapons.Items[W]);
            }

            FS.Close();
            BW.Close();
        }

        /// <summary>
        /// Load a Unit item at selected path.
        /// </summary>
        /// <param name="PartPath">Path from which to open the Unit.</param>
        private void LoadUnit(string UnitPath)
        {
            string Name = UnitPath.Substring(0, UnitPath.Length - 5).Substring(38);
            this.Text = Name + " - Project Eternity Triple Thunder Vehicle Unit Editor";

            FileStream FS = new FileStream("Content/Units/Triple Thunder/Vehicles/" + Name + ".peuv", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            txtMaxHP.Value = BR.ReadInt32();
            txtMaxEN.Value = BR.ReadInt32();
            txtAcceleration.Value = (decimal)BR.ReadSingle();
            txtTopSpeed.Value = (decimal)BR.ReadSingle();
            txtJumpSpeed.Value = (decimal)BR.ReadSingle();
            cbControlType.SelectedIndex = BR.ReadByte();
            cbCaptureType.SelectedIndex = BR.ReadByte();
            cbHasKnockback.Checked = BR.ReadBoolean();
            cbIsDynamic.Checked = BR.ReadBoolean();

            int ExtraAnimationsCount = BR.ReadInt32();
            txtDefaultWeapon.Text = BR.ReadString();
            for (int i = 1; i < ExtraAnimationsCount; i++)
            {
                lbExtraStances.Items.Add(BR.ReadString());
            }

            int lstWeaponsCount = BR.ReadInt32();
            for (int W = 0; W < lstWeaponsCount; ++W)
            {
                lstWeapons.Items.Add(BR.ReadString());
            }

            FS.Close();
            BR.Close();
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, null);
        }

        private void btnSelectWeapon_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.DefaultStance;
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
                    case ItemSelectionChoices.DefaultStance:
                        txtDefaultWeapon.Text = Name;
                        break;

                    case ItemSelectionChoices.ExtraStance:
                        lbExtraStances.Items.Add(Name);
                        break;

                    case ItemSelectionChoices.AddWeapon:
                        lstWeapons.Items.Add(Name);
                        break;
                }
            }
        }
    }
}
