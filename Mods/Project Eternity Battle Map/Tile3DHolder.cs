using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class Tile3DHolder
    {
        private List<Tile3D> ListTile3D;

        private DynamicVertexBuffer VertexBuffer;
        private IndexBuffer IndexBuffer;
        private VertexPositionNormalTexture[] ArrayVertex;
        public Effect Effect3D;
        private int PrimitivesCount => ListTile3D.Count * 2;
        private int VerticesCount => ListTile3D.Count * 4;

        public Tile3DHolder(Effect UnitEffect3D, Texture2D Sprite)
        {
            ListTile3D = new List<Tile3D>();

            Effect3D = UnitEffect3D.Clone();
            Effect3D.Parameters["t0"].SetValue(Sprite);
        }

        public void AddTile(Tile3D NewTile)
        {
            ListTile3D.Add(NewTile);
        }

        public void SetViewMatrix(Matrix ViewProjection, Vector3 CameraPosition3D)
        {
            Effect3D.Parameters["WorldViewProj"].SetValue(ViewProjection);
            Effect3D.Parameters["CameraPosition"].SetValue(CameraPosition3D);
        }

        public void SetWorld(Matrix NewWorld)
        {
            Matrix worldInverse = Matrix.Invert(NewWorld);

            Effect3D.Parameters["World"].SetValue(Matrix.Transpose(NewWorld));

            Effect3D.Parameters["WorldInverseTranspose"].SetValue(worldInverse);
        }

        public void Finish(GraphicsDevice g)
        {
            List<VertexPositionNormalTexture> ListTempVertex = new List<VertexPositionNormalTexture>();
            List<ushort> ListTempIndex = new List<ushort>();
            for (int T = 0; T < ListTile3D.Count; T++)
            {
                ListTempVertex.AddRange(ListTile3D[T].ArrayVertex);

                for (int I = 0; I < ListTile3D[T].ArrayIndex.Length; I++)
                {
                    ListTempIndex.Add((ushort)(T * 4 + ListTile3D[T].ArrayIndex[I]));
                }
            }

            ArrayVertex = ListTempVertex.ToArray();

            VertexBuffer = new DynamicVertexBuffer(g, VertexPositionNormalTexture.VertexDeclaration,
                                                   VerticesCount, BufferUsage.WriteOnly);
            // Create and populate the index buffer.
            ushort[] indices = ListTempIndex.ToArray();

            IndexBuffer = new IndexBuffer(g, typeof(ushort), indices.Length, BufferUsage.WriteOnly);

            IndexBuffer.SetData(indices);
            VertexBuffer.SetData(ArrayVertex);
        }

        public void Draw(GraphicsDevice g)
        {
            // Restore the vertex buffer contents if the graphics device was lost.
            if (VertexBuffer.IsContentLost)
            {
                VertexBuffer.SetData(ArrayVertex);
            }

            g.SetVertexBuffer(VertexBuffer);
            g.Indices = IndexBuffer;

            // Activate the particle effect.
            foreach (EffectPass pass in Effect3D.CurrentTechnique.Passes)
            {
                pass.Apply();

                g.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, VerticesCount, 0, PrimitivesCount);
            }
        }
    }
}
