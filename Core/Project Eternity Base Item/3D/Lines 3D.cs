using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core.Item
{
    public class Lines3D
    {
        public VertexPositionColor[] ArrayVertex;
        private short[] ArrayIndices;
        public BasicEffect LinesEffect;

        public Lines3D(GraphicsDevice g, Matrix Projection)
        {
            LinesEffect = new BasicEffect(g);
            LinesEffect.World = Matrix.Identity;
            LinesEffect.DiffuseColor = new Vector3(0, 0, 0);
            LinesEffect.Projection = Projection;
            int GridWidth = 50;
            int GridDepth = 50;
            ArrayVertex = new VertexPositionColor[GridWidth * 8];
            // Initialize an array of indices of type short.
            ArrayIndices = new short[(ArrayVertex.Length * 2) - 2];

            int LineLength = 100;
            int Index = 0;
            for (int X = -GridWidth; X < GridWidth; ++X)
            {
                ArrayVertex[Index * 2] = new VertexPositionColor(
                    new Vector3(X * LineLength, 0, -LineLength * GridDepth - LineLength / 2), Color.White);
                ArrayVertex[Index * 2 + 1] = new VertexPositionColor(
                    new Vector3(X * LineLength, 0, LineLength * GridDepth - LineLength / 2), Color.White);

                ArrayIndices[Index * 2] = (short)(Index * 2);
                ArrayIndices[Index * 2 + 1] = (short)(Index * 2 + 1);

                ++Index;
            }
            for (int Z = -GridWidth; Z < GridWidth; ++Z)
            {
                ArrayVertex[Index * 2] = new VertexPositionColor(
                    new Vector3(-LineLength * GridWidth - LineLength / 2, 0, Z * LineLength), Color.White);
                ArrayVertex[Index * 2 + 1] = new VertexPositionColor(
                    new Vector3(LineLength * GridWidth - LineLength / 2, 0, Z * LineLength), Color.White);

                ArrayIndices[Index * 2] = (short)(Index * 2);
                ArrayIndices[Index * 2 + 1] = (short)(Index * 2 + 1);

                ++Index;
            }
        }

        public Lines3D(GraphicsDevice g, Matrix Projection, VertexPositionColor[] ArrayVertex, short[] ArrayIndices)
        {
            LinesEffect = new BasicEffect(g);
            LinesEffect.World = Matrix.Identity;
            LinesEffect.DiffuseColor = new Vector3(0, 0, 0);
            LinesEffect.Projection = Projection;

            this.ArrayVertex = ArrayVertex;
            this.ArrayIndices = ArrayIndices;
        }

        public void Draw(CustomSpriteBatch g, Matrix View)
        {
            LinesEffect.View = View;
            // Activate the particle effect.
            foreach (EffectPass pass in LinesEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                g.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.LineList,
                    ArrayVertex,
                    0,  // vertex buffer offset to add to each element of the index buffer
                    ArrayVertex.Length,  // number of vertices in pointList
                    ArrayIndices,  // the index buffer
                    0,  // first index element to read
                    ArrayIndices.Length / 2 - 1   // number of primitives to draw
                );
            }
        }
    }
}
