using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ProjectEternity.Core.Item
{
    public class CrossRing3D : IObject3D
    {
        public Ring3D RingX;
        public Ring3D RingY;
        public Ring3D RingZ;

        private Ring3D SelectedRing;

        private VertexPositionColor[] MouseObject;

        public CrossRing3D(GraphicsDevice g, Matrix Projection)
        {
            RingX = new Ring3D(g, Projection, Color.Blue);
            RingX.Scale(new Vector3(10f, 10f, 10f));
            RingX.SuperTransform();

            RingY = new Ring3D(g, Projection, Color.Yellow);
            RingY.Scale(new Vector3(10f, 10f, 10f));
            RingY.Rotate(0f, 0f, MathHelper.ToRadians(90f));
            RingY.SuperTransform();

            RingZ = new Ring3D(g, Projection, Color.Red);
            RingZ.Scale(new Vector3(10f, 10f, 10f));
            RingZ.Rotate(MathHelper.ToRadians(90f), MathHelper.ToRadians(90f), 0f);
            RingZ.SuperTransform();
        }

        public void Rotate(float TotalYaw, float TotalPitch, float TotalRoll)
        {
            RingX.Rotate(TotalYaw, TotalPitch, TotalRoll);
            RingY.Rotate(TotalYaw, TotalPitch, TotalRoll);
            RingZ.Rotate(TotalYaw, TotalPitch, TotalRoll);
        }

        public void Scale(Vector3 TotalScale)
        {
            RingX.Scale(TotalScale);
            RingY.Scale(TotalScale);
            RingZ.Scale(TotalScale);
        }

        public void Teleport(Vector3 Destination)
        {
            RingX.Teleport(Destination);
            RingY.Teleport(Destination);
            RingZ.Teleport(Destination);
        }

        private void CreateMouseObject(int MouseX, int MouseY, Viewport ActiveViewport, Matrix View, Matrix Projection, Matrix World)
        {
            Vector3 NearSource = new Vector3(MouseX, MouseY, 0f);
            Vector3 FarSource = new Vector3(MouseX, MouseY, 1f);

            Vector3 NearPoint = ActiveViewport.Unproject(NearSource, Projection, View, World);
            Vector3 FarPoint = ActiveViewport.Unproject(FarSource, Projection, View, World);

            // Create a ray from the near clip plane to the far clip plane.
            Vector3 RayDirection = FarPoint - NearPoint;
            RayDirection.Normalize();

            Matrix RotationMatrix = Matrix.Identity;
            RotationMatrix.Forward = RayDirection;
            RotationMatrix.Right = Vector3.Normalize(Vector3.Cross(RotationMatrix.Forward, Vector3.Up));
            RotationMatrix.Up = Vector3.Cross(RotationMatrix.Right, RotationMatrix.Forward);
            Vector3 Forward = Vector3.Transform(Vector3.Forward, RotationMatrix);
            Vector3 Right = Vector3.Transform(Vector3.Right, RotationMatrix);
            Vector3 Up = Vector3.Transform(Vector3.Up, RotationMatrix);
            
            MouseObject = new VertexPositionColor[24];

            Vector3[] ArrayFace = new Vector3[8];

            //FrontTopLeft
            ArrayFace[0] = new Vector3(FarPoint.X, FarPoint.Y, FarPoint.Z) + Right * 0.1f + Up * 0.1f;
            //FrontBottomLeft
            ArrayFace[1] = new Vector3(FarPoint.X, FarPoint.Y, FarPoint.Z) - Right * 0.1f + Up * 0.1f;
            //FrontTopRight
            ArrayFace[2] = new Vector3(FarPoint.X, FarPoint.Y, FarPoint.Z) + Right * 0.1f - Up * 0.1f;
            //FrontBottomRight
            ArrayFace[3] = new Vector3(FarPoint.X, FarPoint.Y, FarPoint.Z) + Right * 0.1f - Up * 0.1f;

            //BottomTopLeft
            ArrayFace[4] = new Vector3(NearPoint.X, NearPoint.Y, NearPoint.Z) + Right * 0.1f + Up * 0.1f;
            //BottomBottomLeft
            ArrayFace[5] = new Vector3(NearPoint.X, NearPoint.Y, NearPoint.Z) - Right * 0.1f + Up * 0.1f;
            //BottomTopRight
            ArrayFace[6] = new Vector3(NearPoint.X, NearPoint.Y, NearPoint.Z) + Right * 0.1f - Up * 0.1f;
            //BottomBottomRight
            ArrayFace[7] = new Vector3(NearPoint.X, NearPoint.Y, NearPoint.Z) + Right * 0.1f - Up * 0.1f;

            MouseObject[0] = new VertexPositionColor(ArrayFace[0], Color.Green);
            MouseObject[1] = new VertexPositionColor(ArrayFace[1], Color.Green);
            MouseObject[2] = new VertexPositionColor(ArrayFace[4], Color.Green);

            MouseObject[3] = new VertexPositionColor(ArrayFace[4], Color.Green);
            MouseObject[4] = new VertexPositionColor(ArrayFace[1], Color.Green);
            MouseObject[5] = new VertexPositionColor(ArrayFace[5], Color.Green);

            MouseObject[6] = new VertexPositionColor(ArrayFace[1], Color.Green);
            MouseObject[7] = new VertexPositionColor(ArrayFace[2], Color.Green);
            MouseObject[8] = new VertexPositionColor(ArrayFace[5], Color.Green);

            MouseObject[9] = new VertexPositionColor(ArrayFace[5], Color.Green);
            MouseObject[10] = new VertexPositionColor(ArrayFace[2], Color.Green);
            MouseObject[11] = new VertexPositionColor(ArrayFace[6], Color.Green);

            MouseObject[12] = new VertexPositionColor(ArrayFace[2], Color.Green);
            MouseObject[13] = new VertexPositionColor(ArrayFace[3], Color.Green);
            MouseObject[14] = new VertexPositionColor(ArrayFace[6], Color.Green);

            MouseObject[15] = new VertexPositionColor(ArrayFace[6], Color.Green);
            MouseObject[16] = new VertexPositionColor(ArrayFace[3], Color.Green);
            MouseObject[17] = new VertexPositionColor(ArrayFace[7], Color.Green);

            MouseObject[18] = new VertexPositionColor(ArrayFace[3], Color.Green);
            MouseObject[19] = new VertexPositionColor(ArrayFace[0], Color.Green);
            MouseObject[20] = new VertexPositionColor(ArrayFace[7], Color.Green);

            MouseObject[21] = new VertexPositionColor(ArrayFace[7], Color.Green);
            MouseObject[22] = new VertexPositionColor(ArrayFace[0], Color.Green);
            MouseObject[23] = new VertexPositionColor(ArrayFace[4], Color.Green);

        }

        public void Draw(CustomSpriteBatch g, Matrix View)
        {
            RingX.Draw(g, View);
            RingY.Draw(g, View);
            RingZ.Draw(g, View);
            if (MouseObject != null)
            {
                g.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, MouseObject, 0, MouseObject.Length / 3);
            }
        }

        public Vector3 Position
        {
            get
            {
                return RingX.Position;
            }

            set
            {
                RingX.Position = value;
                RingY.Position = value;
                RingZ.Position = value;
            }
        }

        public Vector3 Direction
        {
            get
            {
                return RingX.Direction;
            }

            set
            {
                RingX.Direction = value;
                RingY.Direction = value;
                RingZ.Direction = value;
            }
        }

        public Vector3 Size
        {
            get
            {
                return RingX.Size;
            }

            set
            {
                RingX.Size = value;
                RingY.Size = value;
                RingZ.Size = value;
            }
        }

        private void ResetRingsColors()
        {
            if (SelectedRing == RingX)
            {
                RingX.ChangeColor(Color.IndianRed);
            }
            else
            {
                RingX.ChangeColor(Color.Red);
            }

            if (SelectedRing == RingY)
            {
                RingY.ChangeColor(Color.NavajoWhite);
            }
            else
            {
                RingY.ChangeColor(Color.Yellow);
            }

            if (SelectedRing == RingZ)
            {
                RingZ.ChangeColor(Color.CornflowerBlue);
            }
            else
            {
                RingZ.ChangeColor(Color.Blue);
            }
        }

        public bool RingXCollideWithMouse(int MouseX, int MouseY, Viewport ActiveViewport, Matrix View, Matrix Projection)
        {
            CreateMouseObject(MouseX, MouseY, ActiveViewport, View, Projection, Matrix.Identity);
            return RingX.CollideRingWithMouse(MouseX, MouseY, ActiveViewport, View, Projection, Matrix.Identity);
        }

        public bool RingYCollideWithMouse(int MouseX, int MouseY, Viewport ActiveViewport, Matrix View, Matrix Projection)
        {
            return RingY.CollideRingWithMouse(MouseX, MouseY, ActiveViewport, View, Projection, Matrix.Identity);
        }

        public bool RingZCollideWithMouse(int MouseX, int MouseY, Viewport ActiveViewport, Matrix View, Matrix Projection)
        {
            return RingZ.CollideRingWithMouse(MouseX, MouseY, ActiveViewport, View, Projection, Matrix.Identity);
        }

        public void SelectRingX()
        {
            SelectedRing = RingX;
            ResetRingsColors();
        }

        public void SelectRingY()
        {
            SelectedRing = RingY;
            ResetRingsColors();
        }

        public void SelectRingZ()
        {
            SelectedRing = RingZ;
            ResetRingsColors();
        }

        public void UnSelectRing()
        {
            SelectedRing = null;
            ResetRingsColors();
        }
    } 
}
