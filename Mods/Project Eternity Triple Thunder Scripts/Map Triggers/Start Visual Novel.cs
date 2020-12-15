using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Scripts;
using ProjectEternity.GameScreens.VisualNovelScreen;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed class StartVisualNovelTrigger : FightingZoneTrigger
    {
        private string _VNPath;
        private bool _PreLoad;
        public VisualNovel VisualNovel;

        public StartVisualNovelTrigger()
            : this(null)
        {
        }

        public StartVisualNovelTrigger(FightingZone Map)
            : base(Map, 140, 70, "Start Visual Novel Event", new string[] { "Check Condition" }, new string[] { })
        {
            _VNPath = "";
            _PreLoad = true;
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write(VNPath);
        }

        public override void Load(BinaryReader BR)
        {
            VNPath = BR.ReadString();
        }

        public override void Preload()
        {
            if (PreLoad)
                VisualNovel = new VisualNovel(VNPath);
        }

        public override void Update(int Index)
        {
            if (!PreLoad)
                Map.PushScreen(new VisualNovel(VNPath));
            else
                Map.PushScreen(VisualNovel);

            Map.ExecuteFollowingScripts(this, 0);
        }

        public override MapScript CopyScript()
        {
            return new StartVisualNovelTrigger(Map);
        }

        #region Properties

        [CategoryAttribute("Trigger values"),
        DescriptionAttribute("."),
        DefaultValueAttribute("")]
        public string VNPath
        {
            get
            {
                return _VNPath;
            }
            set
            {
                _VNPath = value;
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
