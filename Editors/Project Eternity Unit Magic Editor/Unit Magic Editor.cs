using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Editor;
using ProjectEternity.Units.Magic;

namespace ProjectEternity.Editors.UnitHubEditor
{
    public partial class UnitMagicEditor : BaseEditor
    {
        private enum ItemSelectionChoices { OriginalUnit, Spell };

        private ItemSelectionChoices ItemSelectionChoice;

        public UnitMagicEditor()
        {
            InitializeComponent();
        }

        public UnitMagicEditor(string FilePath, object[] Params)
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
                new EditorInfo(new string[] { GUIRootPathUnitsMagic, GUIRootPathUnits }, "Units/Magic/", new string[] { ".peu" }, typeof(UnitMagicEditor))
            };
            
            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            //Create the Part file.
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);

            BW.Write(txtOriginalUnit.Text);

            BW.Write(lstSpells.Items.Count);
            for (int S = 0; S < lstSpells.Items.Count; ++S)
            {
                BW.Write(lstSpells.Items[S].ToString());
            }

            FS.Close();
            BW.Close();
        }

        private void LoadUnit(string UnitPath)
        {
            string Name = UnitPath.Substring(0, UnitPath.Length - 4).Substring(20);
            UnitMagic NewUnit = new UnitMagic(Name, null, Unit.DicDefaultUnitType, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect, AutomaticSkillTargetType.DicDefaultTarget);

            txtOriginalUnit.Text = NewUnit.OriginalUnitName;

            for (int S = 0; S < NewUnit.ListMagicSpell.Count; ++S)
            {
                lstSpells.Items.Add(NewUnit.ListMagicSpell[S].Name);
            }
        }
        
        private void btnSelectOrginalUnit_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.OriginalUnit;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathUnits));
        }

        private void btnAddSpell_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Spell;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathSpells));
        }

        private void btnRemoveSpell_Click(object sender, EventArgs e)
        {
            if (lstSpells.SelectedIndex >= 0)
            {
                lstSpells.Items.RemoveAt(lstSpells.SelectedIndex);
            }
        }

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            for (int I = 0; I < Items.Count; I++)
            {
                switch (ItemSelectionChoice)
                {
                    case ItemSelectionChoices.OriginalUnit:
                        txtOriginalUnit.Text = Items[I].Substring(0, Items[0].Length - 4).Substring(14);
                        break;

                    case ItemSelectionChoices.Spell:
                        lstSpells.Items.Add(Items[I].Substring(0, Items[0].Length - 4).Substring(15));
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
