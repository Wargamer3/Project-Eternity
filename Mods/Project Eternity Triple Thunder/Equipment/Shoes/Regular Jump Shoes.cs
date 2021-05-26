using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class RegularShoes : BaseShoes
    {
        private bool IsJumping;

        private bool IsReadyToJump;

        public RegularShoes(RobotAnimation Owner)
            : base(Owner)
        {
            CanLongJump = false;
            IsJumping = true;
            IsReadyToJump = true;
            JumpTime = MaxJumpTime;
        }

        public override void Update(GameTime gameTime)
        {
            if (DashCounter > 0)
            {
                DashCounter -= gameTime.ElapsedGameTime.TotalSeconds;
                if (DashCounter <= 0)
                {
                    LastDashMovementInput = MovementInputs.None;
                }
            }

            if (Owner.Position.Y + Owner.Speed.Y < Owner.CurrentLane)
            {
                if (IsJumping)
                {
                    JumpTime += gameTime.ElapsedGameTime.TotalSeconds;
                    Owner.Speed += JumpSpeedVector * LongJumpMultiplier;
                }
                if (Owner.Speed.Y < Owner.GravityMax)
                {
                    Owner.Speed.Y += Owner.Gravity;
                }
            }
            else
            {
                Owner.Speed.Y = 0;
                Owner.Position.Y = Owner.CurrentLane;
                Owner.Land();
            }
        }

        private bool CanJump()
        {
            return Owner.IsOnGround || (!IsJumping && Vector2.Distance(Owner.Position, Owner.LastPositionOnGround) <= Owner.Speed.Length());
        }

        public override void OnJump()
        {
            if (CanLongJump || Owner.IsOnGround)
            {
                if (JumpTime < MaxJumpTime)
                {//Give the character some jump speed on the first step.
                    if (IsReadyToJump && CanJump())
                    {
                        IsReadyToJump = false;
                        Vector2 VerticalSpeed = Owner.Speed * Owner.NormalizedPerpendicularGroundVector;
                        JumpSpeedVector = Owner.JumpSpeed * -Owner.GravityVector;
                        Owner.Speed += VerticalSpeed + JumpSpeedVector;
                        if (Owner.Sounds.JumpStartSound != UnitSounds.JumpStartSounds.None)
                        {
                            Owner.PlayerSFXGenerator.PlayJumpStartSound(Owner.Sounds.JumpStartSound);
                        }
                        if (Owner.Sounds.JumpStrainSound != UnitSounds.JumpStrainSounds.None)
                        {
                            Owner.PlayerSFXGenerator.PlayStrainSound(Owner.Sounds.JumpStrainSound);
                        }
                    }

                    Owner.ActiveMovementStance = "Airborne";
                    IsJumping = true;
                }
                else
                {
                    CanLongJump = false;
                    IsJumping = false;
                }
            }


            IsReadyToJump = false;
        }

        public override void OnLand()
        {
            CanLongJump = true;
            JumpTime = 0;
        }

        public override void OnFall()
        {
            if (!CanLongJump)
            {
                IsJumping = false;
                JumpTime = MaxJumpTime;
            }
        }

        public override void OnStopJump()
        {
            if (CanJump())
            {
                CanLongJump = true;
                JumpTime = 0;
            }
            else
            {
                JumpTime = MaxJumpTime;
            }

            IsJumping = false;
            IsReadyToJump = true;
        }

        public override void OnFloorCollision(List<Tuple<PolygonCollisionResult, Polygon>> ListFloorCollidingPolygon)
        {
            //Jumping
            if (IsJumping)
            {

                Vector2 MovementCorection = Vector2.Zero;
                foreach (Tuple<PolygonCollisionResult, Polygon> FinalCollisionResult in ListFloorCollidingPolygon)
                {
                    MovementCorection += FinalCollisionResult.Item1.Axis * FinalCollisionResult.Item1.Distance;
                }

                Vector2 FinalMovement = Owner.Speed + MovementCorection;

                Owner.Move(FinalMovement);
                return;
            }

            Owner.Land();

            if (Owner.IsInAir)
            {
                Owner.PlayerSFXGenerator.PlayLandSound();
            }

            bool NeedToMoveUp = true;
            bool HasMovedUp = false;
            bool HasCollided = false;

            while (NeedToMoveUp)
            {
                foreach (Polygon ActivePlayerCollisionPolygon in Owner.Collision.ListCollisionPolygon)
                {
                    if (!NeedToMoveUp)
                        continue;

                    foreach (Tuple<PolygonCollisionResult, Polygon> FinalCollisionResult in ListFloorCollidingPolygon)
                    {
                        Vector2 GroundAxis = new Vector2(-FinalCollisionResult.Item1.Axis.Y, FinalCollisionResult.Item1.Axis.X);

                        Owner.NormalizedGroundVector = GroundAxis;
                        Owner.NormalizedPerpendicularGroundVector = FinalCollisionResult.Item1.Axis;

                        PolygonCollisionResult CollisionResult = Polygon.PolygonCollisionSAT(ActivePlayerCollisionPolygon, FinalCollisionResult.Item2, Vector2.Zero, Vector2.Zero);

                        if (CollisionResult.Distance >= 0)
                        {
                            Owner.Move(-FightingZone.GravityVector);
                            HasMovedUp = true;
                            continue;
                        }
                        else
                        {
                            NeedToMoveUp = false;
                        }
                    }
                }
            }

            if (!HasMovedUp)
            {
                float MaxRetry = Vector2.Dot(Owner.Speed, Owner.GravityVector);
                while (!HasCollided && MaxRetry > 0)
                {
                    --MaxRetry;

                    bool NeedToMoveDown = true;
                    foreach (Tuple<PolygonCollisionResult, Polygon> FinalCollisionResult in ListFloorCollidingPolygon)
                    {
                        foreach (Polygon ActivePlayerCollisionPolygon in Owner.Collision.ListCollisionPolygon)
                        {
                            PolygonCollisionResult CollisionResult = Polygon.PolygonCollisionSAT(ActivePlayerCollisionPolygon, FinalCollisionResult.Item2, Owner.GravityVector);

                            if (CollisionResult.Distance >= 0)
                            {
                                NeedToMoveDown = false;
                                break;
                            }
                        }
                    }

                    if (NeedToMoveDown)
                    {
                        Owner.Move(FightingZone.GravityVector);
                    }
                    else
                    {
                        HasCollided = true;
                    }
                }
            }

            Vector2 VerticalSpeedCancelVector = Owner.Speed * -Owner.GravityVector;

            Owner.Move(Owner.Speed + VerticalSpeedCancelVector);

            Owner.Speed += VerticalSpeedCancelVector + Owner.GravityVector * 3;//Extra gravity determine how much you can stick to a downward slope.
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
