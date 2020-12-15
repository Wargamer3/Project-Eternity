using System.Collections.Generic;
using ProjectEternity.Core;
using ProjectEternity.Core.AI;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    //Triggers: OnNewFrame, OnEnemyDetected, OnEnemyLost, OnHit, OnHited, OnMiss, OnMissed
    //Actions: SetVariable, Aim, Shoot, Jump, Walk, Jetpack

    public sealed partial class GameScriptHolder : TripleThunderScriptHolder
    {
        public override KeyValuePair<string, List<AIScript>> GetNameAndContent(params object[] args)
        {
            List<AIScript> ListTripleThunderScript = ReflectionHelper.GetNestedTypes<AIScript>(typeof(GameScriptHolder), args);
            return new KeyValuePair<string, List<AIScript>>("Game", ListTripleThunderScript);
        }
    }
}
