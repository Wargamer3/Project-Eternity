using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed class RotateWeaponEffect : TripleThunderRobotEffect
    {
        public static string Name = "Rotate Weapon";

        int _Angle;
        int _ActiveWeaponIndex;

        public RotateWeaponEffect()
            : base(Name, false)
        {
            _Angle = 0;
            _ActiveWeaponIndex = 0;
        }

        public RotateWeaponEffect(TripleThunderRobotParams Params)
            : base(Name, false, Params)
        {
            _Angle = 0;
            _ActiveWeaponIndex = 0;
        }
        
        protected override void Load(BinaryReader BR)
        {
            _Angle = BR.ReadInt32();
            _ActiveWeaponIndex = BR.ReadInt32();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_Angle);
            BW.Write(_ActiveWeaponIndex);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            Params.LocalContext.Target.Weapons.ActivePrimaryWeapons[_ActiveWeaponIndex].WeaponAngle = _Angle;

            return null;
        }

        protected override BaseEffect DoCopy()
        {
            RotateWeaponEffect NewEffect = new RotateWeaponEffect(Params);

            NewEffect._Angle = _Angle;
            NewEffect._ActiveWeaponIndex = _ActiveWeaponIndex;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            RotateWeaponEffect NewEffect = (RotateWeaponEffect)Copy;

            _Angle = NewEffect._Angle;
            _ActiveWeaponIndex = NewEffect._ActiveWeaponIndex;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute("Angle in degrees.")]
        public int Angle
        {
            get { return _Angle; }
            set { _Angle = value; }
        }

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute("Index of the active weapon to use starting from 0.")]
        public int ActiveWeaponIndex
        {
            get { return _ActiveWeaponIndex; }
            set { _ActiveWeaponIndex = value; }
        }

        #endregion
    }
}
