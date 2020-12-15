using System.Threading;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.Editors.AnimationBackgroundEditor
{
    class AnimationBackground2DViewerControl : GraphicsDeviceControl
    {
        public ContentManager content;
        public CustomSpriteBatch g;
        private Texture2D sprPixel;
        public AnimationBackground2D ActiveAnimationBackground;

        protected override void Initialize()
        {
            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };

            Mouse.WindowHandle = this.Handle;

            content = new ContentManager(Services, "Content");
            g = new CustomSpriteBatch(new SpriteBatch(GraphicsDevice));
        }

        public void Preload()
        {
            OnCreateControl();

            sprPixel = content.Load<Texture2D>("Pixel");
        }

        public AnimationBackground2DImage AddBackground(string Path)
        {
            AnimationBackground2DImage NewBackground = new AnimationBackground2DImage(content, Path);
            ActiveAnimationBackground.ListBackground.Add(NewBackground);

            return NewBackground;
        }

        public AnimationBackground2DImage AddBackgroundObject(string Path)
        {
            AnimationBackground2DImageComplex NewBackground = new AnimationBackground2DImageComplex(content, Path);
            ActiveAnimationBackground.ListBackground.Add(NewBackground);

            return null;
        }

        public void RemoveBackground(int Index)
        {
            ActiveAnimationBackground.ListBackground.RemoveAt(Index);
        }

        /// <summary>
        /// Draws the control.
        /// </summary>
        protected override void Draw()
        {
            Thread.Sleep(33);
            // Clear to the default control background color.
            Color backColor = new Color(BackColor.R, BackColor.G, BackColor.B);

            GraphicsDevice.Clear(backColor);

            ActiveAnimationBackground.Draw(g, Width, Height);

            g.Begin();
            g.Draw(sprPixel, new Rectangle(0, -(int)ActiveAnimationBackground.Position.Y, Width, 1), Color.Red);
            g.Draw(sprPixel, new Rectangle(-(int)ActiveAnimationBackground.Position.X, 0, 1, Height), Color.Red);
            g.End();
        }
    }
}