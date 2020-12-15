using System.Collections.Generic;

namespace ProjectEternity.Core.Scripts
{
    public sealed partial class VisualNovelCutsceneScriptHolder : CutsceneScriptHolder
    {
        public override KeyValuePair<string, List<CutsceneScript>> GetNameAndContent(params object[] args)
        {
            return new KeyValuePair<string, List<CutsceneScript>>("Visual Novel", ReflectionHelper.GetNestedTypes<CutsceneScript>(typeof(VisualNovelCutsceneScriptHolder), args));
        }
    }
}
