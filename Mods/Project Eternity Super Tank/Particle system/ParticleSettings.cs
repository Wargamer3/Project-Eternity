#region File Description
//-----------------------------------------------------------------------------
// ParticleSettings.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Particle3DSample
{
    public class ParticleSettings
    {

        public ParticleSettings()
        {
        }

        public ParticleSettings(string TextureName, int MaxParticles, TimeSpan Duration, Vector2 Gravity, Vector2 Size, Vector2 MinScale)
        {
            this.TextureName = TextureName;
            this.MaxParticles = MaxParticles;
            this.Duration = Duration;
            this.Gravity = Gravity;
            this.Size = Size;
            this.MinScale = MinScale;
        }

        public string TextureName;
        public int MaxParticles;
        public TimeSpan Duration;
        public Vector2 Gravity;
        public Vector2 Size;
        public Vector2 MinScale;
        public float StartingAlpha;
        public float EndAlpha;

        public BlendState BlendState = BlendState.NonPremultiplied;
    }
}
