using System.Collections.Generic;
using ProjectEternity.Core;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    partial class SorcererStreetScriptHolder : SorcererStreetMapCutsceneScriptHolder
    {
        public override KeyValuePair<string, List<CutsceneScript>> GetNameAndContent(params object[] args)
        {
            return new KeyValuePair<string, List<CutsceneScript>>("Sorcerer Street", ReflectionHelper.GetNestedTypes<CutsceneScript>(typeof(SorcererStreetScriptHolder), args));
        }
    }
}
