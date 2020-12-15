using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.SuperTankScreen
{
    public class BulletCase : AnimatedSprite
    {
        public Vector2 PositionOld;
        public Vector2 Speed;
        public bool IsAlive;

        public BulletCase(Vector2 Position, int AnimationIndex, AnimatedSprite Clone)
            : base(Clone.ActiveSprite, Clone.AnimationFrameCount, Clone.Origin, Clone.AnimationSpeed)
        {
            IsAlive = true;

            this.Position = Position;
            Speed = new Vector2();
        }

        public void SetSpeed(float Angle, float NewSpeed)
        {
            Speed.X = (float)Math.Cos(Angle) * NewSpeed;
            Speed.Y = (float)Math.Sin(Angle) * NewSpeed;
        }

        public void Update(GameTime gameTime)
        {
            Position += Speed;
            Speed += SuperTank2.Gravity;
            PositionOld = Position;

            Angle = (float)Math.Atan2(Speed.Y, Speed.X);

            if (Position.X < -10 || Position.X > Constants.Width || Position.Y < 0 || Position.Y > Constants.Height)
                IsAlive = false;
        }

        public void Draw(CustomSpriteBatch g, Vector2 Position, float ScaleX, float Angle, Color DrawingColor)
        {
            g.Draw(ActiveSprite, Position, SpriteSource, DrawingColor, Angle, Origin, new Vector2(ScaleX, 1), SpriteEffects.None, 0);
        }
    }
}
