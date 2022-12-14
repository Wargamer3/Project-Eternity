using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core.Graphics
{
    public class CrossArrow3D : IObject3D
    {
        public Arrow3D ArrowX;
        public Arrow3D ArrowY;
        public Arrow3D ArrowZ;

        private Arrow3D SelectedArrow;

        public CrossArrow3D(GraphicsDevice g, Matrix Projection)
        {
            ArrowX = new Arrow3D(g, Projection, Color.Red);
            ArrowX.Scale(new Vector3(1f, 1f, 5f));
            ArrowX.Position = new Vector3(0f, 0f, -5.5f);

            ArrowY = new Arrow3D(g, Projection, Color.Yellow);
            ArrowY.Scale(new Vector3(1f, 1f, 5f));
            ArrowY.Rotate(0f, MathHelper.ToRadians(270f), 0f);
            ArrowY.Position = new Vector3(0f, -4.5f, 0f);

            ArrowZ = new Arrow3D(g, Projection, Color.Blue);
            ArrowZ.Scale(new Vector3(1f, 1f, 5f));
            ArrowZ.Rotate(MathHelper.ToRadians(90f), 0f, 0f);
            ArrowZ.Position = new Vector3(-5.5f, 0f, 0f);
        }

        public void Rotate(float TotalYaw, float TotalPitch, float TotalRoll)
        {
            ArrowX.Rotate(TotalYaw, TotalPitch, TotalRoll);
            ArrowY.Rotate(TotalYaw, TotalPitch, TotalRoll);
            ArrowZ.Rotate(TotalYaw, TotalPitch, TotalRoll);
        }

        public void Scale(Vector3 TotalScale)
        {
            ArrowX.Scale(TotalScale);
            ArrowY.Scale(TotalScale);
            ArrowZ.Scale(TotalScale);
        }

        public void Teleport(Vector3 Destination)
        {
            ArrowX.Teleport(Destination);
            ArrowY.Teleport(Destination);
            ArrowZ.Teleport(Destination);
        }

        public void Draw(CustomSpriteBatch g, Matrix View)
        {
            ArrowX.Draw(g, View);
            ArrowY.Draw(g, View);
            ArrowZ.Draw(g, View);
        }

        public Vector3 Position
        {
            get
            {
                return ArrowX.Position - new Vector3(0f, 0f, -5.5f);
            }

            set
            {
                ArrowX.Position = new Vector3(0f, 0f, -5.5f) + value;
                ArrowY.Position = new Vector3(0f, -4.5f, 0f) + value;
                ArrowZ.Position = new Vector3(-5.5f, 0f, 0f) + value;
            }
        }

        public Vector3 Direction
        {
            get
            {
                return ArrowX.Direction;
            }

            set
            {
                ArrowX.Direction = value;
                ArrowY.Direction = value;
                ArrowZ.Direction = value;
            }
        }

        public Vector3 Size
        {
            get
            {
                return ArrowX.Size;
            }

            set
            {
                ArrowX.Size = value;
                ArrowY.Size = value;
                ArrowZ.Size = value;
            }
        }

        private void ResetArrowColors()
        {
            if (SelectedArrow == ArrowX)
            {
                ArrowX.ChangeColor(Color.IndianRed);
            }
            else
            {
                ArrowX.ChangeColor(Color.Red);
            }

            if (SelectedArrow == ArrowY)
            {
                ArrowY.ChangeColor(Color.NavajoWhite);
            }
            else
            {
                ArrowY.ChangeColor(Color.Yellow);
            }

            if (SelectedArrow == ArrowZ)
            {
                ArrowZ.ChangeColor(Color.CornflowerBlue);
            }
            else
            {
                ArrowZ.ChangeColor(Color.Blue);
            }
        }

        public bool ArrowXCollideWithMouse(int MouseX, int MouseY, Viewport ActiveViewport, Matrix View, Matrix Projection)
        {
            return ArrowX.CollideWithMouse(MouseX, MouseY, ActiveViewport, View, Projection, Matrix.Identity);
        }

        public bool ArrowYCollideWithMouse(int MouseX, int MouseY, Viewport ActiveViewport, Matrix View, Matrix Projection)
        {
            return ArrowY.CollideWithMouse(MouseX, MouseY, ActiveViewport, View, Projection, Matrix.Identity);
        }

        public bool ArrowZCollideWithMouse(int MouseX, int MouseY, Viewport ActiveViewport, Matrix View, Matrix Projection)
        {
            return ArrowZ.CollideWithMouse(MouseX, MouseY, ActiveViewport, View, Projection, Matrix.Identity);
        }

        public void SelectArrowX()
        {
            SelectedArrow = ArrowX;
            ResetArrowColors();
        }

        public void SelectArrowY()
        {
            SelectedArrow = ArrowY;
            ResetArrowColors();
        }

        public void SelectArrowZ()
        {
            SelectedArrow = ArrowZ;
            ResetArrowColors();
        }

        public void UnSelectArrow()
        {
            SelectedArrow = null;
            ResetArrowColors();
        }
    } 
}
