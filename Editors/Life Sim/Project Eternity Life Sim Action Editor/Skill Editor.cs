using System;
using System.IO;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.Editors.LifeSimCharacterEditor
{
    public partial class CharacterSkillEditor : BaseEditor
    {
        public CharacterSkillEditor()
        {
            InitializeComponent();
        }

        public CharacterSkillEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                SaveItem(FilePath, FilePath);
            }

            LoadSkill(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { EditorHelper.GUIRootPathLifeSimSkills }, "Life Sim/Skills/", new string[] { ".pes" }, typeof(CharacterSkillEditor), true, null, false),
            };

            return Info;
        }

        public void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            this.Text = ItemName + " - Skill Editor";

            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            BW.Write(txtName.Text);
            BW.Write(txtDescription.Text);

            BW.Write((byte)lsActions.Items.Count);
            for (int A = 0; A < lsActions.Items.Count; ++A)
            {
                BW.Write(lsActions.Items[A].ToString());
            }

            FS.Close();
            BW.Close();
        }

        private void LoadSkill(string SkillPath)
        {
            Name = SkillPath.Substring(0, SkillPath.Length - 4).Substring(24);

            CharacterSkill LoadedSkill = new CharacterSkill(Name);

            this.Text = Name + " - Skill Editor";

            txtName.Text = LoadedSkill.Name;
            txtDescription.Text = LoadedSkill.Description;

        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, Path.GetFileNameWithoutExtension(FilePath));
        }
    }
}
