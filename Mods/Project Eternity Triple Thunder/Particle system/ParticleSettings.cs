#region File Description
//-----------------------------------------------------------------------------
// ParticleSettings.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Particle3DSample
{
    public struct ParticleSettings
    {
        public ParticleSettings(string TextureName, int MaxParticles, TimeSpan Duration, Vector2 Gravity, Vector2 Size, Vector2 MinScale, int NumberOfImages = 1)
        {
            this.TextureName = TextureName;
            this.MaxParticles = MaxParticles;
            this.Duration = Duration;
            this.Gravity = Gravity;
            this.MinScale = MinScale;
            this.NumberOfImages = NumberOfImages;
            StartingAlpha = 1f;
            EndAlpha = 0f;
            BlendState = BlendState.NonPremultiplied;
        }

        public string TextureName;
        public int MaxParticles;
        public TimeSpan Duration;
        public Vector2 Gravity;
        public Vector2 MinScale;
        public int NumberOfImages;
        public float StartingAlpha;
        public float EndAlpha;

        public BlendState BlendState;
    }
}
