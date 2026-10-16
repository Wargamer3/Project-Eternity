using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using SharpGLTF.Animations;
using SharpGLTF.Runtime;
using SharpGLTF.Schema2;
using SharpGLTF.Transforms;

namespace ProjectEternity.Core.Graphics
{
    /// <summary>
    /// Loads one glTF or GLB, including every animation clip in that file.
    /// UndoReflectZ mirrors the posed model on Z. Positions, UVs, and triangle order are stored as authored.
    /// A UVScroll clip offsets base-color UVs while a body clip plays.
    /// A material with no pointer track uses extras.rsm.uvScrollPerFrame instead.
    /// </summary>
    public sealed class GltfModel : IDisposable
    {
        private const float UvScrollFramesPerSecond = 60f;
        private static readonly Regex BaseColorOffsetPointer = new Regex(
            @"^/materials/(\d+)/pbrMetallicRoughness/baseColorTexture/extensions/KHR_texture_transform/offset$");

        private readonly bool UndoReflectZ;
        private readonly Matrix ReflectZ;
        private readonly SceneInstance Scene;
        private readonly float[] Durations;
        private readonly List<DrawableGroup> Groups = new List<DrawableGroup>();
        private readonly List<Texture2D> OwnedTextures = new List<Texture2D>();
        private Texture2D WhiteTexture;
        private readonly List<UvOffsetTrack> UvTracks = new List<UvOffsetTrack>();
        private readonly bool[] IsUvOnly = Array.Empty<bool>();
        private readonly Vector2[] ExtraUvPerSecond = Array.Empty<Vector2>();
        private int ActiveAnimationIndex = -1;
        private double AnimationTime;
        private double UvAnimationTime;
        private bool IsDisposed;

        public GltfModel(GraphicsDevice Device, string FilePath)
            : this(Device, FilePath, true)
        {
        }

        public GltfModel(GraphicsDevice Device, string FilePath, bool UndoReflectZ)
        {
            if (Device == null)
            {
                throw new ArgumentNullException(nameof(Device));
            }

            if (string.IsNullOrWhiteSpace(FilePath))
            {
                throw new ArgumentNullException(nameof(FilePath));
            }

            this.UndoReflectZ = UndoReflectZ;
            ReflectZ = UndoReflectZ ? Matrix.CreateScale(1f, 1f, -1f) : Matrix.Identity;

            ModelRoot Root = ModelRoot.Load(FilePath);
            Scene SourceScene = Root.DefaultScene ?? (Root.LogicalScenes.Count > 0 ? Root.LogicalScenes[0] : null);
            if (SourceScene == null)
            {
                throw new InvalidDataException("The glTF file has no scene.");
            }

            List<string> Names = new List<string>();
            Durations = new float[Root.LogicalAnimations.Count];
            for (int Index = 0; Index < Root.LogicalAnimations.Count; Index++)
            {
                Animation Clip = Root.LogicalAnimations[Index];
                string Name = Clip.Name;
                if (string.IsNullOrWhiteSpace(Name))
                {
                    Name = "Animation " + Index.ToString();
                }

                if (Names.Contains(Name))
                {
                    Name = Name + " (" + (Index + 1).ToString() + ")";
                }

                Names.Add(Name);
                Durations[Index] = Clip.Duration;
            }

            AnimationNames = Names;
            IsUvOnly = new bool[Root.LogicalAnimations.Count];
            LoadUvOffsetTracks(Root);
            ExtraUvPerSecond = ReadExtraUvPerSecond(Root);
            Scene = SceneTemplate.Create(SourceScene).CreateInstance();
            WhiteTexture = new Texture2D(Device, 1, 1);
            WhiteTexture.SetData(new[] { Color.White });
            OwnedTextures.Add(WhiteTexture);

            try
            {
                Dictionary<int, Texture2D> Textures = new Dictionary<int, Texture2D>();
                foreach (DrawableInstance Drawable in Scene)
                {
                    if (Drawable.InstanceCount != 1)
                    {
                        throw new NotSupportedException("Instanced glTF meshes are not loaded.");
                    }

                    bool Skinned = Drawable.Transform is SkinnedTransform;
                    if (!Skinned && !(Drawable.Transform is RigidTransform))
                    {
                        throw new NotSupportedException("The glTF transform " + Drawable.Transform.GetType().Name + " is not loaded.");
                    }

                    DrawableGroup Group = new DrawableGroup();
                    Group.Skinned = Skinned;
                    if (Skinned)
                    {
                        int BoneCount = ((SkinnedTransform)Drawable.Transform).SkinMatrices.Count;
                        if (BoneCount > SkinnedBoneEffect.MaxBones)
                        {
                            throw new InvalidDataException("A skinned mesh uses " + BoneCount.ToString() + " bones. The effect holds " + SkinnedBoneEffect.MaxBones.ToString() + ".");
                        }

                        Group.Bones = new Matrix[BoneCount];
                    }

                    Mesh SourceMesh = Root.LogicalMeshes[Drawable.Template.LogicalMeshIndex];
                    foreach (MeshPrimitive Primitive in SourceMesh.Primitives)
                    {
                        MeshPart Part = CreatePart(Device, Primitive, Skinned, Textures);
                        if (Part != null)
                        {
                            Group.Parts.Add(Part);
                        }
                    }

                    Groups.Add(Group);
                }
            }
            catch
            {
                Dispose();
                throw;
            }

            if (AnimationNames.Count > 0)
            {
                int DefaultClip = 0;
                for (int Index = 0; Index < IsUvOnly.Length; Index++)
                {
                    if (!IsUvOnly[Index])
                    {
                        DefaultClip = Index;
                        break;
                    }
                }

                PlayAnimation(AnimationNames[DefaultClip]);
            }
        }

        public IReadOnlyList<string> AnimationNames { get; }

        public string ActiveAnimationName
        {
            get
            {
                if (ActiveAnimationIndex < 0)
                {
                    return null;
                }

                return AnimationNames[ActiveAnimationIndex];
            }
        }

        public bool IsLooping { get; set; } = true;

        public void PlayAnimation(string Name)
        {
            if (Name == null)
            {
                throw new ArgumentNullException(nameof(Name));
            }

            for (int Index = 0; Index < AnimationNames.Count; Index++)
            {
                if (AnimationNames[Index] == Name)
                {
                    ActiveAnimationIndex = Index;
                    AnimationTime = 0;
                    if (Index < IsUvOnly.Length && IsUvOnly[Index])
                    {
                        UvAnimationTime = 0;
                    }

                    ApplyFrame();
                    return;
                }
            }

            throw new ArgumentException("The glTF file has no animation named \"" + Name + "\".", nameof(Name));
        }

        public void Update(GameTime GameTime)
        {
            if (IsDisposed)
            {
                return;
            }

            // UV-only clips keep scrolling under whichever body clip is playing.
            UvAnimationTime += GameTime.ElapsedGameTime.TotalSeconds;
            if (ActiveAnimationIndex < 0)
            {
                return;
            }

            float Duration = Durations[ActiveAnimationIndex];
            if (Duration > 0f)
            {
                AnimationTime += GameTime.ElapsedGameTime.TotalSeconds;
                if (AnimationTime >= Duration)
                {
                    if (IsLooping)
                    {
                        AnimationTime %= Duration;
                    }
                    else
                    {
                        AnimationTime = Duration;
                    }
                }
            }

            ApplyFrame();
        }

        public void Draw(Matrix View, Matrix Projection, Matrix World)
        {
            if (IsDisposed || Groups.Count == 0)
            {
                return;
            }

            GraphicsDevice Device = WhiteTexture.GraphicsDevice;
            Device.SamplerStates[0] = SamplerState.LinearWrap;
            bool ViewReflects = View.Determinant() < 0f;

            int GroupIndex = 0;
            foreach (DrawableInstance Drawable in Scene)
            {
                DrawableGroup Group = Groups[GroupIndex];
                GroupIndex++;
                if (!Drawable.Transform.Visible || Group.Parts.Count == 0)
                {
                    continue;
                }

                // FlipFaces is a negative scale already in the file. UndoReflectZ mirrors Z again.
                // A reflected view does too. Cull follows that combination. Triangle indices are not reordered.
                bool InverseCull = Drawable.Transform.FlipFaces ^ UndoReflectZ ^ ViewReflects;
                Matrix SkinnedWorld = ReflectZ * World;
                Matrix RigidWorld = Matrix.Identity;
                if (Group.Skinned)
                {
                    SkinnedTransform Skin = (SkinnedTransform)Drawable.Transform;
                    for (int Bone = 0; Bone < Group.Bones.Length; Bone++)
                    {
                        Group.Bones[Bone] = ToXna(Skin.SkinMatrices[Bone]);
                    }
                }
                else
                {
                    RigidWorld = ToXna(((RigidTransform)Drawable.Transform).WorldMatrix) * SkinnedWorld;
                }

                foreach (MeshPart Part in Group.Parts)
                {
                    Vector2 UvOffset = Vector2.Zero;
                    if (Part.MaterialIndex >= 0)
                    {
                        TryGetAnimatedUvOffset(Part.MaterialIndex, out UvOffset);
                    }

                    if (Part.RigidSource != null)
                    {
                        ApplyRigidUv(Part, UvOffset);
                    }

                    Device.RasterizerState = Part.Translucent
                        ? RasterizerState.CullNone
                        : (InverseCull ? RasterizerState.CullClockwise : RasterizerState.CullCounterClockwise);
                    Device.BlendState = Part.Translucent ? BlendState.NonPremultiplied : BlendState.Opaque;
                    Device.DepthStencilState = Part.Translucent ? DepthStencilState.DepthRead : DepthStencilState.Default;

                    EffectPassCollection Passes;
                    if (Group.Skinned)
                    {
                        Part.BoneEffect.Prepare(SkinnedWorld, View, Projection, Group.Bones);
                        Part.BoneEffect.SetUvOffset(UvOffset);
                        Passes = Part.BoneEffect.Passes;
                    }
                    else if (Part.BasicEffect != null)
                    {
                        Part.BasicEffect.View = View;
                        Part.BasicEffect.Projection = Projection;
                        Part.BasicEffect.World = RigidWorld;
                        Passes = Part.BasicEffect.CurrentTechnique.Passes;
                    }
                    else
                    {
                        Part.AlphaEffect.View = View;
                        Part.AlphaEffect.Projection = Projection;
                        Part.AlphaEffect.World = RigidWorld;
                        Passes = Part.AlphaEffect.CurrentTechnique.Passes;
                    }

                    Device.SetVertexBuffer(Part.Vertices);
                    Device.Indices = Part.Indices;
                    foreach (EffectPass Pass in Passes)
                    {
                        Pass.Apply();
                        Device.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 0, Part.PrimitiveCount);
                    }
                }
            }
        }

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;
            foreach (DrawableGroup Group in Groups)
            {
                foreach (MeshPart Part in Group.Parts)
                {
                    Part.Vertices.Dispose();
                    Part.Indices.Dispose();
                    Part.BasicEffect?.Dispose();
                    Part.BoneEffect?.Dispose();
                    Part.AlphaEffect?.Dispose();
                }
            }

            Groups.Clear();
            foreach (Texture2D Texture in OwnedTextures)
            {
                Texture.Dispose();
            }

            OwnedTextures.Clear();
            WhiteTexture = null;
        }

        private void ApplyFrame()
        {
            Scene.Armature.SetAnimationFrame(ActiveAnimationIndex, (float)AnimationTime, false);
        }


        private MeshPart CreatePart(GraphicsDevice Device, MeshPrimitive Primitive, bool Skinned, Dictionary<int, Texture2D> Textures)
        {
            var Positions = Primitive.GetVertexAccessor("POSITION")?.AsVector3Array();
            var Indices = Primitive.GetTriangleIndices();
            if (Positions == null)
            {
                return null;
            }

            List<(int A, int B, int C)> Triangles = new List<(int A, int B, int C)>();
            foreach (var Triangle in Indices)
            {
                Triangles.Add((Triangle.Item1, Triangle.Item2, Triangle.Item3));
            }

            if (Triangles.Count == 0)
            {
                return null;
            }

            var Normals = Primitive.GetVertexAccessor("NORMAL")?.AsVector3Array();
            var Colors = Primitive.GetVertexAccessor("COLOR_0")?.AsColorArray();
            var Joints = Primitive.GetVertexAccessor("JOINTS_0")?.AsVector4Array();
            var Weights = Primitive.GetVertexAccessor("WEIGHTS_0")?.AsVector4Array();
            var JointsExtra = Primitive.GetVertexAccessor("JOINTS_1")?.AsVector4Array();
            var WeightsExtra = Primitive.GetVertexAccessor("WEIGHTS_1")?.AsVector4Array();

            System.Numerics.Vector4 BaseColor = System.Numerics.Vector4.One;
            Texture2D Texture = WhiteTexture;
            AlphaMode Mode = AlphaMode.OPAQUE;
            float AlphaCutoff = 0.5f;
            int MaterialIndex = -1;
            int TextureCoordinate = 0;
            Vector2 UvScale = Vector2.One;
            float UvRotation = 0f;
            Vector2 StaticOffset = Vector2.Zero;
            if (Primitive.Material != null)
            {
                MaterialIndex = Primitive.Material.LogicalIndex;
                Mode = Primitive.Material.Alpha;
                AlphaCutoff = Primitive.Material.AlphaCutoff;
                var Channel = Primitive.Material.FindChannel("BaseColor");
                if (Channel.HasValue)
                {
                    BaseColor = Channel.Value.Color;
                    TextureCoordinate = Channel.Value.TextureCoordinate;
                    if (Channel.Value.Texture != null)
                    {
                        Texture = GetTexture(Device, Channel.Value.Texture, Textures);
                        ReadTextureTransform(Channel.Value.TextureTransform, MaterialScrolls(MaterialIndex), ref TextureCoordinate, ref UvScale, ref UvRotation, ref StaticOffset);
                    }
                }
            }

            var Uvs = Primitive.GetVertexAccessor("TEXCOORD_" + TextureCoordinate.ToString())?.AsVector2Array();
            bool Translucent = Mode == AlphaMode.BLEND;
            bool Scrolls = MaterialScrolls(MaterialIndex);
            int VertexCount = Positions.Count;
            MeshPart Part = new MeshPart();
            Part.Translucent = Translucent;
            Part.MaterialIndex = MaterialIndex;
            Part.PrimitiveCount = Triangles.Count;

            if (Skinned)
            {
                SkinnedVertex[] Vertices = new SkinnedVertex[VertexCount];
                for (int Vertex = 0; Vertex < VertexCount; Vertex++)
                {
                    Vertices[Vertex] = new SkinnedVertex
                    {
                        Position = ToXna(Positions[Vertex]),
                        Normal = NormalAt(Normals, Vertex),
                        TextureCoordinate = TransformUv(UvAt(Uvs, Vertex), UvScale, UvRotation, StaticOffset),
                        BlendIndices = BlendIndicesAt(Joints, JointsExtra, Weights, WeightsExtra, Vertex),
                        BlendWeights = BlendWeightsAt(Weights, WeightsExtra, Vertex)
                    };
                }

                Part.Vertices = new VertexBuffer(Device, SkinnedVertex.Declaration, Vertices.Length, BufferUsage.WriteOnly);
                Part.Vertices.SetData(Vertices);
                Part.BoneEffect = new SkinnedBoneEffect(Device, Texture, new Vector3(BaseColor.X, BaseColor.Y, BaseColor.Z), BaseColor.W);
            }
            else if (Mode == AlphaMode.MASK)
            {
                RigidVertex[] Vertices = new RigidVertex[VertexCount];
                FillRigidVertices(Vertices, Positions, Normals, Uvs, Colors, BaseColor, UvScale, UvRotation, StaticOffset);
                Part.Vertices = CreateRigidBuffer(Device, Vertices, Scrolls, Part);
                Part.AlphaEffect = new AlphaTestEffect(Device);
                Part.AlphaEffect.VertexColorEnabled = true;
                Part.AlphaEffect.Texture = Texture;
                Part.AlphaEffect.AlphaFunction = CompareFunction.GreaterEqual;
                Part.AlphaEffect.ReferenceAlpha = (int)Math.Clamp(AlphaCutoff * 255f, 0f, 255f);
            }
            else
            {
                RigidVertex[] Vertices = new RigidVertex[VertexCount];
                FillRigidVertices(Vertices, Positions, Normals, Uvs, Colors, BaseColor, UvScale, UvRotation, StaticOffset);
                Part.Vertices = CreateRigidBuffer(Device, Vertices, Scrolls, Part);
                Part.BasicEffect = new BasicEffect(Device);
                Part.BasicEffect.LightingEnabled = true;
                Part.BasicEffect.VertexColorEnabled = true;
                Part.BasicEffect.TextureEnabled = true;
                Part.BasicEffect.PreferPerPixelLighting = true;
                Part.BasicEffect.EnableDefaultLighting();
                Part.BasicEffect.Texture = Texture;
                Part.BasicEffect.DiffuseColor = Vector3.One;
            }

            int[] IndexData = new int[Triangles.Count * 3];
            for (int Triangle = 0; Triangle < Triangles.Count; Triangle++)
            {
                IndexData[Triangle * 3] = Triangles[Triangle].A;
                IndexData[Triangle * 3 + 1] = Triangles[Triangle].B;
                IndexData[Triangle * 3 + 2] = Triangles[Triangle].C;
            }

            Part.Indices = new IndexBuffer(Device, IndexElementSize.ThirtyTwoBits, IndexData.Length, BufferUsage.WriteOnly);
            Part.Indices.SetData(IndexData);
            return Part;
        }

        private static void FillRigidVertices(RigidVertex[] Vertices, IList<System.Numerics.Vector3> Positions, IList<System.Numerics.Vector3> Normals, IList<System.Numerics.Vector2> Uvs, IList<System.Numerics.Vector4> Colors, System.Numerics.Vector4 BaseColor, Vector2 UvScale, float UvRotation, Vector2 StaticOffset)
        {
            for (int Vertex = 0; Vertex < Vertices.Length; Vertex++)
            {
                System.Numerics.Vector4 Tint = BaseColor;
                if (Colors != null && Vertex < Colors.Count)
                {
                    System.Numerics.Vector4 VertexColor = Colors[Vertex];
                    Tint = new System.Numerics.Vector4(VertexColor.X * BaseColor.X, VertexColor.Y * BaseColor.Y, VertexColor.Z * BaseColor.Z, VertexColor.W * BaseColor.W);
                }

                Vertices[Vertex] = new RigidVertex
                {
                    Position = ToXna(Positions[Vertex]),
                    Normal = NormalAt(Normals, Vertex),
                    TextureCoordinate = TransformUv(UvAt(Uvs, Vertex), UvScale, UvRotation, StaticOffset),
                    Color = new Color(Tint.X, Tint.Y, Tint.Z, Tint.W)
                };
            }
        }

        private void LoadUvOffsetTracks(ModelRoot LoadedModel)
        {
            UvTracks.Clear();
            for (int Index = 0; Index < IsUvOnly.Length; Index++)
            {
                IsUvOnly[Index] = false;
            }

            foreach (Animation ActiveAnimation in LoadedModel.LogicalAnimations)
            {
                bool HasNodeChannel = false;
                int TrackCount = 0;
                foreach (AnimationChannel ActiveChannel in ActiveAnimation.Channels)
                {
                    if (ActiveChannel.TargetNodePath != PropertyPath.pointer)
                    {
                        HasNodeChannel = true;
                        continue;
                    }

                    Match Pointer = BaseColorOffsetPointer.Match(ActiveChannel.TargetPointerPath ?? string.Empty);
                    int MaterialIndex;
                    if (!Pointer.Success || !int.TryParse(Pointer.Groups[1].Value, out MaterialIndex))
                    {
                        continue;
                    }

                    IAnimationSampler<System.Numerics.Vector2> Sampler = ActiveChannel.GetSamplerOrNull<System.Numerics.Vector2>();
                    if (Sampler == null)
                    {
                        continue;
                    }

                    float Duration = 0f;
                    if (Sampler.InterpolationMode == AnimationInterpolationMode.CUBICSPLINE)
                    {
                        foreach (var Key in Sampler.GetCubicKeys())
                        {
                            if (Key.Key > Duration)
                            {
                                Duration = Key.Key;
                            }
                        }
                    }
                    else
                    {
                        foreach (var Key in Sampler.GetLinearKeys())
                        {
                            if (Key.Key > Duration)
                            {
                                Duration = Key.Key;
                            }
                        }
                    }

                    if (!float.IsFinite(Duration))
                    {
                        Duration = 0f;
                    }

                    ICurveSampler<System.Numerics.Vector2> Curve = Sampler.CreateCurveSampler(true);
                    System.Numerics.Vector2 End = Curve.GetPoint(Duration);
                    System.Numerics.Vector2 Start = Curve.GetPoint(0f);
                    System.Numerics.Vector2 Delta = End - Start;
                    UvOffsetTrack Track = new UvOffsetTrack();
                    Track.AnimationIndex = ActiveAnimation.LogicalIndex;
                    Track.MaterialIndex = MaterialIndex;
                    Track.Duration = Duration;
                    Track.LoopDelta = float.IsFinite(Delta.X) && float.IsFinite(Delta.Y)
                        ? new Vector2(Delta.X, Delta.Y)
                        : Vector2.Zero;
                    Track.Sampler = Curve;
                    UvTracks.Add(Track);
                    TrackCount++;
                }

                if (ActiveAnimation.LogicalIndex >= 0 && ActiveAnimation.LogicalIndex < IsUvOnly.Length)
                {
                    IsUvOnly[ActiveAnimation.LogicalIndex] = TrackCount > 0 && !HasNodeChannel;
                }
            }
        }

        private bool MaterialScrolls(int MaterialIndex)
        {
            if (MaterialIndex < 0)
            {
                return false;
            }

            for (int Index = 0; Index < UvTracks.Count; Index++)
            {
                if (UvTracks[Index].MaterialIndex == MaterialIndex)
                {
                    return true;
                }
            }

            if (MaterialIndex < ExtraUvPerSecond.Length && ExtraUvPerSecond[MaterialIndex] != Vector2.Zero)
            {
                return true;
            }

            return false;
        }

        // The pointer clip wins. This rate is the per-frame RSM value times 60, and only for a material with no track.
        private Vector2[] ReadExtraUvPerSecond(ModelRoot LoadedModel)
        {
            Vector2[] Rates = new Vector2[LoadedModel.LogicalMaterials.Count];
            for (int Index = 0; Index < LoadedModel.LogicalMaterials.Count; Index++)
            {
                if (MaterialScrolls(Index))
                {
                    continue;
                }

                Vector2 PerFrame = ReadUvScrollPerFrame(LoadedModel.LogicalMaterials[Index]);
                if (PerFrame == Vector2.Zero)
                {
                    continue;
                }

                Rates[Index] = new Vector2(PerFrame.X * UvScrollFramesPerSecond, PerFrame.Y * UvScrollFramesPerSecond);
            }

            return Rates;
        }

        private static Vector2 ReadUvScrollPerFrame(Material ActiveMaterial)
        {
            if (!(ActiveMaterial.Extras is JsonObject Extras))
            {
                return Vector2.Zero;
            }

            JsonNode RsmNode;
            if (!Extras.TryGetPropertyValue("rsm", out RsmNode) || !(RsmNode is JsonObject Rsm))
            {
                return Vector2.Zero;
            }

            JsonNode ScrollNode;
            if (!Rsm.TryGetPropertyValue("uvScrollPerFrame", out ScrollNode) || !(ScrollNode is JsonArray Scroll) || Scroll.Count < 2)
            {
                return Vector2.Zero;
            }

            float X;
            float Y;
            if (!TryReadFloat(Scroll[0], out X) || !TryReadFloat(Scroll[1], out Y))
            {
                return Vector2.Zero;
            }

            if (!float.IsFinite(X) || !float.IsFinite(Y))
            {
                throw new InvalidDataException("A material UV scroll rate is not finite.");
            }

            return new Vector2(X, Y);
        }

        private static bool TryReadFloat(JsonNode Node, out float Value)
        {
            Value = 0f;
            JsonValue Number = Node as JsonValue;
            if (Number == null)
            {
                return false;
            }

            double Wide;
            if (Number.TryGetValue(out Wide))
            {
                Value = (float)Wide;
                return true;
            }

            float Direct;
            if (Number.TryGetValue(out Direct))
            {
                Value = Direct;
                return true;
            }

            return false;
        }

        private bool TryGetAnimatedUvOffset(int MaterialIndex, out Vector2 Offset)
        {
            bool FoundAmbient = false;
            Vector2 Ambient = Vector2.Zero;
            for (int Index = 0; Index < UvTracks.Count; Index++)
            {
                UvOffsetTrack Track = UvTracks[Index];
                if (Track.MaterialIndex != MaterialIndex)
                {
                    continue;
                }

                bool UvOnly = Track.AnimationIndex >= 0 && Track.AnimationIndex < IsUvOnly.Length && IsUvOnly[Track.AnimationIndex];
                if (Track.AnimationIndex == ActiveAnimationIndex)
                {
                    Offset = SampleUvOffset(Track, UvOnly ? UvAnimationTime : AnimationTime);
                    return true;
                }

                if (!FoundAmbient && UvOnly)
                {
                    Ambient = SampleUvOffset(Track, UvAnimationTime);
                    FoundAmbient = true;
                }
            }

            if (FoundAmbient)
            {
                Offset = Ambient;
                return true;
            }

            if (MaterialIndex >= 0 && MaterialIndex < ExtraUvPerSecond.Length && ExtraUvPerSecond[MaterialIndex] != Vector2.Zero)
            {
                Offset = ScrollOffset(ExtraUvPerSecond[MaterialIndex], UvAnimationTime);
                return true;
            }

            Offset = Vector2.Zero;
            return false;
        }

        // X is negated so the scroll matches the UVScroll clip. The result stays in [0, 1).
        private static Vector2 ScrollOffset(Vector2 PerSecond, double Time)
        {
            if (PerSecond == Vector2.Zero)
            {
                return Vector2.Zero;
            }

            double X = -PerSecond.X * Time;
            double Y = PerSecond.Y * Time;
            return new Vector2((float)(X - Math.Floor(X)), (float)(Y - Math.Floor(Y)));
        }

        // X is negated so the scroll matches the game. Each loop adds the clip's end offset
        // instead of jumping back to the first key. The result stays in [0, 1).
        private static Vector2 SampleUvOffset(UvOffsetTrack Track, double Time)
        {
            double Loops = 0;
            if (Track.Duration > 0f)
            {
                Loops = Math.Floor(Time / Track.Duration);
                Time -= Loops * Track.Duration;
            }
            else
            {
                Time = 0;
            }

            System.Numerics.Vector2 Value = Track.Sampler.GetPoint((float)Time);
            if (!float.IsFinite(Value.X) || !float.IsFinite(Value.Y))
            {
                throw new InvalidDataException("A texture offset animation has a non-finite value.");
            }

            double X = -Value.X - Loops * Track.LoopDelta.X;
            double Y = Value.Y + Loops * Track.LoopDelta.Y;
            return new Vector2((float)(X - Math.Floor(X)), (float)(Y - Math.Floor(Y)));
        }

        private static void ReadTextureTransform(TextureTransform Transform, bool Scrolls, ref int TextureCoordinate, ref Vector2 UvScale, ref float UvRotation, ref Vector2 StaticOffset)
        {
            if (Transform == null)
            {
                return;
            }

            if (Transform.TextureCoordinateOverride.HasValue)
            {
                TextureCoordinate = Transform.TextureCoordinateOverride.Value;
            }

            if (!float.IsFinite(Transform.Offset.X) || !float.IsFinite(Transform.Offset.Y) || !float.IsFinite(Transform.Scale.X) || !float.IsFinite(Transform.Scale.Y) || !float.IsFinite(Transform.Rotation))
            {
                throw new InvalidDataException("A texture transform has a non-finite value.");
            }

            UvScale = new Vector2(Transform.Scale.X, Transform.Scale.Y);
            UvRotation = Transform.Rotation;
            if (!Scrolls)
            {
                StaticOffset = new Vector2(Transform.Offset.X, Transform.Offset.Y);
            }
        }

        private static Vector2 TransformUv(Vector2 Uv, Vector2 Scale, float Rotation, Vector2 Offset)
        {
            Vector2 Scaled = Uv * Scale;
            float Cos = MathF.Cos(Rotation);
            float Sin = MathF.Sin(Rotation);
            return new Vector2(Cos * Scaled.X + Sin * Scaled.Y + Offset.X, -Sin * Scaled.X + Cos * Scaled.Y + Offset.Y);
        }

        private static VertexBuffer CreateRigidBuffer(GraphicsDevice Device, RigidVertex[] Vertices, bool Scrolls, MeshPart Part)
        {
            if (!Scrolls)
            {
                VertexBuffer Buffer = new VertexBuffer(Device, RigidVertex.Declaration, Vertices.Length, BufferUsage.WriteOnly);
                Buffer.SetData(Vertices);
                return Buffer;
            }

            Part.RigidSource = Vertices;
            Part.RigidScratch = new RigidVertex[Vertices.Length];
            Array.Copy(Vertices, Part.RigidScratch, Vertices.Length);
            DynamicVertexBuffer Dynamic = new DynamicVertexBuffer(Device, RigidVertex.Declaration, Vertices.Length, BufferUsage.WriteOnly);
            Dynamic.SetData(Part.RigidScratch);
            return Dynamic;
        }

        private static void ApplyRigidUv(MeshPart Part, Vector2 Offset)
        {
            for (int Index = 0; Index < Part.RigidSource.Length; Index++)
            {
                RigidVertex Vertex = Part.RigidSource[Index];
                Vertex.TextureCoordinate += Offset;
                Part.RigidScratch[Index] = Vertex;
            }

            Part.Vertices.SetData(Part.RigidScratch);
        }

        private Texture2D GetTexture(GraphicsDevice Device, SharpGLTF.Schema2.Texture Source, Dictionary<int, Texture2D> Textures)
        {
            if (Textures.TryGetValue(Source.LogicalIndex, out Texture2D Loaded))
            {
                return Loaded;
            }

            if (Source.PrimaryImage == null)
            {
                throw new InvalidDataException("Texture " + Source.LogicalIndex.ToString() + " has no image.");
            }

            string Name = string.IsNullOrWhiteSpace(Source.Name) ? "Texture " + Source.LogicalIndex.ToString() : Source.Name;
            try
            {
                using (MemoryStream Stream = new MemoryStream(Source.PrimaryImage.Content.Content.ToArray(), false))
                {
                    Loaded = Texture2D.FromStream(Device, Stream);
                }
            }
            catch (Exception Error)
            {
                throw new InvalidDataException("The glTF image \"" + Name + "\" could not be decoded.", Error);
            }

            Loaded.Name = Name;
            Textures.Add(Source.LogicalIndex, Loaded);
            OwnedTextures.Add(Loaded);
            return Loaded;
        }

        private static Vector3 NormalAt(IList<System.Numerics.Vector3> Normals, int Vertex)
        {
            if (Normals == null || Vertex >= Normals.Count)
            {
                return Vector3.Up;
            }

            return ToXna(Normals[Vertex]);
        }

        private static Vector2 UvAt(IList<System.Numerics.Vector2> Uvs, int Vertex)
        {
            if (Uvs == null || Vertex >= Uvs.Count)
            {
                return Vector2.Zero;
            }

            return new Vector2(Uvs[Vertex].X, Uvs[Vertex].Y);
        }

        private static Byte4 BlendIndicesAt(IList<System.Numerics.Vector4> Joints, IList<System.Numerics.Vector4> JointsExtra, IList<System.Numerics.Vector4> Weights, IList<System.Numerics.Vector4> WeightsExtra, int Vertex)
        {
            int Index0;
            int Index1;
            int Index2;
            int Index3;
            SelectInfluences(Joints, JointsExtra, Weights, WeightsExtra, Vertex, out Index0, out Index1, out Index2, out Index3, out _, out _, out _, out _);
            return new Byte4(Index0, Index1, Index2, Index3);
        }

        private static Vector4 BlendWeightsAt(IList<System.Numerics.Vector4> Weights, IList<System.Numerics.Vector4> WeightsExtra, int Vertex)
        {
            float Weight0;
            float Weight1;
            float Weight2;
            float Weight3;
            SelectInfluences(null, null, Weights, WeightsExtra, Vertex, out _, out _, out _, out _, out Weight0, out Weight1, out Weight2, out Weight3);
            return new Vector4(Weight0, Weight1, Weight2, Weight3);
        }

        private static void SelectInfluences(IList<System.Numerics.Vector4> Joints, IList<System.Numerics.Vector4> JointsExtra, IList<System.Numerics.Vector4> Weights, IList<System.Numerics.Vector4> WeightsExtra, int Vertex, out int Index0, out int Index1, out int Index2, out int Index3, out float Weight0, out float Weight1, out float Weight2, out float Weight3)
        {
            int[] Indices = new int[8];
            float[] Influence = new float[8];
            int Count = 0;
            if (Weights != null && Vertex < Weights.Count)
            {
                System.Numerics.Vector4 Weight = Weights[Vertex];
                System.Numerics.Vector4 Joint = Joints != null && Vertex < Joints.Count ? Joints[Vertex] : System.Numerics.Vector4.Zero;
                Count = AddInfluence(Indices, Influence, Count, Joint.X, Weight.X);
                Count = AddInfluence(Indices, Influence, Count, Joint.Y, Weight.Y);
                Count = AddInfluence(Indices, Influence, Count, Joint.Z, Weight.Z);
                Count = AddInfluence(Indices, Influence, Count, Joint.W, Weight.W);
            }

            if (WeightsExtra != null && Vertex < WeightsExtra.Count)
            {
                System.Numerics.Vector4 Weight = WeightsExtra[Vertex];
                System.Numerics.Vector4 Joint = JointsExtra != null && Vertex < JointsExtra.Count ? JointsExtra[Vertex] : System.Numerics.Vector4.Zero;
                Count = AddInfluence(Indices, Influence, Count, Joint.X, Weight.X);
                Count = AddInfluence(Indices, Influence, Count, Joint.Y, Weight.Y);
                Count = AddInfluence(Indices, Influence, Count, Joint.Z, Weight.Z);
                Count = AddInfluence(Indices, Influence, Count, Joint.W, Weight.W);
            }

            for (int Pass = 0; Pass < Count; Pass++)
            {
                int Best = Pass;
                for (int Other = Pass + 1; Other < Count; Other++)
                {
                    if (Influence[Other] > Influence[Best])
                    {
                        Best = Other;
                    }
                }

                if (Best != Pass)
                {
                    float SwapWeight = Influence[Pass];
                    Influence[Pass] = Influence[Best];
                    Influence[Best] = SwapWeight;
                    int SwapIndex = Indices[Pass];
                    Indices[Pass] = Indices[Best];
                    Indices[Best] = SwapIndex;
                }
            }

            Index0 = Count > 0 ? Indices[0] : 0;
            Index1 = Count > 1 ? Indices[1] : 0;
            Index2 = Count > 2 ? Indices[2] : 0;
            Index3 = Count > 3 ? Indices[3] : 0;
            Weight0 = Count > 0 ? Influence[0] : 1f;
            Weight1 = Count > 1 ? Influence[1] : 0f;
            Weight2 = Count > 2 ? Influence[2] : 0f;
            Weight3 = Count > 3 ? Influence[3] : 0f;
            float Sum = Weight0 + Weight1 + Weight2 + Weight3;
            if (Sum > 0f)
            {
                Weight0 /= Sum;
                Weight1 /= Sum;
                Weight2 /= Sum;
                Weight3 /= Sum;
            }
        }

        private static int AddInfluence(int[] Indices, float[] Influence, int Count, float Joint, float Weight)
        {
            if (Count >= 8 || Weight <= 0f)
            {
                return Count;
            }

            Indices[Count] = (int)MathF.Round(Joint);
            Influence[Count] = Weight;
            return Count + 1;
        }

        private static Matrix ToXna(System.Numerics.Matrix4x4 Source)
        {
            return new Matrix(
                Source.M11, Source.M12, Source.M13, Source.M14,
                Source.M21, Source.M22, Source.M23, Source.M24,
                Source.M31, Source.M32, Source.M33, Source.M34,
                Source.M41, Source.M42, Source.M43, Source.M44);
        }

        private static Vector3 ToXna(System.Numerics.Vector3 Source)
        {
            return new Vector3(Source.X, Source.Y, Source.Z);
        }

        private sealed class DrawableGroup
        {
            public bool Skinned;
            public Matrix[] Bones;
            public readonly List<MeshPart> Parts = new List<MeshPart>();
        }

        private sealed class MeshPart
        {
            public VertexBuffer Vertices;
            public IndexBuffer Indices;
            public BasicEffect BasicEffect;
            public SkinnedBoneEffect BoneEffect;
            public AlphaTestEffect AlphaEffect;
            public int MaterialIndex = -1;
            public int PrimitiveCount;
            public bool Translucent;
            public RigidVertex[] RigidSource;
            public RigidVertex[] RigidScratch;
        }

        private sealed class UvOffsetTrack
        {
            public int AnimationIndex;
            public int MaterialIndex;
            public float Duration;
            public Vector2 LoopDelta;
            public ICurveSampler<System.Numerics.Vector2> Sampler;
        }

        private struct RigidVertex : IVertexType
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Vector2 TextureCoordinate;
            public Color Color;

            public static readonly VertexDeclaration Declaration = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(32, VertexElementFormat.Color, VertexElementUsage.Color, 0));

            VertexDeclaration IVertexType.VertexDeclaration => Declaration;
        }

        private struct SkinnedVertex : IVertexType
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Vector2 TextureCoordinate;
            public Byte4 BlendIndices;
            public Vector4 BlendWeights;

            public static readonly VertexDeclaration Declaration = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(32, VertexElementFormat.Byte4, VertexElementUsage.BlendIndices, 0),
                new VertexElement(36, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0));

            VertexDeclaration IVertexType.VertexDeclaration => Declaration;
        }
    }
}
