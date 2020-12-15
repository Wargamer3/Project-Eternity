using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.SuperTankScreen
{
    class TankOrb
    {
        public Vector2 Position;
        public Tank Creator;
        public bool Regen;
        public double Dir;
        public float Resist;
        public double Alarm0;

        public TankOrb()
        {
            Position = Vector2.Zero;
            Creator = null;
            Regen = false;
        }
    }
}