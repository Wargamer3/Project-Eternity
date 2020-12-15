using ProjectEternity.Core.AI;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{

    public sealed partial class GameScriptHolder
    {
        public class DistanceToEnemy : TripleThunderScript, ScriptReference
        {
            public DistanceToEnemy()
                : base(100, 50, "Distance To Enemy", new string[0], new string[1] { "Enemy" })
            {
            }

            public object GetContent()
            {
                return (double)Vector2.Distance(Info.Owner.Position, ((RobotAnimation)ArrayReferences[0].ReferencedScript.GetContent()).Position);
            }

            public override AIScript CopyScript()
            {
                return new DistanceToEnemy();
            }
        }
    }
}
