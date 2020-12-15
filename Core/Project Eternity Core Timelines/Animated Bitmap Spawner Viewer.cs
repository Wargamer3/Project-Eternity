using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class SpawnerViewerControl : GraphicsDeviceControl
    {
        public Texture2D Bitmap;
        public string BitmapName;
        private CustomSpriteBatch g;
        private Texture2D sprPixel;

        public ContentManager content;

        protected override void Initialize()
        {
            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };

            content = new ContentManager(Services, "Content");

            g = new CustomSpriteBatch(new SpriteBatch(GraphicsDevice));

            sprPixel = content.Load<Texture2D>("Pixel");
        }

        /// <summary>
        /// Draws the control.
        /// </summary>
        protected override void Draw()
        {
            // Clear to the default control background color.
            Color backColor = new Color(BackColor.R, BackColor.G, BackColor.B);

            GraphicsDevice.Clear(backColor);

            g.Begin();

            if (Bitmap != null)
            {
                g.Draw(Bitmap, Vector2.Zero, Color.White);
            }

            g.End();
        }
    }
}
