using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core.Graphics
{
    public class Ring3D : Object3DColored
    {
        Vector3[] ArrayFace;
        Vector3[] ArrayFaceQuarter2;
        Vector3[] ArrayFaceQuarter3;
        Vector3[] ArrayFaceQuarter4;

        public Ring3D(GraphicsDevice g, Matrix Projection, Color RingColor)
            : base(g, Projection, null)
        {
            ArrayFace = new Vector3[8];
            ArrayFaceQuarter2 = new Vector3[8];
            ArrayFaceQuarter3 = new Vector3[8];
            ArrayFaceQuarter4 = new Vector3[8];
            float InsideSize = 0.1f;
            
            //Top
            ArrayFace[0] = new Vector3(0.0f, 1.0f, 0.0f);
            ArrayFace[1] = new Vector3(InsideSize, 1.0f - InsideSize, 0.0f);
            ArrayFace[2] = new Vector3(0.0f, 1.0f - InsideSize * 2f, 0.0f);
            ArrayFace[3] = new Vector3(-InsideSize, 1.0f - InsideSize, 0.0f);

            //Bottom
            ArrayFace[4] = new Vector3(0.0f, 0.0f, 1.0f);
            ArrayFace[5] = new Vector3(InsideSize, 0.0f, 1.0f - InsideSize);
            ArrayFace[6] = new Vector3(0.0f, 0.0f, 1.0f - InsideSize * 2f);
            ArrayFace[7] = new Vector3(-InsideSize, 0.0f, 1.0f - InsideSize);

            Matrix RotX90 = Matrix.CreateRotationX(
                               (float)Math.PI / 2);

            Matrix RotX180 = Matrix.CreateRotationX(
                               (float)Math.PI);

            Matrix RotX270 = Matrix.CreateRotationX(
                               -(float)Math.PI / 2);

            for (int F = ArrayFace.Length - 1; F >= 0; --F)
            {
                ArrayFaceQuarter2[F] = Vector3.Transform(ArrayFace[F], RotX90);
                ArrayFaceQuarter3[F] = Vector3.Transform(ArrayFace[F], RotX180);
                ArrayFaceQuarter4[F] = Vector3.Transform(ArrayFace[F], RotX270);
            }

            ArrayVertex = new VertexPositionColor[96];

            ArrayVertex[0] = new VertexPositionColor(ArrayFace[0], RingColor);
            ArrayVertex[1] = new VertexPositionColor(ArrayFace[1], RingColor);
            ArrayVertex[2] = new VertexPositionColor(ArrayFace[4], RingColor);

            ArrayVertex[3] = new VertexPositionColor(ArrayFace[4], RingColor);
            ArrayVertex[4] = new VertexPositionColor(ArrayFace[1], RingColor);
            ArrayVertex[5] = new VertexPositionColor(ArrayFace[5], RingColor);

            ArrayVertex[6] = new VertexPositionColor(ArrayFace[1], RingColor);
            ArrayVertex[7] = new VertexPositionColor(ArrayFace[2], RingColor);
            ArrayVertex[8] = new VertexPositionColor(ArrayFace[5], RingColor);

            ArrayVertex[9] = new VertexPositionColor(ArrayFace[5], RingColor);
            ArrayVertex[10] = new VertexPositionColor(ArrayFace[2], RingColor);
            ArrayVertex[11] = new VertexPositionColor(ArrayFace[6], RingColor);

            ArrayVertex[12] = new VertexPositionColor(ArrayFace[2], RingColor);
            ArrayVertex[13] = new VertexPositionColor(ArrayFace[3], RingColor);
            ArrayVertex[14] = new VertexPositionColor(ArrayFace[6], RingColor);

            ArrayVertex[15] = new VertexPositionColor(ArrayFace[6], RingColor);
            ArrayVertex[16] = new VertexPositionColor(ArrayFace[3], RingColor);
            ArrayVertex[17] = new VertexPositionColor(ArrayFace[7], RingColor);

            ArrayVertex[18] = new VertexPositionColor(ArrayFace[3], RingColor);
            ArrayVertex[19] = new VertexPositionColor(ArrayFace[0], RingColor);
            ArrayVertex[20] = new VertexPositionColor(ArrayFace[7], RingColor);

            ArrayVertex[21] = new VertexPositionColor(ArrayFace[7], RingColor);
            ArrayVertex[22] = new VertexPositionColor(ArrayFace[0], RingColor);
            ArrayVertex[23] = new VertexPositionColor(ArrayFace[4], RingColor);

            //2nd Quarter
            ArrayVertex[24] = new VertexPositionColor(Vector3.Transform(ArrayVertex[0].Position, RotX90), RingColor);
            ArrayVertex[25] = new VertexPositionColor(Vector3.Transform(ArrayVertex[1].Position, RotX90), RingColor);
            ArrayVertex[26] = new VertexPositionColor(Vector3.Transform(ArrayVertex[2].Position, RotX90), RingColor);

            ArrayVertex[27] = new VertexPositionColor(Vector3.Transform(ArrayVertex[3].Position, RotX90), RingColor);
            ArrayVertex[28] = new VertexPositionColor(Vector3.Transform(ArrayVertex[4].Position, RotX90), RingColor);
            ArrayVertex[29] = new VertexPositionColor(Vector3.Transform(ArrayVertex[5].Position, RotX90), RingColor);

            ArrayVertex[30] = new VertexPositionColor(Vector3.Transform(ArrayVertex[6].Position, RotX90), RingColor);
            ArrayVertex[31] = new VertexPositionColor(Vector3.Transform(ArrayVertex[7].Position, RotX90), RingColor);
            ArrayVertex[32] = new VertexPositionColor(Vector3.Transform(ArrayVertex[8].Position, RotX90), RingColor);

            ArrayVertex[33] = new VertexPositionColor(Vector3.Transform(ArrayVertex[9].Position, RotX90), RingColor);
            ArrayVertex[34] = new VertexPositionColor(Vector3.Transform(ArrayVertex[10].Position, RotX90), RingColor);
            ArrayVertex[35] = new VertexPositionColor(Vector3.Transform(ArrayVertex[11].Position, RotX90), RingColor);

            ArrayVertex[36] = new VertexPositionColor(Vector3.Transform(ArrayVertex[12].Position, RotX90), RingColor);
            ArrayVertex[37] = new VertexPositionColor(Vector3.Transform(ArrayVertex[13].Position, RotX90), RingColor);
            ArrayVertex[38] = new VertexPositionColor(Vector3.Transform(ArrayVertex[14].Position, RotX90), RingColor);

            ArrayVertex[39] = new VertexPositionColor(Vector3.Transform(ArrayVertex[15].Position, RotX90), RingColor);
            ArrayVertex[40] = new VertexPositionColor(Vector3.Transform(ArrayVertex[16].Position, RotX90), RingColor);
            ArrayVertex[41] = new VertexPositionColor(Vector3.Transform(ArrayVertex[17].Position, RotX90), RingColor);

            ArrayVertex[42] = new VertexPositionColor(Vector3.Transform(ArrayVertex[18].Position, RotX90), RingColor);
            ArrayVertex[43] = new VertexPositionColor(Vector3.Transform(ArrayVertex[19].Position, RotX90), RingColor);
            ArrayVertex[44] = new VertexPositionColor(Vector3.Transform(ArrayVertex[20].Position, RotX90), RingColor);

            ArrayVertex[45] = new VertexPositionColor(Vector3.Transform(ArrayVertex[21].Position, RotX90), RingColor);
            ArrayVertex[46] = new VertexPositionColor(Vector3.Transform(ArrayVertex[22].Position, RotX90), RingColor);
            ArrayVertex[47] = new VertexPositionColor(Vector3.Transform(ArrayVertex[23].Position, RotX90), RingColor);

            //3rd Quarter
            ArrayVertex[48] = new VertexPositionColor(Vector3.Transform(ArrayVertex[0].Position, RotX180), RingColor);
            ArrayVertex[49] = new VertexPositionColor(Vector3.Transform(ArrayVertex[1].Position, RotX180), RingColor);
            ArrayVertex[50] = new VertexPositionColor(Vector3.Transform(ArrayVertex[2].Position, RotX180), RingColor);

            ArrayVertex[51] = new VertexPositionColor(Vector3.Transform(ArrayVertex[3].Position, RotX180), RingColor);
            ArrayVertex[52] = new VertexPositionColor(Vector3.Transform(ArrayVertex[4].Position, RotX180), RingColor);
            ArrayVertex[53] = new VertexPositionColor(Vector3.Transform(ArrayVertex[5].Position, RotX180), RingColor);

            ArrayVertex[54] = new VertexPositionColor(Vector3.Transform(ArrayVertex[6].Position, RotX180), RingColor);
            ArrayVertex[55] = new VertexPositionColor(Vector3.Transform(ArrayVertex[7].Position, RotX180), RingColor);
            ArrayVertex[56] = new VertexPositionColor(Vector3.Transform(ArrayVertex[8].Position, RotX180), RingColor);

            ArrayVertex[57] = new VertexPositionColor(Vector3.Transform(ArrayVertex[9].Position, RotX180), RingColor);
            ArrayVertex[58] = new VertexPositionColor(Vector3.Transform(ArrayVertex[10].Position, RotX180), RingColor);
            ArrayVertex[59] = new VertexPositionColor(Vector3.Transform(ArrayVertex[11].Position, RotX180), RingColor);

            ArrayVertex[60] = new VertexPositionColor(Vector3.Transform(ArrayVertex[12].Position, RotX180), RingColor);
            ArrayVertex[61] = new VertexPositionColor(Vector3.Transform(ArrayVertex[13].Position, RotX180), RingColor);
            ArrayVertex[62] = new VertexPositionColor(Vector3.Transform(ArrayVertex[14].Position, RotX180), RingColor);

            ArrayVertex[63] = new VertexPositionColor(Vector3.Transform(ArrayVertex[15].Position, RotX180), RingColor);
            ArrayVertex[64] = new VertexPositionColor(Vector3.Transform(ArrayVertex[16].Position, RotX180), RingColor);
            ArrayVertex[65] = new VertexPositionColor(Vector3.Transform(ArrayVertex[17].Position, RotX180), RingColor);

            ArrayVertex[66] = new VertexPositionColor(Vector3.Transform(ArrayVertex[18].Position, RotX180), RingColor);
            ArrayVertex[67] = new VertexPositionColor(Vector3.Transform(ArrayVertex[19].Position, RotX180), RingColor);
            ArrayVertex[68] = new VertexPositionColor(Vector3.Transform(ArrayVertex[20].Position, RotX180), RingColor);

            ArrayVertex[69] = new VertexPositionColor(Vector3.Transform(ArrayVertex[21].Position, RotX180), RingColor);
            ArrayVertex[70] = new VertexPositionColor(Vector3.Transform(ArrayVertex[22].Position, RotX180), RingColor);
            ArrayVertex[71] = new VertexPositionColor(Vector3.Transform(ArrayVertex[23].Position, RotX180), RingColor);

            //4thd Quarter
            ArrayVertex[72] = new VertexPositionColor(Vector3.Transform(ArrayVertex[0].Position, RotX270), RingColor);
            ArrayVertex[73] = new VertexPositionColor(Vector3.Transform(ArrayVertex[1].Position, RotX270), RingColor);
            ArrayVertex[74] = new VertexPositionColor(Vector3.Transform(ArrayVertex[2].Position, RotX270), RingColor);

            ArrayVertex[75] = new VertexPositionColor(Vector3.Transform(ArrayVertex[3].Position, RotX270), RingColor);
            ArrayVertex[76] = new VertexPositionColor(Vector3.Transform(ArrayVertex[4].Position, RotX270), RingColor);
            ArrayVertex[77] = new VertexPositionColor(Vector3.Transform(ArrayVertex[5].Position, RotX270), RingColor);

            ArrayVertex[78] = new VertexPositionColor(Vector3.Transform(ArrayVertex[6].Position, RotX270), RingColor);
            ArrayVertex[79] = new VertexPositionColor(Vector3.Transform(ArrayVertex[7].Position, RotX270), RingColor);
            ArrayVertex[80] = new VertexPositionColor(Vector3.Transform(ArrayVertex[8].Position, RotX270), RingColor);

            ArrayVertex[81] = new VertexPositionColor(Vector3.Transform(ArrayVertex[9].Position, RotX270), RingColor);
            ArrayVertex[82] = new VertexPositionColor(Vector3.Transform(ArrayVertex[10].Position, RotX270), RingColor);
            ArrayVertex[83] = new VertexPositionColor(Vector3.Transform(ArrayVertex[11].Position, RotX270), RingColor);

            ArrayVertex[84] = new VertexPositionColor(Vector3.Transform(ArrayVertex[12].Position, RotX270), RingColor);
            ArrayVertex[85] = new VertexPositionColor(Vector3.Transform(ArrayVertex[13].Position, RotX270), RingColor);
            ArrayVertex[86] = new VertexPositionColor(Vector3.Transform(ArrayVertex[14].Position, RotX270), RingColor);

            ArrayVertex[87] = new VertexPositionColor(Vector3.Transform(ArrayVertex[15].Position, RotX270), RingColor);
            ArrayVertex[88] = new VertexPositionColor(Vector3.Transform(ArrayVertex[16].Position, RotX270), RingColor);
            ArrayVertex[89] = new VertexPositionColor(Vector3.Transform(ArrayVertex[17].Position, RotX270), RingColor);

            ArrayVertex[90] = new VertexPositionColor(Vector3.Transform(ArrayVertex[18].Position, RotX270), RingColor);
            ArrayVertex[91] = new VertexPositionColor(Vector3.Transform(ArrayVertex[19].Position, RotX270), RingColor);
            ArrayVertex[92] = new VertexPositionColor(Vector3.Transform(ArrayVertex[20].Position, RotX270), RingColor);

            ArrayVertex[93] = new VertexPositionColor(Vector3.Transform(ArrayVertex[21].Position, RotX270), RingColor);
            ArrayVertex[94] = new VertexPositionColor(Vector3.Transform(ArrayVertex[22].Position, RotX270), RingColor);
            ArrayVertex[95] = new VertexPositionColor(Vector3.Transform(ArrayVertex[23].Position, RotX270), RingColor);
        }

        public void ChangeColor(Color RingColor)
        {
            for (int i = 0; i < ArrayVertex.Length; i++)
            {
                ArrayVertex[i] = new VertexPositionColor(ArrayVertex[i].Position, RingColor);
            }
        }

        public bool CollideRingWithMouse(int MouseX, int MouseY, Viewport ActiveViewport, Matrix View, Matrix Projection, Matrix World)
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

            PolygonMesh MouseCollisionBox = CreateMouseObject(MouseX, MouseY, ActiveViewport, View, Projection, World);

            var Axis1 = ComputePerpendicularAxis(0, 24);
            var Axis2 = ComputePerpendicularAxis(24, 24);
            var Axis3 = ComputePerpendicularAxis(48, 24);
            var Axis4 = ComputePerpendicularAxis(72, 24);

            var Vertex1 = new Vector3[24];
            var Vertex2 = new Vector3[24];
            var Vertex3 = new Vector3[24];
            var Vertex4 = new Vector3[24];
            for (int V = 0; V < 24; V++)
            {
                Vertex1[V] = ArrayVertex[V].Position;
                Vertex2[V] = ArrayVertex[V + 24].Position;
                Vertex3[V] = ArrayVertex[V + 48].Position;
                Vertex4[V] = ArrayVertex[V + 72].Position;
            }
            
            var col1 = CollideWithQuarterRing(MouseCollisionBox, Vertex1, Axis1);
            var col2 = CollideWithQuarterRing(MouseCollisionBox, Vertex2, Axis2);
            var col3 = CollideWithQuarterRing(MouseCollisionBox, Vertex3, Axis3);
            var col4 = CollideWithQuarterRing(MouseCollisionBox, Vertex4, Axis4);

            return col1 || col2 || col3 || col4;
        }
        
        private bool CollideWithQuarterRing(PolygonMesh MouseCollision, Vector3[] CollisionVertex, Vector3[] ArrayAxis)
        {
            PolygonMesh QuarterRightCollisionBox = new PolygonMesh(CollisionVertex, ArrayAxis);

            return PolygonMesh.PolygonCollisionSAT(MouseCollision, QuarterRightCollisionBox, Vector3.Zero).Collided;
        }

        private Vector3[] ComputePerpendicularAxis(int Offset, int Max)
        {
            if (ArrayVertex.Length < 3)
            {
                return null;
            }
            else
            {
                Vector3 Vertex1;
                Vector3 Vertex2;
                Vector3 Vertex3;

                Vector3[] ArrayAxis = new Vector3[Max / 3];
                Vector3[] TransformedVertex = new Vector3[Max];
                for (int V = 0; V < TransformedVertex.Length; V++)
                {
                    TransformedVertex[V] = ArrayVertex[Offset + V].Position;
                }

                for (int V = 0; V < ArrayAxis.Length; V += 1)
                {
                    Vertex1 = TransformedVertex[V * 3];

                    if (V * 3 + 1 >= Max)
                        Vertex2 = TransformedVertex[0];
                    else
                        Vertex2 = TransformedVertex[V * 3 + 1];

                    if (V * 3 + 2 >= Max)
                        Vertex3 = TransformedVertex[V * 3 + 2 - Max];
                    else
                        Vertex3 = TransformedVertex[V * 3 + 2];

                    var side1 = Vertex2 - Vertex1;
                    var side2 = Vertex3 - Vertex1;

                    ArrayAxis[V] = Vector3.Cross(side1, side2);
                    ArrayAxis[V].Normalize();
                }

                return ArrayAxis;
            }
        }

        private Vector3[] GetTransformedVertex(int Offset, int Max)
        {
            Vector3[] TransformedVertex = new Vector3[Max];
            for (int V = 0; V < TransformedVertex.Length; V++)
            {
                TransformedVertex[V] = Vector3.Transform(ArrayVertex[Offset + V].Position, ObjectEffect.World);
            }

            return TransformedVertex;
        }

        public void SuperTransform()
        {
            Vector3[] TransformedVertex = GetTransformedVertex(0, ArrayVertex.Length);
            for (int V = 0; V < ArrayVertex.Length; V++)
            {
                ArrayVertex[V].Position = TransformedVertex[V];
            }

            Scale(Vector3.One);
            Rotate(0f, 0f, 0f);
            Position = Vector3.Zero;
        }

        private PolygonMesh CreateMouseObject(int MouseX, int MouseY, Viewport ActiveViewport, Matrix View, Matrix Projection, Matrix World)
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

            Vector3[] MouseObject = new Vector3[24];

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

            MouseObject[0] = ArrayFace[0];
            MouseObject[1] = ArrayFace[1];
            MouseObject[2] = ArrayFace[4];

            MouseObject[3] = ArrayFace[4];
            MouseObject[4] = ArrayFace[1];
            MouseObject[5] = ArrayFace[5];

            MouseObject[6] = ArrayFace[1];
            MouseObject[7] = ArrayFace[2];
            MouseObject[8] = ArrayFace[5];

            MouseObject[9] = ArrayFace[5];
            MouseObject[10] = ArrayFace[2];
            MouseObject[11] = ArrayFace[6];

            MouseObject[12] = ArrayFace[2];
            MouseObject[13] = ArrayFace[3];
            MouseObject[14] = ArrayFace[6];

            MouseObject[15] = ArrayFace[6];
            MouseObject[16] = ArrayFace[3];
            MouseObject[17] = ArrayFace[7];

            MouseObject[18] = ArrayFace[3];
            MouseObject[19] = ArrayFace[0];
            MouseObject[20] = ArrayFace[7];

            MouseObject[21] = ArrayFace[7];
            MouseObject[22] = ArrayFace[0];
            MouseObject[23] = ArrayFace[4];

            return new PolygonMesh(MouseObject, new Vector3[3] { Right, Up, Forward});
        }
    }
}
