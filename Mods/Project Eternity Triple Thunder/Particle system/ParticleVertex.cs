using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Particle3DSample
{
    struct ParticleVertex
    {
        public Vector2 Position;
        public Vector2 UV;
        public float Time;
		public Vector2 Speed;
		public Vector2 MinScale;

        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
             new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
             new VertexElement(sizeof(float) * 2, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
             new VertexElement(sizeof(float) * 2 + sizeof(float) * 2, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 1),
				new VertexElement(sizeof(float) * 2 + sizeof(float) * 2 + sizeof(float), VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 2),
				new VertexElement(sizeof(float) * 2 + sizeof(float) * 2 + sizeof(float) + sizeof(float) * 2, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 3)
         );


		public const int SizeInBytes = sizeof(float) * 2 + sizeof(float) * 2 + sizeof(float) + sizeof(float) * 2 + sizeof(float) * 2;
    }
}