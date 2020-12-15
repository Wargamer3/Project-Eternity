using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.SuperTankScreen
{
    public class TankMissile : Bullet
    {
        public TankMissile(Vector2 Position, Color[] Mask, AnimatedSprite Clone)
            : base(Position, Mask, 0, Clone)
        {
        }

        public override void Update(GameTime gameTime)
        {
            new Propulsor(Position, Vector2.Zero).AddParticule();

            base.Update(gameTime);
        }

        public override void Destroyed(Vehicule Owner)
        {
        }
    }
    public class TankGuidedMissile : Bullet
    {
        int LockOnTime;
        float MissileSpeed;

        public TankGuidedMissile(Vector2 Position, float MissileSpeed, Color[] Mask, AnimatedSprite Clone)
            : base(Position, Mask, 0, Clone)
        {
            LockOnTime = 30;
            this.MissileSpeed = MissileSpeed;
        }

        public override void Update(GameTime gameTime)
        {
            if (LockOnTime > 0)
            {
                --LockOnTime;
            }
            else
            {
                Speed = Vector2.Zero;
                float MinDistance = float.MaxValue;
                int ActiveEnemy = -1;
                for (int E = 0; E < Enemy.ListEnemy.Count; E++)
                {
                    if (Enemy.ListEnemy[E].Resist <= 0)
                        continue;

                    float Distance = Vector2.Distance(Position, Enemy.ListEnemy[E].Position);
                    if (Distance < MinDistance)
                    {
                        MinDistance = Distance;
                        ActiveEnemy = E;
                    }
                }
                if (ActiveEnemy >= 0)
                {
					Angle = (float)Math.Atan2(Enemy.ListEnemy[ActiveEnemy].Position.Y - Position.Y, Enemy.ListEnemy[ActiveEnemy].Position.X - Position.X);
                    SetSpeed(Angle, MissileSpeed);
                }
            }
            new Propulsor(Position, Vector2.Zero).AddParticule();

            Position += Speed;

            UpdateTransformationMatrix();

            if (Position.X < -10 || Position.X > Constants.Width || Position.Y < 0 || Position.Y > Constants.Height)
                Resist = 0;
        }

        public override void Destroyed(Vehicule Owner)
        {
        }
    }
}