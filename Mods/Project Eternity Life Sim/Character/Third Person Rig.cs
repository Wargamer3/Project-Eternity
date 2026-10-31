using System;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public enum BodyFacing
    {
        Travel,
        Camera,
        Hold,
    }

    public struct MoveWish
    {
        public Vector2 Stick;
        public BodyFacing Facing;
        public float HoldYaw;
    }

    public struct GroundStep
    {
        public Vector3 Direction;
        public float BodyYaw;
        public bool FaceTravel;
    }

    /// <summary>
    /// Camera orbit and the ground wish. The stick never writes the yaw, so a later mantle can pin the body with Hold while the camera stays where the player left it.
    /// </summary>
    public class ThirdPersonRig
    {
        public const float DistanceMin = 5f;
        public const float DistanceMax = 100f;
        public const float MouseSensitivity = 0.005f;
        public const float ZoomSensitivity = 2f;

        public static readonly float PitchMin = MathHelper.ToRadians(5f);
        public static readonly float PitchMax = MathHelper.ToRadians(85f);

        public float Yaw;
        public float Pitch = MathHelper.ToRadians(25f);
        public float Distance = 100f;

        public Vector3 Forward
        {
            get
            {
                return new Vector3((float)Math.Sin(Yaw), (float)Math.Cos(Yaw), 0f);
            }
        }

        public Vector3 Right
        {
            get
            {
                return new Vector3((float)Math.Cos(Yaw), -(float)Math.Sin(Yaw), 0f);
            }
        }

        public void Orbit(float MouseX, float MouseY)
        {
            Yaw -= MouseX * MouseSensitivity;
            Pitch = MathHelper.Clamp(Pitch + MouseY * MouseSensitivity, PitchMin, PitchMax);
        }

        public void Zoom(float Notches)
        {
            Distance = MathHelper.Clamp(Distance - Notches * ZoomSensitivity, DistanceMin, DistanceMax);
        }

        public Vector3 Offset()
        {
            return Offset(Pitch, Distance);
        }

        public Vector3 Offset(float PitchAngle, float FromDistance)
        {
            double Horizontal = FromDistance * Math.Cos(PitchAngle);
            return new Vector3(
                (float)(-Horizontal * Math.Sin(Yaw)),
                (float)(FromDistance * Math.Sin(PitchAngle)),
                (float)(-Horizontal * Math.Cos(Yaw)));
        }

        public GroundStep Plan(MoveWish Wish, float CurrentYaw)
        {
            Vector3 Direction = Right * Wish.Stick.X + Forward * Wish.Stick.Y;
            if (Direction.LengthSquared() > 0.0000001f)
            {
                Direction.Normalize();
            }
            else
            {
                Direction = Vector3.Zero;
            }

            GroundStep Step = new GroundStep();
            Step.Direction = Direction;
            if (Wish.Facing == BodyFacing.Camera)
            {
                Step.BodyYaw = Yaw;
                Step.FaceTravel = false;
            }
            else if (Wish.Facing == BodyFacing.Hold)
            {
                Step.BodyYaw = Wish.HoldYaw;
                Step.FaceTravel = false;
            }
            else if (Direction != Vector3.Zero)
            {
                Step.BodyYaw = (float)Math.Atan2(Direction.X, Direction.Y);
                Step.FaceTravel = true;
            }
            else
            {
                Step.BodyYaw = CurrentYaw;
                Step.FaceTravel = true;
            }

            return Step;
        }

        public static float YawOf(Matrix Rotation)
        {
            return (float)Math.Atan2(Rotation.M31, Rotation.M33);
        }
    }
}
