using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core.LightningSystem
{
    public struct LightningVertex
    {
        public Vector3 Position;
        public Vector2 TextureCoordinates;
        public Vector2 ColorGradient;

        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3,
                                    VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector2,
                                     VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(20, VertexElementFormat.Vector2,
                                     VertexElementUsage.TextureCoordinate, 1)
         );

        public LightningVertex(Vector2 TextureCoordinates)
        {
            this.TextureCoordinates = TextureCoordinates;
            Position = Vector3.Zero;
            ColorGradient = Vector2.Zero;
        }

            // Describe the size of this vertex structure.
            public const int SizeInBytes = 28;
    }
}
