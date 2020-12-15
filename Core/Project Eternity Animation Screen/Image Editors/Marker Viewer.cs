using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class MarkerViewerControl : GraphicsDeviceControl
    {
        public MarkerTimeline ActiveMarker;
        private SpriteBatch g;

        public ContentManager content;

        protected override void Initialize()
        {
            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };

            content = new ContentManager(Services, "Content");

            g = new SpriteBatch(GraphicsDevice);
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

            if (ActiveMarker != null && ActiveMarker.Sprite != null)
                g.Draw(ActiveMarker.Sprite, Vector2.Zero, Color.White);

            g.End();
        }
    }
}
