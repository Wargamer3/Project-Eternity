using System;
using System.IO;
using System.Drawing.Design;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed class CreateExplosionEffect : TripleThunderAttackEffect
    {
        public static string Name = "Create Explosion";
        
        public Weapon.ExplosionOptions _ExplosionAttributes;

        public CreateExplosionEffect()
            : this(null)
        {
        }

        public CreateExplosionEffect(TripleThunderAttackParams Params)
            : base(Name, false, Params)
        {
            _ExplosionAttributes = new Weapon.ExplosionOptions();
            _ExplosionAttributes.ExplosionAnimation = new SimpleAnimation();
        }

        protected override void Load(BinaryReader BR)
        {
            _ExplosionAttributes = new Weapon.ExplosionOptions(BR);
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
        public Weapon.ExplosionOptions ExplosionAttributes
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
