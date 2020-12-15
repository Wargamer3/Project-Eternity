using System.IO;
using System.Drawing.Design;
using System.ComponentModel;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class StartCutsceneTrigger : BattleTrigger
    {
        private string _CutscenePath;
        private bool _PreLoad;
        public Cutscene Cutscene;

        public StartCutsceneTrigger()
            : this(null)
        {
            _CutscenePath = "";
            _PreLoad = true;
        }

        public StartCutsceneTrigger(BattleMap Map)
            : base(Map, 140, 70, "Start Cutscene", new string[] { "Start Cutscene" }, new string[] { })
        {
            _CutscenePath = "";
            _PreLoad = true;
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write(CutscenePath);
        }

        public override void Load(BinaryReader BR)
        {
            CutscenePath = BR.ReadString();
        }

        public override void Preload()
        {
            if (PreLoad)
                Cutscene = new Cutscene(null, CutscenePath, Map?.DicCutsceneScript);
        }

        public override void Update(int Index)
        {
            if (!PreLoad)
                Map.PushScreen(new Cutscene(null, CutscenePath, Map?.DicCutsceneScript));
            else
                Map.PushScreen(Cutscene);
        }

        public override MapScript CopyScript()
        {
            return new StartCutsceneTrigger(Map);
        }

        #region Properties

        [Editor(typeof(Selectors.CutsceneSelector), typeof(UITypeEditor)),
        CategoryAttribute("Trigger values"),
        DescriptionAttribute("."),
        DefaultValueAttribute("")]
        public string CutscenePath
        {
            get
            {
                return _CutscenePath;
            }
            set
            {
                _CutscenePath = value;
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
