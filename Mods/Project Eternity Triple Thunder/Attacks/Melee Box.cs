using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class MeleeBox : AttackBox
    {
        public MeleeBox(float Damage, RobotAnimation Owner, double Lifetime, bool FollowOwner)
            : base(Damage, new ExplosionOptions(), Owner, Lifetime, FollowOwner)
        {
        }

        public override void OnCollision(PolygonCollisionResult FinalCollisionResult, Polygon FinalCollisionPolygon, out Vector2 CollisionPoint)
        {
            IsAlive = false;
            CollisionPoint = Collision.ListCollisionPolygon[0].Center;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Owner.SetAttackContext(this, Owner, 0, Collision.ListCollisionPolygon[0].Center);

            if (FollowOwner)
            {
                Speed += Owner.TotalMovementThisFrame;
            }
        }

        public override void SetAngle(float Angle)
        {
        }
    }
}
