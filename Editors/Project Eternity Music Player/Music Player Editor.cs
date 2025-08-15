using System.Windows.Forms;
using ProjectEternity.Core.Editor;
using FMOD;

namespace ProjectEternity.Editors.MusicPlayer
{
    public partial class ProjectEternityMusicPlayerEditor : BaseEditor
    {
        private SoundSystem FMODSystem;
        private FMODSound NewSound;

        public ProjectEternityMusicPlayerEditor()
        {
            InitializeComponent();
        }

        public ProjectEternityMusicPlayerEditor(string FilePath, object[] Params)
            : this()
        {
            FMODSystem = new SoundSystem();
            NewSound = new FMODSound(FMODSystem, FilePath);
            NewSound.Play();
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { GUIRootPathSFX }, "SFX/", new string[] {".mp3", ".wav", ".ogg" }, typeof(ProjectEternityMusicPlayerEditor), false)
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
        }

        private void ProjectEternityMusicPlayerEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            SoundSystem.ReleaseSound(NewSound);
        }
    }
}
