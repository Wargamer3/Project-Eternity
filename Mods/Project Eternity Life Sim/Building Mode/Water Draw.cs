using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class WaterDraw
    {
        public static float Time;

        static Effect Effect;
        static Texture2D Sheet;
        static Texture2D Caustics;
        static Texture2D White;
        static bool Failed;
        static DepthStencilState CoverDepth;
        static RasterizerState CoverRaster;
        static readonly Vector4[] RingUpload = new Vector4[8];
        static readonly Vector4[] WakeUpload = new Vector4[WaterTrails.SlotCount];
        static readonly Dictionary<object, WaterCache> MeshCache = new Dictionary<object, WaterCache>();

        class WaterCache
        {
            public int Stamp;
            public List<WaterSheet> Sheets;
        }

        public static void AdvanceClock(float Dt)
        {
            Time += Dt;
            WaterTrails.Age(Time);
        }

        public static void All(GraphicsDevice Device, Microsoft.Xna.Framework.Content.ContentManager Content, IList<WaterVolume> Volumes, WaterVolume Preview, WaterTracks Tracks, Matrix View, Matrix Projection, Vector3 Shift, Vector3 Eye, HeightField Ground, MapInfo Chunk = null)
        {
            if (Device == null)
            {
                return;
            }

            Ensure(Device, Content);
            if (Failed || Effect == null)
            {
                return;
            }

            DrawList(Device, SheetsFor(Volumes, Ground, Chunk), Tracks, View, Projection, Shift, Eye, false);
            if (Preview != null)
            {
                List<WaterVolume> Ghost = new List<WaterVolume>();
                Ghost.Add(Preview);
                DrawList(Device, WaterMesh.Build(Ghost, Ground), null, View, Projection, Shift, Eye, true);
            }
        }

        static void DrawList(GraphicsDevice Device, List<WaterSheet> Sheets, WaterTracks Tracks, Matrix View, Matrix Projection, Vector3 Shift, Vector3 Eye, bool Ghost)
        {
            if (Sheets == null || Sheets.Count == 0)
            {
                return;
            }

            Matrix World = Matrix.CreateTranslation(Shift);
            int Rings = 0;
            int Wakes = 0;
            if (!Ghost)
            {
                Wakes = WaterTrails.Pack(new Vector2(Shift.X, Shift.Z), WakeUpload);
            }

            if (Tracks != null && !Ghost)
            {
                Rings = PackRings(Tracks);
            }

            Effect.Parameters["World"].SetValue(World);
            Effect.Parameters["WorldViewProj"].SetValue(World * View * Projection);
            Effect.Parameters["EyePosition"].SetValue(Matrix.Invert(View).Translation);
            SetColor("SunDir", SkyDraw.SunDirection);
            SetColor("SunColor", SkyDraw.SunTint);
            SetColor("FogColor", SkyDraw.FogColor);
            SetColor("Zenith", SkyDraw.ZenithColor);
            SetMap("Flipbook", Sheet);
            SetMap("CausticMap", Caustics);
            CharacterShadow.Apply(Effect);

            LampGlow.Apply(Effect);

            Effect.Parameters["Time"].SetValue(Time);
            EffectParameter Clouds = Effect.Parameters["Cloud"];
            if (Clouds != null)
            {
                Clouds.SetValue(SkyDraw.Cover);
            }

            EffectParameter CloudClock = Effect.Parameters["CloudTime"];
            if (CloudClock != null)
            {
                CloudClock.SetValue(SkyDraw.CloudClock);
            }
            Effect.Parameters["Ghost"].SetValue(Ghost ? 0.45f : 1f);
            Effect.Parameters["RingCount"].SetValue(Rings);
            Effect.Parameters["WakeCount"].SetValue(Wakes);
            Effect.Parameters["Rings"].SetValue(RingUpload);
            Effect.Parameters["Wakes"].SetValue(WakeUpload);
            CoverStates();
            Device.BlendState = BlendState.AlphaBlend;
            Device.SamplerStates[0] = SamplerState.LinearClamp;
            Device.SamplerStates[1] = SamplerState.LinearClamp;
            foreach (EffectPass Pass in Effect.CurrentTechnique.Passes)
            {
                Pass.Apply();
                Device.BlendState = BlendState.AlphaBlend;
                Device.DepthStencilState = CoverDepth;
                Device.RasterizerState = CoverRaster;
                for (int i = 0; i < Sheets.Count; i++)
                {
                    WaterSheet Surface = Sheets[i];
                    if (Surface.Vertices.Length == 0 || Surface.Indices.Length < 3)
                    {
                        continue;
                    }

                    Device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, Surface.Vertices, 0, Surface.Vertices.Length, Surface.Indices, 0, Surface.Indices.Length / 3, WaterVertex.Declaration);
                }
            }
        }

        static List<WaterSheet> SheetsFor(IList<WaterVolume> Volumes, HeightField Ground, MapInfo Chunk)
        {
            if (Volumes == null || Volumes.Count == 0)
            {
                return new List<WaterSheet>();
            }

            int Stamp = VolumeStamp(Volumes) ^ (Ground == null ? 0 : Ground.Revision * 17) ^ NeighborStamp(Chunk);
            WaterCache Cached;
            if (MeshCache.TryGetValue(Volumes, out Cached) && Cached.Stamp == Stamp)
            {
                return Cached.Sheets;
            }

            List<WaterVolume> Drawn = new List<WaterVolume>(Volumes.Count);
            for (int i = 0; i < Volumes.Count; i++)
            {
                if (Volumes[i] != null)
                {
                    Drawn.Add(Volumes[i]);
                }
            }

            Cached = new WaterCache();
            Cached.Stamp = Stamp;
            Cached.Sheets = WaterMesh.Build(Drawn, Ground, Chunk);
            MeshCache[Volumes] = Cached;
            return Cached.Sheets;
        }

        static int NeighborStamp(MapInfo Chunk)
        {
            if (Chunk == null)
            {
                return 0;
            }

            unchecked
            {
                int Hash = 17;
                Hash = MixNeighbor(Hash, Chunk, 1, 0);
                Hash = MixNeighbor(Hash, Chunk, -1, 0);
                Hash = MixNeighbor(Hash, Chunk, 0, 1);
                Hash = MixNeighbor(Hash, Chunk, 0, -1);
                return Hash;
            }
        }

        static int MixNeighbor(int Hash, MapInfo Chunk, int DX, int DZ)
        {
            MapInfo Other;
            if (!WorldStream.ByCoord.TryGetValue(WorldStream.Key(Chunk.ChunkX + DX, Chunk.ChunkZ + DZ), out Other))
            {
                return Hash * 31;
            }

            Hash = Hash * 31 + (Other.Water == null ? 0 : Other.Water.Count);
            Hash = Hash * 31 + (Other.Heights == null ? 0 : Other.Heights.Revision);
            return Hash;
        }

        static int VolumeStamp(IList<WaterVolume> Volumes)
        {
            unchecked
            {
                int Hash = Volumes.Count * 397;
                for (int i = 0; i < Volumes.Count; i++)
                {
                    WaterVolume Volume = Volumes[i];
                    if (Volume == null)
                    {
                        continue;
                    }

                    Hash = Hash * 31 + (int)Volume.Kind;
                    Hash = Hash * 31 + (int)(Volume.Start.X * 8f);
                    Hash = Hash * 31 + (int)(Volume.Start.Y * 8f);
                    Hash = Hash * 31 + (int)(Volume.Start.Z * 8f);
                    Hash = Hash * 31 + (int)(Volume.End.X * 8f);
                    Hash = Hash * 31 + (int)(Volume.End.Y * 8f);
                    Hash = Hash * 31 + (int)(Volume.End.Z * 8f);
                    Hash = Hash * 31 + (int)(Volume.Width * 8f);
                    Hash = Hash * 31 + (int)(Volume.Radius * 8f);
                    Hash = Hash * 31 + (Volume.LakeCells == null ? 0 : Volume.LakeCells.Length);
                }

                return Hash;
            }
        }

        static int PackRings(WaterTracks Tracks)
        {
            int Count = Math.Min(8, Tracks.Rings.Count);
            for (int i = 0; i < 8; i++)
            {
                if (i >= Count)
                {
                    RingUpload[i] = Vector4.Zero;
                    continue;
                }

                WaterRing Ring = Tracks.Rings[Tracks.Rings.Count - Count + i];
                float Age = WaterDraw.Time - Ring.Born;
                if (Age >= WaterPlace.RingLife)
                {
                    RingUpload[i] = Vector4.Zero;
                    continue;
                }

                float Radius = 6f + Age * 28f;
                float Fade = MathHelper.Clamp(1f - Age / WaterPlace.RingLife, 0f, 1f);
                RingUpload[i] = new Vector4(Ring.Center.X, Ring.Center.Y, Radius, Ring.Strength * Fade * 4f);
            }

            return Count;
        }

        static void SetMap(string Name, Texture2D Map)
        {
            EffectParameter Parameter = Effect.Parameters[Name];
            if (Parameter != null && Map != null)
            {
                Parameter.SetValue(Map);
            }
        }

        static void SetColor(string Name, Vector3 Value)
        {
            EffectParameter Parameter = Effect.Parameters[Name];
            if (Parameter != null)
            {
                Parameter.SetValue(Value);
            }
        }

        static void CoverStates()
        {
            if (CoverDepth != null)
            {
                return;
            }

            CoverDepth = new DepthStencilState();
            CoverDepth.DepthBufferEnable = true;
            CoverDepth.DepthBufferWriteEnable = true;
            CoverDepth.DepthBufferFunction = CompareFunction.LessEqual;
            CoverRaster = new RasterizerState();
            CoverRaster.CullMode = CullMode.None;
            CoverRaster.DepthBias = -0.0002f;
        }

        static void Ensure(GraphicsDevice Device, Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            if (Failed || Effect != null)
            {
                return;
            }

            try
            {
                if (White == null)
                {
                    White = new Texture2D(Device, 1, 1);
                    White.SetData(new[] { Color.White });
                }

                if (Content != null)
                {
                    Effect = Content.Load<Effect>("Shaders/Water Surface");
                    Sheet = TryContent(Content, "3D/Textures/WaterFlipbook");
                    Caustics = TryContent(Content, "3D/Textures/WaterCaustics");
                }

                if (Sheet == null)
                {
                    Sheet = FromPng(Device, Path.Combine("Content", "3D", "Textures", "WaterFlipbook.png"));
                }

                if (Caustics == null)
                {
                    Caustics = FromPng(Device, Path.Combine("Content", "3D", "Textures", "WaterCaustics.png"));
                }

                if (Effect == null)
                {
                    Failed = true;
                }
            }
            catch (Exception)
            {
                Failed = true;
            }
        }

        static Texture2D TryContent(Microsoft.Xna.Framework.Content.ContentManager Content, string Name)
        {
            try
            {
                return Content.Load<Texture2D>(Name);
            }
            catch (Exception)
            {
                return null;
            }
        }

        static Texture2D FromPng(GraphicsDevice Device, string Relative)
        {
            string PathOnDisk = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Relative);
            if (!File.Exists(PathOnDisk))
            {
                return null;
            }

            using (FileStream Stream = File.OpenRead(PathOnDisk))
            {
                return Texture2D.FromStream(Device, Stream);
            }
        }
    }
}
