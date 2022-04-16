using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class RainWeather : BattleMapOverlay
    {
        private Vector2 SpawnOffset;
        private Vector2 SpawnOffsetRandom;
        Vector2 SpawnSpeed;
        Vector2 SpawnSpeedRandom;
        private double ParticlesPerSeconds;
        private double TimeElapsedSinceLastParticle;
        private double TimeBetweenEachParticle;
        private int RenderWidth;
        private int RenderHeight;

        BattleMap Map;
        RenderTarget2D RainRenderTarget;
        Effect DisplacementEffect;
        Texture2D sprRain;
        public Core.ParticleSystem.ParticleSystem2D ParticleSystem;
        List<Rain> ListRain;

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

        public RainWeather(BattleMap Map, ZoneShape Shape)
        {
            this.Map = Map;
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

            Matrix Projection = Matrix.CreateOrthographicOffCenter(0, 800, 600, 0, 0, 1);
            Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            Projection = View * (HalfPixelOffset * Projection);

            DisplacementEffect.Parameters["View"].SetValue(View);
            DisplacementEffect.Parameters["Projection"].SetValue(Projection);
            DisplacementEffect.Parameters["World"].SetValue(Matrix.Identity);
            DisplacementEffect.Parameters["TextureSize"].SetValue(new Vector2(1f / width, 1f / height));

            Core.ParticleSystem.ParticleSettings ParticleSettings = new Core.ParticleSystem.ParticleSettings();
            ParticleSettings.TextureName = "Line";
            ParticleSettings.MaxParticles = 20000;
            ParticleSettings.MinScale = new Vector2(1, 1);
            ParticleSettings.DurationInSeconds = 5d;
            ParticleSettings.Gravity = new Vector2(0, 0);
            ParticleSettings.NumberOfImages = 1;
            ParticleSettings.BlendState = BlendState.AlphaBlend;
            ParticleSettings.StartingAlpha = 0.7f;
            ParticleSettings.EndAlpha = 0.1f;

            SpawnOffset = new Vector2( -100, -100);
            SpawnOffsetRandom = new Vector2(Constants.Width, Constants.Height);
            SpawnSpeed = new Vector2(3, 3);
            SpawnSpeedRandom = new Vector2(3, 3);

            ParticlesPerSeconds = 300;

            TimeBetweenEachParticle = 1 / ParticlesPerSeconds;
            ParticleSystem = new Core.ParticleSystem.ParticleSystem2D(ParticleSettings);
            ParticleSystem.LoadContent(Map.Content, GameScreen.GraphicsDevice, Projection, "Shaders/Displacement Particle shader");
        }

        public void Update(GameTime gameTime)
        {
            double TimeElapsedInSeconds = gameTime.ElapsedGameTime.TotalSeconds;
            TimeElapsedSinceLastParticle += TimeElapsedInSeconds;
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

                //ParticleSystem.AddParticle(SpawnPosition, ParticleSpeed);

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

            //ParticleSystem.Update(gameTime.ElapsedGameTime.TotalSeconds);
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

            //ParticleSystem.Draw(GameScreen.GraphicsDevice, new Vector2());
            foreach (Rain ActiveRainDrop in ListRain)
            {
                g.Draw(sprRain, ActiveRainDrop.Position, null, Color.White, ActiveRainDrop.Angle, new Vector2(sprRain.Width / 2, sprRain.Height / 2), 1f, SpriteEffects.None, 0);
            }

            g.End();
            g.Begin();
            g.GraphicsDevice.SetRenderTarget(null);
        }

        public void Draw(CustomSpriteBatch g)
        {
            Matrix Projection = Matrix.CreateOrthographicOffCenter(0, Constants.Width, Constants.Height, 0, 0, -1f);
            Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            Matrix projectionMatrix = HalfPixelOffset * Projection;
            DisplacementEffect.Parameters["Projection"].SetValue(projectionMatrix);
            g.End();
            g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, DisplacementEffect);
            g.GraphicsDevice.Textures[1] = RainRenderTarget;
            g.GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;
            g.Draw(Map.MapRenderTarget, Vector2.Zero, Color.White);
            g.End();
            g.Begin();
        }

        public void EndDraw(CustomSpriteBatch g)
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

        public void SetCrossfadeValue(double Value)
        {
        }
    }
}
