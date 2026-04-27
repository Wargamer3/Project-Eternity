using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core.Graphics
{
    public class AITunnel : Object3D
    {
        public List<AITunnel> ListNextAITunnel;
        private VertexPositionNormalTexture[] ArrayVertexBound;
        private Lines3D VisibleDirection;

        public AITunnel()
        {
        }

        public AITunnel(GraphicsDevice g, Matrix Projection)
            : base(g, Projection)
        {
            Init(g, Projection);
        }

        public AITunnel(BinaryReader BR, GraphicsDevice g, Matrix Projection)
            : base(BR, g, Projection)
        {
            Init(g, Projection);
        }

        public void Select()
        {
            ObjectEffect.DirectionalLight0.DiffuseColor = new Vector3(0.2f, 0.1f, 0.8f);
        }

        private void Init(GraphicsDevice g, Matrix Projection)
        {
            ListNextAITunnel = new List<AITunnel>();
            
            ObjectEffect.AmbientLightColor = Vector3.Zero;
            ObjectEffect.DirectionalLight0.Enabled = true;
            ObjectEffect.DirectionalLight0.DiffuseColor = Vector3.One;
            ObjectEffect.DirectionalLight0.Direction = Vector3.Normalize(Vector3.One);
            ObjectEffect.LightingEnabled = true;

            ArrayVertexBound = CreateTunnel();
            CreateVisibleDirection(g, Projection);
        }

        public static VertexPositionNormalTexture[] CreateTunnel()
        {
            Vector2 Texcoords = new Vector2(0f, 0f);
            Vector3[] face = new Vector3[4];

            //FrontTopLeft
            face[0] = new Vector3(1f, 1f, 0.0f);
            //FrontBottomLeft
            face[1] = new Vector3(-1f, -1f, 0.0f);
            //FrontTopRight
            face[2] = new Vector3(-1f, 1f, 0.0f);
            //FrontBottomRight
            face[3] = new Vector3(-1f, -1f, 0.0f);

            Matrix RotZ180 = Matrix.CreateRotationZ(
                               -(float)Math.PI);

            VertexPositionNormalTexture[] vertexes = new VertexPositionNormalTexture[24];
            VertexPositionNormalTexture[] FrontFace = new VertexPositionNormalTexture[6];

            //front face
            for (int i = 0; i <= 2; i++)
            {
                FrontFace[i] =
                  new VertexPositionNormalTexture(
                       face[i] + Vector3.UnitZ,
                             Vector3.UnitZ, Texcoords);

                FrontFace[i + 3] =
                  new VertexPositionNormalTexture(
                       Vector3.Transform(face[i], RotZ180) + Vector3.UnitZ,
                             Vector3.UnitZ, Texcoords);
            }

            //left face
            Matrix RotY90 = Matrix.CreateRotationY(
                               -(float)Math.PI / 2f);

            Matrix RotY270 = Matrix.CreateRotationY(
                               (float)Math.PI / 2f);

            for (int i = 0; i <= 2; i++)
            {
                vertexes[i + 0] =
                   new VertexPositionNormalTexture(
                        Vector3.Transform(FrontFace[i].Position, RotY90),
                            -Vector3.UnitX, Texcoords);

                vertexes[i + 0 + 3] =
                   new VertexPositionNormalTexture(
                        Vector3.Transform(FrontFace[i + 3].Position, RotY90),
                            -Vector3.UnitX, Texcoords);
            }

            //Right face
            for (int i = 0; i <= 2; i++)
            {
                vertexes[i + 6] =
                   new VertexPositionNormalTexture(
                        Vector3.Transform(FrontFace[i].Position, RotY270),
                            Vector3.UnitX, Texcoords);

                vertexes[i + 6 + 3] =
                   new VertexPositionNormalTexture(
                        Vector3.Transform(FrontFace[i + 3].Position, RotY270),
                            Vector3.UnitX, Texcoords);
            }

            //Top face
            Matrix RotX90 = Matrix.CreateRotationX(
                                -(float)Math.PI / 2f);
            Matrix RotX270 = Matrix.CreateRotationX(
                                (float)Math.PI / 2f);

            for (int i = 0; i <= 2; i++)
            {
                vertexes[i + 12] =
                   new VertexPositionNormalTexture(
                        Vector3.Transform(FrontFace[i].Position, RotX90),
                            -Vector3.UnitY, Texcoords);

                vertexes[i + 12 + 3] =
                   new VertexPositionNormalTexture(
                        Vector3.Transform(FrontFace[i + 3].Position, RotX90),
                            -Vector3.UnitY, Texcoords);
            }

            //Bottom face
            for (int i = 0; i <= 2; i++)
            {
                vertexes[i + 18] =
                   new VertexPositionNormalTexture(
                        Vector3.Transform(FrontFace[i].Position, RotX270),
                            Vector3.UnitY, Texcoords);

                vertexes[i + 18 + 3] =
                   new VertexPositionNormalTexture(
                        Vector3.Transform(FrontFace[i + 3].Position, RotX270),
                            Vector3.UnitY, Texcoords);
            }

            return vertexes;
        }

        private void CreateVisibleDirection(GraphicsDevice g, Matrix Projection)
        {
            VertexPositionColor[] ArrayVertex = new VertexPositionColor[14];
            // Initialize an array of indices of type short.
            short[] ArrayIndices = new short[(ArrayVertex.Length * 2) - 2];
            
            int Index = 0;
            ArrayVertex[Index * 2] = new VertexPositionColor(
                new Vector3(0.5f, 0, 1), Color.White);
            ArrayVertex[Index * 2 + 1] = new VertexPositionColor(
                new Vector3(0.5f, 0, -0.5f), Color.White);

            ArrayIndices[Index * 2] = (short)(Index * 2);
            ArrayIndices[Index * 2 + 1] = (short)(Index * 2 + 1);

            ++Index;

            ArrayVertex[Index * 2] = new VertexPositionColor(
                new Vector3(0.5f, 0, -0.5f), Color.White);
            ArrayVertex[Index * 2 + 1] = new VertexPositionColor(
                new Vector3(1f, 0, -0.5f), Color.White);

            ArrayIndices[Index * 2] = (short)(Index * 2);
            ArrayIndices[Index * 2 + 1] = (short)(Index * 2 + 1);

            ++Index;

            //Top of arrow
            ArrayVertex[Index * 2] = new VertexPositionColor(
                new Vector3(1f, 0, -0.5f), Color.White);
            ArrayVertex[Index * 2 + 1] = new VertexPositionColor(
                new Vector3(0, 0, -1f), Color.White);

            ArrayIndices[Index * 2] = (short)(Index * 2);
            ArrayIndices[Index * 2 + 1] = (short)(Index * 2 + 1);

            ++Index;

            ArrayVertex[Index * 2] = new VertexPositionColor(
                new Vector3(0, 0, -1f), Color.White);
            ArrayVertex[Index * 2 + 1] = new VertexPositionColor(
                new Vector3(-1f, 0, -0.5f), Color.White);

            ArrayIndices[Index * 2] = (short)(Index * 2);
            ArrayIndices[Index * 2 + 1] = (short)(Index * 2 + 1);

            ++Index;

            ArrayVertex[Index * 2] = new VertexPositionColor(
                new Vector3(-1f, 0, -0.5f), Color.White);
            ArrayVertex[Index * 2 + 1] = new VertexPositionColor(
                new Vector3(-0.5f, 0, -0.5f), Color.White);

            ArrayIndices[Index * 2] = (short)(Index * 2);
            ArrayIndices[Index * 2 + 1] = (short)(Index * 2 + 1);

            ++Index;

            ArrayVertex[Index * 2] = new VertexPositionColor(
                new Vector3(-0.5f, 0, -0.5f), Color.White);
            ArrayVertex[Index * 2 + 1] = new VertexPositionColor(
                new Vector3(-0.5f, 0, 1), Color.White);

            ArrayIndices[Index * 2] = (short)(Index * 2);
            ArrayIndices[Index * 2 + 1] = (short)(Index * 2 + 1);

            ++Index;

            ArrayVertex[Index * 2] = new VertexPositionColor(
                new Vector3(-0.5f, 0, 1), Color.White);
            ArrayVertex[Index * 2 + 1] = new VertexPositionColor(
                new Vector3(0.5f, 0, 1), Color.White);

            ArrayIndices[Index * 2] = (short)(Index * 2);
            ArrayIndices[Index * 2 + 1] = (short)(Index * 2 + 1);

            ++Index;

            VisibleDirection = new Lines3D(g, Projection, ArrayVertex, ArrayIndices);
        }

        public static AITunnel Load(BinaryReader BR, GraphicsDevice g, Matrix Projection)
        {
            AITunnel NewAITunnel = new AITunnel(BR, g, Projection);

            return NewAITunnel;
        }
        
        public void GetEntryPoints(Vector3 Axis, out float Min, out float Max)
        {
            Min = Vector3.Dot(Axis, CollisionBox.ArrayVertex[1]);
            Max = Min;
            float DotProduct = Vector3.Dot(Axis, CollisionBox.ArrayVertex[3]);
            if (DotProduct < Min)
            {
                Min = DotProduct;
            }
            else if (DotProduct > Max)
            {
                Max = DotProduct;
            }
        }

        public void GetExitPoints(Vector3 Axis, out float Min, out float Max)
        {
            Min = Vector3.Dot(Axis, CollisionBox.ArrayVertex[0]);
            Max = Min;
            float DotProduct = Vector3.Dot(Axis, CollisionBox.ArrayVertex[2]);
            if (DotProduct < Min)
            {
                Min = DotProduct;
            }
            else if (DotProduct > Max)
            {
                Max = DotProduct;
            }
        }

        public float GetDistanceEntryPoint()
        {
            return Vector3.Dot(Forward, CollisionBox.ArrayVertex[1]);
        }

        public float GetDistanceExitPoint()
        {
            return Vector3.Dot(Forward, CollisionBox.ArrayVertex[2]);
        }

        public override void Draw(CustomSpriteBatch g, Matrix View)
        {
            ObjectEffect.View = View;
            
            VisibleDirection.LinesEffect.World = ObjectEffect.World;

            ObjectEffect.CurrentTechnique.Passes[0].Apply();
            g.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, ArrayVertexBound, 0, 8);

            VisibleDirection.Draw(g, View);
        }
    }
}
