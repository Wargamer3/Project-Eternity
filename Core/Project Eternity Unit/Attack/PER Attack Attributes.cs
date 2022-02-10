using System;
using System.IO;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.Core.Attacks
{
    public struct PERAttackAttributes
    {
        public enum GroundCollisions { DestroySelf, Stop, Bounce }

        public float ProjectileSpeed;
        public bool AffectedByGravity;
        public bool CanBeShotDown;
        public bool Homing;
        public byte MaxLifetime;

        public SimpleAnimation ProjectileAnimation;

        public byte NumberOfProjectiles;
        public float MaxLateralSpread;
        public float MaxForwardSpread;
        public float MaxUpwardSpread;

        public GroundCollisions GroundCollision;
        public byte BounceLimit;

        public PERAttackAttributes(BinaryReader BR, ContentManager Content)
        {
            ProjectileSpeed = BR.ReadSingle();
            AffectedByGravity = BR.ReadBoolean();
            CanBeShotDown = BR.ReadBoolean();
            Homing = BR.ReadBoolean();
            MaxLifetime = BR.ReadByte();

            ProjectileAnimation = new SimpleAnimation(BR, false);
            if (Content != null)
            {
                ProjectileAnimation.Load(Content, "Animations/Sprites/");
            }

            NumberOfProjectiles = BR.ReadByte();
            MaxLateralSpread = BR.ReadSingle();
            MaxForwardSpread = BR.ReadSingle();
            MaxUpwardSpread = BR.ReadSingle();

            GroundCollision = (GroundCollisions)BR.ReadByte();
            if (GroundCollision == GroundCollisions.Bounce)
            {
                BounceLimit = BR.ReadByte();
            }
            else
            {
                BounceLimit = 0;
            }
        }
    }
}
