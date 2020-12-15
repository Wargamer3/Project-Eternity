using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.SuperTankScreen
{
    class Shuriken : Bullet
    {
        public Shuriken(Vector2 Position, Color[] Mask, AnimatedSprite Clone)
            : base(Position, Mask, 0, Clone)
        {
        }

        public override void Update(GameTime gameTime)
        {
            Position += Speed;
            PositionOld = Position;

            Angle -= 5 * SuperTank2.DegToRad;

            UpdateTransformationMatrix(Scale);

            if (Position.X < -10 || Position.X > Constants.Width || Position.Y < 0 || Position.Y > Constants.Height)
                Resist = 0;
        }
    }
}
