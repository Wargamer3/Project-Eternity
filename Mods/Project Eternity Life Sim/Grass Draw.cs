using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class GrassDraw
    {
        [StructLayout(LayoutKind.Sequential)]
        struct GrassVertex : IVertexType
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Vector2 Uv;
            public Vector2 Bend;

            public static readonly VertexDeclaration Declaration = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(32, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1));

            public VertexDeclaration VertexDeclaration
            {
                get { return Declaration; }
            }
        }

        sealed class Tuft
        {
            public Vector3[] Position;
            public Vector3[] Normal;
            public Vector2[] Uv;
            public int[] Index;
            public float MinY;
            public float MaxY;
        }

        sealed class Patch : IDisposable
        {
            public VertexBuffer Vertices;
            public IndexBuffer Indices;
            public int Triangles;
            public int Revision;
            public int Spread;
            public int Grass;

            public void Dispose()
            {
                if (Vertices != null)
                {
                    Vertices.Dispose();
                    Vertices = null;
                }

                if (Indices != null)
                {
                    Indices.Dispose();
                    Indices = null;
                }

                Triangles = 0;
            }
        }

        const float FadeEnd = 1250f;
        const int Spread = 3;
        const int StepSlots = 8;
        const float CrumbLife = 1.05f;
        const float CrumbGap = 8f;
        static Effect Effect;
        static Texture2D Texture;
        static Tuft Dense;
        static bool Failed;
        static readonly Dictionary<MapInfo, Patch> Patches = new Dictionary<MapInfo, Patch>();
        static readonly List<MapInfo> Drop = new List<MapInfo>();
        static readonly List<GrassVertex> BuiltVerts = new List<GrassVertex>(8192);
        static readonly List<int> BuiltIndices = new List<int>(12288);
        static readonly Vector4[] StepPack = new Vector4[StepSlots];
        static readonly Vector4[] MovePack = new Vector4[StepSlots];
        static readonly Body[] Bodies = new Body[16];
        static readonly bool[] BodyUsed = new bool[16];
        static readonly Track[] Tracks = new Track[16];
        static readonly Crumb[] Crumbs = new Crumb[StepSlots];
        static int CrumbNext;
        static int CrumbCount;
        static float LastClock;

        struct Body
        {
            public PlayerCharacter Who;
            public Vector3 Place;
            public Vector2 Move;
            public float Speed;
            public float Dist2;
        }

        struct Track
        {
            public PlayerCharacter Who;
            public Vector2 At;
            public Vector2 Crumb;
        }

        struct Crumb
        {
            public Vector2 At;
            public Vector2 Move;
            public float FeetY;
            public float Born;
        }

        public static void All(GraphicsDevice Device, MapInfo Info, Matrix View, Matrix Projection, Vector3 Eye)
        {
            if (Info == null)
            {
                return;
            }

            MapInfo[] Single = new MapInfo[1];
            Single[0] = Info;
            All(Device, Single, View, Projection, Eye);
        }

        public static void All(GraphicsDevice Device, IList<MapInfo> Maps, Matrix View, Matrix Projection, Vector3 Eye)
        {
            if (Device == null || Maps == null || Maps.Count == 0)
            {
                return;
            }

            if (PictureOptions.Grass <= 0)
            {
                return;
            }

            Ensure(Device);
            if (Failed || Effect == null || Texture == null || Dense == null)
            {
                return;
            }

            Retain(Maps);
            Matrix ViewProj = View * Projection;
            BoundingFrustum Frustum = new BoundingFrustum(ViewProj);
            Effect.Parameters["ViewProj"].SetValue(ViewProj);
            Effect.Parameters["WindTime"].SetValue(GltfModel.LeafWindTime);
            Effect.Parameters["WindDir"].SetValue(GltfModel.LeafWindDir);
            Effect.Parameters["WindStrength"].SetValue(4f + Climate.Now.Strength);
            Effect.Parameters["SunDir"].SetValue(GltfModel.LeafSunDir);
            Effect.Parameters["SunColor"].SetValue(GltfModel.LeafSunColor);
            Effect.Parameters["Ambient"].SetValue(GltfModel.LeafAmbient);
            Effect.Parameters["Eye"].SetValue(Eye);
            float GrassReach = PictureOptions.Reach;
            EffectParameter FadeNear = Effect.Parameters["FadeStart"];
            EffectParameter FadeFar = Effect.Parameters["FadeEnd"];
            if (FadeNear != null)
            {
                FadeNear.SetValue(860f * GrassReach);
            }

            if (FadeFar != null)
            {
                FadeFar.SetValue(1250f * GrassReach);
            }
            int Steps = Feet(Maps, Eye);
            Effect.Parameters["StepCount"].SetValue(Steps);
            Effect.Parameters["Steps"].SetValue(StepPack);
            Effect.Parameters["StepMove"].SetValue(MovePack);
            Effect.Parameters["Clump"].SetValue(Texture);
            Effect.CurrentTechnique = Effect.Techniques["Grass"];
            Device.BlendState = BlendState.Opaque;
            Device.DepthStencilState = DepthStencilState.Default;
            Device.RasterizerState = RasterizerState.CullNone;
            foreach (EffectPass Pass in Effect.CurrentTechnique.Passes)
            {
                Pass.Apply();
                for (int m = 0; m < Maps.Count; m++)
                {
                    MapInfo Info = Maps[m];
                    if (!Seen(Frustum, Info))
                    {
                        continue;
                    }

                    Patch Field = Ready(Device, Info, Eye);
                    if (Field == null || Field.Triangles < 1)
                    {
                        continue;
                    }

                    Device.SetVertexBuffer(Field.Vertices);
                    Device.Indices = Field.Indices;
                    Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, Field.Triangles);
                }
            }
        }

        static int Feet(IList<MapInfo> Maps, Vector3 Eye)
        {
            for (int i = 0; i < StepSlots; i++)
            {
                StepPack[i] = Vector4.Zero;
                MovePack[i] = Vector4.Zero;
            }

            float Now = GltfModel.LeafWindTime;
            float Dt = Now - LastClock;
            LastClock = Now;
            if (Dt < 0.001f || Dt > 0.5f)
            {
                Dt = 0f;
            }

            int BodyCount = 0;
            for (int m = 0; m < Maps.Count; m++)
            {
                MapInfo Info = Maps[m];
                if (Info == null || Info.People == null)
                {
                    continue;
                }

                Vector3 Shift = ChunkOrigin.Shift(Info);
                for (int p = 0; p < Info.People.Count && BodyCount < Bodies.Length; p++)
                {
                    PlayerCharacter Who = Info.People[p];
                    if (Who == null || Who.HiddenFromBattle)
                    {
                        continue;
                    }

                    Vector3 Place = ChunkOrigin.ModelWorld(Who.Position, Shift);
                    Vector2 At = new Vector2(Place.X, Place.Z);
                    Vector2 Move = new Vector2(Who.MovementSpeed.X + Who.ExternalSpeed.X, Who.MovementSpeed.Y + Who.ExternalSpeed.Y);
                    int Follow = Followed(Who);
                    if (Follow >= 0 && Dt > 0f)
                    {
                        Vector2 StepDelta = (At - Tracks[Follow].At) / Dt;
                        if (StepDelta.LengthSquared() > Move.LengthSquared())
                        {
                            Move = StepDelta;
                        }
                    }

                    float Speed = Move.Length();
                    if (Speed > 80f)
                    {
                        Move *= 80f / Speed;
                        Speed = 80f;
                    }
                    Bodies[BodyCount].Who = Who;
                    Bodies[BodyCount].Place = Place;
                    Bodies[BodyCount].Move = Move;
                    Bodies[BodyCount].Speed = Speed;
                    float DX = Place.X - Eye.X;
                    float DZ = Place.Z - Eye.Z;
                    Bodies[BodyCount].Dist2 = DX * DX + DZ * DZ;
                    BodyUsed[BodyCount] = false;
                    BodyCount++;
                    Trail(Who, At, Move, Place.Y, Speed, Now);
                }
            }

            for (int i = BodyCount; i < Bodies.Length; i++)
            {
                Bodies[i].Who = null;
            }

            Release();
            int Slot = 0;
            int Guard = 0;
            while (Slot < 4 && Guard < BodyCount)
            {
                int Best = -1;
                float BestDist = float.MaxValue;
                for (int i = 0; i < BodyCount; i++)
                {
                    if (BodyUsed[i] || Bodies[i].Dist2 >= BestDist)
                    {
                        continue;
                    }

                    Best = i;
                    BestDist = Bodies[i].Dist2;
                }

                if (Best < 0)
                {
                    break;
                }

                BodyUsed[Best] = true;
                WriteStep(Slot, Bodies[Best].Place, Bodies[Best].Move, 24f + Math.Min(Bodies[Best].Speed * 0.2f, 12f), MathHelper.Clamp(0.75f + Bodies[Best].Speed / 34f, 0.75f, 1.25f));
                Slot++;
                Guard++;
            }

            for (int n = 0; n < CrumbCount && Slot < StepSlots; n++)
            {
                int Index = CrumbNext - 1 - n;
                if (Index < 0)
                {
                    Index += StepSlots;
                }

                Crumb Mark = Crumbs[Index];
                float Life = 1f - (Now - Mark.Born) / CrumbLife;
                if (Life <= 0f)
                {
                    continue;
                }

                WriteStep(Slot, new Vector3(Mark.At.X, Mark.FeetY, Mark.At.Y), Mark.Move, 22f, Life * 0.9f);
                Slot++;
            }

            return Slot;
        }

        static void WriteStep(int Slot, Vector3 Place, Vector2 Move, float Reach, float Strength)
        {
            StepPack[Slot] = new Vector4(Place.X, Place.Y, Place.Z, Strength);
            MovePack[Slot] = new Vector4(Move.X, Move.Y, Reach, 0f);
        }

        static void Trail(PlayerCharacter Who, Vector2 At, Vector2 Move, float FeetY, float Speed, float Now)
        {
            int Follow = Followed(Who);
            if (Follow < 0)
            {
                for (int i = 0; i < Tracks.Length; i++)
                {
                    if (Tracks[i].Who == null)
                    {
                        Tracks[i].Who = Who;
                        Tracks[i].At = At;
                        Tracks[i].Crumb = At;
                        return;
                    }
                }

                return;
            }

            Vector2 Delta = At - Tracks[Follow].Crumb;
            if (Speed > 5f && Delta.LengthSquared() > CrumbGap * CrumbGap)
            {
                Crumbs[CrumbNext].At = Tracks[Follow].At;
                Crumbs[CrumbNext].Move = Move;
                Crumbs[CrumbNext].FeetY = FeetY;
                Crumbs[CrumbNext].Born = Now;
                CrumbNext++;
                if (CrumbNext >= StepSlots)
                {
                    CrumbNext = 0;
                }

                if (CrumbCount < StepSlots)
                {
                    CrumbCount++;
                }

                Tracks[Follow].Crumb = At;
            }

            Tracks[Follow].At = At;
        }

        static int Followed(PlayerCharacter Who)
        {
            for (int i = 0; i < Tracks.Length; i++)
            {
                if (Tracks[i].Who == Who)
                {
                    return i;
                }
            }

            return -1;
        }

        static void Release()
        {
            for (int i = 0; i < Tracks.Length; i++)
            {
                if (Tracks[i].Who == null)
                {
                    continue;
                }

                bool Still = false;
                for (int b = 0; b < Bodies.Length; b++)
                {
                    if (Bodies[b].Who == Tracks[i].Who)
                    {
                        Still = true;
                        break;
                    }
                }

                if (!Still)
                {
                    Tracks[i].Who = null;
                }
            }
        }

        static bool Seen(BoundingFrustum Frustum, MapInfo Info)
        {
            float SpanX;
            float SpanZ;
            Vector3 Shift = ChunkOrigin.Shift(Info);
            if (!Span(Info, out SpanX, out SpanZ))
            {
                return false;
            }

            BoundingBox Box = new BoundingBox(
                new Vector3(Shift.X, -80f, Shift.Z),
                new Vector3(Shift.X + SpanX, 480f, Shift.Z + SpanZ));
            return Frustum.Contains(Box) != ContainmentType.Disjoint;
        }

        static Patch Ready(GraphicsDevice Device, MapInfo Info, Vector3 Eye)
        {
            if (Info == null || Info.Heights == null)
            {
                return null;
            }

            float SpanX;
            float SpanZ;
            if (!Span(Info, out SpanX, out SpanZ))
            {
                return null;
            }

            Vector3 Shift = ChunkOrigin.Shift(Info);
            float NearX = Math.Max(Shift.X, Math.Min(Eye.X, Shift.X + SpanX)) - Eye.X;
            float NearZ = Math.Max(Shift.Z, Math.Min(Eye.Z, Shift.Z + SpanZ)) - Eye.Z;
            float FarGrass = 1055f * PictureOptions.Reach;
            if (NearX * NearX + NearZ * NearZ > FarGrass * FarGrass)
            {
                return null;
            }

            Patch Field;
            int Revision = Info.Heights.Revision;
            int Mode = PictureOptions.Grass;
            if (Patches.TryGetValue(Info, out Field) && Field.Revision == Revision && Field.Spread == Spread && Field.Grass == Mode)
            {
                return Field;
            }

            if (Field == null)
            {
                Field = new Patch();
                Patches[Info] = Field;
            }

            Field.Dispose();
            Field.Revision = Revision;
            Field.Spread = Spread;
            Field.Grass = PictureOptions.Grass;
            Fill(Info, Shift);
            if (BuiltVerts.Count < 3 || BuiltIndices.Count < 3)
            {
                return Field;
            }

            Field.Vertices = new VertexBuffer(Device, GrassVertex.Declaration, BuiltVerts.Count, BufferUsage.WriteOnly);
            Field.Vertices.SetData(BuiltVerts.ToArray());
            Field.Indices = new IndexBuffer(Device, IndexElementSize.ThirtyTwoBits, BuiltIndices.Count, BufferUsage.WriteOnly);
            Field.Indices.SetData(BuiltIndices.ToArray());
            Field.Triangles = BuiltIndices.Count / 3;
            return Field;
        }

        static void Fill(MapInfo Info, Vector3 Shift)
        {
            BuiltVerts.Clear();
            BuiltIndices.Clear();
            HeightField Field = Info.Heights;
            float SpanX = (Field.SamplesX - 1) * Field.Spacing;
            float SpanZ = (Field.SamplesZ - 1) * Field.Spacing;
            bool Woods = Info.Forest != null && Info.Forest.HasTrees && Info.Forest.Density >= 0.2f;
            float Place = (Woods ? 10f : 13f) * PictureOptions.GrassGap;
            if (SpanX < Place || SpanZ < Place)
            {
                return;
            }

            int Across = (int)(Math.Max(SpanX, SpanZ) / Place);
            if (Across > 36)
            {
                Place = Math.Max(SpanX, SpanZ) / 36f;
            }

            int SeedX = Info.ChunkX * 13 + 3;
            int SeedZ = Info.ChunkZ * 29 + 7;
            for (float Z = Place * 0.5f; Z < SpanZ; Z += Place)
            {
                for (float X = Place * 0.5f; X < SpanX; X += Place)
                {
                    int CellX = (int)X;
                    int CellZ = (int)Z;
                    float LocalX = X + (Hash(SeedX + CellX, SeedZ + CellZ) - 0.5f) * Place * 0.7f;
                    float LocalZ = Z + (Hash(SeedZ + CellZ, SeedX + CellX) - 0.5f) * Place * 0.7f;
                    if (LocalX < 1f || LocalZ < 1f || LocalX >= SpanX - 1f || LocalZ >= SpanZ - 1f)
                    {
                        continue;
                    }

                    float Height;
                    float SlopeX;
                    float SlopeY;
                    if (!Field.Sample(LocalX, LocalZ, out Height, out SlopeX, out SlopeY))
                    {
                        continue;
                    }

                    if (SlopeX * SlopeX + SlopeY * SlopeY > 0.95f)
                    {
                        continue;
                    }

                    Vector4 Blend = Field.ReadBlend(LocalX, LocalZ);
                    if (Blend.W > 0.42f || Blend.Z > 0.75f)
                    {
                        continue;
                    }

                    if (Woods)
                    {
                        if (Blend.Y > 0.82f && Blend.X < 0.08f)
                        {
                            continue;
                        }
                    }
                    else if (Blend.X < 0.18f)
                    {
                        continue;
                    }

                    if (Wet(Info, Field, LocalX, LocalZ) || Blocked(Info, LocalX, LocalZ))
                    {
                        continue;
                    }

                    Tuft Mesh = Dense;
                    float Scale = 24f + Hash(CellX * 3 + 1, CellZ * 5 + 2) * 7f;
                    float Yaw = Hash(CellZ + 11, CellX + 19) * 6.2831853f;
                    float Phase = Hash(CellX + 41, CellZ + 17) * 6.2831853f;
                    Vector3 Feet = new Vector3(LocalX + Shift.X, Height + Shift.Y - 0.35f, LocalZ + Shift.Z);
                    Plant(Mesh, Feet, Scale, Yaw, Phase);
                }
            }
        }

        static void Plant(Tuft Mesh, Vector3 Feet, float Scale, float Yaw, float Phase)
        {
            int Room = Mesh.Position.Length;
            if (BuiltVerts.Count + Room > 220000)
            {
                return;
            }

            int Base = BuiltVerts.Count;
            float Turn = (float)Math.Cos(Yaw);
            float Spin = (float)Math.Sin(Yaw);
            float Tall = Mesh.MaxY - Mesh.MinY;
            if (Tall < 0.0001f)
            {
                Tall = 1f;
            }

            for (int v = 0; v < Room; v++)
            {
                Vector3 Local = Mesh.Position[v];
                float X = Local.X * Scale;
                float Z = Local.Z * Scale;
                Vector3 Normal = Mesh.Normal[v];
                GrassVertex Vertex = new GrassVertex();
                Vertex.Position = new Vector3(Feet.X + Turn * X + Spin * Z, Feet.Y + Local.Y * Scale, Feet.Z - Spin * X + Turn * Z);
                Vertex.Normal = new Vector3(Turn * Normal.X + Spin * Normal.Z, Normal.Y, -Spin * Normal.X + Turn * Normal.Z);
                Vertex.Uv = Mesh.Uv[v];
                Vertex.Bend = new Vector2((Local.Y - Mesh.MinY) / Tall, Phase);
                BuiltVerts.Add(Vertex);
            }

            for (int i = 0; i < Mesh.Index.Length; i++)
            {
                BuiltIndices.Add(Base + Mesh.Index[i]);
            }
        }

        static bool Wet(MapInfo Info, HeightField Field, float X, float Z)
        {
            if (Info.Water == null)
            {
                return false;
            }

            for (int i = 0; i < Info.Water.Count; i++)
            {
                WaterVolume Volume = Info.Water[i];
                if (Volume != null && Volume.Covers(Field, X, Z))
                {
                    return true;
                }
            }

            return false;
        }

        static bool Blocked(MapInfo Info, float X, float Z)
        {
            if (Info.Buildings == null)
            {
                return false;
            }

            for (int i = 0; i < Info.Buildings.Count; i++)
            {
                BuildingItem Box = Info.Buildings[i];
                if (Box == null)
                {
                    continue;
                }

                float Reach = 48f;
                if (!string.IsNullOrEmpty(Box.Model) && Box.Model.IndexOf("Medieval Village Pack", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Reach = Box.Model.IndexOf("Props", StringComparison.OrdinalIgnoreCase) >= 0 ? 36f : 120f;
                }

                float DX = Box.Position.X - X;
                float DZ = Box.Position.Y - Z;
                if (DX * DX + DZ * DZ < Reach * Reach)
                {
                    return true;
                }
            }

            return false;
        }

        static bool Span(MapInfo Info, out float SpanX, out float SpanZ)
        {
            SpanX = 0f;
            SpanZ = 0f;
            if (Info.Heights == null || Info.Heights.Spacing <= 0.01f)
            {
                return false;
            }

            SpanX = (Info.Heights.SamplesX - 1) * Info.Heights.Spacing;
            SpanZ = (Info.Heights.SamplesZ - 1) * Info.Heights.Spacing;
            return SpanX > 1f && SpanZ > 1f;
        }

        static void Retain(IList<MapInfo> Maps)
        {
            if (Patches.Count == 0)
            {
                return;
            }

            Drop.Clear();
            foreach (KeyValuePair<MapInfo, Patch> Pair in Patches)
            {
                bool Keep = false;
                for (int i = 0; i < Maps.Count; i++)
                {
                    if (Maps[i] == Pair.Key)
                    {
                        Keep = true;
                        break;
                    }
                }

                if (!Keep)
                {
                    Drop.Add(Pair.Key);
                }
            }

            for (int i = 0; i < Drop.Count; i++)
            {
                Patches[Drop[i]].Dispose();
                Patches.Remove(Drop[i]);
            }
        }

        static float Hash(int X, int Z)
        {
            int N = X * 374761393 + Z * 668265263;
            N = (N ^ (N >> 13)) * 1274126177;
            return (N & 0x7fffffff) / 2147483647f;
        }

        static void Ensure(GraphicsDevice Device)
        {
            if ((Effect != null && Texture != null && Dense != null) || Failed)
            {
                return;
            }

            try
            {
                string Shader = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Life Sim", "Shaders", "Grass.mgfx");
                string Model = Path.Combine(WorldSlot.ModelsRoot(), "Grass", "Grass 2.glb");
                if (!File.Exists(Shader) || !File.Exists(Model))
                {
                    Failed = true;
                    return;
                }

                if (Effect == null)
                {
                    Effect = new Effect(Device, File.ReadAllBytes(Shader));
                }

                if (Dense == null)
                {
                    byte[] Image;
                    Dense = Load(Model, out Image);
                    if (Dense == null || Image == null)
                    {
                        Failed = true;
                        return;
                    }

                    if (Texture == null)
                    {
                        using (MemoryStream Stream = new MemoryStream(Image))
                        {
                            Texture2D Full = Texture2D.FromStream(Device, Stream);
                            Texture = WithMips(Device, Full);
                        }
                    }
                }

            }
            catch (Exception)
            {
                Failed = true;
            }
        }

        static Tuft Load(string PathName, out byte[] Image)
        {
            Image = null;
            byte[] FileBytes = File.ReadAllBytes(PathName);
            if (FileBytes.Length < 20 || BitConverter.ToUInt32(FileBytes, 0) != 0x46546C67)
            {
                return null;
            }

            int Offset = 12;
            byte[] Json = null;
            byte[] Bin = null;
            while (Offset + 8 <= FileBytes.Length)
            {
                int Length = BitConverter.ToInt32(FileBytes, Offset);
                int Kind = BitConverter.ToInt32(FileBytes, Offset + 4);
                Offset += 8;
                if (Length < 0 || Offset + Length > FileBytes.Length)
                {
                    break;
                }

                if (Kind == 0x4E4F534A)
                {
                    Json = new byte[Length];
                    Buffer.BlockCopy(FileBytes, Offset, Json, 0, Length);
                }
                else if (Kind == 0x004E4942)
                {
                    Bin = new byte[Length];
                    Buffer.BlockCopy(FileBytes, Offset, Bin, 0, Length);
                }

                Offset += Length;
            }

            if (Json == null || Bin == null)
            {
                return null;
            }

            int End = Json.Length;
            while (End > 0 && Json[End - 1] == 0)
            {
                End--;
            }

            using (JsonDocument Document = JsonDocument.Parse(Json.AsMemory(0, End)))
            {
                JsonElement Root = Document.RootElement;
                JsonElement Prim = Root.GetProperty("meshes")[0].GetProperty("primitives")[0];
                JsonElement Attr = Prim.GetProperty("attributes");
                JsonElement Accessors = Root.GetProperty("accessors");
                JsonElement Views = Root.GetProperty("bufferViews");
                float[] Position = ReadFloats(Bin, Accessors, Views, Attr.GetProperty("POSITION").GetInt32(), 3);
                float[] Normal = ReadFloats(Bin, Accessors, Views, Attr.GetProperty("NORMAL").GetInt32(), 3);
                float[] Uv = ReadFloats(Bin, Accessors, Views, Attr.GetProperty("TEXCOORD_0").GetInt32(), 2);
                int[] Index = ReadIndices(Bin, Accessors, Views, Prim.GetProperty("indices").GetInt32());
                if (Position == null || Normal == null || Uv == null || Index == null)
                {
                    return null;
                }

                Tuft Mesh = new Tuft();
                int Count = Position.Length / 3;
                Mesh.Position = new Vector3[Count];
                Mesh.Normal = new Vector3[Count];
                Mesh.Uv = new Vector2[Count];
                Mesh.Index = Index;
                Mesh.MinY = float.MaxValue;
                Mesh.MaxY = float.MinValue;
                for (int i = 0; i < Count; i++)
                {
                    Vector3 Point = new Vector3(Position[i * 3], Position[i * 3 + 1], Position[i * 3 + 2]);
                    Mesh.Position[i] = Point;
                    Mesh.Normal[i] = new Vector3(Normal[i * 3], Normal[i * 3 + 1], Normal[i * 3 + 2]);
                    Mesh.Uv[i] = new Vector2(Uv[i * 2], Uv[i * 2 + 1]);
                    if (Point.Y < Mesh.MinY)
                    {
                        Mesh.MinY = Point.Y;
                    }

                    if (Point.Y > Mesh.MaxY)
                    {
                        Mesh.MaxY = Point.Y;
                    }
                }

                Image = ReadImage(Bin, Root, Views);
                return Mesh;
            }
        }

        static Texture2D WithMips(GraphicsDevice Device, Texture2D Source)
        {
            int LevelW = Source.Width;
            int LevelH = Source.Height;
            Texture2D Mipped = new Texture2D(Device, LevelW, LevelH, true, SurfaceFormat.Color);
            Color[] Pixels = new Color[LevelW * LevelH];
            Source.GetData(Pixels);
            Source.Dispose();
            int Level = 0;
            while (true)
            {
                Mipped.SetData(Level, null, Pixels, 0, LevelW * LevelH);
                if (LevelW == 1 && LevelH == 1)
                {
                    return Mipped;
                }

                int NextW = Math.Max(1, LevelW / 2);
                int NextH = Math.Max(1, LevelH / 2);
                Color[] Next = new Color[NextW * NextH];
                for (int Y = 0; Y < NextH; Y++)
                {
                    int Y0 = Y * 2;
                    int Y1 = Math.Min(Y0 + 1, LevelH - 1);
                    for (int X = 0; X < NextW; X++)
                    {
                        int X0 = X * 2;
                        int X1 = Math.Min(X0 + 1, LevelW - 1);
                        Color A = Pixels[Y0 * LevelW + X0];
                        Color B = Pixels[Y0 * LevelW + X1];
                        Color C = Pixels[Y1 * LevelW + X0];
                        Color D = Pixels[Y1 * LevelW + X1];
                        Next[Y * NextW + X] = new Color(
                            (A.R + B.R + C.R + D.R) >> 2,
                            (A.G + B.G + C.G + D.G) >> 2,
                            (A.B + B.B + C.B + D.B) >> 2,
                            (A.A + B.A + C.A + D.A) >> 2);
                    }
                }

                Pixels = Next;
                LevelW = NextW;
                LevelH = NextH;
                Level++;
            }
        }

        static byte[] ReadImage(byte[] Bin, JsonElement Root, JsonElement Views)
        {
            if (!Root.TryGetProperty("images", out JsonElement Images))
            {
                return null;
            }

            byte[] Fallback = null;
            for (int i = 0; i < Images.GetArrayLength(); i++)
            {
                JsonElement Image = Images[i];
                if (!Image.TryGetProperty("bufferView", out JsonElement ViewIndex))
                {
                    continue;
                }

                byte[] Bytes = ViewBytes(Bin, Views[ViewIndex.GetInt32()]);
                string Name = Image.TryGetProperty("name", out JsonElement Named) ? Named.GetString() : string.Empty;
                if (!string.IsNullOrEmpty(Name) && Name.IndexOf("Normal", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    continue;
                }

                if (Bytes != null && (Fallback == null || Bytes.Length > Fallback.Length))
                {
                    Fallback = Bytes;
                }
            }

            return Fallback;
        }

        static byte[] ViewBytes(byte[] Bin, JsonElement View)
        {
            int Offset = View.TryGetProperty("byteOffset", out JsonElement At) ? At.GetInt32() : 0;
            int Length = View.GetProperty("byteLength").GetInt32();
            if (Offset < 0 || Length <= 0 || Offset + Length > Bin.Length)
            {
                return null;
            }

            byte[] Bytes = new byte[Length];
            Buffer.BlockCopy(Bin, Offset, Bytes, 0, Length);
            return Bytes;
        }

        static float[] ReadFloats(byte[] Bin, JsonElement Accessors, JsonElement Views, int Index, int Components)
        {
            JsonElement Accessor = Accessors[Index];
            if (Accessor.GetProperty("componentType").GetInt32() != 5126)
            {
                return null;
            }

            int Offset = AccessorOffset(Accessor, Views);
            int Count = Accessor.GetProperty("count").GetInt32();
            int Stride = StrideOf(Views[Accessor.GetProperty("bufferView").GetInt32()], Components * 4);
            float[] Out = new float[Count * Components];
            for (int i = 0; i < Count; i++)
            {
                int At = Offset + i * Stride;
                for (int c = 0; c < Components; c++)
                {
                    Out[i * Components + c] = BitConverter.ToSingle(Bin, At + c * 4);
                }
            }

            return Out;
        }

        static int[] ReadIndices(byte[] Bin, JsonElement Accessors, JsonElement Views, int Index)
        {
            JsonElement Accessor = Accessors[Index];
            int Type = Accessor.GetProperty("componentType").GetInt32();
            int Offset = AccessorOffset(Accessor, Views);
            int Count = Accessor.GetProperty("count").GetInt32();
            int Width = Type == 5125 ? 4 : 2;
            int Stride = StrideOf(Views[Accessor.GetProperty("bufferView").GetInt32()], Width);
            int[] Out = new int[Count];
            for (int i = 0; i < Count; i++)
            {
                int At = Offset + i * Stride;
                if (Type == 5125)
                {
                    Out[i] = BitConverter.ToInt32(Bin, At);
                }
                else if (Type == 5123)
                {
                    Out[i] = BitConverter.ToUInt16(Bin, At);
                }
                else
                {
                    Out[i] = Bin[At];
                }
            }

            return Out;
        }

        static int AccessorOffset(JsonElement Accessor, JsonElement Views)
        {
            JsonElement View = Views[Accessor.GetProperty("bufferView").GetInt32()];
            int Offset = View.TryGetProperty("byteOffset", out JsonElement At) ? At.GetInt32() : 0;
            if (Accessor.TryGetProperty("byteOffset", out JsonElement Extra))
            {
                Offset += Extra.GetInt32();
            }

            return Offset;
        }

        static int StrideOf(JsonElement View, int Packed)
        {
            if (View.TryGetProperty("byteStride", out JsonElement Stride) && Stride.GetInt32() > 0)
            {
                return Stride.GetInt32();
            }

            return Packed;
        }
    }
}
