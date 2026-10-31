using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class SceneShade
    {
        static Effect Effect;
        static RenderTarget2D Scene;
        static RenderTarget2D Depth;
        static RenderTarget2D Occlusion;
        static RenderTarget2D Soft;
        static VertexPositionTexture[] Quad;
        static bool Failed;
        static Viewport Full;

        public static void Begin(GraphicsDevice Device)
        {
            if (Device == null)
            {
                return;
            }

            if (!PictureOptions.Shade)
            {
                Device.SetRenderTarget(null);
                Device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, new Color(SkyDraw.FogColor), 1f, 0);
                return;
            }

            Ensure(Device);
            if (Failed || Scene == null)
            {
                return;
            }

            Full = Device.Viewport;
            Device.SetRenderTarget(Scene);
            Device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, new Color(SkyDraw.FogColor), 1f, 0);
        }

        public static void Present(GraphicsDevice Device, Camera3D Camera, NavMapGameManager World, System.Collections.Generic.IList<MapInfo> Terrain)
        {
            if (!PictureOptions.Shade || Device == null || Failed || Scene == null || Effect == null)
            {
                Device?.SetRenderTarget(null);
                return;
            }

            Viewport Pane = new Viewport(0, 0, Math.Max(1, Scene.Width / 2), Math.Max(1, Scene.Height / 2));
            Matrix View = Camera != null ? Camera.View : Matrix.Identity;
            float Aspect = Scene.Height > 0 ? Scene.Width / (float)Scene.Height : 1.6f;
            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Aspect, 1f, 10000f);
            Device.SetRenderTarget(Depth);
            Device.Viewport = Pane;
            Device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);
            if (Camera != null && Terrain != null)
            {
                if (!HeightDraw.DrawDepthLoaded(Device, View, Projection))
                {
                    for (int i = 0; i < Terrain.Count; i++)
                    {
                        HeightDraw.DrawDepth(Device, Terrain[i], View, Projection);
                    }
                }

                CharacterShadow.FillView(Device, World, Terrain, View, Projection);
            }

            float Tan = (float)Math.Tan(MathHelper.PiOver4 * 0.5f);
            Vector2 UvPer = new Vector2(0.5f / (Aspect * Tan), 0.5f / Tan);
            Effect.Parameters["Near"].SetValue(1f);
            Effect.Parameters["Far"].SetValue(10000f);
            Effect.Parameters["Radius"].SetValue(28f);
            Effect.Parameters["UvPerUnit"].SetValue(UvPer);
            Effect.Parameters["Pixel"].SetValue(new Vector2(1f / Pane.Width, 1f / Pane.Height));
            Effect.Parameters["DepthMap"].SetValue(Depth);
            Effect.CurrentTechnique = Effect.Techniques["Occlude"];
            Device.SetRenderTarget(Occlusion);
            Device.Clear(Color.White);
            DrawQuad(Device);

            Effect.Parameters["AoMap"].SetValue(Occlusion);
            Effect.Parameters["Direction"].SetValue(Vector2.UnitX);
            Effect.CurrentTechnique = Effect.Techniques["Blur"];
            Device.SetRenderTarget(Soft);
            Device.Clear(Color.White);
            DrawQuad(Device);

            Effect.Parameters["AoMap"].SetValue(Soft);
            Effect.Parameters["Direction"].SetValue(Vector2.UnitY);
            Device.SetRenderTarget(Occlusion);
            Device.Clear(Color.White);
            DrawQuad(Device);

            Vector2 SunUv = new Vector2(0.5f, 0.3f);
            float Shaft = 0f;
            if (Camera != null)
            {
                Vector3 Eye = Matrix.Invert(View).Translation;
                Vector3 Far = Eye + SkyDraw.SunDirection * 4000f;
                Vector4 Clip = Vector4.Transform(new Vector4(Far, 1f), View * Projection);
                if (Clip.W > 0.01f && SkyDraw.SunDirection.Y > 0.02f)
                {
                    SunUv = new Vector2(Clip.X / Clip.W * 0.5f + 0.5f, Clip.Y / Clip.W * -0.5f + 0.5f);
                    float Low = MathHelper.Clamp(1f - SkyDraw.SunDirection.Y * 1.2f, 0.35f, 1f);
                    Shaft = PictureOptions.Shafts ? Low * (1f - SkyDraw.Cover * 0.85f) * 0.8f : 0f;
                }
            }

            Effect.Parameters["SunUv"].SetValue(SunUv);
            Effect.Parameters["Aspect"].SetValue(Aspect);
            Effect.Parameters["DepthMap"].SetValue(Depth);
            Effect.CurrentTechnique = Effect.Techniques["Shafts"];
            Device.SetRenderTarget(Soft);
            Device.Viewport = Pane;
            Device.Clear(Color.Black);
            if (Shaft > 0.001f)
            {
                DrawQuad(Device);
            }

            Effect.Parameters["SceneMap"].SetValue(Scene);
            Effect.Parameters["AoMap"].SetValue(Occlusion);
            Effect.Parameters["ShaftMap"].SetValue(Soft);
            Effect.Parameters["SunColor"].SetValue(SkyDraw.SunTint);
            Effect.Parameters["Shaft"].SetValue(Shaft);
            Effect.CurrentTechnique = Effect.Techniques["Present"];
            Device.SetRenderTarget(null);
            Device.Viewport = Full;
            Device.BlendState = BlendState.Opaque;
            Device.DepthStencilState = DepthStencilState.None;
            Device.RasterizerState = RasterizerState.CullNone;
            DrawQuad(Device);
            Device.DepthStencilState = DepthStencilState.Default;
        }

        static void DrawQuad(GraphicsDevice Device)
        {
            foreach (EffectPass Pass in Effect.CurrentTechnique.Passes)
            {
                Pass.Apply();
                Device.DrawUserPrimitives(PrimitiveType.TriangleStrip, Quad, 0, 2);
            }
        }

        static void Ensure(GraphicsDevice Device)
        {
            int Width = Math.Max(1, Device.Viewport.Width);
            int Height = Math.Max(1, Device.Viewport.Height);
            int Samples = PictureOptions.Msaa ? 4 : 0;
            if (Effect != null && Scene != null && Scene.Width == Width && Scene.Height == Height && Scene.MultiSampleCount == Samples)
            {
                return;
            }

            if (Effect == null && !Failed)
            {
                try
                {
                    string PathName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Life Sim", "Shaders", "SceneShade.mgfx");
                    if (!File.Exists(PathName))
                    {
                        Failed = true;
                        return;
                    }

                    Effect = new Effect(Device, File.ReadAllBytes(PathName));
                    Quad = new[]
                    {
                        new VertexPositionTexture(new Vector3(-1f, 1f, 0f), new Vector2(0f, 0f)),
                        new VertexPositionTexture(new Vector3(1f, 1f, 0f), new Vector2(1f, 0f)),
                        new VertexPositionTexture(new Vector3(-1f, -1f, 0f), new Vector2(0f, 1f)),
                        new VertexPositionTexture(new Vector3(1f, -1f, 0f), new Vector2(1f, 1f)),
                    };
                }
                catch (Exception)
                {
                    Failed = true;
                    return;
                }
            }

            if (Effect == null)
            {
                return;
            }

            Dispose(Scene);
            Dispose(Depth);
            Dispose(Occlusion);
            Dispose(Soft);
            int HalfW = Math.Max(1, Width / 2);
            int HalfH = Math.Max(1, Height / 2);
            try
            {
                Scene = new RenderTarget2D(Device, Width, Height, false, SurfaceFormat.Color, DepthFormat.Depth24, Samples, RenderTargetUsage.DiscardContents);
            }
            catch (Exception)
            {
                Scene = new RenderTarget2D(Device, Width, Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            }

            Depth = new RenderTarget2D(Device, HalfW, HalfH, false, SurfaceFormat.Single, DepthFormat.Depth24);
            Occlusion = new RenderTarget2D(Device, HalfW, HalfH, false, SurfaceFormat.Color, DepthFormat.None);
            Soft = new RenderTarget2D(Device, HalfW, HalfH, false, SurfaceFormat.Color, DepthFormat.None);
        }

        static void Dispose(RenderTarget2D Target)
        {
            if (Target != null && !Target.IsDisposed)
            {
                Target.Dispose();
            }
        }
    }
}
