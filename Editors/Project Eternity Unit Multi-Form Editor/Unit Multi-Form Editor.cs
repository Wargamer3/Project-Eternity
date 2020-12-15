using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ProjectEternity.Core.Editor;
using static ProjectEternity.Core.Units.MultiForm.UnitMultiForm;

namespace ProjectEternity.Editors.UnitTransformingEditor
{
    public partial class UnitMultiFormEditor : BaseEditor
    {
        private enum ItemSelectionChoices { Unit };

        private ItemSelectionChoices ItemSelectionChoice;
        private bool AllowEvents;

        public UnitMultiFormEditor()
        {
            InitializeComponent();
        }

        public UnitMultiFormEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                SaveItem(FilePath, FilePath);
            }

            LoadUnit(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { GUIRootPathUnitsMultiForm, GUIRootPathUnits }, "Units/Multi-Form/", new string[] {".peu" }, typeof(UnitMultiFormEditor))
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            //Create the Part file.
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);

            BW.Write(lstUnits.Items.Count);

            for (int U = 0; U < lstUnits.Items.Count; ++U)
            {
                EquipmentInformations ActiveTransformationInformations = (EquipmentInformations)lstUnits.Items[U];

                BW.Write(ActiveTransformationInformations.EquipmentUnitPath);
                BW.Write(ActiveTransformationInformations.EquipmentName);
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
            string Name = UnitPath.Substring(0, UnitPath.Length - 4).Substring(25);

            FileStream FS = new FileStream("Content/Units/Multi-Form/" + Name + ".peu", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            int ArrayUnitInformationCount = BR.ReadInt32();

            for (int U = 0; U < ArrayUnitInformationCount; ++U)
            {
                string EquipmentUnitPath = BR.ReadString();
                string EquipmentName = BR.ReadString();

                lstUnits.Items.Add(new EquipmentInformations(EquipmentUnitPath, EquipmentName));
            }

            FS.Close();
            BR.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Unit;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathUnitsNormal));
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstUnits.SelectedIndex < 0)
                return;

            lstUnits.Items.RemoveAt(lstUnits.SelectedIndex);
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, Path.GetFileName(FilePath));
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
                    case ItemSelectionChoices.Unit:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(21);
                        if (Name != null)
                        {
                            if (lstUnits.Items.Contains(Name))
                            {
                                MessageBox.Show("This Unit is already listed.\r\n" + Name);
                                return;
                            }
                            lstUnits.Items.Add(new EquipmentInformations(Name, Name));
                        }
                        break;
                }
            }
        }

        private void lstUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstUnits.SelectedItem == null)
                return;

            AllowEvents = false;

            EquipmentInformations SelectedValue = (EquipmentInformations)lstUnits.Items[lstUnits.SelectedIndex];

            txtEquipmentName.Text = SelectedValue.EquipmentName;

            AllowEvents = true;
        }

        private void txtEquipmentName_TextChanged(object sender, EventArgs e)
        {
            if (lstUnits.SelectedItem == null || !AllowEvents)
                return;

            EquipmentInformations SelectedValue = (EquipmentInformations)lstUnits.SelectedItem;

            SelectedValue.EquipmentName = txtEquipmentName.Text;
            lstUnits.Items[lstUnits.SelectedIndex] = SelectedValue;
        }
    }
}
