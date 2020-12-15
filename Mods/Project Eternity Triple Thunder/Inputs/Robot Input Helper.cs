using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public abstract class RobotInputHelper : RobotInput
    {
        public Rectangle CameraBounds { get; private set; }
        private Point Border;
        private Point CameraSpeed;

        protected RobotAnimation Owner;

        public RobotInputHelper(RobotAnimation Owner, Rectangle CameraBounds)
        {
            this.Owner = Owner;
            this.CameraBounds = CameraBounds;

            Border = new Point(Constants.Width / 4, Constants.Height / 4);
            CameraSpeed = new Point(8, 8);
        }

        protected void UpdateCamera(Vector2 MousePosition)
        {
            if (MousePosition.X > Border.X && MousePosition.X < (Owner.Camera.Width - Border.X))
            {
                Owner.Camera.X = (int)Owner.Position.X - (Owner.Camera.Width / 2);
            }
            else if (Owner.Camera.X < ((Owner.Position.X - (Owner.Camera.Width / 2)) - Owner.ViewDistance))
            {
                Owner.Camera.X += CameraSpeed.X;
            }
            else if (Owner.Camera.Right > (((Owner.Position.X) + (Owner.Camera.Width / 2)) + Owner.ViewDistance))
            {
                Owner.Camera.X -= CameraSpeed.X;
            }
            else if (MousePosition.X > (Owner.Camera.Width - Border.X))
            {
                Owner.Camera.X = (int)((Owner.Position.X - (Owner.Camera.Width / 2)) + ((Border.X - (Owner.Camera.Width - MousePosition.X)) * (Owner.ViewDistance / Border.X)));
            }
            else if (MousePosition.X < Border.X)
            {
                Owner.Camera.X = (int)((Owner.Position.X - (Owner.Camera.Width / 2)) - ((Border.X - MousePosition.X) * (Owner.ViewDistance / Border.X)));
            }

            if (MousePosition.Y > Border.Y && MousePosition.Y < (Owner.Camera.Height - Border.Y))
            {
                Owner.Camera.Y = (int)Owner.Position.Y - (Owner.Camera.Height / 2);
            }
            else if (Owner.Camera.Y < ((Owner.Position.Y - (Owner.Camera.Height / 2)) - Owner.ViewDistance))
            {
                Owner.Camera.Y += CameraSpeed.Y;
            }
            else if (Owner.Camera.Bottom > ((Owner.Position.Y + (Owner.Camera.Height / 2)) + Owner.ViewDistance))
            {
                Owner.Camera.Y -= CameraSpeed.Y;
            }
            else if (MousePosition.Y > (Owner.Camera.Height - Border.Y))
            {
                Owner.Camera.Y = (int)((Owner.Position.Y - (Owner.Camera.Height / 2)) + ((this.Border.Y - (Owner.Camera.Height - MousePosition.Y)) * (Owner.ViewDistance / Border.Y)));
            }
            else if (MousePosition.Y < this.Border.Y)
            {
                Owner.Camera.Y = (int)((Owner.Position.Y - (Owner.Camera.Height / 2)) - ((this.Border.Y - MousePosition.Y) * (Owner.ViewDistance / Border.Y)));
            }

            if (Owner.Camera.X < CameraBounds.X)
            {
                Owner.Camera.X = CameraBounds.X;
            }
            else if (Owner.Camera.Right > CameraBounds.Right)
            {
                Owner.Camera.X = CameraBounds.Right - Owner.Camera.Width;
            }

            if (Owner.Camera.Y < CameraBounds.Y)
            {
                Owner.Camera.Y = CameraBounds.Y;
            }
            else if (Owner.Camera.Y + Owner.Camera.Height > CameraBounds.Bottom)
            {
                Owner.Camera.Y = CameraBounds.Bottom - Owner.Camera.Height;
            }
        }

        public void ResetCameraBounds(Rectangle CameraBounds)
        {
            this.CameraBounds = CameraBounds;
        }

        public abstract void Update(GameTime gameTime);
    }
}
