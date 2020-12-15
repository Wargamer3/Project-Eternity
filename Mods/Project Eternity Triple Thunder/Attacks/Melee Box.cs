using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class MeleeBox : AttackBox
    {
        public MeleeBox(float Damage, RobotAnimation Owner, double Lifetime, bool FollowOwner)
            : base(Damage, new Weapon.ExplosionOptions(), Owner, Lifetime, FollowOwner)
        {
        }

        public override void OnCollision(PolygonCollisionResult FinalCollisionResult, Polygon FinalCollisionPolygon, out Vector2 CollisionPoint)
        {
            IsAlive = false;
            CollisionPoint = ListCollisionPolygon[0].Center;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Owner.SetAttackContext(this, Owner, 0, ListCollisionPolygon[0].Center);

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
