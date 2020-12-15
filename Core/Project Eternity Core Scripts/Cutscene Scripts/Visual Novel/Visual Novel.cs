using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using ProjectEternity.GameScreens.VisualNovelScreen;

namespace ProjectEternity.Core.Scripts
{
    public sealed partial class VisualNovelCutsceneScriptHolder
    {
        public class ScriptVisualNovel : CutsceneDataContainer
        {
            private string _VisualNovelName;
            public VisualNovel ActiveVisualNovel;

            public static readonly string ScriptName = "Visual Novel";

            public ScriptVisualNovel()
                : base(100, 50, ScriptName)
            {
                _VisualNovelName = "";
            }

            public override void Load(BinaryReader BR)
            {
                VisualNovelName = BR.ReadString();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(VisualNovelName);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptVisualNovel();
            }

            #region Properties

            [Editor(typeof(Selectors.VisualNovelSelector), typeof(UITypeEditor)),
            CategoryAttribute("Visual Novel"),
            DescriptionAttribute("The name of the Visual Novel.")]
            public string VisualNovelName
            {
                get
                {
                    return _VisualNovelName;
                }
                set
                {
                    _VisualNovelName = value;
                }
            }

            #endregion
        }
    }
}
