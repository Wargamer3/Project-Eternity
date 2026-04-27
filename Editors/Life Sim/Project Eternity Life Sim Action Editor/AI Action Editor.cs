using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.Editors.LifeSimCharacterEditor
{
    public partial class AIActionEditor : BaseEditor
    {
        private enum ItemSelectionChoices { Spell, Skill, Skin,  };

        private ItemSelectionChoices ItemSelectionChoice;

        public AIActionEditor()
        {
            InitializeComponent();
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
            this.Text = ItemName + " - AI Character Action Editor";

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
            Name = AIActionPath.Substring(0, AIActionPath.Length - 4).Substring(28);

            AICharacterAction LoadedAction = new AICharacterAction(Name, null, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect, AutomaticSkillTargetType.DicDefaultTarget, ManualSkillTarget.DicDefaultTarget);

            this.Text = Name + " - AI Character Action Editor";

            txtName.Text = LoadedAction.Name;
            txtDescription.Text = LoadedAction.Description;

        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, Path.GetFileNameWithoutExtension(FilePath));
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
                    case ItemSelectionChoices.Spell:
                        break;

                    case ItemSelectionChoices.Skill:
                        break;
                }
            }
        }
    }
}
