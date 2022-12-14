using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public abstract class BaseShoes : UsableEquipment
    {
        public static double DashCounterStartValue = 0.4d;
        public static double MaxJumpTime = 0.1d;
        public static float LongJumpMultiplier = 0.1f;

        protected double JumpTime;
        protected bool CanLongJump;
        protected Vector2 JumpSpeedVector;
        protected float Accel => Owner.Accel;
        protected float MaxWalkSpeed => Owner.MaxWalkSpeed;

        protected double DashCounter;
        protected MovementInputs LastDashMovementInput;

        public BaseShoes(RobotAnimation Owner)
            : base(Owner)
        {
            CanLongJump = false;
            JumpTime = MaxJumpTime;
            DashCounter = 0;
            LastDashMovementInput = MovementInputs.None;
        }

        public override void Move(MovementInputs MovementInput)
        {
            Vector2 WalkSpeed = MaxWalkSpeed * Owner.NormalizedGroundVector;

            Owner.ActiveSpriteEffects = SpriteEffects.None;

            #region Move Right

            if (MovementInput == MovementInputs.Right)
            {
                if (Owner.NormalizedGroundVector.X > 0)
                {
                    if (Owner.Speed.X < WalkSpeed.X)
                    {
                        Owner.Speed.X += Accel * Owner.NormalizedGroundVector.X;
                        if (Owner.Speed.X > WalkSpeed.X)
                            Owner.Speed.X = MaxWalkSpeed * Owner.NormalizedGroundVector.X;
                    }
                }
                else if (Owner.NormalizedGroundVector.X < 0)
                {
                    if (Owner.Speed.X > -WalkSpeed.X)
                    {
                        Owner.Speed.X += Accel * Owner.NormalizedGroundVector.X;
                        if (Owner.Speed.X < WalkSpeed.X)
                            Owner.Speed.X = MaxWalkSpeed * Owner.NormalizedGroundVector.X;
                    }
                }
                if (Owner.NormalizedGroundVector.Y > 0)
                {
                    if (Owner.Speed.Y < WalkSpeed.Y)
                    {
                        Owner.Speed.Y += Accel * Owner.NormalizedGroundVector.Y;
                        if (Owner.Speed.Y > WalkSpeed.Y)
                            Owner.Speed.Y = MaxWalkSpeed * Owner.NormalizedGroundVector.Y;
                    }
                }
                else if (Owner.NormalizedGroundVector.Y < 0)
                {
                    if (Owner.Speed.Y > -WalkSpeed.Y)
                    {
                        Owner.Speed.Y += Accel * Owner.NormalizedGroundVector.Y;
                        if (Owner.Speed.Y < WalkSpeed.Y)
                            Owner.Speed.Y = MaxWalkSpeed * Owner.NormalizedGroundVector.Y;
                    }
                }
            }

            #endregion

            #region Move Left

            else if (MovementInput == MovementInputs.Left)
            {
                Owner.ActiveSpriteEffects = SpriteEffects.FlipHorizontally;
                if (Owner.NormalizedGroundVector.X > 0)
                {
                    if (Owner.Speed.X > -WalkSpeed.X)
                    {
                        Owner.Speed.X -= Accel * Owner.NormalizedGroundVector.X;
                        if (Owner.Speed.X < WalkSpeed.X)
                            Owner.Speed.X = -MaxWalkSpeed * Owner.NormalizedGroundVector.X;
                    }
                }
                else if (Owner.NormalizedGroundVector.X < 0)
                {
                    if (Owner.Speed.X < WalkSpeed.X)
                    {
                        Owner.Speed.X -= Accel * Owner.NormalizedGroundVector.X;
                        if (Owner.Speed.X > WalkSpeed.X)
                            Owner.Speed.X = -MaxWalkSpeed * Owner.NormalizedGroundVector.X;
                    }
                }
                if (Owner.NormalizedGroundVector.Y > 0)
                {
                    if (Owner.Speed.Y > -WalkSpeed.Y)
                    {
                        Owner.Speed.Y -= Accel * Owner.NormalizedGroundVector.Y;
                        if (Owner.Speed.Y < WalkSpeed.Y)
                            Owner.Speed.Y = -MaxWalkSpeed * Owner.NormalizedGroundVector.Y;
                    }
                }
                else if (Owner.NormalizedGroundVector.Y < 0)
                {
                    if (Owner.Speed.Y < WalkSpeed.Y)
                    {
                        Owner.Speed.Y -= Accel * Owner.NormalizedGroundVector.Y;
                        if (Owner.Speed.Y > WalkSpeed.Y)
                            Owner.Speed.Y = -MaxWalkSpeed * Owner.NormalizedGroundVector.Y;
                    }
                }
            }

            #endregion

            if (DashCounter > 0 && LastDashMovementInput == MovementInput)
            {
                Dash(MovementInput);
            }
            else
            {
                if (LastDashMovementInput != MovementInput)
                {
                    DashCounter = 0;
                }

                LastDashMovementInput = MovementInput;
            }

            Owner.ActiveMovementStance = "Moving";
            Owner.CurrentMovementInput = MovementInput;
        }

        public override void OnIdle()
        {
            if (DashCounter <= 0 && LastDashMovementInput != MovementInputs.None)
            {
                DashCounter = DashCounterStartValue;
            }
        }

        private void Dash(MovementInputs MovementInput)
        {
            Owner.LockAnimation = true;
            LastDashMovementInput = MovementInputs.None;
            DashCounter = 0;

            if (MovementInput == MovementInputs.Right)
            {
                Owner.Speed = MaxWalkSpeed * Owner.NormalizedGroundVector * 2;
            }
            else if (MovementInput == MovementInputs.Left)
            {
                Owner.ActiveSpriteEffects = SpriteEffects.FlipHorizontally;

                Owner.Speed = -MaxWalkSpeed * Owner.NormalizedGroundVector * 2;
            }
        }

        public override void OnJetpackUse(GameTime gameTime)
        {
        }

        public override void OnJetpackRest(GameTime gameTime)
        {
        }

        public override void OnAnyCollision(List<Tuple<PolygonCollisionResult, Polygon>> ListAllCollidingPolygon)
        {
        }

        public override void OnFloorCollision(List<Tuple<PolygonCollisionResult, Polygon>> ListFloorCollidingPolygon)
        {
            Owner.Land();

            Vector2 MovementCorection = Vector2.Zero;
            foreach (Tuple<PolygonCollisionResult, Polygon> FinalCollisionResult in ListFloorCollidingPolygon)
            {
                MovementCorection += FinalCollisionResult.Item1.Axis * FinalCollisionResult.Item1.Distance;

                Vector2 GroundAxis = new Vector2(-FinalCollisionResult.Item1.Axis.Y, FinalCollisionResult.Item1.Axis.X);
                double FinalCollisionResultAngle = Math.Atan2(GroundAxis.X, GroundAxis.Y);

                Owner.NormalizedGroundVector = GroundAxis;
                Owner.NormalizedPerpendicularGroundVector = FinalCollisionResult.Item1.Axis;
            }

            Vector2 FinalMovement = Owner.Speed + MovementCorection;

            Owner.Move(FinalMovement);
        }

        public override void OnCeilingCollision(List<Tuple<PolygonCollisionResult, Polygon>> ListCeilingCollidingPolygon)
        {
            Owner.Speed.Y = 0;
            Owner.UpdateSkills(TripleThunderRobotRequirement.OnCeilingCollisionName);

            Vector2 MovementCorection = Vector2.Zero;
            foreach (Tuple<PolygonCollisionResult, Polygon> FinalCollisionResult in ListCeilingCollidingPolygon)
            {
                MovementCorection += FinalCollisionResult.Item1.Axis * FinalCollisionResult.Item1.Distance;
            }

            Vector2 FinalMovement = Owner.Speed + MovementCorection;

            Owner.Move(FinalMovement);
        }

        public override void OnWallCollision(List<Tuple<PolygonCollisionResult, Polygon>> ListWallCollidingPolygon)
        {
            Vector2 MovementCorection = Vector2.Zero;
            bool IsAWall = false;
            //Check if the wall is not a floor/slope
            foreach (Tuple<PolygonCollisionResult, Polygon> FinalCollisionResult in ListWallCollidingPolygon)
            {
                foreach (Polygon ActivePlayerCollisionPolygon in Owner.Collision.ListCollisionPolygon)
                {
                    PolygonCollisionResult CollisionResult = Polygon.PolygonCollisionSAT(ActivePlayerCollisionPolygon, FinalCollisionResult.Item2, Owner.Speed, new Vector2(0, -Owner.Speed.Y - 1));

                    if (CollisionResult.Distance >= 0)
                    {
                        IsAWall = true;
                        MovementCorection += CollisionResult.Axis * CollisionResult.Distance;
                        break;
                    }
                }
            }

            if (IsAWall)
            {
                Owner.UpdateSkills(TripleThunderRobotRequirement.OnWallCollisionName);

                Vector2 FinalMovement = Owner.Speed + MovementCorection;

                Owner.Move(FinalMovement);
            }
        }
    }
}
