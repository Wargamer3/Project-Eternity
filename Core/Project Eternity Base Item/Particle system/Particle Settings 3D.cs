using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core.ParticleSystem
{
    public struct ParticleSettings3D
    {
        public int MaxParticles;
        public double DurationInSeconds;
        public Vector3 Gravity;
        public Vector3 Size;
        public Vector3 RotationSpeed;
        public Vector3 MinScale;
        public float StartingAlpha;
        public float EndAlpha;

        public BlendState BlendState;
    }
}
