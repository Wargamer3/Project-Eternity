using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class LifeSimFreeCamera : Camera3D
    {
        private readonly PlayerOverseer Owner;

        private float CameraSpeed = 100f;
        public float Yaw = MathHelper.ToRadians(11.5f);
        public float Pitch = MathHelper.ToRadians(45f);
        public float Distance = 255f;

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
            PlayerCharacter Cursor = Owner.InvisibleCharacterAsCursor;
            Vector3 NextPosition = Cursor.Position;
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

            MapInfo Here = Cursor.SharedMapContex.CurrentMapInfo;
            if (ChunkCross.Outside(Here, NextPosition.X, NextPosition.Z) && ChunkCross.TryMoveCursor(Cursor, NextPosition.X, NextPosition.Z, NextPosition.Y))
            {
                SetTarget(ChunkOrigin.Camera(Cursor.SharedMapContex.CurrentMapInfo, Cursor.Position));
                return;
            }

            LifeSimMap Map = Cursor.SharedMapContex.ActiveMap;
            float TileX = Map != null && Map.TileSize.X > 0 ? Map.TileSize.X : 32f;
            float TileY = Map != null && Map.TileSize.Y > 0 ? Map.TileSize.Y : TileX;
            float MaxX = Map != null && Map.MapSize.X > 0 ? Map.MapSize.X * TileX - 0.01f : 0f;
            float MaxZ = Map != null && Map.MapSize.Y > 0 ? Map.MapSize.Y * TileY - 0.01f : 0f;
            if (NextPosition.X < 0f)
            {
                NextPosition.X = 0f;
            }
            else if (NextPosition.X > MaxX)
            {
                NextPosition.X = MaxX;
            }

            if (NextPosition.Z < 0f)
            {
                NextPosition.Z = 0f;
            }
            else if (NextPosition.Z > MaxZ)
            {
                NextPosition.Z = MaxZ;
            }

            Cursor.SetPosition(NextPosition);
            SetTarget(ChunkOrigin.Camera(Here, NextPosition));
        }

        private void SetTarget(Vector3 Target)
        {
            Aim(Target);
        }

        public void Orbit(float MouseX, float MouseY)
        {
            Yaw -= MouseX * 0.005f;
            Pitch = MathHelper.Clamp(Pitch + MouseY * 0.005f, MathHelper.ToRadians(14f), MathHelper.ToRadians(80f));
        }

        public void Aim(Vector3 Target)
        {
            Matrix Boom = Matrix.CreateTranslation(0f, Distance, 0f) * Matrix.CreateFromYawPitchRoll(Yaw, Pitch, 0f);
            CameraPosition3D = Boom.Translation + Target;
            LookAtTarget = Target;
            View = Matrix.CreateLookAt(CameraPosition3D, Target, Vector3.Up);
        }

        public static void Frame(Camera3D Camera, Vector3 Target)
        {
            Frame(Camera, Target, 255f);
        }

        public static void Frame(Camera3D Camera, Vector3 Target, float Distance)
        {
            if (Camera == null)
            {
                return;
            }

            float YawRotation = MathHelper.ToRadians(11.5f);
            float PitchRotation = MathHelper.ToRadians(45f);
            Matrix FinalMatrix = Matrix.CreateTranslation(0, Distance, 0) * Matrix.CreateFromYawPitchRoll(YawRotation, PitchRotation, 0);
            Camera.CameraPosition3D = FinalMatrix.Translation + Target;
            Camera.LookAtTarget = Target;
            Camera.View = Matrix.CreateLookAt(Camera.CameraPosition3D, Target, Vector3.Up);
        }
    }
}
