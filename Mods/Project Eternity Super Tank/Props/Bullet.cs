using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.SuperTankScreen
{
    public class Bullet : AnimatedSprite
    {
        public enum BulletTypes { Normal, Laser }

        public float Damage;
        public float Resist;
        public BulletTypes BulletType;

        protected Vector2 PositionOld;
        public Vector2 Speed;

        public Bullet(Vector2 Position, Color[] Mask, int AnimationIndex, AnimatedSprite Clone, BulletTypes BulletType = BulletTypes.Normal)
            : base(Clone.ActiveSprite, Mask, Clone.AnimationFrameCount, Clone.Origin, Clone.AnimationSpeed)
        {
            this.Position = Position;
            SpriteSource.X = AnimationIndex * SpriteWidth;
            Speed = new Vector2();
            this.BulletType = BulletType;
        }

        public void SetSpeed(float Angle, float NewSpeed)
        {
            Speed.X = (float)Math.Cos(Angle) * NewSpeed;
            Speed.Y = (float)Math.Sin(Angle) * NewSpeed;
        }

        public virtual void Update(GameTime gameTime)
        {
            Position += Speed;
            Speed += SuperTank2.Gravity;
            PositionOld = Position;

            Angle = (float)Math.Atan2(Speed.Y, Speed.X);

            UpdateTransformationMatrix();

            if (Position.X < -10 || Position.X > Constants.Width || Position.Y < 0 || Position.Y > Constants.Height)
				Resist = 0;
        }

        public virtual void Draw(CustomSpriteBatch g)
        {
			g.Draw(ActiveSprite, Position, SpriteSource, Color.White, Angle, Origin, Scale, SpriteEffects.None, 0);
        }

        public virtual void Destroyed(Vehicule Owner)
        {
        }
    }
}