using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core.ParticleSystem
{
    struct ParticleVertex3D
    {
        public Vector3 Position;
        public Vector3 VectorOffset;
        public float Time;
        public Vector3 Speed;
        public Vector3 MinScale;

        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(sizeof(float) * 3 + sizeof(float) * 3, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(sizeof(float) * 3 + sizeof(float) * 3 + sizeof(float), VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 2),
            new VertexElement(sizeof(float) * 3 + sizeof(float) * 3 + sizeof(float) + sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 3)
         );


        public const int SizeInBytes = sizeof(float) * 3 + sizeof(float) * 3 + sizeof(float) + sizeof(float) * 3 + sizeof(float) * 3;
    }
}