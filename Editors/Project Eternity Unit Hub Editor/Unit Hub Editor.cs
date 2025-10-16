using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Units.Hub;

namespace ProjectEternity.Editors.UnitHubEditor
{
    public partial class UnitHubEditor : BaseEditor
    {
        private enum ItemSelectionChoices { VisualNovel, OriginalUnit };

        private ItemSelectionChoices ItemSelectionChoice;

        public UnitHubEditor()
        {
            InitializeComponent();
        }

        public UnitHubEditor(string FilePath, object[] Params)
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
                new EditorInfo(new string[] { EditorHelper.GUIRootPathUnitsHub, EditorHelper.GUIRootPathUnits }, "Deathmatch/Units/Hub/", new string[] { ".peu" }, typeof(UnitHubEditor))
            };
            
            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            //Create the Part file.
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);

            BW.Write(txtOriginalUnit.Text);

            BW.Write(lstVisualNovels.Items.Count);
            for (int U = 0; U < lstVisualNovels.Items.Count; ++U)
            {
                BW.Write(lstVisualNovels.Items[U].ToString());
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
            string Name = UnitPath.Substring(0, UnitPath.Length - 4).Substring(18);
            UnitHub NewUnit = new UnitHub(Name, null, Unit.DicDefaultUnitType, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect, AutomaticSkillTargetType.DicDefaultTarget);

            txtOriginalUnit.Text = NewUnit.OriginalUnitName;

            for (int U = 0; U < NewUnit.ListVisualNovel.Count; ++U)
            {
                lstVisualNovels.Items.Add(NewUnit.ListVisualNovel[U]);
            }
        }
        
        private void btnAdd_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.VisualNovel;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathVisualNovel));
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstVisualNovels.SelectedIndex < 0)
                return;

            lstVisualNovels.Items.RemoveAt(lstVisualNovels.SelectedIndex);
        }

        private void btnSelectOrginalUnit_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.OriginalUnit;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathUnits));
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
                    case ItemSelectionChoices.VisualNovel:
                        lstVisualNovels.Items.Add(Items[I].Substring(0, Items[0].Length - 5).Substring(22));
                        break;

                    case ItemSelectionChoices.OriginalUnit:
                        txtOriginalUnit.Text = Items[I].Substring(0, Items[0].Length - 4).Substring(14);
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
