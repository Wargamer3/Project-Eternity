using Microsoft.Xna.Framework;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class SquadPERContext
    {
        public UnitConquest Target;
        public Attack TargetWeapon;
        public Vector3 TargetWeaponAngle;
        public Vector3 TargetWeaponPosition;

        public SquadPERContext()
        {
        }

        public SquadPERContext(SquadPERContext GlobalContext)
        {
            Target = GlobalContext.Target;
            TargetWeapon = GlobalContext.TargetWeapon;
            TargetWeaponAngle = GlobalContext.TargetWeaponAngle;
            TargetWeaponPosition = GlobalContext.TargetWeaponPosition;
        }

        public void SetRobotContext(UnitConquest ActiveRobotAnimation, Attack ActiveWeapon, Vector3 Angle, Vector3 Position)
        {
            Target = ActiveRobotAnimation;
            TargetWeapon = ActiveWeapon;
            TargetWeaponAngle = Angle;
            TargetWeaponPosition = Position;
        }
    }
}
