using System;
using System.IO;
using System.Drawing.Design;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens.AnimationScreen;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed class CreateExplosionEffect : TripleThunderAttackEffect
    {
        public static string Name = "Create Explosion";
        
        public ExplosionOptions _ExplosionAttributes;

        public CreateExplosionEffect()
            : this(null)
        {
        }

        public CreateExplosionEffect(TripleThunderAttackParams Params)
            : base(Name, false, Params)
        {
            _ExplosionAttributes = new ExplosionOptions();
            _ExplosionAttributes.ExplosionAnimation = new SimpleAnimation();
        }

        protected override void Load(BinaryReader BR)
        {
            _ExplosionAttributes = new ExplosionOptions(BR);
            if (_ExplosionAttributes.ExplosionAnimation.Path != string.Empty && Params != null && Params.SharedParams.Content != null)
            {
                _ExplosionAttributes.ExplosionAnimation.Load(Params.SharedParams.Content, "Animations/Sprites/");
            }
        }

        protected override void Save(BinaryWriter BW)
        {
            _ExplosionAttributes.Save(BW);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            Vector2 ProjectileAngleVector = new Vector2(-(float)Math.Cos(Params.SharedParams.OwnerAngle), -(float)Math.Sin(Params.SharedParams.OwnerAngle));
            Params.LocalContext.Owner.CreateExplosion(Params.LocalContext.OwnerProjectile.Collision.ListCollisionPolygon[0].Center, _ExplosionAttributes, ProjectileAngleVector);

            return null;
        }

        protected override BaseEffect DoCopy()
        {
            CreateExplosionEffect NewEffect = new CreateExplosionEffect(Params);

            NewEffect._ExplosionAttributes = _ExplosionAttributes;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            CreateExplosionEffect NewEffect = (CreateExplosionEffect)Copy;

            _ExplosionAttributes = NewEffect._ExplosionAttributes;
        }

        [Editor(typeof(Selectors.ExplosionOptionsSelector), typeof(UITypeEditor)),
        CategoryAttribute("Effect Attributes"),
        DescriptionAttribute("")]
        public ExplosionOptions ExplosionAttributes
        {
            get
            {
                return _ExplosionAttributes;
            }
            set
            {
                _ExplosionAttributes = value;
            }
        }
    }
}
