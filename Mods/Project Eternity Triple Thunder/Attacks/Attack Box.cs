using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public enum AttackInputs : byte { None, AnyPress, AnyHold, LightPress, LightHold, HeavyPress, HeavyHold }

    public enum MovementInputs : byte { Any, None, Up, Down, Left, Right, Foward, Backward, Running, Dash, Airborne }

    public class InputChoice
    {
        public AttackInputs AttackInput;
        public MovementInputs MovementInput;
        public int NextInputDelay;//Delay before accepting the next input.
        public int CurrentDelay;

        public InputChoice()
        {
        }

        public InputChoice(AttackInputs AttackInput, MovementInputs MovementInput)
        {
            this.AttackInput = AttackInput;
            this.MovementInput = MovementInput;
            NextInputDelay = 0;
        }
    }

    public enum AnimationTypes { FullAnimation, PartialAnimation, SkeletonAnimation, Null }
    public enum ComboRotationTypes { None, RotateAroundWeaponSlot, RotateAroundRobot }

    public abstract class AttackBox : Projectile2D
    {
        public List<RobotAnimation> ListAttackedRobots;//List of robot already attacked.
        public RobotAnimation Owner;
        public bool FollowOwner { get; }
        public ExplosionOptions ExplosionAttributes;

        public AttackBox(float Damage, ExplosionOptions ExplosionAttributes, RobotAnimation Owner, bool FollowOwner)
            : base()
        {
            ListAttackedRobots = new List<RobotAnimation>();

            this.Owner = Owner;
            this.Damage = Damage;
            this.ExplosionAttributes = ExplosionAttributes;
            this.FollowOwner = FollowOwner;
            ListAttackedRobots.Add(Owner);
        }

        public AttackBox(float Damage, ExplosionOptions ExplosionAttributes, RobotAnimation Owner, double Lifetime, bool FollowOwner)
            : base(Lifetime)
        {
            ListAttackedRobots = new List<RobotAnimation>();

            this.Owner = Owner;
            this.Damage = Damage;
            this.ExplosionAttributes = ExplosionAttributes;
            this.FollowOwner = FollowOwner;
            ListAttackedRobots.Add(Owner);
        }

        public virtual void Move(GameTime gameTime)
        {
            foreach (Polygon ActivePolygon in Collision.ListCollisionPolygon)
            {
                ActivePolygon.Offset(Speed.X, Speed.Y);
            }
        }
        
        public virtual void DrawRegular(CustomSpriteBatch g)
        {
            if (FightingZone.ShowCollisionBoxes)
            {
                foreach (Polygon ActivePolygon in Collision.ListCollisionPolygon)
                {
                    g.Draw(FightingZone.sprPixel, new Rectangle((int)(ActivePolygon.Center.X) - 2,
                                               (int)(ActivePolygon.Center.Y) - 2, (int)(5), (int)5), Color.Red);

                    for (int V = 0; V < ActivePolygon.ArrayVertex.Length; V++)
                    {
                        if (V < ActivePolygon.ArrayVertex.Length - 1)
                        {
                            FightingZone.DrawLine(g, ActivePolygon.ArrayVertex[V], ActivePolygon.ArrayVertex[V + 1], Color.Red);
                        }
                        else
                        {
                            FightingZone.DrawLine(g, ActivePolygon.ArrayVertex[V], ActivePolygon.ArrayVertex[0], Color.Red);
                        }
                    }
                }
            }
        }

        public virtual void DrawAdditive(CustomSpriteBatch g)
        {

        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawRegular(g);
        }

        public abstract void OnCollision(PolygonCollisionResult FinalCollisionResult, Polygon FinalCollisionPolygon, out Vector2 CollisionPoint);
    }
}
