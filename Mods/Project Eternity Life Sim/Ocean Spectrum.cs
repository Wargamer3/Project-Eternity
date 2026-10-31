using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    // Phillips spectrum on the CPU when the wind changes. The inverse FFT runs
    // on the GPU. Gameplay only reads the finished height map.
    static class OceanField
    {
        const int N = 64;
        const float G = 9.81f;
        const float TwoPi = (float)(Math.PI * 2.0);

        public static readonly Vector3 Sizes = new Vector3(1024f, 256f, 48f);
        public static Texture2D Far;
        public static Texture2D Mid;
        public static Texture2D Near;

        static readonly float[] Tile = { 1024f, 256f, 48f };
        static readonly float[] Weight = { 0.90f, 0.45f, 0.10f };
        static readonly float[] Chop = { 1f, 0.65f, 0.25f };
        static readonly float[] GaussR = new float[3 * N * N];
        static readonly float[] GaussI = new float[3 * N * N];
        static readonly float[] H0R = new float[3 * N * N];
        static readonly float[] H0I = new float[3 * N * N];
        static readonly float[][] Height = { new float[N * N], new float[N * N], new float[N * N] };
        static readonly Vector4[] SpectrumPixels = new Vector4[N * N];
        static readonly Vector4[] Readback = new Vector4[N * N];
        static readonly VertexPosition[] Quad =
        {
            new VertexPosition(new Vector3(-1f, 1f, 0f)),
            new VertexPosition(new Vector3(1f, 1f, 0f)),
            new VertexPosition(new Vector3(-1f, -1f, 0f)),
            new VertexPosition(new Vector3(1f, -1f, 0f)),
        };

        static bool Seeded;
        static bool Ready;
        static bool Shaped;
        static bool Pending;
        static bool SpectrumDirty;
        static bool FftFailed;
        static float SimTime = float.NaN;
        static float HeldRms;
        static float HeldLambda = 0.4f;
        static Vector2 ShapedWind;
        static Effect FftEffect;
        static Texture2D[] Spectrum;
        static RenderTarget2D[] Ping;
        static RenderTarget2D[] Pong;
        static RenderTarget2D[] Shown;
        static RenderTarget2D Sum8;
        static RenderTarget2D Sum1;

        public static float HeightAt(float WorldX, float WorldZ)
        {
            if (!Ready)
            {
                return Ocean.Level;
            }

            return Ocean.Level + Read(WorldX, WorldZ);
        }

        public static void Advance(Vector2 Wind, float Time)
        {
            float Tick = (float)(Math.Floor(Time * 30.0) / 30.0);
            if (Tick == SimTime)
            {
                return;
            }

            SimTime = Tick;
            if (!Seeded)
            {
                Seed();
            }

            float Span = Wind.Length();
            if (Span < 0.2f)
            {
                Span = 0.2f;
            }

            Vector2 Dir = Wind.LengthSquared() > 0.04f ? Wind / Wind.Length() : new Vector2(1f, 0.2f);
            float Speed = 3.2f + Span * 1.7f;
            float Length = Speed * Speed / G;
            float Sea = 0.45f + Span * Span * 0.1f;
            HeldRms = Sea * 0.25f;
            HeldLambda = Math.Min(1.05f, 0.25f + Span * 0.08f);
            if (!Shaped || Math.Abs(Wind.X - ShapedWind.X) > 0.15f || Math.Abs(Wind.Y - ShapedWind.Y) > 0.15f)
            {
                for (int c = 0; c < 3; c++)
                {
                    Shape(c, Dir, Length);
                }

                ShapedWind = Wind;
                Shaped = true;
                SpectrumDirty = true;
            }

            Pending = true;
        }

        public static void Upload(GraphicsDevice Device)
        {
            if (Device == null || !Pending || !EnsureGpu(Device))
            {
                return;
            }

            RenderTargetBinding[] Was = Device.GetRenderTargets();
            try
            {
                Device.BlendState = BlendState.Opaque;
                Device.DepthStencilState = DepthStencilState.None;
                Device.RasterizerState = RasterizerState.CullNone;
                if (SpectrumDirty)
                {
                    PushSpectrum();
                }

                FftEffect.Parameters["Time"].SetValue(SimTime);
                RunCascade(Device, 0, Shown[0]);
                RunCascade(Device, 1, Shown[1]);
                RunCascade(Device, 2, Shown[2]);
                Far = Shown[0];
                Mid = Shown[1];
                Near = Shown[2];
            }
            catch (Exception)
            {
                FftFailed = true;
                return;
            }
            finally
            {
                if (Was == null || Was.Length == 0)
                {
                    Device.SetRenderTarget(null);
                }
                else
                {
                    Device.SetRenderTargets(Was);
                }
            }

            ReadHeights();
            Pending = false;
            Ready = true;
        }

        static bool EnsureGpu(GraphicsDevice Device)
        {
            if (FftFailed)
            {
                return false;
            }

            if (FftEffect != null && Shown != null && !Shown[0].IsDisposed)
            {
                return true;
            }

            try
            {
                string PathName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Life Sim", "Shaders", "OceanFft.mgfx");
                if (!System.IO.File.Exists(PathName))
                {
                    FftFailed = true;
                    return false;
                }

                FftEffect = new Effect(Device, System.IO.File.ReadAllBytes(PathName));
                Spectrum = new Texture2D[3];
                Ping = new RenderTarget2D[3];
                Pong = new RenderTarget2D[3];
                Shown = new RenderTarget2D[3];
                for (int c = 0; c < 3; c++)
                {
                    Spectrum[c] = new Texture2D(Device, N, N, false, SurfaceFormat.Vector4);
                    Ping[c] = Target(Device, N);
                    Pong[c] = Target(Device, N);
                    Shown[c] = Target(Device, N);
                }

                Sum8 = Target(Device, 8);
                Sum1 = Target(Device, 1);
                SpectrumDirty = true;
                if (FftEffect.Techniques["Evolve"] == null)
                {
                    FftFailed = true;
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                FftFailed = true;
                return false;
            }
        }

        static RenderTarget2D Target(GraphicsDevice Device, int Size)
        {
            return new RenderTarget2D(Device, Size, Size, false, SurfaceFormat.Vector4, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
        }

        static void PushSpectrum()
        {
            for (int c = 0; c < 3; c++)
            {
                int Base = c * N * N;
                for (int i = 0; i < N * N; i++)
                {
                    SpectrumPixels[i] = new Vector4(H0R[Base + i], H0I[Base + i], 0f, 0f);
                }

                Spectrum[c].SetData(SpectrumPixels);
            }

            SpectrumDirty = false;
        }

        static void RunCascade(GraphicsDevice Device, int Cascade, RenderTarget2D Dest)
        {
            FftEffect.Parameters["Tile"].SetValue(Tile[Cascade]);
            FftEffect.Parameters["Spec"].SetValue(Spectrum[Cascade]);
            FftEffect.Parameters["TargetRms"].SetValue(HeldRms * Weight[Cascade]);
            FftEffect.Parameters["Fold"].SetValue(HeldLambda * Chop[Cascade]);
            FftEffect.Parameters["World"].SetValue(N / Tile[Cascade]);
            FftEffect.Parameters["FoamOn"].SetValue(Cascade == 2 ? 1f : 0f);

            Device.SetRenderTargets(Ping[0], Ping[1], Ping[2]);
            DrawTechnique(Device, "Evolve");

            Device.SetRenderTargets(Pong[0], Pong[1], Pong[2]);
            BindPing();
            DrawTechnique(Device, "BitH");
            SwapPing();

            for (int Len = 2; Len <= N; Len <<= 1)
            {
                Device.SetRenderTargets(Pong[0], Pong[1], Pong[2]);
                BindPing();
                FftEffect.Parameters["Len"].SetValue((float)Len);
                FftEffect.Parameters["Inv"].SetValue(Len == N ? 1f / N : 1f);
                DrawTechnique(Device, "ButterH");
                SwapPing();
            }

            Device.SetRenderTargets(Pong[0], Pong[1], Pong[2]);
            BindPing();
            DrawTechnique(Device, "BitV");
            SwapPing();

            for (int Len = 2; Len <= N; Len <<= 1)
            {
                Device.SetRenderTargets(Pong[0], Pong[1], Pong[2]);
                BindPing();
                FftEffect.Parameters["Len"].SetValue((float)Len);
                FftEffect.Parameters["Inv"].SetValue(Len == N ? 1f / N : 1f);
                DrawTechnique(Device, "ButterV");
                SwapPing();
            }

            Device.SetRenderTarget(Sum8);
            FftEffect.Parameters["Src0"].SetValue(Ping[0]);
            DrawTechnique(Device, "Reduce8");

            Device.SetRenderTarget(Sum1);
            FftEffect.Parameters["Src0"].SetValue(Sum8);
            DrawTechnique(Device, "Reduce1");

            Device.SetRenderTarget(Dest);
            BindPing();
            FftEffect.Parameters["ScaleMap"].SetValue(Sum1);
            DrawTechnique(Device, "Finish");
        }

        static void BindPing()
        {
            FftEffect.Parameters["Src0"].SetValue(Ping[0]);
            FftEffect.Parameters["Src1"].SetValue(Ping[1]);
            FftEffect.Parameters["Src2"].SetValue(Ping[2]);
        }

        static void SwapPing()
        {
            RenderTarget2D[] Held = Ping;
            Ping = Pong;
            Pong = Held;
        }

        static void DrawTechnique(GraphicsDevice Device, string Name)
        {
            FftEffect.CurrentTechnique = FftEffect.Techniques[Name];
            FftEffect.CurrentTechnique.Passes[0].Apply();
            Device.DrawUserPrimitives(PrimitiveType.TriangleStrip, Quad, 0, 2);
        }

        static void ReadHeights()
        {
            for (int c = 0; c < 3; c++)
            {
                Shown[c].GetData(Readback);
                float[] Row = Height[c];
                for (int i = 0; i < Row.Length; i++)
                {
                    Row[i] = Readback[i].Y;
                }
            }
        }

        static float Read(float WorldX, float WorldZ)
        {
            return Sample(Height[0], WorldX, WorldZ, Tile[0])
                + Sample(Height[1], WorldX, WorldZ, Tile[1])
                + Sample(Height[2], WorldX, WorldZ, Tile[2]);
        }

        static void Seed()
        {
            Random Roll = new Random(17391);
            for (int i = 0; i < GaussR.Length; i++)
            {
                double U = 1.0 - Roll.NextDouble();
                double V = Roll.NextDouble();
                double Mag = Math.Sqrt(-2.0 * Math.Log(U));
                double Ang = Math.PI * 2.0 * V;
                GaussR[i] = (float)(Mag * Math.Cos(Ang));
                GaussI[i] = (float)(Mag * Math.Sin(Ang));
            }

            Seeded = true;
        }

        static void Shape(int Cascade, Vector2 Dir, float Length)
        {
            int Base = Cascade * N * N;
            float TileSize = Tile[Cascade];
            float Cut = 0.35f;
            float Cut2 = Cut * Cut;
            float L2 = Length * Length;
            for (int Z = 0; Z < N; Z++)
            {
                int Kz = Z <= N / 2 ? Z : Z - N;
                float Kzz = Kz * TwoPi / TileSize;
                for (int X = 0; X < N; X++)
                {
                    int At = Base + Z * N + X;
                    int Kx = X <= N / 2 ? X : X - N;
                    float Kxx = Kx * TwoPi / TileSize;
                    float K2 = Kxx * Kxx + Kzz * Kzz;
                    if (K2 < 1e-8f)
                    {
                        H0R[At] = 0f;
                        H0I[At] = 0f;
                        continue;
                    }

                    float K = (float)Math.Sqrt(K2);
                    float Wave = TwoPi / K;
                    float Short = TileSize * 4f / N;
                    float Long = TileSize * 0.5f;
                    if (Wave < Short || Wave > Long)
                    {
                        H0R[At] = 0f;
                        H0I[At] = 0f;
                        continue;
                    }

                    float Align = (Kxx * Dir.X + Kzz * Dir.Y) / K;
                    float Spread = 0.25f + 0.75f * Align * Align;
                    if (Align < 0f)
                    {
                        Spread *= 0.07f;
                    }

                    float Phillips = (float)(Math.Exp(-1.0 / (K2 * L2)) / (K2 * K2) * Spread * Math.Exp(-K2 * Cut2));
                    float Amp = (float)Math.Sqrt(Phillips * 0.5);
                    H0R[At] = GaussR[At] * Amp;
                    H0I[At] = GaussI[At] * Amp;
                }
            }
        }

        static float Sample(float[] Field, float WorldX, float WorldZ, float TileSize)
        {
            float U = WorldX / TileSize * N - 0.5f;
            float V = WorldZ / TileSize * N - 0.5f;
            U -= (float)Math.Floor(U / N) * N;
            V -= (float)Math.Floor(V / N) * N;
            int X0 = (int)U;
            int Z0 = (int)V;
            if (X0 >= N)
            {
                X0 = 0;
            }

            if (Z0 >= N)
            {
                Z0 = 0;
            }

            int X1 = X0 + 1;
            if (X1 >= N)
            {
                X1 = 0;
            }

            int Z1 = Z0 + 1;
            if (Z1 >= N)
            {
                Z1 = 0;
            }

            float Fx = U - X0;
            float Fz = V - Z0;
            float A = Field[Z0 * N + X0] + (Field[Z0 * N + X1] - Field[Z0 * N + X0]) * Fx;
            float B = Field[Z1 * N + X0] + (Field[Z1 * N + X1] - Field[Z1 * N + X0]) * Fx;
            return A + (B - A) * Fz;
        }
    }
}
