using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Scene;

namespace ProjectEternity.Editors.SceneEditor
{
    internal class ScenePreviewViewerControl : GraphicsDeviceControl
    {
        private SceneScreen Scene;
        public SceneEvent ActiveEvent;

        public ContentManager content;
        private CustomSpriteBatch g;

        public SceneEditor Owner;

        protected override void Initialize()
        {
            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };

            Mouse.WindowHandle = this.Handle;

            content = new ContentManager(Services, "Content");

            g = new CustomSpriteBatch(new SpriteBatch(GraphicsDevice));
        }

        public void Preload(SceneScreen Scene)
        {
            this.Scene = Scene;
            OnCreateControl();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (ActiveEvent != null)
            {
                ActiveEvent.OnMouseDown(e.X, e.Y, e.Button);
            }
            else
            {
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (ActiveEvent != null)
            {
                ActiveEvent.OnMouseMove(e.X, e.Y, e.Button);
            }
            else
            {
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (ActiveEvent != null)
            {
                ActiveEvent.OnMouseUp(e.X, e.Y, e.Button);
            }
            else
            {
            }
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

            Scene.Draw(g);

            g.End();

            Thread.Sleep(1);
        }
    }
}
