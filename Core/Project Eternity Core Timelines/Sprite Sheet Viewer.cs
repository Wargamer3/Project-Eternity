using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class SpriteSheetViewerControl : GraphicsDeviceControl
    {
        public Texture2D SpriteSheet;
        public SpriteBatch g;
        private Texture2D sprPixel;

        public ContentManager content;
        public System.Collections.Generic.Dictionary<Tuple<int, int>, SpriteSheetTimeline> DicActiveSpriteSheetBitmap;//<<PosX, PosY>, Bitmap>.
        public System.Collections.Generic.HashSet<Shape> HashShape;

        public System.Collections.Generic.Dictionary<string, Texture2D> DicSpriteSheet;

        protected override void Initialize()
        {
            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };

            content = new ContentManager(Services, "Content");

            g = new SpriteBatch(GraphicsDevice);

            sprPixel = content.Load<Texture2D>("Pixel");
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

            g.Begin();

            if (SpriteSheet != null)
                g.Draw(SpriteSheet, Vector2.Zero, Color.White);

            if (HashShape != null)
            {
                foreach (Shape CurrentShape in HashShape)
                {
                    for (int X = CurrentShape.X2 - 1; X >= CurrentShape.X1; --X)
                    {
                        g.Draw(sprPixel, new Vector2(X, CurrentShape.Y1), Color.Black);
                        g.Draw(sprPixel, new Vector2(X, CurrentShape.Y2), Color.Black);
                    }
                    for (int Y = CurrentShape.Y2 - 1; Y >= CurrentShape.Y1; --Y)
                    {
                        g.Draw(sprPixel, new Vector2(CurrentShape.X1, Y), Color.Black);
                        g.Draw(sprPixel, new Vector2(CurrentShape.X2, Y), Color.Black);
                    }
                }
            }

            foreach (System.Collections.Generic.KeyValuePair<Tuple<int, int>, SpriteSheetTimeline> ActiveBitmap in DicActiveSpriteSheetBitmap)
            {
                for (int X = ActiveBitmap.Value.SourceRectangle.Right - 1; X >= ActiveBitmap.Value.SourceRectangle.Left; --X)
                {
                    g.Draw(sprPixel, new Vector2(X, ActiveBitmap.Value.SourceRectangle.Top), Color.Red);
                    g.Draw(sprPixel, new Vector2(X, ActiveBitmap.Value.SourceRectangle.Bottom), Color.Red);
                }
                for (int Y = ActiveBitmap.Value.SourceRectangle.Bottom - 1; Y >= ActiveBitmap.Value.SourceRectangle.Top; --Y)
                {
                    g.Draw(sprPixel, new Vector2(ActiveBitmap.Value.SourceRectangle.Left, Y), Color.Red);
                    g.Draw(sprPixel, new Vector2(ActiveBitmap.Value.SourceRectangle.Right, Y), Color.Red);
                }
            }

            g.End();
        }
    }
}
