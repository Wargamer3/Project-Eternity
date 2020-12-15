using ProjectEternity.Core.AI;
using ProjectEternity.GameScreens.ConquestMapScreen;

namespace ProjectEternity.AI.ConquestMapScreen
{
    public sealed partial class ConquestScriptHolder
    {
        public class HasSquadAttacked : ConquestScript, ScriptReference
        {
            public HasSquadAttacked()
                : base(100, 50, "Has Squad Attacked Conquest", new string[0], new string[0])
            {
            }

            public object GetContent()
            {
                return !Info.ActiveSquad.CanMove;
            }

            public override AIScript CopyScript()
            {
                return new HasSquadAttacked();
            }
        }
    }
}
