using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.RacingScreen
{
    public class CollisionBox : Object3D
    {
        private VertexPositionNormalTexture[] ArrayVertexBound;

        public CollisionBox(GraphicsDevice g, Matrix Projection)
            : base(g, Projection)
        {
            Init(g, Projection);
        }

        public CollisionBox(BinaryReader BR, GraphicsDevice g, Matrix Projection)
            : base(BR, g, Projection)
        {
            Init(g, Projection);
        }

        private void Init(GraphicsDevice g, Matrix Projection)
        {
            ObjectEffect.AmbientLightColor = new Vector3(0.2f, 0.5f, 0.2f);
            ObjectEffect.DirectionalLight0.Enabled = true;
            ObjectEffect.DirectionalLight0.DiffuseColor = Vector3.One;
            ObjectEffect.DirectionalLight0.Direction = Vector3.Normalize(Vector3.One);
            ObjectEffect.LightingEnabled = true;

            ArrayVertexBound = RacingMap.CreateCube();
        }
        
        public static CollisionBox Load(BinaryReader BR, GraphicsDevice g, Matrix Projection)
        {
            CollisionBox NewCollisionBox = new CollisionBox(BR, g, Projection);

            return NewCollisionBox;
        }
        
        public override void Draw(CustomSpriteBatch g, Matrix View)
        {
            ObjectEffect.View = View;
            
            ObjectEffect.CurrentTechnique.Passes[0].Apply();
            g.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, ArrayVertexBound, 0, 12);
        }
    }
}
