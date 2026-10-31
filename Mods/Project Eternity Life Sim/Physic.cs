using System;
using System.Collections.Generic;
using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Constraints;
using BepuUtilities;
using BepuUtilities.Memory;
using Physics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    /// <summary>
    /// Bepu 2 world for characters and placed solids. Map triangles are the floor.
    /// </summary>
    public class PhysicsManager
    {
        private const float TimeStep = 1f / 60f;

        private const float CharacterRadius = 8f;
        private const float CharacterLength = 40f;
        private const float CharacterMass = 20f;
        private const float CharacterMargin = 1f;
        private const float CharacterCenter = CharacterLength * 0.5f;

        private BufferPool Pool;
        private Simulation PhysicSimulation;
        private CharacterControllers Characters;
        private readonly Dictionary<PlayerCharacter, BodyHandle> CharacterBodies = new Dictionary<PlayerCharacter, BodyHandle>();
        private readonly Dictionary<PlayerCharacter, Vector2> Commanded = new Dictionary<PlayerCharacter, Vector2>();
        private readonly HashSet<PlayerCharacter> Steered = new HashSet<PlayerCharacter>();
        private StaticHandle[] MapMeshHandles;
        private int MapMeshCount;
        private StaticHandle[] SolidHandles;
        private int SolidCount;
        private int SolidStamp;
        private bool IsActive;
        private readonly Dictionary<MapInfo, List<StaticHandle>> ChunkMeshes = new Dictionary<MapInfo, List<StaticHandle>>();
        private readonly Dictionary<MapInfo, List<StaticHandle>> ChunkSolids = new Dictionary<MapInfo, List<StaticHandle>>();
        private readonly Dictionary<MapInfo, int> ChunkSolidStamp = new Dictionary<MapInfo, int>();

        public void Initialize()
        {
            Start();
        }

        public void ClearMapMeshes()
        {
            if (!IsActive)
            {
                return;
            }

            for (int Index = 0; Index < MapMeshCount; Index++)
            {
                StaticHandle Handle = MapMeshHandles[Index];
                TypedIndex ShapeIndex = PhysicSimulation.Statics.GetDirectReference(Handle).Shape;
                PhysicSimulation.Statics.Remove(Handle);
                PhysicSimulation.Shapes.RemoveAndDispose(ShapeIndex, Pool);
            }

            MapMeshCount = 0;
        }

        public void AddMapMesh(Microsoft.Xna.Framework.Vector3[] Vertices, int[] Indices)
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
            StaticHandle Handle = PhysicSimulation.Statics.Add(new StaticDescription(new Vector3(0f, 0f, 0f), PhysicSimulation.Shapes.Add(Shape)));
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

        public void ClearChunkMeshes(MapInfo Chunk)
        {
            if (Chunk == null || !ChunkMeshes.TryGetValue(Chunk, out List<StaticHandle> Handles))
            {
                return;
            }

            RemoveHandles(Handles);
            ChunkMeshes.Remove(Chunk);
        }

        public void AddChunkMesh(MapInfo Chunk, Microsoft.Xna.Framework.Vector3[] Vertices, int[] Indices, Microsoft.Xna.Framework.Vector3 Shift)
        {
            if (Chunk == null)
            {
                AddMapMesh(Vertices, Indices);
                return;
            }

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
                Vector3 A = new Vector3(PointA.X + Shift.X, PointA.Y + Shift.Y, PointA.Z + Shift.Z);
                Vector3 B = new Vector3(PointB.X + Shift.X, PointB.Y + Shift.Y, PointB.Z + Shift.Z);
                Vector3 C = new Vector3(PointC.X + Shift.X, PointC.Y + Shift.Y, PointC.Z + Shift.Z);
                Triangles[Index * 2] = new Triangle(A, B, C);
                Triangles[Index * 2 + 1] = new Triangle(A, C, B);
            }

            Mesh Shape = new Mesh(Triangles, new Vector3(1f, 1f, 1f), Pool);
            StaticHandle Handle = PhysicSimulation.Statics.Add(new StaticDescription(new Vector3(0f, 0f, 0f), PhysicSimulation.Shapes.Add(Shape)));
            if (!ChunkMeshes.TryGetValue(Chunk, out List<StaticHandle> Handles))
            {
                Handles = new List<StaticHandle>();
                ChunkMeshes.Add(Chunk, Handles);
            }

            Handles.Add(Handle);
        }

        public bool HoldsChunk(MapInfo Chunk)
        {
            return Chunk != null && ChunkMeshes.ContainsKey(Chunk);
        }

        public void DetachChunk(MapInfo Chunk)
        {
            ClearChunkMeshes(Chunk);
            ClearChunkSolids(Chunk);
        }

        public bool Drives(PlayerCharacter Who)
        {
            return Who != null && CharacterBodies.ContainsKey(Who);
        }

        public void Adopt(PlayerCharacter Who)
        {
            if (Who == null || CharacterBodies.ContainsKey(Who))
            {
                return;
            }

            if (!IsActive)
            {
                Start();
            }

            Cylinder Shape = new Cylinder(CharacterRadius, CharacterLength);
            TypedIndex ShapeIndex = PhysicSimulation.Shapes.Add(Shape);
            BodyHandle Handle = PhysicSimulation.Bodies.Add(BodyDescription.CreateDynamic(
                ToPhysics(Who),
                new BodyInertia { InverseMass = 1f / CharacterMass },
                new CollidableDescription(ShapeIndex, CharacterMargin, float.MaxValue, ContinuousDetection.Passive),
                -1f));
            ref CharacterController Character = ref Characters.AllocateCharacter(Handle);
            Character.LocalUp = new Vector3(0f, 1f, 0f);
            Character.ViewDirection = new Vector3(0f, 0f, 1f);
            Character.CosMaximumSlope = MathF.Cos(MathF.PI * 0.3f);
            Character.JumpVelocity = 110f;
            Character.MaximumHorizontalForce = 3000f;
            Character.MaximumVerticalForce = 8000f;
            Character.MinimumSupportDepth = CharacterRadius * -0.01f;
            Character.MinimumSupportContinuationDepth = -CharacterMargin;
            CharacterBodies.Add(Who, Handle);
        }

        public void Release(PlayerCharacter Who)
        {
            if (Who == null || !IsActive || !CharacterBodies.TryGetValue(Who, out BodyHandle Handle))
            {
                return;
            }

            CharacterBodies.Remove(Who);
            TypedIndex Shape = PhysicSimulation.Bodies[Handle].Collidable.Shape;
            PhysicSimulation.Shapes.Remove(Shape);
            PhysicSimulation.Bodies.Remove(Handle);
            Characters.RemoveCharacterByBodyHandle(Handle);
        }

        public void Steer(PlayerCharacter Who, Microsoft.Xna.Framework.Vector3 MapDirection, float Speed)
        {
            if (!IsActive || !CharacterBodies.TryGetValue(Who, out BodyHandle Handle))
            {
                return;
            }

            ref CharacterController Character = ref Characters.GetCharacterByBodyHandle(Handle);
            BodyReference Body = PhysicSimulation.Bodies[Handle];
            Vector3 View = new Vector3(MapDirection.X, 0f, MapDirection.Y);
            float LengthSquared = View.LengthSquared();
            Vector2 Target = Vector2.Zero;
            if (LengthSquared > 1e-8f && Speed > 0f)
            {
                View /= MathF.Sqrt(LengthSquared);
                Target = new Vector2(0f, Speed);
                Character.ViewDirection = View;
            }

            Commanded[Who] = Target;
            Steered.Add(Who);
            if (!Body.Awake && Target != Character.TargetVelocity)
            {
                PhysicSimulation.Awakener.AwakenBody(Handle);
            }

            Character.TargetVelocity = Target;
            if (!Character.Supported && LengthSquared > 1e-8f && Speed > 0f)
            {
                ApplyAirControl(ref Character, Body, View, Speed);
            }
        }

        public float HorizontalSpeed(PlayerCharacter Who)
        {
            if (!IsActive || Who == null || !CharacterBodies.TryGetValue(Who, out BodyHandle Handle))
            {
                return 0f;
            }

            Vector3 Linear = PhysicSimulation.Bodies[Handle].Velocity.Linear;
            return MathF.Sqrt(Linear.X * Linear.X + Linear.Z * Linear.Z);
        }

        public bool Jump(PlayerCharacter Who)
        {
            if (!IsActive || !CharacterBodies.TryGetValue(Who, out BodyHandle Handle))
            {
                return false;
            }

            ref CharacterController Character = ref Characters.GetCharacterByBodyHandle(Handle);
            BodyReference Body = PhysicSimulation.Bodies[Handle];
            float UpSpeed = Vector3.Dot(Body.Velocity.Linear, Vector3.UnitY);
            if (!Character.Supported && UpSpeed > 4f)
            {
                return false;
            }

            PhysicSimulation.Awakener.AwakenBody(Handle);
            Character.TryJump = true;
            return true;
        }

        public void Teleport(PlayerCharacter Who)
        {
            if (!IsActive || Who == null || !CharacterBodies.TryGetValue(Who, out BodyHandle Handle))
            {
                return;
            }

            BodyReference Body = PhysicSimulation.Bodies[Handle];
            Body.Pose.Position = ToPhysics(Who);
            Body.Velocity.Linear = Vector3.Zero;
            PhysicSimulation.Awakener.AwakenBody(Handle);
        }

        public void SyncSolids(IList<FurnitureItem> Furniture, IList<BuildingItem> Buildings)
        {
            int Stamp = Fingerprint(Furniture, Buildings);
            if (Stamp == SolidStamp)
            {
                return;
            }

            SolidStamp = Stamp;
            ReplaceSolids(Blocking(Furniture, Buildings));
        }

        public void SyncChunkSolids(MapInfo Chunk, IList<FurnitureItem> Furniture, IList<BuildingItem> Buildings)
        {
            if (Chunk == null)
            {
                SyncSolids(Furniture, Buildings);
                return;
            }

            int Stamp = Fingerprint(Furniture, Buildings);
            if (Chunk.Scatter != null)
            {
                Stamp = Stamp * 31 + Chunk.Scatter.Count;
                Stamp = Stamp * 31 + PictureOptions.Trees;
            }

            if (ChunkSolidStamp.TryGetValue(Chunk, out int Known) && Known == Stamp)
            {
                return;
            }

            ChunkSolidStamp[Chunk] = Stamp;
            ClearChunkSolids(Chunk);
            ChunkSolidStamp[Chunk] = Stamp;
            if (!IsActive)
            {
                Start();
            }

            Microsoft.Xna.Framework.Vector3 Shift = ChunkOrigin.Shift(Chunk);
            List<StaticHandle> Handles = new List<StaticHandle>();
            List<BoxVolume> Solids = Blocking(Furniture, Buildings);
            Hulls(Buildings, Solids);
            ForestScatter.Collect(Chunk, Solids);
            for (int Index = 0; Index < Solids.Count; Index++)
            {
                BoxVolume Placed = Solids[Index];
                if (Placed.Width <= 0.01f || Placed.Height <= 0.01f || Placed.Length <= 0.01f)
                {
                    continue;
                }

                Box Shape = new Box(Placed.Width, Placed.Height, Placed.Length);
                Vector3 Center = new Vector3(Placed.Position.X + Shift.X, Placed.Position.Z + Placed.Height * 0.5f + Shift.Y, Placed.Position.Y + Shift.Z);
                Quaternion Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, -Placed.Angle);
                StaticHandle Handle = PhysicSimulation.Statics.Add(new StaticDescription(new RigidPose(Center, Rotation), PhysicSimulation.Shapes.Add(Shape)));
                Handles.Add(Handle);
            }

            ChunkSolids[Chunk] = Handles;
        }

        void ClearChunkSolids(MapInfo Chunk)
        {
            if (Chunk == null || !ChunkSolids.TryGetValue(Chunk, out List<StaticHandle> Handles))
            {
                ChunkSolidStamp.Remove(Chunk);
                return;
            }

            RemoveHandles(Handles);
            ChunkSolids.Remove(Chunk);
            ChunkSolidStamp.Remove(Chunk);
        }

        void RemoveHandles(List<StaticHandle> Handles)
        {
            if (!IsActive || Handles == null)
            {
                return;
            }

            for (int Index = 0; Index < Handles.Count; Index++)
            {
                StaticHandle Handle = Handles[Index];
                TypedIndex ShapeIndex = PhysicSimulation.Statics.GetDirectReference(Handle).Shape;
                PhysicSimulation.Statics.Remove(Handle);
                PhysicSimulation.Shapes.RemoveAndDispose(ShapeIndex, Pool);
            }
        }

        public void Update()
        {
            if (!IsActive)
            {
                return;
            }

            Drive();
            PhysicSimulation.Timestep(TimeStep);
            CarryCharacters();
            ReadCharacters();
        }

        private void Start()
        {
            if (IsActive)
            {
                return;
            }

            Pool = new BufferPool();
            Characters = new CharacterControllers(Pool);
            // Map tiles are about 32 units across, so gravity is scaled up from a meter-sized -10.
            PhysicSimulation = Simulation.Create(Pool, new CharacterNarrowphaseCallbacks(Characters), new GravityIntegrator(new Vector3(0f, -160f, 0f)), new SolveDescription(8, 1));
            IsActive = true;
        }

        static List<BoxVolume> Blocking(IList<FurnitureItem> Furniture, IList<BuildingItem> Buildings)
        {
            List<BoxVolume> Solids = new List<BoxVolume>();
            if (Furniture != null)
            {
                for (int i = 0; i < Furniture.Count; i++)
                {
                    FurnitureItem Box = Furniture[i];
                    if (Box != null && Box.BlocksBody())
                    {
                        Solids.Add(Box.Body);
                    }
                }
            }

            if (Buildings != null)
            {
                for (int i = 0; i < Buildings.Count; i++)
                {
                    BuildingItem Box = Buildings[i];
                    if (Box != null && Box.BlocksBody())
                    {
                        Solids.Add(Box.Body);
                    }
                }
            }

            return Solids;
        }

        static void Hulls(IList<BuildingItem> Buildings, List<BoxVolume> Solids)
        {
            if (Buildings == null || Solids == null)
            {
                return;
            }

            for (int i = 0; i < Buildings.Count; i++)
            {
                BuildingItem Box = Buildings[i];
                BoxVolume Hull;
                if (Box != null && Box.TryHull(out Hull))
                {
                    Solids.Add(Hull);
                }
            }
        }

        private void ReplaceSolids(IList<BoxVolume> Solids)
        {
            if (!IsActive)
            {
                Start();
            }

            ClearSolids();
            if (Solids == null)
            {
                return;
            }

            for (int Index = 0; Index < Solids.Count; Index++)
            {
                BoxVolume Placed = Solids[Index];
                if (Placed.Width <= 0.01f || Placed.Height <= 0.01f || Placed.Length <= 0.01f)
                {
                    continue;
                }

                Box Shape = new Box(Placed.Width, Placed.Height, Placed.Length);
                Vector3 Center = new Vector3(Placed.Position.X, Placed.Position.Z + Placed.Height * 0.5f, Placed.Position.Y);
                Quaternion Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, -Placed.Angle);
                StaticHandle Handle = PhysicSimulation.Statics.Add(new StaticDescription(new RigidPose(Center, Rotation), PhysicSimulation.Shapes.Add(Shape)));
                if (SolidHandles == null || SolidCount == SolidHandles.Length)
                {
                    int NewLength = SolidHandles == null ? 8 : SolidHandles.Length * 2;
                    StaticHandle[] Grown = new StaticHandle[NewLength];
                    if (SolidHandles != null)
                    {
                        Array.Copy(SolidHandles, Grown, SolidCount);
                    }

                    SolidHandles = Grown;
                }

                SolidHandles[SolidCount] = Handle;
                SolidCount++;
            }
        }

        private void ClearSolids()
        {
            if (!IsActive)
            {
                return;
            }

            for (int Index = 0; Index < SolidCount; Index++)
            {
                StaticHandle Handle = SolidHandles[Index];
                TypedIndex ShapeIndex = PhysicSimulation.Statics.GetDirectReference(Handle).Shape;
                PhysicSimulation.Statics.Remove(Handle);
                PhysicSimulation.Shapes.RemoveAndDispose(ShapeIndex, Pool);
            }

            SolidCount = 0;
        }

        static void ApplyAirControl(ref CharacterController Character, BodyReference Body, Vector3 View, float Speed)
        {
            QuaternionEx.Transform(Character.LocalUp, Body.Pose.Orientation, out Vector3 Up);
            Vector3 Right = Vector3.Cross(View, Up);
            float RightLengthSquared = Right.LengthSquared();
            if (RightLengthSquared <= 1e-10f)
            {
                return;
            }

            Right /= MathF.Sqrt(RightLengthSquared);
            Vector3 Forward = Vector3.Cross(Up, Right);
            float Current = Vector3.Dot(Body.Velocity.Linear, Forward);
            float AirAcceleration = Body.LocalInertia.InverseMass * Character.MaximumHorizontalForce * 0.2f * TimeStep;
            float MaximumAirSpeed = Speed * 0.2f;
            float Target = MathF.Min(Current + AirAcceleration, MaximumAirSpeed);
            float Change = MathF.Max(0f, Target - Current);
            Body.Velocity.Linear += Forward * Change;
        }

        void Drive()
        {
            Microsoft.Xna.Framework.Vector2 Gust = Climate.Now.Wind;
            bool Windy = Gust.LengthSquared() >= 0.04f;
            Vector2 Wind = Windy ? new Vector2(Gust.X, Gust.Y) : Vector2.Zero;
            foreach (KeyValuePair<PlayerCharacter, BodyHandle> Pair in CharacterBodies)
            {
                PlayerCharacter Who = Pair.Key;
                if (Who == null)
                {
                    continue;
                }

                Vector2 Base = Vector2.Zero;
                Vector2 Held;
                if (Steered.Contains(Who) && Commanded.TryGetValue(Who, out Held))
                {
                    Base = Held;
                }

                Vector2 Nudge = Vector2.Zero;
                if (Windy && !Climate.Roof(Who))
                {
                    ref CharacterController Character = ref Characters.GetCharacterByBodyHandle(Pair.Value);
                    Vector3 Forward = Character.ViewDirection;
                    Forward.Y = 0f;
                    if (Forward.LengthSquared() < 1e-4f)
                    {
                        Forward = new Vector3(0f, 0f, 1f);
                    }
                    else
                    {
                        Forward = Vector3.Normalize(Forward);
                    }

                    Vector3 Right = new Vector3(Forward.Z, 0f, -Forward.X);
                    float Along = Wind.X * Forward.X + Wind.Y * Forward.Z;
                    float Across = Wind.X * Right.X + Wind.Y * Right.Z;
                    Nudge = new Vector2(Across, Along) * 0.12f;
                    Character.TargetVelocity = Base + Nudge;
                }
                else
                {
                    ref CharacterController Character = ref Characters.GetCharacterByBodyHandle(Pair.Value);
                    Character.TargetVelocity = Base;
                }
            }

            Steered.Clear();
        }

        void CarryCharacters()
        {
            foreach (KeyValuePair<PlayerCharacter, BodyHandle> Pair in CharacterBodies)
            {
                PlayerCharacter Who = Pair.Key;
                if (Who == null)
                {
                    continue;
                }

                BodyReference Body = PhysicSimulation.Bodies[Pair.Value];
                Vector3 Center = Body.Pose.Position;
                MapInfo Map = ChunkCross.At(Center.X, Center.Z);
                if (Map == null && Who.SharedMapContex != null)
                {
                    Map = Who.SharedMapContex.CurrentMapInfo;
                }

                if (Map == null)
                {
                    continue;
                }

                Microsoft.Xna.Framework.Vector3 Shift = ChunkOrigin.Shift(Map);
                float Height = Center.Y - CharacterCenter;
                Microsoft.Xna.Framework.Vector3 Feet = new Microsoft.Xna.Framework.Vector3(Center.X - Shift.X, Center.Z - Shift.Z, Height);
                bool Inland = Map.Tracks != null && (Who.WasInWater || (Map.Water != null && Map.Water.Count > 0));
                bool Sea = Ocean.Covers(Map.Heights, Feet.X, Feet.Y);
                if (!Inland && !Sea)
                {
                    continue;
                }

                WaterShift Moved = Inland
                    ? WaterPlace.Move(Map.Water, Feet, WaterPlace.SwimBody, WaterPlace.CharacterDensity, Who.WasInWater, TimeStep, Map.Heights)
                    : new WaterShift();
                if (!Moved.InWater && Sea)
                {
                    Moved = Ocean.Drift(Feet, Shift, Map.Heights, TimeStep);
                }
                if (!Who.WasInWater && !Moved.InWater)
                {
                    continue;
                }

                if (Who.WasInWater && !Moved.InWater)
                {
                    Moved.Position.Z = HeightField.Feet(Map, Moved.Position.X, Moved.Position.Y);
                }

                float WorldX = Moved.Position.X + Shift.X;
                float WorldZ = Moved.Position.Y + Shift.Z;
                MapInfo Landed = ChunkCross.At(WorldX, WorldZ) ?? Map;
                if (!Landed.ViewReady)
                {
                    WorldStream.OpenView(Landed);
                }

                if (Who.SharedMapContex != null)
                {
                    ChunkCross.Settle(Who, WorldX, WorldZ, Moved.Position.Z);
                }
                else
                {
                    Who.Position = Moved.Position;
                }

                Who.WasInWater = Moved.InWater;
                float Above = Moved.InWater ? Moved.Surface - Moved.Position.Z : 0f;
                WaterPlace.Soak(ref Who.WetLine, ref Who.WetAmount, Moved.InWater, Above, TimeStep);
                Microsoft.Xna.Framework.Vector3 NotedShift = ChunkOrigin.Shift(Landed);
                Microsoft.Xna.Framework.Vector2 After = new Microsoft.Xna.Framework.Vector2(WorldX - NotedShift.X, WorldZ - NotedShift.Z);
                if (Landed.Tracks != null)
                {
                    Landed.Tracks.Note(Who, After, Moved.InWater, false, TimeStep, NotedShift);
                }

                Body.Pose.Position = ToPhysics(Who);
                Body.Velocity.Linear = new Vector3(Body.Velocity.Linear.X, 0f, Body.Velocity.Linear.Z);
                PhysicSimulation.Awakener.AwakenBody(Pair.Value);
            }
        }

        void ReadCharacters()
        {
            foreach (KeyValuePair<PlayerCharacter, BodyHandle> Pair in CharacterBodies)
            {
                BodyReference Body = PhysicSimulation.Bodies[Pair.Value];
                Vector3 Center = Body.Pose.Position;
                float WorldX = Center.X;
                float WorldY = Center.Z;
                float Height = Center.Y - CharacterCenter;
                MapInfo Under = ChunkCross.At(WorldX, WorldY);
                if (Under != null && !Under.ViewReady)
                {
                    WorldStream.OpenView(Under);
                }

                if (Under != null && Under.Heights != null && Under.MapMapContainer != null)
                {
                    float LocalX = WorldX - Under.MapMapContainer.OffsetX;
                    float LocalY = WorldY - Under.MapMapContainer.OffsetY;
                    float Ground = HeightField.Feet(Under, LocalX, LocalY);
                    if (Height < Ground - 1f && !WaterPlace.Over(Under, LocalX, LocalY))
                    {
                        Height = Ground + (Wading(Under, LocalX, LocalY, Ground) ? 0.2f : HeightField.StandClearance);
                        Center.Y = Height + CharacterCenter;
                        Body.Pose.Position = Center;
                        Vector3 Velocity = Body.Velocity.Linear;
                        if (Velocity.Y < 0f)
                        {
                            Velocity.Y = 0f;
                            Body.Velocity.Linear = Velocity;
                        }
                    }
                }
                if (Pair.Key.SharedMapContex == null)
                {
                    Pair.Key.Position = new Microsoft.Xna.Framework.Vector3(WorldX, WorldY, Height);
                    continue;
                }

                ChunkCross.Settle(Pair.Key, WorldX, WorldY, Height);
            }
        }

        static bool Wading(MapInfo Map, float LocalX, float LocalY, float Ground)
        {
            if (Map == null || Map.Water == null)
            {
                return false;
            }

            for (int i = 0; i < Map.Water.Count; i++)
            {
                WaterVolume Volume = Map.Water[i];
                float Surface;
                Microsoft.Xna.Framework.Vector2 Flow;
                if (Volume != null && Volume.Footprint(LocalX, LocalY, out Surface, out Flow) && Surface > Ground - 0.5f)
                {
                    return true;
                }
            }

            return false;
        }

        Vector3 ToPhysics(PlayerCharacter Who)
        {
            Microsoft.Xna.Framework.Vector3 Feet = Who.Position;
            Microsoft.Xna.Framework.Vector3 Shift = ChunkOrigin.Shift(Who.SharedMapContex == null ? null : Who.SharedMapContex.CurrentMapInfo);
            return new Vector3(Feet.X + Shift.X, Feet.Z + CharacterCenter, Feet.Y + Shift.Z);
        }

        static int Fingerprint(IList<FurnitureItem> Furniture, IList<BuildingItem> Buildings)
        {
            unchecked
            {
                int Hash = 17;
                Hash = StampFurniture(Hash, Furniture);
                Hash = StampBuildings(Hash, Buildings);
                return Hash;
            }
        }

        static int StampFurniture(int Hash, IList<FurnitureItem> Boxes)
        {
            if (Boxes == null)
            {
                return Hash;
            }

            Hash = Hash * 31 + Boxes.Count;
            for (int i = 0; i < Boxes.Count; i++)
            {
                FurnitureItem Box = Boxes[i];
                if (Box == null)
                {
                    continue;
                }

                Hash = StampVolume(Hash, Box.Body);
                Hash = Hash * 31 + (int)Box.Kind;
            }

            return Hash;
        }

        static int StampBuildings(int Hash, IList<BuildingItem> Boxes)
        {
            if (Boxes == null)
            {
                return Hash;
            }

            Hash = Hash * 31 + Boxes.Count;
            for (int i = 0; i < Boxes.Count; i++)
            {
                BuildingItem Box = Boxes[i];
                if (Box == null)
                {
                    continue;
                }

                Hash = StampVolume(Hash, Box.Body);
                Hash = Hash * 31 + (int)Box.Kind;
            }

            return Hash;
        }

        static int StampVolume(int Hash, BoxVolume Box)
        {
            Hash = Hash * 31 + Box.Position.GetHashCode();
            Hash = Hash * 31 + Box.Width.GetHashCode();
            Hash = Hash * 31 + Box.Height.GetHashCode();
            Hash = Hash * 31 + Box.Length.GetHashCode();
            return Hash * 31 + Box.Angle.GetHashCode();
        }

        private struct GravityIntegrator : IPoseIntegratorCallbacks
        {
            public Vector3 Gravity;
            private Vector3Wide GravityWideDt;

            public GravityIntegrator(Vector3 Gravity)
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
