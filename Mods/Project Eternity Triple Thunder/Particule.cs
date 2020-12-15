using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class Propulsor
    {
        public static Particle3DSample.ParticleSystem ParticleSystem;
        public static Particle3DSample.ParticleSettings ParticleSettings;
        public Vector2 Position;
        public Vector2 Speed;

        public Propulsor(Vector2 Position, Vector2 Speed)
        {
            this.Position = Position;
            this.Speed = Speed;
        }

        public Propulsor(Vector2 Position, float Angle, double Speed)
        {
            this.Position = Position;
            SetSpeed(Angle, Speed);
        }

        public void SetSpeed(float Angle, double NewSpeed)
        {
            Speed.X = (float)(Math.Cos(Angle) * NewSpeed);
            Speed.Y = (float)(Math.Sin(Angle) * NewSpeed);
        }
    }
}
