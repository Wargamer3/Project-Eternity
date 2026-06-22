using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.Editors.LifeSimCharacterEditor
{
    public partial class AIActionEditor : BaseEditor
    {
        private bool AllowEvents;

        public AIActionEditor()
        {
            InitializeComponent();

            AllowEvents = true;
        }

        public AIActionEditor(string FilePath, object[] Params)
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
                new EditorInfo(new string[] { EditorHelper.GUIRootPathLifeSimAIActions }, "Life Sim/AI Actions/", new string[] { ".pea" }, typeof(AIActionEditor), true, null, false),
            };

            return Info;
        }

        public void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            this.Text = ItemName + " - Character AI Action Editor";

            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            BW.Write(txtName.Text);
            BW.Write(txtDescription.Text);

            BW.Write((byte)0);

            FS.Close();
            BW.Close();
        }

        private void LoadAction(string AIActionPath)
        {
            LifeSimCharacterParams.Init();
            foreach (AIAction ActiveAIAction in LifeSimCharacterParams.DicAIAction.Values)
            {
                cbAIActionType.Items.Add(ActiveAIAction);
            }

            Name = AIActionPath.Substring(0, AIActionPath.Length - 4).Substring(28);

            AICharacterAction LoadedAIAction = new AICharacterAction(Name);

            this.Text = Name + " - Character AI Action Editor";

            txtName.Text = LoadedAIAction.Name;
            txtDescription.Text = LoadedAIAction.Description;


            foreach (AIAction ActiveAIAction in LoadedAIAction.ListAIAction)
            {
                lsAIActions.Items.Add(ActiveAIAction);
            }

            if (lsAIActions.Items.Count > 0)
            {
                lsAIActions.SelectedIndex = 0;
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

        #region Lists

        private void btnAddAIAction_Click(object sender, EventArgs e)
        {
            AIAction NewAIAction = (AIAction)cbAIActionType.Items[0];
            lsAIActions.Items.Add(NewAIAction.Copy());
            lsAIActions.SelectedIndex = lsAIActions.Items.Count - 1;

            cbAIActionType.Enabled = true;
        }

        private void btnDeleteAIAction_Click(object sender, EventArgs e)
        {
            DeleteItemFromList(lsAIActions);

            cbAIActionType.Enabled = lsAIActions.Items.Count > 0;
            cbAIActionType.Text = string.Empty;
            cbAIActionType.SelectedIndex = -1;
        }

        private void lsAIActions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!AllowEvents)
                return;

            if (lsAIActions.SelectedItem != null)
            {
                AllowEvents = false;

                AIAction SelectedUnlockable = (AIAction)lsAIActions.SelectedItem;

                cbAIActionType.Text = SelectedUnlockable.ToString();
                pgAIAction.SelectedObject = SelectedUnlockable;

                AllowEvents = true;
            }
            else
            {
                cbAIActionType.SelectedItem = null;
                pgAIAction.SelectedObject = null;
            }
        }

        private void cbAIActionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!AllowEvents)
                return;

            if (lsAIActions.SelectedItem != null)
            {
                AllowEvents = false;

                AIAction SelectedUnlockable = ((AIAction)cbAIActionType.SelectedItem).Copy();

                lsAIActions.Items[lsAIActions.SelectedIndex] = SelectedUnlockable;
                pgAIAction.SelectedObject = SelectedUnlockable;

                AllowEvents = true;
            }
        }

        #endregion
    }
}
