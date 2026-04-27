using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.Editors.LifeSimCharacterEditor
{
    public partial class ConditionEditor : BaseEditor
    {
        private enum ItemSelectionChoices { Trait, Action, Skill, Skin,  };

        private ItemSelectionChoices ItemSelectionChoice;

        public ConditionEditor()
        {
            InitializeComponent();
        }

        public ConditionEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                SaveItem(FilePath, FilePath);
            }

            LoadWeapon(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { EditorHelper.GUIRootPathLifeSimConditions }, "Life Sim/Conditions/", new string[] { ".pec" }, typeof(ConditionEditor), true, null, false),
            };

            return Info;
        }

        public void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            this.Text = ItemName + " - Condition Editor";

            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            BW.Write(txtName.Text);
            BW.Write(txtDescription.Text);

            BW.Write((byte)lsTraits.Items.Count);
            for (int T = 0; T < lsTraits.Items.Count; ++T)
            {
                BW.Write(lsTraits.Items[T].ToString());
            }

            BW.Write(txtDamage.Text);
            BW.Write(txtBulk.Text);

            BW.Write((byte)cbHands.SelectedIndex);
            BW.Write((byte)cbWeaponType.SelectedIndex);
            BW.Write((byte)cbCategory.SelectedIndex);
            BW.Write((byte)cbGroup.SelectedIndex);

            BW.Write(txtRange.Text);
            BW.Write((byte)txtReload.Value);

            FS.Close();
            BW.Close();
        }

        private void LoadWeapon(string ConditionPath)
        {
            Name = ConditionPath.Substring(0, ConditionPath.Length - 4).Substring(28);

            Condition LoadedCondition = new Condition(Name, null, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect, AutomaticSkillTargetType.DicDefaultTarget, ManualSkillTarget.DicDefaultTarget);

            this.Text = Name + " - Condition Editor";

            txtName.Text = LoadedCondition.Name;
            txtDescription.Text = LoadedCondition.Description;
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, Path.GetFileNameWithoutExtension(FilePath));
        }

        private void btnAddTrait_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Trait;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathLifeSimTraits));
        }

        private void btnRemoveTrait_Click(object sender, EventArgs e)
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
                    case ItemSelectionChoices.Trait:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(24);
                        break;

                    case ItemSelectionChoices.Action:
                        break;

                    case ItemSelectionChoices.Skill:
                        break;
                }
            }
        }
    }
}
