using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class Propulsor
    {
        public static Particle3DSample.ParticleSystem ParticleSystem;

        public static void Load(ContentManager Content, GraphicsDevice GraphicsDevice, int Width, int Height)
        {
            Matrix view = Matrix.Identity;

            Matrix Projection = Matrix.CreateOrthographicOffCenter(0, Width, Height, 0, 0, 1);
            Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            Projection = view * (HalfPixelOffset * Projection);

            Particle3DSample.ParticleSettings PropulsorParticleSettings = new Particle3DSample.ParticleSettings();
            PropulsorParticleSettings.TextureName = "Triple Thunder/Jetpack Flare_strip16";
            PropulsorParticleSettings.MaxParticles = 20000;
            PropulsorParticleSettings.MinScale = new Vector2(1, 1);
            PropulsorParticleSettings.DurationInSeconds = 1d;
            PropulsorParticleSettings.Gravity = new Vector2(0, 0);
            PropulsorParticleSettings.NumberOfImages = 16;
            PropulsorParticleSettings.BlendState = BlendState.AlphaBlend;
            PropulsorParticleSettings.StartingAlpha = 0.7f;
            PropulsorParticleSettings.EndAlpha = 0.1f;
            ParticleSystem = new Particle3DSample.ParticleSystem(PropulsorParticleSettings);
            ParticleSystem.LoadContent(Content, GraphicsDevice, Projection);
        }
    }
}
