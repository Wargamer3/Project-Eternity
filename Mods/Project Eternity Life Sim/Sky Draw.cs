using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class SkyDraw
    {
        public static Vector3 FogColor = new Vector3(0.76f, 0.81f, 0.84f);
        public static Vector3 SunDirection = new Vector3(0.35f, 0.86f, 0.25f);

        static Effect Effect;
        static VertexPosition[] Vertices;
        static short[] Indices;
        static DepthStencilState Depth;
        static bool Failed;
        static float Time;

        public static void Advance(double Seconds)
        {
            if (Seconds > 0d)
            {
                Time += (float)Seconds;
            }
        }

        public static void Light(WorldClock Clock, NavMapGameManager World, Vector3 Eye)
        {
            Paint(Clock, World, Eye);
        }

        public static void Draw(GraphicsDevice Device, Camera3D Camera, WorldClock Clock, NavMapGameManager World)
        {
            if (Device == null || Camera == null)
            {
                return;
            }

            Ensure(Device);
            if (Failed || Effect == null || Vertices == null)
            {
                return;
            }

            Viewport Pane = Device.Viewport;
            float Aspect = Pane.Height > 0 ? Pane.Width / (float)Pane.Height : 1.6f;
            Matrix View = Camera.View;
            Vector3 Eye = Matrix.Invert(View).Translation;
            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Aspect, 1f, 10000f);
            Paint(Clock, World, Eye);

            Effect.Parameters["WorldViewProj"].SetValue(Matrix.CreateTranslation(Eye) * View * Projection);
            Effect.Parameters["Zenith"].SetValue(ZenithColor);
            Effect.Parameters["Horizon"].SetValue(FogColor);
            Effect.Parameters["SunDir"].SetValue(SunDirection);
            Effect.Parameters["SunColor"].SetValue(SunTint);
            Effect.Parameters["Cloud"].SetValue(CloudCover);
            Effect.Parameters["Time"].SetValue(Time);
            Device.BlendState = BlendState.Opaque;
            Device.DepthStencilState = Depth;
            Device.RasterizerState = RasterizerState.CullNone;
            foreach (EffectPass Pass in Effect.CurrentTechnique.Passes)
            {
                Pass.Apply();
                Device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, Vertices, 0, Vertices.Length, Indices, 0, Indices.Length / 3);
            }
        }

        public static Vector3 ZenithColor = new Vector3(0.34f, 0.50f, 0.72f);
        public static Vector3 SunTint = new Vector3(1f, 0.92f, 0.78f);
        public static Vector3 KeyDirection = new Vector3(0.35f, 0.86f, 0.25f);
        public static Vector3 KeyColor = new Vector3(0.62f, 0.58f, 0.50f);
        public static Vector3 SkyAmbient = new Vector3(0.32f, 0.34f, 0.38f);
        public static Vector3 FillDirection = new Vector3(-0.2f, 0.92f, -0.15f);
        public static Vector3 FillColor = new Vector3(0.14f, 0.15f, 0.17f);
        static float CloudCover = 0.22f;

        public static float Cover
        {
            get { return CloudCover; }
        }

        public static float CloudClock
        {
            get { return Time; }
        }

        static void Paint(WorldClock Clock, NavMapGameManager World, Vector3 Eye)
        {
            double Hour = Clock == null ? 12d : Clock.Hour;
            Outlook Outlook = Forecast.Look(World, Eye.X, Eye.Z);
            float Cover = Mix(CoverOf(Outlook.Period), CoverOf(Outlook.Dying), Outlook.Strength);
            float Warm = Mix(Warmth(Outlook.Period), Warmth(Outlook.Dying), Outlook.Strength);
            SunDirection = SunAt(Hour);
            float Day = MathHelper.Clamp(SunDirection.Y * 2.4f + 0.05f, 0f, 1f);
            float Golden = MathHelper.Clamp(1f - Math.Abs(SunDirection.Y) * 4.5f, 0f, 1f);
            Golden *= MathHelper.Clamp(SunDirection.Y + 0.25f, 0f, 1f) * (1f - Cover);

            Vector3 ZenithDay = new Vector3(0.34f, 0.50f, 0.72f);
            Vector3 HorizonDay = new Vector3(0.76f, 0.81f, 0.84f);
            if (Warm < 0f)
            {
                float Arctic = -Warm;
                ZenithDay = Vector3.Lerp(ZenithDay, new Vector3(0.48f, 0.60f, 0.70f), Arctic);
                HorizonDay = Vector3.Lerp(HorizonDay, new Vector3(0.86f, 0.88f, 0.90f), Arctic);
            }
            else if (Warm > 0f)
            {
                ZenithDay = Vector3.Lerp(ZenithDay, new Vector3(0.40f, 0.60f, 0.90f), Warm);
                HorizonDay = Vector3.Lerp(HorizonDay, new Vector3(0.88f, 0.76f, 0.62f), Warm);
            }

            ZenithColor = Vector3.Lerp(new Vector3(0.012f, 0.018f, 0.045f), ZenithDay, Day);
            FogColor = Vector3.Lerp(new Vector3(0.05f, 0.06f, 0.09f), HorizonDay, Day);
            FogColor = Vector3.Lerp(FogColor, new Vector3(0.90f, 0.50f, 0.32f), Golden * 0.75f);
            Vector3 Grey = new Vector3(0.62f, 0.66f, 0.70f);
            ZenithColor = Vector3.Lerp(ZenithColor, Grey * 0.72f, Cover * Day);
            FogColor = Vector3.Lerp(FogColor, Grey, Cover * 0.85f);
            float Storm = Forecast.Storm(Outlook.Period) ? 1f : (Forecast.Storm(Outlook.Dying) ? MathHelper.Clamp(Outlook.Strength, 0f, 1f) : 0f);
            ZenithColor = Vector3.Lerp(ZenithColor, Grey * 0.42f, Storm * Day * 0.7f);
            FogColor = Vector3.Lerp(FogColor, Grey * 0.5f, Storm * 0.45f);
            SunTint = Vector3.Lerp(new Vector3(1f, 0.93f, 0.80f), new Vector3(1f, 0.55f, 0.28f), Golden);
            SunTint *= 1f - Cover * 0.85f;
            SunTint *= 1f - Storm * 0.35f;
            CloudCover = Cover;
            RememberLight(Cover, Storm);
            float Bolt = WeatherDraw.Light;
            if (Bolt > 1.001f)
            {
                ZenithColor *= Bolt;
                FogColor = Vector3.Lerp(FogColor, new Vector3(0.85f, 0.88f, 0.95f), (Bolt - 1f) * 0.8f);
            }
        }

        // Same sun the ground shader uses. KeyDirection points toward the sun. BasicEffect wants the opposite.
        static void RememberLight(float Cover, float Storm)
        {
            float Lift = MathHelper.Clamp(SunDirection.Y * 1.6f + 0.05f, 0f, 1f);
            Vector3 Overhead = new Vector3(0.2f, 1f, 0.15f);
            Overhead.Normalize();
            Vector3 Key = Vector3.Lerp(Overhead, SunDirection, Lift);
            if (Key.LengthSquared() > 0.0001f)
            {
                Key.Normalize();
            }
            else
            {
                Key = Vector3.Up;
            }

            KeyDirection = Key;
            Vector3 HueSource = Vector3.Lerp(ZenithColor, FogColor, 0.35f);
            if (HueSource.LengthSquared() < 0.0001f)
            {
                HueSource = Vector3.One;
            }

            Vector3 Hue = Vector3.Lerp(Vector3.One, Vector3.Normalize(HueSource), 0.55f);
            float Dim = (1f - Cover * 0.28f) * (1f - Storm * 0.4f);
            float Bolt = WeatherDraw.Light;
            SkyAmbient = Hue * (MathHelper.Lerp(0.22f, 0.38f, Lift) * Dim) * Bolt;
            KeyColor = SunTint * MathHelper.Lerp(0.25f, 0.62f, Lift) * Bolt;
            Vector3 Flat = new Vector3(SunDirection.X, 0f, SunDirection.Z);
            if (Flat.LengthSquared() < 0.0001f)
            {
                Flat = Vector3.Forward;
            }
            else
            {
                Flat.Normalize();
            }

            Vector3 Fill = Vector3.Up * 0.85f - Flat * 0.45f;
            if (Fill.LengthSquared() > 0.0001f)
            {
                Fill.Normalize();
            }
            else
            {
                Fill = Vector3.Up;
            }

            FillDirection = Fill;
            FillColor = Hue * (MathHelper.Lerp(0.08f, 0.16f, Lift) * (1f - Cover * 0.2f) * (1f - Storm * 0.35f)) * Bolt;
            FbxModel.UseSun(KeyDirection, KeyColor, SkyAmbient, FillDirection, FillColor);
        }

        // The character lamp reads the normal before the world rotation, and its W keeps the texture from blowing out.
        public static ModelLight CharacterLight(Matrix World)
        {
            Vector3 Toward = KeyDirection;
            if (World.Determinant() != 0f)
            {
                Toward = Vector3.TransformNormal(KeyDirection, Matrix.Invert(World));
            }

            if (Toward.LengthSquared() > 0.0001f)
            {
                Toward.Normalize();
            }
            else
            {
                Toward = Vector3.Up;
            }

            return new ModelLight
            {
                Ambient = SkyAmbient,
                Direction = new Vector4(Toward, 1f),
                Color = KeyColor * 1.41421356f,
            };
        }

        static float Mix(float Now, float Was, float Strength)
        {
            if (Strength <= 0f)
            {
                return Now;
            }

            return MathHelper.Lerp(Now, Was, Strength);
        }

        static float CoverOf(TimePeriod Period)
        {
            if (Period == null)
            {
                return 0.22f;
            }

            switch (Period.WeatherType)
            {
                case TimePeriod.WeatherTypes.SomeClouds:
                    return 0.48f;
                case TimePeriod.WeatherTypes.Windy:
                    return 0.38f;
                case TimePeriod.WeatherTypes.Cloudy:
                case TimePeriod.WeatherTypes.LightSnow:
                    return 0.72f;
                case TimePeriod.WeatherTypes.SmallRain:
                case TimePeriod.WeatherTypes.Sandstorm:
                    return 0.84f;
                case TimePeriod.WeatherTypes.HeavyRain:
                case TimePeriod.WeatherTypes.HeavySnow:
                case TimePeriod.WeatherTypes.Thunderstorm:
                    return 0.94f;
                default:
                    return 0.22f;
            }
        }

        static float Warmth(TimePeriod Period)
        {
            if (Period == null)
            {
                return 0f;
            }

            if (Period.SkyType == TimePeriod.SkyTypes.Warm)
            {
                return 1f;
            }

            if (Period.SkyType == TimePeriod.SkyTypes.Arctic)
            {
                return -1f;
            }

            return 0f;
        }

        public static Vector3 SunAt(double Hour)
        {
            double Wrapped = Hour % 24d;
            if (Wrapped < 0d)
            {
                Wrapped += 24d;
            }

            float Angle = (float)((Wrapped - 6d) / 12d * Math.PI);
            float Elev = (float)Math.Sin(Angle) * 0.92f;
            float Horizontal = (float)Math.Sqrt(Math.Max(0f, 1f - Elev * Elev));
            Vector3 Sun = new Vector3((float)Math.Cos(Angle) * Horizontal, Elev, (float)Math.Sin(Angle) * Horizontal);
            if (Sun.LengthSquared() < 0.0001f)
            {
                return Vector3.Up;
            }

            Sun.Normalize();
            return Sun;
        }

        static void Ensure(GraphicsDevice Device)
        {
            if (Effect != null || Failed)
            {
                return;
            }

            try
            {
                string PathName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Life Sim", "Shaders", "SkySphere.mgfx");
                if (!File.Exists(PathName))
                {
                    PathName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mods", "SkySphere.mgfx");
                }

                if (!File.Exists(PathName))
                {
                    Failed = true;
                    return;
                }

                Effect = new Effect(Device, File.ReadAllBytes(PathName));
                BuildSphere();
                Depth = new DepthStencilState();
                Depth.DepthBufferEnable = true;
                Depth.DepthBufferWriteEnable = true;
                Depth.DepthBufferFunction = CompareFunction.LessEqual;
            }
            catch (Exception)
            {
                Failed = true;
            }
        }

        static void BuildSphere()
        {
            const int Stacks = 16;
            const int Slices = 32;
            const float Radius = 8000f;
            Vertices = new VertexPosition[(Stacks + 1) * (Slices + 1)];
            int v = 0;
            for (int Y = 0; Y <= Stacks; Y++)
            {
                float Phi = Y / (float)Stacks * MathHelper.Pi;
                for (int X = 0; X <= Slices; X++)
                {
                    float Theta = X / (float)Slices * MathHelper.TwoPi;
                    float SinPhi = (float)Math.Sin(Phi);
                    Vertices[v++] = new VertexPosition(new Vector3(
                        SinPhi * (float)Math.Cos(Theta),
                        (float)Math.Cos(Phi),
                        SinPhi * (float)Math.Sin(Theta)) * Radius);
                }
            }

            Indices = new short[Stacks * Slices * 6];
            int i = 0;
            for (int Y = 0; Y < Stacks; Y++)
            {
                for (int X = 0; X < Slices; X++)
                {
                    int Row = Y * (Slices + 1);
                    int Next = Row + Slices + 1;
                    Indices[i++] = (short)(Row + X);
                    Indices[i++] = (short)(Next + X);
                    Indices[i++] = (short)(Row + X + 1);
                    Indices[i++] = (short)(Row + X + 1);
                    Indices[i++] = (short)(Next + X);
                    Indices[i++] = (short)(Next + X + 1);
                }
            }
        }
    }
}
