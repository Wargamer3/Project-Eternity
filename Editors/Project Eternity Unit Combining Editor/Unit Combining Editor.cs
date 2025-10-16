using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Units.Combining;

namespace ProjectEternity.Editors.UnitCombiningEditor
{
    public partial class UnitCombiningEditor : BaseEditor
    {
        private enum ItemSelectionChoices { CombiningUnit, OriginalUnit, CombinedUnit };

        private ItemSelectionChoices ItemSelectionChoice;

        public UnitCombiningEditor()
        {
            InitializeComponent();
        }

        public UnitCombiningEditor(string FilePath, object[] Params)
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
                new EditorInfo(new string[] { EditorHelper.GUIRootPathUnitsCombining, EditorHelper.GUIRootPathUnits }, "Deathmatch/Units/Combining/", new string[] { ".peu" }, typeof(UnitCombiningEditor))
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            //Create the Part file.
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);

            BW.Write(txtOriginalUnit.Text);
            BW.Write(txtCombinedUnit.Text);

            BW.Write(lstUnits.Items.Count);

            for (int U = 0; U < lstUnits.Items.Count; ++U)
            {
                BW.Write(lstUnits.Items[U].ToString());
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
            string Name = UnitPath.Substring(0, UnitPath.Length - 4).Substring(24);
            UnitCombining NewUnit = new UnitCombining(Name, null, Unit.DicDefaultUnitType, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect, AutomaticSkillTargetType.DicDefaultTarget);

            txtOriginalUnit.Text = NewUnit.OriginalUnitName;
            txtCombinedUnit.Text = NewUnit.CombinedUnitName;

            for (int U = 0; U < NewUnit.ArrayCombiningUnitName.Length; ++U)
            {
                lstUnits.Items.Add(NewUnit.ArrayCombiningUnitName[U]);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.CombiningUnit;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathUnits));
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstUnits.SelectedIndex < 0)
                return;

            lstUnits.Items.RemoveAt(lstUnits.SelectedIndex);
        }

        private void btnSelectOrginalUnit_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.OriginalUnit;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathUnits));
        }

        private void btnCombinedUnit_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.CombinedUnit;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathUnits));
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, Path.GetFileName(FilePath));
        }

        private void lstUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstUnits.SelectedItem == null)
                return;
        }

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            string Name;
            for (int I = 0; I < Items.Count; I++)
            {
                Name = Items[I].Substring(0, Items[0].Length - 4).Substring(14);
                switch (ItemSelectionChoice)
                {
                    case ItemSelectionChoices.CombiningUnit:
                        lstUnits.Items.Add(Name);
                        break;

                    case ItemSelectionChoices.OriginalUnit:
                        txtOriginalUnit.Text = Name;
                        break;

                    case ItemSelectionChoices.CombinedUnit:
                        txtCombinedUnit.Text = Name;
                        break;
                }
            }
        }
    }
}
