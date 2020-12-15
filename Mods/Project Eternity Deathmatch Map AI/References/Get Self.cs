using ProjectEternity.Core.AI;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.AI.DeathmatchMapScreen
{
    public sealed partial class DeathmatchScriptHolder
    {
        public class GetSelf : DeathmatchScript, ScriptReference
        {
            public GetSelf()
                : base(100, 50, "Get Self Deathmatch", new string[0], new string[0])
            {
            }

            public object GetContent()
            {
                return Info.ActiveSquad;
            }

            public override AIScript CopyScript()
            {
                return new GetSelf();
            }
        }
    }
}
