using System;
using BEPUutilities;
using System.Diagnostics;
using BEPUphysics.Materials;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.PositionUpdating;
using BEPUphysics.UpdateableSystems;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.Character;
using BEPUphysics;

namespace Physics
{
    using System;
    using BEPUphysics;
    using BEPUphysics.Entities.Prefabs;
    using BEPUphysics.CollisionTests;
    using BEPUutilities;

    public class CustomCharacterController
    {
        public Capsule Body { get; }
        private readonly Space space;

        // Movement Parameters
        public float TargetSpeed = 8.0f;
        public float Acceleration = 40.0f;
        public float MaxSlopeAngle = MathHelper.ToRadians(45f);
        public float MaxStepHeight = 0.4f;
        public float MaxStepDownDistance = 0.4f;
        public Vector3 Gravity = new Vector3(0, -19.6f, 0);

        public bool IsGrounded { get; private set; }
        public Vector3 GroundNormal { get; private set; }

        public CustomCharacterController(Space space, Vector3 startPosition, float radius, float height, float mass)
        {
            this.space = space;

            // Use a standard dynamic Capsule Entity
            Body = new Capsule(startPosition, height, radius, mass);

            // Lock rotation so the capsule stays strictly upright
            Body.LocalInertiaTensorInverse = new Matrix3x3();

            // Disable BEPU's default continuous gravity integration (we manage Y-velocity manually)
            Body.Gravity = Vector3.Zero;

            // Prevent body deactivation/sleeping while active
            Body.ActivityInformation.IsAlwaysActive = true;

            space.Add(Body);
        }

        public void Update(Vector2 inputDirection, Vector3 cameraForward, Vector3 cameraRight, float dt)
        {
            // 1. EVALUATE GROUND SUPPORT & STEP SNAPPING
            EvaluateGroundAndSnap(dt);

            // 2. CONSTRUCT INPUT MOVEMENT VECTOR
            Vector3 desiredDir = (cameraRight * inputDirection.X) + (cameraForward * inputDirection.Y);
            desiredDir.Y = 0;
            if (desiredDir.LengthSquared() > 0.001f)
                desiredDir.Normalize();

            Vector3 currentVel = Body.LinearVelocity;

            if (IsGrounded)
            {
                // 3. PROJECT INPUT VECTOR ALONG SLOPE TANGENT
                // Removes velocity components pointing straight into the ground surface
                Vector3 slopeDir = desiredDir - Vector3.Dot(desiredDir, GroundNormal) * GroundNormal;
                if (slopeDir.LengthSquared() > 0.001f)
                    slopeDir.Normalize();

                Vector3 targetVel = slopeDir * (TargetSpeed * inputDirection.Length());

                // 4. ACCELERATE HORIZONTAL / SLOPE VELOCITY
                Vector3 newVel = Vector3.Lerp(currentVel, targetVel, Math.Min(1.0f, Acceleration * dt));

                // Glue Y-velocity to slope unless jumping
                Body.LinearVelocity = newVel;
            }
            else
            {
                // 5. AIRBORNE / FALLING STATE (Apply Gravity)
                Vector3 horizontalVel = new Vector3(currentVel.X, 0, currentVel.Z);
                Vector3 targetHorizontal = desiredDir * (TargetSpeed * inputDirection.Length());

                // Reduced air control blending
                Vector3 blendedHorizontal = Vector3.Lerp(horizontalVel, targetHorizontal, Math.Min(1.0f, (Acceleration * 0.2f) * dt));

                float newVelY = currentVel.Y + (Gravity.Y * dt);
                Body.LinearVelocity = new Vector3(blendedHorizontal.X, newVelY, blendedHorizontal.Z);
            }
        }

        private void EvaluateGroundAndSnap(float dt)
        {
            IsGrounded = false;
            GroundNormal = Vector3.Up;

            Vector3 rayStart = Body.Position;
            float rayLength = (Body.Length * 0.5f) + Body.Radius + MaxStepDownDistance;

            Ray sweepRay = new Ray(rayStart, Vector3.Down);

            // Cast down to detect floor or downhill steps
            if (space.RayCast(sweepRay, rayLength, GroundFilter, out RayCastResult hit))
            {
                Vector3 normal = hit.HitData.Normal;

                // Check if slope angle is walkable
                float slopeAngle = (float)Math.Acos(MathHelper.Clamp(Vector3.Dot(normal, Vector3.Up), -1f, 1f));

                if (slopeAngle <= MaxSlopeAngle)
                {
                    IsGrounded = true;
                    GroundNormal = normal;

                    float currentDistanceToGround = hit.HitData.T - ((Body.Length * 0.5f) + Body.Radius);

                    // STEP UP / SNAP DOWN: Instantly correct position delta if within step tolerances
                    if (Math.Abs(currentDistanceToGround) > 0.001f && currentDistanceToGround <= MaxStepDownDistance)
                    {
                        // Direct position mutation bypasses force solvers for stair/slope tracking
                        Vector3 pos = Body.Position;
                        pos.Y -= currentDistanceToGround;
                        Body.Position = pos;
                    }
                }
            }
        }

        private bool GroundFilter(BroadPhaseEntry entry)
        {
            // Don't collide ground sweeps against the character capsule itself
            return entry != Body.CollisionInformation;
        }

        public void Jump(float jumpVelocity)
        {
            if (IsGrounded)
            {
                Vector3 vel = Body.LinearVelocity;
                vel.Y = jumpVelocity;
                Body.LinearVelocity = vel;
                IsGrounded = false;
            }
        }
    }
}
