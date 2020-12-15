using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.SuperTankScreen
{
    public class FlakBullet : Bullet
    {
        public FlakBullet(Vector2 Position, Color[] Mask, AnimatedSprite Clone)
            : base(Position, Mask, 0, Clone)
        {
        }

        public override void Update(GameTime gameTime)
        {
            new FlakTrace(Position).AddParticule();

            base.Update(gameTime);
        }

        public override void Destroyed(Vehicule Owner)
        {
            for (int i = 0; i < Owner.ArrayWeaponSecondary[0].WeaponSpecial; i++)
                Owner.AddBullet(new FlakFragment(Position));
        }
    }

    public class FlakTrace : Particule
    {
        public static AnimatedSprite FlakSprite;
        public static Particle3DSample.ParticleSystem ParticleSystem;
        public static Particle3DSample.ParticleSettings ParticleSettings;

        public FlakTrace(Vector2 Position)
            : base(FlakSprite, Position, Vector2.Zero)
        {
        }

        public override void AddParticule()
        {
            ParticleSystem.AddParticle(Position, Speed);
        }
    }

    public class FlakFragment : Bullet
    {
        public static AnimatedSprite FlakSprite;
        public static Color[] FlakMask;

        public FlakFragment(Vector2 Position)
            : base(Position, FlakMask, 0, FlakSprite)
        {
            Speed = new Vector2();
            Damage = 1;
            Resist = 1;
        }
    }
}