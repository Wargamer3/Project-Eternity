using ProjectEternity.Core.AI;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.AI.DeathmatchMapScreen
{
    public sealed partial class DeathmatchScriptHolder
    {
        public class HasSquadAttacked : DeathmatchScript, ScriptReference
        {
            public HasSquadAttacked()
                : base(100, 50, "Has Squad Attacked", new string[0], new string[0])
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
