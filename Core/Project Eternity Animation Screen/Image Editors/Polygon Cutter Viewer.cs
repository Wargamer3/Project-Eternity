using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class PolygonCutterViewerControl : GraphicsDeviceControl
    {
        public List<Polygon> ListPolygon;

        private CustomSpriteBatch g;
        private Texture2D sprPixel;
        private Texture2D sprRedTexture;
        public Texture2D sprSource;
        private BasicEffect PolygonEffect;
        public PolygonTriangle SelectedPolygonTriangle;
        public Vector2 SplittingPoint1;
        public Vector2 SplittingPoint2;
        public bool EditOrigin;
        public List<Polygon> ListSelectedPolygon;

        public ContentManager content;
        private int OldWidth;
        private int OldHeight;

        protected override void Initialize()
        {
            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };

            content = new ContentManager(Services, "Content");

            g = new CustomSpriteBatch(new SpriteBatch(GraphicsDevice));

            ListSelectedPolygon = new List<Polygon>();
            sprPixel = content.Load<Texture2D>("Pixel");
            sprRedTexture = content.Load<Texture2D>("Pixel");
            sprRedTexture.SetData<Color>(new Color[] { Color.FromNonPremultiplied(255, 0, 0, 120) });

            OldWidth = OldHeight = 0;
            SelectedPolygonTriangle = new PolygonTriangle();

            if (PolygonEffect == null)
            {
                PolygonEffect = new BasicEffect(GraphicsDevice);

                PolygonEffect.VertexColorEnabled = true;
                PolygonEffect.TextureEnabled = true;

                PolygonEffect.World = Matrix.Identity;
                PolygonEffect.View = Matrix.Identity;
                PolygonEffect.Texture = sprSource;
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

            if (OldWidth != ClientSize.Width || OldHeight != ClientSize.Height)
            {
                Matrix Projection = Matrix.CreateOrthographicOffCenter(0, ClientSize.Width, ClientSize.Height, 0, 0, 1);
                Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

                PolygonEffect.Projection = HalfPixelOffset * Projection;

                OldWidth = ClientSize.Width;
                OldHeight = ClientSize.Height;
            }

            g.Begin();

            if (EditOrigin && sprSource != null)
            {
                g.Draw(sprSource, new Vector2(0, 0), Color.White);
            }
            else
            {
                PolygonEffect.Texture = sprSource;
                PolygonEffect.CurrentTechnique.Passes[0].Apply();
                GraphicsDevice.RasterizerState = RasterizerState.CullNone;

                foreach (Polygon ActivePolygon in ListPolygon)
                {
                    ActivePolygon.Draw(GraphicsDevice);
                }
            }

            //Draw selected polygons.
            PolygonEffect.Texture = sprRedTexture;
            PolygonEffect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            foreach (Polygon ActivePolygon in ListSelectedPolygon)
            {
                ActivePolygon.Draw(GraphicsDevice);
            }

            DrawPolygons(g);

            if (SplittingPoint1 != SplittingPoint2)
                DrawLine(g, SplittingPoint1, SplittingPoint2, Color.Black);

            g.End();
        }

        public void DrawPolygons(CustomSpriteBatch g)
        {
            if (ListSelectedPolygon.Count <= 0)
            {
                SelectedPolygonTriangle.Draw(g, GraphicsDevice, sprPixel);
                foreach (Polygon ActivePolygon in ListPolygon)
                {
                    if (SelectedPolygonTriangle.ActivePolygon != ActivePolygon)
                    {
                        PolygonTriangle.Draw(g, GraphicsDevice, sprPixel, new PolygonTriangle(PolygonTriangle.SelectionTypes.None, ActivePolygon, 0, 0));
                    }
                }

                SelectedPolygonTriangle.Draw(g, GraphicsDevice, sprPixel);
            }
        }

        public void DrawLine(CustomSpriteBatch spriteBatch, Vector2 StartPos, Vector2 EndPos, Color ActiveColor, int width = 1)
        {
            Vector2 v = StartPos - EndPos;
            //Define a line of the length of each small section of the final line.
            Rectangle LineSize = new Rectangle((int)StartPos.X, (int)StartPos.Y, (int)v.Length() + width, width);
            v.Normalize();
            //Get line angle.
            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));

            if (StartPos.Y > EndPos.Y)
                angle = MathHelper.TwoPi - angle;

            spriteBatch.Draw(sprPixel, LineSize, null, ActiveColor, angle, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}
