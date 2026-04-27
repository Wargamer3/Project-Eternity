using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.LifeSimScreen;
using System.Windows.Forms;

namespace ProjectEternity.Editors.LifeSimCharacterEditor
{
    public partial class ActionEditor : BaseEditor
    {
        private enum ItemSelectionChoices { Trait, AIAction,  };

        private ItemSelectionChoices ItemSelectionChoice;

        public ActionEditor()
        {
            InitializeComponent();
        }

        public ActionEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                SaveItem(FilePath, FilePath);
            }

            LoadAction(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { EditorHelper.GUIRootPathLifeSimCharacterActions }, "Life Sim/Character Actions/", new string[] { ".pea" }, typeof(ActionEditor), true, null, false),
            };

            return Info;
        }

        public void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            this.Text = ItemName + " - Character Action Editor";

            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            BW.Write(txtName.Text);
            BW.Write(txtDescription.Text);

            BW.Write((byte)lsTraits.Items.Count);
            foreach (string ActiveTrait in lsTraits.Items)
            {
                BW.Write(ActiveTrait);
            }

            BW.Write((byte)txtActionCost.Value);

            BW.Write((byte)tabControlActions.TabPages.Count);
            foreach (TabPage ActiveTab in tabControlActions.TabPages)
            {
                ((ActionEffect)ActiveTab.Tag).Write(BW);
            }

            BW.Write((byte)lsAIActions.Items.Count);
            foreach (string ActiveAIAction in lsAIActions.Items)
            {
                BW.Write(ActiveAIAction);
            }

            FS.Close();
            BW.Close();
        }

        private void LoadAction(string ActionPath)
        {
            LifeSimParams.Init();

            Name = ActionPath.Substring(0, ActionPath.Length - 4).Substring(35);

            CharacterAction LoadedAction = new CharacterAction(Name, null);

            this.Text = Name + " - Character Action Editor";

            txtName.Text = LoadedAction.Name;
            txtDescription.Text = LoadedAction.Description;

            foreach (string ActiveTrait in LoadedAction.ListTraitsRelativePath)
            {
                lsTraits.Items.Add(ActiveTrait);
            }

            txtActionCost.Value = LoadedAction.ActionCost;
            foreach (ActionEffect ActiveActionEffect in LoadedAction.ListActionEffect)
            {
                AddExtraAction(ActiveActionEffect);
            }
            
            foreach (string ActiveAIAction in LoadedAction.ListAIActionPath)
            {
                lsAIActions.Items.Add(ActiveAIAction);
            }
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, Path.GetFileNameWithoutExtension(FilePath));
        }

        private static void DeleteItemFromList(ListBox ActiveListBox)
        {
            if (ActiveListBox.SelectedIndex >= 0)
            {
                int LastIndex = ActiveListBox.SelectedIndex;

                ActiveListBox.Items.RemoveAt(LastIndex);

                if (ActiveListBox.Items.Count > 0)
                {
                    if (ActiveListBox.Items.Count >= LastIndex)
                    {
                        ActiveListBox.SelectedIndex = ActiveListBox.Items.Count - 1;
                    }
                    else
                    {
                        ActiveListBox.SelectedIndex = LastIndex;
                    }
                }
            }
        }

        private void AddExtraAction(ActionEffect SelectedActionEffect)
        {
            PropertyGrid pgAction = new PropertyGrid();
            Button btnRemoveAction = new Button();
            ComboBox cbActionType = new ComboBox();
            GroupBox gbActionDetail = new GroupBox();
            TabPage tabSkills = new TabPage();
            gbActionDetail.SuspendLayout();
            tabSkills.SuspendLayout();

            // 
            // gbActionDetail
            // 
            gbActionDetail.Controls.Add(btnRemoveAction);
            gbActionDetail.Controls.Add(cbActionType);
            gbActionDetail.Location = new System.Drawing.Point(6, 6);
            gbActionDetail.Name = "gbActionDetail" + tabSkills.Controls.Count;
            gbActionDetail.Size = new System.Drawing.Size(256, 103);
            gbActionDetail.TabIndex = 68;
            gbActionDetail.TabStop = false;
            gbActionDetail.Text = "Action Details";
            tabControlActions.TabPages.Add(tabSkills);
            tabSkills.Controls.Add(pgAction);
            tabSkills.Controls.Add(gbActionDetail);

            // 
            // pgAction
            // 
            pgAction.Location = new System.Drawing.Point(6, 115);
            pgAction.Name = "pgAction" + tabSkills.Controls.Count;
            pgAction.Size = new System.Drawing.Size(255, 236);
            pgAction.TabIndex = 80;
            // 
            // cbActionType
            // 
            cbActionType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbActionType.FormattingEnabled = true;
            cbActionType.Location = new System.Drawing.Point(6, 19);
            cbActionType.Name = "cbActionType" + tabSkills.Controls.Count;
            cbActionType.Size = new System.Drawing.Size(244, 21);
            cbActionType.TabIndex = 0;
            cbActionType.SelectedIndexChanged += (Sender, EventArg) =>
            {
                ActionEffect SelectedEffect = (ActionEffect)cbActionType.SelectedItem;

                pgAction.SelectedObject = SelectedEffect;
                tabSkills.Tag = SelectedEffect;
            };
            foreach (ActionEffect ActiveEffect in LifeSimParams.DicActionEffect.Values)
            {
                cbActionType.Items.Add(ActiveEffect);
            }
            // 
            // btnRemoveAction
            // 
            btnRemoveAction.Location = new System.Drawing.Point(131, 46);
            btnRemoveAction.Name = "btnRemoveAction" + tabSkills.Controls.Count;
            btnRemoveAction.Size = new System.Drawing.Size(119, 23);
            btnRemoveAction.TabIndex = 2;
            btnRemoveAction.Text = "Remove Action";
            btnRemoveAction.UseVisualStyleBackColor = true;
            // 
            // tabSkills
            // 
            tabSkills.Location = new System.Drawing.Point(4, 22);
            tabSkills.Name = "tabSkills";
            tabSkills.Padding = new System.Windows.Forms.Padding(3);
            tabSkills.Size = new System.Drawing.Size(267, 357);
            tabSkills.TabIndex = 0;
            tabSkills.Text = "First Action";
            tabSkills.UseVisualStyleBackColor = true;


            gbActionDetail.ResumeLayout(false);
            tabSkills.ResumeLayout(false);

            if (SelectedActionEffect != null)
            {
                cbActionType.Text = SelectedActionEffect.ToString();
            }
            else
            {
                cbActionType.SelectedIndex = 0;
            }
        }

        #region Buttons

        private void btnAddTrait_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Trait;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathLifeSimTraits));
        }

        private void btnRemoveTrait_Click(object sender, EventArgs e)
        {
            DeleteItemFromList(lsTraits);
        }

        private void btnAddExtraAction_Click(object sender, EventArgs e)
        {
            AddExtraAction(null);
        }

        private void btnAddAIAction_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.AIAction;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathLifeSimAIActions));
        }

        private void btnRemoveAIAction_Click(object sender, EventArgs e)
        {
            DeleteItemFromList(lsAIActions);
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
                        lsTraits.Items.Add(Name);
                        break;

                    case ItemSelectionChoices.AIAction:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(28);
                        lsAIActions.Items.Add(Name);
                        break;
                }
            }
        }
    }
}
