using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core
{
    public class DefaultCamera : Camera3D
    {
        private Vector3 Target;

        public DefaultCamera(GraphicsDevice g)
            : base(g)
        {
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

            CameraPosition3D = Vector3.Transform(new Vector3(500, 0, 500), Matrix.CreateRotationY(0.2f)) + Target;
            CameraPosition3D = Vector3.Transform(CameraPosition3D, Matrix.CreateTranslation(0f, 600, 0f));
            View = Matrix.CreateLookAt(CameraPosition3D, Target, CameraRotation.Up);
        }
    }
}
