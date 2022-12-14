using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.SuperTankScreen
{
    public abstract class Particule : AnimatedSprite
    {
        private static List<Particule> ListParticule = new List<Particule>();
        public virtual void AddParticule()
        {
            for (int P = 0; P < Particule.ListParticule.Count; P++)
            {
                if (!Particule.ListParticule[P].IsAlive)
                {
                    Particule.ListParticule[P] = this;
                    return;
                }
            }

            Particule.ListParticule.Add(this);
        }

        public Vector2 Speed;
        public int Alpha;
        public bool IsAlive;

		protected Particule(AnimatedSprite Clone, Vector2 Position, float Angle, double Speed)
            : base(Clone.ActiveSprite, Clone.AnimationFrameCount, Clone.Origin, Clone.AnimationSpeed)
        {
            IsAlive = true;
            Alpha = 255;
            this.Position = Position;
            SetSpeed(Angle, Speed);
        }
		protected Particule(AnimatedSprite Clone, Vector2 Position, Vector2 Speed)
            : base(Clone.ActiveSprite, Clone.AnimationFrameCount, Clone.Origin, Clone.AnimationSpeed)
        {
            IsAlive = true;
            Alpha = 255;
            this.Position = Position;
            this.Speed = Speed;
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public static void UpdateParticles(GameTime gameTime)
        {
            for (int P = 0; P < Particule.ListParticule.Count; P++)
            {
                if (Particule.ListParticule[P].IsAlive)
                {
                    Particule.ListParticule[P].Update(gameTime);
                }
            }
            Propulsor.ParticleSystem.Update(gameTime);
            FlameParticle.ParticleSystem.Update(gameTime);
            Boss1.BombFlame.ParticleSystem.Update(gameTime);
            FlakTrace.ParticleSystem.Update(gameTime);
        }

        public static void ClearParticles()
        {
            Propulsor.ParticleSystem.ClearParticles();
            FlameParticle.ParticleSystem.ClearParticles();
            Boss1.BombFlame.ParticleSystem.ClearParticles();
            FlakTrace.ParticleSystem.ClearParticles();
        }

        public static void Draw(CustomSpriteBatch g)
        {
            for (int P = 0; P < ListParticule.Count; P++)
            {
                if (Particule.ListParticule[P].IsAlive)
                {
                    g.Draw(ListParticule[P].ActiveSprite, ListParticule[P].Position, ListParticule[P].SpriteSource, Color.FromNonPremultiplied(255, 255, 255, ListParticule[P].Alpha), ListParticule[P].Angle, ListParticule[P].Origin, ListParticule[P].Scale, SpriteEffects.None, 0);
                }
            }

            g.End();
            Propulsor.ParticleSystem.Draw(GameScreen.GraphicsDevice);
            FlameParticle.ParticleSystem.Draw(GameScreen.GraphicsDevice);
            Boss1.BombFlame.ParticleSystem.Draw(GameScreen.GraphicsDevice);
            FlakTrace.ParticleSystem.Draw(GameScreen.GraphicsDevice);

            g.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        }

        public void SetSpeed(float Angle, double NewSpeed)
        {
            Speed.X = (float)(Math.Cos(Angle) * NewSpeed);
            Speed.Y = (float)(Math.Sin(Angle) * NewSpeed);
        }
    }

    public class Propulsor : Particule
    {
        public static AnimatedSprite PropulsorSprite;
        public static Particle3DSample.ParticleSystem ParticleSystem;
        public static Particle3DSample.ParticleSettings ParticleSettings;

        public Propulsor(Vector2 Position, Vector2 Speed)
            : base(PropulsorSprite, Position, Speed)
        {
        }

        public Propulsor(Vector2 Position, float Angle, double Speed)
            : base(PropulsorSprite, Position, Angle, Speed)
        {
        }

        public override void AddParticule()
        {
            ParticleSystem.AddParticle(Position, Speed);
        }
    }

    public class FlameParticle : Particule
    {
        public static AnimatedSprite FlameParticleSprite;
        public static Particle3DSample.ParticleSystem ParticleSystem;
        public static Particle3DSample.ParticleSettings ParticleSettings;

        public FlameParticle(Vector2 Position, Vector2 Speed)
            : base(FlameParticleSprite, Position, Speed)
        {
        }
        public FlameParticle(Vector2 Position, float Angle, double Speed)
            : base(FlameParticleSprite, Position, Angle, Speed)
        {
        }

        public override void AddParticule()
        {
            ParticleSystem.AddParticle(Position, Speed);
        }
    }
}
