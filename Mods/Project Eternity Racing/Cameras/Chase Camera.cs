using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.RacingScreen
{
    public class ChaseCamera : Camera3D
    {
        private Vehicule TargetToChase;
        private Vector3 CameraOffset;
        private Vector3 TargetOffset;

        public ChaseCamera(GraphicsDevice g, Vehicule TargetToChase)
            : base(g)
        {
            CameraPosition3D = new Vector3(0, 0, 0f);

            CameraOffset = new Vector3(0, 10f, -20f);
            TargetOffset = new Vector3(0, 5, 0);

            this.TargetToChase = TargetToChase;
        }

        public override void Update(GameTime gameTime)
        {
            CameraRotation = TargetToChase.Rotation;
            CameraPosition3D = TargetToChase.Position - Vector3.Transform(CameraOffset, CameraRotation);
            Vector3 FinalTargetPosition = TargetToChase.Position - Vector3.Transform(TargetOffset, CameraRotation);

            View = Matrix.CreateLookAt(CameraPosition3D, FinalTargetPosition, Vector3.Down);
        }
    }
}
