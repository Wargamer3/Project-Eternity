using System;
using System.Collections.Generic;
using System.IO;
using Assimp;
using Assimp.Configs;
using Assimp.Unmanaged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace ProjectEternity.Core.Graphics
{
    /// <summary>
    /// Loads one FBX at runtime, including every animation clip in that file.
    /// Default.fbx, Idle.fbx, and Walking.fbx stay on the old XNB path.
    /// A material property UvScrollX/UvScrollY is a Culdcept per-frame UV rate.
    /// </summary>
    public sealed class FbxModel : IDisposable
    {
        private static readonly Matrix AxisFix = Matrix.CreateRotationX(-MathHelper.PiOver2);
        private static bool NativeLoaded;

        private readonly List<PoseNode> Roots = new List<PoseNode>();
        private readonly List<Clip> Clips = new List<Clip>();
        private readonly List<Part> Parts = new List<Part>();
        float WetLine;
        float WetAmount;

        public void SetWet(float Line, float Amount)
        {
            WetLine = Line;
            WetAmount = Amount;
        }
        private readonly Matrix InverseRoot;
        private readonly Matrix SkinFix;
        private readonly List<Texture2D> OwnedTextures = new List<Texture2D>();
        private Texture2D WhiteTexture;
        private const float UvScrollFramesPerSecond = 60f;

        private int ActiveClipIndex = -1;
        private double AnimationTime;
        private double UvAnimationTime;
        private bool IsDisposed;

        public FbxModel(GraphicsDevice Device, string FilePath)
        {
            if (Device == null)
            {
                throw new ArgumentNullException(nameof(Device));
            }

            if (string.IsNullOrWhiteSpace(FilePath))
            {
                throw new ArgumentNullException(nameof(FilePath));
            }

            EnsureNative();
            WhiteTexture = new Texture2D(Device, 1, 1);
            WhiteTexture.SetData(new[] { Color.White });
            OwnedTextures.Add(WhiteTexture);

            try
            {
                using (AssimpContext Context = new AssimpContext())
                {
                    Context.SetConfig(new VertexBoneWeightLimitConfig(4));
                    Context.SetConfig(new FBXImportEmbeddedTexturesConfig(true));
                    Scene Source = Context.ImportFile(FilePath, PostProcessSteps.Triangulate | PostProcessSteps.LimitBoneWeights);
                    if (Source.RootNode == null)
                    {
                        throw new InvalidDataException("\"" + FilePath + "\" has no scene.");
                    }

                    PoseNode Root = CopyNode(null, Source.RootNode);
                    Roots.Add(Root);
                    InverseRoot = Matrix.Invert(ToXna(Source.RootNode.Transform));

                    // This file's skeleton stays Z-up while the vertices are Y-up. The rotation puts the bind pose back on the mesh.
                    SkinFix = InverseRoot * AxisFix;

                    for (int Anim = 0; Anim < Source.AnimationCount; Anim++)
                    {
                        Clips.Add(CopyClip(Source.Animations[Anim], Anim));
                    }

                    string Directory = Path.GetDirectoryName(FilePath);
                    for (int MeshIndex = 0; MeshIndex < Source.MeshCount; MeshIndex++)
                    {
                        Part Loaded = CreatePart(Device, Source, Source.Meshes[MeshIndex], Directory, FilePath);
                        if (Loaded != null)
                        {
                            Parts.Add(Loaded);
                        }
                    }
                }
            }
            catch
            {
                Dispose();
                throw;
            }

            if (Clips.Count > 0)
            {
                ActiveClipIndex = 0;
            }
        }

        public IReadOnlyList<string> AnimationNames
        {
            get
            {
                List<string> Names = new List<string>(Clips.Count);
                foreach (Clip Active in Clips)
                {
                    Names.Add(Active.Name);
                }

                return Names;
            }
        }

        public bool IsLooping { get; set; } = true;

        public void Update(GameTime GameTime)
        {
            if (IsDisposed)
            {
                return;
            }

            UvAnimationTime += GameTime.ElapsedGameTime.TotalSeconds;
            if (ActiveClipIndex < 0)
            {
                return;
            }

            Clip Active = Clips[ActiveClipIndex];
            if (Active.TicksPerSecond <= 0 || Active.DurationTicks <= 0)
            {
                return;
            }

            AnimationTime += GameTime.ElapsedGameTime.TotalSeconds * Active.TicksPerSecond;
            if (AnimationTime >= Active.DurationTicks)
            {
                if (IsLooping)
                {
                    AnimationTime %= Active.DurationTicks;
                }
                else
                {
                    AnimationTime = Active.DurationTicks;
                }
            }
        }

        public void CollectTriangles(List<Vector3> Vertices, List<int> Indices)
        {
            if (Vertices == null || Indices == null)
            {
                return;
            }

            foreach (Part Active in Parts)
            {
                if (Active.BindPositions == null || Active.BindIndices == null)
                {
                    continue;
                }

                int Base = Vertices.Count;
                for (int i = 0; i < Active.BindPositions.Length; i++)
                {
                    Vertices.Add(Active.BindPositions[i]);
                }

                for (int i = 0; i < Active.BindIndices.Length; i++)
                {
                    Indices.Add(Base + Active.BindIndices[i]);
                }
            }
        }

        public void Draw(Matrix View, Matrix Projection, Matrix World)
        {
            if (IsDisposed || Parts.Count == 0)
            {
                return;
            }

            Dictionary<string, Matrix> Globals = new Dictionary<string, Matrix>();
            Clip Active = ActiveClipIndex >= 0 ? Clips[ActiveClipIndex] : null;
            foreach (PoseNode Root in Roots)
            {
                Evaluate(Root, Matrix.Identity, Active, AnimationTime, Globals);
            }

            GraphicsDevice Device = WhiteTexture.GraphicsDevice;
            Device.SamplerStates[0] = SamplerState.LinearWrap;
            foreach (Part ActivePart in Parts)
            {
                Vector2 UvOffset = ScrollOffset(ActivePart.UvPerSecond, UvAnimationTime);
                if (ActivePart.RigidSource != null)
                {
                    ApplyRigidUv(ActivePart, UvOffset);
                }

                // These meshes are wound counterclockwise, the same as the glTF props. That cull keeps the outside.
                Device.RasterizerState = RasterizerState.CullClockwise;
                Device.BlendState = BlendState.Opaque;
                Device.DepthStencilState = DepthStencilState.Default;
                if (ActivePart.Skinned)
                {
                    for (int Bone = 0; Bone < ActivePart.BoneNames.Length; Bone++)
                    {
                        Matrix Global;
                        if (!Globals.TryGetValue(ActivePart.BoneNames[Bone], out Global))
                        {
                            Global = Matrix.Identity;
                        }

                        ActivePart.Bones[Bone] = ActivePart.Offsets[Bone] * Global * SkinFix;
                    }

                    ActivePart.BoneEffect.SetWet(WetLine, WetAmount);
                    ActivePart.BoneEffect.Prepare(World, View, Projection, ActivePart.Bones);
                    ActivePart.BoneEffect.SetUvOffset(UvOffset);
                }
                else
                {
                    ApplySun(ActivePart.BasicEffect);
                    ActivePart.BasicEffect.View = View;
                    ActivePart.BasicEffect.Projection = Projection;
                    ActivePart.BasicEffect.World = World;
                }

                Device.SetVertexBuffer(ActivePart.Vertices);
                Device.Indices = ActivePart.Indices;
                EffectPassCollection Passes = ActivePart.Skinned ? ActivePart.BoneEffect.Passes : ActivePart.BasicEffect.CurrentTechnique.Passes;
                foreach (EffectPass Pass in Passes)
                {
                    Pass.Apply();
                    Device.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 0, ActivePart.PrimitiveCount);
                }
            }
        }

        public void DrawDepth(Effect Depth, Matrix View, Matrix Projection, Matrix World)
        {
            if (IsDisposed || Depth == null || Parts.Count == 0)
            {
                return;
            }

            Dictionary<string, Matrix> Globals = new Dictionary<string, Matrix>();
            Clip Active = ActiveClipIndex >= 0 ? Clips[ActiveClipIndex] : null;
            foreach (PoseNode Root in Roots)
            {
                Evaluate(Root, Matrix.Identity, Active, AnimationTime, Globals);
            }

            GraphicsDevice Device = Depth.GraphicsDevice;
            Matrix ViewProj = View * Projection;
            foreach (Part ActivePart in Parts)
            {
                if (ActivePart.BindPositions == null || ActivePart.BindIndices == null || ActivePart.BindIndices.Length < 3)
                {
                    continue;
                }

                Matrix[] Palette = null;
                if (ActivePart.Skinned)
                {
                    for (int Bone = 0; Bone < ActivePart.BoneNames.Length; Bone++)
                    {
                        Matrix Global;
                        if (!Globals.TryGetValue(ActivePart.BoneNames[Bone], out Global))
                        {
                            Global = Matrix.Identity;
                        }

                        ActivePart.Bones[Bone] = ActivePart.Offsets[Bone] * Global * SkinFix;
                    }

                    Palette = ActivePart.Bones;
                }

                if (ActivePart.ShadowScratch == null || ActivePart.ShadowScratch.Length != ActivePart.BindPositions.Length)
                {
                    ActivePart.ShadowScratch = new VertexPosition[ActivePart.BindPositions.Length];
                }

                for (int Vertex = 0; Vertex < ActivePart.BindPositions.Length; Vertex++)
                {
                    Vector3 Position = ActivePart.BindPositions[Vertex];
                    if (Palette != null && ActivePart.BindJoints != null)
                    {
                        Vector4 Joint = ActivePart.BindJoints[Vertex].ToVector4();
                        Vector4 Weight = ActivePart.BindWeights[Vertex];
                        Vector3 Skinned = Vector3.Zero;
                        Skinned += SkinPoint(Position, Palette, (int)Joint.X, Weight.X);
                        Skinned += SkinPoint(Position, Palette, (int)Joint.Y, Weight.Y);
                        Skinned += SkinPoint(Position, Palette, (int)Joint.Z, Weight.Z);
                        Skinned += SkinPoint(Position, Palette, (int)Joint.W, Weight.W);
                        if (Weight.X + Weight.Y + Weight.Z + Weight.W > 0f)
                        {
                            Position = Skinned;
                        }
                    }

                    ActivePart.ShadowScratch[Vertex].Position = Position;
                }

                Depth.Parameters["WorldViewProj"].SetValue(World * ViewProj);
                foreach (EffectPass Pass in Depth.CurrentTechnique.Passes)
                {
                    Pass.Apply();
                    Device.RasterizerState = RasterizerState.CullNone;
                    Device.BlendState = BlendState.Opaque;
                    Device.DepthStencilState = DepthStencilState.Default;
                    Device.DrawUserIndexedPrimitives(
                        Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList,
                        ActivePart.ShadowScratch,
                        0,
                        ActivePart.ShadowScratch.Length,
                        ActivePart.BindIndices,
                        0,
                        ActivePart.BindIndices.Length / 3,
                        VertexPosition.VertexDeclaration);
                }
            }
        }

        static Vector3 SkinPoint(Vector3 Position, Matrix[] Palette, int Joint, float Weight)
        {
            if (Weight <= 0f || (uint)Joint >= (uint)Palette.Length)
            {
                return Vector3.Zero;
            }

            return Vector3.Transform(Position, Palette[Joint]) * Weight;
        }

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;
            foreach (Part Active in Parts)
            {
                Active.Vertices.Dispose();
                Active.Indices.Dispose();
                Active.BoneEffect?.Dispose();
                Active.BasicEffect?.Dispose();
            }

            Parts.Clear();
            foreach (Texture2D Texture in OwnedTextures)
            {
                Texture.Dispose();
            }

            OwnedTextures.Clear();
            WhiteTexture = null;
        }

        internal static void EnsureNative()
        {
            if (NativeLoaded)
            {
                return;
            }

            string Base = AppDomain.CurrentDomain.BaseDirectory;
            string[] Candidates = new string[]
            {
                Path.Combine(Base, "External", "assimp.dll"),
                Path.Combine(Base, "assimp.dll")
            };
            foreach (string Candidate in Candidates)
            {
                if (!File.Exists(Candidate))
                {
                    continue;
                }

                if (!AssimpLibrary.Instance.LoadLibrary(Candidate))
                {
                    throw new InvalidOperationException("\"" + Candidate + "\" did not load.");
                }

                NativeLoaded = true;
                return;
            }

            throw new FileNotFoundException("assimp.dll was not found in External.");
        }

        private PoseNode CopyNode(PoseNode Parent, Node Source)
        {
            PoseNode Copy = new PoseNode();
            Copy.Name = Source.Name ?? string.Empty;
            Copy.Local = ToXna(Source.Transform);
            Copy.Parent = Parent;
            foreach (Node Child in Source.Children)
            {
                Copy.Children.Add(CopyNode(Copy, Child));
            }

            return Copy;
        }

        private static Clip CopyClip(Animation Source, int Index)
        {
            Clip Copy = new Clip();
            Copy.Name = string.IsNullOrWhiteSpace(Source.Name) ? "Animation " + Index.ToString() : Source.Name;
            Copy.DurationTicks = Source.DurationInTicks;
            Copy.TicksPerSecond = Source.TicksPerSecond > 0 ? Source.TicksPerSecond : 25;
            foreach (NodeAnimationChannel Channel in Source.NodeAnimationChannels)
            {
                if (string.IsNullOrEmpty(Channel.NodeName) || Copy.Channels.ContainsKey(Channel.NodeName))
                {
                    continue;
                }

                Copy.Channels.Add(Channel.NodeName, ChannelData.From(Channel));
            }

            return Copy;
        }

        private Part CreatePart(GraphicsDevice Device, Scene Source, Mesh Active, string Directory, string FilePath)
        {
            if (Active.VertexCount == 0 || Active.FaceCount == 0)
            {
                return null;
            }

            if (Active.BoneCount > SkinnedBoneEffect.MaxBones)
            {
                throw new InvalidDataException("\"" + FilePath + "\" mesh \"" + Active.Name + "\" uses " + Active.BoneCount.ToString() + " bones. The effect holds " + SkinnedBoneEffect.MaxBones.ToString() + ".");
            }

            List<int> IndexList = new List<int>();
            foreach (Face ActiveFace in Active.Faces)
            {
                if (ActiveFace.IndexCount != 3)
                {
                    continue;
                }

                IndexList.Add(ActiveFace.Indices[0]);
                IndexList.Add(ActiveFace.Indices[1]);
                IndexList.Add(ActiveFace.Indices[2]);
            }

            if (IndexList.Count == 0)
            {
                return null;
            }

            Vector3 Diffuse = Vector3.One;
            Texture2D Texture = WhiteTexture;
            Vector2 UvPerSecond = Vector2.Zero;
            if (Active.MaterialIndex >= 0 && Active.MaterialIndex < Source.MaterialCount)
            {
                Material ActiveMaterial = Source.Materials[Active.MaterialIndex];
                UvPerSecond = ReadUvScrollPerSecond(ActiveMaterial);
                if (ActiveMaterial.HasColorDiffuse && !ActiveMaterial.ColorDiffuse.IsBlack())
                {
                    Color4D Color = ActiveMaterial.ColorDiffuse;
                    Diffuse = new Vector3(Color.R, Color.G, Color.B);
                }

                TextureSlot Slot;
                if (ActiveMaterial.GetMaterialTexture(TextureType.Diffuse, 0, out Slot))
                {
                    Texture = LoadTexture(Device, Source, Slot.FilePath, Directory);
                }
            }

            if (Texture == WhiteTexture)
            {
                Diffuse = Lift(Diffuse);
            }

            Part Loaded = new Part();
            Loaded.UvPerSecond = UvPerSecond;
            Loaded.PrimitiveCount = IndexList.Count / 3;
            Loaded.Indices = new IndexBuffer(Device, IndexElementSize.ThirtyTwoBits, IndexList.Count, BufferUsage.WriteOnly);
            Loaded.BindIndices = IndexList.ToArray();
            Loaded.Indices.SetData(Loaded.BindIndices);
            if (Active.BoneCount > 0)
            {
                Loaded.Skinned = true;
                Loaded.BoneNames = new string[Active.BoneCount];
                Loaded.Offsets = new Matrix[Active.BoneCount];
                Loaded.Bones = new Matrix[Active.BoneCount];
                int[] Index0 = new int[Active.VertexCount];
                int[] Index1 = new int[Active.VertexCount];
                int[] Index2 = new int[Active.VertexCount];
                int[] Index3 = new int[Active.VertexCount];
                float[] Weight0 = new float[Active.VertexCount];
                float[] Weight1 = new float[Active.VertexCount];
                float[] Weight2 = new float[Active.VertexCount];
                float[] Weight3 = new float[Active.VertexCount];
                for (int Bone = 0; Bone < Active.BoneCount; Bone++)
                {
                    Bone SourceBone = Active.Bones[Bone];
                    Loaded.BoneNames[Bone] = SourceBone.Name ?? string.Empty;
                    Loaded.Offsets[Bone] = ToXna(SourceBone.OffsetMatrix);
                    foreach (VertexWeight Weight in SourceBone.VertexWeights)
                    {
                        InsertWeight(Index0, Index1, Index2, Index3, Weight0, Weight1, Weight2, Weight3, Weight.VertexID, Bone, Weight.Weight);
                    }
                }

                SkinnedVertex[] Vertices = new SkinnedVertex[Active.VertexCount];
                for (int Vertex = 0; Vertex < Active.VertexCount; Vertex++)
                {
                    float Sum = Weight0[Vertex] + Weight1[Vertex] + Weight2[Vertex] + Weight3[Vertex];
                    if (Sum > 0f)
                    {
                        Weight0[Vertex] /= Sum;
                        Weight1[Vertex] /= Sum;
                        Weight2[Vertex] /= Sum;
                        Weight3[Vertex] /= Sum;
                    }
                    else
                    {
                        Weight0[Vertex] = 1f;
                    }

                    Vector3D Position = Active.Vertices[Vertex];
                    Vertices[Vertex] = new SkinnedVertex
                    {
                        Position = new Vector3(Position.X, Position.Y, Position.Z),
                        Normal = NormalAt(Active, Vertex),
                        TextureCoordinate = UvAt(Active, Vertex),
                        BlendIndices = new Byte4(Index0[Vertex], Index1[Vertex], Index2[Vertex], Index3[Vertex]),
                        BlendWeights = new Vector4(Weight0[Vertex], Weight1[Vertex], Weight2[Vertex], Weight3[Vertex])
                    };
                }

                Loaded.Vertices = new VertexBuffer(Device, SkinnedVertex.Declaration, Vertices.Length, BufferUsage.WriteOnly);
                Loaded.Vertices.SetData(Vertices);
                Loaded.BindPositions = new Vector3[Vertices.Length];
                Loaded.BindJoints = new Byte4[Vertices.Length];
                Loaded.BindWeights = new Vector4[Vertices.Length];
                for (int Vertex = 0; Vertex < Vertices.Length; Vertex++)
                {
                    Loaded.BindPositions[Vertex] = Vertices[Vertex].Position;
                    Loaded.BindJoints[Vertex] = Vertices[Vertex].BlendIndices;
                    Loaded.BindWeights[Vertex] = Vertices[Vertex].BlendWeights;
                }

                Loaded.BoneEffect = new SkinnedBoneEffect(Device, Texture, Diffuse, 1f);
            }
            else
            {
                RigidVertex[] Vertices = new RigidVertex[Active.VertexCount];
                for (int Vertex = 0; Vertex < Active.VertexCount; Vertex++)
                {
                    Vector3D Position = Active.Vertices[Vertex];
                    Vertices[Vertex] = new RigidVertex
                    {
                        Position = new Vector3(Position.X, Position.Y, Position.Z),
                        Normal = NormalAt(Active, Vertex),
                        TextureCoordinate = UvAt(Active, Vertex)
                    };
                }

                if (UvPerSecond != Vector2.Zero)
                {
                    Loaded.RigidSource = Vertices;
                    Loaded.RigidScratch = new RigidVertex[Vertices.Length];
                    Array.Copy(Vertices, Loaded.RigidScratch, Vertices.Length);
                    DynamicVertexBuffer Dynamic = new DynamicVertexBuffer(Device, RigidVertex.Declaration, Vertices.Length, BufferUsage.WriteOnly);
                    Dynamic.SetData(Loaded.RigidScratch);
                    Loaded.Vertices = Dynamic;
                }
                else
                {
                    Loaded.Vertices = new VertexBuffer(Device, RigidVertex.Declaration, Vertices.Length, BufferUsage.WriteOnly);
                    Loaded.Vertices.SetData(Vertices);
                }

                Loaded.BindPositions = new Vector3[Vertices.Length];
                for (int Vertex = 0; Vertex < Vertices.Length; Vertex++)
                {
                    Loaded.BindPositions[Vertex] = Vertices[Vertex].Position;
                }

                Loaded.BasicEffect = new BasicEffect(Device);
                Loaded.BasicEffect.TextureEnabled = true;
                Loaded.BasicEffect.Texture = Texture;
                Loaded.BasicEffect.DiffuseColor = Diffuse;
                Light(Loaded.BasicEffect);
            }

            return Loaded;
        }

        // Furniture files ship flat colors with no texture, and those colors are too dark to read under a normal light.
        static Vector3 Lift(Vector3 Diffuse)
        {
            float Max = Math.Max(Diffuse.X, Math.Max(Diffuse.Y, Diffuse.Z));
            if (Max < 0.02f)
            {
                return new Vector3(0.72f);
            }

            if (Max < 0.72f)
            {
                return Diffuse * (0.72f / Max);
            }

            return Diffuse;
        }

        static bool HasSun;
        static Vector3 SunToward = new Vector3(0.35f, 0.86f, 0.25f);
        static Vector3 SunColor = new Vector3(0.62f, 0.58f, 0.50f);
        static Vector3 SunAmbient = new Vector3(0.32f, 0.34f, 0.38f);
        static Vector3 SunFillFrom = new Vector3(-0.2f, 0.92f, -0.15f);
        static Vector3 SunFill = new Vector3(0.14f, 0.15f, 0.17f);

        // TowardSun points at the sun. The effect stores the direction the rays travel.
        public static void UseSun(Vector3 TowardSun, Vector3 Color, Vector3 Ambient, Vector3 FillFrom, Vector3 Fill)
        {
            HasSun = true;
            SunToward = TowardSun;
            SunColor = Color;
            SunAmbient = Ambient;
            SunFillFrom = FillFrom;
            SunFill = Fill;
        }

        static void Light(BasicEffect Effect)
        {
            Effect.LightingEnabled = true;
            Effect.PreferPerPixelLighting = false;
            Effect.AmbientLightColor = new Vector3(0.45f, 0.47f, 0.5f);
            Effect.EmissiveColor = new Vector3(0.06f);
            Effect.SpecularColor = Vector3.Zero;
            Effect.DirectionalLight0.Enabled = true;
            Effect.DirectionalLight0.Direction = new Vector3(-0.3f, -0.9f, -0.3f);
            Effect.DirectionalLight0.DiffuseColor = new Vector3(1.05f, 1.02f, 0.94f);
            Effect.DirectionalLight0.SpecularColor = Vector3.Zero;
            Effect.DirectionalLight1.Enabled = true;
            Effect.DirectionalLight1.Direction = new Vector3(0.7f, -0.15f, 0.55f);
            Effect.DirectionalLight1.DiffuseColor = new Vector3(0.38f, 0.4f, 0.45f);
            Effect.DirectionalLight1.SpecularColor = Vector3.Zero;
            Effect.DirectionalLight2.Enabled = false;
        }

        public static void ApplySun(BasicEffect Effect)
        {
            if (!HasSun || Effect == null)
            {
                return;
            }

            Effect.AmbientLightColor = SunAmbient;
            Effect.DirectionalLight0.Enabled = true;
            Effect.DirectionalLight0.Direction = -SunToward;
            Effect.DirectionalLight0.DiffuseColor = SunColor;
            Effect.DirectionalLight0.SpecularColor = Vector3.Zero;
            Effect.DirectionalLight1.Enabled = true;
            Effect.DirectionalLight1.Direction = -SunFillFrom;
            Effect.DirectionalLight1.DiffuseColor = SunFill;
            Effect.DirectionalLight1.SpecularColor = Vector3.Zero;
        }

        // RSM stores UV units per frame. The glTF exporter turns that into a per-second rate at 60 fps.
        // X follows the same sign as the glTF UVScroll clip.
        private static Vector2 ReadUvScrollPerSecond(Material ActiveMaterial)
        {
            float X = ReadFloatProperty(ActiveMaterial, "UvScrollX");
            float Y = ReadFloatProperty(ActiveMaterial, "UvScrollY");
            return new Vector2(X * UvScrollFramesPerSecond, Y * UvScrollFramesPerSecond);
        }

        private static float ReadFloatProperty(Material ActiveMaterial, string Name)
        {
            foreach (MaterialProperty Property in ActiveMaterial.GetAllProperties())
            {
                if (Property.Name == null || !Property.Name.EndsWith(Name, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (Property.PropertyType == PropertyType.Float)
                {
                    return Property.GetFloatValue();
                }
            }

            return 0f;
        }

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

        private static void ApplyRigidUv(Part ActivePart, Vector2 Offset)
        {
            for (int Index = 0; Index < ActivePart.RigidSource.Length; Index++)
            {
                RigidVertex Vertex = ActivePart.RigidSource[Index];
                Vertex.TextureCoordinate += Offset;
                ActivePart.RigidScratch[Index] = Vertex;
            }

            ActivePart.Vertices.SetData(ActivePart.RigidScratch);
        }

        private Texture2D LoadTexture(GraphicsDevice Device, Scene Source, string TexturePath, string Directory)
        {
            if (string.IsNullOrWhiteSpace(TexturePath))
            {
                return WhiteTexture;
            }

            try
            {
                if (TexturePath[0] == '*')
                {
                    int Index;
                    if (int.TryParse(TexturePath.Substring(1), out Index) && Index >= 0 && Index < Source.TextureCount && Source.Textures[Index].HasCompressedData)
                    {
                        using (MemoryStream Stream = new MemoryStream(Source.Textures[Index].CompressedData, false))
                        {
                            Texture2D Loaded = Texture2D.FromStream(Device, Stream);
                            OwnedTextures.Add(Loaded);
                            return Loaded;
                        }
                    }

                    return WhiteTexture;
                }

                string FullPath = Path.IsPathRooted(TexturePath) ? TexturePath : Path.Combine(Directory, TexturePath);
                if (!File.Exists(FullPath))
                {
                    return WhiteTexture;
                }

                using (FileStream Stream = File.OpenRead(FullPath))
                {
                    Texture2D Loaded = Texture2D.FromStream(Device, Stream);
                    OwnedTextures.Add(Loaded);
                    return Loaded;
                }
            }
            catch (InvalidOperationException)
            {
                return WhiteTexture;
            }
        }

        private static void InsertWeight(int[] Index0, int[] Index1, int[] Index2, int[] Index3, float[] Weight0, float[] Weight1, float[] Weight2, float[] Weight3, int Vertex, int Bone, float Weight)
        {
            if (Vertex < 0 || Vertex >= Weight0.Length || Weight <= 0f)
            {
                return;
            }

            if (Weight > Weight0[Vertex])
            {
                Index3[Vertex] = Index2[Vertex];
                Weight3[Vertex] = Weight2[Vertex];
                Index2[Vertex] = Index1[Vertex];
                Weight2[Vertex] = Weight1[Vertex];
                Index1[Vertex] = Index0[Vertex];
                Weight1[Vertex] = Weight0[Vertex];
                Index0[Vertex] = Bone;
                Weight0[Vertex] = Weight;
            }
            else if (Weight > Weight1[Vertex])
            {
                Index3[Vertex] = Index2[Vertex];
                Weight3[Vertex] = Weight2[Vertex];
                Index2[Vertex] = Index1[Vertex];
                Weight2[Vertex] = Weight1[Vertex];
                Index1[Vertex] = Bone;
                Weight1[Vertex] = Weight;
            }
            else if (Weight > Weight2[Vertex])
            {
                Index3[Vertex] = Index2[Vertex];
                Weight3[Vertex] = Weight2[Vertex];
                Index2[Vertex] = Bone;
                Weight2[Vertex] = Weight;
            }
            else if (Weight > Weight3[Vertex])
            {
                Index3[Vertex] = Bone;
                Weight3[Vertex] = Weight;
            }
        }

        private static Vector3 NormalAt(Mesh Active, int Vertex)
        {
            if (!Active.HasNormals)
            {
                return Vector3.Up;
            }

            Vector3D Normal = Active.Normals[Vertex];
            return new Vector3(Normal.X, Normal.Y, Normal.Z);
        }

        private static Vector2 UvAt(Mesh Active, int Vertex)
        {
            if (!Active.HasTextureCoords(0))
            {
                return Vector2.Zero;
            }

            Vector3D Uv = Active.TextureCoordinateChannels[0][Vertex];
            return new Vector2(Uv.X, Uv.Y);
        }

        private static void Evaluate(PoseNode Active, Matrix Parent, Clip Clip, double Time, Dictionary<string, Matrix> Globals)
        {
            Matrix Local = Active.Local;
            ChannelData Channel;
            if (Clip != null && Clip.Channels.TryGetValue(Active.Name, out Channel))
            {
                Local = Channel.Sample(Time);
            }

            Matrix Global = Local * Parent;
            if (!string.IsNullOrEmpty(Active.Name))
            {
                Globals[Active.Name] = Global;
            }

            foreach (PoseNode Child in Active.Children)
            {
                Evaluate(Child, Global, Clip, Time, Globals);
            }
        }

        private static Matrix ToXna(Matrix4x4 Source)
        {
            return new Matrix(
                Source.A1, Source.B1, Source.C1, Source.D1,
                Source.A2, Source.B2, Source.C2, Source.D2,
                Source.A3, Source.B3, Source.C3, Source.D3,
                Source.A4, Source.B4, Source.C4, Source.D4);
        }

        private static Microsoft.Xna.Framework.Quaternion ToXna(Assimp.Quaternion Source)
        {
            return new Microsoft.Xna.Framework.Quaternion(Source.X, Source.Y, Source.Z, Source.W);
        }

        private sealed class PoseNode
        {
            public string Name;
            public Matrix Local;
            public PoseNode Parent;
            public readonly List<PoseNode> Children = new List<PoseNode>();
        }

        private sealed class Clip
        {
            public string Name;
            public double DurationTicks;
            public double TicksPerSecond;
            public readonly Dictionary<string, ChannelData> Channels = new Dictionary<string, ChannelData>();
        }

        private sealed class ChannelData
        {
            private readonly VectorKey[] Positions;
            private readonly QuaternionKey[] Rotations;
            private readonly VectorKey[] Scales;

            private ChannelData(VectorKey[] Positions, QuaternionKey[] Rotations, VectorKey[] Scales)
            {
                this.Positions = Positions;
                this.Rotations = Rotations;
                this.Scales = Scales;
            }

            public static ChannelData From(NodeAnimationChannel Source)
            {
                return new ChannelData(CopyKeys(Source.PositionKeys), CopyKeys(Source.RotationKeys), CopyKeys(Source.ScalingKeys));
            }

            public Matrix Sample(double Time)
            {
                Vector3 Position = Positions.Length > 0 ? SampleVector(Positions, Time) : Vector3.Zero;
                Vector3 Scale = Scales.Length > 0 ? SampleVector(Scales, Time) : Vector3.One;
                Microsoft.Xna.Framework.Quaternion Rotation = Rotations.Length > 0 ? SampleRotation(Rotations, Time) : Microsoft.Xna.Framework.Quaternion.Identity;
                return Matrix.CreateScale(Scale) * Matrix.CreateFromQuaternion(Rotation) * Matrix.CreateTranslation(Position);
            }

            private static VectorKey[] CopyKeys(List<VectorKey> Source)
            {
                if (Source == null)
                {
                    return new VectorKey[0];
                }

                return Source.ToArray();
            }

            private static QuaternionKey[] CopyKeys(List<QuaternionKey> Source)
            {
                if (Source == null)
                {
                    return new QuaternionKey[0];
                }

                return Source.ToArray();
            }

            private static Vector3 SampleVector(VectorKey[] Keys, double Time)
            {
                if (Keys.Length == 1 || Time <= Keys[0].Time)
                {
                    return new Vector3(Keys[0].Value.X, Keys[0].Value.Y, Keys[0].Value.Z);
                }

                int Next = Keys.Length - 1;
                for (int Index = 0; Index < Keys.Length; Index++)
                {
                    if (Keys[Index].Time > Time)
                    {
                        Next = Index;
                        break;
                    }
                }

                int Previous = Next == 0 ? 0 : Next - 1;
                double Span = Keys[Next].Time - Keys[Previous].Time;
                float Amount = Span > 0 ? (float)((Time - Keys[Previous].Time) / Span) : 0f;
                Vector3 Start = new Vector3(Keys[Previous].Value.X, Keys[Previous].Value.Y, Keys[Previous].Value.Z);
                Vector3 End = new Vector3(Keys[Next].Value.X, Keys[Next].Value.Y, Keys[Next].Value.Z);
                return Vector3.Lerp(Start, End, Amount);
            }

            private static Microsoft.Xna.Framework.Quaternion SampleRotation(QuaternionKey[] Keys, double Time)
            {
                if (Keys.Length == 1 || Time <= Keys[0].Time)
                {
                    return ToXna(Keys[0].Value);
                }

                int Next = Keys.Length - 1;
                for (int Index = 0; Index < Keys.Length; Index++)
                {
                    if (Keys[Index].Time > Time)
                    {
                        Next = Index;
                        break;
                    }
                }

                int Previous = Next == 0 ? 0 : Next - 1;
                double Span = Keys[Next].Time - Keys[Previous].Time;
                float Amount = Span > 0 ? (float)((Time - Keys[Previous].Time) / Span) : 0f;
                return Microsoft.Xna.Framework.Quaternion.Slerp(ToXna(Keys[Previous].Value), ToXna(Keys[Next].Value), Amount);
            }
        }

        private sealed class Part
        {
            public VertexBuffer Vertices;
            public IndexBuffer Indices;
            public Vector3[] BindPositions;
            public Byte4[] BindJoints;
            public Vector4[] BindWeights;
            public int[] BindIndices;
            public bool Skinned;
            public string[] BoneNames;
            public Matrix[] Offsets;
            public Matrix[] Bones;
            public SkinnedBoneEffect BoneEffect;
            public BasicEffect BasicEffect;
            public int PrimitiveCount;
            public Vector2 UvPerSecond;
            public RigidVertex[] RigidSource;
            public RigidVertex[] RigidScratch;
            public VertexPosition[] ShadowScratch;
        }

        private struct RigidVertex : IVertexType
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Vector2 TextureCoordinate;

            public static readonly VertexDeclaration Declaration = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0));

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
