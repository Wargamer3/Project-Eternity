using System;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class LongJumpShoes : BaseShoes
    {
        private bool IsJumping;

        public LongJumpShoes(RobotAnimation Owner)
            : base(Owner)
        {
            IsJumping = true;
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
            return Owner.IsOnGround || Vector2.Distance(Owner.Position, Owner.LastPositionOnGround) <= Owner.Speed.Length();
        }

        public override void OnJump()
        {
            if (CanLongJump || Owner.IsOnGround)
            {
                if (JumpTime < MaxJumpTime)
                {//Give the character some jump speed on the first step.
                    if (CanJump())
                    {
                        Vector2 VerticalSpeed = Owner.Speed * Owner.NormalizedPerpendicularGroundVector;
                        JumpSpeedVector = Owner.JumpSpeed * Owner.NormalizedPerpendicularGroundVector;
                        Owner.Speed = Owner.Speed + VerticalSpeed + JumpSpeedVector;
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
            IsJumping = false;
            JumpTime = MaxJumpTime;
        }
    }
}
