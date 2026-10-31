using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using ProjectEternity.Core.Graphics;
using SharpGLTF.Runtime;
using SharpGLTF.Schema2;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    // Hair, skirt, and chest chains lag behind the body. Positions are kept in world space
    // so a jump, stop, or turn moves the anchor while the chain catches up.
    public partial class BodySprings
    {
        const float MaxStep = 0.05f;
        const float SnapDistance = 48f;
        // Map motion is in 32-unit tiles. The chains keep that fraction, or a step throws them through the body.
        const float Tile = 32f;
        const int Passes = 4;
        const string WeightTip = "\u9318";
        const string Chest = "\u80F8";
        const string ChestAdjust = "\u304A\u3063\u3071\u3044";

        static readonly Regex Skirt = new Regex("^Q_\\d+_\\d+$", RegexOptions.CultureInvariant);
        static readonly Regex UpperCloth = new Regex("^WT_\\d+_\\d+$", RegexOptions.CultureInvariant);

        public Microsoft.Xna.Framework.Matrix World = Microsoft.Xna.Framework.Matrix.Identity;
        public float Seconds;
        public string SourcePath;

        bool Built;
        bool HasLastWorld;
        Matrix4x4 LastWorld = Matrix4x4.Identity;
        Chain[] Chains = Array.Empty<Chain>();
        Collider[] Colliders = Array.Empty<Collider>();
        Collider[][] Shapes = Array.Empty<Collider[]>();
        WorldCollider[] Obstacles = Array.Empty<WorldCollider>();
        int[] ParentOf = Array.Empty<int>();
        bool[] Posed = Array.Empty<bool>();

        public bool Apply(ArmatureInstance Armature)
        {
            if (Armature == null)
            {
                return false;
            }

            if (!Built)
            {
                Build(Armature);
                Built = true;
            }

            if (Chains.Length == 0)
            {
                return false;
            }

            return Step(Armature, ToNumerics(World), Seconds);
        }

        // The clip poses the skeleton. This moves the spring bones, and the skinned shader draws the mesh.
        public void Pose(GltfModel Model)
        {
            if (Model == null)
            {
                return;
            }

            ArmatureInstance Armature = Model.Armature;
            if (Armature == null)
            {
                return;
            }

            if (Model.ActiveAnimationName == null)
            {
                Armature.SetPoseTransforms();
            }

            Apply(Armature);
        }

        void CarryWithTheBody(Matrix4x4 WorldMatrix)
        {
            if (!HasLastWorld || !Matrix4x4.Invert(LastWorld, out Matrix4x4 InverseLast))
            {
                LastWorld = WorldMatrix;
                HasLastWorld = true;
                return;
            }

            Matrix4x4 Delta = InverseLast * WorldMatrix;
            float Follow = 1f - 1f / Tile;
            for (int Index = 0; Index < Chains.Length; Index++)
            {
                Chain Strand = Chains[Index];
                if (!Strand.Ready)
                {
                    continue;
                }

                for (int Bone = 0; Bone < Strand.Bones.Length; Bone++)
                {
                    Strand.Current[Bone] = Vector3.Lerp(Strand.Current[Bone], Vector3.Transform(Strand.Current[Bone], Delta), Follow);
                    Strand.Previous[Bone] = Vector3.Lerp(Strand.Previous[Bone], Vector3.Transform(Strand.Previous[Bone], Delta), Follow);
                }
            }

            LastWorld = WorldMatrix;
        }

        static Matrix4x4 ToNumerics(Microsoft.Xna.Framework.Matrix World)
        {
            return new Matrix4x4(
                World.M11, World.M12, World.M13, World.M14,
                World.M21, World.M22, World.M23, World.M24,
                World.M31, World.M32, World.M33, World.M34,
                World.M41, World.M42, World.M43, World.M44);
        }

        void Build(ArmatureInstance Armature)
        {
            IReadOnlyList<NodeInstance> Nodes = Armature.LogicalNodes;
            int Count = Nodes.Count;
            var IndexOf = new Dictionary<NodeInstance, int>(Count);
            var Children = new List<int>[Count];
            ParentOf = new int[Count];
            Posed = new bool[Count];
            for (int Index = 0; Index < Count; Index++)
            {
                IndexOf[Nodes[Index]] = Index;
                Children[Index] = new List<int>();
                ParentOf[Index] = -1;
            }

            for (int Index = 0; Index < Count; Index++)
            {
                NodeInstance Parent = Nodes[Index].VisualParent;
                if (Parent != null && IndexOf.TryGetValue(Parent, out int ParentIndex))
                {
                    ParentOf[Index] = ParentIndex;
                    Children[ParentIndex].Add(Index);
                }
            }

            bool[] Dynamic = new bool[Count];
            for (int Index = 0; Index < Count; Index++)
            {
                Dynamic[Index] = IsNamedDynamic(Nodes[Index].Name);
            }

            for (int Index = Count - 1; Index >= 0; Index--)
            {
                string Name = Nodes[Index].Name;
                if (Dynamic[Index] || string.IsNullOrEmpty(Name) || !Name.StartsWith("Ctr_", StringComparison.Ordinal))
                {
                    continue;
                }

                List<int> Kids = Children[Index];
                for (int Child = 0; Child < Kids.Count; Child++)
                {
                    if (Dynamic[Kids[Child]])
                    {
                        Dynamic[Index] = true;
                        break;
                    }
                }
            }

            var Found = new List<Chain>();
            for (int Index = 0; Index < Count; Index++)
            {
                if (!Dynamic[Index])
                {
                    continue;
                }

                int Parent = ParentOf[Index];
                if (Parent >= 0 && Dynamic[Parent])
                {
                    continue;
                }

                int[] Bones = LongestStrand(Index, Dynamic, Children);
                if (Bones.Length == 0 || string.IsNullOrEmpty(Nodes[Bones[0]].Name))
                {
                    continue;
                }

                Tune(Nodes[Bones[0]].Name, out float Stiffness, out float Damping, out float Gravity);
                Found.Add(new Chain(Parent, Bones, Stiffness, Damping, Gravity));
            }

            Chains = Found.ToArray();
            Colliders = LoadColliders(Nodes);
            Obstacles = new WorldCollider[Colliders.Length];
        }

        bool Step(ArmatureInstance Armature, Matrix4x4 WorldMatrix, float StepSeconds)
        {
            if (!Matrix4x4.Invert(WorldMatrix, out Matrix4x4 Inverse))
            {
                return false;
            }

            if (StepSeconds < 0f)
            {
                StepSeconds = 0f;
            }
            else if (StepSeconds > MaxStep)
            {
                StepSeconds = MaxStep;
            }

            CarryWithTheBody(WorldMatrix);

            IReadOnlyList<NodeInstance> Nodes = Armature.LogicalNodes;
            PlaceObstacles(Nodes, WorldMatrix);
            bool Write = false;
            for (int Index = 0; Index < Chains.Length; Index++)
            {
                Chain Strand = Chains[Index];
                Vector3 ParentModel = Strand.Parent >= 0
                    ? Nodes[Strand.Parent].ModelMatrix.Translation
                    : Nodes[Strand.Bones[0]].ModelMatrix.Translation;
                Vector3 Anchor = Vector3.Transform(ParentModel, WorldMatrix);
                for (int Bone = 0; Bone < Strand.Bones.Length; Bone++)
                {
                    Vector3 Model = Nodes[Strand.Bones[Bone]].ModelMatrix.Translation;
                    Strand.ModelPose[Bone] = Model;
                    Strand.WorldPose[Bone] = Vector3.Transform(Model, WorldMatrix);
                }

                bool Reset = !Strand.Ready || Vector3.Distance(Anchor, Strand.LastAnchor) > SnapDistance;
                if (Reset || StepSeconds <= 0.000001f)
                {
                    if (Reset)
                    {
                        for (int Bone = 0; Bone < Strand.Bones.Length; Bone++)
                        {
                            Strand.Current[Bone] = Strand.WorldPose[Bone];
                            Strand.Previous[Bone] = Strand.WorldPose[Bone];
                        }

                        Strand.LastAnchor = Anchor;
                        Strand.Ready = true;
                    }

                    Strand.Live = false;
                    continue;
                }

                float StepScale = StepSeconds * StepSeconds;
                for (int Bone = 0; Bone < Strand.Bones.Length; Bone++)
                {
                    Vector3 Offset = Strand.WorldPose[Bone] - Strand.Current[Bone];
                    Vector3 Accel = new Vector3(0f, -Strand.Gravity, 0f) + Offset * Strand.Stiffness;
                    Vector3 Next = Strand.Current[Bone] + (Strand.Current[Bone] - Strand.Previous[Bone]) * Strand.Damping + Accel * StepScale;
                    Strand.Previous[Bone] = Strand.Current[Bone];
                    Strand.Current[Bone] = Next;
                }

                for (int Pass = 0; Pass < Passes; Pass++)
                {
                    float RootLength = Vector3.Distance(Strand.WorldPose[0], Anchor);
                    Strand.Current[0] = KeepLength(Anchor, Strand.Current[0], RootLength);
                    for (int Bone = 1; Bone < Strand.Bones.Length; Bone++)
                    {
                        float Length = Vector3.Distance(Strand.WorldPose[Bone], Strand.WorldPose[Bone - 1]);
                        Strand.Current[Bone] = KeepLength(Strand.Current[Bone - 1], Strand.Current[Bone], Length);
                    }

                    for (int Bone = 0; Bone < Strand.Bones.Length; Bone++)
                    {
                        PushOut(ref Strand.Current[Bone], Strand.WorldPose[Bone], Strand.Bones[Bone], Nodes, WorldMatrix);
                    }
                }

                bool Finite = true;
                for (int Bone = 0; Bone < Strand.Bones.Length; Bone++)
                {
                    Vector3 Point = Strand.Current[Bone];
                    if (!float.IsFinite(Point.X) || !float.IsFinite(Point.Y) || !float.IsFinite(Point.Z))
                    {
                        Finite = false;
                        break;
                    }
                }

                if (!Finite)
                {
                    for (int Bone = 0; Bone < Strand.Bones.Length; Bone++)
                    {
                        Strand.Current[Bone] = Strand.WorldPose[Bone];
                        Strand.Previous[Bone] = Strand.WorldPose[Bone];
                    }

                    Strand.LastAnchor = Anchor;
                    Strand.Live = false;
                    continue;
                }

                Strand.LastAnchor = Anchor;
                Strand.AnchorModel = ParentModel;
                Strand.Live = true;
                Write = true;
            }

            if (!Write)
            {
                return false;
            }

            Array.Clear(Posed, 0, Posed.Length);
            for (int Index = 0; Index < Chains.Length; Index++)
            {
                Chain Strand = Chains[Index];
                if (!Strand.Live)
                {
                    continue;
                }

                WriteStrand(Armature, Nodes, Strand, Inverse);
                Strand.Live = false;
            }

            for (int Index = 0; Index < Posed.Length; Index++)
            {
                int Parent = ParentOf[Index];
                if (Parent < 0 || !Posed[Parent] || Posed[Index])
                {
                    continue;
                }

                string Name = Nodes[Index].Name;
                if (!string.IsNullOrEmpty(Name))
                {
                    Armature.SetLocalMatrix(Name, Nodes[Index].LocalMatrix);
                }

                Posed[Index] = true;
            }

            return true;
        }

        void WriteStrand(ArmatureInstance Armature, IReadOnlyList<NodeInstance> Nodes, Chain Strand, Matrix4x4 Inverse)
        {
            Vector3 PreviousModel = Strand.AnchorModel;
            Vector3 PreviousSim = Strand.AnchorModel;
            for (int Bone = 0; Bone < Strand.Bones.Length; Bone++)
            {
                int Node = Strand.Bones[Bone];
                Vector3 SimModel = Vector3.Transform(Strand.Current[Bone], Inverse);
                if (!Matrix4x4.Decompose(Nodes[Node].ModelMatrix, out Vector3 Scale, out Quaternion Rotation, out _))
                {
                    PreviousModel = Strand.ModelPose[Bone];
                    PreviousSim = SimModel;
                    continue;
                }

                Quaternion Swing = FromTo(Strand.ModelPose[Bone] - PreviousModel, SimModel - PreviousSim);
                Quaternion Turned = Quaternion.Normalize(Swing * Rotation);
                Matrix4x4 PosedMatrix = Matrix4x4.CreateScale(Scale) * Matrix4x4.CreateFromQuaternion(Turned) * Matrix4x4.CreateTranslation(SimModel);
                string Name = Nodes[Node].Name;
                if (!string.IsNullOrEmpty(Name))
                {
                    Armature.SetModelMatrix(Name, PosedMatrix);
                    Posed[Node] = true;
                }

                PreviousModel = Strand.ModelPose[Bone];
                PreviousSim = SimModel;
            }
        }

        static int[] LongestStrand(int Root, bool[] Dynamic, List<int>[] Children)
        {
            var Bones = new List<int>();
            int Current = Root;
            while (Current >= 0)
            {
                Bones.Add(Current);
                int Next = -1;
                int Best = 0;
                List<int> Kids = Children[Current];
                for (int Index = 0; Index < Kids.Count; Index++)
                {
                    int Child = Kids[Index];
                    if (!Dynamic[Child])
                    {
                        continue;
                    }

                    int Reach = 1 + Depth(Child, Dynamic, Children);
                    if (Reach > Best)
                    {
                        Best = Reach;
                        Next = Child;
                    }
                }

                Current = Next;
            }

            return Bones.ToArray();
        }

        static int Depth(int Node, bool[] Dynamic, List<int>[] Children)
        {
            int Best = 0;
            List<int> Kids = Children[Node];
            for (int Index = 0; Index < Kids.Count; Index++)
            {
                int Child = Kids[Index];
                if (!Dynamic[Child])
                {
                    continue;
                }

                int Reach = 1 + Depth(Child, Dynamic, Children);
                if (Reach > Best)
                {
                    Best = Reach;
                }
            }

            return Best;
        }

        static bool IsNamedDynamic(string Name)
        {
            if (string.IsNullOrEmpty(Name) || Name.Contains("_dummy_") || Name.Contains("_shadow_"))
            {
                return false;
            }

            if (Name.EndsWith(WeightTip, StringComparison.Ordinal) || Skirt.IsMatch(Name) || UpperCloth.IsMatch(Name))
            {
                return true;
            }

            return Name.Contains(Chest) && !Name.Contains(ChestAdjust);
        }

        static void Tune(string Name, out float Stiffness, out float Damping, out float Gravity)
        {
            if (Name.StartsWith("Q_", StringComparison.Ordinal) || Name.StartsWith("WT_", StringComparison.Ordinal))
            {
                Stiffness = 90f;
                Damping = 0.82f;
                Gravity = 30f;
            }
            else if (Name.Contains(Chest))
            {
                Stiffness = 90f;
                Damping = 0.72f;
                Gravity = 25f;
            }
            else
            {
                Stiffness = 70f;
                Damping = 0.9f;
                Gravity = 40f;
            }
        }

        static Vector3 KeepLength(Vector3 From, Vector3 To, float Length)
        {
            if (Length <= 0.00001f)
            {
                return From;
            }

            Vector3 Offset = To - From;
            float Current = Offset.Length();
            if (Current <= 0.00001f)
            {
                return From;
            }

            return From + Offset * (Length / Current);
        }

        static Quaternion FromTo(Vector3 From, Vector3 To)
        {
            if (From.LengthSquared() <= 0.0000001f || To.LengthSquared() <= 0.0000001f)
            {
                return Quaternion.Identity;
            }

            Vector3 A = Vector3.Normalize(From);
            Vector3 B = Vector3.Normalize(To);
            float Dot = Vector3.Dot(A, B);
            if (Dot > 0.9999f)
            {
                return Quaternion.Identity;
            }

            if (Dot < -0.9999f)
            {
                Vector3 Axis = Vector3.Cross(Vector3.UnitX, A);
                if (Axis.LengthSquared() < 0.000001f)
                {
                    Axis = Vector3.Cross(Vector3.UnitY, A);
                }

                return Quaternion.CreateFromAxisAngle(Vector3.Normalize(Axis), MathF.PI);
            }

            Vector3 Cross = Vector3.Cross(A, B);
            return Quaternion.Normalize(new Quaternion(Cross, 1f + Dot));
        }

        Collider[] LoadColliders(IReadOnlyList<NodeInstance> Nodes)
        {
            string Json = ReadColliderJson();
            if (string.IsNullOrEmpty(Json))
            {
                return Array.Empty<Collider>();
            }

            var Names = new Dictionary<string, int>(Nodes.Count, StringComparer.Ordinal);
            for (int Index = 0; Index < Nodes.Count; Index++)
            {
                string Name = Nodes[Index].Name;
                if (!string.IsNullOrEmpty(Name) && !Names.ContainsKey(Name))
                {
                    Names.Add(Name, Index);
                }
            }

            var Found = new List<Collider>();
            var Owned = new List<Collider>[Nodes.Count];
            try
            {
                using JsonDocument Document = JsonDocument.Parse(Json);
                if (Document.RootElement.ValueKind != JsonValueKind.Array)
                {
                    return Array.Empty<Collider>();
                }

                foreach (JsonElement Item in Document.RootElement.EnumerateArray())
                {
                    if (!Item.TryGetProperty("bone", out JsonElement BoneName) || BoneName.ValueKind != JsonValueKind.String)
                    {
                        continue;
                    }

                    string ColliderBone = BoneName.GetString();
                    if (!Names.TryGetValue(ColliderBone, out int Bone))
                    {
                        continue;
                    }

                    string Shape = Item.TryGetProperty("shape", out JsonElement ShapeName) && ShapeName.ValueKind == JsonValueKind.String
                        ? ShapeName.GetString()
                        : "sphere";
                    var ShapeCollider = new Collider
                    {
                        Bone = Bone,
                        Mode = ReadByte(Item, "mode"),
                        Group = ReadByte(Item, "group"),
                        Mask = (ushort)ReadInt(Item, "mask"),
                        Kind = ShapeKind(Shape),
                        Center = ReadVector(Item, "center"),
                        Axis = ReadVector(Item, "axis"),
                        Radius = ReadFloat(Item, "radius"),
                        HalfLength = ReadFloat(Item, "height") * 0.5f,
                        Half = ReadVector(Item, "half"),
                        Rotation = ReadRotation(Item),
                    };
                    if (ShapeCollider.Axis.LengthSquared() > 0.00000001f)
                    {
                        ShapeCollider.Axis = Vector3.Normalize(ShapeCollider.Axis);
                    }
                    else
                    {
                        ShapeCollider.Axis = Vector3.UnitY;
                    }

                    if (ShapeCollider.Kind == 2)
                    {
                        if (ShapeCollider.Half.X <= 0f && ShapeCollider.Half.Y <= 0f && ShapeCollider.Half.Z <= 0f)
                        {
                            continue;
                        }
                    }
                    else if (ShapeCollider.Radius <= 0f)
                    {
                        continue;
                    }

                    if (ShapeCollider.Mode == 0)
                    {
                        Found.Add(ShapeCollider);
                    }
                    else
                    {
                        if (Owned[Bone] == null)
                        {
                            Owned[Bone] = new List<Collider>();
                        }

                        Owned[Bone].Add(ShapeCollider);
                    }
                }
            }
            catch (JsonException)
            {
                Shapes = Array.Empty<Collider[]>();
                return Array.Empty<Collider>();
            }

            Shapes = new Collider[Nodes.Count][];
            for (int Index = 0; Index < Nodes.Count; Index++)
            {
                if (Owned[Index] != null)
                {
                    Shapes[Index] = Owned[Index].ToArray();
                }
            }

            return Found.ToArray();
        }

        string ReadColliderJson()
        {
            if (string.IsNullOrEmpty(SourcePath))
            {
                return null;
            }

            string Side = Path.ChangeExtension(SourcePath, ".physics.json");
            if (File.Exists(Side))
            {
                try
                {
                    return File.ReadAllText(Side);
                }
                catch (IOException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
            }

            if (!File.Exists(SourcePath))
            {
                return null;
            }

            try
            {
                ModelRoot Root = ModelRoot.Load(SourcePath);
                foreach (Node Node in Root.LogicalNodes)
                {
                    string Found = ColliderText(Node.Extras);
                    if (!string.IsNullOrEmpty(Found))
                    {
                        return Found;
                    }
                }
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (JsonException)
            {
            }
            catch (InvalidOperationException)
            {
            }
            catch (ArgumentException)
            {
            }

            return null;
        }

        static string ColliderText(JsonNode Extras)
        {
            if (Extras is not JsonObject Object || !Object.TryGetPropertyValue("mmd_colliders", out JsonNode Value) || Value == null)
            {
                return null;
            }

            if (Value is JsonValue Text && Text.TryGetValue(out string Raw))
            {
                return Raw;
            }

            return Value.ToJsonString();
        }

        void PlaceObstacles(IReadOnlyList<NodeInstance> Nodes, Matrix4x4 WorldMatrix)
        {
            for (int Index = 0; Index < Colliders.Length; Index++)
            {
                Collider Shape = Colliders[Index];
                Matrix4x4 Bone = Nodes[Shape.Bone].ModelMatrix;
                float Scale = ScaleOf(Bone) * ScaleOf(WorldMatrix);
                Vector3 Center = Vector3.Transform(Vector3.Transform(Shape.Center, Bone), WorldMatrix);
                var Placed = new WorldCollider
                {
                    Kind = Shape.Kind,
                    Bone = Shape.Bone,
                    Group = Shape.Group,
                    Mask = Shape.Mask,
                    Radius = Shape.Radius * Scale,
                };
                if (Shape.Kind == 1)
                {
                    Vector3 Axis = Vector3.TransformNormal(Vector3.TransformNormal(Shape.Axis, Bone), WorldMatrix);
                    float Length = Axis.Length();
                    Axis = Length > 0.00000001f ? Axis / Length : Vector3.UnitY;
                    float Half = Shape.HalfLength * Scale;
                    Placed.A = Center - Axis * Half;
                    Placed.B = Center + Axis * Half;
                }
                else if (Shape.Kind == 2)
                {
                    Matrix4x4 Rigid = Matrix4x4.CreateFromQuaternion(Shape.Rotation) * Matrix4x4.CreateTranslation(Shape.Center);
                    Placed.ToWorld = Rigid * Bone * WorldMatrix;
                    Placed.Valid = Matrix4x4.Invert(Placed.ToWorld, out Placed.Inverse);
                    Placed.Half = Shape.Half;
                }
                else
                {
                    Placed.A = Center;
                }

                Obstacles[Index] = Placed;
            }
        }

        void PushOut(ref Vector3 Point, Vector3 Pose, int Bone, IReadOnlyList<NodeInstance> Nodes, Matrix4x4 WorldMatrix)
        {
            if (Bone < 0 || Bone >= Shapes.Length || Shapes[Bone] == null)
            {
                return;
            }

            Matrix4x4 BoneMatrix = Nodes[Bone].ModelMatrix;
            if (!Matrix4x4.Decompose(BoneMatrix, out _, out Quaternion BoneRotation, out _)
                || !Matrix4x4.Decompose(WorldMatrix, out _, out Quaternion WorldRotation, out _))
            {
                return;
            }

            float Scale = ScaleOf(BoneMatrix) * ScaleOf(WorldMatrix);
            Collider[] Owned = Shapes[Bone];
            for (int Shape = 0; Shape < Owned.Length; Shape++)
            {
                Collider Proxy = Owned[Shape];
                Vector3 Offset = Vector3.TransformNormal(Vector3.TransformNormal(Proxy.Center, BoneMatrix), WorldMatrix);
                Vector3 Sim = Point + Offset;
                Vector3 PoseCenter = Pose + Offset;
                Quaternion Orient = Quaternion.Normalize(WorldRotation * BoneRotation * Proxy.Rotation);
                Vector3 Half = Proxy.Half * Scale;
                float Radius = Proxy.Radius * Scale;
                float ReachAlong = Proxy.Kind == 1 ? Proxy.HalfLength * Scale : 0f;
                Vector3 Axis = Vector3.TransformNormal(Vector3.TransformNormal(Proxy.Axis, BoneMatrix), WorldMatrix);
                if (Axis.LengthSquared() > 0.00000001f)
                {
                    Axis = Vector3.Normalize(Axis);
                }

                for (int Index = 0; Index < Obstacles.Length; Index++)
                {
                    WorldCollider Obstacle = Obstacles[Index];
                    if (Obstacle.Bone == Bone || !Hits(Proxy, Obstacle))
                    {
                        continue;
                    }

                    Vector3 Delta = Separate(Sim, PoseCenter, Proxy.Kind, Orient, Half, Radius, ReachAlong, Axis, Obstacle);
                    if (Delta.LengthSquared() <= 0f)
                    {
                        continue;
                    }

                    Sim += Delta;
                    Point += Delta;
                }
            }
        }

        static bool Hits(Collider Proxy, WorldCollider Obstacle)
        {
            int ProxyBit = 1 << (Proxy.Group & 15);
            int ObstacleBit = 1 << (Obstacle.Group & 15);
            return (Proxy.Mask & ObstacleBit) == 0 && (Obstacle.Mask & ProxyBit) == 0;
        }

        static Vector3 Separate(Vector3 Sim, Vector3 PoseCenter, byte Kind, Quaternion Orient, Vector3 Half, float Radius, float ReachAlong, Vector3 Axis, WorldCollider Obstacle, bool RestClears = false)
        {
            Vector3 Closest = Obstacle.Kind == 1 ? ClosestOnSegment(Sim, Obstacle.A, Obstacle.B) : Obstacle.A;
            Vector3 Out = Sim - Closest;
            float DistSq = Out.LengthSquared();
            Vector3 Direction = DistSq > 0.00000001f ? Out / MathF.Sqrt(DistSq) : PoseCenter - Closest;
            if (Direction.LengthSquared() < 0.00000001f)
            {
                Direction = Vector3.UnitY;
            }
            else
            {
                Direction = Vector3.Normalize(Direction);
            }

            float Reach = Obstacle.Radius + Support(Kind, Orient, Half, Radius, ReachAlong, Axis, Direction);
            if (DistSq >= Reach * Reach)
            {
                return Vector3.Zero;
            }

            Vector3 PoseClosest = Obstacle.Kind == 1 ? ClosestOnSegment(PoseCenter, Obstacle.A, Obstacle.B) : Obstacle.A;
            float BodyReach = RestClears ? Reach : Obstacle.Radius;
            if ((PoseCenter - PoseClosest).LengthSquared() < BodyReach * BodyReach)
            {
                return Vector3.Zero;
            }

            float Dist = MathF.Sqrt(DistSq);
            return Direction * (Reach - Dist);
        }

        static float Support(byte Kind, Quaternion Orient, Vector3 Half, float Radius, float ReachAlong, Vector3 Axis, Vector3 Direction)
        {
            if (Kind == 2)
            {
                Vector3 Local = Vector3.Transform(Direction, Quaternion.Conjugate(Orient));
                return Half.X * MathF.Abs(Local.X) + Half.Y * MathF.Abs(Local.Y) + Half.Z * MathF.Abs(Local.Z);
            }

            if (Kind == 1)
            {
                return Radius + ReachAlong * MathF.Abs(Vector3.Dot(Axis, Direction));
            }

            return Radius;
        }

        static Vector3 ClosestOnSegment(Vector3 Point, Vector3 A, Vector3 B)
        {
            Vector3 Segment = B - A;
            float LengthSq = Segment.LengthSquared();
            float Along = 0f;
            if (LengthSq > 0.00000001f)
            {
                Along = Vector3.Dot(Point - A, Segment) / LengthSq;
                if (Along < 0f)
                {
                    Along = 0f;
                }
                else if (Along > 1f)
                {
                    Along = 1f;
                }
            }

            return A + Segment * Along;
        }

        static float ScaleOf(Matrix4x4 Matrix)
        {
            float Length = MathF.Sqrt(Matrix.M11 * Matrix.M11 + Matrix.M12 * Matrix.M12 + Matrix.M13 * Matrix.M13);
            return Length > 0.00000001f ? Length : 1f;
        }

        static byte ShapeKind(string Shape)
        {
            if (string.Equals(Shape, "capsule", StringComparison.Ordinal))
            {
                return 1;
            }

            if (string.Equals(Shape, "box", StringComparison.Ordinal))
            {
                return 2;
            }

            return 0;
        }

        static byte ReadByte(JsonElement Item, string Name)
        {
            int Value = ReadInt(Item, Name);
            if (Value < 0)
            {
                return 0;
            }

            return Value > 255 ? (byte)255 : (byte)Value;
        }

        static int ReadInt(JsonElement Item, string Name)
        {
            if (!Item.TryGetProperty(Name, out JsonElement Value) || Value.ValueKind != JsonValueKind.Number)
            {
                return 0;
            }

            return Value.GetInt32();
        }

        static float ReadFloat(JsonElement Item, string Name)
        {
            if (!Item.TryGetProperty(Name, out JsonElement Value) || Value.ValueKind != JsonValueKind.Number)
            {
                return 0f;
            }

            return Value.GetSingle();
        }

        static Vector3 ReadVector(JsonElement Item, string Name)
        {
            if (!Item.TryGetProperty(Name, out JsonElement Value) || Value.ValueKind != JsonValueKind.Array || Value.GetArrayLength() < 3)
            {
                return Vector3.Zero;
            }

            return new Vector3(Value[0].GetSingle(), Value[1].GetSingle(), Value[2].GetSingle());
        }

        static Quaternion ReadRotation(JsonElement Item)
        {
            if (!Item.TryGetProperty("rotation", out JsonElement Value) || Value.ValueKind != JsonValueKind.Array || Value.GetArrayLength() < 4)
            {
                return Quaternion.Identity;
            }

            var Rotation = new Quaternion(Value[0].GetSingle(), Value[1].GetSingle(), Value[2].GetSingle(), Value[3].GetSingle());
            return Rotation.LengthSquared() > 0.00000001f ? Quaternion.Normalize(Rotation) : Quaternion.Identity;
        }

        struct Collider
        {
            public int Bone;
            public byte Kind;
            public byte Mode;
            public byte Group;
            public ushort Mask;
            public Vector3 Center;
            public Vector3 Axis;
            public float Radius;
            public float HalfLength;
            public Vector3 Half;
            public Quaternion Rotation;
        }

        struct WorldCollider
        {
            public byte Kind;
            public int Bone;
            public byte Group;
            public ushort Mask;
            public Vector3 A;
            public Vector3 B;
            public float Radius;
            public Vector3 Half;
            public Matrix4x4 ToWorld;
            public Matrix4x4 Inverse;
            public bool Valid;
        }

        sealed class Chain
        {
            public readonly int Parent;
            public readonly int[] Bones;
            public readonly float Stiffness;
            public readonly float Damping;
            public readonly float Gravity;
            public readonly Vector3[] ModelPose;
            public readonly Vector3[] WorldPose;
            public readonly Vector3[] Current;
            public readonly Vector3[] Previous;
            public Vector3 LastAnchor;
            public Vector3 AnchorModel;
            public bool Ready;
            public bool Live;

            public Chain(int Parent, int[] Bones, float Stiffness, float Damping, float Gravity)
            {
                this.Parent = Parent;
                this.Bones = Bones;
                this.Stiffness = Stiffness;
                this.Damping = Damping;
                this.Gravity = Gravity;
                ModelPose = new Vector3[Bones.Length];
                WorldPose = new Vector3[Bones.Length];
                Current = new Vector3[Bones.Length];
                Previous = new Vector3[Bones.Length];
            }
        }
    }
}
