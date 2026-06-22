using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class Tile3D
    {
        public int TilesetIndex;
        public short[] ArrayIndex;
        public VertexPositionNormalTexture[] ArrayVertex;
        public BoundingBox CollisionBox;

        public int VertexCount { get { return ArrayVertex.Length; } }

        public int TriangleCount;

        public Tile3D(int TilesetIndex, VertexPositionNormalTexture[] ArrayVertex, short[] ArrayIndex)
        {
            this.TilesetIndex = TilesetIndex;
            this.ArrayVertex = ArrayVertex;
            this.ArrayIndex = ArrayIndex;
            TriangleCount = ArrayIndex.Length / 3;

            CollisionBox = new BoundingBox(ArrayVertex[0].Position, ArrayVertex[3].Position - new Vector3(0, 32, 0));
        }

        public Tile3D(Tile3D Clone)
        {
            this.TilesetIndex = Clone.TilesetIndex;
            this.ArrayVertex = (VertexPositionNormalTexture[])Clone.ArrayVertex.Clone();
            this.ArrayIndex = (short[])Clone.ArrayIndex.Clone();
            TriangleCount = Clone.TriangleCount;
        }

        public void Draw(GraphicsDevice g)
        {
            g.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, ArrayVertex, 0, VertexCount, ArrayIndex, 0, TriangleCount);
        }
    }
}
