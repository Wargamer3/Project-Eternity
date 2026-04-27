using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.Editors.LifeSimCharacterEditor
{
    public partial class DomainEditor : BaseEditor
    {
        private enum ItemSelectionChoices { DomainSpell, AdvancedDomainSpell, LinkedDomain };

        private ItemSelectionChoices ItemSelectionChoice;

        public DomainEditor()
        {
            InitializeComponent();
        }

        public DomainEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                SaveItem(FilePath, FilePath);
            }

            LoadDomain(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { EditorHelper.GUIRootPathLifeSimCharacterDomains }, "Life Sim/Domains/", new string[] { ".ped" }, typeof(DomainEditor), true, null, false),
            };

            return Info;
        }

        public void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            this.Text = ItemName + " - Domain Editor";

            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            BW.Write(txtName.Text);
            BW.Write(txtDescription.Text);

            BW.Write(txtDomainSpell.Text);
            BW.Write(txtAdvancedSpell.Text);
            BW.Write(txtLinkedDomain.Text);

            FS.Close();
            BW.Close();
        }

        private void LoadDomain(string DomainPath)
        {
            Name = DomainPath.Substring(0, DomainPath.Length - 4).Substring(25);

            Domain LoadedDomain = new Domain(Name, null, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect, AutomaticSkillTargetType.DicDefaultTarget, ManualSkillTarget.DicDefaultTarget);

            this.Text = Name + " - Domain Editor";

            txtName.Text = LoadedDomain.Name;
            txtDescription.Text = LoadedDomain.Description;

        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, Path.GetFileNameWithoutExtension(FilePath));
        }

        private void btnSetDomainSpell_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.DomainSpell;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathLifeSimCharacterActions));
        }

        private void btnSetAdvancedSpell_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.AdvancedDomainSpell;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathLifeSimCharacterActions));
        }

        private void btnSetDomain_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.LinkedDomain;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathLifeSimCharacterDomains));
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
                    case ItemSelectionChoices.DomainSpell:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(35);
                        txtDomainSpell.Text = Name;
                        break;

                    case ItemSelectionChoices.AdvancedDomainSpell:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(35);
                        txtAdvancedSpell.Text = Name;
                        break;

                    case ItemSelectionChoices.LinkedDomain:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(25);
                        txtLinkedDomain.Text = Name;
                        break;
                }
            }
        }
    }
}
