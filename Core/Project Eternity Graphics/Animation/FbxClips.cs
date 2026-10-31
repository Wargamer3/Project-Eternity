using System;
using System.Collections.Generic;
using System.IO;
using SharpGLTF.Schema2;
using Matrix4x4 = System.Numerics.Matrix4x4;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;

namespace ProjectEternity.Core.Graphics
{
    /// <summary>
    /// Copies Mixamo FBX clips onto a glTF skeleton that uses the same bone names.
    /// A clip already stored in the glTF is left alone. Assimp splits each Mixamo bone into pivot nodes, and those pivots are folded back into the bone before the motion is applied.
    /// </summary>
    static class FbxClips
    {
        public static void Add(ModelRoot Root, IReadOnlyList<string> Files)
        {
            if (Root == null || Files == null || Files.Count == 0 || !HasHips(Root))
            {
                return;
            }

            if (Covers(Root, "Idle") && Covers(Root, "Walking"))
            {
                return;
            }

            try
            {
                FbxModel.EnsureNative();
            }
            catch (FileNotFoundException)
            {
                return;
            }
            catch (InvalidOperationException)
            {
                return;
            }

            foreach (string FilePath in Files)
            {
                if (string.IsNullOrWhiteSpace(FilePath) || !File.Exists(FilePath))
                {
                    continue;
                }

                string Name = Path.GetFileNameWithoutExtension(FilePath);
                if (string.IsNullOrWhiteSpace(Name) || Covers(Root, Name))
                {
                    continue;
                }

                try
                {
                    Import(Root, FilePath, Name);
                }
                catch (Assimp.AssimpException)
                {
                }
                catch (InvalidDataException)
                {
                }
                catch (IOException)
                {
                }
            }
        }

        static void Import(ModelRoot Root, string FilePath, string Name)
        {
            using (Assimp.AssimpContext Context = new Assimp.AssimpContext())
            {
                Assimp.Scene Source = Context.ImportFile(FilePath, Assimp.PostProcessSteps.None);
                if (Source == null || Source.RootNode == null || Source.AnimationCount == 0)
                {
                    return;
                }

                PoseTree Tree = PoseTree.Build(Source);
                List<Node> Bones = new List<Node>();
                foreach (Node Node in Root.LogicalNodes)
                {
                    if (!string.IsNullOrEmpty(Node.Name) && Tree.Has(Node.Name))
                    {
                        Bones.Add(Node);
                    }
                }

                if (Bones.Count < 8)
                {
                    return;
                }

                Animation Clip = Root.CreateAnimation(Name);
                Assimp.Animation SourceClip = Source.Animations[0];
                double Ticks = SourceClip.TicksPerSecond > 0 ? SourceClip.TicksPerSecond : 30;
                int LastFrame = Math.Max(1, (int)Math.Round(SourceClip.DurationInTicks));
                Dictionary<string, Matrix4x4> Rest = new Dictionary<string, Matrix4x4>();
                Tree.Locals(-1, Rest);

                Dictionary<Node, Dictionary<float, Quaternion>> Rotations = new Dictionary<Node, Dictionary<float, Quaternion>>();
                Dictionary<Node, Dictionary<float, Vector3>> Motions = new Dictionary<Node, Dictionary<float, Vector3>>();
                Dictionary<Node, Quaternion> BindRotation = new Dictionary<Node, Quaternion>();
                Dictionary<Node, Vector3> BindPosition = new Dictionary<Node, Vector3>();
                foreach (Node Bone in Bones)
                {
                    var Bind = Bone.LocalTransform.GetDecomposed();
                    BindRotation.Add(Bone, Bind.Rotation);
                    BindPosition.Add(Bone, Bind.Translation);
                    Rotations.Add(Bone, new Dictionary<float, Quaternion>());
                    if (Bone.Name.EndsWith(":Hips", StringComparison.Ordinal) || Bone.Name == "Hips")
                    {
                        Motions.Add(Bone, new Dictionary<float, Vector3>());
                    }
                }

                Dictionary<string, Matrix4x4> Posed = new Dictionary<string, Matrix4x4>();
                for (int Frame = 0; Frame <= LastFrame; Frame++)
                {
                    float Seconds = (float)(Frame / Ticks);
                    Tree.Locals(Seconds, Posed);
                    foreach (Node Bone in Bones)
                    {
                        if (!Rest.TryGetValue(Bone.Name, out Matrix4x4 SourceRest) || !Posed.TryGetValue(Bone.Name, out Matrix4x4 SourcePose))
                        {
                            continue;
                        }

                        Decompose(SourceRest, out Quaternion RestRotation, out Vector3 RestPosition);
                        Decompose(SourcePose, out Quaternion PoseRotation, out Vector3 PosePosition);
                        Quaternion Delta = Quaternion.Inverse(RestRotation) * PoseRotation;
                        Quaternion Target = Quaternion.Normalize(BindRotation[Bone] * Delta);
                        Rotations[Bone][Seconds] = Target;
                        if (!Motions.TryGetValue(Bone, out Dictionary<float, Vector3> Move))
                        {
                            continue;
                        }

                        float Unit = RestPosition.Length() > 0.001f ? BindPosition[Bone].Length() / RestPosition.Length() : 1f;
                        Quaternion Face = BindRotation[Bone] * Quaternion.Inverse(RestRotation);
                        Vector3 Shift = Vector3.Transform(PosePosition - RestPosition, Face) * Unit;
                        Move[Seconds] = BindPosition[Bone] + Shift;
                    }
                }

                foreach (Node Bone in Bones)
                {
                    if (Rotations[Bone].Count > 0)
                    {
                        Clip.CreateRotationChannel(Bone, Rotations[Bone]);
                    }

                    if (Motions.TryGetValue(Bone, out Dictionary<float, Vector3> Move) && Move.Count > 0)
                    {
                        Clip.CreateTranslationChannel(Bone, Move);
                    }
                }
            }
        }

        static bool HasHips(ModelRoot Root)
        {
            foreach (Node Node in Root.LogicalNodes)
            {
                if (Node.Name != null && (Node.Name == "mixamorig:Hips" || Node.Name.EndsWith(":Hips", StringComparison.Ordinal)))
                {
                    return true;
                }
            }

            return false;
        }

        static bool Covers(ModelRoot Root, string ClipName)
        {
            foreach (Animation Clip in Root.LogicalAnimations)
            {
                if (SameMotion(ClipName, Clip.Name))
                {
                    return true;
                }
            }

            return false;
        }

        static bool SameMotion(string Wanted, string Name)
        {
            if (string.IsNullOrEmpty(Wanted) || string.IsNullOrEmpty(Name))
            {
                return false;
            }

            if (string.Equals(Wanted, Name, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (Wanted.Equals("Idle", StringComparison.OrdinalIgnoreCase))
            {
                return Name.Contains("Idle", StringComparison.OrdinalIgnoreCase) && !Name.Contains("Walk", StringComparison.OrdinalIgnoreCase);
            }

            if (Wanted.Equals("Walking", StringComparison.OrdinalIgnoreCase) || Wanted.Equals("Walk", StringComparison.OrdinalIgnoreCase))
            {
                return Name.Contains("Walk", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        static void Decompose(Matrix4x4 Matrix, out Quaternion Rotation, out Vector3 Position)
        {
            Matrix4x4.Decompose(Matrix, out _, out Rotation, out Position);
            Rotation = Quaternion.Normalize(Rotation);
        }

        sealed class PoseTree
        {
            readonly Dictionary<string, PoseNode> Nodes = new Dictionary<string, PoseNode>();
            readonly List<PoseNode> Roots = new List<PoseNode>();

            public static PoseTree Build(Assimp.Scene Source)
            {
                PoseTree Tree = new PoseTree();
                Assimp.Animation Anim = Source.Animations[0];
                Tree.TicksPerSecond = Anim.TicksPerSecond > 0 ? Anim.TicksPerSecond : 30;
                Dictionary<string, Assimp.NodeAnimationChannel> Channels = new Dictionary<string, Assimp.NodeAnimationChannel>();
                foreach (Assimp.NodeAnimationChannel Channel in Anim.NodeAnimationChannels)
                {
                    if (!string.IsNullOrEmpty(Channel.NodeName) && !Channels.ContainsKey(Channel.NodeName))
                    {
                        Channels.Add(Channel.NodeName, Channel);
                    }
                }

                Tree.Add(null, Source.RootNode, Channels);
                return Tree;
            }

            double TicksPerSecond;

            public bool Has(string Name)
            {
                return Nodes.ContainsKey(Name);
            }

            public void Locals(float Seconds, Dictionary<string, Matrix4x4> Local)
            {
                Dictionary<PoseNode, Matrix4x4> Worlds = new Dictionary<PoseNode, Matrix4x4>();
                foreach (PoseNode Root in Roots)
                {
                    Walk(Root, Matrix4x4.Identity, Seconds, Worlds);
                }

                Local.Clear();
                foreach (KeyValuePair<string, PoseNode> Pair in Nodes)
                {
                    PoseNode Node = Pair.Value;
                    Matrix4x4 World = Worlds[Node];
                    Matrix4x4 Parent = Node.BoneParent == null ? Matrix4x4.Identity : Worlds[Node.BoneParent];
                    if (!Matrix4x4.Invert(Parent, out Matrix4x4 Inverse))
                    {
                        Inverse = Matrix4x4.Identity;
                    }

                    Local[Pair.Key] = World * Inverse;
                }
            }

            void Add(PoseNode Parent, Assimp.Node Source, Dictionary<string, Assimp.NodeAnimationChannel> Channels)
            {
                PoseNode Node = new PoseNode();
                Node.Name = Source.Name ?? string.Empty;
                Node.Rest = ToNumerics(Source.Transform);
                Node.Parent = Parent;
                if (Channels.TryGetValue(Node.Name, out Assimp.NodeAnimationChannel Channel))
                {
                    Node.Channel = Channel;
                }

                if (Parent == null)
                {
                    Roots.Add(Node);
                }
                else
                {
                    Parent.Children.Add(Node);
                }

                bool Pivot = Node.Name.Contains("_$AssimpFbx$_", StringComparison.Ordinal);
                if (!Pivot && Node.Name.Length > 0)
                {
                    Nodes[Node.Name] = Node;
                    PoseNode Bone = Parent;
                    while (Bone != null && (Bone.Name.Length == 0 || Bone.Name.Contains("_$AssimpFbx$_", StringComparison.Ordinal)))
                    {
                        Bone = Bone.Parent;
                    }

                    Node.BoneParent = Bone;
                }

                foreach (Assimp.Node Child in Source.Children)
                {
                    Add(Node, Child, Channels);
                }
            }

            void Walk(PoseNode Node, Matrix4x4 Parent, float Seconds, Dictionary<PoseNode, Matrix4x4> Worlds)
            {
                Matrix4x4 Local = Node.Rest;
                if (Seconds >= 0f && Node.Channel != null)
                {
                    Local = Sample(Node.Channel, Seconds * TicksPerSecond);
                }

                Matrix4x4 World = Local * Parent;
                Worlds[Node] = World;
                foreach (PoseNode Child in Node.Children)
                {
                    Walk(Child, World, Seconds, Worlds);
                }
            }

            static Matrix4x4 Sample(Assimp.NodeAnimationChannel Channel, double Time)
            {
                Vector3 Position = Channel.PositionKeyCount > 0 ? Lerp(Channel.PositionKeys, Time) : Vector3.Zero;
                Vector3 Scale = Channel.ScalingKeyCount > 0 ? Lerp(Channel.ScalingKeys, Time) : Vector3.One;
                Quaternion Rotation = Quaternion.Identity;
                if (Channel.RotationKeyCount > 0)
                {
                    Assimp.Quaternion Value = Slerp(Channel.RotationKeys, Time);
                    Rotation = Quaternion.Normalize(new Quaternion(Value.X, Value.Y, Value.Z, Value.W));
                }

                return Matrix4x4.CreateScale(Scale) * Matrix4x4.CreateFromQuaternion(Rotation) * Matrix4x4.CreateTranslation(Position);
            }

            static Vector3 Lerp(List<Assimp.VectorKey> Keys, double Time)
            {
                if (Keys.Count == 0)
                {
                    return Vector3.Zero;
                }

                if (Time <= Keys[0].Time)
                {
                    return new Vector3(Keys[0].Value.X, Keys[0].Value.Y, Keys[0].Value.Z);
                }

                for (int i = 0; i < Keys.Count - 1; i++)
                {
                    if (Keys[i + 1].Time >= Time)
                    {
                        double Span = Keys[i + 1].Time - Keys[i].Time;
                        float Amount = Span <= 0 ? 0f : (float)((Time - Keys[i].Time) / Span);
                        Vector3 A = new Vector3(Keys[i].Value.X, Keys[i].Value.Y, Keys[i].Value.Z);
                        Vector3 B = new Vector3(Keys[i + 1].Value.X, Keys[i + 1].Value.Y, Keys[i + 1].Value.Z);
                        return Vector3.Lerp(A, B, Amount);
                    }
                }

                Assimp.VectorKey Last = Keys[Keys.Count - 1];
                return new Vector3(Last.Value.X, Last.Value.Y, Last.Value.Z);
            }

            static Assimp.Quaternion Slerp(List<Assimp.QuaternionKey> Keys, double Time)
            {
                if (Time <= Keys[0].Time)
                {
                    return Keys[0].Value;
                }

                for (int i = 0; i < Keys.Count - 1; i++)
                {
                    if (Keys[i + 1].Time >= Time)
                    {
                        double Span = Keys[i + 1].Time - Keys[i].Time;
                        float Amount = Span <= 0 ? 0f : (float)((Time - Keys[i].Time) / Span);
                        return Assimp.Quaternion.Slerp(Keys[i].Value, Keys[i + 1].Value, Amount);
                    }
                }

                return Keys[Keys.Count - 1].Value;
            }

            static Matrix4x4 ToNumerics(Assimp.Matrix4x4 Source)
            {
                return new Matrix4x4(
                    Source.A1, Source.B1, Source.C1, Source.D1,
                    Source.A2, Source.B2, Source.C2, Source.D2,
                    Source.A3, Source.B3, Source.C3, Source.D3,
                    Source.A4, Source.B4, Source.C4, Source.D4);
            }

            sealed class PoseNode
            {
                public string Name;
                public Matrix4x4 Rest;
                public PoseNode Parent;
                public PoseNode BoneParent;
                public Assimp.NodeAnimationChannel Channel;
                public readonly List<PoseNode> Children = new List<PoseNode>();
            }
        }
    }
}
