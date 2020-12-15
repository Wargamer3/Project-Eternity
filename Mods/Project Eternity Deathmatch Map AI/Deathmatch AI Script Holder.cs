using System.Collections.Generic;
using ProjectEternity.Core;
using ProjectEternity.Core.AI;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.AI.DeathmatchMapScreen
{
    public sealed partial class DeathmatchScriptHolder : DeathmatchAIScriptHolder
    {
        public override KeyValuePair<string, List<AIScript>> GetNameAndContent(params object[] args)
        {
            List<AIScript> ListAIScript = ReflectionHelper.GetNestedTypes<AIScript>(typeof(DeathmatchScriptHolder), args);
            return new KeyValuePair<string, List<AIScript>>("Deathmatch", ListAIScript);
        }
    }
}
