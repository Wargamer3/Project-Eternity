using System.IO;
using System.ComponentModel;
using FMOD;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class StartMusicTrigger : BattleTrigger
    {
        private string _MusicPath;
        private bool _Loop;
        private bool _PreLoad;
        public FMODSound Sound;

        public StartMusicTrigger()
            : this(null)
        {
        }

        public StartMusicTrigger(BattleMap Map)
            : base(null, 140, 70, "Music Event", new string[] { "Check Condition" }, new string[] { })
        {
            _MusicPath = "";
            _Loop = false;
            _PreLoad = true;
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write(MusicPath);
            BW.Write(Loop);
        }

        public override void Load(BinaryReader BR)
        {
            MusicPath = BR.ReadString();
            Loop = BR.ReadBoolean();
        }

        public override void Preload()
        {
            if (PreLoad)
                Sound = new FMODSound(GameScreen.FMODSystem, "Content/Maps/BGM/" + MusicPath + ".mp3");
        }

        public override void Update(int Index)
        {
            if (!PreLoad)
                Sound = new FMODSound(GameScreen.FMODSystem, MusicPath);

            if (Loop)
            {
                Sound.SetLoop(true);
                Sound.PlayAsBGM();
                GameScreen.FMODSystem.sndActiveBGMName = MusicPath;
            }
            else
                Sound.Play();
        }

        public override MapScript CopyScript()
        {
            return new StartMusicTrigger(Map);
        }

        #region Properties

        [CategoryAttribute("Trigger values"),
        DescriptionAttribute("."),
        DefaultValueAttribute("")]
        public string MusicPath
        {
            get
            {
                return _MusicPath;
            }
            set
            {
                _MusicPath = value;
            }
        }

        [CategoryAttribute("Trigger values"),
        DescriptionAttribute("."),
        DefaultValueAttribute("")]
        public bool Loop
        {
            get
            {
                return _Loop;
            }
            set
            {
                _Loop = value;
            }
        }

        [CategoryAttribute("Trigger values"),
        DescriptionAttribute("."),
        DefaultValueAttribute("")]
        public bool PreLoad
        {
            get
            {
                return _PreLoad;
            }
            set
            {
                _PreLoad = value;
            }
        }

        #endregion
    }
}
