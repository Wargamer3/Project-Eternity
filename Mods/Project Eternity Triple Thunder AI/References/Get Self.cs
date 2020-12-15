using ProjectEternity.Core.AI;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{

    public sealed partial class GameScriptHolder
    {
        public class GetSelf : TripleThunderScript, ScriptReference
        {
            public GetSelf()
                : base(100, 50, "Get Self", new string[0], new string[0])
            {
            }

            public object GetContent()
            {
                return Info.Owner;
            }

            public override AIScript CopyScript()
            {
                return new GetSelf();
            }
        }
    }
}
