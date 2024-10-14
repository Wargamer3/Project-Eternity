using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class SquadPERContext
    {
        public SorcererStreetUnit Target;
        public Vector3 TargetWeaponAngle;
        public Vector3 TargetWeaponPosition;

        public SquadPERContext()
        {
        }

        public SquadPERContext(SquadPERContext GlobalContext)
        {
            Target = GlobalContext.Target;
            TargetWeaponAngle = GlobalContext.TargetWeaponAngle;
            TargetWeaponPosition = GlobalContext.TargetWeaponPosition;
        }

        public void SetRobotContext(SorcererStreetUnit ActiveRobotAnimation, Vector3 Angle, Vector3 Position)
        {
            Target = ActiveRobotAnimation;
            TargetWeaponAngle = Angle;
            TargetWeaponPosition = Position;
        }
    }
}
