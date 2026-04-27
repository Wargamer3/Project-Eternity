using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.Editors.LifeSimCharacterEditor
{
    public partial class ItemEditor : BaseEditor
    {
        private enum ItemSelectionChoices { Spell, Skill, Skin,  };

        private ItemSelectionChoices ItemSelectionChoice;

        public ItemEditor()
        {
            InitializeComponent();
        }

        public ItemEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                SaveItem(FilePath, FilePath);
            }

            LoadCharacter(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { EditorHelper.GUIRootPathLifeSimItems }, "Life Sim/Items/", new string[] { ".pei" }, typeof(ItemEditor), true, null, false),
            };

            return Info;
        }

        public void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            this.Text = ItemName + " - Item Editor";

            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            BW.Write(txtName.Text);
            BW.Write(txtDescription.Text);


            FS.Close();
            BW.Close();
        }

        private void LoadCharacter(string ItemPath)
        {
            Name = ItemPath.Substring(0, ItemPath.Length - 4).Substring(23);

            Item LoadedItem = new Item(Name, null, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect, AutomaticSkillTargetType.DicDefaultTarget, ManualSkillTarget.DicDefaultTarget);

            this.Text = Name + " - Item Editor";

            txtName.Text = LoadedItem.Name;
            txtDescription.Text = LoadedItem.Description;

        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, Path.GetFileNameWithoutExtension(FilePath));
        }

        private void tsmDetails_Click(object sender, EventArgs e)
        {
        }

        private void tsmQuotes_Click(object sender, EventArgs e)
        {
        }

        private void tsmRelationships_Click(object sender, EventArgs e)
        {
        }

        #region Skills

        private void lsActions_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnAddAction_Click(object sender, EventArgs e)
        {

        }

        private void btnSetAction_Click(object sender, EventArgs e)
        {

        }

        private void btnDeleteAction_Click(object sender, EventArgs e)
        {

        }

        private void txtActionCost_ValueChanged(object sender, EventArgs e)
        {

        }

        private void lsPassiveSkills_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnAddSkill_Click(object sender, EventArgs e)
        {

        }

        private void btnSetSkill_Click(object sender, EventArgs e)
        {

        }

        private void btnDeleteSkill_Click(object sender, EventArgs e)
        {

        }

        #endregion

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            string Name;
            for (int I = 0; I < Items.Count; I++)
            {
                switch (ItemSelectionChoice)
                {
                    case ItemSelectionChoices.Spell:
                        break;

                    case ItemSelectionChoices.Skill:
                        break;
                }
            }
        }
    }
}
