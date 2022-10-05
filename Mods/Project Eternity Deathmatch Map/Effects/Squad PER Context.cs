using Microsoft.Xna.Framework;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Attacks;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class SquadPERContext
    {
        public Squad Target;
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

        public void SetRobotContext(Squad ActiveRobotAnimation, Attack ActiveWeapon, Vector3 Angle, Vector3 Position)
        {
            Target = ActiveRobotAnimation;
            TargetWeapon = ActiveWeapon;
            TargetWeaponAngle = Angle;
            TargetWeaponPosition = Position;
        }
    }
}
