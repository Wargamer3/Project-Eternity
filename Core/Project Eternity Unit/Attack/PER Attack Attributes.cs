using System;
using System.IO;

namespace ProjectEternity.Core.Attacks
{
    public struct PERAttackAttributes
    {
        public enum GroundCollisions { DestroySelf, Stop, Bounce }

        public float ProjectileSpeed;
        public bool AffectedByGravity;
        public bool CanBeShotDown;
        public byte MaxLifetime;

        public byte NumberOfProjectiles;
        public float MaxLateralSpread;
        public float MaxForwardSpread;

        public GroundCollisions GroundCollision;
        public byte BounceLimit;

        public PERAttackAttributes(BinaryReader BR)
        {
            ProjectileSpeed = BR.ReadSingle();
            AffectedByGravity = BR.ReadBoolean();
            CanBeShotDown = BR.ReadBoolean();
            MaxLifetime = BR.ReadByte();

            NumberOfProjectiles = BR.ReadByte();
            MaxLateralSpread = BR.ReadSingle();
            MaxForwardSpread = BR.ReadSingle();

            GroundCollision = (GroundCollisions)BR.ReadByte();
            BounceLimit = BR.ReadByte();
        }
    }
}
