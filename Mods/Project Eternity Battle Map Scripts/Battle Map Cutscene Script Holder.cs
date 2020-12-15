using System.Collections.Generic;
using ProjectEternity.Core;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.BattleMapScreen
{

    public sealed partial class ExtraBattleMapCutsceneScriptHolder : BattleMapCutsceneScriptHolder
    {
        public ExtraBattleMapCutsceneScriptHolder()
        {
        }

        public override KeyValuePair<string, List<CutsceneScript>> GetNameAndContent(params object[] args)
        {
            return new KeyValuePair<string, List<CutsceneScript>>("Battle Map", ReflectionHelper.GetNestedTypes<CutsceneScript>(typeof(ExtraBattleMapCutsceneScriptHolder), args));
        }
    }
}
