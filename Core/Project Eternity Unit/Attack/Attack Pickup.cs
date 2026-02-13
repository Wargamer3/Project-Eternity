using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens;

namespace ProjectEternity.Core.Attacks
{
    public class TemporaryAttackPickup
    {
        public readonly Texture2D sprWeapon;
        public readonly UnitMap3D Attack3D;

        private readonly string SpritePath;
        public readonly string AttackName;

        public Vector3 Position;
        public byte Ammo;

        public Attack Owner;

        public TemporaryAttackPickup(Attack Owner, string SpritePath, Texture2D sprWeapon, Effect Effect3D)
        {
            this.Owner = Owner;
            this.SpritePath = SpritePath;
            this.AttackName = Owner.RelativePath;
            this.sprWeapon = sprWeapon;

            if (!string.IsNullOrEmpty(SpritePath))
            {
                Attack3D = new UnitMap3D(GameScreen.GraphicsDevice, Effect3D, sprWeapon, 1, 0f);
            }
        }
    }
}
