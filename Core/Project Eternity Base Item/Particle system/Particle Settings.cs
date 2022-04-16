using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core.ParticleSystem
{
    public struct ParticleSettings
    {
        public ParticleSettings(string TextureName, int MaxParticles, double DurationInSeconds, Vector2 Gravity, Vector2 Size, Vector2 MinScale, int NumberOfImages = 1)
        {
            this.TextureName = TextureName;
            this.MaxParticles = MaxParticles;
            this.DurationInSeconds = DurationInSeconds;
            this.Gravity = Gravity;
            this.MinScale = MinScale;
            this.NumberOfImages = NumberOfImages;
            StartingAlpha = 1f;
            EndAlpha = 0f;
            BlendState = BlendState.NonPremultiplied;
        }

        public string TextureName;
        public int MaxParticles;
        public double DurationInSeconds;
        public Vector2 Gravity;
        public Vector2 MinScale;
        public int NumberOfImages;
        public float StartingAlpha;
        public float EndAlpha;

        public BlendState BlendState;
    }
}
