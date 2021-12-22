using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core
{
    public class OrbitingCamera : Camera3D
    {
        private float Distance = 50;
        private float Angle;

        public OrbitingCamera(GraphicsDevice g)
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
            SetRotation(0f, 0f, 0f);
            CameraRotation.Forward.Normalize();
            CameraRotation.Up.Normalize();
            CameraRotation.Right.Normalize();

            Vector3 target = new Vector3(16, 0, 16);
            Angle += 0.1f;

            CameraPosition3D = Vector3.Transform(new Vector3(Distance, 0, Distance), Matrix.CreateRotationY(Angle)) + target;
            CameraPosition3D = Vector3.Transform(CameraPosition3D, Matrix.CreateTranslation(0f, 64, 0f));
            View = Matrix.CreateLookAt(CameraPosition3D, target, CameraRotation.Up);
        }
    }
}
