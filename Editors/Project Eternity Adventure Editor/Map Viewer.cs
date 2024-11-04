using System.Threading;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.AdventureScreen;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.Editors.AdventureEditor
{
    internal class MapViewerControl : GraphicsDeviceControl
    {
        public ContentManager content;
        private CustomSpriteBatch g;
        private Texture2D sprPixel;
        public AdventureMap ActiveFightingZone;

        private Texture2D sprRedTexture;
        private BasicEffect PolygonEffect;
        private int OldWidth;
        private int OldHeight;

        public PolygonTriangle SelectedPolygonTriangle;
        public SimpleAnimation SelectedAnimation;
        public Prop SelectedProp;
        public bool DisplayOtherLayers;
        private Vector3 CameraOldPosition = Vector3.Zero;

        protected override void Initialize()
        {
            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };

            Mouse.WindowHandle = this.Handle;

            content = new ContentManager(Services, "Content");

            sprPixel = content.Load<Texture2D>("Pixel");
            OldWidth = OldHeight = 0;
            SelectedPolygonTriangle = new PolygonTriangle();
            DisplayOtherLayers = true;

            sprRedTexture = content.Load<Texture2D>("Pixel");
            sprRedTexture.SetData<Color>(new Color[] { Color.FromNonPremultiplied(255, 0, 0, 120) });
            if (PolygonEffect == null)
            {
                PolygonEffect = new BasicEffect(GraphicsDevice);

                PolygonEffect.VertexColorEnabled = true;
                PolygonEffect.TextureEnabled = true;

                PolygonEffect.World = Matrix.Identity;
                PolygonEffect.View = Matrix.Identity;
                PolygonEffect.Texture = sprRedTexture;
            }

            g = new CustomSpriteBatch(new SpriteBatch(GraphicsDevice));
        }

        public void Preload()
        {
            OnCreateControl();
        }

        public void Reset()
        {

        }

        private void Update(GameTime gameTime)
        {
            Matrix Projection = Matrix.CreateOrthographicOffCenter(
                ActiveFightingZone.Camera.X,
                ActiveFightingZone.Camera.X + ClientSize.Width,
                ActiveFightingZone.Camera.Y + ClientSize.Height,
                ActiveFightingZone.Camera.Y,
                0,
                1);

            Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            PolygonEffect.Projection = HalfPixelOffset * Projection;

            OldWidth = ClientSize.Width;
            OldHeight = ClientSize.Height;

            CameraOldPosition = new Vector3(ActiveFightingZone.Camera.X, ActiveFightingZone.Camera.Y, 0f);
        }

        /// <summary>
        /// Draws the control.
        /// </summary>
        protected override void Draw()
        {
            Update(new GameTime());

            // Clear to the default control background color.
            Color backColor = new Color(BackColor.R, BackColor.G, BackColor.B);

            GraphicsDevice.Clear(backColor);

            Matrix TransformationMatrix = Matrix.CreateTranslation(-ActiveFightingZone.Camera.X, -ActiveFightingZone.Camera.Y, 0);


            g.Begin(SpriteSortMode.Deferred, null, null, null, null, null, TransformationMatrix);

            foreach (Prop ActivePolygon in ActiveFightingZone.ListProp)
            {
                g.Draw(sprPixel, new Rectangle((int)ActivePolygon.Position.X + 13, (int)ActivePolygon.Position.Y, 6, 20), Color.Black);
                g.Draw(sprPixel, new Rectangle((int)ActivePolygon.Position.X + 13, (int)ActivePolygon.Position.Y + 24, 6, 6), Color.Black);
            }

            foreach (Polygon ActivePolygon in ActiveFightingZone.ListWorldCollisionPolygon)
            {
                PolygonTriangle.Draw(g, GraphicsDevice, sprPixel, new PolygonTriangle(PolygonTriangle.SelectionTypes.None, ActivePolygon, 0, 0));
            }

            if (SelectedAnimation != null)
            {
                Vector2 Position1 = SelectedAnimation.Position - SelectedAnimation.Origin;
                Vector2 Position2 = Position1 + new Vector2(SelectedAnimation.PositionRectangle.Width, 0);
                Vector2 Position3 = Position1 + new Vector2(0, SelectedAnimation.PositionRectangle.Height);
                Vector2 Position4 = Position1 + new Vector2(SelectedAnimation.PositionRectangle.Width, SelectedAnimation.PositionRectangle.Height);

                g.DrawLine(sprPixel, Position1, Position2, Color.Red);
                g.DrawLine(sprPixel, Position1, Position3, Color.Red);
                g.DrawLine(sprPixel, Position2, Position4, Color.Red);
                g.DrawLine(sprPixel, Position3, Position4, Color.Red);
            }

            if (SelectedProp != null)
            {
                Vector2 Position1 = SelectedProp.Position;
                Vector2 Position2 = Position1 + new Vector2(32, 0);
                Vector2 Position3 = Position1 + new Vector2(0, 32);
                Vector2 Position4 = Position1 + new Vector2(32, 32);

                g.DrawLine(sprPixel, Position1, Position2, Color.Red);
                g.DrawLine(sprPixel, Position1, Position3, Color.Red);
                g.DrawLine(sprPixel, Position2, Position4, Color.Red);
                g.DrawLine(sprPixel, Position3, Position4, Color.Red);
            }

            //Draw selected polygons.
            PolygonEffect.Texture = sprRedTexture;
            PolygonEffect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            if (SelectedPolygonTriangle != null)
            {
                SelectedPolygonTriangle.Draw(g, GraphicsDevice, sprPixel);
            }

            g.DrawLine(sprPixel, new Vector2(ActiveFightingZone.CameraBounds.X, ActiveFightingZone.CameraBounds.Y), new Vector2(ActiveFightingZone.CameraBounds.Right, ActiveFightingZone.CameraBounds.Y), Color.Red);
            g.DrawLine(sprPixel, new Vector2(ActiveFightingZone.CameraBounds.Right, ActiveFightingZone.CameraBounds.Y), new Vector2(ActiveFightingZone.CameraBounds.Right, ActiveFightingZone.CameraBounds.Bottom), Color.Red);
            g.DrawLine(sprPixel, new Vector2(ActiveFightingZone.CameraBounds.X, ActiveFightingZone.CameraBounds.Y), new Vector2(ActiveFightingZone.CameraBounds.X, ActiveFightingZone.CameraBounds.Bottom), Color.Red);
            g.DrawLine(sprPixel, new Vector2(ActiveFightingZone.CameraBounds.X, ActiveFightingZone.CameraBounds.Bottom), new Vector2(ActiveFightingZone.CameraBounds.Right, ActiveFightingZone.CameraBounds.Bottom), Color.Red);

            Thread.Sleep(15);

            g.End();
        }
    }
}
