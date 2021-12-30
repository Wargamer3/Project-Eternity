using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core
{
    public class DefaultCamera : Camera3D
    {
        private Vector3 Target;
        public float CameraHeight;
        public float CameraDistance;

        public DefaultCamera(GraphicsDevice g)
            : base(g)
        {
            CameraHeight = 400;
            CameraDistance = 300;
            Target = new Vector3(16, 0, 16);
        }

        public void TeleportCamera(Vector3 NewPosition)
        {
            CameraPosition3D = NewPosition;
        }

        public void SetTarget(Vector3 Target)
        {
            this.Target = Target;
        }

        public override void Update(GameTime gameTime)
        {
            CameraRotation.Forward.Normalize();
            CameraRotation.Up.Normalize();
            CameraRotation.Right.Normalize();

            CameraPosition3D = Vector3.Transform(new Vector3(CameraDistance, 0, CameraDistance), Matrix.CreateRotationY(0.2f)) + Target;
            CameraPosition3D = Vector3.Transform(CameraPosition3D, Matrix.CreateTranslation(0f, CameraHeight, 0f));
            View = Matrix.CreateLookAt(CameraPosition3D, Target, CameraRotation.Up);
        }
    }
}
