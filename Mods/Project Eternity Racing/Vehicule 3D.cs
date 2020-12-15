using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.RacingScreen
{
    public class Vehicule3D : Vehicule
    {
        VertexPositionNormalTexture[] vertexes;
        private BasicEffect effect;

        private Vehicule3D(Vector3 Position)
            : base(null, null)
        {
            TeleportRelative(Position);
        }

        public Vehicule3D(List<AITunnel> ListAITunnel, List<Object3D> ListCollisionBox, GraphicsDevice g, Vector3 Position)
            : base(g, Matrix.Identity, ListAITunnel, ListCollisionBox)
        {
            TeleportRelative(Position);

            effect = new BasicEffect(g);
            effect.AmbientLightColor = new Vector3(1, 0, 0);
            effect.DirectionalLight0.Enabled = true;
            effect.DirectionalLight0.DiffuseColor = Vector3.One;
            effect.DirectionalLight0.Direction = Vector3.Normalize(Vector3.One);
            effect.LightingEnabled = true;

            float aspectRatio = g.Viewport.Width / (float)g.Viewport.Height;
            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                     aspectRatio,
                                                                     1, 10000);
            effect.Projection = Projection;

            vertexes = RacingMap.CreateCube();
            ArrayAntiGravPropulsor = new AntiGravPropulsor[1];
            ArrayAntiGravPropulsor[0] = CreateAntiGravPropulsor(g, Projection, 1f, Position);
            ArrayAntiGravPropulsor[0].Position += new Vector3(0f, 2f, 0f);
        }

        public static Vehicule3D GetUnitTestVehicule(Vector3 Position)
        {
            return new Vehicule3D(Position);
        }

        public override void Load(ContentManager Content, GraphicsDevice g)
        {
        }

        protected override void DoUpdate(GameTime gameTime)
        {
        }

        public override void Draw(CustomSpriteBatch g, Matrix View)
        {
            effect.View = View;

            effect.World = Rotation
                * Matrix.CreateTranslation(Position);

            effect.CurrentTechnique.Passes[0].Apply();
            g.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertexes, 0, 12);

            foreach (Object3D AntiGravPropulsor in ArrayAntiGravPropulsor)
            {
                AntiGravPropulsor.Draw(g, View);
            }
        }
    }
}
