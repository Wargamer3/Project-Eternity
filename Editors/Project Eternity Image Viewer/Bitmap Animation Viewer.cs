using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.Editors.BitmapAnimationEditor
{
    internal class BitmapAnimationViewerControl : GraphicsDeviceControl
    {
        public ContentManager content;
        private CustomSpriteBatch g;
        private Texture2D sprPixel;
        public string AnimationPath;
        private AnimatedSprite Sprite;

        protected override void Initialize()
        {
            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };

            Mouse.WindowHandle = this.Handle;

            content = new ContentManager(Services, "Content");

            sprPixel = content.Load<Texture2D>("Pixel");

            g = new CustomSpriteBatch(new SpriteBatch(GraphicsDevice));
            Sprite = new AnimatedSprite(content, AnimationPath.Substring(8, AnimationPath.Length - 12), Vector2.Zero);
            Size = new System.Drawing.Size(Sprite.SpriteWidth, Sprite.SpriteHeight);
        }

        public void Preload()
        {
            OnCreateControl();
        }

        /// <summary>
        /// Draws the control.
        /// </summary>
        protected override void Draw()
        {
            // Clear to the default control background color.
            Color backColor = new Color(BackColor.R, BackColor.G, BackColor.B);

            GraphicsDevice.Clear(backColor);

            g.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            Sprite.Update(new GameTime(new System.TimeSpan(), new System.TimeSpan(160)));
            if (Sprite.AnimationEnded)
                Sprite.RestartAnimation();

            Sprite.Draw(g);

            g.End();
        }
    }
}
