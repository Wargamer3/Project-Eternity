using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Constraints;
using BepuUtilities;
using BepuUtilities.Memory;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    /// <summary>
    /// Life Sim moving-cube body on BepuPhysics 2. The map triangles are the floor.
    /// </summary>
    public sealed class LifeSimCubeSimulation
    {
        private const float TimeStep = 1f / 60f;

        private BufferPool Pool;
        private Simulation ActiveSimulation;
        private BodyHandle CubeHandle;
        private StaticHandle[] MapMeshHandles;
        private int MapMeshCount;
        private BasicEffect CubeEffect;
        private VertexBuffer CubeVertices;
        private IndexBuffer CubeIndices;

        public bool IsActive { get; private set; }

        public Microsoft.Xna.Framework.Vector3 Position { get; private set; }

        public Microsoft.Xna.Framework.Quaternion Orientation { get; private set; }

        public void Start()
        {
            if (IsActive)
            {
                return;
            }

            Pool = new BufferPool();
            ActiveSimulation = Simulation.Create(Pool, new CubeNarrowPhaseCallbacks(), new CubePoseIntegratorCallbacks(new Vector3(0f, -9.81f, 0f)), new SolveDescription(8, 1));

            // A 32 cube of mass 5 at (32, 32, 32). The map adds its triangles later as the floor.
            Box Shape = new Box(32f, 32f, 32f);
            BodyInertia Inertia = Shape.ComputeInertia(5f);
            CubeHandle = ActiveSimulation.Bodies.Add(BodyDescription.CreateDynamic(new Vector3(32f, 32f, 32f), Inertia, ActiveSimulation.Shapes.Add(Shape), 0.01f));
            IsActive = true;
            ReadPose();
        }

        public void ClearStaticTriangles()
        {
            if (!IsActive)
            {
                return;
            }

            for (int Index = 0; Index < MapMeshCount; Index++)
            {
                StaticHandle Handle = MapMeshHandles[Index];
                TypedIndex ShapeIndex = ActiveSimulation.Statics.GetDirectReference(Handle).Shape;
                ActiveSimulation.Statics.Remove(Handle);
                ActiveSimulation.Shapes.RemoveAndDispose(ShapeIndex, Pool);
            }

            MapMeshCount = 0;
        }

        public void AddStaticTriangles(Microsoft.Xna.Framework.Vector3[] Vertices, int[] Indices)
        {
            if (Vertices == null || Indices == null || Indices.Length < 3)
            {
                return;
            }

            if (!IsActive)
            {
                Start();
            }

            int SourceCount = Indices.Length / 3;
            Pool.Take(SourceCount * 2, out Buffer<Triangle> Triangles);
            for (int Index = 0; Index < SourceCount; Index++)
            {
                int First = Index * 3;
                Microsoft.Xna.Framework.Vector3 PointA = Vertices[Indices[First]];
                Microsoft.Xna.Framework.Vector3 PointB = Vertices[Indices[First + 1]];
                Microsoft.Xna.Framework.Vector3 PointC = Vertices[Indices[First + 2]];
                Vector3 A = new Vector3(PointA.X, PointA.Y, PointA.Z);
                Vector3 B = new Vector3(PointB.X, PointB.Y, PointB.Z);
                Vector3 C = new Vector3(PointC.X, PointC.Y, PointC.Z);
                // Bepu 2 triangles are one sided. The map mesh is stored double sided.
                Triangles[Index * 2] = new Triangle(A, B, C);
                Triangles[Index * 2 + 1] = new Triangle(A, C, B);
            }

            Mesh Shape = new Mesh(Triangles, new Vector3(1f, 1f, 1f), Pool);
            StaticHandle Handle = ActiveSimulation.Statics.Add(new StaticDescription(new Vector3(0f, 0f, 0f), ActiveSimulation.Shapes.Add(Shape)));
            if (MapMeshHandles == null || MapMeshCount == MapMeshHandles.Length)
            {
                int NewLength = MapMeshHandles == null ? 4 : MapMeshHandles.Length * 2;
                StaticHandle[] Grown = new StaticHandle[NewLength];
                if (MapMeshHandles != null)
                {
                    Array.Copy(MapMeshHandles, Grown, MapMeshCount);
                }

                MapMeshHandles = Grown;
            }

            MapMeshHandles[MapMeshCount] = Handle;
            MapMeshCount++;
        }

        public void Update()
        {
            if (!IsActive)
            {
                return;
            }

            ActiveSimulation.Timestep(TimeStep);
            ReadPose();
        }

        public void Draw(GraphicsDevice Device, Microsoft.Xna.Framework.Matrix View, Microsoft.Xna.Framework.Matrix Projection)
        {
            if (!IsActive || Device == null)
            {
                return;
            }

            EnsureCubeMesh(Device);
            CubeEffect.View = View;
            CubeEffect.Projection = Projection;
            CubeEffect.World = Microsoft.Xna.Framework.Matrix.CreateFromQuaternion(Orientation) * Microsoft.Xna.Framework.Matrix.CreateTranslation(Position);

            Device.SetVertexBuffer(CubeVertices);
            Device.Indices = CubeIndices;
            foreach (EffectPass Pass in CubeEffect.CurrentTechnique.Passes)
            {
                Pass.Apply();
                Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 12);
            }
        }

        private void ReadPose()
        {
            RigidPose Pose = ActiveSimulation.Bodies[CubeHandle].Pose;
            Position = new Microsoft.Xna.Framework.Vector3(Pose.Position.X, Pose.Position.Y, Pose.Position.Z);
            Orientation = new Microsoft.Xna.Framework.Quaternion(Pose.Orientation.X, Pose.Orientation.Y, Pose.Orientation.Z, Pose.Orientation.W);
        }

        private void EnsureCubeMesh(GraphicsDevice Device)
        {
            if (CubeVertices != null)
            {
                return;
            }

            const float Half = 16f;
            Microsoft.Xna.Framework.Vector3[] Corners = new Microsoft.Xna.Framework.Vector3[]
            {
                new Microsoft.Xna.Framework.Vector3(-Half, -Half, -Half),
                new Microsoft.Xna.Framework.Vector3(Half, -Half, -Half),
                new Microsoft.Xna.Framework.Vector3(Half, Half, -Half),
                new Microsoft.Xna.Framework.Vector3(-Half, Half, -Half),
                new Microsoft.Xna.Framework.Vector3(-Half, -Half, Half),
                new Microsoft.Xna.Framework.Vector3(Half, -Half, Half),
                new Microsoft.Xna.Framework.Vector3(Half, Half, Half),
                new Microsoft.Xna.Framework.Vector3(-Half, Half, Half)
            };
            Microsoft.Xna.Framework.Vector3[] Normals = new Microsoft.Xna.Framework.Vector3[]
            {
                Microsoft.Xna.Framework.Vector3.Backward,
                Microsoft.Xna.Framework.Vector3.Forward,
                Microsoft.Xna.Framework.Vector3.Left,
                Microsoft.Xna.Framework.Vector3.Right,
                Microsoft.Xna.Framework.Vector3.Down,
                Microsoft.Xna.Framework.Vector3.Up
            };
            int[] Faces = new int[]
            {
                0, 1, 2, 0, 2, 3,
                5, 4, 7, 5, 7, 6,
                4, 0, 3, 4, 3, 7,
                1, 5, 6, 1, 6, 2,
                4, 5, 1, 4, 1, 0,
                3, 2, 6, 3, 6, 7
            };

            VertexPositionNormalTexture[] Vertices = new VertexPositionNormalTexture[36];
            int[] Indices = new int[36];
            for (int Vertex = 0; Vertex < 36; Vertex++)
            {
                Vertices[Vertex] = new VertexPositionNormalTexture(Corners[Faces[Vertex]], Normals[Vertex / 6], Microsoft.Xna.Framework.Vector2.Zero);
                Indices[Vertex] = Vertex;
            }

            CubeVertices = new VertexBuffer(Device, typeof(VertexPositionNormalTexture), Vertices.Length, BufferUsage.WriteOnly);
            CubeVertices.SetData(Vertices);
            CubeIndices = new IndexBuffer(Device, IndexElementSize.ThirtyTwoBits, Indices.Length, BufferUsage.WriteOnly);
            CubeIndices.SetData(Indices);

            CubeEffect = new BasicEffect(Device);
            CubeEffect.LightingEnabled = true;
            CubeEffect.TextureEnabled = false;
            CubeEffect.VertexColorEnabled = false;
            CubeEffect.EnableDefaultLighting();
            CubeEffect.DiffuseColor = new Microsoft.Xna.Framework.Vector3(0.85f, 0.4f, 0.2f);
        }

        private struct CubeNarrowPhaseCallbacks : INarrowPhaseCallbacks
        {
            public void Initialize(Simulation Simulation)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool AllowContactGeneration(int WorkerIndex, CollidableReference A, CollidableReference B, ref float SpeculativeMargin)
            {
                return A.Mobility == CollidableMobility.Dynamic || B.Mobility == CollidableMobility.Dynamic;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool AllowContactGeneration(int WorkerIndex, CollidablePair Pair, int ChildIndexA, int ChildIndexB)
            {
                return true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool ConfigureContactManifold<TManifold>(int WorkerIndex, CollidablePair Pair, ref TManifold Manifold, out PairMaterialProperties PairMaterial) where TManifold : unmanaged, IContactManifold<TManifold>
            {
                PairMaterial.FrictionCoefficient = 1f;
                PairMaterial.MaximumRecoveryVelocity = 2f;
                PairMaterial.SpringSettings = new SpringSettings(30, 1);
                return true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool ConfigureContactManifold(int WorkerIndex, CollidablePair Pair, int ChildIndexA, int ChildIndexB, ref ConvexContactManifold Manifold)
            {
                return true;
            }

            public void Dispose()
            {
            }
        }

        private struct CubePoseIntegratorCallbacks : IPoseIntegratorCallbacks
        {
            public Vector3 Gravity;
            private Vector3Wide GravityWideDt;

            public CubePoseIntegratorCallbacks(Vector3 Gravity)
            {
                this.Gravity = Gravity;
                GravityWideDt = default;
            }

            public void Initialize(Simulation Simulation)
            {
            }

            public readonly AngularIntegrationMode AngularIntegrationMode => AngularIntegrationMode.Nonconserving;

            public readonly bool AllowSubstepsForUnconstrainedBodies => false;

            public readonly bool IntegrateVelocityForKinematics => false;

            public void PrepareForIntegration(float Dt)
            {
                GravityWideDt = Vector3Wide.Broadcast(Gravity * Dt);
            }

            public void IntegrateVelocity(System.Numerics.Vector<int> BodyIndices, Vector3Wide Position, QuaternionWide Orientation, BodyInertiaWide LocalInertia, System.Numerics.Vector<int> IntegrationMask, int WorkerIndex, System.Numerics.Vector<float> Dt, ref BodyVelocityWide Velocity)
            {
                Velocity.Linear += GravityWideDt;
            }
        }
    }
}
