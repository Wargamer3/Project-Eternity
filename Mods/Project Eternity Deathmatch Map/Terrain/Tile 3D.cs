using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class Tile3D
    {
        public short[] ArrayIndex;
        public VertexPositionColorTexture[] ArrayVertex;

        public int VertexCount { get { return ArrayVertex.Length; } }

        public int TriangleCount;

        public Tile3D(VertexPositionColorTexture[] ArrayVertex, short[] ArrayIndex)
        {
            this.ArrayVertex = ArrayVertex;
            this.ArrayIndex = ArrayIndex;
            TriangleCount = ArrayIndex.Length / 3;
        }

        public Tile3D(Tile3D Clone)
        {
            this.ArrayVertex = (VertexPositionColorTexture[])Clone.ArrayVertex.Clone();
            this.ArrayIndex = (short[])Clone.ArrayIndex.Clone();
            TriangleCount = Clone.TriangleCount;
        }

        public void Draw(GraphicsDevice g)
        {
            g.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, ArrayVertex, 0, VertexCount, ArrayIndex, 0, TriangleCount);
        }
    }
}
