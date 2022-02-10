using System.Collections.Generic;
using ProjectEternity.Core;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed partial class DeathmatchCutsceneScriptHolder : DeathmatchMapCutsceneScriptHolder
    {
        public override KeyValuePair<string, List<CutsceneScript>> GetNameAndContent(params object[] args)
        {
            return new KeyValuePair<string, List<CutsceneScript>>("Deathmatch Map", ReflectionHelper.GetNestedTypes<CutsceneScript>(typeof(DeathmatchCutsceneScriptHolder), args));
        }
    }
}
