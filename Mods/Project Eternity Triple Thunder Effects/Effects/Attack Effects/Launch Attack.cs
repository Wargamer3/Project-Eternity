using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed class LaunchAttackEffect : TripleThunderAttackEffect
    {
        public static string Name = "Launch Attack";

        private string _WeaponName;
        private Weapon WeaponToUse;

        public LaunchAttackEffect()
            : this(null)
        {
        }

        public LaunchAttackEffect(TripleThunderAttackParams Params)
            : base(Name, false, Params)
        {
            _WeaponName = "";
        }

        protected override void Load(BinaryReader BR)
        {
            _WeaponName = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_WeaponName);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            WeaponToUse.Shoot(Params.LocalContext.Owner, Params.SharedParams.OwnerPosition, Params.SharedParams.OwnerAngle, new System.Collections.Generic.List<BaseAutomaticSkill>());

            return null;
        }

        protected override BaseEffect DoCopy()
        {
            LaunchAttackEffect NewEffect = new LaunchAttackEffect(Params);

            if (WeaponToUse == null && Params != null && Params.LocalContext.Owner != null)
            {
                WeaponToUse = Params.LocalContext.Owner.CreateWeapon(_WeaponName);
            }

            NewEffect._WeaponName = _WeaponName;
            NewEffect.WeaponToUse = WeaponToUse;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            LaunchAttackEffect NewEffect = (LaunchAttackEffect)Copy;

            if (Params != null && Params.LocalContext.Owner != null)
            {
                WeaponToUse = Params.LocalContext.Owner.CreateWeapon(NewEffect._WeaponName);
            }

            _WeaponName = NewEffect._WeaponName;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute("Projectile Speed.")]
        public string WeaponName
        {
            get { return _WeaponName; }
            set { _WeaponName = value; }
        }

        #endregion
    }
}
