using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core.Attacks
{
    public class Attack3D
    {
        public struct Attack3DVertex
        {
            public Vector3 Position;
            public Vector2 UV;
            public float Time;

            public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(sizeof(float) * 3 + sizeof(float) * 2, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 1)
             );

            public const int SizeInBytes = sizeof(float) * 3 + sizeof(float) * 2 + sizeof(float);
        }

        public Attack3DVertex[] ArrayVertex;
        private DynamicVertexBuffer AttackVertexBuffer;
        private IndexBuffer AttackIndexBuffer;
        private Effect AttackEffect3D;
        private EffectParameterCollection EffectParameters;

        private EffectParameter effectViewParameter;
        private EffectParameter effectProjectionParameter;
        private EffectParameter EffectViewportScaleParameter;
        private EffectParameter EffectTimeParameter;

        public Attack3D(GraphicsDevice g, Effect AttackEffect3D, Texture2D Sprite, int NumberOfImages, float currentTime = 0f)
        {
            this.AttackEffect3D = AttackEffect3D.Clone();

            EffectParameters = this.AttackEffect3D.Parameters;

            effectViewParameter = EffectParameters["View"];
            effectProjectionParameter = EffectParameters["Projection"];
            EffectViewportScaleParameter = EffectParameters["ViewportScale"];
            EffectTimeParameter = EffectParameters["CurrentTime"];
            EffectParameters["AnimationSpeed"].SetValue(1f);
            EffectParameters["Size"].SetValue(new Vector2((Sprite.Width / NumberOfImages) * 2f, Sprite.Height * 2f));
            EffectParameters["t0"].SetValue(Sprite);
            EffectParameters["NumberOfImages"].SetValue(NumberOfImages);

            float aspectRatio = g.Viewport.Width / (float)g.Viewport.Height;

            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    aspectRatio,
                                                                    1, 10000);
            effectProjectionParameter.SetValue(Projection);

            ArrayVertex = new Attack3DVertex[4];

            ArrayVertex[0].UV = new Vector2(0, 0);
            ArrayVertex[1].UV = new Vector2(1, 0);
            ArrayVertex[2].UV = new Vector2(1, 1);
            ArrayVertex[3].UV = new Vector2(0, 1);

            for (int i = 0; i < 4; ++i)
            {
                ArrayVertex[i].Time = currentTime;
            }

            AttackVertexBuffer = new DynamicVertexBuffer(g, Attack3DVertex.VertexDeclaration,
                                                   4, BufferUsage.WriteOnly);

            ushort[] indices = new ushort[6];

            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;

            indices[3] = 0;
            indices[4] = 2;
            indices[5] = 3;

            AttackIndexBuffer = new IndexBuffer(g, typeof(ushort), indices.Length, BufferUsage.WriteOnly);

            AttackIndexBuffer.SetData(indices);
        }

        public void SetViewMatrix(Matrix View)
        {
            effectViewParameter.SetValue(View);
        }

        public void SetPosition(float X, float Y, float Z)
        {
            Vector3 Position = new Vector3(X, Y, Z);

            for (int i = 0; i < 4; ++i)
            {
                ArrayVertex[i].Position = Position;
            }
            AttackVertexBuffer.SetData(ArrayVertex);
        }

        public void Draw(GraphicsDevice GraphicsDevice, float currentTime = 0f)
        {
            // Restore the vertex buffer contents if the graphics device was lost.
            if (AttackVertexBuffer.IsContentLost)
            {
                AttackVertexBuffer.SetData(ArrayVertex);
            }

            // Set an effect parameter describing the viewport size. This is
            // needed to convert particle sizes into screen space point sizes.
            EffectViewportScaleParameter.SetValue(new Vector2(0.5f / GraphicsDevice.Viewport.AspectRatio, -0.5f));

            EffectTimeParameter.SetValue(currentTime);

            GraphicsDevice.SetVertexBuffer(AttackVertexBuffer);
            GraphicsDevice.Indices = AttackIndexBuffer;

            foreach (EffectPass pass in AttackEffect3D.CurrentTechnique.Passes)
            {
                pass.Apply();

                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
            }
        }
    }
}
