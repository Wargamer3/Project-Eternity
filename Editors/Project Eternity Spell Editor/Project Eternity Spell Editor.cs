using System;
using System.IO;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Magic;

namespace ProjectEternity.Editors.SpellEditor
{
    public partial class SpellEditor : BaseEditor
    {
        public SpellEditor()
        {
            InitializeComponent();
        }

        public SpellEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream FS = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
                BinaryWriter BW = new BinaryWriter(FS);

                BW.Write(0);//Magic core count.

                FS.Close();
                BW.Close();
            }

            LoadSpell(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            return new EditorInfo[]
            {
                new EditorInfo(new string[] { GUIRootPathSpells }, "Spells/", new string[] { ".pes" }, typeof(SpellEditor)),
            };
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            FileStream FS = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            SpellViewer.Save(BW);

            FS.Close();
            BW.Close();
        }

        private void LoadSpell(string SpellPath)
        {
            string Name = SpellPath.Substring(0, SpellPath.Length - 4).Substring(15);
            this.Text = Name + " - Project Eternity Spell Editor";

            SpellViewer.Preload();

            SpellViewer.Init(Name);
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, Path.GetFileName(FilePath));
        }
    }
}
