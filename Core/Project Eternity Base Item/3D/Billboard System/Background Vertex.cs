using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public struct ParticleVertex
    {
        public Vector3 Position;
        public Vector2 UV;
        public float Time;
        public float RotateTowardCamera;

        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(sizeof(float) * 3 + sizeof(float) * 2, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(sizeof(float) * 3 + sizeof(float) * 2 + sizeof(float), VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 2)
         );

        public const int SizeInBytes = sizeof(float) * 3 + sizeof(float) * 2 + sizeof(float) + sizeof(float);
    }
}
