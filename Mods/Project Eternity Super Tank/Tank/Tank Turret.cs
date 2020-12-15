using System;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.SuperTankScreen
{
    class TankTurret
    {
        public int hspeed;
        public Vector2 Position;
        public Tank Creator;
        public static AnimatedSprite ImageTourelle;
        public float AnimationSpritePosition;

        public TankTurret(Vector2 Position)
        {
            this.Position = Position;
            hspeed = -2 + SuperTank2.Randomizer.Next(4);
        }

        public void Update()
        {
            AnimationSpritePosition += 1 + hspeed / 6;
            if (AnimationSpritePosition >= ImageTourelle.AnimationFrameCount)
                AnimationSpritePosition -= ImageTourelle.AnimationFrameCount;

            if (Math.Abs(Position.X - Creator.Position.X) > 200 && !Creator.Vide)
            {
                if (Position.X > Creator.Position.X)
                    hspeed = -2 - SuperTank2.Randomizer.Next(Creator.VehiculeVitesse);
                if (Position.X < Creator.Position.X)
                    hspeed = 2 + SuperTank2.Randomizer.Next(Creator.VehiculeVitesse);
            }
            if (Creator.Vide)
                hspeed = 0;

            Position.X += hspeed;
        }
    }
}