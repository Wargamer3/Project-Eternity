using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;
using System;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class CubeBackgroundBig
    {
        private Texture2D sprTitleHighlight;
        private Texture2D sprBarLeft;
        private Texture2D sprBarMiddle;

        private Model Cube;

        private float RotationX;
        private Color ColorBackground = Color.FromNonPremultiplied(243, 243, 243, 255);
        private Color ColorLine = Color.FromNonPremultiplied(233, 233, 233, 255);
        private Color ColorLine2 = Color.FromNonPremultiplied(204, 204, 204, 64);

        public readonly Color TextColorLight = Color.FromNonPremultiplied(243, 243, 243, 255);
        public readonly Color TextColorDark = Color.FromNonPremultiplied(65, 70, 65, 255);

        public CubeBackgroundBig()
        {
            RotationX = 0;
        }

        public void Load(ContentManager Content)
        {
            sprTitleHighlight = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Shop/Title Highlight");
            sprBarLeft = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Shop/Bar Left");
            sprBarMiddle = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Shop/Bar Middle");

            Cube = Content.Load<Model>("Deathmatch/Lobby Menu/Cube thing");
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(CustomSpriteBatch g)
        {
            float Ratio = Constants.Height / 2160f;
            int DrawX = 90;
            int DrawY = 26;
            g.Draw(sprTitleHighlight, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 1f);

            Color ColorBackground = Color.FromNonPremultiplied(243, 243, 243, 255);
            Color ColorLine = Color.FromNonPremultiplied(233, 233, 233, 255);
            Color ColorLine2 = Color.FromNonPremultiplied(204, 204, 204, 64);

            g.GraphicsDevice.Clear(ColorBackground);
            g.DrawLine(GameScreen.sprPixel, new Vector2(0 * Ratio, 262 * Ratio), new Vector2(3560 * Ratio, 1940 * Ratio), ColorLine, 500);
            g.DrawLine(GameScreen.sprPixel, new Vector2(66 * Ratio, 1232 * Ratio), new Vector2(1366 * Ratio, 1894 * Ratio), ColorLine, 200);

            g.DrawLine(GameScreen.sprPixel, new Vector2(2500 * Ratio, -592 * Ratio), new Vector2(3800 * Ratio, -36 * Ratio), ColorLine, 220);

            float aspectRatio = 1f;

            Vector3 position = new Vector3(0, 0, 6);

            Vector3 target = new Vector3(0, 0, 3);

            Vector3 up = Vector3.Up;
            Matrix View = Matrix.CreateLookAt(position, target, up);
            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(0.40f,
                                                                    aspectRatio,
                                                                    1000f, 18000);

            ((BasicEffect)Cube.Meshes[0].Effects[0]).DiffuseColor = new Vector3(248f / 255f);
            Cube.Draw(Matrix.CreateTranslation(0, 0, 0) * Matrix.CreateRotationX(1f) * Matrix.CreateRotationY(-0.9f)
                * Matrix.CreateRotationX(0) * Matrix.CreateRotationY(RotationX)
                * Matrix.CreateScale(0.4f) * Matrix.CreateTranslation(-280, -170, -4800), View, Projection);

            g.DrawLine(GameScreen.sprPixel, new Vector2(-550 * Ratio, 766 * Ratio), new Vector2(2550 * Ratio, -696 * Ratio), ColorLine2, 630);
            g.DrawLine(GameScreen.sprPixel, new Vector2(2200 * Ratio, 2402 * Ratio), new Vector2(2700 * Ratio, 2188 * Ratio), ColorLine2, 630);
            g.End();
            g.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            int LineX = 89;
            int LineY = 1507;
            g.Draw(sprBarLeft, new Rectangle((int)(LineX * Ratio), (int)(LineY * Ratio), (int)Math.Ceiling(sprBarLeft.Width * Ratio), (int)Math.Ceiling(sprBarLeft.Height * Ratio)), Color.White);
            g.Draw(sprBarMiddle, new Rectangle((int)(LineX * Ratio + sprBarLeft.Width * Ratio), (int)(LineY * Ratio), (int)Math.Ceiling(2518 * Ratio), (int)Math.Ceiling(sprBarMiddle.Height * Ratio)), Color.White);

            LineX = 110;
            LineY = 222;
            g.Draw(sprBarLeft, new Rectangle((int)(LineX * Ratio), (int)(LineY * Ratio), (int)Math.Ceiling(sprBarLeft.Width * Ratio), (int)Math.Ceiling(sprBarLeft.Height * Ratio)), Color.White);
            g.Draw(sprBarMiddle, new Rectangle((int)(LineX * Ratio + sprBarLeft.Width * Ratio), (int)(LineY * Ratio), (int)Math.Ceiling(3592 * Ratio), (int)Math.Ceiling(sprBarMiddle.Height * Ratio)), Color.White);

            RotationX += 0.005f;
        }
    }
}
