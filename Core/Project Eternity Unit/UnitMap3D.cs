using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core.Units
{
    public class UnitMap3D
    {
        public struct UnitMap3DVertex
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

        public UnitMap3DVertex[] particles;
        private DynamicVertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;
        public readonly Effect UnitEffect3D;
        private EffectParameterCollection parameters;
        // Shortcuts for accessing frequently changed effect parameters.
        private EffectParameter effectViewParameter;

        private EffectParameter effectProjectionParameter;
        private EffectParameter effectViewportScaleParameter;
        private EffectParameter effectTimeParameter;

        public UnitMap3D(GraphicsDevice g, Effect UnitEffect3D, Texture2D Sprite, int NumberOfImages, float currentTime = 0f)
        {
            this.UnitEffect3D = UnitEffect3D.Clone();

            parameters = this.UnitEffect3D.Parameters;

            // Look up shortcuts for parameters that change every frame.
            effectViewParameter = parameters["View"];
            effectProjectionParameter = parameters["Projection"];
            effectViewportScaleParameter = parameters["ViewportScale"];
            effectTimeParameter = parameters["CurrentTime"];
            parameters["AnimationSpeed"].SetValue(1f);
            parameters["Size"].SetValue(new Vector2((Sprite.Width / NumberOfImages) * 2f, Sprite.Height * 2f));
            parameters["t0"].SetValue(Sprite);

            // Set the values of parameters that do not change.
            parameters["NumberOfImages"].SetValue(NumberOfImages);

            float aspectRatio = g.Viewport.Width / (float)g.Viewport.Height;

            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    aspectRatio,
                                                                    1, 10000);
            effectProjectionParameter.SetValue(Projection);

            particles = new UnitMap3DVertex[4];

            particles[0].UV = new Vector2(0, 0);
            particles[1].UV = new Vector2(1, 0);
            particles[2].UV = new Vector2(1, 1);
            particles[3].UV = new Vector2(0, 1);

            for (int i = 0; i < 4; ++i)
            {
                particles[i].Time = currentTime;
            }

            // Create a dynamic vertex buffer.
            vertexBuffer = new DynamicVertexBuffer(g, UnitMap3DVertex.VertexDeclaration,
                                                   4, BufferUsage.WriteOnly);

            // Create and populate the index buffer.
            ushort[] indices = new ushort[6];

            indices[0] = (ushort)(0);
            indices[1] = (ushort)(1);
            indices[2] = (ushort)(2);

            indices[3] = (ushort)(0);
            indices[4] = (ushort)(2);
            indices[5] = (ushort)(3);

            indexBuffer = new IndexBuffer(g, typeof(ushort), indices.Length, BufferUsage.WriteOnly);

            indexBuffer.SetData(indices);
        }

        public void SetViewMatrix(Matrix View)
        {
            effectViewParameter.SetValue(View);
        }

        public void SetScale(Vector2 Scale)
        {
            parameters["Size"].SetValue(Scale);
        }

        public void SetPosition(float X, float Y, float Z)
        {
            Vector3 Position = new Vector3(X, Y, Z);

            for (int i = 0; i < 4; ++i)
            {
                particles[i].Position = Position;
            }
            vertexBuffer.SetData(particles);
        }

        public void Draw(GraphicsDevice GraphicsDevice, float currentTime = 0f)
        {
            // Restore the vertex buffer contents if the graphics device was lost.
            if (vertexBuffer.IsContentLost)
            {
                vertexBuffer.SetData(particles);
            }

            // Set an effect parameter describing the viewport size. This is
            // needed to convert particle sizes into screen space point sizes.
            effectViewportScaleParameter.SetValue(new Vector2(0.5f / GraphicsDevice.Viewport.AspectRatio, -0.5f));

            // Set an effect parameter describing the current time. All the vertex
            // shader particle animation is keyed off this value.
            effectTimeParameter.SetValue(currentTime);

            // Set the particle vertex and index buffer.
            GraphicsDevice.SetVertexBuffer(vertexBuffer);
            GraphicsDevice.Indices = indexBuffer;

            // Activate the particle effect.
            foreach (EffectPass pass in UnitEffect3D.CurrentTechnique.Passes)
            {
                pass.Apply();

                // If the active particles are all in one consecutive range,
                // we can draw them all in a single call.
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
            }
        }
    }
}
