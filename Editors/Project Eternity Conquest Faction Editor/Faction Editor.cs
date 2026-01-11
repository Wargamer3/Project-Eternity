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

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[0], "Conquest Faction Editor", new string[0], typeof(ProjectEternityFactionEditor))
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            FileStream FS = new FileStream("Content/Conquest/Factions.bin", FileMode.Create);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.ASCII);


            BW.Close();
            FS.Close();
        }

        public void LoadRoster()
        {
            if (!File.Exists("Content/Roster.bin"))
            {
                return;
            }

            FileStream FS = new FileStream("Content/Conquest/Factions.bin", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS);


            BR.Close();
            FS.Close();
        }
    }
}
