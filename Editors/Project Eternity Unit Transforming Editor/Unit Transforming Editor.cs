using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Units.Transforming;

namespace ProjectEternity.Editors.UnitTransformingEditor
{
    public partial class UnitTransformingEditor : BaseEditor
    {
        private enum ItemSelectionChoices { Unit };

        private ItemSelectionChoices ItemSelectionChoice;
        private bool AllowEvents;

        public UnitTransformingEditor()
        {
            InitializeComponent();
        }

        public UnitTransformingEditor(string FilePath, object[] Params)
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
                new EditorInfo(new string[] { EditorHelper.GUIRootPathUnitsTransforming, EditorHelper.GUIRootPathUnits }, "Deathmatch/Units/Transforming/", new string[] { ".peu" }, typeof(UnitTransformingEditor))
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
                UnitTransforming.TransformationInformations ActiveTransformationInformations = (UnitTransforming.TransformationInformations)lstUnits.Items[U];

                BW.Write(ActiveTransformationInformations.TransformingUnitName);
                BW.Write(ActiveTransformationInformations.WillRequirement);
                BW.Write(ActiveTransformationInformations.TurnLimit);
                BW.Write(ActiveTransformationInformations.PermanentTransformation);
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
            string Name = UnitPath.Substring(0, UnitPath.Length - 4).Substring(27);
            UnitTransforming NewUnit = new UnitTransforming(Name, null, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect, AutomaticSkillTargetType.DicDefaultTarget);

            for (int U = 0; U < NewUnit.ArrayTransformingUnit.Length; ++U)
            {
                lstUnits.Items.Add(NewUnit.ArrayTransformingUnit[U]);
            }
        }

        private void cbWillRequirement_CheckedChanged(object sender, EventArgs e)
        {
            txtWillRequirement.Enabled = cbWillRequirement.Checked;
        }

        private void cbTurnLimit_CheckedChanged(object sender, EventArgs e)
        {
            txtTurnLimit.Enabled = cbTurnLimit.Checked;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Unit;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathUnitsNormal));
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstUnits.SelectedIndex < 0)
                return;

            lstUnits.Items.RemoveAt(lstUnits.SelectedIndex);
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
                        UnitTransforming.TransformationInformations NewTransformationInformations = new UnitTransforming.TransformationInformations(null, Name);
                        if (Name != null)
                        {
                            if (lstUnits.Items.Contains(NewTransformationInformations))
                            {
                                MessageBox.Show("This Unit is already listed.\r\n" + Name);
                                return;
                            }
                            lstUnits.Items.Add(NewTransformationInformations);
                        }
                        break;
                }
            }
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, Path.GetFileName(FilePath));
        }

        private void lstUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstUnits.SelectedItem == null)
                return;

            AllowEvents = false;

            UnitTransforming.TransformationInformations ActiveTransformationInformations = (UnitTransforming.TransformationInformations)lstUnits.SelectedItem;

            if (ActiveTransformationInformations.WillRequirement >= 0)
            {
                cbWillRequirement.Checked = true;
                txtWillRequirement.Value = ActiveTransformationInformations.WillRequirement;
            }
            else
                cbWillRequirement.Checked = false;

            if (ActiveTransformationInformations.TurnLimit >= 0)
            {
                cbTurnLimit.Checked = true;
                txtTurnLimit.Value = ActiveTransformationInformations.TurnLimit;
            }
            else
                cbTurnLimit.Checked = false;

            txtPermanentTransformation.Checked = ActiveTransformationInformations.PermanentTransformation;

            AllowEvents = true;
        }

        private void txtWillRequirement_ValueChanged(object sender, EventArgs e)
        {
            if (lstUnits.SelectedItem == null || !AllowEvents)
                return;

            UnitTransforming.TransformationInformations ActiveTransformationInformations = (UnitTransforming.TransformationInformations)lstUnits.SelectedItem;

            ActiveTransformationInformations.WillRequirement = (int)txtWillRequirement.Value;
            lstUnits.Items[lstUnits.SelectedIndex] = ActiveTransformationInformations;
        }

        private void txtTurnLimit_ValueChanged(object sender, EventArgs e)
        {
            if (lstUnits.SelectedItem == null || !AllowEvents)
                return;

            UnitTransforming.TransformationInformations ActiveTransformationInformations = (UnitTransforming.TransformationInformations)lstUnits.SelectedItem;

            ActiveTransformationInformations.TurnLimit = (int)txtTurnLimit.Value;
            lstUnits.Items[lstUnits.SelectedIndex] = ActiveTransformationInformations;
        }

        private void txtPermanentTransformation_CheckedChanged(object sender, EventArgs e)
        {
            if (lstUnits.SelectedItem == null || !AllowEvents)
                return;

            UnitTransforming.TransformationInformations ActiveTransformationInformations = (UnitTransforming.TransformationInformations)lstUnits.SelectedItem;

            ActiveTransformationInformations.PermanentTransformation = txtPermanentTransformation.Checked;
            lstUnits.Items[lstUnits.SelectedIndex] = ActiveTransformationInformations;
        }
    }
}
