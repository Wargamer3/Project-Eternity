using System.Windows.Forms;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Scene;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.Editors.SceneEditor
{
    internal class SceneEventViewerControl : GraphicsDeviceControl
    {
        private SceneEvent Event;

        public ContentManager content;
        private CustomSpriteBatch g;

        internal SceneEventViewerControl(SceneEvent Event)
        {
            this.Event = Event;
        }

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
        }

        /// <summary>
        /// Draws the control.
        /// </summary>
        protected override void Draw()
        {
            // Clear to the default control background color.
            Color backColor = new Color(BackColor.R, BackColor.G, BackColor.B);

            GraphicsDevice.Clear(backColor);

            Thread.Sleep(1);
        }
    }
}
