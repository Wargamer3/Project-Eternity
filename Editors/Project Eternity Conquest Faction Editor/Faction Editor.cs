using System;
using System.IO;
using System.Text;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.Editors.RosterEditor
{
    public partial class ProjectEternityFactionEditor : BaseEditor
    {
        public ProjectEternityFactionEditor()
        {
            InitializeComponent();
        }

        public ProjectEternityFactionEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                SaveItem(FilePath, FilePath);
            }

            LoadFaction(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { EditorHelper.GUIRootPathFactionsConquest }, "Conquest/Factions/", new string[] { ".pef" }, typeof(ProjectEternityFactionEditor))
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);

            FS.Close();
            BW.Close();
        }

        private void LoadFaction(string FactionPath)
        {
            string FilePath = FactionPath.Substring(0, FactionPath.Length - 4).Substring(27);
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, Path.GetFileName(FilePath));
        }
    }
}
