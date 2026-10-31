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
    public struct ModelLight
    {
        public Vector3 Ambient;
        public Vector4 Direction;
        public Vector3 Color;

        // Story character selection: a white light from the front.
        public static ModelLight CharacterSelection => new ModelLight
        {
            Ambient = new Vector3(0.3f),
            Direction = new Vector4(0.0349f, -0.0349f, 0.9988f, 1f),
            Color = new Vector3(1.35f),
        };

        // Map light captured from Alcyon.
        public static ModelLight Alcyon => new ModelLight
        {
            Ambient = new Vector3(0.27313f, 0.32163f, 0.38028f),
            Direction = new Vector4(-0.45097f, 0.66687f, 0.59322f, 1f),
            Color = new Vector3(1.08834f, 1.0853f, 0.94626f),
        };
    }

    /// <summary>
    /// Loads one glTF or GLB, including every animation clip in that file.
    /// Clip paths add FBX motions whose bone names match, when this file does not already contain that clip.
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
        private readonly Dictionary<Texture2D, bool> TextureHasHoles = new Dictionary<Texture2D, bool>();
        private Texture2D WhiteTexture;
        private bool HasSkin;
        private readonly List<UvOffsetTrack> UvTracks = new List<UvOffsetTrack>();
        private readonly bool[] IsUvOnly = Array.Empty<bool>();
        private readonly Vector2[] ExtraUvPerSecond = Array.Empty<Vector2>();
        private int ActiveAnimationIndex = -1;
        private double AnimationTime;
        float WetLine;
        float WetAmount;

        public void SetWet(float Line, float Amount)
        {
            WetLine = Line;
            WetAmount = Amount;
        }
        private double UvAnimationTime;
        private bool IsDisposed;

        public static Effect LeafEffect;
        public static float LeafWindTime;
        public static Vector2 LeafWindDir = new Vector2(1f, 0.2f);
        public static float LeafWindStrength = 22f;
        public static Vector3 LeafSunDir = new Vector3(0.35f, 0.86f, 0.25f);
        public static Vector3 LeafSunColor = new Vector3(0.62f, 0.58f, 0.5f);
        public static Vector3 LeafAmbient = new Vector3(0.32f, 0.34f, 0.38f);
        public static Vector3 LeafFillDir = new Vector3(-0.2f, 0.92f, -0.15f);
        public static Vector3 LeafFillColor = new Vector3(0.14f, 0.15f, 0.17f);
        public static bool ShadeBark;
        static DynamicVertexBuffer Instances;
        static InstanceVertex[] InstanceScratch = new InstanceVertex[64];

        public GltfModel(GraphicsDevice Device, string FilePath)
            : this(Device, FilePath, true)
        {
        }

        public GltfModel(GraphicsDevice Device, string FilePath, bool UndoReflectZ, IReadOnlyList<string> ClipPaths = null)
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

            if (ClipPaths != null && ClipPaths.Count > 0)
            {
                FbxClips.Add(Root, ClipPaths);
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
                    if (Drawable.Transform is SkinnedTransform)
                    {
                        HasSkin = true;
                        break;
                    }
                }

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

        public double AnimationSeconds
        {
            get { return AnimationTime; }
        }

        public ArmatureInstance Armature
        {
            get
            {
                if (IsDisposed)
                {
                    return null;
                }

                return Scene.Armature;
            }
        }

        public bool IsLooping { get; set; } = true;

        public bool ReverseCulling;

        // Opaque texels write depth. Softer texels blend afterwards. The lamp matches the character screen.
        public bool CharacterDraw;

        // Life Sim points this at the cascade maps so a body can be darkened by the shadow it just wrote.
        public static Action<Effect> CharacterShade;

        public ModelLight Light;

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
            DrawParts(Device, View, Projection, World, ViewReflects, false);
            DrawParts(Device, View, Projection, World, ViewReflects, true);
        }

        public void DrawPass(Matrix View, Matrix Projection, Matrix World, bool Translucent)
        {
            if (IsDisposed || Groups.Count == 0)
            {
                return;
            }

            GraphicsDevice Device = WhiteTexture.GraphicsDevice;
            Device.SamplerStates[0] = SamplerState.LinearWrap;
            DrawParts(Device, View, Projection, World, View.Determinant() < 0f, Translucent);
        }

        public void DrawMany(Matrix View, Matrix Projection, Matrix[] Worlds, int Count, bool Translucent)
        {
            if (IsDisposed || Count <= 0 || Worlds == null || Groups.Count == 0 || Scene == null)
            {
                return;
            }

            if (LeafEffect == null || FindTechnique(LeafEffect, "LeafInst") == null || FindTechnique(LeafEffect, "BarkInst") == null || HasSkinned())
            {
                for (int i = 0; i < Count; i++)
                {
                    DrawPass(View, Projection, Worlds[i], Translucent);
                }

                return;
            }

            GraphicsDevice Device = LeafEffect.GraphicsDevice;
            EffectTechnique Previous = LeafEffect.CurrentTechnique;
            UploadInstances(Device, Worlds, Count);
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

                Matrix Node = ToXna(((RigidTransform)Drawable.Transform).WorldMatrix) * ReflectZ;
                bool InverseCull = Drawable.Transform.FlipFaces ^ UndoReflectZ ^ ViewReflects ^ ReverseCulling;
                foreach (MeshPart Part in Group.Parts)
                {
                    if (Part.Translucent != Translucent || Part.Vertices == null || Part.Indices == null || Part.PrimitiveCount < 1 || Part.ColorMap == null)
                    {
                        continue;
                    }

                    EffectTechnique Technique = FindTechnique(LeafEffect, Part.Mirrored ? "LeafInst" : "BarkInst");
                    if (Technique == null)
                    {
                        continue;
                    }

                    BindLeaf(LeafEffect, Node, View, Projection, Part.ColorMap);
                    LeafEffect.Parameters["PartWorld"].SetValue(Node);
                    LeafEffect.CurrentTechnique = Technique;
                    RasterizerState Cull = Part.Translucent || Part.DoubleSided || Part.Mirrored
                        ? RasterizerState.CullNone
                        : (InverseCull ? RasterizerState.CullClockwise : RasterizerState.CullCounterClockwise);
                    BlendState Blend = Part.Translucent ? BlendState.NonPremultiplied : BlendState.Opaque;
                    DepthStencilState Depth = Part.Translucent && !Part.Mirrored ? DepthStencilState.DepthRead : DepthStencilState.Default;
                    DrawInstanced(Device, Part, Technique, Cull, Blend, Depth, Count);
                }
            }

            LeafEffect.CurrentTechnique = Previous;
        }

        public void DrawDepthMany(Effect Depth, Matrix View, Matrix Projection, Matrix[] Worlds, int Count, bool Leaves)
        {
            if (IsDisposed || Depth == null || Count <= 0 || Worlds == null || Groups.Count == 0 || Scene == null)
            {
                return;
            }

            if (FindTechnique(Depth, "RigidInst") == null || HasSkinned())
            {
                for (int i = 0; i < Count; i++)
                {
                    DrawDepth(Depth, View, Projection, Worlds[i], Leaves);
                }

                return;
            }

            GraphicsDevice Device = Depth.GraphicsDevice;
            EffectTechnique Previous = Depth.CurrentTechnique;
            UploadInstances(Device, Worlds, Count);
            Depth.Parameters["ViewProj"].SetValue(View * Projection);
            int GroupIndex = 0;
            foreach (DrawableInstance Drawable in Scene)
            {
                DrawableGroup Group = Groups[GroupIndex];
                GroupIndex++;
                if (!Drawable.Transform.Visible || Group.Parts.Count == 0 || !(Drawable.Transform is RigidTransform))
                {
                    continue;
                }

                Matrix Node = ToXna(((RigidTransform)Drawable.Transform).WorldMatrix) * ReflectZ;
                Depth.Parameters["PartWorld"].SetValue(Node);
                foreach (MeshPart Part in Group.Parts)
                {
                    if (Part.Vertices == null || Part.Indices == null || Part.PrimitiveCount < 1)
                    {
                        continue;
                    }

                    if (Part.Mirrored)
                    {
                        if (!Leaves || Part.ColorMap == null)
                        {
                            continue;
                        }

                        EffectTechnique LeavesTechnique = FindTechnique(Depth, "FoliageInst");
                        if (LeavesTechnique == null)
                        {
                            continue;
                        }

                        Depth.Parameters["World"].SetValue(Node);
                        Depth.Parameters["Pivot"]?.SetValue(Node.Translation);
                        Depth.Parameters["WindTime"]?.SetValue(LeafWindTime);
                        Depth.Parameters["WindDir"]?.SetValue(LeafWindDir);
                        Depth.Parameters["WindStrength"]?.SetValue(LeafWindStrength);
                        Depth.Parameters["AlphaCut"]?.SetValue(0.4f);
                        Depth.Parameters["ColorMap"]?.SetValue(Part.ColorMap);
                        Depth.CurrentTechnique = LeavesTechnique;
                        DrawInstanced(Device, Part, LeavesTechnique, RasterizerState.CullNone, BlendState.Opaque, DepthStencilState.Default, Count);
                        continue;
                    }

                    EffectTechnique Trunks = FindTechnique(Depth, "RigidInst");
                    if (Trunks == null)
                    {
                        continue;
                    }

                    Depth.CurrentTechnique = Trunks;
                    DrawInstanced(Device, Part, Trunks, RasterizerState.CullNone, BlendState.Opaque, DepthStencilState.Default, Count);
                }
            }

            Depth.CurrentTechnique = Previous;
        }

        public void DrawDepth(Effect Depth, Matrix View, Matrix Projection, Matrix World, bool Leaves = true)
        {
            if (IsDisposed || Depth == null || Groups.Count == 0 || Scene == null)
            {
                return;
            }

            GraphicsDevice Device = Depth.GraphicsDevice;
            Matrix SkinnedWorld = ReflectZ * World;
            Matrix WorldViewProj = SkinnedWorld * View * Projection;
            int GroupIndex = 0;
            foreach (DrawableInstance Drawable in Scene)
            {
                DrawableGroup Group = Groups[GroupIndex];
                GroupIndex++;
                if (!Drawable.Transform.Visible || Group.Parts.Count == 0)
                {
                    continue;
                }

                foreach (MeshPart Part in Group.Parts)
                {
                    if (Part.ShadowIndices == null || Part.ShadowIndices.Length < 3)
                    {
                        continue;
                    }

                    if (Part.BindPositions != null && !Group.Skinned && Drawable.Transform is RigidTransform)
                    {
                        DrawRigidDepth(Device, Depth, Part, ToXna(((RigidTransform)Drawable.Transform).WorldMatrix), SkinnedWorld, View, Projection, Leaves);
                        continue;
                    }

                    if (Part.ShadowVertices == null)
                    {
                        continue;
                    }

                    Matrix[] Palette = null;
                    if (Group.Skinned && Drawable.Transform is SkinnedTransform)
                    {
                        Palette = BonePalette(Part, ((SkinnedTransform)Drawable.Transform).SkinMatrices);
                    }
                    else if (Part.Bones != null && Drawable.Transform is RigidTransform)
                    {
                        Part.Bones[0] = ToXna(((RigidTransform)Drawable.Transform).WorldMatrix);
                        Palette = Part.Bones;
                    }

                    if (Palette == null)
                    {
                        continue;
                    }

                    if (DrawSkinnedDepth(Device, Depth, Part, Palette, WorldViewProj))
                    {
                        continue;
                    }

                    PoseShadow(Part, Palette);
                    Depth.Parameters["WorldViewProj"].SetValue(WorldViewProj);
                    foreach (EffectPass Pass in Depth.CurrentTechnique.Passes)
                    {
                        Pass.Apply();
                        Device.RasterizerState = RasterizerState.CullNone;
                        Device.BlendState = BlendState.Opaque;
                        Device.DepthStencilState = DepthStencilState.Default;
                        Device.DrawUserIndexedPrimitives(
                            Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList,
                            Part.ShadowScratch,
                            0,
                            Part.ShadowScratch.Length,
                            Part.ShadowIndices,
                            0,
                            Part.ShadowIndices.Length / 3,
                            VertexPosition.VertexDeclaration);
                    }
                }
            }
        }

        static void DrawRigidDepth(GraphicsDevice Device, Effect Depth, MeshPart Part, Matrix Node, Matrix Instance, Matrix View, Matrix Projection, bool Leaves)
        {
            if (!Leaves && Part.Mirrored)
            {
                return;
            }

            if (Part.Mirrored && Part.ColorMap != null && Part.Vertices != null && DrawLeafDepth(Device, Depth, Part, Node, Instance, View, Projection))
            {
                return;
            }

            if (Part.Vertices != null && Part.Indices != null && DrawRigidBuffer(Device, Depth, Part, Node * Instance, View, Projection))
            {
                return;
            }

            Matrix WorldViewProj = Instance * View * Projection;
            if (Part.ShadowScratch == null || Part.ShadowScratch.Length != Part.BindPositions.Length)
            {
                Part.ShadowScratch = new VertexPosition[Part.BindPositions.Length];
            }

            for (int Vertex = 0; Vertex < Part.BindPositions.Length; Vertex++)
            {
                Part.ShadowScratch[Vertex].Position = Vector3.Transform(Part.BindPositions[Vertex], Node);
            }

            Depth.Parameters["WorldViewProj"].SetValue(WorldViewProj);
            foreach (EffectPass Pass in Depth.CurrentTechnique.Passes)
            {
                Pass.Apply();
                Device.RasterizerState = RasterizerState.CullNone;
                Device.BlendState = BlendState.Opaque;
                Device.DepthStencilState = DepthStencilState.Default;
                Device.DrawUserIndexedPrimitives(
                    Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList,
                    Part.ShadowScratch,
                    0,
                    Part.ShadowScratch.Length,
                    Part.ShadowIndices,
                    0,
                    Part.ShadowIndices.Length / 3,
                    VertexPosition.VertexDeclaration);
            }
        }

        static bool DrawLeafDepth(GraphicsDevice Device, Effect Depth, MeshPart Part, Matrix Node, Matrix Instance, Matrix View, Matrix Projection)
        {
            // The shadow effect is already loaded. Its Foliage pass cuts on the leaf texture.
            if (Depth == null)
            {
                return false;
            }

            EffectTechnique Technique = null;
            foreach (EffectTechnique Candidate in Depth.Techniques)
            {
                if (Candidate.Name == "Foliage")
                {
                    Technique = Candidate;
                    break;
                }
            }

            if (Technique == null)
            {
                return false;
            }

            Matrix World = Node * Instance;
            EffectParameter WorldParameter = Depth.Parameters["World"];
            EffectParameter ViewParameter = Depth.Parameters["ViewProj"];
            EffectParameter PivotParameter = Depth.Parameters["Pivot"];
            EffectParameter TimeParameter = Depth.Parameters["WindTime"];
            EffectParameter DirParameter = Depth.Parameters["WindDir"];
            EffectParameter StrengthParameter = Depth.Parameters["WindStrength"];
            EffectParameter CutParameter = Depth.Parameters["AlphaCut"];
            EffectParameter MapParameter = Depth.Parameters["ColorMap"];
            if (WorldParameter == null || ViewParameter == null || MapParameter == null)
            {
                return false;
            }

            WorldParameter.SetValue(World);
            ViewParameter.SetValue(View * Projection);
            PivotParameter?.SetValue(World.Translation);
            TimeParameter?.SetValue(LeafWindTime);
            DirParameter?.SetValue(LeafWindDir);
            StrengthParameter?.SetValue(LeafWindStrength);
            CutParameter?.SetValue(0.4f);
            MapParameter.SetValue(Part.ColorMap);

            EffectTechnique Previous = Depth.CurrentTechnique;
            Depth.CurrentTechnique = Technique;
            Device.SetVertexBuffer(Part.Vertices);
            Device.Indices = Part.Indices;
            foreach (EffectPass Pass in Technique.Passes)
            {
                Pass.Apply();
                Device.RasterizerState = RasterizerState.CullNone;
                Device.BlendState = BlendState.Opaque;
                Device.DepthStencilState = DepthStencilState.Default;
                Device.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 0, Part.PrimitiveCount);
            }

            Depth.CurrentTechnique = Previous;
            return true;
        }

        static bool DrawRigidBuffer(GraphicsDevice Device, Effect Depth, MeshPart Part, Matrix World, Matrix View, Matrix Projection)
        {
            EffectTechnique Technique = FindTechnique(Depth, "Rigid");
            if (Technique == null || Part.PrimitiveCount < 1)
            {
                return false;
            }

            EffectParameter WorldParameter = Depth.Parameters["World"];
            EffectParameter ViewParameter = Depth.Parameters["ViewProj"];
            if (WorldParameter == null || ViewParameter == null)
            {
                return false;
            }

            WorldParameter.SetValue(World);
            ViewParameter.SetValue(View * Projection);
            EffectTechnique Previous = Depth.CurrentTechnique;
            Depth.CurrentTechnique = Technique;
            Device.SetVertexBuffer(Part.Vertices);
            Device.Indices = Part.Indices;
            foreach (EffectPass Pass in Technique.Passes)
            {
                Pass.Apply();
                Device.RasterizerState = RasterizerState.CullNone;
                Device.BlendState = BlendState.Opaque;
                Device.DepthStencilState = DepthStencilState.Default;
                Device.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 0, Part.PrimitiveCount);
            }

            Depth.CurrentTechnique = Previous;
            return true;
        }

        static EffectTechnique FindTechnique(Effect Effect, string Name)
        {
            if (Effect == null)
            {
                return null;
            }

            foreach (EffectTechnique Candidate in Effect.Techniques)
            {
                if (Candidate.Name == Name)
                {
                    return Candidate;
                }
            }

            return null;
        }

        bool HasSkinned()
        {
            for (int i = 0; i < Groups.Count; i++)
            {
                if (Groups[i].Skinned)
                {
                    return true;
                }
            }

            return false;
        }

        static void DrawInstanced(GraphicsDevice Device, MeshPart Part, EffectTechnique Technique, RasterizerState Cull, BlendState Blend, DepthStencilState Depth, int Count)
        {
            foreach (EffectPass Pass in Technique.Passes)
            {
                Pass.Apply();
                Device.SetVertexBuffers(
                    new VertexBufferBinding(Part.Vertices, 0, 0),
                    new VertexBufferBinding(Instances, 0, 1));
                Device.Indices = Part.Indices;
                Device.RasterizerState = Cull;
                Device.BlendState = Blend;
                Device.DepthStencilState = Depth;
                Device.DrawInstancedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 0, Part.PrimitiveCount, Count);
            }
        }

        static void UploadInstances(GraphicsDevice Device, Matrix[] Worlds, int Count)
        {
            if (InstanceScratch.Length < Count)
            {
                InstanceScratch = new InstanceVertex[Math.Max(Count, InstanceScratch.Length * 2)];
            }

            for (int i = 0; i < Count; i++)
            {
                Matrix World = Worlds[i];
                InstanceScratch[i].Row0 = new Vector4(World.M11, World.M12, World.M13, World.M14);
                InstanceScratch[i].Row1 = new Vector4(World.M21, World.M22, World.M23, World.M24);
                InstanceScratch[i].Row2 = new Vector4(World.M31, World.M32, World.M33, World.M34);
                InstanceScratch[i].Row3 = new Vector4(World.M41, World.M42, World.M43, World.M44);
            }

            if (Instances == null || Instances.IsDisposed || Instances.GraphicsDevice != Device || Instances.VertexCount < Count)
            {
                if (Instances != null && !Instances.IsDisposed)
                {
                    Instances.Dispose();
                }

                Instances = new DynamicVertexBuffer(Device, InstanceVertex.Declaration, Math.Max(Count, 256), BufferUsage.WriteOnly);
            }

            Instances.SetData(InstanceScratch, 0, Count, SetDataOptions.Discard);
        }

        static void BindLeaf(Effect Effect, Matrix World, Matrix View, Matrix Projection, Texture2D Texture)
        {
            Effect.Parameters["World"].SetValue(World);
            Effect.Parameters["ViewProj"].SetValue(View * Projection);
            Effect.Parameters["Pivot"].SetValue(World.Translation);
            Effect.Parameters["WindTime"].SetValue(LeafWindTime);
            Effect.Parameters["WindDir"].SetValue(LeafWindDir);
            Effect.Parameters["WindStrength"].SetValue(LeafWindStrength);
            Effect.Parameters["SunDir"].SetValue(LeafSunDir);
            Effect.Parameters["SunColor"].SetValue(LeafSunColor);
            Effect.Parameters["Ambient"].SetValue(LeafAmbient);
            EffectParameter FillDirection = Effect.Parameters["FillDir"];
            if (FillDirection != null)
            {
                FillDirection.SetValue(LeafFillDir);
            }

            EffectParameter FillTint = Effect.Parameters["FillColor"];
            if (FillTint != null)
            {
                FillTint.SetValue(LeafFillColor);
            }

            Effect.Parameters["AlphaCut"].SetValue(0.4f);
            Effect.Parameters["ColorMap"].SetValue(Texture);
        }

        static bool BarkPasses(Matrix World, Matrix View, Matrix Projection, Texture2D Texture, out EffectPassCollection Passes)
        {
            Passes = null;
            if (!ShadeBark || LeafEffect == null || Texture == null)
            {
                return false;
            }

            BindLeaf(LeafEffect, World, View, Projection, Texture);
            foreach (EffectTechnique Candidate in LeafEffect.Techniques)
            {
                if (Candidate.Name != "Bark")
                {
                    continue;
                }

                LeafEffect.CurrentTechnique = Candidate;
                Passes = Candidate.Passes;
                return true;
            }

            return false;
        }

        static bool DrawSkinnedDepth(GraphicsDevice Device, Effect Depth, MeshPart Part, Matrix[] Palette, Matrix WorldViewProj)
        {
            if (Palette == null || Palette.Length == 0 || Part.ShadowVertices == null || Part.Indices == null || Part.PrimitiveCount < 1)
            {
                return false;
            }

            VertexBuffer Buffer = Part.CharacterVertices ?? Part.Vertices;
            EffectTechnique Technique = FindTechnique(Depth, "Skin");
            EffectParameter Bones = Depth.Parameters["Bones"];
            if (Buffer == null || Technique == null || Bones == null)
            {
                return false;
            }

            Bones.SetValue(Palette);
            Depth.Parameters["WorldViewProj"].SetValue(WorldViewProj);
            EffectTechnique Previous = Depth.CurrentTechnique;
            Depth.CurrentTechnique = Technique;
            Device.SetVertexBuffer(Buffer);
            Device.Indices = Part.Indices;
            foreach (EffectPass Pass in Technique.Passes)
            {
                Pass.Apply();
                Device.RasterizerState = RasterizerState.CullNone;
                Device.BlendState = BlendState.Opaque;
                Device.DepthStencilState = DepthStencilState.Default;
                Device.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 0, Part.PrimitiveCount);
            }

            Depth.CurrentTechnique = Previous;
            return true;
        }

        static void PoseShadow(MeshPart Part, Matrix[] Palette)
        {
            SkinnedVertex[] Source = Part.ShadowVertices;
            if (Part.ShadowScratch == null || Part.ShadowScratch.Length != Source.Length)
            {
                Part.ShadowScratch = new VertexPosition[Source.Length];
            }

            for (int Vertex = 0; Vertex < Source.Length; Vertex++)
            {
                Vector3 Position = Source[Vertex].Position;
                Vector4 Weight = Source[Vertex].BlendWeights;
                Vector4 Joint = Source[Vertex].BlendIndices.ToVector4();
                Vector3 Skinned = Vector3.Zero;
                Skinned += SkinPoint(Position, Palette, (int)Joint.X, Weight.X);
                Skinned += SkinPoint(Position, Palette, (int)Joint.Y, Weight.Y);
                Skinned += SkinPoint(Position, Palette, (int)Joint.Z, Weight.Z);
                Skinned += SkinPoint(Position, Palette, (int)Joint.W, Weight.W);
                if (Weight.X + Weight.Y + Weight.Z + Weight.W <= 0f)
                {
                    Skinned = Position;
                }

                Part.ShadowScratch[Vertex].Position = Skinned;
            }
        }

        static Vector3 SkinPoint(Vector3 Position, Matrix[] Palette, int Joint, float Weight)
        {
            if (Weight <= 0f || Palette == null || (uint)Joint >= (uint)Palette.Length)
            {
                return Vector3.Zero;
            }

            return Vector3.Transform(Position, Palette[Joint]) * Weight;
        }

        private void DrawParts(GraphicsDevice Device, Matrix View, Matrix Projection, Matrix World, bool ViewReflects, bool TranslucentPass)
        {
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
                bool InverseCull = Drawable.Transform.FlipFaces ^ UndoReflectZ ^ ViewReflects ^ ReverseCulling;
                Matrix SkinnedWorld = ReflectZ * World;
                Matrix RigidWorld = Matrix.Identity;
                SkinnedTransform Skin = null;
                if (Group.Skinned)
                {
                    Skin = (SkinnedTransform)Drawable.Transform;
                }
                else
                {
                    RigidWorld = ToXna(((RigidTransform)Drawable.Transform).WorldMatrix) * SkinnedWorld;
                }

                foreach (MeshPart Part in Group.Parts)
                {
                    if (!CharacterDraw && Part.Translucent != TranslucentPass)
                    {
                        continue;
                    }

                    Vector2 UvOffset = Vector2.Zero;
                    if (Part.MaterialIndex >= 0)
                    {
                        TryGetAnimatedUvOffset(Part.MaterialIndex, out UvOffset);
                    }

                    if (Part.RigidSource != null)
                    {
                        ApplyRigidUv(Part, UvOffset);
                    }

                    RasterizerState Cull;
                    BlendState Blend;
                    DepthStencilState Depth;
                    if (CharacterDraw)
                    {
                        Cull = RasterizerState.CullClockwise;
                        Blend = TranslucentPass ? BlendState.NonPremultiplied : BlendState.Opaque;
                        Depth = TranslucentPass ? DepthStencilState.DepthRead : DepthStencilState.Default;
                    }
                    else
                    {
                        Cull = Part.Translucent || Part.DoubleSided
                            ? RasterizerState.CullNone
                            : (InverseCull ? RasterizerState.CullClockwise : RasterizerState.CullCounterClockwise);
                        Blend = Part.Translucent ? BlendState.NonPremultiplied : BlendState.Opaque;
                        Depth = Part.Translucent && !Part.Mirrored ? DepthStencilState.DepthRead : DepthStencilState.Default;
                    }

                    EffectPassCollection Passes;
                    bool CharacterLamp = CharacterDraw && Part.CharacterVertices != null;
                    if (Group.Skinned || CharacterLamp)
                    {
                        Matrix[] Palette;
                        if (Group.Skinned)
                        {
                            Palette = BonePalette(Part, Skin.SkinMatrices);
                        }
                        else
                        {
                            Part.Bones[0] = ToXna(((RigidTransform)Drawable.Transform).WorldMatrix);
                            Palette = Part.Bones;
                        }

                        Part.BoneEffect.Prepare(SkinnedWorld, View, Projection, Palette);
                        Part.BoneEffect.SetWet(WetLine, WetAmount);
                        if (CharacterDraw)
                        {
                            Part.BoneEffect.SetCharacterLight(Light.Ambient, Light.Direction, Light.Color, Part.LitFromBothSides, TranslucentPass ? 2f : 1f, Part.SelfIllumination);
                            Part.BoneEffect.BindShade(CharacterShade);
                        }
                        else
                        {
                            Part.BoneEffect.SetDefaultLight();
                        }

                        Part.BoneEffect.SetUvOffset(UvOffset);
                        Passes = Part.BoneEffect.Passes;
                        if (CharacterLamp)
                        {
                            Device.SetVertexBuffer(Part.CharacterVertices);
                        }
                    }
                    else if (Part.Mirrored && LeafEffect != null && Part.ColorMap != null)
                    {
                        BindLeaf(LeafEffect, RigidWorld, View, Projection, Part.ColorMap);
                        EffectTechnique Technique = null;
                        foreach (EffectTechnique Candidate in LeafEffect.Techniques)
                        {
                            if (Candidate.Name == "Leaf")
                            {
                                Technique = Candidate;
                                break;
                            }
                        }

                        if (Technique != null)
                        {
                            LeafEffect.CurrentTechnique = Technique;
                            Passes = Technique.Passes;
                        }
                        else if (Part.AlphaEffect != null)
                        {
                            Part.AlphaEffect.View = View;
                            Part.AlphaEffect.Projection = Projection;
                            Part.AlphaEffect.World = RigidWorld;
                            Passes = Part.AlphaEffect.CurrentTechnique.Passes;
                        }
                        else
                        {
                            FbxModel.ApplySun(Part.BasicEffect);
                            Part.BasicEffect.View = View;
                            Part.BasicEffect.Projection = Projection;
                            Part.BasicEffect.World = RigidWorld;
                            Passes = Part.BasicEffect.CurrentTechnique.Passes;
                        }
                    }
                    else if (Part.Mirrored && Part.AlphaEffect != null)
                    {
                        Part.AlphaEffect.View = View;
                        Part.AlphaEffect.Projection = Projection;
                        Part.AlphaEffect.World = RigidWorld;
                        Passes = Part.AlphaEffect.CurrentTechnique.Passes;
                    }
                    else if (Part.BasicEffect != null)
                    {
                        EffectPassCollection Bark;
                        if (BarkPasses(RigidWorld, View, Projection, Part.ColorMap, out Bark))
                        {
                            Passes = Bark;
                        }
                        else
                        {
                            FbxModel.ApplySun(Part.BasicEffect);
                            Part.BasicEffect.View = View;
                            Part.BasicEffect.Projection = Projection;
                            Part.BasicEffect.World = RigidWorld;
                            Passes = Part.BasicEffect.CurrentTechnique.Passes;
                        }
                    }
                    else
                    {
                        Part.AlphaEffect.View = View;
                        Part.AlphaEffect.Projection = Projection;
                        Part.AlphaEffect.World = RigidWorld;
                        Passes = Part.AlphaEffect.CurrentTechnique.Passes;
                    }

                    if (!CharacterLamp)
                    {
                        Device.SetVertexBuffer(Part.Vertices);
                    }

                    Device.Indices = Part.Indices;
                    foreach (EffectPass Pass in Passes)
                    {
                        Pass.Apply();
                        Device.RasterizerState = Cull;
                        Device.BlendState = Blend;
                        Device.DepthStencilState = Depth;
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
                    Part.CharacterVertices?.Dispose();
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
            bool Holes = false;
            TextureHasHoles.TryGetValue(Texture, out Holes);
            int RenderMode = (int)ReadRsmFloat(Primitive.Material, "renderMode", -1f);
            MeshPart Part = new MeshPart();
            Part.Translucent = Translucent;
            Part.DoubleSided = Primitive.Material != null && Primitive.Material.DoubleSided;
            Part.SelfIllumination = ReadRsmFloat(Primitive.Material, "selfIllumination", 0f);
            // Render mode 1 blends, and modes 2 and 3 add. Those are lit from both sides, and so is a texture with holes.
            Part.LitFromBothSides = Translucent || Holes || RenderMode == 1 || RenderMode == 2 || RenderMode == 3;
            Part.MaterialIndex = MaterialIndex;
            Part.PrimitiveCount = Triangles.Count;

            if (Skinned)
            {
                Dictionary<int, int> Palette = new Dictionary<int, int>();
                List<int> Sources = new List<int>();
                SkinnedVertex[] Vertices = new SkinnedVertex[VertexCount];
                for (int Vertex = 0; Vertex < VertexCount; Vertex++)
                {
                    int Index0;
                    int Index1;
                    int Index2;
                    int Index3;
                    float Weight0;
                    float Weight1;
                    float Weight2;
                    float Weight3;
                    SelectInfluences(Joints, JointsExtra, Weights, WeightsExtra, Vertex, out Index0, out Index1, out Index2, out Index3, out Weight0, out Weight1, out Weight2, out Weight3);
                    Vertices[Vertex] = new SkinnedVertex
                    {
                        Position = ToXna(Positions[Vertex]),
                        Normal = NormalAt(Normals, Vertex),
                        TextureCoordinate = TransformUv(UvAt(Uvs, Vertex), UvScale, UvRotation, StaticOffset),
                        BlendIndices = new Byte4(PaletteSlot(Palette, Sources, Index0), PaletteSlot(Palette, Sources, Index1), PaletteSlot(Palette, Sources, Index2), PaletteSlot(Palette, Sources, Index3)),
                        BlendWeights = new Vector4(Weight0, Weight1, Weight2, Weight3)
                    };
                }

                Part.BoneSource = Sources.ToArray();
                Part.Bones = new Matrix[Math.Max(Part.BoneSource.Length, 1)];
                if (Part.BoneSource.Length == 0)
                {
                    Part.Bones[0] = Matrix.Identity;
                }

                Part.Vertices = new VertexBuffer(Device, SkinnedVertex.Declaration, Vertices.Length, BufferUsage.WriteOnly);
                Part.Vertices.SetData(Vertices);
                Part.ShadowVertices = Vertices;
                Part.ColorMap = Texture;
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
                AddCharacterLamp(Device, Part, Vertices, Texture, BaseColor);
            }
            else
            {
                RigidVertex[] Vertices = new RigidVertex[VertexCount];
                FillRigidVertices(Vertices, Positions, Normals, Uvs, Colors, BaseColor, UvScale, UvRotation, StaticOffset);
                if (Translucent && Part.DoubleSided)
                {
                    Vertices = MirrorLeaves(Vertices);
                    Part.Mirrored = true;
                }

                Part.BindPositions = new Vector3[Vertices.Length];
                for (int Vertex = 0; Vertex < Vertices.Length; Vertex++)
                {
                    Part.BindPositions[Vertex] = Vertices[Vertex].Position;
                }

                if (Part.Mirrored)
                {
                    Part.BindUv = new Vector2[Vertices.Length];
                    for (int Vertex = 0; Vertex < Vertices.Length; Vertex++)
                    {
                        Part.BindUv[Vertex] = Vertices[Vertex].TextureCoordinate;
                    }
                }

                Part.ColorMap = Texture;

                Part.Vertices = CreateRigidBuffer(Device, Vertices, Scrolls, Part);
                Part.BasicEffect = new BasicEffect(Device);
                Part.BasicEffect.LightingEnabled = true;
                Part.BasicEffect.VertexColorEnabled = true;
                Part.BasicEffect.TextureEnabled = true;
                Part.BasicEffect.PreferPerPixelLighting = true;
                Part.BasicEffect.EnableDefaultLighting();
                Part.BasicEffect.Texture = Texture;
                Part.BasicEffect.DiffuseColor = Vector3.One;
                if (Part.Mirrored)
                {
                    Part.AlphaEffect = new AlphaTestEffect(Device);
                    Part.AlphaEffect.VertexColorEnabled = true;
                    Part.AlphaEffect.Texture = Texture;
                    Part.AlphaEffect.AlphaFunction = CompareFunction.GreaterEqual;
                    Part.AlphaEffect.ReferenceAlpha = 102;
                }

                AddCharacterLamp(Device, Part, Vertices, Texture, BaseColor);
            }

            int[] IndexData = new int[Triangles.Count * 3];
            for (int Triangle = 0; Triangle < Triangles.Count; Triangle++)
            {
                IndexData[Triangle * 3] = Triangles[Triangle].A;
                IndexData[Triangle * 3 + 1] = Triangles[Triangle].B;
                IndexData[Triangle * 3 + 2] = Triangles[Triangle].C;
            }

            if (Part.Mirrored)
            {
                int Base = Part.BindPositions.Length / 2;
                int[] Doubled = new int[IndexData.Length * 2];
                Array.Copy(IndexData, Doubled, IndexData.Length);
                for (int Index = 0; Index < IndexData.Length; Index += 3)
                {
                    Doubled[IndexData.Length + Index] = IndexData[Index] + Base;
                    Doubled[IndexData.Length + Index + 1] = IndexData[Index + 2] + Base;
                    Doubled[IndexData.Length + Index + 2] = IndexData[Index + 1] + Base;
                }

                IndexData = Doubled;
                Part.PrimitiveCount = IndexData.Length / 3;
            }

            Part.Indices = new IndexBuffer(Device, IndexElementSize.ThirtyTwoBits, IndexData.Length, BufferUsage.WriteOnly);
            Part.Indices.SetData(IndexData);
            Part.ShadowIndices = IndexData;
            return Part;
        }

        static RigidVertex[] MirrorLeaves(RigidVertex[] Source)
        {
            RigidVertex[] Both = new RigidVertex[Source.Length * 2];
            Array.Copy(Source, Both, Source.Length);
            for (int Vertex = 0; Vertex < Source.Length; Vertex++)
            {
                Both[Source.Length + Vertex] = Source[Vertex];
                Both[Source.Length + Vertex].Normal = -Source[Vertex].Normal;
            }

            return Both;
        }

        // A skinned file's rigid pieces (a bone-parented mesh, the ground shadow) use the same lamp as the skin.
        // One bone carries that node's matrix. Buildings with no skin keep BasicEffect.
        private void AddCharacterLamp(GraphicsDevice Device, MeshPart Part, RigidVertex[] Source, Texture2D Texture, System.Numerics.Vector4 BaseColor)
        {
            if (!HasSkin)
            {
                return;
            }

            SkinnedVertex[] Lamp = new SkinnedVertex[Source.Length];
            for (int Vertex = 0; Vertex < Source.Length; Vertex++)
            {
                Lamp[Vertex] = new SkinnedVertex
                {
                    Position = Source[Vertex].Position,
                    Normal = Source[Vertex].Normal,
                    TextureCoordinate = Source[Vertex].TextureCoordinate,
                    BlendIndices = new Byte4(0, 0, 0, 0),
                    BlendWeights = new Vector4(1f, 0f, 0f, 0f)
                };
            }

            Part.CharacterVertices = new VertexBuffer(Device, SkinnedVertex.Declaration, Lamp.Length, BufferUsage.WriteOnly);
            Part.CharacterVertices.SetData(Lamp);
            Part.ShadowVertices = Lamp;
            Part.ColorMap = Texture;
            Part.Bones = new Matrix[1];
            Part.Bones[0] = Matrix.Identity;
            Part.BoneEffect = new SkinnedBoneEffect(Device, Texture, new Vector3(BaseColor.X, BaseColor.Y, BaseColor.Z), BaseColor.W);
        }

        private static float ReadRsmFloat(Material ActiveMaterial, string Name, float Fallback)
        {
            if (!(ActiveMaterial?.Extras is JsonObject Extras))
            {
                return Fallback;
            }

            JsonNode RsmNode;
            if (!Extras.TryGetPropertyValue("rsm", out RsmNode) || !(RsmNode is JsonObject Rsm))
            {
                return Fallback;
            }

            JsonNode ValueNode;
            if (!Rsm.TryGetPropertyValue(Name, out ValueNode))
            {
                return Fallback;
            }

            float Value;
            if (!TryReadFloat(ValueNode, out Value) || !float.IsFinite(Value))
            {
                return Fallback;
            }

            return Value;
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
            Color[] Pixels = new Color[Loaded.Width * Loaded.Height];
            Loaded.GetData(Pixels);
            bool Holes = false;
            for (int Pixel = 0; Pixel < Pixels.Length; Pixel++)
            {
                if (Pixels[Pixel].A != 255)
                {
                    Holes = true;
                    break;
                }
            }

            TextureHasHoles[Loaded] = Holes;
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

        // A skin can list more joints than the shader palette holds. Each primitive keeps the joints it actually uses.
        private static Matrix[] BonePalette(MeshPart Part, IReadOnlyList<System.Numerics.Matrix4x4> SkinMatrices)
        {
            if (Part.Bones == null || Part.BoneSource == null || Part.BoneSource.Length == 0)
            {
                return Part.Bones;
            }

            for (int Bone = 0; Bone < Part.BoneSource.Length; Bone++)
            {
                int Source = Part.BoneSource[Bone];
                Part.Bones[Bone] = (uint)Source < (uint)SkinMatrices.Count ? ToXna(SkinMatrices[Source]) : Matrix.Identity;
            }

            return Part.Bones;
        }

        private static int PaletteSlot(Dictionary<int, int> Palette, List<int> Sources, int Joint)
        {
            int Slot;
            if (Palette.TryGetValue(Joint, out Slot))
            {
                return Slot;
            }

            if (Sources.Count >= SkinnedBoneEffect.MaxBones)
            {
                throw new InvalidDataException("A skinned primitive uses more than " + SkinnedBoneEffect.MaxBones.ToString() + " bones.");
            }

            Slot = Sources.Count;
            Palette.Add(Joint, Slot);
            Sources.Add(Joint);
            return Slot;
        }

        private sealed class DrawableGroup
        {
            public bool Skinned;
            public readonly List<MeshPart> Parts = new List<MeshPart>();
        }

        private sealed class MeshPart
        {
            public VertexBuffer Vertices;
            public VertexBuffer CharacterVertices;
            public IndexBuffer Indices;
            public BasicEffect BasicEffect;
            public SkinnedBoneEffect BoneEffect;
            public AlphaTestEffect AlphaEffect;
            public int MaterialIndex = -1;
            public int PrimitiveCount;
            public bool Translucent;
            public bool DoubleSided;
            public bool LitFromBothSides;
            public float SelfIllumination;
            public bool Mirrored;
            public Vector3[] BindPositions;
            public Vector2[] BindUv;
            public FoliageShadowVertex[] LeafScratch;
            public int[] BoneSource;
            public Matrix[] Bones;
            public Texture2D ColorMap;
            public SkinnedVertex[] ShadowVertices;
            public int[] ShadowIndices;
            public VertexPosition[] ShadowScratch;
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

        private struct InstanceVertex : IVertexType
        {
            public Vector4 Row0;
            public Vector4 Row1;
            public Vector4 Row2;
            public Vector4 Row3;

            public static readonly VertexDeclaration Declaration = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1),
                new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2),
                new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3),
                new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 4));

            VertexDeclaration IVertexType.VertexDeclaration => Declaration;
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

        private struct FoliageShadowVertex : IVertexType
        {
            public Vector3 Position;
            public Vector2 TextureCoordinate;

            public static readonly VertexDeclaration Declaration = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0));

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
