using System;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.SuperTankScreen
{
    public class Laser : Bullet
    {
        public Laser(Vector2 Position, Color[] Mask, AnimatedSprite Clone)
            : base(Position, Mask, 0, Clone)
        {
            Scale.Y = 2;
        }

        public override void Update(GameTime gameTime)
        {
            Position += Speed;
            PositionOld = Position;

            Angle = (float)Math.Atan2(Speed.Y, Speed.X);

            Scale.X += 1.5f;
            Scale.Y -= 0.1f;

            UpdateTransformationMatrix(Scale);

            if (Scale.Y < 0.1)
                Resist = 0;
        }
    }
}