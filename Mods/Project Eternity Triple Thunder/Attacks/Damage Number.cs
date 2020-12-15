using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class DamageNumber
    {
        public Vector2 Position;
        public int Damage;
        public float LifetimeRemaining;
        public float MaxLifetime;
        public SimpleAnimation DamageAnimation;

        public DamageNumber(Vector2 Position, int Damage, float MaxLifetime)
        {
            this.Position = Position;
            this.Damage = Damage;
            this.MaxLifetime = MaxLifetime;
            this.LifetimeRemaining = MaxLifetime;
        }
    }
}
