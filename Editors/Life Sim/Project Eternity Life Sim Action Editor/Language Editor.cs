using System;
using System.IO;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.Editors.LifeSimCharacterEditor
{
    public partial class LanguageEditor : BaseEditor
    {
        public LanguageEditor()
        {
            InitializeComponent();
        }

        public LanguageEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                SaveItem(FilePath, FilePath);
            }

            LoadLanguage(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { EditorHelper.GUIRootPathLifeSimLanguages }, "Life Sim/Languages/", new string[] { ".pel" }, typeof(LanguageEditor), true, null, false),
            };

            return Info;
        }

        public void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            this.Text = ItemName + " - Language Editor";

            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            BW.Write(txtName.Text);
            BW.Write(txtDescription.Text);

            FS.Close();
            BW.Close();
        }

        private void LoadLanguage(string LanguagePath)
        {
            Name = LanguagePath.Substring(0, LanguagePath.Length - 4).Substring(27);

            Language LoadedLanguage = new Language(Name);

            this.Text = Name + " - Language Editor";

            txtName.Text = LoadedLanguage.Name;
            txtDescription.Text = LoadedLanguage.Description;
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, Path.GetFileNameWithoutExtension(FilePath));
        }
    }
}
