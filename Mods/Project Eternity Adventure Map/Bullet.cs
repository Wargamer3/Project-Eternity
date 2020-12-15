using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.AdventureScreen
{
    public class Bullet : Projectile, ICollisionObject<Bullet>
    {
        public List<Player> ListAttackedRobots;//List of Player already attacked.
        private CollisionObject<Bullet> CollisionBox;
        public CollisionObject<Bullet> Collision => CollisionBox;
        private readonly AdventureMap Owner;

        public Bullet(AdventureMap Owner, Vector2[] ArrayVertex, int MaxWidth, int MaxHeight)
        {
            this.Owner = Owner;

            CollisionBox = new CollisionObject<Bullet>(ArrayVertex, MaxWidth, MaxHeight);
            ListAttackedRobots = new List<Player>();
            Damage = 5;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            PolygonCollisionResult FinalCollisionResult;
            Polygon FinalCollisionPolygon;
            Polygon FinalOtherPolygon;

            HashSet<Player> SetOtherPlayer = Owner.GetCollidingPlayers(this);
            foreach (Player ActivePlayer in SetOtherPlayer)
            {
                if (ActivePlayer.Collision.CollideWith(CollisionBox, Speed, out FinalCollisionResult, out FinalCollisionPolygon, out FinalOtherPolygon))
                {
                    OnCollision(gameTime, ActivePlayer, FinalCollisionResult, FinalCollisionPolygon, FinalOtherPolygon);
                }
            }

            HashSet<WorldObject> SetWorldObject = Owner.GetCollidingWorldObjects(this);
            foreach (WorldObject ActiveWorldObject in SetWorldObject)
            {
                if (ActiveWorldObject.Collision.CollideWith(CollisionBox, Speed, out FinalCollisionResult, out FinalCollisionPolygon, out FinalOtherPolygon))
                {
                    OnCollision(ActiveWorldObject);
                }
            }
        }

        private void Move()
        {
            CollisionBox.Center += Speed;
        }

        public override void SetAngle(float Angle)
        {
            throw new NotImplementedException();
        }

        public void OnCollision(GameTime gameTime, Player Other, PolygonCollisionResult FinalCollisionResult, Polygon FinalCollisionPolygon, Polygon FinalOtherPolygon)
        {
            Other.OnCollision(gameTime, this, FinalCollisionResult, FinalCollisionPolygon, FinalOtherPolygon);

            Vector2 CollisionPoint;
            OnCollision(FinalCollisionResult, FinalCollisionPolygon, out CollisionPoint);

            CreateExplosion(CollisionPoint);

            ListAttackedRobots.Add(Other);

            Die();
        }

        public virtual void OnCollision(PolygonCollisionResult FinalCollisionResult, Polygon FinalCollisionPolygon, out Vector2 CollisionPoint)
        {
            CollisionPoint = Vector2.Zero;
        }

        public void OnCollision(Bullet Other)
        {
            throw new NotImplementedException();
        }

        public void OnCollision(WorldObject Other)
        {
            Die();
        }

        private void CreateExplosion(Vector2 ExplosionCenter)
        {
        }

        public void Die()
        {
            IsAlive = false;
        }

        public override void Draw(CustomSpriteBatch g)
        {
            throw new NotImplementedException();
        }
    }
}
