using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class CharacterShadow
    {
        public const int Count = 3;
        // Camera, body, and this map are in world units. The orbit stops near 100, so the slices stay in that scale.
        public static bool Cast;
        public static Matrix CameraView = Matrix.Identity;
        public static Vector4 Split = new Vector4(160f, 640f, 2560f, 24f);
        public static readonly Matrix[] CascadeViewProj = new Matrix[Count];
        public static readonly Texture2D[] Cascade = new Texture2D[Count];

        // The near cascade, for anything that still reads a single map.
        public static Matrix ViewProj = Matrix.Identity;
        public static Texture2D Map;

        // Near map at 480p. Taller screens scale it so a shadow texel stays about one screen pixel. Farther slices stay smaller.
        static readonly int[] Sizes = { 1024, 512, 256 };
        static Vector4 Texel = new Vector4(1f / 1024f, 1f / 512f, 1f / 256f, 0f);

        static Effect Effect;
        static readonly RenderTarget2D[] Target = new RenderTarget2D[Count];
        static bool Failed;

        static Vector3 LitSun = Vector3.Up;

        public static void Render(GraphicsDevice Device, PlaySession Session, NavMapGameManager World, Camera3D Camera)
        {
            Cast = false;
            GltfModel.CharacterShade = ApplyBody;
            if (Device == null)
            {
                return;
            }

            Ensure(Device);
            if (Failed || Effect == null)
            {
                return;
            }

            PlayerCharacter Who = Session == null ? null : Session.Controlled;
            bool Body = Who != null && Who.BodyModel != null && !Who.Life.Away.Busy;
            bool HasCamera = Camera != null && Math.Abs(Camera.Projection.M22) > 0.01f;
            if (!Body && !HasCamera)
            {
                return;
            }

            if (PictureOptions.ShadowCascades <= 0)
            {
                return;
            }

            Vector3 Sun = SkyDraw.KeyDirection;
            if (Sun.Y < 0.25f)
            {
                Sun.Y = 0.25f;
            }

            if (Sun.LengthSquared() < 0.0001f)
            {
                Sun = Vector3.Up;
            }

            Sun.Normalize();
            LitSun = Sun;
            Vector3 Up = Math.Abs(Vector3.Dot(Sun, Vector3.Up)) > 0.95f ? Vector3.Forward : Vector3.Up;
            Vector3 Focus = Vector3.Zero;
            Matrix BodyWorld = Matrix.Identity;
            if (Body)
            {
                MapInfo Here = Who.SharedMapContex == null ? null : Who.SharedMapContex.CurrentMapInfo;
                BodyWorld = Who.BodyDrawWorld(ChunkOrigin.Shift(Here));
                Focus = BodyWorld.Translation;
            }
            else
            {
                Focus = Camera.CameraPosition3D;
            }

            Matrix View = HasCamera ? Camera.View : Matrix.CreateLookAt(Focus + new Vector3(0f, 8f, -12f), Focus, Vector3.Up);
            Viewport Pane = Device.Viewport;
            float Aspect = Pane.Height > 0 ? Pane.Width / (float)Pane.Height : 1.6f;
            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Aspect, 1f, 10000f);
            float Depth = -Vector3.Transform(Focus, View).Z;
            if (HasCamera)
            {
                float Look = -Vector3.Transform(Camera.LookAtTarget, View).Z;
                if (Look > 8f && (Depth < 8f || Look < Depth))
                {
                    Depth = Look;
                }
            }

            if (Depth < 8f)
            {
                Depth = 8f;
            }

            FitMaps(Device, Pane.Height);
            if (Target[0] == null)
            {
                return;
            }

            Place(Projection, Pane, Depth);
            float Ground = Body ? Focus.Y : (HasCamera ? Camera.LookAtTarget.Y : Focus.Y);
            CameraView = View;
            RenderTargetBinding[] Previous = Device.GetRenderTargets();
            int Limit = PictureOptions.ShadowCascades;
            for (int c = 0; c < Count; c++)
            {
                if (c >= Limit)
                {
                    Device.SetRenderTarget(Target[c]);
                    Device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1f, 0);
                    CascadeViewProj[c] = CascadeViewProj[0];
                    continue;
                }

                Matrix LightView;
                Matrix LightProjection;
                float Radius;
                Vector3 Center;
                Fit(c, View, Projection, Sun, Up, Focus, Ground, out LightView, out LightProjection, out Radius, out Center);
                CascadeViewProj[c] = LightView * LightProjection;
                Device.SetRenderTarget(Target[c]);
                Device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1f, 0);
                if (Body)
                {
                    Who.BodyModel.DrawDepth(Effect, LightView, LightProjection, BodyWorld);
                }

                DrawPeople(World, Who, Center, Radius, LightView, LightProjection);
                ForestScatter.DrawShadows(Effect, World == null ? null : World.UpdatedMaps, LightView, LightProjection, Center, Radius, true);
            }

            Device.SetRenderTargets(Previous);
            Device.Viewport = Pane;
            Device.Clear(ClearOptions.DepthBuffer, Color.Black, 1f, 0);
            for (int c = 0; c < Count; c++)
            {
                Cascade[c] = Target[c];
            }

            ViewProj = CascadeViewProj[0];
            Map = Target[0];
            Cast = true;
        }

        public static void Apply(Effect Shade)
        {
            if (Shade == null)
            {
                return;
            }

            Set(Shade, "CascadeCast", Cast ? 1f : 0f);
            Set(Shade, "CameraView", CameraView);
            Set(Shade, "CascadeSplit", Split);
            Set(Shade, "CascadeTexel", Texel);
            for (int c = 0; c < Count; c++)
            {
                string Index = c.ToString();
                Set(Shade, "CascadeView" + Index, CascadeViewProj[c]);
                EffectParameter Texture = Shade.Parameters["Cascade" + Index];
                if (Texture != null && Cascade[c] != null)
                {
                    Texture.SetValue(Cascade[c]);
                }
            }

            Set(Shade, "ShadowCast", Cast ? 1f : 0f);
            Set(Shade, "ShadowViewProj", ViewProj);
            EffectParameter Near = Shade.Parameters["ShadowMap"];
            if (Near != null && Map != null)
            {
                Near.SetValue(Map);
            }

        }

        public static void ApplyBody(Effect Shade)
        {
            Apply(Shade);
            Set(Shade, "CascadeSun", LitSun);
        }

        static void Set(Effect Shade, string Name, float Value)
        {
            EffectParameter Parameter = Shade.Parameters[Name];
            if (Parameter != null)
            {
                Parameter.SetValue(Value);
            }
        }

        static void Set(Effect Shade, string Name, Matrix Value)
        {
            EffectParameter Parameter = Shade.Parameters[Name];
            if (Parameter != null)
            {
                Parameter.SetValue(Value);
            }
        }

        static void Set(Effect Shade, string Name, Vector4 Value)
        {
            EffectParameter Parameter = Shade.Parameters[Name];
            if (Parameter != null)
            {
                Parameter.SetValue(Value);
            }
        }

        static void Set(Effect Shade, string Name, Vector3 Value)
        {
            EffectParameter Parameter = Shade.Parameters[Name];
            if (Parameter != null)
            {
                Parameter.SetValue(Value);
            }
        }

        // The near slice ends just past the subject, in world units, so one shadow texel is about two screen pixels there.
        static void Place(Matrix Projection, Viewport Pane, float Depth)
        {
            float TanX = 1f / Math.Max(Math.Abs(Projection.M11), 0.01f);
            float TanY = 1f / Math.Max(Math.Abs(Projection.M22), 0.01f);
            float Screen = Math.Max(Pane.Height, 1);
            float Pixel = 2f * Depth * TanY / Screen;
            float Budget = Pixel * 1.25f * Sizes[0];
            float Reach = Budget / (2f * Math.Max(TanX, 0.01f));
            float Past = Math.Max(Depth * 1.35f, Depth + 24f);
            if (Reach < Past)
            {
                Reach = Past;
            }

            float Step = 16f;
            Reach = (float)Math.Ceiling(Reach / Step) * Step;
            float Mid = (float)Math.Ceiling(Reach * 4f / Step) * Step;
            float Far = (float)Math.Ceiling(Reach * 16f / Step) * Step;
            if (Far > 4096f)
            {
                Far = 4096f;
            }

            if (Mid >= Far)
            {
                Mid = Reach + (Far - Reach) * 0.45f;
            }

            float Blend = Math.Max(16f, Math.Min(32f, (Mid - Reach) * 0.15f));
            Split = new Vector4(Reach, Mid, Far, Blend);
        }

        static void Fit(int Index, Matrix View, Matrix Projection, Vector3 Sun, Vector3 Up, Vector3 Focus, float Ground, out Matrix LightView, out Matrix LightProjection, out float Radius, out Vector3 Center)
        {
            float Near = Index == 0 ? 1f : Index == 1 ? Split.X : Split.Y;
            float Far = Index == 0 ? Split.X : Index == 1 ? Split.Y : Split.Z;
            float TanX = 1f / Math.Max(Math.Abs(Projection.M11), 0.01f);
            float TanY = 1f / Math.Max(Math.Abs(Projection.M22), 0.01f);
            Matrix Inverse = Matrix.Invert(View);
            Vector3[] Flat = new Vector3[9];
            int Written = 0;
            float[] Distance = { Near, Far };
            for (int d = 0; d < Distance.Length; d++)
            {
                float Z = Distance[d];
                for (int X = -1; X <= 1; X += 2)
                {
                    for (int Y = -1; Y <= 1; Y += 2)
                    {
                        Vector3 Eye = new Vector3(X * Z * TanX, Y * Z * TanY, -Z);
                        Flat[Written] = Vector3.Transform(Eye, Inverse);
                        Flat[Written].Y = Ground;
                        Written++;
                    }
                }
            }

            Flat[8] = Focus;
            Flat[8].Y = Ground;
            Center = Vector3.Zero;
            for (int i = 0; i < Flat.Length; i++)
            {
                Center += Flat[i];
            }

            Center /= Flat.Length;
            LightView = Matrix.CreateLookAt(Center + Sun * 2048f, Center, Up);
            Vector3 Low = new Vector3(float.MaxValue);
            Vector3 High = new Vector3(float.MinValue);
            for (int i = 0; i < Flat.Length; i++)
            {
                Vector3 Light = Vector3.Transform(Flat[i], LightView);
                Low = Vector3.Min(Low, Light);
                High = Vector3.Max(High, Light);
            }

            float Pad = 16f;
            Low.X -= Pad;
            High.X += Pad;
            Low.Y -= Pad;
            High.Y += Pad;
            float Width = Math.Max(High.X - Low.X, 32f);
            float Height = Math.Max(High.Y - Low.Y, 32f);
            float FootMinX = float.MaxValue;
            float FootMaxX = float.MinValue;
            float FootMinZ = float.MaxValue;
            float FootMaxZ = float.MinValue;
            for (int i = 0; i < Flat.Length; i++)
            {
                FootMinX = Math.Min(FootMinX, Flat[i].X);
                FootMaxX = Math.Max(FootMaxX, Flat[i].X);
                FootMinZ = Math.Min(FootMinZ, Flat[i].Z);
                FootMaxZ = Math.Max(FootMaxZ, Flat[i].Z);
            }

            float Cap = Math.Max(FootMaxX - FootMinX, FootMaxZ - FootMinZ) + 64f;
            if (Cap < 96f)
            {
                Cap = 96f;
            }

            if (Width > Cap)
            {
                float Mid = (Low.X + High.X) * 0.5f;
                Low.X = Mid - Cap * 0.5f;
                High.X = Mid + Cap * 0.5f;
                Width = Cap;
            }

            if (Height > Cap)
            {
                float Mid = (Low.Y + High.Y) * 0.5f;
                Low.Y = Mid - Cap * 0.5f;
                High.Y = Mid + Cap * 0.5f;
                Height = Cap;
            }
            if (Width != Width || Width > 8000f)
            {
                Width = Math.Max(2f * Far * TanX, 64f);
                Height = Width;
                Low.X = -Width * 0.5f;
                High.X = Width * 0.5f;
                Low.Y = -Height * 0.5f;
                High.Y = Height * 0.5f;
                Center = Focus;
                Center.Y = Ground;
                LightView = Matrix.CreateLookAt(Center + Sun * 2048f, Center, Up);
            }

            float TexelX = Width / Sizes[Index];
            float TexelY = Height / Sizes[Index];
            float MidX = (float)Math.Floor(((Low.X + High.X) * 0.5f) / TexelX) * TexelX;
            float MidY = (float)Math.Floor(((Low.Y + High.Y) * 0.5f) / TexelY) * TexelY;
            Low.X = MidX - Width * 0.5f;
            High.X = MidX + Width * 0.5f;
            Low.Y = MidY - Height * 0.5f;
            High.Y = MidY + Height * 0.5f;
            float NearDist = -High.Z - 2048f;
            float FarDist = -Low.Z + 512f;
            if (NearDist < 1f)
            {
                NearDist = 1f;
            }

            if (FarDist < NearDist + 64f)
            {
                FarDist = NearDist + 64f;
            }

            LightProjection = Matrix.CreateOrthographicOffCenter(Low.X, High.X, Low.Y, High.Y, NearDist, FarDist);
            Radius = (float)Math.Sqrt(Width * Width + Height * Height) * 0.5f + 80f;
        }

        static void DrawPeople(NavMapGameManager World, PlayerCharacter Who, Vector3 Center, float Radius, Matrix View, Matrix Projection)
        {
            if (World == null || World.UpdatedMaps == null)
            {
                return;
            }

            float Limit = Radius * Radius;
            for (int m = 0; m < World.UpdatedMaps.Count; m++)
            {
                MapInfo Info = World.UpdatedMaps[m];
                if (Info == null || Info.People == null)
                {
                    continue;
                }

                Vector3 Shift = ChunkOrigin.Shift(Info);
                for (int i = 0; i < Info.People.Count; i++)
                {
                    PlayerCharacter Other = Info.People[i];
                    if (Other == null || Other == Who || Other.BodyModel == null || Other.DungeonPose)
                    {
                        continue;
                    }

                    if (Other.Life != null && Other.Life.Away != null && Other.Life.Away.Busy)
                    {
                        continue;
                    }

                    Matrix Body = Other.BodyDrawWorld(Shift);
                    Vector3 Delta = Body.Translation - Center;
                    Delta.Y = 0f;
                    if (Delta.LengthSquared() > Limit)
                    {
                        continue;
                    }

                    Other.BodyModel.DrawDepth(Effect, View, Projection, Body);
                }
            }
        }

        // Camera-view depth for contact darkening and the sun shafts. Grass is included here and left out of the cascades.
        public static void FillView(GraphicsDevice Device, NavMapGameManager World, IList<MapInfo> Shown, Matrix View, Matrix Projection)
        {
            Ensure(Device);
            if (Failed || Effect == null || World == null)
            {
                return;
            }

            Vector3 Eye = Matrix.Invert(View).Translation;
            DrawPeople(World, null, Eye, 20000f, View, Projection);
            ForestScatter.DrawShadows(Effect, Shown, View, Projection, Eye, 420f, false, false);
        }

        static int Power(float Want, int Min, int Max)
        {
            int Size = Min;
            while (Size < Max && Math.Abs(Want - Size * 2f) < Math.Abs(Want - Size))
            {
                Size *= 2;
            }

            return Size;
        }

        static void FitMaps(GraphicsDevice Device, int ScreenHeight)
        {
            int Near = Power(1024f * Math.Max(ScreenHeight, 1) / 480f, 512, 2048);
            if (Target[0] != null && Target[0].Width == Near)
            {
                return;
            }

            Sizes[0] = Near;
            Sizes[1] = Near / 2;
            Sizes[2] = Near / 4;
            Texel = new Vector4(1f / Sizes[0], 1f / Sizes[1], 1f / Sizes[2], 0f);
            for (int c = 0; c < Count; c++)
            {
                if (Target[c] != null)
                {
                    Target[c].Dispose();
                }

                Target[c] = new RenderTarget2D(Device, Sizes[c], Sizes[c], false, SurfaceFormat.Single, DepthFormat.Depth24);
                Cascade[c] = Target[c];
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
                string PathName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Life Sim", "Shaders", "CharacterShadow.mgfx");
                if (!File.Exists(PathName))
                {
                    Failed = true;
                    return;
                }

                Effect = new Effect(Device, File.ReadAllBytes(PathName));
            }
            catch (Exception)
            {
                Failed = true;
            }
        }
    }
}
