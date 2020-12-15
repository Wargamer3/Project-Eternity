using ProjectEternity.Core.AI;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{

    public sealed partial class GameScriptHolder
    {
        public class GetRobotX : TripleThunderScript, ScriptReference
        {
            private RobotAnimation Target;

            public GetRobotX()
                : base(100, 50, "Get Robot X", new string[0], new string[1] { "Robot to use" })
            {
            }

            public object GetContent()
            {
                if (Target == null)
                    Target = (RobotAnimation)ArrayReferences[0].ReferencedScript.GetContent();

                return (double)Target.Position.X;
            }

            public override AIScript CopyScript()
            {
                return new GetRobotX();
            }
        }
    }
}
