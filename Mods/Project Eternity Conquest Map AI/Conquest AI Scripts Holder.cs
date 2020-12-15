using System.Collections.Generic;
using ProjectEternity.Core;
using ProjectEternity.Core.AI;
using ProjectEternity.GameScreens.ConquestMapScreen;

namespace ProjectEternity.AI.ConquestMapScreen
{
    public sealed partial class ConquestScriptHolder : ConquestAIScriptHolder
    {
        public override KeyValuePair<string, List<AIScript>> GetNameAndContent(params object[] args)
        {
            List<AIScript> ListAIScript = ReflectionHelper.GetNestedTypes<AIScript>(typeof(ConquestScriptHolder), args);
            return new KeyValuePair<string, List<AIScript>>("Conquest", ListAIScript);
        }
    }
}
