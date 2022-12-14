using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class SorcererStreetCamera : Camera3D
    {
        public SorcererStreetCamera(GraphicsDevice g)
            : base(g)
        {
        }

        public void TeleportCamera(Vector3 NewPosition)
        {
            CameraPosition3D = NewPosition;
        }

        public void SetRotation(float Yaw, float Pitch, float Roll)
        {
            CameraRotation = Matrix.Identity;
            RotateCameraPitch(Pitch);
            RotateCameraYaw(Yaw);
            RotateCameraRoll(Roll);
        }

        public override void Update(GameTime gameTime)
        {
            CameraRotation.Forward.Normalize();
            CameraRotation.Up.Normalize();
            CameraRotation.Right.Normalize();

            Vector3 target = CameraPosition3D + CameraRotation.Forward;

            View = Matrix.CreateLookAt(CameraPosition3D, target, CameraRotation.Up);
        }
    }
}
