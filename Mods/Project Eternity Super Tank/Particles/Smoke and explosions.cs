using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.SuperTankScreen
{
	public class Smoke : AnimatedSprite
    {
        public static List<Smoke> ListSmoke = new List<Smoke>();
        public static void AddSmoke(Smoke NewSmoke)
        {
            for (int B = 0; B < Smoke.ListSmoke.Count; B++)
            {
                if (!Smoke.ListSmoke[B].IsAlive)
                {
                    Smoke.ListSmoke[B] = NewSmoke;
                    return;
                }
            }

            Smoke.ListSmoke.Add(NewSmoke);
        }

        public Vector2 Speed;
        public bool IsAlive;
        private float AnimationSpritePosition;
        private int AnimationSpriteRealPosition;

        public Smoke(Vector2 Position, AnimatedSprite Clone, float Angle, float NewSpeed)
            : base(Clone.ActiveSprite, Clone.AnimationFrameCount, Clone.Origin, Clone.AnimationSpeed)
        {
            IsAlive = true;
            this.Position = Position;
            AnimationSpritePosition = 0;

            Speed.X = (float)Math.Cos(Angle) * NewSpeed;
            Speed.Y = (float)Math.Sin(Angle) * NewSpeed;
        }

        public void Update(GameTime gameTime)
        {
            Position += Speed;
            Speed.Y -= 0.7f;

            AnimationSpritePosition += AnimationSpeed;
            AnimationSpriteRealPosition = ((int)Math.Truncate(AnimationSpritePosition)) * SpriteWidth;

            if (AnimationSpritePosition >= AnimationFrameCount)
                IsAlive = false;
        }

        public void Draw(CustomSpriteBatch g)
        {
            SpriteSource.X = AnimationSpriteRealPosition;
            g.Draw(ActiveSprite, Position, SpriteSource, Color.White);
        }
    }

    public class PlaneSmoke : Smoke
    {
        public static AnimatedSprite SmokeSprite;

        public PlaneSmoke(Vector2 Position, float Angle, float NewSpeed)
            : base(Position, SmokeSprite, Angle, NewSpeed)
        {
        }
    }

    public class BombSmoke : Smoke
    {
        public static AnimatedSprite SmokeSprite;

        public BombSmoke(Vector2 Position, float Angle, float NewSpeed)
            : base(Position, SmokeSprite, Angle, NewSpeed)
        {
        }
    }

	public class Explosion : AnimatedSprite
	{
		public static List<Explosion> ListExplosion = new List<Explosion>();
		public static void AddExplosion(Explosion NewExplosion)
		{
			for (int B = 0; B < Explosion.ListExplosion.Count; B++)
			{
				if (!Explosion.ListExplosion[B].IsAlive)
				{
					Explosion.ListExplosion[B] = NewExplosion;
					return;
				}
			}

			Explosion.ListExplosion.Add(NewExplosion);
		}

        public Vector2 Speed;
        public bool IsAlive;
        private float AnimationSpritePosition;
        private int AnimationSpriteRealPosition;

		public Explosion(Vector2 Position, AnimatedSprite Clone, double Angle, double NewSpeed)
            : base(Clone.ActiveSprite, Clone.AnimationFrameCount, Clone.Origin, Clone.AnimationSpeed)
		{
			IsAlive = true;
			this.Position = Position;
			AnimationSpritePosition = 0;

			Speed.X = (float)(Math.Cos (Angle) * NewSpeed);
			Speed.Y = (float)(Math.Sin (Angle) * NewSpeed);
		}

        public void Update(GameTime gameTime)
        {
            Position += Speed;

            AnimationSpritePosition += AnimationSpeed;
            AnimationSpriteRealPosition = ((int)Math.Truncate(AnimationSpritePosition)) * SpriteWidth;

            if (AnimationSpritePosition >= AnimationFrameCount)
                IsAlive = false;
        }

        public void Draw(CustomSpriteBatch g)
        {
            SpriteSource.X = AnimationSpriteRealPosition;
            g.Draw(ActiveSprite, Position, SpriteSource, Color.White);
        }
    }

    public class Explosion1 : Explosion
	{
		public static AnimatedSprite ExplosionSprite;

        public Explosion1(Vector2 Position, float Angle, float NewSpeed)
            : base(Position, ExplosionSprite, Angle, NewSpeed)
        {
        }
    }
}
