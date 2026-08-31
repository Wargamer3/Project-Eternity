using System;
using Microsoft.Xna.Framework.Graphics;
using BEPUphysics;
using BEPUutilities;
using BEPUphysics.Entities;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysicsDrawer.Lines;
using BEPUphysicsDrawer.Models;
using BEPUutilities.Threading;
using ConversionHelper;
using BEPUphysics.Settings;
using BEPUphysics.CollisionTests.CollisionAlgorithms;
using BEPUphysics.Constraints;
using BEPUphysics.CollisionTests.CollisionAlgorithms.GJK;
using BEPUphysics.PositionUpdating;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class PhysicsManager
    {
        public readonly Space PhysicSimulation;
        private readonly ParallelLooper ParallelLooper;

        public ModelDrawer ModelDrawer;
        public LineDrawer ConstraintDrawer;
        public ContactDrawer ContactDrawer;
        public BoundingBoxDrawer BoundingBoxDrawer;
        public SimulationIslandDrawer SimulationIslandDrawer;
        public BasicEffect LineDrawer;

        private bool displayEntities;
        private bool displayConstraints;
        private bool displayContacts;
        private bool displayBoundingBoxes;
        private bool displaySimulationIslands;

        public PhysicsManager()
        {
            displayEntities = true;
            displayConstraints = true;
            displayContacts = true;
            displayBoundingBoxes = true;
            displaySimulationIslands = true;

            ParallelLooper = new ParallelLooper();
            //This section lets the engine know that it can make use of multithreaded systems
            //by adding threads to its thread pool.

            if (Environment.ProcessorCount > 1)
            {
                for (int i = 0; i < Environment.ProcessorCount; i++)
                {
                    ParallelLooper.AddThread();
                }
            }

            PhysicSimulation = new Space(ParallelLooper);
            PhysicSimulation.ForceUpdater.Gravity = new Vector3(0, -9.81f, 0f); //If left unset, the default value is (0,0,0).

            MotionSettings.DefaultPositionUpdateMode = PositionUpdateMode.Discrete;
            SolverSettings.DefaultMinimumIterationCount = 1;
            PhysicSimulation.Solver.IterationLimit = 10;
            GeneralConvexPairTester.UseSimplexCaching = false;
            MotionSettings.UseExtraExpansionForContinuousBoundingBoxes = false;

            //Set all the scaling settings back to their defaults.
            PhysicSimulation.DeactivationManager.VelocityLowerLimit = 0.26f;
            CollisionResponseSettings.MaximumPenetrationRecoverySpeed = 2;
            CollisionResponseSettings.BouncinessVelocityThreshold = 1;
            CollisionResponseSettings.StaticFrictionVelocityThreshold = .2f;
            CollisionDetectionSettings.ContactInvalidationLength = .1f;
            CollisionDetectionSettings.ContactMinimumSeparationDistance = .03f;
            CollisionDetectionSettings.MaximumContactDistance = .1f;
            CollisionDetectionSettings.DefaultMargin = .04f;
            CollisionDetectionSettings.AllowedPenetration = .01f;
            SolverSettings.DefaultMinimumImpulse = 0.001f;

            //Adjust epsilons back to defaults.
            Toolbox.Epsilon = 1e-7f;
            Toolbox.BigEpsilon = 1e-5f;
            MPRToolbox.DepthRefinementEpsilon = 1e-4f;
            MPRToolbox.RayCastSurfaceEpsilon = 1e-9f;
            MPRToolbox.SurfaceEpsilon = 1e-7f;
            PairSimplex.DistanceConvergenceEpsilon = 1e-7f;
            PairSimplex.ProgressionEpsilon = 1e-8f;
        }

        public void Initialize()
        {
            ModelDrawer = new InstancedModelDrawer(GameScreen.GraphicsDevice, GameScreen.ContentFallback);

            ConstraintDrawer = new LineDrawer(GameScreen.GraphicsDevice);

            ContactDrawer = new ContactDrawer();
            BoundingBoxDrawer = new BoundingBoxDrawer();
            SimulationIslandDrawer = new SimulationIslandDrawer();

            LineDrawer = new BasicEffect(GameScreen.GraphicsDevice);

            foreach (Entity ActiveEntity in PhysicSimulation.Entities)
            {
                if ((string)ActiveEntity.Tag != "noDisplayObject")
                {
                    ModelDrawer.Add(ActiveEntity);
                }
                else //Remove the now unnecessary tag.
                {
                    ActiveEntity.Tag = null;
                }
            }

            for (int i = 0; i < PhysicSimulation.Solver.SolverUpdateables.Count; i++)
            {
                //Add the solver updateable and match up the activity setting.
                LineDisplayObjectBase objectAdded = ConstraintDrawer.Add(PhysicSimulation.Solver.SolverUpdateables[i]);
                if (objectAdded != null)
                    objectAdded.IsDrawing = PhysicSimulation.Solver.SolverUpdateables[i].IsActive;
            }
            if (LifeSimMap.PhysicsManager.PhysicSimulation.Entities.Count == 0)
            {
                var Cube = new BEPUphysics.Entities.Prefabs.Box(new BEPUutilities.Vector3(32, 32, 32), 32f, 32f, 32f, 5);
                LifeSimMap.PhysicsManager.PhysicSimulation.Add(Cube);
            }
        }

        public void Update()
        {
            PhysicSimulation.Update();

            if (displayConstraints)
                ConstraintDrawer.Update();

            if (displayEntities)
                ModelDrawer.Update();
        }

        public void Draw(Microsoft.Xna.Framework.Matrix ViewMatrix, Microsoft.Xna.Framework.Matrix ProjectionMatrix)
        {
            if (displayEntities)
            {
                ModelDrawer.Draw(GameScreen.GraphicsDevice, ViewMatrix, ProjectionMatrix);
            }

            if (displayConstraints)
            {
                ConstraintDrawer.Draw(ViewMatrix, ProjectionMatrix);
            }

            LineDrawer.LightingEnabled = false;
            LineDrawer.VertexColorEnabled = true;
            LineDrawer.World = Microsoft.Xna.Framework.Matrix.Identity;
            LineDrawer.View = ViewMatrix;
            LineDrawer.Projection = ProjectionMatrix;

            if (displayContacts)
            {
                ContactDrawer.Draw(LineDrawer, PhysicSimulation);
            }

            if (displayBoundingBoxes)
            {
                BoundingBoxDrawer.Draw(LineDrawer, PhysicSimulation);
            }

            if (displaySimulationIslands)
            {
                SimulationIslandDrawer.Draw(LineDrawer, PhysicSimulation);
            }
        }
    }

    public class VaultScript
    {
        private Space _physicsSpace;

        // Configuration
        private float _vaultReach = 1.2f;        // How far forward in facing direction to sweep
        private float _vaultDuration = 0.35f;    // Seconds to complete the arc
        private float _vaultHeightOffset = 0.3f; // Height boost over the wall

        // State
        private bool _isVaulting;
        private float _vaultTimer;
        private Vector3 _startPos;
        private Vector3 _apexPos;
        private Vector3 _targetPos;

        BEPUphysics.Entities.Entity BEPUBody;

        public bool IsVaulting => _isVaulting;

        public void Initialize(Space physicsSpace)
        {
            _physicsSpace = physicsSpace;
        }

        public void Update(float deltaTime)
        {
            if (_isVaulting)
            {
                ProcessVaultInterpolation(deltaTime);
            }
        }

        /// <summary>
        /// Evaluates if the player can vault in the direction they are facing/moving.
        /// </summary>
        public bool TryVault(Vector3 inputDir)
        {
            if (_isVaulting) return false;

            // 1. EXTRACT FACING DIRECTION DIRECTLY FROM BEPU ORIENTATION
            Matrix3x3 orientation = BEPUBody.OrientationMatrix;
            Vector3 characterForward = orientation.Forward; // Vector pointing straight out of character's chest


            // Prefer movement input if actively pushing a key; otherwise fall back to facing direction
            Vector3 vaultDirection = inputDir.LengthSquared() > 0.01f ? inputDir : characterForward;
            vaultDirection.Normalize();

            // 3. SWEEP FORWARD ALONG THAT SPECIFIC VECTOR
            if (DetectVaultableWall(vaultDirection, out Vector3 wallHitPoint, out float wallHeight))
            {
                // Verify safe landing spot directly past the wall along the facing vector
                Vector3 estimatedLanding = wallHitPoint + (vaultDirection * 0.8f);
                if (CheckLandingZone(estimatedLanding, out Vector3 safeLandingPos))
                {
                    StartVaultSequence(safeLandingPos, wallHitPoint.Y + _vaultHeightOffset);
                    return true;
                }
            }

            return false;
        }

        private bool DetectVaultableWall(Vector3 facingDir, out Vector3 hitLocation, out float wallHeight)
        {
            hitLocation = Vector3.Zero;
            wallHeight = 0f;

            Vector3 charPos = BEPUBody.Position;

            // 0.4m wide x 0.3m tall box sweep volume
            var boxShape = new BoxShape(0.4f, 0.3f, 0.1f);

            Func<BroadPhaseEntry, bool> filter = candidate =>
                candidate != BEPUBody.CollisionInformation;

            // A. LOWER BOX (Waist Height ~0.7m) - Swept forward along facingDir
            RigidTransform lowerTransform = new RigidTransform(charPos + new Vector3(0, 0.7f, 0));
            bool hitLower = _physicsSpace.ConvexCast(boxShape, ref lowerTransform, ref facingDir, out RayCastResult lowerHit);

            // B. UPPER BOX (Head Height ~1.5m) - Swept forward along facingDir
            RigidTransform upperTransform = new RigidTransform(charPos + new Vector3(0, 1.5f, 0));
            bool hitUpper = _physicsSpace.ConvexCast(boxShape, ref upperTransform, ref facingDir, out RayCastResult upperHit);

            // Condition: Lower box hits solid barrier IN FRONT, upper box sweeps freely
            if (hitLower && !hitUpper)
            {
                hitLocation = lowerHit.HitData.Location;
                wallHeight = lowerHit.HitData.Location.Y - charPos.Y;
                return true;
            }

            return false;
        }

        private bool CheckLandingZone(Vector3 targetLanding, out Vector3 safeLandingPos)
        {
            safeLandingPos = targetLanding;

            // Cast downward from above landing target to find floor
            Ray downRay = new Ray(targetLanding + new Vector3(0, 1.0f, 0), new Vector3(0, -1f, 0));

            Func<BroadPhaseEntry, bool> filter = candidate =>
                candidate != BEPUBody.CollisionInformation;

            if (_physicsSpace.RayCast(downRay, 2.5f, filter, out RayCastResult hit))
            {
                safeLandingPos = hit.HitData.Location + new Vector3(0, 0.1f, 0);
                return true;
            }

            return false;

        }

        private void StartVaultSequence(Vector3 targetLandingPos, float apexY)
        {
            _isVaulting = true;
            _vaultTimer = 0f;

            _startPos = BEPUBody.Position;
            _targetPos = targetLandingPos;

            // Apex midpoint raised over the barrier
            _apexPos = Vector3.Lerp(_startPos, _targetPos, 0.5f);
            _apexPos.Y = Math.Max(_startPos.Y, _targetPos.Y) + _vaultHeightOffset;

            // Disable BEPU character controller during vault interpolation
            BEPUBody.LinearVelocity = Vector3.Zero;
        }

        private void ProcessVaultInterpolation(float deltaTime)
        {
            _vaultTimer += deltaTime;
            float t = Math.Min(_vaultTimer / _vaultDuration, 1.0f);

            // Smooth parabolic arc along the facing axis
            Vector3 currentPos = CalculateBezierPoint(t, _startPos, _apexPos, _targetPos);
            BEPUBody.Position = currentPos;

            // Re-enable BEPU physics on landing
            if (t >= 1.0f)
            {
                _isVaulting = false;
                BEPUBody.Position = _targetPos;
            }
        }

        private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1f - t;
            return (u * u * p0) + (2f * u * t * p1) + (t * t * p2);
        }
        public bool TryMantle(Vector3 inputDir)
        {
            if (_isVaulting) return false;

            Matrix3x3 orientation = BEPUBody.OrientationMatrix;
            Vector3 characterForward = orientation.Forward;

            Vector3 mantleDirection = inputDir.LengthSquared() > 0.01f ? inputDir : characterForward;
            mantleDirection.Normalize();

            // 1. Check for a tall ledge wall in front
            if (DetectMantleLedge(mantleDirection, out Vector3 wallHitPoint, out Vector3 ledgeTopPoint))
            {
                // 2. Ensure there is enough headroom to stand on top of the ledge
                if (HasHeadroom(ledgeTopPoint))
                {
                    // Landing position is ON TOP of the ledge, set inward from the edge
                    Vector3 finalStandingPos = ledgeTopPoint + (mantleDirection * 0.5f) + new Vector3(0, 0.1f, 0);

                    StartMantleSequence(finalStandingPos, ledgeTopPoint.Y);
                    return true;
                }
            }

            return false;
        }

        private bool DetectMantleLedge(Vector3 facingDir, out Vector3 wallHitPoint, out Vector3 ledgeTopPoint)
        {
            wallHitPoint = Vector3.Zero;
            ledgeTopPoint = Vector3.Zero;

            Vector3 charPos = BEPUBody.Position;
            var boxShape = new BoxShape(0.4f, 0.3f, 0.1f);

            Func<BroadPhaseEntry, bool> filter = candidate =>
                candidate != BEPUBody.CollisionInformation;

            // A. SWEEP FORWARD AT TALL WALL HEIGHT (~1.8m)
            RigidTransform chestTransform = new RigidTransform(charPos + new Vector3(0, 1.8f, 0));
            bool hitWall = _physicsSpace.ConvexCast(boxShape, ref chestTransform, ref facingDir, filter, out RayCastResult wallHit);

            if (hitWall)
            {
                wallHitPoint = wallHit.HitData.Location;

                // B. CAST DOWNWARD FROM ABOVE THE WALL TO FIND THE TOP LEDGE SURFACE
                Vector3 probeAboveLedge = wallHitPoint + (facingDir * 0.4f) + new Vector3(0, 1.0f, 0);
                Ray downRay = new Ray(probeAboveLedge, new Vector3(0, -1f, 0));

                if (_physicsSpace.RayCast(downRay, 2.0f, filter, out RayCastResult ledgeHit))
                {
                    // Verify the surface normal is pointing up (flat floor to stand on)
                    if (ledgeHit.HitData.Normal.Y > 0.7f) // ~45 degree max slope limit
                    {
                        ledgeTopPoint = ledgeHit.HitData.Location;
                        return true;
                    }
                }
            }

            return false;
        }

        private bool HasHeadroom(Vector3 ledgeTopPoint)
        {
            // Test if a character-sized box can fit on top of the ledge without hitting a ceiling
            var characterVolumeBox = new BoxShape(0.6f, 1.8f, 0.6f);
            RigidTransform headroomTransform = new RigidTransform(ledgeTopPoint + new Vector3(0, 1.0f, 0));

            Func<BroadPhaseEntry, bool> filter = candidate =>
                candidate != BEPUBody.CollisionInformation;

            Vector3 sweep = Vector3.Up;
            // Zero-distance convex query to check for overlapping geometry at the target standing spot
            return !_physicsSpace.ConvexCast(characterVolumeBox, ref headroomTransform, ref sweep, filter, out _);
        }

        private void StartMantleSequence(Vector3 targetStandingPos, float ledgeY)
        {
            _isVaulting = true; // Re-use active movement lockout state
            _vaultTimer = 0f;

            _startPos = BEPUBody.Position;
            _targetPos = targetStandingPos;

            // Mantle arc pulls UP onto the edge first, then FORWARD onto the surface
            _apexPos = new Vector3(_startPos.X, ledgeY + 0.2f, _startPos.Z) + (BEPUBody.OrientationMatrix.Forward * 0.2f);

            BEPUBody.LinearVelocity = Vector3.Zero;
        }
    }
}
