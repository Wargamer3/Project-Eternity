using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core.ParticleSystem
{
    public struct ParticleSettingsNoTexture
    {
        public string TextureName;
        public int MaxParticles;
        public double DurationInSeconds;
        public Vector2 Gravity;
        public Vector2 MinScale;
        public Vector2 Size;
        public float StartingAlpha;
        public float EndAlpha;

        public BlendState BlendState;
    }
}
