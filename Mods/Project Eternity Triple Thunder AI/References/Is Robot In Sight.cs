using ProjectEternity.Core.AI;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{

    public sealed partial class GameScriptHolder
    {
        public class IsRobotInSight : TripleThunderScript, ScriptReference
        {
            private RobotAnimation Target;

            public IsRobotInSight()
                : base(100, 50, "Is Robot In Sight", new string[0], new string[1] { "Robot to use" })
            {
            }

            public object GetContent()
            {
                if (Target == null)
                    Target = (RobotAnimation)ArrayReferences[0].ReferencedScript.GetContent();

                Rectangle TargetCollisionSize = Target.GetCollisionSize();

                Vector2 TargetPoint1 = new Vector2(TargetCollisionSize.Center.X - Info.Owner.Position.X, TargetCollisionSize.Top - Info.Owner.Position.Y);
                Vector2 TargetPoint2 = new Vector2(TargetCollisionSize.Center.X - Info.Owner.Position.X, TargetCollisionSize.Bottom - Info.Owner.Position.Y);

                PointOfViewBox FirstPointOfView = new PointOfViewBox(Info.Owner.Position, TargetPoint1);
                PointOfViewBox SecondPointOfView = new PointOfViewBox(Info.Owner.Position, TargetPoint2);

                Info.OwnerLayer.UpdateAttackCollisionWithWorld(FirstPointOfView);
                Info.OwnerLayer.UpdateAttackCollisionWithWorld(SecondPointOfView);

                //If the collision box didn't collide with the world, the target is visible.
                if (FirstPointOfView.IsAlive || SecondPointOfView.IsAlive)
                {
                    return true;
                }

                return false;
            }

            public override AIScript CopyScript()
            {
                return new IsRobotInSight();
            }
        }
    }
}
