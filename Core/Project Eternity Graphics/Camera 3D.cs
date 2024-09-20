using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.Core.Graphics
{
    public abstract class Camera3D
    {
        public Matrix View;
        public Matrix Projection;
        protected Matrix CameraRotation;
        public Vector3 CameraPosition3D;

        public Camera3D(GraphicsDevice g)
        {
            CameraPosition3D = new Vector3(607, 500, 659);
            CameraRotation = Matrix.Identity;
            RotateCameraPitch((float)-Math.PI / 8f);
            RotateCameraYaw((float)-Math.PI / 4f);
            float aspectRatio = g.Viewport.Width / (float)g.Viewport.Height;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                     aspectRatio,
                                                                     1, 10000);
        }

        public abstract void Update(GameTime gameTime);

        public void MoveCamera(float Speed, Vector3 Direction)
        {
            CameraPosition3D += Speed * Direction;
        }

        public void MoveCameraFoward(float Speed)
        {
            MoveCamera(Speed, CameraRotation.Forward);
        }

        public void MoveCameraStrafe(float Speed)
        {
            MoveCamera(Speed, CameraRotation.Right);
        }

        public void MoveCameraUpward(float Speed)
        {
            MoveCamera(Speed, CameraRotation.Up);
        }

        public void RotateCameraPitch(float Pitch)
        {
            CameraRotation *= Matrix.CreateFromAxisAngle(CameraRotation.Right, Pitch);
        }

        public void RotateCameraYaw(float Yaw)
        {
            CameraRotation *= Matrix.CreateFromAxisAngle(new Vector3(0, -1, 0), Yaw);
        }

        public void RotateCameraRoll(float Roll)
        {
            CameraRotation *= Matrix.CreateFromAxisAngle(CameraRotation.Forward, Roll);
        }
    }

    public class FreeCamera : Camera3D
    {
        private MouseState MouseStateCurrent;
        private MouseState MouseStateLast;
        private float CameraSpeed = 100f;

        public FreeCamera(GraphicsDevice g)
            : base(g)
        {
            CameraPosition3D = new Vector3(0, -150, 0f);
            CameraRotation = Matrix.Identity;
            RotateCameraPitch(2.9f);
        }

        public override void Update(GameTime gameTime)
        {
            CameraRotation.Forward.Normalize();
            CameraRotation.Up.Normalize();
            CameraRotation.Right.Normalize();

            MouseStateCurrent = Mouse.GetState();

            if (MouseStateCurrent.RightButton == ButtonState.Pressed)
            {
                RotateCameraPitch((MouseStateLast.Y - MouseStateCurrent.Y) / 100f);
                RotateCameraYaw((MouseStateLast.X - MouseStateCurrent.X) / 100f);
            }

            if (KeyboardHelper.KeyHold(Keys.W))
            {
                MoveCameraFoward(CameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            if (KeyboardHelper.KeyHold(Keys.S))
            {
                MoveCameraFoward(-CameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            if (KeyboardHelper.KeyHold(Keys.A))
            {
                MoveCameraStrafe(-CameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            if (KeyboardHelper.KeyHold(Keys.D))
            {
                MoveCameraStrafe(CameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            if (KeyboardHelper.KeyHold(Keys.E))
            {
                MoveCameraUpward(CameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            if (KeyboardHelper.KeyHold(Keys.Q))
            {
                MoveCameraUpward(-CameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            MouseStateLast = MouseStateCurrent;

            Vector3 target = CameraPosition3D + CameraRotation.Forward;

            View = Matrix.CreateLookAt(CameraPosition3D, target, CameraRotation.Up);
        }
    }
}
