using System.Collections.Generic;
using ProjectEternity.Core;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public sealed partial class ConquestCutsceneScriptHolder : ConquestMapCutsceneScriptHolder
    {
        public override KeyValuePair<string, List<CutsceneScript>> GetNameAndContent(params object[] args)
        {
            return new KeyValuePair<string, List<CutsceneScript>>("Conquest Map", ReflectionHelper.GetNestedTypes<CutsceneScript>(typeof(ConquestCutsceneScriptHolder), args));
        }
    }
}
