using ProjectEternity.Core.AI;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{

    public sealed partial class GameScriptHolder
    {
        public class GetRobotY : TripleThunderScript, ScriptReference
        {
            private RobotAnimation Target;

            public GetRobotY()
                : base(100, 50, "Get Robot Y", new string[0], new string[1] { "Robot to use" })
            {
            }

            public object GetContent()
            {
                if (Target == null)
                    Target = (RobotAnimation)ArrayReferences[0].ReferencedScript.GetContent();

                return Target.Position.Y;
            }

            public override AIScript CopyScript()
            {
                return new GetRobotY();
            }
        }
    }
}
