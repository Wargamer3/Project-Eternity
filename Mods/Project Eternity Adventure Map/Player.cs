using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.AdventureScreen
{
    public class Player : ICollisionObjectHolder2D<Player>
    {
        public AIContainer PlayerAI;
        private bool HasKnockback;
        private Vector2 Position;
        public float HP;
        private Vector2 Speed;
        private CollisionObject2D<Player> CollisionBox;
        public CollisionObject2D<Player> Collision => CollisionBox;
        private readonly AdventureMap Owner;

        public Player(AdventureMap Owner, Vector2[] ArrayVertex, int MaxWidth, int MaxHeight)
        {
            this.Owner = Owner;
            HP = 100;

            CollisionBox = new CollisionObject2D<Player>(ArrayVertex, MaxWidth, MaxHeight);
        }

        public void Update(GameTime gameTime)
        {
            if (PlayerAI != null)
            {
                PlayerAI.UpdateStep(gameTime);
            }

            PolygonCollisionResult FinalCollisionResult;
            Polygon FinalCollisionPolygon;
            Polygon FinalOtherPolygon;

            HashSet<Player> SetOtherPlayer = Owner.GetCollidingPlayers(this);
            foreach(Player ActivePlayer in SetOtherPlayer)
            {
                if (ActivePlayer.Collision.CollideWith(CollisionBox, Speed, out FinalCollisionResult, out FinalCollisionPolygon, out FinalOtherPolygon))
                {
                    OnCollision(ActivePlayer, FinalCollisionResult, FinalCollisionPolygon, FinalOtherPolygon);
                }
            }

            HashSet<WorldObject> SetWorldObject = Owner.GetCollidingWorldObjects(this);
            foreach (WorldObject ActiveWorldObject in SetWorldObject)
            {
                if (ActiveWorldObject.Collision.CollideWith(CollisionBox, Speed, out FinalCollisionResult, out FinalCollisionPolygon, out FinalOtherPolygon))
                {
                    OnCollision(ActiveWorldObject, FinalCollisionResult, FinalCollisionPolygon, FinalOtherPolygon);
                }
            }
        }

        public void Walk(Vector2 Speed)
        {
            this.Speed = Speed;
        }

        public void DropBomb()
        {
            Owner.Add(new Bomb(Owner, Position));
        }

        private void Move(Vector2 Movement)
        {
            Position += Movement;
            Owner.Move(this, Movement);
        }

        public void OnCollision(Player Other, PolygonCollisionResult FinalCollisionResult, Polygon FinalCollisionPolygon, Polygon FinalOtherPolygon)
        {
            Vector2 FinalMovement;
            if (FinalCollisionResult.Distance >= 0)
            {
                Vector2 MovementCorection = FinalCollisionResult.Axis * FinalCollisionResult.Distance;
                FinalMovement = Speed + MovementCorection;
            }
            else
            {
                FinalMovement = Speed;
            }

            Move(FinalMovement);
        }

        public void OnCollision(GameTime gameTime, Bullet Other, PolygonCollisionResult FinalCollisionResult, Polygon FinalCollisionPolygon, Polygon FinalOtherPolygon)
        {
            if (PlayerAI != null)
                PlayerAI.Update(gameTime, "On Hit");

            if (HasKnockback)
            {
                Speed.X = Math.Sign(Other.Speed.X) * 3;
                Speed.Y = Math.Sign(Other.Speed.Y) * 3;
            }

            int FinalDamage = (int)Other.Damage;

            //FightingZone.ListDamageNumber.Add(new DamageNumber(FinalCollisionPolygon.Center, FinalDamage, 1000));
            HP -= FinalDamage;

            if (HP < 0)
                Die();
        }

        public void OnCollision(WorldObject Other, PolygonCollisionResult FinalCollisionResult, Polygon FinalPlayerPolygon, Polygon FinalWorldPolygon)
        {
            Vector2 GroundAxis = new Vector2(-FinalCollisionResult.Axis.Y, FinalCollisionResult.Axis.X);
            double FinalCollisionResultAngle = Math.Atan2(GroundAxis.X, GroundAxis.Y);

            Vector2 MovementCorection = FinalCollisionResult.Axis * FinalCollisionResult.Distance;
            Vector2 FinalMovement = Speed + MovementCorection;

            Move(FinalMovement);
        }

        public void Die()
        {
            Owner.Remove(this);
        }
    }
}
