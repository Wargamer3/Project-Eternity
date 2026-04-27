using System;
using System.IO;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.Editors.LifeSimCharacterEditor
{
    public partial class DeityCategoryEditor : BaseEditor
    {
        public DeityCategoryEditor()
        {
            InitializeComponent();
        }

        public DeityCategoryEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                SaveItem(FilePath, FilePath);
            }

            LoadDeityCategory(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { EditorHelper.GUIRootPathLifeSimCharacterDeityCategories }, "Life Sim/Deity Categories/", new string[] { ".pedc" }, typeof(DeityCategoryEditor), true, null, false),
            };

            return Info;
        }

        public void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            this.Text = ItemName + " - Deity Category Editor";

            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            BW.Write(txtName.Text);
            BW.Write(txtDescription.Text);

            FS.Close();
            BW.Close();
        }

        private void LoadDeityCategory(string DeityCategoryPath)
        {
            Name = DeityCategoryPath.Substring(0, DeityCategoryPath.Length - 5).Substring(34);

            DeityCategory LoadedDeityCategory = new DeityCategory(Name);

            this.Text = Name + " - Deity Category Editor";

            txtName.Text = LoadedDeityCategory.Name;
            txtDescription.Text = LoadedDeityCategory.Description;
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, Path.GetFileNameWithoutExtension(FilePath));
        }
    }
}
