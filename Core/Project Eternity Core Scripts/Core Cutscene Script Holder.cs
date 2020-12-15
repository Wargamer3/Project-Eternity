using System.Collections.Generic;

namespace ProjectEternity.Core.Scripts
{
    public sealed partial class ScriptingScriptHolder : CutsceneScriptHolder
    {
        public override KeyValuePair<string, List<CutsceneScript>> GetNameAndContent(params object[] args)
        {
            List<CutsceneScript> ListCutsceneScript = ReflectionHelper.GetNestedTypes<CutsceneScript>(typeof(ScriptingScriptHolder), args);
            ListCutsceneScript.Insert(0, new ScriptCutsceneBehavior());
            return new KeyValuePair<string, List<CutsceneScript>>("Scripting", ListCutsceneScript);
        }
    }
}
