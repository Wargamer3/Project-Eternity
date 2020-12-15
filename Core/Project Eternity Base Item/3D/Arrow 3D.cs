using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core.Item
{
    public class Arrow3D : Object3DColored
    {
        public Arrow3D(GraphicsDevice g, Matrix Projection, Color ArrowColor)
            : base(g, Projection, null)
        {
            Vector3[] ArrayFace = new Vector3[4];

            //FrontTopLeft
            ArrayFace[0] = new Vector3(0.5f, 0.5f, 0.0f);
            //FrontBottomLeft
            ArrayFace[1] = new Vector3(-0.5f, -0.5f, 0.0f);
            //FrontTopRight
            ArrayFace[2] = new Vector3(-0.5f, 0.5f, 0.0f);
            //FrontBottomRight
            ArrayFace[3] = new Vector3(-0.5f, -0.5f, 0.0f);

            Matrix RotZ180 = Matrix.CreateRotationZ(
                               -(float)Math.PI);

            ArrayVertex = new VertexPositionColor[42];

            #region Arrow Body

            //front face
            for (int i = 0; i <= 2; i++)
            {
                ArrayVertex[i] = new VertexPositionColor(ArrayFace[i] + Vector3.UnitZ * 0.5f, ArrowColor);

                ArrayVertex[i + 3] = new VertexPositionColor(Vector3.Transform(ArrayFace[i], RotZ180) + Vector3.UnitZ * 0.5f, ArrowColor);
            }

            //left face
            Matrix RotY90 = Matrix.CreateRotationY(
                               -(float)Math.PI / 2f);

            Matrix RotY270 = Matrix.CreateRotationY(
                               (float)Math.PI / 2f);

            for (int i = 0; i <= 2; i++)
            {
                ArrayVertex[i + 6] = new VertexPositionColor(Vector3.Transform(ArrayVertex[i].Position, RotY90), ArrowColor);

                ArrayVertex[i + 6 + 3] = new VertexPositionColor(Vector3.Transform(ArrayVertex[i + 3].Position, RotY90), ArrowColor);
            }

            //Right face
            for (int i = 0; i <= 2; i++)
            {
                ArrayVertex[i + 12] = new VertexPositionColor(Vector3.Transform(ArrayVertex[i].Position, RotY270), ArrowColor);

                ArrayVertex[i + 12 + 3] = new VertexPositionColor(Vector3.Transform(ArrayVertex[i + 3].Position, RotY270), ArrowColor);
            }

            //Top face
            Matrix RotX90 = Matrix.CreateRotationX(
                                -(float)Math.PI / 2f);
            Matrix RotX270 = Matrix.CreateRotationX(
                                (float)Math.PI / 2f);

            for (int i = 0; i <= 2; i++)
            {
                ArrayVertex[i + 18] = new VertexPositionColor(Vector3.Transform(ArrayVertex[i].Position, RotX90), ArrowColor);

                ArrayVertex[i + 18 + 3] = new VertexPositionColor(Vector3.Transform(ArrayVertex[i + 3].Position, RotX90), ArrowColor);
            }

            //Bottom face
            for (int i = 0; i <= 2; i++)
            {
                ArrayVertex[i + 24] = new VertexPositionColor(Vector3.Transform(ArrayVertex[i].Position, RotX270), ArrowColor);

                ArrayVertex[i + 24 + 3] = new VertexPositionColor(Vector3.Transform(ArrayVertex[i + 3].Position, RotX270), ArrowColor);
            }

            for (int i = 0; i < 30; i++)
            {
                ArrayVertex[i] = new VertexPositionColor(ArrayVertex[i].Position + Vector3.UnitZ * 0.5f, ArrowColor);
            }

            #endregion

            #region Arrow Head

            Matrix RotZ90 = Matrix.CreateRotationZ(
                                -(float)Math.PI / 2f);
            Matrix RotZ270 = Matrix.CreateRotationZ(
                                (float)Math.PI / 2f);

            ArrayVertex[30] = new VertexPositionColor(new Vector3(0f, 0f, -1f), ArrowColor);
            ArrayVertex[31] = new VertexPositionColor(new Vector3(-1f, 0f, 0f), ArrowColor);
            ArrayVertex[32] = new VertexPositionColor(new Vector3(0f, -1f, 0f), ArrowColor);

            ArrayVertex[33] = new VertexPositionColor(Vector3.Transform(ArrayVertex[30].Position, RotZ90), ArrowColor);
            ArrayVertex[34] = new VertexPositionColor(Vector3.Transform(ArrayVertex[31].Position, RotZ90), ArrowColor);
            ArrayVertex[35] = new VertexPositionColor(Vector3.Transform(ArrayVertex[32].Position, RotZ90), ArrowColor);

            ArrayVertex[36] = new VertexPositionColor(Vector3.Transform(ArrayVertex[30].Position, RotZ180), ArrowColor);
            ArrayVertex[37] = new VertexPositionColor(Vector3.Transform(ArrayVertex[31].Position, RotZ180), ArrowColor);
            ArrayVertex[38] = new VertexPositionColor(Vector3.Transform(ArrayVertex[32].Position, RotZ180), ArrowColor);

            ArrayVertex[39] = new VertexPositionColor(Vector3.Transform(ArrayVertex[30].Position, RotZ270), ArrowColor);
            ArrayVertex[40] = new VertexPositionColor(Vector3.Transform(ArrayVertex[31].Position, RotZ270), ArrowColor);
            ArrayVertex[41] = new VertexPositionColor(Vector3.Transform(ArrayVertex[32].Position, RotZ270), ArrowColor);

            #endregion
        }

        public void ChangeColor(Color ArrowColor)
        {
            for (int i = 0; i < ArrayVertex.Length; i++)
            {
                ArrayVertex[i] = new VertexPositionColor(ArrayVertex[i].Position, ArrowColor);
            }
        }
    }
}
