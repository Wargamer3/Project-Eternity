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

        public override void Update(GameTime gameTime)
        {
        }
    }
}
