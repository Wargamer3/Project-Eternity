using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Units.Builder;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;

namespace ProjectEternity.Editors.UnitHubEditor
{
    public partial class UnitBuilderEditor : BaseEditor
    {
        private enum ItemSelectionChoices { Unit, OriginalUnit };

        private ItemSelectionChoices ItemSelectionChoice;
        
        private Dictionary<string, Unit> DicUnitType;
        private Dictionary<string, BaseSkillRequirement> DicRequirement;
        private Dictionary<string, BaseEffect> DicEffect;

        public UnitBuilderEditor()
        {
            InitializeComponent();

            DicUnitType = Unit.LoadAllUnits();
            DicRequirement = BaseSkillRequirement.LoadAllRequirements();
            DicEffect = BaseEffect.LoadAllEffects();
        }

        public UnitBuilderEditor(string FilePath, object[] Params)
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
                new EditorInfo(new string[] { GUIRootPathUnitsBuilder, GUIRootPathUnits }, "Units/Builder/", new string[] { ".peu" }, typeof(UnitBuilderEditor))
            };
            
            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            //Create the Part file.
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);

            BW.Write(txtOriginalUnit.Text);

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
        /// <param name="UnitPath">Path from which to open the Unit.</param>
        private void LoadUnit(string UnitPath)
        {
            string Name = UnitPath.Substring(0, UnitPath.Length - 4).Substring(21);
            UnitBuilder NewUnit = new UnitBuilder(Name, null, DicUnitType, DicRequirement, DicEffect);

            txtOriginalUnit.Text = NewUnit.OriginalUnitName;

            for (int U = 0; U < NewUnit.ListUnitToBuild.Count; ++U)
            {
                lstUnits.Items.Add(NewUnit.ListUnitToBuild[U]);
            }
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

        private void btnSelectOrginalUnit_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.OriginalUnit;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathUnits));
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
                    case ItemSelectionChoices.Unit:
                        lstUnits.Items.Add(Name);
                        break;

                    case ItemSelectionChoices.OriginalUnit:
                        txtOriginalUnit.Text = Name;
                        break;
                }
            }
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, Path.GetFileName(FilePath));
        }
    }
}
