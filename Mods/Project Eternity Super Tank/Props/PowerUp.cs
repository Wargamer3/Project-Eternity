using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.SuperTankScreen
{
    public class PowerUp : AnimatedSprite
    {
        public enum PowerUpTypes { VehiculeSpeed, Shield, VehiculeRegen, VehiculeRepair, Power1, Power2, Power3, RateOfFire1, RateOfFire2, RateOfFire3, Speed1, Speed2, Speed3, Flak, Missile, GuidedMissile, Laser, Orb, Turret, Shuriken };

        public static List<PowerUp> ListPowerUp = new List<PowerUp>();
        public static void AddPowerUp(PowerUp NewPowerUp)
        {
            for (int B = 0; B < PowerUp.ListPowerUp.Count; B++)
            {
                if (PowerUp.ListPowerUp[B].VisibleTime <= 0)
                {
                    PowerUp.ListPowerUp[B] = NewPowerUp;
                    return;
                }
            }

            PowerUp.ListPowerUp.Add(NewPowerUp);
        }

        public static PowerUpTypes[] ArrayPowerUpChoice;
        public static Color[] ActiveMask;

        public float VisibleTime = 60;
        public PowerUpTypes PowerUpType;

        public PowerUp(Vector2 Position, PowerUpTypes PowerUpType)
        {
            this.Position = Position;
            this.PowerUpType = PowerUpType;

            SpriteWidth = 24;
            SpriteHeight = 24;
            Origin.Y = 24;
            this.Mask = PowerUp.ActiveMask;
        }
    }
}
