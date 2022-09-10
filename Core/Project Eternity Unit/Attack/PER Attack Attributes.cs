using System;
using System.IO;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.Core.Attacks
{
    public struct PERAttackAttributes
    {
        public enum GroundCollisions { DestroySelf, Stop, Bounce }
        public enum AttackTypes { Shoot, Throw, Kick, Hold }

        public float ProjectileSpeed;
        public bool AffectedByGravity;
        public bool CanBeShotDown;
        public bool Homing;
        public byte MaxLifetime;
        public AttackTypes AttackType;

        public SimpleAnimation ProjectileAnimation;
        public string Projectile3DModelPath;
        public AnimatedModel Projectile3DModel;

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
            AttackType = (AttackTypes)BR.ReadByte();

            ProjectileAnimation = new SimpleAnimation(BR, false);
            Projectile3DModelPath = BR.ReadString();
            Projectile3DModel = null;
            if (Content != null)
            {
                if (!string.IsNullOrEmpty(ProjectileAnimation.Path))
                {
                    ProjectileAnimation.Load(Content, "Animations/Sprites/");
                }

                if (!string.IsNullOrEmpty(Projectile3DModelPath))
                {
                    Projectile3DModel = new AnimatedModel("Attacks/Models/" + Projectile3DModelPath);
                    Projectile3DModel.LoadContent(Content);
                }
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
