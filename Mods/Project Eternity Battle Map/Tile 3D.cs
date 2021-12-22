using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class Tile3D
    {
        public int TilesetIndex;
        public short[] ArrayIndex;
        public VertexPositionColorTexture[] ArrayVertex;

        public int VertexCount { get { return ArrayVertex.Length; } }

        public int TriangleCount;

        public Tile3D(int TilesetIndex, VertexPositionColorTexture[] ArrayVertex, short[] ArrayIndex)
        {
            this.TilesetIndex = TilesetIndex;
            this.ArrayVertex = ArrayVertex;
            this.ArrayIndex = ArrayIndex;
            TriangleCount = ArrayIndex.Length / 3;
        }

        public Tile3D(Tile3D Clone)
        {
            this.TilesetIndex = Clone.TilesetIndex;
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
