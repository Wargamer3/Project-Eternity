using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public abstract class AnimationBackground
    {
        public readonly string AnimationFullPath;
        protected Texture2D sprPixel;

        public readonly string BackgroundType;
        public float Yaw, Pitch, Roll;
        public Matrix CameraRotation;
        protected Vector3 CameraPosition;
        public bool IsEditor;
        public Vector3 Position { get { return CameraPosition; } }
        public Vector3 MoveSpeed;
        public Vector3 DefaultCameraPosition;
        public Vector3 DefaultCameraRotation;

        public AnimationBackground(string BackgroundType, string AnimationFullPath, ContentManager Content, GraphicsDevice g)
        {
            this.BackgroundType = BackgroundType;
            this.AnimationFullPath = AnimationFullPath;
            sprPixel = Content.Load<Texture2D>("Pixel");
        }

        public static AnimationBackground LoadAnimationBackground(string AnimationBackgroundPath, ContentManager Content, GraphicsDevice g)
        {
            switch (AnimationBackgroundPath.Substring(0, 14))
            {
                case "Backgrounds 2D":
                    return new AnimationBackground2D(AnimationBackgroundPath, Content, g);

                case "Backgrounds 3D":
                    return new AnimationBackground3D(AnimationBackgroundPath, Content, g);
            }

            return null;
        }

        public abstract void Save(BinaryWriter BW);

        public abstract void Update(GameTime gameTime);

        public void ResetCamera()
        {
            CameraPosition = DefaultCameraPosition;
            CameraRotation = Matrix.Identity;
            Yaw = DefaultCameraRotation.X;
            Pitch = DefaultCameraRotation.Y;
            Roll = DefaultCameraRotation.Z;
        }

        public abstract void Draw(CustomSpriteBatch g, int ScreenWidth, int ScreenHeight);
        public abstract void Draw3D(Camera3D Camera, Matrix World);
    }
}
