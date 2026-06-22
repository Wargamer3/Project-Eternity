using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class LifeSimFreeCamera : Camera3D
    {
        private readonly PlayerOverseer Owner;

        private float CameraSpeed = 100f;
        private float Camera3DDistance = 255;
        private float Camera3DYawAngle = 11.5f;
        private float Camera3DPitchAngle = 45;

        public LifeSimFreeCamera(GraphicsDevice g, PlayerOverseer Owner)
            : base(g)
        {
            this.Owner = Owner;
            CameraPosition3D = new Vector3(0, -150, 0f);
            CameraRotation = Matrix.Identity;
            RotateCameraPitch(2.9f);
        }

        public override void Update(GameTime gameTime)
        {
            SetTarget(Owner.InvisibleCharacterAsCursor.Position);

            Vector3 NextPosition = Owner.InvisibleCharacterAsCursor.Position;
            if (Owner.ActiveInputManager.InputUpHold())
            {
                NextPosition -= new Vector3(0, 0, CameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            else if (Owner.ActiveInputManager.InputDownHold())
            {
                NextPosition += new Vector3(0, 0, CameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            if (Owner.ActiveInputManager.InputLeftHold())
            {
                NextPosition -= new Vector3(CameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0, 0);
            }
            else if (Owner.ActiveInputManager.InputRightHold())
            {
                NextPosition += new Vector3(CameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0, 0);
            }

            if (Owner.InvisibleCharacterAsCursor.SharedMapContex.ActiveMap.IsInsideMap(NextPosition))
            {
                Owner.InvisibleCharacterAsCursor.SetPosition(NextPosition);
            }
        }

        private void SetTarget(Vector3 Target)
        {
            float YawRotation = MathHelper.ToRadians(Camera3DYawAngle);
            float PitchRotation = MathHelper.ToRadians(Camera3DPitchAngle);
            float RollRotation = 0;

            Matrix FinalMatrix = Matrix.CreateTranslation(0, Camera3DDistance, 0) * Matrix.CreateFromYawPitchRoll(YawRotation, PitchRotation, RollRotation);
            CameraPosition3D = FinalMatrix.Translation + Target;

            View = Matrix.CreateLookAt(CameraPosition3D, Target, Vector3.Up);
        }
    }
}
