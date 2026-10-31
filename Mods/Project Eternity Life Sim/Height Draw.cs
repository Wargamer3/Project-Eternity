using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class HeightDraw
    {
        static Effect Effect;
        static Texture2D Noise;
        static Texture2D ColorAtlas;
        static Texture2D NormalAtlas;
        static Texture2D RoughAtlas;
        static bool Failed;
        static VertexBuffer Mesh;
        static IndexBuffer MeshIndices;
        static HeightVertex[] ScratchVerts = Array.Empty<HeightVertex>();
        static int[] ScratchIndices = Array.Empty<int>();
        static Piece[] Pieces = Array.Empty<Piece>();
        static int PieceCount;
        static int Triangles;

        struct Piece
        {
            public MapInfo Info;
            public int Revision;
            public float[] Shade;
            public float[] Open;
            public int Wet;
        }

        public static void Draw(GraphicsDevice Device, MapInfo Info, Matrix View, Matrix Projection)
        {
            if (Device == null || Info == null || Info.Heights == null || Info.Heights.DrawVertices == null || Info.Heights.DrawVertices.Length == 0)
            {
                return;
            }

            Ensure(Device);
            if (Failed || Effect == null)
            {
                return;
            }

            ShadeBed(Info);
            BindFrame(Device, View, Projection, ChunkOrigin.Shift(Info));
            foreach (EffectPass Pass in Effect.CurrentTechnique.Passes)
            {
                Pass.Apply();
                Device.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    Info.Heights.DrawVertices,
                    0,
                    Info.Heights.DrawVertices.Length,
                    Info.Heights.DrawIndices,
                    0,
                    Info.Heights.DrawIndices.Length / 3,
                    HeightVertex.Declaration);
            }
        }

        public static void DrawDepth(GraphicsDevice Device, MapInfo Info, Matrix View, Matrix Projection)
        {
            if (Device == null || Info == null || Info.Heights == null || Info.Heights.DrawVertices == null || Info.Heights.DrawVertices.Length == 0)
            {
                return;
            }

            Ensure(Device);
            if (Failed || Effect == null || Effect.Techniques["Depth"] == null)
            {
                return;
            }

            Effect.Parameters["WorldViewProj"].SetValue(View * Projection);
            Effect.Parameters["ChunkOrigin"].SetValue(ChunkOrigin.Shift(Info));
            Effect.CurrentTechnique = Effect.Techniques["Depth"];
            Device.BlendState = BlendState.Opaque;
            Device.DepthStencilState = DepthStencilState.Default;
            Device.RasterizerState = RasterizerState.CullNone;
            foreach (EffectPass Pass in Effect.CurrentTechnique.Passes)
            {
                Pass.Apply();
                Device.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    Info.Heights.DrawVertices,
                    0,
                    Info.Heights.DrawVertices.Length,
                    Info.Heights.DrawIndices,
                    0,
                    Info.Heights.DrawIndices.Length / 3,
                    HeightVertex.Declaration);
            }

            Effect.CurrentTechnique = Effect.Techniques["Height"];
        }

        public static void DrawLoaded(GraphicsDevice Device, IList<MapInfo> Maps, Matrix View, Matrix Projection)
        {
            if (Device == null)
            {
                return;
            }

            Ensure(Device);
            if (Failed || Effect == null || !Hold(Device, Maps))
            {
                return;
            }

            BindFrame(Device, View, Projection, Vector3.Zero);
            Device.SetVertexBuffer(Mesh);
            Device.Indices = MeshIndices;
            foreach (EffectPass Pass in Effect.CurrentTechnique.Passes)
            {
                Pass.Apply();
                Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, Triangles);
            }
        }

        public static bool DrawDepthLoaded(GraphicsDevice Device, Matrix View, Matrix Projection)
        {
            if (Device == null || Triangles < 1 || Mesh == null || MeshIndices == null)
            {
                return false;
            }

            Ensure(Device);
            if (Failed || Effect == null || Effect.Techniques["Depth"] == null)
            {
                return false;
            }

            Effect.Parameters["WorldViewProj"].SetValue(View * Projection);
            Effect.Parameters["ChunkOrigin"].SetValue(Vector3.Zero);
            Effect.CurrentTechnique = Effect.Techniques["Depth"];
            Device.BlendState = BlendState.Opaque;
            Device.DepthStencilState = DepthStencilState.Default;
            Device.RasterizerState = RasterizerState.CullNone;
            Device.SetVertexBuffer(Mesh);
            Device.Indices = MeshIndices;
            foreach (EffectPass Pass in Effect.CurrentTechnique.Passes)
            {
                Pass.Apply();
                Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, Triangles);
            }

            Effect.CurrentTechnique = Effect.Techniques["Height"];
            return true;
        }

        static bool Hold(GraphicsDevice Device, IList<MapInfo> Maps)
        {
            if (Matches(Maps))
            {
                return Triangles > 0;
            }

            int Chunks = 0;
            int VertexCount = 0;
            int IndexCount = 0;
            if (Maps != null)
            {
                for (int i = 0; i < Maps.Count; i++)
                {
                    HeightField Field = FieldOf(Maps[i]);
                    if (Field == null)
                    {
                        continue;
                    }

                    Chunks++;
                    VertexCount += Field.DrawVertices.Length;
                    IndexCount += Field.DrawIndices.Length;
                }
            }

            PieceCount = 0;
            Triangles = 0;
            if (Chunks == 0 || VertexCount == 0 || IndexCount < 3)
            {
                return false;
            }

            if (Pieces.Length < Chunks)
            {
                Pieces = new Piece[Chunks];
            }

            if (ScratchVerts.Length < VertexCount)
            {
                ScratchVerts = new HeightVertex[VertexCount];
            }

            if (ScratchIndices.Length < IndexCount)
            {
                ScratchIndices = new int[IndexCount];
            }

            int VertexWrite = 0;
            int IndexWrite = 0;
            for (int i = 0; i < Maps.Count; i++)
            {
                MapInfo Info = Maps[i];
                HeightField Field = FieldOf(Info);
                if (Field == null)
                {
                    continue;
                }

                float[] Shade = SunShade.For(Info);
                float[] Open = GroundOpen.For(Info);
                Vector3 Shift = ChunkOrigin.Shift(Info);
                HeightVertex[] Source = Field.DrawVertices;
                bool HasWater = Info.Water != null && Info.Water.Count > 0;
                float Rain = Climate.Now.WetGround;
                for (int v = 0; v < Source.Length; v++)
                {
                    HeightVertex Vertex = Source[v];
                    float WorldX = Vertex.Position.X + Shift.X;
                    float WorldZ = Vertex.Position.Z + Shift.Z;
                    Vertex.Position += Shift;
                    Vertex.Wet = Soaked(WetAt(Info, Field, v, HasWater), Rain, WorldX, WorldZ);
                    Vertex.Shade = v < Shade.Length ? Shade[v] : 1f;
                    Vertex.Open = v < Open.Length ? Open[v] : 1f;
                    ScratchVerts[VertexWrite + v] = Vertex;
                }

                int[] SourceIndices = Field.DrawIndices;
                for (int n = 0; n < SourceIndices.Length; n++)
                {
                    ScratchIndices[IndexWrite + n] = SourceIndices[n] + VertexWrite;
                }

                Pieces[PieceCount] = new Piece
                {
                    Info = Info,
                    Revision = Field.Revision,
                    Shade = Shade,
                    Open = Open,
                    Wet = WetKey(Info)
                };
                PieceCount++;
                VertexWrite += Source.Length;
                IndexWrite += SourceIndices.Length;
            }

            if (Mesh == null || Mesh.VertexCount != VertexWrite)
            {
                Mesh?.Dispose();
                Mesh = new VertexBuffer(Device, HeightVertex.Declaration, VertexWrite, BufferUsage.WriteOnly);
            }

            if (MeshIndices == null || MeshIndices.IndexCount != IndexWrite)
            {
                MeshIndices?.Dispose();
                MeshIndices = new IndexBuffer(Device, IndexElementSize.ThirtyTwoBits, IndexWrite, BufferUsage.WriteOnly);
            }

            Mesh.SetData(ScratchVerts, 0, VertexWrite);
            MeshIndices.SetData(ScratchIndices, 0, IndexWrite);
            Triangles = IndexWrite / 3;
            return Triangles > 0;
        }

        static bool Matches(IList<MapInfo> Maps)
        {
            int Count = 0;
            if (Maps != null)
            {
                for (int i = 0; i < Maps.Count; i++)
                {
                    MapInfo Info = Maps[i];
                    HeightField Field = FieldOf(Info);
                    if (Field == null)
                    {
                        continue;
                    }

                    if (Count >= PieceCount)
                    {
                        return false;
                    }

                    Piece Held = Pieces[Count];
                    if (Held.Info != Info || Held.Revision != Field.Revision || Held.Shade != SunShade.For(Info) || Held.Open != GroundOpen.For(Info) || Held.Wet != WetKey(Info))
                    {
                        return false;
                    }

                    Count++;
                }
            }

            return Count == PieceCount;
        }

        static HeightField FieldOf(MapInfo Info)
        {
            HeightField Field = Info == null ? null : Info.Heights;
            if (Field == null || Field.DrawVertices == null || Field.DrawVertices.Length == 0 || Field.DrawIndices == null || Field.DrawIndices.Length < 3)
            {
                return null;
            }

            return Field;
        }

        static int WetKey(MapInfo Info)
        {
            int Key = Info == null || Info.Water == null ? 1 : Info.Water.Count + 1;
            int Volumes = Info == null || Info.Water == null ? 0 : Info.Water.Count;
            for (int i = 0; i < Volumes; i++)
            {
                WaterVolume Volume = Info.Water[i];
                if (Volume == null)
                {
                    continue;
                }

                Key = Mix(Key, (int)Volume.Kind);
                Key = Mix(Key, Volume.Start.X);
                Key = Mix(Key, Volume.Start.Y);
                Key = Mix(Key, Volume.Start.Z);
                Key = Mix(Key, Volume.End.X);
                Key = Mix(Key, Volume.End.Y);
                Key = Mix(Key, Volume.End.Z);
                Key = Mix(Key, Volume.Radius);
                Key = Mix(Key, Volume.Width);
                Key = Mix(Key, Volume.Depth);
                Key = Mix(Key, Volume.Cell);
                int[] Cells = Volume.LakeCells;
                Key = Mix(Key, Cells == null ? 0 : Cells.Length);
                if (Cells != null)
                {
                    for (int c = 0; c < Cells.Length; c++)
                    {
                        Key = Mix(Key, Cells[c]);
                    }
                }
            }

            Key = Mix(Key, (int)(Climate.Now.WetGround * 20f));
            Key = Mix(Key, RoofStamp(Info));
            return Key;
        }

        static int RoofStamp(MapInfo Info)
        {
            if (Info == null || Info.Buildings == null)
            {
                return 0;
            }

            int Key = Info.Buildings.Count;
            for (int i = 0; i < Info.Buildings.Count; i++)
            {
                BuildingItem Box = Info.Buildings[i];
                if (Box == null || Box.Kind != BuildingKind.Floor || Box.Floor <= 0)
                {
                    continue;
                }

                Key = Mix(Key, Box.Floor);
                Key = Mix(Key, Box.Position.X);
                Key = Mix(Key, Box.Position.Y);
                Key = Mix(Key, Box.Width);
                Key = Mix(Key, Box.Length);
            }

            return Key;
        }

        static float Soaked(float Water, float Rain, float WorldX, float WorldZ)
        {
            if (Water >= 0.5f)
            {
                return 1f;
            }

            if (Rain <= 0.02f || WeatherDraw.CeilingAbove(WorldX, WorldZ, 0))
            {
                return 0f;
            }

            return Rain * ForestScatter.Shade(WorldX, WorldZ);
        }

        static int Mix(int Key, int Value)
        {
            return unchecked(Key * 31 + Value);
        }

        static int Mix(int Key, float Value)
        {
            return Mix(Key, BitConverter.SingleToInt32Bits(Value));
        }

        static float WetAt(MapInfo Info, HeightField Field, int Index, bool HasWater)
        {
            if (!HasWater)
            {
                return 0f;
            }

            int X = Index % Field.SamplesX;
            int Z = Index / Field.SamplesX;
            float LocalX = X * Field.Spacing;
            float LocalY = Z * Field.Spacing;
            for (int v = 0; v < Info.Water.Count; v++)
            {
                WaterVolume Volume = Info.Water[v];
                if (Volume != null && Volume.Covers(Field, LocalX, LocalY))
                {
                    return 1f;
                }
            }

            return 0f;
        }

        static void BindFrame(GraphicsDevice Device, Matrix View, Matrix Projection, Vector3 Origin)
        {
            Effect.Parameters["WorldViewProj"].SetValue(View * Projection);
            Effect.Parameters["ChunkOrigin"].SetValue(Origin);
            Effect.Parameters["TileMeters"].SetValue(26f);
            Matrix Inverse = Matrix.Invert(View);
            EffectParameter Eye = Effect.Parameters["Eye"];
            if (Eye != null)
            {
                Eye.SetValue(Inverse.Translation);
            }

            EffectParameter Sun = Effect.Parameters["SunDir"];
            if (Sun != null)
            {
                Sun.SetValue(SkyDraw.SunDirection);
            }

            EffectParameter Fog = Effect.Parameters["FogColor"];
            if (Fog != null)
            {
                Fog.SetValue(SkyDraw.FogColor);
            }

            CharacterShadow.Apply(Effect);
            LampGlow.Apply(Effect);
            EffectTechnique Surface = PictureOptions.ShadowCascades <= 0 ? Effect.Techniques["Plain"] : null;
            Effect.CurrentTechnique = Surface ?? Effect.Techniques["Height"];
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

            EffectParameter Rain = Effect.Parameters["RainWet"];
            if (Rain != null)
            {
                Rain.SetValue(Climate.Now.WetGround);
            }

            EffectParameter Bolt = Effect.Parameters["Flash"];
            if (Bolt != null)
            {
                Bolt.SetValue(WeatherDraw.Light);
            }

            EffectParameter Disks = Effect.Parameters["Contacts"];
            if (Disks != null)
            {
                Disks.SetValue(Occlusion.Contacts);
            }

            EffectParameter DiskCount = Effect.Parameters["ContactCount"];
            if (DiskCount != null)
            {
                DiskCount.SetValue(Occlusion.Count);
            }

            Bind("Noise", Noise);
            Bind("ColorAtlas", ColorAtlas);
            Bind("NormalAtlas", NormalAtlas);
            Bind("RoughAtlas", RoughAtlas);
            Device.BlendState = BlendState.Opaque;
            Device.DepthStencilState = DepthStencilState.Default;
            Device.RasterizerState = RasterizerState.CullNone;
            for (int Slot = 0; Slot < 4; Slot++)
            {
                Device.SamplerStates[Slot] = SamplerState.LinearWrap;
            }
        }

        static void ShadeBed(MapInfo Info)
        {
            HeightField Field = Info.Heights;
            HeightVertex[] Vertices = Field.DrawVertices;
            if (Vertices == null || Field.SamplesX < 1)
            {
                return;
            }

            bool HasWater = Info.Water != null && Info.Water.Count > 0;
            float[] Shade = SunShade.For(Info);
            float[] Open = GroundOpen.For(Info);
            float Rain = Climate.Now.WetGround;
            Vector3 Shift = ChunkOrigin.Shift(Info);
            for (int i = 0; i < Vertices.Length; i++)
            {
                HeightVertex Vertex = Vertices[i];
                Vertex.Wet = Soaked(WetAt(Info, Field, i, HasWater), Rain, Vertex.Position.X + Shift.X, Vertex.Position.Z + Shift.Z);
                Vertex.Shade = i < Shade.Length ? Shade[i] : 1f;
                Vertex.Open = i < Open.Length ? Open[i] : 1f;
                Vertices[i] = Vertex;
            }
        }

        static void Ensure(GraphicsDevice Device)
        {
            if (Effect != null || Failed)
            {
                return;
            }

            try
            {
                string PathName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Life Sim", "Shaders", "HeightTriplanar.mgfx");
                if (!File.Exists(PathName))
                {
                    PathName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mods", "HeightTriplanar.mgfx");
                }

                if (!File.Exists(PathName))
                {
                    Failed = true;
                    return;
                }

                Effect = new Effect(Device, File.ReadAllBytes(PathName));
                ColorAtlas = Sheet(Device, new string[]
                {
                    Path.Combine("Grass", "grass_ground_diff_4k.jpg"),
                    Path.Combine("Dirt", "dirt_diff_4k.jpg"),
                    Path.Combine("Small Rocks", "gray_rocks_diff_4k.jpg"),
                    Path.Combine("Sand", "dense_sand_diff_4k.jpg"),
                    Path.Combine("Beach Sand", "coast_sand_01_diff_4k.jpg"),
                    Path.Combine("Damp Beach Sand", "damp_beach_sand_diff_4k.jpg"),
                }, new Color(120, 110, 80));
                NormalAtlas = Sheet(Device, new string[]
                {
                    Path.Combine("Grass", "grass_ground_nor_gl_4k.jpg"),
                    Path.Combine("Dirt", "dirt_nor_gl_4k.jpg"),
                    Path.Combine("Small Rocks", "gray_rocks_nor_gl_4k.jpg"),
                    Path.Combine("Sand", "dense_sand_nor_gl_4k.jpg"),
                    Path.Combine("Beach Sand", "coast_sand_01_nor_gl_4k.jpg"),
                    Path.Combine("Damp Beach Sand", "damp_beach_sand_nor_gl_4k.jpg"),
                }, new Color(128, 128, 255));
                RoughAtlas = Sheet(Device, new string[]
                {
                    Path.Combine("Grass", "grass_ground_rough_4k.jpg"),
                    Path.Combine("Dirt", "dirt_rough_4k.jpg"),
                    Path.Combine("Small Rocks", "gray_rocks_rough_4k.jpg"),
                    Path.Combine("Sand", "dense_sand_rough_4k.jpg"),
                    Path.Combine("Beach Sand", "coast_sand_01_rough_4k.jpg"),
                    Path.Combine("Damp Beach Sand", "damp_beach_sand_rough_4k.jpg"),
                }, new Color(210, 210, 210));
                Noise = new Texture2D(Device, 64, 64, false, SurfaceFormat.Color);
                Color[] Pixels = new Color[64 * 64];
                for (int Y = 0; Y < 64; Y++)
                {
                    for (int X = 0; X < 64; X++)
                    {
                        float Value = Hash(X, Y);
                        byte Byte = (byte)(150 + Value * 80f);
                        Pixels[Y * 64 + X] = new Color(Byte, Byte, Byte);
                    }
                }

                Noise.SetData(Pixels);
            }
            catch (Exception)
            {
                Failed = true;
            }
        }

        static void Bind(string Name, Texture2D Map)
        {
            EffectParameter Parameter = Effect.Parameters[Name];
            if (Parameter != null && Map != null)
            {
                Parameter.SetValue(Map);
            }
        }

        static Texture2D Sheet(GraphicsDevice Device, string[] Files, Color Fallback)
        {
            const int Cell = 1024;
            const int Columns = 3;
            const int Rows = 2;
            int Width = Columns * Cell;
            int Height = Rows * Cell;
            Color[] SheetPixels = new Color[Width * Height];
            for (int i = 0; i < SheetPixels.Length; i++)
            {
                SheetPixels[i] = Fallback;
            }

            int Count = Math.Min(Files.Length, Columns * Rows);
            for (int i = 0; i < Count; i++)
            {
                Color[] Pixels = CellPixels(Device, Files[i], Fallback, Cell);
                int Column = i % Columns;
                int Row = i / Columns;
                for (int Y = 0; Y < Cell; Y++)
                {
                    Array.Copy(Pixels, Y * Cell, SheetPixels, (Row * Cell + Y) * Width + Column * Cell, Cell);
                }
            }

            return Mips(Device, SheetPixels, Width, Height);
        }

        static Color[] CellPixels(GraphicsDevice Device, string Relative, Color Fallback, int Cell)
        {
            const int Pad = 32;
            int Inner = Cell - Pad * 2;
            Texture2D PhotoMap = Photo(Device, Relative, Fallback);
            Color[] Source = new Color[PhotoMap.Width * PhotoMap.Height];
            PhotoMap.GetData(Source);
            int Width = PhotoMap.Width;
            int Height = PhotoMap.Height;
            PhotoMap.Dispose();
            Color[] Tile = new Color[Cell * Cell];
            for (int i = 0; i < Tile.Length; i++)
            {
                Tile[i] = Fallback;
            }

            for (int Y = 0; Y < Inner; Y++)
            {
                int SY = Math.Min(Height - 1, Y * Height / Inner);
                int Row = (Y + Pad) * Cell;
                for (int X = 0; X < Inner; X++)
                {
                    int SX = Math.Min(Width - 1, X * Width / Inner);
                    Tile[Row + X + Pad] = Source[SY * Width + SX];
                }
            }

            for (int Y = 0; Y < Inner; Y++)
            {
                int Row = (Y + Pad) * Cell;
                for (int X = 0; X < Pad; X++)
                {
                    Tile[Row + X] = Tile[Row + Inner + X];
                    Tile[Row + Pad + Inner + X] = Tile[Row + Pad + X];
                }
            }

            for (int Y = 0; Y < Pad; Y++)
            {
                Array.Copy(Tile, (Inner + Y) * Cell, Tile, Y * Cell, Cell);
                Array.Copy(Tile, (Pad + Y) * Cell, Tile, (Pad + Inner + Y) * Cell, Cell);
            }

            return Tile;
        }

        static Texture2D Photo(GraphicsDevice Device, string Relative, Color Fallback)
        {
            string PathOnDisk = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Life Sim", "Assets", "Textures", Relative);
            try
            {
                if (!File.Exists(PathOnDisk))
                {
                    return Solid(Device, Fallback);
                }

                Texture2D Full;
                using (FileStream Stream = File.OpenRead(PathOnDisk))
                {
                    Full = Texture2D.FromStream(Device, Stream);
                }

                Color[] Pixels = new Color[Full.Width * Full.Height];
                Full.GetData(Pixels);
                int Width = Full.Width;
                int Height = Full.Height;
                Full.Dispose();
                while (Width > 1024 || Height > 1024)
                {
                    int NextW;
                    int NextH;
                    Pixels = Box(Pixels, Width, Height, out NextW, out NextH);
                    Width = NextW;
                    Height = NextH;
                }

                return Mips(Device, Pixels, Width, Height);
            }
            catch (Exception)
            {
                return Solid(Device, Fallback);
            }
        }

        static Texture2D Solid(GraphicsDevice Device, Color Ink)
        {
            Texture2D Tex = new Texture2D(Device, 1, 1, false, SurfaceFormat.Color);
            Tex.SetData(new[] { Ink });
            return Tex;
        }

        static Texture2D Mips(GraphicsDevice Device, Color[] Top, int Width, int Height)
        {
            Texture2D Tex = new Texture2D(Device, Width, Height, true, SurfaceFormat.Color);
            Color[] Level = Top;
            int W = Width;
            int H = Height;
            for (int Index = 0; Index < Tex.LevelCount; Index++)
            {
                Tex.SetData(Index, null, Level, 0, Level.Length);
                if (W <= 1 && H <= 1)
                {
                    break;
                }

                int NextW;
                int NextH;
                Level = Box(Level, W, H, out NextW, out NextH);
                W = NextW;
                H = NextH;
            }

            return Tex;
        }

        static Color[] Box(Color[] Source, int Width, int Height, out int OutW, out int OutH)
        {
            OutW = Math.Max(1, Width / 2);
            OutH = Math.Max(1, Height / 2);
            Color[] Dest = new Color[OutW * OutH];
            for (int Y = 0; Y < OutH; Y++)
            {
                for (int X = 0; X < OutW; X++)
                {
                    int R = 0;
                    int G = 0;
                    int B = 0;
                    int N = 0;
                    for (int FY = 0; FY < 2; FY++)
                    {
                        int SY = Math.Min(Height - 1, Y * 2 + FY);
                        for (int FX = 0; FX < 2; FX++)
                        {
                            int SX = Math.Min(Width - 1, X * 2 + FX);
                            Color Pixel = Source[SY * Width + SX];
                            R += Pixel.R;
                            G += Pixel.G;
                            B += Pixel.B;
                            N++;
                        }
                    }

                    Dest[Y * OutW + X] = new Color((byte)(R / N), (byte)(G / N), (byte)(B / N));
                }
            }

            return Dest;
        }

        static float Hash(int X, int Y)
        {
            int N = X * 374761393 + Y * 668265263;
            N = (N ^ (N >> 13)) * 1274126177;
            N = N ^ (N >> 16);
            return (N & 255) / 255f;
        }
    }
}
