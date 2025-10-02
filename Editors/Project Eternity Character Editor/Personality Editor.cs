using System;
using System.IO;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.Editors.CharacterEditor
{
    public partial class ProjectEternityPersonalityEditor : BaseEditor
    {
        public ProjectEternityPersonalityEditor()
        {
            InitializeComponent();
        }

        public ProjectEternityPersonalityEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                SaveItem(FilePath, FilePath);
            }

            LoadCharacter(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { GUIRootPathCharacterPersonalities }, "Deathmatch/Characters/Personalities/", new string[] { ".pecp" }, typeof(ProjectEternityPersonalityEditor))
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            //Create the Part file.
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            BW.Write((int)txtHitEnemy.Value);
            BW.Write((int)txtMissedEnemy.Value);
            BW.Write((int)txtDestroyedEnemy.Value);
            BW.Write((int)txtGotHit.Value);
            BW.Write((int)txtEvaded.Value);
            BW.Write((int)txtAlliedUnitDestroyed.Value);

            FS.Close();
            BW.Close();
        }

        private void LoadCharacter(string CharacterPath)
        {
            Name = CharacterPath.Substring(0, CharacterPath.Length - 5).Substring(33);
            this.Text = Name + " - Project Eternity Personality Editor";

            Character.CharacterPersonality ActivePersonality = new Character.CharacterPersonality(Name);

            //Update the editor's controls.
            txtHitEnemy.Value = ActivePersonality.WillGainHitEnemy;
            txtMissedEnemy.Value = ActivePersonality.WillGainMissedEnemy;
            txtDestroyedEnemy.Value = ActivePersonality.WillGainDestroyedEnemy;
            txtGotHit.Value = ActivePersonality.WillGainGotHit;
            txtEvaded.Value = ActivePersonality.WillGainEvaded;
            txtAlliedUnitDestroyed.Value = ActivePersonality.WillGainAlliedUnitDestroyed;
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, FilePath);
        }
    }
}
