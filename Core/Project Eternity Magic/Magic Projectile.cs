using System;
using Microsoft.Xna.Framework;

namespace ProjectEternity.Core.Magic
{
    public class MagicProjectile : Projectile
    {
        protected MagicUserParams MagicParams;

        public MagicProjectile(MagicUserParams MagicParams, double Lifetime)
            : base(Lifetime)
        {
            this.MagicParams = MagicParams;
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }

        public override void SetAngle(float Angle)
        {
        }
    }
}
