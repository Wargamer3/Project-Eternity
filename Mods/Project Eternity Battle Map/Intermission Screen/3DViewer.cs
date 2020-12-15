using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    class ModelViewer : GameScreen
    {
        Model MyModel;
        float aspectRatio;
        // Set the position of the model in world space, and set the rotation.
        Vector3 modelPosition = Vector3.Zero;
        float modelRotation = 0.0f;

        // Set the position of the camera in world space, for our view matrix.
        Vector3 cameraPosition = new Vector3(0.0f, 50.0f, 5000.0f);
        public ModelViewer()
            : base()
        {

        }
        public override void Load()
        {
            MyModel = Content.Load<Model>("3D/Models/p1_wedge");
        }
        public override void Update(GameTime gameTime)
        {
            modelRotation += (float)gameTime.ElapsedGameTime.TotalMilliseconds * MathHelper.ToRadians(0.1f);
            if (KeyboardHelper.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
                RemoveScreen(this);
        }
        public override void Draw(CustomSpriteBatch g)
        {
            g.End();
            aspectRatio = GraphicsDevice.Viewport.AspectRatio;
            GraphicsDevice.Clear(Color.CornflowerBlue);
            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[MyModel.Bones.Count];
            MyModel.CopyAbsoluteBoneTransformsTo(transforms);
            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in MyModel.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(modelRotation) * Matrix.CreateTranslation(modelPosition);
                    effect.View = Matrix.CreateLookAt(cameraPosition,
                        Vector3.Zero, Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(45.0f), aspectRatio,
                        1.0f, 10000.0f);
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
            g.Begin();
        }
    }
}
