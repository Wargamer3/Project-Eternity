using System.Threading;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.Core.Editor
{
    public class Texture2DViewerControl : GraphicsDeviceControl
    {
        public delegate void DrawOverlayDelegate(CustomSpriteBatch g);

        public ContentManager content;
        private CustomSpriteBatch g;
        private Texture2D sprPixel;
        private Texture2D Sprite;
        public DrawOverlayDelegate DrawOverlay;

        protected override void Initialize()
        {
            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };

            Mouse.WindowHandle = this.Handle;

            content = new ContentManager(Services, "Content");

            sprPixel = content.Load<Texture2D>("Pixel");

            g = new CustomSpriteBatch(new SpriteBatch(GraphicsDevice));
        }

        public void Preload()
        {
            OnCreateControl();
        }

        public void ChangeTexture(string SpritePath)
        {
            if (SpritePath != null)
            {
                Sprite = content.Load<Texture2D>(SpritePath);
                Size = new System.Drawing.Size(Sprite.Width, Sprite.Height);
            }
            else
            {
                Sprite = null;
            }
        }

        public void ChangeTexture(Texture2D Sprite)
        {
            this.Sprite = Sprite;
            if (Sprite != null)
            {
                Size = new System.Drawing.Size(Sprite.Width, Sprite.Height);
            }
        }

        /// <summary>
        /// Draws the control.
        /// </summary>
        protected override void Draw()
        {
            if (Sprite != null)
            {
                // Clear to the default control background color.
                Color backColor = new Color(BackColor.R, BackColor.G, BackColor.B);

                GraphicsDevice.Clear(backColor);

                g.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

                g.Draw(Sprite, Vector2.Zero, Color.White);

                g.End();

                if (DrawOverlay != null)
                {
                    DrawOverlay(g);
                }

                Thread.Sleep(15);
            }
        }
    }
}
