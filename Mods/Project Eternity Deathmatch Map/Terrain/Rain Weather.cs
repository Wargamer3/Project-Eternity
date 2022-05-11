using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class RainWeather : DefaultWeather
    {
        private Vector2 SpawnOffset;
        private Vector2 SpawnOffsetRandom;
        Vector2 SpawnSpeed;
        Vector2 SpawnSpeedRandom;
        private double ParticlesPerSeconds;
        private double TimeElapsedSinceLastParticle;
        private double TimeBetweenEachParticle;

        RenderTarget2D RainRenderTarget;
        Effect DisplacementEffect;
        Texture2D sprRain;
        List<Rain> ListRain;

        Texture2D RippleTexture;
        Texture2D Droplet;
        Texture2D sprSkyTexture;
        Effect RippleEffect;
        Effect WetEffect;
        RenderTarget2D RippleRenderTarget;
        RenderTargetCube RefCubeMap;
        Model SkyModel;
        Matrix[] SkyBones;
        double Time;

        private struct Rain
        {
            public Vector2 Position;
            public Vector2 Speed;
            public float Angle;
            public double Lifetime;

            public Rain(Vector2 Position, Vector2 Speed, float Angle)
            {
                this.Position = Position;
                this.Speed = Speed;
                this.Angle = Angle;
                Lifetime = 5;
            }

            public Rain Update(double TimeElapsedInSeconds)
            {
                Lifetime -= TimeElapsedInSeconds;
                Position += Speed;
                return this;
            }
        }

        public RainWeather(DeathmatchMap Map, ZoneShape Shape)
            : base(Map, Shape)
        {
            ListRain = new List<Rain>();
        }

        public void Init()
        {
            sprRain = Map.Content.Load<Texture2D>("Line");
            DisplacementEffect = Map.Content.Load<Effect>("Shaders/Displacement");
            // Look up the resolution and format of our main backbuffer.
            PresentationParameters pp = GameScreen.GraphicsDevice.PresentationParameters;

            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;

            SurfaceFormat format = pp.BackBufferFormat;

            RainRenderTarget = new RenderTarget2D(GameScreen.GraphicsDevice, width, height, false,
                                                   format, pp.DepthStencilFormat, pp.MultiSampleCount,
                                                   RenderTargetUsage.DiscardContents);
            Matrix View = Matrix.Identity;

            Matrix Projection = Matrix.CreateOrthographicOffCenter(0, Constants.Width, Constants.Height, 0, 0, -1);
            Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            Projection = HalfPixelOffset * Projection;

            DisplacementEffect.Parameters["View"].SetValue(View);
            DisplacementEffect.Parameters["Projection"].SetValue(Projection);
            DisplacementEffect.Parameters["World"].SetValue(Matrix.Identity);
            DisplacementEffect.Parameters["TextureSize"].SetValue(new Vector2(1f / width, 1f / height));

            SpawnOffset = new Vector2( -100, -100);
            SpawnOffsetRandom = new Vector2(Constants.Width, Constants.Height);
            SpawnSpeed = new Vector2(5, 5);
            SpawnSpeedRandom = new Vector2(3, 3);

            ParticlesPerSeconds = 300;

            TimeBetweenEachParticle = 1 / ParticlesPerSeconds;

            //Wet weather
            RippleTexture = Map.Content.Load<Texture2D>("3D/Textures/RippleNormal");
            sprSkyTexture = Map.Content.Load<Texture2D>("3D/Textures/Sky");
            Droplet = Map.Content.Load<Texture2D>("3D/Textures/Glass Droplets");

            RefCubeMap = new RenderTargetCube(GameScreen.GraphicsDevice, 256, true, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            SkyModel = Map.Content.Load<Model>("3D/Models/Sphere");
            SkyBones = new Matrix[this.SkyModel.Bones.Count];
            SkyModel.CopyAbsoluteBoneTransformsTo(SkyBones);

            RippleRenderTarget = new RenderTarget2D(GameScreen.GraphicsDevice, Constants.Width, Constants.Height, false, SurfaceFormat.Color, DepthFormat.None);

            WetEffect = Map.Content.Load<Effect>("Shaders/Water");
            RippleEffect = Map.Content.Load<Effect>("Shaders/Ripple");

            RippleEffect.Parameters["View"].SetValue(View);
            RippleEffect.Parameters["Projection"].SetValue(Projection);
            RippleEffect.Parameters["World"].SetValue(Matrix.Identity);
            RippleEffect.Parameters["RippleTexture"].SetValue(RippleTexture);
            RippleEffect.Parameters["Time"].SetValue(81.139168f);
            RippleEffect.Parameters["TextureSize"].SetValue(256f);
            RippleEffect.Parameters["RainIntensity"].SetValue(1f);

            WetEffect.Parameters["DropletsTexture"].SetValue(Droplet);
            WetEffect.Parameters["View"].SetValue(View);
            WetEffect.Parameters["Projection"].SetValue(Projection);
            WetEffect.Parameters["World"].SetValue(Matrix.Identity);
            WetEffect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Invert(Matrix.Identity));

            WetEffect.Parameters["MaxPuddleDepth"].SetValue(1f);
            WetEffect.Parameters["WaterLevel"].SetValue(0.7f);
            WetEffect.Parameters["MinWaterLevel"].SetValue(0.5f);
            WetEffect.Parameters["WetLevel"].SetValue(1f);
            WetEffect.Parameters["RainIntensity"].SetValue(1f);

            WetEffect.Parameters["CameraPosition"].SetValue(new Vector4(700, 600, 2000, 1f));
            WetEffect.Parameters["LightPosition"].SetValue(new Vector3(1000, 3500, 3500));
            WetEffect.Parameters["LightIntensity"].SetValue(1f);
            WetEffect.Parameters["LightDirection"].SetValue(Vector3.Normalize(new Vector3(80, 15, 8)));

            DicTile3DByTileset.Clear();
            for (int L = 0; L < 1; L++)
            {
                CreateMap(Map, Map.LayerManager.ListLayer[L], WetEffect);
            }
        }

        public override void Update(GameTime gameTime)
        {
            double TimeElapsedInSeconds = gameTime.ElapsedGameTime.TotalSeconds;
            TimeElapsedSinceLastParticle += TimeElapsedInSeconds;
            Time += TimeElapsedInSeconds;

            for (int R = 0; R < ListRain.Count; R++)
            {
                if (ListRain[R].Lifetime > 0)
                {
                    ListRain[R] = ListRain[R].Update(TimeElapsedInSeconds);
                }
            }

            while (TimeElapsedSinceLastParticle >= TimeBetweenEachParticle)
            {
                TimeElapsedSinceLastParticle -= TimeBetweenEachParticle;
                Vector2 SpawnPosition = new Vector2(SpawnOffset.X + (float)RandomHelper.NextDouble() * SpawnOffsetRandom.X,
                    SpawnOffset.Y + (float)RandomHelper.NextDouble() * SpawnOffsetRandom.Y);

                Vector2 ParticleSpeed = new Vector2(SpawnSpeed.X + (float)RandomHelper.NextDouble() * SpawnSpeedRandom.X, SpawnSpeed.Y + (float)RandomHelper.NextDouble() * SpawnSpeedRandom.Y);

                bool RainFound = false;
                for (int R = 0; R < ListRain.Count; R++)
                {
                    Rain ActiveRainDrop = ListRain[R];
                    if (ActiveRainDrop.Lifetime <= 0)
                    {
                        ListRain[R] = new Rain(SpawnPosition, ParticleSpeed, (float)Math.Atan2(-ParticleSpeed.Y, -ParticleSpeed.X));
                        RainFound = true;
                        break;
                    }
                }
                if (!RainFound)
                {
                    ListRain.Add(new Rain(SpawnPosition, ParticleSpeed, (float)Math.Atan2(-ParticleSpeed.Y, -ParticleSpeed.X)));
                }
            }
        }

        void DrawSky(Matrix ViewMatrix)
        {
            foreach (ModelMesh mesh in SkyModel.Meshes)
            {
                // This is where the mesh orientation is set, as well as our camera and projection.  
                foreach (BasicEffect effectB in mesh.Effects)
                {
                    effectB.TextureEnabled = true;
                    effectB.EnableDefaultLighting();
                    effectB.PreferPerPixelLighting = true;


                    effectB.AmbientLightColor = new Vector3(1.0f, 1.0f, 1.0f);
                    effectB.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
                    effectB.SpecularColor = new Vector3(1.0f, 0.0f, 0.0f);
                    effectB.SpecularPower = 32;
                    effectB.Texture = sprSkyTexture;
                    effectB.World = SkyBones[mesh.ParentBone.Index] * Matrix.CreateRotationX((float)Time / 10) * Matrix.CreateRotationY((float)Time / 7) * Matrix.CreateScale(30);
                    effectB.View = ViewMatrix;
                    effectB.Projection = Matrix.CreateOrthographicOffCenter(-Constants.Width / 2, Constants.Width / 2, Constants.Height / 2, -Constants.Height / 2, 5000, -5000f);
                }
                mesh.Draw();
            }
        }

        private void DrawSkyCube()
        {
            GameScreen.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            //Sky cube
            for (int i = 0; i < 6; i++)
            {
                // render the scene to all cubemap faces
                CubeMapFace cubeMapFace = (CubeMapFace)i;
                Matrix CubeViewMatrix = Matrix.Identity;

                switch (cubeMapFace)
                {
                    case CubeMapFace.NegativeX:
                        {
                            CubeViewMatrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.Left, Vector3.Up);
                            break;
                        }
                    case CubeMapFace.NegativeY:
                        {
                            CubeViewMatrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.Down, Vector3.Forward);
                            break;
                        }
                    case CubeMapFace.NegativeZ:
                        {
                            CubeViewMatrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.Backward, Vector3.Up);
                            break;
                        }
                    case CubeMapFace.PositiveX:
                        {
                            CubeViewMatrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.Right, Vector3.Up);
                            break;
                        }
                    case CubeMapFace.PositiveY:
                        {
                            CubeViewMatrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.Up, Vector3.Backward);
                            break;
                        }
                    case CubeMapFace.PositiveZ:
                        {
                            CubeViewMatrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.Forward, Vector3.Up);
                            break;
                        }
                }

                // Set the cubemap render target, using the selected face
                GameScreen.GraphicsDevice.SetRenderTarget(RefCubeMap, cubeMapFace);
                GameScreen.GraphicsDevice.Clear(Color.White);
                DrawSky(CubeViewMatrix);
            }

            GameScreen.GraphicsDevice.SetRenderTarget(null);

            GameScreen.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        }

        private void DrawRipples(CustomSpriteBatch g)
        {
            RippleEffect.Parameters["Time"].SetValue((float)Time);
            g.End();
            g.GraphicsDevice.SetRenderTarget(RippleRenderTarget);
            g.GraphicsDevice.Clear(Color.Transparent);
            g.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, RippleEffect);
            g.Draw(RippleTexture, new Rectangle(0, 0, Constants.Width, Constants.Height), Color.White);
            g.End();
            g.GraphicsDevice.SetRenderTarget(null);
            g.Begin();
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            DrawRipples(g);
            DrawSkyCube();

            g.End();
            g.Begin();

            g.GraphicsDevice.SetRenderTarget(RainRenderTarget);
            g.GraphicsDevice.Clear(Color.Transparent);

            foreach (Rain ActiveRainDrop in ListRain)
            {
                g.Draw(sprRain, ActiveRainDrop.Position, null, Color.White, ActiveRainDrop.Angle, new Vector2(sprRain.Width / 2, sprRain.Height / 2), 1f, SpriteEffects.None, 0);
            }

            g.End();

            g.GraphicsDevice.SetRenderTarget(Map.MapRenderTarget);
            g.GraphicsDevice.Clear(Color.White);
            foreach (var ActiveTileset in DicTile3DByTileset.Values)
            {
                ActiveTileset.WetEffect.Parameters["Time"].SetValue((float)Time);
                ActiveTileset.WetEffect.Parameters["RippleTexture"].SetValue(RippleRenderTarget);
                ActiveTileset.WetEffect.Parameters["ReflectionCubeMap"].SetValue(RefCubeMap);
                ActiveTileset.Draw(g);
            }

            g.GraphicsDevice.SetRenderTarget(null);
            g.Begin();
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.End();

            g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, DisplacementEffect);
            g.GraphicsDevice.Textures[1] = RainRenderTarget;
            g.GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;
            g.Draw(Map.MapRenderTarget, Vector2.Zero, Color.White);
            g.End();
            g.Begin();
        }

        public override void EndDraw(CustomSpriteBatch g)
        {
            g.End();
            g.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
            for (int R = 0; R < ListRain.Count; R += 1)
            {
                Rain ActiveRainDrop = ListRain[R];
                if (ActiveRainDrop.Lifetime > 0)
                g.Draw(sprRain, ActiveRainDrop.Position, null, Color.FromNonPremultiplied(255, 255, 255, 70), ActiveRainDrop.Angle, new Vector2(sprRain.Width / 2, sprRain.Height / 2), 1f, SpriteEffects.None, 0);
            }
            g.End();
            g.Begin();
        }

        public override void SetCrossfadeValue(double Value)
        {
        }
    }
}
