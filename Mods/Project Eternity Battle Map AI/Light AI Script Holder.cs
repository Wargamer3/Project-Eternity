using System.Collections.Generic;
using ProjectEternity.Core;
using ProjectEternity.Core.AI;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.AI.BattleMapScreen
{
    public sealed partial class BattleLightScriptHolder : BattleAIScriptHolder
    {
        public override KeyValuePair<string, List<AIScript>> GetNameAndContent(params object[] args)
        {
            List<AIScript> ListAIScript = ReflectionHelper.GetNestedTypes<AIScript>(typeof(BattleLightScriptHolder), args);
            return new KeyValuePair<string, List<AIScript>>("Battle Lights", ListAIScript);
        }
    }
}
