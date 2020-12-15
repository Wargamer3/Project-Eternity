using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed class ShootWeaponEffect : TripleThunderRobotEffect
    {
        public static string Name = "Shoot Weapon";

        public ShootWeaponEffect()
            : base(Name, false)
        {
        }

        public ShootWeaponEffect(TripleThunderRobotParams Params)
            : base(Name, false, Params)
        {
        }
        
        protected override void Load(BinaryReader BR)
        {
        }

        protected override void Save(BinaryWriter BW)
        {
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            Params.LocalContext.TargetWeapon.Shoot(Params.LocalContext.Target, Params.LocalContext.TargetWeaponPosition, Params.LocalContext.TargetWeaponAngle, ListFollowingSkill);
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            ShootWeaponEffect NewEffect = new ShootWeaponEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
