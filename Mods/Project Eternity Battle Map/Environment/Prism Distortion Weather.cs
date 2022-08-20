using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class PrismDistortionWeather : BattleMapOverlay
    {
        private Vector3 SpawnOffset;
        private Vector3 SpawnOffsetRandom;
        Vector3 SpawnSpeed;
        Vector3 SpawnSpeedRandom;
        private double ParticlesPerSeconds;
        private double TimeElapsedSinceLastParticle;
        private double TimeBetweenEachParticle;
        private int RenderWidth;
        private int RenderHeight;

        BattleMap Map;
        RenderTarget2D RainRenderTarget;
        public Core.ParticleSystem.ParticleSystem3D ParticleSystem;

        public PrismDistortionWeather(BattleMap Map, ZoneShape Shape)
        {
            this.Map = Map;
        }

        public void Init()
        {
            // Look up the resolution and format of our main backbuffer.
            PresentationParameters pp = GameScreen.GraphicsDevice.PresentationParameters;

            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;

            SurfaceFormat format = pp.BackBufferFormat;

            RainRenderTarget = new RenderTarget2D(GameScreen.GraphicsDevice, width, height, false,
                                                   format, pp.DepthStencilFormat, pp.MultiSampleCount,
                                                   RenderTargetUsage.DiscardContents);
            Matrix View = Matrix.Identity;

            Matrix Projection = Matrix.CreateOrthographicOffCenter(0, 800, 600, 0, 0, 1);
            Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            Projection = View * (HalfPixelOffset * Projection);

            Core.ParticleSystem.ParticleSettings3D ParticleSettings = new Core.ParticleSystem.ParticleSettings3D();
            ParticleSettings.MaxParticles = 20000;
            ParticleSettings.MinScale = new Vector3(1, 1, 1);
            ParticleSettings.DurationInSeconds = 5d;
            ParticleSettings.Gravity = new Vector3(0, 0, 0);
            ParticleSettings.BlendState = BlendState.AlphaBlend;
            ParticleSettings.StartingAlpha = 0.7f;
            ParticleSettings.EndAlpha = 0.1f;
            ParticleSettings.Size = new Vector3(32, 32, 32);
            ParticleSettings.RotationSpeed = new Vector3(1, 1, 1);

            SpawnOffset = new Vector3( Constants.Width / 2 - 50, Constants.Height / 2 - 50, 0);
            SpawnOffsetRandom = new Vector3(100, 100, 0);
            SpawnSpeed = new Vector3(-2, -2, -2);
            SpawnSpeedRandom = new Vector3(5, 5, 5);

            ParticlesPerSeconds = 30;

            TimeBetweenEachParticle = 1 / ParticlesPerSeconds;
            ParticleSystem = new Core.ParticleSystem.ParticleSystem3D(ParticleSettings);
            ParticleSystem.LoadContent(Map.Content, GameScreen.GraphicsDevice, Projection);
        }

        public void Reset()
        {
        }

        public void Update(GameTime gameTime)
        {
            double TimeElapsedInSeconds = gameTime.ElapsedGameTime.TotalSeconds;
            TimeElapsedSinceLastParticle += TimeElapsedInSeconds;

            while (TimeElapsedSinceLastParticle >= TimeBetweenEachParticle)
            {
                TimeElapsedSinceLastParticle -= TimeBetweenEachParticle;
                Vector3 SpawnPosition = new Vector3(SpawnOffset.X + (float)RandomHelper.NextDouble() * SpawnOffsetRandom.X,
                    SpawnOffset.Y + (float)RandomHelper.NextDouble() * SpawnOffsetRandom.Y,
                    SpawnOffset.Z + (float)RandomHelper.NextDouble() * SpawnOffsetRandom.Z);

                Vector3 ParticleSpeed = new Vector3(SpawnSpeed.X + (float)RandomHelper.NextDouble() * SpawnSpeedRandom.X,
                    SpawnSpeed.Y + (float)RandomHelper.NextDouble() * SpawnSpeedRandom.Y,
                    SpawnSpeed.Z + (float)RandomHelper.NextDouble() * SpawnSpeedRandom.Z);

                if (ParticleSpeed.X == 0) ParticleSpeed.X = 1;
                if (ParticleSpeed.Y == 0) ParticleSpeed.Y = 1;
                ParticleSystem.AddParticle(SpawnPosition, ParticleSpeed);
            }

            ParticleSystem.Update(gameTime.ElapsedGameTime.TotalSeconds);
        }

        public void BeginDraw(CustomSpriteBatch g)
        {
            g.End();
            g.Begin();

            g.GraphicsDevice.SetRenderTarget(RainRenderTarget);
            g.GraphicsDevice.Clear(Color.Transparent);

            if (g.GraphicsDevice.PresentationParameters.BackBufferWidth != RenderWidth
                || g.GraphicsDevice.PresentationParameters.BackBufferHeight != RenderHeight)
            {
                RenderWidth = g.GraphicsDevice.PresentationParameters.BackBufferWidth;
                RenderHeight = g.GraphicsDevice.PresentationParameters.BackBufferHeight;
                Matrix view = Matrix.Identity;

                Matrix Projection = Matrix.CreateOrthographicOffCenter(0, RenderWidth, RenderHeight, 0, 0, 1);
                Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

                Projection = view * (HalfPixelOffset * Projection);
                ParticleSystem.parameters["ViewProjection"].SetValue(Projection);
            }

            ParticleSystem.parameters["t0"].SetValue(Map.MapRenderTarget);
            g.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            ParticleSystem.Draw(GameScreen.GraphicsDevice, new Vector3());
            g.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            g.End();
            g.Begin();
            g.GraphicsDevice.SetRenderTarget(null);
        }

        public void Draw(CustomSpriteBatch g)
        {
            g.End();
            g.Begin();
            g.Draw(RainRenderTarget, Vector2.Zero, Color.White);
            g.End();
            g.Begin();
        }

        public void EndDraw(CustomSpriteBatch g)
        {
        }

        public void SetCrossfadeValue(double Value)
        {
        }
    }
}
